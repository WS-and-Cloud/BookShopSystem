namespace BookShop.Services.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.OData;

    using BookShop.Data;
    using BookShop.Models;
    using BookShop.Services.Models;

    using Microsoft.AspNet.Identity;

    [RoutePrefix("api/books")]
    public class BooksController : BaseApiController
    {
        private const int ThirtyDays = 30;

        public BooksController()
            : base(new BookShopData())
        {
        }

        [HttpGet]
        public IHttpActionResult BookById(int id)
        {
            var book =
                this.Data.Books.All()
                    .Where(b => b.Id == id)
                    .Select(
                        b =>
                        new BookModel
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Description = b.Description,
                            Copies = b.Copies,
                            Edition = b.Edition,
                            Price = b.Price,
                            AuthorLastName = b.Author.FirstName,
                            ReleaseDate = b.ReleaseDate,
                            Categories = b.Categories.Select(c => c.Name)
                        })
                    .FirstOrDefault();
            if (book == null)
            {
                return this.BadRequest("Book with such id does not exists");
            }

            return this.Ok(book);
        }

        [EnableQuery]
        [HttpGet]
        public IHttpActionResult TopTenBooksByKeyWord([FromUri]string search)
        {
            var topTenBooksByWord =
                this.Data.Books.All()
                    .Where(b => b.Title.Contains(search))
                    .OrderBy(b => b.Title)
                    .Select(b => new BookModel { Id = b.Id, Title = b.Title })
                    .Take(10);

            return this.Ok(topTenBooksByWord);
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpPatch]
        public IHttpActionResult EditBookById(int id, [FromBody]BookModel bookModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var book = this.Data.Books.All().FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.BadRequest("Invalid book id");
            }

            this.UpdateBook(book, bookModel);
            this.Data.SaveChanges();

            bookModel.Id = book.Id;

            return this.Ok(bookModel);
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpDelete]
        public IHttpActionResult DeleteBookById(int id)
        {
            var book = this.Data.Books.All().FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.BadRequest("Invalid book id");
            }

            this.Data.Books.Delete(book);
            this.Data.SaveChanges();
            return this.Ok();
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpPost]
        public IHttpActionResult CreateBook([FromBody]BookModel bookModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var book = new Book
                       {
                           Title = bookModel.Title,
                           Description = bookModel.Description,
                           Price = bookModel.Price,
                           Copies = bookModel.Copies,
                           Edition = bookModel.Edition,
                           ReleaseDate = bookModel.ReleaseDate,
                           AuthorId = Convert.ToInt32(bookModel.AuthorId),
                           Categories = this.GetCategories(bookModel.Categories)
                       };
            this.Data.Books.Add(book);
            this.Data.SaveChanges();
            bookModel.Id = book.Id;
            return this.Ok(bookModel);
        }

        [Authorize(Roles = AdministratorRole)]
        [Authorize]
        [HttpPut]
        [Route("buy/{id}")]
        public IHttpActionResult PurchaseBook([FromUri] int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var book = this.Data.Books.All().FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.NotFound();
            }

            if (book.Copies < 1)
            {
                return this.BadRequest("Book does not have any copies left");
            }

            book.Copies -= 1;

            var currentlyLoggedUserId = this.User.Identity.GetUserId();
            var currentUser = this.Data.ApplicationUsers.All().FirstOrDefault(au => au.Id == currentlyLoggedUserId);
            var purchase = new Purchase
                           {
                               ApplicationUserId = new Guid(currentUser.Id),
                               BookId = book.Id,
                               DateOfPurchase = DateTime.Now,
                               IsRecalled = false
                           };
            this.Data.Purchases.Add(purchase);
            this.Data.SaveChanges();
            return this.Ok(purchase.Id);
        }

        [Authorize(Roles = AdministratorRole)]
        [Authorize]
        [HttpPut]
        [Route("recall/{id}")]
        public IHttpActionResult ReturnBook(int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var book = this.Data.Books.All().FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.NotFound();
            }

            book.Copies += 1;

            var purchase = this.Data.Purchases.All().FirstOrDefault(p => p.BookId == book.Id);
            if (purchase == null)
            {
                return this.NotFound();
            }

            var timeDifference  = (DateTime.Now - purchase.DateOfPurchase).Days;
            if (timeDifference > ThirtyDays)
            {
                return this.BadRequest("More than 30 days have past.");
            }

            purchase.IsRecalled = true;
            this.Data.SaveChanges();
            
            return this.Ok(purchase.Id);
        }

        private ICollection<Category> GetCategories(IEnumerable<string> categoriesAsStrings)
        {
            var categories = new List<Category>();
            foreach (var catName in categoriesAsStrings)
            {
                Category category = this.Data.Categories.All().FirstOrDefault(c => c.Name == catName);
                if (category == null)
                {
                    throw new InvalidOperationException("Such category does not exists");
                }

                categories.Add(category);
            }

            return categories;
        }

        private void UpdateBook(Book book, BookModel bookModel)
        {
            book.Title = bookModel.Title;
            book.ReleaseDate = bookModel.ReleaseDate;
            book.Price = bookModel.Price;
            book.Copies = bookModel.Copies;
            book.Description = bookModel.Description;
            if (bookModel.AuthorId != null)
            {
                book.AuthorId = Convert.ToInt32(bookModel.AuthorId);
            }
        }
    }
}

namespace BookShop.Services.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using System.Web.OData;

    using BookShop.Data;
    using BookShop.Models;
    using BookShop.Services.Models;

    [RoutePrefix("api/authors")]
    public class AuthorsController : BaseApiController
    {
        public AuthorsController()
            : base(new BookShopData())
        {
        }

        [HttpGet]
        public IHttpActionResult AuthorsById(int id)
        {
            var author =
                this.Data
                .Authors
                .All()
                .Where(a => a.Id == id)
                .Select(a => new AuthorModel
                        {
                            Id = a.Id,
                            FirstName = a.FirstName,
                            LastName = a.LastName
                        })
                .FirstOrDefault();
            if (author == null)
            {
                return this.BadRequest("Author with such id does not exists!");
            }

            return this.Ok(author);
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpPost]
        public IHttpActionResult CreateAuthor([FromBody] AuthorModel authorModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var author = new Author() { FirstName = authorModel.FirstName, LastName = authorModel.LastName };

            this.Data.Authors.Add(author);
            this.Data.SaveChanges();
            authorModel.Id = author.Id;
            return this.Ok(authorModel);
        }

        [EnableQuery]
        [HttpGet]
        [Route("{id}/books")]
        public IHttpActionResult AuthorBooks(int id)
        {
            var author = this.Data.Authors.All().FirstOrDefault(a => a.Id == id);
            if (author == null)
            {
                return this.BadRequest("Author does not exists");
            }

            var books =
                author.Books.Select(
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
                    });
            return this.Ok(books);
        }
    }
}

namespace BookShop.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Web;
    using BookShop.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<BookShopContext>
    {
        private const string AuthorFileLocation = "../../sample-data/authors.txt";
        private const string CategoriesFileLocation = "../../sample-data/categories.txt";
        private const string BooksFileLocation = "../../sample-data/books.txt";
        
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BookShopContext context)
        {
            if (!context.Authors.Any())
            {
                this.AddAuthors(context);
            }

            if (!context.Categories.Any())
            {
                this.AddCategories(context);
            }

            if (!context.Books.Any())
            {
                this.AddBooks(context);
            }
        }

        private void AddBooks(BookShopContext context)
        {
            var books = this.ReadFile(BooksFileLocation);
            foreach (var row in books.Skip(1))
            {
                var authorId = context.Authors.OrderBy(x => Guid.NewGuid()).Take(1).Select(a => a.Id).FirstOrDefault();
                var categoryId = context.Categories.OrderBy(x => Guid.NewGuid()).Take(1).Select(a => a.Id).FirstOrDefault();
                var book = row.Split(' ');
                var edition = book[0];
                var releaseDate = book[1];
                var copies = book[2];
                var price = book[3];
                var title = book[5];
                var bookEntity = new Book
                                 {
                                     Title = title, 
                                     ReleaseDate = DateTime.Parse(releaseDate), 
                                     Edition = (Edition)Enum.Parse(typeof(Edition), edition), 
                                     Price = decimal.Parse(price), 
                                     Copies = int.Parse(copies), 
                                     AuthorId = authorId
                                 };
                bookEntity.Categories.Add(context.Categories.Find(categoryId));
                context.Books.Add(bookEntity);
            }
        }

        private void AddCategories(BookShopContext context)
        {
            var categories = this.ReadFile(CategoriesFileLocation);
            foreach (var categoryName in categories)
            {
                context.Categories.Add(new Category { Name = categoryName });
            }

            context.SaveChanges();
        }

        private void AddAuthors(BookShopContext context)
        {
            var authors = this.ReadFile(AuthorFileLocation);
            foreach (var author in authors.Skip(1))
            {
                var authorNames = author.Split(' ');
                var firstName = authorNames[0];
                var lastName = authorNames[1];
                context.Authors.Add(new Author
                                    {
                                        FirstName = firstName, 
                                        LastName = lastName
                                    });
            }
        }

        private string[] ReadFile(string filePath)
        {
            var file = File.ReadAllText(HttpContext.Current.Server.MapPath(filePath));
            var objects = file.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return objects;
        }
    }
}

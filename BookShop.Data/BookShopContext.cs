namespace BookShop.Data
{
    using System.Data.Entity;

    using BookShop.Data.Migrations;
    using BookShop.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    public class BookShopContext : IdentityDbContext<ApplicationUser>, IBookShopContext
    {
        public BookShopContext()
            : base("name=BookShopContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BookShopContext, Configuration>());
        }

        public virtual IDbSet<Author> Authors { get; set; }

        public virtual IDbSet<Book> Books { get; set; }

        public virtual IDbSet<Category> Categories { get; set; }

        public override IDbSet<IdentityRole> Roles { get; set; }

        public static BookShopContext Create()
        {
            return new BookShopContext();
        }

        public new void SaveChanges()
        {
            base.SaveChanges();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}
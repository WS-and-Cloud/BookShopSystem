namespace BookShop.Data
{
    using BookShop.Data.Repositories;
    using BookShop.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    public interface IBookShopData
    {
        IRepository<Author> Authors { get; }

        IRepository<Book> Books { get; }

        IRepository<Category> Categories { get; }

        IRepository<Purchase> Purchases { get; }

        IRepository<IdentityRole> Roles { get; }

        IRepository<ApplicationUser> ApplicationUsers { get; }

        void SaveChanges();
    }
}
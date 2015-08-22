namespace BookShop.Data
{
    using System;
    using System.Collections.Generic;

    using BookShop.Data.Repositories;
    using BookShop.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    public class BookShopData : IBookShopData
    {
        private IBookShopContext context;
        private IDictionary<Type, object> repositories;

        public BookShopData()
            : this(new BookShopContext())
        {
        }

        public BookShopData(IBookShopContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<IdentityRole> Roles
        {
            get
            {
                return this.GetRepository<IdentityRole>();
            }
        }

        public IRepository<Purchase> Purchases
        {
            get
            {
                return this.GetRepository<Purchase>();
            }
        }

        public IRepository<ApplicationUser> ApplicationUsers
        {
            get
            {
                return this.GetRepository<ApplicationUser>();
            }
        }

        public IRepository<Author> Authors
        {
            get
            {
                return this.GetRepository<Author>();
            }
        }

        public IRepository<Category> Categories
        {
            get
            {
                return this.GetRepository<Category>();
            }
        }

        public IRepository<Book> Books
        {
            get
            {
                return this.GetRepository<Book>();
            }
        }

        public void SaveChanges()
        {
            this.context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            var typeOfModel = typeof(T);
            if (!this.repositories.ContainsKey(typeOfModel))
            {
                var type = typeof(Repository<T>);

                //if (typeOfModel.IsAssignableFrom(typeof(Student)))
                //{
                //    type = typeof(StudentsRepository);
                //}

                this.repositories.Add(typeOfModel, Activator.CreateInstance(type, this.context));
            }

            return (IRepository<T>)this.repositories[typeOfModel];
        } 
    }
}
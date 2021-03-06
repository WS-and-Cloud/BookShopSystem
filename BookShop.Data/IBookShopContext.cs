﻿namespace BookShop.Data
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    using BookShop.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    public interface IBookShopContext
    {
        IDbSet<Author> Authors { get; set; }

        IDbSet<Book> Books { get; set; }

        IDbSet<Category> Categories { get; set; }

        IDbSet<IdentityRole> Roles { get; set; }

        void SaveChanges();

        IDbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}
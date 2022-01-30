using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data
{
    public class MyDbContext : IdentityDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<Article> Article { get; set; }
        public DbSet<Category> Category { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Article>().HasOne(a => a.Category).WithMany(c => c.Articles).HasForeignKey(a => a.CategoryId);

            modelBuilder.Entity<Category>().HasData(
                new Category() { Id = 1, Nazwa = "Toys" },
                new Category() { Id = 2, Nazwa = "Flowers" },
                new Category() { Id = 3, Nazwa = "Tools" }
            );

            modelBuilder.Entity<Article>().HasData(
                new Article() { Id = 1, Nazwa = "Doll", Price = 12, CategoryId = 1, Photo = "noimage.jpg" },
                new Article() { Id = 2, Nazwa = "Hammer", Price = 45, CategoryId = 3, Photo = "noimage.jpg" },
                new Article() { Id = 3, Nazwa = "Gazania", Price = 5, CategoryId = 2, Photo = "gazania31312021.jpg" },
                new Article() { Id = 4, Nazwa = "Gazania 2", Price = 5, CategoryId = 2, Photo = "gazania231312021.jpg" }
            );
        }
    }
}

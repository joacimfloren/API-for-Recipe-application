using Microsoft.EntityFrameworkCore;
using Mjecipies_Group_D.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Business.DataLayer
{
    public class ApplicationUser : IdentityUser
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Token { get; set; }

        public string FacebookId { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Recipe> Recipes { get; set; }

        public ICollection<Favorites> FavoriteRecipes { get; set; }
    }
    public class MjecipiesContext : IdentityDbContext<ApplicationUser>
    {
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //Server DB String - For live usage
            // ****
            //Local DB String - For testing
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MjecipiesDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<Favorites>()
                .HasKey(f => new { f.RecipeId, f.AccountId });

            modelBuilder.Entity<Favorites>()
                .HasOne(f => f.Account)
                .WithMany(f => f.FavoriteRecipes)
                .HasForeignKey(f => f.AccountId);

            modelBuilder.Entity<Favorites>()
                .HasOne(f => f.Recipe)
                .WithMany(f => f.FavoritedBy)
                .HasForeignKey(f => f.RecipeId);
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<ApplicationUser> User { get; set; }
    }
}

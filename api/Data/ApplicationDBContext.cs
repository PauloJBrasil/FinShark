using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Stock> Stocks { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Portifolio> Portifolios { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Portifolio>(x => x.HasKey(p => new { p.AppUserId, p.StockId }));

            modelBuilder.Entity<Portifolio>()
                .HasOne(p => p.AppUser)
                .WithMany(u => u.Portifolios)
                .HasForeignKey(p => p.AppUserId);

            modelBuilder.Entity<Portifolio>()
                .HasOne(p => p.Stock)
                .WithMany(u => u.Portifolios)
                .HasForeignKey(p => p.StockId);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" }
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
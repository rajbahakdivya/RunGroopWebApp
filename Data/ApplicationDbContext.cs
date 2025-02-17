using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Data
{
    public class ApplicationDbContext :IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Race> Races { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AppUser> AspNetUsers { get; set; }
        //public DbSet<UserRoles> AspNetUserRoles { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<UserRoles>().HasNoKey(); // Marks it as a keyless entity
        //    modelBuilder.Entity<AppUser>().HasNoKey(); // Marks it as a keyless entity
        //}
    }
}

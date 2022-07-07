using Library.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        // EF Core Database customization.

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Authors> Authors => Set<Authors>();
        public DbSet<Books> Books => Set<Books>();
        public DbSet<Languages> Languages => Set<Languages>();
        public DbSet<Locations> Locations => Set<Locations>();
        public DbSet<UserAddresses> UserAddresses => Set<UserAddresses>();
        public DbSet<Categories> Categories => Set<Categories>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

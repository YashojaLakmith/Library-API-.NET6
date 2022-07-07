using Library.Entities;

using Microsoft.AspNetCore.Identity;

namespace Library.Infrastructure
{
    public static class Extensions
    {
        // Method for invoke data feed methods in DBInitializer class, if no database exists.

        public static async Task CreateDbIfNotExistsAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<Users>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await context.Database.EnsureCreatedAsync();
                
                await DbInitializer.InitializeUserStore(context, userManager, roleManager);
                await DbInitializer.InitializeBookStore(context);
            }
        }
    }
}

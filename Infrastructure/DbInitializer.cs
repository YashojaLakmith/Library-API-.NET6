using Library.Entities;

using Microsoft.AspNetCore.Identity;

namespace Library.Infrastructure
{
    public static class DbInitializer
    {
        // Initial data to feed in to the database when the application starts.

        public static async Task InitializeUserStore(ApplicationDbContext context, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (context.Users.Any())
            {
                return;
            }

            var samplePassword = "User@123456789";
            string[] roles = { UserRoles.System, UserRoles.Admin, UserRoles.User };

            var userAddressSys = new UserAddresses()
            {
                Id = "SYSTEM_DEFAULT_USER",
                District = "SYSTEM",
                City = "SYSTEM",
                Street1 = "SYSTEM",
                Street2 = "SYSTEM",
                Street3 = "SYSTEM"
            };

            var userAddressAdmin = new UserAddresses()
            {
                Id = "ADMIN_DEFAULT_USER",
                District = "SYSTEM",
                City = "SYSTEM",
                Street1 = "SYSTEM",
                Street2 = "SYSTEM",
                Street3 = "ADMIN"
            };

            var userAddressUser = new UserAddresses()
            {
                Id = "USER_DEFAULT_USER",
                District = "SYSTEM",
                City = "SYSTEM",
                Street1 = "SYSTEM",
                Street2 = "SYSTEM",
                Street3 = "USER"
            };

            var userList = new List<Users>()
            {
                new Users()
                {
                    Id = "SYSTEM_DEFAULT_USER",
                    UserName = "SYSTEM",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    Address = userAddressSys
                },

                new Users()
                {
                    Id = "ADMIN_DEFAULT_USER",
                    UserName = "ADMIN",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    Address = userAddressAdmin
                },

                new Users()
                {
                    Id = "USER_DEFAULT_USER",
                    UserName = "USER",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    Address = userAddressUser
                }
            };

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
            foreach(var user in userList)
            {
                await userManager.CreateAsync(user, samplePassword);
                if (user.Id.Equals("SYSTEM_DEFAULT_USER"))
                {
                    await userManager.AddToRoleAsync(user, UserRoles.System);
                }
                if (user.Id.Equals("ADMIN_DEFAULT_USER"))
                {
                    await userManager.AddToRoleAsync(user, UserRoles.Admin);
                }
                if (user.Id.Equals("USER_DEFAULT_USER"))
                {
                    await userManager.AddToRoleAsync(user, UserRoles.User);
                }
            }
        }

        public static async Task InitializeBookStore(ApplicationDbContext context)
        {
            if(context.Authors.Any()
                && context.Books.Any()
                && context.Categories.Any()
                && context.Languages.Any()
                && context.Locations.Any()
                && context.UserAddresses.Any())
            {
                return;
            }

            Authors[] authors = 
            {
                new Authors(){Name = "SampleAuthor1"},
                new Authors(){Name = "SampleAuthor2"},
                new Authors(){Name = "SampleAuthor3"}
            };

            Languages[] languages =
            {
                new Languages(){LanguageId = "EN"},
                new Languages(){LanguageId = "SIN"},
                new Languages(){LanguageId = "TAM"}
            };

            Categories[] categories =
            {
                new Categories(){CategoryName = "SampleCategory1"},
                new Categories(){CategoryName = "SampleCategory2"},
                new Categories(){CategoryName = "SampleCategory3"},
                new Categories(){CategoryName = "SampleCategory4"},
                new Categories(){CategoryName = "SampleCategory5"},
                new Categories(){CategoryName = "SampleCategory6"},
            };

            Locations[] locations =
            {
                new Locations()
                {
                    Id = "0-1-1",
                    Floor = 0,
                    Rack = 1,
                    Shelf = 1
                },
                new Locations()
                {
                    Id = "1-2-2",
                    Floor = 1,
                    Rack = 2,
                    Shelf = 2
                },
                new Locations()
                {
                    Id = "2-3-3",
                    Floor = 2,
                    Rack = 3,
                    Shelf = 3
                },
            };

            var bookList = new List<Books>()
            {
                new Books()
                {
                    Id = "SampleBook1",
                    Name = "SampleBook1",
                    CopiesAvailable = 5,
                    Edition = 1,
                    ISBN = "0000000",
                    Description = "sample",
                    Author = authors[0],
                    Language = languages[0],
                    Categories = new List<Categories>(){categories[0], categories[1] },
                    Location = locations[0],
                },
                new Books()
                {
                    Id = "SampleBook2",
                    Name = "SampleBook2",
                    CopiesAvailable = 0,
                    Edition = 1,
                    ISBN = "0000001",
                    Description = "sample",
                    Author = authors[1],
                    Language = languages[1],
                    Categories = new List<Categories>(){categories[2], categories[3] },
                    Location = locations[1],
                },
                new Books()
                {
                    Id = "SampleBook3",
                    Name = "SampleBook3",
                    CopiesAvailable = 1,
                    Edition = 1,
                    ISBN = "0000002",
                    Description = "sample",
                    Author = authors[2],
                    Language = languages[2],
                    Categories = new List<Categories>(){categories[4], categories[5] },
                    Location = locations[2],
                },
            };



            await context.Books.AddRangeAsync(bookList);
            await context.SaveChangesAsync();
        }
    }
}

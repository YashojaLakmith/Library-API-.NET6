using Library.Entities;
using Library.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public class UserServices : IUserServices
    {
        // This class provides data access layer functionalities relating to the users that not available through ASP.NET Identity implementation, by implementing IUserServices interface.

        private readonly ApplicationDbContext _context;

        public UserServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAddressAsync(UserAddresses address)
        {
            await _context.UserAddresses.AddAsync(address);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Users>> GetAllUsersAsync(string? bookId = null, bool? isActive = null)
        {
            if (string.IsNullOrEmpty(bookId) && isActive == null)
            {
                return await _context.Users.ToListAsync();
            }

            if(!string.IsNullOrEmpty(bookId) && isActive == null)
            {
                return await _context.Users
                                .Where(u => (u.RentedBooks != null)
                                    && u.RentedBooks.Any(b => b.Id == bookId))
                                .ToListAsync();
            }
            
            if(string.IsNullOrEmpty(bookId) && isActive != null)
            {
                return await _context.Users
                                .Where(u => u.IsActive == isActive)
                                .ToListAsync();
            }

            return await _context.Users
                                .Where(u => (u.RentedBooks != null)
                                    && u.RentedBooks.Any(b => b.Id == bookId)
                                    && u.IsActive == isActive)
                                .ToListAsync();
        }

        public async Task<Users?> GetUserByIdAsync(string id)
        {
            return await _context.Users
                                .Include(u => u.Address)
                                .Include(u => u.RentedBooks)
                                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task ModifyUserInfoAsync(Users user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            existingUser = user;
            await _context.SaveChangesAsync();
        }
    }
}

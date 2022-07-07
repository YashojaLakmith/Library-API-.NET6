using Library.Entities;
using Library.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public class CounterServices : ICounterServices
    {
        // This class provides data access layer functionalities relating to the borrowing and returning of the books through the implementation of ICounterServices interface.

        private readonly ApplicationDbContext _context;

        public CounterServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task IgnoreRentedBooksAsync(Users user, IEnumerable<Books> books)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(user.Id));

            foreach (var book in books)
            {
                existingUser.RentedBooks.Remove(book);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RentBooksAsync(Users user, IEnumerable<Books> books)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(user.Id));
            var existingBooks = await _context.Books.Where(e => books.Contains(e)).ToListAsync();
            
            foreach(var book in books)
            {
                existingUser.RentedBooks.Add(book);
            }

            foreach(var book in existingBooks)
            {
                book.CopiesAvailable -= 1;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ReturnBooksAsync(Users user, IEnumerable<Books> books)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(user.Id));
            var existingBooks = await _context.Books.Where(b => books.Contains(b)).ToListAsync();

            foreach(var book in books)
            {
                existingUser.RentedBooks.Remove(book);
            }

            foreach(var book in existingBooks)
            {
                book.CopiesAvailable += 1;
            }

            await _context.SaveChangesAsync();
        }
    }
}

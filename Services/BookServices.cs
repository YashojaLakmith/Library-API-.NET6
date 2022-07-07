using Library.Entities;
using Library.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public class BookServices : IBookServices
    {
        // This class provides the data access layer functionalities relating to the book information through implemetation of IBookServices interface.

        private readonly ApplicationDbContext _context;

        public BookServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Books>> GetAllBooksAsync(string? bookName = null, string? author = null, string? category = null, string? language = null)
        {
            return await _context.Books
                            .Where(b => (bookName == null || b.Name.Equals(bookName))
                                    && (author == null || b.Author.Name.Equals(author))
                                    && (category == null || b.Categories.Any(c => c.Equals(category)))
                                    && (language == null || b.Language.LanguageId.Equals(language)))
                            .Include(b => b.Author)
                            .Include(b => b.Language)
                            .Include(b => b.Categories)
                            .Include(b => b.Location)
                            .Include(b => b.Location)
                            .OrderBy(b => b.Name)
                            .ToListAsync();
        }

        public async Task<Books?> GetBookByIdAsync(string id)
        {
            return await _context.Books
                            .Include(b => b.Author)
                            .Include(b => b.Language)
                            .Include(b => b.Categories)
                            .Include(b => b.Location)
                            .Include(b => b.Location)
                            .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task RemoveBookAsync(Books book)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id.Equals(book.Location.Id));

            _context.Books.Remove(book);
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }

        public async Task<Authors?> GetAuthorByNameAsync(string name)
        {
            return await _context.Authors.FirstOrDefaultAsync(a => a.Name.Equals(name));
        }

        public async Task<Locations?> GetLocationsByIdAsync(string id)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Languages?> GetLanguageAsync(string language)
        {
            return await _context.Languages.FirstOrDefaultAsync(l => l.LanguageId.Equals(language));
        }

        public async Task<List<Categories>> GetAllCategoriesAsync(string[]? categories = null)
        {
            return await _context.Categories
                                    .Where(c => categories == null || categories.Contains(c.CategoryName))
                                    .ToListAsync();
        }

        public async Task<IEnumerable<Books>> GetMatchingBooksMultipleAsync(string[] books)
        {
            return await _context.Books
                                    .Where(b => books.Contains(b.Id))
                                    .ToListAsync();                       
        }

        public async Task CreateBookAsync(Books book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }
    }
}

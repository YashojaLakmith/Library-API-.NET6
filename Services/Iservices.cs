using Library.Entities;

namespace Library.Services

    // This includes all the interfaces that provides data access layer functionalities of the application.
{
    public interface IUserServices
    {
        public Task<IEnumerable<Users>> GetAllUsersAsync(string? bookId = null, bool? isActive = null);
        public Task AddUserAddressAsync(UserAddresses address);
        public Task<Users?> GetUserByIdAsync(string id);
        public Task ModifyUserInfoAsync(Users user);
    }

    public interface IBookServices
    {
        public Task<IEnumerable<Books>> GetAllBooksAsync(string? bookName = null, string? author = null, string? category = null, string? language = null);
        public Task<IEnumerable<Books>> GetMatchingBooksMultipleAsync(string[] books);
        public Task<Books?> GetBookByIdAsync(string id);
        public Task<Locations?> GetLocationsByIdAsync(string id);
        public Task RemoveBookAsync(Books book);
        public Task<Languages?> GetLanguageAsync(string language);
        public Task<Authors?> GetAuthorByNameAsync(string name);
        public Task<List<Categories>> GetAllCategoriesAsync(string[]? categories = null);
        public Task CreateBookAsync(Books book);
    }

    public interface ICounterServices
    {
        public Task RentBooksAsync(Users user, IEnumerable<Books> books);
        public Task ReturnBooksAsync(Users user, IEnumerable<Books> books);
        public Task IgnoreRentedBooksAsync(Users user, IEnumerable<Books> books);
    }
}
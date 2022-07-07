using Library.Dtos;
using Library.Entities;

namespace Library.Extensions
{
    public static class BookExtensions
    {
        // Include all custom extension methods used for shaping book related model data to the DTOs.

        public static SingleBookMinimalViewDto AsMinimalViewDto(this Books book)
        {
            var categories = new List<string>();
            var location = new { book.Location.Floor, book.Location.Rack, book.Location.Shelf };

            if (book.Categories != null)
            {
                foreach(var category in book.Categories)
                {
                    categories.Add(category.CategoryName);
                }
            }
           
            return new SingleBookMinimalViewDto(book.Id, book.Name, book.Author.Name, book.Language.LanguageId, book.CopiesAvailable, book.Edition, book.ISBN, book.Description, categories, location);
        }

        public static BookListViewDto AsBookListViewDto(this Books book)
        {
            return new BookListViewDto(book.Id, book.Name, book.Author.Name, book.Language.LanguageId, book.Edition);
        }

        public static SingleBookInAdminViewDto AsAdminViewDto(this Books book)
        {
            var categories = new List<string>();
            var rentedUsers = new List<object>();

            var location = new {book.Location.Floor, book.Location.Rack, book.Location.Shelf};

            if(book.RentedUsers != null)
            {
                foreach(var renterUser in book.RentedUsers)
                {
                    rentedUsers.Add(new { renterUser.Id, renterUser.UserName });
                }
            }
            if(book.Categories != null)
            {
                foreach(var category in book.Categories)
                {
                    categories.Add(category.CategoryName);
                }
            }

            return new SingleBookInAdminViewDto(book.Id, book.Name, book.Author.Name, book.Language.LanguageId, book.CopiesAvailable, book.Edition, book.ISBN, book.Description, location, categories, rentedUsers);
        }

        public static string AsCategoryViewDto(this Categories category)
        {
            return category.CategoryName;
        }
    }
}

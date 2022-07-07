using Library.Dtos;
using Library.Entities;

namespace Library.Extensions
{
    public static class UserExtensions
    {
        // Include all custom extension methods used for shaping user related model data to the DTOs.

        public static UserListViewDto AsUserListView(this Users user)
        {
            return new UserListViewDto(user.Id, user.UserName);
        }

        public static SingleUserViewDto AsSingleUserDto(this Users user)
        {
            var bookList = Enumerable.Empty<object>().ToList();
            if (user.RentedBooks is null || !user.RentedBooks.Any())
            {
                return new SingleUserViewDto(user.Id, user.UserName, user.CreatedOn, user.IsActive, bookList);
            }

            bookList.Add(user.RentedBooks.Select(b => new { BookId = b.Id, BookName = b.Name, b.Edition }));

            return new SingleUserViewDto(user.Id, user.UserName, user.CreatedOn, user.IsActive, bookList);

        }
    }
}

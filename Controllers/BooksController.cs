using Library.Dtos;
using Library.Entities;
using Library.Extensions;
using Library.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [ApiController]
    [Route("books/")]
    public class BooksController : ControllerBase
    {
        private readonly IBookServices _bookServices;
        private readonly UserManager<Users> _userManager;

        public BooksController(IBookServices bookServices, UserManager<Users> userManager)
        {
            _bookServices = bookServices;
            _userManager = userManager;
        }

        // Gets a list of all books (with optional filters) with limited information
        // Route: /books/
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<BookListViewDto>> GetAllBooksAsync([FromQuery] string? bookName = null, string? author = null, string? category = null, string? language = null)
        {
            return (await _bookServices.GetAllBooksAsync(bookName, author, category, language))
                                            .Select(b => b.AsBookListViewDto())
                                            .ToList();
        }

        // Gets information on a selected book in a detailed manner.
        // Route: /books/*bookId*
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<SingleBookMinimalViewDto>> GetBookByIdAsync([FromRoute] string id)
        {
            if (HttpContext.User.IsInRole(UserRoles.Admin) || HttpContext.User.IsInRole(UserRoles.System))
            {
                return RedirectToAction(nameof(GetSingleBookInAdminView), new { id });
            }

            var book = await _bookServices.GetBookByIdAsync(id);
            if(book is null)
            {
                return NotFound("Couldn't find the book with given id");
            }

            return book.AsMinimalViewDto();
        }

        // Gets information on a selected book in a detailed manner- including any borrowers
        // Route: /books/admin/*bookId*
        [HttpGet]
        [Authorize(Roles = UserRoles.Sys_Admin)]
        [Route("admin/{id}")]
        public async Task<ActionResult<SingleBookInAdminViewDto>> GetSingleBookInAdminView([FromRoute] string id)
        {
            var book = await _bookServices.GetBookByIdAsync(id);
            if(book is null)
            {
                return NotFound("Couldn't find a book with given id");
            }
            return book.AsAdminViewDto();
        }

        // Adds new book to the system
        // Route: /books/
        [HttpPost]
        [Authorize(Roles = UserRoles.Sys_Admin)]
        public async Task<ActionResult> CreateNewBookAsync([FromBody] CreateBookDto dto)
        {
            var existingBook = await _bookServices.GetBookByIdAsync(dto.Id);
            if(existingBook is not null)
            {
                return BadRequest("A book with the given id already exists");
            }

            Languages language;
            var existingLanguage = await _bookServices.GetLanguageAsync(dto.Language);

            if(existingLanguage is null)
            {
                return BadRequest("Language is not valid");
            }
            language = existingLanguage;

            Authors author;
            var existingAuthor = await _bookServices.GetAuthorByNameAsync(dto.Author);

            if(existingAuthor is null)
            {
                author = new()
                {
                    Name = dto.Author
                };
            }
            else
            {
                author = existingAuthor;
            }

            Locations location;
            var locationId = $"{dto.Location.Floor}-{dto.Location.Rack}-{dto.Location.Shelf}";
            var existingLocation = await _bookServices.GetLocationsByIdAsync(locationId);

            if(existingLocation is not null)
            {
                return BadRequest("Location is not available- has already been occupied by another book");
            }
            location = new()
            {
                Id = locationId,
                Floor = dto.Location.Floor,
                Rack = dto.Location.Rack,
                Shelf = dto.Location.Shelf
            };

            var existingCategories = await _bookServices.GetAllCategoriesAsync(dto.Categories);

            foreach(var category in dto.Categories)
            {
                if(!existingCategories.Any(e => e.CategoryName.Equals(category)))
                {
                    existingCategories.Add(new Categories() { CategoryName = category });
                }
            }

            Books book = new()
            {
                Id = dto.Id,
                Name = dto.Name,
                Author = author,
                Edition = dto.Edition,
                CopiesAvailable = dto.CopiesToInsert,
                ISBN = dto.ISBN,
                Description = dto.Description,
                Location = location,
                Language = language,
                Categories = existingCategories 
            };

            await _bookServices.CreateBookAsync(book);
            return CreatedAtAction(nameof(GetBookByIdAsync), new {id = dto.Id}, book.AsAdminViewDto());
        }

        // Completely remove a book from the system
        // Route: /books/*bookId*
        [HttpDelete]
        [Authorize(Roles = UserRoles.Sys_Admin)]
        [Route("{id}")]
        public async Task<ActionResult> RemoveBookAsync([FromRoute] string id)
        {
            var book = await _bookServices.GetBookByIdAsync(id);
            if(book is null)
            {
                return NotFound("Couldn't find the book with given id");
            }

            await _bookServices.RemoveBookAsync(book);
            return NoContent();
        }

        // Gets a list of all book categories
        // Route: /books/categories
        [HttpGet]
        [Authorize]
        [Route("categories")]
        public async Task<IEnumerable<string>> GetAllCategories()
        {
            return (await _bookServices.GetAllCategoriesAsync()).Select(c => c.AsCategoryViewDto());
        }
    }
}

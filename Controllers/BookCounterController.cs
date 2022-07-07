using Library.Dtos;
using Library.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [ApiController]
    [Route("counter/")]
    public class BookCounterController : ControllerBase
    {
        private readonly IBookServices _bookServices;
        private readonly ICounterServices _counterServices;
        private readonly IUserServices _userServices;

        public BookCounterController(IBookServices bookServices, ICounterServices counterServices, IUserServices userServices)
        {
            _bookServices = bookServices;
            _counterServices = counterServices;
            _userServices = userServices;
        }

        // Records borrowing of selected books by a user
        // Route: /counter/rent
        [HttpPut]
        [Authorize]
        [Route("rent")]
        public async Task<ActionResult> RentBooksAsync([FromBody] CounterDto dto)
        {
            var user = await _userServices.GetUserByIdAsync(dto.UserId);
            if(user is null)
            {
                return NotFound("Unable to find the user with given id");
            }

            var matchingBooks = await _bookServices.GetMatchingBooksMultipleAsync(dto.BookId);

            if (!matchingBooks.Any())
            {
                return BadRequest("Non existing book Ids selected");
            }

            if(user.RentedBooks != null && ((user.RentedBooks.Count + matchingBooks.Count()) > 5))
            {
                return BadRequest("Maximum rent limit reached");
            }

            if(user.RentedBooks != null && user.RentedBooks.Any(r => matchingBooks.Contains(r)))
            {
                return BadRequest("This book has already been borrowed by the user");
            }

            await _counterServices.RentBooksAsync(user, matchingBooks);

            return NoContent();
        }

        // Records returning of borrowed books by a user
        // Route: /counter/return
        [HttpPut]
        [Authorize]
        [Route("return")]
        public async Task<ActionResult> ReturnBooksDto([FromBody] CounterDto dto)
        {
            var user = await _userServices.GetUserByIdAsync(dto.UserId);
            if(user is null)
            {
                return NotFound("Coudn't find the user with given id");
            }

            var matchingBooks = await _bookServices.GetMatchingBooksMultipleAsync(dto.BookId);
            if(matchingBooks.Count() != dto.BookId.Count())
            {
                return BadRequest("Some books do not exist");
            }

            if (!matchingBooks.Any())
            {
                return BadRequest("There are no books with given IDs");
            }

            if(!matchingBooks.Any(m => user.RentedBooks.Contains(m)))
            {
                return BadRequest("User hasn't borrowed selected books");
            }

            await _counterServices.ReturnBooksAsync(user, matchingBooks);

            return NoContent();
        }

        // Ignnores and writes down selected books borrowed by a user
        // Route: /counter/ignore
        [HttpPut]
        [Authorize]
        [Route("ignore")]
        public async Task<ActionResult> IgnoreRentedBooksAsync([FromBody] CounterDto dto)
        {
            var user = await _userServices.GetUserByIdAsync(dto.UserId);
            if(user is null)
            {
                return NotFound("Couldn't find the user with given id");
            }

            var matchingBooks = await _bookServices.GetMatchingBooksMultipleAsync(dto.BookId);
            if( matchingBooks.Count() != dto.BookId.Count())
            {
                return BadRequest("Couldn't find some books");
            }

            var crossMatchWithUser = matchingBooks.Intersect(user.RentedBooks);
            if(crossMatchWithUser.Count() != dto.BookId.Count())
            {
                return BadRequest("Some books haven't been rented by the user");
            }

            await _counterServices.IgnoreRentedBooksAsync(user, matchingBooks);

            return NoContent();
        }
    }
}

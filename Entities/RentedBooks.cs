using Microsoft.EntityFrameworkCore;

namespace Library.Entities
{
    [Keyless]
    public class RentedBooks
    {
        public Books Book { get; set; }
        public Users User { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace Library.Entities
{
    public class Users : IdentityUser
    {
        public UserAddresses Address { get; set; }
        public DateTime CreatedOn { get; set; }
        public IList<Books> RentedBooks { get; set; } = new List<Books>();
        public bool IsActive { get; set; }
    }
}

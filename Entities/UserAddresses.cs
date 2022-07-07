using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace Library.Entities
{
    public class UserAddresses
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        [MaxLength(119)]
        [MinLength(5)]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(15)]
        public string District { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(20)]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(30)]
        public string Street1 { get; set; }

        [MaxLength(30)]
        public string? Street2 { get; set; }

        [MaxLength(30)]
        public string? Street3 { get; set; }
    }
}

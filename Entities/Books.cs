using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities
{
    public class Books
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name { get; set; }

        public Authors Author { get; set; }
        public Languages Language { get; set; }

        [Required]
        [Range(0, 100)]
        public int CopiesAvailable { get; set; }

        [ForeignKey("Locations")]
        public Locations Location { get; set; }

        public IList<Users> RentedUsers { get; set; } = new List<Users>();
        public IList<Categories> Categories { get; set; } = new List<Categories>();

        [Range(0, 100)]
        public double Edition { get; set; }

        [MaxLength(100)]
        public string? ISBN { get; set; }

        [MaxLength(450)]
        public string? Description { get; set; }
    }
}

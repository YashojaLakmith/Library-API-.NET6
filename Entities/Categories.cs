using System.ComponentModel.DataAnnotations;

namespace Library.Entities
{
    public class Categories
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        [MaxLength(25)]
        public string CategoryName { get; set; }

        public IList<Books> Books { get; set; } = new List<Books>();

        [MaxLength(450)]
        public string? Description { get; set; }
    }
}

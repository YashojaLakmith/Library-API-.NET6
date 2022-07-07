using System.ComponentModel.DataAnnotations;

namespace Library.Entities
{
    public class Authors
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name { get; set; }

        public IList<Books> Books { get; set; } = new List<Books>();
    }
}

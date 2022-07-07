using System.ComponentModel.DataAnnotations;

namespace Library.Entities
{
    public class Languages
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        [MaxLength(20)]
        public string LanguageId { get; set; }

        public IList<Books> Books { get; set; } = new List<Books>();
    }
}

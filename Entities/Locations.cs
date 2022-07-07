using System.ComponentModel.DataAnnotations;

namespace Library.Entities
{
    public class Locations
    {
        [Key]
        [MinLength(5)]
        [MaxLength(12)]
        public string Id { get; set; }      // Floor-Rack-Shelf format

        [Required]
        [Range(-100, 100)]
        public int Floor { get; set; }

        [Required]
        [Range(1, 500)]
        public int Rack { get; set; }

        [Required]
        [Range(1, 200)]
        public int Shelf { get; set; }
        public Books Book { get; set; }
    }
}

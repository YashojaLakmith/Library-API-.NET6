using System.ComponentModel.DataAnnotations;

namespace Library.Dtos
{
    public class AddressDtoClass
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(15)]
        public string District { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength (20)]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(30)]
        public string Street1 { get; set; }

        [MaxLength(30)]
        public string? Street2 { get; set; }

        [MaxLength(30)]
        public string? Street3 { get; set; }
    }

    public class LocationClassForDto
    {
        [Required]
        [Range(-100, 100)]
        public int Floor { get; set; }

        [Required]
        [Range(1, 500)]
        public int Rack { get; set; }

        [Required]
        [Range(1, 200)]
        public int Shelf { get; set; }
    }
}

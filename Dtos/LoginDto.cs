using System.ComponentModel.DataAnnotations;

namespace Library.Dtos
{
    // This includes all defined Data Transfer Objects used when getting login information from the user.

    public record LoginDto
    (
        [Required(AllowEmptyStrings = false)]
        [MinLength(3), MaxLength(100)]
        string Id,

        [Required(AllowEmptyStrings = false)]
        [MinLength(6), MaxLength(15)]
        string Password
    );
}

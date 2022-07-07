using System.ComponentModel.DataAnnotations;

namespace Library.Dtos
{
    // // This includes all defined Data Transfer Objects used when communicating book counter related information with the user.

    public record CounterDto
    (
        [Required(AllowEmptyStrings = false)]
        [MinLength(3), MaxLength(100)]
        string UserId,

        [Required(AllowEmptyStrings = false)]
        [MinLength(1), MaxLength(5)]
        string[] BookId
    );
}

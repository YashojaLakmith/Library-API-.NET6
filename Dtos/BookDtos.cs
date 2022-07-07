using System.ComponentModel.DataAnnotations;

namespace Library.Dtos
{
    // This includes all defined Data Transfer Objects used when communicating book related information with the user.

    public record SingleBookMinimalViewDto
    (
        string Id,
        string BookName,
        string Author,
        string Language,
        int CopiesAvailable,
        double Edition,
        string ISBN,
        string Description,
        IList<string> Categories,
        object Location
    );

    public record BookListViewDto
    (
        string Id,
        string Name,
        string Author,
        string Language,
        double Edition
    );

    public record SingleBookInAdminViewDto
    (
        string Id,
        string BookName,
        string Author,
        string Language,
        int CopiesAvailable,
        double Edition,
        string ISBN,
        string Description,
        object Location,
        IList<string> Categories,
        IList<object> RentedUsers
    );

    public record CreateBookDto
    (
        [Required(AllowEmptyStrings = false)] [MaxLength(100), MinLength(1)]
        string Id,

        [Required(AllowEmptyStrings = false)] [MaxLength(100), MinLength(1)]
        string Name,

        [Required(AllowEmptyStrings = false)] [MaxLength(100), MinLength(1)]
        string Author,

        [Required(AllowEmptyStrings = false)] [MaxLength(20), MinLength(1)]
        string Language,

        [Required] [Range(0, 100)]
        double Edition,

        [Required] [Range(0, 100)]
        int CopiesToInsert,

        [MaxLength(100)]
        string ISBN,

        [MaxLength(450)]
        string Description,

        [Required(AllowEmptyStrings = false)] [MaxLength(10), MinLength(1)]
        string[] Categories,

        [Required]
        LocationClassForDto Location
    );
}

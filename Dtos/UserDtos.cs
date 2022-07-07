using System.ComponentModel.DataAnnotations;

using Library.Entities;

namespace Library.Dtos
{
    // This includes all defined Data Transfer Objects used when communicating user account related information with the user.

    public record RegisterUserDto
    (
        [Required(AllowEmptyStrings = false)]
        [MaxLength(3), MinLength(100)]
        string Id,

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100), MinLength(3)]
        string Name,

        [Required(AllowEmptyStrings = false)]
        [MaxLength(15), MinLength(6)]
        string Password,

        [Required(AllowEmptyStrings =false)]
        [MaxLength(5), MinLength(4)]
        string Role,

        [Required]
        AddressDtoClass Address
    );

    public record UserListViewDto
    (
        string Id,
        string Name
    );

    public record SingleUserViewDto
    (
        string Id,
        string Name,
        DateTime CreatedOn,
        bool IsActive,
        List<object> RentedBooks
    );

    public record ModifyUserDto
    (
        [MinLength(3), MaxLength(100)]
        string? Name,

        AddressDtoClass? Address,

        [MinLength(4), MaxLength(5)]
        string? IsActive
    );

    public record ChangePasswordDto
    (
        [Required(AllowEmptyStrings = false)]
        [MinLength(6), MaxLength(15)]
        string CurrentPassword,

        [Required(AllowEmptyStrings = false)]
        [MinLength(6), MaxLength(15)]
        string NewPassword,

        [Required(AllowEmptyStrings = false)]
        [MinLength(6), MaxLength(15)]
        string ConfirmNewPassword
    );
}

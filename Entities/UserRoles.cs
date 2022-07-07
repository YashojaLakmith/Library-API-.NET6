namespace Library.Entities
{
    public class UserRoles
    {
        public const string Admin = "ADMIN";
        public const string User = "USER";
        public const string System = "SYSTEM";

        public const string Sys_Admin = $"{System},{Admin}";
    }
}

using System.ComponentModel.DataAnnotations;

namespace EShop.Dto.UserModel
{
    public class UserDto
    {
        [Required (ErrorMessage = "Name is required")]
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? OtherName { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public List<string>? Roles { get; set; }

    }
}

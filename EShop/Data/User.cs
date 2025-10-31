using System.ComponentModel.DataAnnotations;

namespace EShop.Data
{
    public class User : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? OtherName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

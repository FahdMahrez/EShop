namespace EShop.Data
{
    public class UserRole
    {
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}

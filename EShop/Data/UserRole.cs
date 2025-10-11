namespace EShop.Data
{
    public class UserRole : BaseEntity
    {
        public Role RoleId { get; set; }
        public User UserId { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
    }
}

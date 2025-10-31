namespace EShop.Data
{
    public class Role : BaseEntity
    {
        public string Description { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Data
{
    public class UserRole : BaseEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [ForeignKey(nameof(Role))]
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}

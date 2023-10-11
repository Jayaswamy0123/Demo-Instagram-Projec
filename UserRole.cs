using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Instagram.Entities
{
    [Table("user_role")]
    public class UserRole
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserRole")]
        public User User { get; set; }

        [Column("role_id")]
        public long RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        [InverseProperty("UserRole")]
        public Role Role { get; set; }


    }
}

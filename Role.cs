using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Instagram.Entities
{
    [Table("role")]
    public class Role
    {
        public Role()
        {
            UserRole = new HashSet<UserRole>();
        }
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("name")]
        [MaxLength(20)]
        public string Name { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<UserRole> UserRole { get; set; }

    }
}

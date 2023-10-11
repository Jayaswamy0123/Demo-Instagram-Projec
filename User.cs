using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instagram.Entities
{
    [Table("user")]
    public class User
    {
        public User()
        {
            UserRole = new HashSet<UserRole>();
            UserMedia = new HashSet<UserMedia>();
            Followers = new HashSet<Followers>();
        }
        [Column("id")]
        [Key]
        public long Id { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("mobile_number")]
        public long MobileNumber { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Followers> Followers { get; set; }
        
        [InverseProperty("User")]
        public virtual ICollection<UserRole> UserRole { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserMedia> UserMedia { get; set; }
    }    
}

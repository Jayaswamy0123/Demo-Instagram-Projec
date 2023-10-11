using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Instagram.Entities
{
    [Table("user_media")]
    public class UserMedia
    {

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserMedia")]
        public User User { get; set; }

        [Column("type")]
        public int Type { get; set; }

        [Column("type_name")]
        [MaxLength(50)]
        public string TypeName { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("media_url")]
        public string MediaUrl { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; }

        [Column("caption")]
        public string Caption { get; set; } 

    }
}

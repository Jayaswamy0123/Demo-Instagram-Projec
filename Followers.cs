using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instagram.Entities
{
    public class Followers
    {
        [Column("id")]
        [Key]
        public long Id { get; set; }

        [Column("user_id")] 
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Column("followers_id")]
        public long FollowersId { get; set; }
        
        [ForeignKey(nameof(FollowersId))]
        public User UserFollowers { get; set; }


    }
}

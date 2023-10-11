using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Instagram.Models
{
    public class RegisterModel : Emails
    {
        //[JsonProperty("id")]
        //public long Id { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("mobileNumber")]
        public long MobileNumber { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("roleId")]
        public int RoleId { get; set; }
    }
    public class Emails
    {
        [JsonProperty("email")]
        public string Email { get; set; }


    }

    public class LoginModel : Emails
    {
        [JsonProperty("mobileNumber")]
        public long MobileNumber { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }


    }

    public class ForgotPasswordModel : Emails
    {
        [JsonProperty("mobileNumber")]
        public long MobileNumber { get; set; }
    }

    public class ResetPasswordModel : Emails
    {
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }

    public class UserDetailsModel: Emails
    {
        [JsonProperty("mobileNumber")]
        public new long MobileNumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("roleId")]
        public long RoleId { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("userRolename")]
        public string UserRoleName { get; set; }


    }

    public class UserMediaProfileModel
    {
        [JsonProperty("profileImage")]
        public IFormFile ProfileImage { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set;}
    }

        public class UserMediaPostModel
        { 

        [JsonProperty("userPost")]
        public IFormFile UserPost { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }
    }

    public class FollowersFollowingCountsModel
    {
        [JsonProperty("followersCount")]
        public int FollowersCount { get; set; }

        [JsonProperty("followingCount")]
        public int FollowingCount { get; set; }
    }

    public class postModel
    {
        [JsonProperty("postCount")]
        public int PostCount { get; set; }
    }
}

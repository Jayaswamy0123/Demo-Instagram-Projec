using Instagram.Entities;
using Instagram.Models;
using Instagram.Repository.Iservices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Instagram.Repository.Services.AccountServices;

namespace Instagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountservices;

        public AccountController(IAccountServices accountservices)
        {
            _accountservices = accountservices;
        }

        [HttpPost("[action]")]
        public async Task<ApiResponse> Register(RegisterModel model)
        {
            return await _accountservices.Register(model);
        }
        [HttpDelete("[action]")]
        public async Task<ApiResponse> RemoveUsersById(int id)
        {
            return await _accountservices.RemoveUsersById(id);
        }

        [HttpPost("[action]")]
        public async Task<ServiceResponse<LoginResponse>> Login(LoginModel model)
        {
            return await _accountservices.Login(model);
        }

        [HttpPost("[action]")]
        public async Task<ApiResponse> ForgotPassword(ForgotPasswordModel model)
        {
            return await _accountservices.ForgotPassword(model);
        }

        [HttpPost("[action]")]
        public async Task<ApiResponse> ResetPassword(ResetPasswordModel model)
        {
            return await _accountservices.ResetPassword(model);
        }

        [HttpGet("[action]")]
        public async Task<ServiceResponse<object>> FollowingUsers()
        {
            return await _accountservices.FollowingUsers();
        }
        [HttpGet("[action]")]
        public async Task<ServiceResponse<object>> FollowersUsers()
        {
            return await _accountservices.FollowersUsers();
        }

        [HttpGet("[action]")]
        public async Task<ServiceResponse<object>> TotalNumberofFollowing()
        {
            return await _accountservices.TotalNumberOfFollowing();
        }

        [HttpGet("[action]")]
        public async Task<ServiceResponse<object>> TotalNumberofFollowers()
        {
            return await _accountservices.TotalNumberOfFollowers();
        }

        [HttpGet("[action]")]
        public async Task<ServiceResponse<object>> GetFollowingDetails()
        {
            return await _accountservices.GetFollowingDetails();
        }

        [HttpGet("[action]")]
        public async Task<ServiceResponse<object>> GetFollowersDetails()
        {
            return await _accountservices.GetFollowersDetails();
        }


        [HttpPost("[action]")]
        [Authorize]
        public async Task<ApiResponse> ProfilePictureUpload([FromForm] UserMediaProfileModel model)
        {
            return await _accountservices.ProfilePictureUpload(model);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<ServiceResponse<object>> Follow(int followerId)
        {
            return await _accountservices.Follow(followerId);
        }


        [HttpDelete("[action]")]
        [Authorize]
        public async Task<ServiceResponse<object>> Unfollow(int followerId)
        {
            return await _accountservices.Unfollow(followerId);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<ApiResponse> PostUpload([FromForm] UserMediaPostModel model)
        {
            return await _accountservices.PostUpload(model);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<ApiResponse> UploadOrEditProfilePicture([FromForm] UserMediaProfileModel model)
        {
            return await _accountservices.UploadOrEditProfilePicture(model);
        }

       [HttpPost("[action]")]
        public async Task<List<User>> GetFollowers(int userId)
        {
            return await _accountservices.GetFollowers(userId);
        }

        [HttpPost("[action]")]
        public async Task<List<User>> GetFollowing(int userId)
        {
            return await _accountservices.GetFollowing(userId);
        }

        [HttpPost("[action]")]
        public async Task<FollowersFollowingCountsModel> GetFollowersandFollowingCounts(int userId)
        {
            return await _accountservices.GetFollowersandFollowingCounts(userId);
        }

        [HttpGet("[action]")]
        public async Task<int> GetPost(int userId)
        { 
            return await _accountservices.GetPost(userId);
        }


        [HttpGet("[action]")]
        public async Task<int> GetFollowersCount(int userId)
        {
            return await _accountservices.GetFollowersCount(userId);
        }



        [HttpGet("[action]")]
        public async Task<int> GetFollowingCount(int userId)
        {
            return await _accountservices.GetFollowingCount(userId);
        }

        [HttpGet("[action]")]
         public async Task<dynamic> GetFollowersAndFollowingCounts(int userId)
        {
            return await _accountservices.GetFollowersAndFollowingCounts(userId);
        }
    }
}

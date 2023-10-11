using Instagram.Entities;
using Instagram.Models;
using Microsoft.AspNetCore.Mvc;
using static Instagram.Repository.Services.AccountServices;

namespace Instagram.Repository.Iservices
{
    public interface IAccountServices
    {
        Task<ApiResponse> Register(RegisterModel model);
        Task<ApiResponse> RemoveUsersById(int id);
        Task<ServiceResponse<LoginResponse>> Login(LoginModel model);
        Task<ApiResponse> ForgotPassword(ForgotPasswordModel model);
        Task<ApiResponse> ResetPassword(ResetPasswordModel model);
        Task<ServiceResponse<object>> FollowersUsers();

        Task<ServiceResponse<object>> FollowingUsers();
        Task<ServiceResponse<object>> TotalNumberOfFollowing();

        Task<ServiceResponse<object>> TotalNumberOfFollowers();
        Task<ServiceResponse<object>> GetFollowingDetails();
        Task<ServiceResponse<object>> GetFollowersDetails();

        Task<ApiResponse> ProfilePictureUpload([FromForm] UserMediaProfileModel model);

        Task<ServiceResponse<object>> Follow(int followerId);
        Task<ServiceResponse<object>> Unfollow(int followerId);
        Task<ApiResponse> PostUpload([FromForm] UserMediaPostModel model);

        Task<ApiResponse> UploadOrEditProfilePicture([FromForm] UserMediaProfileModel model);

        Task<List<User>> GetFollowers(int userId);

        Task<List<User>> GetFollowing(int userId);

        Task<FollowersFollowingCountsModel> GetFollowersandFollowingCounts(int userId);

        Task<int> GetPost(int userId);

        Task<int> GetFollowersCount(int userId);

        Task<int> GetFollowingCount(int userId);

        Task<dynamic> GetFollowersAndFollowingCounts(int userId);


        }
}

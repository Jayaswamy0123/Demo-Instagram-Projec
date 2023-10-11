using Instagram.Entities;
using Instagram.GlobalVariables;
using Instagram.Models;
using Instagram.Repository.Iservices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Instagram.GlobalVariables.ResponseConstents;

namespace Instagram.Repository.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly IConfiguration _configuration;
        private readonly InstagramContext _context;
        private readonly IEmailServices _emailServices;
        private readonly IUserServices _currentUser;

        public AccountServices(InstagramContext context,
                              IConfiguration configuration,
                              IEmailServices emailServices,
                              IUserServices currentUser)
        {
            _context = context;
            _configuration = configuration;
            _emailServices = emailServices;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse> Register(RegisterModel model)
        {
            var resut = await _context.User.Where(x => x.Email == model.Email).FirstOrDefaultAsync();
            if (resut != null)
            {
                return new ApiResponse("Email Already Exists", 400);
            }
            else
            {
                var users = new User
                {
                    FullName = model.FullName,
                    UserName = model.UserName,
                    MobileNumber = model.MobileNumber,
                    Email = model.Email,
                    Password = Encipher(model.Password),

                };
                users.UserRole.Add(new()
                {
                    RoleId = model.RoleId,
                    UserId = users.Id
                });
                _context.Add(users);
                _context.SaveChanges();
                return new ApiResponse("Register Successfully", 200);

            }

        }

        public async Task<ApiResponse> RemoveUsersById(int id)
        {
            var result = await _context.User.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (result != null)
            {
                _context.Remove(result);
                _context.SaveChanges();
                return new ApiResponse("Delete Successfully", 200);
            }
            else
            {
                return new ApiResponse("Id Not Found", 400);
            }
        }

        private static string Encipher(string password)
        {
            string key = "abcdefghijklmnopqrstuvwxyz1234567890";
            byte[] bytesBuff = Encoding.Unicode.GetBytes(password);
            using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new(key,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using MemoryStream mStream = new();
                using (CryptoStream cStream = new(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cStream.Write(bytesBuff, 0, bytesBuff.Length);
                    cStream.Close();
                }
                password = Convert.ToBase64String(mStream.ToArray());
            }
            return password;
        }

        private static string Decipher(string password)
        {
            string key = "abcdefghijklmnopqrstuvwxyz1234567890";
            password = password.Replace(" ", "+");
            byte[] bytesBuff = Convert.FromBase64String(password);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new(key,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using MemoryStream mStream = new();
                using (CryptoStream cStream = new(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cStream.Write(bytesBuff, 0, bytesBuff.Length);
                    cStream.Close();
                }
                password = Encoding.Unicode.GetString(mStream.ToArray());
            }
            return password;
        }

        private string AccessToken(string resultEmail, long resultId, string UserName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JwtOptions:SecurityKey"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration.GetValue<string>("JwtOptions:Issuer"),
                Audience = _configuration.GetValue<string>("JwtOptions:Audience"),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, resultEmail),
                    new Claim(ClaimTypes.NameIdentifier, $"{resultId}"),
                    new Claim("UserName", JsonConvert.SerializeObject(UserName))
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            // UserName.ForEach(x => tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.UserName, x)));
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<ServiceResponse<LoginResponse>> Login(LoginModel model)
        {
            var result = await _context.User.FirstOrDefaultAsync(x => x.Email == model.Email || x.MobileNumber == model.MobileNumber);

            if (result == null)
            {
                return new ServiceResponse<LoginResponse>(400, "User Not Found", null);
            }

            if (Decipher(result.Password) == model.Password)
            {
                LoginResponse response = new()
                {
                    UserName = result.UserName,
                    AccessToken = AccessToken(result.Email, result.Id, result.UserName),
                    ExpiresIn = 43200000,
                    TokenType = "bearer",
                    UserId = result.Id,
                };

                return new ServiceResponse<LoginResponse>(200, "Login Successful", response);
            }
            else
            {
                return new ServiceResponse<LoginResponse>(400, "Invalid Password", null);
            }
        }

        public async Task<ApiResponse> ForgotPassword(ForgotPasswordModel model)
        {
            var result = await _context.User.FirstOrDefaultAsync(x => x.Email == model.Email || x.MobileNumber == model.MobileNumber);
            if (result != null)
            {
                PasswordResetToken token = new()
                {
                    Token = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    ExpiryDate = DateTime.UtcNow.AddHours(3),
                    CreatedDate = DateTime.UtcNow,
                };
                await _context.PasswordResetToken.AddAsync(token);
                await _context.SaveChangesAsync();

                string tokenValue = token.Token;

                await _emailServices.ForgotPasswordEmail(model.Email, tokenValue);

                return new ApiResponse("Login link sent to the user's email", 200);
            }
            else
            {
                return new ApiResponse("User not found", 404);
            }
        }
        public async Task<ApiResponse> ResetPassword(ResetPasswordModel model)
        {
            var result = await _context.User.Where(x => x.Email == model.Email).FirstOrDefaultAsync();
            if (result != null)
            {
                var prt = await _context.PasswordResetToken.Where(x => x.Email == model.Email && x.Token == model.Token).FirstOrDefaultAsync();
                if (prt != null)
                {
                    result.Password = Encipher(model.Password);
                    _context.Update(result);
                    _context.Remove(prt);
                    await _context.SaveChangesAsync();
                    return new ApiResponse(ResponseConstents.Constants.SuccessResponse, 200);
                }
            }
            return new ApiResponse("Wrong reset link", 404);
        }

        public async Task<ServiceResponse<object>> FollowersUsers()
        {
            var result = await _context.UserRole.Where(s => s.RoleId == 1).Select(s => s.UserId).ToListAsync();
            if (result.Count > 0)
            {
                var users = await _context.User.Include(x => x.UserRole).ThenInclude(x => x.Role).Where(s => result.Contains(s.Id)).Select(s => new UserDetailsModel
                {
                    MobileNumber = s.MobileNumber,
                    Name = s.UserName,
                    Email = s.Email,
                    UserRoleName = s.UserRole.Select(x => x.Role.Name).FirstOrDefault(),
                    RoleId = s.UserRole.Select(s => s.RoleId).FirstOrDefault(),
                }).ToListAsync();

                return new ServiceResponse<object>(200, "Followers Found", users);
            }
            else
            {
                return new ServiceResponse<object>(404, "Followers Not Found", null);
            }
        }
        public async Task<ServiceResponse<object>> FollowingUsers()
        {
            var result = await _context.UserRole.Where(s => s.RoleId == 2).Select(s => s.UserId).ToListAsync();
            if (result.Count > 0)
            {
                var users = await _context.User.Include(x => x.UserRole).ThenInclude(x => x.Role).Where(s => result.Contains(s.Id)).Select(s => new UserDetailsModel
                {
                    MobileNumber = s.MobileNumber,
                    Name = s.UserName,
                    Email = s.Email,
                    UserRoleName = s.UserRole.Select(x => x.Role.Name).FirstOrDefault(),
                    RoleId = s.UserRole.Select(s => s.RoleId).FirstOrDefault(),
                }).ToListAsync();

                return new ServiceResponse<object>(200, "Following Found", users);
            }
            else
            {
                return new ServiceResponse<object>(404, "Following Not Found", null);
            }
        }

        public async Task<ServiceResponse<object>> TotalNumberOfFollowing()
        {
            var result = await _context.UserRole.Where(s => s.RoleId == 2).Select(s => s.UserId).ToListAsync();
            if (result.Count > 0)
            {
                int followingCount = await _context.User
                    .Where(s => result.Contains(s.Id))
                    .CountAsync();

                return new ServiceResponse<object>(200, "Following Count Found", new { FollowingCount = followingCount });
            }
            else
            {
                return new ServiceResponse<object>(404, "Following Not Found", null);
            }
        }
        public async Task<ServiceResponse<object>> TotalNumberOfFollowers()
        {
            var result = await _context.UserRole.Where(s => s.RoleId == 1).Select(s => s.UserId).ToListAsync();
            if (result.Count > 0)
            {
                int followingCount = await _context.User
                    .Where(s => result.Contains(s.Id))
                    .CountAsync();

                return new ServiceResponse<object>(200, "Following Count Found", new { FollowersCount = followingCount });
            }
            else
            {
                return new ServiceResponse<object>(404, "Following Not Found", null);
            }
        }

        public async Task<ServiceResponse<object>> GetFollowingDetails()
        {
            var result = await _context.UserRole.Where(s => s.RoleId == 2).Select(s => s.UserId).ToListAsync();
            if (result.Count > 0)
            {
                int followingCount = await _context.User
                    .Where(s => result.Contains(s.Id))
                    .CountAsync();

                var users = await _context.User.Include(x => x.UserRole).ThenInclude(x => x.Role).Where(s => result.Contains(s.Id)).Select(s => new UserDetailsModel
                {
                    MobileNumber = s.MobileNumber,
                    Name = s.UserName,
                    Email = s.Email,
                    UserRoleName = s.UserRole.Select(x => x.Role.Name).FirstOrDefault(),
                    RoleId = s.UserRole.Select(s => s.RoleId).FirstOrDefault(),
                }).ToListAsync();

                var response = new
                {
                    FollowingCount = followingCount,
                    Users = users
                };

                return new ServiceResponse<object>(200, "Following Found", response);
            }
            else
            {
                return new ServiceResponse<object>(404, "Following Not Found", null);
            }
        }
        public async Task<ServiceResponse<object>> GetFollowersDetails()
        {
            var result = await _context.UserRole.Where(s => s.RoleId == 1).Select(s => s.UserId).ToListAsync();
            if (result.Count > 0)
            {
                int followingCount = await _context.User
                    .Where(s => result.Contains(s.Id))
                    .CountAsync();

                var users = await _context.User.Include(x => x.UserRole).ThenInclude(x => x.Role).Where(s => result.Contains(s.Id)).Select(s => new UserDetailsModel
                {
                    MobileNumber = s.MobileNumber,
                    Name = s.UserName,
                    Email = s.Email,
                    UserRoleName = s.UserRole.Select(x => x.Role.Name).FirstOrDefault(),
                    RoleId = s.UserRole.Select(s => s.RoleId).FirstOrDefault(),
                }).ToListAsync();

                var response = new
                {
                    FollowingCount = followingCount,
                    Users = users
                };

                return new ServiceResponse<object>(200, "Followers Found", response);
            }
            else
            {
                return new ServiceResponse<object>(404, "Followers Not Found", null);
            }
        }

        public async Task<ApiResponse> ProfilePictureUpload([FromForm] UserMediaProfileModel model)
        {
            var profile = await _context.UserMedia.Where(x => x.UserId == _currentUser.UserID).FirstOrDefaultAsync();
            if (profile is not null)
                return new("Already Profile picture uploaded", 400);

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await model.ProfileImage.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            string base64Image = Convert.ToBase64String(imageBytes);

            UserMedia add = new()
            {
                MediaUrl = base64Image,
                Type = (int)MediaTypes.ProfilePicture,
                TypeName = GetMediaTypes.GetTypeName((int)MediaTypes.ProfilePicture),
                CreatedDate = DateTime.UtcNow,
                UserId = _currentUser.UserID,
                Caption = model.Caption
            };
            _context.UserMedia.Add(add);
            await _context.SaveChangesAsync();

            return new("ProfilePicture Uploaded Successully", 200);
        }

        public async Task<ServiceResponse<object>> Follow(int followerId)
        {
           
            var relationshipExists = await _context.Followers.AnyAsync(f => f.UserId == _currentUser.UserID && f.FollowersId == followerId);

            if (relationshipExists)
            {
                return new ServiceResponse<object>(400, "You are already following this user", null);
            }

            var follower = new Followers
            {
                UserId = _currentUser.UserID,
                FollowersId = followerId,
            };

            _context.Followers.Add(follower);
            await _context.SaveChangesAsync();

            return new ServiceResponse<object>(200, "You are now following the user", null);
        }

        public async Task<ServiceResponse<object>> Unfollow(int followerId)
        {
            var existingFollower = await _context.Followers
                .SingleOrDefaultAsync(f => f.UserId == _currentUser.UserID && f.FollowersId == followerId);

            if (existingFollower == null)
            {
                return new ServiceResponse<object>(400, "You are not following this user", null);
            }

            _context.Followers.Remove(existingFollower);
            await _context.SaveChangesAsync();

            return new ServiceResponse<object>(200, "You have unfollowed the user", null);
        }


        public async Task<ApiResponse> PostUpload([FromForm] UserMediaPostModel model)
        {
            var profile = await _context.UserMedia.Where(x => x.UserId == _currentUser.UserID).FirstOrDefaultAsync();


            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await model.UserPost.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            string base64Image = Convert.ToBase64String(imageBytes);

            UserMedia add = new()
            {
                MediaUrl = base64Image,
                Type = (int)MediaTypes.UserPosts,
                TypeName = GetMediaTypes.GetTypeName((int)MediaTypes.UserPosts),
                CreatedDate = DateTime.UtcNow,
                UserId = _currentUser.UserID,
                Caption = model.Caption,
                
            };
            _context.UserMedia.Add(add);
            await _context.SaveChangesAsync();

            return new("ProfilePicture Uploaded Successully", 200);
        }
        public async Task<ApiResponse> UploadOrEditProfilePicture([FromForm] UserMediaProfileModel model)
        {
            var existingProfile = await _context.UserMedia
                .Where(x => x.UserId == _currentUser.UserID && x.Type == (int)MediaTypes.ProfilePicture)
                .FirstOrDefaultAsync();

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await model.ProfileImage.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            string base64Image = Convert.ToBase64String(imageBytes);

            if (existingProfile is not null)
            {
              
                existingProfile.MediaUrl = base64Image;
                existingProfile.UpdatedDate = DateTime.UtcNow;
                _context.UserMedia.Update(existingProfile);

                await _context.SaveChangesAsync();

                return new("Profile Picture Updated Successfully", 200);
            }
            else
            {
                
                UserMedia add = new()
                {
                    MediaUrl = base64Image,
                    Type = (int)MediaTypes.ProfilePicture,
                    TypeName = GetMediaTypes.GetTypeName((int)MediaTypes.ProfilePicture),
                    CreatedDate = DateTime.UtcNow,
                    UserId = _currentUser.UserID,
                    Caption = model.Caption,
                };
                _context.UserMedia.Add(add);

                await _context.SaveChangesAsync();

                return new("Profile Picture Uploaded Successfully", 200);
            }
        }

        public async Task<List<User>> GetFollowers(int userId)
        {
            var followers = await _context.Followers
                .Where(f => f.FollowersId == userId)
                .Select(f => f.User)
                .ToListAsync();

            return followers;
        }
        public async Task<List<User>> GetFollowing(int userId)
        {
            var following = await _context.Followers
                .Where(f => f.UserId == userId)
                .Select(f => f.UserFollowers)
                .ToListAsync();

            return following;
        }

        public async Task<FollowersFollowingCountsModel> GetFollowersandFollowingCounts(int userId)
        {
            var followers = await GetFollowers(userId);
            var following = await GetFollowing(userId);

            int followersCount = followers.Count;
            int followingCount = following.Count;

            var counts = new FollowersFollowingCountsModel
            {
                FollowersCount = followersCount,
                FollowingCount = followingCount
            };

            return counts;
        }

        public async Task<int> GetPost(int userId)
        {

            var counts = await _context.UserMedia.Where(u => u.UserId == userId && u.Type == 2).CountAsync();

            return counts;
        }

        public async Task<int> GetFollowersCount(int userId)
        {
            var counts = await _context.Followers.Where(u => u.FollowersId == userId).CountAsync();
            return counts;
        }

        public async Task<int> GetFollowingCount(int userId)
        {
            var counts = await _context.Followers.Where(u => u.UserId == userId).CountAsync();
            return counts;
        }

        public async Task<dynamic> GetFollowersAndFollowingCounts(int userId)
        {
            var followers = await GetFollowersCount(userId);
            var following = await GetFollowingCount(userId);

            var counts = new
            {
                FollowersCount = followers,
                FollowingCount = following
            };

            return counts;
        }


    }
}



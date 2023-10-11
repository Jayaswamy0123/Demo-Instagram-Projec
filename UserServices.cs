
using System.Security.Claims;
using Instagram.Repository.Iservices;

namespace Instagram.Repository.Services
{
    public class UserServices : IUserServices
    {
        private readonly IHttpContextAccessor _httpContext;
        public UserServices(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public int UserID => int.Parse(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        public string Email => _httpContext.HttpContext.User.FindFirst(ClaimTypes.Email).Value;

        public string Name => _httpContext.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

    }
}

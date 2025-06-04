using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;



namespace ICS_Tickets_Tools.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public UserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public string GetUserId()
        {
            return _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string UserName()
        {
            return _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.Email);
        }
        public string AdminAuth()
        {
            return _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.Role);
        }
        public bool IsAuthenticated()
        {
            return _httpContext.HttpContext.User.Identity.IsAuthenticated;
        }

        //public bool EmailVerified()
        //{
        //    return _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.Email);
        //}
    }
}




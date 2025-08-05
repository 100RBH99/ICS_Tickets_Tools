using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ICS_Tickets_Tools.Helpers
{
    public static class IpAddressHelper
    {
        public static string GetClientIp(HttpContext context)
        {
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
            {
                return forwarded.Split(',')[0];
            }

            // Fallback to connection
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }

}

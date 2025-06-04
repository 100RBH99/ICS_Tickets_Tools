using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICS_Tickets_Tools.Services
{
    public interface IUserService
    {
        string GetUserId();

        bool IsAuthenticated();
        string UserName();
        string AdminAuth();

      


    }
}

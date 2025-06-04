using ICS_Tickets_Tools.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ICS_Tickets_Tools.Models;

namespace ICS_Tickets_Tools.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
    //    Task<SignInResult> LoginAsync(LoginViewModel model);

		Task<(SignInResult result, string role)> LoginAsync(LoginViewModel model);

		Task LogoutAsync();
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model);

        Task<bool> UpdatePasswordAsync(ApplicationUser user, string newPassword);
        Task<ApplicationUser> FindByEmployeeIDAsync(string EmployeeName);
    }
}

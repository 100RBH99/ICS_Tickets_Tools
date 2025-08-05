using ICS_Tickets_Tools.DB_Context;
using ICS_Tickets_Tools.Models;
using ICS_Tickets_Tools.Repositories.Interfaces;
using ICS_Tickets_Tools.Services;
using ICS_Tickets_Tools.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ICS_Tickets_Tools.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IUserService _userService;
		private readonly TicketsDBContext _context;

		public AccountRepository(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager, IUserService userService , TicketsDBContext context)
		{
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userService = userService;
			_context = context;
		}

		public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
		{
			var user = new ApplicationUser()
			{
				UserName = model.EmployeeName,
				Email = model.EmployeeName,
				EmployeeCode = model.EmployeeCode,
				Department = model.Department,
				Designation = model.Designation,
				PhoneNumber = model.PhoneNumber
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				// Check and create role if it does not exist
				if (!await _roleManager.RoleExistsAsync(model.Role))
				{
					await _roleManager.CreateAsync(new IdentityRole(model.Role));
				}

				// Assign selected role to user
				await _userManager.AddToRoleAsync(user, model.Role);
			}

			return result;
		}


		public async Task<(SignInResult, string)> LoginAsync(LoginViewModel model)
		{
			var result = await _signInManager.PasswordSignInAsync(model.EmployeeName, model.Password,model.RememberMe , true);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.EmployeeName);
                var roles = await _userManager.GetRolesAsync(user);
				return (result, roles.FirstOrDefault());
			}
			return (result, null);
		}

		public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var user = await _userManager.GetUserAsync(model.HttpContextUser);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
                await _signInManager.RefreshSignInAsync(user);

            return result;
        }

		public async Task<ApplicationUser> FindByEmployeeIDAsync(string EmployeeName)
		{
			// Implement logic to find user by EmployeeID
			var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == EmployeeName);

			return user;
		}

		public async Task<bool> UpdatePasswordAsync(ApplicationUser user, string newPassword)
		{
			// Implement logic to update user password
			var GeneratePasswordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

			var result = await _userManager.ResetPasswordAsync(user, GeneratePasswordResetToken, newPassword);

			return result.Succeeded;
		}
	}
}

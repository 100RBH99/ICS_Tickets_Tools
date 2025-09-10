using ICS_Tickets_Tools.ViewModels;
using ICS_Tickets_Tools.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ICS_Tickets_Tools.Models;
using ICS_Tickets_Tools.Services;
using ICS_Tickets_Tools.Repositories;

namespace ICS_Tickets_Tools.Controllers
{
    public class AccountController : Controller//rinkuuuuyddf
    {
        private readonly IAccountRepository _accountRepo;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserService _userServices;
        private readonly GoogleReCaptchaService _captchaService;

        public AccountController(IAccountRepository accountRepo, RoleManager<IdentityRole> roleManager , IUserService userServices , UserManager<ApplicationUser> userManager , GoogleReCaptchaService captchaService)
        {
            _accountRepo = accountRepo;
			_roleManager = roleManager;
			_userServices = userServices;
			_userManager = userManager;
            _captchaService = captchaService;
		}

        [Authorize(Roles = "Admin")]     
        public  async Task<IActionResult>  Register()
        {			
			var roles = await _roleManager.Roles.ToListAsync();
			ViewBag.Roles = new SelectList(roles, "Name", "Name");			
			return View();
		}

        [Authorize(Roles = "Admin")]
        [HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
            string getData = TempData["TransferData"] as string;
            var roles = await _roleManager.Roles.ToListAsync();
			ViewBag.Roles = new SelectList(roles, "Name", "Name");

			try
			{
				if (ModelState.IsValid)
				{
					var result = await _accountRepo.RegisterAsync(model);
					if (!result.Succeeded)
					{
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError("", error.Description);
						}
						return View(model);
					}
                    ModelState.Clear();
                    TempData["TransferData"] = "User registered successfully!";
                    ViewBag.Message = TempData["TransferData"];
                }
			}
			catch (Exception ex)
			{
				TempData["TransferData"] = $"Registration failed: {ex.Message}";
                ViewBag.PasswordResetAlert = getData;
            }

			return View(model);
		}
	
		public async Task<IActionResult> Login()
		{
			var roles = await _roleManager.Roles.ToListAsync();
		//	ViewBag.Roles = new SelectList(roles, "Name", "Name");
			return View();
		}
    
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)     
        {
			try
            {                
                if (ModelState.IsValid)
                {
                    var isCaptchaValid = await _captchaService.VerifyToken(model.RecaptchaToken);

                    if (!isCaptchaValid)
                    {
                        ModelState.AddModelError("", "reCAPTCHA validation failed. Please try again.");
                        return View(model);
                    }

                    var (result, role) = await _accountRepo.LoginAsync(model);
					if (result.Succeeded)
                    {
						if (!string.IsNullOrEmpty(returnUrl))
						{
							return LocalRedirect(returnUrl);
						}
						if (role == "Admin")
                            return RedirectToAction("Index", "Home");
                        if (role == "User")
                            return RedirectToAction("Index", "Home");
						if (role == "ITEngineer")
							//return RedirectToAction("AssignTicketsList", "AssignTickets", new { status = "Open" });
							return RedirectToAction("Index", "Home");

						return RedirectToAction("Login");
                    }
                    else
                    {
						ModelState.AddModelError("", "Invalid login attempt");
					}
				}
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Login failed: {ex.Message}");
            }

            return View(model);
        }

        public async Task<IActionResult> ChangePassword()
        {
            string getData = TempData["TransferData"] as string;

            // Use ViewBag or ViewData to pass data to the view
            ViewBag.PasswordResetAlert = getData;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            model.HttpContextUser = User;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountRepo.ChangePasswordAsync(model);

            if (result.Succeeded)
            {
                TempData["TransferData"] = "Change Password updated successfully!!";
                return RedirectToAction("ChangePassword");
            }

            // If failed, display errors
            TempData["TransferData"] = "Change Password not updated!!";

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        [Authorize(Roles = "Admin")]
		public async Task<IActionResult>  ResetPassword()
		{
			string getData = TempData["TransferData"] as string;

			// Use ViewBag or ViewData to pass data to the view
			ViewBag.PasswordResetAlert = getData;
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> ResetPassword(RegisterViewModel model)
		{
			var user = await _accountRepo.FindByEmployeeIDAsync(model.EmployeeName);

			if (user == null)
			{
				// Handle user not found
				return RedirectToAction("ResetPassword");
			}

			var passwordUpdateResult = await _accountRepo.UpdatePasswordAsync(user, model.ConfirmPassword);

			if (passwordUpdateResult)
				TempData["TransferData"] = "Password updated successfully!!";
			else
				TempData["TransferData"] = "Password not updated!!";

			return RedirectToAction("ResetPassword");
		}

		[Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _accountRepo.LogoutAsync();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                TempData["LogoutError"] = $"Logout failed: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}

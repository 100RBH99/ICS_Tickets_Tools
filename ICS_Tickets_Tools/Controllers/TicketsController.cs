using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using ICS_Tickets_Tools.DB_Context;
using ICS_Tickets_Tools.Helpers;
using ICS_Tickets_Tools.Hubs;
using ICS_Tickets_Tools.Models;
using ICS_Tickets_Tools.Repositories;
using ICS_Tickets_Tools.Services;
using ICS_Tickets_Tools.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

 
namespace ICS_Tickets_Tools.Controllers //vvvvvghghhgghiiojijiirgfgfgjvhg
{
	[Authorize]
	public class TicketsController : Controller
    {
        private readonly ITicketsRepository _repository;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHubContext<TicketsHub> _hubContext;
        private readonly LocationService _locationService;


        public TicketsController(ITicketsRepository repository , IUserService userService, UserManager<ApplicationUser> userManager , 
            RoleManager<IdentityRole> roleManager , IHubContext<TicketsHub> hubContext, LocationService locationService)
        {
            _repository = repository;
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
            _hubContext = hubContext;
            _locationService = locationService;
        }

        public async Task<IActionResult> TicketsIndex(string year)
        {
            var UserName = _userService.UserName();           
            // var sortedTickets = allTickets.OrderByDescending(t => t.CreatedDate).ToList();
            if (year == null)
            {
                var currentYear = DateTime.Now.Month <= 3 ? DateTime.Now.AddYears(-1).ToString("yyyy") : DateTime.Now.ToString("yyyy");
                var nextYear = DateTime.Now.Month >= 4 ? DateTime.Now.AddYears(1).ToString("yy") : DateTime.Now.ToString("yy");
                year = currentYear + "-" + nextYear;
            }
            var allTickets = await _repository.GetAllAsync(year);
            ViewBag.finacialyear = year;
            return View(allTickets);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _repository.GetAllCategoriesAsync();
            var subCategories = await _repository.GetAllSubCategoriesAsync();

            ViewBag.Category_Tbl = new SelectList(categories, "CategoryId", "CategoryName");
            ViewBag.SubCategory_Tbl = new SelectList(subCategories, "SubCategoryId", "SubCategoryName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tickets tickets, List<IFormFile> Files)
        {
            try
            {
                tickets.Status = "Open";
                tickets.CreatedDate = DateTime.Now.Date;

                var publicIp = await _locationService.GetClientPublicIpAsync();
                var location = await _locationService.GetLocationAsync(publicIp);
                tickets.Location = location;
                

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TicketIssueImage");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var fileNames = new List<string>();

                foreach (var file in Files)
                {
                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (file.Length > 1 * 1024 * 1024 || !allowedExts.Contains(ext))
                    {
                        ModelState.AddModelError("FileUpload", $"Please upload image or PDF only (max 1MB): {file.FileName}");
                        ViewBag.Category_Tbl = new SelectList(await _repository.GetAllCategoriesAsync(), "CategoryId", "CategoryName");
                        ViewBag.SubCategory_Tbl = new SelectList(await _repository.GetAllSubCategoriesAsync(), "SubCategoryId", "SubCategoryName");
                        return View(tickets);
                    }

                    var uniqueName = Guid.NewGuid() + ext;
                    var filePath = Path.Combine(uploadsFolder, uniqueName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                    fileNames.Add(uniqueName);
                }

                tickets.Image = string.Join(",", fileNames);
                tickets.TicketNo = await _repository.GenerateTicketNoAsync();

                await _repository.Add(tickets);

                return RedirectToAction("TicketsIndex");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the ticket: " + ex.Message);           
                return View(tickets);
            }
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _repository.GetById(id);  // Correct async call
            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.Category_Tbl = new SelectList(await _repository.GetAllCategoriesAsync(), "Id", "CategoryName", ticket.CategoryId);
            ViewBag.SubCategory_Tbl = new SelectList(await _repository.GetSubCategoriesByCategoryIdAsync(ticket.CategoryId), "Id", "SubCategoryName", ticket.SubCategoryId);

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tickets formTicket, List<IFormFile> Files)
        {
            if (id != formTicket.Id)
                return NotFound();
            
                try
                {
                    // Load the existing ticket from DB (this will be tracked)
                    var existingTicket = await _repository.GetById(id);
                    if (existingTicket == null)
                        return NotFound();

                    // Update only required fields
                    existingTicket.TicketNo = formTicket.TicketNo;
                    existingTicket.TicketDescription = formTicket.TicketDescription;
                    existingTicket.CategoryId = formTicket.CategoryId;
                    existingTicket.SubCategoryId = formTicket.SubCategoryId;
                    existingTicket.Status = "Open";
                    existingTicket.UpdatedDate = DateTime.Now;

                    if (Files != null && Files.Count > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TicketIssueImage");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        List<string> fileNames = new List<string>();

                        foreach (var file in Files)
                        {
                            if (file != null && file.Length > 0)
                            {
                                if (file.Length > 1 * 1024 * 1024)
                                {
                                    ModelState.AddModelError("FileUpload", $"Please upload image or PDF only (max 1MB): {file.FileName}");
                                    ViewBag.Category_Tbl = new SelectList(await _repository.GetAllCategoriesAsync(), "CategoryId", "CategoryName");
                                    ViewBag.SubCategory_Tbl = new SelectList(await _repository.GetAllSubCategoriesAsync(), "SubCategoryId", "SubCategoryName");
                                    return View(formTicket);
                                }
                                string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                var filePath = Path.Combine(uploadsFolder, uniqueName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                fileNames.Add(uniqueName);
                            }
                        }

                        // Assign new file names
                        existingTicket.Image = string.Join(",", fileNames);
                    }
                    // Else, keep existingTicket.Image as is

                    await _repository.Update(existingTicket); 
                 
                    return RedirectToAction(nameof(TicketsIndex));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating data: " + ex.Message);
                }
            
            ViewBag.Category_Tbl = new SelectList(await _repository.GetAllCategoriesAsync(), "Id", "CategoryName", formTicket.CategoryId);
            ViewBag.SubCategory_Tbl = new SelectList(await _repository.GetAllSubCategoriesAsync(), "Id", "SubCategoryName", formTicket.SubCategoryId);

            return View(formTicket);
        }
    
        [HttpGet]
        public async Task<JsonResult> GetRolesByDepartment(int categoryId)
        {
            var subcategories = await _repository.GetSubCategoriesByCategoryIdAsync(categoryId);
            var result = subcategories.Select(r => new { r.SubCategoryId, r.SubCategoryName }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> AssignTicket(int id)
        {

            var roleName = "ITEngineer";
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return BadRequest("Role not found");
            }

            // Step 2: Get all users in the "ITEngineer" role
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            // Step 3: Select only username (or email if needed)
            var userList = usersInRole.Select(u => new { Name = u.UserName }).ToList();

            ViewBag.AssignedTos = new SelectList(userList, "Name", "Name");

            var ticket = await _repository.GetById(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var model = new AssignTicketViewModel
            {
                Id = ticket.Id,
                TicketNo = ticket.TicketNo,
                Priority = ticket.Priority,
                AssignedTo = ticket.AssignedTo,
                Status = ticket.Status,
                AssignedRemark = ticket.AssignedRemark
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTicket(AssignTicketViewModel model)
        {
			var CreatedBy = _userService.UserName();         
            try
            {               
                    var ticket = await _repository.GetById(model.Id);
                    if (ticket == null)
                    {
                        return NotFound();
                    }

                    ticket.Priority = model.Priority;
                    ticket.AssignedTo = model.AssignedTo;
                    ticket.AssignedBy = CreatedBy;
                    ticket.Status = model.Status;
                    ticket.AssignedRemark = model.AssignedRemark;
                    ticket.AssignedDate = DateTime.Now;

                   await _repository.Update(ticket);
                    
                   return RedirectToAction("TicketsIndex");
                
            }
            catch (Exception ex)
            {
                // Log exception here if needed (e.g., _logger.LogError(ex, "Error assigning ticket"));

                ModelState.AddModelError(string.Empty, "An unexpected error occurred while assigning the ticket. Please try again.");
                return View(model);
            }
        }

        #region datewise detail & yearwise

        public async Task<IActionResult> AllDateWiseTickets(DateTime fromdate, DateTime todate, int? engineername)
        {
            try
            {
                var role = _userService.AdminAuth();
                var ermail = _userService.UserName();
                var ersalesdata = await _repository.GetAllDateWiseTickets();
                ViewBag.fromdate = fromdate;
                ViewBag.todate = todate;
                ViewData["toDate"] = todate;
                ViewBag.engineername = engineername;
                if (engineername == null)
                {
                    if (role == "Engineer")
                    {
                        var data = ersalesdata.Where(x => (x.CreatedDate != null) && (x.CreatedDate >= fromdate) && (x.CreatedDate <= todate) && (x.EmpName == ermail)).ToList();

                        return PartialView("TicketsIndex", data);
                    }
                    else
                    {
                        var data = ersalesdata.Where(x => (x.CreatedDate != null) && (x.CreatedDate >= fromdate) && (x.CreatedDate <= todate)).ToList();

                        return PartialView("TicketsIndex", data);
                    }

                }
                else
                {
                    var data = ersalesdata.Where(x => (x.CreatedDate != null) && (x.CreatedDate >= fromdate) && (x.CreatedDate <= todate)).ToList();
                    return PartialView("TicketsIndex", data);
                }

                // return PartialView("SalesRecord", data); 
            }
            catch (Exception ex)
            {
                return null;
            }
        }
     
        #endregion
    }
}

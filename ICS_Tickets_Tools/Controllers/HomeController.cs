using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ICS_Tickets_Tools.DB_Context;
using ICS_Tickets_Tools.Models;
using ICS_Tickets_Tools.Repositories;
using ICS_Tickets_Tools.Services;
using ICS_Tickets_Tools.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ICS_Tickets_Tools.Controllers
{
    public class HomeController : Controller
    {
        private readonly TicketsDBContext _context;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(TicketsDBContext context, IUserService userService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {      
            _context = context;
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize]
        public async Task<IActionResult> Index(string assignedTo)
        {
            var role = _userService.AdminAuth();
            var userId = _userService.UserName();
            var today = DateTime.Today;

            var allTickets = _context.Tickets_Tbl.ToList();
            var allCategory = _context.Category_Tbl.ToList();

            // Dropdown data
            var assignedTos = allTickets
                .Where(t => !string.IsNullOrEmpty(t.AssignedTo))
                .Select(t => t.AssignedTo)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ViewBag.AssignedTos = new SelectList(assignedTos, assignedTo);

            // 🔁 Filter tickets if assignedTo selected
            if (!string.IsNullOrEmpty(assignedTo))
            {
                allTickets = allTickets.Where(t => t.AssignedTo == assignedTo).ToList();
            }

            // role wise filler data
            if (role == "User")
            {
                allTickets = allTickets.Where(t => t.EmpName == userId).ToList();
            }
            else if (role == "ITEngineer")
            {
                allTickets = allTickets.Where(t => t.AssignedTo == userId).ToList();
            }

            //var unresolvedTickets = allTickets.Where(t => t.Status != "Closed").ToList();
            var unresolvedTickets = allTickets.ToList();

            var targetCategories = new[] { "Hardware", "Software", "Other" };

            var newOpenTickets = from t in allTickets
                                 join c in allCategory on t.CategoryId equals c.CategoryId
                                 where targetCategories.Contains(c.CategoryName)
                                 select new
                                 {
                                     Ticket = t,
                                     CategoryName = c.CategoryName
                                 };

            var model = new DashboardViewModel
            {
                TotalTickets = allTickets.Count,
                OpenTickets = allTickets.Count(t => t.Status == "Open"),
                OnHoldTickets = allTickets.Count(t => t.Status == "Hold"),
                ClosedTickets = allTickets.Count(t => t.Status == "Closed"),
                UnassignedTickets = allTickets.Count(t => string.IsNullOrEmpty(t.AssignedTo)),
                TicketsToday = allTickets.Count(t => t.CreatedDate != null && t.CreatedDate.Date == today),

                UnresolvedByPriority = unresolvedTickets
                    .GroupBy(t => t.Priority ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count()),

                UnresolvedByStatus = allTickets
                    .GroupBy(t => t.Status ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count()),

                CategoryWiseTickets = newOpenTickets
                    .GroupBy(x => x.CategoryName)
                    .ToDictionary(g => g.Key, g => g.Count()),

                AssignedTo = assignedTo // 🔁 Add selected value to viewmodel
            };

            return View(model);
        }
        public async Task<IActionResult> allCountingShow(string Status)
        {
            var role = _userService.AdminAuth();
            var userId = _userService.UserName();
            var today = DateTime.Today;

            IQueryable<Tickets> query = from t in _context.Tickets_Tbl
                                        join c in _context.Category_Tbl on t.CategoryId equals c.CategoryId into categoryGroup
                                        from c in categoryGroup.DefaultIfEmpty()
                                        join sc in _context.SubCategory_Tbl on t.SubCategoryId equals sc.SubCategoryId into subCategoryGroup
                                        from sc in subCategoryGroup.DefaultIfEmpty()
                                        select new Tickets
                                        {
                                            Id = t.Id,
                                            TicketNo = t.TicketNo,
                                            EmpName = t.EmpName,
                                            TicketDescription = t.TicketDescription,
                                            CategoryName = c != null ? c.CategoryName : "No Category",
                                            SubCategoryName = sc != null ? sc.SubCategoryName : "No SubCategory",
                                            Status = t.Status,
                                            Image = t.Image,
                                            CreatedDate = t.CreatedDate,
                                            UpdatedDate = t.UpdatedDate,
                                            AssignedTo = t.AssignedTo ?? "N/A",
                                            AssignedBy = t.AssignedBy ?? "N/A",
                                            HoldDate = t.HoldDate,
                                            HoldRemark = t.HoldRemark,
                                            CloseingDate = t.CloseingDate,
                                            AssignedRemark = t.AssignedRemark ?? "N/A",
                                            AssignedDate = t.AssignedDate,
                                            ClosedRemark = t.ClosedRemark,
                                            Priority = t.Priority ?? "N/A",
                                            ClosedBy = t.ClosedBy
                                        };

            // Filter based on status
            if(role == "Admin")
            {
                if (Status == "AllTickets")
                {
                    ViewBag.StatusType = Status;
                    return View("~/Views/Tickets/TicketsIndex.cshtml", await query.ToListAsync());
                }
                if (Status == "TodayTickets")
                {
                    query = query.Where(t => t.CreatedDate != null && t.CreatedDate.Date == today);
                    ViewBag.StatusType = Status;
                    return View("~/Views/Tickets/TicketsIndex.cshtml", await query.ToListAsync());
                }
                if (Status == "Open")
                {
                    query = query.Where(t => t.Status == "Hold");
                    ViewBag.StatusType = Status;
                    ViewBag.StatusType2 = "HoldTickets";
                    return View("~/Views/AssignTickets/AssignTicketsList.cshtml", await query.ToListAsync());
                }
                query = query.Where(t => t.Status == "Open");
                ViewBag.StatusType = "Open";
                ViewBag.StatusType2 = Status;
                return View("~/Views/AssignTickets/AssignTicketsList.cshtml", await query.ToListAsync());
            }
            else if (role == "ITEngineer")
            {
                if (Status == "AllTickets")
                {
                    query = query.Where(t => t.AssignedTo == userId);
                    ViewBag.StatusType = Status;
                    return View("~/Views/Tickets/TicketsIndex.cshtml", await query.ToListAsync());
                }
                if (Status == "TodayTickets")
                {
                    query = query.Where(t => t.CreatedDate != null && t.CreatedDate.Date == today && t.AssignedTo == userId);
                    ViewBag.StatusType = Status;
                    return View("~/Views/Tickets/TicketsIndex.cshtml", await query.ToListAsync());
                }
                if (Status == "Open")
                {
                    query = query.Where(t => t.Status == "Hold" && t.AssignedTo == userId);
                    ViewBag.StatusType = Status;
                    ViewBag.StatusType2 = "HoldTickets";
                    return View("~/Views/AssignTickets/AssignTicketsList.cshtml", await query.ToListAsync());
                }
                query = query.Where(t => t.Status == "Open" && t.AssignedTo == userId);
                ViewBag.StatusType = "Open";
                ViewBag.StatusType2 = Status;
                return View("~/Views/AssignTickets/AssignTicketsList.cshtml", await query.ToListAsync());
            }
            else
            {
                if (Status == "AllTickets")
                {
                    query = query.Where(t => t.EmpName == userId);
                    ViewBag.StatusType = Status;
                    return View("~/Views/Tickets/TicketsIndex.cshtml", await query.ToListAsync());
                }
                if (Status == "TodayTickets")
                {
                    query = query.Where(t => t.CreatedDate != null && t.CreatedDate.Date == today && t.EmpName == userId);
                    ViewBag.StatusType = Status;
                    return View("~/Views/Tickets/TicketsIndex.cshtml", await query.ToListAsync());
                }
                if (Status == "Open")
                {
                    query = query.Where(t => t.Status == "Hold" && t.EmpName == userId);
                    ViewBag.StatusType = Status;
                    ViewBag.StatusType2 = "HoldTickets";
                    return View("~/Views/AssignTickets/AssignTicketsList.cshtml", await query.ToListAsync());
                }
                query = query.Where(t => t.Status == "Open" && t.EmpName == userId);
                ViewBag.StatusType = "Open";
                ViewBag.StatusType2 = Status;
                return View("~/Views/AssignTickets/AssignTicketsList.cshtml", await query.ToListAsync());
            }            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Home/StatusCode")]
        public async Task<IActionResult> StatusCode(int code)
        {
            if (code == 404)
            {
                return View("NotFound");
            }
            else if (code == 403)
            {
                return View("AccessDenied"); // Optional
            }

            ViewBag.StatusCode = code;
            return View("GeneralError");
        }

        #region AllPartialView

        public IActionResult TicketsFillterDate()
        {
            return PartialView("_FillterDate");
        }


        #endregion
    }
}

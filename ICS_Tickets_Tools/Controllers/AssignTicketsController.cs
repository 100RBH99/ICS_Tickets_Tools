using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	[Authorize]
	public class AssignTicketsController : Controller
    {
        private readonly IAssignTicketsRepository _repository;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;


        public AssignTicketsController(IAssignTicketsRepository repository, IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<IActionResult> AssignTicketsList(string Status)
        {
            var tickets = await _repository.GetTicketsByStatus(Status);
            ViewBag.StatusType = Status;
            ViewBag.Status = tickets;

            List<Tickets> sortedTickets;

            if (Status == "Open")
            {
                sortedTickets = tickets.OrderByDescending(t => t.AssignedDate).ToList();
            }
            else
            {
                sortedTickets = tickets.OrderByDescending(t => t.CloseingDate).ToList();
            }

            return View(sortedTickets);
        }

        public async Task<IActionResult> AssignTicket(int id)
        {
            var ticket = await _repository.GetById(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var model = new AssignTicketViewModel
            {
                Id = ticket.Id,
                TicketNo = ticket.TicketNo,               
                Status = ticket.Status,
                HoldRemark = ticket.HoldRemark,
                HoldDate = ticket.HoldDate?.Date,
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

                if (model.Status == "Hold" )
                {
                    ticket.Status = model.Status;
                    ticket.HoldRemark = model.HoldRemark;
                    ticket.HoldDate = model.HoldDate;
                }
                else
                {
                    ticket.Status = model.Status;
                    ticket.ClosedRemark = model.ClosedRemark;
                    ticket.CloseingDate = DateTime.Now;
                    ticket.ClosedBy = CreatedBy;
                }
              
               await _repository.Update(ticket);
               
               return RedirectToAction("AssignTicketsList", new { status = "Open" });
            }
            catch (Exception ex)
            {
                // Log exception here if needed (e.g., _logger.LogError(ex, "Error assigning ticket"));

                ModelState.AddModelError(string.Empty, "An unexpected error occurred while assigning the ticket. Please try again.");
                return View(model);
            }
        }
    }
}

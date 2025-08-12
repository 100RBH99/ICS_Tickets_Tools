using ICS_Tickets_Tools.DB_Context;
using ICS_Tickets_Tools.Hubs;
using ICS_Tickets_Tools.Models;
using ICS_Tickets_Tools.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICS_Tickets_Tools.Repositories
{
    public class TicketsRepository : ITicketsRepository
    {
        private readonly TicketsDBContext _context;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<TicketsHub> _hubContext;

        public TicketsRepository(TicketsDBContext context ,IUserService userService , UserManager<ApplicationUser> userManager, IHubContext<TicketsHub> hubContext)
        {
            _context = context;
            _userService = userService;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public async Task<List<Tickets>> GetAllAsync(string year)
        {
            var role = _userService.AdminAuth();
            var userId = _userService.UserName();

            if (role == "Admin" )
            {
                var query = from t in _context.Tickets_Tbl
                            join c in _context.Category_Tbl on t.CategoryId equals c.CategoryId into categoryGroup
                            from c in categoryGroup.DefaultIfEmpty()
                            join sc in _context.SubCategory_Tbl on t.SubCategoryId equals sc.SubCategoryId into subCategoryGroup
                            from sc in subCategoryGroup.DefaultIfEmpty()
                            where string.IsNullOrEmpty(t.AssignedTo) && t.Status != "Closed" && t.FinancialYear == year
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
                                AssignedTo = t.AssignedTo,
                                HoldDate = t.HoldDate,
                                HoldRemark = t.HoldRemark,
                            };

                return await query.ToListAsync();

            }
            else
            {
                var query = from t in _context.Tickets_Tbl
                            join c in _context.Category_Tbl on t.CategoryId equals c.CategoryId into categoryGroup
                            from c in categoryGroup.DefaultIfEmpty()
                            join sc in _context.SubCategory_Tbl on t.SubCategoryId equals sc.SubCategoryId into subCategoryGroup
                            from sc in subCategoryGroup.DefaultIfEmpty()
                            where t.Status != "Closed" && t.EmpName == userId && t.FinancialYear == year
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
                                AssignedTo = t.AssignedTo,
                                HoldDate = t.HoldDate,
                                HoldRemark = t.HoldRemark,
                            };

                return await query.ToListAsync();
            }
        }

        public async Task<List<Tickets>> GetAllDateWiseTickets()
        {
            var role = _userService.AdminAuth();
            var userId = _userService.UserName();

            if (role == "Admin")
            {
                var query = from t in _context.Tickets_Tbl
                            join c in _context.Category_Tbl on t.CategoryId equals c.CategoryId into categoryGroup
                            from c in categoryGroup.DefaultIfEmpty()
                            join sc in _context.SubCategory_Tbl on t.SubCategoryId equals sc.SubCategoryId into subCategoryGroup
                            from sc in subCategoryGroup.DefaultIfEmpty()
                            where string.IsNullOrEmpty(t.AssignedTo) && t.Status != "Closed"
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
                                AssignedTo = t.AssignedTo,
                                HoldDate = t.HoldDate,
                                HoldRemark = t.HoldRemark,
                            };

                return await query.ToListAsync();

            }
            else
            {
                var query = from t in _context.Tickets_Tbl
                            join c in _context.Category_Tbl on t.CategoryId equals c.CategoryId into categoryGroup
                            from c in categoryGroup.DefaultIfEmpty()
                            join sc in _context.SubCategory_Tbl on t.SubCategoryId equals sc.SubCategoryId into subCategoryGroup
                            from sc in subCategoryGroup.DefaultIfEmpty()
                            where t.Status != "Closed" && t.EmpName == userId
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
                                AssignedTo = t.AssignedTo,
                                HoldDate = t.HoldDate,
                                HoldRemark = t.HoldRemark,
                            };

                return await query.ToListAsync();
            }
        }

        public async Task<Tickets> GetById(int id)
        {
            return  _context.Tickets_Tbl.FirstOrDefault(t => t.Id == id);
        }

        public async Task Add(Tickets tickets)
        {
            var role = _userService.AdminAuth();
            var CreatedBy = _userService.UserName();
            tickets.EmpName = CreatedBy;

            _context.Tickets_Tbl.Add(tickets);
            await _context.SaveChangesAsync();

            //  await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", tickets.TicketNo);
        }

        public async Task Update(Tickets tickets)
        {
            _context.Tickets_Tbl.Update(tickets);
            await _context.SaveChangesAsync();

            // await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", tickets.TicketNo);
        }
      
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Category_Tbl.ToListAsync();
        }

        public async Task<List<SubCategory>> GetAllSubCategoriesAsync()
        {
            return await _context.SubCategory_Tbl.ToListAsync();
        }

        public async Task<List<SubCategory>> GetSubCategoriesByCategoryIdAsync(int categoryId)
        {
            return await _context.SubCategory_Tbl
                .Where(r => r.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<string> GenerateTicketNoAsync()//tetst
        {
            var lastTicket = await _context.Tickets_Tbl.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            int number = 1;

            if (lastTicket != null && !string.IsNullOrEmpty(lastTicket.TicketNo))
            {
                string digits = lastTicket.TicketNo.Substring(4); // Skip "ICST"
                int.TryParse(digits, out number);
                number++;
            }

            return "ICST" + number.ToString("D2"); // ICST01, ICST02 etc.
        }

    }
}

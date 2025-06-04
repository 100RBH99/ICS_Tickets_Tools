using ICS_Tickets_Tools.DB_Context;
using ICS_Tickets_Tools.Models;
using ICS_Tickets_Tools.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ICS_Tickets_Tools.Repositories
{
    public class AssignTicketsRepository : IAssignTicketsRepository
    {
        private readonly TicketsDBContext _context; 
        private readonly IUserService _userService;

        public AssignTicketsRepository(TicketsDBContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<List<Tickets>> GetTicketsByStatus(string status)
        {
            var role = _userService.AdminAuth();
            var userId = _userService.UserName();

            var ticketsQuery = _context.Tickets_Tbl.AsQueryable();

            // Apply status filters
            if (status == "Open")
            {
                ticketsQuery = ticketsQuery.Where(t => t.Status != "Closed");
            }
            else
            {
                ticketsQuery = ticketsQuery.Where(t => t.Status == "Closed");
            }

            // Apply role-based filters
            if (role == "Admin")
            {
                if (status == "Open")
                    ticketsQuery = ticketsQuery.Where(t => !string.IsNullOrEmpty(t.AssignedTo));
            }
            else if (role == "ITEngineer")
            {
                ticketsQuery = ticketsQuery.Where(t => t.AssignedTo == userId);
            }
            else // Regular user
            {
                ticketsQuery = ticketsQuery.Where(t => t.EmpName == userId && !string.IsNullOrEmpty(t.AssignedTo));
            }

            // Final projection with joins
            var query = from t in ticketsQuery
                        join c in _context.Category_Tbl on t.CategoryId equals c.CategoryId into categoryGroup
                        from c in categoryGroup.DefaultIfEmpty()
                        join sc in _context.SubCategory_Tbl on t.SubCategoryId equals sc.SubCategoryId into subCategoryGroup
                        from sc in subCategoryGroup.DefaultIfEmpty()
                        select new Tickets
                        {
                            Id = t.Id,
                            TicketNo = t.TicketNo,
                            AssignedTo = t.AssignedTo,
                            AssignedBy = t.AssignedBy,
                            TicketDescription = t.TicketDescription,
                            CategoryName = c != null ? c.CategoryName : "No Category",
                            SubCategoryName = sc != null ? sc.SubCategoryName : "No SubCategory",
                            Image = t.Image,
                            CreatedDate = t.CreatedDate,
                            UpdatedDate = t.UpdatedDate,
                            CloseingDate = t.CloseingDate,
                            AssignedRemark = t.AssignedRemark,
                            AssignedDate = t.AssignedDate,
                            ClosedRemark = t.ClosedRemark,
                            Priority = t.Priority,
                            Status = t.Status,
                            HoldDate = t.HoldDate,
                            HoldRemark = t.HoldRemark,
                            ClosedBy = t.ClosedBy,
                            EmpName = t.EmpName
                        };

            return await query.ToListAsync();
        }

        public async Task<Tickets> GetById(int id)
        {
            return _context.Tickets_Tbl.FirstOrDefault(t => t.Id == id);
        }

        public async Task Update(Tickets tickets)
        {
            _context.Tickets_Tbl.Update(tickets);
            await _context.SaveChangesAsync();
        }
    }
}

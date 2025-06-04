using ClosedXML.Excel;
using ICS_Tickets_Tools.Hubs;
using ICS_Tickets_Tools.Models;
using ICS_Tickets_Tools.Repositories;
using ICS_Tickets_Tools.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.IO;
using System.Threading.Tasks;


namespace ICS_Tickets_Tools.Controllers
{
    public class ExportController : Controller
    {
        private readonly ITicketsRepository _repository;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHubContext<TicketsHub> _hubContext;

        public ExportController(ITicketsRepository repository, IUserService userService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IHubContext<TicketsHub> hubContext)
        {
            _repository = repository;
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var tickets = await _repository.GetAllDateWiseTickets(); // Replace with your actual data source

            // Create DataTable
            var dt = new DataTable("Tickets");
            dt.Columns.AddRange(new DataColumn[]
            {
        new DataColumn("Ticket No"),
        new DataColumn("User Name"),
        new DataColumn("Description"),
        new DataColumn("Category"),
        new DataColumn("SubCategory"),
        new DataColumn("Status"),
        new DataColumn("Created Date"),
        new DataColumn("Updated Date")
            });

            foreach (var t in tickets)
            {
                dt.Rows.Add(
                    t.TicketNo,
                    t.EmpName,
                    t.TicketDescription,
                    t.CategoryName,
                    t.SubCategoryName,
                    t.Status,
                    t.CreatedDate.ToString("yyyy-MM-dd"),
                    t.UpdatedDate?.ToString("yyyy-MM-dd")
                );
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(dt);
                worksheet.Columns().AdjustToContents(); // Autofit columns

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "Tickets.xlsx");
                }
            }
        }
    }
}

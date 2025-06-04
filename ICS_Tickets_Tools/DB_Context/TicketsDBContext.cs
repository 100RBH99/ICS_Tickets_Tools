using ICS_Tickets_Tools.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace ICS_Tickets_Tools. DB_Context
{
    public class TicketsDBContext : IdentityDbContext<ApplicationUser>
    {
        public TicketsDBContext(DbContextOptions<TicketsDBContext> options) : base(options)
        {

        }

        public DbSet<Tickets> Tickets_Tbl { get; set; }
        public DbSet<Category> Category_Tbl { get; set; }
        public DbSet<SubCategory> SubCategory_Tbl { get; set; }
     //   public DbSet<AssignTickets> AssignTicket_Tbl { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional configurations if needed

        }
    }
}

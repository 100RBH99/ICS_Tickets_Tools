using ICS_Tickets_Tools.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICS_Tickets_Tools.Repositories
{
    public interface ITicketsRepository
    {
        Task<List<Tickets>> GetAllAsync(string year);
        Task<List<Tickets>> GetAllDateWiseTickets();
        Task<Tickets> GetById(int id);

        //Task<AssignTickets> GetByAssignId(int id);

        Task Add(Tickets tickets);
        Task Update(Tickets tickets);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<SubCategory>> GetAllSubCategoriesAsync();
        Task<List<SubCategory>> GetSubCategoriesByCategoryIdAsync(int categoryId);

        Task<string> GenerateTicketNoAsync();

    }
}

using ICS_Tickets_Tools.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ICS_Tickets_Tools.Repositories
{
    public interface IAssignTicketsRepository
    {

        //Task<List<Tickets>> GetAllAssign();
        //Task<List<Tickets>> GetAllClosed();
        Task<List<Tickets>> GetTicketsByStatus(string Status);

        Task<Tickets> GetById(int id);

        Task Update(Tickets tickets);

    }
}

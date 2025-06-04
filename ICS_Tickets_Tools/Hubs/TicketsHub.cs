using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ICS_Tickets_Tools.Hubs;

namespace ICS_Tickets_Tools.Hubs
{
    public class TicketsHub : Hub
    {
        public async Task SendUpdateData(string data)
        {
            await Clients.All.SendAsync("ReceiveUpdateData", data);
        }
    }
}


using System.Collections.Generic;

namespace ICS_Tickets_Tools.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int OnHoldTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int UnassignedTickets { get; set; }
        public int TicketsToday { get; set; }
        public string AssignedTo { get; set; }

        public Dictionary<string, int> UnresolvedByPriority { get; set; }
        public Dictionary<string, int> UnresolvedByStatus { get; set; }
        public Dictionary<string, int> CategoryWiseTickets { get; set; }
    }
}

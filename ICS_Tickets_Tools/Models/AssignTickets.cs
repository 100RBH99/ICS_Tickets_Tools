namespace ICS_Tickets_Tools.Models
{
    public class AssignTickets
    {
        public int Id { get; set; }
        public string TicketNo { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}

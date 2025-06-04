using System;
using System.ComponentModel.DataAnnotations;

namespace ICS_Tickets_Tools.ViewModels
{
    public class AssignTicketViewModel
    {
        public int Id { get; set; }
        public string TicketNo { get; set; }

        [Required(ErrorMessage = "Please Select Priority")]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Please Select AssignedTo")]
        public string AssignedTo { get; set; }

        [Required(ErrorMessage = "Please Enter Your AssignedRemark")]
        public string AssignedRemark { get; set; }
   
        [Required(ErrorMessage = "Please Enter Your ClosedRemark")]
        public string ClosedRemark { get; set; }
     
        [Required(ErrorMessage = "Please Enter  HoldRemark")]
        public string HoldRemark { get; set; }

        [Required(ErrorMessage = "Please Select  HoldDate")]
        public DateTime? HoldDate { get; set; }

        public string AssignedBy { get; set; }

        [Required(ErrorMessage = "Please Select  Status")]
        public string Status { get; set; }

        public DateTime CloseingDate { get; set; }

        public string ClosedBy { get; set; }
    }
}

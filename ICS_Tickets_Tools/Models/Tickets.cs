using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ICS_Tickets_Tools.Models
{
    public class Tickets
    {
        [Key]
        public int Id { get; set; }

        [Column("TicketNo", TypeName = "nvarchar(Max)")]
        public string TicketNo { get; set; }

        [Required(ErrorMessage = "Please Enter Ticket Description")]
        [Column("TicketDescription", TypeName = "nvarchar(Max)")]
        public string TicketDescription { get; set; }

        [Required(ErrorMessage = "Please select Category")]
        [Column("CategoryId")]
        public int CategoryId { get; set; }


        [Required(ErrorMessage = "Please select SubCategory")]
        [Column("SubCategoryId")]
        public int SubCategoryId { get; set; }


        [Column("Image", TypeName = "nvarchar(Max)")]
        public string Image { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }


        [Column("Status", TypeName = "nvarchar(Max)")]
        public string Status { get; set; }

        [NotMapped]
        public List<IFormFile> Files { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }

        [NotMapped]
        public string SubCategoryName { get; set; }

        public string Priority { get; set; }
        public string AssignedTo { get; set; }
		public string AssignedBy { get; set; }
		public string AssignedRemark { get; set; }
		public DateTime? AssignedDate { get; set; }

		public string ClosedRemark { get; set; }
		public string ClosedBy { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime?  CloseingDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string EmpName { get; set; }

        public DateTime? HoldDate { get; set; }
        public string HoldRemark { get; set; }

        [Required(ErrorMessage = "Please select FinancialYear")]
        [Column("FinancialYear", TypeName = "nvarchar(255)")]   
        public string FinancialYear { get; set; }


    }
}

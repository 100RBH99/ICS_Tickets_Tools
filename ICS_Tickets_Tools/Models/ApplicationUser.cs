using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICS_Tickets_Tools.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
     //   public DateTime DateOfBirth { get; set; }

      //  public bool IsActive { get; set; }
        //public string Username { get; set; }


    }
}

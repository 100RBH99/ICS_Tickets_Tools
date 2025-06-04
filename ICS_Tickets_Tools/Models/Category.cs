using System.ComponentModel.DataAnnotations;

namespace ICS_Tickets_Tools.Models
{
    public class Category
    {
        [Key]
        public int  Id { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace ICS_Tickets_Tools.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }
        public string SubCategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; } 

    }
}

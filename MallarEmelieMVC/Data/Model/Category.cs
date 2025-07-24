using System.ComponentModel.DataAnnotations;

namespace MallarEmelieMVC.Data.Model
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }
    }
}

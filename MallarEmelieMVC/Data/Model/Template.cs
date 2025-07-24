using System.ComponentModel.DataAnnotations;

namespace MallarEmelieMVC.Data.Model
{
    public class Template
    {
        public int TemplateId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }   // fk
        public Category? Category { get; set; }  // nav prop


    }
}

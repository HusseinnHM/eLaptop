using System.ComponentModel.DataAnnotations;

namespace eLaptop.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a company name")]
        public string Name { get; set; }
        
    }
}
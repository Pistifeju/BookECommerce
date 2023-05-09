using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookECommerce.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    [DisplayName("Category Name")]
    [Required]
    [MaxLength(30, ErrorMessage = "Category Name cannot be more than 30 characters")]
    [MinLength(1, ErrorMessage = "Category Name must be at least 1 character")]
    public string Name { get; set; }
    
    [DisplayName("Display Order")]
    [Range(1, 100, ErrorMessage = "Display Order for category must be between 1 and 100")]
    public int DisplayOrder { get; set; }
}
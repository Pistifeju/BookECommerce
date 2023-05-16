using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookECommerce.Models;

public class Company
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Company name is required.")]
    [StringLength(100, ErrorMessage = "Company name cannot be longer than 100 characters.")]
    [DisplayName("Company Name")]
    public string Name { get; set; }

    [StringLength(100, ErrorMessage = "Street address cannot be longer than 100 characters.")]
    [DisplayName("Street Address")]
    public string? StreetAddress { get; set; }

    [StringLength(50, ErrorMessage = "City cannot be longer than 50 characters.")]
    public string? City { get; set; }

    [StringLength(50, ErrorMessage = "State cannot be longer than 50 characters.")]
    public string? State { get; set; }

    [StringLength(10, ErrorMessage = "Postal code cannot be longer than 10 characters.")]
    [DisplayName("Postal Code")]
    public string? PostalCode { get; set; }

    [StringLength(15, ErrorMessage = "Phone number cannot be longer than 15 characters.")]
    [DisplayName("Phone Number")]
    public string? PhoneNumber { get; set; }
}
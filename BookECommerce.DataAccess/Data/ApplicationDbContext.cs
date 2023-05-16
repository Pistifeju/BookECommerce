using BookECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookECommerce.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
         
    }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    
    public DbSet<Company> Companies { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Company>().HasData(
            new Company { Id = 1, Name = "ABC Company", StreetAddress = "123 Main Street", City = "New York", State = "NY", PostalCode = "10001", PhoneNumber = "123-456-7890" },
            new Company { Id = 2, Name = "XYZ Corporation", StreetAddress = "456 Elm Street", City = "Los Angeles", State = "CA", PostalCode = "90001", PhoneNumber = "987-654-3210" },
            new Company { Id = 3, Name = "123 Industries", StreetAddress = "789 Oak Street", City = "Chicago", State = "IL", PostalCode = "60601", PhoneNumber = "555-123-4567" },
            new Company { Id = 4, Name = "Acme Enterprises", StreetAddress = "321 Pine Street", City = "San Francisco", State = "CA", PostalCode = "94101", PhoneNumber = "777-888-9999" },
            new Company { Id = 5, Name = "Global Solutions", StreetAddress = "654 Maple Street", City = "Seattle", State = "WA", PostalCode = "98101", PhoneNumber = "111-222-3333" },
            new Company { Id = 6, Name = "Tech Innovators", StreetAddress = "987 Cedar Street", City = "Austin", State = "TX", PostalCode = "78701", PhoneNumber = "444-555-6666" },
            new Company { Id = 7, Name = "Dynamic Ventures", StreetAddress = "741 Birch Street", City = "Boston", State = "MA", PostalCode = "02101", PhoneNumber = "999-888-7777" },
            new Company { Id = 8, Name = "Prime Industries", StreetAddress = "852 Walnut Street", City = "Miami", State = "FL", PostalCode = "33101", PhoneNumber = "222-333-4444" },
            new Company { Id = 9, Name = "Infinite Solutions", StreetAddress = "369 Pineapple Street", City = "Denver", State = "CO", PostalCode = "80201", PhoneNumber = "888-777-6666" },
            new Company { Id = 10, Name = "Smart Technologies", StreetAddress = "147 Orange Street", City = "Phoenix", State = "AZ", PostalCode = "85001", PhoneNumber = "333-444-5555" }
        );
        
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
            new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
            new Category { Id = 3, Name = "History", DisplayOrder = 3 }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Action-Packed Thriller",
                Description = "An exciting action-packed thriller that will keep you on the edge of your seat.",
                ISBN = "978-1-234567-89-0",
                Author = "John Doe",
                ListPrice = 19.99,
                Price = 14.99,
                Price50 = 12.99,
                Price100 = 9.99,
                CategoryId = 1,
                ImageUrl = "https://images-na.ssl-images-amazon.com/images/I/51Zymoq7UnL._SX258_BO1,204,203,200_.jpg"
            },
            new Product
            {
                Id = 2,
                Name = "Epic Sci-Fi Adventure",
                Description = "Embark on an epic sci-fi adventure in a vast universe filled with wonders and dangers.",
                ISBN = "978-9-876543-21-0",
                Author = "Jane Smith",
                ListPrice = 24.99,
                Price = 19.99,
                Price50 = 16.99,
                Price100 = 14.99,
                CategoryId = 2,
                ImageUrl = "https://images-na.ssl-images-amazon.com/images/I/51Zymoq7UnL._SX258_BO1,204,203,200_.jpg"
            },
            new Product
            {
                Id = 3,
                Name = "Historical Fiction Masterpiece",
                Description = "Experience history come alive in this captivating and meticulously researched historical fiction.",
                ISBN = "978-3-567890-12-4",
                Author = "Michael Johnson",
                ListPrice = 29.99,
                Price = 24.99,
                Price50 = 21.99,
                Price100 = 19.99,
                CategoryId = 3,
                ImageUrl = "https://images-na.ssl-images-amazon.com/images/I/51Zymoq7UnL._SX258_BO1,204,203,200_.jpg"
            }
        );
    }
}
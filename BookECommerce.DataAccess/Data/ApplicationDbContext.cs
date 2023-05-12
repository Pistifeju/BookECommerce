using BookECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace BookECommerce.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
         
    }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
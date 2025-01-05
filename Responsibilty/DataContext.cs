using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;

namespace WebApplicationlaptop.Responsibility
{
    public class DataContext : IdentityDbContext  
    {
        
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

		public DbSet<ContactModel> Contacts { get; set; }

        public DbSet<ReviewModel> Reviews { get; set; }




    }
}


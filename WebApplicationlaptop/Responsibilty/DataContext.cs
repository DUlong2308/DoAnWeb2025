using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;


namespace WebApplicationlaptop.Responsibility
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
     

        public DbSet<ShippingModel> Shippings { get; set; }

        public DbSet<MomoInfoModel> MomoInfos { get; set; }
        public DbSet<CouponModel> Coupons { get; set; }

        public DbSet<ProductQuantityModel> ProductQuantities { get; set; }
        public DbSet<InformationshopModel> Informationshops { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Nếu cần, bạn có thể cấu hình thêm các thực thể khác ở đây
        }
    }
}
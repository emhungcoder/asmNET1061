using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using foodService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace foodService.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Các DbSet cho các model của bạn
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // Bạn có thể cần override OnModelCreating nếu muốn cấu hình thêm các ràng buộc, chỉ mục, hoặc mối quan hệ.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình mối quan hệ giữa các bảng nếu cần
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Food)
                .WithMany()
                .HasForeignKey(od => od.FoodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Food)
                .WithMany()
                .HasForeignKey(ci => ci.FoodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Combo>()
                .HasMany(c => c.Foods)
                .WithMany(f => f.Combos)
                .UsingEntity(j => j.ToTable("ComboFoods"));
        }
    }
}

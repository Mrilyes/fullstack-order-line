using Microsoft.EntityFrameworkCore;
using order_orderline.Domain.Entites;

namespace order_orderline.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for the Article table
            modelBuilder.Entity<Article>().HasData(
                new Article { ArticleId = 1, Name = "Product A", Price = 100 },
                new Article { ArticleId = 2, Name = "Product B", Price = 150 }
            );

            // Seed data for the Order table
            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId = 1, OrderNumber = "ORD001", CustomerName = "John Doe", OrderDate = DateOnly.FromDateTime(DateTime.Now), },
                new Order { OrderId = 2, OrderNumber = "ORD002", CustomerName = "Jane Smith", OrderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), }
            );

            // Seed data for the OrderLine table
            modelBuilder.Entity<OrderLine>().HasData(
                new OrderLine { OrderLineId = 1, OrderId = 1, ArticleId = 1, Quantity = 2, Price = 100 },
                new OrderLine { OrderLineId = 2, OrderId = 2, ArticleId = 2, Quantity = 1, Price = 150 }
            );

            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Article)
                .WithMany()
                .HasForeignKey(ol => ol.ArticleId);

            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLines)
                .HasForeignKey(ol => ol.OrderId)
                .IsRequired() // Ensure the FK is required
                .OnDelete(DeleteBehavior.Cascade); // Ensures cascading delete

        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace order_orderline.Domain.Entites;

public partial class OrderOrderlineContext : DbContext
{
    public OrderOrderlineContext()
    {
    }

    public OrderOrderlineContext(DbContextOptions<OrderOrderlineContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderLine> OrderLines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=si-ighalila;Database=order-orderline;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Articles__9C6270E8144DC631");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFB1CA42F5");

            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.OrderNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasKey(e => e.OrderLineId).HasName("PK__OrderLin__29068A10ACD21A0A");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Article).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK__OrderLine__Artic__3C69FB99");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__OrderLine__Order__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data;

public partial class KDVDbContext : DbContext
{
    public KDVDbContext(DbContextOptions<KDVDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Audit> Audit { get; set; }

    public virtual DbSet<Brand> Brand { get; set; }

    public virtual DbSet<Category> Category { get; set; }

    public virtual DbSet<Diet> Diet { get; set; }

    public virtual DbSet<Employee> Employee { get; set; }

    public virtual DbSet<EnergyValue> EnergyValue { get; set; }

    public virtual DbSet<ExpenseOrder> ExpenseOrder { get; set; }

    public virtual DbSet<ExpenseOrderProduct> ExpenseOrderProduct { get; set; }

    public virtual DbSet<Filling> Filling { get; set; }

    public virtual DbSet<Manufacturer> Manufacturer { get; set; }

    public virtual DbSet<Order> Order { get; set; }

    public virtual DbSet<OrderProduct> OrderProduct { get; set; }

    public virtual DbSet<Package> Package { get; set; }

    public virtual DbSet<PriceUnit> PriceUnit { get; set; }

    public virtual DbSet<Product> Product { get; set; }

    public virtual DbSet<ProductDetails> ProductDetails { get; set; }

    public virtual DbSet<ProductImage> ProductImage { get; set; }

    public virtual DbSet<ReceiptOrder> ReceiptOrder { get; set; }

    public virtual DbSet<ReceiptOrderProduct> ReceiptOrderProduct { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<Taste> Taste { get; set; }

    public virtual DbSet<Models.Type> Type { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Audit>(entity =>
        {
            entity.Property(e => e.Action).HasMaxLength(200);
            entity.Property(e => e.DateOfAction).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Audit)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Audit_User");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Icon).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Diet>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.EmployeeId).ValueGeneratedNever();
            entity.Property(e => e.MiddleName).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(15);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.Surname).HasMaxLength(20);

            entity.HasOne(d => d.EmployeeNavigation).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.EmployeeId)
                .HasConstraintName("FK_Employee_User");
        });

        modelBuilder.Entity<EnergyValue>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.Property(e => e.ProductId).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithOne(p => p.EnergyValue)
                .HasForeignKey<EnergyValue>(d => d.ProductId)
                .HasConstraintName("FK_EnergyValue_Product");
        });

        modelBuilder.Entity<ExpenseOrder>(entity =>
        {
            entity.Property(e => e.Commentary).HasMaxLength(500);
            entity.Property(e => e.DateOfCreate).HasColumnType("datetime");
            entity.Property(e => e.DateOfExpense).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.ExpenseOrder)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_ExpenseOrder_Employee");
        });

        modelBuilder.Entity<ExpenseOrderProduct>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.ExpenseOrderId });

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ExpenseOrder).WithMany(p => p.ExpenseOrderProduct)
                .HasForeignKey(d => d.ExpenseOrderId)
                .HasConstraintName("FK_ExpenseOrderProduct_ExpenseOrder");

            entity.HasOne(d => d.Product).WithMany(p => p.ExpenseOrderProduct)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ExpenseOrderProduct_Product");
        });

        modelBuilder.Entity<Filling>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Commentary).HasMaxLength(500);
            entity.Property(e => e.DateOfOrder).HasColumnType("datetime");
            entity.Property(e => e.DateOfShipment).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.Order)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Order_Employee");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId });

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProduct)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderProduct_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProduct)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_OrderProduct_Product");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<PriceUnit>(entity =>
        {
            entity.Property(e => e.Unit).HasMaxLength(20);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_Product_1");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.MinPriceOfSale).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.PriceOfSale).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.ProductNumber).HasMaxLength(10);
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(7, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Product)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Product_Category");

            entity.HasOne(d => d.PriceUnit).WithMany(p => p.Product)
                .HasForeignKey(d => d.PriceUnitId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Product_PriceUnit");
        });

        modelBuilder.Entity<ProductDetails>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.Property(e => e.ProductId).ValueGeneratedNever();
            entity.Property(e => e.CountryOfProduction).HasMaxLength(20);

            entity.HasOne(d => d.Brand).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductDetails_Brand");

            entity.HasOne(d => d.Diet).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.DietId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductDetails_Diet");

            entity.HasOne(d => d.Filling).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.FillingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductDetails_Filling");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductDetails_Manufacturer");

            entity.HasOne(d => d.Package).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductDetails_Package");

            entity.HasOne(d => d.Product).WithOne(p => p.ProductDetails)
                .HasForeignKey<ProductDetails>(d => d.ProductId)
                .HasConstraintName("FK_ProductDetails_Product");

            entity.HasOne(d => d.Taste).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.TasteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductDetails_Taste");

            entity.HasOne(d => d.Type).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductDetails_Type");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);

            entity.Property(e => e.Path).HasMaxLength(150);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImage)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductImage_Product");
        });

        modelBuilder.Entity<ReceiptOrder>(entity =>
        {
            entity.Property(e => e.Commentary).HasMaxLength(500);
            entity.Property(e => e.DateOfCreate).HasColumnType("datetime");
            entity.Property(e => e.DateOfReceipt).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.ReceiptOrder)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_ReceiptOrder_Employee");
        });

        modelBuilder.Entity<ReceiptOrderProduct>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.ReceiptOrderId });

            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(7, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.ReceiptOrderProduct)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ReceiptOrderProduct_Product");

            entity.HasOne(d => d.ReceiptOrder).WithMany(p => p.ReceiptOrderProduct)
                .HasForeignKey(d => d.ReceiptOrderId)
                .HasConstraintName("FK_ReceiptOrderProduct_ReceiptOrder");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Taste>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Models.Type>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Image).HasMaxLength(100);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Login).HasMaxLength(15);
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.RefreshToken).HasMaxLength(250);
            entity.Property(e => e.RefreshTokenExp).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.User)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

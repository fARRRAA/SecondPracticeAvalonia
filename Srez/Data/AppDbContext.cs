using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Srez.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<MaterialMeasurement> MaterialMeasurements { get; set; }

    public virtual DbSet<MaterialType> MaterialTypes { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierMaterial> SupplierMaterials { get; set; }

    public virtual DbSet<SupplierType> SupplierTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=micialware.ru;Port=5432;Database=srez-fara;Username=trieco_admin;Password=trieco");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("materials_pkey");

            entity.ToTable("materials");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaterialTypeId).HasColumnName("material_type_id");
            entity.Property(e => e.MeasurementId).HasColumnName("measurement_id");
            entity.Property(e => e.MinQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("min_quantity");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PackageQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("package_quantity");
            entity.Property(e => e.StockQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("stock_quantity");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.MaterialType).WithMany(p => p.Materials)
                .HasForeignKey(d => d.MaterialTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("materials_material_type_id_fkey");

            entity.HasOne(d => d.Measurement).WithMany(p => p.Materials)
                .HasForeignKey(d => d.MeasurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_materials_measurement");
        });

        modelBuilder.Entity<MaterialMeasurement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("material_measurement_pkey");

            entity.ToTable("material_measurement");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<MaterialType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("material_types_pkey");

            entity.ToTable("material_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LossPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("loss_percentage");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_types_pkey");

            entity.ToTable("product_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Coefficient)
                .HasPrecision(10, 2)
                .HasColumnName("coefficient");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("suppliers_pkey");

            entity.ToTable("suppliers");

            entity.HasIndex(e => e.Inn, "suppliers_inn_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Inn)
                .HasMaxLength(20)
                .HasColumnName("inn");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.SupplierTypeId).HasColumnName("supplier_type_id");

            entity.HasOne(d => d.SupplierType).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.SupplierTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("suppliers_supplier_type_id_fkey");
        });

        modelBuilder.Entity<SupplierMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("supplier_materials_pkey");

            entity.ToTable("supplier_materials");

            entity.HasIndex(e => new { e.MaterialId, e.SupplierId }, "supplier_materials_material_id_supplier_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

            entity.HasOne(d => d.Material).WithMany(p => p.SupplierMaterials)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("supplier_materials_material_id_fkey");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierMaterials)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("supplier_materials_supplier_id_fkey");
        });

        modelBuilder.Entity<SupplierType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("supplier_types_pkey");

            entity.ToTable("supplier_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

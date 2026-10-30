using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagement.Models.Data.InvoiceDbContext;

public partial class InvoiceDbContext : IdentityDbContext <ApplicationUser>
{
    public InvoiceDbContext()
    {
    }

    public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<PurchaseItem> PurchaseItems { get; set; }

    public virtual DbSet<SalaryPayment> SalaryPayments { get; set; }

    public virtual DbSet<StockTransaction> StockTransactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=InvoiceDb; Integrated Security = true; MultipleActiveResultSets=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            //entity.HasKey(e => e.UserId)
            //    .HasName("PK8")
            //    .IsClustered(false);

            entity.ToTable("ApplicationUser");

            entity.HasOne(d => d.Company).WithMany(p => p.ApplicationUsers)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCompany14");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId)
                .HasName("PK1")
                .IsClustered(false);

            entity.ToTable("Company");

            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.CompanyName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId)
                .HasName("PK12")
                .IsClustered(false);

            entity.ToTable("Customer");

            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmplyeeId)
                .HasName("PK2")
                .IsClustered(false);

            entity.ToTable("Employee");

            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Salary).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.Employees)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCompany6");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId)
                .HasName("PK7")
                .IsClustered(false);

            entity.ToTable("Invoice");

            entity.Property(e => e.AmountPaid).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.Balance).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.CustomerName).HasMaxLength(25);
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCompany4");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCustomer21");
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.InvoiceItemId)
                .HasName("PK6")
                .IsClustered(false);

            entity.ToTable("InvoiceItem");

            entity.Property(e => e.SellingPrice).HasColumnType("decimal(25, 0)");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefInvoice15");

            entity.HasOne(d => d.Product).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefProduct20");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId)
                .HasName("PK5")
                .IsClustered(false);

            entity.ToTable("Payment");

            entity.Property(e => e.Amount).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(25, 0)")
                .HasColumnName("Payment");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefInvoice22");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId)
                .HasName("PK4")
                .IsClustered(false);

            entity.ToTable("Product");

            entity.Property(e => e.CostPrice).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.Description).HasMaxLength(25);
            entity.Property(e => e.SellingPrice).HasColumnType("decimal(25, 0)");

            entity.HasOne(d => d.Company).WithMany(p => p.Products)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCompany8");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId)
                .HasName("PK9")
                .IsClustered(false);

            entity.ToTable("Purchase");

            entity.Property(e => e.PurchaseDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCompany7");
        });

        modelBuilder.Entity<PurchaseItem>(entity =>
        {
            entity.HasKey(e => e.PurchaseItemId)
                .HasName("PK3")
                .IsClustered(false);

            entity.ToTable("PurchaseItem");

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(25, 0)");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefProduct16");

            entity.HasOne(d => d.Purchase).WithMany(p => p.PurchaseItems)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefPurchase17");
        });

        modelBuilder.Entity<SalaryPayment>(entity =>
        {
            entity.HasKey(e => e.SalaryPaymentId)
                .HasName("PK10")
                .IsClustered(false);

            entity.ToTable("SalaryPayment");

            entity.Property(e => e.Month).HasColumnType("datetime");
            entity.Property(e => e.PaidDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.SalaryPayments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCompany11");

            entity.HasOne(d => d.Emplyee).WithMany(p => p.SalaryPayments)
                .HasForeignKey(d => d.EmplyeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefEmployee18");
        });

        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.HasKey(e => e.StockTransactionId)
                .HasName("PK11")
                .IsClustered(false);

            entity.ToTable("StockTransaction");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCompany9");

            entity.HasOne(d => d.Product).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefProduct19");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

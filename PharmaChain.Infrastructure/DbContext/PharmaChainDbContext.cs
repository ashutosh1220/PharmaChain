using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Interfaces;

namespace PharmaChain.Infrastructure.Models;

public partial class PharmaChainDbContext : DbContext, IPharmaChainDbContext
{

    public PharmaChainDbContext(DbContextOptions<PharmaChainDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerPayment> CustomerPayments { get; set; }

    public virtual DbSet<CustomerReturn> CustomerReturns { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<MedicineBatch> MedicineBatches { get; set; }

    public virtual DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }

    public virtual DbSet<PurchaseItem> PurchaseItems { get; set; }

    public virtual DbSet<StockLedger> StockLedgers { get; set; }

    public virtual DbSet<StockRequest> StockRequests { get; set; }

    public virtual DbSet<StockTransfer> StockTransfers { get; set; }

    public virtual DbSet<StockTransferItem> StockTransferItems { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierPayment> SupplierPayments { get; set; }

    public virtual DbSet<SupplierReturn> SupplierReturns { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Roles> Roles { get; set; }
    public virtual DbSet<RolePermissions> RolePermissions { get; set; }
    public virtual DbSet<Permissions> Permissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=PharmaChainDb;Trusted_Connection=True;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleName)
                  .HasMaxLength(100)
                  .IsRequired();
            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);
        });

        modelBuilder.Entity<Permissions>(entity =>
        {
            entity.HasKey(e => e.PermissionId);
            entity.Property(e => e.PermissionName)
                  .HasMaxLength(100)
                  .IsRequired();
            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);
            entity.Property(e => e.Module)
                  .HasMaxLength(50);
        });

        modelBuilder.Entity<RolePermissions>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);

            entity.HasOne(rp => rp.Role)
                  .WithMany(r => r.RolePermission)
                  .HasForeignKey(rp => rp.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rp => rp.Permission)
                  .WithMany(p => p.RolePermission)
                  .HasForeignKey(rp => rp.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.RoleId, e.PermissionId })
                  .IsUnique();
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__branches__751EBD5FB134E5BC");

            entity.ToTable("branches");

            entity.Property(e => e.BranchId)
                .HasMaxLength(16)
                .HasColumnName("branchId");
            entity.Property(e => e.AddressLine1)
                .HasMaxLength(200)
                .HasColumnName("addressLine1");
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(200)
                .HasColumnName("addressLine2");
            entity.Property(e => e.BranchName)
                .HasMaxLength(150)
                .HasColumnName("branchName");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt).HasColumnName("deletedAt");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Gstin)
                .HasMaxLength(15)
                .HasColumnName("gstin");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Pincode)
                .HasMaxLength(10)
                .HasColumnName("pincode");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .HasColumnName("state");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__customer__B611CB7D95684B0E");

            entity.ToTable("customers");

            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .HasColumnName("address");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.BranchId)
                .HasMaxLength(16)
                .HasColumnName("branchId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");

            entity.HasOne(d => d.Branch).WithMany(p => p.Customers)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customers_branch");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Customers)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_customers_createdBy");
        });

        modelBuilder.Entity<CustomerPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__customer__A0D9EFC6986412EA");

            entity.ToTable("customerPayments");

            entity.Property(e => e.PaymentId)
                .HasMaxLength(16)
                .HasColumnName("paymentId");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");
            entity.Property(e => e.InvoiceId)
                .HasMaxLength(16)
                .HasColumnName("invoiceId");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("paymentDate");
            entity.Property(e => e.PaymentMode)
                .HasMaxLength(30)
                .HasColumnName("paymentMode");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(100)
                .HasColumnName("referenceNumber");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerPayments)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customerPayments_createdBy");

            entity.HasOne(d => d.Invoice).WithMany(p => p.CustomerPayments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customerPayments_invoice");
        });

        modelBuilder.Entity<CustomerReturn>(entity =>
        {
            entity.HasKey(e => e.ReturnId).HasName("PK__customer__EBA763197D8142E9");

            entity.ToTable("customerReturns");

            entity.Property(e => e.ReturnId)
                .HasMaxLength(16)
                .HasColumnName("returnId");
            entity.Property(e => e.BatchId)
                .HasMaxLength(16)
                .HasColumnName("batchId");
            entity.Property(e => e.BranchId)
                .HasMaxLength(16)
                .HasColumnName("branchId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");
            entity.Property(e => e.InvoiceId)
                .HasMaxLength(16)
                .HasColumnName("invoiceId");
            entity.Property(e => e.MedicineId)
                .HasMaxLength(16)
                .HasColumnName("medicineId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Reason)
                .HasMaxLength(200)
                .HasColumnName("reason");

            entity.HasOne(d => d.Batch).WithMany(p => p.CustomerReturns)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customerReturns_batch");

            entity.HasOne(d => d.Branch).WithMany(p => p.CustomerReturns)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customerReturns_branch");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerReturns)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customerReturns_createdBy");

            entity.HasOne(d => d.Invoice).WithMany(p => p.CustomerReturns)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customerReturns_invoice");

            entity.HasOne(d => d.Medicine).WithMany(p => p.CustomerReturns)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customerReturns_medicine");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__invoices__1252416C3D8F5EA2");

            entity.ToTable("invoices");

            entity.HasIndex(e => e.InvoiceNumber, "UQ__invoices__C72749EEBD88D100").IsUnique();

            entity.Property(e => e.InvoiceId)
                .HasMaxLength(16)
                .HasColumnName("invoiceId");
            entity.Property(e => e.BranchId)
                .HasMaxLength(16)
                .HasColumnName("branchId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");
            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.Discount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discount");
            entity.Property(e => e.InvoiceDate).HasColumnName("invoiceDate");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(100)
                .HasColumnName("invoiceNumber");
            entity.Property(e => e.PaymentMode)
                .HasMaxLength(50)
                .HasColumnName("paymentMode");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasColumnName("paymentStatus");
            entity.Property(e => e.TaxAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("taxAmount");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("totalAmount");

            entity.HasOne(d => d.Branch).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invoices_branch");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invoices_user");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invoices_customer");
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__invoiceI__3213E83F4AA6B88D");

            entity.ToTable("invoiceItems");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatchId)
                .HasMaxLength(16)
                .HasColumnName("batchId");
            entity.Property(e => e.BranchId)
                .HasMaxLength(16)
                .HasColumnName("branchId");
            entity.Property(e => e.GstPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("gstPercentage");
            entity.Property(e => e.InvoiceId)
                .HasMaxLength(16)
                .HasColumnName("invoiceId");
            entity.Property(e => e.MedicineId)
                .HasMaxLength(16)
                .HasColumnName("medicineId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("totalPrice");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unitPrice");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invoiceItems_batch");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invoiceItems_branch");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invoiceItems_invoice");

            entity.HasOne(d => d.Medicine).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invoiceItems_medicine");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__logs__7839F64D65FD20D4");

            entity.ToTable("logs");

            entity.Property(e => e.LogId).HasColumnName("logId");
            entity.Property(e => e.Action)
                .HasMaxLength(150)
                .HasColumnName("action");
            entity.Property(e => e.ActionType).HasColumnName("actionType");
            entity.Property(e => e.BranchId)
                .HasMaxLength(16)
                .HasColumnName("branchId");
            entity.Property(e => e.ChangedFields).HasColumnName("changedFields");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.Delta).HasColumnName("delta");
            entity.Property(e => e.DeviceInfo)
                .HasMaxLength(250)
                .HasColumnName("deviceInfo");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .HasColumnName("ipAddress");
            entity.Property(e => e.ModuleName)
                .HasMaxLength(100)
                .HasColumnName("moduleName");
            entity.Property(e => e.NewValue).HasColumnName("newValue");
            entity.Property(e => e.Notes)
                .HasMaxLength(250)
                .HasColumnName("notes");
            entity.Property(e => e.OldValue).HasColumnName("oldValue");
            entity.Property(e => e.ProcessedAt).HasColumnName("processedAt");
            entity.Property(e => e.RecordId)
                .HasMaxLength(50)
                .HasColumnName("recordId");
            entity.Property(e => e.RelatedRecordId)
                .HasMaxLength(50)
                .HasColumnName("relatedRecordId");
            entity.Property(e => e.SessionId)
                .HasMaxLength(100)
                .HasColumnName("sessionId");
            entity.Property(e => e.Severity).HasColumnName("severity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .HasColumnName("tableName");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .HasColumnName("userId");

            entity.HasOne(d => d.Branch).WithMany(p => p.Logs)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_logs_branch");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_logs_user");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__login__3213E83F542D0030");

            entity.ToTable("login");

            entity.HasIndex(e => e.Username, "UQ__login__F3DBC572BBE1963F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.FailedAttemptsCount).HasColumnName("failedAttemptsCount");
            entity.Property(e => e.FailedLoginAttempts).HasColumnName("failedLoginAttempts");
            entity.Property(e => e.IsLocked).HasColumnName("isLocked");
            entity.Property(e => e.LastLoginAt).HasColumnName("lastLoginAt");
            entity.Property(e => e.LockoutEndTime).HasColumnName("lockoutEndTime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("passwordHash");
            entity.Property(e => e.UserId)
                .HasMaxLength(16)
                .HasColumnName("userId");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.User).WithMany(p => p.Logins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_login_user");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.MedicineId)
                .HasName("PK__medicine__BA9E65EE512821EF");

            entity.ToTable("medicines");

            entity.Property(e => e.MedicineId)
                .HasMaxLength(16)
                .HasColumnName("medicineId");

            entity.Property(e => e.MedicineName)
                .HasMaxLength(150)
                .HasColumnName("medicineName");

            entity.Property(e => e.GenericName)
                .HasMaxLength(150)
                .HasColumnName("genericName");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");

            entity.Property(e => e.Strength)
                .HasMaxLength(50)
                .HasColumnName("strength");

            entity.Property(e => e.Manufacturer)
                .HasMaxLength(150)
                .HasColumnName("manufacturer");

            entity.Property(e => e.HsnCode)
                .HasMaxLength(20)
                .HasColumnName("hsnCode");

            entity.Property(e => e.GstPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("gstPercentage");

            entity.Property(e => e.MinimumStockLevel)
                .HasColumnName("minimumStockLevel");

            entity.Property(e => e.IsPrescriptionRequired)
                .HasColumnName("isPrescriptionRequired");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");

            entity.Property(e => e.IsDeleted)
                .HasColumnName("isDeleted");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deletedAt");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");

            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(16)
                .HasColumnName("updatedBy");

            entity.Property(e => e.DeletedBy)
                .HasMaxLength(16)
                .HasColumnName("deletedBy");

            entity.HasOne(d => d.CreatedByNavigation)
                .WithMany(p => p.MedicineCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_medicines_createdBy");

            entity.HasOne(d => d.DeletedByNavigation)
                .WithMany(p => p.MedicineDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_medicines_deletedBy");

            entity.HasOne(d => d.UpdatedByNavigation)
                .WithMany(p => p.MedicineUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_medicines_updatedBy");

            // ❗ IMPORTANT: DO NOT ADD StockTransferItems navigation mapping here
        });

        modelBuilder.Entity<MedicineBatch>(entity =>
        {
            entity.HasKey(e => e.BatchId).HasName("PK__medicine__78CCD773E15A550F");

            entity.ToTable("medicineBatch");

            entity.Property(e => e.BatchId)
                .HasMaxLength(16)
                .HasColumnName("batchId");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(100)
                .HasColumnName("batchNumber");
            entity.Property(e => e.BranchId)
                .HasMaxLength(16)
                .HasColumnName("branchId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");
            entity.Property(e => e.ExpDate).HasColumnName("expDate");
            entity.Property(e => e.GrnNumber)
                .HasMaxLength(100)
                .HasColumnName("grnNumber");
            entity.Property(e => e.MedicineId)
                .HasMaxLength(16)
                .HasColumnName("medicineId");
            entity.Property(e => e.MfgDate).HasColumnName("mfgDate");
            entity.Property(e => e.SupplierId)
                .HasMaxLength(16)
                .HasColumnName("supplierId");
            entity.Property(e => e.TotalStockReceived).HasColumnName("totalStockReceived");
            entity.Property(e => e.UnitPurchasePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unitPurchasePrice");
            entity.Property(e => e.UnitSellingPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unitSellingPrice");

            entity.HasOne(d => d.Branch).WithMany(p => p.MedicineBatches)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_medicineBatch_branch");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MedicineBatches)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_medicineBatch_createdBy");

            entity.HasOne(d => d.Medicine).WithMany(p => p.MedicineBatches)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_medicineBatch_medicine");

            entity.HasOne(d => d.Supplier).WithMany(p => p.MedicineBatches)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_medicineBatch_supplier");
        });

        modelBuilder.Entity<PurchaseInvoice>(entity =>
        {
            entity.HasKey(e => e.PurchaseInvoiceId).HasName("PK__purchase__B100BE005858A72E");

            entity.ToTable("purchaseInvoices");

            entity.Property(e => e.PurchaseInvoiceId)
                .HasMaxLength(16)
                .HasColumnName("purchaseInvoiceId");
            entity.Property(e => e.BranchId)
                .HasMaxLength(16)
                .HasColumnName("branchId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");
            entity.Property(e => e.DeletedAt).HasColumnName("deletedAt");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discountAmount");
            entity.Property(e => e.GrandTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("grandTotal");
            entity.Property(e => e.InvoiceDate).HasColumnName("invoiceDate");
            entity.Property(e => e.InwardCharges)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("inwardCharges");
            entity.Property(e => e.SubTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subTotal");
            entity.Property(e => e.SupplierId)
                .HasMaxLength(16)
                .HasColumnName("supplierId");
            entity.Property(e => e.SupplierInvoiceNumber)
                .HasMaxLength(50)
                .HasColumnName("supplierInvoiceNumber");
            entity.Property(e => e.TotalTax)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("totalTax");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(16)
                .HasColumnName("updatedBy");

            entity.HasOne(d => d.Branch).WithMany(p => p.PurchaseInvoices)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_purchaseInvoices_branch");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseInvoiceCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_purchaseInvoices_createdBy");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseInvoices)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_purchaseInvoices_supplier");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PurchaseInvoiceUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_purchaseInvoices_updatedBy");
        });

        modelBuilder.Entity<PurchaseItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__purchase__3213E83FB6DE985B");

            entity.ToTable("purchaseItems");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(100)
                .HasColumnName("batchNumber");
            entity.Property(e => e.ExpDate).HasColumnName("expDate");
            entity.Property(e => e.GstPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("gstPercentage");
            entity.Property(e => e.MedicineId)
                .HasMaxLength(16)
                .HasColumnName("medicineId");
            entity.Property(e => e.MfgDate).HasColumnName("mfgDate");
            entity.Property(e => e.PurchaseInvoiceId)
                .HasMaxLength(16)
                .HasColumnName("purchaseInvoiceId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPurchasePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unitPurchasePrice");
            entity.Property(e => e.UnitSellingPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unitSellingPrice");

            entity.HasOne(d => d.Medicine).WithMany(p => p.PurchaseItems)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_purchaseItems_medicine");

            entity.HasOne(d => d.PurchaseInvoice).WithMany(p => p.PurchaseItems)
                .HasForeignKey(d => d.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_purchaseItems_invoice");
        });

        modelBuilder.Entity<StockLedger>(entity =>
        {
            entity.HasKey(e => e.LedgerId).HasName("PK__stockLed__298DF4D5EA193C57");

            entity.ToTable("stockLedger");

            entity.Property(e => e.LedgerId).HasColumnName("ledgerId");
            entity.Property(e => e.BatchId)
                .HasMaxLength(16)
                .HasColumnName("batchId");
            entity.Property(e => e.BranchId)
                .HasMaxLength(16)
                .HasColumnName("branchId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");
            entity.Property(e => e.MedicineId)
                .HasMaxLength(16)
                .HasColumnName("medicineId");
            entity.Property(e => e.QuantityIn).HasColumnName("quantityIn");
            entity.Property(e => e.QuantityOut).HasColumnName("quantityOut");
            entity.Property(e => e.ReferenceId)
                .HasMaxLength(16)
                .HasColumnName("referenceId");
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(30)
                .HasColumnName("referenceType");
            entity.Property(e => e.Remarks)
                .HasMaxLength(250)
                .HasColumnName("remarks");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(30)
                .HasColumnName("transactionType");

            entity.HasOne(d => d.Batch).WithMany(p => p.StockLedgers)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stockLedger_batch");

            entity.HasOne(d => d.Branch).WithMany(p => p.StockLedgers)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stockLedger_branch");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockLedgers)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stockLedger_user");

            entity.HasOne(d => d.Medicine).WithMany(p => p.StockLedgers)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stockLedger_medicine");
        });

        modelBuilder.Entity<StockRequest>(entity =>
        {
            entity.HasKey(e => e.AssignedStockId)
                .HasName("PK__stockReq__B2D60BB6C7183A8B");

            entity.ToTable("stockRequests");

            entity.Property(e => e.AssignedStockId)
                .HasMaxLength(16)
                .HasColumnName("assignedStockId");

            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .HasDefaultValue("PENDING")
                .HasColumnName("approvalStatus");

            entity.Property(e => e.ApprovedBy)
                .HasMaxLength(16)
                .HasColumnName("approvedBy");

            entity.Property(e => e.ApprovedDate)
                .HasColumnName("approvedDate");

            //entity.Property(e => e.BatchId)
            //    .HasMaxLength(16)
            //    .HasColumnName("batchId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deletedAt");

            entity.Property(e => e.FromBranchId)
                .HasMaxLength(16)
                .HasColumnName("fromBranchId");

            entity.Property(e => e.FulfillmentType)
                .HasMaxLength(20)
                .HasColumnName("fulfillmentType");

            entity.Property(e => e.ItemsId)
                .HasMaxLength(1000)
                .HasColumnName("itemsId");

            entity.Property(e => e.QuantityApproved)
                .HasColumnName("quantityApproved");

            entity.Property(e => e.QuantityRequested)
                .HasColumnName("quantityRequested");

            entity.Property(e => e.QuantityTransferred)
                .HasColumnName("quantityTransferred");

            entity.Property(e => e.Remarks)
                .HasMaxLength(250)
                .HasColumnName("remarks");

            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("requestDate");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(16)
                .HasColumnName("supplierId");

            entity.Property(e => e.ToBranchId)
                .HasMaxLength(16)
                .HasColumnName("toBranchId");

            entity.Property(e => e.TransferDate)
                .HasColumnName("transferDate");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.ApprovedByNavigation)
                .WithMany(p => p.StockRequestApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_assignedStock_approvedBy");

            //entity.HasOne(d => d.Batch)
            //    .WithMany(p => p.StockRequests)
            //    .HasForeignKey(d => d.BatchId)
            //    .HasConstraintName("FK_assignedStock_batch");

            entity.HasOne(d => d.CreatedByNavigation)
                .WithMany(p => p.StockRequestCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_assignedStock_createdBy");

            entity.HasOne(d => d.FromBranch)
                .WithMany(p => p.StockRequestFromBranches)
                .HasForeignKey(d => d.FromBranchId)
                .HasConstraintName("FK_assignedStock_fromBranch");

            //entity.HasOne(d => d.Medicine)
            //    .WithMany(p => p.StockRequests)
            //    .HasForeignKey(d => d.ItemsId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_assignedStock_items");

            entity.HasOne(d => d.Supplier)
                .WithMany(p => p.StockRequests)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_assignedStock_supplier");

            entity.HasOne(d => d.ToBranch)
                .WithMany(p => p.StockRequestToBranches)
                .HasForeignKey(d => d.ToBranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_assignedStock_toBranch");
        });

        modelBuilder.Entity<StockTransfer>(entity =>
        {
            entity.HasKey(e => e.TransferId)
                .HasName("PK__stockTra__AAADCD81391CA40C");

            entity.ToTable("stockTransfers");

            entity.Property(e => e.TransferId)
                .HasMaxLength(16)
                .HasColumnName("transferId");

            entity.Property(e => e.ApprovedBy)
                .HasMaxLength(16)
                .HasColumnName("approvedBy");

            entity.Property(e => e.AssignedStockId)
                .HasMaxLength(16)
                .HasColumnName("assignedStockId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deletedAt");

            entity.Property(e => e.Remarks)
                .HasMaxLength(250)
                .HasColumnName("remarks");

            entity.Property(e => e.RequestedBy)
                .HasMaxLength(16)
                .HasColumnName("requestedBy");

            entity.Property(e => e.TransferDate)
                .HasColumnName("transferDate");

            entity.Property(e => e.TransferStatus)
                .HasMaxLength(20)
                .HasDefaultValue("PENDING")
                .HasColumnName("transferStatus");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.ApprovedByNavigation)
                .WithMany(p => p.StockTransferApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_stockTransfers_approvedBy");

            entity.HasOne(d => d.AssignedStock)
                .WithMany(p => p.StockTransfers)
                .HasForeignKey(d => d.AssignedStockId)
                .HasConstraintName("FK_stockTransfers_assignedStock");

            entity.HasOne(d => d.RequestedByNavigation)
                .WithMany(p => p.StockTransferRequestedByNavigations)
                .HasForeignKey(d => d.RequestedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stockTransfers_requestedBy");
        });

        modelBuilder.Entity<StockTransferItem>(entity =>
        {
            entity.HasKey(e => e.TransferItemId)
                .HasName("PK__stockTra__A36BC1BFF5DD2BBF");

            entity.ToTable("stockTransferItems");

            entity.Property(e => e.TransferItemId)
                .HasColumnName("transferItemId");

            entity.Property(e => e.TransferId)
                .HasMaxLength(16)
                .HasColumnName("transferId")
                .IsRequired();

            entity.Property(e => e.AssignedStockId)
                .HasMaxLength(16)
                .HasColumnName("assignedStockId")
                .IsRequired();

            entity.Property(e => e.MedicineId)
                .HasMaxLength(16)
                .HasColumnName("medicineId")
                .IsRequired();

            entity.Property(e => e.BatchId)
                .HasMaxLength(16)
                .HasColumnName("batchId")
                .IsRequired();

            entity.Property(e => e.Quantity)
                .HasColumnName("quantity");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("createdAt")
                .HasDefaultValueSql("(getdate())");

            /*
            // ===================== NAVIGATION CONFIG (KEEP DISABLED FOR NOW) =====================

            entity.HasOne(d => d.Batch)
                .WithMany()
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_stockTransferItems_batch");

            entity.HasOne(d => d.Medicine)
                .WithMany()
                .HasForeignKey(d => d.MedicineId)
                .HasConstraintName("FK_stockTransferItems_medicine");

            entity.HasOne(d => d.Transfer)
                .WithMany()
                .HasForeignKey(d => d.TransferId)
                .HasConstraintName("FK_stockTransferItems_transfer");

            entity.HasOne<StockRequest>()
                .WithMany()
                .HasForeignKey(e => e.AssignedStockId)
                .HasConstraintName("FK_stockTransferItems_assignedStock");

            ============================================================================ 
            */
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__supplier__DB8E62ED9796234B");

            entity.ToTable("suppliers");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(16)
                .HasColumnName("supplierId");
            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .HasColumnName("address");
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(50)
                .HasColumnName("bankAccountNumber");
            entity.Property(e => e.BankBranchName)
                .HasMaxLength(150)
                .HasColumnName("bankBranchName");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(150)
                .HasColumnName("contactPerson");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");
            entity.Property(e => e.DeletedBy)
                .HasMaxLength(16)
                .HasColumnName("deletedBy");
            entity.Property(e => e.DeletedOn).HasColumnName("deletedOn");
            entity.Property(e => e.DrugLicenseNumber)
                .HasMaxLength(50)
                .HasColumnName("drugLicenseNumber");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Gstin)
                .HasMaxLength(15)
                .HasColumnName("gstin");
            entity.Property(e => e.IfscCode)
                .HasMaxLength(20)
                .HasColumnName("ifscCode");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(150)
                .HasColumnName("supplierName");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(16)
                .HasColumnName("updatedBy");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SupplierCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_suppliers_createdBy");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.SupplierDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_suppliers_deletedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SupplierUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_suppliers_updatedBy");
        });

        modelBuilder.Entity<SupplierPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__supplier__A0D9EFC65CBEB0C1");

            entity.ToTable("supplierPayments");

            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.PaymentDate).HasColumnName("paymentDate");
            entity.Property(e => e.PaymentMode)
                .HasMaxLength(20)
                .HasColumnName("paymentMode");
            entity.Property(e => e.PurchaseInvoiceId)
                .HasMaxLength(16)
                .HasColumnName("purchaseInvoiceId");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(50)
                .HasColumnName("referenceNumber");
            entity.Property(e => e.SupplierId)
                .HasMaxLength(16)
                .HasColumnName("supplierId");

            entity.HasOne(d => d.PurchaseInvoice).WithMany(p => p.SupplierPayments)
                .HasForeignKey(d => d.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_supplierPayments_invoice");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierPayments)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_supplierPayments_supplier");
        });

        modelBuilder.Entity<SupplierReturn>(entity =>
        {
            entity.HasKey(e => e.ReturnId).HasName("PK__supplier__EBA763190F393E2F");

            entity.ToTable("supplierReturns");

            entity.Property(e => e.ReturnId)
                .HasMaxLength(16)
                .HasColumnName("returnId");
            entity.Property(e => e.BatchId)
                .HasMaxLength(16)
                .HasColumnName("batchId");
            entity.Property(e => e.BranchId)
                .HasMaxLength(16)
                .HasColumnName("branchId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(16)
                .HasColumnName("createdBy");
            entity.Property(e => e.MedicineId)
                .HasMaxLength(16)
                .HasColumnName("medicineId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Reason)
                .HasMaxLength(200)
                .HasColumnName("reason");
            entity.Property(e => e.SupplierId)
                .HasMaxLength(16)
                .HasColumnName("supplierId");

            entity.HasOne(d => d.Batch).WithMany(p => p.SupplierReturns)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_supplierReturns_batch");

            entity.HasOne(d => d.Branch).WithMany(p => p.SupplierReturns)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_supplierReturns_branch");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SupplierReturns)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_supplierReturns_createdBy");

            entity.HasOne(d => d.Medicine).WithMany(p => p.SupplierReturns)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_supplierReturns_medicine");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierReturns)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_supplierReturns_supplier");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.Property(e => e.RoleId).IsRequired();

            // ✅ Role FK
            entity.HasOne(d => d.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Login_Role");

            // ✅ Branch FK
            entity.HasOne(d => d.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(u => u.CreatedByNavigation)
                .WithMany(u => u.InverseCreatedByNavigation)
                .HasForeignKey(u => u.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_CreatedBy");

            entity.HasOne(u => u.DeletedByNavigation)
                .WithMany(u => u.InverseDeletedByNavigation)
                .HasForeignKey(u => u.DeletedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_DeletedBy");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

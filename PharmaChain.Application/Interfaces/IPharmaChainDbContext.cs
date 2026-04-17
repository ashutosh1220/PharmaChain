using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PharmaChain.Infrastructure.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PharmaChain.Application.Interfaces
{
    public interface IPharmaChainDbContext
    {
        DbSet<Branch> Branches { get; set; }

        DbSet<Customer> Customers { get; set; }

        DbSet<CustomerPayment> CustomerPayments { get; set; }

        DbSet<CustomerReturn> CustomerReturns { get; set; }

        DbSet<Invoice> Invoices { get; set; }

        DbSet<InvoiceItem> InvoiceItems { get; set; }

        DbSet<Log> Logs { get; set; }

        DbSet<Login> Logins { get; set; }

        DbSet<Medicine> Medicines { get; set; }

        DbSet<MedicineBatch> MedicineBatches { get; set; }

        DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }

        DbSet<PurchaseItem> PurchaseItems { get; set; }

        DbSet<StockLedger> StockLedgers { get; set; }

        DbSet<StockRequest> StockRequests { get; set; }

        DbSet<StockTransfer> StockTransfers { get; set; }

        DbSet<StockTransferItem> StockTransferItems { get; set; }

        DbSet<Supplier> Suppliers { get; set; }

        DbSet<SupplierPayment> SupplierPayments { get; set; }

        DbSet<SupplierReturn> SupplierReturns { get; set; }

        DbSet<User> Users { get; set; }
        DbSet<Roles> Roles { get; set; }
        DbSet<RolePermissions> RolePermissions { get; set; }
        DbSet<Permissions> Permissions { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DatabaseFacade Database { get; }
    }
}
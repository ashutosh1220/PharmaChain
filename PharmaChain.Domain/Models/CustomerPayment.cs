using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class CustomerPayment
{
    public string PaymentId { get; set; } = null!;

    public string InvoiceId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string PaymentMode { get; set; } = null!;

    public string? ReferenceNumber { get; set; }

    public DateTime PaymentDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;
}

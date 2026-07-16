namespace ecom_backend.Domain.Entities;

public class Invoice
{
    public int InvoiceId { get; set; }
    public int SubscriptionId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Paid";

    public Subscription Subscription { get; set; } = null!;
}

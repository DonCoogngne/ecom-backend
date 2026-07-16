namespace ecom_backend.DTOs.Subscription;

public class SubscriptionDto
{
    public string PlanName { get; set; } = "Free";
    public int FreeCredits { get; set; }
    public int RemainingCredits { get; set; }
    public List<InvoiceDto> Invoices { get; set; } = [];
}

public class InvoiceDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}

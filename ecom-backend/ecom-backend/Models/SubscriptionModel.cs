namespace ecom_backend.Models;

public class SubscriptionModel
{
    public int SubscriptionId { get; set; }
    public int UserId { get; set; }
    public string PlanName { get; set; } = "Free";
    public int FreeCredits { get; set; } = 25;
    public int RemainingCredits { get; set; } = 25;
    public List<InvoiceModel> Invoices { get; set; } = [];
}

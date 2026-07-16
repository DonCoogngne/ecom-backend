namespace ecom_backend.Domain.Entities;

public class Subscription
{
    public int SubscriptionId { get; set; }
    public int UserId { get; set; }
    public string PlanName { get; set; } = "Free";
    public int FreeCredits { get; set; } = 25;
    public int RemainingCredits { get; set; } = 25;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

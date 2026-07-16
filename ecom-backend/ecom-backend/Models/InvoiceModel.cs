namespace ecom_backend.Models;

public class InvoiceModel
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}

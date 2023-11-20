public class Transaction
{
    public string? Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string AccountIban { get; set; }
    public DateTime ValueDate { get; set; }
    public string Description { get; set; }
}
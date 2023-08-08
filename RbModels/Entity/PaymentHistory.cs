namespace RbModels.Entity;

public class PaymentHistory
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public bool PaymentSucceeded { get; set; }
    public decimal PaymentValue { get; set; }
    public decimal PaymentWithFee { get; set; }
    public decimal CurrentBalance { get; set; }
}
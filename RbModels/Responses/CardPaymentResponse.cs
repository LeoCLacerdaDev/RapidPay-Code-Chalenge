namespace RbModels.Responses;

public record CardPaymentResponse
{
    public bool PaymentSucceeded { get; init; }
    public Guid CardId { get; init; }
    public decimal PaymentValue { get; init; }
    public decimal PaymentWithFee { get; init; }
    public decimal CurrentBalance { get; init; }
};
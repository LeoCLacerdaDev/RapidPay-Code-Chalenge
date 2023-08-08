namespace RbModels.Responses;

public record CardBalanceResponse
{
    public string UserEmail { get; init; }
    public Guid CardId { get; init; }
    public decimal Balance { get; init; }
};
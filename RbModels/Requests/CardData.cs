namespace RbModels.Requests;

public record CardData
{
    public Guid CardId { get; set; }
    public string UserEmail { get; set; }
    public decimal Balance { get; set; }
};
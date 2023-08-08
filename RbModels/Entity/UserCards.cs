namespace RbModels.Entity;

public class UserCards
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ICollection<Card> Cards { get; set; } = new List<Card>();
}
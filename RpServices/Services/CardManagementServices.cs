using Microsoft.EntityFrameworkCore;
using RbModels.Entity;
using RbModels.Requests;
using RbModels.Responses;
using RpServices.Services.Interfaces;

namespace RpServices.Services;

public class CardManagementServices : ICardManagement
{
    private readonly DatabaseContext _context;

    public CardManagementServices(DatabaseContext context) =>
        _context = context;

    public async Task<CardCreatedResponse> CreateCardAsync(CardCreate card)
    {
        if (await _context.Cards.AnyAsync(t => t.Digits == card.Digits))
            throw new Exception("Digits already exists in database.");

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var newCard = new Card
            {
                Digits = card.Digits
            };

            _context.Add(newCard);
            await _context.SaveChangesAsync();

            var userCard = new UserCards
            {
                Id = newCard.Id,
                UserId = card.UserId
            };
            _context.Add(userCard);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var user = await _context.Users.FirstOrDefaultAsync(t => t.Id == card.UserId.ToString());

            return new CardCreatedResponse
            {
                UserEmail = user.Email,
                CardId = newCard.Id
            };
        }
        catch(Exception e)
        {
            await transaction.RollbackAsync();
            throw new Exception(e.Message);
        }
        finally

        {
            await transaction.DisposeAsync();
        }
    }
}
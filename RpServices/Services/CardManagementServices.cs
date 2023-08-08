using Microsoft.EntityFrameworkCore;
using RbModels.Entity;
using RbModels.Requests;
using RbModels.Responses;
using RpServices.Services.Interfaces;

namespace RpServices.Services;

public class CardManagementServices : ICardManagement
{
    private readonly DatabaseContext _context;
    private readonly IUniversalFeeExchange _feeExchange;

    private static readonly object Sync = new();

    public CardManagementServices(DatabaseContext context, IUniversalFeeExchange feeExchange)
    {
        _context = context;
        _feeExchange = feeExchange;
    }

    public async Task<CardCreatedResponse> CreateCardAsync(CardCreate card)
    {
        Monitor.Enter(Sync);
        
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
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw new Exception(e.Message);
        }
        finally
        {
            await transaction.DisposeAsync();
            Monitor.Exit(Sync);
        }
    }

    public async Task<CardPaymentResponse> Pay(CardPayment payment, Guid userId)
    {
        Monitor.Enter(Sync);
        
        await VerifyUserCard(payment.CardId, userId);

        var dbCard = await _context.Cards
            .Where(t => t.Id == payment.CardId)
            .FirstOrDefaultAsync();

        // TODO confirm this
        var feeAmount = payment.Value * _feeExchange.CurrentFee;
        var paymentWithFee = payment.Value + feeAmount;

        if (dbCard.Balance < paymentWithFee)
            return new CardPaymentResponse
            {
                PaymentSuccedded = false,
                CardId = payment.CardId,
                CurrentBalance = dbCard.Balance,
                PaymentValue = payment.Value,
                PaymentWithFee = paymentWithFee
            };

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            dbCard.Balance -= paymentWithFee;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new CardPaymentResponse
            {
                PaymentSuccedded = true,
                CardId = payment.CardId,
                CurrentBalance = dbCard.Balance,
                PaymentValue = payment.Value,
                PaymentWithFee = paymentWithFee
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw new Exception(e.Message);
        }
        finally
        {
            await transaction.DisposeAsync();
            Monitor.Exit(Sync);
        }
    }

    public async Task<CardBalanceResponse> GetBalance(CardBalance card, Guid userId)
    {
        await VerifyUserCard(card.CardId, userId);

        var dbCard = await _context.Cards.FirstOrDefaultAsync(t => t.Id == card.CardId);
        var user = await _context.Users.FirstOrDefaultAsync(t => t.Id == userId.ToString());

        return new CardBalanceResponse
        {
            Balance = dbCard.Balance,
            UserEmail = user.Email,
            CardId = card.CardId
        };
    }

    private async Task VerifyUserCard(Guid cardId, Guid userId)
    {
        var validate = await _context.UserCards.AnyAsync(t => t.Id == cardId && t.UserId == userId);
        if (!validate)
            throw new Exception("Card doesnt bellong to user.");
    }
}
using Microsoft.EntityFrameworkCore;
using RbModels.Entity;
using RbModels.Requests.Controllers;
using RbModels.Responses;
using RpServices.Services.Interfaces;

namespace RpServices.Services;

public class CardManagementServices : ICardManagement
{
    private readonly DatabaseContext _context;
    private readonly IUniversalFeeExchange _feeExchange;
    private readonly IPaymentRegister _paymentRegister;

    private static SemaphoreSlim _semaphore = new(1, 1);
    private static readonly object Sync = new();

    public CardManagementServices(DatabaseContext context, IUniversalFeeExchange feeExchange,
        IPaymentRegister paymentRegister)
    {
        _context = context;
        _feeExchange = feeExchange;
        _paymentRegister = paymentRegister;
    }

    public async Task<CardCreatedResponse> CreateCardAsync(CardCreate card)
    {
        await _semaphore.WaitAsync();

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
            _semaphore.Release();
        }
    }

    public async Task<CardPaymentResponse> Pay(CardPayment payment, Guid userId)
    {
        await _semaphore.WaitAsync();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await VerifyUserCard(payment.CardId, userId);

            var dbCard = await _context.Cards
                .Where(t => t.Id == payment.CardId)
                .FirstOrDefaultAsync();

            // TODO confirm this
            var feeAmount = payment.Value * _feeExchange.CurrentFee;
            var paymentWithFee = payment.Value + feeAmount;

            if (dbCard.Balance < paymentWithFee)
            {
                await RegisterPayment(payment, false, dbCard, paymentWithFee);
                await transaction.CommitAsync();
                return CreatePaymentResponse(false, payment, dbCard, paymentWithFee);
            }

            dbCard.Balance -= paymentWithFee;

            await RegisterPayment(payment, true, dbCard, paymentWithFee);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreatePaymentResponse(true, payment, dbCard, paymentWithFee);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw new Exception(e.Message);
        }
        finally
        {
            await transaction.DisposeAsync();
            _semaphore.Release();
        }
    }

    private static CardPaymentResponse CreatePaymentResponse(bool isSuccess, CardPayment payment, Card dbCard,
        decimal paymentWithFee)
    {
        return new CardPaymentResponse
        {
            PaymentSucceeded = isSuccess,
            CardId = payment.CardId,
            CurrentBalance = dbCard.Balance,
            PaymentValue = payment.Value,
            PaymentWithFee = paymentWithFee
        };
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

    private async Task RegisterPayment(CardPayment payment, bool isSuccess, Card dbCard, decimal paymentWithFee)
    {
        var history = new PaymentHistory
        {
            CardId = payment.CardId,
            PaymentSucceeded = isSuccess,
            PaymentValue = payment.Value,
            PaymentWithFee = paymentWithFee,
            CurrentBalance = dbCard.Balance
        };
        await _paymentRegister.RegisterPayment(history);
    }

    private async Task VerifyUserCard(Guid cardId, Guid userId)
    {
        var validate = await _context.UserCards.AnyAsync(t => t.Id == cardId && t.UserId == userId);
        if (!validate)
            throw new Exception("Card doesn't belong to user.");
    }
}
using RbModels.Requests;
using RbModels.Responses;

namespace RpServices.Services.Interfaces;

public interface ICardManagement
{
    Task<CardCreatedResponse> CreateCardAsync(CardCreate card);
    Task<CardPaymentResponse> Pay(CardPayment payment, Guid userId);
    Task<CardBalanceResponse> GetBalance(CardBalance card, Guid userId);
}
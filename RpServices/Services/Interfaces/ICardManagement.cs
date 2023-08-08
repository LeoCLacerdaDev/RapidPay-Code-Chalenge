using RbModels.Requests;
using RbModels.Responses;

namespace RpServices.Services.Interfaces;

public interface ICardManagement
{
    Task<CardCreatedResponse> CreateCardAsync(CardCreate card);
    // Task Pay(CardData card);
}
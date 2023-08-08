using RbModels.Entity;

namespace RpServices.Services.Interfaces;

public interface IPaymentRegister
{
    Task RegisterPayment(PaymentHistory paymentHistory);
}
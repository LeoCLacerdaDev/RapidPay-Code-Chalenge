using Microsoft.EntityFrameworkCore;
using RbModels.Entity;
using RpServices.Services.Interfaces;

namespace RpServices.Services;

public class PaymentRegisterService : IPaymentRegister
{
    private readonly DatabaseContext _context;

    public PaymentRegisterService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task RegisterPayment(PaymentHistory paymentHistory)
    {
        _context.Add(paymentHistory);
        await _context.SaveChangesAsync();
    }
}
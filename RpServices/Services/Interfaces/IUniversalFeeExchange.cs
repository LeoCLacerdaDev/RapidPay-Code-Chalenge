using Timer = System.Timers.Timer;

namespace RpServices.Services.Interfaces;

public interface IUniversalFeeExchange
{
    decimal CurrentFee { get; }
}
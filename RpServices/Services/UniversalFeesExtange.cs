using System.Timers;
using Microsoft.Extensions.Logging;
using RpServices.Services.Interfaces;
using Timer = System.Timers.Timer;

namespace RpServices.Services;

public class UniversalFeesExtange : IUniversalFeeExchange
{
    private readonly ILogger<UniversalFeesExtange> _logger;
    public decimal CurrentFee { get; private set; }

    private readonly Timer _timer;

    public UniversalFeesExtange(ILogger<UniversalFeesExtange> logger)
    {
        _logger = logger;
        var random = new Random();
        CurrentFee = 1 * (decimal)random.NextDouble() * 2;
        _timer = new Timer(36000000); // todo 1 hour in milliseconds
        _timer.Elapsed += UpdateFee;
        _timer.Start();
    }

    private void UpdateFee(object? sender, ElapsedEventArgs e)
    {
        var random = new Random();
        var multiplier = (decimal)random.NextDouble() * 2;
        CurrentFee *= multiplier;
        _logger.LogInformation(CurrentFee.ToString());
    }

    public void Dispose() =>
        _timer.Dispose();
}
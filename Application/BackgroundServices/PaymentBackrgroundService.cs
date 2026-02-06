using Application.Interfaces.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Application.BackgroundServices
{
    public class PaymentBackrgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentBackrgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var _paymentService = scope.ServiceProvider
                        .GetRequiredService<IPaymentService>();

                    await _paymentService.ExspirePaymentAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in PaymentBackgroundService: " + ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
            }

        }
    }
}

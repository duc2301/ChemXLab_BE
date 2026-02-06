using Application.Interfaces.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BackgroundServices
{
    public class SubscriptionBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SubscriptionBackgroundService(IServiceScopeFactory scopeFactory)
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
                    var _subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();

                    await _subscriptionService.ExspireSubscription();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in SubscriptionBackgroundService: " + ex.Message);
                }
                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }

    }
}

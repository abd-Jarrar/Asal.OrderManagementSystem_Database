using Asal.OrderManagementSystem.Api.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Asal.OrderManagementSystem.Api.BackgroundJobs
{
    public class CheckExpiredReservationsBackgroundJob(
        IServiceScopeFactory scopeFactory,
        ILogger<CheckExpiredReservationsBackgroundJob> logger)
        : BackgroundService
    {
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Expired reservations background job started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var reservationRepository =
                        scope.ServiceProvider
                            .GetRequiredService<IReservationRepository>();

                    var expiredReservations =
                        await reservationRepository
                            .GetReservationsPendingExpirationAsync(stoppingToken);

                    if (expiredReservations.Count > 0)
                    {
                        await reservationRepository
                            .RemoveItemsFromExpiredReservationsAsync(
                                expiredReservations,
                                stoppingToken);

                        logger.LogInformation(
                            "Expired {Count} reservations.",
                            expiredReservations.Count);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "An error occurred while checking expired reservations.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            logger.LogInformation("Expired reservations background job stopped.");
        }
    }
}
using CodeQuestBackend.Services;

namespace CodeQuestBackend.Services;

public class RankingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RankingBackgroundService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromMinutes(15); // Run every 15 minutes

    public RankingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RankingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Ranking Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var trendingService = scope.ServiceProvider.GetRequiredService<TrendingService>();
                var popularityService = scope.ServiceProvider.GetRequiredService<PopularityService>();

                _logger.LogInformation("Starting ranking calculations");

                // Calculate trending scores
                await trendingService.CalculateTrendingScoresAsync();

                // Calculate popularity scores (less frequently)
                if (DateTime.UtcNow.Minute % 30 == 0) // Every 30 minutes
                {
                    await popularityService.CalculatePopularityScoresAsync();
                }

                _logger.LogInformation("Ranking calculations completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ranking background service");
            }

            await Task.Delay(_period, stoppingToken);
        }

        _logger.LogInformation("Ranking Background Service stopped");
    }
}

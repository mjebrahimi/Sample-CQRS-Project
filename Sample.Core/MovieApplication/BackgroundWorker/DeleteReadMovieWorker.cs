using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Core.Common;
using Sample.Core.MovieApplication.BackgroundWorker.AddReadMovie;
using Sample.Core.MovieApplication.Commands.DeleteMovie;
using Sample.DAL.ReadRepositories;

namespace Sample.Core.MovieApplication.BackgroundWorker.DeleteReadMovie
{
    public class DeleteReadMovieWorker : BackgroundService
    {
        private readonly ChannelQueue<MovieDeletedEvent> _channel;
        private readonly ILogger<AddReadMovieWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DeleteReadMovieWorker(ChannelQueue<MovieDeletedEvent> channel, ILogger<AddReadMovieWorker> logger, IServiceProvider serviceProvider)
        {
            _channel = channel;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var readMovieRepository = scope.ServiceProvider.GetRequiredService<ReadMovieRepository>();

                    await foreach (var item in _channel.ReadAsync(stoppingToken))
                    {
                        await readMovieRepository.DeleteByMovieIdAsync(item.MovieId, stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
        }
    }
}
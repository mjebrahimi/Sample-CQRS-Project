using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Core.Common;
using Sample.Core.MovieApplication.Commands.AddMovie;
using Sample.DAL.Models.ReadModels;
using Sample.DAL.ReadRepositories;
using Sample.DAL.WriteRepositories;

namespace Sample.Core.MovieApplication.BackgroundWorker.AddReadMovie
{
    public class AddReadMovieWorker : BackgroundService
    {
        private readonly ChannelQueue<MovieAddedEvent> _channel;
        private readonly ILogger<AddReadMovieWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public AddReadMovieWorker(ChannelQueue<MovieAddedEvent> channel, ILogger<AddReadMovieWorker> logger, IServiceProvider serviceProvider)
        {
            _channel = channel;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                var writeMovieRepository = scope.ServiceProvider.GetRequiredService<WriteMovieRepository>();
                var readMovieRepository = scope.ServiceProvider.GetRequiredService<ReadMovieRepository>();

                try
                {
                    await foreach (var item in _channel.ReadAsync(stoppingToken))
                    {
                        var movie = await writeMovieRepository.GetByIdAsync(item.MovieId, stoppingToken);

                        if (movie != null)
                        {
                            await readMovieRepository.AddAsync(new Movie
                            {
                                MovieId = movie.Id,
                                Director = movie.Director.FullName,
                                Name = movie.Name,
                                PublishYear = movie.PublishYear,
                                BoxOffice = movie.BoxOffice,
                                ImdbRate = movie.ImdbRate
                            }, stoppingToken);
                        }
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
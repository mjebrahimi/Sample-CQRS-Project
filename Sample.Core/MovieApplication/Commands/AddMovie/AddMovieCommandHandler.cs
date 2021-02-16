using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sample.Core.Common;
using Sample.DAL;
using Sample.DAL.Models.WriteModels;
using Sample.DAL.WriteRepositories;

namespace Sample.Core.MovieApplication.Commands.AddMovie
{
    public class AddMovieCommandHandler : IRequestHandler<AddMovieCommand>
    {
        private readonly ApplicationDbContext _db;
        private readonly WriteMovieRepository _movieRepository;
        private readonly DirectorRepository _directorRepository;
        private readonly ChannelQueue<MovieAddedEvent> _channel;

        public AddMovieCommandHandler(ApplicationDbContext db, WriteMovieRepository movieRepository, DirectorRepository directorRepository, ChannelQueue<MovieAddedEvent> channel)
        {
            _db = db;
            _movieRepository = movieRepository;
            _directorRepository = directorRepository;
            _channel = channel;
        }

        public async Task<Unit> Handle(AddMovieCommand request, CancellationToken cancellationToken)
        {
            var director = await _directorRepository.GetByNameAsync(request.Director, cancellationToken);

            if (director is null)
            {
                director = new Director { FullName = request.Director };
                await _directorRepository.AddAsync(director, cancellationToken);
            }

            var movie = new Movie
            {
                PublishYear = request.PublishYear,
                BoxOffice = request.BoxOffice,
                ImdbRate = request.ImdbRate,
                Name = request.Name,
                Director = director
            };

            await _movieRepository.AddAsync(movie, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            await _channel.AddAsync(new MovieAddedEvent { MovieId = movie.Id }, cancellationToken);

            return Unit.Value;
        }
    }
}
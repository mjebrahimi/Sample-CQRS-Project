using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sample.Core.Common;
using Sample.DAL;
using Sample.DAL.WriteRepositories;

namespace Sample.Core.MovieApplication.Commands.DeleteMovie
{
    public class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommand>
    {
        private readonly ApplicationDbContext _db;
        private readonly WriteMovieRepository _movieRepository;
        private readonly ChannelQueue<MovieDeletedEvent> _channel;

        public DeleteMovieCommandHandler(ApplicationDbContext db, WriteMovieRepository movieRepository, ChannelQueue<MovieDeletedEvent> channel)
        {
            _db = db;
            _movieRepository = movieRepository;
            _channel = channel;
        }

        public async Task<Unit> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken);

            if (!(movie is null))
            {
                _movieRepository.Delete(movie);

                await _db.SaveChangesAsync(cancellationToken);

                await _channel.AddAsync(new MovieDeletedEvent { MovieId = movie.Id }, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
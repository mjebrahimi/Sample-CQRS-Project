using MediatR;

namespace Sample.Core.MovieApplication.Commands.DeleteMovie
{
    public class DeleteMovieCommand : IRequest
    {
        public int MovieId { get; set; }
    }
}
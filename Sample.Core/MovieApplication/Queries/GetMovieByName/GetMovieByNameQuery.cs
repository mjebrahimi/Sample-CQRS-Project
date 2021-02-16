using MediatR;
using Sample.DAL.Models.ReadModels;

namespace Sample.Core.MovieApplication.Queries.GetMovieByName
{
    public class GetMovieByNameQuery : IRequest<Movie>
    {
        public string MovieName { get; set; }
    }
}
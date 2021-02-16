using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sample.DAL.Models.WriteModels;

namespace Sample.DAL.WriteRepositories
{
    public class WriteMovieRepository
    {
        private readonly ApplicationDbContext _db;

        public WriteMovieRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<Movie> GetByIdAsync(int movieId, CancellationToken cancellationToken = default)
        {
            return _db.Movies.Include(c => c.Director).FirstOrDefaultAsync(c => c.Id == movieId, cancellationToken);
        }

        public async Task AddAsync(Movie movie, CancellationToken cancellationToken = default)
        {
            await _db.Movies.AddAsync(movie, cancellationToken);
        }

        public void Delete(Movie movie)
        {
            _db.Movies.Remove(movie);
        }
    }
}
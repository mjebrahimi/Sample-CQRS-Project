using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sample.DAL.Models.WriteModels;

namespace Sample.DAL.WriteRepositories
{
    public class DirectorRepository
    {
        private readonly ApplicationDbContext _db;

        public DirectorRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<Director> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return _db.Directors.FirstOrDefaultAsync(d => d.FullName == name, cancellationToken: cancellationToken);
        }

        public async Task AddAsync(Director director, CancellationToken cancellationToken = default)
        {
            await _db.Directors.AddAsync(director, cancellationToken);
        }
    }
}
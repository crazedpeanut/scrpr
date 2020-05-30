using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Models;
using MongoDB.Driver;

namespace Api.Services
{
    public class ScraperJobRepository
    {
        private readonly IMongoCollection<ScraperJob> jobCollection;
        public ScraperJobRepository(IMongoCollection<ScraperJob> jobCollection)
        {
            this.jobCollection = jobCollection;
        }

        public Task Create(ScraperJob job, CancellationToken cancellationToken)
        {
            return jobCollection.InsertOneAsync(job, null, cancellationToken);
        }

        public Task Get(string id, CancellationToken cancellationToken)
        {
            return jobCollection.Find(j => j.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<ScraperJob>> Get(CancellationToken cancellationToken)
        {
            return jobCollection
                .Find(_ => true)
                .ToListAsync(cancellationToken);
        }

        public Task Update(ScraperJob job, CancellationToken cancellationToken)
        {
            return jobCollection
               .ReplaceOneAsync(j => j.Id == job.Id, job, new ReplaceOptions(), cancellationToken);
        }
    }
}
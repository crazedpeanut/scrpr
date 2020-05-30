using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Api.Models;
using System.Linq.Expressions;
using System;

namespace Api.Services
{
    public class ScraperResultRespository
    {
        private readonly IMongoCollection<ScraperResult> resultCollection;
        public ScraperResultRespository(IMongoCollection<ScraperResult> resultCollection)
        {
            this.resultCollection = resultCollection;
        }

        public Task Create(ScraperResult job, CancellationToken cancellationToken)
        {
            return resultCollection.InsertOneAsync(job, null, cancellationToken);
        }

        public Task Get(string id, CancellationToken cancellationToken)
        {
            return resultCollection.Find(j => j.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<ScraperResult>> Get(Expression<Func<ScraperResult, bool>> query, CancellationToken cancellationToken)
        {
            return resultCollection
                .Find(query)
                .ToListAsync(cancellationToken);
        }
        
        public Task<List<ScraperResult>> Get(FilterDefinition<ScraperResult> query, CancellationToken cancellationToken)
        {
            return resultCollection
                .Find(query)
                .ToListAsync(cancellationToken);
        }

        public Task Update(ScraperResult job, CancellationToken cancellationToken)
        {
            return resultCollection
               .ReplaceOneAsync(j => j.Id == job.Id, job, new ReplaceOptions(), cancellationToken);
        }
    }
}
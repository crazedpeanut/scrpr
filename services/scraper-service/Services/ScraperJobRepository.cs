using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeetleX.Redis;
using ScraperService.Models;
using shared;

namespace ScraperService.Services
{
    public class ScraperJobRepository
    {
        private readonly RedisDB db;
        private readonly SharedConfiguration configuration;

        public ScraperJobRepository(RedisDB db, SharedConfiguration configuration)
        {
            this.configuration = configuration;
            this.db = db;
        }

        public async Task Create(ScraperJob job)
        {
            job.Id = CreateId();
            await db.Set(CreateKey(job), job);
        }

        public async Task<ScraperJob> Get(string id)
        {
            return await db.Get<ScraperJob>(CreateKey(id));
        }

        public async Task<List<ScraperJob>> Get()
        {
            var cursor = 0;

            var results = new List<ScraperJob>();

            do
            {
                var scan = await db.Scan(cursor, 20, $"{configuration.Cache.Items.ScraperJobs.BaseKey}/*");
                cursor = scan.NextCursor;
                var scanResults = await db.MGet(scan.Keys.ToArray(), scan.Keys.Select(_ => typeof(ScraperJob)).ToArray());
                results.AddRange(scanResults.Select(r => (ScraperJob)r));
            }
            while (cursor != 0);

            return results;
        }

        public async Task Update(ScraperJob job)
        {
            await db.Set(CreateKey(job), job);
        }

        private string CreateId() => Guid.NewGuid().ToString();
        private string CreateKey(ScraperJob job) => CreateKey(job.Id);
        private string CreateKey(string id) => $"{configuration.Cache.Items.ScraperJobs.BaseKey}/{id}";
    }
}
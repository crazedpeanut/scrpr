using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeetleX.Redis;
using ScraperService.Models;
using shared;

namespace ScraperService.Services
{
    public class ScraperResultRepository
    {
        private readonly RedisDB db;
        private readonly SharedConfiguration configuration;

        public ScraperResultRepository(RedisDB db, SharedConfiguration configuration)
        {
            this.configuration = configuration;
            this.db = db;
        }

        public async Task Create(ScraperResult job)
        {
            await db.Set(CreateKey(job), job, configuration.Cache.Items.ScraperResults.TTL);
        }

        public async Task<List<ScraperResult>> Get()
        {
            var cursor = 0;

            var results = new List<ScraperResult>();

            do
            {
                var scan = await db.Scan(cursor, 20, $"{configuration.Cache.Items.ScraperJobs.BaseKey}/*");
                cursor = scan.NextCursor;
                var scanResults = await db.MGet(scan.Keys.ToArray(), scan.Keys.Select(_ => typeof(ScraperJob)).ToArray());
                results.AddRange(scanResults.Select(r => (ScraperResult)r));
            }
            while (cursor != 0);

            return results;
        }

        public async Task Update(ScraperResult job)
        {
            await db.Set(CreateKey(job), job, configuration.Cache.Items.ScraperResults.TTL);
        }

        private string CreateKey(ScraperResult job) => CreateKey(job.Url);
        private string CreateKey(Uri url) => $"{configuration.Cache.Items.ScraperJobs.BaseKey}/{url}";
    }
}
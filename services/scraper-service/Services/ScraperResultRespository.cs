using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeetleX.Redis;
using ScraperService.Models;

namespace ScraperService.Services
{
    public class ScraperResultRepository
    {
        private readonly RedisDB db;
        private const string baseKey = "/scraper-results/";

        public ScraperResultRepository(RedisDB db)
        {
            this.db = db;
        }

        public async Task Create(ScraperJob job)
        {
            job.Id = CreateId();
            await db.Set(CreateKey(job), job);
        }

        public async Task<ScraperResult> Get(string id)
        {
            return await db.Get<ScraperResult>(CreateKey(id));
        }

        public async Task<List<ScraperResult>> Get()
        {
            var cursor = 0;

            var results = new List<ScraperResult>();

            do
            {
                var scan = await db.Scan(cursor, 20, $"{baseKey}/*");
                cursor = scan.NextCursor;
                var scanResults = await db.MGet(scan.Keys.ToArray(), scan.Keys.Select(_ => typeof(ScraperJob)).ToArray());
                results.AddRange(scanResults.Select(r => (ScraperResult)r));
            }
            while (cursor != 0);

            return results;
        }

        public async Task Update(ScraperResult job)
        {
            await db.Set(CreateKey(job), job);
        }

        private string CreateId() => Guid.NewGuid().ToString();
        private string CreateKey(ScraperResult job) => CreateKey(job.Url);
        private string CreateKey(Uri url) => $"{baseKey}/{url}";
    }
}
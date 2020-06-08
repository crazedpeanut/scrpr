using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Scraper.Mongo
{
    public class SimpleRepository<T> : BaseRepository<T> where T : MongoEntity
    {
        public SimpleRepository(IMongoCollection<T> collection) : base(collection)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Scraper.Mongo
{
    public abstract class BaseRepository<T> : IRepository<T> where T : MongoEntity
    {
        protected readonly IMongoCollection<T> collection;

        protected BaseRepository(IMongoCollection<T> collection)
        {
            this.collection = collection;
        }

        public virtual Task Create(T entity, CancellationToken cancellationToken = default)
        {
            return collection.InsertOneAsync(entity, null, cancellationToken);
        }

        public virtual Task<T> Get(string id, CancellationToken cancellationToken = default)
        {
            return collection.Find(c => c.Id == id).SingleOrDefaultAsync(cancellationToken);
        }

        public virtual Task<P> Get<P>(string id, Expression<Func<T, P>> projection, CancellationToken cancellationToken = default)
        {
            return collection.Find(c => c.Id == id).Project(Builders<T>.Projection.Expression(projection)).SingleOrDefaultAsync(cancellationToken);
        }

        public virtual Task<List<P>> Query<P>(Expression<Func<T, bool>> selection, Expression<Func<T, P>> projection, uint skip = 0, uint take = 50, CancellationToken cancellationToken = default)
        {
            return collection
                .Find(selection)
                .Project(Builders<T>.Projection.Expression(projection))
                .Skip((int)skip)
                .Limit((int)take)
                .ToListAsync(cancellationToken);
        }

        public virtual Task<List<T>> Query(Expression<Func<T, bool>> selection,  uint skip = 0, uint take = 50, CancellationToken cancellationToken = default)
        {
            return collection
            .Find(selection)
            .Skip((int)skip)
            .Limit((int)take)
            .ToListAsync(cancellationToken);
        }

        public virtual Task Update(T entity, CancellationToken cancellationToken = default)
        {
            return collection.ReplaceOneAsync(e => e.Id == entity.Id, entity, new ReplaceOptions(), cancellationToken);
        }

        public virtual async Task<bool> Delete(string id, CancellationToken cancellationToken = default)
        {
            var result = await collection.DeleteOneAsync(e => e.Id == id, new DeleteOptions(), cancellationToken);
            return result.DeletedCount == 1;
        }
    }
}
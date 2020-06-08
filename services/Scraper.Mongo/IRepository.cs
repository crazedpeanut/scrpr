using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Scraper.Mongo
{
    public interface IRepository<T> where T : MongoEntity
    {
        Task Create(T entity, CancellationToken cancellationToken = default);
        Task<T> Get(string id, CancellationToken cancellationToken = default);
        Task<P> Get<P>(string id, Expression<Func<T, P>> projection, CancellationToken cancellationToken = default);
        Task<List<P>> Query<P>(Expression<Func<T, bool>> selection, Expression<Func<T, P>> projection, uint skip = 0, uint take = 50, CancellationToken cancellationToken = default);
        Task<List<T>> Query(Expression<Func<T, bool>> selection, uint skip = 0, uint take = 50, CancellationToken cancellationToken = default);
        Task Update(T entity, CancellationToken cancellationToken = default);
        Task<bool> Delete(string id, CancellationToken cancellationToken = default);
    }
}
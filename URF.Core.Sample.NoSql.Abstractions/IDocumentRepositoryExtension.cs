using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using URF.Core.Abstractions;

namespace URF.Core.Sample.NoSql.Abstractions
{
    public interface IDocumentRepositoryExtension<TEntity> : IDocumentRepository<TEntity> where TEntity : class
    {
        Task<TEntity> FindOneAsync(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default);
        Task<TEntity> FindOneAndUpdateAsync(Expression<Func<TEntity, bool>> filter, UpdateDefinition<TEntity> update, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateOneAsync(Expression<Func<TEntity, bool>> filter, UpdateDefinition<TEntity> update, UpdateOptions updateOptions, CancellationToken cancellationToken = default);
        Task<TEntity> FindOneAndUpdateAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update, CancellationToken cancellationToken = default);
        Task<int> DeleteOneAsync(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default);

    }
}

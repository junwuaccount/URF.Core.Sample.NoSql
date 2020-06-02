using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using URF.Core.Mongo;
using URF.Core.Sample.NoSql.Abstractions;

namespace URF.Core.Sample.NoSql.Api
{
    public class DocumentRepositoryExtension<TEntity> : DocumentRepository<TEntity>, IDocumentRepositoryExtension<TEntity> where TEntity : class
    {
        public DocumentRepositoryExtension(IMongoCollection<TEntity> collection) : base(collection)
        {

        }

        public virtual async Task<TEntity> FindOneAsync(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default)
            => await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        public virtual async Task<TEntity> FindOneAndUpdateAsync(Expression<Func<TEntity, bool>> filter, UpdateDefinition<TEntity> update, CancellationToken cancellationToken = default)
        {
            await Collection.FindOneAndUpdateAsync(filter, update, null, cancellationToken);
            return await FindOneAsync(filter, cancellationToken);
        }

        public virtual async Task<TEntity> FindOneAndUpdateAsync(Expression<Func<TEntity, bool>> filter, UpdateDefinition<TEntity> update, FindOneAndUpdateOptions<TEntity, TEntity> updateOptions, CancellationToken cancellationToken = default)
        {
            await Collection.FindOneAndUpdateAsync(filter, update, updateOptions, cancellationToken);
            return await FindOneAsync(filter, cancellationToken);
        }

        public virtual async Task<TEntity> FindOneAndUpdateAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update, CancellationToken cancellationToken = default)
        {
            await Collection.FindOneAndUpdateAsync(filter, update, null, cancellationToken);
            return await FindOneAsync(filter, cancellationToken);
        }

        public virtual async Task<int> DeleteOneAsync(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default)
        {
            var result = await Collection.DeleteOneAsync(filter, cancellationToken);
            return (int)result.DeletedCount;
        }

        public virtual async Task<TEntity> UpdateOneAsync(Expression<Func<TEntity, bool>> filter, UpdateDefinition<TEntity> update, UpdateOptions updateOptions, CancellationToken cancellationToken = default)
        {
            await Collection.UpdateOneAsync(filter, update, updateOptions, cancellationToken);
            return await FindOneAsync(filter, cancellationToken);
        }
    }
}

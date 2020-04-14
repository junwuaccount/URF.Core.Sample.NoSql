using URF.Core.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Abstractions
{
    public interface IBookstoreUnitOfWork
    {
        public IDocumentRepositoryExtension<Author> AuthorsRepository { get; }

        public IDocumentRepositoryExtension<Book> BooksRepository { get; }
    }
}

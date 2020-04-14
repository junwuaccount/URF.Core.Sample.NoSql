using URF.Core.Abstractions;
using URF.Core.Sample.NoSql.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Mongo
{
    public class BookstoreUnitOfWork : IBookstoreUnitOfWork
    {
        public BookstoreUnitOfWork(IDocumentRepositoryExtension<Author> authorsRepository,
            IDocumentRepositoryExtension<Book>booksRepository)
        {
            AuthorsRepository = authorsRepository;
            BooksRepository = booksRepository;
        }

        public IDocumentRepositoryExtension<Author> AuthorsRepository { get; }

        public IDocumentRepositoryExtension<Book> BooksRepository { get; }
    }
}

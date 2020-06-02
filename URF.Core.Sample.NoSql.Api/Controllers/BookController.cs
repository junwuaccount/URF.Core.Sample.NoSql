using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using URF.Core.Mongo;
using URF.Core.Sample.NoSql.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public IBookstoreUnitOfWork UnitOfWork { get; }

        public BookController(IBookstoreUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        // GET: api/Book
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get()
        {
            var result = await UnitOfWork.BooksRepository
                .Queryable()
                .OrderBy(e => e.BookName)
                .ToListAsync();
            return Ok(result);
        }

        // GET: api/Book/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(string id)
        {
            var result = await UnitOfWork.BooksRepository.FindOneAsync(e => e.Id == id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/Book
        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromBody] Book value)
        {
            var result = await UnitOfWork.BooksRepository.InsertOneAsync(value);
            return CreatedAtAction(nameof(Get), new { id = value.Id }, result);
        }

        // PUT: api/Book/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Put(string id, [FromBody] Book value)
        {
            if (string.Compare(id, value.Id, true) != 0) return BadRequest();
            var result = await UnitOfWork.BooksRepository.FindOneAndReplaceAsync(e => e.Id == id, value);
            return Ok(result);
        }

        // PUT: api/Book/AddReviewer/5e94940cf9ccc34df04c9e74/
        [HttpPut("AddReviewer/{id}")]
        public async Task<ActionResult<Book>> AddReviewer(string id, string reviewerName, [FromBody] Reviewer value)
        {
            var update = Builders<Book>.Update.Push<Reviewer>(e => e.Reviewers, value);
            var result = await UnitOfWork.BooksRepository.FindOneAndUpdateAsync(e => e.Id == id, update);
            return Ok(result);
        }

        // PUT: api/Book/AddReviewerPublisher/5e94940cf9ccc34df04c9e74/Aho Ulm
        [HttpPut("AddReviewerPublisher/{id}/{reviewerName}")]
        public async Task<ActionResult<Book>> AddReviewerPublisher(string id, string reviewerName, [FromBody] Publisher value)
        {
            var filter = Builders<Book>.Filter;
            var bookReviewerFilter = filter.And(
              filter.Eq(x => x.Id, id),
              filter.ElemMatch(x => x.Reviewers, c => c.Name == reviewerName));

            // update with positional operator
            var update = Builders<Book>.Update;
            var reviewerPublisherSetter = update.Push("Reviewers.$.Publishers", value);
            var result = await UnitOfWork.BooksRepository.FindOneAndUpdateAsync(bookReviewerFilter, reviewerPublisherSetter);
            return Ok(result);
        }

        // PUT: api/Book/AddReviewerPublisher/5e94940cf9ccc34df04c9e74/Aho Ulm
        [HttpPut("AddReviewerPublishers/{id}/{reviewerName}")]
        public async Task<ActionResult<Book>> AddReviewerPublishers(string id, string reviewerName, [FromBody] List<Publisher> values)
        {
            var filter = Builders<Book>.Filter;
            var bookReviewerFilter = filter.And(
              filter.Eq(x => x.Id, id),
              filter.ElemMatch(x => x.Reviewers, c => c.Name == reviewerName));

            // update with positional operator
            var update = Builders<Book>.Update;
            var reviewerPublisherSetter = update.PushEach("Reviewers.$.Publishers", values);
            var result = await UnitOfWork.BooksRepository.FindOneAndUpdateAsync(bookReviewerFilter, reviewerPublisherSetter);
            return Ok(result);
        }

        // POST: api/Book/AddReviewerPublisher/5e94940cf9ccc34df04c9e74/Aho Ulm
        [HttpPost("UpdateReviewerPublisher/{id}/{reviewerName}")]
        public async Task<ActionResult<Book>> UpdateReviewerPublisher(string id, string reviewerName, [FromBody] Publisher value)
        {
            var update = Builders<Book>.Update.Set("Reviewers.$[r].Publishers.$[p]", value);
            var updateOptions =  new UpdateOptions
            {
                ArrayFilters = new List<ArrayFilterDefinition>
                {
                    new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("r.Name", reviewerName)),
                    new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("p.Name", value.Name))
                }
            };
            var result = await UnitOfWork.BooksRepository.UpdateOneAsync(x => x.Id == id,
                update, updateOptions);
            return Ok(result);
        }

        // DELETE: api/Book/DeleteReviewerPublisher/5e94940cf9ccc34df04c9e74/Aho Ulm/Addison Wesley
        [HttpDelete("DeleteReviewerPublisher/{id}/{reviewerName}/{publisherName}")]
        public async Task<ActionResult<Book>> DeleteReviewerPublisher(string id, string reviewerName, string publisherName)
        {
            var filter = Builders<Book>.Filter;
            var bookReviewerFilter = filter.And(
              filter.Eq(x => x.Id, id),
              filter.ElemMatch(x => x.Reviewers, c => c.Name == reviewerName));

            //var filter = new BsonDocument("_id", ObjectId.Parse(id));

            var updateValues = Builders<Book>.Update.PullFilter("Reviewers.0.Publishers",
                Builders<Publisher>.Filter.Eq("Name", publisherName));
            var result = await UnitOfWork.BooksRepository.FindOneAndUpdateAsync(bookReviewerFilter, updateValues);

            //var update = Builders<Book>.Update.Pull("Reviewers.$[r].Publishers.$[p].Name", publisherName);
            //var updateOptions = new UpdateOptions
            //{
            //    ArrayFilters = new List<ArrayFilterDefinition>
            //    {
            //        new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("r.Name", reviewerName)),
            //        //new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("p.Name", publisherName))
            //    }
            //};

            //var result = await UnitOfWork.BooksRepository.UpdateOneAsync(x => x.Id == id,
            //     update, updateOptions);
            return Ok(result);
        }

            // PUT: api/Book/UpdateReviewer/5
        [HttpPut("UpdateReviewer/{id}")]
        public async Task<ActionResult<Book>> UpdateReviewer(string id, [FromBody] Reviewer value)
        {
            var filter = Builders<Book>.Filter;
            var bookReviewerFilter = filter.And(
              filter.Eq(x => x.Id, id),
              filter.ElemMatch(x => x.Reviewers, c => c.Name == value.Name));

            // update with positional operator
            var update = Builders<Book>.Update;
            var reviewerSetter = update.Set("Reviewers.$.Institute", value.Institute);
            var result = await UnitOfWork.BooksRepository.FindOneAndUpdateAsync(bookReviewerFilter, reviewerSetter);
            return Ok(result);
        }

        // DELETE: api/Book/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var count = await UnitOfWork.BooksRepository.DeleteOneAsync(e => e.Id == id);
            if (count == 0) return NotFound();
            return NoContent();
        }

        // DELETE: api/Book/5/Reviewer/Jamse Wood
        [HttpDelete("{id}/Reviewer/{name}")]
        public async Task<IActionResult> DeleteReviewer(string id, string name)
        {
            var update = Builders<Book>.Update.PullFilter(p => p.Reviewers,
                                                f => f.Name == name);
            var result = await UnitOfWork.BooksRepository.FindOneAndUpdateAsync(p => p.Id == id, update);
            return Ok(result);
        }
    }
}
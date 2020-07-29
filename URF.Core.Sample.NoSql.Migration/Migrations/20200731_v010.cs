using System;
using System.Collections.Generic;
using System.Text;
using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Migration.Migrations
{
    public class v010_migration : DatabaseMigration
    {
        public v010_migration() : base("0.1.0")
        {
        }

        public override void Up(IMongoDatabase db)
        {
            string bookId = "5e94940cf9ccc34df04c9e74";
            string reviewerName = "Aho Ulm";

            var bookCollection = db.GetCollection<Book>("Books");

            List<Publisher> publishers = new List<Publisher>();
            publishers.Add(new Publisher()
            {
                Address = "4569 Broadway Blvd",
                Name = "Publisher X1",
                Inserted = DateTime.Now
            });

            publishers.Add(new Publisher()
            {
                Address = "4569 Beltline Dr",
                Name = "Publisher X2",
                Inserted = DateTime.Now
            });

            var filter = Builders<Book>.Filter;
            var bookReviewerFilter = filter.And(
              filter.Eq(x => x.Id, bookId),
              filter.ElemMatch(x => x.Reviewers, c => c.Name == reviewerName));

            // update with positional operator
            var update = Builders<Book>.Update;
            var reviewerPublisherSetter = update.PushEach("Reviewers.$.Publishers", publishers);
            var result = bookCollection.UpdateOne(bookReviewerFilter, reviewerPublisherSetter);

            //delete publisher
            string publisherName = "Addison Wesley";
            filter = Builders<Book>.Filter;
            bookReviewerFilter = filter.And(
              filter.Eq(x => x.Id, bookId),
              filter.ElemMatch(x => x.Reviewers, c => c.Name == reviewerName));

            //var filter = new BsonDocument("_id", ObjectId.Parse(id));

            var updateValues = Builders<Book>.Update.PullFilter("Reviewers.0.Publishers",
                Builders<Publisher>.Filter.Eq("Name", publisherName));
            result = bookCollection.UpdateOne(bookReviewerFilter, updateValues);

        }

        public override void Down(IMongoDatabase db)
        {
            string bookId = "5e94940cf9ccc34df04c9e74";
            string reviewerName = "Aho Ulm";

            var bookCollection = db.GetCollection<Book>("Books");

            //delete publisher
            
            var filter = Builders<Book>.Filter;
            var bookReviewerFilter = filter.And(
              filter.Eq(x => x.Id, bookId),
              filter.ElemMatch(x => x.Reviewers, c => c.Name == reviewerName));

            //var filter = new BsonDocument("_id", ObjectId.Parse(id));

            string publisherName = "Publisher X1";
            var updateValues = Builders<Book>.Update.PullFilter("Reviewers.0.Publishers",
                Builders<Publisher>.Filter.Eq("Name", publisherName));
            var result = bookCollection.UpdateOne(bookReviewerFilter, updateValues);

            publisherName = "Publisher X2";
            updateValues = Builders<Book>.Update.PullFilter("Reviewers.0.Publishers",
                Builders<Publisher>.Filter.Eq("Name", publisherName));
            result = bookCollection.UpdateOne(bookReviewerFilter, updateValues);

            //Add deleted publisher back
            // update with positional operator
            var update = Builders<Book>.Update;

            var publisher = new Publisher()
            {
                Address = "4569 Broadway Blvd",
                Name = "3778 Main St",
                Inserted = DateTime.Now
            };

            var reviewerPublisherSetter = update.Push("Reviewers.$.Publishers", publisher);
            result = bookCollection.UpdateOne(bookReviewerFilter, reviewerPublisherSetter);

        }
    }
}

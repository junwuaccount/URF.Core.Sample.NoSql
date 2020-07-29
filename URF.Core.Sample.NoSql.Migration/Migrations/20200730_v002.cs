using System;
using System.Collections.Generic;
using System.Text;
using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Migration.Migrations
{
    public class v002_migration : DatabaseMigration
    {
        public v002_migration() : base("0.0.2")
        {
        }

        public override void Up(IMongoDatabase db)
        {
            //Update Book Reviewer's Institute
            var bookCollection = db.GetCollection<Book>("Books");

            string bookName= "CLR via C#";

            Reviewer reviewer = new Reviewer()
            {
                Name = "Aho Ulm",
                Institute = "Institute 9"
            };

            var filter = Builders<Book>.Filter;
            var bookReviewerFilter = filter.And(
              filter.Eq(x => x.BookName, bookName),
              filter.ElemMatch(x => x.Reviewers, c => c.Name == reviewer.Name));

            // update with positional operator
            var update = Builders<Book>.Update;
            var reviewerSetter = update.Set("Reviewers.$.Institute", reviewer.Institute).Set("Updated", DateTime.Now);

            bookCollection.UpdateOne(bookReviewerFilter, reviewerSetter);
        }

        public override void Down(IMongoDatabase db)
        {
            var bookCollection = db.GetCollection<Book>("Books");
            string bookName = "CLR via C#";

            //Update Reviewer back to old value
            Reviewer reviewer = new Reviewer()
            {
                Name = "Aho Ulm",
                Institute = "McGrow"
            };

            var filter = Builders<Book>.Filter;
            var bookReviewerFilter = filter.And(
               filter.Eq(x => x.BookName, bookName),
              filter.ElemMatch(x => x.Reviewers, c => c.Name == reviewer.Name));

            // update with positional operator
            var update = Builders<Book>.Update;
            var reviewerSetter = update.Set("Reviewers.$.Institute", reviewer.Institute).Set("Updated", DateTime.Now);

            bookCollection.UpdateOne(bookReviewerFilter, reviewerSetter);
        }
    }
}

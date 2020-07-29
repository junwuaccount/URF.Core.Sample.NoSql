using System;
using System.Collections.Generic;
using System.Text;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Migration.Migrations
{
    public class v001_migration : DatabaseMigration
    {
        public v001_migration() : base("0.0.1")
        {
        }

        public override void Up(IMongoDatabase db)
        {
            //add authors
            var authorCollection = db.GetCollection<Author>("Authors");
            authorCollection.InsertOne(new Author
            {
                Country = "US",
                Name = "Mark Twin",
                Inserted = DateTime.Now
            });
            authorCollection.InsertOne(new Author
            {
                Country = "Britain",
                Name = "Maum Kiln",
                Inserted = DateTime.Now
            });

            //add reviewers
            var bookCollection = db.GetCollection<Book>("Books");
            //find book document to add reviewers
            var filter = Builders<Book>.Filter.Eq(c => c.BookName, "CLR via C#");

            List<Publisher> publishers = new List<Publisher>();
            publishers.Add(new Publisher()
            {
                Address = "123 Main St, Dallas, TX 75234",
                Name = "Publisher 1",
                Inserted = DateTime.Now

            });

            Reviewer reviewer = new Reviewer()
            {
                Inserted = DateTime.Now,
                Name = "Review 3",
                Institute = "Institute 1",
                Publishers = publishers
            };

            var update = Builders<Book>.Update.Push<Reviewer>(e => e.Reviewers, reviewer);
            bookCollection.UpdateOne(filter, update);
        }

        public override void Down(IMongoDatabase db)
        {
            //delete added authors to rollback
            var authorCollection = db.GetCollection<Author>("Authors");
            authorCollection.DeleteMany(Builders<Author>.Filter.In(c => c.Name, new[] { "Mark Twin", "Maum Kiln" }));

            var bookCollection = db.GetCollection<Book>("Books");
            //find book document to add reviewers
            var filter = Builders<Book>.Filter.Eq(c => c.BookName, "CLR via C#");

            var update = Builders<Book>.Update.PullFilter(p => p.Reviewers,
                                               f => f.Name == "Review 3");
            bookCollection.UpdateOne(filter, update);
        }
    }
}

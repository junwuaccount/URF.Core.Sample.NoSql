using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace URF.Core.Sample.NoSql.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string BookName { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }

        public List<Reviewer> Reviewers { get; set; }
    }

    public class Publisher
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class Reviewer
    {
        public string Name { get; set; }

        public string Institute { get; set; }

        public List<Publisher> Publishers { get; set; }
    }

}

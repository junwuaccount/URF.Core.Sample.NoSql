﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace URF.Core.Sample.NoSql.Models
{
    public class Author
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }
        public DateTime Inserted { get; set; }
        public DateTime Updated { get; set; }
    }
}

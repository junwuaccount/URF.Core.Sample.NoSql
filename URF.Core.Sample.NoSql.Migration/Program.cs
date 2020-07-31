using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.Static;
using MongoDB.Bson;
using MongoDB.Driver;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb+srv://junwuaccount:pa$$w0rd@hiltitest-xxxxx.mongodb.net/test?retryWrites=true&w=majority");

            // Init MongoMigration
            MongoMigrationClient.Initialize(
                client,
                new MongoMigrationSettings()
                {
                    ConnectionString = "mongodb+srv://junwuaccount:pa$$w0rd@hiltitest-xxxx.mongodb.net/test?retryWrites=true&w=majority",
                    Database = "BookstoreDb",
                    DatabaseMigrationVersion = new DocumentVersion(0, 0, 0) //set to old version to roll back to by apply appled migration reversely
                },
                new LightInjectAdapter(new LightInject.ServiceContainer()));

            Console.WriteLine("Apply database migrations: ");
            Console.WriteLine("\n");

            var migrationsCollection = client.GetDatabase("BookstoreDb").GetCollection<BsonDocument>("_migrations");
            var migrations = migrationsCollection.FindAsync(_ => true).Result.ToListAsync().Result;
            migrations.ForEach(r => Console.WriteLine(r + "\n"));
        }
    }
}

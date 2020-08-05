using System;
using System.IO;
using Microsoft.Extensions.Configuration;
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
        private static IConfiguration _appConfig;

        private static IConfiguration LoadConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables();
            var appConfig = builder.Build();

            return appConfig;
        }

        static void Main(string[] args)
        {
            string migrateToVersion = null;

            if (args.Length > 0)
                migrateToVersion = args[0];

            _appConfig = LoadConfiguration();

            var dbConnStr = _appConfig["DBConnectionString"];
            var client = new MongoClient(dbConnStr);

            // Init MongoMigration
            MongoMigrationSettings migrationSettings = null;

            if (!string.IsNullOrEmpty(migrateToVersion))
                migrationSettings = new MongoMigrationSettings()
                {
                    ConnectionString = dbConnStr,
                    Database = "BookstoreDb",
                    DatabaseMigrationVersion = new DocumentVersion(migrateToVersion) //set to old version to roll back to by apply appled migration reversely
                };
            else
                migrationSettings = new MongoMigrationSettings()
                {
                    ConnectionString = dbConnStr,
                    Database = "BookstoreDb",
                };

            MongoMigrationClient.Initialize(
                client,
                migrationSettings,
                new LightInjectAdapter(new LightInject.ServiceContainer()));

            Console.WriteLine("Apply database migrations: ");
            Console.WriteLine("\n");

            var migrationsCollection = client.GetDatabase("BookstoreDb").GetCollection<BsonDocument>("_migrations");
            var migrations = migrationsCollection.FindAsync(_ => true).Result.ToListAsync().Result;
            migrations.ForEach(r => Console.WriteLine(r + "\n"));
        }
    }
}

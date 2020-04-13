using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using CourseLibrary.API.Models;
using CourseLibrary.API.Entities;

namespace CourseLibrary.API.DbContext
{
    public class CourseLibraryContext : ICourseLibraryContext
    {
        private readonly IMongoDatabase _db;
        //private readonly MongoDBSetting _options;

        public CourseLibraryContext(IOptions<MongoDBSetting> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _db = client.GetDatabase(options.Value.Database);
        }

        public IMongoCollection<Author> Author => _db.GetCollection<Author>("authors");
    }
}

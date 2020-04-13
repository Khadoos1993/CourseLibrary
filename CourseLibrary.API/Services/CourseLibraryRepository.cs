using CourseLibrary.API.DbContext;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Services
{
    public class CourseLibraryRepository: ICourseLibraryRepository
    {
        private readonly ICourseLibraryContext _courseLibraryContext;

        public CourseLibraryRepository(ICourseLibraryContext courseLibraryContext)
        {
            _courseLibraryContext = courseLibraryContext;
        }

        public IEnumerable<Author> GetAuthors()
        {
            var authors =  _courseLibraryContext.Author.Find(_ => true).ToEnumerable();
            return authors;
        }

        public IEnumerable<Author> GetAuthors(string mainCategory, string searchQuery)
        {
            if (string.IsNullOrEmpty(mainCategory) && string.IsNullOrEmpty(searchQuery))
                return GetAuthors();
            var collection = _courseLibraryContext.Author as IQueryable<Author>;
            if (!string.IsNullOrEmpty(mainCategory))
                return collection.Where(_ => _.MainCategory == mainCategory.Trim());
            if (!string.IsNullOrEmpty(searchQuery))
                return collection.Where(_ => _.MainCategory.Contains(searchQuery)
                || _.FirstName.Contains(searchQuery)
                || _.LastName.Contains(searchQuery));

            return collection.ToList();
        }

        public void CreateAuthor(Author author)
        {
            _courseLibraryContext.Author.InsertOne(author);
        }

        public Author GetAuthor(ObjectId id)
        {
            return _courseLibraryContext.Author.Find(_ => _.Id== id).FirstOrDefault();
        }

        public void UpdateAuthor(ObjectId id, Author authorFromRepo)
        {
            //FilterDefinition<Author> filterDefinition = Builders<Author>.Filter.Eq(m => m.Id, id);
            //UpdateDefinition<Author> updateDefinition = Builders<Author>.Update.Set(a => a.LastName, authorFromRepo.LastName);
            _courseLibraryContext.Author.ReplaceOne(x => x.Id == id, authorFromRepo);
        }

        public void DeleteAuthor(ObjectId objectId)
        {
            FilterDefinition<Author> filterDefinition = Builders<Author>.Filter.Eq(m => m.Id, objectId);

            _courseLibraryContext.Author.DeleteOne(filterDefinition);
        }
    }
}

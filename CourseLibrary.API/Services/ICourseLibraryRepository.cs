using System.Collections.Generic;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CourseLibrary.API.Services
{
    public interface ICourseLibraryRepository
    {
        IEnumerable<Author> GetAuthors();
        IEnumerable<Author> GetAuthors(string mainCategory, string searchQuery);
        Author GetAuthor(ObjectId id);
        void CreateAuthor(Author author);
        void UpdateAuthor(ObjectId id,Author authorFromRepo);
        void DeleteAuthor(ObjectId objectId);
    }
}

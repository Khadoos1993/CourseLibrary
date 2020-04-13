using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using MongoDB.Driver;

namespace CourseLibrary.API.DbContext
{
    public interface ICourseLibraryContext
    {
        IMongoCollection<Author> Author { get; }
    }
}
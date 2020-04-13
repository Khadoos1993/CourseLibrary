using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    public class CourseDto
    {

        public ObjectId CourseId { get; set; }

        public string Title { get; set; }

       
        public string Description { get; set; }

        public ObjectId AuthorId { get; set; }
    }
}

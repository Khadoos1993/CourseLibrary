using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Entities
{
    public class Author
    {
        [BsonElement("_id")]
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("firstName")]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [BsonElement("lastName")]

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [BsonElement("dateOfBirth")]
        public string DateOfBirth { get; set; }


        [BsonElement("dateOfDeath")]
        public DateTimeOffset? DateOfDeath { get; set; }


        [Required]
        [BsonElement("mainCategory")]

        [MaxLength(50)]
        public string MainCategory { get; set; }

        //public ICollection<Course> Courses { get; set; }
           // = new List<Course>();
    }
}


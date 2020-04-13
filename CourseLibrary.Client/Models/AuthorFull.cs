using System;
using System.Collections.Generic;
using System.Text;

namespace CourseLibrary.Client.Models
{
    public class AuthorFull
    {
        public string Id { get; set; }

        public string FirstName { get; set; }


        public string LastName { get; set; }


        public DateTimeOffset DateOfBirth { get; set; }


        public string MainCategory { get; set; }
    }
}

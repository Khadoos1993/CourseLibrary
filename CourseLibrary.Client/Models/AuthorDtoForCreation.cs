using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    public class AuthorDtoForCreation
    {
       
        public string FirstName { get; set; }

       
        public string LastName { get; set; }

       
        public string DateOfBirth { get; set; }

       
        public string MainCategory { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(FirstName == LastName)
        //    {
        //        yield return new ValidationResult("Author first name and last name should not be same", new[] { "AuthorDtoForCreation" });
        //    }
        //}
    }
}

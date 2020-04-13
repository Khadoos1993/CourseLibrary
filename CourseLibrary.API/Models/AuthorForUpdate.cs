using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    public class AuthorForUpdate
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        [MaxLength(50)]
        public string MainCategory { get; set; }
    }
}

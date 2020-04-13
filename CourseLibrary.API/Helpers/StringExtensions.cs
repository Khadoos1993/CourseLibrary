using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Helpers
{
    public static class StringExtensions
    {
        public static int GetCurrentAge(this string dateTime)
        {
            if (!DateTime.TryParse(dateTime, out DateTime dateOfBirth))
                dateOfBirth = DateTime.UtcNow;
            var currentDate = DateTime.UtcNow.AddYears(2);
            int age = currentDate.Year - dateOfBirth.Year;
            if (currentDate < dateOfBirth.AddYears(age))
                age--;
            return age;
        }
    }
}

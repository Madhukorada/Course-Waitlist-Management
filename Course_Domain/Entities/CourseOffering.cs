using System;
using System.Collections.Generic;
using System.Text;

namespace Course_Domain.Entities
{
    public class CourseOffering
    {
        public int Id { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public int Capacity { get; set; }
    }
}

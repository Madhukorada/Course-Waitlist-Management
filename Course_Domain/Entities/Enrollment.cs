using System;
using System.Collections.Generic;
using System.Text;

namespace Course_Domain.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int LearnerId { get; set; }

        public int CourseOfferingId { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}

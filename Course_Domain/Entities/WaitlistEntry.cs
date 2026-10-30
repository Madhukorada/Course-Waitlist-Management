using System;
using System.Collections.Generic;
using System.Text;

namespace Course_Domain.Entities
{
    public class WaitlistEntry
    {
        public int Id { get; set; }

        public int LearnerId { get; set; }

        public int CourseOfferingId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Course_Domain.Entities
{
    public class Learner
    {
        public int LearnerId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool HasCompletedPrerequisites { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}

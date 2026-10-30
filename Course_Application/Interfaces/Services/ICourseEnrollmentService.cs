using Course_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course_Application.Interfaces.Services
{
    public interface ICourseEnrollmentService
    {
        string EnrollLearner(int learnerId);

        string DropLearner(int learnerId);
        List<Enrollment> GetEnrolledLearners();
        List<WaitlistEntry> GetWaitlistedLearners();
    }
}

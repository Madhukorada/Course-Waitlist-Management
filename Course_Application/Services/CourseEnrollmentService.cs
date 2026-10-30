
using Course_Application.Interfaces.Repositories;
using Course_Application.Interfaces.Services;
using Course_Domain.Entities;

namespace Course_Application.Services
{
    public class CourseEnrollmentService : ICourseEnrollmentService
    {
        private readonly ICourseRepository _repository;

        private static readonly object _lock = new();

        public CourseEnrollmentService(ICourseRepository repository)
        {
            _repository = repository;
        }

        // API 1: Enroll a learner or add them to the waitlist.
        public string EnrollLearner(int learnerId)
        {
            lock (_lock)
            {
                var learner = _repository.GetLearnerById(learnerId);

                if (learner == null)
                    return "Learner not found.";

                var course = _repository.GetCourse();

                var enrollments = _repository.GetEnrollments();
                var waitlist = _repository.GetWaitlistEntries();

                if (enrollments.Any(e =>
                    e.LearnerId == learnerId &&
                    e.CourseOfferingId == course.Id))
                {
                    return "Learner is already enrolled.";
                }

                if (waitlist.Any(w =>
                    w.LearnerId == learnerId &&
                    w.CourseOfferingId == course.Id))
                {
                    return "Learner is already on the waitlist.";
                }

                if (!learner.HasCompletedPrerequisites)
                {
                    return "Learner has not completed the prerequisites.";
                }

                if (course.Capacity <= 0)
                    return "Course capacity is invalid.";

                if (enrollments.Count(e =>
                    e.CourseOfferingId == course.Id) < course.Capacity)
                {
                    _repository.AddEnrollment(new Enrollment
                    {
                        LearnerId = learnerId,
                        CourseOfferingId = course.Id,
                        EnrolledAt = DateTime.UtcNow
                    });

                    return "Learner enrolled successfully.";
                }

                _repository.AddWaitlistEntry(new WaitlistEntry
                {
                    LearnerId = learnerId,
                    CourseOfferingId = course.Id,
                    AddedAt = DateTime.UtcNow
                });

                return "Course is full. Learner added to the waitlist.";
            }
        }

        // API 2: Drop an enrollment and promote the next eligible learner.
        public string DropLearner(int learnerId)
        {
            lock (_lock)
            {
                var course = _repository.GetCourse();

                var enrollment = _repository.GetEnrollments()
                    .FirstOrDefault(e =>
                        e.LearnerId == learnerId &&
                        e.CourseOfferingId == course.Id);

                if (enrollment == null)
                    return "Learner is not enrolled in this course.";

                _repository.RemoveEnrollment(enrollment);

                var promotedLearner = PromoteNextEligibleLearner(course.Id);

                if (promotedLearner != null)
                {
                    return $"Enrollment dropped successfully. " +
                           $"{promotedLearner.FirstName} has been promoted " +
                           "from the waitlist.";
                }

                return "Enrollment dropped successfully. No eligible " +
                       "learner is available on the waitlist.";
            }
        }

        // API 3: Get all currently enrolled learners.
        public List<Enrollment> GetEnrolledLearners()
        {
            lock (_lock)
            {
                var course = _repository.GetCourse();

                return _repository.GetEnrollments()
                    .Where(e => e.CourseOfferingId == course.Id)
                    .OrderBy(e => e.EnrolledAt)
                    .ThenBy(e => e.Id)
                    .ToList();
            }
        }

        // API 4: Get waitlisted learners in FIFO order.
        public List<WaitlistEntry> GetWaitlistedLearners()
        {
            lock (_lock)
            {
                var course = _repository.GetCourse();

                return _repository.GetWaitlistEntries()
                    .Where(w => w.CourseOfferingId == course.Id)
                    .OrderBy(w => w.AddedAt)
                    .ThenBy(w => w.Id)
                    .ToList();
            }
        }

        // Promote the earliest eligible learner.
        // Ineligible learners remain on the waitlist.
        private Learner? PromoteNextEligibleLearner(int courseOfferingId)
        {
            var waitlist = _repository.GetWaitlistEntries()
                .Where(w => w.CourseOfferingId == courseOfferingId)
                .OrderBy(w => w.AddedAt)
                .ThenBy(w => w.Id)
                .ToList();

            foreach (var entry in waitlist)
            {
                var learner = _repository.GetLearnerById(entry.LearnerId);

                if (learner == null ||
                    !learner.HasCompletedPrerequisites)
                {
                    // Skip this entry but retain it on the waitlist.
                    continue;
                }

                _repository.RemoveWaitlistEntry(entry);

                _repository.AddEnrollment(new Enrollment
                {
                    LearnerId = learner.LearnerId,
                    CourseOfferingId = courseOfferingId,
                    EnrolledAt = DateTime.UtcNow
                });

                return learner;
            }

            return null;
        }
    }
}

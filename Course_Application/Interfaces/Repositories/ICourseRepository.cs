using Course_Domain.Entities;

namespace Course_Application.Interfaces.Repositories;

public interface ICourseRepository
{
    CourseOffering GetCourse();

    Learner? GetLearnerById(int learnerId);

    List<Enrollment> GetEnrollments();

    List<WaitlistEntry> GetWaitlistEntries();

    void AddEnrollment(Enrollment enrollment);

    void RemoveEnrollment(Enrollment enrollment);

    void AddWaitlistEntry(WaitlistEntry entry);

    void RemoveWaitlistEntry(WaitlistEntry entry);
}
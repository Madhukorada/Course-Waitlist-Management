using Course_Application.Interfaces.Repositories;
using Course_Domain.Entities;

namespace Course_Infrastructure.Repositories;

public class InMemoryCourseRepository : ICourseRepository
{
    private readonly List<Learner> _learners = new()
    {
        new Learner
        {
            LearnerId = 1,
            FirstName = "Ravi",
            LastName = "Kumar",
            Email = "ravi@example.com",
            HasCompletedPrerequisites = true
        },
        new Learner
        {
            LearnerId = 2,
            FirstName = "Priya",
            LastName = "Sharma",
            Email = "priya@example.com",
            HasCompletedPrerequisites = true
        },
        new Learner
        {
            LearnerId = 3,
            FirstName = "Arun",
            LastName = "Kumar",
            Email = "arun@example.com",
            HasCompletedPrerequisites = false
        },
        new Learner
        {
            LearnerId = 4,
            FirstName = "Sita",
            LastName = "Reddy",
            Email = "sita@example.com",
            HasCompletedPrerequisites = true
        },
        new Learner
        {
            LearnerId = 5,
            FirstName = "Kiran",
            LastName = "Rao",
            Email = "kiran@example.com",
            HasCompletedPrerequisites = true
        }
    };

    private readonly CourseOffering _course = new()
    {
        Id = 101,
        CourseName = "C# Programming",
        Capacity = 2
    };

    private readonly List<Enrollment> _enrollments = new()
    {
        new Enrollment
        {
            Id = 1,
            LearnerId = 1,
            CourseOfferingId = 101
        },
        new Enrollment
        {
            Id = 2,
            LearnerId = 2,
            CourseOfferingId = 101
        }
    };

    private readonly List<WaitlistEntry> _waitlistEntries = new();

    private int _nextEnrollmentId = 3;
    private int _nextWaitlistEntryId = 1;

    public CourseOffering GetCourse()
    {
        return _course;
    }

    public Learner? GetLearnerById(int learnerId)
    {
        return _learners.FirstOrDefault(
            learner => learner.LearnerId == learnerId);
    }

    public List<Enrollment> GetEnrollments()
    {
        return _enrollments
            .Where(e => e.CourseOfferingId == _course.Id)
            .ToList();
    }

    public List<WaitlistEntry> GetWaitlistEntries()
    {
        return _waitlistEntries
            .Where(w => w.CourseOfferingId == _course.Id)
            .OrderBy(w => w.AddedAt)
            .ThenBy(w => w.Id)
            .ToList();
    }

    public void AddEnrollment(Enrollment enrollment)
    {
        enrollment.Id = _nextEnrollmentId++;
        _enrollments.Add(enrollment);
    }

    public void RemoveEnrollment(Enrollment enrollment)
    {
        _enrollments.Remove(enrollment);
    }

    public void AddWaitlistEntry(WaitlistEntry entry)
    {
        entry.Id = _nextWaitlistEntryId++;
        entry.AddedAt = DateTime.UtcNow;

        _waitlistEntries.Add(entry);
    }

    public void RemoveWaitlistEntry(WaitlistEntry entry)
    {
        _waitlistEntries.Remove(entry);
    }
}
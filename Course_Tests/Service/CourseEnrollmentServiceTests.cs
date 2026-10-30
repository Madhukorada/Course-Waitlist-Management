using Course_Application.Interfaces.Repositories;
using Course_Application.Services;
using Course_Domain.Entities;

namespace Course_Tests.Services;

public class CourseEnrollmentServiceTests
{
    private readonly FakeCourseRepository _repository;
    private readonly CourseEnrollmentService _service;

    public CourseEnrollmentServiceTests()
    {
        _repository = new FakeCourseRepository(capacity: 2);
        _service = new CourseEnrollmentService(_repository);
    }

    [Fact]
    public void EnrollLearner_WhenSeatIsAvailable_ShouldEnrollLearner()
    {
        // Act
        var result = _service.EnrollLearner(1);

        // Assert
        Assert.Equal("Learner enrolled successfully.", result);
        Assert.Contains(
            _repository.GetEnrollments(),
            e => e.LearnerId == 1);
    }

    [Fact]
    public void EnrollLearner_WhenCourseIsFull_ShouldAddLearnerToWaitlist()
    {
        // Arrange
        _repository.AddEnrollment(new Enrollment
        {
            LearnerId = 1,
            CourseOfferingId = 101
        });

        _repository.AddEnrollment(new Enrollment
        {
            LearnerId = 2,
            CourseOfferingId = 101
        });

        // Act
        var result = _service.EnrollLearner(5);

        // Assert
        Assert.Equal(
            "Course is full. Learner added to the waitlist.",
            result);

        Assert.Contains(
            _repository.GetWaitlistEntries(),
            w => w.LearnerId == 5);
    }


    [Fact]
    public void EnrollLearner_WhenAlreadyEnrolled_ShouldRejectDuplicate()
    {
        // Arrange
        _repository.AddEnrollment(new Enrollment
        {
            LearnerId = 1,
            CourseOfferingId = 101
        });

        // Act
        var result = _service.EnrollLearner(1);

        // Assert
        Assert.Equal(
    "Learner is already enrolled.",
    result);

        Assert.Single(_repository.GetEnrollments());
    }

    [Fact]
    public void EnrollLearner_WhenAlreadyWaitlisted_ShouldRejectDuplicate()
    {
        // Arrange
        _repository.AddEnrollment(new Enrollment
        {
            LearnerId = 1,
            CourseOfferingId = 101
        });

        _repository.AddEnrollment(new Enrollment
        {
            LearnerId = 2,
            CourseOfferingId = 101
        });

        _repository.AddWaitlistEntry(new WaitlistEntry
        {
            LearnerId = 5,
            CourseOfferingId = 101,
            AddedAt = DateTime.UtcNow
        });

        // Act
        var result = _service.EnrollLearner(5);

        // Assert
        Assert.Equal(
            "Learner is already on the waitlist.",
            result);

        Assert.Single(_repository.GetWaitlistEntries());
    }

    [Fact]
    public void EnrollLearner_WhenPrerequisitesAreIncomplete_ShouldRejectEnrollment()
    {

        // Act
        var result = _service.EnrollLearner(4);

        // Assert
        Assert.Equal(
            "Learner has not completed the prerequisites.",
            result);

        Assert.DoesNotContain(
            _repository.GetEnrollments(),
            e => e.LearnerId == 4);
    }

    [Fact]
    public void EnrollLearner_WhenLearnerDoesNotExist_ShouldReturnNotFoundMessage()
    {
        // Act
        var result = _service.EnrollLearner(999);

        // Assert
        Assert.Equal("Learner not found.", result);
    }

    [Fact]
    public void DropLearner_WhenLearnerIsEnrolled_ShouldRemoveEnrollment()
    {
        // Arrange
        _repository.AddEnrollment(new Enrollment
        {
            LearnerId = 1,
            CourseOfferingId = 101
        });

        // Act
        var result = _service.DropLearner(1);

        // Assert
        Assert.StartsWith("Enrollment dropped successfully.", result);

        Assert.DoesNotContain(
            _repository.GetEnrollments(),
            e => e.LearnerId == 1);
    }

    [Fact]
    public void DropLearner_WhenLearnerIsNotEnrolled_ShouldReturnNotEnrolledMessage()
    {
        // Act
        var result = _service.DropLearner(1);

        // Assert
        Assert.Equal(
            "Learner is not enrolled in this course.",
            result);
    }

    [Fact]
    public void DropLearner_WhenCalledTwice_ShouldRejectSecondDrop()
    {
        // Arrange
        _repository.AddEnrollment(new Enrollment
        {
            LearnerId = 1,
            CourseOfferingId = 101
        });

        // Act
        var firstResult = _service.DropLearner(1);
        var secondResult = _service.DropLearner(1);

        // Assert
        Assert.StartsWith(
            "Enrollment dropped successfully.",
            firstResult);

        Assert.Equal(
            "Learner is not enrolled in this course.",
            secondResult);
    }

    [Fact]
    public void DropLearner_WhenWaitlistExists_ShouldPromoteFirstEligibleLearner()
    {
        // Arrange
        _repository.AddEnrollment(new Enrollment
        {
            LearnerId = 1,
            CourseOfferingId = 101
        });

        _repository.AddWaitlistEntry(new WaitlistEntry
        {
            LearnerId = 2,
            CourseOfferingId = 101,
            AddedAt = DateTime.UtcNow.AddMinutes(-2)
        });

        _repository.AddWaitlistEntry(new WaitlistEntry
        {
            LearnerId = 3,
            CourseOfferingId = 101,
            AddedAt = DateTime.UtcNow.AddMinutes(-1)
        });

        // Act
        var result = _service.DropLearner(1);

        // Assert
        Assert.StartsWith(
            "Enrollment dropped successfully.",
            result);

        Assert.Contains(
            _repository.GetEnrollments(),
            e => e.LearnerId == 2);

        Assert.DoesNotContain(
            _repository.GetWaitlistEntries(),
            w => w.LearnerId == 2);

       
        Assert.Contains(
            _repository.GetWaitlistEntries(),
            w => w.LearnerId == 3);
    }

    [Fact]
    public void DropLearner_WhenFirstWaitlistedLearnerIsIneligible_ShouldPromoteNextEligibleLearner()
    {
        // Arrange
        _repository.AddEnrollment(new Enrollment
        {
            LearnerId = 1,
            CourseOfferingId = 101
        });

        // Learner 4 has not completed prerequisites.
        _repository.AddWaitlistEntry(new WaitlistEntry
        {
            LearnerId = 4,
            CourseOfferingId = 101,
            AddedAt = DateTime.UtcNow.AddMinutes(-2)
        });

        // Learner 5 is eligible.
        _repository.AddWaitlistEntry(new WaitlistEntry
        {
            LearnerId = 5,
            CourseOfferingId = 101,
            AddedAt = DateTime.UtcNow.AddMinutes(-1)
        });

        // Act
        _service.DropLearner(1);

        // Assert
        Assert.Contains(
            _repository.GetEnrollments(),
            e => e.LearnerId == 5);

        // The ineligible learner should remain on the waitlist.
        Assert.Contains(
            _repository.GetWaitlistEntries(),
            w => w.LearnerId == 4);

        Assert.DoesNotContain(
            _repository.GetWaitlistEntries(),
            w => w.LearnerId == 5);
    }

    [Fact]
    public void GetEnrolledLearners_ShouldReturnCurrentCourseEnrollments()
    {
        // Arrange
        _repository.AddEnrollment(new Enrollment
        {
            LearnerId = 1,
            CourseOfferingId = 101
        });

        // Act
        var result = _service.GetEnrolledLearners();

        // Assert
        Assert.Contains(result, e => e.LearnerId == 1);
    }

    [Fact]
    public void GetWaitlistedLearners_ShouldReturnWaitlistEntries()
    {
        // Arrange
        _repository.AddWaitlistEntry(new WaitlistEntry
        {
            LearnerId = 5,
            CourseOfferingId = 101,
            AddedAt = DateTime.UtcNow
        });

        // Act
        var result = _service.GetWaitlistedLearners();

        // Assert
        Assert.Contains(result, w => w.LearnerId == 5);
    }

    // Test-only repository.
    private sealed class FakeCourseRepository : ICourseRepository
    {
        private readonly CourseOffering _course;
        private readonly List<Learner> _learners;
        private readonly List<Enrollment> _enrollments = [];
        private readonly List<WaitlistEntry> _waitlistEntries = [];

        private int _nextEnrollmentId = 1;
        private int _nextWaitlistId = 1;

        public FakeCourseRepository(int capacity)
        {
            _course = new CourseOffering
            {
                Id = 101,
                CourseName = "C# Programming",
                Capacity = capacity
            };

            _learners =
            [
                new Learner
                {
                    LearnerId = 1,
                    FirstName = "Ravi",
                    HasCompletedPrerequisites = true
                },
                new Learner
                {
                    LearnerId = 2,
                    FirstName = "Priya",
                    HasCompletedPrerequisites = true
                },
                new Learner
                {
                    LearnerId = 3,
                    FirstName = "Arun",
                    HasCompletedPrerequisites = true
                },
                new Learner
                {
                    LearnerId = 4,
                    FirstName = "Sita",
                    HasCompletedPrerequisites = false
                },
                new Learner
                {
                    LearnerId = 5,
                    FirstName = "Kiran",
                    HasCompletedPrerequisites = true
                }
            ];
        }

        public CourseOffering GetCourse()
        {
            return _course;
        }

        public Learner? GetLearnerById(int learnerId)
        {
            return _learners.FirstOrDefault(
                l => l.LearnerId == learnerId);
        }

        public List<Enrollment> GetEnrollments()
        {
            return _enrollments
                .Select(e => new Enrollment
                {
                    Id = e.Id,
                    LearnerId = e.LearnerId,
                    CourseOfferingId = e.CourseOfferingId,
                    EnrolledAt = e.EnrolledAt
                })
                .ToList();
        }

        public List<WaitlistEntry> GetWaitlistEntries()
        {
            return _waitlistEntries
                .Select(w => new WaitlistEntry
                {
                    Id = w.Id,
                    LearnerId = w.LearnerId,
                    CourseOfferingId = w.CourseOfferingId,
                    AddedAt = w.AddedAt
                })
                .ToList();
        }

        public void AddEnrollment(Enrollment enrollment)
        {
            enrollment.Id = _nextEnrollmentId++;

            _enrollments.Add(new Enrollment
            {
                Id = enrollment.Id,
                LearnerId = enrollment.LearnerId,
                CourseOfferingId = enrollment.CourseOfferingId,
                EnrolledAt = enrollment.EnrolledAt
            });
        }

        public void RemoveEnrollment(Enrollment enrollment)
        {
            var existing = _enrollments.FirstOrDefault(
                e => e.Id == enrollment.Id);

            if (existing != null)
            {
                _enrollments.Remove(existing);
            }
        }

        public void AddWaitlistEntry(WaitlistEntry entry)
        {
            entry.Id = _nextWaitlistId++;

            _waitlistEntries.Add(new WaitlistEntry
            {
                Id = entry.Id,
                LearnerId = entry.LearnerId,
                CourseOfferingId = entry.CourseOfferingId,
                AddedAt = entry.AddedAt
            });
        }

        public void RemoveWaitlistEntry(WaitlistEntry entry)
        {
            var existing = _waitlistEntries.FirstOrDefault(
                w => w.Id == entry.Id);

            if (existing != null)
            {
                _waitlistEntries.Remove(existing);
            }
        }
    }
}
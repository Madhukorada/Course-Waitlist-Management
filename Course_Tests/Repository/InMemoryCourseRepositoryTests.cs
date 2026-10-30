using Course_Domain.Entities;
using Course_Infrastructure.Repositories;

namespace Course_Tests.Repositories;

public class InMemoryCourseRepositoryTests
{
    private readonly InMemoryCourseRepository _repository = new();

    [Fact]
    public void GetCourse_ShouldReturnConfiguredCourse()
    {
        // Act
        var course = _repository.GetCourse();

        // Assert
        Assert.NotNull(course);
        Assert.Equal(101, course.Id);
        Assert.Equal("C# Programming", course.CourseName);
        Assert.True(course.Capacity > 0);
    }

    [Fact]
    public void GetLearnerById_WhenLearnerExists_ShouldReturnLearner()
    {
        // Act
        var learner = _repository.GetLearnerById(1);

        // Assert
        Assert.NotNull(learner);
        Assert.Equal(1, learner.LearnerId);
    }

    [Fact]
    public void GetLearnerById_WhenLearnerDoesNotExist_ShouldReturnNull()
    {
        // Act
        var learner = _repository.GetLearnerById(999);

        // Assert
        Assert.Null(learner);
    }

    [Fact]
    public void AddEnrollment_ShouldStoreEnrollment()
    {
        // Arrange
        var enrollment = new Enrollment
        {
            LearnerId = 5,
            CourseOfferingId = 101,
            EnrolledAt = DateTime.UtcNow
        };

        // Act
        _repository.AddEnrollment(enrollment);

        // Assert
        var enrollments = _repository.GetEnrollments();

        Assert.Contains(
            enrollments,
            e => e.LearnerId == 5 &&
                 e.CourseOfferingId == 101);

        Assert.True(enrollment.Id > 0);
    }

    [Fact]
    public void RemoveEnrollment_ShouldRemoveSpecifiedEnrollment()
    {
        // Arrange
        var enrollment = new Enrollment
        {
            LearnerId = 5,
            CourseOfferingId = 101,
            EnrolledAt = DateTime.UtcNow
        };

        _repository.AddEnrollment(enrollment);

        // Act
        _repository.RemoveEnrollment(enrollment);

        // Assert
        Assert.DoesNotContain(
            _repository.GetEnrollments(),
            e => e.Id == enrollment.Id);
    }

    [Fact]
    public void AddWaitlistEntry_ShouldStoreWaitlistEntry()
    {
        // Arrange
        var entry = new WaitlistEntry
        {
            LearnerId = 5,
            CourseOfferingId = 101,
            AddedAt = DateTime.UtcNow
        };

        // Act
        _repository.AddWaitlistEntry(entry);

        // Assert
        var waitlist = _repository.GetWaitlistEntries();

        Assert.Contains(
            waitlist,
            w => w.LearnerId == 5 &&
                 w.CourseOfferingId == 101);

        Assert.True(entry.Id > 0);
    }

    [Fact]
    public void RemoveWaitlistEntry_ShouldRemoveSpecifiedEntry()
    {
        // Arrange
        var entry = new WaitlistEntry
        {
            LearnerId = 5,
            CourseOfferingId = 101,
            AddedAt = DateTime.UtcNow
        };

        _repository.AddWaitlistEntry(entry);

        // Act
        _repository.RemoveWaitlistEntry(entry);

        // Assert
        Assert.DoesNotContain(
            _repository.GetWaitlistEntries(),
            w => w.Id == entry.Id);
    }

    [Fact]
    public void GetEnrollments_ShouldReturnSeparateList()
    {
        // Arrange
        var enrollment = new Enrollment
        {
            LearnerId = 5,
            CourseOfferingId = 101
        };

        _repository.AddEnrollment(enrollment);

        // Act
        var enrollments = _repository.GetEnrollments();
        enrollments.Clear();

        // Assert
        Assert.Contains(
            _repository.GetEnrollments(),
            e => e.Id == enrollment.Id);
    }

    [Fact]
    public void GetWaitlistEntries_ShouldReturnSeparateList()
    {
        // Arrange
        var entry = new WaitlistEntry
        {
            LearnerId = 5,
            CourseOfferingId = 101,
            AddedAt = DateTime.UtcNow
        };

        _repository.AddWaitlistEntry(entry);

        // Act
        var waitlist = _repository.GetWaitlistEntries();
        waitlist.Clear();

        // Assert
        Assert.Contains(
            _repository.GetWaitlistEntries(),
            w => w.Id == entry.Id);
    }
}
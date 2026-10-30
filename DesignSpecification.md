 Design Specification — Course Waitlist Management API

 1. Introduction

 1.1 Purpose

The purpose of this application is to manage learner enrollment in a course with limited capacity. When a course is full, eligible learners can be added to a waitlist.

The application supports enrolling learners, dropping enrollments, retrieving enrolled learners, and retrieving waitlisted learners.

 1.2 Scope

The application implements the following features:

- Enroll a learner in a course.
- Validate learner eligibility and prerequisites.
- Prevent duplicate enrollments.
- Add eligible learners to the waitlist when the course is full.
- Drop a learner's enrollment.
- Retrieve enrolled learners.
- Retrieve waitlisted learners.

 2. Architecture

The application follows the Clean Architecture approach, separating domain entities, business logic, infrastructure, and API endpoints.

 2.1 Project Responsibilities

| Project                   | Responsibility |

| `Course_Domain`           | Defines the core entities used by the application. |
| `Course_Application`      | Contains enrollment business logic and service/repository interfaces. |
| `Course_Infrastructure`   | Implements the repository using in-memory collections. |
| `CourseRegistration`      | Exposes HTTP endpoints through ASP.NET Core Web API controllers. |
| `Course_Tests`            | Contains unit tests for application services and repository behavior. |

 2.2 Dependency Flow

The API uses the application service to execute enrollment operations. The application service depends on the repository interface, while the infrastructure project provides its implementation.

This separation keeps business rules independent of the storage implementation.

 3. Domain Model

The application uses the following entities.

 3.1 Learner

Represents a learner who can enroll in a course.

| Property | Type | Description |
|---|---|---|
| `LearnerId` | `int` | Unique learner identifier. |
| `FirstName` | `string` | Learner's first name. |
| `LastName` | `string` | Learner's last name. |
| `Email` | `string` | Learner's email address. |
| `PhoneNumber` | `string` | Learner's phone number. |
| `HasCompletedPrerequisites` | `bool` | Indicates whether the learner has completed the required prerequisites. |
| `CreatedAt` | `DateTime` | Date and time the learner record was created. |

 3.2 CourseOffering

Represents the course available for enrollment.

| Property | Type | Description |
|---|---|---|
| `Id` | `int` | Unique course offering identifier. |
| `CourseName` | `string` | Name of the course. |
| `Capacity` | `int` | Maximum number of learners who can enroll. |

 3.3 Enrollment

Represents a learner's enrollment in a course.

| Property | Type | Description |
|---|---|---|
| `Id` | `int` | Unique enrollment identifier. |
| `LearnerId` | `int` | Identifier of the enrolled learner. |
| `CourseOfferingId` | `int` | Identifier of the course offering. |
| `EnrolledAt` | `DateTime` | Date and time of enrollment. |

### 3.4 WaitlistEntry

Represents a learner waiting for an available course seat.

| Property | Type | Description |
|---|---|---|
| `Id` | `int` | Unique waitlist entry identifier. |
| `LearnerId` | `int` | Identifier of the waitlisted learner. |
| `CourseOfferingId` | `int` | Identifier of the course offering. |
| `AddedAt` | `DateTime` | Date and time the learner was added to the waitlist. |

 4. Business Logic

 4.1 Enrollment Process

When a learner requests enrollment, the application:

1. Checks whether the learner exists.
2. Checks whether the learner is already enrolled.
3. Checks whether the learner is already on the waitlist.
4. Verifies that the learner has completed the required prerequisites.
5. Validates the course capacity.
6. Enrolls the learner if a seat is available.
7. Adds the eligible learner to the waitlist if the course is full.

The application returns a result message describing the outcome.

 4.2 Drop Enrollment

When a learner requests to drop an enrollment, the application checks the current enrollment records and processes the request according to the implemented business rules.

If waitlist promotion is implemented, an eligible waitlisted learner can be promoted when a seat becomes available.

 4.3 Waitlist Ordering

Waitlist entries are retrieved in ascending order by `AddedAt`. If multiple entries have the same timestamp, the entry ID is used as a secondary sorting criterion.

 4.4 Duplicate Prevention

The application prevents a learner from being enrolled more than once in the same course and prevents duplicate waitlist entries.

 5. API Design

The application exposes the following REST API endpoints.

| HTTP Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/course-enrollment/enroll/{learnerId}` | Enrolls a learner or adds them to the waitlist. |
| `DELETE` | `/api/course-enrollment/drop/{learnerId}` | Drops a learner's enrollment. |
| `GET` | `/api/course-enrollment` | Retrieves enrolled learners. |
| `GET` | `/api/course-enrollment/waitlist` | Retrieves waitlisted learners. |

 5.1 Enrollment Endpoint

**Request:**

`POST /api/course-enrollment/enroll/{learnerId}`

The endpoint delegates the enrollment request to the application service and returns the resulting message.

 5.2 Drop Endpoint

**Request:**

`DELETE /api/course-enrollment/drop/{learnerId}`

The endpoint delegates the drop request to the application service.

 5.3 Retrieve Enrollments

**Request:**

`GET /api/course-enrollment`

Returns the current enrollment records for the course.

 5.4 Retrieve Waitlist

**Request:**

`GET /api/course-enrollment/waitlist`

Returns the current waitlist entries in the configured order.

 6. Repository Design

The application uses the `ICourseRepository` interface to abstract data access.

The repository supports the following operations:

- Retrieve the course offering.
- Retrieve a learner by ID.
- Retrieve enrollment records.
- Retrieve waitlist entries.
- Add and remove enrollment records.
- Add and remove waitlist entries.

The infrastructure project implements this interface using in-memory collections.

 7. Dependency Injection

The API uses ASP.NET Core dependency injection to register the repository and application service.

The in-memory repository is registered as a singleton so that its data can be shared across HTTP requests during the application's lifetime.

The application service is registered as a scoped service.

 8. Testing Strategy

The `Course_Tests` project uses xUnit to test the application service and repository behavior.

The test cases cover relevant scenarios such as:

- Successful enrollment when a seat is available.
- Adding a learner to the waitlist when the course is full.
- Preventing duplicate enrollment.
- Preventing duplicate waitlist entries.
- Rejecting learners who have not completed prerequisites.
- Handling nonexistent learners.
- Dropping an enrollment.
- Retrieving enrollment and waitlist records.
- Verifying waitlist ordering and promotion behavior, where implemented.

 9. Limitations and Assumptions

- The application currently uses in-memory storage.
- Data is not persisted when the application stops or restarts.
- The implementation is designed around the configured course offering.
- The API does not include authentication or authorization unless implemented separately.
- Concurrent enrollment requests require appropriate synchronization to prevent exceeding course capacity.

 10. Future Enhancements

Potential improvements include:

- Integrating SQL Server with Entity Framework Core.
- Adding authentication and authorization.
- Introducing request and response DTOs.
- Adding centralized exception handling and structured logging.
- Adding API integration tests.
- Improving concurrency control for simultaneous enrollment and drop requests.
- Supporting multiple course offerings.
 Course Waitlist Management API

 1. Project Overview

The Course Waitlist Management API is a RESTful Web API developed using ASP.NET Core and C#. It manages learner enrollment in a course and maintains a waitlist when the course reaches its maximum capacity.

The application allows learners to enroll in a course, drop their enrollment, view enrolled learners, and retrieve waitlisted learners.

The system validates learner eligibility, prevents duplicate enrollments, and adds eligible learners to the waitlist when the course is full.

 2. Technologies Used

- **Programming Language:** C#
- **Framework:** ASP.NET Core Web API
- **Target Framework:** .NET 10
- **Architecture:** Clean Architecture
- **Testing Framework:** xUnit
- **Storage:** In-memory repository
- **API Testing:** Swagger UI or Postman

 3. Project Structure

The solution is organized into the following projects:

```text
CourseWaitlistManagement/
│
├── Course_Domain/
│   └── Entities/
│       ├── Learner.cs
│       ├── CourseOffering.cs
│       ├── Enrollment.cs
│       └── WaitlistEntry.cs
│
├── Course_Application/
│   ├── Interfaces/
│   └── Services/
│       └── CourseEnrollmentService.cs
│
├── Course_Infrastructure/
│   └── Repositories/
│       └── InMemoryCourseRepository.cs
│
├── CourseRegistration/
│   ├── Controllers/
│   ├── Program.cs
│   └── appsettings.json
│
├── Course_Tests/
│   ├── Services/
│   └── Repositories/
│
├── CourseWaitlistManagement.sln
└── README.md
```

*Note: Adjust the folder and file names above to match your actual solution structure.*

 Project Responsibilities

- **Course_Domain:** Contains the core entities representing learners, course offerings, enrollments, and waitlist entries.
- **Course_Application:** Contains the enrollment service, business logic, and repository/service interfaces.
- **Course_Infrastructure:** Implements the repository using in-memory collections.
- **CourseRegistration:** Exposes REST API endpoints through ASP.NET Core controllers.
- **Course_Tests:** Contains unit tests for the application service and repository.

 4. Prerequisites

Before running the application, ensure that the following are installed:

- .NET 10 SDK
- Visual Studio 2022 or a compatible IDE
- Postman or Swagger UI for API testing

Verify the installed .NET SDK using:

`dotnet --version`

 5. How to Run the Application

 Step 1: Clone or Download the Repository

Download the source code or clone the repository to your local machine.

 Step 2: Open the Solution

Open `CourseWaitlistManagement.sln` in Visual Studio.

Alternatively, navigate to the solution directory using a terminal.

 Step 3: Restore Dependencies

`dotnet restore`

 Step 4: Build the Solution

`dotnet build`

 Step 5: Run the API

Navigate to the API project directory:

`cd CourseRegistration`

Run the application:

`dotnet run`

The application will display the local URLs in the terminal. Use the appropriate URL to access the API.

If Swagger is configured and enabled, open the Swagger URL displayed by the application.

 6. API Endpoints

The API provides the following endpoints.

| HTTP Method      | Endpoint |                                    Description |
| POST             | `/api/course-enrollment/enroll/{learnerId}`   | Enrolls a learner or adds the learner to the waitlist when the course is full. |
| DELETE           | `/api/course-enrollment/drop/{learnerId}`     | Drops a learner's enrollment. |
| GET              | `/api/course-enrollment`                      | Retrieves the enrolled learners. |
| GET              | `/api/course-enrollment/waitlist`             | Retrieves the learners on the waitlist. |

 6.1 Enroll a Learner

**Request:**

```http
POST /api/course-enrollment/enroll/3
```

**Possible outcomes:**

- The learner is enrolled if a seat is available and the learner meets the prerequisites.
- The learner is added to the waitlist if the course is full.
- An appropriate message is returned if the learner does not exist, has not completed the prerequisites, or is already enrolled or waitlisted.

 6.2 Drop a Learner

**Request:**

```http
DELETE /api/course-enrollment/drop/1
```

Drops the specified learner's enrollment, subject to the application's implemented business rules.

 6.3 Get Enrolled Learners

**Request:**

```http
GET /api/course-enrollment
```

Returns the learners currently enrolled in the course.

 6.4 Get Waitlisted Learners

**Request:**

```http
GET /api/course-enrollment/waitlist
```

Returns the learners currently on the waitlist, ordered by their waitlist entry date.

 7. Business Rules

The application implements the following business rules:

1. A learner must exist before attempting to enroll.
2. A learner must have completed the required prerequisites.
3. A learner cannot enroll in the same course more than once.
4. A learner cannot be added to the same course's waitlist more than once.
5. A learner is enrolled directly when a seat is available.
6. If the course is full, an eligible learner is added to the waitlist.
7. The waitlist is retrieved in the order in which entries were added.
8. When an enrollment is dropped, the application applies its implemented waitlist-promotion rules, if any.

 8. How to Run Unit Tests

Navigate to the solution directory and execute:

`dotnet test`

To run tests for the test project specifically:

`dotnet test Course_Tests/Course_Tests.csproj`

The test project uses xUnit to verify application service and repository behavior.

 9. Storage and Limitations

- The application currently uses an in-memory repository rather than a persistent database.
- Data is maintained in application memory and is reset when the application restarts.
- The repository is initialized with sample course and enrollment data.
- The API is designed as a demonstration of course enrollment and waitlist management.
- Persistent storage, authentication, and authorization are not included unless implemented separately.

 10. Future Enhancements

Potential enhancements include:

- Integrating SQL Server with Entity Framework Core.
- Adding authentication and authorization.
- Adding request validation and centralized exception handling.
- Implementing more comprehensive logging.
- Adding integration tests for API endpoints.
- Improving concurrency handling for simultaneous enrollment requests.


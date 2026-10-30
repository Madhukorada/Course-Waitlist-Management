
using Course_Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseRegistration.Controllers
{
    [ApiController]
    [Route("api/course-enrollment")]
    public class CourseEnrollmentController : ControllerBase
    {
        private readonly ICourseEnrollmentService _service;

        public CourseEnrollmentController(
            ICourseEnrollmentService service)
        {
            _service = service;
        }

        // API 1: Enroll a learner or add them to the waitlist.
        [HttpPost("enroll/{learnerId:int}")]
        public IActionResult EnrollLearner(int learnerId)
        {
            var result = _service.EnrollLearner(learnerId);

            if (result == "Learner not found.")
                return NotFound(new { message = result });

            if (result == "Learner enrolled successfully.")
                return Ok(new { message = result });

            if (result == "Course is full. Learner added to the waitlist.")
                return Ok(new { message = result });

            return BadRequest(new { message = result });
        }

        // API 2: Drop an enrollment and promote the next eligible learner.
        [HttpDelete("drop/{learnerId:int}")]
        public IActionResult DropLearner(int learnerId)
        {
            var result = _service.DropLearner(learnerId);

            if (result.StartsWith("Enrollment dropped successfully."))
                return Ok(new { message = result });

            if (result == "Learner is not enrolled in this course.")
                return NotFound(new { message = result });

            return BadRequest(new { message = result });
        }

        // API 3: Get currently enrolled learners.
        [HttpGet]
        public IActionResult GetEnrolledLearners()
        {
            var learners = _service.GetEnrolledLearners();

            return Ok(learners);
        }

        // API 4: Get the waitlist in FIFO order.
        [HttpGet("waitlist")]
        public IActionResult GetWaitlistedLearners()
        {
            var waitlist = _service.GetWaitlistedLearners();

            return Ok(waitlist);
        }
    }
}

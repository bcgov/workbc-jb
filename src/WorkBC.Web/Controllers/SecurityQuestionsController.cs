using Microsoft.AspNetCore.Mvc;
using WorkBC.Web.Services;

namespace WorkBC.Web.Controllers
{
    [Route("api/security-questions")]
    [ApiController]
    public class SecurityQuestionsController : ControllerBase
    {
        private readonly ISecurityQuestionService _securityQuestionService;
        public SecurityQuestionsController(ISecurityQuestionService securityQuestionService)
        {
            _securityQuestionService = securityQuestionService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = _securityQuestionService.GetSecurityQuestions();
            return Ok(result);
        }
    }
}
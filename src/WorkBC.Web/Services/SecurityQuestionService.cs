using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Web.Services
{

    public interface ISecurityQuestionService
    {
        IEnumerable<SecurityQuestion> GetSecurityQuestions();
    }
    public class SecurityQuestionService: ISecurityQuestionService
    {
        private readonly JobBoardContext _jobBoardContext;
        public SecurityQuestionService(JobBoardContext jobBoardContext)
        {
            _jobBoardContext = jobBoardContext;
        }

        public IEnumerable<SecurityQuestion> GetSecurityQuestions()
        {
            return _jobBoardContext.SecurityQuestions.AsNoTracking();
        }
    }
}

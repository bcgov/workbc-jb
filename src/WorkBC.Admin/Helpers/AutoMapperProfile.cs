using AutoMapper;
using WorkBC.Admin.Areas.JobSeekers.Models;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Admin.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<JobSeeker, UserViewModel>();
            CreateMap<UserViewModel, JobSeeker>();
            CreateMap<JobSeeker, JobSeeker>();
        }
    }
}
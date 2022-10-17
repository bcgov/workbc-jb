using AutoMapper;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Web.Models;

namespace WorkBC.Web.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<JobSeeker, IRegisterModel>().AsProxy();
            CreateMap<IRegisterModel, JobSeeker>();
            CreateMap<JobSeeker, IUserInfo>().AsProxy();
        }
    }
}
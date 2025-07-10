using Contracts;

namespace JobRepositoryService
{
    using AutoMapper;

    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {
            CreateMap<JobDocument, Job>();
            CreateMap<Job, JobDocument>();
        }
    }
}

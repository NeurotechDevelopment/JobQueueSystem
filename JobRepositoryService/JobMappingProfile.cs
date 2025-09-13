using AutoMapper;
using Contracts;

namespace JobRepositoryService
{
    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {
            CreateMap<KeyValuePair<JobType, string>, JobTypeDescriptor>()
                .ForMember(d => d.JobType, s => s.MapFrom(opt => opt.Key))
                .ForMember(d => d.Description, s => s.MapFrom(opt => opt.Value));
            CreateMap<JobDocument, Job>();

            CreateMap<Job, JobDocument>();

            CreateMap<JobRequest, JobDocument>()
                .ForMember(d => d.Payload,
                    s => s.MapFrom(x => x.Payload));
        }
    }
}

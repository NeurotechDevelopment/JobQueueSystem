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
            CreateMap<JobDocument, Job>()
                .ForMember(d => d.RequestPayload, s => s.MapFrom(opt => opt.Payload))
                .ForMember(d => d.ResultPayload, s => s.MapFrom(opt => opt.Result));

            CreateMap<JobDocument, JobInfo>()
                .ForMember(d => d.ErrorMessage, s => s.MapFrom(opt => opt.Result != null ? opt.Result.ErrorMessage : null))
                .ForMember(d => d.IsSuccess, s => s.MapFrom(opt => opt.Result != null ? opt.Result.IsSuccess : null));

            CreateMap<Job, JobDocument>();

            CreateMap<JobRequest, JobDocument>()
                .ForMember(d => d.Payload,
                    s => s.MapFrom(x => x.Payload));
        }
    }
}

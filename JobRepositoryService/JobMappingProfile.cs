using AutoMapper;
using Contracts;
using MongoDB.Bson;

namespace JobRepositoryService
{
    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {
            CreateMap<KeyValuePair<JobType, string>, JobTypeDescriptor>()
                .ForMember(d => d.Key, s => s.MapFrom(opt => opt.Key))
                .ForMember(d => d.Description, s => s.MapFrom(opt => opt.Value));
            CreateMap<JobDocument, Job>();

            CreateMap<Job, JobDocument>();

            CreateMap<JobRequest, JobDocument>()
                .ForMember(d => d.Payload,
                    s => s.MapFrom(x => BsonDocument.Parse(x.Payload)));
        }
    }
}

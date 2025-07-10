using AutoMapper;
using Contracts;
using MongoDB.Bson;

namespace JobRepositoryService
{
    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {
            CreateMap<JobDocument, Job>();

            CreateMap<Job, JobDocument>();

            CreateMap<JobRequest, JobDocument>()
                .ForMember(d => d.Payload,
                    s => s.MapFrom(x => BsonDocument.Parse(x.Payload)));
        }
    }
}

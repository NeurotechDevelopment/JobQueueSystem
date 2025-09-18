using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Contracts;

namespace JobRepositoryService
{
    public class JobTypesService
    {
        private static Lazy<IEnumerable<KeyValuePair<JobType, string>>> JobTypes =
            new(() =>
            {
                var jobTypes = Enum.GetValues<JobType>();
                return jobTypes.Select(x => new KeyValuePair<JobType, string>(x, GetDescription(x)));
            });

        public IEnumerable<KeyValuePair<JobType, string>> GetJobTypes()
        {
            return JobTypes.Value;
        }

        private static string GetDescription(JobType jobType)
        {
            return jobType.GetType().GetMember(jobType.ToString()).First().GetCustomAttribute<DisplayAttribute>()
                .Description;
        }
    }
}

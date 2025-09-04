using System.ComponentModel;
using System.Reflection;
using Contracts;

namespace JobRepositoryService
{
    public class JobTypesService
    {
        private static Lazy<IEnumerable<KeyValuePair<string, string>>> JobTypes =
            new Lazy<IEnumerable<KeyValuePair<string, string>>>(() =>
            {
                var jobTypes = Enum.GetValues<JobType>();
                return jobTypes.Select(x => new KeyValuePair<string, string>(x.ToString(), GetDescription(x)));
            });

        public IEnumerable<KeyValuePair<string, string>> GetJobTypes()
        {
            return JobTypes.Value;
        }

        private static string GetDescription(JobType jobType)
        {
            return jobType.GetType().GetMember(jobType.ToString()).First().GetCustomAttribute<DescriptionAttribute>()
                .Description;
        }
    }
}

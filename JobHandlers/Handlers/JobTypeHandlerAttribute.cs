using Contracts;

namespace JobHandlers.Handlers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class JobTypeHandlerAttribute : Attribute
    {
        public JobTypeHandlerAttribute(JobType jobType)
        {
            HandlesJob = jobType;
        }

        public JobType HandlesJob { get; }
    }
}

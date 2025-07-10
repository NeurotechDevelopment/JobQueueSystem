using System.Runtime.Serialization;

namespace Contracts
{
    public enum JobStatus
    {
        NotStarted,
        InProgress,
        Failed,
        Finished
    }
}
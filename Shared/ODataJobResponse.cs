using Contracts;

namespace Shared
{
    internal class ODataJobResponse
    {
        public IEnumerable<Job> Value { get; set; }
    }
}

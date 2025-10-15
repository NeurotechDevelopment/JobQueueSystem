namespace Shared
{
    public static class QueueNames
    {
        /// <summary>
        /// Queue name where job requests are initially posted (by JobProducerService) and picked up by JobRequestReceiverService.
        /// </summary>
        public const string InitialJobRequests = "job-request-initial";

        /// <summary>
        /// Queue name where job requests are rerouted by JobRequestReceiverService after saving to the repository.
        /// Same queue is picked up by JobDispatcherService to dispatch jobs to workers.
        /// </summary>
        public const string DispatcherReady = "job-request-ready";

        /// <summary>
        /// Represents the suffix appended to the name of a dispatched queue as {jobType}-DispatchedQueueSuffix
        /// </summary>
        public const string DispatchedQueueSuffix = "-dispatched";
    }
}

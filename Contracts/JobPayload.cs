namespace Contracts
{
    public class JobPayload
    {
        public JobPayload()
        {
        }
        
        public JobPayload(string data, bool isExternal = false)
        {
            Data = data;
            IsExternal = isExternal;
        }

        /// <summary>
        /// Indicates whether the payload is stored externally (e.g., in blob storage) and only a reference is provided here.
        /// </summary>
        public bool IsExternal { get; set; } = false;

        public string? Data { get; set; }
    }
}

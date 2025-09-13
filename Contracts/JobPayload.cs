namespace Contracts
{
    public class JobPayload
    {
        public JobPayload()
        {
        }
        
        public JobPayload(string data)
        {
            Data = data;
        }
        public string? Data { get; set; }
    }
}

namespace JobHandlers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<DummyHandlerWorker>();

            var host = builder.Build();
            host.Run();
        }
    }
}
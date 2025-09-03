using Shared;

namespace JobDispatcherService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            // Bind JobRepositoryClientConfig. Cryptic code, but what it does is allows DI to know with what to instantiate AddSingleton below.
            builder.Services.Configure<JobRepositoryClientConfig>(
                builder.Configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(JobRepositoryClientConfig)}"));
            
            builder.Services.AddHostedService<JobDispatcherWorker>();
            builder.Services.AddSingleton<JobRepositoryClient>();

            var host = builder.Build();
            host.Run();
        }
    }
}
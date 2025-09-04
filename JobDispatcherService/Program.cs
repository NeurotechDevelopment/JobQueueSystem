using MassTransit;
using Shared;
using Shared.Configuration;

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
            builder.Services.AddSingleton<IJobDispatcher, JobDispatcher>();

            builder.Services.AddMassTransit(opt =>
            {
                opt.UsingRabbitMq((ctx, cfg) =>
                {
                    var rabbitConfigSection = builder.Configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(RabbitConfig)}");
                    var rabbitConfig = rabbitConfigSection.Get<RabbitConfig>();
                    cfg.Host(rabbitConfig.Host, h =>
                    {
                        h.Username(rabbitConfig.User);
                        h.Password(rabbitConfig.Password);
                    });

                    cfg.ConfigureEndpoints(ctx);
                });
            });

            var host = builder.Build();
            host.Run();
        }
    }
}
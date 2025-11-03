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

            // Bind ApplicationSettings. Cryptic code, but what it does is allows DI to know with what to instantiate AddSingleton below.
            var appSettingsSection = builder.Configuration.GetSection(nameof(ApplicationSettings));
            builder.Services.Configure<ApplicationSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<ApplicationSettings>();

            // Bind JobRepositoryClientConfig. Cryptic code, but what it does is allows DI to know with what to instantiate AddSingleton below.
            builder.Services.Configure<JobRepositoryClientConfig>(
                builder.Configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(JobRepositoryClientConfig)}"));

            // Polling worker. Might come in handy for stuck jobs.
            // builder.Services.AddHostedService<JobDispatcherWorker>();
            builder.Services.AddSingleton<IJobRepositoryClient, JobRepositoryClient>();
            builder.Services.AddSingleton<IJobDispatcher, JobDispatcher>();

            builder.Services.AddMassTransit(opt =>
            {
                opt.AddConsumer<JobRequestConsumer>();
                opt.UsingRabbitMq((ctx, cfg) =>
                {
                    var rabbitConfig = appSettings.RabbitConfig;
                    cfg.Host(rabbitConfig.Host, h =>
                    {
                        h.Username(rabbitConfig.User);
                        h.Password(rabbitConfig.Password);
                    });

                    cfg.ConfigureEndpoints(ctx);

                    var queueName = string.IsNullOrWhiteSpace(rabbitConfig.QueueName) ? QueueNames.DispatcherReady : rabbitConfig.QueueName;
                    cfg.ReceiveEndpoint(queueName, e =>
                    {
                        e.ConfigureConsumer<JobRequestConsumer>(ctx);
                    });
                });
            });

            var host = builder.Build();
            host.Run();
        }
    }
}
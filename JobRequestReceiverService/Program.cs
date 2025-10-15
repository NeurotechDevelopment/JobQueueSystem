using MassTransit;
using Shared;
using Shared.Configuration;

namespace JobRequestReceiverService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    var config = context.Configuration;
                    var appSettingsSection = config.GetSection(nameof(ApplicationSettings));
                    var appSettings = appSettingsSection.Get<ApplicationSettings>();
                    services.Configure<ApplicationSettings>(appSettingsSection);

                    // Explicitly bind this subsection for the JobRepositoryClient
                    var jobReposClientSection = appSettingsSection.GetSection(nameof(JobRepositoryClientConfig));
                    services.Configure<JobRepositoryClientConfig>(jobReposClientSection);

                    services.AddLogging();

                    // Register client to communicate with JobRepository service.
                    services.AddSingleton<IJobRepositoryClient, JobRepositoryClient>();

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<JobRequestConsumer>(); // Consumer

                        var rabbitConfig = appSettings.RabbitConfig;
                        x.UsingRabbitMq((ctx, cfg) =>
                        {
                            cfg.Host(rabbitConfig.Host, h =>
                            {
                                h.Username(rabbitConfig.User);
                                h.Password(rabbitConfig.Password);
                            });

                            var queueName = string.IsNullOrWhiteSpace(rabbitConfig.QueueName) ? QueueNames.InitialJobRequests : rabbitConfig.QueueName;
                            cfg.ReceiveEndpoint(queueName, e =>
                            {
                                e.ConfigureConsumer<JobRequestConsumer>(ctx);
                            });
                        });
                    });
                })
                .Build();

            host.Run();
        }
    }
}
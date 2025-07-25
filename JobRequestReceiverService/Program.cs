using Contracts;
using MassTransit;
using MassTransit.Transports.Fabric;
using Shared;

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
                    services.AddSingleton<JobRepositoryClient, JobRepositoryClient>();

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<JobRequestConsumer>(); // Consumer

                        x.UsingRabbitMq((ctx, cfg) =>
                        {
                            cfg.Host(appSettings.RabbitConfig.Host, h =>
                            {
                                h.Username(appSettings.RabbitConfig.User);
                                h.Password(appSettings.RabbitConfig.Password);
                            });
                            
                            cfg.ReceiveEndpoint($"{appSettings.RabbitConfig.QueueName}", e =>
                            {
                                e.ConfigureConsumeTopology = false;
                                e.ExchangeType = ExchangeType.Direct.ToString().ToLowerInvariant();
                                e.Bind<JobRequest>(b =>
                                {
                                    b.ExchangeType = ExchangeType.Direct.ToString().ToLowerInvariant();
                                });
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
using JobHandlers.Handlers;
using MassTransit;
using Shared;
using Shared.Configuration;
using System.Reflection;

namespace JobHandlers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddLogging();
            
            builder.Services.Configure<JobRepositoryClientConfig>(
                builder.Configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(JobRepositoryClientConfig)}"));
            builder.Services.AddSingleton<IJobRepositoryClient, JobRepositoryClient>();
            
            var handlerTypes = ResolveJobHandlers();

            builder.Services.AddMassTransit(opt =>
            {
                foreach (var jobHandlerType in handlerTypes)
                {
                    opt.AddConsumer(jobHandlerType);
                }

                opt.UsingRabbitMq((ctx, cfg) =>
                {
                    // Fetch job types from repository service.
                    var logger = ctx.GetRequiredService<ILogger<Program>>();
                    
                    var repoClient = ctx.GetRequiredService<IJobRepositoryClient>();
                    var allowedJobTypes = repoClient.GetJobTypes();
                    
                    // Find all registered JobHandlers by the above opt.AddConsumers(assembly)
                    // and filter out by allowedJobTypes.
                    foreach (var jobTypeDescriptor in allowedJobTypes)
                    {
                        // Locate handler for jobTypeDescriptor.JobType from the JobTypeHandlerAttribute of a handler type.
                        var handler = handlerTypes.SingleOrDefault(x => x.GetCustomAttribute<JobTypeHandlerAttribute>()?.HandlesJob == jobTypeDescriptor.JobType);
                        if (handler == null)
                        {
                            logger.LogWarning($"No handler found for job type {jobTypeDescriptor.JobType}. No queue subscription will be done.");
                            continue;
                        }
                        cfg.ReceiveEndpoint($"{jobTypeDescriptor.JobType}-queue",
                            e =>
                            {
                                e.ConfigureConsumeTopology = false;
                                e.ConfigureConsumer(ctx, handler);
                            });
                    }
                });
            });

            // Register Syncfusion license
            var licenseKey = builder.Configuration.GetValue<string>("SYNCFUSION_LICENSE_KEY");
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

            var host = builder.Build();
            host.Run();
        }

        private static IEnumerable<Type> ResolveJobHandlers()
        {
            var jobHandlerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => typeof(JobHandler).IsAssignableFrom(x) 
                       && !x.IsAbstract 
                       && x.GetCustomAttribute<JobTypeHandlerAttribute>() != null);
            return jobHandlerTypes;
        }
    }
}
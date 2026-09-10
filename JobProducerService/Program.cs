using JobProducerService.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Shared;
using Shared.Configuration;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;

namespace JobProducerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // Allow enum values to be serialized as strings.
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var appSettingsSection = builder.Configuration.GetSection(nameof(ApplicationSettings));
            var appSettings = appSettingsSection.Get<ApplicationSettings>();
            builder.Services.Configure<ApplicationSettings>(appSettingsSection);

            // Add authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = appSettings.AuthOptions.RealmAuthority;
                    options.RequireHttpsMetadata = appSettings.AuthOptions.RequireHttpsMetadata;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = appSettings.AuthOptions.Audiences,
                        // Below is what set by default. List here for clarity on what is validated.
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });
            // Explicitly bind this subsection for the JobRepositoryClient
            var jobReposClientSection = appSettingsSection.GetSection(nameof(JobRepositoryClientConfig));
            builder.Services.Configure<JobRepositoryClientConfig>(jobReposClientSection);

            // Set file size upload limit from config.
            if (appSettings.MaxAttachmentSizeInBytes.HasValue)
            {
                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = appSettings.MaxAttachmentSizeInBytes.Value;
                });

                builder.Services.Configure<FormOptions>(options =>
                {
                    options.MultipartBodyLengthLimit = appSettings.MaxAttachmentSizeInBytes.Value;
                });
            }

            // Register client to communicate with JobRepository service.
            builder.Services.AddSingleton<IJobRepositoryClient, JobRepositoryClient>();
            
            builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.UsingRabbitMq((context, rabbitBusFactoryConfigurator) =>
                {
                    rabbitBusFactoryConfigurator.Host(host: appSettings.RabbitConfig.Host, h =>
                    {
                        h.Username(appSettings.RabbitConfig.User);
                        h.Password(appSettings.RabbitConfig.Password);
                    });
                });
            });

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
using JobProducerService.Configuration;
using MassTransit;
using System.Text.Json.Serialization;

namespace JobProducerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
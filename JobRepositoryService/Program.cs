using Contracts;
using Contracts.Payloads;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;
using System.Text.Json.Serialization;

namespace JobRepositoryService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // Add ability for swagger to render enum as string rather than integers.
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.AddControllers().AddOData(opt =>
            {
                opt.AddRouteComponents(ServicesConstants.OdataRoutePrefix, GetEdmModel())
                    .Select().Filter().OrderBy().Expand().SetMaxTop(100).Count();
            });
            builder.Services.AddMemoryCache();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(ServicesConstants.JobsRepository,
                    new OpenApiInfo { Title = "Jobs REST API", Version = "v1" });
                c.SwaggerDoc(ServicesConstants.JobsAttachments,
                    new OpenApiInfo { Title = "Attachments REST API", Version = "v1" });
                c.SwaggerDoc(ServicesConstants.OdataRoutePrefix,
                    new OpenApiInfo { Title = "Jobs ODATA API", Version = "v1" });
            });
            builder.Services.AddAutoMapper( p => p.AddMaps(Assembly.GetExecutingAssembly()));
            var appSettingsSection = builder.Configuration.GetSection(nameof(ApplicationSettings));
            builder.Services.Configure<ApplicationSettings>(appSettingsSection);
            builder.Services.AddSingleton<IJobRepository, MongoJobRepository>();
            builder.Services.AddSingleton<IBlobStorage, GridFsBlobStorage>();
            builder.Services.AddSingleton<IJobService, JobService>();
            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApplicationSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            // Add authentication
            var appSettings = appSettingsSection.Get<ApplicationSettings>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = appSettings.AuthOptions.Authority;
                    options.RequireHttpsMetadata = appSettings.AuthOptions.RequireHttpsMetadata;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudiences = appSettings.AuthOptions.Audiences
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/swagger/{ServicesConstants.JobsRepository}/swagger.json", "Job Repository API");
                    c.SwaggerEndpoint($"/swagger/{ServicesConstants.JobsAttachments}/swagger.json", "Attachments API");
                    c.SwaggerEndpoint($"/swagger/{ServicesConstants.OdataRoutePrefix}/swagger.json", "Jobs ODATA API");
                });
            }

           // app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            var jobEntitySet = builder.EntitySet<Job>(ServicesConstants.JobsEntity);
            jobEntitySet.EntityType.HasKey(j => j.JobId);

            // Explicitly declare these as embedded complex objects
            builder.ComplexType<JobPayload>();
            builder.ComplexType<Attachment>();

            return builder.GetEdmModel();
        }
    }
}
using Contracts;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using MongoDB.Driver;
using System.Reflection;
using System.Text.Json.Serialization;
using Contracts.Payloads;

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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper( p => p.AddMaps(Assembly.GetExecutingAssembly()));
            builder.Services.Configure<ApplicationSettings>(
                builder.Configuration.GetSection(nameof(ApplicationSettings)));
            builder.Services.AddSingleton<IJobRepository, MongoJobRepository>();
            builder.Services.AddSingleton<IBlobStorage, GridFsBlobStorage>();
            builder.Services.AddSingleton<IJobService, JobService>();
            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApplicationSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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
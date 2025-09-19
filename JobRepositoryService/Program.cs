using Contracts;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper( p => p.AddMaps(Assembly.GetExecutingAssembly()));
            builder.Services.Configure<ApplicationSettings>(
                builder.Configuration.GetSection(nameof(ApplicationSettings)));
            builder.Services.AddSingleton<IJobRepository, MongoJobRepository>();
            builder.Services.AddSingleton<IBlobStorage, GridFsBlobStorage>();
            builder.Services.AddSingleton<JobTypesService>();

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
            builder.EntitySet<Job>(ServicesConstants.JobsEntity);
            return builder.GetEdmModel();
        }
    }
}
using HospitalService.MessageConsumer.AzureBus;
using HospitalService.MessageConsumer.Data;
using HospitalService.MessageConsumer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace HospitalService.MessageConsumer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MessageDbContext>(opt => opt.UseNpgsql(Environment.GetEnvironmentVariable("MESSAGEDB_KEY")), ServiceLifetime.Singleton);

            services.AddScoped<IMessageConsumerRepository, MessageConsumerRepository>();

            services.AddHostedService<MessageConsumerService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MESSAGECONSUMER API", Version = "v1" });
            });
            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My MessageConsumer API"); });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
using HospitalService.MessageConsumer.Data;
using HospitalService.MessageConsumer.Repositories;
using HospitalService.MessageConsumer.ServiceBusMessaging;
using Microsoft.EntityFrameworkCore;
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
            var connectionString = Environment.GetEnvironmentVariable("MESSAGEDB_KEY");
            services.AddDbContext<MessageDbContext>(opt => opt.UseNpgsql(connectionString), ServiceLifetime.Transient);
            services.AddScoped<IMessageConsumerRepository, MessageConsumerRepository>();

            services.AddSingleton<IProcessData, ProcessData>();
            services.AddSingleton<IServiceBusConsumer, ServiceBusConsumer>();

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

            var bus = app.ApplicationServices.GetService<IServiceBusConsumer>();
            bus.RegisterOnMessageHandlerAndReceiveMessages().GetAwaiter().GetResult();
        }
    }
}
using HospitalService.MessageConsumer.Data;
using HospitalService.MessageConsumer.Repositories;
using HospitalService.MessageConsumer.ServiceBusMessaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

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
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "MESSAGECONSUMER API",
                        Version = "v1",
                        Description = "An ASP.NET Core Web API for managing messages from HospitalService",
                        Contact = new OpenApiContact
                        {
                            Name = "GitHub",
                            Url = new Uri("https://github.com/kravchenkoegorii/HospitalService.MessageConsumer.git")
                        }
                    });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
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
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "MessageConsumer API"); });

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
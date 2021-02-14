using DerMistkaefer.DvbLive.Backend.Cache;
using DerMistkaefer.DvbLive.Backend.Cache.Api;
using DerMistkaefer.DvbLive.Backend.Database;
using DerMistkaefer.DvbLive.Backend.Database.Api;
using DerMistkaefer.DvbLive.Backend.HostedServices;
using DerMistkaefer.DvbLive.Backend.ServiceSetup;
using DerMistkaefer.DvbLive.GetPublicTransportLines.DependencyInjection;
using DerMistkaefer.DvbLive.TriasCommunication.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace DerMistkaefer.DvbLive.Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);
            services.AddHealthChecks();
            services.AddTriasCommunication(Configuration);
            services.AddPublicTransportLines();
            services.AddDistributedMemoryCache();
            services.AddEntityFrameworkMySql()
                .AddDbContext<DvbDbContext>((serviceProvider, options) =>
                {
                    var configConnectionString = Configuration.GetConnectionString("DvbDatabase");
                    if (configConnectionString != null)
                    {
                        options.UseMySql(configConnectionString, ServerVersion.AutoDetect(configConnectionString));
                        options.UseInternalServiceProvider(serviceProvider);
                    }
                    else
                    {
                        options.UseSqlite("Data Source=DvbDatabase.db");
                    }
                });
            services.AddSingleton<ICacheAdapter, CacheAdapter>();
            services.AddSingleton<IDatabaseAdapter, DatabaseAdapter>();
            services.AddHostedService<TripLogger>();

            services.AddSwaggerDvbLive();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerDvbLive();

            app.UseRouting();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc");
            });
        }
    }
}

using DerMistkaefer.DvbLive.Backend.Cache;
using DerMistkaefer.DvbLive.Backend.Cache.Api;
using DerMistkaefer.DvbLive.Backend.Database;
using DerMistkaefer.DvbLive.Backend.Database.Api;
using DerMistkaefer.DvbLive.Backend.HostedServices;
using DerMistkaefer.DvbLive.TriasCommunication.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddControllers();
            services.AddTriasCommunication(Configuration);
            services.AddDistributedMemoryCache();
            services.AddEntityFrameworkMySql()
                .AddDbContext<DvbDbContext>((serviceProvider, options) =>
                {
                    var conString = Configuration.GetConnectionString("DvbDatabase");
                    options.UseMySql(conString, ServerVersion.AutoDetect(conString));
                    options.UseInternalServiceProvider(serviceProvider);
                });
            services.AddSingleton<ICacheAdapter, CacheAdapter>();
            services.AddSingleton<IDatabaseAdapter, DatabaseAdapter>();
            services.AddHostedService<TripLogger>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

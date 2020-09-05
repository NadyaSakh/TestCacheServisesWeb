using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alachisoft.NCache.Caching.Distributed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestCacheServisesWeb.Services;

namespace TestCacheServisesWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

// TODO возможно нужно отключать сервисы, которые не используются, чтобы они не мешали использовать нужный кеш
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
// внедрение зависимости RedisCacheService и MemcacheCacheService
            services.AddTransient<RedisCacheService>();
            services.AddTransient<MemcacheCacheService>();
            //added to use Redis cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
            });

            // added to use memcached
            services.AddEnyimMemcached();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //to use memcached
            app.UseEnyimMemcached();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            /*app.UseStaticFiles();*/

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

using ExpressBase.Common;
using ExpressBase.Common.ServiceClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.FileWeb
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddScoped<IEbStaticFileClient, EbStaticFileClient>(serviceProvider =>
            {
                return new EbStaticFileClient();
            });

            services.AddHttpClient<EbStaticFileClient2>();

            // var redisServer = Environment.GetEnvironmentVariable(EnvironmentConstants.EB_REDIS_SERVER);
            var redisPassword = Environment.GetEnvironmentVariable(EnvironmentConstants.EB_REDIS_PASSWORD);
            var redisPort = Environment.GetEnvironmentVariable(EnvironmentConstants.EB_REDIS_PORT);
            //var redisConnectionString = string.Format("redis://{0}@{1}:{2}", redisPassword, redisServer, redisPort);

            var redisServer = "127.0.0.1";
            string redisConnectionString = string.Format("redis://{0}:{1}", redisServer, redisPort);

            var redisManager = new RedisManagerPool(redisConnectionString);
            services.AddScoped<IRedisClient, IRedisClient>(serviceProvider =>
            {
                return redisManager.GetClient();
            });

            var listRWRedis = new List<string>() { redisConnectionString }; var listRORedis = new List<string>() { redisConnectionString.Replace("-master", "-replicas") };
            PooledRedisClientManager pooledRedisManager = new PooledRedisClientManager(listRWRedis, listRORedis);
            services.AddSingleton<PooledRedisClientManager, PooledRedisClientManager>(serviceProvider =>
            {
                return pooledRedisManager;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                _ = routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

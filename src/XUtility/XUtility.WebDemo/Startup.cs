using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace XUtility.WebDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            //services.AddDistributedRedisCache(options =>
            //{
            //    //abortConnect 的默认值为真，这使得 StackExchange.Redis 在网络中断并重新连接后不会自动连接到Redis服务器。我们强烈建议您设置的 abortConnect = FALSE。
            //    //defaultDatabase可以指定使用哪一个redis数据库，默认配置下redis有0-15共16个数据库，可以通过databases配置节进行修改
            //    //redis配置项可参考StackExchange.Redis.ConfigurationOptions类
            //    //options.Configuration = "192.168.87.129,password=123456,abortConnect=false,defaultDatabase=1";
            //    options.Configuration = "192.168.18.130,password=123456,abortConnect=false,defaultDatabase=1";
            //    options.InstanceName = "master";
            //});

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDistributedCache cache)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvcWithDefaultRoute();
        }
    }
}

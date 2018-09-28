using Autofac.Extensions.DependencyInjection;
using Infrastructure;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NovelOnline.App;
using NovelOnline.App.AutoMapper;
using Repository;
using System;
using System.IO;
using System.Text;

namespace NovelOnlie.Mvc
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }


        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(env.ContentRootPath)
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            //log4 初始化
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var repository = LogManager.CreateRepository("NETCoreRepository");
            //log4net从log4net.config文件中读取配置信息
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            LoggerProperties.LogRepository = repository;

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var sqlConnectionString = Configuration.GetConnectionString("Default");
            //添加数据上下文
            services.AddDbContext<HLDBContext>(options => options.UseSqlServer(sqlConnectionString));

            return new AutofacServiceProvider(AutofacExt.InitAutofac(services));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Index}/{id?}");
            });
            //生成初始数据
            SendData.Initialize(app.ApplicationServices.CreateScope().ServiceProvider);
            //AutoMapper 配置
            AutoMapperConfiguration.Configure();
        }
    }
}

﻿using FlixOne.BookStore.OrderService.Clients;
using FlixOne.BookStore.OrderService.Common;
using FlixOne.BookStore.OrderService.Context;
using FlixOne.BookStore.OrderService.Persistance;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;

namespace FlixOne.BookStore.OrderService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            //Inject
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<IProductClient, ProductClient>();
            services.AddDbContext<OrderContext>(
                o => o.UseSqlServer(Configuration.GetConnectionString("OrderConnection")));

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                //options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                options.DescribeAllEnumsAsStrings();
                options.SingleApiVersion(new Info
                {
                    Title = "Ordering HTTP API",
                    //Version = "v1",
                    Description = "The Ordering Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            //AppSettings
            var appSettings = Configuration.GetSection("AppSettings");
            //register class
            services.Configure<AppSettings>(appSettings);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
            app.UseSwaggerUi();

        }
    }
}
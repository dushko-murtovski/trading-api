using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using StockWolfTrading.Core;
using StockWolfTrading.Core.Models;
using StockWolfTrading.DataModel.UnitOfWork;
using StockWolfTrading.Services;
using StockWolfTrading.Services.Interfaces;

namespace StockWolfTradingAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static AppSettings appSettings = new AppSettings();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<StockWolfTradingDBContext>();
            var p = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
                .SetBasePath(p)
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            config.GetSection("AppSettings").Bind(appSettings);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
                //builder =>
                //{
                //    builder/*.WithOrigins(appSettings.AllowOrigin)*/
                //            .AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                //});//.WithOrigins(appSettings.AllowOrigin).AllowAnyHeader(); });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "StockWolfTrading.Security.Bearer",
                        ValidAudience = "StockWolfTrading.Security.Bearer",
                        IssuerSigningKey = Provider.JwtSecurityKey.Create("Test-secret-key-1234-SWT")
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                            return Task.CompletedTask;
                        }
                    };

                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("AdminClaim"));
                options.AddPolicy("User", policy => policy.RequireClaim("UserClaim"));
            });
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllHeaders"));
            //});

            //services.AddMvc(options =>
            //    options.EnableEndpointRouting = false)
            //    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            //    .AddXmlSerializerFormatters();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            //services.AddTransient<ITradeService, TradeService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITickerService, TickerService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ITradeService, TradeService>();
            services.AddOptions();
            //services.AddAutoMapper();

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("AllowAllHeaders");
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            //app.UseCors("AllowAllHeaders");
            //app.UseAuthentication();
            //app.UseHttpsRedirection();
            //app.UseMvc();
        }
    }
}

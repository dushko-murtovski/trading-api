using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockwolfTrading.ViewModel;
using StockWolfTrading.Core;
using StockWolfTrading.Core.Models;
using StockWolfTrading.DataModel.UnitOfWork;
using StockWolfTrading.Services;
using StockWolfTrading.Services.Interfaces;
using StockWolfTradingAPI.Authorization;
using StockWolfTradingAPI.Helpers;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddDbContext<StockWolfTradingDBContext>(optionsBuilder => { optionsBuilder.UseLazyLoadingProxies(); });
    services.AddCors();
    services.AddControllers().AddNewtonsoftJson();

    // configure automapper with all automapper profiles from this assembly
    services.AddAutoMapper(typeof(AutoMapperProfile));

    services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    //services.AddCors(options =>
    //{
    //    options.AddPolicy("AllowAllHeaders", corsPolicyBuilder =>
    //    {
    //        corsPolicyBuilder.WithOrigins("http://localhost:3000", "http://85.215.179.91:5001").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    //    });
    //});

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddTransient<IUnitOfWork, UnitOfWork>();
    services.AddTransient<IUserService, UserService>();
    services.AddTransient<ITickerService, TickerService>();
    services.AddTransient<IOrderService, OrderService>();
    services.AddTransient<ITradeService, TradeService>();
    services.AddTransient<IDailyAnalysisService, DailyAnalysisService>();
    services.AddTransient<ISettingsService, SettingsService>();
    services.AddOptions();
}

var app = builder.Build();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<StockWolfTradingDBContext>();
    dataContext.Database.Migrate();
}

// configure HTTP request pipeline
{
    app.UseHsts();
    app.UseHttpsRedirection();
    // global cors policy
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();
}

app.Run();
//namespace StockWolfTradingAPI
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateWebHostBuilder(args).Build().Run();
//        }

//        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
//            WebHost.CreateDefaultBuilder(args)
//                .UseStartup<Startup>();
//    }
//}

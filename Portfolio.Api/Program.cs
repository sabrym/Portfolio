﻿using Portfolio.Services;
using Portfolio.Services.Interfaces;
using Serilog;

namespace Portfolio.Api;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .CreateLogger();
           
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMemoryCache();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();
            #region Add Logging

            builder.Host.UseSerilog((ctx, lc) => lc
               .ReadFrom.Configuration(ctx.Configuration));
            #endregion
            #region Add HTTPClient
            builder.Services.AddHttpClient();
            #endregion

            #region Add Business Logic
            builder.Services.AddSingleton<IStockTickerService, StockTickerService>();
            builder.Services.AddSingleton<ITradeReaderService, XMLTradeReaderService>();
            builder.Services.AddSingleton<IReportingService, StockReportingService>();
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception");

        }
    }
}


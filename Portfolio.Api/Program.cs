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

            builder.Services.AddControllers();

            #region Add Logging

            builder.Host.UseSerilog((ctx, lc) => lc
               .ReadFrom.Configuration(ctx.Configuration));
            #endregion

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            //app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception");

        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Portfolio.Data;
using Portfolio.Services;
using Serilog;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ProcessingResult> Get()
    {
        var xml = new XMLTradeReaderService();
        return await xml.RetrieveItems();
    }
}


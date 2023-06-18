using Microsoft.AspNetCore.Mvc;
using Portfolio.Data;
using Portfolio.Services;
using Portfolio.Services.Interfaces;
using Serilog;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportingController : ControllerBase
{

    private readonly IReportingService _reportingService;

    public ReportingController(IReportingService reportingService)
    {
        (_reportingService) = (reportingService);
    }

    [HttpGet]
    public async Task<IActionResult> Get(DateTime reportingDate)
    {
        var generatedReport = await _reportingService.GenerateReportWithItemRetrieval(new DateTime(2018, 04, 10));
        return File(generatedReport, "application/vnd.ms-excel", "Report.xlsx");
    }
}


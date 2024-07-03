using CRM.Core.Interfaces.Services.ReportsServices.ExcelReports;
using CRM.Core.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace CRM.API.Controllers.Reports.ExcelReports
{
  [ApiController]
  [Route("api/reports")]
  [Authorize(Policy = "ManagerOrUpper")]
  public class ExcelReportsController(
      IExcelReportsService reportsService
    ) : ControllerBase
  {
    private readonly IExcelReportsService _reportsService = reportsService;

    [SwaggerOperation(
      Summary = "Creates an Excel menu file.",
      OperationId = "Menu",
      Tags = ["Reports"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpGet("excel_menu")]
    public async Task<IActionResult> Menu() =>
      await _reportsService.MenuAsync();

    [SwaggerOperation(
      Summary = "Creates an Excel file with workers.",
      OperationId = "Workers",
      Tags = ["Reports"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpGet("excel_workers")]
    public async Task<IActionResult> Workers() =>
      await _reportsService.WorkersAsync();

    [SwaggerOperation(
      Summary = "Creates an Excel file with orders.",
      OperationId = "Orders",
      Tags = ["Reports"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpGet("excel_orders")]
    public async Task<IActionResult> Orders() =>
      await _reportsService.OrdersAsync();

    [SwaggerOperation(
      Summary = "Creates an Excel file with orders by date.",
      OperationId = "ByDate",
      Tags = ["Reports"]
    )]
    [SwaggerResponse(200)]
    [SwaggerResponse(400, null, typeof(ExceptionResponse))]
    [SwaggerResponse(500, null, typeof(ExceptionResponse))]
    [HttpGet("excel_orders_by_date")]
    public async Task<IActionResult> OrdersByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate) =>
      await _reportsService.OrdersByDateAsync(startDate, endDate);
  }
}
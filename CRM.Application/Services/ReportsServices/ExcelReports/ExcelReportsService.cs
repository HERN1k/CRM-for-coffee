using ClosedXML.Excel;

using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Infrastructure.Excel;
using CRM.Core.Interfaces.Services.ReportsServices.ExcelReports;

using Microsoft.AspNetCore.Mvc;

namespace CRM.Application.Services.ReportsServices.ExcelReports
{
  public class ExcelReportsService(
      IExcelService excelService,
      IExcelReportsComponents components
    ) : Controller, IExcelReportsService
  {
    private readonly IExcelService _excelService = excelService;
    private readonly IExcelReportsComponents _components = components;

    public async Task<IActionResult> MenuAsync()
    {
      XLWorkbook workbook = await _excelService.MenuWorkbook();

      using (MemoryStream stream = new MemoryStream())
      {
        workbook.SaveAs(stream);
        stream.Position = 0;

        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        string fileName = _components.GenerateFileNameMenu();

        return File(stream.ToArray(), contentType, fileName);
      }
    }

    public async Task<IActionResult> WorkersAsync()
    {
      XLWorkbook workbook = await _excelService.WorkersWorkbook();

      using (MemoryStream stream = new MemoryStream())
      {
        workbook.SaveAs(stream);
        stream.Position = 0;

        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        string fileName = _components.GenerateFileNameWorkers();

        return File(stream.ToArray(), contentType, fileName);
      }
    }

    public async Task<IActionResult> OrdersAsync()
    {
      XLWorkbook workbook = await _excelService.OrdersWorkbook();

      using (MemoryStream stream = new MemoryStream())
      {
        workbook.SaveAs(stream);
        stream.Position = 0;

        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        string fileName = _components.GenerateFileNameOrders();

        return File(stream.ToArray(), contentType, fileName);
      }
    }

    public async Task<IActionResult> OrdersByDateAsync(DateTime startDate, DateTime endDate)
    {
      if (startDate > endDate)
        throw new CustomException(ErrorTypes.BadRequest, "Start date is greater than end date");

      XLWorkbook workbook = await _excelService.OrdersByDateWorkbook(startDate, endDate);

      using (MemoryStream stream = new MemoryStream())
      {
        workbook.SaveAs(stream);
        stream.Position = 0;

        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        string fileName = _components.GenerateFileNameOrdersByDate(startDate, endDate);

        return File(stream.ToArray(), contentType, fileName);
      }
    }
  }
}
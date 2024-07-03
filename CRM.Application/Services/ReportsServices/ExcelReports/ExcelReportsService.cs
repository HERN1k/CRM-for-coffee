using System.Text;

using ClosedXML.Excel;

using CRM.Core.Enums;
using CRM.Core.Exceptions.Custom;
using CRM.Core.Interfaces.Infrastructure.Excel;
using CRM.Core.Interfaces.Services.ReportsServices.ExcelReports;

using Microsoft.AspNetCore.Mvc;

namespace CRM.Application.Services.ReportsServices.ExcelReports
{
  public class ExcelReportsService(
      IExcelService excelService
    ) : Controller, IExcelReportsService
  {
    private readonly IExcelService _excelService = excelService;

    public async Task<IActionResult> MenuAsync()
    {
      XLWorkbook workbook = await _excelService.MenuWorkbook();

      using (MemoryStream stream = new MemoryStream())
      {
        workbook.SaveAs(stream);
        stream.Position = 0;

        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        string fileName = new StringBuilder()
          .Append("Menu ")
          .Append(DateTime.Now.ToString("dd/MM/yyyy HH-mm"))
          .Append(".xlsx")
          .ToString();

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
        string fileName = new StringBuilder()
          .Append("Workers ")
          .Append(DateTime.Now.ToString("dd/MM/yyyy HH-mm"))
          .Append(".xlsx")
          .ToString();

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
        string fileName = new StringBuilder()
          .Append("Orders ")
          .Append(DateTime.Now.ToString("dd/MM/yyyy HH-mm"))
          .Append(".xlsx")
          .ToString();

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
        string fileName = new StringBuilder()
          .Append("Orders (")
          .Append(startDate.ToString("dd/MM/yyyy"))
          .Append(" - ")
          .Append(endDate.ToString("dd/MM/yyyy"))
          .Append(") ")
          .Append(DateTime.Now.ToString("dd/MM/yyyy HH-mm"))
          .Append(".xlsx")
          .ToString();

        return File(stream.ToArray(), contentType, fileName);
      }
    }
  }
}
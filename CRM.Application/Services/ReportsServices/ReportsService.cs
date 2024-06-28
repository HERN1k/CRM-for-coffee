using ClosedXML.Excel;

using CRM.Core.Excel;
using CRM.Core.Interfaces.ReportsServices;

using Microsoft.AspNetCore.Mvc;

namespace CRM.Application.Services.ReportsServices
{
  public class ReportsService(
      IExcelService excelService
    ) : Controller, IReportsService
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
        string fileName = $"Menu {DateTime.Now.ToString("dd/MM/yyyy HH-mm")}.xlsx";

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
        string fileName = $"Workers {DateTime.Now.ToString("dd/MM/yyyy HH-mm")}.xlsx";

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
        string fileName = $"Orders {DateTime.Now.ToString("dd/MM/yyyy HH-mm")}.xlsx";

        return File(stream.ToArray(), contentType, fileName);
      }
    }
  }
}
using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Services.ReportsServices.ExcelReports
{
    public interface IExcelReportsService
    {
        Task<IActionResult> MenuAsync();
        Task<IActionResult> WorkersAsync();
        Task<IActionResult> OrdersAsync();
        Task<IActionResult> OrdersByDateAsync(DateTime startDate, DateTime endDate);
    }
}
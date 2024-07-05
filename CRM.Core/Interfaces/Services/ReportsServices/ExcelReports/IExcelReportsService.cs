using CRM.Core.Exceptions.Custom;

using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Interfaces.Services.ReportsServices.ExcelReports
{
  /// <summary>
  ///   Provides services for generating and downloading Excel reports.
  /// </summary>
  public interface IExcelReportsService
  {
    /// <summary>
    ///   Generates an Excel report for the menu and returns it as a file result.
    /// </summary>
    /// <returns>
    ///   A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/> for the menu report.
    /// </returns>
    Task<IActionResult> MenuAsync();

    /// <summary>
    ///   Generates an Excel report for workers and returns it as a file result.
    /// </summary>
    /// <returns>
    ///   A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/> for the workers report.
    /// </returns>
    Task<IActionResult> WorkersAsync();

    /// <summary>
    ///   Generates an Excel report for orders and returns it as a file result.
    /// </summary>
    /// <returns>
    ///   A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/> for the orders report.
    /// </returns>
    Task<IActionResult> OrdersAsync();

    /// <summary>
    ///   Generates an Excel report for orders within a specified date range and returns it as a file result.
    /// </summary>
    /// <param name="startDate">The start date of the date range.</param>
    /// <param name="endDate">The end date of the date range.</param>
    /// <returns>
    ///   A task representing the asynchronous operation. The task result contains an <see cref="IActionResult"/> for the orders by date report.
    /// </returns>
    /// <exception cref="CustomException">Thrown if the start date is greater than the end date.</exception>
    Task<IActionResult> OrdersByDateAsync(DateTime startDate, DateTime endDate);
  }
}
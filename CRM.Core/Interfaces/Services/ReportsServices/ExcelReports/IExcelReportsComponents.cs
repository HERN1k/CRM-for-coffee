namespace CRM.Core.Interfaces.Services.ReportsServices.ExcelReports
{
  /// <summary>
  ///   Provides methods for generating filenames for Excel reports.
  /// </summary>
  public interface IExcelReportsComponents
  {
    /// <summary>
    ///   Generates a filename for the menu report.
    /// </summary>
    /// <returns>The generated filename for the menu report.</returns>
    string GenerateFileNameMenu();

    /// <summary>
    ///   Generates a filename for the workers report.
    /// </summary>
    /// <returns>The generated filename for the workers report.</returns>
    string GenerateFileNameWorkers();

    /// <summary>
    ///   Generates a filename for the orders report.
    /// </summary>
    /// <returns>The generated filename for the orders report.</returns>
    string GenerateFileNameOrders();

    /// <summary>
    ///   Generates a filename for the orders report by date range.
    /// </summary>
    /// <param name="startDate">The start date of the date range.</param>
    /// <param name="endDate">The end date of the date range.</param>
    /// <returns>The generated filename for the orders report by date range.</returns>
    string GenerateFileNameOrdersByDate(DateTime startDate, DateTime endDate);
  }
}
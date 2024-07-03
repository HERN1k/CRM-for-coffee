using ClosedXML.Excel;

namespace CRM.Core.Interfaces.Infrastructure.Excel
{
    /// <summary>
    ///   Provides methods for generating Excel workbooks with various types of data.
    /// </summary>
    public interface IExcelService
    {
        /// <summary>
        ///   Asynchronously creates and populates an Excel workbook with menu-related data
        /// </summary>
        Task<XLWorkbook> MenuWorkbook();

        /// <summary>
        ///   Asynchronously creates and populates an Excel workbook with workers-related data
        /// </summary>
        Task<XLWorkbook> WorkersWorkbook();

        /// <summary>
        ///   Asynchronously creates and populates an Excel workbook with orders-related data.
        /// </summary>
        Task<XLWorkbook> OrdersWorkbook();

        /// <summary>
        ///   Asynchronously creates and populates an Excel workbook with orders-related data for a specified date range
        /// </summary>
        Task<XLWorkbook> OrdersByDateWorkbook(DateTime startDate, DateTime endDate);
    }
}
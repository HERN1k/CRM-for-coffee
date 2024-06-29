using ClosedXML.Excel;

namespace CRM.Core.Excel
{
  public interface IExcelService
  {
    Task<XLWorkbook> MenuWorkbook();
    Task<XLWorkbook> WorkersWorkbook();
    Task<XLWorkbook> OrdersWorkbook();
    Task<XLWorkbook> OrdersByDateWorkbook(DateTime startDate, DateTime endDate);
  }
}
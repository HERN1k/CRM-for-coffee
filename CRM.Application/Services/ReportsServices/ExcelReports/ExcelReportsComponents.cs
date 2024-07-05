using System.Text;

using CRM.Core.Interfaces.Services.ReportsServices.ExcelReports;

namespace CRM.Application.Services.ReportsServices.ExcelReports
{
  public class ExcelReportsComponents : IExcelReportsComponents
  {
    public string GenerateFileNameMenu()
    {
      string fileName = new StringBuilder()
        .Append("Menu ")
        .Append(DateTime.Now.ToString("dd/MM/yyyy HH-mm"))
        .Append(".xlsx")
        .ToString();
      return fileName;
    }

    public string GenerateFileNameWorkers()
    {
      string fileName = new StringBuilder()
        .Append("Workers ")
        .Append(DateTime.Now.ToString("dd/MM/yyyy HH-mm"))
        .Append(".xlsx")
        .ToString();
      return fileName;
    }

    public string GenerateFileNameOrders()
    {
      string fileName = new StringBuilder()
        .Append("Orders ")
        .Append(DateTime.Now.ToString("dd/MM/yyyy HH-mm"))
        .Append(".xlsx")
        .ToString();
      return fileName;
    }

    public string GenerateFileNameOrdersByDate(DateTime startDate, DateTime endDate)
    {
      string fileName = new StringBuilder()
        .Append("Orders (")
        .Append(startDate.ToString("dd/MM/yyyy"))
        .Append(" - ")
        .Append(endDate.ToString("dd/MM/yyyy"))
        .Append(") ")
        .Append(DateTime.Now.ToString("dd/MM/yyyy HH-mm"))
        .Append(".xlsx")
        .ToString();
      return fileName;
    }
  }
}
using ClosedXML.Excel;

using CRM.Core.Excel;
using CRM.Core.Interfaces.Repositories;

namespace CRM.Infrastructure.Excel
{
  public partial class ExcelService(
      IRepository repository
    ) : IExcelService
  {
    private readonly IRepository _repository = repository;

    public async Task<XLWorkbook> MenuWorkbook()
    {
      var workbook = new XLWorkbook();

      workbook.Worksheets.Add("Product categories");
      workbook.Worksheets.Add("Products");
      workbook.Worksheets.Add("Addons");

      FillingProductCategoriesSheet(workbook.Worksheet("Product categories"));
      await FillingProductsSheet(workbook.Worksheet("Products"));
      await FillingAddOnsSheet(workbook.Worksheet("Addons"));

      return workbook;
    }

    public async Task<XLWorkbook> WorkersWorkbook()
    {
      var workbook = new XLWorkbook();

      workbook.Worksheets.Add("Workers");

      await FillingWorkersSheet(workbook.Worksheet("Workers"));

      return workbook;
    }

    public async Task<XLWorkbook> OrdersWorkbook()
    {
      var workbook = new XLWorkbook();

      workbook.Worksheets.Add("Orders");
      workbook.Worksheets.Add("Products");
      workbook.Worksheets.Add("Addons");

      var orders = FindEntitiesOrder();

      await FillingOrdersSheet(workbook.Worksheet("Orders"), orders);
      await FillingOrderProductsSheet(workbook.Worksheet("Products"), orders);
      await FillingOrderAddOnsSheet(workbook.Worksheet("Addons"), orders);

      return workbook;
    }

    public async Task<XLWorkbook> OrdersByDateWorkbook(DateTime startDate, DateTime endDate)
    {
      var workbook = new XLWorkbook();

      workbook.Worksheets.Add("Orders");
      workbook.Worksheets.Add("Products");
      workbook.Worksheets.Add("Addons");

      var orders = FindEntitiesOrderByDate(startDate, endDate);

      await FillingOrdersSheet(workbook.Worksheet("Orders"), orders);
      await FillingOrderProductsSheet(workbook.Worksheet("Products"), orders);
      await FillingOrderAddOnsSheet(workbook.Worksheet("Addons"), orders);

      return workbook;
    }
  }
}
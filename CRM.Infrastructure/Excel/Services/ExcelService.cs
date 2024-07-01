using ClosedXML.Excel;

using CRM.Core.Interfaces.Excel;
using CRM.Core.Interfaces.Repositories;

namespace CRM.Infrastructure.Excel.Services
{
  public class ExcelService(
      IExcelFillingSheet fillingSheet,
      IExcelRepository repository
    ) : IExcelService
  {
    private readonly IExcelFillingSheet _fillingSheet = fillingSheet;
    private readonly IExcelRepository _repository = repository;

    public async Task<XLWorkbook> MenuWorkbook()
    {
      var workbook = new XLWorkbook();

      workbook.Worksheets.Add("Product categories");
      workbook.Worksheets.Add("Products");
      workbook.Worksheets.Add("Addons");

      _fillingSheet.ProductCategories(workbook.Worksheet("Product categories"));
      await _fillingSheet.Products(workbook.Worksheet("Products"));
      await _fillingSheet.AddOns(workbook.Worksheet("Addons"));

      return workbook;
    }

    public async Task<XLWorkbook> WorkersWorkbook()
    {
      var workbook = new XLWorkbook();

      workbook.Worksheets.Add("Workers");

      await _fillingSheet.Workers(workbook.Worksheet("Workers"));

      return workbook;
    }

    public async Task<XLWorkbook> OrdersWorkbook()
    {
      var workbook = new XLWorkbook();

      workbook.Worksheets.Add("Orders");
      workbook.Worksheets.Add("Products");
      workbook.Worksheets.Add("Addons");

      var orders = _repository.FindEntitiesOrder();

      await _fillingSheet.Orders(workbook.Worksheet("Orders"), orders);
      await _fillingSheet.OrderProducts(workbook.Worksheet("Products"), orders);
      await _fillingSheet.OrderAddOns(workbook.Worksheet("Addons"), orders);

      return workbook;
    }

    public async Task<XLWorkbook> OrdersByDateWorkbook(DateTime startDate, DateTime endDate)
    {
      var workbook = new XLWorkbook();

      workbook.Worksheets.Add("Orders");
      workbook.Worksheets.Add("Products");
      workbook.Worksheets.Add("Addons");

      var orders = _repository.FindEntitiesOrderByDate(startDate, endDate);

      await _fillingSheet.Orders(workbook.Worksheet("Orders"), orders);
      await _fillingSheet.OrderProducts(workbook.Worksheet("Products"), orders);
      await _fillingSheet.OrderAddOns(workbook.Worksheet("Addons"), orders);

      return workbook;
    }
  }
}
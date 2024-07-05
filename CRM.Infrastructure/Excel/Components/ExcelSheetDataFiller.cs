using ClosedXML.Excel;

using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Interfaces.Infrastructure.Excel;
using CRM.Core.Interfaces.Repositories.Excel;
using CRM.Core.Models;

namespace CRM.Infrastructure.Excel.Components
{
  public class ExcelSheetDataFiller(
      IExcelRepository repository,
      IExcelStyles styles
    ) : IExcelSheetDataFiller
  {
    private readonly IExcelRepository _repository = repository;
    private readonly IExcelStyles _styles = styles;

    public int FillProductCategories(IXLWorksheet sheet)
    {
      List<ProductCategory> productCategories = _repository.FindEntitiesProductCategory();

      int rowIndex = 2;

      foreach (var category in productCategories)
      {
        int numberProducts = category.Products != null ? category.Products.Count : 0;
        rowIndex++;
        sheet.Cell("B" + rowIndex).Value = rowIndex - 2;
        sheet.Cell("C" + rowIndex).Value = category.Name;
        sheet.Cell("D" + rowIndex).Value = numberProducts;
      }

      return rowIndex;
    }

    public async Task<int> FillProducts(IXLWorksheet sheet)
    {
      List<Product> products = await _repository.FindEntities<Product, EntityProduct>();

      int rowIndex = 2;

      foreach (var product in products)
      {
        var tempCategory = await _repository.FindEntityById<ProductCategory, EntityProductCategory>(product.ProductCategoryId);
        int tempNumberAddons = _repository.NumberEntitiesWithPredicate<EntityAddOn>(e => e.ProductId == product.Id);

        rowIndex++;
        sheet.Cell("B" + rowIndex).Value = rowIndex - 2;
        sheet.Cell("C" + rowIndex).Value = tempCategory.Name;
        sheet.Cell("D" + rowIndex).Value = product.Name;
        sheet.Cell("E" + rowIndex).Value = product.Price;
        sheet.Cell("F" + rowIndex).Value = tempNumberAddons;
      }

      return rowIndex;
    }

    public async Task<int> FillAddOns(IXLWorksheet sheet)
    {
      List<AddOn> addOns = await _repository.FindEntities<AddOn, EntityAddOn>();

      int rowIndex = 2;

      foreach (var addOn in addOns)
      {
        Product tempProduct = await _repository.FindEntityById<Product, EntityProduct>(addOn.ProductId);

        rowIndex++;
        sheet.Cell("B" + rowIndex).Value = rowIndex - 2;
        sheet.Cell("C" + rowIndex).Value = tempProduct.Name;
        sheet.Cell("D" + rowIndex).Value = addOn.Name;
        sheet.Cell("E" + rowIndex).Value = addOn.Price;
      }

      return rowIndex;
    }

    public async Task<int> FillWorkers(IXLWorksheet sheet)
    {
      List<User> workers = await _repository.FindEntities<User, EntityUser>();

      int rowIndex = 2;

      foreach (var worker in workers)
      {
        rowIndex++;
        sheet.Cell("B" + rowIndex).Value = rowIndex - 2;
        sheet.Cell("C" + rowIndex).Value = worker.FirstName;
        sheet.Cell("D" + rowIndex).Value = worker.LastName;
        sheet.Cell("E" + rowIndex).Value = worker.FatherName;
        sheet.Cell("F" + rowIndex).Value = worker.Age;
        sheet.Cell("G" + rowIndex).Value = worker.Gender;

        bool isSuccess = long.TryParse(worker.PhoneNumber[1..], out long tempPhoneNumber);
        sheet.Cell("H" + rowIndex).Value = string.Format("{0:+## (###) ###-##-##}", tempPhoneNumber);

        sheet.Cell("I" + rowIndex).Value = worker.Email;
        sheet.Cell("J" + rowIndex).Value = worker.Post;
        sheet.Cell("K" + rowIndex).Value = worker.RegistrationDate;
      }

      return rowIndex;
    }

    public async Task<int> FillOrders(IXLWorksheet sheet, List<Order> orders)
    {
      int rowIndex = 2;

      foreach (var order in orders)
      {
        User tempWorker = await _repository.FindEntityById<User, EntityUser>(order.WorkerId);

        int numberAddOns = 0;
        foreach (var product in order.Products)
        {
          numberAddOns += product.AddOns.Count;
        }

        rowIndex++;
        sheet.Cell("B" + rowIndex).Value = rowIndex - 2;
        sheet.Cell("C" + rowIndex).Value = order.Id.ToString();
        sheet.Cell("D" + rowIndex).Value = order.Total;
        sheet.Cell("E" + rowIndex).Value = order.Taxes;
        sheet.Cell("F" + rowIndex).Value = ((PaymentMethods)order.PaymentMethod).ToString();
        sheet.Cell("G" + rowIndex).Value = tempWorker.Email;
        sheet.Cell("H" + rowIndex).Value = order.OrderСreationDate;
        sheet.Cell("I" + rowIndex).Value = order.Products.Count;
        sheet.Cell("J" + rowIndex).Value = numberAddOns;
      }

      return rowIndex;
    }

    public async Task<int> FillOrderProductsRow(IXLWorksheet sheet, Order order, int rowIndex)
    {
      int index = rowIndex;
      _styles.ItemTitleStyles(sheet, index, 2, "ID:", order.Id.ToString());

      index = await FillOrderProducts(sheet, order, index);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = $"B{rowIndex + 1}", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = $"C{rowIndex + 1}", Value = "Product category", Format = ExcelCellFormat.String },
        new() { CellAddress = $"D{rowIndex + 1}", Value = "Name of product", Format = ExcelCellFormat.String },
        new() { CellAddress = $"E{rowIndex + 1}", Value = "Amount", Format = ExcelCellFormat.Number },
        new() { CellAddress = $"F{rowIndex + 1}", Value = "Price", Format = ExcelCellFormat.Money },
        new() { CellAddress = $"G{rowIndex + 1}", Value = "Total", Format = ExcelCellFormat.Money },
        new() { CellAddress = $"H{rowIndex + 1}", Value = "Number of addons", Format = ExcelCellFormat.Number },
      };

      _styles.TitleStyles(sheet, rowIndex + 1, titleCellList);

      return index;
    }

    public async Task<int> FillOrderAddOnsRow(IXLWorksheet sheet, Order order, int rowIndex)
    {
      int index = rowIndex;
      _styles.ItemTitleStyles(sheet, index, 2, "ID:", order.Id.ToString());

      index = await FillOrderAddOns(sheet, order, index);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = $"B{rowIndex + 1}", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = $"C{rowIndex + 1}", Value = "Product", Format = ExcelCellFormat.String },
        new() { CellAddress = $"D{rowIndex + 1}", Value = "Name of addon", Format = ExcelCellFormat.String },
        new() { CellAddress = $"E{rowIndex + 1}", Value = "Amount", Format = ExcelCellFormat.Number },
        new() { CellAddress = $"F{rowIndex + 1}", Value = "Price", Format = ExcelCellFormat.Money },
        new() { CellAddress = $"G{rowIndex + 1}", Value = "Total", Format = ExcelCellFormat.Money },
      };

      _styles.TitleStyles(sheet, rowIndex + 1, titleCellList);

      return index;
    }

    /// <summary>
    ///   Asynchronously fills the worksheet with order products data.
    /// </summary>
    /// <param name="sheet">The worksheet to fill with order products data.</param>
    /// <param name="order">The order containing the products to use for filling the worksheet.</param>
    /// <param name="index">The starting row index to fill the data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the last row index of the filled data.</returns>
    private async Task<int> FillOrderProducts(IXLWorksheet sheet, Order order, int index)
    {
      int rowIndex = index;

      int indexId = 0;

      rowIndex++;
      foreach (var product in order.Products)
      {
        var tempProduct = await _repository
          .FindEntityById<Product, EntityProduct>(product.ProductId);
        var tempProductCategory = await _repository
          .FindEntityById<ProductCategory, EntityProductCategory>(tempProduct.ProductCategoryId);

        rowIndex++;
        indexId++;
        sheet.Cell("B" + rowIndex).Value = indexId;
        sheet.Cell("C" + rowIndex).Value = tempProductCategory.Name;
        sheet.Cell("D" + rowIndex).Value = tempProduct.Name;
        sheet.Cell("E" + rowIndex).Value = product.Amount;
        sheet.Cell("F" + rowIndex).Value = tempProduct.Price;
        sheet.Cell("G" + rowIndex).Value = product.Amount * tempProduct.Price;
        sheet.Cell("H" + rowIndex).Value = product.AddOns.Count;
      }

      _styles.MainTableStyle(sheet, "B", index + 1, "H", rowIndex);

      return rowIndex;
    }

    /// <summary>
    ///   Asynchronously fills the worksheet with order addons data.
    /// </summary>
    /// <param name="sheet">The worksheet to fill with order addons data.</param>
    /// <param name="order">The order entity containing the addons to use for filling the worksheet.</param>
    /// <param name="index">The starting row index to fill the data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the last row index of the filled data.</returns>
    private async Task<int> FillOrderAddOns(IXLWorksheet sheet, Order order, int index)
    {
      int rowIndex = index;

      int indexId = 0;

      rowIndex++;
      foreach (var product in order.Products)
      {
        var tempProduct = await _repository
          .FindEntityById<Product, EntityProduct>(product.ProductId);

        foreach (var addOn in product.AddOns)
        {
          var tempAddOn = await _repository
            .FindEntityById<AddOn, EntityAddOn>(addOn.AddOnId);

          rowIndex++;
          indexId++;
          sheet.Cell("B" + rowIndex).Value = indexId;
          sheet.Cell("C" + rowIndex).Value = tempProduct.Name;
          sheet.Cell("D" + rowIndex).Value = tempAddOn.Name;
          sheet.Cell("E" + rowIndex).Value = addOn.Amount;
          sheet.Cell("F" + rowIndex).Value = tempAddOn.Price;
          sheet.Cell("G" + rowIndex).Value = addOn.Amount * tempAddOn.Price;
        }
      }

      _styles.MainTableStyle(sheet, "B", index + 1, "G", rowIndex);

      return rowIndex;
    }
  }
}
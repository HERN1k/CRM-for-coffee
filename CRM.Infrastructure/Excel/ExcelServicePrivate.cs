using System.Linq.Expressions;

using ClosedXML.Excel;

using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Interfaces.Entity;

using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Excel
{
  public partial class ExcelService
  {
    private void FillingTitleStyles(IXLWorksheet sheet, int row, List<TitleCellStylesDto> columns)
    {
      foreach (var column in columns)
      {
        sheet.Cell(column.CellAddress).Value = $"   {column.Value}   ";
        sheet.Cell(column.CellAddress).Style.Font.Bold = true;
        sheet.Cell(column.CellAddress).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        sheet.Cell(column.CellAddress).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        sheet.Cell(column.CellAddress).Style.Border.TopBorder = XLBorderStyleValues.Medium;
        sheet.Cell(column.CellAddress).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
        sheet.Cell(column.CellAddress).Style.Border.RightBorder = XLBorderStyleValues.Medium;
        sheet.Cell(column.CellAddress).Style.Border.LeftBorder = XLBorderStyleValues.Medium;

        sheet.Row(row).Height = 25;

        if (column.Value == "№")
          sheet.Column(column.CellAddress[0].ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        switch (column.Format)
        {
          case ExcelCellFormat.String:
            sheet.Column(column.CellAddress[0].ToString()).Style.NumberFormat.SetFormat("@");
            break;
          case ExcelCellFormat.Number:
            sheet.Column(column.CellAddress[0].ToString()).Style.NumberFormat.SetFormat("0");
            break;
          case ExcelCellFormat.Money:
            sheet.Column(column.CellAddress[0].ToString()).Style.NumberFormat.SetFormat("#,##0.00 \"₴\"");
            break;
          case ExcelCellFormat.Date:
            sheet.Column(column.CellAddress[0].ToString()).Style.DateFormat.SetFormat("dd.mm.yyyy hh:mm:ss");
            break;
        }
      }
    }
    private void MainTableStyle(IXLWorksheet sheet, string firstRow, int indexFirstRow, string seccondRow, int indexSeccondRow)
    {
      string rangeAddress = $"{firstRow + indexFirstRow}:{seccondRow + indexSeccondRow}";
      var table = sheet.Range(rangeAddress);
      table.Style.Border.TopBorder = XLBorderStyleValues.Thin;
      table.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
      table.Style.Border.RightBorder = XLBorderStyleValues.Thin;
      table.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
    }
    private IQueryable<EntityProductCategory> FindEntitiesProductCategory()
    {
      var result = _repository
        .GetQueryable<EntityProductCategory>()
        .AsNoTracking()
        .Include(e => e.Products)
        .OrderBy(e => e.Name)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }
    private IQueryable<EntityOrder> FindEntitiesOrder()
    {
      var result = _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Include(e => e.Products)
          .ThenInclude(e => e.AddOns)
        .OrderBy(e => e.OrderСreationDate)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }
    private IQueryable<EntityOrder> FindEntitiesOrderByDate(DateTime startDate, DateTime endDate)
    {
      var result = _repository
        .GetQueryable<EntityOrder>()
        .AsNoTracking()
        .Where(e => e.OrderСreationDate > startDate.ToUniversalTime())
        .Where(e => e.OrderСreationDate < endDate.ToUniversalTime())
        .Include(e => e.Products)
          .ThenInclude(e => e.AddOns)
        .OrderBy(e => e.OrderСreationDate)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }
    private async Task<IEnumerable<T>> FindEntities<T>() where T : class
    {
      var result = await _repository
        .GetEnumerable<T>()
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }
    private async Task<T> FindEntityById<T>(Guid id) where T : class, IEntityWithId
    {
      var result = await _repository
        .FindSingleAsync<T>(e => e.Id == id)
          ?? throw new CustomException(ErrorTypes.ServerError, "Server error");
      return result;
    }
    private int NumberEntitiesWithPredicate<T>(Expression<Func<T, bool>> predicate) where T : class
    {
      int result = _repository
        .GetQueryable<T>()
        .AsNoTracking()
        .Where(predicate)
        .Count();
      return result;
    }
    private int FillingDataProductCategoriesSheet(IXLWorksheet sheet)
    {
      var entityProductCategories = FindEntitiesProductCategory();

      int rowIndex = 2;

      foreach (var category in entityProductCategories)
      {
        int numberProducts = category.Products != null ? category.Products.Count : 0;
        rowIndex++;
        sheet.Cell("B" + rowIndex).Value = rowIndex - 2;
        sheet.Cell("C" + rowIndex).Value = category.Name;
        sheet.Cell("D" + rowIndex).Value = numberProducts;
      }

      return rowIndex;
    }
    private async Task<int> FillingDataProductsSheet(IXLWorksheet sheet)
    {
      var entityProducts = await FindEntities<EntityProduct>();

      int rowIndex = 2;

      foreach (var product in entityProducts)
      {
        var tempEntityCategory = await FindEntityById<EntityProductCategory>(product.ProductCategoryId);
        int tempNumberAddons = NumberEntitiesWithPredicate<EntityAddOn>(e => e.ProductId == product.Id);

        rowIndex++;
        sheet.Cell("B" + rowIndex).Value = rowIndex - 2;
        sheet.Cell("C" + rowIndex).Value = tempEntityCategory.Name;
        sheet.Cell("D" + rowIndex).Value = product.Name;
        sheet.Cell("E" + rowIndex).Value = product.Price;
        sheet.Cell("F" + rowIndex).Value = tempNumberAddons;
      }

      return rowIndex;
    }
    private async Task<int> FillingDataAddOnsSheet(IXLWorksheet sheet)
    {
      var entityAddOns = await FindEntities<EntityAddOn>();

      int rowIndex = 2;

      foreach (var addOn in entityAddOns)
      {
        var tempEntityProduct = await FindEntityById<EntityProduct>(addOn.ProductId);

        rowIndex++;
        sheet.Cell("B" + rowIndex).Value = rowIndex - 2;
        sheet.Cell("C" + rowIndex).Value = tempEntityProduct.Name;
        sheet.Cell("D" + rowIndex).Value = addOn.Name;
        sheet.Cell("E" + rowIndex).Value = addOn.Price;
      }

      return rowIndex;
    }
    private async Task<int> FillingDataWorkersSheet(IXLWorksheet sheet)
    {
      var entityWorkers = await FindEntities<EntityUser>();

      int rowIndex = 2;

      foreach (var worker in entityWorkers)
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
    private async Task<int> FillingDataOrdersSheet(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders)
    {
      int rowIndex = 2;

      foreach (var order in entityOrders)
      {
        var tempWorker = await FindEntityById<EntityUser>(order.WorkerId);

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
    private void FillingProductCategoriesSheet(IXLWorksheet sheet)
    {
      int rowIndex = FillingDataProductCategoriesSheet(sheet);

      MainTableStyle(sheet, "B", 3, "D", rowIndex);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = "B2", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = "C2", Value = "Name", Format = ExcelCellFormat.String },
        new() { CellAddress = "D2", Value = "Number of products", Format = ExcelCellFormat.Number }
      };

      FillingTitleStyles(sheet, 2, titleCellList);

      sheet.Columns().AdjustToContents();
    }
    private async Task FillingProductsSheet(IXLWorksheet sheet)
    {
      int rowIndex = await FillingDataProductsSheet(sheet);

      MainTableStyle(sheet, "B", 3, "F", rowIndex);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = "B2", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = "C2", Value = "Category", Format = ExcelCellFormat.String },
        new() { CellAddress = "D2", Value = "Name", Format = ExcelCellFormat.String },
        new() { CellAddress = "E2", Value = "Price", Format = ExcelCellFormat.Money },
        new() { CellAddress = "F2", Value = "Number of addons", Format = ExcelCellFormat.Number }
      };

      FillingTitleStyles(sheet, 2, titleCellList);

      sheet.Columns().AdjustToContents();
    }
    private async Task FillingAddOnsSheet(IXLWorksheet sheet)
    {
      int rowIndex = await FillingDataAddOnsSheet(sheet);

      MainTableStyle(sheet, "B", 3, "E", rowIndex);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = "B2", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = "C2", Value = "Product", Format = ExcelCellFormat.String },
        new() { CellAddress = "D2", Value = "Name", Format = ExcelCellFormat.String },
        new() { CellAddress = "E2", Value = "Price", Format = ExcelCellFormat.Money }
      };

      FillingTitleStyles(sheet, 2, titleCellList);

      sheet.Columns().AdjustToContents();
    }
    private async Task FillingWorkersSheet(IXLWorksheet sheet)
    {
      int rowIndex = await FillingDataWorkersSheet(sheet);

      MainTableStyle(sheet, "B", 3, "K", rowIndex);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = "B2", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = "C2", Value = "First name", Format = ExcelCellFormat.String },
        new() { CellAddress = "D2", Value = "Last name", Format = ExcelCellFormat.String },
        new() { CellAddress = "E2", Value = "Father name", Format = ExcelCellFormat.String },
        new() { CellAddress = "F2", Value = "Age", Format = ExcelCellFormat.Number },
        new() { CellAddress = "G2", Value = "Gender", Format = ExcelCellFormat.String },
        new() { CellAddress = "H2", Value = "Phone number", Format = ExcelCellFormat.String },
        new() { CellAddress = "I2", Value = "Email", Format = ExcelCellFormat.String },
        new() { CellAddress = "J2", Value = "Post", Format = ExcelCellFormat.String },
        new() { CellAddress = "K2", Value = "Registration date", Format = ExcelCellFormat.Date }
      };

      FillingTitleStyles(sheet, 2, titleCellList);

      sheet.Columns().AdjustToContents();
    }
    private async Task FillingOrdersSheet(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders)
    {
      int rowIndex = await FillingDataOrdersSheet(sheet, entityOrders);

      MainTableStyle(sheet, "B", 3, "J", rowIndex);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = "B2", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = "C2", Value = "ID", Format = ExcelCellFormat.String },
        new() { CellAddress = "D2", Value = "Total", Format = ExcelCellFormat.Money },
        new() { CellAddress = "E2", Value = "Taxes", Format = ExcelCellFormat.Money },
        new() { CellAddress = "F2", Value = "Payment method", Format = ExcelCellFormat.String },
        new() { CellAddress = "G2", Value = "Worker email", Format = ExcelCellFormat.String },
        new() { CellAddress = "H2", Value = "Order creation date", Format = ExcelCellFormat.Date },
        new() { CellAddress = "I2", Value = "Number of products", Format = ExcelCellFormat.Number },
        new() { CellAddress = "J2", Value = "Number of addons", Format = ExcelCellFormat.Number }
      };

      FillingTitleStyles(sheet, 2, titleCellList);

      sheet.Columns().AdjustToContents();
    }
    private void FillingItemTitleStyles(IXLWorksheet sheet, int row, int column, string name, string value)
    {
      sheet.Cell(row, column).Value = $"   {name}   ";
      sheet.Cell(row, column).Style.Font.Bold = true;
      sheet.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      sheet.Cell(row, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      sheet.Cell(row, column).Style.Font.FontSize = 15;

      sheet.Cell(row, column + 1).Value = value;
      sheet.Cell(row, column + 1).Style.Font.Bold = true;
      sheet.Cell(row, column + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
      sheet.Cell(row, column + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      sheet.Cell(row, column + 1).Style.Font.FontSize = 15;

      sheet.Row(row).Height = 25;
    }
    private async Task<int> SetDataOrderProductsSheet(IXLWorksheet sheet, EntityOrder order, int index)
    {
      int rowIndex = index;

      int indexId = 0;

      rowIndex++;
      foreach (var product in order.Products)
      {
        var tempEntityProduct = await FindEntityById<EntityProduct>(product.ProductId);
        var tempEntityProductCategory = await FindEntityById<EntityProductCategory>(tempEntityProduct.ProductCategoryId);

        rowIndex++;
        indexId++;
        sheet.Cell("B" + rowIndex).Value = indexId;
        sheet.Cell("C" + rowIndex).Value = tempEntityProductCategory.Name;
        sheet.Cell("D" + rowIndex).Value = tempEntityProduct.Name;
        sheet.Cell("E" + rowIndex).Value = product.Amount;
        sheet.Cell("F" + rowIndex).Value = tempEntityProduct.Price;
        sheet.Cell("G" + rowIndex).Value = product.Amount * tempEntityProduct.Price;
        sheet.Cell("H" + rowIndex).Value = product.AddOns.Count;
      }

      MainTableStyle(sheet, "B", index + 1, "H", rowIndex);

      return rowIndex;
    }
    private async Task<int> SetDataOrderAddOnsSheet(IXLWorksheet sheet, EntityOrder order, int index)
    {
      int rowIndex = index;

      int indexId = 0;

      rowIndex++;
      foreach (var product in order.Products)
      {
        var tempEntityProduct = await FindEntityById<EntityProduct>(product.ProductId);
        foreach (var addOn in product.AddOns)
        {
          var tempEntityAddOn = await FindEntityById<EntityAddOn>(addOn.AddOnId);

          rowIndex++;
          indexId++;
          sheet.Cell("B" + rowIndex).Value = indexId;
          sheet.Cell("C" + rowIndex).Value = tempEntityProduct.Name;
          sheet.Cell("D" + rowIndex).Value = tempEntityAddOn.Name;
          sheet.Cell("E" + rowIndex).Value = addOn.Amount;
          sheet.Cell("F" + rowIndex).Value = tempEntityAddOn.Price;
          sheet.Cell("G" + rowIndex).Value = addOn.Amount * tempEntityAddOn.Price;
        }
      }

      MainTableStyle(sheet, "B", index + 1, "G", rowIndex);

      return rowIndex;
    }
    private async Task<int> FillingDataOrderProductsSheet(IXLWorksheet sheet, EntityOrder order, int rowIndex)
    {
      int index = rowIndex;
      FillingItemTitleStyles(sheet, index, 2, "ID:", order.Id.ToString());

      index = await SetDataOrderProductsSheet(sheet, order, index);

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

      FillingTitleStyles(sheet, rowIndex + 1, titleCellList);

      return index;
    }
    private async Task<int> FillingDataOrderAddOnsSheet(IXLWorksheet sheet, EntityOrder order, int rowIndex)
    {
      int index = rowIndex;
      FillingItemTitleStyles(sheet, index, 2, "ID:", order.Id.ToString());

      index = await SetDataOrderAddOnsSheet(sheet, order, index);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = $"B{rowIndex + 1}", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = $"C{rowIndex + 1}", Value = "Product", Format = ExcelCellFormat.String },
        new() { CellAddress = $"D{rowIndex + 1}", Value = "Name of addon", Format = ExcelCellFormat.String },
        new() { CellAddress = $"E{rowIndex + 1}", Value = "Amount", Format = ExcelCellFormat.Number },
        new() { CellAddress = $"F{rowIndex + 1}", Value = "Price", Format = ExcelCellFormat.Money },
        new() { CellAddress = $"G{rowIndex + 1}", Value = "Total", Format = ExcelCellFormat.Money },
      };

      FillingTitleStyles(sheet, rowIndex + 1, titleCellList);

      return index;
    }
    private async Task FillingOrderProductsSheet(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders)
    {
      int rowIndex = 1;
      foreach (var order in entityOrders)
      {
        rowIndex++;
        rowIndex = await FillingDataOrderProductsSheet(sheet, order, rowIndex);
        rowIndex += 3;
      }

      sheet.Columns().AdjustToContents();
    }
    private async Task FillingOrderAddOnsSheet(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders)
    {
      int rowIndex = 1;

      foreach (var order in entityOrders)
      {
        rowIndex++;
        rowIndex = await FillingDataOrderAddOnsSheet(sheet, order, rowIndex);
        rowIndex += 3;
      }

      sheet.Columns().AdjustToContents();
    }
  }
}
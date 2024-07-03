using ClosedXML.Excel;

using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Entities;
using CRM.Core.Enums;
using CRM.Core.Interfaces.Infrastructure.Excel;

namespace CRM.Infrastructure.Excel.Components
{
    public class ExcelFillingSheet(
      IExcelStyles styles,
      IExcelSheetDataFiller dataFiller
    ) : IExcelFillingSheet
  {
    private readonly IExcelStyles _styles = styles;
    private readonly IExcelSheetDataFiller _dataFiller = dataFiller;

    public void ProductCategories(IXLWorksheet sheet)
    {
      int rowIndex = _dataFiller.FillProductCategories(sheet);

      _styles.MainTableStyle(sheet, "B", 3, "D", rowIndex);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = "B2", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = "C2", Value = "Name", Format = ExcelCellFormat.String },
        new() { CellAddress = "D2", Value = "Number of products", Format = ExcelCellFormat.Number }
      };

      _styles.TitleStyles(sheet, 2, titleCellList);

      sheet.Columns().AdjustToContents();
    }

    public async Task Products(IXLWorksheet sheet)
    {
      int rowIndex = await _dataFiller.FillProducts(sheet);

      _styles.MainTableStyle(sheet, "B", 3, "F", rowIndex);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = "B2", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = "C2", Value = "Category", Format = ExcelCellFormat.String },
        new() { CellAddress = "D2", Value = "Name", Format = ExcelCellFormat.String },
        new() { CellAddress = "E2", Value = "Price", Format = ExcelCellFormat.Money },
        new() { CellAddress = "F2", Value = "Number of addons", Format = ExcelCellFormat.Number }
      };

      _styles.TitleStyles(sheet, 2, titleCellList);

      sheet.Columns().AdjustToContents();
    }

    public async Task AddOns(IXLWorksheet sheet)
    {
      int rowIndex = await _dataFiller.FillAddOns(sheet);

      _styles.MainTableStyle(sheet, "B", 3, "E", rowIndex);

      List<TitleCellStylesDto> titleCellList = new List<TitleCellStylesDto>()
      {
        new() { CellAddress = "B2", Value = "№", Format = ExcelCellFormat.Number },
        new() { CellAddress = "C2", Value = "Product", Format = ExcelCellFormat.String },
        new() { CellAddress = "D2", Value = "Name", Format = ExcelCellFormat.String },
        new() { CellAddress = "E2", Value = "Price", Format = ExcelCellFormat.Money }
      };

      _styles.TitleStyles(sheet, 2, titleCellList);

      sheet.Columns().AdjustToContents();
    }

    public async Task Workers(IXLWorksheet sheet)
    {
      int rowIndex = await _dataFiller.FillWorkers(sheet);

      _styles.MainTableStyle(sheet, "B", 3, "K", rowIndex);

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

      _styles.TitleStyles(sheet, 2, titleCellList);

      sheet.Columns().AdjustToContents();
    }

    public async Task Orders(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders)
    {
      int rowIndex = await _dataFiller.FillOrders(sheet, entityOrders);

      _styles.MainTableStyle(sheet, "B", 3, "J", rowIndex);

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

      _styles.TitleStyles(sheet, 2, titleCellList);

      sheet.Columns().AdjustToContents();
    }

    public async Task OrderProducts(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders)
    {
      int rowIndex = 1;
      foreach (var order in entityOrders)
      {
        rowIndex++;
        rowIndex = await _dataFiller.FillOrderProductsRow(sheet, order, rowIndex);
        rowIndex += 3;
      }

      sheet.Columns().AdjustToContents();
    }

    public async Task OrderAddOns(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders)
    {
      int rowIndex = 1;

      foreach (var order in entityOrders)
      {
        rowIndex++;
        rowIndex = await _dataFiller.FillOrderAddOnsRow(sheet, order, rowIndex);
        rowIndex += 3;
      }

      sheet.Columns().AdjustToContents();
    }
  }
}
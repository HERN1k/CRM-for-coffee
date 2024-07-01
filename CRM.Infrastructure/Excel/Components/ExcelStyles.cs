using ClosedXML.Excel;

using CRM.Core.Contracts.ApplicationDto;
using CRM.Core.Enums;
using CRM.Core.Interfaces.Excel;

namespace CRM.Infrastructure.Excel.Components
{
  public class ExcelStyles : IExcelStyles
  {
    public void TitleStyles(IXLWorksheet sheet, int row, List<TitleCellStylesDto> columns)
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

    public void MainTableStyle(IXLWorksheet sheet, string firstRow, int indexFirstRow, string seccondRow, int indexSeccondRow)
    {
      string rangeAddress = $"{firstRow + indexFirstRow}:{seccondRow + indexSeccondRow}";
      var table = sheet.Range(rangeAddress);
      table.Style.Border.TopBorder = XLBorderStyleValues.Thin;
      table.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
      table.Style.Border.RightBorder = XLBorderStyleValues.Thin;
      table.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
    }

    public void ItemTitleStyles(IXLWorksheet sheet, int row, int column, string name, string value)
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
  }
}
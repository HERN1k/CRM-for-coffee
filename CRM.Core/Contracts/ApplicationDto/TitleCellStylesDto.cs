using CRM.Core.Enums;

namespace CRM.Core.Contracts.ApplicationDto
{
  public class TitleCellStylesDto
  {
    public string CellAddress { get; set; } = null!;
    public string Value { get; set; } = null!;
    public ExcelCellFormat Format { get; set; }
  }
}
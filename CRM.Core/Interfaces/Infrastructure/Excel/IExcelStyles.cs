using ClosedXML.Excel;
using CRM.Core.Contracts.ApplicationDto;

namespace CRM.Core.Interfaces.Infrastructure.Excel
{
    /// <summary>
    ///   Provides methods for applying styles to Excel worksheets.
    /// </summary>
    public interface IExcelStyles
    {
        /// <summary>
        ///   Applies title styles and formats to the specified worksheet cells based on the provided column styles.
        /// </summary>
        /// <param name="sheet">The worksheet to which the styles and formats will be applied.</param>
        /// <param name="row">The row number where the title styles will be applied.</param>
        /// <param name="columns">A list of <see cref="TitleCellStylesDto"/> objects representing the columns to style and format.</param>
        void TitleStyles(IXLWorksheet sheet, int row, List<TitleCellStylesDto> columns);

        /// <summary>
        ///   Applies border styles to a specified range of cells in the worksheet to format it as a table.
        /// </summary>
        /// <param name="sheet">The worksheet to which the styles will be applied.</param>
        /// <param name="firstRow">The letter of the column for the first cell in the range (e.g., "A").</param>
        /// <param name="indexFirstRow">The row number for the first cell in the range.</param>
        /// <param name="seccondRow">The letter of the column for the last cell in the range (e.g., "D").</param>
        /// <param name="indexSeccondRow">The row number for the last cell in the range.</param>
        void MainTableStyle(IXLWorksheet sheet, string firstRow, int indexFirstRow, string seccondRow, int indexSeccondRow);

        /// <summary>
        ///   Applies title and value styles to the specified cells in the worksheet.
        /// </summary>
        /// <param name="sheet">The worksheet to which the styles will be applied.</param>
        /// <param name="row">The row number where the styles will be applied.</param>
        /// <param name="column">The column number of the cell where the title will be placed.</param>
        /// <param name="name">The title text to be displayed in the specified cell.</param>
        /// <param name="value">The value text to be displayed in the cell to the right of the title cell.</param>
        void ItemTitleStyles(IXLWorksheet sheet, int row, int column, string name, string value);
    }
}
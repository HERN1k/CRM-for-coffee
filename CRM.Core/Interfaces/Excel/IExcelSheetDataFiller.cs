using ClosedXML.Excel;

using CRM.Core.Entities;

namespace CRM.Core.Interfaces.Excel
{
  /// <summary>
  ///   Provides methods for filling Excel worksheets with various types of data.
  /// </summary>
  public interface IExcelSheetDataFiller
  {
    /// <summary>
    ///   Fills the worksheet with product categories data.
    /// </summary>
    /// <param name="sheet">The worksheet to fill with product categories data.</param>
    /// <returns>The last row index of the filled data.</returns>
    int FillProductCategories(IXLWorksheet sheet);

    /// <summary>
    ///   Asynchronously fills the worksheet with products data.
    /// </summary>
    /// <param name="sheet">The worksheet to fill with products data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the last row index of the filled data.</returns>
    Task<int> FillProducts(IXLWorksheet sheet);

    /// <summary>
    ///   Asynchronously fills the worksheet with addons data.
    /// </summary>
    /// <param name="sheet">The worksheet to fill with addons data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the last row index of the filled data.</returns>
    Task<int> FillAddOns(IXLWorksheet sheet);

    /// <summary>
    ///   Asynchronously fills the worksheet with workers data.
    /// </summary>
    /// <param name="sheet">The worksheet to fill with workers data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the last row index of the filled data.</returns>
    Task<int> FillWorkers(IXLWorksheet sheet);

    /// <summary>
    ///   Asynchronously fills the worksheet with orders data.
    /// </summary>
    /// <param name="sheet">The worksheet to fill with orders data.</param>
    /// <param name="entityOrders">The collection of order entities to use for filling the worksheet.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the last row index of the filled data.</returns>
    Task<int> FillOrders(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders);

    /// <summary>
    ///   Asynchronously fills a specific row in the worksheet with order products data.
    /// </summary>
    /// <param name="sheet">The worksheet to fill with order products data.</param>
    /// <param name="order">The order entity containing the products to use for filling the worksheet.</param>
    /// <param name="rowIndex">The starting row index to fill the data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the last row index of the filled data.</returns>
    public
    Task<int> FillOrderProductsRow(IXLWorksheet sheet, EntityOrder order, int rowIndex);

    /// <summary>
    ///   Asynchronously fills a specific row in the worksheet with order addons data.
    /// </summary>
    /// <param name="sheet">The worksheet to fill with order addons data.</param>
    /// <param name="order">The order entity containing the addons to use for filling the worksheet.</param>
    /// <param name="rowIndex">The starting row index to fill the data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the last row index of the filled data.</returns>
    Task<int> FillOrderAddOnsRow(IXLWorksheet sheet, EntityOrder order, int rowIndex);
  }
}
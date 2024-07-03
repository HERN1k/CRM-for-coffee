using ClosedXML.Excel;
using CRM.Core.Entities;

namespace CRM.Core.Interfaces.Infrastructure.Excel
{
    /// <summary>
    ///   Provides methods for filling Excel worksheets with various types of data.
    /// </summary>
    public interface IExcelFillingSheet
    {
        /// <summary>
        ///   Fills the worksheet with product categories data and applies styles.
        /// </summary>
        /// <param name="sheet">The worksheet to fill with product categories data.</param>
        void ProductCategories(IXLWorksheet sheet);

        /// <summary>
        ///   Asynchronously fills the worksheet with products data and applies styles.
        /// </summary>
        /// <param name="sheet">The worksheet to fill with products data.</param>
        Task Products(IXLWorksheet sheet);

        /// <summary>
        ///   Asynchronously fills the worksheet with addons data and applies styles.
        /// </summary>
        /// <param name="sheet">The worksheet to fill with addons data.</param>
        Task AddOns(IXLWorksheet sheet);

        /// <summary>
        ///   Asynchronously fills the worksheet with workers data and applies styles.
        /// </summary>
        /// <param name="sheet">The worksheet to fill with workers data.</param>
        Task Workers(IXLWorksheet sheet);

        /// <summary>
        ///   Asynchronously fills the worksheet with orders data and applies styles.
        /// </summary>
        /// <param name="sheet">The worksheet to fill with orders data.</param>
        /// <param name="entityOrders">The collection of order entities to use for filling the worksheet.</param>
        Task Orders(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders);

        /// <summary>
        ///   Asynchronously fills the worksheet with order products data and applies styles.
        /// </summary>
        /// <param name="sheet">The worksheet to fill with order products data.</param>
        /// <param name="entityOrders">The collection of order entities to use for filling the worksheet.</param>
        Task OrderProducts(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders);

        /// <summary>
        ///   Asynchronously fills the worksheet with order addons data and applies styles.
        /// </summary>
        /// <param name="sheet">The worksheet to fill with order addons data.</param>
        /// <param name="entityOrders">The collection of order entities to use for filling the worksheet.</param>
        Task OrderAddOns(IXLWorksheet sheet, IQueryable<EntityOrder> entityOrders);
    }
}
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.ApplicationModel.Resources;
#endregion

namespace Dt.Cells.Data
{
    public class ResourceStrings
    {
        public const string AnotherWorksheetWithTheSameNameError = "工作表集合中已经存在相同工作表名称。";

        /// <summary>
        /// Looks up a localized string similar to Array format is illegal.
        /// </summary>
        public const string ArrayFormatIsIllegal = "数组格式是非法的。";

        /// <summary>
        /// Looks up a localized string similar to Cannot change part of the merged cell.
        /// </summary>
        public const string AutoFillChangedPartOfMergedCell = "不能改变合并单元格的一部分";

        /// <summary>
        /// Looks up a localized string similar to BorderlineLayout engine only support horizontal or vertical direction at the current version.
        /// </summary>
        public const string BorderLineLayoutNotSupportDirectionError = "BorderLineLayoutNotSupportDirectionError";

        /// <summary>
        /// Looks up a localized string similar to Cannot set a member of NamedStyleCollection to null -- call RemoveAt instead.
        /// </summary>
        public const string CannotSetNullToStyleInfo = "不能设置NamedStyleCollection的成员为空， 调用RemoveAt。";

        /// <summary>
        /// Looks up a localized string similar to Invalid {0} {1} index: {2} (must be between -1 and {3}).
        /// </summary>
        public const string CheckArgumentsInvalidRowColumn = "非法 {0} {1}索引: {2} (必须介于-1 和{3}之间)。";

        /// <summary>
        /// Looks up a localized string similar to {0} scale must be between 0 and 1.
        /// </summary>
        public const string ConditionalFormatDataBarRuleScaleOutOfRangeError = "ConditionalFormatDataBarRuleScaleOutOfRangeError";

        /// <summary>
        /// Looks up a localized string similar to A theme with the same name already exists.
        /// </summary>
        public const string CouldnotAddThemeWithSameName = "同名主题名称已经存在。";

        /// <summary>
        /// Looks up a localized string similar to The operation is not supported in the corner area.
        /// </summary>
        public const string CouldnotChangeConnerHeaderCell = "角落区域不支持此操作。";

        /// <summary>
        /// Looks up a localized string similar to The sheet area cannot be found.
        /// </summary>
        public const string CouldnotFindSpecifiedSheetArea = "不能发现工作表区域。";

        /// <summary>
        /// Looks up a localized string similar to There is only one {0} viewport!.
        /// </summary>
        public const string CouldnotRemoveTheLastViewport = "只有一个 {0}视窗！";

        /// <summary>
        /// Looks up a localized string similar to Specified sheet is not in the collection.
        /// </summary>
        public const string CouldnotSetActiveSheetToNonExistingSheet = "指定的工作表不再集合内。";

        /// <summary>
        /// Looks up a localized string similar to The theme {0} cannot be found in the workbook.
        /// </summary>
        public const string CouldnotSetCurrentThemeToNonExistingTheme = "工作薄中找不到主题 {0}。";

        /// <summary>
        /// Looks up a localized string similar to Column width must be greater than 0.
        /// </summary>
        public const string CouldnotSetNegetiveColumnWidth = "列宽必须大于0。";

        /// <summary>
        /// Looks up a localized string similar to Row height must be greater than 0.
        /// </summary>
        public const string CouldnotSetNegetiveRowHeight = "行高必须大于0。";

        /// <summary>
        /// Looks up a localized string similar to The current worksheet already exists in a table with the same name.
        /// </summary>
        public const string CurrentWorksheetHasTheSameTableError = "工作表中已经存在同名表。";

        /// <summary>
        /// Looks up a localized string similar to The type of descriptor was added.
        /// </summary>
        public const string CustomNumberFormatNotSupportAddPartError = "已经添加描述类型。";

        /// <summary>
        /// Looks up a localized string similar to Does not support this operation in this verison.
        /// </summary>
        public const string DataBindingNotSupport = "此版本不支持此操作。";

        /// <summary>
        /// Looks up a localized string similar to Spread does not support data binding of the data source.
        /// </summary>
        public const string DataBindingNullConnection = "工作薄不支持数据源的数据绑定。";

        /// <summary>
        /// Looks up a localized string similar to It is already bound to a datasource, please unbind first.
        /// </summary>
        public const string DataBindingRebindError = "已经绑定到数据源，请解除绑定。";

        /// <summary>
        /// Looks up a localized string similar to Cannot set sheet when it has been bound.
        /// </summary>
        public const string DataBindingSetSheetWhenAlreadyBound = "当绑定时不能设置工作表。";

        public const string DefaultAxisTitle = "坐标轴标题";

        public const string DefaultChartTitle = "图表标题";

        public const string DefaultDataSeriesName = "系列";

        /// <summary>
        /// Looks up a localized string similar to Does not support setting the style name of the default style.
        /// </summary>
        public const string DonotAllowSetTheNameOfTheDefaultStyle = "不支持设置默认样式的样式名称。";

        /// <summary>
        /// Looks up a localized string similar to The table name cannot be empty.
        /// </summary>
        public const string EmptyTableNameError = "表名称不能为空。";

        public const string ExcelAddChartError = "添加Chart {0} 到工作表 {1}失败。";

        /// <summary>
        /// Looks up a localized string similar to Failed to update control status.
        /// </summary>
        public const string excelReaderFinish = "更新控件状态失败。";

        /// <summary>
        /// Looks up a localized string similar to Failed to set Global NamedRange {0} to control.
        /// </summary>
        public const string ExcelSetGlobalNameError = "设置全局 NamedRange {0} 失败。";

        /// <summary>
        /// Looks up a localized string similar to Failed to set Local NamedRange {0}, Sheet: {1} to control.
        /// </summary>
        public const string ExcelSetNamedCellRangeError = "设置本地 NamedRange {0}, Sheet: {1}失败。";

        /// <summary>
        /// Looks up a localized string similar to Failed to process conditional format.
        /// </summary>
        public const string ExcelWriterWriteConditionalFormatError = "处理条件格式失败。";

        /// <summary>
        /// Looks up a localized string similar to The worksheet '{0}' cannot be found.
        /// </summary>
        public const string FailedFoundWorksheetError = "找不到工作表 '{0}'。";

        /// <summary>
        /// Looks up a localized string similar to Target range should not have merged cells.
        /// </summary>
        public const string FillRangeHasMergedCell = "目标区域不能包含合并单元格。";

        /// <summary>
        /// Looks up a localized string similar to This operation requires the merged cells to be identically sized.
        /// </summary>
        public const string FillRangeHaveDifferentSize = "此操作要求合并后的单元格大小相同。";

        /// <summary>
        /// Looks up a localized string similar to The filter should be added in a specified sheet.
        /// </summary>
        public const string FilterSheetNullError = "筛选应该添加到指定工作表。";

        /// <summary>
        /// Looks up a localized string similar to char is illegal.
        /// </summary>
        public const string FormatIllegalCharError = "FormatIllegalCharError";

        /// <summary>
        /// Looks up a localized string similar to format is illegal.
        /// </summary>
        public const string FormatIllegalFormatError = "FormatIllegalFormatError";

        /// <summary>
        /// Looks up a localized string similar to string is illegal.
        /// </summary>
        public const string FormatIllegalStringError = "FormatIllegalStringError";

        /// <summary>
        /// Looks up a localized string similar to The type of descriptor was added.
        /// </summary>
        public const string FormatterCustomNumberFormatAddPartError = "FormatterCustomNumberFormatAddPartError";

        /// <summary>
        /// Looks up a localized string similar to color is not a valid color name.
        /// </summary>
        public const string FormatterIllegalColorNameError = "FormatterIllegalColorNameError";

        /// <summary>
        /// Looks up a localized string similar to value is illegal.
        /// </summary>
        public const string FormatterIllegalValueError = "FormatterIllegalValueError";

        /// <summary>
        /// Looks up a localized string similar to token is illegal.
        /// </summary>
        public const string FormatterIllegaTokenError = "FormatterIllegaTokenError";

        /// <summary>
        /// Looks up a localized string similar to The color is not a valid color name.
        /// </summary>
        public const string FormatterInvalidColorNameError = "颜色名称不合法。";

        /// <summary>
        /// Looks up a localized string similar to the '\' can't be evaluated.
        /// </summary>
        public const string FormatterTransformEscapeSymbolError = "FormatterTransformEscapeSymbolError";

        /// <summary>
        /// Looks up a localized string similar to You cannot change part of an array.
        /// </summary>
        public const string FormulaChangePartOfArrayFormulaError = "FormulaChangePartOfArrayFormulaError";

        /// <summary>
        /// Looks up a localized string similar to GrapeCity.Windows.SpreadSheet.Data.
        /// </summary>
        public const string GrapeCityWindowsSpreadSheetDataAssemblyName = "GrapeCityWindowsSpreadSheetDataAssemblyName";

        /// <summary>
        /// Looks up a localized string similar to Does not support the type of sheet area.
        /// </summary>
        public const string HtmlGetCellInvalidSheetAreaError = "不支持此类工作表区域。";

        /// <summary>
        /// Looks up a localized string similar to The character is illegal.
        /// </summary>
        public const string IllegalCharError = "字符非法。";

        /// <summary>
        /// Looks up a localized string similar to The format is illegal.
        /// </summary>
        public const string IllegalFormatError = "格式非法。";

        /// <summary>
        /// Looks up a localized string similar to The token is illegal.
        /// </summary>
        public const string IllegalTokenError = "令牌非法。";

        /// <summary>
        /// Looks up a localized string similar to The tables cannot be intersected.
        /// </summary>
        public const string IntersectTableError = "表不能交叉。";

        /// <summary>
        /// Looks up a localized string similar to Invalid row index: {0} (must be between -1 and {1}).
        /// </summary>
        public const string InvaildRowIndexWithAllowedRangeBehind = "行索引非法：{0} （必须在-1 和{1}之间）。";

        /// <summary>
        /// Looks up a localized string similar to Invalid column count: {0} (must be between -1 and {1}.
        /// </summary>
        public const string InvalidColumnCount0MustBeBetween1And1 = "列数目非法：{0} （必须在-1 和{1}之间）。";

        /// <summary>
        /// Looks up a localized string similar to Invalid column count: {0} (must be between -1 and {1}).
        /// </summary>
        public const string InvalidColumnCountWithAllowedColumnCountBehind = "列数目非法：{0} （必须在-1 和{1}之间）。";

        /// <summary>
        /// Looks up a localized string similar to Invalid column index: .
        /// </summary>
        public const string InvalidColumnIndex = "列索引非法：";

        /// <summary>
        /// Looks up a localized string similar to Invalid column index:{0}(must be between -1 and {1}).
        /// </summary>
        public const string InvalidColumnIndex0MustBeBetween1And1 = "列数目非法：{0} （必须在-1 和{1}之间）。";

        /// <summary>
        /// Looks up a localized string similar to Invalid column index: {0} (must be between -1 and {1}).
        /// </summary>
        public const string InvalidColumnIndexWithAllowedRangeBehind = "列数目非法：{0} （必须在-1 和{1}之间）。";

        /// <summary>
        /// Looks up a localized string similar to Width must be between -1 and 9999999.
        /// </summary>
        public const string InvalidColumnWidth = "宽度必须介于-1 和9999999。";

        /// <summary>
        /// Looks up a localized string similar to Invalid {0} count: {1} (must be between 1 and {2}.
        /// </summary>
        public const string InvalidRowColumntCount2 = "非法{0} 数目: {1} (必须介于1和{2}之间).";

        /// <summary>
        /// Looks up a localized string similar to Invalid row count: {0} (must be between -1 and {1}).
        /// </summary>
        public const string InvalidRowCountWithAllowedRowCountBehind = "行数目非法：{0} （必须在-1 和{1}之间）。";

        /// <summary>
        /// Looks up a localized string similar to Invalid row index: {0} (must be between -1 and {1}).
        /// </summary>
        public const string InvalidRowIndex0MustBeBetween1And1 = "行数目非法：{0} （必须在-1 和{1}之间）。";

        /// <summary>
        /// Looks up a localized string similar to Invalid sheet area: {0}.
        /// </summary>
        public const string InvalidSheetArea = "工作表区域非法：{0}";

        /// <summary>
        /// Looks up a localized string similar to Invalid column index: {0} (must be between 0 and {1}.
        /// </summary>
        public const string InvalidTableLocationColumnIndex = "列索引非法：{0} （必须在0 和{1}之间）";

        /// <summary>
        /// Looks up a localized string similar to Invalid row index: {0} (must be between 0 and {1}).
        /// </summary>
        public const string InvalidTableLocationRowIndex = "行索引非法：{0} （必须在0 和{1}之间）";

        /// <summary>
        /// Looks up a localized string similar to The theme color is invalid.
        /// </summary>
        public const string InvalidThemeColor = "主题颜色非法。";

        /// <summary>
        /// Looks up a localized string similar to The collection is read-only.
        /// </summary>
        public const string ModifyReadonlyCollectionError = "集合是只读的。";

        /// <summary>
        /// Looks up a localized string similar to The name that you set is not valid.
        /// </summary>
        public const string NamedStyleInfoInvalidNameError = "名称是非法的。";

        /// <summary>
        /// Looks up a localized string similar to item.Name cannot be null.
        /// </summary>
        public const string NamedStyleInfoNameNullError = "item.Name 不能为空";

        /// <summary>
        /// Looks up a localized string similar to The reference is not valid. References for titles, values, or sizes must be a single cell, row, or column.
        /// </summary>
        public const string NeedSingleCellRowColumn = "引用无效。标题引用，数值或者是大小必须是单个单元格，行或者列。";

        /// <summary>
        /// Looks up a localized string similar to The '\' cannot be evaluated.
        /// </summary>
        public const string NumberFormatValidEscapeError = "不能评估'\'。";

        /// <summary>
        /// Looks up a localized string similar to Only works for numbers.
        /// </summary>
        public const string NumberSourceOnlyWorkedWithNumbers = "只对数字起作用。";

        /// <summary>
        /// Looks up a localized string similar to Transformation is not invertible.
        /// </summary>
        public const string PdfInvertError = "转换是不可逆的。";

        /// <summary>
        /// Looks up a localized string similar to A value between 1 and 5 is valid.
        /// </summary>
        public const string PdfSetNumberOfCopiesError = "合法的数值是介于1到5之间。";

        /// <summary>
        /// Looks up a localized string similar to Index is missing.
        /// </summary>
        public const string RangeGroupSerializeError = "索引丢失。";

        /// <summary>
        /// Looks up a localized string similar to The range should not have a merged cell.
        /// </summary>
        public const string RangeShouldNotHaveMergedCell = "区域不能包含合并单元格。";

        /// <summary>
        /// Looks up a localized string similar to {0} end index must be less than or equal to the start index.
        /// </summary>
        public const string ReportingGcSheetSectionEndIndexError = "{ 0 }结束索引必须小于或等于起始索引。";

        /// <summary>
        /// Looks up a localized string similar to The repeat {0} end index must be less than or equal to the start index.
        /// </summary>
        public const string ReportingGcSheetSectionRepeatEndIndexError = "重复{ 0 }结束索引必须小于或等于起始索引。";

        /// <summary>
        /// Looks up a localized string similar to Margins do not fit the page size.
        /// </summary>
        public const string ReportingMarginError = "页边距不适合页面尺寸";

        /// <summary>
        /// Looks up a localized string similar to Invalid page range.
        /// </summary>
        public const string ReportingPageRangeError = "非法页面区域。";

        /// <summary>
        /// Looks up a localized string similar to The height must be greater than 0.
        /// </summary>
        public const string ReportingPaperSizeHightError = "高度必须大于0。";

        /// <summary>
        /// Looks up a localized string similar to The width must be greater than 0.
        /// </summary>
        public const string ReportingPaperSizeWidthError = "宽度必须大于0。";

        /// <summary>
        /// Looks up a localized string similar to Value must be greater than or equal to -1.
        /// </summary>
        public const string ReportingPrintInfoRepeatColumnError = "值必须大于等于-1。";

        /// <summary>
        /// Looks up a localized string similar to Invalid row index: {0}  or {1}.
        /// </summary>
        public const string RowIndexerOutOfRangeError = "行索引非法：{0}  或 {1}";

        /// <summary>
        /// Looks up a localized string similar to There should be at least one worksheet when saving to Excel.
        /// </summary>
        public const string SaveEmptyWorkbookToExcelError = "工作薄至少应该包含一个工作表。";

        /// <summary>
        /// Looks up a localized string similar to The specified startSheetIndex or endSheetIndex is out of range.
        /// </summary>
        public const string SearchArgumentOutOfRange = "指定的工作表开始索引和终止索引越界。";

        /// <summary>
        /// Looks up a localized string similar to Row, Column, RowCount, or ColumnCount is missing.
        /// </summary>
        public const string SerializationError = "Row, Column, RowCount, 和 ColumnCount丢失。";

        /// <summary>
        /// Looks up a localized string similar to Cannot create an instance for the type of array.
        /// </summary>
        public const string SerializeCannotCreateTypeOfArray = "不能为数组类型创建实例。";

        /// <summary>
        /// Looks up a localized string similar to The index is missing.
        /// </summary>
        public const string SerializeDeserializerArrayError = "索引丢失。";

        /// <summary>
        /// Looks up a localized string similar to The type cannot be found.
        /// </summary>
        public const string SerializeDeserializerCellError = "找不到此类型。";

        /// <summary>
        /// Looks up a localized string similar to The current version does not support a serialized image.
        /// </summary>
        public const string SerializeImageError = "当前版本不支持序列化图片。";

        /// <summary>
        /// Looks up a localized string similar to The matrix data is invalid.
        /// </summary>
        public const string SerializerDeserializeMatrixError = "数据矩阵是非法的。";

        /// <summary>
        /// Looks up a localized string similar to Array data format is illegal.
        /// </summary>
        public const string SerializerDeserializerIllegalArrayFormat = "数组数据格式是非法的。";

        /// <summary>
        /// Looks up a localized string similar to Does not support this case.
        /// </summary>
        public const string SerializerInvalidCastError = "不支持此用例。";

        /// <summary>
        /// Looks up a localized string similar to The type '{0}' cannot be formatted.
        /// </summary>
        public const string SerializerNotSupportError = "类型 '{0}' 不能格式化。";

        /// <summary>
        /// Looks up a localized string similar to The type does not match.
        /// </summary>
        public const string SerializerParseTypeNotMatchError = "类型不匹配。";

        /// <summary>
        /// Looks up a localized string similar to Spread does not support setting the built-in borders (GridLine or NoBorder).
        /// </summary>
        public const string SetBuiltInBorderError = "工作薄不支持设置内置的边框（GridLine 或者NoBorder）。";

        /// <summary>
        /// Looks up a localized string similar to Invalid object type specified: {0} (must be StyleInfo).
        /// </summary>
        public const string SetOtherTypeToStyleInfoCollectionError = "指定的对象{0}类型不合法：(必须是StyleInfo)";

        /// <summary>
        /// Looks up a localized string similar to Setting the span to area {0} is not supported.
        /// </summary>
        public const string SetSpanToNotSupportAreaError = "不支持区域 {0}的合并。";

        /// <summary>
        /// Looks up a localized string similar to Could not set the tag to a nonexistent cell.
        /// </summary>
        public const string SetTagToNonExistingCell = "不能为一个不存在的单元格设置标签。";

        /// <summary>
        /// Looks up a localized string similar to Sheet name can not be null or empty.
        /// </summary>
        public const string SheetNameCannotBeNullOrEmpty = "工作表名称不能为空。";

        /// <summary>
        /// Looks up a localized string similar to The object must implement the IComparable interface.
        /// </summary>
        public const string SortCompareError = "对象必须实现 Icomparable的接口。";

        /// <summary>
        /// Looks up a localized string similar to This operation will cause overlapping spans.
        /// </summary>
        public const string SpanModelOverlappingError = "此操作将导致重叠的合并。";

        /// <summary>
        /// Looks up a localized string similar to "Value must be greater than or equal to -1.".
        /// </summary>
        public const string SpreadReporting_Msg = "值必须大于等于-1。";

        /// <summary>
        /// Looks up a localized string similar to The string is illegal.
        /// </summary>
        public const string StringIsIllegal = "字符串是非法的。";

        /// <summary>
        /// Looks up a localized string similar to and.
        /// </summary>
        public const string StyleInfoAnd = "并";

        /// <summary>
        /// Looks up a localized string similar to This collection is read-only.
        /// </summary>
        public const string StyleInfoChangeReadOnlyCollectionError = "集合是只读的。";

        /// <summary>
        /// Looks up a localized string similar to Specified array has insufficient length (array length is {0}, length required is at least {1}).
        /// </summary>
        public const string StyleInfoCopyToArrayLengthError = "指定数组长度不足(数组长度为{ 0 },长度要求是至少{ 1 })。";

        /// <summary>
        /// Looks up a localized string similar to Specified array must have rank of 1 (specified array has rank='{0}').
        /// </summary>
        public const string StyleInfoCopyToArrayRankGreaterThanOneError = "数组秩必须是1（指定的秩为'{0}'）。";

        /// <summary>
        /// Looks up a localized string similar to Specified array argument is null (Nothing).
        /// </summary>
        public const string StyleInfoCopyToDestionationNullError = "数组参数是空。";

        /// <summary>
        /// Looks up a localized string similar to are expected.
        /// </summary>
        public const string StyleInfoexpected = "期望的。";

        /// <summary>
        /// Looks up a localized string similar to Specified index is invalid: {0} (must be greater than 0).
        /// </summary>
        public const string StyleInfoOperationIndexOutOfRangeError = "指定的{0} 非法：（必须介于0 到 {1}1之间）。";

        /// <summary>
        /// Looks up a localized string similar to Invalid index specified: {0} (should be between 0 and {1}).
        /// </summary>
        public const string StyleInfoOperationIndexOutOfRangeWithAllowedRangeBehind = "指定的{0} 非法：（必须介于0 到 {1}1之间）。";

        /// <summary>
        /// Looks up a localized string similar to TextIndent only accepts non-negative values.
        /// </summary>
        public const string StyleInfoTextIndentMustBePositiveValue = "TextIndent只接受正数。";

        /// <summary>
        /// Looks up a localized string similar to The table {0} already exists in worksheet {1}.
        /// </summary>
        public const string TableAlreayExistInOtherWorksheet = "表 {0} 已经在工作表 {1}中存在。";

        /// <summary>
        /// Looks up a localized string similar to The table '{0}' already exists in the sheet.
        /// </summary>
        public const string TableCollectionAddTableError = "表 '{0}' 已经在工作表中存在。";

        /// <summary>
        /// Looks up a localized string similar to The column '{0}' is out of the sheet range.
        /// </summary>
        public const string TableColumnDestinationOutOfRange = "列'{0}'超出工作表范围。";

        /// <summary>
        /// Looks up a localized string similar to dataSource.
        /// </summary>
        public const string TableDataSourceCannotBeNull = "数据源。";

        /// <summary>
        /// Looks up a localized string similar to The table cannot be moved out of the sheet.
        /// </summary>
        public const string TableMoveDestinationOutOfRange = "表格不能移出工作表。";

        /// <summary>
        /// Looks up a localized string similar to The table cannot be found.
        /// </summary>
        public const string TableNotFoundError = "表格找不到。";

        /// <summary>
        /// Looks up a localized string similar to Does not support the type of data source.
        /// </summary>
        public const string TableNotSupportDataSouceError = "不支持的数据源类型。";

        /// <summary>
        /// Looks up a localized string similar to The table must be added in a sheet.
        /// </summary>
        public const string TableOwnerNullError = "表必须添加到工作表中。";

        /// <summary>
        /// Looks up a localized string similar to The table must have at least a {0}.
        /// </summary>
        public const string TableResizeOutOfRangeError = "表必须包含一个{0}。";

        /// <summary>
        /// Looks up a localized string similar to Setting a range in the table is not supported, please resize the table instead.
        /// </summary>
        public const string TableResizeRangeError = "不支持设定表范围， 请调整表大小。";

        /// <summary>
        /// Looks up a localized string similar to The row '{0}' is out of the sheet range.
        /// </summary>
        public const string TableRowDestinationOutOfRangeError = "行{0} 超出工作表范围。";

        /// <summary>
        /// Looks up a localized string similar to The row count {0} is out of the sheet range.
        /// </summary>
        public const string TableShowFooterError = "行数目{0}超出工作表区域。";

        /// <summary>
        /// Looks up a localized string similar to There are no more rows for the header.
        /// </summary>
        public const string TableShowHeaderError = "没有更多的标题。";

        /// <summary>
        /// Looks up a localized string similar to The style '{0}' already exists in the styles.
        /// </summary>
        public const string TableStyleAddCustomStyleError = "样式 '{0}' 已经存在。";

        /// <summary>
        /// Looks up a localized string similar to The value is illegal.
        /// </summary>
        public const string ValueIsIllegal = "值是非法的。";

        /// <summary>
        /// Looks up a localized string similar to The ColumnCount must &gt;= 0.
        /// </summary>
        public const string WorksheetColumnCountMsg = "列数目必须大于0。";

        /// <summary>
        /// Looks up a localized string similar to Internal Error: If you want to support additional argument types for the CreateExpression method, please add it here.
        /// </summary>
        public const string WorksheetCreateExpressionError = "内部错误:如果您想为CreateExpression方法支持额外的参数类型,请将它添加在这里。";

        /// <summary>
        /// Looks up a localized string similar to The selection model cannot be null.
        /// </summary>
        public const string WorksheetEmptySelection = "选择模块不能为空。";

        /// <summary>
        /// Looks up a localized string similar to Value was out of the range: 0 to {0}.
        /// </summary>
        public const string WorksheetInvalidRowHeaderColumnCount = "值超出范围：0到{0}。";

        /// <summary>
        /// Looks up a localized string similar to The RowCount must &gt;= 0.
        /// </summary>
        public const string WorksheetRowCountMsg = "行数必须大于0。";

        /// <summary>
        /// Looks up a localized string similar to ZoomFactor must be between 0.1 and 4.
        /// </summary>
        public const string ZoomFactorOutOfRange = "缩放比例必须介于0.1和4之间。";

        public const string addFloatingObj = "添加浮动对象";

        /// <summary>
        /// Looks up a localized string similar to Cancel.
        /// </summary>
        public const string Cancel = "取消";

        public const string ChangeChartType = "改变图表类型";

        /// <summary>
        /// Looks up a localized string similar to Width: {0} pixels.
        /// </summary>
        public const string ColumnResize = "宽度: {0} 像素";

        public const string copyFloatingObj = "拷贝浮动对象";

        public const string deleteFloatingObj = "清除浮动对象";

        public const string dragFloatingObj = "复制浮动对象";

        public const string Filter_Blanks = "（空白）";

        /// <summary>
        /// Looks up a localized string similar to (Select All).
        /// </summary>
        public const string Filter_SelectAll = "（全选）";

        /// <summary>
        /// Looks up a localized string similar to (FormulaBar_FunctionInformation).
        /// </summary>
        public const string FormulaBar_FunctionInformation = "FormulaBar_FunctionInformation.zh_CN.xml";

        /// <summary>
        /// Looks up a localized string similar to The file  {0} cannot be found..
        /// </summary>
        public const string gcSpreadExcelOpenExcelFileNotFound = "找不到文件{0}。";

        /// <summary>
        /// Looks up a localized string similar to The horizontal position is invalid..
        /// </summary>
        public const string gcSpreadInvalidHorizontalPosition = "水平位置不合法。";

        /// <summary>
        /// Looks up a localized string similar to The vertical position is invalid..
        /// </summary>
        public const string gcSpreadInvalidVerticalPosition = "垂直位置不合法。";

        /// <summary>
        /// Looks up a localized string similar to The filename cannot be empty..
        /// </summary>
        public const string gcSpreadSaveXMLInvalidFileName = "文件名称不能为空。";

        /// <summary>
        /// Looks up a localized string similar to /GrapeCity.Silverlight.SpreadSheet.UI.
        /// </summary>
        public const string GrapeCitySilverlightSpreadSheetUIAssemblyName = "/GrapeCity.Silverlight.SpreadSheet.UI";

        /// <summary>
        /// Looks up a localized string similar to /GrapeCity.WPF.SpreadSheet.UI.
        /// </summary>
        public const string GrapeCityWPFSpreadSheetUIAssemblyName = "/GrapeCity.WPF.SpreadSheet.UI";

        /// <summary>
        /// Looks up a localized string similar to Column: {0}.
        /// </summary>
        public const string HorizentalScroll = "列：{0}";

        public const string moveFloatingObj = "移动浮动对象";

        /// <summary>
        /// Looks up a localized string similar to borderIndex error..
        /// </summary>
        public const string NotSupportExceptionBorderIndexError = "边框索引错误。";

        public const string OK = "确定";

        public const string resizeFloatingObj = "改变浮动对象大小";

        /// <summary>
        /// Looks up a localized string similar to Height: {0} pixels.
        /// </summary>
        public const string RowResize = "高度：{0} 像素";

        /// <summary>
        /// Looks up a localized string similar to range.
        /// </summary>
        public const string SheetViewClipboardArgumentException = "范围";

        /// <summary>
        /// Looks up a localized string similar to Cannot change part of an array..
        /// </summary>
        public const string SheetViewDragDropChangePartOChangePartOfAnArray = "不能更改数组的某一部分。";

        /// <summary>
        /// Looks up a localized string similar to Cannot change part of a merged cell..
        /// </summary>
        public const string SheetViewDragDropChangePartOfMergedCell = "不能更改合并单元格的某一部分。";

        /// <summary>
        /// Looks up a localized string similar to Cannot complete operation: You are attempting to change a portion of a table row or column in a way that is not allowed..
        /// </summary>
        public const string SheetViewDragDropChangePartOfTable = "无法完成操作:修改表的一部分行和列是不允许的。";

        /// <summary>
        /// Looks up a localized string similar to The cell you are trying to change is protected and therefore read-only..
        /// </summary>
        public const string SheetViewDragDropChangeProtectCell = "您想修改的单元格是受保护的，因此是只读的。";

        /// <summary>
        /// Looks up a localized string similar to The column you are trying to change is protected and therefore read-only..
        /// </summary>
        public const string SheetViewDragDropChangeProtectColumn = "您想修改的列是受保护的，因此是只读的。";

        /// <summary>
        /// Looks up a localized string similar to The row you are trying to change is protected and therefore read-only..
        /// </summary>
        public const string SheetViewDragDropChangeProtectRow = "您想修改的行是受保护的，因此是只读的。";

        /// <summary>
        /// Looks up a localized string similar to This operation is not allowed. The operation is attempting to shift cells in a table on your worksheet..
        /// </summary>
        public const string SheetViewDragDropShiftTableCell = "操作不合法。此操作视图转移工作表中的一个表的单元格。";

        /// <summary>
        /// Looks up a localized string similar to Cannot fill a range that contains a merged cell..
        /// </summary>
        public const string SheetViewDragFillChangePartOfMergeCell = "不能填充带合并单元格的区域。";

        /// <summary>
        /// Looks up a localized string similar to The cells you are trying to fill are protected and therefore read-only..
        /// </summary>
        public const string SheetViewDragFillChangeProtectCell = "您想填充的单元格是受保护的，因此是只读的。";

        public const string SheetViewDragFillInvalidOperation = "自动填充失败，如果填充的单元格中含有数组，不能更改数组的某一部分。";

        /// <summary>
        /// Looks up a localized string similar to Cannot change part of a merged cell..
        /// </summary>
        public const string SheetViewPasteChangeMergeCell = "不能修改合并单元格的一部分。";

        /// <summary>
        /// Looks up a localized string similar to Cannot change part of an array..
        /// </summary>
        public const string SheetViewPasteChangePartOfArrayFormula = "不能修改数组的一部分。";

        /// <summary>
        /// Looks up a localized string similar to The cell you are trying to change is protected and therefore read-only..
        /// </summary>
        public const string SheetViewPasteDestinationSheetCellsAreLocked = "您想修改的单元格是受保护的，因此是只读的。";

        /// <summary>
        /// Looks up a localized string similar to Source sheet's cells are locked..
        /// </summary>
        public const string SheetViewPasteSouceSheetCellsAreLocked = "源工作表单元格被锁定。";

        /// <summary>
        /// Looks up a localized string similar to The copy and paste areas are not the same size..
        /// </summary>
        public const string SheetViewTheCopyAreaAndPasteAreaAreNotTheSameSize = "复制和粘贴区域大小不同。";

        /// <summary>
        /// Looks up a localized string similar to Cannot change part of an array..
        /// </summary>
        public const string SortCommandInvalidOperation = "不能改变数组的一部分。";

        /// <summary>
        /// Looks up a localized string similar to Sort Ascend.
        /// </summary>
        public const string SortDropdownItemSortAscend = "升序";

        /// <summary>
        /// Looks up a localized string similar to Sort Descend.
        /// </summary>
        public const string SortDropdownItemSortDescend = "降序";

        /// <summary>
        /// Looks up a localized string similar to That command cannot be used on multiple selections..
        /// </summary>
        public const string spreadActionCopyMultiplySelection = "不能对多重选定区域使用此命令。";

        /// <summary>
        /// Looks up a localized string similar to The command you chose cannot be performed with multiple selections.
        /// Select a single range and click the command again..
        /// </summary>
        public const string spreadActionCutMultipleSelections = "不能对多重选定区域使用此命令。选择一片区域，再次单击命令。";

        /// <summary>
        /// Looks up a localized string similar to The pasted area should have the same size as the copy or cut area..
        /// </summary>
        public const string spreadActionPasteSizeDifferent = "粘贴区域大小必须和复制或者剪贴区域大小相同。";

        /// <summary>
        /// Looks up a localized string similar to New....
        /// </summary>
        public const string TabStrip_NewSheet = "新建…";

        /// <summary>
        /// Looks up a localized string similar to Copy Cells.
        /// </summary>
        public const string UIFill_CopyCells = "复制单元格";

        /// <summary>
        /// Looks up a localized string similar to Fill Formatting Only.
        /// </summary>
        public const string UIFill_FillFormattingOnly = "仅填充格式";

        /// <summary>
        /// Looks up a localized string similar to Fill Series.
        /// </summary>
        public const string UIFill_FillSeries = "填充序列";

        /// <summary>
        /// Looks up a localized string similar to Fill Without Formatting.
        /// </summary>
        public const string UIFill_FillWithOutFormatting = "不带格式填充";

        /// <summary>
        /// Looks up a localized string similar to None.
        /// </summary>
        public const string UIFill_None = "无";

        /// <summary>
        /// Looks up a localized string similar to Action failed..
        /// </summary>
        public const string undoActionActionFailed = "操作失败";

        /// <summary>
        /// Looks up a localized string similar to Row: {0}.
        /// </summary>
        public const string undoActionArrayFormula = "设置数组公式";

        /// <summary>
        /// Looks up a localized string similar to Auto Fill.
        /// </summary>
        public const string undoActionAutoFill = "自动填充";

        /// <summary>
        /// Looks up a localized string similar to The formula '{0}' cannot be applied..
        /// </summary>
        public const string undoActionCannotApplyFormula = "公式 '{0}' 不能应用。";

        /// <summary>
        /// Looks up a localized string similar to The value cannot be applied..
        /// </summary>
        public const string undoActionCannotApplyValue = "值不能应用。";

        /// <summary>
        /// Looks up a localized string similar to This validation flag is not supported..
        /// </summary>
        public const string undoActionCellEditInvalidValidationFlag = "不支持此类校验标记";

        /// <summary>
        /// Looks up a localized string similar to Clear.
        /// </summary>
        public const string undoActionClear = "清除";

        /// <summary>
        /// Looks up a localized string similar to Clipboard Paste.
        /// </summary>
        public const string undoActionClipboardPaste = "剪贴板粘贴";

        /// <summary>
        /// Looks up a localized string similar to Column AutoFit.
        /// </summary>
        public const string undoActionColumnAutoFit = "列自适应";

        /// <summary>
        /// Looks up a localized string similar to Column Group.
        /// </summary>
        public const string undoActionColumnGroup = "列分组";

        /// <summary>
        /// Looks up a localized string similar to Column Group Expand.
        /// </summary>
        public const string undoActionColumnGroupExpand = "展开列分组";

        /// <summary>
        /// Looks up a localized string similar to Column Group Header Expand.
        /// </summary>
        public const string undoActionColumnGroupHeaderExpand = "展开列分组标题";

        /// <summary>
        /// Looks up a localized string similar to Column Resize.
        /// </summary>
        public const string undoActionColumnResize = "调整列宽度";

        /// <summary>
        /// Looks up a localized string similar to Column Ungroup.
        /// </summary>
        public const string undoActionColumnUngroup = "列分组";

        /// <summary>
        /// Looks up a localized string similar to Drag Drop.
        /// </summary>
        public const string undoActionDragDrop = "拖拽";

        /// <summary>
        /// Looks up a localized string similar to Editing Cell.
        /// </summary>
        public const string undoActionEditingCell = "编辑单元格";

        /// <summary>
        /// Looks up a localized string similar to Target ranges cannot be null or empty..
        /// </summary>
        public const string undoActionPasteTargetEmptyError = "目标区域不能为空。";

        /// <summary>
        /// Looks up a localized string similar to Row AutoFit.
        /// </summary>
        public const string undoActionRowAutoFit = "行自适应";

        /// <summary>
        /// Looks up a localized string similar to Row Group.
        /// </summary>
        public const string undoActionRowGroup = "行分组";

        /// <summary>
        /// Looks up a localized string similar to Row Group Expand.
        /// </summary>
        public const string undoActionRowGroupExpand = "展开行分组";

        /// <summary>
        /// Looks up a localized string similar to Row Group Header Expand.
        /// </summary>
        public const string undoActionRowGroupHeaderExpand = "展开行分组标题";

        /// <summary>
        /// Looks up a localized string similar to Row Resize.
        /// </summary>
        public const string undoActionRowResize = "调整行高度";

        /// <summary>
        /// Looks up a localized string similar to Row Ungroup.
        /// </summary>
        public const string undoActionRowUngroup = "取消行分组";

        /// <summary>
        /// Looks up a localized string similar to Sheet Rename.
        /// </summary>
        public const string undoActionSheetRename = "工作表重命名";

        /// <summary>
        /// Looks up a localized string similar to Typing '{0}'  in {1}.
        /// </summary>
        public const string undoActionTypingInCell = "在 {1}键入'{0}'";

        /// <summary>
        /// Looks up a localized string similar to Zoom.
        /// </summary>
        public const string undoActionZoom = "缩放";

        /// <summary>
        /// Looks up a localized string similar to Row: {0}.
        /// </summary>
        public const string VerticalScroll = "行：{0}";
    }
}


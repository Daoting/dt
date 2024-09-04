#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal static class BorderSettingImp
    {
        static int GetColumnCount(Worksheet worksheet, SheetArea sheetArea)
        {
            if ((sheetArea == SheetArea.Cells) || (sheetArea == SheetArea.ColumnHeader))
            {
                return worksheet.ColumnCount;
            }
            if (sheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader))
            {
                return worksheet.RowHeader.ColumnCount;
            }
            return 0;
        }

        static int GetRowCount(Worksheet worksheet, SheetArea sheetArea)
        {
            if ((sheetArea == SheetArea.Cells) || (sheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader)))
            {
                return worksheet.RowCount;
            }
            if (sheetArea == SheetArea.ColumnHeader)
            {
                return worksheet.ColumnHeader.RowCount;
            }
            return 0;
        }

        static void SetAxisBorder(Worksheet workSheet, SheetArea sheetArea, int index, int count, bool[] flags, BorderLine[] lines, bool isRowDirection)
        {
            for (int i = 0; i < count; i++)
            {
                int row = isRowDirection ? (index + i) : -1;
                int column = isRowDirection ? -1 : (index + i);
                StyleInfo style = workSheet.GetStyleInfo(row, column, sheetArea);
                if (style == null)
                {
                    style = new StyleInfo();
                }
                UpdateStyle(style, flags, lines);
                workSheet.SetStyleInfo(row, column, sheetArea, style);
            }
        }

        static void SetCellBorder(Worksheet workSheet, SheetArea sheetArea, int row, int rowCount, int column, int columnCount, bool[] flags, BorderLine[] lines, bool isinside, bool clear = false)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (isinside)
                    {
                        if (!clear)
                        {
                            if (i == (rowCount - 1))
                            {
                                flags[3] = false;
                            }
                            if (j == (columnCount - 1))
                            {
                                flags[2] = false;
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                flags[1] = false;
                            }
                            if (j == 0)
                            {
                                flags[0] = false;
                            }
                        }
                    }
                    StyleInfo style = workSheet.GetStyleInfo(i + row, j + column, sheetArea);
                    if (style == null)
                    {
                        style = new StyleInfo();
                    }
                    UpdateStyle(style, flags, lines);
                    workSheet.SetStyleInfo(i + row, j + column, sheetArea, style);
                    if (isinside)
                    {
                        if (!clear)
                        {
                            if (lines[3] != null)
                            {
                                flags[3] = true;
                            }
                            if (lines[2] != null)
                            {
                                flags[2] = true;
                            }
                        }
                        else
                        {
                            if (lines[1] == null)
                            {
                                flags[1] = true;
                            }
                            if (lines[0] == null)
                            {
                                flags[0] = true;
                            }
                        }
                    }
                }
            }
        }

        public static void SetCellsBorder(CellRange cellRange, SheetArea sheetArea, Worksheet workSheet, SetBorderOptions option, BorderLine borderLine)
        {
            if (option.HasFlag(SetBorderOptions.All))
            {
                if (cellRange.Column > 0)
                {
                    bool[] flags = new bool[4];
                    flags[2] = true;
                    BorderLine[] lines = new BorderLine[4];
                    SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, cellRange.Column - 1, 1, flags, lines, false, false);
                }
                if (cellRange.Row > 0)
                {
                    bool[] flagArray2 = new bool[4];
                    flagArray2[3] = true;
                    BorderLine[] lineArray2 = new BorderLine[4];
                    SetCellBorder(workSheet, sheetArea, cellRange.Row - 1, 1, cellRange.Column, cellRange.ColumnCount, flagArray2, lineArray2, false, false);
                }
                if ((cellRange.Column + cellRange.ColumnCount) < GetColumnCount(workSheet, sheetArea))
                {
                    bool[] flagArray3 = new bool[4];
                    flagArray3[0] = true;
                    BorderLine[] lineArray3 = new BorderLine[4];
                    SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, cellRange.Column + cellRange.ColumnCount, 1, flagArray3, lineArray3, false, false);
                }
                if ((cellRange.Row + cellRange.RowCount) < GetRowCount(workSheet, sheetArea))
                {
                    bool[] flagArray4 = new bool[4];
                    flagArray4[1] = true;
                    BorderLine[] lineArray4 = new BorderLine[4];
                    SetCellBorder(workSheet, sheetArea, cellRange.Row + cellRange.RowCount, 1, cellRange.Column, cellRange.ColumnCount, flagArray4, lineArray4, false, false);
                }
                SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, cellRange.Column, cellRange.ColumnCount, new bool[] { true, true, true, true }, new BorderLine[] { borderLine, borderLine, borderLine, borderLine }, false, false);
            }
            else
            {
                if (option.HasFlag(SetBorderOptions.Left))
                {
                    if (cellRange.Column > 0)
                    {
                        bool[] flagArray5 = new bool[4];
                        flagArray5[2] = true;
                        BorderLine[] lineArray6 = new BorderLine[4];
                        SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, cellRange.Column - 1, 1, flagArray5, lineArray6, false, false);
                    }
                    bool[] flagArray6 = new bool[4];
                    flagArray6[0] = true;
                    BorderLine[] lineArray7 = new BorderLine[4];
                    lineArray7[0] = borderLine;
                    SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, cellRange.Column, 1, flagArray6, lineArray7, false, false);
                }
                if (option.HasFlag(SetBorderOptions.Top))
                {
                    if (cellRange.Row > 0)
                    {
                        bool[] flagArray7 = new bool[4];
                        flagArray7[3] = true;
                        BorderLine[] lineArray8 = new BorderLine[4];
                        SetCellBorder(workSheet, sheetArea, cellRange.Row - 1, 1, cellRange.Column, cellRange.ColumnCount, flagArray7, lineArray8, false, false);
                    }
                    bool[] flagArray8 = new bool[4];
                    flagArray8[1] = true;
                    BorderLine[] lineArray9 = new BorderLine[4];
                    lineArray9[1] = borderLine;
                    SetCellBorder(workSheet, sheetArea, cellRange.Row, 1, cellRange.Column, cellRange.ColumnCount, flagArray8, lineArray9, false, false);
                }
                if (option.HasFlag(SetBorderOptions.Right))
                {
                    if ((cellRange.Column + cellRange.ColumnCount) < GetColumnCount(workSheet, sheetArea))
                    {
                        bool[] flagArray9 = new bool[4];
                        flagArray9[0] = true;
                        BorderLine[] lineArray10 = new BorderLine[4];
                        SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, cellRange.Column + cellRange.ColumnCount, 1, flagArray9, lineArray10, false, false);
                    }
                    bool[] flagArray10 = new bool[4];
                    flagArray10[2] = true;
                    BorderLine[] lineArray11 = new BorderLine[4];
                    lineArray11[2] = borderLine;
                    SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, (cellRange.Column + cellRange.ColumnCount) - 1, 1, flagArray10, lineArray11, false, false);
                }
                if (option.HasFlag(SetBorderOptions.Bottom))
                {
                    if ((cellRange.Row + cellRange.RowCount) < GetRowCount(workSheet, sheetArea))
                    {
                        bool[] flagArray11 = new bool[4];
                        flagArray11[1] = true;
                        BorderLine[] lineArray12 = new BorderLine[4];
                        SetCellBorder(workSheet, sheetArea, cellRange.Row + cellRange.RowCount, 1, cellRange.Column, cellRange.ColumnCount, flagArray11, lineArray12, false, false);
                    }
                    bool[] flagArray12 = new bool[4];
                    flagArray12[3] = true;
                    BorderLine[] lineArray13 = new BorderLine[4];
                    lineArray13[3] = borderLine;
                    SetCellBorder(workSheet, sheetArea, (cellRange.Row + cellRange.RowCount) - 1, 1, cellRange.Column, cellRange.ColumnCount, flagArray12, lineArray13, false, false);
                }
                if (option.HasFlag(SetBorderOptions.Inside))
                {
                    bool[] flagArray13 = new bool[4];
                    flagArray13[0] = true;
                    flagArray13[1] = true;
                    BorderLine[] lineArray14 = new BorderLine[4];
                    SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, cellRange.Column, cellRange.ColumnCount, flagArray13, lineArray14, true, true);
                    bool[] flagArray14 = new bool[4];
                    flagArray14[2] = true;
                    flagArray14[3] = true;
                    BorderLine[] lineArray15 = new BorderLine[4];
                    lineArray15[2] = borderLine;
                    lineArray15[3] = borderLine;
                    SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, cellRange.Column, cellRange.ColumnCount, flagArray14, lineArray15, true, false);
                }
                else
                {
                    if (option.HasFlag(SetBorderOptions.InnerHorizontal))
                    {
                        if (cellRange.RowCount > 1)
                        {
                            bool[] flagArray15 = new bool[4];
                            flagArray15[1] = true;
                            BorderLine[] lineArray16 = new BorderLine[4];
                            SetCellBorder(workSheet, sheetArea, cellRange.Row + 1, cellRange.RowCount - 1, cellRange.Column, cellRange.ColumnCount, flagArray15, lineArray16, false, false);
                        }
                        bool[] flagArray16 = new bool[4];
                        flagArray16[3] = true;
                        BorderLine[] lineArray17 = new BorderLine[4];
                        lineArray17[3] = borderLine;
                        SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount - 1, cellRange.Column, cellRange.ColumnCount, flagArray16, lineArray17, false, false);
                    }
                    if (option.HasFlag(SetBorderOptions.InnerVertical))
                    {
                        if (cellRange.ColumnCount > 1)
                        {
                            bool[] flagArray17 = new bool[4];
                            flagArray17[0] = true;
                            BorderLine[] lineArray18 = new BorderLine[4];
                            SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, cellRange.Column + 1, cellRange.ColumnCount - 1, flagArray17, lineArray18, false, false);
                        }
                        bool[] flagArray18 = new bool[4];
                        flagArray18[2] = true;
                        BorderLine[] lineArray19 = new BorderLine[4];
                        lineArray19[2] = borderLine;
                        SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, cellRange.Column, cellRange.ColumnCount - 1, flagArray18, lineArray19, false, false);
                    }
                }
            }
        }

        public static void SetColumnsBorder(CellRange cellRange, SheetArea sheetArea, Worksheet workSheet, SetBorderOptions option, BorderLine borderLine)
        {
            if (option.HasFlag(SetBorderOptions.All))
            {
                if (cellRange.Column > 0)
                {
                    bool[] flags = new bool[4];
                    flags[2] = true;
                    BorderLine[] lines = new BorderLine[4];
                    SetAxisBorder(workSheet, sheetArea, cellRange.Column - 1, 1, flags, lines, false);
                }
                if ((cellRange.Column + cellRange.ColumnCount) < GetColumnCount(workSheet, sheetArea))
                {
                    bool[] flagArray2 = new bool[4];
                    flagArray2[0] = true;
                    BorderLine[] lineArray2 = new BorderLine[4];
                    SetAxisBorder(workSheet, sheetArea, cellRange.Column + cellRange.ColumnCount, 1, flagArray2, lineArray2, false);
                }
                SetAxisBorder(workSheet, sheetArea, cellRange.Column, cellRange.ColumnCount, new bool[] { true, true, true, true }, new BorderLine[] { borderLine, borderLine, borderLine, borderLine }, false);
            }
            else
            {
                if (option.HasFlag(SetBorderOptions.Left))
                {
                    if (cellRange.Column > 0)
                    {
                        bool[] flagArray3 = new bool[4];
                        flagArray3[2] = true;
                        BorderLine[] lineArray4 = new BorderLine[4];
                        SetAxisBorder(workSheet, sheetArea, cellRange.Column - 1, 1, flagArray3, lineArray4, false);
                    }
                    bool[] flagArray4 = new bool[4];
                    flagArray4[0] = true;
                    BorderLine[] lineArray5 = new BorderLine[4];
                    lineArray5[0] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, cellRange.Column, 1, flagArray4, lineArray5, false);
                }
                if (option.HasFlag(SetBorderOptions.Right))
                {
                    if ((cellRange.Column + cellRange.ColumnCount) < GetColumnCount(workSheet, sheetArea))
                    {
                        bool[] flagArray5 = new bool[4];
                        flagArray5[0] = true;
                        BorderLine[] lineArray6 = new BorderLine[4];
                        SetAxisBorder(workSheet, sheetArea, cellRange.Column + cellRange.ColumnCount, 1, flagArray5, lineArray6, false);
                    }
                    bool[] flagArray6 = new bool[4];
                    flagArray6[2] = true;
                    BorderLine[] lineArray7 = new BorderLine[4];
                    lineArray7[2] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, (cellRange.Column + cellRange.ColumnCount) - 1, 1, flagArray6, lineArray7, false);
                }
                if (option.HasFlag(SetBorderOptions.Top))
                {
                    bool[] flagArray7 = new bool[4];
                    flagArray7[1] = true;
                    BorderLine[] lineArray8 = new BorderLine[4];
                    lineArray8[1] = borderLine;
                    SetCellBorder(workSheet, sheetArea, 0, 1, cellRange.Column, cellRange.ColumnCount, flagArray7, lineArray8, false, false);
                }
                if (option.HasFlag(SetBorderOptions.Bottom))
                {
                    bool[] flagArray8 = new bool[4];
                    flagArray8[3] = true;
                    BorderLine[] lineArray9 = new BorderLine[4];
                    lineArray9[3] = borderLine;
                    SetCellBorder(workSheet, sheetArea, workSheet.RowCount - 1, 1, cellRange.Column, cellRange.ColumnCount, flagArray8, lineArray9, false, false);
                }
                if (option.HasFlag(SetBorderOptions.InnerHorizontal))
                {
                    if (workSheet.RowCount > 1)
                    {
                        bool[] flagArray9 = new bool[4];
                        flagArray9[1] = true;
                        BorderLine[] lineArray10 = new BorderLine[4];
                        SetCellBorder(workSheet, sheetArea, 1, workSheet.RowCount - 1, cellRange.Column, cellRange.ColumnCount, flagArray9, lineArray10, false, false);
                    }
                    bool[] flagArray10 = new bool[4];
                    flagArray10[3] = true;
                    BorderLine[] lineArray11 = new BorderLine[4];
                    lineArray11[3] = borderLine;
                    SetCellBorder(workSheet, sheetArea, 0, workSheet.RowCount - 1, cellRange.Column, cellRange.ColumnCount, flagArray10, lineArray11, false, false);
                }
                if (option.HasFlag(SetBorderOptions.InnerVertical))
                {
                    if (cellRange.ColumnCount > 1)
                    {
                        bool[] flagArray11 = new bool[4];
                        flagArray11[0] = true;
                        BorderLine[] lineArray12 = new BorderLine[4];
                        SetAxisBorder(workSheet, sheetArea, cellRange.Column + 1, cellRange.ColumnCount - 1, flagArray11, lineArray12, false);
                    }
                    bool[] flagArray12 = new bool[4];
                    flagArray12[2] = true;
                    BorderLine[] lineArray13 = new BorderLine[4];
                    lineArray13[2] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, cellRange.Column, cellRange.ColumnCount - 1, flagArray12, lineArray13, false);
                }
            }
        }

        public static void SetRowsBorder(CellRange cellRange, SheetArea sheetArea, Worksheet workSheet, SetBorderOptions option, BorderLine borderLine)
        {
            if (option.HasFlag(SetBorderOptions.All))
            {
                if (cellRange.Row > 0)
                {
                    bool[] flags = new bool[4];
                    flags[3] = true;
                    BorderLine[] lines = new BorderLine[4];
                    SetAxisBorder(workSheet, sheetArea, cellRange.Row - 1, 1, flags, lines, true);
                }
                if ((cellRange.Row + cellRange.RowCount) < GetRowCount(workSheet, sheetArea))
                {
                    bool[] flagArray2 = new bool[4];
                    flagArray2[1] = true;
                    BorderLine[] lineArray2 = new BorderLine[4];
                    SetAxisBorder(workSheet, sheetArea, cellRange.Row + cellRange.RowCount, 1, flagArray2, lineArray2, true);
                }
                SetAxisBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, new bool[] { true, true, true, true }, new BorderLine[] { borderLine, borderLine, borderLine, borderLine }, true);
            }
            else
            {
                if (option.HasFlag(SetBorderOptions.Left))
                {
                    bool[] flagArray3 = new bool[4];
                    flagArray3[0] = true;
                    BorderLine[] lineArray4 = new BorderLine[4];
                    lineArray4[0] = borderLine;
                    SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, 0, 1, flagArray3, lineArray4, false, false);
                }
                if (option.HasFlag(SetBorderOptions.Right))
                {
                    bool[] flagArray4 = new bool[4];
                    flagArray4[2] = true;
                    BorderLine[] lineArray5 = new BorderLine[4];
                    lineArray5[2] = borderLine;
                    SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, workSheet.ColumnCount - 1, 1, flagArray4, lineArray5, false, false);
                }
                if (option.HasFlag(SetBorderOptions.Top))
                {
                    if (cellRange.Row > 0)
                    {
                        bool[] flagArray5 = new bool[4];
                        flagArray5[3] = true;
                        BorderLine[] lineArray6 = new BorderLine[4];
                        SetAxisBorder(workSheet, sheetArea, cellRange.Row - 1, 1, flagArray5, lineArray6, true);
                    }
                    bool[] flagArray6 = new bool[4];
                    flagArray6[1] = true;
                    BorderLine[] lineArray7 = new BorderLine[4];
                    lineArray7[1] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, cellRange.Row, 1, flagArray6, lineArray7, true);
                }
                if (option.HasFlag(SetBorderOptions.Bottom))
                {
                    if ((cellRange.Row + cellRange.RowCount) < GetRowCount(workSheet, sheetArea))
                    {
                        bool[] flagArray7 = new bool[4];
                        flagArray7[1] = true;
                        BorderLine[] lineArray8 = new BorderLine[4];
                        SetAxisBorder(workSheet, sheetArea, cellRange.Row + cellRange.RowCount, 1, flagArray7, lineArray8, true);
                    }
                    bool[] flagArray8 = new bool[4];
                    flagArray8[3] = true;
                    BorderLine[] lineArray9 = new BorderLine[4];
                    lineArray9[3] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, (cellRange.Row + cellRange.RowCount) - 1, 1, flagArray8, lineArray9, true);
                }
                if (option.HasFlag(SetBorderOptions.InnerHorizontal))
                {
                    if (cellRange.RowCount > 1)
                    {
                        bool[] flagArray9 = new bool[4];
                        flagArray9[1] = true;
                        BorderLine[] lineArray10 = new BorderLine[4];
                        SetAxisBorder(workSheet, sheetArea, cellRange.Row + 1, cellRange.RowCount - 1, flagArray9, lineArray10, true);
                    }
                    bool[] flagArray10 = new bool[4];
                    flagArray10[3] = true;
                    BorderLine[] lineArray11 = new BorderLine[4];
                    lineArray11[3] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount - 1, flagArray10, lineArray11, true);
                }
                if (option.HasFlag(SetBorderOptions.InnerVertical))
                {
                    if (workSheet.ColumnCount > 1)
                    {
                        bool[] flagArray11 = new bool[4];
                        flagArray11[0] = true;
                        BorderLine[] lineArray12 = new BorderLine[4];
                        SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, 1, workSheet.ColumnCount - 1, flagArray11, lineArray12, false, false);
                    }
                    bool[] flagArray12 = new bool[4];
                    flagArray12[2] = true;
                    BorderLine[] lineArray13 = new BorderLine[4];
                    lineArray13[2] = borderLine;
                    SetCellBorder(workSheet, sheetArea, cellRange.Row, cellRange.RowCount, 0, workSheet.ColumnCount - 1, flagArray12, lineArray13, false, false);
                }
            }
        }

        public static void SetSheetBorder(SheetArea sheetArea, Worksheet workSheet, SetBorderOptions option, BorderLine borderLine, SheetArea sheatarea, StyleInfo borderstyle)
        {
            if (option.HasFlag(SetBorderOptions.All))
            {
                borderstyle.BorderLeft = borderLine.Clone() as BorderLine;
                borderstyle.BorderTop = borderLine.Clone() as BorderLine;
                borderstyle.BorderRight = borderLine.Clone() as BorderLine;
                borderstyle.BorderBottom = borderLine.Clone() as BorderLine;
            }
            else
            {
                if (option.HasFlag(SetBorderOptions.Left))
                {
                    bool[] flags = new bool[4];
                    flags[0] = true;
                    BorderLine[] lines = new BorderLine[4];
                    lines[0] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, 0, 1, flags, lines, false);
                }
                if (option.HasFlag(SetBorderOptions.Top))
                {
                    bool[] flagArray2 = new bool[4];
                    flagArray2[1] = true;
                    BorderLine[] lineArray2 = new BorderLine[4];
                    lineArray2[1] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, 0, 1, flagArray2, lineArray2, true);
                }
                if (option.HasFlag(SetBorderOptions.Right))
                {
                    bool[] flagArray3 = new bool[4];
                    flagArray3[2] = true;
                    BorderLine[] lineArray3 = new BorderLine[4];
                    lineArray3[2] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, workSheet.ColumnCount - 1, 1, flagArray3, lineArray3, false);
                }
                if (option.HasFlag(SetBorderOptions.Bottom))
                {
                    bool[] flagArray4 = new bool[4];
                    flagArray4[3] = true;
                    BorderLine[] lineArray4 = new BorderLine[4];
                    lineArray4[3] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, workSheet.RowCount - 1, 1, flagArray4, lineArray4, true);
                }
                if (option.HasFlag(SetBorderOptions.InnerHorizontal))
                {
                    if (workSheet.RowCount > 1)
                    {
                        bool[] flagArray5 = new bool[4];
                        flagArray5[1] = true;
                        BorderLine[] lineArray5 = new BorderLine[4];
                        SetAxisBorder(workSheet, sheetArea, 1, workSheet.RowCount - 1, flagArray5, lineArray5, true);
                    }
                    bool[] flagArray6 = new bool[4];
                    flagArray6[3] = true;
                    BorderLine[] lineArray6 = new BorderLine[4];
                    lineArray6[3] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, 0, workSheet.RowCount - 1, flagArray6, lineArray6, true);
                }
                if (option.HasFlag(SetBorderOptions.InnerVertical))
                {
                    if (workSheet.ColumnCount > 1)
                    {
                        bool[] flagArray7 = new bool[4];
                        flagArray7[0] = true;
                        BorderLine[] lineArray7 = new BorderLine[4];
                        SetAxisBorder(workSheet, sheetArea, 1, workSheet.ColumnCount - 1, flagArray7, lineArray7, false);
                    }
                    bool[] flagArray8 = new bool[4];
                    flagArray8[2] = true;
                    BorderLine[] lineArray8 = new BorderLine[4];
                    lineArray8[2] = borderLine;
                    SetAxisBorder(workSheet, sheetArea, 0, workSheet.ColumnCount - 1, flagArray8, lineArray8, false);
                }
            }
        }

        static void UpdateStyle(StyleInfo style, bool[] sets, BorderLine[] lines)
        {
            if (sets[0])
            {
                style.BorderLeft = lines[0];
            }
            if (sets[1])
            {
                style.BorderTop = lines[1];
            }
            if (sets[2])
            {
                style.BorderRight = lines[2];
            }
            if (sets[3])
            {
                style.BorderBottom = lines[3];
            }
        }
    }
}


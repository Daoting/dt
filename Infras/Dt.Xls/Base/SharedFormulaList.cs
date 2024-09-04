#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Xls
{
    internal class SharedFormulaList
    {
        private ExcelReferenceStyle _refMode;
        private Dictionary<int, Vector<Vector<List<SharedFormula>>>> _sharedFormulas = new Dictionary<int, Vector<Vector<List<SharedFormula>>>>();
        private List<object> _unfoundFormulaRefList;

        public SharedFormulaList(ExcelReferenceStyle refMode)
        {
            this._refMode = refMode;
            this._unfoundFormulaRefList = new List<object>();
        }

        public bool AddSharedFormula(int sheet, int rowFirst, int rowLast, short colFirst, short colLast, byte[] formula, byte[] extra, string baseFormula, LinkTable linkTable)
        {
            SharedFormula formula2 = new SharedFormula(sheet, rowFirst, rowLast, colFirst, colLast, formula, extra, baseFormula);
            Vector<Vector<List<SharedFormula>>> vector = null;
            if (!this._sharedFormulas.ContainsKey(sheet))
            {
                this._sharedFormulas.Add(sheet, new Vector<Vector<List<SharedFormula>>>());
            }
            vector = this._sharedFormulas[sheet];
            for (int i = rowFirst; i <= rowLast; i++)
            {
                if (vector[i] == null)
                {
                    vector[i] = new Vector<List<SharedFormula>>();
                }
                Vector<List<SharedFormula>> vector2 = vector[i];
                for (int j = colFirst; j <= colLast; j++)
                {
                    if (vector2[j] == null)
                    {
                        vector2[j] = new List<SharedFormula>();
                    }
                    vector2[j].Add(formula2);
                }
            }
            if (this._unfoundFormulaRefList.Count > 0)
            {
                int num3 = this._unfoundFormulaRefList.Count;
                for (int k = 0; k < num3; k++)
                {
                    TriCoord coord = (TriCoord) this._unfoundFormulaRefList[k];
                    if (((coord.Col >= colFirst) && (coord.Row >= rowFirst)) && ((coord.Row <= rowLast) && (coord.Col <= colLast)))
                    {
                        MemoryStream stream = new MemoryStream(formula);
                        new BinaryReader((Stream) stream).Close();
                        this._unfoundFormulaRefList.RemoveAt(k);
                        break;
                    }
                }
            }
            return true;
        }

        public bool AddUnfoundFormulaReference(int sheet, int row, int col)
        {
            TriCoord coord = new TriCoord(sheet, row, col);
            this._unfoundFormulaRefList.Add(coord);
            return true;
        }

        public bool Clear()
        {
            this._sharedFormulas.Clear();
            this._unfoundFormulaRefList.Clear();
            return true;
        }

        public bool GetSharedFormula(int sheetIndex, int row, int col, ref byte[] formula, ref byte[] extra)
        {
            Vector<Vector<List<SharedFormula>>> vector = null;
            if (this._sharedFormulas.TryGetValue(sheetIndex, out vector) && (vector[row] != null))
            {
                List<SharedFormula> list = vector[row][col];
                if ((list != null) && (list.Count > 0))
                {
                    formula = list[0].formula;
                    extra = list[0].extra;
                    return true;
                }
            }
            return false;
        }

        public bool GetSharedFormula(int sheetIndex, int row, int col, int firstRow, int firstCol, ref byte[] formula, ref byte[] extra)
        {
            Vector<Vector<List<SharedFormula>>> vector = null;
            if (this._sharedFormulas.TryGetValue(sheetIndex, out vector) && (vector[row] != null))
            {
                List<SharedFormula> list = vector[row][col];
                if ((list != null) && (list.Count > 0))
                {
                    foreach (SharedFormula formula2 in list)
                    {
                        if (((formula2 != null) && (formula2.FirstRow == firstRow)) && (formula2.FirstCol == firstCol))
                        {
                            formula = formula2.formula;
                            extra = formula2.extra;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal class SharedFormula
        {
            internal string baseFormula;
            internal byte[] extra;
            internal short firstCol = -1;
            internal int firstRow = -1;
            internal byte[] formula;
            internal short lastCol = -1;
            internal int lastRow = -1;
            private int sheet = -1;

            public SharedFormula(int sheet, int firstRow, int lastRow, short firstCol, short lastCol, byte[] formula, byte[] extra, string baseFormula)
            {
                this.sheet = sheet;
                this.firstRow = firstRow;
                this.lastRow = lastRow;
                this.firstCol = firstCol;
                this.lastCol = lastCol;
                this.formula = formula;
                this.extra = extra;
                this.baseFormula = baseFormula;
            }

            public byte[] Extra
            {
                get { return  this.extra; }
                set { this.extra = value; }
            }

            public short FirstCol
            {
                get { return  this.firstCol; }
                set { this.firstCol = value; }
            }

            public int FirstRow
            {
                get { return  this.firstRow; }
                set { this.firstRow = value; }
            }

            public byte[] Formula
            {
                get { return  this.formula; }
                set { this.formula = value; }
            }

            public short LastCol
            {
                get { return  this.lastCol; }
                set { this.lastCol = value; }
            }

            public int LastRow
            {
                get { return  this.lastRow; }
                set { this.lastRow = value; }
            }

            public int Sheet
            {
                get { return  this.sheet; }
                set { this.sheet = value; }
            }
        }
    }
}


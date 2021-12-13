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
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// All type of references in stored in the LinkTable inside the workbook Global Substream. All
    /// formulas use only indexes for specified references. The Linktable itself is optional and occurs
    /// only if there are references in the document.
    /// </summary>
    internal class LinkTable
    {
        private List<string> _customOrFuctionNames;
        private Dictionary<ExcelExternSheet, int> _excelExternSheets = new Dictionary<ExcelExternSheet, int>();
        private List<string> _internalSheetNames;
        internal List<object> CustomNames = new List<object>();
        internal List<Tuple<string, int, object>> DefinedNames = new List<Tuple<string, int, object>>();
        internal HashSet<string> DefinedNamesHashSet = new HashSet<string>();
        internal Dictionary<string, List<IName>> ExternalNamedCellRanges = new Dictionary<string, List<IName>>();
        internal List<List<string>> ExternalNames = new List<List<string>>();
        internal Dictionary<string, List<string>> externalReferencedWookbookInfo = new Dictionary<string, List<string>>();
        internal List<ExcelExternSheet> ExternalSheets = new List<ExcelExternSheet>();
        internal SharedFormulaList sharedFormulaList;
        internal List<ExcelSupBook> SupBooks = new List<ExcelSupBook>();

        internal void AddDefinedNames(string name, int sheetIndex, object self = null)
        {
            this.DefinedNames.Add(new Tuple<string, int, object>(name, sheetIndex, null));
            this.DefinedNamesHashSet.Add(name);
        }

        internal void AddExternalBook(ExcelSupBook book)
        {
            this.SupBooks.Add(book);
            if (!string.IsNullOrWhiteSpace(book.FileName))
            {
                this.ExternalNamedCellRanges.Add(book.FileName, new List<IName>());
            }
            else
            {
                this.ExternalNames.Add(new List<string>());
            }
        }

        internal void AddExternNames(string name, int sheetIndex)
        {
            if (this.SupBooks.Count > 0)
            {
                ExcelSupBook book = this.SupBooks[this.SupBooks.Count - 1];
                string fileName = book.FileName;
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    if (sheetIndex >= 0)
                    {
                        this.ExternalNamedCellRanges[fileName].Add(new NamedCellRange(name, sheetIndex));
                    }
                    else
                    {
                        this.ExternalNamedCellRanges[fileName].Add(new NamedCellRange(name, -1));
                    }
                }
                else if ((book.SheetCount != book.SheetNames.Count) && !book.IsSelfReferenced)
                {
                    this.CustomOrFuctionNames.Add(name);
                }
            }
            else
            {
                this.CustomOrFuctionNames.Add(name);
            }
        }

        internal void AddExternSheet(int externBookIndex, int beginSheetIndex, int endSheetIndex)
        {
            ExcelExternSheet sheet = new ExcelExternSheet {
                fileName = this.SupBooks[externBookIndex].FileName,
                beginSheetIndex = beginSheetIndex,
                beginSheetName = this.GetExternSheetName(externBookIndex, beginSheetIndex),
                endSheetIndex = endSheetIndex,
                endSheetName = this.GetExternSheetName(externBookIndex, endSheetIndex)
            };
            this.ExternalSheets.Add(sheet);
        }

        internal int GetCustomOrFunctionNameIndex(string name)
        {
            string str = name.ToUpperInvariant();
            for (int i = 0; i < this.CustomOrFuctionNames.Count; i++)
            {
                if (this.CustomOrFuctionNames[i].ToUpperInvariant() == str)
                {
                    return i;
                }
            }
            this.CustomOrFuctionNames.Add(str);
            return (this.CustomOrFuctionNames.Count - 1);
        }

        internal int GetDefinedNameIndex(string name)
        {
            for (int i = 0; i < this.DefinedNames.Count; i++)
            {
                if (this.DefinedNames[i].Item1 == name)
                {
                    return i;
                }
            }
            if (name.ToUpperInvariant().StartsWith("_XLFN."))
            {
                IBuiltInName name2 = new BuiltInName(name);
                this.DefinedNames.Add(new Tuple<string, int, object>(name, -1, name2));
                this.DefinedNamesHashSet.Add(name);
                return (this.DefinedNames.Count - 1);
            }
            return -1;
        }

        internal int GetDefinedNameIndex(string name, int sheet)
        {
            int num = 0;
            foreach (Tuple<string, int, object> tuple in this.DefinedNames)
            {
                if ((tuple.Item1 == name) && (tuple.Item2 == sheet))
                {
                    return num;
                }
                num++;
            }
            this.DefinedNames.Add(new Tuple<string, int, object>(name, sheet, null));
            this.DefinedNamesHashSet.Add(name);
            return num;
        }

        internal int GetExcelExternSheetIndex(ExcelExternSheet sheet)
        {
            int num;
            if (!this._excelExternSheets.TryGetValue(sheet, out num))
            {
                num = this.ExternalSheets.Count;
                this._excelExternSheets.Add(sheet, num);
                this.ExternalSheets.Add(sheet);
            }
            return num;
        }

        private string GetExternSheetName(int externBookIndex, int sheetIndex)
        {
            if (sheetIndex == -2)
            {
                return null;
            }
            if (sheetIndex == -1)
            {
                return "#REF";
            }
            if (sheetIndex >= this.SupBooks[externBookIndex].SheetNames.Count)
            {
                return null;
            }
            return this.SupBooks[externBookIndex].SheetNames[sheetIndex];
        }

        internal string GetExternSheetName(string fileName, int sheetIndex)
        {
            if (this.externalReferencedWookbookInfo.ContainsKey(fileName) && (sheetIndex >= 0))
            {
                return this.externalReferencedWookbookInfo[fileName][sheetIndex];
            }
            return string.Empty;
        }

        internal List<string> GetInternalSheetNames()
        {
            foreach (ExcelSupBook book in this.SupBooks)
            {
                if (string.IsNullOrWhiteSpace(book.FileName))
                {
                    return book.SheetNames;
                }
            }
            return null;
        }

        internal bool HasDefindName(string name, int sheetIndex = -1)
        {
            if (this.DefinedNamesHashSet.Contains(name))
            {
                foreach (Tuple<string, int, object> tuple in this.DefinedNames)
                {
                    if ((tuple.Item2 == sheetIndex) && (tuple.Item1 == name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal List<string> CustomOrFuctionNames
        {
            get
            {
                if (this._customOrFuctionNames == null)
                {
                    this._customOrFuctionNames = new List<string>();
                }
                return this._customOrFuctionNames;
            }
            set { this._customOrFuctionNames = value; }
        }

        internal List<string> InternalSheetNames
        {
            get
            {
                if (this._internalSheetNames == null)
                {
                    this._internalSheetNames = new List<string>();
                }
                return this._internalSheetNames;
            }
            set { this._internalSheetNames = value; }
        }
    }
}


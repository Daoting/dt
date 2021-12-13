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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// A class implements the <see cref="T:Dt.Xls.IExternalWorkbookInfo" /> interface which used to represents an external workbook
    /// </summary>
    public class ExternalWorkbookInfo : IExternalWorkbookInfo
    {
        private List<IName> _definedNames;
        private List<Tuple<int, byte[]>> _exteranNameBits;
        private List<Tuple<int, short[]>> _externSheetBits;
        private List<string> _sheetNames;
        internal byte[] ExternalBookBits;

        /// <summary>
        /// Get the collection of named cell ranges of the external workbook
        /// </summary>
        public List<IName> DefinedNames
        {
            get
            {
                if (this._definedNames == null)
                {
                    this._definedNames = new List<IName>();
                }
                return this._definedNames;
            }
            internal set { this._definedNames = value; }
        }

        internal List<Tuple<int, byte[]>> ExternNameBits
        {
            get
            {
                if (this._exteranNameBits == null)
                {
                    this._exteranNameBits = new List<Tuple<int, byte[]>>();
                }
                return this._exteranNameBits;
            }
            set { this._exteranNameBits = value; }
        }

        internal List<Tuple<int, short[]>> ExternSheetBits
        {
            get
            {
                if (this._externSheetBits == null)
                {
                    this._externSheetBits = new List<Tuple<int, short[]>>();
                }
                return this._externSheetBits;
            }
            set { this._externSheetBits = value; }
        }

        /// <summary>
        /// Get the external workbook name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Get the collection of sheet names of the external workbook
        /// </summary>
        public List<string> SheetNames
        {
            get
            {
                if (this._sheetNames == null)
                {
                    this._sheetNames = new List<string>();
                }
                return this._sheetNames;
            }
            internal set { this._sheetNames = value; }
        }
    }
}


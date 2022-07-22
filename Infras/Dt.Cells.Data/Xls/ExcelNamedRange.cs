#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal class ExcelNamedRange : IName
    {
        public ExcelNamedRange(string name, int sheet = -1)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            this.Name = name;
            this.Index = sheet;
        }

        public string Comment
        {
            get { return  null; }
        }

        public int Index { get; set; }

        public bool IsHidden
        {
            get { return  false; }
        }

        public string Name { get; set; }

        public string RefersTo { get; internal set; }

        public string RefersToR1C1 { get; internal set; }
    }
}


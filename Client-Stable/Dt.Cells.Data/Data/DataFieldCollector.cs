#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    internal class DataFieldCollector
    {
        Worksheet sheet;

        public DataFieldCollector(Worksheet sheet)
        {
            this.sheet = sheet;
        }

        public void Collect(int columnIndex, int count)
        {
        }

        public void Flush(int columnIndex, int count)
        {
        }

        public void Reset()
        {
        }
    }
}


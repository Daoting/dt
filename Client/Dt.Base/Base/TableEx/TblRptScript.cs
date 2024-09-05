#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 导出Lv报表脚本
    /// </summary>
    internal class TblRptScript : RptScript
    {
        public Table Data {  get; set; }

        public override Task<Table> GetData(string p_name)
        {
            return Task.FromResult(Data);
        }
    }
}

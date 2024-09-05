#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
using Dt.Base.Report;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Lv导出打印时的报表设置
    /// </summary>
    public class LvRptInfo : TblRptInfo
    {
        public LvRptInfo()
        {
            Name = "Lv导出";
            ScriptObj = new LvRptScript();
        }

        /// <summary>
        /// 只导出选择行数据，默认false
        /// </summary>
        public bool OnlySelection { get; set; }
    }
}

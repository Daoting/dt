#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-08-04
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自动生成各类字典，生成方法：win版app -> 系统日志 -> 存根
    /// </summary>
    public abstract partial class DefaultStub : Stub
    {
        // 本地库结构变化后，需通过《 win版app -> 系统日志 -> 存根 》重新生成！

        /// <summary>
        /// 合并本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// 先调用base.MergeSqliteDbs，不可覆盖上级的同名本地库
        /// </summary>
        /// <param name="p_sqliteDbs"></param>
        protected override void MergeSqliteDbs(Dictionary<string, SqliteTblsInfo> p_sqliteDbs)
        {
            base.MergeSqliteDbs(p_sqliteDbs);
            p_sqliteDbs["state"] = new SqliteTblsInfo
            {
                Version = "8a4008dd30da199d2a223dd260adb354",
                Tables = new List<Type>
                {
                    typeof(Dt.Base.Docking.DockLayout),
                    typeof(Dt.Base.ModuleView.SearchHistory),
                    typeof(Dt.Base.FormView.CellLastVal),
                    typeof(Dt.Core.ClientCookie),
                }
            };
        }
    }
}
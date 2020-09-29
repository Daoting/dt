#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base.Report;
using Dt.Core;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 报表脚本基类
    /// </summary>
    public abstract class RptScript
    {
        /// <summary>
        /// 报表预览控件
        /// </summary>
        public RptView View { get; internal set; }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="p_name">数据表名称</param>
        /// <returns></returns>
        public virtual Task<Table> GetData(string p_name)
        {
            return Task.FromResult(default(Table));
        }

        /// <summary>
        /// 初始化工具栏菜单，报表组不支持
        /// </summary>
        /// <param name="p_menu"></param>
        public virtual void InitMenu(Menu p_menu)
        {
        }

        /// <summary>
        /// 点击单元格脚本
        /// </summary>
        /// <param name="p_id">脚本标识</param>
        /// <param name="p_text">单元格</param>
        public virtual void OnCellClick(string p_id, IRptCell p_text)
        {

        }
    }
}

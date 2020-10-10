#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
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
        /// 绘制单元格内容和样式
        /// </summary>
        /// <param name="p_cell">单元格</param>
        /// <param name="p_args">单元格脚本参数</param>
        public virtual void RenderCell(Cells.Data.Cell p_cell, RptCellArgs p_args)
        {
        }

        /// <summary>
        /// 初始化工具栏菜单，报表组不支持
        /// </summary>
        /// <param name="p_menu"></param>
        public virtual void InitMenu(Menu p_menu)
        {
        }

        /// <summary>
        /// RptView中打开上下文菜单
        /// </summary>
        /// <param name="rightMenu"></param>
        public virtual void OpenContextMenu(Menu p_contextMenu)
        {
        }

        /// <summary>
        /// 点击单元格脚本
        /// </summary>
        /// <param name="p_args">单元格脚本参数</param>
        public virtual void OnCellClick(RptCellArgs p_args)
        {
        }
    }
}

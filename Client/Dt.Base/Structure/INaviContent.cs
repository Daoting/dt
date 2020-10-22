#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自定义Tab内容
    /// </summary>
    public interface INaviContent
    {
        /// <summary>
        /// 所属容器
        /// </summary>
        INaviHost Host { get; set; }

        /// <summary>
        /// 在容器显示的菜单
        /// </summary>
        Menu HostMenu { get; }

        /// <summary>
        /// 容器标题
        /// </summary>
        string HostTitle { get; }
    }
}

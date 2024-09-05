#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-01 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Mgr.Workflow;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 流程表单列表
    /// </summary>
    public abstract partial class WfList : List
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        public abstract string PrcName { get; }

        /// <summary>
        /// 添加默认菜单项：发起新任务，若提供Menu则添加默认项，否则创建新菜单
        /// </summary>
        /// <param name="p_menu">若提供Menu则添加默认项，否则创建新菜单</param>
        /// <returns></returns>
        protected async Task<Menu> CreateMenu(Menu p_menu = null)
        {
            Menu menu = p_menu ?? new Menu();
            if (await WfdDs.IsStartable(PrcName))
            {
                menu.Items.Add(new Mi("发起新任务", Icons.播放, call: OnAdd));
            }
            return menu;
        }

        /// <summary>
        /// 添加Lv默认上下文菜单项：打开表单、查看日志，若提供Menu则添加默认项，否则创建新菜单
        /// </summary>
        /// <param name="p_menu">若提供Menu则添加默认项，否则创建新菜单</param>
        /// <returns></returns>
        protected Menu CreateContextMenu(Menu p_menu = null)
        {
            Menu menu = p_menu ?? new Menu();
            menu.Items.Add(new Mi("打开表单", Icons.文件, OnEdit));
            menu.Items.Add(new Mi("查看日志", Icons.日志, OnShowLog));
            return menu;
        }

        /// <summary>
        /// 当前用户是否可"发起新任务"
        /// </summary>
        /// <returns></returns>
        protected Task<bool> IsStartable()
        {
            return WfdDs.IsStartable(PrcName);
        }
        
        void OnShowLog(Mi e)
        {
            AtWf.ShowLog(e.Row.ID);
        }
    }
}

#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-19 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 报表预览视图，可由菜单命令启动或自定义启动
    /// </summary>
    [View("报表")]
    public class ReportView : IView
    {
        /// <summary>
        /// 视图启动入口
        /// </summary>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">启动参数</param>
        public void Run(string p_title, Icons p_icon, object p_params)
        {
            if (p_params == null || string.IsNullOrEmpty(p_params.ToString()))
                return;

            string param = p_params.ToString();
            Icons icon = p_icon == Icons.None ? Icons.折线图 : p_icon;
            string title = string.IsNullOrEmpty(p_title) ? param : p_title;

            var ls = param.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (ls.Length == 1)
            {
                AtRpt.Show(new RptInfo { Name = ls[0] }, title, icon);
            }
            else
            {
                List<RptInfo> infos = new List<RptInfo>();
                foreach (var name in ls)
                {
                    infos.Add(new RptInfo { Name = name });
                }
                AtRpt.Show(infos, title, icon);
            }
        }
    }
}

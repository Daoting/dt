#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-05 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Base
{
    public class GenericView
    {
        /// <summary>
        /// 根据视图配置激活旧窗口或打开新窗口
        /// </summary>
        /// <param name="p_cfg">视图配置</param>
        /// <param name="p_title">标题</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenSingleTbl(EntityCfg p_cfg, string p_title = null)
        {
            return Kit.OpenView("通用单表视图", p_title: p_title, p_params: p_cfg);
        }

        /// <summary>
        /// 根据视图配置激活旧窗口或打开新窗口
        /// </summary>
        /// <param name="p_jsonCfg">视图配置json</param>
        /// <param name="p_title">标题</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenSingleTbl(string p_jsonCfg, string p_title = null)
        {
            return Kit.OpenView("通用单表视图", p_title: p_title, p_params: p_jsonCfg);
        }
    }
}
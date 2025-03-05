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
        public static object SingleTbl(SingleTblCfg p_cfg, string p_title = null)
        {
            return Kit.OpenView("通用单表视图", p_title: p_title, p_params: p_cfg);
        }
    }
}
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-04-07 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr
{
    class Per
    {
        public class 系统预留
        {
            public class 文件管理
            {
                public static Task<bool> 公共文件增删 => Kit.HasPermission(1L);
                public static Task<bool> 素材库增删 => Kit.HasPermission(2L);
            }
        }
    }
}

#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-18 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    /// <summary>
    /// 权限，VS扩展自动生成
    /// </summary>
    public static partial class Gs
	{
        public static class 系统预留
        {
            public static class 文件管理
            {
                public static Task<bool> 公共文件增删 => Kit.HasPermission(1L);
                public static Task<bool> 素材库增删 => Kit.HasPermission(2L);
            }
        }

        public static class 模块1
        {
            public static class 功能1
            {
                public static Task<bool> 权限1 => Kit.HasPermission(87434193769099264L);
            }
            public static class 功能2
            {
                public static Task<bool> 新权限1 => Kit.HasPermission(87428792977747968L);
                public static Task<bool> qx1 => Kit.HasPermission(87433224209592320L);
                public static Task<bool> 阿斯顿 => Kit.HasPermission(87433598698024960L);
            }
        }

        public static class 物资管理
        {
            public static class 入出
            {
                public static Task<bool> 冲销 => Kit.HasPermission(87434002596917248L);
            }
        }
	}
}

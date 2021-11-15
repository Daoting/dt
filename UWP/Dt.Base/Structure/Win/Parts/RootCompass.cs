#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 全局停靠导航，最底层四个停靠位置
    /// </summary>
    public partial class RootCompass : Compass
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RootCompass()
        {
            DefaultStyleKey = typeof(RootCompass);
        }
    }
}


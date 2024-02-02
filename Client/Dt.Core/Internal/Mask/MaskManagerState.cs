#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 掩码状态基类
    /// </summary>
    public abstract class MaskManagerState
    {
        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        protected MaskManagerState() { }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 比较掩码状态是否和当前状态相同
        /// </summary>
        /// <param name="comparedState">掩码状态实例</param>
        /// <returns></returns>
        public abstract bool IsSame(MaskManagerState comparedState);
        #endregion
    }
}


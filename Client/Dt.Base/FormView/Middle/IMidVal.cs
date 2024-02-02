#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-24 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 格取值赋值接口
    /// </summary>
    public interface IFvCall
    {
        /// <summary>
        /// 从数据源取值
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        object Get(Mid m);

        /// <summary>
        /// 将值写人数据源
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        object Set(Mid m);
    }
}
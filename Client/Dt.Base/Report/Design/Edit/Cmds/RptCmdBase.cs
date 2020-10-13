#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表项命令基类
    /// </summary>
    internal abstract class RptCmdBase
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="p_args"></param>
        /// <returns></returns>
        public abstract object Execute(object p_args);

        /// <summary>
        /// 撤消
        /// </summary>
        /// <param name="p_args"></param>
        public abstract void Undo(object p_args);
    }
}

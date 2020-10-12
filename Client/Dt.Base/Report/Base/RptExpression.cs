#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 表达式描述列
    /// </summary>
    internal class RptExpression
    {
        /// <summary>
        /// 获取设置功能种类
        /// </summary>
        public RptExpFunc Func { get; set; }

        /// <summary>
        /// 获取设置数据源名称
        /// </summary>
        public string DataName { get; set; }

        /// <summary>
        /// 获取设置变量名称
        /// </summary>
        public string VarName { get; set; }
    }

    /// <summary>
    /// 表达式功能种类
    /// </summary>
    internal enum RptExpFunc
    {
        /// <summary>
        /// 取值
        /// </summary>
        Val,

        /// <summary>
        /// 求和
        /// </summary>
        Sum,

        /// <summary>
        /// 求平均
        /// </summary>
        Avg,

        /// <summary>
        /// 求最大
        /// </summary>
        Max,

        /// <summary>
        /// 求最小
        /// </summary>
        Min,

        /// <summary>
        /// 总行数
        /// </summary>
        Count,

        /// <summary>
        /// 当前行数
        /// </summary>
        Index,

        /// <summary>
        /// 取参数值
        /// </summary>
        Param,

        /// <summary>
        /// 取全局变量值
        /// </summary>
        Global,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown
    }
}

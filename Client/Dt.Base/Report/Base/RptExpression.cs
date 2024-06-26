﻿#region 文件描述
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
    public class RptExpression
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
    public enum RptExpFunc
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
        /// 分组内的列值
        /// </summary>
        Group,

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
        Var,

        /// <summary>
        /// 调用外部方法取值，和自定义参数缺省值相同格式
        /// </summary>
        Call,
        
        /// <summary>
        /// 未知
        /// </summary>
        Unknown
    }

    public enum PlaceholderType
    {
        None = 0,
        PageNum = 1,
        PageCount = 2,
        Call = 4
    }
}

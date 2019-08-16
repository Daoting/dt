#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-08-19 创建
**************************************************************************/
#endregion

#region 引用命名


#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 掩码类型
    /// </summary>
    public enum MaskType
    {
        /// <summary>
        /// 支持简单型表达式，适合处理有固定长度或固定格式的字符串，如电话号码、邮箱等
        /// </summary>
        Simple,

        /// <summary>
        /// 时间掩码
        /// </summary>
        DateTime,

        /// <summary>
        /// 智能补充式时间掩码
        /// </summary>
        DateTimeAdvancingCaret,

        /// <summary>
        /// 数字类型
        /// </summary>
        Numeric,

        /// <summary>
        /// 全功能正则表达式
        /// </summary>
        RegEx,

        /// <summary>
        /// 支持简单的正则表达式，通常用来指定某范围的可选字符，或某位置的字符个数等，
        /// 缺少动态内容替换功能，也缺少自动完成功能
        /// </summary>
        Regular
    }
}

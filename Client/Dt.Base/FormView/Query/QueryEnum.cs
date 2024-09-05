#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-11-24 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单元格查询控制
    /// </summary>
    public enum QueryType
    {
        /// <summary>
        /// 禁止查询
        /// </summary>
        Disable,

        /// <summary>
        /// 比较操作符可修改
        /// </summary>
        Editable,

        /// <summary>
        /// 比较操作符只读
        /// </summary>
        ReadOnly
    }

    /// <summary>
    /// 单元格内容构成检索条件时用的比较操作符，
    /// </summary>
    public enum CompFlag
    {
        /// <summary>
        /// 忽略，不参与检索
        /// </summary>
        Ignore,

        /// <summary>
        /// 相等
        /// </summary>
        Equal,

        /// <summary>
        /// 不相等
        /// </summary>
        Unequal,

        /// <summary>
        /// 小于
        /// </summary>
        Less,

        /// <summary>
        /// 小于等于
        /// </summary>
        Ceil,

        /// <summary>
        /// 大于
        /// </summary>
        Greater,

        /// <summary>
        /// 大于等于
        /// </summary>
        Floor,

        /// <summary>
        /// 以 ... 开头
        /// </summary>
        StartsWith,

        /// <summary>
        /// 以 ... 结尾
        /// </summary>
        EndsWith,

        /// <summary>
        /// 在任何位置出现
        /// </summary>
        Contains,

        /// <summary>
        /// 空
        /// </summary>
        Null,
    }
}
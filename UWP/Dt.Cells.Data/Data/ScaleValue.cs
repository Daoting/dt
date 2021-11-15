#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a scale value.
    /// </summary>
    public sealed class ScaleValue
    {
        ScaleValueType type;
        object value;

        /// <summary>
        /// Constructs a scale value with the specified type and value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        public ScaleValue(ScaleValueType type, object value)
        {
            this.type = type;
            this.value = value;
        }

        /// <summary>
        /// Gets the scale value type.
        /// </summary>
        public ScaleValueType Type
        {
            get { return  this.type; }
        }

        /// <summary>
        /// Gets the scale value.
        /// </summary>
        public object Value
        {
            get { return  this.value; }
        }
    }
}


#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单元格UI接口
    /// </summary>
    public interface ICellUI
    {
        /// <summary>
        /// 获取设置列名(字段名)
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// 获取设置单元格UI类型
        /// </summary>
        CellUIType UI { get; set; }

        /// <summary>
        /// 获取设置格式串，null或空时按默认显示，如：时间格式、小数位格式、枚举类型名称
        /// </summary>
        string Format { get; set; }
    }
}

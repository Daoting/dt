#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
using System.Reflection;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// ViewItem的宿主控件接口
    /// </summary>
    public interface IViewItemHost
    {
        /// <summary>
        /// 设置行/项目样式，前景、背景、字体样式大小粗细
        /// </summary>
        /// <param name="p_item">视图项</param>
        void SetItemStyle(ViewItem p_item);

        /// <summary>
        /// 获取外部定义的视图扩展方法
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <returns></returns>
        MethodInfo GetViewExMethod(string p_colName);
    }
}

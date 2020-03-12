#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自定义格内容接口
    /// </summary>
    public interface IFreeCell
    {
        /// <summary>
        /// 获取设置所属格
        /// </summary>
        FvCell Owner { get; set; }

        /// <summary>
        /// 数据源绑定编辑器
        /// </summary>
        /// <param name="p_bind"></param>
        void SetValBinding(Binding p_bind);

        /// <summary>
        /// 设置编辑焦点
        /// </summary>
        /// <returns></returns>
        bool SetFocus();
    }
}

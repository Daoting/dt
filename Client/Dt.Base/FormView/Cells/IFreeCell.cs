#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

using Windows.UI.Xaml.Data;

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


        void SetValBinding(Binding p_bind);

        bool SetFocus();
    }
}

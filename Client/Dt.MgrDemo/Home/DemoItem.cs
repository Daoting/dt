#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.MgrDemo
{
    public class DemoItem
    {
        public DemoItem(string p_title, Type p_type, string p_desc)
        {
            Title = p_title;
            Type = p_type;
            Desc = p_desc;
        }

        /// <summary>
        /// 获取设置标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取设置描述信息
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 获取设置窗口类型
        /// </summary>
        public Type Type { get; set; }
    }
}
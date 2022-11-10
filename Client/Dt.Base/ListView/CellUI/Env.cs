#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自定义单元格UI的参数
    /// </summary>
    public class Env
    {
        ViewItem _item;
        Dot _dot;

        internal Env(ViewItem p_item, Dot p_dot)
        {
            _item = p_item;
            _dot = p_dot;
        }

        /// <summary>
        /// 获取行的数据源
        /// </summary>
        public object Data => _item.Data;

        /// <summary>
        /// 获取Row数据源
        /// </summary>
        public Row Row => _item.Row;

        /// <summary>
        /// 获取当前单元格的值
        /// </summary>
        public object CellVal => _item[_dot.ID];

        /// <summary>
        /// 自定义单元格UI
        /// </summary>
        public UIElement UI { get; set; }

        /// <summary>
        /// 格式串，自定义单元格UI方法的参数
        /// </summary>
        public string Format => _dot.Format;

        /// <summary>
        /// 列名(字段名)
        /// </summary>
        public string ID => _dot.ID;

        /// <summary>
        /// 当前行视图
        /// </summary>
        public ViewItem ViewItem => _item;

        /// <summary>
        /// 当前单元格的Dot
        /// </summary>
        public Dot Dot => _dot;

        /// <summary>
        /// 获取设置单元格前景画刷
        /// </summary>
        public Brush Foreground
        {
            get { return Root == null ? _dot.Foreground : Root.Foreground; }
            set { GetRoot().Foreground = value; }
        }

        /// <summary>
        /// 获取设置单元格背景画刷
        /// </summary>
        public Brush Background
        {
            get { return Root == null ? _dot.Background : Root.Background; }
            set { GetRoot().Background = value; }
        }

        /// <summary>
        /// 获取设置单元格字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return Root == null ? _dot.FontWeight : Root.FontWeight; }
            set { GetRoot().FontWeight = value; }
        }

        /// <summary>
        /// 获取设置单元格文本样式
        /// </summary>
        public FontStyle FontStyle
        {
            get { return Root == null ? _dot.FontStyle : Root.FontStyle; }
            set { GetRoot().FontStyle = value; }
        }

        /// <summary>
        /// 获取设置单元格文本大小
        /// </summary>
        public double FontSize
        {
            get { return Root == null ? _dot.FontSize : Root.FontSize; }
            set { GetRoot().FontSize = value; }
        }

        /// <summary>
        /// 获取设置单元格内边距
        /// </summary>
        public Thickness Padding
        {
            get { return Root == null ? _dot.Padding : Root.Padding; }
            set { GetRoot().Padding = value; }
        }

        /// <summary>
        /// 获取设置单元格边距
        /// </summary>
        public Thickness Margin
        {
            get { return Root == null ? _dot.Margin : Root.Margin; }
            set { GetRoot().Margin = value; }
        }

        /// <summary>
        /// 以默认方式生成单元格UI
        /// </summary>
        /// <returns></returns>
        public UIElement CreateDefaultUI()
        {
            return _item.CreateDefaultUI(_dot);
        }

        /// <summary>
        /// 承载样式的根元素，设置样式时自动创建
        /// </summary>
        internal ContentPresenter Root { get; set; }

        ContentPresenter GetRoot()
        {
            if (Root == null)
                Root = new ContentPresenter();
            return Root;
        }
    }
}

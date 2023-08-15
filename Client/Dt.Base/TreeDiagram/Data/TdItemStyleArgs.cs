#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 设置行样式参数
    /// </summary>
    public class TdItemStyleArgs
    {
        TdItem _item;

        internal TdItemStyleArgs(TdItem p_item)
        {
            _item = p_item;
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
        /// 获取子级节点集合
        /// </summary>
        public List<TdItem> Children => _item.Children;

        /// <summary>
        /// 获取设置行前景画刷
        /// </summary>
        public SolidColorBrush Foreground
        {
            get { return _item.Foreground; }
            set { _item.Foreground = value; }
        }

        /// <summary>
        /// 获取设置行背景画刷
        /// </summary>
        public SolidColorBrush Background
        {
            get { return _item.Background; }
            set { _item.Background = value; }
        }

        /// <summary>
        /// 获取设置行字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return _item.FontWeight; }
            set { _item.FontWeight = value; }
        }

        /// <summary>
        /// 获取设置行文本样式
        /// </summary>
        public FontStyle FontStyle
        {
            get { return _item.FontStyle; }
            set { _item.FontStyle = value; }
        }

        /// <summary>
        /// 获取设置行文本大小
        /// </summary>
        public double FontSize
        {
            get { return _item.FontSize; }
            set { _item.FontSize = value; }
        }
    }
}

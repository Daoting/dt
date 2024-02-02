#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单击行事件参数
    /// </summary>
    public class ItemClickArgs : EventArgs
    {
        public ItemClickArgs(object p_data, object p_oldData)
        {
            Data = p_data;
            OldData = p_oldData;
        }

        /// <summary>
        /// 当前点击行是否和上次点击行相同
        /// </summary>
        public bool IsChanged
        {
            get { return Data != OldData; }
        }

        /// <summary>
        /// 当前点击行
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// 上次点击行
        /// </summary>
        public object OldData { get; }

        /// <summary>
        /// 当前点击的Row
        /// </summary>
        public Row Row
        {
            get { return Data as Row; }
        }
    }

    /// <summary>
    /// 设置行样式参数
    /// </summary>
    public class ItemStyleArgs
    {
        ViewItem _item;

        internal ItemStyleArgs(ViewItem p_item)
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

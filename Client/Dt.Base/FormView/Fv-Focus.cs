#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 焦点处理
    /// </summary>
    public partial class Fv
    {
        public readonly static DependencyProperty AutoFocusProperty = DependencyProperty.Register(
            "AutoFocus",
            typeof(bool),
            typeof(Fv),
            new PropertyMetadata(false));

        /// <summary>
        /// 获取设置切换数据源时是否自动获得焦点，默认false
        /// </summary>
        public bool AutoFocus
        {
            get { return (bool)GetValue(AutoFocusProperty); }
            set { SetValue(AutoFocusProperty, value); }
        }

        /// <summary>
        /// 自动跳入第一个可接收焦点的列
        /// </summary>
        public void GotoFirstCell()
        {
            foreach (var item in _panel.Children)
            {
                FvCell cell = item as FvCell;
                if (cell != null && cell.ReceiveFocus())
                    return;
            }
        }

        /// <summary>
        /// 最后一个非只读单元格获得焦点
        /// </summary>
        public void GotoLastCell()
        {
            for (int i = _panel.Children.Count - 1; i >= 0; i--)
            {
                FvCell cell = _panel.Children[i] as FvCell;
                if (cell != null && cell.ReceiveFocus())
                    return;
            }
        }

        /// <summary>
        /// 跳到指定单元格
        /// </summary>
        /// <param name="p_cellName">要跳到的格名称</param>
        /// <returns>true 跳成功</returns>
        public void GotoCell(string p_cellName)
        {
            FvCell cell = this[p_cellName];
            if (cell != null)
            {
                if (!cell.ReceiveFocus())
                    Kit.Msg(string.Format("要跳入的单元格({0})无法获得焦点！", p_cellName));
            }
            else
            {
                Kit.Msg(string.Format("未找到要跳入的单元格({0})！", p_cellName));
            }
        }

        /// <summary>
        /// 移向下一编辑器
        /// </summary>
        /// <param name="p_cell">当前格</param>
        internal void GotoNextCell(FvCell p_cell)
        {
            int index = _panel.Children.IndexOf(p_cell);
            if (index == -1)
                return;

            int preIndex = index;
            while (true)
            {
                index++;
                if (index >= _panel.Children.Count)
                    index = 0;

                // 避免只一个可编辑格
                if (index == preIndex)
                    break;

                FvCell cell = _panel.Children[index] as FvCell;
                if (cell != null && cell.ReceiveFocus())
                    break;
            }
        }

        /// <summary>
        /// 移向上一编辑器
        /// </summary>
        /// <param name="p_cell">当前格</param>
        internal void GotoPreviousCell(FvCell p_cell)
        {
            int index = _panel.Children.IndexOf(p_cell);
            if (index == -1)
                return;

            while (true)
            {
                index--;
                if (index < 0)
                    break;

                FvCell cell = _panel.Children[index] as FvCell;
                if (cell != null && cell.ReceiveFocus())
                    break;
            }
        }

        /// <summary>
        /// 是否为末尾的单元格
        /// </summary>
        /// <param name="p_cell"></param>
        /// <returns></returns>
        internal bool IsLastCell(FvCell p_cell)
        {
            return Items[Items.Count - 1] == p_cell;
        }

        /// <summary>
        /// 处理在末尾单元格按回车事件
        /// </summary>
        /// <returns></returns>
        internal bool DoLastCellEnter()
        {
            if (OnLastCellEnter())
                return true;

            if (LastCellEnter != null)
            {
                LastCellEnter();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 处理在末尾单元格按回车
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnLastCellEnter()
        {
            return false;
        }
    }
}
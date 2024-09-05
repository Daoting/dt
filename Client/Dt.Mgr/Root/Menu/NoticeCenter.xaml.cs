#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Home
{
    public partial class NoticeCenter : Tab
    {
        public NoticeCenter()
        {
            InitializeComponent();
            _lv.Data = Notice.Data;
        }

        void OnItemClick(ItemClickArgs e)
        {
            if (e.Row.Tag is NotifyInfo ni
                && ni.LinkCallback != null)
            {
                ni.LinkCallback.Invoke(ni);
                Notice.Data.Remove(e.Row);
            }
        }

        void OnClear()
        {
            Notice.Data.Clear();
        }

        void OnCloseItem(object sender, RoutedEventArgs e)
        {
            Notice.Data.Remove(((LvItem)((Button)sender).DataContext).Row);
        }
    }
}
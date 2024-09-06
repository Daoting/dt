#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    public partial class ShareTabWin : Win
    {
        readonly List _list;
        readonly Form _form;
        
        public ShareTabWin()
        {
            InitializeComponent();
            _list = Kit.GetShareObj<List>("基础List");
            Items.Add(_list);
            _form = Kit.GetShareObj<Form>("Crud基础Form");
            
            _list.Msg += e => _ = _form.Query(e);
            _form.UpdateList += e => _ = _list.Refresh(e.ID);

            _list.Loaded += OnListLoaded;
        }

        void OnListLoaded(object sender, RoutedEventArgs e)
        {
            _list.Loaded -= OnListLoaded;
            _ = _list.Refresh();
        }
    }
}
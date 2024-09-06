#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    [View("物资目录管理")]
    public partial class 物资目录Win : Win
    {
        readonly 物资分类Form _form分类;
        readonly 物资目录Form _form;

        public 物资目录Win()
        {
            InitializeComponent();
            _form分类 = new 物资分类Form { OwnWin = this };
            _form = new 物资目录Form { OwnWin = this };
            Attach();
        }

        void Attach()
        {
            _list分类.Msg += e => _ = _form分类.Query(e);
            _form分类.UpdateList += e => _ = _list分类.Refresh(e.ID);
            _form分类.UpdateRelated += e => _list.Query(e.ID);

            _query.Query += e =>
            {
                _list.Query(e);
                NaviTo(_list.Title);
            };
            _list.Msg += e => _ = _form.Query(e);
            _form.UpdateList += e => _ = _list.Refresh(e.ID);

            _list分类.Query(0);
        }
    }
}
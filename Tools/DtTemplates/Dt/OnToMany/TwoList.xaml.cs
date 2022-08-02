#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace $rootnamespace$
{
    public partial class $maincls$List : Mv
    {
        public $maincls$List()
        {
            InitializeComponent();
        }

        public void Update()
        {
            Query();
        }

        protected override void OnInit(object p_params)
        {
            Query();
        }

        void OnAdd(object sender, Mi e)
        {
            ShowForm(-1);
        }

        void OnEdit(object sender, Mi e)
        {
            ShowForm(e.Data.To<$maincls$Obj>().ID);
        }

        async void ShowForm(long p_id)
        {
            var form = new $maincls$Form();
            form.Update(p_id);
            if (await Forward<bool>(form, null, true))
                Query();
        }

        async void OnDel(object sender, Mi e)
        {
            var d = e.Data.To<$maincls$Obj>();
            if (d == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (await $agent$.Delete(d))
                Query();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            NaviTo(new List<Mv> { $navitolist$ });
            if (!e.IsChanged)
                return;

            var p_id = e.Row.ID;
$relatedupdate$
        }

        void OnDataChanged(object sender, INotifyList e)
        {
$relatedclear$
        }

$listsearchcs$

        $maincls$Win _win => ($maincls$Win)_tab.OwnWin;
    }
}
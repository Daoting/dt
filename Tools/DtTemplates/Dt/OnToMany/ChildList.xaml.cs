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
    public sealed partial class $maincls$$childcls$List : Mv
    {
        long _id;

        public $maincls$$childcls$List()
        {
            InitializeComponent();
        }

        public void Update(long p_id)
        {
            _id = p_id;
            Menu["添加"].IsEnabled = true;
            Query();
        }

        public void Clear()
        {
            _id = -1;
            Menu["添加"].IsEnabled = false;
            _lv.Data = null;
        }

        async void Query()
        {
            _lv.Data = await $agent$.Query<$childcls$Obj>("$maintitle$-关联$childtitle$", new { ParentID = _id });
        }

        void OnAdd(object sender, Mi e)
        {
            ShowForm(-1);
        }

        void OnEdit(object sender, Mi e)
        {
            ShowForm(e.Data.To<$childcls$Obj>().ID);
        }

        async void ShowForm(long p_id)
        {
            var form = new $maincls$$childcls$Form();
            form.Update(p_id, _id);
            if (await Forward<bool>(form, null, true))
                Query();
        }

        async void OnDel(object sender, Mi e)
        {
            var d = e.Data.To<$childcls$Obj>();
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

        $maincls$Win _win => ($maincls$Win)_tab.OwnWin;
    }
}

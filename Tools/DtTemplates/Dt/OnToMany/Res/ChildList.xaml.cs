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
    public partial class $parentroot$$childroot$List : Tab
    {
        long _parentID;

        public $parentroot$$childroot$List()
        {
            InitializeComponent();
        }

        public void Update(long p_parentID)
        {
            _parentID = p_parentID;
            Menu["增加"].IsEnabled = _parentID > 0;
            Refresh();
        }

        public async void Refresh()
        {
            if (_parentID > 0)
            {
                _lv.Data = await $entity$.Query("$parentidname$=@ParentID", new Dict { { "ParentID", _parentID.ToString() } });
            }
            else
            {
                _lv.Data = null;
            }
        }

        void OnAdd(object sender, Mi e)
        {
            ShowForm(-1);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            ShowForm(e.Row.ID);
        }

        async void ShowForm(long p_id)
        {
            //var form = new $maincls$$childcls$Form();
            //form.Update(p_id, _id);
            //if (await Forward<bool>(form, null, true))
            //    Query();
        }

        #region 视图
        private void OnListSelected(object sender, EventArgs e)
        {
            _lv?.ChangeView(Resources["ListView"], ViewMode.List);
        }

        private void OnTableSelected(object sender, EventArgs e)
        {
            _lv?.ChangeView(Resources["TableView"], ViewMode.Table);
        }

        private void OnTileSelected(object sender, EventArgs e)
        {
            _lv?.ChangeView(Resources["TileView"], ViewMode.Tile);
        }
        #endregion

        $parentroot$Win _win => ($parentroot$Win)OwnWin;
    }
}
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-11-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 搜索文件
    /// </summary>
    public sealed partial class SelectSearchPage : Tab
    {
        readonly SelectFileDlg _owner;

        public SelectSearchPage(SelectFileDlg p_owner)
        {
            InitializeComponent();

            _owner = p_owner;
            if (_owner.IsMultiSelection)
                _lv.SelectionMode = Base.SelectionMode.Multiple;
        }

        async void OnSearch(string e)
        {
            _lv.Data = await FileDs.SearchFiles(e, _owner.TypeFilter);
        }

        void OnSelect(Mi e)
        {
            if (_lv.SelectedCount == 0)
                return;

            List<string> ls = new List<string>();
            foreach (var row in _lv.SelectedRows)
            {
                string info = row.Str("Info");
                if (info.Length > 2)
                    ls.Add(info.Substring(1, info.Length - 2));
            }
            _owner.SelectedFiles = ls;
            _owner.Close(true);
        }
    }
}

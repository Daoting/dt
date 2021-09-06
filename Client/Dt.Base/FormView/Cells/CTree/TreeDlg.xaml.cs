#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.FormView
{
    public partial class TreeDlg : Dlg
    {
        CTree _owner;
        string[] _srcIDs;
        string[] _tgtIDs;

        public TreeDlg(CTree p_owner)
        {
            InitializeComponent();
            Title = "选择";
            _owner = p_owner;
            Content = _owner.TreeView;
        }

        public async void ShowDlg()
        {
            TreeView tv = _owner.TreeView;

            tv.ItemClick -= OnSingleClick;
            if (tv.SelectionMode == SelectionMode.Multiple)
            {
                _menu.Show("确定");
            }
            else
            {
                _menu.Hide("确定");
                tv.ItemClick += OnSingleClick;
            }

            // 拆分填充列
            if (!string.IsNullOrEmpty(_owner.SrcID) && !string.IsNullOrEmpty(_owner.TgtID))
            {
                _srcIDs = _owner.SrcID.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                _tgtIDs = _owner.TgtID.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                if (_srcIDs.Length != _tgtIDs.Length)
                {
                    _srcIDs = null;
                    _tgtIDs = null;
                    Kit.Error("数据填充：源列表、目标列表列个数不一致！");
                }
            }

            // 第一次或动态加载数据源时
            if (tv.Data == null || _owner.RefreshData)
                await _owner.OnLoadData();

            if (tv.View == null)
                tv.View = (tv.Data is Table) ? Application.Current.Resources["CListRowView"] : Application.Current.Resources["CListObjView"];

            // 不向下层对话框传递Press事件
            AllowRelayPress = false;
            Show();
        }

        /// <summary>
        /// 单选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSingleClick(object sender, ItemClickArgs e)
        {
            if (e.Data is Row srcRow)
            {
                _owner.Text = srcRow.Str("name");
                if (_srcIDs != null)
                {
                    // 同步填充
                    object tgtObj = _owner.Owner.Data;
                    Row tgtRow = tgtObj as Row;
                    for (int i = 0; i < _srcIDs.Length; i++)
                    {
                        string srcID = _srcIDs[i];
                        string tgtID = _tgtIDs[i];
                        if (srcRow.Contains(srcID))
                        {
                            if (tgtRow != null)
                            {
                                if (tgtRow.Contains(tgtID))
                                    tgtRow[tgtID] = srcRow[srcID];
                            }
                            else
                            {
                                var pi = tgtObj.GetType().GetProperty(tgtID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                if (pi != null)
                                    pi.SetValue(tgtObj, srcRow[srcID]);
                            }
                        }
                    }
                }
            }
            else
            {
                _owner.Text = e.Data.ToString();
                if (_srcIDs != null)
                {
                    // 同步填充
                    object tgtObj = _owner.Owner.Data;
                    Row tgtRow = tgtObj as Row;
                    for (int i = 0; i < _srcIDs.Length; i++)
                    {
                        var srcPi = e.Data.GetType().GetProperty(_srcIDs[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (srcPi != null)
                        {
                            string tgtID = _tgtIDs[i];
                            if (tgtRow != null)
                            {
                                if (tgtRow.Contains(tgtID))
                                    tgtRow[tgtID] = srcPi.GetValue(e.Data);
                            }
                            else
                            {
                                var tgtPi = tgtObj.GetType().GetProperty(tgtID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                if (tgtPi != null)
                                    tgtPi.SetValue(tgtObj, srcPi.GetValue(e.Data));
                            }
                        }
                    }
                }
            }
            Close();
            _owner.OnSelected(e.Data);
        }

        /// <summary>
        /// 多选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMultipleOK(object sender, Mi e)
        {
            // 暂未实现同步填充！
            List<object> ls = new List<object>();
            StringBuilder sb = new StringBuilder();
            if (_owner.TreeView.Data is Table tbl)
            {
                foreach (var row in _owner.TreeView.SelectedItems.Cast<Row>())
                {
                    if (sb.Length > 0)
                        sb.Append("#");
                    sb.Append(row.Str("name"));
                    ls.Add(row);
                }
            }
            else
            {
                foreach (var obj in _owner.TreeView.SelectedItems)
                {
                    if (sb.Length > 0)
                        sb.Append("#");
                    sb.Append(obj.ToString());
                    ls.Add(obj);
                }
            }
            _owner.Text = sb.ToString();
            Close();
            _owner.OnSelected(ls);
        }

        void OnClear(object sender, Mi e)
        {
            _owner.Text = null;
            if (_tgtIDs != null)
            {
                object tgtObj = _owner.Owner.Data;
                Row tgtRow = tgtObj as Row;
                foreach (var tgtID in _tgtIDs)
                {
                    if (tgtRow != null)
                    {
                        tgtRow[tgtID] = null;
                    }
                    else
                    {
                        var tgtPi = tgtObj.GetType().GetProperty(tgtID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (tgtPi != null)
                            tgtPi.SetValue(tgtObj, null);
                    }
                }
            }
            Close();
            _owner.OnSelected(null);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            // 回车跳下一格
            if (e.Key == VirtualKey.Enter)
            {
                _owner.Owner.GotoNextCell(_owner);
                e.Handled = true;
                if (IsOpened)
                    Close();
            }
        }
    }
}

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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base.FormView
{
    public partial class TreeDlg : Dlg
    {
        const string _error = " 单元格填充时源列表、目标列表列个数不一致！";
        CTree _owner;
        string[] _srcIDs;
        string[] _tgtIDs;

        public TreeDlg(CTree p_owner)
        {
            InitializeComponent();
            Title = "选择";
            _owner = p_owner;
            Content = _owner.Tv;
        }

        public async void ShowDlg()
        {
            Tv tv = _owner.Tv;

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

            // 第一次或动态加载数据源时
            if (tv.Data != null && !_owner.RefreshData)
            {
                // 已设置数据源
            }
            else if (await _owner.OnLoadData())
            {
                // 外部自定义数据源
            }
            else if (_owner.Sql != null)
            {
                // Sql属性定义的数据源
                tv.Data = await _owner.Sql.GetData(_owner.Owner.Data, null);
            }

            if (tv.Data == null)
                Throw.Msg("数据源为空！");
            
            // 拆分填充列
            if (_srcIDs == null)
            {
                if (!string.IsNullOrEmpty(_owner.SrcID))
                {
                    _srcIDs = _owner.SrcID.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);

                    if (!string.IsNullOrEmpty(_owner.TgtID))
                    {
                        _tgtIDs = _owner.TgtID.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else if (_srcIDs.Length == 1)
                    {
                        // 默认填充当前列
                        _tgtIDs = new string[] { _owner.ID };
                    }
                    else
                    {
                        _srcIDs = null;
                        Throw.Msg(_owner.ID + _error);
                    }

                    if (_srcIDs.Length != _tgtIDs.Length)
                    {
                        _srcIDs = null;
                        _tgtIDs = null;
                        Throw.Msg(_owner.ID + _error);
                    }
                }
                else
                {
                    if (tv.Data is Table tbl)
                    {
                        // 取name列 或 第一列作为源列名
                        _srcIDs = new string[] { tbl.Columns.Contains("name") ? "name" : tbl.Columns[0].ID };
                        _tgtIDs = new string[] { _owner.ID };
                    }
                    else if (tv.Data.GetTreeRoot().FirstOrDefault() != null
                        && tv.Data.GetTreeRoot().First().GetType().GetProperty("name", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) is PropertyInfo pi)
                    {
                        _srcIDs = new string[] { "name" };
                        _tgtIDs = new string[] { _owner.ID };
                    }
                    else
                    {
                        Throw.Msg(_owner.ID + " 单元格未设置源、目标填充列！");
                    }
                }
            }

            if (tv.View == null)
            {
                string xaml;
                if (tv.Data is Table tbl)
                {
                    var colName = tbl.Columns.Contains("name") ? "name" : tbl.Columns[0].ID;
                    xaml = $"<DataTemplate><a:Dot ID=\"{colName}\" Margin=\"10,0,10,0\" /></DataTemplate>";
                }
                else
                {
                    xaml = "<DataTemplate><a:Dot Margin=\"10,0,10,0\" /></DataTemplate>";
                }
                tv.View = Kit.LoadXaml<DataTemplate>(xaml);
            }

            // 不向下层对话框传递Press事件
            AllowRelayPress = false;
            Show();
        }

        /// <summary>
        /// 单选
        /// </summary>
        /// <param name="e"></param>
        void OnSingleClick(ItemClickArgs e)
        {
            if (_srcIDs != null)
            {
                // 同步填充
                if (e.Data is Row srcRow)
                {
                    object tgtObj = _owner.Owner.Data;
                    Row tgtRow = tgtObj as Row;
                    for (int i = 0; i < _srcIDs.Length; i++)
                    {
                        string srcID = _srcIDs[i];
                        // - 代表当前列值
                        string tgtID = _tgtIDs[i] == "-" ? _owner.ID : _tgtIDs[i];
                        
                        if (srcRow.Contains(srcID) || srcID == "-")
                        {
                            object tgtVal = null;
                            if (srcRow.Contains(srcID))
                                tgtVal = srcRow[srcID];

                            if (tgtRow != null)
                            {
                                if (tgtRow.Contains(tgtID))
                                    tgtRow[tgtID] = tgtVal;
                            }
                            else
                            {
                                var pi = tgtObj.GetType().GetProperty(tgtID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                if (pi != null)
                                    pi.SetValue(tgtObj, tgtVal);
                            }

                            // 对于联动的CList CPick CTree，清空其下拉数据源
                            if (srcID == "-")
                            {
                                var cell = _owner.Owner[tgtID];
                                if (cell is CList cl)
                                    cl.Data = null;
                                else if (cell is CPick cp)
                                    cp.Data = null;
                                else if (cell is CTree ct)
                                    ct.Data = null;
                            }
                        }
                    }
                }
                else
                {
                    object tgtObj = _owner.Owner.Data;
                    Row tgtRow = tgtObj as Row;
                    for (int i = 0; i < _srcIDs.Length; i++)
                    {
                        string srcID = _srcIDs[i];
                        PropertyInfo srcPi = null;
                        if (srcID == "-"
                            || (srcPi = e.Data.GetType().GetProperty(srcID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)) != null)
                        {
                            string tgtID = _tgtIDs[i] == "-" ? _owner.ID : _tgtIDs[i];
                            object tgtVal = srcPi == null ? null : srcPi.GetValue(e.Data);

                            if (tgtRow != null)
                            {
                                if (tgtRow.Contains(tgtID))
                                    tgtRow[tgtID] = tgtVal;
                            }
                            else
                            {
                                var tgtPi = tgtObj.GetType().GetProperty(tgtID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                if (tgtPi != null)
                                    tgtPi.SetValue(tgtObj, tgtVal);
                            }

                            // 对于联动的CList CPick CTree，清空其下拉数据源
                            if (srcID == "-")
                            {
                                var cell = _owner.Owner[tgtID];
                                if (cell is CList cl)
                                    cl.Data = null;
                                else if (cell is CPick cp)
                                    cp.Data = null;
                                else if (cell is CTree ct)
                                    ct.Data = null;
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
        /// <param name="e"></param>
        void OnMultipleOK(Mi e)
        {
            List<object> ls = new List<object>();
            if (_srcIDs != null)
            {
                // 暂未实现同步填充！
                StringBuilder sb = new StringBuilder();
                if (_owner.Tv.Data is Table tbl)
                {
                    foreach (var row in _owner.Tv.SelectedItems.Cast<Row>())
                    {
                        if (sb.Length > 0)
                            sb.Append("#");
                        sb.Append(row.Str(_srcIDs[0]));
                        ls.Add(row);
                    }
                }
                else
                {
                    foreach (var obj in _owner.Tv.SelectedItems)
                    {
                        if (sb.Length > 0)
                            sb.Append("#");
                        sb.Append(obj.ToString());
                        ls.Add(obj);
                    }
                }
                _owner.Text = sb.ToString();
            }
            Close();
            _owner.OnSelected(ls);
        }

        void OnClear(Mi e)
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

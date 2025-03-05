#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using System.Reflection;
using System.Text;
using Windows.System;
#endregion

namespace Dt.Base
{
    public partial class ListDlg : Dlg
    {
        const string _error = " 单元格填充时源列表、目标列表列个数不一致！";
        CList _owner;
        string[] _srcIDs;
        string[] _tgtIDs;
        CListEx _ex;

        public ListDlg(CList p_owner)
        {
            Title = "选择";
            _owner = p_owner;
            LoadLv();
            InitEx();

            if (!Kit.IsPhoneUI)
            {
                HideTitleBar = Menu == null;
            }
        }

        public async void ShowDlg()
        {
            Lv lv = _owner.Lv;
            if (lv.Data != null && !_owner.RefreshData)
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
                lv.Data = await _owner.Sql.GetData(_owner.Owner.Data, null);
            }
            else if (_ex != null)
            {
                // 功能扩展提供的数据源
                lv.Data = await _ex.GetData();
            }
            else if (_owner.Items.Count > 0)
            {
                // xaml中定义的对象列表
                lv.Data = _owner.Items;
            }
            else if (_owner.ValBinding.ConverterParameter is Type tp)
            {
                // 支持可null的枚举类型
                if (tp.IsGenericType && tp.GetGenericTypeDefinition() == typeof(Nullable<>))
                    tp = tp.GetGenericArguments()[0];

                // 枚举类型
                if (tp.IsEnum)
                    lv.Data = CreateEnumData(tp);
            }

            // 未设置源、目标填充列时，设置默认
            if (_srcIDs == null)
            {
                if (lv.Data is Table tbl)
                {
                    // 取name列 或 第一列作为源列名
                    _srcIDs = new string[] { tbl.Columns.Contains("name") ? "name" : tbl.Columns[0].ID };
                    _tgtIDs = new string[] { _owner.ID };
                }

                // 数据源为普通对象 且 未设置SrcIDs时，源为对象本身
                //else if (lv.Data.Count > 0)
                //{
                //    if (lv.Data[0].GetType().GetProperty("name", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) is PropertyInfo pi)
                //    {
                //        _srcIDs = new string[] { "name" };
                //        _tgtIDs = new string[] { _owner.ID };
                //    }
                //    // 无name属性时，源为对象本身
                //}
                //else
                //{
                //    Throw.Msg(_owner.ID + " 单元格未设置源、目标填充列！");
                //}
            }

            if (lv.View == null)
            {
                lv.ShowItemBorder = false;
                string xaml;
                if (lv.Data is Table tbl)
                {
                    var colName = tbl.Columns.Contains("name") ? "name" : tbl.Columns[0].ID;
                    xaml = $"<DataTemplate><a:Dot ID=\"{colName}\" Margin=\"10,0,10,0\" /></DataTemplate>";
                }
                else
                {
                    xaml = "<DataTemplate><a:Dot Margin=\"10,0,10,0\" /></DataTemplate>";
                }
                lv.View = Kit.LoadXaml<DataTemplate>(xaml);
            }

            // 不向下层对话框传递Press事件
            AllowRelayPress = false;

            // phone模式先最大化
            if (Kit.IsPhoneUI)
            {
                ClearValue(HideTitleBarProperty);
                ClearValue(PhonePlacementProperty);
                ClearValue(HeightProperty);
            }
            Show();

            // phone模式选项内容不足半屏时在下部显示
            if (Kit.IsPhoneUI && DesiredSize.Height * 2 < Kit.ViewHeight)
            {
                //HideTitleBar = true;
                Top = Kit.ViewHeight - DesiredSize.Height;
                // uno中不可设置为固定高度！
                Height = double.NaN;
            }

            if (lv.Data != null && lv.Data.Count > 0)
            {
                lv.SelectedIndex = 0;
                if (lv.Data.Count > 4
                    && _owner.FilterCfg == null)
                {
                    // 默认显示过滤框
                    lv.FilterCfg = new FilterCfg
                    {
                        EnablePinYin = true,
                        IsRealtime = true,
                        Placeholder = "拼音简码或包含的文字",
                    };
                }
                lv.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
            }
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
                            else if (_owner.Owner[tgtID] is FvCell fc && fc.ValBinding.Source is PropertyView pv)
                            {
                                // 通过 PropertyView 赋值确保 UI 同步更新
                                pv.Val = tgtVal;
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
            else if (e.Data is not Row)
            {
                // 未设置源ID时，源为对象本身
                _owner.Value = e.Data;
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
                if (_owner.Lv.Data is Table tbl)
                {
                    foreach (var row in _owner.Lv.SelectedItems.Cast<Row>())
                    {
                        if (sb.Length > 0)
                            sb.Append("#");
                        sb.Append(row.Str(_srcIDs[0]));
                        ls.Add(row);
                    }
                }
                else
                {
                    foreach (var obj in _owner.Lv.SelectedItems)
                    {
                        if (sb.Length > 0)
                            sb.Append("#");
                        sb.Append(obj.ToString());
                        ls.Add(obj);
                    }
                }
                _owner.Value = sb.ToString();
            }

            Close();
            _owner.OnSelected(ls);
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

        /// <summary>
        /// 创建枚举类型数据源
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        internal Nl<object> CreateEnumData(Type p_type)
        {
            Nl<object> ls = new Nl<object>();
            foreach (var fi in from f in p_type.GetRuntimeFields()
                               where f.IsLiteral
                               select f)
            {
                ls.Add(fi.GetValue(p_type));
            }
            return ls;
        }

        void LoadLv()
        {
            Lv lv = _owner.Lv;
            Content = lv;

            if (lv.SelectionMode == SelectionMode.Multiple)
            {
                Mi mi = new Mi { ID = "确定", Icon = Icons.保存 };
                mi.Click += OnMultipleOK;
                Menu menu = new Menu();
                menu.Items.Add(mi);
                Menu = menu;
            }
            else
            {
                lv.ItemClick += OnSingleClick;
            }

            // 拆分填充列
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
        }

        void InitEx()
        {
            if (string.IsNullOrEmpty(_owner.Ex))
                return;

            string cls, pars = null;
            int index = _owner.Ex.IndexOf("#");
            if (index != -1)
            {
                cls = _owner.Ex.Substring(0, index).Trim();
                pars = _owner.Ex.Substring(index + 1).Trim();
            }
            else
            {
                cls = _owner.Ex.Trim();
            }
            var tp = Kit.GetTypeByAlias(typeof(CListExAttribute), cls);
            if (tp != null && tp.IsSubclassOf(typeof(CListEx)))
            {
                _ex = Activator.CreateInstance(tp) as CListEx;
                _ex.Init(_owner, this, pars);
            }
            else
            {
                Kit.Warn("未找到CList.Ex：" + _owner.Ex);
            }
        }
    }
}

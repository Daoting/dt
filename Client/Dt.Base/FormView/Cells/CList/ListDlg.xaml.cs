#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Input;
using System.Reflection;
using System.Text;
using Windows.System;
#endregion

namespace Dt.Base
{
    public partial class ListDlg : Dlg
    {
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
            else if (_ex != null)
            {
                // 功能扩展提供的数据源
                lv.Data = await _ex.GetData();
            }
            else if (((Type)_owner.ValBinding.ConverterParameter).IsEnum)
            {
                // 枚举类型
                lv.Data = CreateEnumData((Type)_owner.ValBinding.ConverterParameter);
            }
            else if (_owner.Items.Count > 0)
            {
                // xaml中定义的对象列表
                lv.Data = _owner.Items;
            }

            if (lv.View == null)
            {
                lv.ShowItemBorder = false;
                lv.View = (lv.Data is Table) ? Res.FormRes["CListRowView"] : Res.FormRes["CListObjView"];
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
                _owner.Value = srcRow["name"];
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
                _owner.Value = e.Data;
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
            if (_owner.Lv.Data is Table tbl)
            {
                foreach (var row in _owner.Lv.SelectedItems.Cast<Row>())
                {
                    if (sb.Length > 0)
                        sb.Append("#");
                    sb.Append(row.Str("name"));
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
        }

        void InitEx()
        {
            if (string.IsNullOrEmpty(_owner.Ex))
                return;

            string cls, pars = null;
            int index = _owner.Ex.IndexOf("#");
            if (index != -1)
            {
                cls = _owner.Ex.Substring(0, index);
                pars = _owner.Ex.Substring(index + 1);
            }
            else
            {
                cls = _owner.Ex;
            }
            var tp = Kit.GetAllTypesByAlias(typeof(CListExAttribute), cls).FirstOrDefault();
            if (tp != null && tp.IsSubclassOf(typeof(CListEx)))
            {
                _ex = Activator.CreateInstance(tp) as CListEx;
                _ex.Init(_owner, this, pars);
            }
        }
    }
}

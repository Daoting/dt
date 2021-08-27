#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.FormView
{
    public partial class ListDlg : Dlg
    {
        CList _owner;
        string[] _srcIDs;
        string[] _tgtIDs;

        public ListDlg(CList p_owner)
        {
            InitializeComponent();
            Title = "选择";
            _owner = p_owner;
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
            else if (!string.IsNullOrEmpty(_owner.Option))
            {
                // 基础选项
                lv.Data = AtModel.Query($"select name from OmOption where Category=\"{_owner.Option}\"");
            }
            else if (!string.IsNullOrEmpty(_owner.Sql))
            {
                // Sql查询数据
                lv.Data = await GetDataBySql(_owner.Sql);
            }
            else if (!string.IsNullOrEmpty(_owner.SqlKey))
            {
                // Sql键值查询
                lv.Data = await GetDataByKey(_owner.SqlKey, _owner.SqlKeyFilter);
            }
            else if (!string.IsNullOrEmpty(_owner.Enum))
            {
                // 枚举数据
                Type type = Type.GetType(_owner.Enum, true, true);
                lv.Data = CreateEnumData(type);
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
                lv.View = (lv.Data is Table) ? Application.Current.Resources["CListRowView"] : Application.Current.Resources["CListObjView"];
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
        Nl<object> CreateEnumData(Type p_type)
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

        /// <summary>
        /// 根据sql、过滤条件获取数据源，sql必须含有服务名前缀，如：
        /// Cm:select * from dt_log
        /// local:select * from letter
        /// </summary>
        /// <param name="p_sql">带前缀的sql</param>
        /// <param name="p_filter"></param>
        /// <returns></returns>
        static async Task<Table> GetDataBySql(string p_sql, string p_filter = null)
        {
            if (string.IsNullOrEmpty(p_sql))
                return null;

            string[] info = p_sql.Trim().Split(':');
            if (info.Length != 2 || string.IsNullOrEmpty(info[0]) || string.IsNullOrEmpty(info[1]))
                throw new Exception("Sql格式不正确！" + p_sql);

            Table data;
            string sql = string.IsNullOrEmpty(p_filter) ? info[1] : $"select * from ({info[1]}) a where {p_filter}";
            if (info[0].ToLower() == "local")
                data = AtState.Query(sql);
            else
                data = await new UnaryRpc(info[0], "Da.Query", sql, null).Call<Table>();
            return data;
        }

        /// <summary>
        /// 根据Sql语句键值、过滤条件获取数据源，键值必须含有服务名前缀
        /// </summary>
        /// <param name="p_key">带服务名前缀的键</param>
        /// <param name="p_filter"></param>
        /// <returns></returns>
        static Task<Table> GetDataByKey(string p_key, string p_filter = null)
        {
            if (string.IsNullOrEmpty(p_key))
                return null;

            string[] info = p_key.Trim().Split(':');
            if (info.Length != 2 || string.IsNullOrEmpty(info[0]) || string.IsNullOrEmpty(info[1]))
                throw new Exception("Key格式不正确！" + p_key);
            return new UnaryRpc(info[0], "Da.GetDataByKey", info[1], p_filter).Call<Table>();
        }
    }
}

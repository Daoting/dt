#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class DataSourceDlg : Dlg
    {
        RptText _item;

        public DataSourceDlg()
        {
            InitializeComponent();
            _dataSets.SelectionChanged += OnSelctionChanged;
        }

        internal async Task<bool> Show(FrameworkElement p_target, RptText p_item)
        {
            _item = p_item;
            if (_dataSets.Items != null && _dataSets.Items.Count > 0)
                _dataSets.Items.Clear();

            Table dataSet = p_item.Root.Data.DataSet;
            if (dataSet.Count > 0)
            {
                foreach (var r in dataSet)
                {
                    var temp = new ComboBoxItem();
                    temp.Content = r.Str("name");
                    _dataSets.Items.Add(temp);
                }

                if (_dataSets.Items.Count == 1)
                {
                    // 只一项时默认选中，并触发selectionChanged事件，加载其字段集填充显示字段Lv
                    _dataSets.SelectedIndex = 0;
                }
                else
                {
                    // 默认选中单元格所在对象的数据集
                    RptItem topItem = null;
                    foreach (RptItem item in p_item.Part.Items)
                    {
                        if (item.Row <= p_item.Row
                            && item.Row + item.RowSpan - 1 >= p_item.Row
                            && item.Col <= p_item.Col
                            && item.Col + item.ColSpan - 1 >= p_item.Col)
                        {
                            topItem = item;
                            break;
                        }
                    }

                    string itemDataName;
                    if (topItem != null
                        && topItem.Data != null
                        && topItem.Data.Contains("tbl")
                        && (itemDataName = topItem.Data.Str("tbl")) != "")
                    {
                        for (int i = 0; i < _dataSets.Items.Count; i++)
                        {
                            if ((_dataSets.Items[i] as ComboBoxItem).Content as string == itemDataName)
                            {
                                _dataSets.SelectedIndex = i;
                                break;
                            }
                        }
                    }

                    if (_dataSets.SelectedIndex == -1)
                        _dataSets.SelectedIndex = 0;
                }
            }

            if (!Kit.IsPhoneUI)
            {
                WinPlacement = DlgPlacement.TargetBottomLeft;
                PlacementTarget = p_target;
                ClipElement = p_target;
                Height = 400;
                Width = 300;
            }
            return await ShowAsync();
        }

        public string GetExpression()
        {
            var row = _lv.SelectedRow;
            if (row == null)
                return null;

            string dsName = _dataSets.SelectionBoxItem as string;
            string colName = row.Str("name");
            switch (_func.SelectionBoxItem as string)
            {
                case "数据":
                    return $"Val({dsName},{colName})";
                case "合计":
                    return $"Sum({dsName},{colName})";
                case "最大":
                    return $"Max({dsName},{colName})";
                case "最小":
                    return $"Min({dsName},{colName})";
                case "平均":
                    return $"Avg({dsName},{colName})";
                case "序号":
                    return $"Index({dsName})";
                case "总数":
                    return $"Count({dsName})";
            }
            return null;
        }

        void OnSave(object sender, Mi e)
        {
            if (_lv.SelectedItem == null)
            {
                Kit.Warn("请选择列名！");
            }
            else
            {
                Close(true);
            }
        }

        void OnSelctionChanged(object sender, RoutedEventArgs e)
        {
            if (_dataSets.SelectedIndex > -1)
            {
                string dsName = (_dataSets.SelectedItem as ComboBoxItem).Content as string;
                _lv.Data = _item.Root.Data.GetColsData(dsName);
            }
            else
            {
                _lv.Data = null;
            }
        }

        void OnDoubleClick(object sender, object e)
        {
            OnSave(null, null);
        }
    }
}

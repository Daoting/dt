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
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base.ListView
{
    public sealed partial class DefFilterDlg : Dlg
    {
        Lv _owner;
        Row _data;
        Row _condition;
        Menu _valMenu;
        Menu _strMenu;

        public DefFilterDlg()
        {
            InitializeComponent();
        }

        public void ShowDlg(Lv p_owner)
        {
            _owner = p_owner;
            LoadCols();
            if (!Kit.IsPhoneUI)
            {
                MinHeight = 300;
                MaxHeight = Kit.ViewHeight * 0.8;
                Width = 350;
            }
            Show();
        }

        #region 过滤过程
        void OnFilter(object sender, Mi e)
        {
            if (_data.IsChanged)
                _owner.Filter = DoFilter;
            else
                _owner.Filter = null;
            Close();
        }

        void OnReset(object sender, Mi e)
        {
            _data.RejectChanges();
        }

        bool DoFilter(object p_obj)
        {
            bool isAnd = (bool)_cbAnd.IsChecked;
            foreach (var c in _data.Cells)
            {
                if (!c.IsChanged)
                    continue;

                bool suc;
                var con = (SearchConditionType)_condition[c.ID];
                if (c.Type == typeof(string))
                    suc = DoStringFilter(c, p_obj, con);
                else
                    suc = DoValueFilter(c, p_obj, con);

                if (!suc && isAnd)
                    return false;

                if (suc && !isAnd)
                    return true;
            }
            return isAnd;
        }

        bool DoValueFilter(Cell p_filter, object p_data, SearchConditionType p_condition)
        {
            object objVal = null;
            if (!(p_data is Row))
            {
                var pi = p_data.GetType().GetProperty(p_filter.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                objVal = pi.GetValue(p_data);
            }

            switch (p_condition)
            {
                case SearchConditionType.等于:
                    if (p_filter.Type == typeof(int))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<int>() == row.Int(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<int>() == (int)objVal;
                    }

                    if (p_filter.Type == typeof(long))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<long>() == row.Long(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<long>() == (long)objVal;
                    }

                    if (p_filter.Type == typeof(double))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<double>() == row.Double(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<double>() == (double)objVal;
                    }

                    if (p_filter.Type == typeof(DateTime))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<DateTime>().ToString("yyyy-MM-dd") == row.Date(p_filter.ID).ToString("yyyy-MM-dd");

                        if (objVal != null)
                            return p_filter.GetVal<DateTime>().ToString("yyyy-MM-dd") == ((DateTime)objVal).ToString("yyyy-MM-dd");
                    }
                    break;

                case SearchConditionType.不等于:
                    if (p_filter.Type == typeof(int))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<int>() != row.Int(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<int>() != (int)objVal;
                    }

                    if (p_filter.Type == typeof(long))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<long>() != row.Long(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<long>() != (long)objVal;
                    }

                    if (p_filter.Type == typeof(double))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<double>() != row.Double(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<double>() != (double)objVal;
                    }

                    if (p_filter.Type == typeof(DateTime))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<DateTime>().ToString("yyyy-MM-dd") != row.Date(p_filter.ID).ToString("yyyy-MM-dd");

                        if (objVal != null)
                            return p_filter.GetVal<DateTime>().ToString("yyyy-MM-dd") != ((DateTime)objVal).ToString("yyyy-MM-dd");
                    }
                    break;

                case SearchConditionType.大于:
                    if (p_filter.Type == typeof(int))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<int>() < row.Int(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<int>() < (int)objVal;
                    }

                    if (p_filter.Type == typeof(long))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<long>() < row.Long(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<long>() < (long)objVal;
                    }

                    if (p_filter.Type == typeof(double))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<double>() < row.Double(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<double>() < (double)objVal;
                    }

                    if (p_filter.Type == typeof(DateTime))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<DateTime>() < row.Date(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<DateTime>() < ((DateTime)objVal);
                    }
                    break;

                case SearchConditionType.大于等于:
                    if (p_filter.Type == typeof(int))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<int>() <= row.Int(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<int>() <= (int)objVal;
                    }

                    if (p_filter.Type == typeof(long))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<long>() <= row.Long(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<long>() <= (long)objVal;
                    }

                    if (p_filter.Type == typeof(double))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<double>() <= row.Double(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<double>() <= (double)objVal;
                    }

                    if (p_filter.Type == typeof(DateTime))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<DateTime>() <= row.Date(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<DateTime>() <= ((DateTime)objVal);
                    }
                    break;

                case SearchConditionType.小于:
                    if (p_filter.Type == typeof(int))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<int>() > row.Int(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<int>() > (int)objVal;
                    }

                    if (p_filter.Type == typeof(long))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<long>() > row.Long(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<long>() > (long)objVal;
                    }

                    if (p_filter.Type == typeof(double))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<double>() > row.Double(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<double>() > (double)objVal;
                    }

                    if (p_filter.Type == typeof(DateTime))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<DateTime>() > row.Date(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<DateTime>() > ((DateTime)objVal);
                    }
                    break;

                case SearchConditionType.小于等于:
                    if (p_filter.Type == typeof(int))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<int>() >= row.Int(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<int>() >= (int)objVal;
                    }

                    if (p_filter.Type == typeof(long))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<long>() >= row.Long(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<long>() >= (long)objVal;
                    }

                    if (p_filter.Type == typeof(double))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<double>() >= row.Double(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<double>() >= (double)objVal;
                    }

                    if (p_filter.Type == typeof(DateTime))
                    {
                        if (p_data is Row row)
                            return p_filter.GetVal<DateTime>() >= row.Date(p_filter.ID);

                        if (objVal != null)
                            return p_filter.GetVal<DateTime>() >= ((DateTime)objVal);
                    }
                    break;
            }
            return true;
        }

        bool DoStringFilter(Cell p_filter, object p_data, SearchConditionType p_condition)
        {
            string filterVal = p_filter.GetVal<string>().Trim().ToLower();
            bool isChinesss = false;
            foreach (char vChar in filterVal)
            {
                if ((vChar >= 'a' && vChar <= 'z')
                    || (vChar >= 'A' && vChar <= 'Z')
                    || (vChar >= '0' && vChar <= '9'))
                {
                    continue;
                }
                isChinesss = true;
                break;
            }

            string dataVal;
            if (p_data is Row r)
            {
                dataVal = r.Str(p_filter.ID).ToLower();
            }
            else
            {
                object objVal;
                var pi = p_data.GetType().GetProperty(p_filter.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi == null || (objVal = pi.GetValue(p_data)) == null)
                    return false;
                dataVal = objVal.ToString().ToLower();
            }

            if (!isChinesss)
                dataVal = Kit.GetPinYin(dataVal);
            switch (p_condition)
            {
                case SearchConditionType.左包含:
                    return dataVal.StartsWith(filterVal);
                case SearchConditionType.包含:
                    return dataVal.IndexOf(filterVal) != -1;
                case SearchConditionType.右包含:
                    return dataVal.EndsWith(filterVal);
                case SearchConditionType.等于:
                    return dataVal == filterVal;
                case SearchConditionType.不等于:
                    return dataVal != filterVal;
                case SearchConditionType.为空:
                    return string.IsNullOrEmpty(dataVal);
                case SearchConditionType.不为空:
                    return !string.IsNullOrEmpty(dataVal);
            }
            return true;
        }
        #endregion

        #region UI
        void LoadCols()
        {
            _data = new Row();
            _condition = new Row();
            if (_owner.Data is Table tbl)
            {
                foreach (var col in tbl.Columns)
                {
                    CreateCol(col.ID, col.Type);
                }
            }
            else
            {
                var props = _owner.Data[0].GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var p in props)
                {
                    CreateCol(p.Name, p.PropertyType);
                }
            }
            _fv.Data = _data;
        }

        void CreateCol(string p_id, Type p_type)
        {
            new Cell(_data, p_id, p_type);
            var conCell = _condition.AddCell(p_id, p_type == typeof(string) ? SearchConditionType.包含 : SearchConditionType.等于);

            CBar bar = new CBar();
            var grid = new Grid();
            var sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10) };
            var tb = new TextBlock { FontFamily = Res.IconFont, Text = "\uE02D", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) };
            sp.Children.Add(tb);
            tb = new TextBlock { Text = p_id, TextWrapping = TextWrapping.NoWrap, VerticalAlignment = VerticalAlignment.Center };
            sp.Children.Add(tb);
            grid.Children.Add(sp);
            var btn = new Button { Content = conCell.Val.ToString(), Tag = conCell, HorizontalAlignment = HorizontalAlignment.Right, Background = Res.TransparentBrush };
            btn.Click += OnConditionClick;
            grid.Children.Add(btn);
            bar.Content = grid;
            _fv.Items.Add(bar);

            FvCell cell;
            if (p_type == typeof(DateTime))
            {
                cell = new CDate();
            }
            else if (p_type == typeof(bool))
            {
                cell = new CBool();
            }
            else if (p_type == typeof(int) || p_type == typeof(long))
            {
                cell = new CNum { IsInteger = true };
            }
            else if (p_type == typeof(double))
            {
                cell = new CNum();
            }
            else if (p_type == typeof(string))
            {
                var ls = new CList { IsEditable = true };
                ls.LoadData += OnLoadList;
                cell = ls;
            }
            else
            {
                cell = new CText();
            }
            cell.ID = p_id;
            cell.ShowTitle = false;
            _fv.Items.Add(cell);
        }

        void OnLoadList(object sender, AsyncEventArgs e)
        {
            CList cl = (CList)sender;
            var data = new Table { { "name" } };
            if (_owner.Data is Table tbl)
            {
                var ls = tbl.Distinct(new EqualityComparer(cl.ID));
                foreach (var r in ls)
                {
                    var row = data.AddRow();
                    row.InitVal("name", r[cl.ID]);
                }
            }
            else
            {
                var pi = _owner.Data[0].GetType().GetProperty(cl.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (var obj in _owner.Data)
                {
                    var objVal = pi.GetValue(obj);
                    if (objVal != null && !dict.ContainsKey(objVal.ToString()))
                    {
                        var str = objVal.ToString();
                        var row = data.AddRow();
                        row.InitVal("name", str);
                        dict[str] = "";
                    }
                }
            }
            ((CList)sender).Data = data;
        }
        
        void OnConditionClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            Cell cell = (Cell)btn.Tag;
            if (_data.Cells[cell.ID].Type == typeof(string))
            {
                if (_strMenu == null)
                    CreateStrMenu();
                _strMenu.Tag = btn;
                _strMenu.OpenContextMenu(btn);
                return;
            }

            if (_valMenu == null)
                CreateValMenu();
            _valMenu.Tag = btn;
            _valMenu.OpenContextMenu(btn);
        }

        void CreateValMenu()
        {
            _valMenu = new Menu { IsContextMenu = true, Placement = MenuPosition.BottomLeft };
            _valMenu.ItemClick += OnMiClick;
            Mi item = new Mi()
            {
                ID = "等于",
                Tag = SearchConditionType.等于,
            };
            _valMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "不等于",
                Tag = SearchConditionType.不等于,
            };
            _valMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "大于",
                Tag = SearchConditionType.大于,
            };
            _valMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "大于等于",
                Tag = SearchConditionType.大于等于,
            };
            _valMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "小于",
                Tag = SearchConditionType.小于,
            };
            _valMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "小于等于",
                Tag = SearchConditionType.小于等于,
            };
            _valMenu.Items.Add(item);
        }

        void CreateStrMenu()
        {
            _strMenu = new Menu { IsContextMenu = true, Placement = MenuPosition.BottomLeft };
            _strMenu.ItemClick += OnMiClick;
            Mi item = new Mi()
            {
                ID = "以...开头",
                Tag = SearchConditionType.左包含,
            };
            _strMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "包含",
                Tag = SearchConditionType.包含,
            };
            _strMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "以...结束",
                Tag = SearchConditionType.右包含,
            };
            _strMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "等于",
                Tag = SearchConditionType.等于,
            };
            _strMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "不等于",
                Tag = SearchConditionType.不等于,
            };
            _strMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "为空",
                Tag = SearchConditionType.为空,
            };
            _strMenu.Items.Add(item);

            item = new Mi()
            {
                ID = "不为空",
                Tag = SearchConditionType.不为空,
            };
            _strMenu.Items.Add(item);
        }

        void OnMiClick(object sender, Mi e)
        {
            var con = (SearchConditionType)e.Tag;
            var btn = (Button)e.Owner.Tag;
            btn.Content = con.ToString();
            Cell cell = (Cell)btn.Tag;
            cell.Val = con;

            if (con == SearchConditionType.为空)
                _data[cell.ID] = "#空#";
            else if (con == SearchConditionType.不为空)
                _data[cell.ID] = "#非空#";
        }
        #endregion

        #region Row列比较
        class EqualityComparer : IEqualityComparer<Row>
        {
            public string _colID;

            /// <summary>
            /// 比较器
            /// </summary>
            /// <param name="p_colID">列名</param>
            public EqualityComparer(string p_colID)
            {
                _colID = p_colID;
            }

            /// <summary>
            /// 比较指定列数据
            /// </summary>
            /// <param name="b1"></param>
            /// <param name="b2"></param>
            /// <returns></returns>
            public bool Equals(Row b1, Row b2)
            {
                return b1.Str(_colID) == b2.Str(_colID);
            }

            public int GetHashCode(Row p_dr)
            {
                return base.GetHashCode();
            }
        }
        #endregion

        #region 查询条件类型
        /// <summary>
        /// 查询条件类型
        /// </summary>
        enum SearchConditionType
        {
            等于,
            不等于,
            大于,
            大于等于,
            小于,
            小于等于,
            左包含,
            包含,
            右包含,
            为空,
            不为空
        }
        #endregion
    }
}

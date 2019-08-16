#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using Windows.UI.Xaml;
#endregion

namespace Dt.Sample
{
    public partial class FvLayout : Win
    {
        public FvLayout()
        {
            InitializeComponent();
            CreateCells(50);
            _fv.CellClick += OnCellClick;
            _ob.Data = _fv.Items[0];
        }

        void OnAddClick(object sender, RoutedEventArgs e)
        {
            FvCell cell = new FvCell();
            cell.Title = $"单元格{_fv.Items.Count + 1}";
            _fv.Items.Add(cell);
        }

        void OnAddFive(object sender, RoutedEventArgs e)
        {
            CreateCells(5);
        }

        void OnAddBatch(object sender, RoutedEventArgs e)
        {
            CreateCells(20);
        }

        void OnDelClick(object sender, RoutedEventArgs e)
        {
            if (_fv.Items.Count > 0)
                _fv.Items.RemoveAt(_fv.Items.Count - 1);
        }

        void OnClearClick(object sender, RoutedEventArgs e)
        {
            _fv.Items.Clear();
        }

        void OnReset(object sender, RoutedEventArgs e)
        {
            var items = _fv.Items;
            using (items.Defer())
            {
                items.Clear();
                CreateCells(20);
            }
        }

        void OnCellClick(object sender, FvCell e)
        {
            _ob.Data = e;
        }

        void CreateCells(int p_count)
        {
            Random rnd = new Random();
            for (int i = 0; i < p_count; i++)
            {
                FvCell cell = new FvCell();
                if ((i + 1) % 4 == 0)
                {
                    cell.ShowTitle = false;
                    //TextBlock tb = new TextBlock { Text = $"空格{_fv.Items.Count + 1}", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                    //cell.Editor = tb;
                }
                else
                {
                    cell.Title = $"单元格{_fv.Items.Count + 1}";
                }
                cell.RowSpan = rnd.Next(1, 3);
                _fv.Items.Add(cell);

                if ((i + 1) % 8 == 0)
                {
                    CBar sep = new CBar();
                    sep.Title = $"分隔行{_fv.Items.Count + 1}";
                    _fv.Items.Add(sep);
                }
            }
        }
    }
}
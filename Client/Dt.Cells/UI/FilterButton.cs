#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal partial class FilterButton : Button
    {
        public static readonly DependencyProperty BitmapProperty = DependencyProperty.Register(
            "Bitmap",
            typeof(ImageSource),
            typeof(FilterButton),
            new PropertyMetadata(null));

        CellItem _owner;

        public FilterButton(CellItem p_cellView)
        {
            DefaultStyleKey = typeof(FilterButton);
            IsHitTestVisible = false;
            _owner = p_cellView;
        }

        /// <summary>
        /// 获取设置源图像
        /// </summary>
        public ImageSource Bitmap
        {
            get { return (ImageSource)GetValue(BitmapProperty); }
            set { SetValue(BitmapProperty, value); }
        }

        internal async void ApplyState()
        {
            FilterButtonInfo filterButtonInfo = _owner.FilterButtonInfo;
            if (filterButtonInfo == null)
            {
                Bitmap = await SR.GetImage("NoSortFilter.png");
            }
            else
            {
                SortState sortState = filterButtonInfo.GetSortState();
                if (filterButtonInfo.IsFiltered())
                {
                    switch (sortState)
                    {
                        case SortState.None:
                            Bitmap = await SR.GetImage("Filter.png");
                            break;

                        case SortState.Ascending:
                            Bitmap = await SR.GetImage("FilterAscend.png");
                            break;

                        case SortState.Descending:
                            Bitmap = await SR.GetImage("FilterDescend.png");
                            break;
                    }
                }
                else
                {
                    switch (sortState)
                    {
                        case SortState.None:
                            Bitmap = await SR.GetImage("NoSortFilter.png");
                            break;

                        case SortState.Ascending:
                            Bitmap = await SR.GetImage("Ascend.png");
                            break;

                        case SortState.Descending:
                            Bitmap = await SR.GetImage("Descend.png");
                            break;
                    }
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplyState();
        }
    }
}


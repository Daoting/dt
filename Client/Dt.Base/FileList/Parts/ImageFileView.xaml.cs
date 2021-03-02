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
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Base.FileLists
{
    public sealed partial class ImageFileView : Dlg
    {
        FileItem _curItem;
        List<FileItem> _imgItems;
        bool _isDragging = false;
        Point _ptStart;
        double _horOffset;
        double _verOffset;

        public ImageFileView()
        {
            InitializeComponent();

            _img.PointerWheelChanged += OnPointerWheelChanged;
            _img.PointerPressed += OnPointerPressed;
            _img.PointerMoved += OnPointerMoved;
            _img.PointerReleased += OnPointerReleased;
        }

        public async Task ShowDlg(FileList p_fileList, FileItem p_curItem)
        {
            Throw.If(p_fileList == null, "无图片");
            _imgItems = (from item in p_fileList.Items
                         where item.FileType == FileItemType.Image
                         select item).ToList();
            Throw.If(_imgItems.Count == 0, "无图片");

            if (_imgItems.Count == 1)
            {
                _btnPre.Visibility = Visibility.Collapsed;
                _btnNext.Visibility = Visibility.Collapsed;
            }

            _curItem = p_curItem;
            if (_curItem == null)
                GotoNextImg();
            else
                LoadImage();

            if (!AtSys.IsPhoneUI)
            {
                Height = SysVisual.ViewHeight / 2;
                Width = SysVisual.ViewHeight / 2;
            }

            await ShowAsync();
        }

        void GotoNextImg()
        {
            if (_curItem == null)
            {
                _curItem = _imgItems[0];
            }
            else
            {
                int index = _imgItems.IndexOf(_curItem);
                if (index < _imgItems.Count - 1)
                    _curItem = _imgItems[index + 1];
                else
                    _curItem = _imgItems[0];
            }
            LoadImage();
        }

        void GotoPreImg()
        {
            if (_curItem == null)
            {
                _curItem = _imgItems[0];
            }
            else
            {
                int index = _imgItems.IndexOf(_curItem);
                if (index > 0)
                    _curItem = _imgItems[index - 1];
                else
                    _curItem = _imgItems[_imgItems.Count - 1];
            }
            LoadImage();
        }

        async void LoadImage()
        {
            if (_curItem == null || _curItem.FileType != FileItemType.Image)
            {
                _img.Source = null;
            }
            else
            {
                _img.Source = new BitmapImage(new Uri(await _curItem.EnsureFileExists()));
            }
        }

        void OnPreClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GotoPreImg();
        }

        void OnNextClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GotoNextImg();
        }

        void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            // Ctrl + 鼠标滚轮 缩放
            if (!InputManager.IsCtrlPressed || _img.Source == null)
                return;

            float delta = (float)(e.GetCurrentPoint(null).Properties.MouseWheelDelta * 0.001);
            if (_sv.ZoomFactor + delta > 0.1)
                _sv.ChangeView(null, null, _sv.ZoomFactor + delta);
        }


        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && _img.CapturePointer(e.Pointer))
            {
                e.Handled = true;
                _ptStart = e.GetCurrentPoint(null).Position;
                _horOffset = _sv.HorizontalOffset;
                _verOffset = _sv.VerticalOffset;
                _isDragging = true;
            }
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDragging)
                return;

            Point pt = e.GetCurrentPoint(null).Position;
            // 直接使用_sv.HorizontalOffset会造成拖拽距离和滚动距离不同步！
            _sv.ChangeView(_horOffset - pt.X + _ptStart.X, _verOffset - pt.Y + _ptStart.Y, null);
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDragging)
                return;

            _img.ReleasePointerCapture(e.Pointer);
            _isDragging = false;
        }

        void OnSave(object sender, Mi e)
        {
            _curItem?.SaveAs();
        }
    }
}

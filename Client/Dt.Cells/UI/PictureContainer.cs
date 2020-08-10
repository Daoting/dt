#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    internal partial class PictureContainer : FloatingObjectContainer
    {
        PictureView _pictureView;

        public PictureContainer(Picture picture, CellsPanel parentViewport) : base(picture, parentViewport)
        {
            _pictureView = new PictureView(picture, base.ParentViewport);
            _pictureView.HorizontalAlignment = HorizontalAlignment.Stretch;
            _pictureView.VerticalAlignment = VerticalAlignment.Stretch;
            base.Content = _pictureView;
        }

        internal override void Refresh(object parameter)
        {
            base.Refresh(parameter);
            _pictureView.RefreshPictureView();
        }
    }
}


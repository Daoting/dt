#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 图像格
    /// </summary>
    public partial class CImage : FvCell
    {
        #region 静态内容
        public static readonly DependencyProperty AllowMultipleProperty = DependencyProperty.Register(
            "AllowMultiple",
            typeof(bool),
            typeof(CImage),
            new PropertyMetadata(false));
        #endregion

        #region 成员变量
        ImageBoxDlg _dlg;
        #endregion

        #region 构造方法
        public CImage()
        {
            DefaultStyleKey = typeof(CImage);
        }
        #endregion

        /// <summary>
        /// 获取设置是否允许多个图像文件，默认false
        /// </summary>
        public bool AllowMultiple
        {
            get { return (bool)GetValue(AllowMultipleProperty); }
            set { SetValue(AllowMultipleProperty, value); }
        }

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var grid = (Grid)GetTemplateChild("Grid");
            grid.Tapped -= OnShowDlg;
            grid.Tapped += OnShowDlg;
        }

        protected override void SetValBinding()
        {
            string json = null;
            object data = GetVal();
            if (data != null)
                json = data.ToString();
            LoadImages(json);
        }
        #endregion

        /// <summary>
        /// 更新选择结果
        /// </summary>
        /// <param name="p_icon"></param>
        internal void UpdateValue(string p_json)
        {
            SetVal(p_json);
            LoadImages(p_json);
        }

        void OnShowDlg(object sender, TappedRoutedEventArgs e)
        {
            if (ReadOnlyBinding || (_dlg != null && _dlg.IsOpened))
                return;

            if (_dlg == null)
            {
                _dlg = new ImageBoxDlg(this);
            }
            _dlg.ShowDlg();
        }

        void LoadImages(string p_json)
        {

        }
    }
}
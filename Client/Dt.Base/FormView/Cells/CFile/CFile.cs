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
    /// 文件格
    /// </summary>
    public partial class CFile : FvCell
    {
        #region 成员变量
        readonly FileList _fl;
        #endregion

        #region 构造方法
        public CFile()
        {
            DefaultStyleKey = typeof(CFile);
            _fl = new FileList();
            // 自动行高
            RowSpan = -1;
        }
        #endregion

        /// <summary>
        /// 获取设置是否允许多个图像文件，默认false
        /// </summary>
        public int MaxFileCount
        {
            get { return _fl.MaxFileCount; }
            set { _fl.MaxFileCount = value; }
        }

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var grid = (Grid)GetTemplateChild("RootGrid");
            Grid.SetRow(_fl, 1);
            grid.Children.Add(_fl);
        }

        protected override void SetValBinding()
        {
            _fl.SetBinding(FileList.DataProperty, ValBinding);
        }
        #endregion

    }
}
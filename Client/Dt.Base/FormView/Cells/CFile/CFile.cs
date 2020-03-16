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
        FileBoxDlg _dlg;
        #endregion

        #region 构造方法
        public CFile()
        {
            DefaultStyleKey = typeof(CFile);
            _fl = new FileList();
        }
        #endregion

        /// <summary>
        /// 获取设置是否允许多个图像文件，默认false
        /// </summary>
        public bool AllowMultiple
        {
            get { return _fl.AllowMultiple; }
            set { _fl.AllowMultiple = value; }
        }

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var grid = (Grid)GetTemplateChild("Grid");
            grid.Children.Add(_fl);
        }

        protected override void SetValBinding()
        {
            _fl.SetBinding(FileList.DataProperty, ValBinding);
        }
        #endregion
    }
}
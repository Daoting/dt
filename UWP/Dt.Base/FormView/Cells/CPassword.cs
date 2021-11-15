#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 密码格
    /// </summary>
    public partial class CPassword : FvCell
    {
        /// <summary>
        /// 占位符
        /// </summary>
        public static readonly DependencyProperty HolderProperty = DependencyProperty.Register(
            "Holder",
            typeof(string),
            typeof(CPassword),
            new PropertyMetadata("●"));

        /// <summary>
        /// 最大字符数
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
            "MaxLength",
            typeof(int),
            typeof(CPassword),
            new PropertyMetadata(0));

        PasswordBox _pb;

        public CPassword()
        {
            DefaultStyleKey = typeof(CPassword);
        }

        /// <summary>
        /// 获取设置占位符
        /// </summary>
        [CellParam("占位符")]
        public string Holder
        {
            get { return (string)GetValue(HolderProperty); }
            set { SetValue(HolderProperty, value); }
        }

        /// <summary>
        /// 获取设置最大字符数
        /// </summary>
        [CellParam("最大字符数")]
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        protected override void OnApplyCellTemplate()
        {
            _pb = (PasswordBox)GetTemplateChild("PasswordBox");
        }

        protected override void SetValBinding()
        {
            _pb.SetBinding(PasswordBox.PasswordProperty, ValBinding);
        }
    }
}
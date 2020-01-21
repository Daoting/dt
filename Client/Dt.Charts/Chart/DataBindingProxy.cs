#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Charts
{
    [Bindable]
    public partial class DataBindingProxy : FrameworkElement
    {
        public static readonly DependencyProperty ObjectProperty = Utils.RegisterProperty("Object", typeof(object), typeof(DataBindingProxy));

        public object GetValue(Binding bnd)
        {
            base.SetBinding(ObjectProperty, bnd);
            object obj2 = Object;
            base.ClearValue(ObjectProperty);
            return obj2;
        }

        public object Object
        {
            get { return  base.GetValue(ObjectProperty); }
            set { base.SetValue(ObjectProperty, value); }
        }
    }
}


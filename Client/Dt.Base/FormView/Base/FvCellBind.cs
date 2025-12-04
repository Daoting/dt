#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 简洁版bind
    /// </summary>
    class FvCellBind : Binding
    {
        public FvCellBind(FvCell p_owner, object p_data)
        {
            Converter = new FreeConverter(this);
            Cell = p_owner;
            Data = p_data;
        }

        public object Data { get; }

        public FvCell Cell { get; }
    }

    partial class FreeConverter : IValueConverter
    {
        FvCellBind _owner;

        public FreeConverter(FvCellBind p_owner)
        {
            _owner = p_owner;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var mid = _owner.Cell.GetMiddle();
            if (mid != null)
                return mid.Get(new Mid(_owner, value));
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var mid = _owner.Cell.GetMiddle();
            if (mid != null)
                return mid.Set(new Mid(_owner, value));
            return value;
        }
    }
}
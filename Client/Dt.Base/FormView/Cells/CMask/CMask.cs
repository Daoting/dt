#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Toolkit.Mask;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 掩码格
    /// </summary>
    public partial class CMask : FvCell
    {
        readonly MaskBox _box;

        #region 构造方法
        public CMask()
        {
            DefaultStyleKey = typeof(CMask);
            _box = new MaskBox();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 掩码类型
        /// </summary>
        [CellParam("掩码类型")]
        public MaskType MaskType
        {
            get { return _box.MaskType; }
            set { _box.MaskType = value; }
        }

        /// <summary>
        /// 掩码表达式
        /// </summary>
        [CellParam("掩码内容")]
        public string Mask
        {
            get { return _box.Mask; }
            set { _box.Mask = value; }
        }

        /// <summary>
        /// 是否显示掩码占位符，RegEx有效
        /// </summary>
        [CellParam("是否显示占位符")]
        public bool ShowPlaceHolder
        {
            get { return _box.ShowPlaceHolder; }
            set { _box.ShowPlaceHolder = value; }
        }

        /// <summary>
        /// 是否保存为转换后的结果，Simple、Regular有效
        /// </summary>
        [CellParam("是否保存文本")]
        public bool SaveLiteral
        {
            get { return _box.SaveLiteral; }
            set { _box.SaveLiteral = value; }
        }

        /// <summary>
        /// 自动完成方式，RegEx有效
        /// </summary>
        [CellParam("自动完成方式")]
        public AutoCompleteType AutoComplete
        {
            get { return _box.AutoComplete; }
            set { _box.AutoComplete = value; }
        }

        /// <summary>
        /// 是否按掩码格式化显示
        /// </summary>
        [CellParam("格式化显示")]
        public bool UseAsDisplayFormat
        {
            get { return _box.UseAsDisplayFormat; }
            set { _box.UseAsDisplayFormat = value; }
        }

        /// <summary>
        /// 输入是否可为空
        /// </summary>
        [CellParam("是否可为空")]
        public bool AllowNullInput
        {
            get { return _box.AllowNullInput; }
            set { _box.AllowNullInput = value; }
        }

        /// <summary>
        /// 忽略空格
        /// </summary>
        [CellParam("是否忽略空格")]
        public bool IgnoreBlank
        {
            get { return _box.IgnoreBlank; }
            set { _box.IgnoreBlank = value; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 设置掩码占位符，RegEx有效，不再使用属性，改为方法，因FvCell.Placeholder为string类型
        /// </summary>
        public void SetPlaceHolder(char p_ch)
        {
            _box.PlaceHolder = p_ch;
        }
        #endregion

        #region 重写方法
        protected override void OnApplyCellTemplate()
        {
            _panel.Child = _box;
            var bind = new Binding
            {
                Path = new PropertyPath("Placeholder"),
                Source = this
            };
            _box.Box.SetBinding(TextBox.PlaceholderTextProperty, bind);
        }

        protected override void SetValBinding()
        {
            _box.SetBinding(MaskBox.ValueProperty,ValBinding);
        }

        protected override void OnReadOnlyChanged()
        {
            _box.Box.IsReadOnly = ReadOnlyBinding;
        }
        #endregion
    }
}
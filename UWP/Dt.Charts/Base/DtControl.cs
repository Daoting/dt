#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Charts
{
    /// <summary>
    /// 为屏蔽不同平台主事件调用顺序的不同，避免和DtControl重名
    /// </summary>
    public partial class UnoControl : Control
    {
        public UnoControl()
        {
            Loaded += OnLoaded;
        }

        /**********************************************************************************************************************************************************/
        // 不同平台主事件调用顺序
        // 
        // UWP：
        // ApplyTemplate(父子) -> OnLoadTemplate(父子) -> MeasureOverride(父子) -> ArrangeOverride(父子) -> SizeChanged(父子) -> Loaded(父子) -> OnControlLoaded(父子)
        // 
        // Android：
        // ApplyTemplate(子父) -> Loaded(父子) -> OnLoadTemplate(父子) -> OnControlLoaded(父子) -> MeasureOverride(父子) -> ArrangeOverride(父子) -> SizeChanged(子父)
        // 
        // iOS：
        // ApplyTemplate(子父) -> Loaded(子父) -> OnLoadTemplate(子父) -> OnControlLoaded(子父) -> MeasureOverride(父子) -> SizeChanged(父子) -> ArrangeOverride(父子)
        //
        // wasm:
        // ApplyTemplate(父子) -> Loaded(父子) -> OnLoadTemplate(父子) -> OnControlLoaded(父子) -> MeasureOverride(父子) -> ArrangeOverride(父子) -> SizeChanged(子父)
        //
        // UWP的OnApplyTemplate时控件已在可视树上，可查询父元素；uno此时不在可视树上，只能在Loaded时查询父元素！！！
        /***********************************************************************************************************************************************************/


        /**********************************************************************************************************************************************************/
        // uno中的Style
        // 
        // xaml自动生成代码时：控件对象在创建、设置属性后直接调用ApplyTemplate
        // 代码创建控件对象时：在添加到可视树时才调用ApplyTemplate
        // 设置Style时：直接调用ApplyTemplate，在构造方法中设置也一样，DefaultStyleKey则不同
        //
        // 因此控件不再可视树时无需指定 DefaultStyleKey 或 Style！如PhoneUI模式的Win, WinItem, Tabs等
        /***********************************************************************************************************************************************************/


        /// <summary>
        /// UWP：OnApplyTemplate时调用
        /// uno：只在第一次Loaded事件时调用
        /// </summary>
        protected virtual void OnLoadTemplate()
        {
        }

        /// <summary>
        /// 只在第一次Loaded事件时调用，始终在OnLoadTemplate后调用
        /// </summary>
        protected virtual void OnControlLoaded()
        {
        }

#if UWP
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnLoadTemplate();
        }
#endif

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
#if !UWP
            OnLoadTemplate();
#endif
            OnControlLoaded();
        }
    }
}

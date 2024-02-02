﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 为屏蔽不同平台主事件调用顺序的不同
    /// </summary>
    public partial class DtControl : Control
    {
        public DtControl()
        {
            Loaded += OnLoaded;
        }

        /**********************************************************************************************************************************************************/
        // 不同平台主事件调用顺序
        // 
        // WIN：
        // ApplyTemplate(父子) -> OnLoadTemplate(父子) -> MeasureOverride(父子) -> ArrangeOverride(父子) -> SizeChanged(父子) -> Loaded(父子) -> OnFirstLoaded(父子)
        // 
        // Android：
        // ApplyTemplate(子父) -> Loaded(父子) -> OnLoadTemplate(父子) -> OnFirstLoaded(父子) -> MeasureOverride(父子) -> ArrangeOverride(父子) -> SizeChanged(子父)
        // 
        // iOS：
        // ApplyTemplate(子父) -> Loaded(子父) -> OnLoadTemplate(子父) -> OnFirstLoaded(子父) -> MeasureOverride(父子) -> SizeChanged(父子) -> ArrangeOverride(父子)
        //
        // wasm:
        // ApplyTemplate(父子) -> Loaded(父子) -> OnLoadTemplate(父子) -> OnFirstLoaded(父子) -> MeasureOverride(父子) -> ArrangeOverride(父子) -> SizeChanged(子父)
        //
        // WIN的OnApplyTemplate时控件已在可视树上，可查询父元素；uno此时不在可视树上，只能在Loaded时查询父元素！！！
        /***********************************************************************************************************************************************************/


        /**********************************************************************************************************************************************************/
        // uno中的Style
        // 
        // xaml自动生成代码时：控件对象在创建、设置属性后直接调用ApplyTemplate
        // 代码创建控件对象时：在添加到可视树时才调用ApplyTemplate
        // 设置Style时：直接调用ApplyTemplate，在构造方法中设置也一样，DefaultStyleKey则不同
        //
        // 因此控件不再可视树时无需指定 DefaultStyleKey 或 Style！如PhoneUI模式的Win, Pane, Tabs等
        /***********************************************************************************************************************************************************/


        /// <summary>
        /// WIN：OnApplyTemplate时调用
        /// uno：只在第一次Loaded事件时调用
        /// </summary>
        protected virtual void OnLoadTemplate()
        {
        }

        /// <summary>
        /// 只在第一次Loaded事件时调用，始终在OnLoadTemplate后调用
        /// </summary>
        protected virtual void OnFirstLoaded()
        {
        }

#if WIN
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnLoadTemplate();
        }
#endif

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
#if !WIN
            OnLoadTemplate();
#endif
            OnFirstLoaded();
        }
    }
}

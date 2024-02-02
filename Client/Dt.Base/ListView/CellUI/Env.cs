#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自定义单元格UI的参数
    /// </summary>
    public class Env
    {
        #region 成员变量
        ViewItem _item;
        Dot _dot;
        #endregion

        #region 构造方法
        internal Env(ViewItem p_item, Dot p_dot)
        {
            _item = p_item;
            _dot = p_dot;
        }
        #endregion

        /*************************************************************************************/
        // 父子元素 DataContextChanged 事件及属性绑定在切换 DataContext 时的执行顺序
        //
        // uno
        // 父绑定 -> 子绑定 -> 子事件 -> 父事件
        // 
        // win
        // 父绑定 -> 父事件 -> 子绑定 -> 子事件
        /*************************************************************************************/

        /// <summary>
        /// 设置可视元素属性值事件，DataContext切换时触发
        /// </summary>
        public event Action<CallArgs> Set;

        /// <summary>
        /// 自定义单元格UI
        /// </summary>
        public UIElement UI { get; set; }

        /// <summary>
        /// 当前单元格的Dot
        /// </summary>
        public Dot Dot => _dot;

        /// <summary>
        /// 以默认方式生成单元格UI，该元素无需再设置值内部已处理
        /// </summary>
        /// <returns></returns>
        public UIElement CreateDefaultUI()
        {
            var elem = _dot.CreateDefaultUI(_item);
            if (_dot.SetCallback != null)
                Set += _dot.SetCallback;
            return elem;
        }

        /// <summary>
        /// DataContext切换时设置可视元素属性值
        /// </summary>
        internal void InternalSet(CallArgs c)
        {
            Set?.Invoke(c);
        }
    }
}
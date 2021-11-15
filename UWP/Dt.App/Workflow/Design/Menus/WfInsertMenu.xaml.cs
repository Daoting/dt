#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-31 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    public sealed partial class WfInsertMenu : Menu
    {
        Sketch _sketch;

        public WfInsertMenu(Sketch p_sketch)
        {
            InitializeComponent();
            _sketch = p_sketch;
            Opening += OnOpening;
        }

        void OnOpening(object sender, AsyncCancelEventArgs e)
        {
            this["插入起始活动"].Visibility = IsValidByShapetype("开始") ? Visibility.Collapsed : Visibility.Visible;
            this["插入结束活动"].Visibility = IsValidByShapetype("结束") ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 执行插入开始菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnStartNode(object sender, Mi e)
        {
            SNode node = new SNode(_sketch)
            {
                Title = "开始",
                Shape = "开始",
                Width = 80,
                Height = 60
            };
            Insert(node);
        }

        /// <summary>
        /// 执行插入任务菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAtvNode(object sender, Mi e)
        {
            SNode node = new SNode(_sketch)
            {
                Title = "任务项",
                Shape = "任务",
                Width = 120,
                Height = 60
            };
            Insert(node);
        }

        /// <summary>
        /// 执行插入延时菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSyncNode(object sender, Mi e)
        {
            SNode node = new SNode(_sketch)
            {
                Title = "同步",
                Shape = "同步",
                Width = 120,
                Height = 60,
                Background = Res.深灰1,
                BorderBrush = Res.深灰2
            };
            Insert(node);
        }

        /// <summary>
        /// 执行插入结束菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnEndNode(object sender, Mi e)
        {
            SNode node = new SNode(_sketch)
            {
                Title = "完成",
                Shape = "结束",
                Width = 80,
                Height = 60,
                Background = Res.深灰1,
                BorderBrush = Res.深灰2
            };
            Insert(node);
        }

        /// <summary>
        /// 执行插入文本项菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTextNode(object sender, Mi e)
        {
            TextBlock tb = new TextBlock { Text = "新文本", FontSize = 20 };
            Insert(tb);
        }

        /// <summary>
        /// 执行插入方法
        /// </summary>
        /// <param name="p_elem"></param>
        async void Insert(FrameworkElement p_elem)
        {
            _sketch.SetNodePos(p_elem, this.GetRelativePosition(_sketch));
            if (p_elem is SNode node)
                node.ID = await AtCm.NewID();
            _sketch.Insert(p_elem);
        }

        /// <summary>
        /// 在sketch中判断是否存在指定形状的Snode对象。
        /// </summary>
        /// <param name="p_shapetype"></param>
        /// <returns></returns>
        bool IsValidByShapetype(string p_shapetype)
        {
            return (from obj in _sketch.Container.Children
                    let item = obj as SNode
                    where item != null && item.Shape == p_shapetype
                    select item).Any();
        }
    }
}

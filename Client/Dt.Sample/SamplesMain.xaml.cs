#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Sample
{
    /// <summary>
    /// 控件样例
    /// </summary>
    [View("控件样例")]
    public sealed partial class SamplesMain : NaviWin
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public SamplesMain()
        {
            InitializeComponent();
            LoadSampleList();
        }

        void LoadSampleList()
        {
            List<GroupData<DataCmd>> ds = new List<GroupData<DataCmd>>();
            GroupData<DataCmd> group = null;

            #region 框架结构
            group = new GroupData<DataCmd>
            {
                new DataCmd
                {
                    Title = "页面窗口",
                    Note = "继承自PageWin，基础内容控件，包括标题、图标、工具栏和页面内容；手机上承载在PhonePage内进行导航",
                    Tag = typeof(PageWinDemo)
                },
                new DataCmd
                {
                    Title = "普通窗口",
                    Note = "继承自Win，窗口在PC上分上下左右和主区五个区域，由Tab承载内容，拖动时自动停靠；手机上自适应为可导航多页面",
                    Tag = typeof(WinDemo)
                },
                new DataCmd
                {
                    Title = "导航窗口",
                    Note = "停靠式窗口包含多个子Tab，手机上在Tab之间实现页面导航，将Tab承载在PhonePage内",
                    Tag = typeof(NaviWinDemo)
                },
                new DataCmd
                {
                    Title = "对话框",
                    Note = "模拟传统对话框，PC上显示在窗口上层，可拖动、调整大小、自动关闭等，手机上承载在PhonePage内",
                    Tag = typeof(DlgDemo),
                    Callback = OnNaviTo
                },
                new DataCmd
                {
                    Title = "提示信息",
                    Note = "提供两个级别的提示信息（普通、警告），在对话框上层显示，可自动关闭，最多可显示一个操作按钮",
                    Tag = typeof(NotifyDemo),
                    Callback = OnNaviTo
                },
                new DataCmd
                {
                    Title = "浮动面板",
                    Note = "显示在最上层的面板容器，内部使用Popup实现，始终有遮罩",
                    Tag = typeof(FlyoutDemo),
                    Callback = OnNaviTo
                },
            };
            group.Title = "框架结构";
            ds.Add(group);
            #endregion

            #region 数据控件
            group = new GroupData<DataCmd>
            {
                new DataCmd
                {
                    Title = "表单Fv",
                    Note = "表单由单元格组成，单元格包括列名和编辑器，自动布局，支持自定义行数和内容，可作为独立的布局面板使用",
                    Tag = typeof(FvHome)
                },
                new DataCmd
                {
                    Title = "列表Lv",
                    Note = "支持三类视图表格、列表、磁贴，两种数据源，三种选择模式，定制分组，上下文菜单",
                    Tag = typeof(LvHome)
                },
                new DataCmd
                {
                    Title = "树Tv",
                    Note = "支持动态加载子节点，自定义节点样式、节点内容，动态设置节点模板，上下文菜单",
                    Tag = typeof(TvHome)
                },
                new DataCmd
                {
                    Title = "数据图表",
                    Note = "支持柱线饼等9大类40种不同图表，每种图表提供多种不同的调色板，支持交互操作",
                    Tag = typeof(ChartHome)
                },
                new DataCmd
                {
                    Title = "数据访问与Rpc",
                    Note = "包括数据表操作、数据序列化、远程/本地数据的增删改查、远程过程调用等",
                    Tag = typeof(DataAccessHome)
                },
            };
            group.Title = "数据控件";
            ds.Add(group);
            #endregion

            #region 基础控件
            group = new GroupData<DataCmd>
            {
                new DataCmd
                {
                    Title = "菜单",
                    Note = "包括普通工具栏菜单、上下文菜单，支持多层子项、选择和分组单选等功能",
                    Tag = typeof(MenuHome),
                },
                new DataCmd
                {
                    Title = "可停靠面板",
                    Note = "停靠式窗口的布局面板",
                    Tag = typeof(DockPanelDemo),
                    Callback = OnNaviTo
                },
                new DataCmd
                {
                    Title = "分隔栏",
                    Note = "包括水平/垂直分隔功能",
                    Tag = typeof(SplitterDemo),
                    Callback = OnNaviTo
                },
                new DataCmd
                {
                    Title = "流程图",
                    Note = "任务流程定义示意图",
                    Tag = typeof(SketchPage)
                },
                 new DataCmd
                {
                    Title = "Tab页",
                    Note = "TabControl控件基本功能演示",
                    Tag = typeof(TabControlDemo)
                },
                new DataCmd
                {
                    Title = "系统监视输出",
                    Note = "内部使用的调试输出与断言处理，调试输出内容同时保存在日志，断言处理在调试与非调试状态有不同行为",
                    Tag = typeof(SysTraceDemo)
                },
                new DataCmd
                {
                    Title = "基础事件",
                    Tag = typeof(RouteEventDemo)
                },
                new DataCmd
                {
                    Title = "跨平台",
                    Note = "文件选择、上传下载文件",
                    Tag = typeof(CrossHome),
                },
            };
            group.Title = "基础控件";
            ds.Add(group);
            #endregion

            #region 样式资源
            group = new GroupData<DataCmd>
            {
                new DataCmd
                {
                    Title = "图标",
                    Note = "内置的矢量文字，可用作图标、提示",
                    Tag = typeof(IconDemo),
                    Callback = OnNaviTo
                },
                new DataCmd
                {
                    Title = "按钮",
                    Note = "标准按钮和自定义按钮的常用样式",
                    Tag = typeof(BtnDemo),
                    Callback = OnNaviTo
                },
                new DataCmd
                {
                    Title = "常用画刷",
                    Note = "",
                    Tag = typeof(BrushDemo),
                    Callback = OnNaviTo
                }
            };
            group.Title = "样式资源";
            ds.Add(group);
            #endregion

            #region 临时
            group = new GroupData<DataCmd>
            {
                new DataCmd
                {
                    Title = "注销",
                    Callback = (e) => AtSys.Logout()
                },
                new DataCmd
                {
                    Title = "测试1",
                    Note = "Daoting测试",
                    Tag = typeof(TestDemo1)
                },
                new DataCmd
                {
                    Title = "测试2",
                    Note = "忠宝测试",
                    Tag = typeof(TestDemo2)
                },
            };
            group.Title = "临时";
            ds.Add(group);
            #endregion

            _lv.Data = ds;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            AtKit.RunAsync(() =>
            {
                DataCmd dc = (DataCmd)e.Data;
                if (dc.Callback != null)
                    dc.Callback(dc);
                else if (dc.Tag is Type type)
                    AtApp.OpenWin(type, dc.Title);
            });
        }

        void OnNaviTo(DataCmd p_cmd)
        {
            IWin win;
            if (p_cmd.Tag is Type tp)
            {
                win = Activator.CreateInstance(tp) as IWin;
                if (win != null)
                {
                    win.Title = p_cmd.Title;
                    p_cmd.Tag = win;
                }
            }
            win = p_cmd.Tag as IWin;
            if (win != null)
                LoadWin(win);
        }
    }
}
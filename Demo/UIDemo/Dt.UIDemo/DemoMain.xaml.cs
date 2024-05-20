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
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Dt.Base.Tools;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.UIDemo
{
    /// <summary>
    /// UI样例
    /// </summary>
    [View("控件样例")]
    public sealed partial class DemoMain : Win
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public DemoMain()
        {
            InitializeComponent();
            LoadControlList();
            LoadModuleList();
            LoadTools();
        }

        void LoadControlList()
        {
            Nl<GroupData<Nav>> ds = new Nl<GroupData<Nav>>();

            #region 框架结构
            var group = new GroupData<Nav>
            {
                new Nav("空白窗口", typeof(BlankWin)) { Desc = "窗口内容为空" },
                new Nav("主区窗口", typeof(SingleViewWin)) { Desc = "有标题栏的空白窗口" },
                new Nav("动态主区窗口", typeof(ToggleWinCenter)) { Desc = "左区联动切换主区内容" },
                new Nav("三区窗口", typeof(ThreePanelWin)) { Desc = "左区、主区、右区" },
                new Nav("xaml精简写法", typeof(MinWinXaml)) { Desc = "Tab、Tabs或普通元素可直接放在Win下，用Ex.Dock指定停靠位置" },
                new Nav("窗口布局", typeof(WinLayout)) { Desc = "只Win模式：窗口内的所有Tab可拖动并自动停靠" },
                new Nav("窗口内导航", typeof(WinNavi)) { Desc = "只Phone模式：窗口内的所有Tab可互相导航" },
                new Nav("Tab区域内导航", typeof(TabNavi)) { Desc = "导航时支持带遮罩的模式视图、导航参数、导航结果" },
                new Nav("对话框", typeof(DlgHome)) { Desc = "模拟传统对话框" },
                new Nav("提示信息", typeof(NotifyDemo)) { Desc = "普通信息、警告信息、Toast通知、后台任务" },
            };
            group.Title = "框架结构";
            ds.Add(group);
            #endregion

            #region 基础控件
            group = new GroupData<Nav>
            {
                new Nav("表单Fv", typeof(FvHome)) { Desc = "表单、单元格、编辑器、自动布局" },
                new Nav("列表Lv", typeof(LvHome)) { Desc = "三种视图、两类数据源、各种变形" },
                new Nav("树", typeof(TvHome)) { Desc = "传统树，自定义节点样式、节点内容" },
                new Nav("菜单", typeof(MenuHome)) { Desc = "菜单、工具栏、上下文菜单" },
                new Nav("Excel", typeof(ExcelHome)) { Desc = "模拟Excel的常用功能" },
                new Nav("Pdf", typeof(PdfHome)) { Desc = "Pdf预览、导入、导出、打印" },
                new Nav("报表", typeof(RptHome)) { Desc = "报表模板设计、预览、导出、打印" },
                new Nav("图表", typeof(ChartHome)) { Desc = "柱线饼等9大类40种不同图表" },
                new Nav("富文本", typeof(RichTextHome)) { Desc = "支持Html、Markdown两种格式的富文本编辑及浏览" },
                new Nav("杂项", typeof(MiscHome)) { Desc = "分隔栏、可停靠面板等" },
            };
            group.Title = "基础控件";
            ds.Add(group);
            #endregion
            
            _navControl.Data = ds;
        }

        void LoadModuleList()
        {
            _navModule.Data = new Nl<Nav>
            {
                new Nav("功能列表视图", typeof(NavListDemo)) { Desc = "通过功能项打开新窗口或切换主区内容" },
                new Nav("通用搜索视图", typeof(SearchMvWin)) { Desc = "包括固定搜索项、历史搜索项、搜索事件、导航等功能" },
                new Nav("日志", typeof(LogDemo)) { Desc = "可通过AppStub.LogSetting设置日志输出，支持输出到Console、Trace或保存到文件" },
                new Nav("数据表操作", typeof(TableAccess)) { Desc = "Table, Row, Col, Cell的常用方法" },
                new Nav("异常处理", typeof(ExceptionDemo)) { Desc = "客户端同步、异步异常及处理" },
                new Nav("本地事件", typeof(LocalEventDemo)) { Desc = "客户端全局事件定义及处理" },
                new Nav("多语言", typeof(LocalizationDemo)) { Desc = "文字本地化" },
            };
        }

        void LoadTools()
        {
            var lv = _navTools.ListView;
            lv.MaxWidth = 500;
            lv.HorizontalAlignment = HorizontalAlignment.Left;

            _navTools.Data = new Nl<Nav>
            {
                new Nav("查找图标", typeof(IconDemo), Icons.图标) { Desc = "内置的矢量文字，可用作图标、提示" },
#if WIN
                new Nav("生成App图片", typeof(AppIcon), Icons.图片) { Desc = "生成 android 和 iOS 中用到的app图片" },
#endif
                new Nav("字体", typeof(FontDemo), Icons.字体大小) { Desc = "常用字体大小" },
                new Nav("常用画刷", typeof(BrushDemo), Icons.扫帚) { Desc = "内部标准画刷" },
                new Nav("按钮", typeof(BtnDemo), Icons.光标) { Desc = "常用按钮样式" },
                new Nav("标准控件", typeof(StyleHome), Icons.排列) { Desc = "常用系统控件的样式" },
#if DEBUG
                new Nav("测试", typeof(TestDemo1), Icons.Bug) { Desc = "临时测试用" },
#endif
            };
        }

        void OpenHomeWin(object p_owner, Nav p_nav)
        {
            Stub.Reboot(Type.GetType("Dt.MgrDemo.AppStub,Dt.MgrDemo"));
        }
    }
}
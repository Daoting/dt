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

namespace Demo.UI
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

            #region 窗口
            var group = new GroupData<Nav>
            {
                new Nav("空白窗口", typeof(BlankWin), Icons.新窗口) { Desc = "窗口内容为空" },
                new Nav("主区窗口", typeof(SingleViewWin), Icons.打开新窗口) { Desc = "有标题栏的空白窗口" },
                new Nav("动态主区窗口", typeof(ToggleWinCenter), Icons.停靠左侧) { Desc = "左区联动切换主区内容" },
                new Nav("三区窗口", typeof(ThreePanelWin), Icons.预览链接) { Desc = "左区、主区、右区" },
                new Nav("xaml精简写法", typeof(MinWinXaml), Icons.Html) { Desc = "Tab、Tabs或普通元素可直接放在Win下，用Ex.Dock指定停靠位置" },
                new Nav("窗口布局", typeof(WinLayout), Icons.打开面板) { Desc = "只Win模式：窗口内的所有Tab可拖动并自动停靠" },
                new Nav("窗口内导航", typeof(WinNavi), Icons.手机) { Desc = "只Phone模式：窗口内的所有Tab可互相导航" },
                new Nav("Tab区域内导航", typeof(TabNavi), Icons.关闭面板) { Desc = "导航时支持带遮罩的模式视图、导航参数、导航结果" },
            };
            group.Title = "窗口";
            ds.Add(group);
            #endregion

            ds.Add(GetGroup(typeof(DlgHome), "对话框", "模拟传统对话框"));
            ds.Add(GetGroup(typeof(FvHome), "表单Fv", "表单、单元格、编辑器、自动布局"));
            ds.Add(GetGroup(typeof(LvHome), "列表Lv", "三种视图、两类数据源、各种变形"));
            ds.Add(GetGroup(typeof(MsgHome), "消息", "提示信息、Toast通知、托盘通知"));
            ds.Add(GetGroup(typeof(TvHome), "树", "传统树，自定义节点样式、节点内容"));
            ds.Add(GetGroup(typeof(MenuHome), "菜单", "菜单、工具栏、上下文菜单"));
            ds.Add(GetGroup(typeof(ExcelHome), "Excel", "模拟Excel的常用功能"));
            ds.Add(GetGroup(typeof(RptHome), "报表", "报表模板设计、预览、导出、打印"));
            ds.Add(GetGroup(typeof(ChartHome), "图表1", "柱线饼等9大类40种不同图表，支持嵌入Excel"));
            ds.Add(GetGroup(typeof(Chart2Home), "图表2", "ScottPlot开源图表，高性能、交互性强"));
            ds.Add(GetGroup(typeof(PdfHome), "Pdf及富文本", "Pdf预览、导入、导出、打印，Html、Markdown格式的编辑及浏览"));
            ds.Add(GetGroup(typeof(MiscHome), "杂项", "分隔栏、可停靠面板等"));
            
            _navControl.Data = ds;
        }

        GroupData<Nav> GetGroup(Type p_homeType, string p_title, string p_desc)
        {
            var group = new GroupData<Nav> { Title = p_title };
            group.Add(new Nav("总览", p_homeType, Icons.主页) { Desc = p_desc });
            var prop = p_homeType.GetProperty("Dir", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (prop != null && prop.GetValue(null) is Nl<Nav> dir)
                group.AddRange(dir);
            return group;
        }
        
        void LoadModuleList()
        {
            _navModule.Data = new Nl<Nav>
            {
                new Nav("功能列表NavList", typeof(NavListDemo)) { Desc = "通过功能项打开新窗口或切换主区内容，Lv始终真实行/项绘制(IRowView方式)" },
                new Nav("模糊搜索FuzzySearch", typeof(SearchMvWin)) { Desc = "包括固定搜索项、历史搜索项、搜索事件、导航等功能" },
                new Nav("日志", typeof(LogDemo)) { Desc = "可通过AppStub.LogSetting设置日志输出，支持输出到Console、Trace或保存到文件" },
                new Nav("数据表操作", typeof(TableAccess)) { Desc = "Table, Row, Col, Cell的常用方法" },
                new Nav("异常处理", typeof(ExceptionDemo)) { Desc = "客户端同步、异步异常及处理" },
                new Nav("后台任务", typeof(BgJogDemo), Icons.命令) { Desc = "后台任务的提示信息" },
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
                new Nav("查找图标", typeof(IconWin), Icons.图标) { Desc = "内置的矢量文字，可用作图标、提示" },
#if WIN
                new Nav("生成App图片", typeof(AppIcon), Icons.图片) { Desc = "生成 android 和 iOS 中用到的app图片" },
#endif
                new Nav("字体", typeof(FontDemo), Icons.字体大小) { Desc = "常用字体大小" },
                new Nav("常用画刷", typeof(BrushDemo), Icons.扫帚) { Desc = "内部标准画刷" },
                new Nav("按钮", typeof(BtnDemo), Icons.光标) { Desc = "常用按钮样式" },
                new Nav("标准控件", typeof(StyleHome), Icons.排列) { Desc = "常用系统控件的样式" },
                new Nav("内存泄漏", typeof(TestMemLeak), Icons.扫帚) { Desc = "检查内存泄漏" },
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
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 布局管理
    /// 1. 宽度足够时，加载历史布局或默认布局
    /// 2. 宽度小时，自动隐藏两侧，隐藏优先级：先右再左，Center始终显示
    /// </summary>
    internal class LayoutManager
    {
        #region 成员变量
        const double _centerWidth = 240;
        static readonly XmlWriterSettings _writerSettings = new XmlWriterSettings() { OmitXmlDeclaration = true };
        Win _owner;
        string _default;
        readonly Dictionary<string, Tab> _tabs;
        List<double> _colsWidth;
        // 自动隐藏位置，-1表示不自动隐藏
        int _fitCols = -1;
        #endregion

        #region 构造方法
        public LayoutManager(Win p_owner)
        {
            _owner = p_owner;
            _tabs = new Dictionary<string, Tab>();
            Init();
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 恢复默认布局
        /// 1. 删除状态库的历史布局
        /// 2. 加载最初布局
        /// </summary>
        public void LoadDefaultLayout()
        {
            if (AllowSaveLayout())
                AtState.Exec($"delete from DockLayout where BaseUri=\"{_owner.BaseUri.AbsolutePath}\"");
            ApplyLayout(_default);
            _owner.AllowResetLayout = false;
        }

        /// <summary>
        /// 保存当前布局
        /// </summary>
        public void SaveCurrentLayout()
        {
            // 宽度小时不保存
            if (!AllowSaveLayout())
                return;

            Kit.RunAsync(async () =>
            {
                var xml = WriteXml();
                DockLayout cookie = AtState.First<DockLayout>($"select * from DockLayout where BaseUri='{_owner.BaseUri.AbsolutePath}'");
                if (cookie == null)
                    cookie = new DockLayout(_owner.BaseUri.AbsolutePath, xml);
                else
                    cookie.Layout = xml;
                await AtState.Save(cookie, false);
                _owner.AllowResetLayout = true;
            });
        }

        //**************************************************************************
        // 响应式设计：三种布局方式对应三种界面宽度
        // 1. 界面宽度 <= 640px，PhoneUI模式，4"到6"设备 或 缩小的窗口，只一列面板
        // 2. 界面宽度在 641px ~ 1007px，7"到12"设备 或 缩小的窗口，最多两列面板
        // 3. 界面宽度 >= 1008px，13"及更大设备，最多三列面板
        //**************************************************************************

        /// <summary>
        /// Win宽度变化时自动调整
        /// </summary>
        /// <param name="p_width"></param>
        public void OnWidthChanged(double p_width)
        {
            double width = 0;
            int index = -1;
            for (int i = 0; i < _colsWidth.Count; i++)
            {
                width += _colsWidth[i];
                if (width > p_width)
                {
                    index = i;
                    break;
                }
            }
            if (_fitCols == index)
                return;

            _fitCols = index;
            if (_fitCols == -1)
            {
                // 宽度足够，加载历史布局或默认布局
                DockLayout cookie;
                if (AllowSaveLayout()
                    && (cookie = AtState.First<DockLayout>($"select * from DockLayout where BaseUri=\"{_owner.BaseUri.AbsolutePath}\"")) != null
                    && ApplyLayout(cookie.Layout))
                {
                    _owner.AllowResetLayout = true;
                }
                else
                {
                    ApplyLayout(_default);
                    _owner.AllowResetLayout = false;
                }
            }
            else
            {
                // 自动隐藏两侧
                ApplyAutoHide();
                _owner.AllowResetLayout = false;
            }
        }

        /// <summary>
        /// 深度清除所有子项
        /// </summary>
        /// <param name="p_items"></param>
        public static void ClearItems(IPaneList p_items)
        {
            // 不可使用Items.Clear
            while (p_items.Items.Count > 0)
            {
                // 先移除当前项，再清除子项，不可颠倒！
                var item = p_items.Items[0];
                p_items.Items.RemoveAt(0);
                if (item is IPaneList child)
                    ClearItems(child);
                else if (item is TabControl tabs)
                    tabs.Items.Clear();
            }
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化布局环境
        /// 1. 记录默认布局
        /// 2. 加载状态库中的历史布局
        /// 3. 无历史布局则加载默认布局
        /// </summary>
        void Init()
        {
            // 记录默认布局
            SaveDefaultXml();
            if (AllowSaveLayout())
            {
                DockLayout cookie = AtState.First<DockLayout>($"select * from DockLayout where BaseUri=\"{_owner.BaseUri.AbsolutePath}\"");
                if (cookie != null)
                {
                    // 加载历史布局
                    if (ApplyLayout(cookie.Layout))
                    {
                        _owner.AllowResetLayout = true;
                    }
                    else
                    {
                        // 历史布局加载失败，重载默认布局
                        ApplyLayout(_default);
                        _owner.AllowResetLayout = false;
                    }
                }
            }
        }

        /// <summary>
        /// 保存初始布局，同步处理布局、提取Tab字典，已调优
        /// </summary>
        void SaveDefaultXml()
        {
            _owner.IsReseting = true;

            // 按类型提取各项
            List<Main> centers = new List<Main>();
            List<Pane> lefts = new List<Pane>();
            List<Pane> rights = new List<Pane>();
            List<Pane> topBottom = new List<Pane>();
            List<Pane> floats = new List<Pane>();
            List<Tab> leftHide = new List<Tab>();
            List<Tab> rightHide = new List<Tab>();
            List<Tab> topHide = new List<Tab>();
            List<Tab> bottomHide = new List<Tab>();
            while (_owner.Items.Count > 0)
            {
                object obj = _owner.Items[0];
                _owner.Items.RemoveAt(0);

                if (obj is Pane di)
                {
                    ExtractItems(di);
                    if (di.Pos == PanePosition.Floating)
                    {
                        floats.Add(di);
                    }
                    else
                    {
                        // 在停靠中挑出自动隐藏项
                        foreach (Tab tab in GetHideItems(di))
                        {
                            if (di.Pos == PanePosition.Left)
                                leftHide.Add(tab);
                            else if (di.Pos == PanePosition.Right)
                                rightHide.Add(tab);
                            else if (di.Pos == PanePosition.Top)
                                topHide.Add(tab);
                            else if (di.Pos == PanePosition.Bottom)
                                bottomHide.Add(tab);
                        }

                        if (!IsRemoved(di))
                        {
                            RemoveUnused(di);

                            if (di.Pos == PanePosition.Left)
                                lefts.Add(di);
                            else if (di.Pos == PanePosition.Right)
                                rights.Add(di);
                            else
                                topBottom.Add(di);
                        }
                    }
                }
                else if (obj is Main center)
                {
                    ExtractItems(center);
                    centers.Add(center);
                }
                else
                {
                    // 包含普通界面元素时：
                    // 1. 将其放于主区
                    // 2. 不显示标题栏
                    // 3. 不自动保存布局状态
                    // 4. 不显示恢复默认布局按钮
                    // 5. 一般为单视图窗口
                    _owner.AutoSaveLayout = false;
                    Main wc = new Main();
                    Tabs tabs = new Tabs { ShowHeader = false };
                    tabs.Items.Add(new Tab { Content = obj });
                    wc.Items.Add(tabs);
                    centers.Add(wc);
                }
            }

            StringBuilder xml = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(xml, _writerSettings))
            {
                writer.WriteStartElement("Items");

                // 停靠项，按左右上下顺序输出
                writer.WriteStartElement("Win");
                _colsWidth = new List<double>();
                // 首先Center宽度
                _colsWidth.Add(centers.Count > 0 ? _centerWidth : 0);
                if (lefts.Count > 0)
                {
                    foreach (var di in lefts)
                    {
                        WriteWinItem(di, writer);
                        _owner.Items.Add(di);
                        _owner.RootPanel.Children.Add(di);
                        _colsWidth.Add(di.InitWidth);
                    }
                }
                if (rights.Count > 0)
                {
                    foreach (var di in rights)
                    {
                        WriteWinItem(di, writer);
                        _owner.Items.Add(di);
                        _owner.RootPanel.Children.Add(di);
                        _colsWidth.Add(di.InitWidth);
                    }
                }
                if (topBottom.Count > 0)
                {
                    foreach (var di in topBottom)
                    {
                        WriteWinItem(di, writer);
                        _owner.Items.Add(di);
                        _owner.RootPanel.Children.Add(di);
                    }
                }
                writer.WriteEndElement();

                // 中部项
                if (centers.Count > 0)
                {
                    foreach (Main center in centers)
                    {
                        // 挪到CenterItem，特殊处理！
                        while (center.Items.Count > 0)
                        {
                            var centerItem = center.Items[0];
                            center.Items.RemoveAt(0);
                            _owner.CenterItem.Items.Add(centerItem);
                        }
                    }
                    WriteCenter(writer);
                }

                // 左侧隐藏项
                if (leftHide.Count > 0)
                {
                    writer.WriteStartElement("Left");
                    if (_owner.LeftAutoHide == null)
                        _owner.CreateLeftAutoHideTab();
                    foreach (Tab tab in leftHide)
                    {
                        WriteTab(tab, writer);
                        _owner.LeftAutoHide.Unpin(tab);
                    }
                    writer.WriteEndElement();
                }

                // 右侧隐藏项
                if (rightHide.Count > 0)
                {
                    writer.WriteStartElement("Right");
                    if (_owner.RightAutoHide == null)
                        _owner.CreateRightAutoHideTab();
                    foreach (Tab tab in rightHide)
                    {
                        WriteTab(tab, writer);
                        _owner.RightAutoHide.Unpin(tab);
                    }
                    writer.WriteEndElement();
                }

                // 上侧隐藏项
                if (topHide.Count > 0)
                {
                    writer.WriteStartElement("Top");
                    if (_owner.TopAutoHide == null)
                        _owner.CreateTopAutoHideTab();
                    foreach (Tab tab in topHide)
                    {
                        WriteTab(tab, writer);
                        _owner.TopAutoHide.Unpin(tab);
                    }
                    writer.WriteEndElement();
                }

                // 下侧隐藏项
                if (bottomHide.Count > 0)
                {
                    writer.WriteStartElement("Bottom");
                    if (_owner.BottomAutoHide == null)
                        _owner.CreateBottomAutoHideTab();
                    foreach (Tab tab in bottomHide)
                    {
                        WriteTab(tab, writer);
                        _owner.BottomAutoHide.Unpin(tab);
                    }
                    writer.WriteEndElement();
                }

                // 浮动项
                if (floats.Count > 0)
                {
                    writer.WriteStartElement("Float");
                    foreach (Pane wi in floats)
                    {
                        WriteWinItem(wi, writer);
                        OpenInWindow(wi);
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.Flush();
            }
            _default = xml.ToString();
            _owner.IsReseting = false;
        }
        #endregion

        #region 应用xml布局
        /// <summary>
        /// 将xml布局描述应用到当前布局，若应用过程中若异常从状态库中删除xml，已调优
        /// </summary>
        /// <param name="p_content"></param>
        /// <returns>应用布局是否成功</returns>
        bool ApplyLayout(string p_content)
        {
            if (string.IsNullOrEmpty(p_content))
                return false;

            bool succ = true;
            _owner.IsReseting = true;
            ClearAllItems();

            try
            {
                using (StringReader reader = new StringReader(p_content))
                {
                    XDocument doc = XDocument.Load(reader);
                    if (p_content != _default)
                    {
                        // 校验标签
                        int count = 0;
                        foreach (XElement item in doc.Root.Descendants("Tab"))
                        {
                            count++;
                            XAttribute attr = item.Attribute("Title");
                            if (attr == null
                                || string.IsNullOrEmpty(attr.Value)
                                || !_tabs.ContainsKey(attr.Value))
                            {
                                succ = false;
                                break;
                            }
                        }
                        if (!succ || count != _tabs.Count)
                            throw new Exception("加载时确保标签个数和标题都能对上，否则删除历史布局！");
                    }

                    // 停靠项
                    XElement elem = doc.Root.Element("Win");
                    foreach (XElement item in elem.Elements())
                    {
                        Pane winItem = CreateWinItem(item);
                        _owner.Items.Add(winItem);
                        _owner.RootPanel.Children.Add(winItem);
                    }

                    // 中部项
                    elem = doc.Root.Element("Center");
                    if (elem != null)
                    {
                        foreach (XElement item in elem.Elements())
                        {
                            if (item.Name == "Tabs")
                                _owner.CenterItem.Items.Add(CreateTabs(item));
                            else if (item.Name == "Pane")
                                _owner.CenterItem.Items.Add(CreateWinItem(item));
                        }
                    }

                    // 左侧隐藏项
                    elem = doc.Root.Element("Left");
                    if (elem != null)
                    {
                        if (_owner.LeftAutoHide == null)
                            _owner.CreateLeftAutoHideTab();
                        foreach (XElement item in elem.Elements())
                        {
                            _owner.LeftAutoHide.Unpin(CreateTab(item));
                        }
                    }

                    // 右侧隐藏项
                    elem = doc.Root.Element("Right");
                    if (elem != null)
                    {
                        if (_owner.RightAutoHide == null)
                            _owner.CreateRightAutoHideTab();
                        foreach (XElement item in elem.Elements())
                        {
                            _owner.RightAutoHide.Unpin(CreateTab(item));
                        }
                    }

                    // 上侧隐藏项
                    elem = doc.Root.Element("Top");
                    if (elem != null)
                    {
                        if (_owner.TopAutoHide == null)
                            _owner.CreateTopAutoHideTab();
                        foreach (XElement item in elem.Elements())
                        {
                            _owner.TopAutoHide.Unpin(CreateTab(item));
                        }
                    }

                    // 下侧隐藏项
                    elem = doc.Root.Element("Bottom");
                    if (elem != null)
                    {
                        if (_owner.BottomAutoHide == null)
                            _owner.CreateBottomAutoHideTab();
                        foreach (XElement item in elem.Elements())
                        {
                            _owner.BottomAutoHide.Unpin(CreateTab(item));
                        }
                    }

                    // 浮动项
                    elem = doc.Root.Element("Float");
                    if (elem != null)
                    {
                        foreach (XElement item in elem.Elements())
                        {
                            OpenInWindow(CreateWinItem(item));
                        }
                    }
                }
            }
            catch
            {
                succ = false;
                if (AllowSaveLayout())
                    AtState.Exec($"delete from DockLayout where BaseUri=\"{_owner.BaseUri.AbsolutePath}\"");
            }
            finally
            {
                _owner.IsReseting = false;
            }
            return succ;
        }

        /// <summary>
        /// 为适应宽度自动隐藏两侧窗口
        /// </summary>
        void ApplyAutoHide()
        {
            _owner.IsReseting = true;
            ClearAllItems();

            try
            {
                using (StringReader reader = new StringReader(_default))
                {
                    XDocument doc = XDocument.Load(reader);

                    // 停靠项，xml中按左右上下顺序
                    int start = _fitCols - 1;
                    int end = _colsWidth.Count - 1;
                    int index = 0;
                    XElement elem = doc.Root.Element("Win");
                    foreach (XElement item in elem.Elements())
                    {
                        Pane di = CreateWinItem(item);
                        if (index >= start && index < end)
                        {
                            // 范围之内的放入两侧自动隐藏
                            MoveToAutoHide(di, di.Pos);
                        }
                        else
                        {
                            _owner.Items.Add(di);
                            _owner.RootPanel.Children.Add(di);
                        }
                        index++;
                    }

                    // 中部项
                    elem = doc.Root.Element("Center");
                    if (elem != null)
                    {
                        foreach (XElement item in elem.Elements())
                        {
                            if (item.Name == "Tabs")
                                _owner.CenterItem.Items.Add(CreateTabs(item));
                            else if (item.Name == "Pane")
                                _owner.CenterItem.Items.Add(CreateWinItem(item));
                        }
                    }

                    // 左侧隐藏项
                    elem = doc.Root.Element("Left");
                    if (elem != null)
                    {
                        if (_owner.LeftAutoHide == null)
                            _owner.CreateLeftAutoHideTab();
                        foreach (XElement item in elem.Elements())
                        {
                            _owner.LeftAutoHide.Unpin(CreateTab(item));
                        }
                    }

                    // 右侧隐藏项
                    elem = doc.Root.Element("Right");
                    if (elem != null)
                    {
                        if (_owner.RightAutoHide == null)
                            _owner.CreateRightAutoHideTab();
                        foreach (XElement item in elem.Elements())
                        {
                            _owner.RightAutoHide.Unpin(CreateTab(item));
                        }
                    }

                    // 上侧隐藏项
                    elem = doc.Root.Element("Top");
                    if (elem != null)
                    {
                        if (_owner.TopAutoHide == null)
                            _owner.CreateTopAutoHideTab();
                        foreach (XElement item in elem.Elements())
                        {
                            _owner.TopAutoHide.Unpin(CreateTab(item));
                        }
                    }

                    // 下侧隐藏项
                    elem = doc.Root.Element("Bottom");
                    if (elem != null)
                    {
                        if (_owner.BottomAutoHide == null)
                            _owner.CreateBottomAutoHideTab();
                        foreach (XElement item in elem.Elements())
                        {
                            _owner.BottomAutoHide.Unpin(CreateTab(item));
                        }
                    }

                    // 浮动项
                    elem = doc.Root.Element("Float");
                    if (elem != null)
                    {
                        foreach (XElement item in elem.Elements())
                        {
                            OpenInWindow(CreateWinItem(item));
                        }
                    }
                }
            }
            finally
            {
                _owner.IsReseting = false;
            }
        }

        Pane CreateWinItem(XElement p_elem)
        {
            Pane item = new Pane();
            XAttribute attr = p_elem.Attribute("Pos");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                item.Pos = (PanePosition)Enum.Parse(typeof(PanePosition), attr.Value);

            attr = p_elem.Attribute("InitWidth");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                item.InitWidth = Convert.ToDouble(attr.Value);

            attr = p_elem.Attribute("InitHeight");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                item.InitHeight = Convert.ToDouble(attr.Value);

            attr = p_elem.Attribute("Orientation");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                item.Orientation = (Orientation)Enum.Parse(typeof(Orientation), attr.Value);

            attr = p_elem.Attribute("FloatLocation");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                Point pt = new Point();
                string[] loc = attr.Value.Split(',');
                if (loc.Length == 2)
                {
                    pt.X = Convert.ToDouble(loc[0]);
                    pt.Y = Convert.ToDouble(loc[1]);
                    item.FloatLocation = pt;
                }
            }

            attr = p_elem.Attribute("FloatPos");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                item.FloatPos = (FloatPosition)Enum.Parse(typeof(FloatPosition), attr.Value);

            attr = p_elem.Attribute("FloatSize");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                Size size = new Size();
                string[] loc = attr.Value.Split(',');
                if (loc.Length == 2)
                {
                    size.Width = Convert.ToDouble(loc[0]);
                    size.Height = Convert.ToDouble(loc[1]);
                    item.FloatSize = size;
                }
            }

            foreach (XElement elem in p_elem.Elements())
            {
                if (elem.Name == "Tabs")
                    item.Items.Add(CreateTabs(elem));
                else if (elem.Name == "Pane")
                    item.Items.Add(CreateWinItem(elem));
            }

            attr = p_elem.Attribute("IsInCenter");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                item.IsInCenter = Convert.ToBoolean(attr.Value);

            attr = p_elem.Attribute("IsInWindow");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                item.IsInWindow = Convert.ToBoolean(attr.Value);
            return item;
        }

        Tabs CreateTabs(XElement p_elem)
        {
            Tabs tabs = new Tabs();
            XAttribute attr = p_elem.Attribute("InitWidth");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                tabs.InitWidth = Convert.ToDouble(attr.Value);

            attr = p_elem.Attribute("InitHeight");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                tabs.InitHeight = Convert.ToDouble(attr.Value);

            attr = p_elem.Attribute("Padding");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                string[] padding = attr.Value.Split(',');
                if (padding.Length == 4)
                {
                    double left = Convert.ToDouble(padding[0]);
                    double top = Convert.ToDouble(padding[1]);
                    double right = Convert.ToDouble(padding[2]);
                    double bottom = Convert.ToDouble(padding[3]);
                    tabs.Padding = new Thickness(left, top, right, bottom);
                }
            }

            foreach (XElement elem in p_elem.Elements("Tab"))
            {
                tabs.Items.Add(CreateTab(elem));
            }

            // 索引需后设置
            attr = p_elem.Attribute("SelectedIndex");
            if (attr != null && int.TryParse(attr.Value, out int index))
                tabs.SelectedIndex = index;
            else
                tabs.SelectedIndex = 0;
            return tabs;
        }

        Tab CreateTab(XElement p_elem)
        {
            Tab tab;
            XAttribute attr = p_elem.Attribute("Title");
            if (attr == null
                || string.IsNullOrEmpty(attr.Value)
                || !_tabs.TryGetValue(attr.Value, out tab))
                return new Tab();

            tab.Title = attr.Value;
            attr = p_elem.Attribute("Name");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                tab.Name = attr.Value;

            attr = p_elem.Attribute("IsPinned");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                tab.IsPinned = Convert.ToBoolean(attr.Value);
            else
                tab.ClearValue(Tab.IsPinnedProperty);

            attr = p_elem.Attribute("CanDockInCenter");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                tab.CanDockInCenter = Convert.ToBoolean(attr.Value);

            attr = p_elem.Attribute("CanDock");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                tab.CanDock = Convert.ToBoolean(attr.Value);

            attr = p_elem.Attribute("CanFloat");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                tab.CanFloat = Convert.ToBoolean(attr.Value);

            attr = p_elem.Attribute("CanUserPin");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                tab.CanUserPin = Convert.ToBoolean(attr.Value);

            attr = p_elem.Attribute("PopHeight");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                tab.PopHeight = Convert.ToDouble(attr.Value);
            else
                tab.ClearValue(Tab.PopHeightProperty);

            attr = p_elem.Attribute("PopWidth");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                tab.PopWidth = Convert.ToDouble(attr.Value);
            else
                tab.ClearValue(Tab.PopWidthProperty);

            // 避免旧Tabs的影响
            tab.Owner = null;
            // 清除旧的选择标志
            tab.ClearValue(Tab.IsSelectedProperty);
            return tab;
        }
        #endregion

        #region 输出xml
        /// <summary>
        /// 将当前布局输出为xml
        /// </summary>
        /// <returns></returns>
        string WriteXml()
        {
            _owner.UpdateLayout();
            StringBuilder xml = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(xml, _writerSettings))
            {
                writer.WriteStartElement("Items");

                // 停靠项
                writer.WriteStartElement("Win");
                foreach (Pane wi in _owner.Items.OfType<Pane>())
                {
                    WriteWinItem(wi, writer);
                }
                writer.WriteEndElement();

                // 中部项
                WriteCenter(writer);

                // 左侧隐藏项
                if (_owner.LeftAutoHide?.Items.Count > 0)
                {
                    writer.WriteStartElement("Left");
                    foreach (Tab tab in _owner.LeftAutoHide.Items.OfType<Tab>())
                    {
                        WriteTab(tab, writer);
                    }
                    writer.WriteEndElement();
                }

                // 右侧隐藏项
                if (_owner.RightAutoHide?.Items.Count > 0)
                {
                    writer.WriteStartElement("Right");
                    foreach (Tab tab in _owner.RightAutoHide.Items.OfType<Tab>())
                    {
                        WriteTab(tab, writer);
                    }
                    writer.WriteEndElement();
                }

                // 上侧隐藏项
                if (_owner.TopAutoHide?.Items.Count > 0)
                {
                    writer.WriteStartElement("Top");
                    foreach (Tab tab in _owner.TopAutoHide.Items.OfType<Tab>())
                    {
                        WriteTab(tab, writer);
                    }
                    writer.WriteEndElement();
                }

                // 下侧隐藏项
                if (_owner.BottomAutoHide?.Items.Count > 0)
                {
                    writer.WriteStartElement("Bottom");
                    foreach (Tab tab in _owner.BottomAutoHide.Items.OfType<Tab>())
                    {
                        WriteTab(tab, writer);
                    }
                    writer.WriteEndElement();
                }

                // 浮动项
                var fl = _owner.FloatItems.ToList();
                if (fl.Count > 0)
                {
                    writer.WriteStartElement("Float");
                    foreach (Pane wi in fl)
                    {
                        WriteWinItem(wi, writer);
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.Flush();
            }
            return xml.ToString();
        }

        void WriteCenter(XmlWriter p_writer)
        {
            var ls = _owner.CenterItem.Items;
            if (ls.Count == 0)
                return;

            var mainTabs = _owner.GetValue(Win.MainTabsProperty);

            // 只动态主区不记录
            if (ls.Count == 1 && ls[0] == mainTabs)
                return;

            p_writer.WriteStartElement("Center");
            foreach (var obj in ls)
            {
                if (obj is Tabs tabs && tabs != mainTabs)
                {
                    WriteTabs(tabs, p_writer);
                }
                else if (obj is Pane wi)
                {
                    WriteWinItem(wi, p_writer);
                }
            }
            p_writer.WriteEndElement();
        }

        void WriteWinItem(Pane p_item, XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Pane");

            if (p_item.ReadLocalValue(Pane.PosProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("Pos", p_item.Pos.ToString());

            if (!double.IsNaN(p_item.ActualWidth) && p_item.ActualWidth > 0)
                p_writer.WriteAttributeString("InitWidth", p_item.ActualWidth.ToString());
            else if (p_item.ReadLocalValue(Pane.InitWidthProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("InitWidth", p_item.InitWidth.ToString());

            if (!double.IsNaN(p_item.ActualHeight) && p_item.ActualHeight > 0)
                p_writer.WriteAttributeString("InitHeight", p_item.ActualHeight.ToString());
            else if (p_item.ReadLocalValue(Pane.InitHeightProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("InitHeight", p_item.InitHeight.ToString());

            if (p_item.ReadLocalValue(Pane.OrientationProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("Orientation", p_item.Orientation.ToString());

            ToolWindow win = p_item.Parent as ToolWindow;
            if (win != null)
            {
                p_writer.WriteAttributeString("FloatLocation", string.Format("{0},{1}", Math.Ceiling(win.HorizontalOffset), Math.Ceiling(win.VerticalOffset)));
                p_writer.WriteAttributeString("FloatSize", string.Format("{0},{1}", Math.Ceiling(win.ActualWidth), Math.Ceiling(win.ActualHeight)));
            }
            else
            {
                if (p_item.ReadLocalValue(Pane.FloatLocationProperty) != DependencyProperty.UnsetValue)
                    p_writer.WriteAttributeString("FloatLocation", string.Format("{0},{1}", p_item.FloatLocation.X, p_item.FloatLocation.Y));
                else if (p_item.ReadLocalValue(Pane.FloatPosProperty) != DependencyProperty.UnsetValue)
                    p_writer.WriteAttributeString("FloatPos", p_item.FloatPos.ToString());

                if (p_item.ReadLocalValue(Pane.FloatSizeProperty) != DependencyProperty.UnsetValue)
                    p_writer.WriteAttributeString("FloatSize", string.Format("{0},{1}", p_item.FloatSize.Width, p_item.FloatSize.Height));
            }

            if (p_item.IsInCenter)
                p_writer.WriteAttributeString("IsInCenter", "True");

            if (p_item.IsInWindow)
                p_writer.WriteAttributeString("IsInWindow", "True");

            foreach (object obj in p_item.Items)
            {
                if (obj is Tabs tabs)
                {
                    WriteTabs(tabs, p_writer);
                }
                else if (obj is Pane wi)
                {
                    WriteWinItem(wi, p_writer);
                }
            }
            p_writer.WriteEndElement();
        }

        void WriteTabs(Tabs p_sect, XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Tabs");
            if (p_sect.ReadLocalValue(TabControl.SelectedIndexProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("SelectedIndex", p_sect.SelectedIndex.ToString());

            if (!double.IsNaN(p_sect.ActualWidth) && p_sect.ActualWidth > 0)
                p_writer.WriteAttributeString("InitWidth", p_sect.ActualWidth.ToString());
            else if (p_sect.ReadLocalValue(Tabs.InitWidthProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("InitWidth", p_sect.InitWidth.ToString());

            if (!double.IsNaN(p_sect.ActualHeight) && p_sect.ActualHeight > 0)
                p_writer.WriteAttributeString("InitHeight", p_sect.ActualHeight.ToString());
            else if (p_sect.ReadLocalValue(Tabs.InitHeightProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("InitHeight", p_sect.InitHeight.ToString());

            if (p_sect.ReadLocalValue(Tabs.PaddingProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("Padding", string.Format("{0},{1},{2},{3}", p_sect.Padding.Left, p_sect.Padding.Top, p_sect.Padding.Right, p_sect.Padding.Bottom));

            foreach (Tab tab in p_sect.Items.OfType<Tab>())
            {
                // 不输出自动隐藏的项
                if (tab.IsPinned && !string.IsNullOrEmpty(tab.Title))
                    WriteTab(tab, p_writer);
            }
            p_writer.WriteEndElement();
        }

        void WriteTab(Tab p_item, XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Tab");
            p_writer.WriteAttributeString("Title", p_item.Title);

            if (!string.IsNullOrEmpty(p_item.Name))
                p_writer.WriteAttributeString("Name", p_item.Name);

            if (p_item.ReadLocalValue(Tab.IsPinnedProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("IsPinned", p_item.IsPinned.ToString());

            if (p_item.ReadLocalValue(Tab.CanDockInCenterProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("CanDockInCenter", p_item.CanDockInCenter.ToString());

            if (p_item.ReadLocalValue(Tab.CanDockProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("CanDock", p_item.CanDock.ToString());

            if (p_item.ReadLocalValue(Tab.CanFloatProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("CanFloat", p_item.CanFloat.ToString());

            if (p_item.ReadLocalValue(Tab.CanUserPinProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("CanUserPin", p_item.CanUserPin.ToString());

            if (p_item.ReadLocalValue(TabItem.PopWidthProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("PopWidth", Math.Ceiling(p_item.PopWidth).ToString());

            if (p_item.ReadLocalValue(TabItem.PopHeightProperty) != DependencyProperty.UnsetValue)
                p_writer.WriteAttributeString("PopHeight", Math.Ceiling(p_item.PopHeight).ToString());

            p_writer.WriteEndElement();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 深度查找所有Tab项，构造以Tab.Title为键以Tab为值的字典，Title不为空
        /// </summary>
        /// <param name="p_items"></param>
        void ExtractItems(IPaneList p_items)
        {
            foreach (var item in p_items.Items)
            {
                if (item is Tabs tabs)
                {
                    foreach (var tab in tabs.Items.OfType<Tab>())
                    {
                        string title = tab.Title;
                        if (string.IsNullOrEmpty(title))
                        {
                            // Tab无标题时内部生成id代替标题，id每次不同因此无法加载历史布局！
                            title = Kit.NewGuid.Substring(0, 6);
                            tab.Title = title;
                        }
                        tab.OwnWin = _owner;
                        _tabs[title] = tab;
                    }
                }
                else if (item is IPaneList child)
                {
                    ExtractItems(child);
                }
            }
        }

        /// <summary>
        /// 深度移除所有子项
        /// </summary>
        void ClearAllItems()
        {
            ClearItems(_owner.CenterItem);
            ClearItems(_owner);
            _owner.LeftAutoHide?.Items.Clear();
            _owner.RightAutoHide?.Items.Clear();
            _owner.TopAutoHide?.Items.Clear();
            _owner.BottomAutoHide?.Items.Clear();
            _owner.RootPanel.Clear();
            _owner.ClearWindows();
        }

        /// <summary>
        /// 深度查找所有自动隐藏项，同步移除
        /// </summary>
        /// <param name="p_items"></param>
        /// <returns></returns>
        IEnumerable<Tab> GetHideItems(IPaneList p_items)
        {
            foreach (var item in p_items.Items)
            {
                if (item is Tabs tabs)
                {
                    int index = 0;
                    while (index < tabs.Items.Count)
                    {
                        Tab si = tabs.Items[index] as Tab;
                        if (si != null && !si.IsPinned)
                        {
                            tabs.Items.RemoveAt(index);
                            yield return si;
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
                else if (item is IPaneList child)
                {
                    foreach (Tab si in GetHideItems(child))
                    {
                        yield return si;
                    }
                }
            }
        }

        /// <summary>
        /// 将内部所有的Tab转移到两侧隐藏
        /// </summary>
        /// <param name="p_items"></param>
        /// <param name="p_state"></param>
        void MoveToAutoHide(IPaneList p_items, PanePosition p_state)
        {
            foreach (var item in p_items.Items)
            {
                if (item is Tabs tabs)
                {
                    int index = 0;
                    while (index < tabs.Items.Count)
                    {
                        Tab si = tabs.Items[index] as Tab;
                        if (si != null)
                        {
                            tabs.Items.RemoveAt(index);
                            if (p_state == PanePosition.Left)
                            {
                                if (_owner.LeftAutoHide == null)
                                    _owner.CreateLeftAutoHideTab();
                                _owner.LeftAutoHide.Unpin(si);
                            }
                            else if (p_state == PanePosition.Right)
                            {
                                if (_owner.RightAutoHide == null)
                                    _owner.CreateRightAutoHideTab();
                                _owner.RightAutoHide.Unpin(si);
                            }
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
                else if (item is IPaneList child)
                {
                    MoveToAutoHide(child, p_state);
                }
            }
        }

        /// <summary>
        /// 构造ToolWindow承载Pane
        /// </summary>
        /// <param name="p_winItem"></param>
        /// <returns></returns>
        ToolWindow OpenInWindow(Pane p_winItem)
        {
            ToolWindow win = _owner.CreateWindow(p_winItem.FloatSize, p_winItem.FloatLocation);
            if (p_winItem.ReadLocalValue(Pane.FloatLocationProperty) == DependencyProperty.UnsetValue)
            {
                // 默认位置
                switch (p_winItem.FloatPos)
                {
                    case FloatPosition.Center:
                        win.HorizontalOffset = Math.Ceiling((Kit.ViewWidth - p_winItem.FloatSize.Width) / 2);
                        win.VerticalOffset = Math.Ceiling((Kit.ViewHeight - p_winItem.FloatSize.Height) / 2);
                        break;
                    case FloatPosition.TopLeft:
                        win.HorizontalOffset = 0;
                        win.VerticalOffset = 0;
                        break;
                    case FloatPosition.TopRight:
                        win.HorizontalOffset = Kit.ViewWidth - p_winItem.FloatSize.Width;
                        win.VerticalOffset = 0;
                        break;
                    case FloatPosition.BottomLeft:
                        win.HorizontalOffset = 0;
                        win.VerticalOffset = Kit.ViewHeight - p_winItem.FloatSize.Height;
                        break;
                    case FloatPosition.BottomRight:
                        win.HorizontalOffset = Kit.ViewWidth - p_winItem.FloatSize.Width;
                        win.VerticalOffset = Kit.ViewHeight - p_winItem.FloatSize.Height;
                        break;
                }
            }

            win.Content = p_winItem;
            win.Show();
            return win;
        }

        /// <summary>
        /// 判断Pane是否需要从布局中移除
        /// </summary>
        /// <param name="p_di"></param>
        /// <returns></returns>
        bool IsRemoved(Pane p_di)
        {
            foreach (object item in p_di.Items)
            {
                Pane di;
                Tabs tabs = item as Tabs;

                // 因之前已将IsPinned = false的所有Tab移除
                if (tabs != null && tabs.Items.Count > 0)
                    return false;
                else if ((di = item as Pane) != null)
                {
                    if (!IsRemoved(di))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 移除当前Pane子项中不需要的Pane
        /// </summary>
        /// <param name="p_di"></param>
        void RemoveUnused(Pane p_di)
        {
            var dis = from item in p_di.Items
                      where item is Pane
                      select (item as Pane);

            foreach (Pane di in dis)
            {
                if (IsRemoved(di))
                    p_di.Items.Remove(di);
                else
                    RemoveUnused(di);
            }
        }

        bool AllowSaveLayout()
        {
            // 为内嵌窗口时不保存布局
            return _owner.AutoSaveLayout
                && _owner.BaseUri != null
                && _fitCols == -1
                && !_owner.IsInnerWin;
        }
        #endregion
    }
}
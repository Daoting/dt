#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 文件格
    /// </summary>
    public partial class CFile : FvCell, IMenuHost
    {
        #region 静态内容
        public static readonly DependencyProperty ToolbarProperty = DependencyProperty.Register(
            "Toolbar",
            typeof(FrameworkElement),
            typeof(CFile),
            new PropertyMetadata(null, OnToolbarChanged));

        public static readonly DependencyProperty ShowDefaultToolbarProperty = DependencyProperty.Register(
            "ShowDefaultToolbar",
            typeof(bool),
            typeof(CFile),
            new PropertyMetadata(true, OnShowToolbarChanged));

        public static readonly DependencyProperty ShowDefaultMenuProperty = DependencyProperty.Register(
            "ShowDefaultMenu",
            typeof(bool),
            typeof(CFile),
            new PropertyMetadata(true, OnShowMenuChanged));

        static void OnToolbarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CFile c = (CFile)d;
            var bar = (FrameworkElement)e.NewValue;
            if (bar != null)
                bar.DataContext = c._fl;
        }

        static void OnShowToolbarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CFile)d).LoadDefaultToolbar();
        }

        static void OnShowMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CFile c = (CFile)d;
            if (Ex.GetMenu(c) == null)
            {
                if ((bool)e.NewValue)
                    c.UpdateDefaultMenu();
                else
                    c._fl.ClearValue(Ex.MenuProperty);
            }
        }
        #endregion

        #region 成员变量
        readonly FileList _fl;
        FrameworkElement _toolbar;
        #endregion

        #region 构造方法
        public CFile()
        {
            DefaultStyleKey = typeof(CFile);

            _fl = new FileList();
            // 自动行高
            RowSpan = -1;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置列数，默认1列
        /// </summary>
        [CellParam("每行文件数")]
        public int ColCount
        {
            get { return _fl.ColCount; }
            set { _fl.ColCount = value; }
        }

        /// <summary>
        /// 获取设置文件项之间的间隔距离，默认0
        /// </summary>
        [CellParam("文件项间距")]
        public double Spacing
        {
            get { return _fl.Spacing; }
            set { _fl.Spacing = value; }
        }

        /// <summary>
        /// 获取设置图像的显示高度，默认82，0表示和宽度相同
        /// </summary>
        [CellParam("图像高度")]
        public double ImageHeight
        {
            get { return _fl.ImageHeight; }
            set { _fl.ImageHeight = value; }
        }

        /// <summary>
        ///  获取设置图像填充模式，默认Uniform
        /// </summary>
        [CellParam("图像填充模式")]
        public Stretch ImageStretch
        {
            get { return _fl.ImageStretch; }
            set { _fl.ImageStretch = value; }
        }

        /// <summary>
        /// 获取设置文件项是否可点击，默认true
        /// </summary>
        [CellParam("文件项可点击")]
        public bool EnableClick
        {
            get { return _fl.EnableClick; }
            set { _fl.EnableClick = value; }
        }

        /// <summary>
        /// 获取设置文件数量上限，默认int.MaxValue
        /// </summary>
        [CellParam("文件数量上限")]
        public int MaxFileCount
        {
            get { return _fl.MaxFileCount; }
            set { _fl.MaxFileCount = value; }
        }

        /// <summary>
        /// 获取设置要上传的固定卷名，默认null表示上传到普通卷
        /// </summary>
        [CellParam("固定卷名")]
        public string FixedVolume
        {
            get { return _fl.FixedVolume; }
            set { _fl.FixedVolume = value; }
        }

        /// <summary>
        /// 获取设置工具栏内容
        /// </summary>
        public FrameworkElement Toolbar
        {
            get { return (FrameworkElement)GetValue(ToolbarProperty); }
            set { SetValue(ToolbarProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示默认工具栏，默认true
        /// </summary>
        [CellParam("显示默认工具栏")]
        public bool ShowDefaultToolbar
        {
            get { return (bool)GetValue(ShowDefaultToolbarProperty); }
            set { SetValue(ShowDefaultToolbarProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示默认菜单，默认true
        /// </summary>
        [CellParam("显示默认菜单")]
        public bool ShowDefaultMenu
        {
            get { return (bool)GetValue(ShowDefaultMenuProperty); }
            set { SetValue(ShowDefaultMenuProperty, value); }
        }

        /// <summary>
        /// 获取内部的文件列表
        /// </summary>
        public FileList FileList
        {
            get { return _fl; }
        }
        #endregion

        #region 重写方法
        protected override void OnApplyCellTemplate()
        {
            var grid = (Grid)GetTemplateChild("RootGrid");
            Grid.SetRow(_fl, 1);
            grid.Children.Add(_fl);
            LoadDefaultToolbar();
            UpdateDefaultMenu();
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (Owner != null)
                Owner.OnCellClick(this);
        }

        protected override void SetValBinding()
        {
            _fl.SetBinding(FileList.DataProperty, ValBinding);
        }

        protected override void OnReadOnlyChanged()
        {
            LoadDefaultToolbar();
            UpdateDefaultMenu();
        }
        #endregion

        #region IMenuHost
        /// <summary>
        /// 切换上下文菜单或修改触发事件种类时通知宿主刷新
        /// </summary>
        void IMenuHost.UpdateContextMenu()
        {
            Menu menu = Ex.GetMenu(this);
            if (menu != null)
                Ex.SetMenu(_fl, menu);
            else
                UpdateDefaultMenu();
        }
        #endregion

        #region 内部方法
        void LoadDefaultToolbar()
        {
            var bar = Toolbar;

            // 外部自定义
            if (bar != null && bar != _toolbar)
                return;

            // 不显示 或 只读
            if (!ShowDefaultToolbar || ReadOnlyBinding)
            {
                ClearValue(ToolbarProperty);
                return;
            }

            if (_toolbar == null)
            {
                Button btn = new Button { Content = "\uE038", Style = Res.字符按钮 };
                if (Type.GetType(FileItem.SelectFileDlgType) != null)
                {
                    var menu = new Menu { Placement = MenuPosition.BottomLeft };
                    Mi mi = new Mi { ID = "上传文件", Icon = Icons.曲别针 };
                    mi.Click += (s, e) => OnAddFile();
                    menu.Items.Add(mi);

                    mi = new Mi { ID = "选择文件", Icon = Icons.文件夹 };
                    mi.Click += OnSelectFiles;
                    menu.Items.Add(mi);
                    Ex.SetMenu(btn, menu);
                }
                else
                {
                    btn.Click += (s, e) => OnAddFile();
                }
                _toolbar = btn;
            }
            Toolbar = _toolbar;
        }

        void UpdateDefaultMenu()
        {
            if (Ex.GetMenu(this) != null || !ShowDefaultMenu)
                return;

            Menu menu = new Menu();
            Mi mi = new Mi { ID = "分享", Icon = Icons.分享 };
            mi.SetBinding(Mi.CmdProperty, new Binding { Path = new PropertyPath("CmdShare") });
            menu.Items.Add(mi);

            mi = new Mi { ID = "保存", Icon = Icons.保存 };
            mi.SetBinding(Mi.CmdProperty, new Binding { Path = new PropertyPath("CmdSaveAs") });
            menu.Items.Add(mi);

            if (!ReadOnlyBinding)
            {
                mi = new Mi { ID = "更新", Icon = Icons.刷新 };
                mi.SetBinding(Mi.CmdProperty, new Binding { Path = new PropertyPath("CmdUpdate") });
                menu.Items.Add(mi);

                mi = new Mi { ID = "删除", Icon = Icons.删除 };
                mi.SetBinding(Mi.CmdProperty, new Binding { Path = new PropertyPath("CmdDelete") });
                menu.Items.Add(mi);
            }
            Ex.SetMenu(_fl, menu);
        }

        void OnAddFile()
        {
            if (ValBinding.Source != null)
                _fl.AddFile();
        }

        async void OnSelectFiles(object sender, Mi e)
        {
            var dlg = (ISelectFileDlg)Activator.CreateInstance(Type.GetType(FileItem.SelectFileDlgType));
            if (await dlg.Show(MaxFileCount > 1, null)
                && dlg.SelectedFiles != null
                && dlg.SelectedFiles.Count > 0)
            {
                _fl.AddExistFiles(dlg.SelectedFiles);
            }
        }
        #endregion
    }

    public interface ISelectFileDlg
    {
        /// <summary>
        /// 已选择的文件列表，每个字符串为独立的文件描述json，如：["v0/52/37/142888904373956608.xlsx","12","xlsx文件",8153,"daoting","2020-10-29 15:09"]
        /// </summary>
        List<string> SelectedFiles { get; set; }

        /// <summary>
        /// 显示文件选择对话框
        /// </summary>
        /// <param name="p_isMultiSelection">是否允许多选</param>
        /// <param name="p_typeFilter">按文件扩展名过滤</param>
        /// <returns></returns>
        Task<bool> Show(bool p_isMultiSelection, string p_typeFilter);
    }
}
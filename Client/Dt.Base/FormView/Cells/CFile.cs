#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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
            typeof(UIElement),
            typeof(CFile),
            new PropertyMetadata(null));

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
        UIElement _toolbar;
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
        /// 获取设置图像的显示高度，默认82，0表示和宽度相同
        /// </summary>
        [CellParam("图像高度")]
        public double ImageHeight
        {
            get { return _fl.ImageHeight; }
            set { _fl.ImageHeight = value; }
        }

        /// <summary>
        /// 获取设置图像边距，默认6
        /// </summary>
        public Thickness ImagePadding
        {
            get { return _fl.ImagePadding; }
            set { _fl.ImagePadding = value; }
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
        public UIElement Toolbar
        {
            get { return (UIElement)GetValue(ToolbarProperty); }
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
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

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
                Button btn = new Button { Content = "\uE080", Style = AtRes.字符按钮 };
                btn.Click += (s, e) => _fl.AddFile();
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
        #endregion
    }
}
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using System.Reflection;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 简单选择器
    /// </summary>
    [ContentProperty(Name = nameof(View))]
    public partial class CPick : FvCell
    {
        #region 静态内容
        public readonly static DependencyProperty SrcIDProperty = DependencyProperty.Register(
            "SrcID",
            typeof(string),
            typeof(CPick),
            new PropertyMetadata(null));

        public readonly static DependencyProperty TgtIDProperty = DependencyProperty.Register(
            "TgtID",
            typeof(string),
            typeof(CPick),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ShowButtonProperty = DependencyProperty.Register(
            "ShowButton",
            typeof(bool),
            typeof(CPick),
            new PropertyMetadata(false));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(CPick),
            new PropertyMetadata(null));

        #endregion

        #region 成员变量
        readonly Lv _lv;
        Grid _grid;
        PickDlg _dlg;
        string[] _srcIDs;
        string[] _tgtIDs;
        #endregion

        #region 构造方法
        public CPick()
        {
            DefaultStyleKey = typeof(CPick);
            _lv = new Lv();
            _lv.Toolbar = new Menu();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 查询事件
        /// </summary>
        public event Action<CPick, string> Search;

        /// <summary>
        /// 选择前事件，支持异步，可取消选择
        /// </summary>
        public event Action<CPick, AsyncCancelArgs> Picking;

        /// <summary>
        /// 选择后事件
        /// </summary>
        public event Action<CPick> Picked;

        /// <summary>
        /// 筛选按钮事件
        /// </summary>
        public event Action<CPick> BtnClick;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置源属性列表，用'#'隔开
        /// </summary>
        [CellParam("源属性列表")]
        public string SrcID
        {
            get { return (string)GetValue(SrcIDProperty); }
            set { SetValue(SrcIDProperty, value); }
        }

        /// <summary>
        /// 获取设置目标属性列表，用'#'隔开
        /// </summary>
        [CellParam("目标属性列表")]
        public string TgtID
        {
            get { return (string)GetValue(TgtIDProperty); }
            set { SetValue(TgtIDProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示筛选按钮
        /// </summary>
        [CellParam("显示筛选按钮")]
        public bool ShowButton
        {
            get { return (bool)GetValue(ShowButtonProperty); }
            set { SetValue(ShowButtonProperty, value); }
        }

        /// <summary>
        /// 获取设置当前值
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 下拉框
        /// </summary>
        public PickDlg Dlg => _dlg;

        internal double CurrentWidth => _grid.ActualWidth > 0 ? _grid.ActualWidth : 300;
        
        protected override IFvCall DefaultMiddle => new ListValConverter();
        #endregion

        #region 重写方法
        protected override void OnApplyCellTemplate()
        {
            _grid = (Grid)GetTemplateChild("Grid");
            
            var border = (Border)GetTemplateChild("TextBorder");
#if WIN
            // TextBlock可复制
            border.AddHandler(TappedEvent, new TappedEventHandler(OnShowDlg), true);
#else
            border.Tapped += OnShowDlg;
#endif

            var btn = (Button)GetTemplateChild("BtnFilter");
            if (btn != null)
            {
                btn.Click -= OnButtonClick;
                btn.Click += OnButtonClick;
            }

            // 拆分填充列
            if (!string.IsNullOrEmpty(SrcID) && !string.IsNullOrEmpty(TgtID))
            {
                _srcIDs = SrcID.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                _tgtIDs = TgtID.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                if (_srcIDs.Length != _tgtIDs.Length)
                {
                    _srcIDs = null;
                    _tgtIDs = null;
                    Kit.Error("数据填充：源列表、目标列表列个数不一致！");
                }
            }
        }

        protected override void SetValBinding()
        {
            SetBinding(TextProperty, ValBinding);
        }

        protected override bool SetFocus()
        {
            if (_grid != null && !ReadOnlyBinding)
            {
                OnShowDlg(null, null);
                return true;
            }
            return false;
        }
        #endregion

        #region 内部方法
        void OnShowDlg(object sender, TappedRoutedEventArgs e)
        {
            if (ReadOnlyBinding)
                return;

            if (_dlg != null && _dlg.IsOpened)
            {
                _dlg.BringToTop();
                return;
            }

            if (_dlg == null)
            {
                if (Kit.IsPhoneUI)
                {
                    _dlg = new PickDlg(this);
                }
                else
                {
                    _dlg = new PickDlg(this)
                    {
                        WinPlacement = DlgPlacement.TargetTopLeft,
                        PlacementTarget = _grid,
                        MaxHeight = 400,
                    };
                }
            }
            _dlg.Show();
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            BtnClick?.Invoke(this);
        }

        internal void OnSearch(string p_txt)
        {
            Search?.Invoke(this, p_txt);
        }
        #endregion

        #region 选择
        internal async void OnPicking()
        {
            if (_lv.SelectedItem == null)
                return;

            // 外部判断选择项是否允许
            if (Picking != null)
            {
                var args = new AsyncCancelArgs();
                Picking(this, args);
                await args.EnsureAllCompleted();
                if (args.Cancel)
                    return;
            }

            if (_lv.SelectionMode == SelectionMode.Single)
            {
                // 同步填充目标Cell的值
                FillCells(_lv.SelectedItem);
            }

            Picked?.Invoke(this);
            _dlg.Close();
            Owner.GotoNextCell(this);
        }

        /// <summary>
        /// 同步填充目标Cell的值
        /// </summary>
        /// <param name="p_selectedItem"></param>
        public void FillCells(object p_selectedItem)
        {
            // 无需要填充的
            if (_srcIDs == null || p_selectedItem == null)
                return;

            // 同步填充
            if (p_selectedItem is Row srcRow)
            {
                object tgtObj = Owner.Data;
                Row tgtRow = tgtObj as Row;
                for (int i = 0; i < _srcIDs.Length; i++)
                {
                    string srcID = _srcIDs[i];
                    string tgtID = _tgtIDs[i];
                    if (srcRow.Contains(srcID))
                    {
                        if (tgtRow != null)
                        {
                            if (tgtRow.Contains(tgtID))
                                tgtRow[tgtID] = srcRow[srcID];
                        }
                        else
                        {
                            var pi = tgtObj.GetType().GetProperty(tgtID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (pi != null)
                                pi.SetValue(tgtObj, srcRow[srcID]);
                        }
                    }
                }
            }
            else
            {
                object tgtObj = Owner.Data;
                Row tgtRow = tgtObj as Row;
                for (int i = 0; i < _srcIDs.Length; i++)
                {
                    var srcPi = p_selectedItem.GetType().GetProperty(_srcIDs[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (srcPi != null)
                    {
                        string tgtID = _tgtIDs[i];
                        if (tgtRow != null)
                        {
                            if (tgtRow.Contains(tgtID))
                                tgtRow[tgtID] = srcPi.GetValue(p_selectedItem);
                        }
                        else
                        {
                            var tgtPi = tgtObj.GetType().GetProperty(tgtID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (tgtPi != null)
                                tgtPi.SetValue(tgtObj, srcPi.GetValue(p_selectedItem));
                        }
                    }
                }
            }
        }
        #endregion
    }
}
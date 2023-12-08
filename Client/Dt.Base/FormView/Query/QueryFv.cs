#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-01-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 查询面板
    /// </summary>
    public partial class QueryFv : Fv
    {
        #region 静态内容
        public readonly static DependencyProperty IsAndProperty = DependencyProperty.Register(
            "IsAnd",
            typeof(bool),
            typeof(QueryFv),
            new PropertyMetadata(true));

        public readonly static DependencyProperty ShowQueryButtonProperty = DependencyProperty.Register(
            "ShowQueryButton",
            typeof(bool),
            typeof(QueryFv),
            new PropertyMetadata(true));
        #endregion

        #region 成员变量
        BaseCommand _cmdQuery;
        BaseCommand _cmdReset;
        #endregion

        #region 构造方法
        public QueryFv()
        {
            DefaultStyleKey = typeof(QueryFv);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 查询事件
        /// </summary>
        public event Action<QueryClause> Query;
        #endregion

        /// <summary>
        /// 获取设置筛选条件之间是否为【与】操作，默认true，false时【或】操作
        /// </summary>
        public bool IsAnd
        {
            get { return (bool)GetValue(IsAndProperty); }
            set { SetValue(IsAndProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示查询、重置按钮，默认true
        /// </summary>
        public bool ShowQueryButton
        {
            get { return (bool)GetValue(ShowQueryButtonProperty); }
            set { SetValue(ShowQueryButtonProperty, value); }
        }

        public void OnQuery()
        {
            Query?.Invoke(new QueryClause(this));
        }

        public void OnReset()
        {
            if (Data is Row row)
                row.RejectChanges();
        }

        #region 命令对象
        /// <summary>
        /// 获取查询命令
        /// </summary>
        public BaseCommand CmdQuery
        {
            get
            {
                if (_cmdQuery == null)
                    _cmdQuery = new BaseCommand(p_params => OnQuery());
                return _cmdQuery;
            }
        }

        /// <summary>
        /// 获取重置命令
        /// </summary>
        public BaseCommand CmdReset
        {
            get
            {
                if (_cmdReset == null)
                {
                    _cmdReset = new BaseCommand(p_params => OnReset());
                }
                return _cmdReset;
            }
        }
        #endregion

        #region 重写方法
        protected override void OnLoadTemplate()
        {
            base.OnLoadTemplate();

            var btn = (Button)GetTemplateChild("BtnQuery");
            if (btn != null)
            {
                btn.Click += (s, e) => OnQuery();
            }

            btn = (Button)GetTemplateChild("BtnReset");
            if (btn != null)
            {
                btn.Click += (s, e) => OnReset();
            }
        }
        #endregion

    }
}
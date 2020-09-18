#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Collections;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 分页数据源
    /// </summary>
    public partial class PageData : DependencyObject
    {
        #region 静态内容
        /// <summary>
        /// sql字典键值
        /// </summary>
        public static readonly DependencyProperty SqlProperty = DependencyProperty.Register(
            "Sql",
            typeof(string),
            typeof(PageData),
            new PropertyMetadata(null));

        /// <summary>
        /// sql查询语句，语句中不需包含分页功能
        /// </summary>
        public static readonly DependencyProperty SqlStrProperty = DependencyProperty.Register(
            "SqlStr",
            typeof(string),
            typeof(PageData),
            new PropertyMetadata(null));

        /// <summary>
        /// sql查询参数值字典
        /// </summary>
        public static readonly DependencyProperty SqlParamsProperty = DependencyProperty.Register(
            "SqlParams",
            typeof(Dict),
            typeof(PageData),
            new PropertyMetadata(null));

        /// <summary>
        /// 进入下一页面的回调方法
        /// </summary>
        public static readonly DependencyProperty NextPageProperty = DependencyProperty.Register(
            "NextPage",
            typeof(Action<PageData>),
            typeof(PageData),
            new PropertyMetadata(null));

        /// <summary>
        /// 页面行数
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register(
            "PageSize",
            typeof(int),
            typeof(PageData),
            new PropertyMetadata(30));

        /// <summary>
        /// 是否将新页面数据插入到头部
        /// </summary>
        public static readonly DependencyProperty InsertTopProperty = DependencyProperty.Register(
            "InsertTop",
            typeof(bool),
            typeof(PageData),
            new PropertyMetadata(false));
        #endregion

        #region 成员变量
        Lv _owner;
        int _pageNo;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置sql字典键值，语句中不需包含分页功能
        /// </summary>
        public string Sql
        {
            get { return (string)GetValue(SqlProperty); }
            set { SetValue(SqlProperty, value); }
        }

        /// <summary>
        /// 获取设置sql查询语句，语句中不需包含分页功能
        /// </summary>
        public string SqlStr
        {
            get { return (string)GetValue(SqlStrProperty); }
            set { SetValue(SqlStrProperty, value); }
        }

        /// <summary>
        /// 获取设置sql查询参数值字典
        /// </summary>
        public Dict SqlParams
        {
            get { return (Dict)GetValue(SqlParamsProperty); }
            set { SetValue(SqlParamsProperty, value); }
        }

        /// <summary>
        /// 获取设置进入下一页面的回调方法，通过方法内调用LoadPageData加载下页数据。
        /// 未采用返回下页数据的方式，因获取数据可能异步且需要转INotifyList写法麻烦！
        /// </summary>
        public Action<PageData> NextPage
        {
            get { return (Action<PageData>)GetValue(NextPageProperty); }
            set { SetValue(NextPageProperty, value); }
        }

        /// <summary>
        /// 获取设置页面行数
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        /// <summary>
        /// 获取设置是否还有未加载的页面
        /// </summary>
        public bool HasMorePages { get; set; }

        /// <summary>
        /// 获取设置是否将新页面数据插入到头部
        /// </summary>
        public bool InsertTop
        {
            get { return (bool)GetValue(InsertTopProperty); }
            set { SetValue(InsertTopProperty, value); }
        }

        /// <summary>
        /// 获取当前页序号
        /// </summary>
        public int PageNo
        {
            get { return _pageNo; }
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        internal PageDataState State { get; set; }

        internal int LastPageCount { get; set; }
        #endregion

        /// <summary>
        /// 加载当前页面数据
        /// </summary>
        /// <param name="p_pageData">当前页面数据</param>
        public void LoadPageData(INotifyList p_pageData)
        {
            // 不满一页时认为无下一页，正好满页且无下页是特殊情况！未处理，使用中应该不在意！
            HasMorePages = (p_pageData != null && p_pageData.Count >= PageSize);

            // uno中数据变化调用UpdateLayout并未测量布局，因此状态在LvPanel.OnSizeChanged中重置！
            if (_pageNo == 0)
            {
                // 首页，可能为空行
                _owner.Data = p_pageData;
            }
            else if (p_pageData != null && p_pageData.Count > 0)
            {
                if (InsertTop)
                {
                    LastPageCount = p_pageData.Count;
                    _owner.Data.InsertRange(0, p_pageData);
                }
                else
                {
                    _owner.Data.AddRange(p_pageData);
                }
            }

            // 无数据时未触发LvPanel.SizeChanged，直接重置状态
            if (p_pageData == null || p_pageData.Count == 0)
                State = PageDataState.Normal;
        }

        internal void SetOwner(Lv p_owner)
        {
            _owner = p_owner;
        }

        /// <summary>
        /// 加载首页
        /// </summary>
        internal Task GotoFirstPage()
        {
            return GotoPage(0);
        }

        /// <summary>
        /// 进入下一页
        /// </summary>
        internal Task GotoNextPage()
        {
            if (HasMorePages)
                return GotoPage(_pageNo + 1);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 加载指定页数据
        /// </summary>
        /// <param name="p_pageNo"></param>
        async Task GotoPage(int p_pageNo)
        {
            State = (p_pageNo == 0) ? PageDataState.LoadingFirstPage : PageDataState.Loading;
            _pageNo = p_pageNo;
            int start = _pageNo * PageSize + 1;
            if (NextPage != null)
            {
                NextPage(this);
            }
            else if (!string.IsNullOrEmpty(Sql))
            {
                string[] info = Sql.Trim().Split(':');
                if (info.Length != 2 || string.IsNullOrEmpty(info[0]) || string.IsNullOrEmpty(info[1]))
                    throw new Exception("Sql格式不正确！" + Sql);

                LoadPageData(await new UnaryRpc(info[0], "Da.QueryPage", start, PageSize, info[1], SqlParams, null).Call<Table>());
            }
            else if (!string.IsNullOrEmpty(SqlStr))
            {
                string[] sqlInfo = Sql.Trim().Split(':');
                if (sqlInfo.Length != 2 || string.IsNullOrEmpty(sqlInfo[0]) || string.IsNullOrEmpty(sqlInfo[1]))
                    throw new Exception("Sql格式不正确！" + SqlStr);

                LoadPageData(await new UnaryRpc(sqlInfo[0], "Da.QueryPage", start, PageSize, sqlInfo[1], SqlParams).Call<Table>());
            }
            else
                throw new Exception("未指定获取页面数据源的方法！");
        }
    }

    internal enum PageDataState
    {
        /// <summary>
        /// 未加载数据
        /// </summary>
        Normal,

        /// <summary>
        /// 正在加载首页数据
        /// </summary>
        LoadingFirstPage,

        /// <summary>
        /// 正在加载普通页数据
        /// </summary>
        Loading
    }
}

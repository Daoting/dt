#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Base.Tools
{
    public partial class ModelDb : Win
    {
        public ModelDb()
        {
            InitializeComponent();
            _lvTbl.Data = AtLocal.QueryModelTblsName();
            _lvTbl.ItemClick += OnTblClick;
        }

        void OnTblClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _lvData.Data = AtLocal.QueryModel(string.Format("select * from {0}", e.Row.Str("name")));
            NaviTo("数据");
        }
    }
}
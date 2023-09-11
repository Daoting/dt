#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Tools
{
    public partial class TypeAliasWin : Win
    {
        public TypeAliasWin()
        {
            InitializeComponent();
            LoadGroup();
        }

        void LoadGroup()
        {
            var tmp = new Dictionary<string, int>();
            var dt = Stub.Inst._typeAlias;
            foreach (var key in dt.Keys)
            {
                int index = key.IndexOf('-');
                if (index < 1)
                    continue;

                var tp = key.Substring(0, index);
                if (tmp.TryGetValue(tp, out int cnt))
                {
                    tmp[tp] = cnt + 1;
                }
                else
                {
                    tmp.Add(tp, 1);
                }
            }

            var tbl = new Table { { "name" }, { "count" } };
            foreach (var item in tmp)
            {
                tbl.AddRow(new { Name = item.Key + "Attribute", Count = $"共 {item.Value} 个类型" });
            }
            _lvGroup.Data = tbl;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            var pre = e.Row.Str(0).Replace("Attribute", "-");

            var tbl = new Table { { "alias" }, { "types" } };
            var dt = Stub.Inst._typeAlias;
            foreach (var item in Stub.Inst._typeAlias)
            {
                if (!item.Key.StartsWith(pre))
                    continue;

                var r = tbl.AddRow();
                r.InitVal("alias", item.Key.Substring(pre.Length));
                string tps = "";
                foreach (var tp in item.Value)
                {
                    tps += tp.FullName + ", ";
                }
                r.InitVal("types", tps);
            }
            _lvType.Data = tbl;
            NaviTo("类型列表");
        }
    }
}
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
            _lvAttr.Data = LoadGroup(Kit.AllAliasTypes.Keys);
            _lvGroup.Data = LoadGroup(Kit.AllAliasTypeList.Keys);
            _lvType.FilterCfg = new FilterCfg
            {
                FilterCols = "alias",
                EnablePinYin = true,
                IsRealtime = true,
            };
        }

        Table LoadGroup(IEnumerable<string> p_keys)
        {
            var tmp = new Dictionary<string, int>();
            foreach (var key in p_keys)
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
            return tbl;
        }

        void OnAttrItemClick(ItemClickArgs e)
        {
            var pre = e.Row.Str(0).Replace("Attribute", "-");

            var tbl = new Table { { "alias" }, { "types" } };
            foreach (var item in Kit.AllAliasTypes)
            {
                if (!item.Key.StartsWith(pre))
                    continue;

                var r = tbl.AddRow();
                r.InitVal("alias", item.Key.Substring(pre.Length));
                r.InitVal("types", item.Value.FullName);
            }
            _lvType.Data = tbl;
            NaviTo("类型列表");
        }

        void OnGroupItemClick(ItemClickArgs e)
        {
            var pre = e.Row.Str(0).Replace("Attribute", "-");

            var tbl = new Table { { "alias" }, { "types" } };
            foreach (var item in Kit.AllAliasTypeList)
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dt
{
    class OnToManyParams
    {
        public string NameSpace { get; set; }

        public string ParentRoot { get; set; }

        public string ParentTbl { get; set; }

        public string ParentEntity { get; set; }

        public string Time { get; } = DateTime.Now.ToString("yyyy-MM-dd");

        public string UserName => Environment.UserName;

        public List<ChildInfo> Children { get; } = new List<ChildInfo>();

        public Dictionary<string, string> ParentWinParams
        {
            get
            {
                var dt = BaseParams;
                if (Children.Count > 1)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("    <a:Tabs a:Ex.Dock=\"Bottom\">");
                    string cs = "";
                    for (int i = 0; i < Children.Count; i++)
                    {
                        var name = "_" + SetFirstToLower(Children[i].Root) + "List";
                        sb.AppendLine($"        <l:{ParentRoot}{Children[i].Root}List x:Name=\"{name}\" />");
                        cs += $"\r\n        public {ParentRoot}{Children[i].Root}List {Children[i].Root}List => {name};\r\n";
                    }
                    sb.Append("    </a:Tabs>");
                    dt["$childlistxaml$"] = sb.ToString();
                    dt["$childlistcs$"] = cs;
                }
                else
                {
                    var name = "_" + SetFirstToLower(Children[0].Root) + "List";
                    dt["$childlistxaml$"] = $"    <l:{ParentRoot}{Children[0].Root}List x:Name=\"{name}\" a:Ex.Dock=\"Bottom\" />";
                    dt["$childlistcs$"] = $"\r\n        public {ParentRoot}{Children[0].Root}List {Children[0].Root}List => {name};\r\n";
                }

                return dt;
            }
        }

        public async Task<Dictionary<string, string>> GetParentListParams()
        {
            var dt = BaseParams;
            dt["$entity$"] = ParentEntity;
            dt["$lvtemp$"] = await AtSvc.GetLvItemTemplate(new List<string> { ParentTbl });
            dt["$lvcols$"] = await AtSvc.GetLvTableCols(new List<string> { ParentTbl });
            dt["$blurclause$"] = await AtSvc.GetBlurClause(new List<string> { ParentTbl });

            string tabs = "";
            foreach (var item in Children)
            {
                tabs += $", _win.{item.Root}List";
            }
            dt["$childtabs$"] = tabs;

            return dt;
        }

        public async Task<Dictionary<string, string>> GetParentFormParams()
        {
            var dt = BaseParams;
            dt["$entity$"] = ParentEntity;
            var body = await AtSvc.GetFvCells(new List<string> { ParentTbl });
            // 可能包含命名空间
            dt["$fvbody$"] = body.Replace("$namespace$", NameSpace).Replace("$rootnamespace$", Kit.GetRootNamespace());

            string update = "";
            foreach (var item in Children)
            {
                if (update != "")
                    update += "\r\n";
                update += $"            _win.{item.Root}List.Update(p_id);";
            }
            dt["$relatedupdate$"] = update;
            return dt;
        }

        public async Task<Dictionary<string, string>> GetParentQueryParams()
        {
            var dt = BaseParams;
            dt["$queryxaml$"] = await AtSvc.GetQueryFvCells(new List<string> { ParentTbl });
            dt["$querydata$"] = await AtSvc.GetQueryFvData(new List<string> { ParentTbl });
            return dt;
        }

        public async Task<Dictionary<string, string>> GetChildParams(ChildInfo p_ci)
        {
            var dt = BaseParams;
            dt["$childroot$"] = p_ci.Root;
            dt["$entity$"] = p_ci.Entity;
            dt["$lvtemp$"] = await AtSvc.GetLvItemTemplate(new List<string> { p_ci.Tbl });
            dt["$lvcols$"] = await AtSvc.GetLvTableCols(new List<string> { p_ci.Tbl });
            var body = await AtSvc.GetFvCells(new List<string> { p_ci.Tbl });
            // 可能包含命名空间
            dt["$fvbody$"] = body.Replace("$namespace$", NameSpace).Replace("$rootnamespace$", Kit.GetRootNamespace());
            dt["$parentidname$"] = p_ci.ParentIDName;
            return dt;
        }

        Dictionary<string, string> BaseParams => new Dictionary<string, string>
                    {
                        {"$rootnamespace$", NameSpace },
                        {"$parentroot$", ParentRoot },
                        {"$time$", Time },
                        {"$username$", UserName },
                    };

        static string SetFirstToLower(string p_str)
        {
            char[] a = p_str.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }
    }

    class ChildInfo
    {
        public string Tbl { get; set; }

        public string Entity { get; set; }

        public string Root { get; set; }

        public string ParentIDName { get; set; }
    }
}

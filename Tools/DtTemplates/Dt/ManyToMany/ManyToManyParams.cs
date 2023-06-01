using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dt
{
    class ManyToManyParams
    {
        public string NameSpace { get; set; }

        public string MainRoot { get; set; }

        public string MainTbl { get; set; }

        public string MainEntity { get; set; }

        public string Time { get; } = DateTime.Now.ToString("yyyy-MM-dd");

        public string UserName => Environment.UserName;

        public List<RelatedInfo> Related { get; } = new List<RelatedInfo>();

        public Dictionary<string, string> GetWinParams()
        {
            var dt = BaseParams;
            if (Related.Count > 1)
            {
                var sb = new StringBuilder();
                sb.AppendLine("    <a:Tabs a:Ex.Dock=\"Right\">");
                string cs = "";
                for (int i = 0; i < Related.Count; i++)
                {
                    var name = "_" + SetFirstToLower(Related[i].Root) + "List";
                    sb.AppendLine($"        <l:{MainRoot}{Related[i].Root}List x:Name=\"{name}\" />");
                    cs += $"\r\n        public {MainRoot}{Related[i].Root}List {Related[i].Root}List => {name};\r\n";
                }
                sb.Append("    </a:Tabs>");
                dt["$releatedxaml$"] = sb.ToString();
                dt["$releatedcs$"] = cs;
            }
            else
            {
                var name = "_" + SetFirstToLower(Related[0].Root) + "List";
                dt["$releatedxaml$"] = $"    <l:{MainRoot}{Related[0].Root}List x:Name=\"{name}\" a:Ex.Dock=\"Right\" />";
                dt["$releatedcs$"] = $"\r\n        public {MainRoot}{Related[0].Root}List {Related[0].Root}List => {name};\r\n";
            }

            return dt;
        }

        public async Task<Dictionary<string, string>> GetMainListParams()
        {
            var dt = BaseParams;
            dt["$entity$"] = MainEntity;
            dt["$lvtemp$"] = await AtSvc.GetLvItemTemplate(new List<string> { MainTbl });
            dt["$lvcols$"] = await AtSvc.GetLvTableCols(new List<string> { MainTbl });
            //dt["$blurclause$"] = await AtSvc.GetBlurClause(new List<string> { MainTbl });

            string tabs = "";
            foreach (var item in Related)
            {
                tabs += $", _win.{item.Root}List";
            }
            dt["$childtabs$"] = tabs;

            return dt;
        }

        public async Task<Dictionary<string, string>> GetMainFormParams()
        {
            var dt = BaseParams;
            dt["$entity$"] = MainEntity;
            dt["$fvbody$"] = await AtSvc.GetFvCells(new List<string> { MainTbl });

            string update = "";
            foreach (var item in Related)
            {
                if (update != "")
                    update += "\r\n";
                update += $"            _win.{item.Root}List.Update(p_id);";
            }
            dt["$relatedupdate$"] = update;
            return dt;
        }

        public async Task<Dictionary<string, string>> GetMainQueryParams()
        {
            var dt = BaseParams;
            dt["$queryxaml$"] = await AtSvc.GetQueryFvCells(new List<string> { MainTbl });
            dt["$querydata$"] = await AtSvc.GetQueryFvData(new List<string> { MainTbl });
            return dt;
        }

        public async Task<Dictionary<string, string>> GetRelatedParams(RelatedInfo p_ci)
        {
            var dt = BaseParams;
            dt["$childroot$"] = p_ci.Root;
            dt["$entity$"] = p_ci.Entity;
            dt["$lvtemp$"] = await AtSvc.GetLvItemTemplate(new List<string> { p_ci.Tbl });
            dt["$lvcols$"] = await AtSvc.GetLvTableCols(new List<string> { p_ci.Tbl });

            dt["$relatedentity$"] = p_ci.RelatedEntity;
            dt["$mainrelatedid$"] = p_ci.MainRelatedID;
            dt["$relatedid$"] = p_ci.RelatedID;
            dt["$selectdlg$"] = $"{p_ci.Root}4{MainRoot}Dlg";

            var filter = $"exists ( select {p_ci.RelatedID} from {p_ci.RelatedTbl} b where a.ID = b.{p_ci.RelatedID} and {p_ci.MainRelatedID}=@ReleatedID )";
            dt["$whereclause$"] = "where " + filter;
            dt["$notexistclause$"] = "where not " + filter;
            return dt;
        }

        Dictionary<string, string> BaseParams => new Dictionary<string, string>
                    {
                        {"$rootnamespace$", NameSpace },
                        {"$mainroot$", MainRoot },
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

    class RelatedInfo
    {
        public string Tbl { get; set; }

        public string Entity { get; set; }

        public string Root { get; set; }

        public string RelatedTbl { get; set; }

        public string RelatedEntity { get; set; }

        /// <summary>
        /// 关联实体外键
        /// </summary>
        public string RelatedID { get; set; }

        /// <summary>
        /// 主实体外键
        /// </summary>
        public string MainRelatedID { get; set; }
    }
}

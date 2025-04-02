#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
#endregion

namespace Dt.Base.Views
{
    public sealed partial class EntityQuery : Tab
    {
        EntityCfg _cfg;
        QueryFv _fv;

        public void LoadCfg(EntityCfg p_cfg)
        {
            Title = "查询";
            Icon = Icons.漏斗;
            _cfg = p_cfg;

            if (!string.IsNullOrEmpty(_cfg.QueryFvXaml))
            {
                _fv = Kit.LoadXaml<QueryFv>(_cfg.QueryFvXaml);
                if (_fv == null)
                    Throw.Msg($"加载查询面板xaml时错误：\n{_cfg.QueryFvXaml}");
            }
            else
            {
                string xaml = GetCellsXaml();
                _fv = Kit.LoadXaml<QueryFv>(xaml);
            }

            Content = _fv;
            LoadData();
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event Action<QueryClause> Query
        {
            add { _fv.Query += value; }
            remove { _fv.Query -= value; }
        }

        /// <summary>
        /// 生成查询面板的FvCells，和服务端 SysTools.GetQueryFvCells 相同
        /// </summary>
        /// <returns></returns>
        string GetCellsXaml()
        {
            StringBuilder sb = new StringBuilder("<a:QueryFv>");
            foreach (var col in _cfg.Table.Columns)
            {
                // 字段可能为null
                Type tp = col.Type;
                if (col.Type.IsGenericType && col.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    tp = col.Type.GetGenericArguments()[0];

                // 长整型通常为键值，忽略
                if (tp == typeof(long))
                    continue;

                sb.AppendLine();

                string title;
                if (col.IsEnumCol)
                {
                    // 枚举 CList
                    if (col.IsChinessName)
                    {
                        title = "";
                    }
                    else
                    {
                        string tpName = col.GetEnumName();
                        title = col.Comments.Substring(tpName.Length + 2);
                        title = string.IsNullOrEmpty(title) ? "" : $" Title=\"{title}\"";
                    }
                    sb.Append($"<a:CList ID=\"{col.Name.ToLower()}\"{title} Query=\"Editable\" QueryFlag=\"Equal\" />");
                    continue;
                }

                // 字段名中文时不再需要Title
                if (!string.IsNullOrEmpty(col.Comments) && !col.IsChinessName)
                {
                    title = $" Title=\"{col.Comments}\"";
                }
                else
                {
                    title = "";
                }

                if (tp == typeof(bool))
                {
                    sb.Append($"<a:CBool ID=\"{col.Name.ToLower()}\"{title} Query=\"Editable\" QueryFlag=\"Equal\" />");
                }
                else if (tp == typeof(int))
                {
                    sb.Append($"<a:CBar Title=\"{(title == "" ? col.Name : col.Comments)}\" />\r\n");
                    sb.Append($"<a:CNum ID=\"{col.Name.ToLower()}_min\" IsInteger=\"True\" Query=\"Editable\" QueryFlag=\"Floor\" ShowTitle=\"False\" ColSpan=\"0.5\" />\r\n");
                    sb.Append($"<a:CNum ID=\"{col.Name.ToLower()}_max\" IsInteger=\"True\" Query=\"Editable\" QueryFlag=\"Ceil\" ShowTitle=\"False\" ColSpan=\"0.5\" />");
                }
                else if (tp == typeof(float) || tp == typeof(double))
                {
                    sb.Append($"<a:CBar Title=\"{(title == "" ? col.Name : col.Comments)}\" />\r\n");
                    sb.Append($"<a:CNum ID=\"{col.Name.ToLower()}_min\" Query=\"Editable\" QueryFlag=\"Floor\" ShowTitle=\"False\" ColSpan=\"0.5\" />\r\n");
                    sb.Append($"<a:CNum ID=\"{col.Name.ToLower()}_max\" Query=\"Editable\" QueryFlag=\"Ceil\" ShowTitle=\"False\" ColSpan=\"0.5\" />");
                }
                else if (tp == typeof(DateTime))
                {
                    sb.Append($"<a:CBar Title=\"{(title == "" ? col.Name : col.Comments)}\" />\r\n");
                    sb.Append($"<a:CDate ID=\"{col.Name.ToLower()}_min\" Query=\"Editable\" QueryFlag=\"Floor\" ShowTitle=\"False\" ColSpan=\"0.5\" />\r\n");
                    sb.Append($"<a:CDate ID=\"{col.Name.ToLower()}_max\" Query=\"Editable\" QueryFlag=\"Ceil\" ShowTitle=\"False\" ColSpan=\"0.5\" />");
                }
                else
                {
                    sb.Append($"<a:CText ID=\"{col.Name.ToLower()}\"{title} Query=\"Editable\" QueryFlag=\"Contains\" />");
                }
            }
            sb.Append("</a:QueryFv>");
            return sb.ToString();
        }

        void LoadData()
        {
            var row = new Row();
            var cols = _cfg.Table.Columns;
            foreach (var cell in _fv.IDCells)
            {
                var id = cell.ID;
                if (id.EndsWith("_min") || id.EndsWith("_max"))
                    id = id.Substring(0, id.Length - 4);

                if (cols.TryGetValue(id, out var col))
                {
                    row.Add(cell.ID, col.Type);
                }
            }
            _fv.Data = row;
        }
    }
}

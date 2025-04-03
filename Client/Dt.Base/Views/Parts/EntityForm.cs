#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using System.Text;
#endregion

namespace Dt.Base.Views
{
    public sealed partial class EntityForm : Form
    {
        EntityCfg _cfg;
        Fv _fv;

        public void LoadCfg(EntityCfg p_cfg)
        {
            _cfg = p_cfg;

            if (!string.IsNullOrEmpty(_cfg.FormCfg.Xaml))
            {
                _fv = Kit.LoadXaml<Fv>(_cfg.FormCfg.Xaml);
                if (_fv == null)
                    Throw.Msg($"加载Fv的xaml时错误：{_cfg.FormCfg.Xaml}");
            }
            else
            {
                string xaml = GetXaml();
                _fv = Kit.LoadXaml<Fv>(xaml);
            }

            Content = _fv;
            MainFv = _fv;
         
            _fv.DataChanged += (s) =>
            {
                var id = _fv.Row.ID;
            };
            
            var cfg = _cfg.FormCfg;
            if (cfg.ShowAddMi || cfg.ShowDelMi || cfg.ShowSaveMi)
            {
                Menu = CreateMenu(null, cfg.ShowAddMi, cfg.ShowSaveMi, cfg.ShowDelMi);
            }
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await _cfg.New();
        }

        protected override async Task OnGet()
        {
            _fv.Data = await _cfg.GetByID(_args.ID);
        }

        string GetXaml()
        {
            StringBuilder sb = new StringBuilder("<a:Fv>");
            foreach (var col in _cfg.Table.Columns)
            {
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
                    sb.Append($"<a:CList ID=\"{col.Name.ToLower()}\"{title} />");
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

                // 按照字段类型生成FvCell
                Type tp = col.Type;
                if (col.Type.IsGenericType && col.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    tp = col.Type.GetGenericArguments()[0];

                if (tp == typeof(bool))
                {
                    sb.Append($"<a:CBool ID=\"{col.Name.ToLower()}\"{title} />");
                }
                else if (tp == typeof(int) || tp == typeof(long) || tp == typeof(short))
                {
                    sb.Append($"<a:CNum ID=\"{col.Name.ToLower()}\"{title} IsInteger=\"True\" />");
                }
                else if (tp == typeof(float) || tp == typeof(double))
                {
                    sb.Append($"<a:CNum ID=\"{col.Name.ToLower()}\"{title} />");
                }
                else if (tp == typeof(DateTime))
                {
                    sb.Append($"<a:CDate ID=\"{col.Name.ToLower()}\"{title} />");
                }
                else
                {
                    sb.Append($"<a:CText ID=\"{col.Name.ToLower()}\"{title} />");
                }
            }
            
            sb.Append("</a:Fv>");
            return sb.ToString();
        }
    }
}
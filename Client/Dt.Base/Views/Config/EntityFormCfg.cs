#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Dt.Base
{
    public class EntityFormCfg
    {
        string _title;
        
        public string Xaml { get; set; }

        public string Title
        {
            get
            {
                if (!string.IsNullOrEmpty(_title))
                    return _title;
                if (Owner != null && !string.IsNullOrEmpty(Owner.Cls))
                    return Type.GetType(Owner.Cls).Name.TrimEnd('X') + "表单";
                return "表单";
            }
            set => _title = value;
        }

        public bool ShowAddMi { get; set; } = true;

        public bool ShowSaveMi { get; set; } = true;

        public bool ShowDelMi { get; set; } = true;

        public bool IsChanged => !string.IsNullOrEmpty(Xaml) || !ShowAddMi || !ShowDelMi || !ShowSaveMi || !string.IsNullOrEmpty(_title);

        public bool IsCustomTitle => !string.IsNullOrEmpty(_title);

        public EntityCfg Owner { get; set; }
    }
}
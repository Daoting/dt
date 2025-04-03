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
        EntityCfg _owner;
        string _title;

        public EntityFormCfg(EntityCfg p_owner)
        {
            _owner = p_owner;
        }

        public string Xaml { get; set; }

        public string Title
        {
            get => _title ??= Type.GetType(_owner.Cls).Name.TrimEnd('X') + "列表";
            set => _title = value;
        }

        public bool ShowAddMi { get; set; } = true;

        public bool ShowSaveMi { get; set; } = true;

        public bool ShowDelMi { get; set; } = true;

        public bool IsChanged => !string.IsNullOrEmpty(Xaml) || !ShowAddMi || !ShowDelMi || !ShowSaveMi;
    }
}
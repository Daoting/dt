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
    public class EntityListCfg
    {
        public string Xaml { get; set; }

        public bool ShowAddMi { get; set; } = true;

        public bool ShowDelMi { get; set; } = true;

        public bool ShowMultiSelMi { get; set; } = true;
        
        public bool IsChanged => !string.IsNullOrEmpty(Xaml) || !ShowAddMi || !ShowDelMi || !ShowMultiSelMi;
    }
}
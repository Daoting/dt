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
    public class OneToManyCfg
    {
        public EntityCfg ParentCfg { get; set; }

        public Nl<EntityCfg> ChildCfgs { get; } = new Nl<EntityCfg>();

        internal string Serialize()
        {
            using (var stream = new MemoryStream())
            using (var writer = new Utf8JsonWriter(stream, JsonOptions.UnsafeWriter))
            {
                writer.WriteStartObject();
                if (ParentCfg != null)
                {
                    writer.WriteStartObject("ParentCfg");
                    ParentCfg.DoSerialize(writer);
                    writer.WriteEndObject();
                }

                writer.WriteStartArray("ChildCfgs");
                foreach (var cfg in ChildCfgs)
                {
                    writer.WriteStartObject();
                    cfg.DoSerialize(writer);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                
                writer.WriteEndObject();
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
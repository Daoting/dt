#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 一对多关系配置
    /// </summary>
    public class OneToManyCfg
    {
        /// <summary>
        /// 一对多关系的父实体配置
        /// </summary>
        public EntityCfg ParentCfg { get; set; } = new EntityCfg();

        /// <summary>
        /// 一对多关系的子实体配置
        /// </summary>
        public Nl<EntityCfg> ChildCfgs { get; set; } = new Nl<EntityCfg>();

        /// <summary>
        /// 是否采用父子表单
        /// </summary>
        public bool IsUnionForm { get; set; }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            using (var stream = new MemoryStream())
            using (var writer = new Utf8JsonWriter(stream, JsonOptions.UnsafeWriter))
            {
                writer.WriteStartArray();
                writer.WriteStringValue("#object");
                writer.WriteStartObject();

                if (IsUnionForm)
                    writer.WriteBoolean("IsUnionForm", true);

                if (ParentCfg != null)
                {
                    writer.WritePropertyName("ParentCfg");
                    ParentCfg.DoSerialize(writer);
                }

                if (ChildCfgs != null && ChildCfgs.Count > 0)
                {
                    writer.WritePropertyName("ChildCfgs");
                    writer.WriteStartArray();
                    writer.WriteStringValue("&object");
                    foreach (var cfg in ChildCfgs)
                    {
                        cfg.DoSerialize(writer);
                    }
                    writer.WriteEndArray();
                }

                writer.WriteEndObject();
                writer.WriteEndArray();
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="p_json"></param>
        /// <returns></returns>
        public static OneToManyCfg Deserialize(string p_json)
        {
            if (string.IsNullOrEmpty(p_json))
                return new OneToManyCfg();

            var cfg = Kit.Deserialize<OneToManyCfg>(p_json);
            if (cfg.ParentCfg != null)
            {
                if (cfg.ParentCfg.ListCfg != null)
                    cfg.ParentCfg.ListCfg.Owner = cfg.ParentCfg;
                if (cfg.ParentCfg.FormCfg != null)
                    cfg.ParentCfg.FormCfg.Owner = cfg.ParentCfg;
            }

            if (cfg.ChildCfgs != null && cfg.ChildCfgs.Count > 0)
            {
                foreach (var childCfg in cfg.ChildCfgs)
                {
                    if (childCfg.ListCfg != null)
                        childCfg.ListCfg.Owner = childCfg;
                    if (childCfg.FormCfg != null)
                        childCfg.FormCfg.Owner = childCfg;
                }
            }
            return cfg;
        }
    }
}
﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 多对多关系配置
    /// </summary>
    public class ManyToManyCfg
    {
        /// <summary>
        /// 多对多关系的主实体配置
        /// </summary>
        public EntityCfg MainCfg { get; set; } = new EntityCfg();

        /// <summary>
        /// 多对多关系的关联实体配置
        /// </summary>
        public Nl<RelatedEntityCfg> RelatedCfgs { get; set; } = new Nl<RelatedEntityCfg>();

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            using (var stream = new MemoryStream())
            using (var writer = new Utf8JsonWriter(stream, JsonOptions.UnsafeWriter))
            {
                writer.WriteStartObject();

                writer.WriteStartObject("MainCfg");
                if (MainCfg != null)
                    MainCfg.DoSerialize(writer);
                writer.WriteEndObject();

                writer.WriteStartArray("RelatedCfgs");
                if (RelatedCfgs != null && RelatedCfgs.Count > 0)
                {
                    foreach (var cfg in RelatedCfgs)
                    {
                        writer.WriteStartObject();
                        cfg.DoSerialize(writer);
                        writer.WriteEndObject();
                    }
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="p_json"></param>
        /// <returns></returns>
        public static ManyToManyCfg Deserialize(string p_json)
        {
            if (string.IsNullOrEmpty(p_json))
                return new ManyToManyCfg();

            var cfg = JsonSerializer.Deserialize<ManyToManyCfg>(p_json);
            if (cfg.MainCfg != null)
            {
                if (cfg.MainCfg.ListCfg != null)
                    cfg.MainCfg.ListCfg.Owner = cfg.MainCfg;
                if (cfg.MainCfg.FormCfg != null)
                    cfg.MainCfg.FormCfg.Owner = cfg.MainCfg;
            }
            
            return cfg;
        }
    }
}
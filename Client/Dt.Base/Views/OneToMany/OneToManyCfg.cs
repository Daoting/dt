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
        
        public List<EntityCfg> ChildCfgs { get; } = new List<EntityCfg>();

        internal async Task Init()
        {
            await ParentCfg.Init();
            foreach (var cfg in ChildCfgs)
            {
                await cfg.Init();
            }
        }
    }
}
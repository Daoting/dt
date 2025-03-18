#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-07 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Base
{
    public class FvDesignInfo
    {
        public string Xaml { get; set; }
        
        public List<EntityCol> Cols { get; set; }
        
        public bool IsQueryFv { get; set; }
    }

    public class EntityCol
    {
        public EntityCol(string p_name, Type p_type)
        {
            Name = p_name;
            Type = p_type;
        }

        public string Name { get; set; }

        public Type Type { get; set; }
    }
}
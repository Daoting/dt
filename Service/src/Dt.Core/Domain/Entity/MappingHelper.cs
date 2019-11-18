#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Domain;
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体与通用数据结构的转换
    /// </summary>
    public static class MappingHelper
    {
        public static TEntity To<TEntity>(this Row p_row)
            where TEntity : class, IEntity
        {
            Check.NotNull(p_row);
            Type type = typeof(TEntity);
            var entity = Activator.CreateInstance<TEntity>();
            foreach (var cell in p_row.Cells)
            {
                var prop = type.GetProperty(cell.ID, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null)
                    continue;

                if (prop.PropertyType == cell.Type)
                {
                    prop.SetValue(entity, cell.Val);
                }
                else
                {
                    try
                    {
                        prop.SetValue(entity, Convert.ChangeType(cell.Val, prop.PropertyType));
                    }
                    catch { }
                }
            }
            return entity;
        }

        public static TEntity To<TEntity>(this Dict p_row)
            where TEntity : class
        {
            throw new NotImplementedException();
        }

        public static Row ToRow(this IEntity p_entity)
        {
            throw new NotImplementedException();
        }

        public static Dict ToDict(this IEntity p_entity)
        {
            throw new NotImplementedException();
        }
    }
}

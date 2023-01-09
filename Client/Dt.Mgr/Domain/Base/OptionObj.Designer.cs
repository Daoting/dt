#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_option")]
    public partial class OptionObj : Entity
    {
        #region 构造方法
        OptionObj() { }

        public OptionObj(
            string Name,
            string Category,
            int Dispidx = default)
        {
            AddCell("Name", Name);
            AddCell("Category", Category);
            AddCell("Dispidx", Dispidx);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 选项名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 所属分类
        /// </summary>
        public string Category
        {
            get { return (string)this["Category"]; }
            set { this["Category"] = value; }
        }

        new public long ID { get { return -1; } }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
        }

        #region 静态方法
        /// <summary>
        /// 返回所有实体的列表，每个实体包含所有列值
        /// </summary>
        /// <returns></returns>
        public static Task<Table<OptionObj>> GetAll()
        {
            return EntityEx.GetAll<OptionObj>();
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<OptionObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<OptionObj>(p_colName);
        }
        #endregion
    }
}
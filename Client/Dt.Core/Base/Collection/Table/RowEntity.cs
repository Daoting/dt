#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Row数据实体包装类
    /// </summary>
    public abstract class RowEntity
    {
        protected Row _row;

        /// <summary>
        /// 构造方法
        /// </summary>
        public RowEntity()
        {
            string tbl = GetTblName();
            if (!string.IsNullOrEmpty(tbl))
            {
                Table data = Table.Create(tbl);
                _row = data.NewRow();
                _row.Entity = this;
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_row">数据行</param>
        public RowEntity(Row p_row)
        {
            if (p_row != null)
            {
                _row = p_row;
                _row.Entity = this;
                _row.Table.Name = GetTblName();
            }
        }

        /// <summary>
        /// 获取数据行
        /// </summary>
        public Row Row
        {
            get { return _row; }
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        public Table Table
        {
            get { return _row.Table; }
        }

        /// <summary>
        /// 获取数据是否有变化
        /// </summary>
        public bool IsChanged
        {
            get { return _row.IsChanged; }
        }

        /// <summary>
        /// 获取设置当前行是否为新增
        /// </summary>
        public bool IsAdded
        {
            get { return _row.IsAdded; }
        }

        /// <summary>
        /// 获取实体类对应的表名
        /// </summary>
        /// <returns></returns>
        protected abstract string GetTblName();
    }
}

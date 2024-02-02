﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// 模型库的回调接口
    /// </summary>
    public interface IModelCallback
    {
        /// <summary>
        /// 查询表结构信息
        /// </summary>
        /// <param name="p_tblAttr">实体类属性标签</param>
        /// <returns></returns>
        Task<TableSchema> GetTableSchema(TblAttribute p_tblAttr);

        /// <summary>
        /// 读取模板内容，默认取模型库的 OmReport
        /// </summary>
        /// <param name="p_rptName">报表名称</param>
        /// <returns></returns>
        Task<string> GetReportTemplate(string p_rptName);
    }
}
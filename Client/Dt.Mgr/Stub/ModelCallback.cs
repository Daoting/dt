#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 模型库的回调接口
    /// </summary>
    class ModelCallback : IModelCallback
    {
        /// <summary>
        /// 查询指定表的所有列，默认取模型库的 OmColumn
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public Task<IEnumerable<OmColumn>> GetTableColumns(string p_tblName)
        {
            return AtModel.Each<OmColumn>($"select * from OmColumn where tabname='{p_tblName}'");
        }

        /// <summary>
        /// 读取模板内容，默认取模型库的 OmReport
        /// </summary>
        /// <param name="p_rptName">报表名称</param>
        /// <returns></returns>
        public async Task<string> GetReportTemplate(string p_rptName)
        {
            string define = await AtModel.GetScalar<string>("select define from OmReport where name=@name", new Dict { { "name", p_rptName } });
            if (string.IsNullOrEmpty(define))
                Kit.Warn($"未找到报表模板【{p_rptName}】！");
            return define;
        }
    }
}
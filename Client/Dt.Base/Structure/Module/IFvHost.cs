#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 内部含Fv，数据源为Row或Entity的宿主接口
    /// </summary>
    interface IFvHost
    {
        /// <summary>
        /// 是否已显示
        /// </summary>
        bool IsOpened { get; }
        
        /// <summary>
        /// 内部Fv
        /// </summary>
        Fv Fv { get; }

        /// <summary>
        /// 增加
        /// </summary>
        Task OnAdd();

        /// <summary>
        /// 加载数据源
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        Task OnGet(long p_id);

        /// <summary>
        /// 保存数据
        /// </summary>
        Task<bool> OnSave();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task OnDel();
        
        /// <summary>
        /// 清空数据源
        /// </summary>
        void Clear();

        /// <summary>
        /// 更新关联视图
        /// </summary>
        /// <param name="p_id"></param>
        void UpdateRelated(long p_id);
    }
}

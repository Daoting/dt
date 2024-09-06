#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    public partial class 部门人员X
    {
        public string 部门名称 => (string)this["部门名称"];

        protected override void InitHook()
        {
            //OnSaving(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnSaved(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnDeleting(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnDeleted(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnChanging(cName, e =>
            //{
                
            //});
        }

        #region Sql

        #endregion
    }
}
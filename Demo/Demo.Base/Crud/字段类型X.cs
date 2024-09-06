#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    public partial class 字段类型X
    {
        public static async Task<字段类型X> New(
            string 字符串 = default,
            int 整型 = default,
            int? 可空整型 = default,
            long? 长整型 = default,
            bool 布尔 = default,
            bool? 可空布尔 = default,
            DateTime 日期时间 = default,
            DateTime? 可空时间 = default,
            Gender 枚举 = default,
            Gender? 可空枚举 = default,
            float 单精度 = default,
            float? 可空单精度 = default)
        {
            return new 字段类型X(
                ID: await NewID(),
                字符串: 字符串,
                整型: 整型,
                可空整型: 可空整型,
                长整型: 长整型,
                布尔: 布尔,
                可空布尔: 可空布尔,
                日期时间: 日期时间,
                可空时间: 可空时间,
                枚举: 枚举,
                可空枚举: 可空枚举,
                单精度: 单精度,
                可空单精度: 可空单精度);
        }

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
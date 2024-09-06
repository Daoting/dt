#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    public partial class 扩展2X
    {
        public static async Task<扩展2X> New(
            string 扩展2名称 = default,
            bool 禁止删除 = default,
            string 值变事件 = default)
        {
            return new 扩展2X(
                ID: await NewID(),
                扩展2名称: 扩展2名称,
                禁止删除: 禁止删除,
                值变事件: 值变事件);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                if (_cells["值变事件"].IsChanged)
                {
                    AddEvent(new 值变Event
                    {
                        OriginalVal = _cells["值变事件"].GetOriginalVal<string>(),
                        NewVal = 值变事件,
                    });
                }
                return Task.CompletedTask;
            });

            OnDeleting(() =>
            {
                if (禁止删除)
                {
                    Throw.Msg("已选中[禁止删除]，删除前校验不通过！");
                }
                return Task.CompletedTask;
            });
        }
    }
}
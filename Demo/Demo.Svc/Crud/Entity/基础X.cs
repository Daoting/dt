#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Crud
{
    public partial class 基础X
    {
        public static async Task<基础X> New(
            string 名称 = default,
            string 限长4 = default,
            string 不重复 = default,
            bool 禁止选中 = default,
            bool 禁止保存 = default,
            bool 禁止删除 = default,
            string 值变事件 = default,
            bool 发布插入事件 = default,
            bool 发布删除事件 = default)
        {
            return new 基础X(
                ID: await NewID(),
                序列: await NewSeq("序列"),
                名称: 名称,
                限长4: 限长4,
                不重复: 不重复,
                禁止选中: 禁止选中,
                禁止保存: 禁止保存,
                禁止删除: 禁止删除,
                值变事件: 值变事件,
                发布插入事件: 发布插入事件,
                发布删除事件: 发布删除事件,
                创建时间: Kit.Now,
                修改时间: Kit.Now);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                if (IsAdded && 发布插入事件)
                {
                    AddEvent(new 插入Event { ID = ID });
                }
                
                if (c值变事件.IsChanged)
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

                if (发布删除事件)
                {
                    AddEvent(new 删除Event { Tgt = this });
                }
                return Task.CompletedTask;
            });
            
        }
    }
}
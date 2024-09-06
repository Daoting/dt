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
    public partial class 基础X
    {
        public static async Task<基础X> New(
            string 名称 = default,
            long? ParentID = default,
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
                ParentID: ParentID,
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
            OnSaving(async () =>
            {
                if (IsAdded && 发布插入事件)
                {
                    AddEvent(new 插入Event { ID = ID });
                }
                
                if (c不重复.IsChanged)
                {
                    int cnt = await AtSvc.GetScalar<int>($"select count(1) from crud_基础 where 不重复='{不重复}' and ID!={ID}");
                    if (cnt > 0)
                    {
                        Throw.Msg("[不重复]列存在重复值！", c不重复);
                    }
                }

                if (禁止保存)
                {
                    Throw.Msg("已选中[禁止保存]，保存前校验不通过！", c禁止保存);
                }

                if (c值变事件.IsChanged)
                {
                    AddEvent(new 值变Event
                    {
                        OriginalVal = _cells["值变事件"].GetOriginalVal<string>(),
                        NewVal = 值变事件,
                    });
                }
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

            OnChanging(c限长4, e =>
            {
                e.NewVal = e.Str.ToUpper();

                // 内部已有默认的超长校验，按照数据库字段长度
                Throw.If(e.GbkLength > 8, "超出最大长度", e.Cell);
                Throw.If(e.Str.Length > 8, "超出最大长度", e.Cell);
                Throw.If(e.Utf8Length > 8, "超出最大长度", e.Cell);
                Throw.If(e.UnicodeLength > 8, "超出最大长度", e.Cell);
            });

            OnChanging(c禁止选中, e =>
            {
                Throw.If(e.Bool, "[禁止选中]列无法选中");
            });

        }
    }
}
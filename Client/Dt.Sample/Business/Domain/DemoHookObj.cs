#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Sample
{
    public partial class DemoHookObj
    {
        public static async Task<DemoHookObj> New(
            string MaxLength = "限长",
            string NotNull = "非空",
            string Src = default,
            string Tgt = default,
            bool IsCheck = default,
            int NoBinding = default,
            int NoHook = default,
            bool NoDelete = default)
        {
            long id = await NewID(_svcName);
            return new DemoHookObj(id, MaxLength, NotNull, Src, Tgt, IsCheck, NoBinding, NoHook, NoDelete);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                Throw.If(MaxLength.Length >= NotNull.Length, "【不为空】的长度必须大于【最大长度3】的长度！");
                return Task.CompletedTask;
            });

            OnDeleting(() =>
            {
                Throw.If(NoDelete, "已设置禁止删除！");
                return Task.CompletedTask;
            });

            // 使用 nameof 避免列名不存在
            OnChanging<string>(nameof(MaxLength), v => Throw.If(v.Length > 3, "最大长度3"));

            OnChanging<string>(nameof(NotNull), v => Throw.If(string.IsNullOrEmpty(v), "不可为空"));

            OnChanging<string>(nameof(Src), v => Tgt = v);

            OnChanging<bool>(nameof(IsCheck), v => Throw.If(v && string.IsNullOrEmpty(Src), "联动源不为空"));

            OnChanging<int>(nameof(NoBinding), v =>
            {
                Kit.Msg($"ID：{ID}\r\n新值：{v}");
                //Throw.If(v > 50, "最大值不可超过50");
            });
        }
    }
}
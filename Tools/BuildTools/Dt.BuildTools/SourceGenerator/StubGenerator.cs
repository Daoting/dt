#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
#endregion

namespace Dt.BuildTools
{
    [Generator]
    public class StubGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // 设计时不处理，不再判断 <UseStub>true</UseStub>，只要引用就处理
            if (Kit.IsDesignTime(context))
                return;

            if (Kit.IsExePrj(context))
                new AppGenerator().Generate(context);
            else
                new DictGenerator().Generate(context);
        }
    }
}
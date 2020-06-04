#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(GroupName = "功能测试")]
    public class TestException : BaseApi
    {
        public string ThrowException()
        {
            throw new Exception("服务器端普通异常内容");
        }

        public string ThrowBusinessException()
        {
            Throw.Msg("服务器端返回的业务警告");
            return "test";
        }

        public string ThrowPostionException()
        {
            Throw.If(true);
            return "test";
        }

        public Dict ThrowSerializeException()
        {
            Dict dt = new Dict();
            TestCls menu = new TestCls();
            menu.ID = Guid.NewGuid().ToString();
            menu.Name = "名称";
            dt["menu"] = menu;
            return dt;
        }

        public class TestCls
        {
            public string ID { get; set; }

            public string Name { get; set; }
        }
    }
}

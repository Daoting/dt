#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class DataAccessHome : Win
    {
        public DataAccessHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("数据表操作", typeof(TableAccess), Icons.分组) { Desc = "Table, Row, Column, Cell的常用方法" },
                new Nav("序列化类型", typeof(SerializeDemo), Icons.全选),
                new Nav("异常处理", typeof(ExceptionDemo), Icons.警告),
                new Nav("实体类业务逻辑", typeof(EntityDemo), Icons.传真),
                new Nav("远程过程调用", typeof(RpcDemo), Icons.耳麦), 
                new Nav("服务端Api授权控制", typeof(AuthAccess), Icons.小图标),
            };
        }
    }
}

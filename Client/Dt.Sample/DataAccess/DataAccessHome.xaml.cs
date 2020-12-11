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
            _lv.Data = new Nl<MainInfo>
            {
                new MainInfo(Icons.分组, "数据表操作", typeof(TableAccess), "Table, Row, Column, Cell的常用方法"),
                new MainInfo(Icons.详细, "序列化类型", typeof(SerializeDemo), ""),
                new MainInfo(Icons.警告, "异常处理", typeof(ExceptionDemo), ""),
                new MainInfo(Icons.传真, "实体类业务逻辑", typeof(EntityDemo), ""),
                new MainInfo(Icons.耳麦, "远程过程调用", typeof(RpcDemo), ""),
                new MainInfo(Icons.小图标, "增删改查", typeof(DbAccess), ""),
                new MainInfo(Icons.排列, "本地库操作", typeof(LocalDbAccess), ""),
            };
        }
    }
}

#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    internal interface IPropertyAccess
    {
        T GetProperty<T>(int propertyId);
        void RemoveProperty(int propertyId);
        void SetProperty<T>(int propertyId, T value);
    }
}


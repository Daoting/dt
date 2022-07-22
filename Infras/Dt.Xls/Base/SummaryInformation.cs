#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    internal sealed class SummaryInformation : IPropertyAccess
    {
        private byte[] _dataBuffer;
        private Dictionary<int, object> _properties;
        public const int AUTHOR_PROPERTY_ID = 0;

        private SummaryInformation(byte[] data)
        {
            this._dataBuffer = data;
            this._properties = new Dictionary<int, object>();
        }

        public T GetProperty<T>(int propertyId)
        {
            object obj2;
            if (!this._properties.TryGetValue(propertyId, out obj2))
            {
                throw new NotImplementedException();
            }
            return (T) obj2;
        }

        internal static SummaryInformation Read(byte[] data)
        {
            return new SummaryInformation(data);
        }

        public void RemoveProperty(int propertyId)
        {
            if (this._properties.ContainsKey(propertyId))
            {
                this._properties.Remove(propertyId);
            }
        }

        public void SetProperty<T>(int propertyId, T value)
        {
            this._properties[propertyId] = value;
        }
    }
}


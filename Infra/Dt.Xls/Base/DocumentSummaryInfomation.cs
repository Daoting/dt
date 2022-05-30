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
    internal sealed class DocumentSummaryInfomation : IPropertyAccess
    {
        private byte[] _dataBuffer;

        private DocumentSummaryInfomation(byte[] data)
        {
            this._dataBuffer = data;
        }

        public T GetProperty<T>(int propertyId)
        {
            throw new NotImplementedException();
        }

        internal static DocumentSummaryInfomation Read(byte[] data)
        {
            return new DocumentSummaryInfomation(data);
        }

        public void RemoveProperty(int propertyId)
        {
            throw new NotImplementedException();
        }

        public void SetProperty<T>(int propertyId, T value)
        {
            throw new NotImplementedException();
        }
    }
}


#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections.Generic;
using System.Reflection;
#endregion

namespace Dt.Cells.Data
{
    internal static class ConnectionBuilder
    {
        public static ConnectionBase Build(object source)
        {
            ConnectionBase[] baseArray = SearchConnections();
            if (baseArray != null)
            {
                foreach (ConnectionBase base2 in baseArray)
                {
                    if (base2 != null)
                    {
                        base2.DataSource = source;
                        if (base2.CanOpen())
                        {
                            return base2;
                        }
                        base2.DataSource = null;
                    }
                }
            }
            throw new NotSupportedException(ResourceStrings.DataBindingNullConnection);
        }

        static ConnectionBase[] SearchConnections()
        {
            List<ConnectionBase> list = new List<ConnectionBase>();
            if (ConnectionBase.externalConnectionTypes != null)
            {
                foreach (Type type in ConnectionBase.externalConnectionTypes)
                {
                    if (IntrospectionExtensions.GetTypeInfo((Type) typeof(ConnectionBase)).IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(type)) && (type != typeof(CollectionViewConnection)))
                    {
                        try
                        {
                            object obj2 = Activator.CreateInstance(type);
                            list.Add(obj2 as ConnectionBase);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            list.Add(new CollectionViewConnection());
            list.Add(new CsvFileConnection());
            return list.ToArray();
        }
    }
}


#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Charts
{
    public class ChartTypeInfo
    {
        string _name;
        List<ChartSubtype> _stypes;

        public ChartTypeInfo()
        {
            _stypes = new List<ChartSubtype>();
        }

        internal ChartTypeInfo(string name)
        {
            _stypes = new List<ChartSubtype>();
            _name = name;
        }

        public ChartSubtype this[string name]
        {
            get
            {
                foreach (ChartSubtype subtype in Subtypes)
                {
                    if (subtype.Name == name)
                    {
                        return subtype;
                    }
                }
                return null;
            }
        }

        public string Name
        {
            get { return  _name; }
            set { _name = value; }
        }

        public List<ChartSubtype> Subtypes
        {
            get { return  _stypes; }
        }
    }
}


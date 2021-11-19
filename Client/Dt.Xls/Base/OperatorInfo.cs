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
    internal class OperatorInfo
    {
        private string _name = string.Empty;

        public OperatorInfo(string name)
        {
            this._name = name;
        }

        /// <summary>
        /// Gets the name of the operator.
        /// </summary>
        public string Name
        {
            get { return  this._name; }
        }
    }
}


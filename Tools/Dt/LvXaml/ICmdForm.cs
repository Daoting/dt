using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dt.Editor
{
    public interface ICmdForm
    {
        /// <summary>
        /// 获取要插入的文本
        /// </summary>
        /// <returns></returns>
        string GetText();
    }
}

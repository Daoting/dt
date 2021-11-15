#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Mask
{
    internal class RegExpDupSymbol
    {
        #region 成员变量
        public readonly int _MaxMatches;
        public readonly int _MinMatches;
        #endregion

        #region 构造方法
        public RegExpDupSymbol(object minMatches, object maxMatches)
        {
            _MinMatches = (int) minMatches;
            _MaxMatches = (int) maxMatches;
        }
        #endregion
    }
}


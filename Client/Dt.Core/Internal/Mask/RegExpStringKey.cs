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
    internal sealed class RegExpStringKey
    {
        #region 成员变量
        int _hash;
        public int _Length;
        public RegExpStringKey _Next;
        public char _Symbol;
        public RegExpDfaWave _Wave;
        #endregion

        #region 构造方法
        public RegExpStringKey(string str, RegExpDfaWave wave, RegExpStringKey next, char symbol)
        {
            this._Next = next;
            this._Symbol = symbol;
            this._hash = str.GetHashCode();
            this._Length = str.Length;
            this._Wave = wave;
        }
        #endregion

        #region 重写方法
        public override int GetHashCode()
        {
            return this._hash;
        }
        #endregion
    }
}


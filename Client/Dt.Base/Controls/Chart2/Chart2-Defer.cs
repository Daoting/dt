#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-09-13 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 延时刷新或重绘
    /// </summary>
    public partial class Chart2
    {
        /// <summary>
        /// 延时刷新或清空重绘
        /// <example>
        /// <code>
        /// using (_c.Defer(true))
        /// {
        ///     _c.Add.Signal(Generate.Sin(51));
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="p_reset">是否清空重绘</param>
        /// <returns></returns>
        public IDisposable Defer(bool p_reset)
        {
            if (p_reset)
                Reset();
            return new InternalCls(this);
        }

        int _updating;
        
        internal int Updating
        {
            get { return _updating; }
            set
            {
                _updating = value;
                if (_updating == 0)
                {
                    Refresh();
                }
            }
        }

        class InternalCls : IDisposable
        {
            Chart2 _owner;

            public InternalCls(Chart2 p_owner)
            {
                _owner = p_owner;
                _owner.Updating = _owner.Updating + 1;
            }

            public void Dispose()
            {
                _owner.Updating = _owner.Updating - 1;
            }
        }
    }
}
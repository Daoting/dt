#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Core
{
    public class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// INotifyPropertyChanged接口事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 检查新值和原有值是否相同，不同则赋值并触发属性变化事件
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="storage">属性的引用</param>
        /// <param name="value">属性新值</param>
        /// <param name="propertyName">通知更改时的属性名称，可自动给出</param>
        /// <returns>True表示属性变化, false表示无更改.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// 触发属性变化事件
        /// </summary>
        /// <param name="propertyName">通知更改时的属性名称<see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
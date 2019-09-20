#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-09-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Base.Transfer
{
    /// <summary>
    /// 下载命令
    /// </summary>
    public class DownloadFileCmd : BaseCommand
    {
        private FileTransfer _owner;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_owner"></param>
        public DownloadFileCmd(FileTransfer p_owner)
        {
            _owner = p_owner;
            AllowExecute = true;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoExecute(object p_parameter)
        {
            _owner.DownloadFile(_owner.Current);
        }
    }
}

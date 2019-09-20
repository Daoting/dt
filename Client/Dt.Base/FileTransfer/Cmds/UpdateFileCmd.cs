#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-09-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
#endregion

namespace Dt.Base.Transfer
{
    /// <summary>
    /// 更新文件命令
    /// </summary>
    public class UpdateFileCmd : BaseCommand
    {
        private FileTransfer _owner;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_owner"></param>
        public UpdateFileCmd(FileTransfer p_owner)
        {
            _owner = p_owner;
            AllowExecute = true;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="p_parameter"></param>
        protected async override void DoExecute(object p_parameter)
        {
            VirFile vf = _owner.Current;
            if (vf == null || vf.State != VirFileState.None)
                return;

            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
                await _owner.UpdateFile(file, vf);
        }
    }
}

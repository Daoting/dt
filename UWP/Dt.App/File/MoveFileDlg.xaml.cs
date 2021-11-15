#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.App.File
{
    public sealed partial class MoveFileDlg : Dlg
    {
        IFileMgr _fileMgr;
        IEnumerable<Row> _files;

        public MoveFileDlg()
        {
            InitializeComponent();
            Title = "移到";
        }

        public IFileMgr Target { get; private set; }

        public async Task<bool> Show(IFileMgr p_fileMgr, IEnumerable<Row> p_rows)
        {
            _fileMgr = p_fileMgr;
            _files = p_rows;
            if (!Kit.IsPhoneUI)
            {
                Width = 300;
                Height = 400;
            }

            var mgr = (IFileMgr)Activator.CreateInstance(_fileMgr.GetType());
            LoadMv(new MoveFilePage(mgr, this));
            return await ShowAsync();
        }

        public async void MoveTo(IFileMgr p_target)
        {
            if (_fileMgr.FolderID == p_target.FolderID)
            {
                Kit.Warn("不需要移动！");
                return;
            }

            if (await _fileMgr.MoveFiles(_files, p_target.FolderID))
            {
                Target = p_target;
                Close(true);
            }
            else
            {
                Kit.Warn("移动失败！");
            }
        }

        /// <summary>
        /// 过滤已选择的文件夹，避免移到自己的子目录！
        /// </summary>
        /// <param name="p_tbl"></param>
        /// <param name="p_target"></param>
        public void RemoveSelection(Table p_tbl, IFileMgr p_target)
        {
            if (p_target.FolderID != _fileMgr.FolderID || p_tbl.Count == 0)
                return;

            foreach (var row in _files)
            {
                if (row.Bool("IsFolder"))
                {
                    for (int i = 0; i < p_tbl.Count; i++)
                    {
                        if (row.ID == p_tbl[i].ID)
                        {
                            p_tbl.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }
    }
}

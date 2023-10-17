﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public sealed partial class 角色Form : Tab
    {
        #region 构造方法
        public 角色Form()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async Task Update(long p_id)
        {
            var d = Data;
            if (d != null && d.ID == p_id)
                return;

            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                Data = await 角色X.GetByID(p_id);
                UpdateRelated(p_id);
            }
            else
            {
                Create();
            }
        }

        public void Clear()
        {
            Data = null;
            UpdateRelated(-1);
        }

        public 角色X Data
        {
            get { return _fv.Data.To<角色X>(); }
            private set { _fv.Data = value; }
        }
        #endregion

        #region 内部
        async void Create()
        {
            Data = await 角色X.New();
            UpdateRelated(-1);
        }

        async void Save()
        {
            var d = Data;
            bool isNew = d.IsAdded;
            if (await d.Save())
            {
                _win.MainList.Update();
                if (isNew)
                {
                    UpdateRelated(d.ID);
                }
            }
        }

        async void Delete()
        {
            var d = Data;
            if (d == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (d.IsAdded)
            {
                Clear();
                return;
            }

            if (await d.Delete())
            {
                Clear();
                _win.MainList.Update();
            }
        }

        void UpdateRelated(long p_id)
        {
            _win.权限List.Update(p_id);
            _win.用户List.Update(p_id);
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        角色Win _win => OwnWin as 角色Win;
        #endregion
    }
}

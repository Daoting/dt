﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Base.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Demo.Crud
{
    [View("管理样例")]
    public partial class DemoMain : Win
    {
        public DemoMain()
        {
            InitializeComponent();
            LoadBase();
            LoadDomain();
        }

        void LoadBase()
        {
            var ds = new Nl<GroupData<Nav>>();
            var group = new GroupData<Nav>
            {
                new Nav("序列化类型", typeof(SerializeDemo), Icons.全选) { Desc ="与服务之间的调用和数据传递" },
                new Nav("异常处理", typeof(ExceptionDemo), Icons.警告) { Desc ="客户端和服务端异常" },
                new Nav("远程过程调用", typeof(RpcDemo), Icons.耳麦) { Desc = "Rpc调用" },
                new Nav("服务端Api授权控制", typeof(AuthAccess), Icons.小图标) { Desc ="Api授权控制" },
            };
            group.Title = "Rpc基础";
            ds.Add(group);

            group = new GroupData<Nav>
            {
                new Nav("FileList上传下载", typeof(FileListDemo), Icons.日历) { Desc ="跨平台文件上传下载" },
                new Nav("文件格", typeof(FileCellDemo), Icons.文件) { Desc = "文件格、图像格" },
                new Nav("Lv的文件列表", typeof(FileLvDemo), Icons.文件夹) { Desc = "文件格、图像格" },
                new Nav("文件选择", typeof(FilePickerDemo), Icons.保存) { Desc = "文件类型过滤、单选、多选" },
                new Nav("拍照录像", typeof(CameraCaptureDemo), Icons.保存) { Desc = "拍照、录像生成文件" },
                new Nav("图像资源", typeof(ImgFileDemo), Icons.图片) { Desc = "不同类型图像资源的显示" },
                new Nav("CList数据源", typeof(CListDemo), Icons.列表) { Desc = "CList通过Ex Sql属性定义数据源" },
                new Nav("CPick数据源", typeof(CPickDemo), Icons.表格) { Desc = "CPick数据源sql过滤、本地过滤" },
            };
            group.Title = "控件";
            ds.Add(group);
            _navBase.Data = ds;
        }

        void LoadDomain()
        {
            var ds = new Nl<GroupData<Nav>>();
            var group = new GroupData<Nav>
            {
                new Nav("实体基础", typeof(AccessDemo)) { Desc = "客户端实体、虚拟实体、父子实体的增删改查、缓存、领域事件等" },
                new Nav("Sqlite实体", typeof(SqliteAccessDemo)) { Desc = "Sqlite实体除了无缓存和无序列外，其余功能都包括"},
                new Nav("服务端实体", typeof(SvcAccessDemo)) { Desc = "服务端实体、虚拟实体、父子实体的增删改查、缓存、领域事件等" },
                new Nav("直连数据库", typeof(DbAccessDemo)) { Desc = "客户端直连数据库进行实体增删改查，可切换不同库"},
            };
            group.Title = "基础";
            ds.Add(group);

            group = new GroupData<Nav>
            {
                new Nav("单实体", typeof(基础Win)) { Desc = "单表的增删改查框架" },
                new Nav("虚拟实体", typeof(虚拟Win)) { Desc = "因字段过多将单表拆分成多表时适用于虚拟实体，本质还是单实体框架" },
                new Nav("树形单实体", typeof(树形Win)) { Desc = "树形单表的增删改查框架" },
                new Nav("字段类型", typeof(字段类型Win)) { Desc = "框架根据字段类型生成的默认查询，方便后续修改" },
                new Nav("通用单表视图")
                {
                    Callback = (o, e) => GenericView.OpenSingleTbl(new EntityCfg { Cls = "Demo.Base.基础X,Demo.Base" }),
                    Desc = "只提供参数无需另外代码，实现单表的增删改查"
                },
                new Nav("视图参数编辑")
                {
                    Callback = async (o, e) =>
                    {
                        var cfg = new EntityCfg { Cls = "Demo.Base.基础X,Demo.Base" };
                        var json = await new EntityDesign().ShowDlg(cfg.Serialize());
                        if (!string.IsNullOrEmpty(json))
                        {
                            Kit.Msg("json请查看日志");
                            Log.Debug(json);
                        }
                    },
                    Desc = "通用单表视图参数编辑"
                },
            };
            group.Title = "单实体框架";
            ds.Add(group);

            group = new GroupData<Nav>
            {
                new Nav("普通表单", typeof(普通Win)) { Desc = "一对多实体增删改查" },
                new Nav("父子表单", typeof(父表Win)) { Desc = "一对多实体增删改查，表单统一处理父子增删改" },
                new Nav("通用一对多视图")
                {
                    Callback = (o, e) => GenericView.OpenOneToMany(CreateOneToManyCfg()),
                    Desc = "只提供参数无需另外代码，实现一对多表的增删改查"
                },

                new Nav("视图参数编辑")
                {
                    Callback = async (o, e) =>
                    {
                        var cfg = CreateOneToManyCfg();
                        var json = await new OneToManyDesign().ShowDlg(cfg.Serialize());
                        if (!string.IsNullOrEmpty(json))
                        {
                            Kit.Msg("json请查看日志");
                            Log.Debug(json);
                        }
                    },
                    Desc = "通用一对多视图参数编辑"
                },
            };
            group.Title = "一对多框架";
            ds.Add(group);

            group = new GroupData<Nav>
            {
                new Nav("角色", typeof(角色Win)) { Desc = "主实体对多个关联实体" },
                new Nav("用户", typeof(用户Win)) { Desc = "用户对角色" },
                new Nav("权限", typeof(权限Win)) { Desc = "权限对角色" },
                new Nav("通用多对多视图")
                {
                    Callback = (o, e) => GenericView.OpenManyToMany(CreateManyToManyCfg()),
                    Desc = "只提供参数无需另外代码，实现多对多表的增删改查"
                },

                new Nav("视图参数编辑")
                {
                    Callback = async (o, e) =>
                    {
                        var cfg = CreateManyToManyCfg();
                        var json = await new ManyToManyDesign().ShowDlg(cfg.Serialize());
                        if (!string.IsNullOrEmpty(json))
                        {
                            Kit.Msg("json请查看日志");
                            Log.Debug(json);
                        }
                    },
                    Desc = "通用多对多视图参数编辑"
                },
            };
            group.Title = "多对多框架";
            ds.Add(group);
            _navEntity.Data = ds;
        }

        OneToManyCfg CreateOneToManyCfg()
        {
            OneToManyCfg cfg = new OneToManyCfg();
            cfg.IsUnionForm = true;
            cfg.ParentCfg = new EntityCfg { Cls = "Demo.Base.父表X,Demo.Base" };
            cfg.ChildCfgs.Add(new EntityCfg { Cls = "Demo.Base.大儿X,Demo.Base", ParentID = "parent_id", IsChild = true });
            cfg.ChildCfgs.Add(new EntityCfg { Cls = "Demo.Base.小儿X,Demo.Base", ParentID = "group_id", IsChild = true });
            return cfg;
        }

        ManyToManyCfg CreateManyToManyCfg()
        {
            var cfg = new ManyToManyCfg();
            cfg.MainCfg = new EntityCfg { Cls = "Demo.Base.角色X,Demo.Base" };
            cfg.RelatedCfgs.Add(
                new RelatedEntityCfg
                {
                    RelatedCls = "Demo.Base.用户X,Demo.Base",
                    MiddleCls = "Demo.Base.用户角色X,Demo.Base",
                    MainFk = "role_id",
                    RelatedFk = "user_id"
                });
            cfg.RelatedCfgs.Add(
                new RelatedEntityCfg
                {
                    RelatedCls = "Demo.Base.权限X,Demo.Base",
                    MiddleCls = "Demo.Base.角色权限X,Demo.Base",
                    MainFk = "role_id",
                    RelatedFk = "prv_id"
                });
            return cfg;
        }
    }
}
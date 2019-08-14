#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Cm.Sqlite
{
    /// <summary>
    /// 业务菜单
    /// </summary>
    [Table]
    public class OmMenu
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        [PrimaryKey, MaxLength(32)]
        public string ID { get; set; }

        /// <summary>
        /// 父菜单项ID
        /// </summary>
        [Indexed, MaxLength(32)]
        public string ParentID { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// 菜单类型是否为分组
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// 菜单对应的视图名称
        /// </summary>
        [MaxLength(128)]
        public string ViewName { get; set; }

        /// <summary>
        /// 菜单参数
        /// </summary>
        public string Params { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 提供提示信息的服务名称，空表示无提示信息
        /// </summary>
        public string SrvName { get; set; }

        /// <summary>
        /// 菜单描述
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DispIdx { get; set; }
    }

    /// <summary>
    /// 基础代码
    /// </summary>
    [Table]
    public class OmBaseCode
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey, MaxLength(64)]
        public string ID { get; set; }

        /// <summary>
        /// 所属分组
        /// </summary>
        [Indexed, MaxLength(64)]
        public string Grp { get; set; }
    }

    /// <summary>
    /// 报表模板
    /// </summary>
    [Table]
    public class OmReport
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey, MaxLength(32)]
        public string ID { get; set; }

        /// <summary>
        /// 报表名称
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// 报表定义
        /// </summary>
        public string Define { get; set; }
    }

    /// <summary>
    /// 系统列定义
    /// </summary>
    [Table]
    public class OmColumn
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// 所属表名
        /// </summary>
        [Indexed, MaxLength(30)]
        public string TabName { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        [MaxLength(30)]
        public string ColName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [MaxLength(128)]
        public string DbType { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// 列长度，只字符类型有效
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// 列是否允许为空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 列注释
        /// </summary>
        public string Comments { get; set; }
    }

    /// <summary>
    /// 公告类型
    /// </summary>
    [Table]
    public class OmNoticeType
    {
        /// <summary>
        /// 公告类型标识
        /// </summary>
        [PrimaryKey]
        public string ID { get; set; }

        /// <summary>
        /// 提供对该类型公告浏览功能的类名
        /// </summary>
        public string Browser { get; set; }

        /// <summary>
        /// 提供对该类型公告编辑功能的类名
        /// </summary>
        public string Editor { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DispIdx { get; set; }
    }

    /// <summary>
    /// 流程模板定义
    /// </summary>
    [Table]
    public class PrcDef
    {
        /// <summary>
        /// 流程标识
        /// </summary>
        [PrimaryKey]
        public string ID { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string SrvName { get; set; }

        /// <summary>
        /// 业务主表表名
        /// </summary>
        public string TblName { get; set; }

        /// <summary>
        /// 流程图
        /// </summary>
        public string Diagram { get; set; }
    }

    /// <summary>
    /// 活动模板定义
    /// </summary>
    [Table]
    public class AtvDef
    {
        /// <summary>
        /// 活动标识
        /// </summary>
        [PrimaryKey]
        public string ID { get; set; }

        /// <summary>
        /// 流程标识
        /// </summary>
        public string PrcID { get; set; }

        /// <summary>
        /// 活动名称，同时作为状态名称，可重复
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 活动类别
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 执行者范围
        /// </summary>
        public string ExecScope { get; set; }

        /// <summary>
        /// 执行者限制
        /// </summary>
        public string ExecLimit { get; set; }

        /// <summary>
        /// 在执行者执行者限制为F或S时选择的活动
        /// </summary>
        public string ExecAtvID { get; set; }

        /// <summary>
        /// 是否自动签收，打开工作流视图时自动签收工作项
        /// </summary>
        public string AutoAccept { get; set; }

        /// <summary>
        /// 能否删除流程实例和业务数据，0否 1能
        /// </summary>
        public string CanDelete { get; set; }

        /// <summary>
        /// 能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能
        /// </summary>
        public string CanTerminate { get; set; }

        /// <summary>
        /// 是否可作为跳转目标，0不可跳转 1可以
        /// </summary>
        public string CanJumpinto { get; set; }

        /// <summary>
        /// 同步活动有效，聚合方式
        /// </summary>
        public string JoinKind { get; set; }

        /// <summary>
        /// 当前活动的后续迁移方式
        /// </summary>
        public string TransKind { get; set; }
    }

    /// <summary>
    /// 角色关联的菜单
    /// </summary>
    [Table]
    public class RoleMenu
    {
        /// <summary>
        /// 角色标识
        /// </summary>
        [Indexed, MaxLength(32)]
        public string RoleID { get; set; }

        /// <summary>
        /// 菜单标识
        /// </summary>
        public string MenuID { get; set; }
    }

    /// <summary>
    /// 角色关联的权限
    /// </summary>
    [Table]
    public class RolePrv
    {
        /// <summary>
        /// 角色标识
        /// </summary>
        [Indexed, MaxLength(32)]
        public string RoleID { get; set; }

        /// <summary>
        /// 权限标识
        /// </summary>
        public string PrvID { get; set; }
    }

    /// <summary>
    /// 仪表盘
    /// </summary>
    [Table]
    public class OmBoard
    {
        /// <summary>
        /// 标识
        /// </summary>
        [PrimaryKey]
        public string ID { get; set; }

        /// <summary>
        /// 名称，唯一
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所属服务名称
        /// </summary>
        public string SrvName { get; set; }

        /// <summary>
        /// 仪表的类名
        /// </summary>
        public string Cls { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DispIdx { get; set; }
    }

    /// <summary>
    /// 仪表盘角色授权
    /// </summary>
    [Table]
    public class BoardRole
    {
        /// <summary>
        /// 角色标识
        /// </summary>
        [Indexed, MaxLength(32)]
        public string RoleID { get; set; }

        /// <summary>
        /// 仪表标识
        /// </summary>
        public string BoardID { get; set; }
    }

    /// <summary>
    /// 仪表盘用户授权
    /// </summary>
    [Table]
    public class BoardUser
    {
        /// <summary>
        /// 角色标识
        /// </summary>
        [Indexed, MaxLength(32)]
        public string UserID { get; set; }

        /// <summary>
        /// 仪表标识
        /// </summary>
        public string BoardID { get; set; }
    }
}

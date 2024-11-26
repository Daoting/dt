-- ----------------------------
-- 按照依赖顺序删除demo库对象
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[v_物资目录]') AND type IN ('V')) DROP VIEW [dbo].[v_物资目录]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[v_人员]') AND type IN ('V')) DROP VIEW [dbo].[v_人员]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[v_部门]') AND type IN ('V')) DROP VIEW [dbo].[v_部门]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[v_物资主单]') AND type IN ('V')) DROP VIEW [dbo].[v_物资主单]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[v_物资详单]') AND type IN ('V')) DROP VIEW [dbo].[v_物资详单]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_基础]') AND type IN ('U')) DROP TABLE [dbo].[crud_基础]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_扩展1]') AND type IN ('U')) DROP TABLE [dbo].[crud_扩展1]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_扩展2]') AND type IN ('U')) DROP TABLE [dbo].[crud_扩展2]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_主表]') AND type IN ('U')) DROP TABLE [dbo].[crud_主表]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_大儿]') AND type IN ('U')) DROP TABLE [dbo].[crud_大儿]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_小儿]') AND type IN ('U')) DROP TABLE [dbo].[crud_小儿]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_父表]') AND type IN ('U')) DROP TABLE [dbo].[crud_父表]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_缓存表]') AND type IN ('U')) DROP TABLE [dbo].[crud_缓存表]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_角色权限]') AND type IN ('U')) DROP TABLE [dbo].[crud_角色权限]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_用户角色]') AND type IN ('U')) DROP TABLE [dbo].[crud_用户角色]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_用户]') AND type IN ('U')) DROP TABLE [dbo].[crud_用户]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_角色]') AND type IN ('U')) DROP TABLE [dbo].[crud_角色]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_权限]') AND type IN ('U')) DROP TABLE [dbo].[crud_权限]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_字段类型]') AND type IN ('U')) DROP TABLE [dbo].[crud_字段类型]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[物资详单]') AND type IN ('U')) DROP TABLE [dbo].[物资详单]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[物资主单]') AND type IN ('U')) DROP TABLE [dbo].[物资主单]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[物资计划明细]') AND type IN ('U')) DROP TABLE [dbo].[物资计划明细]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[物资计划]') AND type IN ('U')) DROP TABLE [dbo].[物资计划]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[物资库存]') AND type IN ('U')) DROP TABLE [dbo].[物资库存]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[物资目录]') AND type IN ('U')) DROP TABLE [dbo].[物资目录]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[物资分类]') AND type IN ('U')) DROP TABLE [dbo].[物资分类]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[物资入出类别]') AND type IN ('U')) DROP TABLE [dbo].[物资入出类别]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[供应商]') AND type IN ('U')) DROP TABLE [dbo].[供应商]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[部门人员]') AND type IN ('U')) DROP TABLE [dbo].[部门人员]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[部门]') AND type IN ('U')) DROP TABLE [dbo].[部门]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[人员]') AND type IN ('U')) DROP TABLE [dbo].[人员]
GO


-- ----------------------------
-- 按照依赖顺序删除dt初始库对象
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_cache]') AND type IN ('U')) DROP TABLE [dbo].[cm_cache]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfi_trs]') AND type IN ('U')) DROP TABLE [dbo].[cm_wfi_trs]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfi_item]') AND type IN ('U')) DROP TABLE [dbo].[cm_wfi_item]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfi_atv]') AND type IN ('U')) DROP TABLE [dbo].[cm_wfi_atv]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfi_prc]') AND type IN ('U')) DROP TABLE [dbo].[cm_wfi_prc]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfd_trs]') AND type IN ('U')) DROP TABLE [dbo].[cm_wfd_trs]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfd_atv_role]') AND type IN ('U')) DROP TABLE [dbo].[cm_wfd_atv_role]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfd_atv]') AND type IN ('U')) DROP TABLE [dbo].[cm_wfd_atv]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfd_prc]') AND type IN ('U')) DROP TABLE [dbo].[cm_wfd_prc]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_user_group]') AND type IN ('U')) DROP TABLE [dbo].[cm_user_group]
GO
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_user_params]') AND type IN ('U')) DROP TABLE [dbo].[cm_user_params]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_user_role]') AND type IN ('U')) DROP TABLE [dbo].[cm_user_role]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_group_role]') AND type IN ('U')) DROP TABLE [dbo].[cm_group_role]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_role_menu]') AND type IN ('U')) DROP TABLE [dbo].[cm_role_menu]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_role_per]') AND type IN ('U')) DROP TABLE [dbo].[cm_role_per]
GO
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_group]') AND type IN ('U')) DROP TABLE [dbo].[cm_group]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_menu]') AND type IN ('U')) DROP TABLE [dbo].[cm_menu]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_option]') AND type IN ('U')) DROP TABLE [dbo].[cm_option]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_option_group]') AND type IN ('U')) DROP TABLE [dbo].[cm_option_group]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_params]') AND type IN ('U')) DROP TABLE [dbo].[cm_params]
GO
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_permission]') AND type IN ('U')) DROP TABLE [dbo].[cm_permission]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_permission_func]') AND type IN ('U')) DROP TABLE [dbo].[cm_permission_func]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_permission_module]') AND type IN ('U')) DROP TABLE [dbo].[cm_permission_module]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_role]') AND type IN ('U')) DROP TABLE [dbo].[cm_role]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_rpt]') AND type IN ('U')) DROP TABLE [dbo].[cm_rpt]
GO
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_file_pub]') AND type IN ('U')) DROP TABLE [dbo].[cm_file_pub]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_file_my]') AND type IN ('U')) DROP TABLE [dbo].[cm_file_my]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_user]') AND type IN ('U')) DROP TABLE [dbo].[cm_user]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[fsm_file]') AND type IN ('U')) DROP TABLE [dbo].[fsm_file]
GO

-- ----------------------------
-- Table structure for cm_cache
-- ----------------------------
CREATE TABLE [dbo].[cm_cache] (
  [id] nvarchar(255) NOT NULL,
  [val] nvarchar(512) NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'模拟redis缓存key value数据，直连数据库时用',
'SCHEMA', N'dbo',
'TABLE', N'cm_cache'
GO

-- ----------------------------
-- Table structure for cm_file_my
-- ----------------------------
CREATE TABLE [dbo].[cm_file_my] (
  [id] bigint NOT NULL,
  [parent_id] bigint NULL,
  [name] nvarchar(255) NOT NULL,
  [is_folder] bit NOT NULL,
  [ext_name] nvarchar(8) NULL,
  [info] nvarchar(512) NULL,
  [ctime] datetime NOT NULL,
  [user_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'文件标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_my',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'上级目录，根目录的parendid为空',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_my',
'COLUMN', N'parent_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_my',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否为文件夹',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_my',
'COLUMN', N'is_folder'
GO

EXEC sp_addextendedproperty
'MS_Description', N'文件扩展名',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_my',
'COLUMN', N'ext_name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'文件描述信息',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_my',
'COLUMN', N'info'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_my',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'所属用户',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_my',
'COLUMN', N'user_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'个人文件',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_my'
GO


-- ----------------------------
-- Table structure for cm_file_pub
-- ----------------------------
CREATE TABLE [dbo].[cm_file_pub] (
  [id] bigint NOT NULL,
  [parent_id] bigint NULL,
  [name] nvarchar(255) NOT NULL,
  [is_folder] bit NOT NULL,
  [ext_name] nvarchar(8) NULL,
  [info] nvarchar(512) NULL,
  [ctime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'文件标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_pub',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'上级目录，根目录的parendid为空',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_pub',
'COLUMN', N'parent_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_pub',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否为文件夹',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_pub',
'COLUMN', N'is_folder'
GO

EXEC sp_addextendedproperty
'MS_Description', N'文件扩展名',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_pub',
'COLUMN', N'ext_name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'文件描述信息',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_pub',
'COLUMN', N'info'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_pub',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'公共文件',
'SCHEMA', N'dbo',
'TABLE', N'cm_file_pub'
GO


-- ----------------------------
-- Records of cm_file_pub
-- ----------------------------
INSERT INTO [dbo].[cm_file_pub] VALUES (N'1', NULL, N'公共文件', N'1', NULL, N'', N'2020-10-21 15:19:20'), (N'2', NULL, N'素材库', N'1', NULL, N'', N'2020-10-21 15:20:21')
GO


-- ----------------------------
-- Table structure for cm_group
-- ----------------------------
CREATE TABLE [dbo].[cm_group] (
  [id] bigint NOT NULL,
  [name] nvarchar(64) NOT NULL,
  [note] nvarchar(255) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'组标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_group',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'组名',
'SCHEMA', N'dbo',
'TABLE', N'cm_group',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'组描述',
'SCHEMA', N'dbo',
'TABLE', N'cm_group',
'COLUMN', N'note'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户组，与用户和角色多对多',
'SCHEMA', N'dbo',
'TABLE', N'cm_group'
GO


-- ----------------------------
-- Table structure for cm_group_role
-- ----------------------------
CREATE TABLE [dbo].[cm_group_role] (
  [group_id] bigint NOT NULL,
  [role_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'组标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_group_role',
'COLUMN', N'group_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_group_role',
'COLUMN', N'role_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'组一角色多对多',
'SCHEMA', N'dbo',
'TABLE', N'cm_group_role'
GO


-- ----------------------------
-- Table structure for cm_menu
-- ----------------------------
CREATE TABLE [dbo].[cm_menu] (
  [id] bigint NOT NULL,
  [parent_id] bigint NULL,
  [name] nvarchar(64) NOT NULL,
  [is_group] bit NOT NULL,
  [view_name] nvarchar(128) NULL,
  [params] nvarchar(4000) NULL,
  [icon] nvarchar(128) NULL,
  [note] nvarchar(512) NULL,
  [dispidx] int NOT NULL,
  [is_locked] bit NOT NULL,
  [ctime] datetime NOT NULL,
  [mtime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'菜单标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'父菜单标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'parent_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'菜单名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'分组或实例。0表实例，1表分组',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'is_group'
GO

EXEC sp_addextendedproperty
'MS_Description', N'视图名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'view_name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'传递给菜单程序的参数',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'params'
GO

EXEC sp_addextendedproperty
'MS_Description', N'图标',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'icon'
GO

EXEC sp_addextendedproperty
'MS_Description', N'备注',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'note'
GO

EXEC sp_addextendedproperty
'MS_Description', N'显示顺序',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'dispidx'
GO

EXEC sp_addextendedproperty
'MS_Description', N'定义了菜单是否被锁定。0表未锁定，1表锁定不可用',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'is_locked'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'最后修改时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'业务菜单',
'SCHEMA', N'dbo',
'TABLE', N'cm_menu'
GO


-- ----------------------------
-- Records of cm_menu
-- ----------------------------
INSERT INTO [dbo].[cm_menu] VALUES
(N'93146668397260800', NULL, N'基础维护', N'1', NULL, NULL, NULL, NULL, N'1', N'0', N'2024-06-14 08:51:37', N'2024-06-14 08:51:37'),
(N'93147399237955584', N'93146668397260800', N'部门管理', N'0', N'部门管理', NULL, N'多人', NULL, N'115', N'0', N'2024-06-14 08:54:32', N'2024-06-14 08:54:32'),
(N'93147789455028224', N'93146668397260800', N'人员管理', N'0', N'人员管理', NULL, N'个人信息', NULL, N'117', N'0', N'2024-06-14 08:56:05', N'2024-06-14 08:56:05'),
(N'95003376719523840', N'93146668397260800', N'供应商管理', N'0', N'供应商管理', NULL, N'全局', NULL, N'119', N'0', N'2024-06-19 11:49:30', N'2024-06-19 11:49:30'),
(N'96885816660619264', NULL, N'物资管理', N'1', NULL, NULL, NULL, NULL, N'122', N'0', N'2024-06-24 16:29:45', N'2024-06-24 16:29:45'),
(N'1', NULL, N'工作台', N'1', N'', NULL, N'搬运工', NULL, N'123', N'0', N'2019-03-07 10:45:44', N'2019-03-07 10:45:43'),
(N'97869834403213312', NULL, N'测试组', N'1', NULL, NULL, NULL, NULL, N'130', N'0', N'2024-06-27 09:39:50', N'2024-06-27 09:39:50'),
(N'97869954830069760', N'97869834403213312', N'一级菜单1', N'0', NULL, NULL, N'文件', NULL, N'131', N'0', N'2024-06-27 09:40:18', N'2024-06-27 09:40:18'),
(N'97870059381485568', N'97869834403213312', N'一级菜单2', N'0', NULL, NULL, N'文件', NULL, N'132', N'0', N'2024-06-27 09:40:43', N'2024-06-27 09:40:43'),
(N'97870113269903360', N'97869834403213312', N'二级组', N'1', NULL, NULL, NULL, NULL, N'133', N'0', N'2024-06-27 09:40:56', N'2024-06-27 09:40:56'),
(N'97870286377218048', N'97870113269903360', N'二级菜单1', N'0', NULL, NULL, N'文件', NULL, N'134', N'0', N'2024-06-27 09:41:37', N'2024-06-27 09:41:37'),
(N'97870350000615424', N'97870113269903360', N'二级菜单2', N'0', NULL, NULL, N'文件', NULL, N'135', N'0', N'2024-06-27 09:41:52', N'2024-06-27 09:41:52'),
(N'97871217135218688', N'97870113269903360', N'三级组', N'1', NULL, NULL, NULL, NULL, N'136', N'0', N'2024-06-27 09:45:19', N'2024-06-27 09:45:19'),
(N'97871290111913984', N'97871217135218688', N'三级菜单', N'0', NULL, NULL, N'文件', NULL, N'137', N'0', N'2024-06-27 09:45:37', N'2024-06-27 09:45:37'),
(N'105150016726003712', N'93146668397260800', N'物资入出类别', N'0', N'物资入出类别', NULL, N'分组', NULL, N'138', N'0', N'2024-07-17 11:48:40', N'2024-07-17 11:48:40'),
(N'95004558183657472', N'93146668397260800', N'物资目录管理', N'0', N'物资目录管理', NULL, N'树形', NULL, N'121', N'0', N'2024-06-19 11:54:11', N'2024-06-19 11:54:11'),
(N'3', N'1', N'用户组', N'0', N'用户组', NULL, N'分组', N'管理用户组、组内用户，为用户组分配角色', N'3', N'0', N'2023-03-10 08:34:49', N'2023-03-10 08:34:49'),
(N'5', N'1', N'基础权限', N'0', N'基础权限', NULL, N'审核', N'按照模块和功能两级目录管理权限、将权限分配给角色', N'5', N'0', N'2019-03-12 09:11:22', N'2019-03-07 11:23:40'),
(N'6', N'1', N'菜单管理', N'0', N'菜单管理', NULL, N'大图标', N'菜单和菜单组管理、将菜单授权给角色', N'6', N'0', N'2019-03-11 11:35:59', N'2019-03-11 11:35:58'),
(N'96886018188537856', N'96885816660619264', N'物资入出管理', N'0', N'物资入出', NULL, N'四面体', NULL, N'124', N'0', N'2024-06-24 16:30:33', N'2024-06-24 16:30:33'),
(N'96889439553613824', N'96885816660619264', N'物资盘存管理', N'0', N'物资盘存', NULL, N'文件', NULL, N'128', N'0', N'2024-06-24 16:44:09', N'2024-06-24 16:44:09'),
(N'96889910070636544', N'96885816660619264', N'物资计划管理', N'0', N'物资计划', NULL, N'外设', NULL, N'129', N'0', N'2024-06-24 16:46:01', N'2024-06-24 16:46:01'),
(N'7', N'1', N'报表设计', N'0', N'报表设计', NULL, N'折线图', N'报表管理及报表模板设计', N'7', N'0', N'2020-10-19 11:21:38', N'2020-10-19 11:21:38'),
(N'2', N'1', N'用户账号', N'0', N'用户账号', NULL, N'钥匙', N'用户账号及所属用户组管理、为用户分配角色、查看用户可访问菜单和已授权限', N'2', N'0', N'2019-11-08 11:42:28', N'2019-11-08 11:43:53'),
(N'9', N'1', N'参数定义', N'0', N'参数定义', NULL, N'调色板', N'参数名称、默认值的定义管理', N'9', N'0', N'2019-03-12 15:35:56', N'2019-03-12 15:37:10'),
(N'4', N'1', N'系统角色', N'0', N'系统角色', NULL, N'两人', N'角色管理、为用户和用户组分配角色、设置角色可访问菜单、授予权限', N'4', N'0', N'2019-11-08 11:47:21', N'2019-11-08 11:48:22'),
(N'10', N'1', N'基础选项', N'0', N'基础选项', NULL, N'修理', N'按照分组管理的选项列表，如民族、学历等静态列表', N'10', N'0', N'2019-11-08 11:49:40', N'2019-11-08 11:49:46'),
(N'8', N'1', N'流程设计', N'0', N'流程设计', NULL, N'双绞线', N'流程模板设计及流程实例查询', N'8', N'0', N'2020-11-02 16:21:19', N'2020-11-02 16:21:19')
GO


-- ----------------------------
-- Table structure for cm_option
-- ----------------------------
CREATE TABLE [dbo].[cm_option] (
  [id] bigint NOT NULL,
  [name] nvarchar(64) NOT NULL,
  [dispidx] int NOT NULL,
  [group_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_option',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'选项名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_option',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'显示顺序',
'SCHEMA', N'dbo',
'TABLE', N'cm_option',
'COLUMN', N'dispidx'
GO

EXEC sp_addextendedproperty
'MS_Description', N'所属分组',
'SCHEMA', N'dbo',
'TABLE', N'cm_option',
'COLUMN', N'group_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'基础选项',
'SCHEMA', N'dbo',
'TABLE', N'cm_option'
GO


-- ----------------------------
-- Records of cm_option
-- ----------------------------
INSERT INTO [dbo].[cm_option] VALUES (N'2', N'汉族', N'2', N'1'), (N'3', N'蒙古族', N'3', N'1'), (N'4', N'回族', N'4', N'1'), (N'5', N'藏族', N'5', N'1'), (N'6', N'维吾尔族', N'6', N'1'), (N'7', N'苗族', N'7', N'1'), (N'8', N'彝族', N'8', N'1'), (N'9', N'壮族', N'9', N'1'), (N'10', N'布依族', N'10', N'1'), (N'11', N'朝鲜族', N'11', N'1'), (N'12', N'满族', N'12', N'1'), (N'13', N'侗族', N'13', N'1'), (N'14', N'瑶族', N'14', N'1'), (N'15', N'白族', N'15', N'1'), (N'16', N'土家族', N'16', N'1'), (N'17', N'哈尼族', N'17', N'1'), (N'18', N'哈萨克族', N'18', N'1'), (N'19', N'傣族', N'19', N'1'), (N'20', N'黎族', N'20', N'1'), (N'21', N'傈僳族', N'21', N'1'), (N'22', N'佤族', N'22', N'1'), (N'23', N'畲族', N'23', N'1'), (N'24', N'高山族', N'24', N'1'), (N'25', N'拉祜族', N'25', N'1'), (N'26', N'水族', N'26', N'1'), (N'27', N'东乡族', N'27', N'1'), (N'28', N'纳西族', N'28', N'1'), (N'29', N'景颇族', N'29', N'1'), (N'30', N'柯尔克孜族', N'30', N'1'), (N'31', N'土族', N'31', N'1'), (N'32', N'达斡尔族', N'32', N'1'), (N'33', N'仫佬族', N'33', N'1'), (N'34', N'羌族', N'34', N'1'), (N'35', N'布朗族', N'35', N'1'), (N'36', N'撒拉族', N'36', N'1'), (N'37', N'毛难族', N'37', N'1'), (N'38', N'仡佬族', N'38', N'1'), (N'39', N'锡伯族', N'39', N'1'), (N'40', N'阿昌族', N'40', N'1'), (N'41', N'普米族', N'41', N'1'), (N'42', N'塔吉克族', N'42', N'1'), (N'43', N'怒族', N'43', N'1'), (N'44', N'乌孜别克族', N'44', N'1'), (N'45', N'俄罗斯族', N'45', N'1'), (N'46', N'鄂温克族', N'46', N'1'), (N'47', N'德昂族', N'47', N'1'), (N'48', N'保安族', N'48', N'1'), (N'49', N'裕固族', N'49', N'1'), (N'50', N'京族', N'50', N'1'), (N'51', N'塔塔尔族', N'51', N'1'), (N'52', N'独龙族', N'52', N'1'), (N'53', N'鄂伦春族', N'53', N'1'), (N'54', N'赫哲族', N'54', N'1'), (N'55', N'门巴族', N'55', N'1'), (N'56', N'珞巴族', N'56', N'1'), (N'57', N'基诺族', N'57', N'1'), (N'58', N'大学', N'58', N'2'), (N'59', N'高中', N'59', N'2'), (N'60', N'中学', N'60', N'2'), (N'61', N'小学', N'61', N'2'), (N'62', N'硕士', N'62', N'2'), (N'63', N'博士', N'63', N'2'), (N'64', N'其他', N'64', N'2')
GO

INSERT INTO [dbo].[cm_option] VALUES (N'342', N'男', N'342', N'4'), (N'343', N'女', N'343', N'4'), (N'344', N'未知', N'344', N'4'), (N'345', N'不明', N'345', N'4'), (N'346', N'string', N'346', N'5'), (N'347', N'int', N'347', N'5'), (N'348', N'double', N'348', N'5'), (N'349', N'DateTime', N'349', N'5'), (N'350', N'Date', N'350', N'5'), (N'351', N'bool', N'351', N'5')
GO


-- ----------------------------
-- Table structure for cm_option_group
-- ----------------------------
CREATE TABLE [dbo].[cm_option_group] (
  [id] bigint NOT NULL,
  [name] nvarchar(255) NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_option_group',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'分组名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_option_group',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'基础选项分组',
'SCHEMA', N'dbo',
'TABLE', N'cm_option_group'
GO


-- ----------------------------
-- Records of cm_option_group
-- ----------------------------
INSERT INTO [dbo].[cm_option_group] VALUES (N'1', N'民族'), (N'2', N'学历'), (N'3', N'地区'), (N'4', N'性别'), (N'5', N'数据类型')
GO


-- ----------------------------
-- Table structure for cm_params
-- ----------------------------
CREATE TABLE [dbo].[cm_params] (
  [id] bigint NOT NULL,
  [name] nvarchar(255) NOT NULL,
  [value] nvarchar(255) NULL,
  [note] nvarchar(255) NULL,
  [ctime] datetime NOT NULL,
  [mtime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户参数标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_params',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'参数名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_params',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'参数缺省值',
'SCHEMA', N'dbo',
'TABLE', N'cm_params',
'COLUMN', N'value'
GO

EXEC sp_addextendedproperty
'MS_Description', N'参数描述',
'SCHEMA', N'dbo',
'TABLE', N'cm_params',
'COLUMN', N'note'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_params',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'修改时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_params',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户参数定义',
'SCHEMA', N'dbo',
'TABLE', N'cm_params'
GO


-- ----------------------------
-- Records of cm_params
-- ----------------------------
INSERT INTO [dbo].[cm_params] VALUES (N'1', N'接收新任务', N'true', N'', N'2020-12-01 15:13:49', N'2020-12-02 09:23:53'), (N'2', N'接收新发布通知', N'true', N'', N'2020-12-02 09:25:15', N'2020-12-02 09:25:15'), (N'3', N'接收新消息', N'true', N'接收通讯录消息推送', N'2020-12-02 09:24:28', N'2020-12-02 09:24:28')
GO


-- ----------------------------
-- Table structure for cm_permission_module
-- ----------------------------
CREATE TABLE [dbo].[cm_permission_module] (
  [id] bigint NOT NULL,
  [name] nvarchar(64) NOT NULL,
  [note] nvarchar(255) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'模块标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission_module',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'模块名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission_module',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'模块描述',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission_module',
'COLUMN', N'note'
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限所属模块',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission_module'
GO


-- ----------------------------
-- Records of cm_permission_module
-- ----------------------------
INSERT INTO [dbo].[cm_permission_module] VALUES (N'1', N'系统预留', N'系统内部使用的权限控制，禁止删除'), (N'87433840629673984', N'物资管理', NULL)
GO


-- ----------------------------
-- Table structure for cm_permission_func
-- ----------------------------
CREATE TABLE [dbo].[cm_permission_func] (
  [id] bigint NOT NULL,
  [module_id] bigint NOT NULL,
  [name] nvarchar(64) NOT NULL,
  [note] nvarchar(255) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'所属模块',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission_func',
'COLUMN', N'module_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'功能名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission_func',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'功能描述',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission_func',
'COLUMN', N'note'
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限所属功能',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission_func'
GO


-- ----------------------------
-- Records of cm_permission_func
-- ----------------------------
INSERT INTO [dbo].[cm_permission_func] VALUES (N'1', N'1', N'文件管理', N'管理文件的上传、删除等'), (N'87433900117487616', N'87433840629673984', N'入出', NULL)
GO


-- ----------------------------
-- Table structure for cm_permission
-- ----------------------------
CREATE TABLE [dbo].[cm_permission] (
  [id] bigint NOT NULL,
  [func_id] bigint NOT NULL,
  [name] nvarchar(64) NOT NULL,
  [note] nvarchar(255) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'所属功能',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission',
'COLUMN', N'func_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限描述',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission',
'COLUMN', N'note'
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission'
GO


-- ----------------------------
-- Records of cm_permission
-- ----------------------------
INSERT INTO [dbo].[cm_permission] VALUES (N'1', N'1', N'公共文件增删', N'公共文件的上传、删除等'), (N'2', N'1', N'素材库增删', N'素材库目录的上传、删除等'), (N'87434002596917248', N'87433900117487616', N'冲销', NULL)
GO


-- ----------------------------
-- Table structure for cm_role
-- ----------------------------
CREATE TABLE [dbo].[cm_role] (
  [id] bigint NOT NULL,
  [name] nvarchar(32) NOT NULL,
  [note] nvarchar(255) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_role',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_role',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色描述',
'SCHEMA', N'dbo',
'TABLE', N'cm_role',
'COLUMN', N'note'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色',
'SCHEMA', N'dbo',
'TABLE', N'cm_role'
GO


-- ----------------------------
-- Records of cm_role
-- ----------------------------
INSERT INTO [dbo].[cm_role] VALUES (N'1', N'任何人', N'所有用户默认都具有该角色，不可删除'), (N'2', N'系统管理员', N'系统角色，不可删除')
GO


-- ----------------------------
-- Table structure for cm_role_menu
-- ----------------------------
CREATE TABLE [dbo].[cm_role_menu] (
  [role_id] bigint NOT NULL,
  [menu_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_role_menu',
'COLUMN', N'role_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'菜单标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_role_menu',
'COLUMN', N'menu_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色一菜单多对多',
'SCHEMA', N'dbo',
'TABLE', N'cm_role_menu'
GO


-- ----------------------------
-- Records of cm_role_menu
-- ----------------------------
INSERT INTO [dbo].[cm_role_menu] VALUES (N'2', N'2'), (N'2', N'3'), (N'2', N'4'), (N'2', N'5'), (N'2', N'6'), (N'2', N'7'), (N'2', N'8'), (N'2', N'9'), (N'2', N'10')
GO


-- ----------------------------
-- Table structure for cm_role_per
-- ----------------------------
CREATE TABLE [dbo].[cm_role_per] (
  [role_id] bigint NOT NULL,
  [per_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_role_per',
'COLUMN', N'role_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_role_per',
'COLUMN', N'per_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色一权限多对多',
'SCHEMA', N'dbo',
'TABLE', N'cm_role_per'
GO


-- ----------------------------
-- Records of cm_role_per
-- ----------------------------
INSERT INTO [dbo].[cm_role_per] VALUES (N'2', N'1'), (N'2', N'2')
GO


-- ----------------------------
-- Table structure for cm_rpt
-- ----------------------------
CREATE TABLE [dbo].[cm_rpt] (
  [id] bigint NOT NULL,
  [name] nvarchar(64) NOT NULL,
  [define] nvarchar(max) NULL,
  [note] nvarchar(255) NULL,
  [ctime] datetime NOT NULL,
  [mtime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'报表标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_rpt',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'报表名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_rpt',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'报表模板定义',
'SCHEMA', N'dbo',
'TABLE', N'cm_rpt',
'COLUMN', N'define'
GO

EXEC sp_addextendedproperty
'MS_Description', N'报表描述',
'SCHEMA', N'dbo',
'TABLE', N'cm_rpt',
'COLUMN', N'note'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_rpt',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'修改时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_rpt',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'报表模板定义',
'SCHEMA', N'dbo',
'TABLE', N'cm_rpt'
GO



-- ----------------------------
-- Table structure for cm_user
-- ----------------------------
CREATE TABLE [dbo].[cm_user] (
  [id] bigint NOT NULL,
  [acc] nvarchar(32) NULL,
  [phone] nvarchar(16) NULL,
  [pwd] char(32) NOT NULL,
  [name] nvarchar(32) NULL,
  [photo] nvarchar(255) NULL,
  [expired] bit NOT NULL,
  [ctime] datetime NOT NULL,
  [mtime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'手机号，唯一',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'phone'
GO

EXEC sp_addextendedproperty
'MS_Description', N'账号，唯一',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'acc'
GO

EXEC sp_addextendedproperty
'MS_Description', N'密码的md5',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'pwd'
GO

EXEC sp_addextendedproperty
'MS_Description', N'姓名',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'头像',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'photo'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否停用',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'expired'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'修改时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'系统用户',
'SCHEMA', N'dbo',
'TABLE', N'cm_user'
GO


-- ----------------------------
-- Records of cm_user
-- ----------------------------
INSERT INTO [dbo].[cm_user] VALUES (1, N'admin', N'13511111111', 'b59c67bf196a4758191e42f76670ceba', N'', N'', '0', '2019-10-24 09:06:38.000', '2023-03-16 08:35:39.000'); GO



-- ----------------------------
-- Table structure for cm_user_group
-- ----------------------------
CREATE TABLE [dbo].[cm_user_group] (
  [user_id] bigint NOT NULL,
  [group_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_group',
'COLUMN', N'user_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'组标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_group',
'COLUMN', N'group_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户一组多对多',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_group'
GO


-- ----------------------------
-- Table structure for cm_user_params
-- ----------------------------
CREATE TABLE [dbo].[cm_user_params] (
  [user_id] bigint NOT NULL,
  [param_id] bigint NOT NULL,
  [value] nvarchar(255) NULL,
  [mtime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_params',
'COLUMN', N'user_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'参数标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_params',
'COLUMN', N'param_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'参数值',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_params',
'COLUMN', N'value'
GO

EXEC sp_addextendedproperty
'MS_Description', N'修改时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_params',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户参数值',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_params'
GO


-- ----------------------------
-- Table structure for cm_user_role
-- ----------------------------
CREATE TABLE [dbo].[cm_user_role] (
  [user_id] bigint NOT NULL,
  [role_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_role',
'COLUMN', N'user_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_role',
'COLUMN', N'role_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户一角色多对多',
'SCHEMA', N'dbo',
'TABLE', N'cm_user_role'
GO


-- ----------------------------
-- Records of cm_user_role
-- ----------------------------
INSERT INTO [dbo].[cm_user_role] VALUES (N'1', N'2')
GO


-- ----------------------------
-- Table structure for cm_wfd_atv
-- ----------------------------
CREATE TABLE [dbo].[cm_wfd_atv] (
  [id] bigint NOT NULL,
  [prc_id] bigint NOT NULL,
  [name] nvarchar(64) NOT NULL,
  [type] tinyint NOT NULL,
  [exec_scope] tinyint NOT NULL,
  [exec_limit] tinyint NOT NULL,
  [exec_atv_id] bigint NULL,
  [auto_accept] bit NOT NULL,
  [can_delete] bit NOT NULL,
  [can_terminate] bit NOT NULL,
  [can_jump_into] bit NOT NULL,
  [trans_kind] tinyint NOT NULL,
  [join_kind] tinyint NOT NULL,
  [ctime] datetime NOT NULL,
  [mtime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'活动标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'流程标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'prc_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'活动名称，同时作为状态名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#WfdAtvType#活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'type'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#WfdAtvExecScope#执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'exec_scope'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#WfdAtvExecLimit#执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'exec_limit'
GO

EXEC sp_addextendedproperty
'MS_Description', N'在执行者限制为3或4时选择的活动',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'exec_atv_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否自动签收，打开工作流视图时自动签收工作项',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'auto_accept'
GO

EXEC sp_addextendedproperty
'MS_Description', N'能否删除流程实例和业务数据，0否 1',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'can_delete'
GO

EXEC sp_addextendedproperty
'MS_Description', N'能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'can_terminate'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否可作为跳转目标，0不可跳转 1可以',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'can_jump_into'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#WfdAtvTransKind#当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'trans_kind'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#WfdAtvJoinKind#同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'join_kind'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'修改时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'活动模板',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv'
GO



-- ----------------------------
-- Table structure for cm_wfd_atv_role
-- ----------------------------
CREATE TABLE [dbo].[cm_wfd_atv_role] (
  [atv_id] bigint NOT NULL,
  [role_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'活动标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv_role',
'COLUMN', N'atv_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv_role',
'COLUMN', N'role_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'活动授权',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_atv_role'
GO



-- ----------------------------
-- Table structure for cm_wfd_prc
-- ----------------------------
CREATE TABLE [dbo].[cm_wfd_prc] (
  [id] bigint NOT NULL,
  [name] nvarchar(64) NOT NULL,
  [diagram] nvarchar(max) NULL,
  [is_locked] bit NOT NULL,
  [singleton] bit NOT NULL,
  [note] nvarchar(255) NULL,
  [dispidx] int NOT NULL,
  [ctime] datetime NOT NULL,
  [mtime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'流程标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_prc',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'流程名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_prc',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'流程图',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_prc',
'COLUMN', N'diagram'
GO

EXEC sp_addextendedproperty
'MS_Description', N'锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_prc',
'COLUMN', N'is_locked'
GO

EXEC sp_addextendedproperty
'MS_Description', N'同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_prc',
'COLUMN', N'singleton'
GO

EXEC sp_addextendedproperty
'MS_Description', N'描述',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_prc',
'COLUMN', N'note'
GO

EXEC sp_addextendedproperty
'MS_Description', N'显示顺序',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_prc',
'COLUMN', N'dispidx'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_prc',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'最后修改时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_prc',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'流程模板',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_prc'
GO




-- ----------------------------
-- Table structure for cm_wfd_trs
-- ----------------------------
CREATE TABLE [dbo].[cm_wfd_trs] (
  [id] bigint NOT NULL,
  [prc_id] bigint NOT NULL,
  [src_atv_id] bigint NOT NULL,
  [tgt_atv_id] bigint NOT NULL,
  [is_rollback] bit NOT NULL,
  [trs_id] bigint NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'迁移标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_trs',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'流程模板标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_trs',
'COLUMN', N'prc_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'起始活动模板标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_trs',
'COLUMN', N'src_atv_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'目标活动模板标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_trs',
'COLUMN', N'tgt_atv_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否为回退迁移',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_trs',
'COLUMN', N'is_rollback'
GO

EXEC sp_addextendedproperty
'MS_Description', N'类别为回退迁移时对应的常规迁移标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_trs',
'COLUMN', N'trs_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'迁移模板',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfd_trs'
GO



-- ----------------------------
-- Table structure for cm_wfi_atv
-- ----------------------------
CREATE TABLE [dbo].[cm_wfi_atv] (
  [id] bigint NOT NULL,
  [prci_id] bigint NOT NULL,
  [atvd_id] bigint NOT NULL,
  [status] tinyint NOT NULL,
  [inst_count] int NOT NULL,
  [ctime] datetime NOT NULL,
  [mtime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'活动实例标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_atv',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'流程实例标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_atv',
'COLUMN', N'prci_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'活动模板标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_atv',
'COLUMN', N'atvd_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#WfiAtvStatus#活动实例的状态 0活动 1结束 2终止 3同步活动',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_atv',
'COLUMN', N'status'
GO

EXEC sp_addextendedproperty
'MS_Description', N'活动实例在流程实例被实例化的次数',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_atv',
'COLUMN', N'inst_count'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_atv',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'最后一次状态改变的时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_atv',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'活动实例',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_atv'
GO




-- ----------------------------
-- Table structure for cm_wfi_item
-- ----------------------------
CREATE TABLE [dbo].[cm_wfi_item] (
  [id] bigint NOT NULL,
  [atvi_id] bigint NOT NULL,
  [status] tinyint NOT NULL,
  [assign_kind] tinyint NOT NULL,
  [sender_id] bigint NULL,
  [sender] nvarchar(32) NULL,
  [stime] datetime NOT NULL,
  [is_accept] bit NOT NULL,
  [accept_time] datetime NULL,
  [role_id] bigint NULL,
  [user_id] bigint NULL,
  [note] nvarchar(255) NULL,
  [dispidx] int NOT NULL,
  [ctime] datetime NOT NULL,
  [mtime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'工作项标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'活动实例标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'atvi_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#WfiItemStatus#工作项状态 0活动 1结束 2终止 3同步活动',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'status'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#WfiItemAssignKind#指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'assign_kind'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发送者标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'sender_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发送者姓名',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'sender'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发送时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'stime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否签收此项任务',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'is_accept'
GO

EXEC sp_addextendedproperty
'MS_Description', N'签收时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'accept_time'
GO

EXEC sp_addextendedproperty
'MS_Description', N'执行者角色标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'role_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'执行者用户标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'user_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'工作项备注',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'note'
GO

EXEC sp_addextendedproperty
'MS_Description', N'显示顺序',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'dispidx'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'最后一次状态改变的时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'工作项',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_item'
GO



-- ----------------------------
-- Table structure for cm_wfi_prc
-- ----------------------------
CREATE TABLE [dbo].[cm_wfi_prc] (
  [id] bigint NOT NULL,
  [prcd_id] bigint NOT NULL,
  [name] nvarchar(255) NOT NULL,
  [status] tinyint NOT NULL,
  [dispidx] int NOT NULL,
  [ctime] datetime NOT NULL,
  [mtime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'流程实例标识，同时为业务数据主键',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_prc',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'流程模板标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_prc',
'COLUMN', N'prcd_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'流转单名称',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_prc',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#WfiPrcStatus#流程实例状态 0活动 1结束 2终止',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_prc',
'COLUMN', N'status'
GO

EXEC sp_addextendedproperty
'MS_Description', N'显示顺序',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_prc',
'COLUMN', N'dispidx'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_prc',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'最后一次状态改变的时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_prc',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'流程实例',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_prc'
GO



-- ----------------------------
-- Table structure for cm_wfi_trs
-- ----------------------------
CREATE TABLE [dbo].[cm_wfi_trs] (
  [id] bigint NOT NULL,
  [trsd_id] bigint NOT NULL,
  [src_atvi_id] bigint NOT NULL,
  [tgt_atvi_id] bigint NOT NULL,
  [is_rollback] bit NOT NULL,
  [ctime] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'迁移实例标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_trs',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'迁移模板标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_trs',
'COLUMN', N'trsd_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'起始活动实例标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_trs',
'COLUMN', N'src_atvi_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'目标活动实例标识',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_trs',
'COLUMN', N'tgt_atvi_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否为回退迁移，1表回退',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_trs',
'COLUMN', N'is_rollback'
GO

EXEC sp_addextendedproperty
'MS_Description', N'迁移时间',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_trs',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'迁移实例',
'SCHEMA', N'dbo',
'TABLE', N'cm_wfi_trs'
GO


-- ----------------------------
-- Table structure for fsm_file
-- ----------------------------
CREATE TABLE [dbo].[fsm_file] (
  [id] bigint NOT NULL,
  [name] nvarchar(512) NOT NULL,
  [path] nvarchar(512) NOT NULL,
  [size] bigint NOT NULL,
  [info] nvarchar(512) NULL,
  [uploader] bigint NOT NULL,
  [ctime] datetime NOT NULL,
  [downloads] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'文件标识',
'SCHEMA', N'dbo',
'TABLE', N'fsm_file',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'文件名称',
'SCHEMA', N'dbo',
'TABLE', N'fsm_file',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'存放路径：卷/两级目录/id.ext',
'SCHEMA', N'dbo',
'TABLE', N'fsm_file',
'COLUMN', N'path'
GO

EXEC sp_addextendedproperty
'MS_Description', N'文件长度',
'SCHEMA', N'dbo',
'TABLE', N'fsm_file',
'COLUMN', N'size'
GO

EXEC sp_addextendedproperty
'MS_Description', N'文件描述',
'SCHEMA', N'dbo',
'TABLE', N'fsm_file',
'COLUMN', N'info'
GO

EXEC sp_addextendedproperty
'MS_Description', N'上传人id',
'SCHEMA', N'dbo',
'TABLE', N'fsm_file',
'COLUMN', N'uploader'
GO

EXEC sp_addextendedproperty
'MS_Description', N'上传时间',
'SCHEMA', N'dbo',
'TABLE', N'fsm_file',
'COLUMN', N'ctime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'下载次数',
'SCHEMA', N'dbo',
'TABLE', N'fsm_file',
'COLUMN', N'downloads'
GO


-- ----------------------------
-- Table structure for crud_大儿
-- ----------------------------
CREATE TABLE [dbo].[crud_大儿] (
  [id] bigint NOT NULL,
  [parent_id] bigint NOT NULL,
  [大儿名] nvarchar(255) NULL
)
GO


-- ----------------------------
-- Records of crud_大儿
-- ----------------------------

-- ----------------------------
-- Table structure for crud_父表
-- ----------------------------
CREATE TABLE [dbo].[crud_父表] (
  [id] bigint NOT NULL,
  [父名] nvarchar(255) NULL
)
GO


-- ----------------------------
-- Records of crud_父表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_缓存表
-- ----------------------------
CREATE TABLE [dbo].[crud_缓存表] (
  [id] bigint NOT NULL,
  [手机号] char(11) NULL,
  [姓名] nvarchar(32) NULL
)
GO


-- ----------------------------
-- Records of crud_缓存表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_基础
-- ----------------------------
CREATE TABLE [dbo].[crud_基础] (
  [id] bigint NOT NULL,
  [parent_id] bigint NULL,
  [序列] int NOT NULL,
  [名称] nvarchar(255) NULL,
  [限长4] nvarchar(16) NULL,
  [不重复] nvarchar(64) NULL,
  [禁止选中] bit NOT NULL,
  [禁止保存] bit NOT NULL,
  [禁止删除] bit NOT NULL,
  [值变事件] nvarchar(64) NULL,
  [发布插入事件] bit NOT NULL,
  [发布删除事件] bit NOT NULL,
  [创建时间] datetime NOT NULL,
  [修改时间] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'标识',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'上级id，演示树状结构',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'parent_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'序列自动赋值',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'序列'
GO

EXEC sp_addextendedproperty
'MS_Description', N'限制最大长度4',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'限长4'
GO

EXEC sp_addextendedproperty
'MS_Description', N'列值无重复',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'不重复'
GO

EXEC sp_addextendedproperty
'MS_Description', N'始终为false',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'禁止选中'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时保存前校验不通过',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'禁止保存'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时删除前校验不通过',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'禁止删除'
GO

EXEC sp_addextendedproperty
'MS_Description', N'每次值变化时触发领域事件',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'值变事件'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时允许发布插入事件',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'发布插入事件'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时允许发布删除事件',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'发布删除事件'
GO

EXEC sp_addextendedproperty
'MS_Description', N'初次创建时间',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'创建时间'
GO

EXEC sp_addextendedproperty
'MS_Description', N'最后修改时间',
'SCHEMA', N'dbo',
'TABLE', N'crud_基础',
'COLUMN', N'修改时间'
GO


-- ----------------------------
-- Records of crud_基础
-- ----------------------------

-- ----------------------------
-- Table structure for crud_角色
-- ----------------------------
CREATE TABLE [dbo].[crud_角色] (
  [id] bigint NOT NULL,
  [角色名称] nvarchar(32) NULL,
  [角色描述] nvarchar(255) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'crud_角色',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色名称',
'SCHEMA', N'dbo',
'TABLE', N'crud_角色',
'COLUMN', N'角色名称'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色描述',
'SCHEMA', N'dbo',
'TABLE', N'crud_角色',
'COLUMN', N'角色描述'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色',
'SCHEMA', N'dbo',
'TABLE', N'crud_角色'
GO


-- ----------------------------
-- Records of crud_角色
-- ----------------------------

-- ----------------------------
-- Table structure for crud_角色权限
-- ----------------------------
CREATE TABLE [dbo].[crud_角色权限] (
  [role_id] bigint NOT NULL,
  [prv_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'crud_角色权限',
'COLUMN', N'role_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限标识',
'SCHEMA', N'dbo',
'TABLE', N'crud_角色权限',
'COLUMN', N'prv_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色关联的权限',
'SCHEMA', N'dbo',
'TABLE', N'crud_角色权限'
GO


-- ----------------------------
-- Records of crud_角色权限
-- ----------------------------

-- ----------------------------
-- Table structure for crud_扩展1
-- ----------------------------
CREATE TABLE [dbo].[crud_扩展1] (
  [id] bigint NOT NULL,
  [扩展1名称] nvarchar(255) NULL,
  [禁止选中] bit NOT NULL,
  [禁止保存] bit NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'标识',
'SCHEMA', N'dbo',
'TABLE', N'crud_扩展1',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'始终为false',
'SCHEMA', N'dbo',
'TABLE', N'crud_扩展1',
'COLUMN', N'禁止选中'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时保存前校验不通过',
'SCHEMA', N'dbo',
'TABLE', N'crud_扩展1',
'COLUMN', N'禁止保存'
GO


-- ----------------------------
-- Records of crud_扩展1
-- ----------------------------

-- ----------------------------
-- Table structure for crud_扩展2
-- ----------------------------
CREATE TABLE [dbo].[crud_扩展2] (
  [id] bigint NOT NULL,
  [扩展2名称] nvarchar(255) NULL,
  [禁止删除] bit NOT NULL,
  [值变事件] nvarchar(255) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'标识',
'SCHEMA', N'dbo',
'TABLE', N'crud_扩展2',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时删除前校验不通过',
'SCHEMA', N'dbo',
'TABLE', N'crud_扩展2',
'COLUMN', N'禁止删除'
GO

EXEC sp_addextendedproperty
'MS_Description', N'每次值变化时触发领域事件',
'SCHEMA', N'dbo',
'TABLE', N'crud_扩展2',
'COLUMN', N'值变事件'
GO


-- ----------------------------
-- Records of crud_扩展2
-- ----------------------------

-- ----------------------------
-- Table structure for crud_权限
-- ----------------------------
CREATE TABLE [dbo].[crud_权限] (
  [id] bigint NOT NULL,
  [权限名称] nvarchar(255) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限名称',
'SCHEMA', N'dbo',
'TABLE', N'crud_权限',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限',
'SCHEMA', N'dbo',
'TABLE', N'crud_权限'
GO


-- ----------------------------
-- Records of crud_权限
-- ----------------------------

-- ----------------------------
-- Table structure for crud_小儿
-- ----------------------------
CREATE TABLE [dbo].[crud_小儿] (
  [id] bigint NOT NULL,
  [group_id] bigint NOT NULL,
  [小儿名] nvarchar(255) NULL
)
GO


-- ----------------------------
-- Records of crud_小儿
-- ----------------------------

-- ----------------------------
-- Table structure for crud_用户
-- ----------------------------
CREATE TABLE [dbo].[crud_用户] (
  [id] bigint NOT NULL,
  [手机号] char(11) NULL,
  [姓名] nvarchar(32) NULL,
  [密码] char(32) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户标识',
'SCHEMA', N'dbo',
'TABLE', N'crud_用户',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'手机号，唯一',
'SCHEMA', N'dbo',
'TABLE', N'crud_用户',
'COLUMN', N'手机号'
GO

EXEC sp_addextendedproperty
'MS_Description', N'姓名',
'SCHEMA', N'dbo',
'TABLE', N'crud_用户',
'COLUMN', N'姓名'
GO

EXEC sp_addextendedproperty
'MS_Description', N'密码的md5',
'SCHEMA', N'dbo',
'TABLE', N'crud_用户',
'COLUMN', N'密码'
GO

EXEC sp_addextendedproperty
'MS_Description', N'系统用户',
'SCHEMA', N'dbo',
'TABLE', N'crud_用户'
GO


-- ----------------------------
-- Records of crud_用户
-- ----------------------------

-- ----------------------------
-- Table structure for crud_用户角色
-- ----------------------------
CREATE TABLE [dbo].[crud_用户角色] (
  [user_id] bigint NOT NULL,
  [role_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户标识',
'SCHEMA', N'dbo',
'TABLE', N'crud_用户角色',
'COLUMN', N'user_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'crud_用户角色',
'COLUMN', N'role_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户关联的角色',
'SCHEMA', N'dbo',
'TABLE', N'crud_用户角色'
GO


-- ----------------------------
-- Records of crud_用户角色
-- ----------------------------

-- ----------------------------
-- Table structure for crud_主表
-- ----------------------------
CREATE TABLE [dbo].[crud_主表] (
  [id] bigint NOT NULL,
  [主表名称] nvarchar(255) NULL,
  [限长4] nvarchar(16) NULL,
  [不重复] nvarchar(255) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'限制最大长度4',
'SCHEMA', N'dbo',
'TABLE', N'crud_主表',
'COLUMN', N'限长4'
GO

EXEC sp_addextendedproperty
'MS_Description', N'列值无重复',
'SCHEMA', N'dbo',
'TABLE', N'crud_主表',
'COLUMN', N'不重复'
GO


-- ----------------------------
-- Records of crud_主表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_字段类型
-- ----------------------------
CREATE TABLE [dbo].[crud_字段类型] (
  [id] bigint NOT NULL,
  [字符串] nvarchar(255) NULL,
  [整型] int NOT NULL,
  [可空整型] int NULL,
  [长整型] bigint NULL,
  [布尔] bit NOT NULL,
  [可空布尔] bit NULL,
  [日期时间] datetime NOT NULL,
  [可空时间] datetime NULL,
  [枚举] smallint NOT NULL,
  [可空枚举] smallint NULL,
  [单精度] real NOT NULL,
  [可空单精度] real NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'#Gender#性别',
'SCHEMA', N'dbo',
'TABLE', N'crud_字段类型',
'COLUMN', N'枚举'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#Gender#性别',
'SCHEMA', N'dbo',
'TABLE', N'crud_字段类型',
'COLUMN', N'可空枚举'
GO


-- ----------------------------
-- Records of crud_字段类型
-- ----------------------------

-- ----------------------------
-- Table structure for 部门
-- ----------------------------
CREATE TABLE [dbo].[部门] (
  [id] bigint NOT NULL,
  [上级id] bigint NULL,
  [编码] nvarchar(16) NULL,
  [名称] nvarchar(32) NULL,
  [说明] nvarchar(64) NULL,
  [建档时间] datetime NULL,
  [撤档时间] datetime NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'位置、环境、备注等',
'SCHEMA', N'dbo',
'TABLE', N'部门',
'COLUMN', N'说明'
GO


-- ----------------------------
-- Records of 部门
-- ----------------------------
INSERT INTO [dbo].[部门] VALUES (N'93173171340210176', NULL, N'01', N'设备科', NULL, N'2024-06-14 10:37:22', NULL), (N'93173345370271744', NULL, N'02', N'物资科', NULL, N'2024-06-14 10:37:41', NULL), (N'93174118862843904', NULL, N'03', N'财务科', NULL, N'2024-06-14 10:40:52', NULL)
GO


-- ----------------------------
-- Table structure for 部门人员
-- ----------------------------
CREATE TABLE [dbo].[部门人员] (
  [部门id] bigint NOT NULL,
  [人员id] bigint NOT NULL,
  [缺省] bit NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'当一个人员属于多个部门时，当前是否为缺省',
'SCHEMA', N'dbo',
'TABLE', N'部门人员',
'COLUMN', N'缺省'
GO


-- ----------------------------
-- Records of 部门人员
-- ----------------------------
INSERT INTO [dbo].[部门人员] VALUES (N'93173345370271744', N'93233663974862848', N'1'), (N'93173345370271744', N'100436029211963392', N'1')
GO


-- ----------------------------
-- Table structure for 供应商
-- ----------------------------
CREATE TABLE [dbo].[供应商] (
  [id] bigint NOT NULL,
  [名称] nvarchar(64) NULL,
  [执照号] nvarchar(32) NULL,
  [执照效期] datetime NULL,
  [税务登记号] nvarchar(32) NULL,
  [地址] nvarchar(128) NULL,
  [电话] nvarchar(16) NULL,
  [开户银行] nvarchar(64) NULL,
  [帐号] nvarchar(32) NULL,
  [联系人] nvarchar(32) NULL,
  [建档时间] datetime NULL,
  [撤档时间] datetime NULL,
  [备注] nvarchar(255) NULL
)
GO


-- ----------------------------
-- Records of 供应商
-- ----------------------------
INSERT INTO [dbo].[供应商] VALUES (N'95034724012290048', N'物资东厂', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'2024-06-19 13:54:37', NULL, NULL), (N'95034312534290432', N'仁和制药', NULL, NULL, NULL, NULL, N'13698562456', NULL, NULL, NULL, N'2024-06-19 13:53:52', NULL, NULL)
GO


-- ----------------------------
-- Table structure for 人员
-- ----------------------------
CREATE TABLE [dbo].[人员] (
  [id] bigint NOT NULL,
  [姓名] nvarchar(32) NULL,
  [出生日期] datetime NULL,
  [性别] smallint NULL,
  [工作日期] datetime NULL,
  [办公室电话] nvarchar(32) NULL,
  [电子邮件] nvarchar(32) NULL,
  [建档时间] datetime NULL,
  [撤档时间] datetime NULL,
  [撤档原因] nvarchar(64) NULL,
  [user_id] bigint NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'#Gender#',
'SCHEMA', N'dbo',
'TABLE', N'人员',
'COLUMN', N'性别'
GO

EXEC sp_addextendedproperty
'MS_Description', N'账号ID',
'SCHEMA', N'dbo',
'TABLE', N'人员',
'COLUMN', N'user_id'
GO


-- ----------------------------
-- Records of 人员
-- ----------------------------
INSERT INTO [dbo].[人员] VALUES (N'93233663974862848', N'王库管', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'87374677803298816'), (N'93233694710722560', N'张主管', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'87375101197316096'), (N'100436029211963392', N'测试员', NULL, NULL, NULL, NULL, NULL, N'2024-07-04 11:37:09', NULL, NULL, N'1')
GO


-- ----------------------------
-- Table structure for 物资分类
-- ----------------------------
CREATE TABLE [dbo].[物资分类] (
  [id] bigint NOT NULL,
  [名称] nvarchar(64) NOT NULL
)
GO


-- ----------------------------
-- Records of 物资分类
-- ----------------------------
INSERT INTO [dbo].[物资分类] VALUES (N'95413444640272384', N'电工材料'), (N'95419313314623488', N'劳保材料'), (N'95419350320967680', N'水暖材料'), (N'95419395929829376', N'维修材料'), (N'95419431795322880', N'办公材料'), (N'95419477521625088', N'低值易耗'), (N'95419514808987648', N'易耗材料'), (N'95419598749593600', N'其他材料')
GO


-- ----------------------------
-- Table structure for 物资计划
-- ----------------------------
CREATE TABLE [dbo].[物资计划] (
  [id] bigint NOT NULL,
  [部门id] bigint NULL,
  [no] nvarchar(8) NULL,
  [计划类型] smallint NULL,
  [编制方法] smallint NULL,
  [摘要] nvarchar(64) NULL,
  [编制人] nvarchar(32) NULL,
  [编制日期] datetime NULL,
  [审核人] nvarchar(32) NULL,
  [审核日期] datetime NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'#计划类型#月;季;年',
'SCHEMA', N'dbo',
'TABLE', N'物资计划',
'COLUMN', N'计划类型'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#计划编制方法#1-往年同期线性参照法,2-临近期间平均参照法,3-物资储备定额参照法,4-由部门申购计划产生',
'SCHEMA', N'dbo',
'TABLE', N'物资计划',
'COLUMN', N'编制方法'
GO


-- ----------------------------
-- Records of 物资计划
-- ----------------------------

-- ----------------------------
-- Table structure for 物资计划明细
-- ----------------------------
CREATE TABLE [dbo].[物资计划明细] (
  [计划id] bigint NOT NULL,
  [物资id] bigint NOT NULL,
  [前期数量] real NULL,
  [上期数量] real NULL,
  [库存数量] real NULL,
  [计划数量] real NULL,
  [审批数量] real NULL,
  [单价] real NULL,
  [金额] real NULL,
  [显示顺序] int NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'前年、上上月、前季度数量',
'SCHEMA', N'dbo',
'TABLE', N'物资计划明细',
'COLUMN', N'前期数量'
GO

EXEC sp_addextendedproperty
'MS_Description', N'去年、上个月、上季度数量',
'SCHEMA', N'dbo',
'TABLE', N'物资计划明细',
'COLUMN', N'上期数量'
GO


-- ----------------------------
-- Records of 物资计划明细
-- ----------------------------

-- ----------------------------
-- Table structure for 物资库存
-- ----------------------------
CREATE TABLE [dbo].[物资库存] (
  [id] bigint NOT NULL,
  [部门id] bigint NULL,
  [物资id] bigint NULL,
  [批次] nvarchar(16) NULL,
  [可用数量] real NULL,
  [可用金额] real NULL,
  [实际数量] real NULL,
  [实际金额] real NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'相同物资ID不同批次的物资独立计算库存，部门ID+物资ID+批次构成唯一索引',
'SCHEMA', N'dbo',
'TABLE', N'物资库存',
'COLUMN', N'批次'
GO

EXEC sp_addextendedproperty
'MS_Description', N'当填写申领单还未审批时只影响可用数量，确保后填写申领单时数量有效',
'SCHEMA', N'dbo',
'TABLE', N'物资库存',
'COLUMN', N'可用数量'
GO


-- ----------------------------
-- Records of 物资库存
-- ----------------------------

-- ----------------------------
-- Table structure for 物资目录
-- ----------------------------
CREATE TABLE [dbo].[物资目录] (
  [id] bigint NOT NULL,
  [分类id] bigint NULL,
  [名称] nvarchar(64) NULL,
  [规格] nvarchar(64) NULL,
  [产地] nvarchar(64) NULL,
  [成本价] real NULL,
  [核算方式] smallint NULL,
  [摊销月数] int NULL,
  [建档时间] datetime NULL,
  [撤档时间] datetime NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'计量单位，如 盒、10个/包、20个/箱、支',
'SCHEMA', N'dbo',
'TABLE', N'物资目录',
'COLUMN', N'规格'
GO

EXEC sp_addextendedproperty
'MS_Description', N'名称,规格,产地构成唯一索引',
'SCHEMA', N'dbo',
'TABLE', N'物资目录',
'COLUMN', N'产地'
GO

EXEC sp_addextendedproperty
'MS_Description', N'预估价格，库存计算金额用',
'SCHEMA', N'dbo',
'TABLE', N'物资目录',
'COLUMN', N'成本价'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#物资核算方式#一次性、分期摊销(折旧)',
'SCHEMA', N'dbo',
'TABLE', N'物资目录',
'COLUMN', N'核算方式'
GO

EXEC sp_addextendedproperty
'MS_Description', N'当核算方式为分期摊销时的总月数',
'SCHEMA', N'dbo',
'TABLE', N'物资目录',
'COLUMN', N'摊销月数'
GO


-- ----------------------------
-- Records of 物资目录
-- ----------------------------
INSERT INTO [dbo].[物资目录] VALUES (N'104839509410344960', N'95413444640272384', N'电线', N'米', N'上海第一电线厂', NULL, NULL, NULL, N'2024-07-16 15:15:05', NULL), (N'95434428013375488', N'95413444640272384', N'测电笔', N'只', N'江苏苏州电工工具厂', NULL, NULL, NULL, NULL, NULL)
GO


-- ----------------------------
-- Table structure for 物资入出类别
-- ----------------------------
CREATE TABLE [dbo].[物资入出类别] (
  [id] bigint NOT NULL,
  [名称] nvarchar(32) NOT NULL,
  [系数] smallint NOT NULL,
  [单号前缀] char(2) NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'1-入库；-1-出库',
'SCHEMA', N'dbo',
'TABLE', N'物资入出类别',
'COLUMN', N'系数'
GO


-- ----------------------------
-- Records of 物资入出类别
-- ----------------------------
INSERT INTO [dbo].[物资入出类别] VALUES (N'1', N'外购入库', N'1', N'WG'), (N'2', N'自产入库', N'1', N'ZC'), (N'3', N'返还入库', N'1', N'FH'), (N'4', N'盘盈', N'1', N'PY'), (N'5', N'申领出库', N'-1', N'SL'), (N'6', N'物资报废', N'-1', N'BF'), (N'7', N'盘亏', N'-1', N'PK')
GO


-- ----------------------------
-- Table structure for 物资详单
-- ----------------------------
CREATE TABLE [dbo].[物资详单] (
  [id] bigint NOT NULL,
  [单据id] bigint NOT NULL,
  [物资id] bigint NULL,
  [序号] smallint NULL,
  [批次] nvarchar(16) NULL,
  [数量] real NULL,
  [单价] real NULL,
  [金额] real NULL,
  [随货单号] nvarchar(128) NULL,
  [发票号] nvarchar(128) NULL,
  [发票日期] datetime NULL,
  [发票金额] real NULL,
  [盘点时间] datetime NULL,
  [盘点金额] real NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'在一张单据内部从1连续编号，入出类别+冲销状态+单号+序号共同构成唯一索引',
'SCHEMA', N'dbo',
'TABLE', N'物资详单',
'COLUMN', N'序号'
GO

EXEC sp_addextendedproperty
'MS_Description', N'按散装单位填写',
'SCHEMA', N'dbo',
'TABLE', N'物资详单',
'COLUMN', N'数量'
GO

EXEC sp_addextendedproperty
'MS_Description', N'售价',
'SCHEMA', N'dbo',
'TABLE', N'物资详单',
'COLUMN', N'单价'
GO

EXEC sp_addextendedproperty
'MS_Description', N'实际数量与单价的乘积。',
'SCHEMA', N'dbo',
'TABLE', N'物资详单',
'COLUMN', N'金额'
GO

EXEC sp_addextendedproperty
'MS_Description', N'外购入库时填写',
'SCHEMA', N'dbo',
'TABLE', N'物资详单',
'COLUMN', N'随货单号'
GO

EXEC sp_addextendedproperty
'MS_Description', N'外购入库时填写',
'SCHEMA', N'dbo',
'TABLE', N'物资详单',
'COLUMN', N'发票号'
GO

EXEC sp_addextendedproperty
'MS_Description', N'外购入库时填写',
'SCHEMA', N'dbo',
'TABLE', N'物资详单',
'COLUMN', N'发票日期'
GO

EXEC sp_addextendedproperty
'MS_Description', N'外购入库时填写',
'SCHEMA', N'dbo',
'TABLE', N'物资详单',
'COLUMN', N'发票金额'
GO

EXEC sp_addextendedproperty
'MS_Description', N'盘点有效',
'SCHEMA', N'dbo',
'TABLE', N'物资详单',
'COLUMN', N'盘点时间'
GO

EXEC sp_addextendedproperty
'MS_Description', N'盘点有效',
'SCHEMA', N'dbo',
'TABLE', N'物资详单',
'COLUMN', N'盘点金额'
GO


-- ----------------------------
-- Records of 物资详单
-- ----------------------------

-- ----------------------------
-- Table structure for 物资主单
-- ----------------------------
CREATE TABLE [dbo].[物资主单] (
  [id] bigint NOT NULL,
  [部门id] bigint NOT NULL,
  [入出类别id] bigint NOT NULL,
  [状态] smallint NOT NULL,
  [单号] nvarchar(8) NOT NULL,
  [摘要] nvarchar(64) NULL,
  [填制人] nvarchar(32) NULL,
  [填制日期] datetime NULL,
  [审核人] nvarchar(32) NULL,
  [审核日期] datetime NULL,
  [入出系数] smallint NULL,
  [供应商id] bigint NULL,
  [发料人] nvarchar(32) NULL,
  [发料日期] datetime NULL,
  [金额] real NULL,
  [发票金额] real NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'#单据状态#0-填写;1-待审核;2-已审核;3-被冲销;4-冲销',
'SCHEMA', N'dbo',
'TABLE', N'物资主单',
'COLUMN', N'状态'
GO

EXEC sp_addextendedproperty
'MS_Description', N'相同单号可以不同的冲销状态，命名：前缀+连续序号',
'SCHEMA', N'dbo',
'TABLE', N'物资主单',
'COLUMN', N'单号'
GO

EXEC sp_addextendedproperty
'MS_Description', N'如果是申领单，表示申领人',
'SCHEMA', N'dbo',
'TABLE', N'物资主单',
'COLUMN', N'填制人'
GO

EXEC sp_addextendedproperty
'MS_Description', N'1:物资入,-1:物资出;0-盘点记录单',
'SCHEMA', N'dbo',
'TABLE', N'物资主单',
'COLUMN', N'入出系数'
GO

EXEC sp_addextendedproperty
'MS_Description', N'外购入库时填写',
'SCHEMA', N'dbo',
'TABLE', N'物资主单',
'COLUMN', N'供应商id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'申请单时用效,主要反应该张单据什么人发的料',
'SCHEMA', N'dbo',
'TABLE', N'物资主单',
'COLUMN', N'发料人'
GO

EXEC sp_addextendedproperty
'MS_Description', N'申请单时用效',
'SCHEMA', N'dbo',
'TABLE', N'物资主单',
'COLUMN', N'发料日期'
GO

EXEC sp_addextendedproperty
'MS_Description', N'单据内所有详单的金额和',
'SCHEMA', N'dbo',
'TABLE', N'物资主单',
'COLUMN', N'金额'
GO


-- ----------------------------
-- Records of 物资主单
-- ----------------------------


-- ----------------------------
-- Indexes structure for table cm_file_my
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_file_my_parentid]
ON [dbo].[cm_file_my] (
  [parent_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_file_my_userid]
ON [dbo].[cm_file_my] (
  [user_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_cache
-- ----------------------------
ALTER TABLE [dbo].[cm_cache] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table cm_file_my
-- ----------------------------
ALTER TABLE [dbo].[cm_file_my] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_file_pub
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_file_pub_parentid]
ON [dbo].[cm_file_pub] (
  [parent_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_file_pub
-- ----------------------------
ALTER TABLE [dbo].[cm_file_pub] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_group
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [idx_group_name]
ON [dbo].[cm_group] (
  [name] ASC
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'不重复',
'SCHEMA', N'dbo',
'TABLE', N'cm_group',
'INDEX', N'idx_group_name'
GO


-- ----------------------------
-- Primary Key structure for table cm_group
-- ----------------------------
ALTER TABLE [dbo].[cm_group] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_group_role
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_group_role_groupid]
ON [dbo].[cm_group_role] (
  [group_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_group_role_roleid]
ON [dbo].[cm_group_role] (
  [role_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_group_role
-- ----------------------------
ALTER TABLE [dbo].[cm_group_role] ADD PRIMARY KEY CLUSTERED ([group_id], [role_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_menu
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_menu_parentid]
ON [dbo].[cm_menu] (
  [parent_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_menu
-- ----------------------------
ALTER TABLE [dbo].[cm_menu] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_option
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_option_groupid]
ON [dbo].[cm_option] (
  [group_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_option
-- ----------------------------
ALTER TABLE [dbo].[cm_option] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table cm_option_group
-- ----------------------------
ALTER TABLE [dbo].[cm_option_group] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_params
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [idx_params_name]
ON [dbo].[cm_params] (
  [name] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_params
-- ----------------------------
ALTER TABLE [dbo].[cm_params] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_permission
-- ----------------------------
CREATE NONCLUSTERED INDEX [fk_permission]
ON [dbo].[cm_permission] (
  [func_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_permission
-- ----------------------------
ALTER TABLE [dbo].[cm_permission] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_permission_func
-- ----------------------------
CREATE NONCLUSTERED INDEX [fk_permission_func]
ON [dbo].[cm_permission_func] (
  [module_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_permission_func
-- ----------------------------
ALTER TABLE [dbo].[cm_permission_func] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table cm_permission_module
-- ----------------------------
ALTER TABLE [dbo].[cm_permission_module] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Foreign Keys structure for table cm_permission
-- ----------------------------
ALTER TABLE [dbo].[cm_permission] ADD CONSTRAINT [fk_permission_func] FOREIGN KEY ([func_id]) REFERENCES [dbo].[cm_permission_func] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_permission_func
-- ----------------------------
ALTER TABLE [dbo].[cm_permission_func] ADD CONSTRAINT [fk_permission_module] FOREIGN KEY ([module_id]) REFERENCES [dbo].[cm_permission_module] ([id])
GO

ALTER TABLE [dbo].[cm_permission_func] ADD CONSTRAINT [uq_permission_func] UNIQUE NONCLUSTERED ([module_id], [name])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

ALTER TABLE [dbo].[cm_permission] ADD CONSTRAINT [uq_permission] UNIQUE NONCLUSTERED ([func_id], [name])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

-- ----------------------------
-- Indexes structure for table cm_role
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [idx_role_name]
ON [dbo].[cm_role] (
  [name] ASC
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'不重复',
'SCHEMA', N'dbo',
'TABLE', N'cm_role',
'INDEX', N'idx_role_name'
GO


-- ----------------------------
-- Primary Key structure for table cm_role
-- ----------------------------
ALTER TABLE [dbo].[cm_role] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_role_menu
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_role_menu_menuid]
ON [dbo].[cm_role_menu] (
  [menu_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_role_menu_roleid]
ON [dbo].[cm_role_menu] (
  [role_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_role_menu
-- ----------------------------
ALTER TABLE [dbo].[cm_role_menu] ADD PRIMARY KEY CLUSTERED ([role_id], [menu_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_role_per
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_role_per_roleid]
ON [dbo].[cm_role_per] (
  [role_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_role_per_perid]
ON [dbo].[cm_role_per] (
  [per_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_role_per
-- ----------------------------
ALTER TABLE [dbo].[cm_role_per] ADD PRIMARY KEY CLUSTERED ([role_id], [per_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_rpt
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [idx_rpt_name]
ON [dbo].[cm_rpt] (
  [name] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_rpt
-- ----------------------------
ALTER TABLE [dbo].[cm_rpt] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_user
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_user_acc]
ON [dbo].[cm_user] (
  [acc] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_user_phone]
ON [dbo].[cm_user] (
  [phone] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_user
-- ----------------------------
ALTER TABLE [dbo].[cm_user] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_user_group
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_user_group_userid]
ON [dbo].[cm_user_group] (
  [user_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_user_group_groupid]
ON [dbo].[cm_user_group] (
  [group_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_user_group
-- ----------------------------
ALTER TABLE [dbo].[cm_user_group] ADD PRIMARY KEY CLUSTERED ([user_id], [group_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_user_params
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_user_params_userid]
ON [dbo].[cm_user_params] (
  [user_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_user_params_paramsid]
ON [dbo].[cm_user_params] (
  [param_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_user_params
-- ----------------------------
ALTER TABLE [dbo].[cm_user_params] ADD PRIMARY KEY CLUSTERED ([user_id], [param_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_user_role
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_user_role_userid]
ON [dbo].[cm_user_role] (
  [user_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_user_role_roleid]
ON [dbo].[cm_user_role] (
  [role_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_user_role
-- ----------------------------
ALTER TABLE [dbo].[cm_user_role] ADD PRIMARY KEY CLUSTERED ([user_id], [role_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_wfd_atv
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_wfd_atv_prcid]
ON [dbo].[cm_wfd_atv] (
  [prc_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_wfd_atv
-- ----------------------------
ALTER TABLE [dbo].[cm_wfd_atv] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_wfd_atv_role
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_wfd_atv_role_roleid]
ON [dbo].[cm_wfd_atv_role] (
  [role_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_wfd_atv_role
-- ----------------------------
ALTER TABLE [dbo].[cm_wfd_atv_role] ADD PRIMARY KEY CLUSTERED ([atv_id], [role_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table cm_wfd_prc
-- ----------------------------
ALTER TABLE [dbo].[cm_wfd_prc] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_wfd_trs
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_wfd_trs_prcid]
ON [dbo].[cm_wfd_trs] (
  [prc_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_wfd_trs
-- ----------------------------
ALTER TABLE [dbo].[cm_wfd_trs] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_wfi_atv
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_wfi_atv_prciid]
ON [dbo].[cm_wfi_atv] (
  [prci_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_wfi_atv_atvdid]
ON [dbo].[cm_wfi_atv] (
  [atvd_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_wfi_atv
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_atv] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_wfi_item
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_wfi_item_atviid]
ON [dbo].[cm_wfi_item] (
  [atvi_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_wfi_item
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_item] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_wfi_prc
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_wfi_prc_prcdid]
ON [dbo].[cm_wfi_prc] (
  [prcd_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_wfi_prc
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_prc] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table cm_wfi_trs
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_wfi_trs_trsdid]
ON [dbo].[cm_wfi_trs] (
  [trsd_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_wfi_trs_srcatviid]
ON [dbo].[cm_wfi_trs] (
  [src_atvi_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_wfi_trs_tgtatviid]
ON [dbo].[cm_wfi_trs] (
  [tgt_atvi_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table cm_wfi_trs
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_trs] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table crud_大儿
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_大儿_parendid]
ON [dbo].[crud_大儿] (
  [parent_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table crud_大儿
-- ----------------------------
ALTER TABLE [dbo].[crud_大儿] ADD CONSTRAINT [crud_大儿_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table crud_父表
-- ----------------------------
ALTER TABLE [dbo].[crud_父表] ADD CONSTRAINT [crud_父表_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table crud_缓存表
-- ----------------------------
ALTER TABLE [dbo].[crud_缓存表] ADD CONSTRAINT [crud_缓存表_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table crud_基础
-- ----------------------------
ALTER TABLE [dbo].[crud_基础] ADD CONSTRAINT [crud_基础_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table crud_角色
-- ----------------------------
ALTER TABLE [dbo].[crud_角色] ADD CONSTRAINT [crud_角色_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table crud_角色权限
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_crud_角色权限_prvid]
ON [dbo].[crud_角色权限] (
  [prv_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_crud_角色权限_roleid]
ON [dbo].[crud_角色权限] (
  [role_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table crud_角色权限
-- ----------------------------
ALTER TABLE [dbo].[crud_角色权限] ADD CONSTRAINT [crud_角色权限_pkey] PRIMARY KEY CLUSTERED ([role_id], [prv_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table crud_扩展1
-- ----------------------------
ALTER TABLE [dbo].[crud_扩展1] ADD CONSTRAINT [crud_扩展1_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table crud_扩展2
-- ----------------------------
ALTER TABLE [dbo].[crud_扩展2] ADD CONSTRAINT [crud_扩展2_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table crud_权限
-- ----------------------------
ALTER TABLE [dbo].[crud_权限] ADD CONSTRAINT [crud_权限_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table crud_小儿
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_小儿_parentid]
ON [dbo].[crud_小儿] (
  [group_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table crud_小儿
-- ----------------------------
ALTER TABLE [dbo].[crud_小儿] ADD CONSTRAINT [crud_小儿_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table crud_用户
-- ----------------------------
ALTER TABLE [dbo].[crud_用户] ADD CONSTRAINT [crud_用户_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table crud_用户角色
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_crud_用户角色_roleid]
ON [dbo].[crud_用户角色] (
  [role_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_crud_用户角色_userid]
ON [dbo].[crud_用户角色] (
  [user_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table crud_用户角色
-- ----------------------------
ALTER TABLE [dbo].[crud_用户角色] ADD CONSTRAINT [crud_用户角色_pkey] PRIMARY KEY CLUSTERED ([user_id], [role_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table crud_主表
-- ----------------------------
ALTER TABLE [dbo].[crud_主表] ADD CONSTRAINT [crud_主表_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table crud_字段类型
-- ----------------------------
ALTER TABLE [dbo].[crud_字段类型] ADD CONSTRAINT [crud_字段类型_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table fsm_file
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [idx_fsm_file_path]
ON [dbo].[fsm_file] (
  [path] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table fsm_file
-- ----------------------------
ALTER TABLE [dbo].[fsm_file] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 部门
-- ----------------------------
ALTER TABLE [dbo].[部门] ADD CONSTRAINT [部门_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 部门人员
-- ----------------------------
ALTER TABLE [dbo].[部门人员] ADD CONSTRAINT [部门人员_pkey] PRIMARY KEY CLUSTERED ([部门id], [人员id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 供应商
-- ----------------------------
ALTER TABLE [dbo].[供应商] ADD CONSTRAINT [pk_供应商] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 人员
-- ----------------------------
ALTER TABLE [dbo].[人员] ADD CONSTRAINT [人员_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 物资分类
-- ----------------------------
ALTER TABLE [dbo].[物资分类] ADD CONSTRAINT [pk_物资分类] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 物资计划
-- ----------------------------
ALTER TABLE [dbo].[物资计划] ADD CONSTRAINT [pk_物资计划] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 物资计划明细
-- ----------------------------
ALTER TABLE [dbo].[物资计划明细] ADD CONSTRAINT [pk_物资计划明细] PRIMARY KEY CLUSTERED ([计划id], [物资id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table 物资库存
-- ----------------------------
CREATE NONCLUSTERED INDEX [ix_物资库存_物资id]
ON [dbo].[物资库存] (
  [物资id] ASC
)
GO

CREATE NONCLUSTERED INDEX [ix_物资库存_部门id]
ON [dbo].[物资库存] (
  [部门id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table 物资库存
-- ----------------------------
ALTER TABLE [dbo].[物资库存] ADD CONSTRAINT [pk_物资库存] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 物资目录
-- ----------------------------
ALTER TABLE [dbo].[物资目录] ADD CONSTRAINT [pk_物资目录] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 物资入出类别
-- ----------------------------
ALTER TABLE [dbo].[物资入出类别] ADD CONSTRAINT [物资入出类别_pkey] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 物资详单
-- ----------------------------
ALTER TABLE [dbo].[物资详单] ADD CONSTRAINT [pk_物资详单] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table 物资主单
-- ----------------------------
ALTER TABLE [dbo].[物资主单] ADD CONSTRAINT [pk_物资主单] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Foreign Keys structure for table cm_file_my
-- ----------------------------
ALTER TABLE [dbo].[cm_file_my] ADD CONSTRAINT [fk_file_my_parentid] FOREIGN KEY ([parent_id]) REFERENCES [dbo].[cm_file_my] ([id])
GO

ALTER TABLE [dbo].[cm_file_my] ADD CONSTRAINT [fk_file_my_userid] FOREIGN KEY ([user_id]) REFERENCES [dbo].[cm_user] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_file_pub
-- ----------------------------
ALTER TABLE [dbo].[cm_file_pub] ADD CONSTRAINT [fk_file_pub_parentid] FOREIGN KEY ([parent_id]) REFERENCES [dbo].[cm_file_pub] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_group_role
-- ----------------------------
ALTER TABLE [dbo].[cm_group_role] ADD CONSTRAINT [fk_group_role_groupid] FOREIGN KEY ([group_id]) REFERENCES [dbo].[cm_group] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[cm_group_role] ADD CONSTRAINT [fk_group_role_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[cm_role] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_menu
-- ----------------------------
ALTER TABLE [dbo].[cm_menu] ADD CONSTRAINT [fk_menu_parentid] FOREIGN KEY ([parent_id]) REFERENCES [dbo].[cm_menu] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_option
-- ----------------------------
ALTER TABLE [dbo].[cm_option] ADD CONSTRAINT [fk_option_groupid] FOREIGN KEY ([group_id]) REFERENCES [dbo].[cm_option_group] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_role_menu
-- ----------------------------
ALTER TABLE [dbo].[cm_role_menu] ADD CONSTRAINT [fk_role_menu_menuid] FOREIGN KEY ([menu_id]) REFERENCES [dbo].[cm_menu] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[cm_role_menu] ADD CONSTRAINT [fk_role_menu_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[cm_role] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_role_per
-- ----------------------------
ALTER TABLE [dbo].[cm_role_per] ADD CONSTRAINT [fk_role_per_perid] FOREIGN KEY ([per_id]) REFERENCES [dbo].[cm_permission] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[cm_role_per] ADD CONSTRAINT [fk_role_per_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[cm_role] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_user_group
-- ----------------------------
ALTER TABLE [dbo].[cm_user_group] ADD CONSTRAINT [fk_user_group_groupid] FOREIGN KEY ([group_id]) REFERENCES [dbo].[cm_group] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[cm_user_group] ADD CONSTRAINT [fk_user_group_userid] FOREIGN KEY ([user_id]) REFERENCES [dbo].[cm_user] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_user_params
-- ----------------------------
ALTER TABLE [dbo].[cm_user_params] ADD CONSTRAINT [fk_user_params_paramsid] FOREIGN KEY ([param_id]) REFERENCES [dbo].[cm_params] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[cm_user_params] ADD CONSTRAINT [fk_user_params_userid] FOREIGN KEY ([user_id]) REFERENCES [dbo].[cm_user] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_user_role
-- ----------------------------
ALTER TABLE [dbo].[cm_user_role] ADD CONSTRAINT [fk_user_role_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[cm_role] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[cm_user_role] ADD CONSTRAINT [fk_user_role_userid] FOREIGN KEY ([user_id]) REFERENCES [dbo].[cm_user] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfd_atv
-- ----------------------------
ALTER TABLE [dbo].[cm_wfd_atv] ADD CONSTRAINT [fk_wfd_atv_prcid] FOREIGN KEY ([prc_id]) REFERENCES [dbo].[cm_wfd_prc] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfd_atv_role
-- ----------------------------
ALTER TABLE [dbo].[cm_wfd_atv_role] ADD CONSTRAINT [fk_wfd_atv_role_atvid] FOREIGN KEY ([atv_id]) REFERENCES [dbo].[cm_wfd_atv] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[cm_wfd_atv_role] ADD CONSTRAINT [fk_wfd_atv_role_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[cm_role] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfd_trs
-- ----------------------------
ALTER TABLE [dbo].[cm_wfd_trs] ADD CONSTRAINT [fk_wfd_trs_prcid] FOREIGN KEY ([prc_id]) REFERENCES [dbo].[cm_wfd_prc] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfi_atv
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_atv] ADD CONSTRAINT [fk_wfi_atv_atvdid] FOREIGN KEY ([atvd_id]) REFERENCES [dbo].[cm_wfd_atv] ([id])
GO

ALTER TABLE [dbo].[cm_wfi_atv] ADD CONSTRAINT [fk_wfi_atv_prciid] FOREIGN KEY ([prci_id]) REFERENCES [dbo].[cm_wfi_prc] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfi_item
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_item] ADD CONSTRAINT [fk_wfi_item_atviid] FOREIGN KEY ([atvi_id]) REFERENCES [dbo].[cm_wfi_atv] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfi_prc
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_prc] ADD CONSTRAINT [fk_wfi_prc_prcdid] FOREIGN KEY ([prcd_id]) REFERENCES [dbo].[cm_wfd_prc] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfi_trs
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_trs] ADD CONSTRAINT [fk_wfi_trs_srcatviid] FOREIGN KEY ([src_atvi_id]) REFERENCES [dbo].[cm_wfi_atv] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[cm_wfi_trs] ADD CONSTRAINT [fk_wfi_trs_tgtatviid] FOREIGN KEY ([tgt_atvi_id]) REFERENCES [dbo].[cm_wfi_atv] ([id])
GO

ALTER TABLE [dbo].[cm_wfi_trs] ADD CONSTRAINT [fk_wfi_trs_trsdid] FOREIGN KEY ([trsd_id]) REFERENCES [dbo].[cm_wfd_trs] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table crud_大儿
-- ----------------------------
ALTER TABLE [dbo].[crud_大儿] ADD CONSTRAINT [fk_大儿_parendid] FOREIGN KEY ([parent_id]) REFERENCES [dbo].[crud_父表] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table crud_角色权限
-- ----------------------------
ALTER TABLE [dbo].[crud_角色权限] ADD CONSTRAINT [fk_角色权限_prvid] FOREIGN KEY ([prv_id]) REFERENCES [dbo].[crud_权限] ([id])
GO

ALTER TABLE [dbo].[crud_角色权限] ADD CONSTRAINT [fk_角色权限_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[crud_角色] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table crud_小儿
-- ----------------------------
ALTER TABLE [dbo].[crud_小儿] ADD CONSTRAINT [fk_小儿_parentid] FOREIGN KEY ([group_id]) REFERENCES [dbo].[crud_父表] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table crud_用户角色
-- ----------------------------
ALTER TABLE [dbo].[crud_用户角色] ADD CONSTRAINT [fk_crud_用户角色_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[crud_角色] ([id])
GO

ALTER TABLE [dbo].[crud_用户角色] ADD CONSTRAINT [fk_crud_用户角色_userid] FOREIGN KEY ([user_id]) REFERENCES [dbo].[crud_用户] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table 部门
-- ----------------------------
ALTER TABLE [dbo].[部门] ADD CONSTRAINT [fk_部门_上级id] FOREIGN KEY ([上级id]) REFERENCES [dbo].[部门] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table 部门人员
-- ----------------------------
ALTER TABLE [dbo].[部门人员] ADD CONSTRAINT [fk_部门人员_人员] FOREIGN KEY ([人员id]) REFERENCES [dbo].[人员] ([id])
GO

ALTER TABLE [dbo].[部门人员] ADD CONSTRAINT [fk_部门人员_部门] FOREIGN KEY ([部门id]) REFERENCES [dbo].[部门] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table 物资计划
-- ----------------------------
ALTER TABLE [dbo].[物资计划] ADD CONSTRAINT [fk_物资计划_部门] FOREIGN KEY ([部门id]) REFERENCES [dbo].[部门] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table 物资计划明细
-- ----------------------------
ALTER TABLE [dbo].[物资计划明细] ADD CONSTRAINT [fk_物资计划明细_物资] FOREIGN KEY ([物资id]) REFERENCES [dbo].[物资目录] ([id])
GO

ALTER TABLE [dbo].[物资计划明细] ADD CONSTRAINT [fk_物资计划明细_计划] FOREIGN KEY ([计划id]) REFERENCES [dbo].[物资计划] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table 物资库存
-- ----------------------------
ALTER TABLE [dbo].[物资库存] ADD CONSTRAINT [fk_物资库存_物资] FOREIGN KEY ([物资id]) REFERENCES [dbo].[物资目录] ([id])
GO

ALTER TABLE [dbo].[物资库存] ADD CONSTRAINT [fk_物资库存_部门] FOREIGN KEY ([部门id]) REFERENCES [dbo].[部门] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table 物资目录
-- ----------------------------
ALTER TABLE [dbo].[物资目录] ADD CONSTRAINT [fk_物资目录_分类] FOREIGN KEY ([分类id]) REFERENCES [dbo].[物资分类] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table 物资详单
-- ----------------------------
ALTER TABLE [dbo].[物资详单] ADD CONSTRAINT [fk_物资详单_单据] FOREIGN KEY ([单据id]) REFERENCES [dbo].[物资主单] ([id])
GO

ALTER TABLE [dbo].[物资详单] ADD CONSTRAINT [fk_物资详单_物资] FOREIGN KEY ([物资id]) REFERENCES [dbo].[物资目录] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table 物资主单
-- ----------------------------
ALTER TABLE [dbo].[物资主单] ADD CONSTRAINT [fk_物资主单_供应商] FOREIGN KEY ([供应商id]) REFERENCES [dbo].[供应商] ([id])
GO

ALTER TABLE [dbo].[物资主单] ADD CONSTRAINT [fk_物资主单_入出类别] FOREIGN KEY ([入出类别id]) REFERENCES [dbo].[物资入出类别] ([id])
GO

ALTER TABLE [dbo].[物资主单] ADD CONSTRAINT [fk_物资主单_部门] FOREIGN KEY ([部门id]) REFERENCES [dbo].[部门] ([id])
GO


-- ----------------------------
-- 序列
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_menu_dispidx]') AND type IN ('SO'))
drop sequence cm_menu_dispidx
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_option_dispidx]') AND type IN ('SO'))
drop sequence cm_option_dispidx
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfd_prc_dispidx]') AND type IN ('SO'))
drop sequence cm_wfd_prc_dispidx
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfi_item_dispidx]') AND type IN ('SO'))
drop sequence cm_wfi_item_dispidx
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[cm_wfi_prc_dispidx]') AND type IN ('SO'))
drop sequence cm_wfi_prc_dispidx
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[crud_基础_序列]') AND type IN ('SO'))
drop sequence crud_基础_序列
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[物资主单_单号]') AND type IN ('SO'))
drop sequence 物资主单_单号
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[物资入出类别_id]') AND type IN ('SO'))
drop sequence 物资入出类别_id
GO


create sequence cm_menu_dispidx start with 138;
create sequence cm_option_dispidx start with 1050;
create sequence cm_wfd_prc_dispidx start with 15;
create sequence cm_wfi_item_dispidx start with 258;
create sequence cm_wfi_prc_dispidx start with 81;

create sequence crud_基础_序列 start with 85;
create sequence 物资主单_单号 start with 11;
create sequence 物资入出类别_id start with 12;
GO

-- ----------------------------
-- View structure for v_部门
-- ----------------------------
CREATE VIEW [dbo].[v_部门] AS SELECT a.id,
    a.上级id,
    a.编码,
    a.名称,
    a.说明,
    a.建档时间,
    a.撤档时间,
    b.名称 AS 上级部门
   FROM 部门 a
     LEFT JOIN 部门 b ON a.上级id = b.id
GO


-- ----------------------------
-- View structure for v_人员
-- ----------------------------
CREATE VIEW [dbo].[v_人员] AS SELECT a.id,
    a.姓名,
    a.出生日期,
    a.性别,
    a.工作日期,
    a.办公室电话,
    a.电子邮件,
    a.建档时间,
    a.撤档时间,
    a.撤档原因,
    a.user_id,
    COALESCE(b.name, b.acc, b.phone) AS 账号
   FROM 人员 a
     LEFT JOIN cm_user b ON a.user_id = b.id
GO


-- ----------------------------
-- View structure for v_物资目录
-- ----------------------------
CREATE VIEW [dbo].[v_物资目录] AS SELECT a.id,
    a.分类id,
    a.名称,
    a.规格,
    a.产地,
    a.成本价,
    a.核算方式,
    a.摊销月数,
    a.建档时间,
    a.撤档时间,
    b.名称 AS 物资分类
   FROM 物资目录 a
     LEFT JOIN 物资分类 b ON a.分类id = b.id
GO


-- ----------------------------
-- View structure for v_物资详单
-- ----------------------------
CREATE VIEW [dbo].[v_物资详单] AS SELECT a.id,
    a.单据id,
    a.物资id,
    a.序号,
    a.批次,
    a.数量,
    a.单价,
    a.金额,
    a.随货单号,
    a.发票号,
    a.发票日期,
    a.发票金额,
    a.盘点时间,
    a.盘点金额,
    b.名称 AS 物资名称,
    b.规格,
    b.产地
   FROM 物资详单 a
     LEFT JOIN 物资目录 b ON a.物资id = b.id
GO


-- ----------------------------
-- View structure for v_物资主单
-- ----------------------------
CREATE VIEW [dbo].[v_物资主单] AS SELECT a.id,
    a.部门id,
    a.入出类别id,
    a.状态,
    a.单号,
    a.摘要,
    a.填制人,
    a.填制日期,
    a.审核人,
    a.审核日期,
    a.入出系数,
    a.供应商id,
    a.发料人,
    a.发料日期,
    a.金额,
    a.发票金额,
    b.名称 AS 部门名称,
    c.名称 AS 供应商,
    d.名称 AS 入出类别
   FROM 物资主单 a
     LEFT JOIN 部门 b ON a.部门id = b.id
     LEFT JOIN 供应商 c ON a.供应商id = c.id
     LEFT JOIN 物资入出类别 d ON a.入出类别id = d.id
GO
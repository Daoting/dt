-- ----------------------------
-- Table structure for cm_file_my
-- ----------------------------
CREATE TABLE [dbo].[cm_file_my] (
  [id] bigint NOT NULL,
  [parent_id] bigint NULL,
  [name] nvarchar(255) NOT NULL,
  [is_folder] bit NOT NULL,
  [ext_name] nvarchar(8) NULL,
  [info] nvarchar(512) NOT NULL,
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
  [info] nvarchar(512) NOT NULL,
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
  [note] nvarchar(255) NOT NULL
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
'MS_Description', N'分组，与用户和角色多对多',
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
  [view_name] nvarchar(128) NOT NULL,
  [params] nvarchar(4000) NOT NULL,
  [icon] nvarchar(128) NOT NULL,
  [note] nvarchar(512) NOT NULL,
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
INSERT INTO [dbo].[cm_menu] VALUES (N'1', NULL, N'工作台', N'1', N'', N'', N'搬运工', N'', N'1', N'0', N'2019-03-07 10:45:44', N'2019-03-07 10:45:43'), (N'2', N'1', N'用户账号', N'0', N'用户账号', N'', N'钥匙', N'', N'2', N'0', N'2019-11-08 11:42:28', N'2019-11-08 11:43:53'), (N'3', N'1', N'菜单管理', N'0', N'菜单管理', N'', N'大图标', N'', N'3', N'0', N'2019-03-11 11:35:59', N'2019-03-11 11:35:58'), (N'4', N'1', N'系统角色', N'0', N'系统角色', N'', N'两人', N'', N'4', N'0', N'2019-11-08 11:47:21', N'2019-11-08 11:48:22'), (N'5', N'1', N'分组管理', N'0', N'分组管理', N'', N'分组', N'', N'5', N'0', N'2023-03-10 08:34:49', N'2023-03-10 08:34:49'), (N'6', N'1', N'基础权限', N'0', N'基础权限', N'', N'审核', N'', N'6', N'0', N'2019-03-12 09:11:22', N'2019-03-07 11:23:40'), (N'7', N'1', N'参数定义', N'0', N'参数定义', N'', N'调色板', N'', N'7', N'0', N'2019-03-12 15:35:56', N'2019-03-12 15:37:10'), (N'8', N'1', N'基础选项', N'0', N'基础选项', N'', N'修理', N'', N'8', N'0', N'2019-11-08 11:49:40', N'2019-11-08 11:49:46'), (N'9', N'1', N'报表设计', N'0', N'报表设计', N'', N'折线图', N'', N'76', N'0', N'2020-10-19 11:21:38', N'2020-10-19 11:21:38'), (N'10', N'1', N'流程设计', N'0', N'流程设计', N'', N'双绞线', N'', N'79', N'0', N'2020-11-02 16:21:19', N'2020-11-02 16:21:19')
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
  [value] nvarchar(255) NOT NULL,
  [note] nvarchar(255) NOT NULL,
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
INSERT INTO [dbo].[cm_permission_module] VALUES (N'1', N'系统预留', N'系统内部使用的权限控制，禁止删除')
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
INSERT INTO [dbo].[cm_permission_func] VALUES (N'1', N'1', N'文件管理', N'管理文件的上传、删除等')
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
INSERT INTO [dbo].[cm_permission] VALUES (N'1', N'1', N'公共文件增删', N'公共文件的上传、删除等'), (N'2', N'1', N'素材库增删', N'素材库目录的上传、删除等')
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
INSERT INTO [dbo].[cm_role_menu] VALUES (N'2', N'2'), (N'2', N'3'), (N'2', N'4'), (N'2', N'5'), (N'2', N'6'), (N'1', N'7'), (N'1', N'8'), (N'1', N'9'), (N'2', N'10')
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
  [define] nvarchar(max) NOT NULL,
  [note] nvarchar(255) NOT NULL,
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
  [name] nvarchar(32) NOT NULL,
  [phone] nvarchar(16) NOT NULL,
  [pwd] char(32) NOT NULL,
  [photo] nvarchar(255) NOT NULL,
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
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'密码的md5',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'pwd'
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
INSERT INTO [dbo].[cm_user] VALUES (1, N'admin', N'13511111111', 'b59c67bf196a4758191e42f76670ceba', N'', '0', '2019-10-24 09:06:38.000', '2023-03-16 08:35:39.000'); GO



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
  [value] nvarchar(255) NOT NULL,
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
  [sender] nvarchar(32) NOT NULL,
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
'MS_Description', N'发送者',
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
-- Records of fsm_file
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
CREATE NONCLUSTERED INDEX [idx_user_name]
ON [dbo].[cm_user] (
  [name] ASC
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
-- 序列
-- ----------------------------
drop sequence cm_menu_dispidx;
drop sequence cm_option_dispidx;
drop sequence cm_wfd_prc_dispidx;
drop sequence cm_wfi_item_dispidx;
drop sequence cm_wfi_prc_dispidx;
GO

create sequence cm_menu_dispidx start with 90;
create sequence cm_option_dispidx start with 1032;
create sequence cm_wfd_prc_dispidx start with 12;
create sequence cm_wfi_item_dispidx start with 177;
create sequence cm_wfi_prc_dispidx start with 66;
GO
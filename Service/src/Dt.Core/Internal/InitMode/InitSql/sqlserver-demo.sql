/*
Navicat 从 mysql 导出后修改：
1. tinyint bool类型转bit 枚举类型保留
2. nchar 转 char
3. datetime2 转 datetime
*/

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
-- Records of cm_file_my
-- ----------------------------
INSERT INTO [dbo].[cm_file_my] VALUES (N'140724076930789376', NULL, N'新目录1', N'1', NULL, N'', N'2020-10-23 15:47:16', N'1'), (N'140724154458304512', N'140724076930789376', N'b', N'1', NULL, N'', N'2020-10-23 15:47:34', N'1'), (N'141735914371936256', NULL, N'新目录12', N'1', NULL, N'', N'2020-10-26 10:48:01', N'2'), (N'456284281217503232', NULL, N'新Tab', N'1', N'', N'', N'2023-03-13 10:30:55', N'1')
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
INSERT INTO [dbo].[cm_file_pub] VALUES (N'1', NULL, N'公共文件', N'1', NULL, N'', N'2020-10-21 15:19:20'), (N'2', NULL, N'素材库', N'1', NULL, N'', N'2020-10-21 15:20:21'), (N'140015729575325696', N'1', N'新目录a', N'1', NULL, N'', N'2020-11-19 13:17:25'), (N'140016348063199232', N'1', N'新目录1111', N'1', NULL, N'', N'2020-10-21 16:55:00'), (N'140244264617373696', N'140016348063199232', N'新目录q', N'1', NULL, N'', N'2020-10-22 08:00:39'), (N'140253323206717440', N'140244264617373696', N'ab', N'1', NULL, N'', N'2020-10-22 08:36:39'), (N'140266906502164480', N'140244264617373696', N'aa', N'0', N'xlsx', N'[["v0/1F/4A/140266906879651840.xlsx","aa","xlsx文件",8236,"daoting","2020-10-22 09:30"]]', N'2020-10-22 09:30:37'), (N'142873261784297472', N'2', N'新目录1', N'1', NULL, N'', N'2020-10-29 14:07:20'), (N'142888903606398976', N'2', N'12', N'0', N'xlsx', N'[["v0/52/37/142888904373956608.xlsx","12","xlsx文件",8153,"daoting","2020-10-29 15:09"]]', N'2020-10-29 15:09:30'), (N'142913881819181056', N'2', N'未标题-2', N'0', N'jpg', N'[["v0/E3/18/142913882284748800.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2020-10-29 16:48"]]', N'2020-10-29 16:48:44'), (N'142914110945619968', N'2', N'Icon-20@2x', N'0', N'png', N'[["v0/E3/0D/142914111109197824.png","Icon-20@2x","40 x 40 (.png)",436,"daoting","2020-10-29 16:49"]]', N'2020-10-29 16:49:39'), (N'143174605384577024', N'140016348063199232', N'Icon-20@3x', N'0', N'png', N'[["v0/56/59/143174606269575168.png","Icon-20@3x","60 x 60 (.png)",496,"daoting","2020-10-30 10:04"]]', N'2020-10-30 10:04:47'), (N'143191060503195648', N'1', N'Icon-20@3x', N'0', N'png', N'[["v0/56/59/143191060947791872.png","Icon-20@3x","60 x 60 (.png)",534,"daoting","2020-10-30 11:10"]]', N'2020-10-30 11:10:10'), (N'143192411157164032', N'140015729575325696', N'Icon-29@2x', N'0', N'png', N'[["v0/46/CE/143192411832446976.png","Icon-29@2x","58 x 58 (.png)",624,"daoting","2020-10-30 11:15"]]', N'2020-10-30 11:15:32'), (N'143193081423720448', N'140015729575325696', N'3709740f5c5e4cb4909a6cc79f412734_th', N'0', N'png', N'[["v0/BF/6D/143193081931231232.png","3709740f5c5e4cb4909a6cc79f412734_th","537 x 302 (.png)",27589,"daoting","2020-10-30 11:18"]]', N'2020-10-30 11:18:12'), (N'143195001659977728', N'1', N'未标题-2', N'0', N'jpg', N'[["v0/E3/18/143195002217820160.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2020-10-30 11:25"]]', N'2020-10-30 11:25:50'), (N'143203944146792448', N'1', N'ImageStabilization', N'0', N'wmv', N'[["v0/EA/34/143203944767549440.wmv","ImageStabilization","00:00:06 (480 x 288)",403671,"daoting","2020-10-30 12:01"]]', N'2020-10-30 12:01:22'), (N'172190549410705408', N'1', N'公司服务器及网络', N'0', N'txt', N'[["v0/5F/37/172190549775609856.txt","公司服务器及网络","txt文件",435,"daoting","2021-01-18 11:43"]]', N'2021-01-18 11:43:37'), (N'185641691419373568', N'1', N'1', N'0', N'png', N'[["v0/FC/63/185641725430984704.png","1","1101 x 428 (.png)",47916,"daoting","2021-02-24 14:33"]]', N'2021-02-24 14:33:46'), (N'187725770344230912', N'1', N'doc1', N'0', N'png', N'[["v0/D8/28/187725778074333184.png","doc1","1076 x 601 (.png)",59038,"daoting","2021-03-02 08:35"]]', N'2021-03-02 08:35:12'), (N'205916917767991296', N'140015729575325696', N'state', N'0', N'db', N'[["v0/DF/F3/205916918690738176.db","state","db文件",90112,"苹果","2021-04-21 13:20"]]', N'2021-04-21 13:20:20'), (N'255970120425140224', N'456277006646005760', N'abc', N'1', N'', N'', N'2021-09-06 16:13:53'), (N'322270820868235264', N'1', N'172190549775609856', N'0', N'txt', N'[["editor/57/01/322270823007330304.txt","172190549775609856","txt文件",435,"daoting","2022-03-08 15:09"]]', N'2022-03-08 15:09:10'), (N'456276498464133120', N'456277006646005760', N'未标题-2', N'0', N'jpg', N'[["editor/E3/18/456276498854203392.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2023-03-13 09:59"]]', N'2023-03-13 09:59:59'), (N'456277006646005760', N'1', N'新Tab', N'1', N'', N'', N'2023-03-13 10:02:00'), (N'456281421624922112', N'255970120425140224', N'未标题-2', N'0', N'jpg', N'[["editor/E3/18/456281422107267072.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2023-03-13 10:19"]]', N'2023-03-13 10:19:33'), (N'456281921225248768', N'456277006646005760', N'UserList', N'0', N'xaml', N'[["editor/C1/45/456281921523044352.xaml","UserList","xaml文件",2682,"daoting","2023-03-13 10:21"]]', N'2023-03-13 10:21:32')
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
-- Records of cm_group
-- ----------------------------
INSERT INTO [dbo].[cm_group] VALUES (N'454483802783240192', N'分组1', N''), (N'454484847190102016', N'2', N''), (N'454484924033945600', N'3', N'')
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
-- Records of cm_group_role
-- ----------------------------
INSERT INTO [dbo].[cm_group_role] VALUES (N'454483802783240192', N'2'), (N'454483802783240192', N'22844822693027840'), (N'454483802783240192', N'152695933758603264'), (N'454483802783240192', N'152696004814307328'), (N'454484847190102016', N'152695933758603264'), (N'454484924033945600', N'22844822693027840')
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
INSERT INTO [dbo].[cm_menu] VALUES (N'1', NULL, N'工作台', N'1', N'', N'', N'搬运工', N'', N'1', N'0', N'2019-03-07 10:45:44', N'2019-03-07 10:45:43'), (N'2', N'1', N'用户账号', N'0', N'用户账号', N'', N'钥匙', N'', N'2', N'0', N'2019-11-08 11:42:28', N'2019-11-08 11:43:53'), (N'3', N'1', N'菜单管理', N'0', N'菜单管理', N'', N'大图标', N'', N'3', N'0', N'2019-03-11 11:35:59', N'2019-03-11 11:35:58'), (N'4', N'1', N'系统角色', N'0', N'系统角色', N'', N'两人', N'', N'4', N'0', N'2019-11-08 11:47:21', N'2019-11-08 11:48:22'), (N'5', N'1', N'分组管理', N'0', N'分组管理', N'', N'分组', N'', N'5', N'0', N'2023-03-10 08:34:49', N'2023-03-10 08:34:49'), (N'6', N'1', N'基础权限', N'0', N'基础权限', N'', N'审核', N'', N'6', N'0', N'2019-03-12 09:11:22', N'2019-03-07 11:23:40'), (N'7', N'1', N'参数定义', N'0', N'参数定义', N'', N'调色板', N'', N'7', N'0', N'2019-03-12 15:35:56', N'2019-03-12 15:37:10'), (N'8', N'1', N'基础选项', N'0', N'基础选项', N'', N'修理', N'', N'8', N'0', N'2019-11-08 11:49:40', N'2019-11-08 11:49:46'), (N'9', N'1', N'报表设计', N'0', N'报表设计', N'', N'折线图', N'', N'76', N'0', N'2020-10-19 11:21:38', N'2020-10-19 11:21:38'), (N'10', N'1', N'流程设计', N'0', N'流程设计', N'', N'双绞线', N'', N'79', N'0', N'2020-11-02 16:21:19', N'2020-11-02 16:21:19'), (N'15268145234386944', N'15315938808373248', N'新菜单组22', N'1', N'', N'', N'文件夹', N'', N'25', N'0', N'2019-11-12 11:10:10', N'2019-11-12 11:10:13'), (N'15315637929975808', N'18562741636898816', N'新菜单12', N'0', N'', N'', N'文件', N'', N'48', N'0', N'2019-11-12 14:18:53', N'2019-11-12 14:31:38'), (N'15315938808373248', NULL, N'新菜单组额', N'1', N'', N'', N'文件夹', N'', N'67', N'0', N'2019-11-12 14:20:04', N'2019-11-12 14:20:14'), (N'18562741636898816', N'15315938808373248', N'新组t', N'1', N'', N'', N'文件夹', N'', N'63', N'0', N'2019-11-21 13:21:43', N'2019-11-21 13:21:43'), (N'18860286065975296', NULL, N'新菜单a123', N'0', N'报表', N'新报表111,abc1', N'文件', N'', N'68', N'0', N'2019-11-22 09:04:04', N'2019-11-22 09:04:04'), (N'154430055023640576', NULL, N'新菜单xxx', N'0', N'报表', N'', N'文件', N'', N'84', N'0', N'2020-11-30 11:29:56', N'2020-11-30 11:29:56'), (N'259520016549801984', NULL, N'新组bcd', N'1', N'', N'', N'文件夹', N'', N'83', N'0', N'2021-09-16 11:19:54', N'2021-09-16 11:19:54')
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
INSERT INTO [dbo].[cm_option] VALUES (N'2', N'汉族', N'2', N'1'), (N'3', N'蒙古族', N'3', N'1'), (N'4', N'回族', N'4', N'1'), (N'5', N'藏族', N'5', N'1'), (N'6', N'维吾尔族', N'6', N'1'), (N'7', N'苗族', N'7', N'1'), (N'8', N'彝族', N'8', N'1'), (N'9', N'壮族', N'9', N'1'), (N'10', N'布依族', N'10', N'1'), (N'11', N'朝鲜族', N'11', N'1'), (N'12', N'满族', N'12', N'1'), (N'13', N'侗族', N'13', N'1'), (N'14', N'瑶族', N'14', N'1'), (N'15', N'白族', N'15', N'1'), (N'16', N'土家族', N'16', N'1'), (N'17', N'哈尼族', N'17', N'1'), (N'18', N'哈萨克族', N'18', N'1'), (N'19', N'傣族', N'19', N'1'), (N'20', N'黎族', N'20', N'1'), (N'21', N'傈僳族', N'21', N'1'), (N'22', N'佤族', N'22', N'1'), (N'23', N'畲族', N'23', N'1'), (N'24', N'高山族', N'24', N'1'), (N'25', N'拉祜族', N'25', N'1'), (N'26', N'水族', N'26', N'1'), (N'27', N'东乡族', N'27', N'1'), (N'28', N'纳西族', N'28', N'1'), (N'29', N'景颇族', N'29', N'1'), (N'30', N'柯尔克孜族', N'30', N'1'), (N'31', N'土族', N'31', N'1'), (N'32', N'达斡尔族', N'32', N'1'), (N'33', N'仫佬族', N'33', N'1'), (N'34', N'羌族', N'34', N'1'), (N'35', N'布朗族', N'35', N'1'), (N'36', N'撒拉族', N'36', N'1'), (N'37', N'毛难族', N'37', N'1'), (N'38', N'仡佬族', N'38', N'1'), (N'39', N'锡伯族', N'39', N'1'), (N'40', N'阿昌族', N'40', N'1'), (N'41', N'普米族', N'41', N'1'), (N'42', N'塔吉克族', N'42', N'1'), (N'43', N'怒族', N'43', N'1'), (N'44', N'乌孜别克族', N'44', N'1'), (N'45', N'俄罗斯族', N'45', N'1'), (N'46', N'鄂温克族', N'46', N'1'), (N'47', N'德昂族', N'47', N'1'), (N'48', N'保安族', N'48', N'1'), (N'49', N'裕固族', N'49', N'1'), (N'50', N'京族', N'50', N'1'), (N'51', N'塔塔尔族', N'51', N'1'), (N'52', N'独龙族', N'52', N'1'), (N'53', N'鄂伦春族', N'53', N'1'), (N'54', N'赫哲族', N'54', N'1'), (N'55', N'门巴族', N'55', N'1'), (N'56', N'珞巴族', N'56', N'1'), (N'57', N'基诺族', N'57', N'1'), (N'58', N'大学', N'58', N'2'), (N'59', N'高中', N'59', N'2'), (N'60', N'中学', N'60', N'2'), (N'61', N'小学', N'61', N'2'), (N'62', N'硕士', N'62', N'2'), (N'63', N'博士', N'63', N'2'), (N'64', N'其他', N'64', N'2'), (N'65', N'黑龙江杜尔伯特县', N'65', N'3'), (N'66', N'黑龙江富裕县', N'66', N'3'), (N'67', N'黑龙江林甸县', N'67', N'3'), (N'68', N'黑龙江克山县', N'68', N'3'), (N'69', N'黑龙江克东县', N'69', N'3'), (N'70', N'黑龙江省拜泉县', N'70', N'3'), (N'71', N'黑龙江鸡西市', N'71', N'3'), (N'72', N'黑龙江鸡东县', N'72', N'3'), (N'73', N'黑龙江鹤岗市', N'73', N'3'), (N'74', N'黑龙江萝北县', N'74', N'3'), (N'75', N'黑龙江绥滨县', N'75', N'3'), (N'76', N'黑龙江双鸭山市', N'76', N'3'), (N'77', N'黑龙江集贤县', N'77', N'3'), (N'78', N'黑龙江大庆市', N'78', N'3'), (N'79', N'黑龙江伊春市', N'79', N'3'), (N'80', N'黑龙江嘉荫县', N'80', N'3'), (N'81', N'黑龙江佳木斯市', N'81', N'3'), (N'82', N'黑龙江桦南县', N'82', N'3'), (N'83', N'黑龙江依兰县', N'83', N'3'), (N'84', N'黑龙江桦川县', N'84', N'3'), (N'85', N'黑龙江省宝清县', N'85', N'3'), (N'86', N'黑龙江汤原县', N'86', N'3'), (N'87', N'黑龙江饶河县', N'87', N'3'), (N'88', N'黑龙江抚远县', N'88', N'3'), (N'89', N'黑龙江友谊县', N'89', N'3'), (N'90', N'黑龙江七台河市', N'90', N'3'), (N'91', N'黑龙江省勃利县', N'91', N'3'), (N'92', N'黑龙江牡丹江市', N'92', N'3'), (N'93', N'黑龙江宁安县', N'93', N'3'), (N'94', N'黑龙江海林县', N'94', N'3'), (N'95', N'黑龙江穆棱县', N'95', N'3'), (N'96', N'黑龙江东宁县', N'96', N'3'), (N'97', N'黑龙江林口县', N'97', N'3'), (N'98', N'黑龙江虎林县', N'98', N'3'), (N'99', N'黑龙江双城市', N'99', N'3'), (N'100', N'黑龙江尚志市', N'100', N'3'), (N'101', N'黑龙江省宾县', N'101', N'3')
GO

INSERT INTO [dbo].[cm_option] VALUES (N'102', N'黑龙江五常县', N'102', N'3'), (N'103', N'黑龙江省巴彦县', N'103', N'3'), (N'104', N'黑龙江木兰县', N'104', N'3'), (N'105', N'黑龙江通河县', N'105', N'3'), (N'106', N'黑龙江方正县', N'106', N'3'), (N'107', N'黑龙江延寿县', N'107', N'3'), (N'108', N'黑龙江绥化市', N'108', N'3'), (N'109', N'黑龙江省安达市', N'109', N'3'), (N'110', N'黑龙江肇东市', N'110', N'3'), (N'111', N'黑龙江海伦县', N'111', N'3'), (N'112', N'黑龙江望奎县', N'112', N'3'), (N'113', N'黑龙江兰西县', N'113', N'3'), (N'114', N'黑龙江青冈县', N'114', N'3'), (N'115', N'黑龙江肇源县', N'115', N'3'), (N'116', N'黑龙江肇州县', N'116', N'3'), (N'117', N'黑龙江庆安县', N'117', N'3'), (N'118', N'黑龙江明水县', N'118', N'3'), (N'119', N'黑龙江绥棱县', N'119', N'3'), (N'120', N'黑龙江黑河市', N'120', N'3'), (N'121', N'黑龙江省北安市', N'121', N'3'), (N'122', N'黑龙江五大连池市', N'122', N'3'), (N'123', N'黑龙江嫩江县', N'123', N'3'), (N'124', N'黑龙江省德都县', N'124', N'3'), (N'125', N'黑龙江逊克县', N'125', N'3'), (N'126', N'黑龙江孙吴县', N'126', N'3'), (N'127', N'黑龙江呼玛县', N'127', N'3'), (N'128', N'黑龙江塔河县', N'128', N'3'), (N'129', N'黑龙江漠河县', N'129', N'3'), (N'130', N'黑龙江绥芬河市', N'130', N'3'), (N'131', N'黑龙江省阿城市', N'131', N'3'), (N'132', N'黑龙江同江市', N'132', N'3'), (N'133', N'黑龙江富锦市', N'133', N'3'), (N'134', N'黑龙江铁力市', N'134', N'3'), (N'135', N'黑龙江密山市', N'135', N'3'), (N'136', N'吉林省长春市', N'136', N'3'), (N'137', N'内蒙古呼和浩特市', N'137', N'3'), (N'138', N'内蒙古土默特左旗', N'138', N'3'), (N'139', N'内蒙古托克托县', N'139', N'3'), (N'140', N'内蒙古包头市', N'140', N'3'), (N'141', N'内蒙古土默特右旗', N'141', N'3'), (N'142', N'内蒙古固阳县', N'142', N'3'), (N'143', N'内蒙古乌海市', N'143', N'3'), (N'144', N'内蒙古赤峰市', N'144', N'3'), (N'145', N'内蒙古阿鲁科尔沁旗', N'145', N'3'), (N'146', N'内蒙古巴林左旗', N'146', N'3'), (N'147', N'内蒙古巴林右旗', N'147', N'3'), (N'148', N'内蒙古林西县', N'148', N'3'), (N'149', N'内蒙古克什克腾旗', N'149', N'3'), (N'150', N'内蒙古翁牛特旗', N'150', N'3'), (N'151', N'内蒙古喀喇沁旗', N'151', N'3'), (N'152', N'内蒙古宁城县', N'152', N'3'), (N'153', N'内蒙古敖汉旗', N'153', N'3'), (N'154', N'内蒙古海拉尔市', N'154', N'3'), (N'155', N'内蒙古满州里市', N'155', N'3'), (N'156', N'内蒙古扎兰屯市', N'156', N'3'), (N'157', N'内蒙古牙克石市', N'157', N'3'), (N'158', N'内蒙古阿荣旗', N'158', N'3'), (N'159', N'内蒙古莫力县', N'159', N'3'), (N'160', N'内蒙古额尔古纳右旗', N'160', N'3'), (N'161', N'内蒙古额尔古纳左旗', N'161', N'3'), (N'162', N'内蒙古鄂伦春自治旗', N'162', N'3'), (N'163', N'内蒙古鄂温克族自治旗', N'163', N'3'), (N'164', N'内蒙古新巴尔虎右旗', N'164', N'3'), (N'165', N'内蒙古新巴尔虎左旗', N'165', N'3'), (N'166', N'内蒙古陈巴尔虎旗', N'166', N'3'), (N'167', N'内蒙古乌兰浩特市', N'167', N'3'), (N'168', N'内蒙古科尔沁右翼前旗', N'168', N'3'), (N'169', N'内蒙古科尔沁右翼中旗', N'169', N'3'), (N'170', N'内蒙古扎赉特旗', N'170', N'3'), (N'171', N'内蒙古突泉县', N'171', N'3'), (N'172', N'内蒙古通辽市', N'172', N'3'), (N'173', N'内蒙古霍林郭勒市', N'173', N'3'), (N'174', N'内蒙古科尔沁左翼中旗', N'174', N'3'), (N'175', N'内蒙古科尔沁左翼后旗', N'175', N'3'), (N'176', N'内蒙古开鲁县', N'176', N'3'), (N'177', N'内蒙古库伦旗', N'177', N'3'), (N'178', N'内蒙古奈曼旗', N'178', N'3'), (N'179', N'内蒙古扎鲁特旗', N'179', N'3'), (N'180', N'内蒙古二连浩特市', N'180', N'3'), (N'181', N'内蒙古锡林浩特市', N'181', N'3'), (N'182', N'内蒙古阿巴嘎旗', N'182', N'3'), (N'183', N'内蒙古苏尼特左旗', N'183', N'3'), (N'184', N'内蒙古苏尼特右旗', N'184', N'3'), (N'185', N'内蒙古东乌珠穆沁旗', N'185', N'3'), (N'186', N'内蒙古西乌珠穆沁旗', N'186', N'3'), (N'187', N'内蒙古太仆寺旗', N'187', N'3'), (N'188', N'内蒙古镶黄旗', N'188', N'3'), (N'189', N'内蒙古正镶白旗', N'189', N'3'), (N'190', N'内蒙古正蓝旗', N'190', N'3'), (N'191', N'内蒙古多伦县', N'191', N'3'), (N'192', N'内蒙古集宁市', N'192', N'3'), (N'193', N'内蒙古武川县', N'193', N'3'), (N'194', N'内蒙古和林格尔县', N'194', N'3'), (N'195', N'内蒙古清水河县', N'195', N'3'), (N'196', N'内蒙古卓资县', N'196', N'3'), (N'197', N'内蒙古化德县', N'197', N'3'), (N'198', N'内蒙古商都县', N'198', N'3'), (N'199', N'内蒙古兴和县', N'199', N'3'), (N'200', N'内蒙古丰镇县', N'200', N'3'), (N'201', N'内蒙古凉城县', N'201', N'3')
GO

INSERT INTO [dbo].[cm_option] VALUES (N'202', N'内蒙古察哈尔右翼前旗', N'202', N'3'), (N'203', N'内蒙古察哈尔右翼中旗', N'203', N'3'), (N'204', N'内蒙古察哈尔右翼后旗', N'204', N'3'), (N'205', N'内蒙古达尔罕茂明安联', N'205', N'3'), (N'206', N'内蒙古四子王旗', N'206', N'3'), (N'207', N'内蒙古东胜市', N'207', N'3'), (N'208', N'内蒙古达拉特旗', N'208', N'3'), (N'209', N'内蒙古准格尔旗', N'209', N'3'), (N'210', N'内蒙古鄂托克前旗', N'210', N'3'), (N'211', N'内蒙古鄂托克旗', N'211', N'3'), (N'212', N'内蒙古杭锦旗', N'212', N'3'), (N'213', N'内蒙古乌审旗', N'213', N'3'), (N'214', N'内蒙古伊金霍洛旗', N'214', N'3'), (N'215', N'内蒙古临河市', N'215', N'3'), (N'216', N'内蒙古五原县', N'216', N'3'), (N'217', N'内蒙古磴口县', N'217', N'3'), (N'218', N'内蒙古乌拉特前旗', N'218', N'3'), (N'219', N'内蒙古乌拉特中旗', N'219', N'3'), (N'220', N'内蒙古乌拉特后旗', N'220', N'3'), (N'221', N'内蒙古杭锦后旗', N'221', N'3'), (N'222', N'内蒙古阿拉善左旗', N'222', N'3'), (N'223', N'内蒙古阿拉善右旗', N'223', N'3'), (N'224', N'内蒙古额济纳旗', N'224', N'3'), (N'225', N'辽宁省', N'225', N'3'), (N'226', N'辽宁省沈阳市', N'226', N'3'), (N'227', N'辽宁省新民县', N'227', N'3'), (N'228', N'辽宁省辽中县', N'228', N'3'), (N'229', N'辽宁省大连市', N'229', N'3'), (N'230', N'辽宁省新金县', N'230', N'3'), (N'231', N'辽宁省长海县', N'231', N'3'), (N'232', N'辽宁省庄河县', N'232', N'3'), (N'233', N'辽宁省鞍山市', N'233', N'3'), (N'234', N'辽宁省台安县', N'234', N'3'), (N'235', N'辽宁省抚顺市', N'235', N'3'), (N'236', N'辽宁省抚顺县', N'236', N'3'), (N'237', N'辽宁省新宾县', N'237', N'3'), (N'238', N'辽宁省清原县', N'238', N'3'), (N'239', N'辽宁省本溪市', N'239', N'3'), (N'240', N'辽宁省本溪县', N'240', N'3'), (N'241', N'辽宁省桓仁县', N'241', N'3'), (N'242', N'辽宁省丹东市', N'242', N'3'), (N'243', N'辽宁省凤城县', N'243', N'3'), (N'244', N'辽宁省岫岩县', N'244', N'3'), (N'245', N'辽宁省东沟县', N'245', N'3'), (N'246', N'辽宁省宽甸县', N'246', N'3'), (N'247', N'辽宁省锦州市', N'247', N'3'), (N'248', N'辽宁省绥中县', N'248', N'3'), (N'249', N'辽宁省锦  县', N'249', N'3'), (N'250', N'辽宁省北镇县', N'250', N'3'), (N'251', N'辽宁省黑山县', N'251', N'3'), (N'252', N'辽宁省义  县', N'252', N'3'), (N'253', N'辽宁省营口市', N'253', N'3'), (N'254', N'辽宁省营口县', N'254', N'3'), (N'255', N'辽宁省盖  县', N'255', N'3'), (N'256', N'辽宁省阜新市', N'256', N'3'), (N'257', N'辽宁省阜新县', N'257', N'3'), (N'258', N'辽宁省彰武县', N'258', N'3'), (N'259', N'辽宁省辽阳市', N'259', N'3'), (N'260', N'辽宁省辽阳县', N'260', N'3'), (N'261', N'辽宁省灯塔县', N'261', N'3'), (N'262', N'辽宁省盘锦市', N'262', N'3'), (N'263', N'辽宁省大洼县', N'263', N'3'), (N'264', N'辽宁省盘山县', N'264', N'3'), (N'265', N'辽宁省铁岭市', N'265', N'3'), (N'266', N'辽宁省铁岭县', N'266', N'3'), (N'267', N'辽宁省西丰县', N'267', N'3'), (N'268', N'辽宁省昌图县', N'268', N'3'), (N'269', N'辽宁省康平县', N'269', N'3'), (N'270', N'辽宁省法库县', N'270', N'3'), (N'271', N'辽宁省朝阳市', N'271', N'3'), (N'272', N'辽宁省朝阳县', N'272', N'3'), (N'273', N'辽宁省建平县', N'273', N'3'), (N'274', N'辽宁省凌源县', N'274', N'3'), (N'275', N'辽宁省喀喇沁县', N'275', N'3'), (N'276', N'辽宁省建昌县', N'276', N'3'), (N'277', N'辽宁省直辖行政单位', N'277', N'3'), (N'278', N'辽宁省瓦房店市', N'278', N'3'), (N'279', N'辽宁省海城市', N'279', N'3'), (N'280', N'辽宁省锦西市', N'280', N'3'), (N'281', N'辽宁省兴城市', N'281', N'3'), (N'282', N'辽宁省铁法市', N'282', N'3'), (N'283', N'辽宁省北票市', N'283', N'3'), (N'284', N'辽宁省开原市', N'284', N'3'), (N'285', N'吉林省', N'285', N'3'), (N'286', N'吉林省榆树县', N'286', N'3'), (N'287', N'吉林省农安县', N'287', N'3'), (N'288', N'吉林省德惠县', N'288', N'3'), (N'289', N'吉林省双阳县', N'289', N'3'), (N'290', N'吉林省吉林市', N'290', N'3'), (N'291', N'吉林省永吉县', N'291', N'3'), (N'292', N'吉林省舒兰县', N'292', N'3'), (N'293', N'吉林省磐石县', N'293', N'3'), (N'294', N'吉林省蛟河县', N'294', N'3'), (N'295', N'吉林省四平市', N'295', N'3'), (N'296', N'吉林省梨树县', N'296', N'3'), (N'297', N'吉林省伊通县', N'297', N'3'), (N'298', N'吉林省双辽县', N'298', N'3'), (N'299', N'吉林省辽源市', N'299', N'3'), (N'300', N'吉林省东丰县', N'300', N'3'), (N'301', N'吉林省东辽县', N'301', N'3')
GO

INSERT INTO [dbo].[cm_option] VALUES (N'302', N'吉林省通化市', N'302', N'3'), (N'303', N'吉林省通化县', N'303', N'3'), (N'304', N'吉林省辉南县', N'304', N'3'), (N'305', N'吉林省柳河县', N'305', N'3'), (N'306', N'吉林省浑江市', N'306', N'3'), (N'307', N'吉林省抚松县', N'307', N'3'), (N'308', N'吉林省靖宇县', N'308', N'3'), (N'309', N'吉林省长白县', N'309', N'3'), (N'310', N'吉林省白城地区', N'310', N'3'), (N'311', N'吉林省白城市', N'311', N'3'), (N'312', N'吉林省洮南市', N'312', N'3'), (N'313', N'吉林省扶余市', N'313', N'3'), (N'314', N'吉林省大安市', N'314', N'3'), (N'315', N'吉林省长岭县', N'315', N'3'), (N'316', N'吉林省前郭尔罗斯县', N'316', N'3'), (N'317', N'吉林省镇赉县', N'317', N'3'), (N'318', N'吉林省通榆县', N'318', N'3'), (N'319', N'吉林省乾安县', N'319', N'3'), (N'320', N'吉林省延吉市', N'320', N'3'), (N'321', N'吉林省图们市', N'321', N'3'), (N'322', N'吉林省敦化市', N'322', N'3'), (N'323', N'吉林省珲春市', N'323', N'3'), (N'324', N'吉林省龙井市', N'324', N'3'), (N'325', N'吉林省和龙县', N'325', N'3'), (N'326', N'吉林省汪清县', N'326', N'3'), (N'327', N'吉林省安图县', N'327', N'3'), (N'328', N'吉林省公主岭市', N'328', N'3'), (N'329', N'吉林省梅河口市', N'329', N'3'), (N'330', N'吉林省集安市', N'330', N'3'), (N'331', N'吉林省桦甸市', N'331', N'3'), (N'332', N'吉林省九台市', N'332', N'3'), (N'333', N'黑龙江省', N'333', N'3'), (N'334', N'黑龙江哈尔滨市', N'334', N'3'), (N'335', N'黑龙江呼兰县', N'335', N'3'), (N'336', N'黑龙江齐齐哈尔市', N'336', N'3'), (N'337', N'黑龙江龙江县', N'337', N'3'), (N'338', N'黑龙江讷河县', N'338', N'3'), (N'339', N'黑龙江依安县', N'339', N'3'), (N'340', N'黑龙江泰来县', N'340', N'3'), (N'341', N'黑龙江甘南县', N'341', N'3'), (N'342', N'男', N'342', N'4'), (N'343', N'女', N'343', N'4'), (N'344', N'未知', N'344', N'4'), (N'345', N'不明', N'345', N'4'), (N'346', N'string', N'346', N'5'), (N'347', N'int', N'347', N'5'), (N'348', N'double', N'348', N'5'), (N'349', N'DateTime', N'349', N'5'), (N'350', N'Date', N'350', N'5'), (N'351', N'bool', N'351', N'5'), (N'456661440205443072', N'1', N'1023', N'456659310463700992'), (N'456662703420755968', N'2', N'1026', N'456659310463700992')
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
INSERT INTO [dbo].[cm_option_group] VALUES (N'1', N'民族'), (N'2', N'学历'), (N'3', N'地区'), (N'4', N'性别'), (N'5', N'数据类型'), (N'456659310463700992', N'新组')
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
-- Table structure for cm_permission
-- ----------------------------
CREATE TABLE [dbo].[cm_permission] (
  [id] bigint NOT NULL,
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
INSERT INTO [dbo].[cm_permission] VALUES (N'1', N'公共文件管理', N'禁止删除'), (N'2', N'素材库管理', N'禁止删除'), (N'455253883184238592', N'测试1', N'')
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
INSERT INTO [dbo].[cm_role] VALUES (N'1', N'任何人', N'所有用户默认都具有该角色，不可删除'), (N'2', N'系统管理员', N'系统角色，不可删除'), (N'22844822693027840', N'收发员', N''), (N'152695933758603264', N'市场经理', N''), (N'152696004814307328', N'综合经理', N''), (N'152696042718232576', N'财务经理', N'')
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
INSERT INTO [dbo].[cm_role_menu] VALUES (N'2', N'2'), (N'2', N'3'), (N'2', N'4'), (N'2', N'5'), (N'2', N'6'), (N'1', N'7'), (N'1', N'8'), (N'1', N'9'), (N'2', N'10'), (N'1', N'15315637929975808'), (N'2', N'18860286065975296'), (N'22844822693027840', N'154430055023640576')
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
INSERT INTO [dbo].[cm_role_per] VALUES (N'1', N'1'), (N'1', N'2'), (N'22844822693027840', N'455253883184238592'), (N'152696004814307328', N'455253883184238592')
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
-- Records of cm_rpt
-- ----------------------------
INSERT INTO [dbo].[cm_rpt] VALUES (N'139241259579338752', N'测试报表111', N'<Rpt cols="80,80,80,80,80,80,80">
  <Params>
    <Param name="新参数1"><![CDATA[<a:CText Title="标题1" />]]></Param>
    <Param name="新参数2"><![CDATA[<a:CText Title="标题2" />]]></Param>
  </Params>
  <Data />
  <Page />
  <Header />
  <Body rows="30,30,30,30,30,30,30,30,30,30">
    <Text row="4" col="6" val="文本" lbs="None" tbs="None" rbs="None" bbs="None" />
    <Text row="7" col="6" rowspan="3" val="文本" lbs="None" tbs="None" rbs="None" bbs="None" />
  </Body>
  <Footer />
  <View />
</Rpt>', N'新增测试1', N'2020-10-19 13:35:10', N'2023-06-28 08:39:08'), (N'139540400075304960', N'abc1', N'<Rpt cols="80,80,80,80,80">
  <Params />
  <Data />
  <Page />
  <Header />
  <Body rows="30,30,30,30,30,30,30,30,30,30,30,30,30">
    <Text row="2" col="2" val="文本" lbs="None" tbs="None" rbs="None" bbs="None" />
    <Text row="4" col="3" colspan="2" val="文本" lbs="None" tbs="None" rbs="None" bbs="None" />
    <Text row="7" col="3" val="文本" lbs="None" tbs="None" rbs="None" bbs="None" />
    <Text row="12" col="4" val="文本" lbs="None" tbs="None" rbs="None" bbs="None" />
  </Body>
  <Footer />
  <View />
</Rpt>', N'阿斯顿法定', N'2020-10-20 09:24:01', N'2023-03-13 16:14:41'), (N'150118388697264128', N'abc12', N'', N'', N'2020-11-18 13:57:21', N'2020-11-18 13:57:21'), (N'154424288497369088', N'新报表abc', N'', N'', N'2020-11-30 11:07:07', N'2020-11-30 11:07:07'), (N'259588273038290944', N'新报表3', N'', N'', N'2021-09-16 15:51:31', N'2021-09-16 15:51:53')
GO


-- ----------------------------
-- Table structure for cm_user
-- ----------------------------
CREATE TABLE [dbo].[cm_user] (
  [id] bigint NOT NULL,
  [phone] char(11) NOT NULL,
  [name] nvarchar(32) NOT NULL,
  [pwd] char(32) NOT NULL,
  [sex] tinyint NOT NULL,
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
'MS_Description', N'姓名',
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
'MS_Description', N'#Gender#性别',
'SCHEMA', N'dbo',
'TABLE', N'cm_user',
'COLUMN', N'sex'
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
INSERT INTO [dbo].[cm_user] VALUES (N'1', N'13511111111', N'Windows', N'af3303f852abeccd793068486a391626', N'1', N'[["photo/1.jpg","1","300 x 300 (.jpg)",49179,"daoting","2020-03-13 10:37"]]', N'0', N'2019-10-24 09:06:38', N'2023-03-16 08:35:39'), (N'2', N'13522222222', N'安卓', N'b59c67bf196a4758191e42f76670ceba', N'2', N'[["photo/2.jpg","2","300 x 300 (.jpg)",49179,"daoting","2020-03-13 10:37"]]', N'0', N'2019-10-24 13:03:19', N'2023-03-16 08:36:23'), (N'3', N'13533333333', N'苹果', N'674f3c2c1a8a6f90461e8a66fb5550ba', N'1', N'[["photo/3.jpg","3","300 x 300 (.jpg)",49179,"daoting","2020-03-13 10:37"]]', N'0', N'2020-11-19 13:17:25', N'2023-03-16 08:36:46'), (N'149709966847897600', N'13122222222', N'李市场', N'934b535800b1cba8f96a5d72f72f1611', N'1', N'', N'0', N'2020-11-17 10:54:29', N'2020-11-25 16:37:55'), (N'152695627289198592', N'13211111111', N'王综合', N'b59c67bf196a4758191e42f76670ceba', N'1', N'', N'0', N'2020-11-25 16:38:34', N'2020-11-25 16:38:34'), (N'152695790787362816', N'13866666666', N'张财务', N'e9510081ac30ffa83f10b68cde1cac07', N'1', N'', N'0', N'2020-11-25 16:38:54', N'2020-11-25 16:38:54'), (N'184215437633777664', N'15955555555', N'15955555555', N'6074c6aa3488f3c2dddff2a7ca821aab', N'1', N'', N'0', N'2021-02-20 16:06:23', N'2021-02-20 16:06:23'), (N'185188338092601344', N'15912345678', N'15912345678', N'674f3c2c1a8a6f90461e8a66fb5550ba', N'1', N'', N'0', N'2021-02-23 08:32:20', N'2021-02-23 08:32:20'), (N'185212597401677824', N'15912345671', N'15912345677', N'cca8f108b55ec9e39d7885e24f7da0af', N'2', N'', N'0', N'2021-02-23 10:08:43', N'2022-01-19 15:49:43'), (N'192818293676994560', N'18543175028', N'18543175028', N'bf8dd8c68d02e161c28dc9ea139d4784', N'1', N'', N'0', N'2021-03-16 09:51:02', N'2021-03-16 09:51:02'), (N'196167762048839680', N'18843175028', N'18843175028', N'bf8dd8c68d02e161c28dc9ea139d4784', N'1', N'', N'0', N'2021-03-25 15:40:38', N'2021-03-25 15:40:38'), (N'224062063923556352', N'14411111111', N'14411111111', N'b59c67bf196a4758191e42f76670ceba', N'1', N'', N'0', N'2021-06-10 15:02:39', N'2021-06-10 15:02:39'), (N'227949556179791872', N'13612345678', N'WebAssembly', N'674f3c2c1a8a6f90461e8a66fb5550ba', N'1', N'', N'0', N'2021-06-21 08:30:10', N'2021-06-21 08:30:34'), (N'229519641138819072', N'13311111111', N'13311111111', N'b59c67bf196a4758191e42f76670ceba', N'1', N'[["editor/E3/18/452737920958222336.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2023-03-03 15:38"]]', N'0', N'2021-06-25 16:29:06', N'2021-06-25 16:29:06'), (N'231620526086156288', N'13611111111', N'13611111111', N'b59c67bf196a4758191e42f76670ceba', N'1', N'', N'0', N'2021-07-01 11:37:18', N'2021-07-01 11:37:18'), (N'247170018466197504', N'15948341897', N'15948341892', N'af3303f852abeccd793068486a391626', N'1', N'', N'0', N'2021-08-13 09:25:26', N'2021-09-10 09:36:37')
GO


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
-- Records of cm_user_group
-- ----------------------------
INSERT INTO [dbo].[cm_user_group] VALUES (N'1', N'454483802783240192'), (N'1', N'454484924033945600'), (N'149709966847897600', N'454484847190102016')
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
-- Records of cm_user_params
-- ----------------------------
INSERT INTO [dbo].[cm_user_params] VALUES (N'2', N'1', N'false', N'2020-12-04 13:29:05')
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
INSERT INTO [dbo].[cm_user_role] VALUES (N'1', N'2'), (N'1', N'22844822693027840'), (N'1', N'152695933758603264'), (N'1', N'152696004814307328'), (N'2', N'2'), (N'2', N'22844822693027840'), (N'2', N'152695933758603264'), (N'3', N'2'), (N'149709966847897600', N'2'), (N'149709966847897600', N'152695933758603264'), (N'152695627289198592', N'152696004814307328'), (N'152695790787362816', N'152696042718232576'), (N'247170018466197504', N'22844822693027840')
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
-- Records of cm_wfd_atv
-- ----------------------------
INSERT INTO [dbo].[cm_wfd_atv] VALUES (N'146898715155492864', N'146898695127691264', N'开始', N'1', N'0', N'0', NULL, N'1', N'1', N'0', N'0', N'0', N'0', N'2020-11-09 16:43:10', N'2020-11-09 16:43:10'), (N'146898876447453184', N'146898695127691264', N'任务项', N'0', N'0', N'0', NULL, N'1', N'0', N'0', N'0', N'0', N'0', N'2020-11-09 16:43:48', N'2020-11-09 16:43:48'), (N'146900570585559040', N'146900552231284736', N'开始', N'1', N'0', N'0', NULL, N'1', N'1', N'0', N'0', N'0', N'0', N'2020-11-09 16:50:32', N'2020-11-09 16:50:32'), (N'146900847761944576', N'146900823984435200', N'开始', N'1', N'0', N'0', NULL, N'1', N'1', N'0', N'0', N'0', N'0', N'2020-11-09 16:51:38', N'2020-11-09 16:51:38'), (N'146901433265811456', N'146901403339452416', N'开始', N'1', N'0', N'0', NULL, N'1', N'1', N'0', N'0', N'0', N'0', N'2020-11-09 16:53:58', N'2020-11-09 16:53:58'), (N'147141181158846464', N'147141147767992320', N'开始', N'1', N'0', N'0', NULL, N'1', N'1', N'0', N'0', N'0', N'0', N'2020-11-10 08:46:31', N'2020-11-10 08:46:31'), (N'147141718000398336', N'147141147767992320', N'任务项', N'0', N'0', N'0', NULL, N'1', N'0', N'0', N'0', N'0', N'0', N'2020-11-10 08:48:39', N'2020-11-10 08:48:39'), (N'152588671081775104', N'152588581545967616', N'接收文件', N'1', N'0', N'0', NULL, N'1', N'1', N'0', N'0', N'0', N'0', N'2020-11-25 09:32:55', N'2020-12-09 10:45:33'), (N'152683112727576576', N'152588581545967616', N'市场部', N'0', N'0', N'0', NULL, N'1', N'0', N'0', N'0', N'2', N'0', N'2020-11-25 15:48:12', N'2020-12-14 15:36:36'), (N'152684512937246720', N'152588581545967616', N'综合部', N'0', N'2', N'0', NULL, N'1', N'0', N'0', N'0', N'2', N'0', N'2020-11-25 15:53:46', N'2020-12-14 15:33:30'), (N'152684758027206656', N'152588581545967616', N'市场部传阅', N'0', N'0', N'0', NULL, N'1', N'0', N'0', N'0', N'0', N'0', N'2020-11-25 15:54:44', N'2020-11-25 15:56:10'), (N'152684895835258880', N'152588581545967616', N'同步', N'2', N'0', N'0', NULL, N'1', N'0', N'0', N'0', N'0', N'2', N'2020-11-25 15:55:17', N'2020-12-16 08:39:31'), (N'152685032993193984', N'152588581545967616', N'综合部传阅', N'0', N'0', N'0', NULL, N'1', N'0', N'0', N'0', N'0', N'0', N'2020-11-25 15:55:50', N'2020-11-25 15:56:10'), (N'152685491275431936', N'152588581545967616', N'返回收文人', N'0', N'0', N'0', NULL, N'1', N'0', N'0', N'0', N'0', N'0', N'2020-11-25 15:57:39', N'2020-11-25 15:58:18'), (N'152685608543977472', N'152588581545967616', N'完成', N'3', N'0', N'0', NULL, N'1', N'0', N'0', N'0', N'0', N'0', N'2020-11-25 15:58:07', N'2020-11-25 15:58:07')
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
-- Records of cm_wfd_atv_role
-- ----------------------------
INSERT INTO [dbo].[cm_wfd_atv_role] VALUES (N'146898715155492864', N'1'), (N'146900570585559040', N'1'), (N'146900847761944576', N'1'), (N'146901433265811456', N'1'), (N'146898715155492864', N'2'), (N'146900570585559040', N'2'), (N'146901433265811456', N'2'), (N'152588671081775104', N'22844822693027840'), (N'152684758027206656', N'22844822693027840'), (N'152685032993193984', N'22844822693027840'), (N'152685491275431936', N'22844822693027840'), (N'152683112727576576', N'152695933758603264'), (N'152684512937246720', N'152696004814307328')
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
-- Records of cm_wfd_prc
-- ----------------------------
INSERT INTO [dbo].[cm_wfd_prc] VALUES (N'146898695127691264', N'555', N'<Sketch><Node id="146898715155492864" title="开始" shape="开始" left="340" top="100" width="80" height="60" /><Node id="146898876447453184" title="任务项" shape="任务" left="340" top="360" width="120" height="60" /><Line id="146898896794021888" headerid="146898715155492864" bounds="380,160,30,200" headerport="4" tailid="146898876447453184" tailport="0" /></Sketch>', N'0', N'0', N'', N'1', N'2020-11-19 13:17:25', N'2020-11-19 13:17:25'), (N'146900552231284736', N'666', N'<Sketch><Node id="146900570585559040" title="开始" shape="开始" left="620" top="120" width="80" height="60" /></Sketch>', N'0', N'0', N'', N'3', N'2020-11-19 13:17:25', N'2020-11-09 16:50:56'), (N'146900823984435200', N'777', N'<Sketch><Node id="146900847761944576" title="开始" shape="开始" left="300" top="220" width="80" height="60" /></Sketch>', N'0', N'0', N'', N'4', N'2020-11-19 13:17:25', N'2020-11-09 16:52:58'), (N'146901403339452416', N'888', N'<Sketch><Node id="146901433265811456" title="开始" shape="开始" left="340" top="140" width="80" height="60" /></Sketch>', N'0', N'0', N'', N'6', N'2020-11-19 13:17:25', N'2020-11-09 16:54:39'), (N'147141147767992320', N'ggg', N'<Sketch><Node id="147141181158846464" title="开始" shape="开始" left="320" top="40" width="80" height="60" /><Node id="147141718000398336" title="任务项" shape="任务" left="380" top="480" width="120" height="60" /><Line id="147141749642227712" headerid="147141181158846464" bounds="400,100,50,380" headerport="3" tailid="147141718000398336" tailport="0" /></Sketch>', N'1', N'0', N'', N'2', N'2020-11-10 08:46:24', N'2020-11-10 08:50:03'), (N'152588581545967616', N'收文样例', N'<Sketch><Node id="152588671081775104" title="接收文件" shape="开始" left="300" top="40" width="80" height="60" /><Node id="152683112727576576" title="市场部" shape="任务" left="160" top="140" width="120" height="60" /><Line id="152683122982649856" headerid="152588671081775104" bounds="210,70,50,70" headerport="6" tailid="152683112727576576" tailport="0" /><Node id="152684512937246720" title="综合部" shape="任务" left="400" top="140" width="120" height="60" /><Line id="152684673721696256" headerid="152588671081775104" bounds="380,70,90,70" headerport="2" tailid="152684512937246720" tailport="0" /><Node id="152684758027206656" title="市场部传阅" shape="任务" left="160" top="260" width="120" height="60" /><Node id="152684895835258880" title="同步" shape="同步" background="#FF9D9D9D" borderbrush="#FF969696" left="280" top="400" width="120" height="60" /><Line id="152684951493672960" headerid="152683112727576576" bounds="210,200,20,60" headerport="4" tailid="152684758027206656" tailport="0" /><Line id="152684981348728832" headerid="152683112727576576" bounds="120,170,160,470" headerport="6" tailid="152685608543977472" tailport="6" /><Node id="152685032993193984" title="综合部传阅" shape="任务" left="400" top="260" width="120" height="60" /><Line id="152685133509689344" headerid="152684512937246720" bounds="450,200,20,60" headerport="4" tailid="152685032993193984" tailport="0" /><Line id="152685169891082240" headerid="152684512937246720" bounds="400,170,160,270" headerport="2" tailid="152684895835258880" tailport="2" /><Line id="152685211767013376" headerid="152684758027206656" bounds="220,320,60,120" headerport="4" tailid="152684895835258880" tailport="6" /><Line id="152685247745753088" headerid="152685032993193984" bounds="400,320,60,120" headerport="4" tailid="152684895835258880" tailport="2" /><Node id="152685491275431936" title="返回收文人" shape="任务" left="280" top="500" width="120" height="60" /><Line id="152685585135566848" headerid="152684895835258880" bounds="330,460,20,40" headerport="4" tailid="152685491275431936" tailport="0" /><Node id="152685608543977472" title="完成" shape="结束" background="#FF9D9D9D" borderbrush="#FF969696" left="300" top="600" width="80" height="60" /><Line id="152685622099968000" headerid="152685491275431936" bounds="330,560,20,40" headerport="4" tailid="152685608543977472" tailport="0" /></Sketch>', N'0', N'0', N'', N'5', N'2020-11-25 09:32:33', N'2021-08-24 15:45:54')
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
-- Records of cm_wfd_trs
-- ----------------------------
INSERT INTO [dbo].[cm_wfd_trs] VALUES (N'146898896794021888', N'146898695127691264', N'146898715155492864', N'146898876447453184', N'0', NULL), (N'147141749642227712', N'147141147767992320', N'147141181158846464', N'147141718000398336', N'0', NULL), (N'152683122982649856', N'152588581545967616', N'152588671081775104', N'152683112727576576', N'0', NULL), (N'152684673721696256', N'152588581545967616', N'152588671081775104', N'152684512937246720', N'0', NULL), (N'152684951493672960', N'152588581545967616', N'152683112727576576', N'152684758027206656', N'0', NULL), (N'152684981348728832', N'152588581545967616', N'152683112727576576', N'152685608543977472', N'0', NULL), (N'152685133509689344', N'152588581545967616', N'152684512937246720', N'152685032993193984', N'0', NULL), (N'152685169891082240', N'152588581545967616', N'152684512937246720', N'152684895835258880', N'0', NULL), (N'152685211767013376', N'152588581545967616', N'152684758027206656', N'152684895835258880', N'0', NULL), (N'152685247745753088', N'152588581545967616', N'152685032993193984', N'152684895835258880', N'0', NULL), (N'152685585135566848', N'152588581545967616', N'152684895835258880', N'152685491275431936', N'0', NULL), (N'152685622099968000', N'152588581545967616', N'152685491275431936', N'152685608543977472', N'0', NULL), (N'160910207789953024', N'152588581545967616', N'152683112727576576', N'152588671081775104', N'1', N'152683122982649856')
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
-- Records of cm_wfi_atv
-- ----------------------------
INSERT INTO [dbo].[cm_wfi_atv] VALUES (N'162025231375790080', N'162025231350624256', N'152588671081775104', N'1', N'1', N'2020-12-21 10:30:29', N'2020-12-21 10:30:31'), (N'162025255044247552', N'162025231350624256', N'152683112727576576', N'1', N'1', N'2020-12-21 10:30:31', N'2020-12-21 16:45:05'), (N'162119526644576256', N'162025231350624256', N'152684758027206656', N'1', N'1', N'2020-12-21 16:45:05', N'2020-12-21 16:45:11'), (N'162119548043915264', N'162025231350624256', N'152684895835258880', N'3', N'1', N'2020-12-21 16:45:11', N'2020-12-21 16:45:11'), (N'162119548199104512', N'162025231350624256', N'152685491275431936', N'1', N'1', N'2020-12-21 16:45:11', N'2020-12-21 16:45:13'), (N'162401333625614336', N'162401333600448512', N'152588671081775104', N'1', N'1', N'2020-12-22 11:25:22', N'2023-03-16 10:42:58'), (N'457374494836674560', N'162401333600448512', N'152683112727576576', N'1', N'1', N'2023-03-16 10:42:57', N'2023-03-16 11:10:31'), (N'457374495587454976', N'162401333600448512', N'152684512937246720', N'0', N'1', N'2023-03-16 10:42:57', N'2023-03-16 10:42:57'), (N'457381430491631616', N'162401333600448512', N'152684758027206656', N'0', N'1', N'2023-03-16 11:10:31', N'2023-03-16 11:10:31'), (N'457384397022187520', N'457384396879581184', N'152588671081775104', N'1', N'1', N'2023-03-16 11:22:27', N'2023-03-16 11:23:30'), (N'457384696747151360', N'457384396879581184', N'152683112727576576', N'1', N'1', N'2023-03-16 11:23:29', N'2023-03-16 11:27:51'), (N'457384697418240000', N'457384396879581184', N'152684512937246720', N'1', N'1', N'2023-03-16 11:23:29', N'2023-03-16 11:28:13'), (N'457385791041064960', N'457384396879581184', N'152684758027206656', N'0', N'2', N'2023-03-16 11:27:50', N'2023-03-16 11:27:50'), (N'457385885710700544', N'457384396879581184', N'152685032993193984', N'0', N'1', N'2023-03-16 11:28:13', N'2023-03-16 11:28:13'), (N'457388173628035072', N'457388173615452160', N'152588671081775104', N'1', N'1', N'2023-03-16 11:37:33', N'2023-03-16 11:38:10'), (N'457388387768225792', N'457388173615452160', N'152683112727576576', N'1', N'1', N'2023-03-16 11:38:10', N'2023-03-16 11:38:50'), (N'457388561571794944', N'457388173615452160', N'152684758027206656', N'0', N'1', N'2023-03-16 11:38:49', N'2023-03-16 11:38:49')
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
-- Records of cm_wfi_item
-- ----------------------------
INSERT INTO [dbo].[cm_wfi_item] VALUES (N'162025231392567296', N'162025231375790080', N'1', N'1', N'daoting', N'2020-12-21 10:30:29', N'1', N'2020-12-21 10:30:29', NULL, N'1', N'', N'157', N'2020-12-21 10:30:29', N'2020-12-21 10:30:31'), (N'162025255065219072', N'162025255044247552', N'1', N'0', N'daoting', N'2020-12-21 10:30:31', N'1', N'2020-12-21 13:27:15', NULL, N'1', N'', N'158', N'2020-12-21 10:30:31', N'2020-12-21 16:45:05'), (N'162119526686519296', N'162119526644576256', N'1', N'0', N'daoting', N'2020-12-21 16:45:05', N'1', N'2020-12-21 16:45:07', NULL, N'1', N'', N'159', N'2020-12-21 16:45:05', N'2020-12-21 16:45:11'), (N'162119548064886784', N'162119548043915264', N'3', N'0', N'daoting', N'2020-12-21 16:45:11', N'0', NULL, NULL, N'1', N'', N'160', N'2020-12-21 16:45:11', N'2020-12-21 16:45:11'), (N'162119548220076032', N'162119548199104512', N'1', N'0', N'daoting', N'2020-12-21 16:45:11', N'1', N'2020-12-21 16:45:12', NULL, N'1', N'', N'161', N'2020-12-21 16:45:11', N'2020-12-21 16:45:13'), (N'162401333642391552', N'162401333625614336', N'1', N'1', N'daoting', N'2020-12-22 11:25:22', N'1', N'2020-12-22 11:25:22', NULL, N'1', N'', N'162', N'2020-12-22 11:25:22', N'2023-03-16 10:42:58'), (N'457374495021223936', N'457374494836674560', N'1', N'0', N'', N'2023-03-16 10:42:57', N'1', N'2023-03-16 10:43:13', NULL, N'1', N'', N'163', N'2023-03-16 10:42:57', N'2023-03-16 11:10:31'), (N'457374495696506880', N'457374495587454976', N'0', N'0', N'', N'2023-03-16 10:42:57', N'0', NULL, NULL, N'152695627289198592', N'', N'164', N'2023-03-16 10:42:57', N'2023-03-16 10:42:57'), (N'457381430646820864', N'457381430491631616', N'0', N'0', N'', N'2023-03-16 11:10:31', N'1', N'2023-03-16 11:11:00', NULL, N'1', N'', N'165', N'2023-03-16 11:10:31', N'2023-03-16 11:10:31'), (N'457384397164793856', N'457384397022187520', N'1', N'1', N'Windows', N'2023-03-16 11:22:27', N'1', N'2023-03-16 11:22:27', NULL, N'1', N'', N'167', N'2023-03-16 11:22:27', N'2023-03-16 11:23:30'), (N'457384696902340608', N'457384696747151360', N'1', N'0', N'', N'2023-03-16 11:23:29', N'1', N'2023-03-16 11:23:45', NULL, N'1', N'', N'168', N'2023-03-16 11:23:29', N'2023-03-16 11:27:51'), (N'457384697523097600', N'457384697418240000', N'1', N'0', N'', N'2023-03-16 11:23:29', N'1', N'2023-03-16 11:23:46', NULL, N'1', N'', N'169', N'2023-03-16 11:23:29', N'2023-03-16 11:28:13'), (N'457385791196254208', N'457385791041064960', N'1', N'0', N'', N'2023-03-16 11:27:50', N'1', N'2023-03-16 11:28:02', NULL, N'1', N'', N'170', N'2023-03-16 11:27:50', N'2023-03-16 11:28:25'), (N'457385791531798528', N'457385791041064960', N'0', N'0', N'', N'2023-03-16 11:27:50', N'0', NULL, NULL, N'247170018466197504', N'', N'171', N'2023-03-16 11:27:50', N'2023-03-16 11:27:50'), (N'457385885811363840', N'457385885710700544', N'0', N'0', N'', N'2023-03-16 11:28:13', N'0', NULL, NULL, N'2', N'', N'172', N'2023-03-16 11:28:13', N'2023-03-16 11:28:13'), (N'457388173640617984', N'457388173628035072', N'1', N'1', N'Windows', N'2023-03-16 11:37:33', N'1', N'2023-03-16 11:37:33', NULL, N'1', N'', N'174', N'2023-03-16 11:37:33', N'2023-03-16 11:38:10'), (N'457388387776614400', N'457388387768225792', N'1', N'0', N'', N'2023-03-16 11:38:10', N'1', N'2023-03-16 11:38:22', NULL, N'2', N'', N'175', N'2023-03-16 11:38:10', N'2023-03-16 11:38:50'), (N'457388561714401280', N'457388561571794944', N'0', N'0', N'', N'2023-03-16 11:38:49', N'0', NULL, NULL, N'1', N'', N'176', N'2023-03-16 11:38:49', N'2023-03-16 11:38:49')
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
-- Records of cm_wfi_prc
-- ----------------------------
INSERT INTO [dbo].[cm_wfi_prc] VALUES (N'162025231350624256', N'152588581545967616', N'a', N'1', N'58', N'2020-12-21 10:30:29', N'2020-12-21 16:45:13'), (N'162401333600448512', N'152588581545967616', N'关于新冠疫情的批示', N'0', N'59', N'2020-12-22 11:25:22', N'2020-12-22 11:25:22'), (N'457384396879581184', N'152588581545967616', N'阿斯蒂芬', N'0', N'64', N'2023-03-16 11:22:27', N'2023-03-16 11:22:27'), (N'457388173615452160', N'152588581545967616', N'疫情在', N'0', N'65', N'2023-03-16 11:37:33', N'2023-03-16 11:37:33')
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
-- Records of cm_wfi_trs
-- ----------------------------
INSERT INTO [dbo].[cm_wfi_trs] VALUES (N'162025255165882368', N'152683122982649856', N'162025231375790080', N'162025255044247552', N'0', N'2020-12-21 10:30:31'), (N'162119526820737024', N'152684951493672960', N'162025255044247552', N'162119526644576256', N'0', N'2020-12-21 16:45:05'), (N'162119548186521600', N'152685211767013376', N'162119526644576256', N'162119548043915264', N'0', N'2020-12-21 16:45:11'), (N'162119548320739328', N'152685585135566848', N'162119548043915264', N'162119548199104512', N'0', N'2020-12-21 16:45:11'), (N'457374495470014464', N'152683122982649856', N'162401333625614336', N'457374494836674560', N'0', N'2023-03-16 10:42:57'), (N'457374496069799936', N'152684673721696256', N'162401333625614336', N'457374495587454976', N'0', N'2023-03-16 10:42:57'), (N'457381431104000000', N'152684951493672960', N'457374494836674560', N'457381430491631616', N'0', N'2023-03-16 11:10:31'), (N'457384697296605184', N'152683122982649856', N'457384397022187520', N'457384696747151360', N'0', N'2023-03-16 11:23:29'), (N'457384697883807744', N'152684673721696256', N'457384397022187520', N'457384697418240000', N'0', N'2023-03-16 11:23:29'), (N'457385791921868800', N'152684951493672960', N'457384696747151360', N'457385791041064960', N'0', N'2023-03-16 11:27:50'), (N'457385886172073984', N'152685133509689344', N'457384697418240000', N'457385885710700544', N'0', N'2023-03-16 11:28:13'), (N'457388387831140352', N'152683122982649856', N'457388173628035072', N'457388387768225792', N'0', N'2023-03-16 11:38:10'), (N'457388562041556992', N'152684951493672960', N'457388387768225792', N'457388561571794944', N'0', N'2023-03-16 11:38:49')
GO


-- ----------------------------
-- Table structure for demo_cache_tbl1
-- ----------------------------
CREATE TABLE [dbo].[demo_cache_tbl1] (
  [id] bigint NOT NULL,
  [phone] nvarchar(255) NOT NULL,
  [name] nvarchar(255) NOT NULL
)
GO


-- ----------------------------
-- Records of demo_cache_tbl1
-- ----------------------------
INSERT INTO [dbo].[demo_cache_tbl1] VALUES (N'454454068519129088', N'ca4f271212bc4add946c55feed7400bb', N'3917'), (N'484620968746045440', N'3f435d84c76a46e29002f467a4cd0187', N'7425'), (N'484621133057904640', N'3329d521b2134b0195083828152cb5b0', N'1786'), (N'484624179913576448', N'd80e785d1d44472abe88723e4ed17ca8', N'156')
GO


-- ----------------------------
-- Table structure for demo_child_tbl1
-- ----------------------------
CREATE TABLE [dbo].[demo_child_tbl1] (
  [id] bigint NOT NULL,
  [parent_id] bigint NOT NULL,
  [item_name] nvarchar(255) NOT NULL
)
GO


-- ----------------------------
-- Records of demo_child_tbl1
-- ----------------------------
INSERT INTO [dbo].[demo_child_tbl1] VALUES (N'443588385740705792', N'443588385522601984', N'修改370'), (N'443588388055961600', N'443588385522601984', N'修改370'), (N'443588388299231232', N'443588385522601984', N'修改370'), (N'443588583695077376', N'443588583535693824', N'新增0'), (N'443588583913181184', N'443588583535693824', N'新增1'), (N'443588584148062208', N'443588583535693824', N'新增2'), (N'443588895562551296', N'443588895352836096', N'新增0'), (N'443588895814209536', N'443588895352836096', N'新增1'), (N'443588896132976640', N'443588895352836096', N'新增2'), (N'443588932807970816', N'443588932694724608', N'新增0'), (N'443588933026074624', N'443588932694724608', N'新增1'), (N'443588933248372736', N'443588932694724608', N'新增2'), (N'445140374660337664', N'445140374589034496', N'新增0'), (N'445140374786166784', N'445140374589034496', N'新增1'), (N'446130095746207744', N'446130095742013440', N'新增0'), (N'446130095754596352', N'446130095742013440', N'新增1'), (N'484622270955802624', N'484622270804807680', N'新增0'), (N'484622271224238080', N'484622270804807680', N'新增1'), (N'484622408784826368', N'484622408633831424', N'新增0'), (N'484622408994541568', N'484622408633831424', N'新增1'), (N'484623850744598528', N'484623850568437760', N'新增0'), (N'484623850987868160', N'484623850568437760', N'新增1'), (N'484623946806743040', N'484623946693496832', N'新增0'), (N'484623947016458240', N'484623946693496832', N'新增1')
GO


-- ----------------------------
-- Table structure for demo_child_tbl2
-- ----------------------------
CREATE TABLE [dbo].[demo_child_tbl2] (
  [id] bigint NOT NULL,
  [group_id] bigint NOT NULL,
  [item_name] nvarchar(255) NOT NULL
)
GO


-- ----------------------------
-- Records of demo_child_tbl2
-- ----------------------------
INSERT INTO [dbo].[demo_child_tbl2] VALUES (N'443588388416671744', N'443588385522601984', N'修改975'), (N'443588583799934976', N'443588583535693824', N'新增0'), (N'443588584039010304', N'443588583535693824', N'新增1'), (N'443588584374554624', N'443588583535693824', N'新增2'), (N'443588895692574720', N'443588895352836096', N'新增0'), (N'443588895931650048', N'443588895352836096', N'新增1'), (N'443588896258805760', N'443588895352836096', N'新增2'), (N'443588932917022720', N'443588932694724608', N'新增0'), (N'443588933135126528', N'443588932694724608', N'新增1'), (N'443588933361618944', N'443588932694724608', N'新增2'), (N'445140374735835136', N'445140374589034496', N'新增0'), (N'445140374819721216', N'445140374589034496', N'新增1'), (N'446130095750402048', N'446130095742013440', N'新增0'), (N'446130095754596353', N'446130095742013440', N'新增1'), (N'484622271115186176', N'484622270804807680', N'新增0'), (N'484622271333289984', N'484622270804807680', N'新增1'), (N'484622408889683968', N'484622408633831424', N'新增0'), (N'484622409107787776', N'484622408633831424', N'新增1'), (N'484623850878816256', N'484623850568437760', N'新增0'), (N'484623851092725760', N'484623850568437760', N'新增1'), (N'484623946907406336', N'484623946693496832', N'新增0'), (N'484623947121315840', N'484623946693496832', N'新增1')
GO


-- ----------------------------
-- Table structure for demo_crud
-- ----------------------------
CREATE TABLE [dbo].[demo_crud] (
  [id] bigint NOT NULL,
  [name] nvarchar(255) NOT NULL,
  [dispidx] int NOT NULL,
  [mtime] datetime NOT NULL,
  [enable_insert_event] bit NOT NULL,
  [enable_name_changed_event] bit NOT NULL,
  [enable_del_event] bit NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'标识',
'SCHEMA', N'dbo',
'TABLE', N'demo_crud',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'名称',
'SCHEMA', N'dbo',
'TABLE', N'demo_crud',
'COLUMN', N'name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'显示顺序',
'SCHEMA', N'dbo',
'TABLE', N'demo_crud',
'COLUMN', N'dispidx'
GO

EXEC sp_addextendedproperty
'MS_Description', N'最后修改时间',
'SCHEMA', N'dbo',
'TABLE', N'demo_crud',
'COLUMN', N'mtime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时允许发布插入事件',
'SCHEMA', N'dbo',
'TABLE', N'demo_crud',
'COLUMN', N'enable_insert_event'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时允许发布Name变化事件',
'SCHEMA', N'dbo',
'TABLE', N'demo_crud',
'COLUMN', N'enable_name_changed_event'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时允许发布删除事件',
'SCHEMA', N'dbo',
'TABLE', N'demo_crud',
'COLUMN', N'enable_del_event'
GO

EXEC sp_addextendedproperty
'MS_Description', N'#demo#基础增删改',
'SCHEMA', N'dbo',
'TABLE', N'demo_crud'
GO


-- ----------------------------
-- Records of demo_crud
-- ----------------------------
INSERT INTO [dbo].[demo_crud] VALUES (N'446127712370708480', N'批增更944', N'50', N'2023-02-13 09:52:21', N'0', N'0', N'0'), (N'446127712387485696', N'批量605', N'51', N'2023-02-13 09:52:21', N'0', N'0', N'0'), (N'446127744155144192', N'批增更887', N'52', N'2023-02-13 09:52:28', N'0', N'0', N'0'), (N'446127778095452160', N'批增更删501', N'53', N'2023-02-13 09:52:36', N'0', N'0', N'0'), (N'446127928557719552', N'新增事件9083', N'54', N'2023-02-13 09:53:12', N'1', N'0', N'0'), (N'447641397090078720', N'领域服务', N'61', N'2023-02-17 14:07:07', N'0', N'0', N'0'), (N'447641397589200896', N'服务更', N'62', N'2023-02-17 14:07:08', N'0', N'0', N'0'), (N'484620702760062976', N'单个9897', N'63', N'2023-05-30 15:09:40', N'0', N'0', N'0'), (N'484620769650823168', N'批量430', N'64', N'2023-05-30 15:09:56', N'0', N'0', N'0'), (N'484620769889898496', N'批量813', N'65', N'2023-05-30 15:09:56', N'0', N'0', N'0'), (N'484620770128973824', N'批量572', N'66', N'2023-05-30 15:09:56', N'0', N'0', N'0'), (N'484620773429891072', N'批增更218', N'67', N'2023-05-30 15:09:57', N'0', N'0', N'0'), (N'484623044423208960', N'单个5122', N'68', N'2023-05-30 15:18:58', N'0', N'0', N'0'), (N'484623148454531072', N'批量40', N'69', N'2023-05-30 15:19:23', N'0', N'0', N'0'), (N'484623148689412096', N'批量680', N'70', N'2023-05-30 15:19:23', N'0', N'0', N'0'), (N'484623148932681728', N'批量531', N'71', N'2023-05-30 15:19:23', N'0', N'0', N'0'), (N'484623187683856384', N'批增更615', N'72', N'2023-05-30 15:19:33', N'0', N'0', N'0'), (N'484623231044571136', N'批增更删992', N'73', N'2023-05-30 15:19:43', N'0', N'0', N'0'), (N'484624288650907648', N'领域服务', N'74', N'2023-05-30 15:23:55', N'0', N'0', N'0'), (N'484624288994840576', N'服务更', N'75', N'2023-05-30 15:23:55', N'0', N'0', N'0'), (N'484956889089593344', N'单个8461', N'76', N'2023-05-31 13:25:35', N'0', N'0', N'0'), (N'484957035659546624', N'单个8271', N'77', N'2023-05-31 13:26:09', N'0', N'0', N'0'), (N'484957333266386944', N'批量652', N'78', N'2023-05-31 13:27:20', N'0', N'0', N'0'), (N'484957333782286336', N'批量521', N'79', N'2023-05-31 13:27:21', N'0', N'0', N'0'), (N'484957334516289536', N'批量955', N'80', N'2023-05-31 13:27:21', N'0', N'0', N'0'), (N'484988812650369024', N'批增更778', N'81', N'2023-05-31 15:32:23', N'0', N'0', N'0'), (N'486788489460862976', N'单个4284', N'82', N'2023-06-05 14:43:45', N'0', N'0', N'0'), (N'487086064026013696', N'单个1221', N'83', N'2023-06-06 10:26:08', N'0', N'0', N'0'), (N'487086286626115584', N'单个685', N'84', N'2023-06-06 10:27:01', N'0', N'0', N'0')
GO


-- ----------------------------
-- Table structure for demo_par_tbl
-- ----------------------------
CREATE TABLE [dbo].[demo_par_tbl] (
  [id] bigint NOT NULL,
  [name] nvarchar(255) NOT NULL
)
GO


-- ----------------------------
-- Records of demo_par_tbl
-- ----------------------------
INSERT INTO [dbo].[demo_par_tbl] VALUES (N'443588385522601984', N'91471c9846a44fe8a7fc4b76e9f702ea'), (N'443588583535693824', N'新增'), (N'443588895352836096', N'新增'), (N'443588932694724608', N'新增'), (N'445140374589034496', N'新增'), (N'446130095742013440', N'新增'), (N'484622270804807680', N'新增'), (N'484622408633831424', N'新增'), (N'484623850568437760', N'新增'), (N'484623946693496832', N'新增')
GO


-- ----------------------------
-- Table structure for demo_virtbl1
-- ----------------------------
CREATE TABLE [dbo].[demo_virtbl1] (
  [id] bigint NOT NULL,
  [name1] nvarchar(255) NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'名称1',
'SCHEMA', N'dbo',
'TABLE', N'demo_virtbl1',
'COLUMN', N'name1'
GO


-- ----------------------------
-- Records of demo_virtbl1
-- ----------------------------
INSERT INTO [dbo].[demo_virtbl1] VALUES (N'484613811564728320', N'新1'), (N'484613939734269952', N'新1'), (N'484614242416218112', N'批增1'), (N'484621407772233728', N'新1'), (N'484623466739290112', N'新1')
GO


-- ----------------------------
-- Table structure for demo_virtbl2
-- ----------------------------
CREATE TABLE [dbo].[demo_virtbl2] (
  [id] bigint NOT NULL,
  [name2] nvarchar(255) NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'名称2',
'SCHEMA', N'dbo',
'TABLE', N'demo_virtbl2',
'COLUMN', N'name2'
GO


-- ----------------------------
-- Records of demo_virtbl2
-- ----------------------------
INSERT INTO [dbo].[demo_virtbl2] VALUES (N'484613811564728320', N'新2'), (N'484613939734269952', N'新2'), (N'484614242416218112', N'批增2'), (N'484621407772233728', N'新2'), (N'484623466739290112', N'新2')
GO


-- ----------------------------
-- Table structure for demo_virtbl3
-- ----------------------------
CREATE TABLE [dbo].[demo_virtbl3] (
  [id] bigint NOT NULL,
  [name3] nvarchar(255) NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'名称3',
'SCHEMA', N'dbo',
'TABLE', N'demo_virtbl3',
'COLUMN', N'name3'
GO


-- ----------------------------
-- Records of demo_virtbl3
-- ----------------------------
INSERT INTO [dbo].[demo_virtbl3] VALUES (N'484613811564728320', N'新3'), (N'484613939734269952', N'新3'), (N'484614242416218112', N'批增3'), (N'484621407772233728', N'新3'), (N'484623466739290112', N'新3')
GO


-- ----------------------------
-- Table structure for demo_大儿
-- ----------------------------
CREATE TABLE [dbo].[demo_大儿] (
  [id] bigint NOT NULL,
  [parent_id] bigint NOT NULL,
  [大儿名] nvarchar(255) NOT NULL
)
GO


-- ----------------------------
-- Records of demo_大儿
-- ----------------------------
INSERT INTO [dbo].[demo_大儿] VALUES (N'453807589999792128', N'448686488403595264', N'啊北侧'), (N'453810847795400704', N'453810798449414144', N'bd'), (N'453811346175184896', N'453810798449414144', N'asdf'), (N'453811364621733888', N'453810798449414144', N'bde')
GO


-- ----------------------------
-- Table structure for demo_父表
-- ----------------------------
CREATE TABLE [dbo].[demo_父表] (
  [id] bigint NOT NULL,
  [父名] nvarchar(255) NOT NULL
)
GO


-- ----------------------------
-- Records of demo_父表
-- ----------------------------
INSERT INTO [dbo].[demo_父表] VALUES (N'448686488403595264', N'123'), (N'449120963746877440', N'单位'), (N'453810798449414144', N'aaaa')
GO


-- ----------------------------
-- Table structure for demo_基础
-- ----------------------------
CREATE TABLE [dbo].[demo_基础] (
  [id] bigint NOT NULL,
  [序列] int NOT NULL,
  [限长4] nvarchar(16) NOT NULL,
  [不重复] nvarchar(64) NOT NULL,
  [禁止选中] bit NOT NULL,
  [禁止保存] bit NOT NULL,
  [禁止删除] bit NOT NULL,
  [值变事件] nvarchar(64) NOT NULL,
  [创建时间] datetime NOT NULL,
  [修改时间] datetime NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'标识',
'SCHEMA', N'dbo',
'TABLE', N'demo_基础',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'序列自动赋值',
'SCHEMA', N'dbo',
'TABLE', N'demo_基础',
'COLUMN', N'序列'
GO

EXEC sp_addextendedproperty
'MS_Description', N'限制最大长度4',
'SCHEMA', N'dbo',
'TABLE', N'demo_基础',
'COLUMN', N'限长4'
GO

EXEC sp_addextendedproperty
'MS_Description', N'列值无重复',
'SCHEMA', N'dbo',
'TABLE', N'demo_基础',
'COLUMN', N'不重复'
GO

EXEC sp_addextendedproperty
'MS_Description', N'始终为false',
'SCHEMA', N'dbo',
'TABLE', N'demo_基础',
'COLUMN', N'禁止选中'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时保存前校验不通过',
'SCHEMA', N'dbo',
'TABLE', N'demo_基础',
'COLUMN', N'禁止保存'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时删除前校验不通过',
'SCHEMA', N'dbo',
'TABLE', N'demo_基础',
'COLUMN', N'禁止删除'
GO

EXEC sp_addextendedproperty
'MS_Description', N'每次值变化时触发领域事件',
'SCHEMA', N'dbo',
'TABLE', N'demo_基础',
'COLUMN', N'值变事件'
GO

EXEC sp_addextendedproperty
'MS_Description', N'初次创建时间',
'SCHEMA', N'dbo',
'TABLE', N'demo_基础',
'COLUMN', N'创建时间'
GO

EXEC sp_addextendedproperty
'MS_Description', N'最后修改时间',
'SCHEMA', N'dbo',
'TABLE', N'demo_基础',
'COLUMN', N'修改时间'
GO


-- ----------------------------
-- Records of demo_基础
-- ----------------------------
INSERT INTO [dbo].[demo_基础] VALUES (N'1', N'1', N'adb', N'ddd', N'1', N'1', N'1', N'a', N'2023-01-17 10:08:10', N'2023-01-17 10:08:14'), (N'447570516976357376', N'6', N'11', N'dd', N'0', N'0', N'1', N'snv111', N'2023-02-17 09:25:27', N'2023-02-17 09:25:27')
GO


-- ----------------------------
-- Table structure for demo_角色
-- ----------------------------
CREATE TABLE [dbo].[demo_角色] (
  [id] bigint NOT NULL,
  [角色名称] nvarchar(32) NOT NULL,
  [角色描述] nvarchar(255) NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'demo_角色',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色名称',
'SCHEMA', N'dbo',
'TABLE', N'demo_角色',
'COLUMN', N'角色名称'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色描述',
'SCHEMA', N'dbo',
'TABLE', N'demo_角色',
'COLUMN', N'角色描述'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色',
'SCHEMA', N'dbo',
'TABLE', N'demo_角色'
GO


-- ----------------------------
-- Records of demo_角色
-- ----------------------------
INSERT INTO [dbo].[demo_角色] VALUES (N'449487215124303872', N'xxx', N'df'), (N'449812931669938176', N'管理员', N''), (N'449812975420723200', N'维护1', N''), (N'449813053959065600', N'维护2', N'')
GO


-- ----------------------------
-- Table structure for demo_角色权限
-- ----------------------------
CREATE TABLE [dbo].[demo_角色权限] (
  [role_id] bigint NOT NULL,
  [prv_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'demo_角色权限',
'COLUMN', N'role_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限标识',
'SCHEMA', N'dbo',
'TABLE', N'demo_角色权限',
'COLUMN', N'prv_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色关联的权限',
'SCHEMA', N'dbo',
'TABLE', N'demo_角色权限'
GO


-- ----------------------------
-- Records of demo_角色权限
-- ----------------------------
INSERT INTO [dbo].[demo_角色权限] VALUES (N'449487215124303872', N'449812884102336512')
GO


-- ----------------------------
-- Table structure for demo_扩展1
-- ----------------------------
CREATE TABLE [dbo].[demo_扩展1] (
  [id] bigint NOT NULL,
  [扩展1名称] nvarchar(255) NOT NULL,
  [禁止选中] bit NOT NULL,
  [禁止保存] bit NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'标识',
'SCHEMA', N'dbo',
'TABLE', N'demo_扩展1',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'始终为false',
'SCHEMA', N'dbo',
'TABLE', N'demo_扩展1',
'COLUMN', N'禁止选中'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时保存前校验不通过',
'SCHEMA', N'dbo',
'TABLE', N'demo_扩展1',
'COLUMN', N'禁止保存'
GO


-- ----------------------------
-- Records of demo_扩展1
-- ----------------------------
INSERT INTO [dbo].[demo_扩展1] VALUES (N'447555037331214336', N'a', N'0', N'0'), (N'447577275388416000', N'221', N'0', N'0'), (N'447577372700463104', N'', N'0', N'0')
GO


-- ----------------------------
-- Table structure for demo_扩展2
-- ----------------------------
CREATE TABLE [dbo].[demo_扩展2] (
  [id] bigint NOT NULL,
  [扩展2名称] nvarchar(255) NOT NULL,
  [禁止删除] bit NOT NULL,
  [值变事件] nvarchar(255) NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'标识',
'SCHEMA', N'dbo',
'TABLE', N'demo_扩展2',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'true时删除前校验不通过',
'SCHEMA', N'dbo',
'TABLE', N'demo_扩展2',
'COLUMN', N'禁止删除'
GO

EXEC sp_addextendedproperty
'MS_Description', N'每次值变化时触发领域事件',
'SCHEMA', N'dbo',
'TABLE', N'demo_扩展2',
'COLUMN', N'值变事件'
GO


-- ----------------------------
-- Records of demo_扩展2
-- ----------------------------
INSERT INTO [dbo].[demo_扩展2] VALUES (N'447555037331214336', N'a', N'0', N''), (N'447577275388416000', N'', N'0', N'221'), (N'447577372700463104', N'', N'0', N'')
GO


-- ----------------------------
-- Table structure for demo_权限
-- ----------------------------
CREATE TABLE [dbo].[demo_权限] (
  [id] bigint NOT NULL,
  [权限名称] nvarchar(255) NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限名称',
'SCHEMA', N'dbo',
'TABLE', N'demo_权限',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'权限',
'SCHEMA', N'dbo',
'TABLE', N'demo_权限'
GO


-- ----------------------------
-- Records of demo_权限
-- ----------------------------
INSERT INTO [dbo].[demo_权限] VALUES (N'449812852120768512', N'删除'), (N'449812884102336512', N'修改')
GO


-- ----------------------------
-- Table structure for demo_收文
-- ----------------------------
CREATE TABLE [dbo].[demo_收文] (
  [id] bigint NOT NULL,
  [来文单位] nvarchar(255) NOT NULL,
  [来文时间] date NOT NULL,
  [密级] tinyint NOT NULL,
  [文件标题] nvarchar(255) NOT NULL,
  [文件附件] nvarchar(512) NOT NULL,
  [市场部经理意见] nvarchar(255) NOT NULL,
  [综合部经理意见] nvarchar(255) NOT NULL,
  [收文完成时间] date NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'#密级#',
'SCHEMA', N'dbo',
'TABLE', N'demo_收文',
'COLUMN', N'密级'
GO


-- ----------------------------
-- Records of demo_收文
-- ----------------------------
INSERT INTO [dbo].[demo_收文] VALUES (N'162025231350624256', N'123', N'2020-12-21', N'0', N'a', N'', N'', N'', N'0001-01-01'), (N'162401333600448512', N'abc', N'2020-12-22', N'0', N'关于新冠疫情的批示', N'', N'', N'', N'0001-01-01'), (N'457384396879581184', N'', N'2023-03-16', N'0', N'阿斯蒂芬', N'', N'', N'', N'0001-01-01'), (N'457388173615452160', N'', N'2023-03-16', N'0', N'疫情在', N'', N'', N'', N'0001-01-01')
GO


-- ----------------------------
-- Table structure for demo_小儿
-- ----------------------------
CREATE TABLE [dbo].[demo_小儿] (
  [id] bigint NOT NULL,
  [group_id] bigint NOT NULL,
  [小儿名] nvarchar(255) NOT NULL
)
GO


-- ----------------------------
-- Records of demo_小儿
-- ----------------------------
INSERT INTO [dbo].[demo_小儿] VALUES (N'449113382156521472', N'448686488403595264', N'wwww'), (N'453810909078376448', N'453810798449414144', N'34'), (N'453811464773324800', N'453810798449414144', N'adgas')
GO


-- ----------------------------
-- Table structure for demo_用户
-- ----------------------------
CREATE TABLE [dbo].[demo_用户] (
  [id] bigint NOT NULL,
  [手机号] char(11) NOT NULL,
  [姓名] nvarchar(32) NOT NULL,
  [密码] char(32) NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户标识',
'SCHEMA', N'dbo',
'TABLE', N'demo_用户',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'手机号，唯一',
'SCHEMA', N'dbo',
'TABLE', N'demo_用户',
'COLUMN', N'手机号'
GO

EXEC sp_addextendedproperty
'MS_Description', N'姓名',
'SCHEMA', N'dbo',
'TABLE', N'demo_用户',
'COLUMN', N'姓名'
GO

EXEC sp_addextendedproperty
'MS_Description', N'密码的md5',
'SCHEMA', N'dbo',
'TABLE', N'demo_用户',
'COLUMN', N'密码'
GO

EXEC sp_addextendedproperty
'MS_Description', N'系统用户',
'SCHEMA', N'dbo',
'TABLE', N'demo_用户'
GO


-- ----------------------------
-- Records of demo_用户
-- ----------------------------
INSERT INTO [dbo].[demo_用户] VALUES (N'449772627373871104', N'13223333', N'阿斯顿', N''), (N'453805638385946624', N'111', N'', N''), (N'453805654500462592', N'222', N'', N'')
GO


-- ----------------------------
-- Table structure for demo_用户角色
-- ----------------------------
CREATE TABLE [dbo].[demo_用户角色] (
  [user_id] bigint NOT NULL,
  [role_id] bigint NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户标识',
'SCHEMA', N'dbo',
'TABLE', N'demo_用户角色',
'COLUMN', N'user_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色标识',
'SCHEMA', N'dbo',
'TABLE', N'demo_用户角色',
'COLUMN', N'role_id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户关联的角色',
'SCHEMA', N'dbo',
'TABLE', N'demo_用户角色'
GO


-- ----------------------------
-- Records of demo_用户角色
-- ----------------------------
INSERT INTO [dbo].[demo_用户角色] VALUES (N'449772627373871104', N'449487215124303872'), (N'449772627373871104', N'449812931669938176')
GO


-- ----------------------------
-- Table structure for demo_主表
-- ----------------------------
CREATE TABLE [dbo].[demo_主表] (
  [id] bigint NOT NULL,
  [主表名称] nvarchar(255) NOT NULL,
  [限长4] nvarchar(16) NOT NULL,
  [不重复] nvarchar(255) NOT NULL
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'限制最大长度4',
'SCHEMA', N'dbo',
'TABLE', N'demo_主表',
'COLUMN', N'限长4'
GO

EXEC sp_addextendedproperty
'MS_Description', N'列值无重复',
'SCHEMA', N'dbo',
'TABLE', N'demo_主表',
'COLUMN', N'不重复'
GO


-- ----------------------------
-- Records of demo_主表
-- ----------------------------
INSERT INTO [dbo].[demo_主表] VALUES (N'447555037331214336', N'a', N'', N''), (N'447577275388416000', N'1', N'222222', N'121'), (N'447577372700463104', N'', N'', N'1')
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
CREATE UNIQUE NONCLUSTERED INDEX [idx_permission_name]
ON [dbo].[cm_permission] (
  [name] ASC
)
GO

EXEC sp_addextendedproperty
'MS_Description', N'不重复',
'SCHEMA', N'dbo',
'TABLE', N'cm_permission',
'INDEX', N'idx_permission_name'
GO


-- ----------------------------
-- Primary Key structure for table cm_permission
-- ----------------------------
ALTER TABLE [dbo].[cm_permission] ADD PRIMARY KEY CLUSTERED ([id])
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
CREATE UNIQUE NONCLUSTERED INDEX [idx_user_phone]
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
-- Primary Key structure for table demo_cache_tbl1
-- ----------------------------
ALTER TABLE [dbo].[demo_cache_tbl1] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_child_tbl1
-- ----------------------------
ALTER TABLE [dbo].[demo_child_tbl1] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_child_tbl2
-- ----------------------------
ALTER TABLE [dbo].[demo_child_tbl2] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_crud
-- ----------------------------
ALTER TABLE [dbo].[demo_crud] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_par_tbl
-- ----------------------------
ALTER TABLE [dbo].[demo_par_tbl] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_virtbl1
-- ----------------------------
ALTER TABLE [dbo].[demo_virtbl1] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_virtbl2
-- ----------------------------
ALTER TABLE [dbo].[demo_virtbl2] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_virtbl3
-- ----------------------------
ALTER TABLE [dbo].[demo_virtbl3] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table demo_大儿
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_大儿_parendid]
ON [dbo].[demo_大儿] (
  [parent_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table demo_大儿
-- ----------------------------
ALTER TABLE [dbo].[demo_大儿] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_父表
-- ----------------------------
ALTER TABLE [dbo].[demo_父表] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_基础
-- ----------------------------
ALTER TABLE [dbo].[demo_基础] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_角色
-- ----------------------------
ALTER TABLE [dbo].[demo_角色] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table demo_角色权限
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_demo_角色权限_prvid]
ON [dbo].[demo_角色权限] (
  [prv_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_demo_角色权限_roleid]
ON [dbo].[demo_角色权限] (
  [role_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table demo_角色权限
-- ----------------------------
ALTER TABLE [dbo].[demo_角色权限] ADD PRIMARY KEY CLUSTERED ([role_id], [prv_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_扩展1
-- ----------------------------
ALTER TABLE [dbo].[demo_扩展1] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_扩展2
-- ----------------------------
ALTER TABLE [dbo].[demo_扩展2] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_权限
-- ----------------------------
ALTER TABLE [dbo].[demo_权限] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_收文
-- ----------------------------
ALTER TABLE [dbo].[demo_收文] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table demo_小儿
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_小儿_parentid]
ON [dbo].[demo_小儿] (
  [group_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table demo_小儿
-- ----------------------------
ALTER TABLE [dbo].[demo_小儿] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_用户
-- ----------------------------
ALTER TABLE [dbo].[demo_用户] ADD PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Indexes structure for table demo_用户角色
-- ----------------------------
CREATE NONCLUSTERED INDEX [idx_demo_用户角色_roleid]
ON [dbo].[demo_用户角色] (
  [role_id] ASC
)
GO

CREATE NONCLUSTERED INDEX [idx_demo_用户角色_userid]
ON [dbo].[demo_用户角色] (
  [user_id] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table demo_用户角色
-- ----------------------------
ALTER TABLE [dbo].[demo_用户角色] ADD PRIMARY KEY CLUSTERED ([user_id], [role_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


-- ----------------------------
-- Primary Key structure for table demo_主表
-- ----------------------------
ALTER TABLE [dbo].[demo_主表] ADD PRIMARY KEY CLUSTERED ([id])
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
ALTER TABLE [dbo].[cm_group_role] ADD CONSTRAINT [fk_group_role_groupid] FOREIGN KEY ([group_id]) REFERENCES [dbo].[cm_group] ([id])
GO

ALTER TABLE [dbo].[cm_group_role] ADD CONSTRAINT [fk_group_role_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[cm_role] ([id])
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
ALTER TABLE [dbo].[cm_role_menu] ADD CONSTRAINT [fk_role_menu_menuid] FOREIGN KEY ([menu_id]) REFERENCES [dbo].[cm_menu] ([id])
GO

ALTER TABLE [dbo].[cm_role_menu] ADD CONSTRAINT [fk_role_menu_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[cm_role] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_role_per
-- ----------------------------
ALTER TABLE [dbo].[cm_role_per] ADD CONSTRAINT [fk_role_per_perid] FOREIGN KEY ([per_id]) REFERENCES [dbo].[cm_permission] ([id])
GO

ALTER TABLE [dbo].[cm_role_per] ADD CONSTRAINT [fk_role_per_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[cm_role] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_user_group
-- ----------------------------
ALTER TABLE [dbo].[cm_user_group] ADD CONSTRAINT [fk_user_group_groupid] FOREIGN KEY ([group_id]) REFERENCES [dbo].[cm_group] ([id])
GO

ALTER TABLE [dbo].[cm_user_group] ADD CONSTRAINT [fk_user_group_userid] FOREIGN KEY ([user_id]) REFERENCES [dbo].[cm_user] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_user_params
-- ----------------------------
ALTER TABLE [dbo].[cm_user_params] ADD CONSTRAINT [fk_user_params_paramsid] FOREIGN KEY ([param_id]) REFERENCES [dbo].[cm_params] ([id])
GO

ALTER TABLE [dbo].[cm_user_params] ADD CONSTRAINT [fk_user_params_userid] FOREIGN KEY ([user_id]) REFERENCES [dbo].[cm_user] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_user_role
-- ----------------------------
ALTER TABLE [dbo].[cm_user_role] ADD CONSTRAINT [fk_user_role_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[cm_role] ([id])
GO

ALTER TABLE [dbo].[cm_user_role] ADD CONSTRAINT [fk_user_role_userid] FOREIGN KEY ([user_id]) REFERENCES [dbo].[cm_user] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfd_atv
-- ----------------------------
ALTER TABLE [dbo].[cm_wfd_atv] ADD CONSTRAINT [fk_wfd_atv_prcid] FOREIGN KEY ([prc_id]) REFERENCES [dbo].[cm_wfd_prc] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfd_atv_role
-- ----------------------------
ALTER TABLE [dbo].[cm_wfd_atv_role] ADD CONSTRAINT [fk_wfd_atv_role_atvid] FOREIGN KEY ([atv_id]) REFERENCES [dbo].[cm_wfd_atv] ([id])
GO

ALTER TABLE [dbo].[cm_wfd_atv_role] ADD CONSTRAINT [fk_wfd_atv_role_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[cm_role] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfd_trs
-- ----------------------------
ALTER TABLE [dbo].[cm_wfd_trs] ADD CONSTRAINT [fk_wfd_trs_prcid] FOREIGN KEY ([prc_id]) REFERENCES [dbo].[cm_wfd_prc] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfi_atv
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_atv] ADD CONSTRAINT [fk_wfi_atv_atvdid] FOREIGN KEY ([atvd_id]) REFERENCES [dbo].[cm_wfd_atv] ([id])
GO

ALTER TABLE [dbo].[cm_wfi_atv] ADD CONSTRAINT [fk_wfi_atv_prciid] FOREIGN KEY ([prci_id]) REFERENCES [dbo].[cm_wfi_prc] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfi_item
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_item] ADD CONSTRAINT [fk_wfi_item_atviid] FOREIGN KEY ([atvi_id]) REFERENCES [dbo].[cm_wfi_atv] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfi_prc
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_prc] ADD CONSTRAINT [fk_wfi_prc_prcdid] FOREIGN KEY ([prcd_id]) REFERENCES [dbo].[cm_wfd_prc] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table cm_wfi_trs
-- ----------------------------
ALTER TABLE [dbo].[cm_wfi_trs] ADD CONSTRAINT [fk_wfi_trs_srcatviid] FOREIGN KEY ([src_atvi_id]) REFERENCES [dbo].[cm_wfi_atv] ([id])
GO

ALTER TABLE [dbo].[cm_wfi_trs] ADD CONSTRAINT [fk_wfi_trs_tgtatviid] FOREIGN KEY ([tgt_atvi_id]) REFERENCES [dbo].[cm_wfi_atv] ([id])
GO

ALTER TABLE [dbo].[cm_wfi_trs] ADD CONSTRAINT [fk_wfi_trs_trsdid] FOREIGN KEY ([trsd_id]) REFERENCES [dbo].[cm_wfd_trs] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table demo_大儿
-- ----------------------------
ALTER TABLE [dbo].[demo_大儿] ADD CONSTRAINT [fk_大儿_parendid] FOREIGN KEY ([parent_id]) REFERENCES [dbo].[demo_父表] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table demo_角色权限
-- ----------------------------
ALTER TABLE [dbo].[demo_角色权限] ADD CONSTRAINT [fk_角色权限_prvid] FOREIGN KEY ([prv_id]) REFERENCES [dbo].[demo_权限] ([id])
GO

ALTER TABLE [dbo].[demo_角色权限] ADD CONSTRAINT [fk_角色权限_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[demo_角色] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table demo_小儿
-- ----------------------------
ALTER TABLE [dbo].[demo_小儿] ADD CONSTRAINT [fk_小儿_parentid] FOREIGN KEY ([group_id]) REFERENCES [dbo].[demo_父表] ([id])
GO


-- ----------------------------
-- Foreign Keys structure for table demo_用户角色
-- ----------------------------
ALTER TABLE [dbo].[demo_用户角色] ADD CONSTRAINT [fk_demo_用户角色_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[demo_角色] ([id])
GO

ALTER TABLE [dbo].[demo_用户角色] ADD CONSTRAINT [fk_demo_用户角色_userid] FOREIGN KEY ([user_id]) REFERENCES [dbo].[demo_用户] ([id])
GO

-- ----------------------------
-- 序列
-- ----------------------------
create sequence cm_menu_dispidx start with 90;
create sequence cm_option_dispidx start with 1032;
create sequence cm_wfd_prc_dispidx start with 12;
create sequence cm_wfi_item_dispidx start with 177;
create sequence cm_wfi_prc_dispidx start with 66;
create sequence demo_crud_dispidx start with 86;
create sequence demo_基础_序列 start with 12;
GO

-- ----------------------------
-- View structure for demo_child_view
-- ----------------------------
CREATE VIEW [dbo].[demo_child_view] AS SELECT
	c.*, 
	p.name
FROM
	demo_child_tbl1 c join
	demo_par_tbl p on c.parent_id=p.id
GO


-- ----------------------------
-- procedure structure for demo_用户可访问的菜单
-- ----------------------------
CREATE PROCEDURE [dbo].[demo_用户可访问的菜单]
  @p_userid AS bigint 
AS
BEGIN
	select id,name
  from (select distinct (b.id), b.name, dispidx
          from cm_role_menu a
          left join cm_menu b
            on a.menu_id = b.id
         where exists
         (select role_id
                  from cm_user_role c
                 where a.role_id = c.role_id
                   and user_id = @p_userid
					union
					select role_id
					        from cm_group_role d
									where a.role_id = d.role_id
									  and exists (select group_id from cm_user_group e where d.group_id=e.group_id and e.user_id=@p_userid)
					) or a.role_id=1
			 ) t
  order by dispidx;
END
GO
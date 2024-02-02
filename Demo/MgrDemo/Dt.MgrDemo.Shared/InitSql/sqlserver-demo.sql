IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_大儿]') AND type IN ('U')) DROP TABLE [dbo].[demo_大儿]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_小儿]') AND type IN ('U')) DROP TABLE [dbo].[demo_小儿]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_角色权限]') AND type IN ('U')) DROP TABLE [dbo].[demo_角色权限]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_用户角色]') AND type IN ('U')) DROP TABLE [dbo].[demo_用户角色]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_cache_tbl1]') AND type IN ('U')) DROP TABLE [dbo].[demo_cache_tbl1]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_child_tbl1]') AND type IN ('U')) DROP TABLE [dbo].[demo_child_tbl1]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_child_tbl2]') AND type IN ('U')) DROP TABLE [dbo].[demo_child_tbl2]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_crud]') AND type IN ('U')) DROP TABLE [dbo].[demo_crud]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_par_tbl]') AND type IN ('U')) DROP TABLE [dbo].[demo_par_tbl]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_virtbl1]') AND type IN ('U')) DROP TABLE [dbo].[demo_virtbl1]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_virtbl2]') AND type IN ('U')) DROP TABLE [dbo].[demo_virtbl2]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_virtbl3]') AND type IN ('U')) DROP TABLE [dbo].[demo_virtbl3]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_父表]') AND type IN ('U')) DROP TABLE [dbo].[demo_父表]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_基础]') AND type IN ('U')) DROP TABLE [dbo].[demo_基础]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_角色]') AND type IN ('U')) DROP TABLE [dbo].[demo_角色]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_扩展1]') AND type IN ('U')) DROP TABLE [dbo].[demo_扩展1]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_扩展2]') AND type IN ('U')) DROP TABLE [dbo].[demo_扩展2]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_权限]') AND type IN ('U')) DROP TABLE [dbo].[demo_权限]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_收文]') AND type IN ('U')) DROP TABLE [dbo].[demo_收文]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_用户]') AND type IN ('U')) DROP TABLE [dbo].[demo_用户]
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_主表]') AND type IN ('U')) DROP TABLE [dbo].[demo_主表]
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
-- Foreign Keys structure for table demo_大儿
-- ----------------------------
ALTER TABLE [dbo].[demo_大儿] ADD CONSTRAINT [fk_大儿_parendid] FOREIGN KEY ([parent_id]) REFERENCES [dbo].[demo_父表] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table demo_角色权限
-- ----------------------------
ALTER TABLE [dbo].[demo_角色权限] ADD CONSTRAINT [fk_角色权限_prvid] FOREIGN KEY ([prv_id]) REFERENCES [dbo].[demo_权限] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[demo_角色权限] ADD CONSTRAINT [fk_角色权限_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[demo_角色] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table demo_小儿
-- ----------------------------
ALTER TABLE [dbo].[demo_小儿] ADD CONSTRAINT [fk_小儿_parentid] FOREIGN KEY ([group_id]) REFERENCES [dbo].[demo_父表] ([id]) ON DELETE CASCADE
GO


-- ----------------------------
-- Foreign Keys structure for table demo_用户角色
-- ----------------------------
ALTER TABLE [dbo].[demo_用户角色] ADD CONSTRAINT [fk_demo_用户角色_roleid] FOREIGN KEY ([role_id]) REFERENCES [dbo].[demo_角色] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[demo_用户角色] ADD CONSTRAINT [fk_demo_用户角色_userid] FOREIGN KEY ([user_id]) REFERENCES [dbo].[demo_用户] ([id]) ON DELETE CASCADE
GO

-- ----------------------------
-- 序列
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_crud_dispidx]') AND type IN ('SO'))
drop sequence demo_crud_dispidx
GO

IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_基础_序列]') AND type IN ('SO'))
drop sequence demo_基础_序列
GO

create sequence demo_crud_dispidx start with 86;
create sequence demo_基础_序列 start with 12;
GO

-- ----------------------------
-- View structure for demo_child_view
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_child_view]') AND type IN ('V'))
	DROP VIEW [dbo].[demo_child_view]
GO

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
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[demo_用户可访问的菜单]') AND type IN ('P', 'PC', 'RF', 'X'))
	DROP PROCEDURE[dbo].[demo_用户可访问的菜单]
GO

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
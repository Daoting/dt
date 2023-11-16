/*
Navicat 从 mysql 导出后修改：
1. 删除 delimiter ;;\r\n
2. 删除 delimiter ;\r\n
3. ;; 替换成 ;
*/

SET NAMES utf8;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for cm_file_my
-- ----------------------------
DROP TABLE IF EXISTS `cm_file_my`;
CREATE TABLE `cm_file_my`  (
  `id` bigint(20) NOT NULL COMMENT '文件标识',
  `parent_id` bigint(20) NULL DEFAULT NULL COMMENT '上级目录，根目录的parendid为空',
  `name` varchar(255) NOT NULL COMMENT '名称',
  `is_folder` tinyint(1) NOT NULL COMMENT '是否为文件夹',
  `ext_name` varchar(8) NULL DEFAULT NULL COMMENT '文件扩展名',
  `info` varchar(512) NULL DEFAULT NULL COMMENT '文件描述信息',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `user_id` bigint(20) NOT NULL COMMENT '所属用户',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_file_my_parentid`(`parent_id`) USING BTREE,
  INDEX `idx_file_my_userid`(`user_id`) USING BTREE,
  CONSTRAINT `fk_file_my_parentid` FOREIGN KEY (`parent_id`) REFERENCES `cm_file_my` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_file_my_userid` FOREIGN KEY (`user_id`) REFERENCES `cm_user` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB COMMENT = '个人文件';

-- ----------------------------
-- Table structure for cm_file_pub
-- ----------------------------
DROP TABLE IF EXISTS `cm_file_pub`;
CREATE TABLE `cm_file_pub`  (
  `id` bigint(20) NOT NULL COMMENT '文件标识',
  `parent_id` bigint(20) NULL DEFAULT NULL COMMENT '上级目录，根目录的parendid为空',
  `name` varchar(255) NOT NULL COMMENT '名称',
  `is_folder` tinyint(1) NOT NULL COMMENT '是否为文件夹',
  `ext_name` varchar(8) NULL DEFAULT NULL COMMENT '文件扩展名',
  `info` varchar(512) NULL DEFAULT NULL COMMENT '文件描述信息',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_file_pub_parentid`(`parent_id`) USING BTREE,
  CONSTRAINT `fk_file_pub_parentid` FOREIGN KEY (`parent_id`) REFERENCES `cm_file_pub` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB COMMENT = '公共文件';

-- ----------------------------
-- Records of cm_file_pub
-- ----------------------------
INSERT INTO `cm_file_pub` VALUES (1, NULL, '公共文件', 1, NULL, '', '2020-10-21 15:19:20'), (2, NULL, '素材库', 1, NULL, '', '2020-10-21 15:20:21');

-- ----------------------------
-- Table structure for cm_group
-- ----------------------------
DROP TABLE IF EXISTS `cm_group`;
CREATE TABLE `cm_group`  (
  `id` bigint(20) NOT NULL COMMENT '组标识',
  `name` varchar(64) NOT NULL COMMENT '组名',
  `note` varchar(255) NULL DEFAULT NULL COMMENT '组描述',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_group_name`(`name`) USING BTREE COMMENT '不重复'
) ENGINE = InnoDB COMMENT = '分组，与用户和角色多对多';

-- ----------------------------
-- Table structure for cm_group_role
-- ----------------------------
DROP TABLE IF EXISTS `cm_group_role`;
CREATE TABLE `cm_group_role`  (
  `group_id` bigint(20) NOT NULL COMMENT '组标识',
  `role_id` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`group_id`, `role_id`) USING BTREE,
  INDEX `idx_group_role_groupid`(`group_id`) USING BTREE,
  INDEX `idx_group_role_roleid`(`role_id`) USING BTREE,
  CONSTRAINT `fk_group_role_groupid` FOREIGN KEY (`group_id`) REFERENCES `cm_group` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_group_role_roleid` FOREIGN KEY (`role_id`) REFERENCES `cm_role` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '组一角色多对多';

-- ----------------------------
-- Table structure for cm_menu
-- ----------------------------
DROP TABLE IF EXISTS `cm_menu`;
CREATE TABLE `cm_menu`  (
  `id` bigint(20) NOT NULL COMMENT '菜单标识',
  `parent_id` bigint(20) NULL DEFAULT NULL COMMENT '父菜单标识',
  `name` varchar(64) NOT NULL COMMENT '菜单名称',
  `is_group` tinyint(1) NOT NULL COMMENT '分组或实例。0表实例，1表分组',
  `view_name` varchar(128) NULL DEFAULT NULL COMMENT '视图名称',
  `params` varchar(4000) NULL DEFAULT NULL COMMENT '传递给菜单程序的参数',
  `icon` varchar(128) NULL DEFAULT NULL COMMENT '图标',
  `note` varchar(512) NULL DEFAULT NULL COMMENT '备注',
  `dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `is_locked` tinyint(1) NOT NULL DEFAULT 0 COMMENT '定义了菜单是否被锁定。0表未锁定，1表锁定不可用',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '最后修改时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_menu_parentid`(`parent_id`) USING BTREE,
  CONSTRAINT `fk_menu_parentid` FOREIGN KEY (`parent_id`) REFERENCES `cm_menu` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB COMMENT = '业务菜单';

-- ----------------------------
-- Records of cm_menu
-- ----------------------------
INSERT INTO `cm_menu` VALUES 
(1, NULL, '工作台', 1, '', '', '搬运工', '', 1, 0, '2019-03-07 10:45:44', '2019-03-07 10:45:43'), 
(2, 1, '用户账号', 0, '用户账号', '', '钥匙', '账号管理及所属分组、关联角色，查看拥有菜单、已授权限', 2, 0, '2019-11-08 11:42:28', '2019-11-08 11:43:53'), 
(3, 1, '菜单管理', 0, '菜单管理', '', '大图标', '菜单、菜单组管理和菜单授权角色', 3, 0, '2019-03-11 11:35:59', '2019-03-11 11:35:58'), 
(4, 1, '系统角色', 0, '系统角色', '', '两人', '角色管理及所属分组、关联用户、拥有菜单、授予权限的管理', 4, 0, '2019-11-08 11:47:21', '2019-11-08 11:48:22'), 
(5, 1, '分组管理', 0, '分组管理', '', '分组', '分组管理及关联的角色、关联的用户', 5, 0, '2023-03-10 08:34:49', '2023-03-10 08:34:49'), 
(6, 1, '基础权限', 0, '基础权限', '', '审核', '按照模块、功能两级目录管理的权限和关联的角色', 6, 0, '2019-03-12 09:11:22', '2019-03-07 11:23:40'), 
(7, 1, '参数定义', 0, '参数定义', '', '调色板', '参数名称、默认值的定义管理', 7, 0, '2019-03-12 15:35:56', '2019-03-12 15:37:10'), 
(8, 1, '基础选项', 0, '基础选项', '', '修理', '按照分组管理的选项列表，如民族、学历等静态列表', 8, 0, '2019-11-08 11:49:40', '2019-11-08 11:49:46'), 
(9, 1, '报表设计', 0, '报表设计', '', '折线图', '报表管理及报表模板设计', 9, 0, '2020-10-19 11:21:38', '2020-10-19 11:21:38'), 
(10, 1, '流程设计', 0, '流程设计', '', '双绞线', '流程模板设计及流程实例查询', 10, 0, '2020-11-02 16:21:19', '2020-11-02 16:21:19');

-- ----------------------------
-- Table structure for cm_option
-- ----------------------------
DROP TABLE IF EXISTS `cm_option`;
CREATE TABLE `cm_option`  (
  `id` bigint(20) NOT NULL COMMENT '标识',
  `name` varchar(64) NOT NULL COMMENT '选项名称',
  `dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `group_id` bigint(20) NOT NULL COMMENT '所属分组',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_option_groupid`(`group_id`) USING BTREE,
  CONSTRAINT `fk_option_groupid` FOREIGN KEY (`group_id`) REFERENCES `cm_option_group` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB COMMENT = '基础选项';

-- ----------------------------
-- Records of cm_option
-- ----------------------------
INSERT INTO `cm_option` VALUES (2,'汉族',2,1),(3,'蒙古族',3,1),(4,'回族',4,1),(5,'藏族',5,1),(6,'维吾尔族',6,1),(7,'苗族',7,1),(8,'彝族',8,1),(9,'壮族',9,1),(10,'布依族',10,1),(11,'朝鲜族',11,1),(12,'满族',12,1),(13,'侗族',13,1),(14,'瑶族',14,1),(15,'白族',15,1),(16,'土家族',16,1),(17,'哈尼族',17,1),(18,'哈萨克族',18,1),(19,'傣族',19,1),(20,'黎族',20,1),(21,'傈僳族',21,1),(22,'佤族',22,1),(23,'畲族',23,1),(24,'高山族',24,1),(25,'拉祜族',25,1),(26,'水族',26,1),(27,'东乡族',27,1),(28,'纳西族',28,1),(29,'景颇族',29,1),(30,'柯尔克孜族',30,1),(31,'土族',31,1),(32,'达斡尔族',32,1),(33,'仫佬族',33,1),(34,'羌族',34,1),(35,'布朗族',35,1),(36,'撒拉族',36,1),(37,'毛难族',37,1),(38,'仡佬族',38,1),(39,'锡伯族',39,1),(40,'阿昌族',40,1),(41,'普米族',41,1),(42,'塔吉克族',42,1),(43,'怒族',43,1),(44,'乌孜别克族',44,1),(45,'俄罗斯族',45,1),(46,'鄂温克族',46,1),(47,'德昂族',47,1),(48,'保安族',48,1),(49,'裕固族',49,1),(50,'京族',50,1),(51,'塔塔尔族',51,1),(52,'独龙族',52,1),(53,'鄂伦春族',53,1),(54,'赫哲族',54,1),(55,'门巴族',55,1),(56,'珞巴族',56,1),(57,'基诺族',57,1),(58,'大学',58,2),(59,'高中',59,2),(60,'中学',60,2),(61,'小学',61,2),(62,'硕士',62,2),(63,'博士',63,2),(64,'其他',64,2),(342,'男',342,4),(343,'女',343,4),(344,'未知',344,4),(345,'不明',345,4),(346,'string',346,5),(347,'int',347,5),(348,'double',348,5),(349,'DateTime',349,5),(350,'Date',350,5),(351,'bool',351,5);

-- ----------------------------
-- Table structure for cm_option_group
-- ----------------------------
DROP TABLE IF EXISTS `cm_option_group`;
CREATE TABLE `cm_option_group`  (
  `id` bigint(20) NOT NULL COMMENT '标识',
  `name` varchar(255) NOT NULL COMMENT '分组名称',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB COMMENT = '基础选项分组';

-- ----------------------------
-- Records of cm_option_group
-- ----------------------------
INSERT INTO `cm_option_group` VALUES (1,'民族'),(2,'学历'),(4,'性别'),(5,'数据类型');

-- ----------------------------
-- Table structure for cm_params
-- ----------------------------
DROP TABLE IF EXISTS `cm_params`;
CREATE TABLE `cm_params`  (
  `id` bigint(20) NOT NULL COMMENT '用户参数标识',
  `name` varchar(255) NOT NULL COMMENT '参数名称',
  `value` varchar(255) NULL DEFAULT NULL COMMENT '参数缺省值',
  `note` varchar(255) NULL DEFAULT NULL COMMENT '参数描述',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_params_name`(`name`) USING BTREE
) ENGINE = InnoDB COMMENT = '用户参数定义';

-- ----------------------------
-- Records of cm_params
-- ----------------------------
INSERT INTO `cm_params` VALUES (1, '接收新任务', 'true', '', '2020-12-01 15:13:49', '2020-12-02 09:23:53'), (2, '接收新发布通知', 'true', '', '2020-12-02 09:25:15', '2020-12-02 09:25:15'), (3, '接收新消息', 'true', '接收通讯录消息推送', '2020-12-02 09:24:28', '2020-12-02 09:24:28');

-- ----------------------------
-- Table structure for cm_permission_module
-- ----------------------------
DROP TABLE IF EXISTS `cm_permission_module`;
CREATE TABLE `cm_permission_module`  (
  `id` bigint(20) NOT NULL COMMENT '模块标识',
  `name` varchar(64) NOT NULL COMMENT '模块名称',
  `note` varchar(255) NULL DEFAULT NULL COMMENT '模块描述',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB COMMENT = '权限所属模块';

-- ----------------------------
-- Records of cm_permission_module
-- ----------------------------
INSERT INTO `cm_permission_module` VALUES (1, '系统预留', '系统内部使用的权限控制，禁止删除');

-- ----------------------------
-- Table structure for cm_permission_func
-- ----------------------------
DROP TABLE IF EXISTS `cm_permission_func`;
CREATE TABLE `cm_permission_func`  (
  `id` bigint(20) NOT NULL,
  `module_id` bigint(20) NOT NULL COMMENT '所属模块',
  `name` varchar(64) NOT NULL COMMENT '功能名称',
  `note` varchar(255) NULL DEFAULT NULL COMMENT '功能描述',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `fk_permission_func`(`module_id`) USING BTREE,
  CONSTRAINT `fk_permission_func` FOREIGN KEY (`module_id`) REFERENCES `cm_permission_module` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB COMMENT = '权限所属功能';

-- ----------------------------
-- Records of cm_permission_func
-- ----------------------------
INSERT INTO `cm_permission_func` VALUES (1, 1, '文件管理', '管理文件的上传、删除等');

-- ----------------------------
-- Table structure for cm_permission
-- ----------------------------
DROP TABLE IF EXISTS `cm_permission`;
CREATE TABLE `cm_permission`  (
  `id` bigint(20) NOT NULL COMMENT '权限标识',
  `func_id` bigint(20) NOT NULL COMMENT '所属功能',
  `name` varchar(64) NOT NULL COMMENT '权限名称',
  `note` varchar(255) NULL DEFAULT NULL COMMENT '权限描述',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `fk_permission`(`func_id`) USING BTREE,
  CONSTRAINT `fk_permission` FOREIGN KEY (`func_id`) REFERENCES `cm_permission_func` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB COMMENT = '权限';

-- ----------------------------
-- Records of cm_permission
-- ----------------------------
INSERT INTO `cm_permission` VALUES (1, 1, '公共文件增删', '公共文件的上传、删除等'), (2, 1, '素材库增删', '素材库目录的上传、删除等');

-- ----------------------------
-- Table structure for cm_role
-- ----------------------------
DROP TABLE IF EXISTS `cm_role`;
CREATE TABLE `cm_role`  (
  `id` bigint(20) NOT NULL COMMENT '角色标识',
  `name` varchar(32) NOT NULL COMMENT '角色名称',
  `note` varchar(255) NULL DEFAULT NULL COMMENT '角色描述',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_role_name`(`name`) USING BTREE COMMENT '不重复'
) ENGINE = InnoDB COMMENT = '角色';

-- ----------------------------
-- Records of cm_role
-- ----------------------------
INSERT INTO `cm_role` VALUES (1, '任何人', '所有用户默认都具有该角色，不可删除'), (2, '系统管理员', '系统角色，不可删除');

-- ----------------------------
-- Table structure for cm_role_menu
-- ----------------------------
DROP TABLE IF EXISTS `cm_role_menu`;
CREATE TABLE `cm_role_menu`  (
  `role_id` bigint(20) NOT NULL COMMENT '角色标识',
  `menu_id` bigint(20) NOT NULL COMMENT '菜单标识',
  PRIMARY KEY (`role_id`, `menu_id`) USING BTREE,
  INDEX `idx_role_menu_menuid`(`menu_id`) USING BTREE,
  INDEX `idx_role_menu_roleid`(`role_id`) USING BTREE,
  CONSTRAINT `fk_role_menu_menuid` FOREIGN KEY (`menu_id`) REFERENCES `cm_menu` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_role_menu_roleid` FOREIGN KEY (`role_id`) REFERENCES `cm_role` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '角色一菜单多对多';

-- ----------------------------
-- Records of cm_role_menu
-- ----------------------------
INSERT INTO `cm_role_menu` VALUES (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (1, 7), (1, 8), (1, 9), (2, 10);

-- ----------------------------
-- Table structure for cm_role_per
-- ----------------------------
DROP TABLE IF EXISTS `cm_role_per`;
CREATE TABLE `cm_role_per`  (
  `role_id` bigint(20) NOT NULL COMMENT '角色标识',
  `per_id` bigint(20) NOT NULL COMMENT '权限标识',
  PRIMARY KEY (`role_id`, `per_id`) USING BTREE,
  INDEX `idx_role_per_roleid`(`role_id`) USING BTREE,
  INDEX `idx_role_per_perid`(`per_id`) USING BTREE,
  CONSTRAINT `fk_role_per_perid` FOREIGN KEY (`per_id`) REFERENCES `cm_permission` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_role_per_roleid` FOREIGN KEY (`role_id`) REFERENCES `cm_role` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '角色一权限多对多';

-- ----------------------------
-- Records of cm_role_per
-- ----------------------------
INSERT INTO `cm_role_per` VALUES (2, 1), (2, 2);

-- ----------------------------
-- Table structure for cm_rpt
-- ----------------------------
DROP TABLE IF EXISTS `cm_rpt`;
CREATE TABLE `cm_rpt`  (
  `id` bigint(20) NOT NULL COMMENT '报表标识',
  `name` varchar(64) NOT NULL COMMENT '报表名称',
  `define` varchar(21000) NULL DEFAULT NULL COMMENT '报表模板定义',
  `note` varchar(255) NULL DEFAULT NULL COMMENT '报表描述',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_rpt_name`(`name`) USING BTREE
) ENGINE = InnoDB COMMENT = '报表模板定义';

-- ----------------------------
-- Table structure for cm_user
-- ----------------------------
DROP TABLE IF EXISTS `cm_user`;
CREATE TABLE `cm_user`  (
  `id` bigint(20) NOT NULL COMMENT '用户标识',
  `acc` varchar(32) NULL DEFAULT NULL COMMENT '账号，唯一',
  `phone` varchar(16) NULL DEFAULT NULL COMMENT '手机号，唯一',
  `pwd` char(32) NOT NULL COMMENT '密码的md5',
  `name` varchar(32) NULL DEFAULT NULL COMMENT '姓名',
  `photo` varchar(255) NULL DEFAULT NULL DEFAULT '' COMMENT '头像',
  `expired` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否停用',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_user_acc`(`acc`) USING BTREE,
  INDEX `idx_user_phone`(`phone`) USING BTREE
) ENGINE = InnoDB COMMENT = '系统用户';

-- ----------------------------
-- Records of cm_user
-- ----------------------------
INSERT INTO `cm_user` VALUES (1, 'admin', '13511111111', 'b59c67bf196a4758191e42f76670ceba', '', '', 0, '2019-10-24 09:06:38', '2023-03-16 08:35:39');

-- ----------------------------
-- Table structure for cm_user_group
-- ----------------------------
DROP TABLE IF EXISTS `cm_user_group`;
CREATE TABLE `cm_user_group`  (
  `user_id` bigint(20) NOT NULL COMMENT '用户标识',
  `group_id` bigint(20) NOT NULL COMMENT '组标识',
  PRIMARY KEY (`user_id`, `group_id`) USING BTREE,
  INDEX `idx_user_group_userid`(`user_id`) USING BTREE,
  INDEX `idx_user_group_groupid`(`group_id`) USING BTREE,
  CONSTRAINT `fk_user_group_groupid` FOREIGN KEY (`group_id`) REFERENCES `cm_group` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_user_group_userid` FOREIGN KEY (`user_id`) REFERENCES `cm_user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '用户一组多对多';

-- ----------------------------
-- Table structure for cm_user_params
-- ----------------------------
DROP TABLE IF EXISTS `cm_user_params`;
CREATE TABLE `cm_user_params`  (
  `user_id` bigint(20) NOT NULL COMMENT '用户标识',
  `param_id` bigint(20) NOT NULL COMMENT '参数标识',
  `value` varchar(255) NULL DEFAULT NULL COMMENT '参数值',
  `mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`user_id`, `param_id`) USING BTREE,
  INDEX `idx_user_params_userid`(`user_id`) USING BTREE,
  INDEX `idx_user_params_paramsid`(`param_id`) USING BTREE,
  CONSTRAINT `fk_user_params_paramsid` FOREIGN KEY (`param_id`) REFERENCES `cm_params` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_user_params_userid` FOREIGN KEY (`user_id`) REFERENCES `cm_user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '用户参数值';

-- ----------------------------
-- Table structure for cm_user_role
-- ----------------------------
DROP TABLE IF EXISTS `cm_user_role`;
CREATE TABLE `cm_user_role`  (
  `user_id` bigint(20) NOT NULL COMMENT '用户标识',
  `role_id` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`user_id`, `role_id`) USING BTREE,
  INDEX `idx_user_role_userid`(`user_id`) USING BTREE,
  INDEX `idx_user_role_roleid`(`role_id`) USING BTREE,
  CONSTRAINT `fk_user_role_roleid` FOREIGN KEY (`role_id`) REFERENCES `cm_role` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_user_role_userid` FOREIGN KEY (`user_id`) REFERENCES `cm_user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '用户一角色多对多';

-- ----------------------------
-- Records of cm_user_role
-- ----------------------------
INSERT INTO `cm_user_role` VALUES (1,2);

-- ----------------------------
-- Table structure for cm_wfd_atv
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfd_atv`;
CREATE TABLE `cm_wfd_atv`  (
  `id` bigint(20) NOT NULL COMMENT '活动标识',
  `prc_id` bigint(20) NOT NULL COMMENT '流程标识',
  `name` varchar(64) NOT NULL COMMENT '活动名称，同时作为状态名称',
  `type` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfdAtvType#活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动',
  `exec_scope` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfdAtvExecScope#执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户',
  `exec_limit` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfdAtvExecLimit#执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者',
  `exec_atv_id` bigint(20) NULL DEFAULT NULL COMMENT '在执行者限制为3或4时选择的活动',
  `auto_accept` tinyint(1) NOT NULL COMMENT '是否自动签收，打开工作流视图时自动签收工作项',
  `can_delete` tinyint(1) NOT NULL COMMENT '能否删除流程实例和业务数据，0否 1',
  `can_terminate` tinyint(1) NOT NULL COMMENT '能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能',
  `can_jump_into` tinyint(1) NOT NULL COMMENT '是否可作为跳转目标，0不可跳转 1可以',
  `trans_kind` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfdAtvTransKind#当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择',
  `join_kind` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfdAtvJoinKind#同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_wfd_atv_prcid`(`prc_id`) USING BTREE,
  CONSTRAINT `fk_wfd_atv_prcid` FOREIGN KEY (`prc_id`) REFERENCES `cm_wfd_prc` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '活动模板';

-- ----------------------------
-- Table structure for cm_wfd_atv_role
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfd_atv_role`;
CREATE TABLE `cm_wfd_atv_role`  (
  `atv_id` bigint(20) NOT NULL COMMENT '活动标识',
  `role_id` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`atv_id`, `role_id`) USING BTREE,
  INDEX `idx_wfd_atv_role_roleid`(`role_id`) USING BTREE,
  CONSTRAINT `fk_wfd_atv_role_atvid` FOREIGN KEY (`atv_id`) REFERENCES `cm_wfd_atv` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfd_atv_role_roleid` FOREIGN KEY (`role_id`) REFERENCES `cm_role` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '活动授权';

-- ----------------------------
-- Table structure for cm_wfd_prc
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfd_prc`;
CREATE TABLE `cm_wfd_prc`  (
  `id` bigint(20) NOT NULL COMMENT '流程标识',
  `name` varchar(64) NOT NULL COMMENT '流程名称',
  `diagram` varchar(21000) NULL DEFAULT NULL COMMENT '流程图',
  `is_locked` tinyint(1) NOT NULL COMMENT '锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行',
  `singleton` tinyint(1) NOT NULL COMMENT '同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例',
  `note` varchar(255) NULL DEFAULT NULL COMMENT '描述',
  `dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '最后修改时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB COMMENT = '流程模板';

-- ----------------------------
-- Table structure for cm_wfd_trs
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfd_trs`;
CREATE TABLE `cm_wfd_trs`  (
  `id` bigint(20) NOT NULL COMMENT '迁移标识',
  `prc_id` bigint(20) NOT NULL COMMENT '流程模板标识',
  `src_atv_id` bigint(20) NOT NULL COMMENT '起始活动模板标识',
  `tgt_atv_id` bigint(20) NOT NULL COMMENT '目标活动模板标识',
  `is_rollback` tinyint(1) NOT NULL COMMENT '是否为回退迁移',
  `trs_id` bigint(20) NULL DEFAULT NULL COMMENT '类别为回退迁移时对应的常规迁移标识',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_wfd_trs_prcid`(`prc_id`) USING BTREE,
  CONSTRAINT `fk_wfd_trs_prcid` FOREIGN KEY (`prc_id`) REFERENCES `cm_wfd_prc` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '迁移模板';

-- ----------------------------
-- Table structure for cm_wfi_atv
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfi_atv`;
CREATE TABLE `cm_wfi_atv`  (
  `id` bigint(20) NOT NULL COMMENT '活动实例标识',
  `prci_id` bigint(20) NOT NULL COMMENT '流程实例标识',
  `atvd_id` bigint(20) NOT NULL COMMENT '活动模板标识',
  `status` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfiAtvStatus#活动实例的状态 0活动 1结束 2终止 3同步活动',
  `inst_count` int(11) NOT NULL COMMENT '活动实例在流程实例被实例化的次数',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_wfi_atv_prciid`(`prci_id`) USING BTREE,
  INDEX `idx_wfi_atv_atvdid`(`atvd_id`) USING BTREE,
  CONSTRAINT `fk_wfi_atv_atvdid` FOREIGN KEY (`atvd_id`) REFERENCES `cm_wfd_atv` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_wfi_atv_prciid` FOREIGN KEY (`prci_id`) REFERENCES `cm_wfi_prc` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '活动实例';

-- ----------------------------
-- Table structure for cm_wfi_item
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfi_item`;
CREATE TABLE `cm_wfi_item`  (
  `id` bigint(20) NOT NULL COMMENT '工作项标识',
  `atvi_id` bigint(20) NOT NULL COMMENT '活动实例标识',
  `status` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfiItemStatus#工作项状态 0活动 1结束 2终止 3同步活动',
  `assign_kind` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfiItemAssignKind#指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派',
  `sender_id` bigint(20) NULL DEFAULT NULL COMMENT '发送者标识',
  `sender` varchar(32) NULL DEFAULT NULL COMMENT '发送者姓名',
  `stime` datetime NOT NULL COMMENT '发送时间',
  `is_accept` tinyint(1) NOT NULL COMMENT '是否签收此项任务',
  `accept_time` datetime NULL DEFAULT NULL COMMENT '签收时间',
  `role_id` bigint(20) NULL DEFAULT NULL COMMENT '执行者角色标识',
  `user_id` bigint(20) NULL DEFAULT NULL COMMENT '执行者用户标识',
  `note` varchar(255) NULL DEFAULT NULL COMMENT '工作项备注',
  `dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_wfi_item_atviid`(`atvi_id`) USING BTREE,
  CONSTRAINT `fk_wfi_item_atviid` FOREIGN KEY (`atvi_id`) REFERENCES `cm_wfi_atv` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '工作项';

-- ----------------------------
-- Table structure for cm_wfi_prc
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfi_prc`;
CREATE TABLE `cm_wfi_prc`  (
  `id` bigint(20) NOT NULL COMMENT '流程实例标识，同时为业务数据主键',
  `prcd_id` bigint(20) NOT NULL COMMENT '流程模板标识',
  `name` varchar(255) NOT NULL COMMENT '流转单名称',
  `status` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfiPrcStatus#流程实例状态 0活动 1结束 2终止',
  `dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_wfi_prc_prcdid`(`prcd_id`) USING BTREE,
  CONSTRAINT `fk_wfi_prc_prcdid` FOREIGN KEY (`prcd_id`) REFERENCES `cm_wfd_prc` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB COMMENT = '流程实例';

-- ----------------------------
-- Table structure for cm_wfi_trs
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfi_trs`;
CREATE TABLE `cm_wfi_trs`  (
  `id` bigint(20) NOT NULL COMMENT '迁移实例标识',
  `trsd_id` bigint(20) NOT NULL COMMENT '迁移模板标识',
  `src_atvi_id` bigint(20) NOT NULL COMMENT '起始活动实例标识',
  `tgt_atvi_id` bigint(20) NOT NULL COMMENT '目标活动实例标识',
  `is_rollback` tinyint(1) NOT NULL COMMENT '是否为回退迁移，1表回退',
  `ctime` datetime NOT NULL COMMENT '迁移时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_wfi_trs_trsdid`(`trsd_id`) USING BTREE,
  INDEX `idx_wfi_trs_srcatviid`(`src_atvi_id`) USING BTREE,
  INDEX `idx_wfi_trs_tgtatviid`(`tgt_atvi_id`) USING BTREE,
  CONSTRAINT `fk_wfi_trs_srcatviid` FOREIGN KEY (`src_atvi_id`) REFERENCES `cm_wfi_atv` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfi_trs_tgtatviid` FOREIGN KEY (`tgt_atvi_id`) REFERENCES `cm_wfi_atv` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfi_trs_trsdid` FOREIGN KEY (`trsd_id`) REFERENCES `cm_wfd_trs` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '迁移实例';

-- ----------------------------
-- Table structure for fsm_file
-- ----------------------------
DROP TABLE IF EXISTS `fsm_file`;
CREATE TABLE `fsm_file`  (
  `id` bigint(20) UNSIGNED NOT NULL COMMENT '文件标识',
  `name` varchar(512) NOT NULL COMMENT '文件名称',
  `path` varchar(512) NOT NULL COMMENT '存放路径：卷/两级目录/id.ext',
  `size` bigint(20) UNSIGNED NOT NULL COMMENT '文件长度',
  `info` varchar(512) NULL DEFAULT NULL COMMENT '文件描述',
  `uploader` bigint(20) UNSIGNED NOT NULL COMMENT '上传人id',
  `ctime` datetime NOT NULL COMMENT '上传时间',
  `downloads` bigint(20) UNSIGNED NOT NULL COMMENT '下载次数',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_fsm_file_path`(`path`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Table structure for sequence
-- ----------------------------
DROP TABLE IF EXISTS `sequence`;
CREATE TABLE `sequence`  (
  `id` varchar(64) NOT NULL COMMENT '序列名称',
  `val` int(11) NOT NULL COMMENT '序列的当前值',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB COMMENT = '模拟Sequence';

-- ----------------------------
-- Records of sequence
-- ----------------------------
INSERT INTO `sequence` VALUES ('cm_menu_dispidx', 89), ('cm_option_dispidx', 1031), ('cm_wfd_prc_dispidx', 11), ('cm_wfi_item_dispidx', 176), ('cm_wfi_prc_dispidx', 65);

-- ----------------------------
-- Function structure for nextval
-- ----------------------------
DROP FUNCTION IF EXISTS `nextval`;
CREATE FUNCTION `nextval`(v_seq_name VARCHAR ( 200 ))
 RETURNS int(11)
BEGIN
DECLARE
	res INTEGER;
UPDATE sequence 
SET val = val + 1 
WHERE
	id = v_seq_name;

SET res = 0;
SELECT
	val INTO res 
FROM
	sequence 
WHERE
	id = v_seq_name;
RETURN res;

END
;

SET FOREIGN_KEY_CHECKS = 1;

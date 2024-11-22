SET NAMES utf8;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for cm_cache
-- ----------------------------
DROP TABLE IF EXISTS `cm_cache`;
CREATE TABLE `cm_cache` (
  `id` varchar(255) NOT NULL,
  `val` varchar(512) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB COMMENT = '模拟redis缓存key value数据，直连数据库时用';

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
) ENGINE = InnoDB COMMENT = '用户组，与用户和角色多对多';

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
(93146668397260800, NULL, '基础维护', 1, NULL, NULL, NULL, NULL, 1, 0, '2024-06-14 08:51:37', '2024-06-14 08:51:37'),
(93147399237955584, 93146668397260800, '部门管理', 0, '部门管理', NULL, '多人', NULL, 115, 0, '2024-06-14 08:54:32', '2024-06-14 08:54:32'),
(93147789455028224, 93146668397260800, '人员管理', 0, '人员管理', NULL, '个人信息', NULL, 117, 0, '2024-06-14 08:56:05', '2024-06-14 08:56:05'),
(95003376719523840, 93146668397260800, '供应商管理', 0, '供应商管理', NULL, '全局', NULL, 119, 0, '2024-06-19 11:49:30', '2024-06-19 11:49:30'),
(96885816660619264, NULL, '物资管理', 1, NULL, NULL, NULL, NULL, 122, 0, '2024-06-24 16:29:45', '2024-06-24 16:29:45'),
(1, NULL, '工作台', 1, '', NULL, '搬运工', NULL, 123, 0, '2019-03-07 10:45:44', '2019-03-07 10:45:43'),
(97869834403213312, NULL, '测试组', 1, NULL, NULL, NULL, NULL, 130, 0, '2024-06-27 09:39:50', '2024-06-27 09:39:50'),
(97869954830069760, 97869834403213312, '一级菜单1', 0, NULL, NULL, '文件', NULL, 131, 0, '2024-06-27 09:40:18', '2024-06-27 09:40:18'),
(97870059381485568, 97869834403213312, '一级菜单2', 0, NULL, NULL, '文件', NULL, 132, 0, '2024-06-27 09:40:43', '2024-06-27 09:40:43'),
(97870113269903360, 97869834403213312, '二级组', 1, NULL, NULL, NULL, NULL, 133, 0, '2024-06-27 09:40:56', '2024-06-27 09:40:56'),
(97870286377218048, 97870113269903360, '二级菜单1', 0, NULL, NULL, '文件', NULL, 134, 0, '2024-06-27 09:41:37', '2024-06-27 09:41:37'),
(97870350000615424, 97870113269903360, '二级菜单2', 0, NULL, NULL, '文件', NULL, 135, 0, '2024-06-27 09:41:52', '2024-06-27 09:41:52'),
(97871217135218688, 97870113269903360, '三级组', 1, NULL, NULL, NULL, NULL, 136, 0, '2024-06-27 09:45:19', '2024-06-27 09:45:19'),
(97871290111913984, 97871217135218688, '三级菜单', 0, NULL, NULL, '文件', NULL, 137, 0, '2024-06-27 09:45:37', '2024-06-27 09:45:37'),
(105150016726003712, 93146668397260800, '物资入出类别', 0, '物资入出类别', NULL, '分组', NULL, 138, 0, '2024-07-17 11:48:40', '2024-07-17 11:48:40'),
(95004558183657472, 93146668397260800, '物资目录管理', 0, '物资目录管理', NULL, '树形', NULL, 121, 0, '2024-06-19 11:54:11', '2024-06-19 11:54:11'),
(3, 1, '用户组', 0, '用户组', NULL, '分组', '管理用户组、组内用户，为用户组分配角色', 3, 0, '2023-03-10 08:34:49', '2023-03-10 08:34:49'),
(5, 1, '基础权限', 0, '基础权限', NULL, '审核', '按照模块和功能两级目录管理权限、将权限分配给角色', 5, 0, '2019-03-12 09:11:22', '2019-03-07 11:23:40'),
(6, 1, '菜单管理', 0, '菜单管理', NULL, '大图标', '菜单和菜单组管理、将菜单授权给角色', 6, 0, '2019-03-11 11:35:59', '2019-03-11 11:35:58'),
(96886018188537856, 96885816660619264, '物资入出管理', 0, '物资入出', NULL, '四面体', NULL, 124, 0, '2024-06-24 16:30:33', '2024-06-24 16:30:33'),
(96889439553613824, 96885816660619264, '物资盘存管理', 0, '物资盘存', NULL, '文件', NULL, 128, 0, '2024-06-24 16:44:09', '2024-06-24 16:44:09'),
(96889910070636544, 96885816660619264, '物资计划管理', 0, '物资计划', NULL, '外设', NULL, 129, 0, '2024-06-24 16:46:01', '2024-06-24 16:46:01'),
(7, 1, '报表设计', 0, '报表设计', NULL, '折线图', '报表管理及报表模板设计', 7, 0, '2020-10-19 11:21:38', '2020-10-19 11:21:38'),
(2, 1, '用户账号', 0, '用户账号', NULL, '钥匙', '用户账号及所属用户组管理、为用户分配角色、查看用户可访问菜单和已授权限', 2, 0, '2019-11-08 11:42:28', '2019-11-08 11:43:53'),
(9, 1, '参数定义', 0, '参数定义', NULL, '调色板', '参数名称、默认值的定义管理', 9, 0, '2019-03-12 15:35:56', '2019-03-12 15:37:10'),
(4, 1, '系统角色', 0, '系统角色', NULL, '两人', '角色管理、为用户和用户组分配角色、设置角色可访问菜单、授予权限', 4, 0, '2019-11-08 11:47:21', '2019-11-08 11:48:22'),
(10, 1, '基础选项', 0, '基础选项', NULL, '修理', '按照分组管理的选项列表，如民族、学历等静态列表', 10, 0, '2019-11-08 11:49:40', '2019-11-08 11:49:46'),
(8, 1, '流程设计', 0, '流程设计', NULL, '双绞线', '流程模板设计及流程实例查询', 8, 0, '2020-11-02 16:21:19', '2020-11-02 16:21:19');

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
INSERT INTO `cm_permission_module` VALUES (1, '系统预留', '系统内部使用的权限控制，禁止删除'), (87433840629673984, '物资管理', NULL);

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
INSERT INTO `cm_permission_func` VALUES (1, 1, '文件管理', '管理文件的上传、删除等'), (87433900117487616, 87433840629673984, '入出', NULL);

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
INSERT INTO `cm_permission` VALUES (1, 1, '公共文件增删', '公共文件的上传、删除等'), (2, 1, '素材库增删', '素材库目录的上传、删除等'), (87434002596917248, 87433900117487616, '冲销', NULL);

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
INSERT INTO `cm_role` VALUES (1, '任何人', '所有用户默认都具有该角色，不可删除'), (2, '系统管理员', '系统角色，不可删除'), (87363447483035648, '库管员', NULL), (87368228331089920, '库主管', NULL);

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
INSERT INTO `cm_role_menu` VALUES (2, 93147399237955584), (2, 93147789455028224), (2, 95003376719523840), (2, 95004558183657472), (1, 97869954830069760), (1, 97870059381485568), (1, 97870350000615424), (1, 97870286377218048), (1, 97871290111913984), (1, 96886018188537856), (1, 96889439553613824), (1, 96889910070636544), (2, 105150016726003712), (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (2, 7), (2, 8), (2, 9), (2, 10);

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
INSERT INTO `cm_role_per` VALUES (2, 1), (2, 2), (87368228331089920, 1), (87363447483035648, 87434002596917248);

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
INSERT INTO `cm_user` VALUES (1, 'admin', '13511111111', 'b59c67bf196a4758191e42f76670ceba', NULL, '', 0, '2019-10-24 09:06:38', '2024-05-30 09:38:24'), (87375101197316096, 'kzg1', '13511113333', 'b59c67bf196a4758191e42f76670ceba', '', NULL, 0, '2024-05-29 10:37:34', '2024-06-25 15:30:49'), (97233424511954944, 'kgy2', NULL, 'b59c67bf196a4758191e42f76670ceba', NULL, NULL, 0, '2024-06-25 15:31:09', '2024-06-25 15:31:09'), (97233490068926464, 'kgy3', NULL, 'b59c67bf196a4758191e42f76670ceba', NULL, NULL, 0, '2024-06-25 15:31:18', '2024-06-25 15:31:18'), (97233514383306752, 'kgy4', NULL, 'b59c67bf196a4758191e42f76670ceba', NULL, NULL, 0, '2024-06-25 15:31:24', '2024-06-25 15:31:24'), (97233573971783680, 'kzg2', NULL, 'b59c67bf196a4758191e42f76670ceba', NULL, NULL, 0, '2024-06-25 15:31:37', '2024-06-25 15:31:37'), (87374677803298816, 'kgy1', '13511112222', 'b59c67bf196a4758191e42f76670ceba', NULL, NULL, 0, '2024-05-29 10:35:53', '2024-07-01 14:55:34');

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
INSERT INTO `cm_user_role` VALUES (1, 2), (87374677803298816, 87363447483035648), (87375101197316096, 87368228331089920), (97233573971783680, 87368228331089920), (97233514383306752, 87363447483035648), (97233490068926464, 87363447483035648), (97233424511954944, 87363447483035648), (1, 87363447483035648), (1, 87368228331089920);

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

INSERT INTO `cm_wfd_atv` VALUES (96767714337779712, 96767646822068224, '完成', 3, 0, 0, NULL, 1, 0, 0, 0, 0, 0, '2024-06-24 08:40:27', '2024-06-24 08:40:27'), (96767673514618880, 96767646822068224, '填写', 1, 0, 0, NULL, 1, 1, 0, 0, 2, 0, '2024-06-24 08:40:17', '2024-06-25 15:32:48'), (96767684025544704, 96767646822068224, '审核', 0, 2, 0, NULL, 1, 0, 0, 0, 0, 0, '2024-06-24 08:40:20', '2024-06-25 15:33:26');

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

INSERT INTO `cm_wfd_atv_role` VALUES (96767673514618880, 87363447483035648), (96767684025544704, 87368228331089920);

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

INSERT INTO `cm_wfd_prc` VALUES (96767646822068224, '物资入出', '<Sketch><Node id=\"96767673514618880\" title=\"填写\" shape=\"开始\" left=\"460\" top=\"60\" width=\"80\" height=\"60\" /><Node id=\"96767684025544704\" title=\"审核\" shape=\"任务\" left=\"440\" top=\"200\" width=\"120\" height=\"60\" /><Line id=\"96767701062807552\" headerid=\"96767673514618880\" bounds=\"490,120,20,80\" headerport=\"4\" tailid=\"96767684025544704\" tailport=\"0\" /><Node id=\"96767714337779712\" title=\"完成\" shape=\"结束\" background=\"#FF9D9D9D\" borderbrush=\"#FF969696\" left=\"460\" top=\"340\" width=\"80\" height=\"60\" /><Line id=\"96767731547009024\" headerid=\"96767684025544704\" bounds=\"490,260,20,80\" headerport=\"4\" tailid=\"96767714337779712\" tailport=\"0\" /></Sketch>', 0, 0, NULL, 13, '2024-06-24 08:40:11', '2024-07-05 09:20:34');

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

INSERT INTO `cm_wfd_trs` VALUES (96767701062807552, 96767646822068224, 96767673514618880, 96767684025544704, 0, NULL), (96767731547009024, 96767646822068224, 96767684025544704, 96767714337779712, 0, NULL), (100764090209955840, 96767646822068224, 96767684025544704, 96767673514618880, 1, 96767701062807552);

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
INSERT INTO `sequence` VALUES
('cm_menu_dispidx', 138),
('cm_option_dispidx', 1050),
('cm_wfd_prc_dispidx', 15),
('cm_wfi_item_dispidx', 258),
('crud_基础_序列"', 85),
('物资主单_单号', 11),
('物资入出类别_id', 12),
('cm_wfi_prc_dispidx', 81);

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


-- ----------------------------
-- Table structure for crud_主表
-- ----------------------------
DROP TABLE IF EXISTS `crud_主表`;
CREATE TABLE `crud_主表`  (
  `id` bigint NOT NULL,
  `主表名称` varchar(255) NULL,
  `限长4` varchar(16) NULL COMMENT '限制最大长度4',
  `不重复` varchar(255) NULL COMMENT '列值无重复',
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of crud_主表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_基础
-- ----------------------------
DROP TABLE IF EXISTS `crud_基础`;
CREATE TABLE `crud_基础`  (
  `id` bigint NOT NULL COMMENT '标识',
  `parent_id` bigint NULL COMMENT '上级id，演示树状结构',
  `序列` int NOT NULL COMMENT '序列自动赋值',
  `名称` varchar(255) NULL,
  `限长4` varchar(16) NULL COMMENT '限制最大长度4',
  `不重复` varchar(64) NULL COMMENT '列值无重复',
  `禁止选中` tinyint(1) NOT NULL COMMENT '始终为false',
  `禁止保存` tinyint(1) NOT NULL COMMENT 'true时保存前校验不通过',
  `禁止删除` tinyint(1) NOT NULL COMMENT 'true时删除前校验不通过',
  `值变事件` varchar(64) NULL COMMENT '每次值变化时触发领域事件',
  `发布插入事件` tinyint(1) NOT NULL COMMENT 'true时允许发布插入事件',
  `发布删除事件` tinyint(1) NOT NULL COMMENT 'true时允许发布删除事件',
  `创建时间` datetime NOT NULL COMMENT '初次创建时间',
  `修改时间` datetime NOT NULL COMMENT '最后修改时间',
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of crud_基础
-- ----------------------------

-- ----------------------------
-- Table structure for crud_大儿
-- ----------------------------
DROP TABLE IF EXISTS `crud_大儿`;
CREATE TABLE `crud_大儿`  (
  `id` bigint NOT NULL,
  `parent_id` bigint NOT NULL,
  `大儿名` varchar(255) NULL,
  PRIMARY KEY (`id`),
  INDEX `idx_大儿_parendid`(`parent_id` ASC) USING BTREE,
  CONSTRAINT `fk_大儿_parendid` FOREIGN KEY (`parent_id`) REFERENCES `crud_父表` (`id`)
);

-- ----------------------------
-- Records of crud_大儿
-- ----------------------------

-- ----------------------------
-- Table structure for crud_字段类型
-- ----------------------------
DROP TABLE IF EXISTS `crud_字段类型`;
CREATE TABLE `crud_字段类型`  (
  `id` bigint NOT NULL,
  `字符串` varchar(255) NULL,
  `整型` int NOT NULL,
  `可空整型` int NULL,
  `长整型` bigint NULL,
  `布尔` tinyint(1) NOT NULL,
  `可空布尔` tinyint(1) NULL,
  `日期时间` datetime NOT NULL,
  `可空时间` datetime NULL,
  `枚举` smallint NOT NULL COMMENT '#Gender#性别',
  `可空枚举` smallint NULL COMMENT '#Gender#性别',
  `单精度` float NOT NULL,
  `可空单精度` float NULL,
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of crud_字段类型
-- ----------------------------

-- ----------------------------
-- Table structure for crud_小儿
-- ----------------------------
DROP TABLE IF EXISTS `crud_小儿`;
CREATE TABLE `crud_小儿`  (
  `id` bigint NOT NULL,
  `group_id` bigint NOT NULL,
  `小儿名` varchar(255) NULL,
  PRIMARY KEY (`id`),
  INDEX `idx_小儿_parentid`(`group_id` ASC) USING BTREE,
  CONSTRAINT `fk_小儿_parentid` FOREIGN KEY (`group_id`) REFERENCES `crud_父表` (`id`)
);

-- ----------------------------
-- Records of crud_小儿
-- ----------------------------

-- ----------------------------
-- Table structure for crud_扩展1
-- ----------------------------
DROP TABLE IF EXISTS `crud_扩展1`;
CREATE TABLE `crud_扩展1`  (
  `id` bigint NOT NULL COMMENT '标识',
  `扩展1名称` varchar(255) NULL,
  `禁止选中` tinyint(1) NOT NULL COMMENT '始终为false',
  `禁止保存` tinyint(1) NOT NULL COMMENT 'true时保存前校验不通过',
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of crud_扩展1
-- ----------------------------

-- ----------------------------
-- Table structure for crud_扩展2
-- ----------------------------
DROP TABLE IF EXISTS `crud_扩展2`;
CREATE TABLE `crud_扩展2`  (
  `id` bigint NOT NULL COMMENT '标识',
  `扩展2名称` varchar(255) NULL,
  `禁止删除` tinyint(1) NOT NULL COMMENT 'true时删除前校验不通过',
  `值变事件` varchar(255) NULL COMMENT '每次值变化时触发领域事件',
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of crud_扩展2
-- ----------------------------

-- ----------------------------
-- Table structure for crud_权限
-- ----------------------------
DROP TABLE IF EXISTS `crud_权限`;
CREATE TABLE `crud_权限`  (
  `id` bigint NOT NULL COMMENT '权限名称',
  `权限名称` varchar(255) NULL,
  PRIMARY KEY (`id`)
) COMMENT = '权限';

-- ----------------------------
-- Records of crud_权限
-- ----------------------------

-- ----------------------------
-- Table structure for crud_父表
-- ----------------------------
DROP TABLE IF EXISTS `crud_父表`;
CREATE TABLE `crud_父表`  (
  `id` bigint NOT NULL,
  `父名` varchar(255) NULL,
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of crud_父表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_用户
-- ----------------------------
DROP TABLE IF EXISTS `crud_用户`;
CREATE TABLE `crud_用户`  (
  `id` bigint NOT NULL COMMENT '用户标识',
  `手机号` char(11) NULL COMMENT '手机号，唯一',
  `姓名` varchar(32) NULL COMMENT '姓名',
  `密码` char(32) NULL COMMENT '密码的md5',
  PRIMARY KEY (`id`)
) COMMENT = '系统用户';

-- ----------------------------
-- Records of crud_用户
-- ----------------------------

-- ----------------------------
-- Table structure for crud_用户角色
-- ----------------------------
DROP TABLE IF EXISTS `crud_用户角色`;
CREATE TABLE `crud_用户角色`  (
  `user_id` bigint NOT NULL COMMENT '用户标识',
  `role_id` bigint NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`user_id`, `role_id`),
  INDEX `idx_crud_用户角色_roleid`(`role_id` ASC) USING BTREE,
  INDEX `idx_crud_用户角色_userid`(`user_id` ASC) USING BTREE,
  CONSTRAINT `fk_crud_用户角色_roleid` FOREIGN KEY (`role_id`) REFERENCES `crud_角色` (`id`),
  CONSTRAINT `fk_crud_用户角色_userid` FOREIGN KEY (`user_id`) REFERENCES `crud_用户` (`id`)
) COMMENT = '用户关联的角色';

-- ----------------------------
-- Records of crud_用户角色
-- ----------------------------

-- ----------------------------
-- Table structure for crud_缓存表
-- ----------------------------
DROP TABLE IF EXISTS `crud_缓存表`;
CREATE TABLE `crud_缓存表`  (
  `id` bigint NOT NULL,
  `手机号` char(11) NULL,
  `姓名` varchar(32) NULL,
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of crud_缓存表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_角色
-- ----------------------------
DROP TABLE IF EXISTS `crud_角色`;
CREATE TABLE `crud_角色`  (
  `id` bigint NOT NULL COMMENT '角色标识',
  `角色名称` varchar(32) NULL COMMENT '角色名称',
  `角色描述` varchar(255) NULL COMMENT '角色描述',
  PRIMARY KEY (`id`)
) COMMENT = '角色';

-- ----------------------------
-- Records of crud_角色
-- ----------------------------

-- ----------------------------
-- Table structure for crud_角色权限
-- ----------------------------
DROP TABLE IF EXISTS `crud_角色权限`;
CREATE TABLE `crud_角色权限`  (
  `role_id` bigint NOT NULL COMMENT '角色标识',
  `prv_id` bigint NOT NULL COMMENT '权限标识',
  PRIMARY KEY (`role_id`, `prv_id`),
  INDEX `idx_crud_角色权限_prvid`(`prv_id` ASC) USING BTREE,
  INDEX `idx_crud_角色权限_roleid`(`role_id` ASC) USING BTREE,
  CONSTRAINT `fk_角色权限_prvid` FOREIGN KEY (`prv_id`) REFERENCES `crud_权限` (`id`),
  CONSTRAINT `fk_角色权限_roleid` FOREIGN KEY (`role_id`) REFERENCES `crud_角色` (`id`)
) COMMENT = '角色关联的权限';

-- ----------------------------
-- Records of crud_角色权限
-- ----------------------------

-- ----------------------------
-- Table structure for 人员
-- ----------------------------
DROP TABLE IF EXISTS `人员`;
CREATE TABLE `人员`  (
  `id` bigint NOT NULL,
  `姓名` varchar(32) NULL,
  `出生日期` datetime NULL,
  `性别` smallint NULL COMMENT '#Gender#',
  `工作日期` datetime NULL,
  `办公室电话` varchar(32) NULL,
  `电子邮件` varchar(32) NULL,
  `建档时间` datetime NULL,
  `撤档时间` datetime NULL,
  `撤档原因` varchar(64) NULL,
  `user_id` bigint NULL COMMENT '账号ID',
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of 人员
-- ----------------------------
INSERT INTO `人员` VALUES (93233663974862848, '王库管', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 87374677803298816), (93233694710722560, '张主管', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 87375101197316096), (100436029211963392, '测试员', NULL, NULL, NULL, NULL, NULL, '2024-07-04 11:37:09', NULL, NULL, 1);

-- ----------------------------
-- Table structure for 供应商
-- ----------------------------
DROP TABLE IF EXISTS `供应商`;
CREATE TABLE `供应商`  (
  `id` bigint NOT NULL,
  `名称` varchar(64) NULL,
  `执照号` varchar(32) NULL,
  `执照效期` datetime NULL,
  `税务登记号` varchar(32) NULL,
  `地址` varchar(128) NULL,
  `电话` varchar(16) NULL,
  `开户银行` varchar(64) NULL,
  `帐号` varchar(32) NULL,
  `联系人` varchar(32) NULL,
  `建档时间` datetime NULL,
  `撤档时间` datetime NULL,
  `备注` varchar(255) NULL,
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of 供应商
-- ----------------------------
INSERT INTO `供应商` VALUES (95034724012290048, '物资东厂', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2024-06-19 13:54:37', NULL, NULL), (95034312534290432, '仁和制药', NULL, NULL, NULL, NULL, '13698562456', NULL, NULL, NULL, '2024-06-19 13:53:52', NULL, NULL);

-- ----------------------------
-- Table structure for 物资主单
-- ----------------------------
DROP TABLE IF EXISTS `物资主单`;
CREATE TABLE `物资主单`  (
  `id` bigint NOT NULL,
  `部门id` bigint NOT NULL,
  `入出类别id` bigint NOT NULL,
  `状态` smallint NOT NULL COMMENT '#单据状态#0-填写;1-待审核;2-已审核;3-被冲销;4-冲销',
  `单号` varchar(8) NOT NULL COMMENT '相同单号可以不同的冲销状态，命名：前缀+连续序号',
  `摘要` varchar(64) NULL,
  `填制人` varchar(32) NULL COMMENT '如果是申领单，表示申领人',
  `填制日期` datetime NULL,
  `审核人` varchar(32) NULL,
  `审核日期` datetime NULL,
  `入出系数` smallint NULL COMMENT '1:物资入,-1:物资出;0-盘点记录单',
  `供应商id` bigint NULL COMMENT '外购入库时填写',
  `发料人` varchar(32) NULL COMMENT '申请单时用效,主要反应该张单据什么人发的料',
  `发料日期` datetime NULL COMMENT '申请单时用效',
  `金额` float NULL COMMENT '单据内所有详单的金额和',
  `发票金额` float NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_物资主单_供应商` FOREIGN KEY (`供应商id`) REFERENCES `供应商` (`id`),
  CONSTRAINT `fk_物资主单_入出类别` FOREIGN KEY (`入出类别id`) REFERENCES `物资入出类别` (`id`),
  CONSTRAINT `fk_物资主单_部门` FOREIGN KEY (`部门id`) REFERENCES `部门` (`id`)
);

-- ----------------------------
-- Records of 物资主单
-- ----------------------------

-- ----------------------------
-- Table structure for 物资入出类别
-- ----------------------------
DROP TABLE IF EXISTS `物资入出类别`;
CREATE TABLE `物资入出类别`  (
  `id` bigint NOT NULL,
  `名称` varchar(32) NOT NULL,
  `系数` smallint NOT NULL COMMENT '1-入库；-1-出库',
  `单号前缀` char(2) NOT NULL,
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of 物资入出类别
-- ----------------------------
INSERT INTO `物资入出类别` VALUES (1, '外购入库', 1, 'WG'), (2, '自产入库', 1, 'ZC'), (3, '返还入库', 1, 'FH'), (4, '盘盈', 1, 'PY'), (5, '申领出库', -1, 'SL'), (6, '物资报废', -1, 'BF'), (7, '盘亏', -1, 'PK');

-- ----------------------------
-- Table structure for 物资分类
-- ----------------------------
DROP TABLE IF EXISTS `物资分类`;
CREATE TABLE `物资分类`  (
  `id` bigint NOT NULL,
  `名称` varchar(64) NOT NULL,
  PRIMARY KEY (`id`)
);

-- ----------------------------
-- Records of 物资分类
-- ----------------------------
INSERT INTO `物资分类` VALUES (95413444640272384, '电工材料'), (95419313314623488, '劳保材料'), (95419350320967680, '水暖材料'), (95419395929829376, '维修材料'), (95419431795322880, '办公材料'), (95419477521625088, '低值易耗'), (95419514808987648, '易耗材料'), (95419598749593600, '其他材料');

-- ----------------------------
-- Table structure for 物资库存
-- ----------------------------
DROP TABLE IF EXISTS `物资库存`;
CREATE TABLE `物资库存`  (
  `id` bigint NOT NULL,
  `部门id` bigint NULL,
  `物资id` bigint NULL,
  `批次` varchar(16) NULL COMMENT '相同物资ID不同批次的物资独立计算库存，部门ID+物资ID+批次构成唯一索引',
  `可用数量` float NULL COMMENT '当填写申领单还未审批时只影响可用数量，确保后填写申领单时数量有效',
  `可用金额` float NULL,
  `实际数量` float NULL,
  `实际金额` float NULL,
  PRIMARY KEY (`id`),
  INDEX `ix_物资库存_物资id`(`物资id` ASC) USING BTREE,
  INDEX `ix_物资库存_部门id`(`部门id` ASC) USING BTREE,
  CONSTRAINT `fk_物资库存_物资` FOREIGN KEY (`物资id`) REFERENCES `物资目录` (`id`),
  CONSTRAINT `fk_物资库存_部门` FOREIGN KEY (`部门id`) REFERENCES `部门` (`id`)
);

-- ----------------------------
-- Records of 物资库存
-- ----------------------------

-- ----------------------------
-- Table structure for 物资目录
-- ----------------------------
DROP TABLE IF EXISTS `物资目录`;
CREATE TABLE `物资目录`  (
  `id` bigint NOT NULL,
  `分类id` bigint NULL,
  `名称` varchar(64) NULL,
  `规格` varchar(64) NULL COMMENT '计量单位，如 盒、10个/包、20个/箱、支',
  `产地` varchar(64) NULL COMMENT '名称,规格,产地构成唯一索引',
  `成本价` float NULL COMMENT '预估价格，库存计算金额用',
  `核算方式` smallint NULL COMMENT '#物资核算方式#一次性、分期摊销(折旧)',
  `摊销月数` int NULL COMMENT '当核算方式为分期摊销时的总月数',
  `建档时间` datetime NULL,
  `撤档时间` datetime NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_物资目录_分类` FOREIGN KEY (`分类id`) REFERENCES `物资分类` (`id`)
);

-- ----------------------------
-- Records of 物资目录
-- ----------------------------
INSERT INTO `物资目录` VALUES (104839509410344960, 95413444640272384, '电线', '米', '上海第一电线厂', NULL, NULL, NULL, '2024-07-16 15:15:05', NULL), (95434428013375488, 95413444640272384, '测电笔', '只', '江苏苏州电工工具厂', NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for 物资计划
-- ----------------------------
DROP TABLE IF EXISTS `物资计划`;
CREATE TABLE `物资计划`  (
  `id` bigint NOT NULL,
  `部门id` bigint NULL,
  `no` varchar(8) NULL,
  `计划类型` smallint NULL COMMENT '#计划类型#月;季;年',
  `编制方法` smallint NULL COMMENT '#计划编制方法#1-往年同期线性参照法,2-临近期间平均参照法,3-物资储备定额参照法,4-由部门申购计划产生',
  `摘要` varchar(64) NULL,
  `编制人` varchar(32) NULL,
  `编制日期` datetime NULL,
  `审核人` varchar(32) NULL,
  `审核日期` datetime NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_物资计划_部门` FOREIGN KEY (`部门id`) REFERENCES `部门` (`id`)
);

-- ----------------------------
-- Records of 物资计划
-- ----------------------------

-- ----------------------------
-- Table structure for 物资计划明细
-- ----------------------------
DROP TABLE IF EXISTS `物资计划明细`;
CREATE TABLE `物资计划明细`  (
  `计划id` bigint NOT NULL,
  `物资id` bigint NOT NULL,
  `前期数量` float NULL COMMENT '前年、上上月、前季度数量',
  `上期数量` float NULL COMMENT '去年、上个月、上季度数量',
  `库存数量` float NULL,
  `计划数量` float NULL,
  `审批数量` float NULL,
  `单价` float NULL,
  `金额` float NULL,
  `显示顺序` int NULL,
  PRIMARY KEY (`计划id`, `物资id`),
  CONSTRAINT `fk_物资计划明细_物资` FOREIGN KEY (`物资id`) REFERENCES `物资目录` (`id`),
  CONSTRAINT `fk_物资计划明细_计划` FOREIGN KEY (`计划id`) REFERENCES `物资计划` (`id`)
);

-- ----------------------------
-- Records of 物资计划明细
-- ----------------------------

-- ----------------------------
-- Table structure for 物资详单
-- ----------------------------
DROP TABLE IF EXISTS `物资详单`;
CREATE TABLE `物资详单`  (
  `id` bigint NOT NULL,
  `单据id` bigint NOT NULL,
  `物资id` bigint NULL,
  `序号` smallint NULL COMMENT '在一张单据内部从1连续编号，入出类别+冲销状态+单号+序号共同构成唯一索引',
  `批次` varchar(16) NULL,
  `数量` float NULL COMMENT '按散装单位填写',
  `单价` float NULL COMMENT '售价',
  `金额` float NULL COMMENT '实际数量与单价的乘积。',
  `随货单号` varchar(128) NULL COMMENT '外购入库时填写',
  `发票号` varchar(128) NULL COMMENT '外购入库时填写',
  `发票日期` datetime NULL COMMENT '外购入库时填写',
  `发票金额` float NULL COMMENT '外购入库时填写',
  `盘点时间` datetime NULL COMMENT '盘点有效',
  `盘点金额` float NULL COMMENT '盘点有效',
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_物资详单_单据` FOREIGN KEY (`单据id`) REFERENCES `物资主单` (`id`),
  CONSTRAINT `fk_物资详单_物资` FOREIGN KEY (`物资id`) REFERENCES `物资目录` (`id`)
);

-- ----------------------------
-- Records of 物资详单
-- ----------------------------

-- ----------------------------
-- Table structure for 部门
-- ----------------------------
DROP TABLE IF EXISTS `部门`;
CREATE TABLE `部门`  (
  `id` bigint NOT NULL,
  `上级id` bigint NULL,
  `编码` varchar(16) NULL,
  `名称` varchar(32) NULL,
  `说明` varchar(64) NULL COMMENT '位置、环境、备注等',
  `建档时间` datetime NULL,
  `撤档时间` datetime NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_部门_上级id` FOREIGN KEY (`上级id`) REFERENCES `部门` (`id`)
);

-- ----------------------------
-- Records of 部门
-- ----------------------------
INSERT INTO `部门` VALUES (93173171340210176, NULL, '01', '设备科', NULL, '2024-06-14 10:37:22', NULL), (93173345370271744, NULL, '02', '物资科', NULL, '2024-06-14 10:37:41', NULL), (93174118862843904, NULL, '03', '财务科', NULL, '2024-06-14 10:40:52', NULL);

-- ----------------------------
-- Table structure for 部门人员
-- ----------------------------
DROP TABLE IF EXISTS `部门人员`;
CREATE TABLE `部门人员`  (
  `部门id` bigint NOT NULL,
  `人员id` bigint NOT NULL,
  `缺省` tinyint(1) NULL COMMENT '当一个人员属于多个部门时，当前是否为缺省',
  PRIMARY KEY (`部门id`, `人员id`),
  CONSTRAINT `fk_部门人员_人员` FOREIGN KEY (`人员id`) REFERENCES `人员` (`id`),
  CONSTRAINT `fk_部门人员_部门` FOREIGN KEY (`部门id`) REFERENCES `部门` (`id`)
);

-- ----------------------------
-- Records of 部门人员
-- ----------------------------
INSERT INTO `部门人员` VALUES (93173345370271744, 93233663974862848, 1), (93173345370271744, 100436029211963392, 1);

-- ----------------------------
-- View structure for v_人员
-- ----------------------------
DROP VIEW IF EXISTS `v_人员`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_人员` AS select `a`.`id` AS `id`,`a`.`姓名` AS `姓名`,`a`.`出生日期` AS `出生日期`,`a`.`性别` AS `性别`,`a`.`工作日期` AS `工作日期`,`a`.`办公室电话` AS `办公室电话`,`a`.`电子邮件` AS `电子邮件`,`a`.`建档时间` AS `建档时间`,`a`.`撤档时间` AS `撤档时间`,`a`.`撤档原因` AS `撤档原因`,`a`.`user_id` AS `user_id`,coalesce(`b`.`name`,`b`.`acc`,`b`.`phone`) AS `账号` from (`人员` `a` left join `cm_user` `b` on((`a`.`user_id` = `b`.`id`)));

-- ----------------------------
-- View structure for v_物资主单
-- ----------------------------
DROP VIEW IF EXISTS `v_物资主单`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_物资主单` AS select `a`.`id` AS `id`,`a`.`部门id` AS `部门id`,`a`.`入出类别id` AS `入出类别id`,`a`.`状态` AS `状态`,`a`.`单号` AS `单号`,`a`.`摘要` AS `摘要`,`a`.`填制人` AS `填制人`,`a`.`填制日期` AS `填制日期`,`a`.`审核人` AS `审核人`,`a`.`审核日期` AS `审核日期`,`a`.`入出系数` AS `入出系数`,`a`.`供应商id` AS `供应商id`,`a`.`发料人` AS `发料人`,`a`.`发料日期` AS `发料日期`,`a`.`金额` AS `金额`,`a`.`发票金额` AS `发票金额`,`b`.`名称` AS `部门名称`,`c`.`名称` AS `供应商`,`d`.`名称` AS `入出类别` from (((`物资主单` `a` left join `部门` `b` on((`a`.`部门id` = `b`.`id`))) left join `供应商` `c` on((`a`.`供应商id` = `c`.`id`))) left join `物资入出类别` `d` on((`a`.`入出类别id` = `d`.`id`)));

-- ----------------------------
-- View structure for v_物资目录
-- ----------------------------
DROP VIEW IF EXISTS `v_物资目录`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_物资目录` AS select `a`.`id` AS `id`,`a`.`分类id` AS `分类id`,`a`.`名称` AS `名称`,`a`.`规格` AS `规格`,`a`.`产地` AS `产地`,`a`.`成本价` AS `成本价`,`a`.`核算方式` AS `核算方式`,`a`.`摊销月数` AS `摊销月数`,`a`.`建档时间` AS `建档时间`,`a`.`撤档时间` AS `撤档时间`,`b`.`名称` AS `物资分类` from (`物资目录` `a` left join `物资分类` `b` on((`a`.`分类id` = `b`.`id`)));

-- ----------------------------
-- View structure for v_物资详单
-- ----------------------------
DROP VIEW IF EXISTS `v_物资详单`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_物资详单` AS select `a`.`id` AS `id`,`a`.`单据id` AS `单据id`,`a`.`物资id` AS `物资id`,`a`.`序号` AS `序号`,`a`.`批次` AS `批次`,`a`.`数量` AS `数量`,`a`.`单价` AS `单价`,`a`.`金额` AS `金额`,`a`.`随货单号` AS `随货单号`,`a`.`发票号` AS `发票号`,`a`.`发票日期` AS `发票日期`,`a`.`发票金额` AS `发票金额`,`a`.`盘点时间` AS `盘点时间`,`a`.`盘点金额` AS `盘点金额`,`b`.`名称` AS `物资名称`,`b`.`规格` AS `规格`,`b`.`产地` AS `产地` from (`物资详单` `a` left join `物资目录` `b` on((`a`.`物资id` = `b`.`id`)));

-- ----------------------------
-- View structure for v_部门
-- ----------------------------
DROP VIEW IF EXISTS `v_部门`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_部门` AS select `a`.`id` AS `id`,`a`.`上级id` AS `上级id`,`a`.`编码` AS `编码`,`a`.`名称` AS `名称`,`a`.`说明` AS `说明`,`a`.`建档时间` AS `建档时间`,`a`.`撤档时间` AS `撤档时间`,`b`.`名称` AS `上级部门` from (`部门` `a` left join `部门` `b` on((`a`.`上级id` = `b`.`id`)));

SET FOREIGN_KEY_CHECKS = 1;

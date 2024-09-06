SET NAMES utf8;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for demo_cache_tbl1
-- ----------------------------
DROP TABLE IF EXISTS `demo_cache_tbl1`;
CREATE TABLE `demo_cache_tbl1`  (
  `id` bigint(20) NOT NULL,
  `phone` varchar(255) NOT NULL,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_cache_tbl1
-- ----------------------------
INSERT INTO `demo_cache_tbl1` VALUES (454454068519129088, 'ca4f271212bc4add946c55feed7400bb', '3917'), (484620968746045440, '3f435d84c76a46e29002f467a4cd0187', '7425'), (484621133057904640, '3329d521b2134b0195083828152cb5b0', '1786'), (484624179913576448, 'd80e785d1d44472abe88723e4ed17ca8', '156');

-- ----------------------------
-- Table structure for demo_child_tbl1
-- ----------------------------
DROP TABLE IF EXISTS `demo_child_tbl1`;
CREATE TABLE `demo_child_tbl1`  (
  `id` bigint(20) NOT NULL,
  `parent_id` bigint(20) NOT NULL,
  `item_name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_child_tbl1
-- ----------------------------
INSERT INTO `demo_child_tbl1` VALUES (443588385740705792, 443588385522601984, '修改370'), (443588388055961600, 443588385522601984, '修改370'), (443588388299231232, 443588385522601984, '修改370'), (443588583695077376, 443588583535693824, '新增0'), (443588583913181184, 443588583535693824, '新增1'), (443588584148062208, 443588583535693824, '新增2'), (443588895562551296, 443588895352836096, '新增0'), (443588895814209536, 443588895352836096, '新增1'), (443588896132976640, 443588895352836096, '新增2'), (443588932807970816, 443588932694724608, '新增0'), (443588933026074624, 443588932694724608, '新增1'), (443588933248372736, 443588932694724608, '新增2'), (445140374660337664, 445140374589034496, '新增0'), (445140374786166784, 445140374589034496, '新增1'), (446130095746207744, 446130095742013440, '新增0'), (446130095754596352, 446130095742013440, '新增1'), (484622270955802624, 484622270804807680, '新增0'), (484622271224238080, 484622270804807680, '新增1'), (484622408784826368, 484622408633831424, '新增0'), (484622408994541568, 484622408633831424, '新增1'), (484623850744598528, 484623850568437760, '新增0'), (484623850987868160, 484623850568437760, '新增1'), (484623946806743040, 484623946693496832, '新增0'), (484623947016458240, 484623946693496832, '新增1');

-- ----------------------------
-- Table structure for demo_child_tbl2
-- ----------------------------
DROP TABLE IF EXISTS `demo_child_tbl2`;
CREATE TABLE `demo_child_tbl2`  (
  `id` bigint(20) NOT NULL,
  `group_id` bigint(20) NOT NULL,
  `item_name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_child_tbl2
-- ----------------------------
INSERT INTO `demo_child_tbl2` VALUES (443588388416671744, 443588385522601984, '修改975'), (443588583799934976, 443588583535693824, '新增0'), (443588584039010304, 443588583535693824, '新增1'), (443588584374554624, 443588583535693824, '新增2'), (443588895692574720, 443588895352836096, '新增0'), (443588895931650048, 443588895352836096, '新增1'), (443588896258805760, 443588895352836096, '新增2'), (443588932917022720, 443588932694724608, '新增0'), (443588933135126528, 443588932694724608, '新增1'), (443588933361618944, 443588932694724608, '新增2'), (445140374735835136, 445140374589034496, '新增0'), (445140374819721216, 445140374589034496, '新增1'), (446130095750402048, 446130095742013440, '新增0'), (446130095754596353, 446130095742013440, '新增1'), (484622271115186176, 484622270804807680, '新增0'), (484622271333289984, 484622270804807680, '新增1'), (484622408889683968, 484622408633831424, '新增0'), (484622409107787776, 484622408633831424, '新增1'), (484623850878816256, 484623850568437760, '新增0'), (484623851092725760, 484623850568437760, '新增1'), (484623946907406336, 484623946693496832, '新增0'), (484623947121315840, 484623946693496832, '新增1');

-- ----------------------------
-- Table structure for demo_crud
-- ----------------------------
DROP TABLE IF EXISTS `demo_crud`;
CREATE TABLE `demo_crud`  (
  `id` bigint(20) NOT NULL COMMENT '标识',
  `name` varchar(255) NOT NULL COMMENT '名称',
  `dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `mtime` datetime NOT NULL COMMENT '最后修改时间',
  `enable_insert_event` tinyint(1) NOT NULL COMMENT 'true时允许发布插入事件',
  `enable_name_changed_event` tinyint(1) NOT NULL COMMENT 'true时允许发布Name变化事件',
  `enable_del_event` tinyint(1) NOT NULL COMMENT 'true时允许发布删除事件',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB COMMENT = '#demo#基础增删改';

-- ----------------------------
-- Records of demo_crud
-- ----------------------------
INSERT INTO `demo_crud` VALUES (446127712370708480, '批增更944', 50, '2023-02-13 09:52:21', 0, 0, 0), (446127712387485696, '批量605', 51, '2023-02-13 09:52:21', 0, 0, 0), (446127744155144192, '批增更887', 52, '2023-02-13 09:52:28', 0, 0, 0), (446127778095452160, '批增更删501', 53, '2023-02-13 09:52:36', 0, 0, 0), (446127928557719552, '新增事件9083', 54, '2023-02-13 09:53:12', 1, 0, 0), (447641397090078720, '领域服务', 61, '2023-02-17 14:07:07', 0, 0, 0), (447641397589200896, '服务更', 62, '2023-02-17 14:07:08', 0, 0, 0), (484620702760062976, '单个9897', 63, '2023-05-30 15:09:40', 0, 0, 0), (484620769650823168, '批量430', 64, '2023-05-30 15:09:56', 0, 0, 0), (484620769889898496, '批量813', 65, '2023-05-30 15:09:56', 0, 0, 0), (484620770128973824, '批量572', 66, '2023-05-30 15:09:56', 0, 0, 0), (484620773429891072, '批增更218', 67, '2023-05-30 15:09:57', 0, 0, 0), (484623044423208960, '单个5122', 68, '2023-05-30 15:18:58', 0, 0, 0), (484623148454531072, '批量40', 69, '2023-05-30 15:19:23', 0, 0, 0), (484623148689412096, '批量680', 70, '2023-05-30 15:19:23', 0, 0, 0), (484623148932681728, '批量531', 71, '2023-05-30 15:19:23', 0, 0, 0), (484623187683856384, '批增更615', 72, '2023-05-30 15:19:33', 0, 0, 0), (484623231044571136, '批增更删992', 73, '2023-05-30 15:19:43', 0, 0, 0), (484624288650907648, '领域服务', 74, '2023-05-30 15:23:55', 0, 0, 0), (484624288994840576, '服务更', 75, '2023-05-30 15:23:55', 0, 0, 0), (484956889089593344, '单个8461', 76, '2023-05-31 13:25:35', 0, 0, 0), (484957035659546624, '单个8271', 77, '2023-05-31 13:26:09', 0, 0, 0), (484957333266386944, '批量652', 78, '2023-05-31 13:27:20', 0, 0, 0), (484957333782286336, '批量521', 79, '2023-05-31 13:27:21', 0, 0, 0), (484957334516289536, '批量955', 80, '2023-05-31 13:27:21', 0, 0, 0), (484988812650369024, '批增更778', 81, '2023-05-31 15:32:23', 0, 0, 0), (486788489460862976, '单个4284', 82, '2023-06-05 14:43:45', 0, 0, 0), (487086064026013696, '单个1221', 83, '2023-06-06 10:26:08', 0, 0, 0), (487086286626115584, '单个685', 84, '2023-06-06 10:27:01', 0, 0, 0);

-- ----------------------------
-- Table structure for demo_par_tbl
-- ----------------------------
DROP TABLE IF EXISTS `demo_par_tbl`;
CREATE TABLE `demo_par_tbl`  (
  `id` bigint(20) NOT NULL,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_par_tbl
-- ----------------------------
INSERT INTO `demo_par_tbl` VALUES (443588385522601984, '91471c9846a44fe8a7fc4b76e9f702ea'), (443588583535693824, '新增'), (443588895352836096, '新增'), (443588932694724608, '新增'), (445140374589034496, '新增'), (446130095742013440, '新增'), (484622270804807680, '新增'), (484622408633831424, '新增'), (484623850568437760, '新增'), (484623946693496832, '新增');

-- ----------------------------
-- Table structure for demo_virtbl1
-- ----------------------------
DROP TABLE IF EXISTS `demo_virtbl1`;
CREATE TABLE `demo_virtbl1`  (
  `id` bigint(20) NOT NULL,
  `name1` varchar(255) NOT NULL COMMENT '名称1',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_virtbl1
-- ----------------------------
INSERT INTO `demo_virtbl1` VALUES (484613811564728320, '新1'), (484613939734269952, '新1'), (484614242416218112, '批增1'), (484621407772233728, '新1'), (484623466739290112, '新1');

-- ----------------------------
-- Table structure for demo_virtbl2
-- ----------------------------
DROP TABLE IF EXISTS `demo_virtbl2`;
CREATE TABLE `demo_virtbl2`  (
  `id` bigint(20) NOT NULL,
  `name2` varchar(255) NOT NULL COMMENT '名称2',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_virtbl2
-- ----------------------------
INSERT INTO `demo_virtbl2` VALUES (484613811564728320, '新2'), (484613939734269952, '新2'), (484614242416218112, '批增2'), (484621407772233728, '新2'), (484623466739290112, '新2');

-- ----------------------------
-- Table structure for demo_virtbl3
-- ----------------------------
DROP TABLE IF EXISTS `demo_virtbl3`;
CREATE TABLE `demo_virtbl3`  (
  `id` bigint(20) NOT NULL,
  `name3` varchar(255) NOT NULL COMMENT '名称3',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_virtbl3
-- ----------------------------
INSERT INTO `demo_virtbl3` VALUES (484613811564728320, '新3'), (484613939734269952, '新3'), (484614242416218112, '批增3'), (484621407772233728, '新3'), (484623466739290112, '新3');

-- ----------------------------
-- Table structure for demo_大儿
-- ----------------------------
DROP TABLE IF EXISTS `demo_大儿`;
CREATE TABLE `demo_大儿`  (
  `id` bigint(20) NOT NULL,
  `parent_id` bigint(20) NOT NULL,
  `大儿名` varchar(255) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_大儿_parendid`(`parent_id`) USING BTREE,
  CONSTRAINT `fk_大儿_parendid` FOREIGN KEY (`parent_id`) REFERENCES `demo_父表` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_大儿
-- ----------------------------
INSERT INTO `demo_大儿` VALUES (453807589999792128, 448686488403595264, '啊北侧'), (453810847795400704, 453810798449414144, 'bd'), (453811346175184896, 453810798449414144, 'asdf'), (453811364621733888, 453810798449414144, 'bde');

-- ----------------------------
-- Table structure for demo_父表
-- ----------------------------
DROP TABLE IF EXISTS `demo_父表`;
CREATE TABLE `demo_父表`  (
  `id` bigint(20) NOT NULL,
  `父名` varchar(255) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_父表
-- ----------------------------
INSERT INTO `demo_父表` VALUES (448686488403595264, '123'), (449120963746877440, '单位'), (453810798449414144, 'aaaa');

-- ----------------------------
-- Table structure for demo_基础
-- ----------------------------
DROP TABLE IF EXISTS `demo_基础`;
CREATE TABLE `demo_基础`  (
  `id` bigint(20) NOT NULL COMMENT '标识',
  `序列` int(11) NOT NULL COMMENT '序列自动赋值',
  `限长4` varchar(16) NOT NULL COMMENT '限制最大长度4',
  `不重复` varchar(64) NOT NULL COMMENT '列值无重复',
  `禁止选中` tinyint(1) NOT NULL COMMENT '始终为false',
  `禁止保存` tinyint(1) NOT NULL COMMENT 'true时保存前校验不通过',
  `禁止删除` tinyint(1) NOT NULL COMMENT 'true时删除前校验不通过',
  `值变事件` varchar(64) NOT NULL COMMENT '每次值变化时触发领域事件',
  `创建时间` datetime NOT NULL COMMENT '初次创建时间',
  `修改时间` datetime NOT NULL COMMENT '最后修改时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_基础
-- ----------------------------
INSERT INTO `demo_基础` VALUES (1, 1, 'adb', 'ddd', 1, 1, 1, 'a', '2023-01-17 10:08:10', '2023-01-17 10:08:14'), (447570516976357376, 6, '11', 'dd', 0, 0, 1, 'snv111', '2023-02-17 09:25:27', '2023-02-17 09:25:27');

-- ----------------------------
-- Table structure for demo_角色
-- ----------------------------
DROP TABLE IF EXISTS `demo_角色`;
CREATE TABLE `demo_角色`  (
  `id` bigint(20) NOT NULL COMMENT '角色标识',
  `角色名称` varchar(32) NOT NULL COMMENT '角色名称',
  `角色描述` varchar(255) NULL DEFAULT NULL COMMENT '角色描述',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB COMMENT = '角色';

-- ----------------------------
-- Records of demo_角色
-- ----------------------------
INSERT INTO `demo_角色` VALUES (449487215124303872, 'xxx', 'df'), (449812931669938176, '管理员', ''), (449812975420723200, '维护1', ''), (449813053959065600, '维护2', '');

-- ----------------------------
-- Table structure for demo_角色权限
-- ----------------------------
DROP TABLE IF EXISTS `demo_角色权限`;
CREATE TABLE `demo_角色权限`  (
  `role_id` bigint(20) NOT NULL COMMENT '角色标识',
  `prv_id` bigint(20) NOT NULL COMMENT '权限标识',
  PRIMARY KEY (`role_id`, `prv_id`) USING BTREE,
  INDEX `idx_demo_角色权限_prvid`(`prv_id`) USING BTREE,
  INDEX `idx_demo_角色权限_roleid`(`role_id`) USING BTREE,
  CONSTRAINT `fk_角色权限_prvid` FOREIGN KEY (`prv_id`) REFERENCES `demo_权限` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_角色权限_roleid` FOREIGN KEY (`role_id`) REFERENCES `demo_角色` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '角色关联的权限';

-- ----------------------------
-- Records of demo_角色权限
-- ----------------------------
INSERT INTO `demo_角色权限` VALUES (449487215124303872, 449812884102336512);

-- ----------------------------
-- Table structure for demo_扩展1
-- ----------------------------
DROP TABLE IF EXISTS `demo_扩展1`;
CREATE TABLE `demo_扩展1`  (
  `id` bigint(20) NOT NULL COMMENT '标识',
  `扩展1名称` varchar(255) NOT NULL,
  `禁止选中` tinyint(1) NOT NULL COMMENT '始终为false',
  `禁止保存` tinyint(1) NOT NULL COMMENT 'true时保存前校验不通过',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_扩展1
-- ----------------------------
INSERT INTO `demo_扩展1` VALUES (447555037331214336, 'a', 0, 0), (447577275388416000, '221', 0, 0), (447577372700463104, '', 0, 0);

-- ----------------------------
-- Table structure for demo_扩展2
-- ----------------------------
DROP TABLE IF EXISTS `demo_扩展2`;
CREATE TABLE `demo_扩展2`  (
  `id` bigint(20) NOT NULL COMMENT '标识',
  `扩展2名称` varchar(255) NOT NULL,
  `禁止删除` tinyint(1) NOT NULL COMMENT 'true时删除前校验不通过',
  `值变事件` varchar(255) NOT NULL COMMENT '每次值变化时触发领域事件',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_扩展2
-- ----------------------------
INSERT INTO `demo_扩展2` VALUES (447555037331214336, 'a', 0, ''), (447577275388416000, '', 0, '221'), (447577372700463104, '', 0, '');

-- ----------------------------
-- Table structure for demo_权限
-- ----------------------------
DROP TABLE IF EXISTS `demo_权限`;
CREATE TABLE `demo_权限`  (
  `id` bigint(20) NOT NULL COMMENT '权限名称',
  `权限名称` varchar(255) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB COMMENT = '权限';

-- ----------------------------
-- Records of demo_权限
-- ----------------------------
INSERT INTO `demo_权限` VALUES (449812852120768512, '删除'), (449812884102336512, '修改');

-- ----------------------------
-- Table structure for demo_收文
-- ----------------------------
DROP TABLE IF EXISTS `demo_收文`;
CREATE TABLE `demo_收文`  (
  `id` bigint(20) NOT NULL,
  `来文单位` varchar(255) NOT NULL,
  `来文时间` date NOT NULL,
  `密级` tinyint(4) UNSIGNED NOT NULL COMMENT '#密级#',
  `文件标题` varchar(255) NOT NULL,
  `文件附件` varchar(512) NOT NULL,
  `市场部经理意见` varchar(255) NOT NULL,
  `综合部经理意见` varchar(255) NOT NULL,
  `收文完成时间` date NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_收文
-- ----------------------------
INSERT INTO `demo_收文` VALUES (162025231350624256, '123', '2020-12-21', 0, 'a', '', '', '', '0001-01-01'), (162401333600448512, 'abc', '2020-12-22', 0, '关于新冠疫情的批示', '', '', '', '0001-01-01'), (457384396879581184, '', '2023-03-16', 0, '阿斯蒂芬', '', '', '', '0001-01-01'), (457388173615452160, '', '2023-03-16', 0, '疫情在', '', '', '', '0001-01-01');

-- ----------------------------
-- Table structure for demo_小儿
-- ----------------------------
DROP TABLE IF EXISTS `demo_小儿`;
CREATE TABLE `demo_小儿`  (
  `id` bigint(20) NOT NULL,
  `group_id` bigint(20) NOT NULL,
  `小儿名` varchar(255) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_小儿_parentid`(`group_id`) USING BTREE,
  CONSTRAINT `fk_小儿_parentid` FOREIGN KEY (`group_id`) REFERENCES `demo_父表` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_小儿
-- ----------------------------
INSERT INTO `demo_小儿` VALUES (449113382156521472, 448686488403595264, 'wwww'), (453810909078376448, 453810798449414144, '34'), (453811464773324800, 453810798449414144, 'adgas');

-- ----------------------------
-- Table structure for demo_用户
-- ----------------------------
DROP TABLE IF EXISTS `demo_用户`;
CREATE TABLE `demo_用户`  (
  `id` bigint(20) NOT NULL COMMENT '用户标识',
  `手机号` char(11) NOT NULL COMMENT '手机号，唯一',
  `姓名` varchar(32) NOT NULL COMMENT '姓名',
  `密码` char(32) NOT NULL COMMENT '密码的md5',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB COMMENT = '系统用户';

-- ----------------------------
-- Records of demo_用户
-- ----------------------------
INSERT INTO `demo_用户` VALUES (449772627373871104, '13223333', '阿斯顿', ''), (453805638385946624, '111', '', ''), (453805654500462592, '222', '', '');

-- ----------------------------
-- Table structure for demo_用户角色
-- ----------------------------
DROP TABLE IF EXISTS `demo_用户角色`;
CREATE TABLE `demo_用户角色`  (
  `user_id` bigint(20) NOT NULL COMMENT '用户标识',
  `role_id` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`user_id`, `role_id`) USING BTREE,
  INDEX `idx_demo_用户角色_roleid`(`role_id`) USING BTREE,
  INDEX `idx_demo_用户角色_userid`(`user_id`) USING BTREE,
  CONSTRAINT `fk_demo_用户角色_roleid` FOREIGN KEY (`role_id`) REFERENCES `demo_角色` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_demo_用户角色_userid` FOREIGN KEY (`user_id`) REFERENCES `demo_用户` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '用户关联的角色';

-- ----------------------------
-- Records of demo_用户角色
-- ----------------------------
INSERT INTO `demo_用户角色` VALUES (449772627373871104, 449487215124303872), (449772627373871104, 449812931669938176);

-- ----------------------------
-- Table structure for demo_主表
-- ----------------------------
DROP TABLE IF EXISTS `demo_主表`;
CREATE TABLE `demo_主表`  (
  `id` bigint(20) NOT NULL,
  `主表名称` varchar(255) NOT NULL,
  `限长4` varchar(16) NOT NULL COMMENT '限制最大长度4',
  `不重复` varchar(255) NOT NULL COMMENT '列值无重复',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;

-- ----------------------------
-- Records of demo_主表
-- ----------------------------
INSERT INTO `demo_主表` VALUES (447555037331214336, 'a', '', ''), (447577275388416000, '1', '222222', '121'), (447577372700463104, '', '', '1');

-- ----------------------------
-- Records of sequence
-- ----------------------------
INSERT INTO `sequence` VALUES ('demo_crud_dispidx', 84), ('demo_基础_序列', 11);

-- ----------------------------
-- View structure for demo_child_view
-- ----------------------------
DROP VIEW IF EXISTS `demo_child_view`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `demo_child_view` AS select `c`.`id` AS `id`,`c`.`parent_id` AS `parent_id`,`c`.`item_name` AS `item_name`,`p`.`name` AS `name` from (`demo_child_tbl1` `c` join `demo_par_tbl` `p` on((`c`.`parent_id` = `p`.`id`)));

-- ----------------------------
-- Procedure structure for demo_用户可访问的菜单
-- ----------------------------
DROP PROCEDURE IF EXISTS `demo_用户可访问的菜单`;
CREATE PROCEDURE `demo_用户可访问的菜单`(`p_userid` bigint)
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
                   and user_id = p_userid
					union
					select role_id
					        from cm_group_role d
									where a.role_id = d.role_id
									  and exists (select group_id from cm_user_group e where d.group_id=e.group_id and e.user_id=p_userid)
					) or a.role_id=1
			 ) t
 order by dispidx;
 
END
;

SET FOREIGN_KEY_CHECKS = 1;

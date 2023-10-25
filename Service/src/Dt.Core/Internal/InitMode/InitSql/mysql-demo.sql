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
  `info` varchar(512) NOT NULL COMMENT '文件描述信息',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `user_id` bigint(20) NOT NULL COMMENT '所属用户',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_file_my_parentid`(`parent_id`) USING BTREE,
  INDEX `idx_file_my_userid`(`user_id`) USING BTREE,
  CONSTRAINT `fk_file_my_parentid` FOREIGN KEY (`parent_id`) REFERENCES `cm_file_my` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_file_my_userid` FOREIGN KEY (`user_id`) REFERENCES `cm_user` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB COMMENT = '个人文件';

-- ----------------------------
-- Records of cm_file_my
-- ----------------------------
INSERT INTO `cm_file_my` VALUES (140724076930789376, NULL, '新目录1', 1, NULL, '', '2020-10-23 15:47:16', 1), (140724154458304512, 140724076930789376, 'b', 1, NULL, '', '2020-10-23 15:47:34', 1), (141735914371936256, NULL, '新目录12', 1, NULL, '', '2020-10-26 10:48:01', 2), (456284281217503232, NULL, '新Tab', 1, '', '', '2023-03-13 10:30:55', 1);

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
  `info` varchar(512) NOT NULL COMMENT '文件描述信息',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_file_pub_parentid`(`parent_id`) USING BTREE,
  CONSTRAINT `fk_file_pub_parentid` FOREIGN KEY (`parent_id`) REFERENCES `cm_file_pub` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB COMMENT = '公共文件';

-- ----------------------------
-- Records of cm_file_pub
-- ----------------------------
INSERT INTO `cm_file_pub` VALUES (1, NULL, '公共文件', 1, NULL, '', '2020-10-21 15:19:20'), (2, NULL, '素材库', 1, NULL, '', '2020-10-21 15:20:21'), (140015729575325696, 1, '新目录a', 1, NULL, '', '0001-01-01 00:00:00'), (140016348063199232, 1, '新目录1111', 1, NULL, '', '2020-10-21 16:55:00'), (140244264617373696, 140016348063199232, '新目录q', 1, NULL, '', '2020-10-22 08:00:39'), (140253323206717440, 140244264617373696, 'ab', 1, NULL, '', '2020-10-22 08:36:39'), (140266906502164480, 140244264617373696, 'aa', 0, 'xlsx', '[[\"v0/1F/4A/140266906879651840.xlsx\",\"aa\",\"xlsx文件\",8236,\"daoting\",\"2020-10-22 09:30\"]]', '2020-10-22 09:30:37'), (142873261784297472, 2, '新目录1', 1, NULL, '', '2020-10-29 14:07:20'), (142888903606398976, 2, '12', 0, 'xlsx', '[[\"v0/52/37/142888904373956608.xlsx\",\"12\",\"xlsx文件\",8153,\"daoting\",\"2020-10-29 15:09\"]]', '2020-10-29 15:09:30'), (142913881819181056, 2, '未标题-2', 0, 'jpg', '[[\"v0/E3/18/142913882284748800.jpg\",\"未标题-2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-10-29 16:48\"]]', '2020-10-29 16:48:44'), (142914110945619968, 2, 'Icon-20@2x', 0, 'png', '[[\"v0/E3/0D/142914111109197824.png\",\"Icon-20@2x\",\"40 x 40 (.png)\",436,\"daoting\",\"2020-10-29 16:49\"]]', '2020-10-29 16:49:39'), (143174605384577024, 140016348063199232, 'Icon-20@3x', 0, 'png', '[[\"v0/56/59/143174606269575168.png\",\"Icon-20@3x\",\"60 x 60 (.png)\",496,\"daoting\",\"2020-10-30 10:04\"]]', '2020-10-30 10:04:47'), (143191060503195648, 1, 'Icon-20@3x', 0, 'png', '[[\"v0/56/59/143191060947791872.png\",\"Icon-20@3x\",\"60 x 60 (.png)\",534,\"daoting\",\"2020-10-30 11:10\"]]', '2020-10-30 11:10:10'), (143192411157164032, 140015729575325696, 'Icon-29@2x', 0, 'png', '[[\"v0/46/CE/143192411832446976.png\",\"Icon-29@2x\",\"58 x 58 (.png)\",624,\"daoting\",\"2020-10-30 11:15\"]]', '2020-10-30 11:15:32'), (143193081423720448, 140015729575325696, '3709740f5c5e4cb4909a6cc79f412734_th', 0, 'png', '[[\"v0/BF/6D/143193081931231232.png\",\"3709740f5c5e4cb4909a6cc79f412734_th\",\"537 x 302 (.png)\",27589,\"daoting\",\"2020-10-30 11:18\"]]', '2020-10-30 11:18:12'), (143195001659977728, 1, '未标题-2', 0, 'jpg', '[[\"v0/E3/18/143195002217820160.jpg\",\"未标题-2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-10-30 11:25\"]]', '2020-10-30 11:25:50'), (143203944146792448, 1, 'ImageStabilization', 0, 'wmv', '[[\"v0/EA/34/143203944767549440.wmv\",\"ImageStabilization\",\"00:00:06 (480 x 288)\",403671,\"daoting\",\"2020-10-30 12:01\"]]', '2020-10-30 12:01:22'), (172190549410705408, 1, '公司服务器及网络', 0, 'txt', '[[\"v0/5F/37/172190549775609856.txt\",\"公司服务器及网络\",\"txt文件\",435,\"daoting\",\"2021-01-18 11:43\"]]', '2021-01-18 11:43:37'), (185641691419373568, 1, '1', 0, 'png', '[[\"v0/FC/63/185641725430984704.png\",\"1\",\"1101 x 428 (.png)\",47916,\"daoting\",\"2021-02-24 14:33\"]]', '2021-02-24 14:33:46'), (187725770344230912, 1, 'doc1', 0, 'png', '[[\"v0/D8/28/187725778074333184.png\",\"doc1\",\"1076 x 601 (.png)\",59038,\"daoting\",\"2021-03-02 08:35\"]]', '2021-03-02 08:35:12'), (205916917767991296, 140015729575325696, 'state', 0, 'db', '[[\"v0/DF/F3/205916918690738176.db\",\"state\",\"db文件\",90112,\"苹果\",\"2021-04-21 13:20\"]]', '2021-04-21 13:20:20'), (255970120425140224, 456277006646005760, 'abc', 1, '', '', '2021-09-06 16:13:53'), (322270820868235264, 1, '172190549775609856', 0, 'txt', '[[\"editor/57/01/322270823007330304.txt\",\"172190549775609856\",\"txt文件\",435,\"daoting\",\"2022-03-08 15:09\"]]', '2022-03-08 15:09:10'), (456276498464133120, 456277006646005760, '未标题-2', 0, 'jpg', '[[\"editor/E3/18/456276498854203392.jpg\",\"未标题-2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2023-03-13 09:59\"]]', '2023-03-13 09:59:59'), (456277006646005760, 1, '新Tab', 1, '', '', '2023-03-13 10:02:00'), (456281421624922112, 255970120425140224, '未标题-2', 0, 'jpg', '[[\"editor/E3/18/456281422107267072.jpg\",\"未标题-2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2023-03-13 10:19\"]]', '2023-03-13 10:19:33'), (456281921225248768, 456277006646005760, 'UserList', 0, 'xaml', '[[\"editor/C1/45/456281921523044352.xaml\",\"UserList\",\"xaml文件\",2682,\"daoting\",\"2023-03-13 10:21\"]]', '2023-03-13 10:21:32');

-- ----------------------------
-- Table structure for cm_group
-- ----------------------------
DROP TABLE IF EXISTS `cm_group`;
CREATE TABLE `cm_group`  (
  `id` bigint(20) NOT NULL COMMENT '组标识',
  `name` varchar(64) NOT NULL COMMENT '组名',
  `note` varchar(255) NOT NULL COMMENT '组描述',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_group_name`(`name`) USING BTREE COMMENT '不重复'
) ENGINE = InnoDB COMMENT = '分组，与用户和角色多对多';

-- ----------------------------
-- Records of cm_group
-- ----------------------------
INSERT INTO `cm_group` VALUES (454483802783240192, '分组1', ''), (454484847190102016, '2', ''), (454484924033945600, '3', '');

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
-- Records of cm_group_role
-- ----------------------------
INSERT INTO `cm_group_role` VALUES (454483802783240192, 2), (454483802783240192, 22844822693027840), (454483802783240192, 152695933758603264), (454483802783240192, 152696004814307328), (454484847190102016, 152695933758603264), (454484924033945600, 22844822693027840);

-- ----------------------------
-- Table structure for cm_menu
-- ----------------------------
DROP TABLE IF EXISTS `cm_menu`;
CREATE TABLE `cm_menu`  (
  `id` bigint(20) NOT NULL COMMENT '菜单标识',
  `parent_id` bigint(20) NULL DEFAULT NULL COMMENT '父菜单标识',
  `name` varchar(64) NOT NULL COMMENT '菜单名称',
  `is_group` tinyint(1) NOT NULL COMMENT '分组或实例。0表实例，1表分组',
  `view_name` varchar(128) NOT NULL COMMENT '视图名称',
  `params` varchar(4000) NOT NULL COMMENT '传递给菜单程序的参数',
  `icon` varchar(128) NOT NULL COMMENT '图标',
  `note` varchar(512) NOT NULL COMMENT '备注',
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
INSERT INTO `cm_menu` VALUES (1, NULL, '工作台', 1, '', '', '搬运工', '', 1, 0, '2019-03-07 10:45:44', '2019-03-07 10:45:43'), (2, 1, '用户账号', 0, '用户账号', '', '钥匙', '', 2, 0, '2019-11-08 11:42:28', '2019-11-08 11:43:53'), (3, 1, '菜单管理', 0, '菜单管理', '', '大图标', '', 3, 0, '2019-03-11 11:35:59', '2019-03-11 11:35:58'), (4, 1, '系统角色', 0, '系统角色', '', '两人', '', 4, 0, '2019-11-08 11:47:21', '2019-11-08 11:48:22'), (5, 1, '分组管理', 0, '分组管理', '', '分组', '', 5, 0, '2023-03-10 08:34:49', '2023-03-10 08:34:49'), (6, 1, '基础权限', 0, '基础权限', '', '审核', '', 6, 0, '2019-03-12 09:11:22', '2019-03-07 11:23:40'), (7, 1, '参数定义', 0, '参数定义', '', '调色板', '', 7, 0, '2019-03-12 15:35:56', '2019-03-12 15:37:10'), (8, 1, '基础选项', 0, '基础选项', '', '修理', '', 8, 0, '2019-11-08 11:49:40', '2019-11-08 11:49:46'), (9, 1, '报表设计', 0, '报表设计', '', '折线图', '', 76, 0, '2020-10-19 11:21:38', '2020-10-19 11:21:38'), (10, 1, '流程设计', 0, '流程设计', '', '双绞线', '', 79, 0, '2020-11-02 16:21:19', '2020-11-02 16:21:19'), (15268145234386944, 15315938808373248, '新菜单组22', 1, '', '', '文件夹', '', 25, 0, '2019-11-12 11:10:10', '2019-11-12 11:10:13'), (15315637929975808, 18562741636898816, '新菜单12', 0, '', '', '文件', '', 48, 0, '2019-11-12 14:18:53', '2019-11-12 14:31:38'), (15315938808373248, NULL, '新菜单组额', 1, '', '', '文件夹', '', 67, 0, '2019-11-12 14:20:04', '2019-11-12 14:20:14'), (18562741636898816, 15315938808373248, '新组t', 1, '', '', '文件夹', '', 63, 0, '2019-11-21 13:21:43', '2019-11-21 13:21:43'), (18860286065975296, NULL, '新菜单a123', 0, '报表', '新报表111,abc1', '文件', '', 68, 0, '2019-11-22 09:04:04', '2019-11-22 09:04:04'), (154430055023640576, NULL, '新菜单xxx', 0, '报表', '', '文件', '', 84, 0, '2020-11-30 11:29:56', '2020-11-30 11:29:56'), (259520016549801984, NULL, '新组bcd', 1, '', '', '文件夹', '', 83, 0, '2021-09-16 11:19:54', '2021-09-16 11:19:54');

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
INSERT INTO `cm_option` VALUES (2, '汉族', 2, 1), (3, '蒙古族', 3, 1), (4, '回族', 4, 1), (5, '藏族', 5, 1), (6, '维吾尔族', 6, 1), (7, '苗族', 7, 1), (8, '彝族', 8, 1), (9, '壮族', 9, 1), (10, '布依族', 10, 1), (11, '朝鲜族', 11, 1), (12, '满族', 12, 1), (13, '侗族', 13, 1), (14, '瑶族', 14, 1), (15, '白族', 15, 1), (16, '土家族', 16, 1), (17, '哈尼族', 17, 1), (18, '哈萨克族', 18, 1), (19, '傣族', 19, 1), (20, '黎族', 20, 1), (21, '傈僳族', 21, 1), (22, '佤族', 22, 1), (23, '畲族', 23, 1), (24, '高山族', 24, 1), (25, '拉祜族', 25, 1), (26, '水族', 26, 1), (27, '东乡族', 27, 1), (28, '纳西族', 28, 1), (29, '景颇族', 29, 1), (30, '柯尔克孜族', 30, 1), (31, '土族', 31, 1), (32, '达斡尔族', 32, 1), (33, '仫佬族', 33, 1), (34, '羌族', 34, 1), (35, '布朗族', 35, 1), (36, '撒拉族', 36, 1), (37, '毛难族', 37, 1), (38, '仡佬族', 38, 1), (39, '锡伯族', 39, 1), (40, '阿昌族', 40, 1), (41, '普米族', 41, 1), (42, '塔吉克族', 42, 1), (43, '怒族', 43, 1), (44, '乌孜别克族', 44, 1), (45, '俄罗斯族', 45, 1), (46, '鄂温克族', 46, 1), (47, '德昂族', 47, 1), (48, '保安族', 48, 1), (49, '裕固族', 49, 1), (50, '京族', 50, 1), (51, '塔塔尔族', 51, 1), (52, '独龙族', 52, 1), (53, '鄂伦春族', 53, 1), (54, '赫哲族', 54, 1), (55, '门巴族', 55, 1), (56, '珞巴族', 56, 1), (57, '基诺族', 57, 1), (58, '大学', 58, 2), (59, '高中', 59, 2), (60, '中学', 60, 2), (61, '小学', 61, 2), (62, '硕士', 62, 2), (63, '博士', 63, 2), (64, '其他', 64, 2), (65, '黑龙江杜尔伯特县', 65, 3), (66, '黑龙江富裕县', 66, 3), (67, '黑龙江林甸县', 67, 3), (68, '黑龙江克山县', 68, 3), (69, '黑龙江克东县', 69, 3), (70, '黑龙江省拜泉县', 70, 3), (71, '黑龙江鸡西市', 71, 3), (72, '黑龙江鸡东县', 72, 3), (73, '黑龙江鹤岗市', 73, 3), (74, '黑龙江萝北县', 74, 3), (75, '黑龙江绥滨县', 75, 3), (76, '黑龙江双鸭山市', 76, 3), (77, '黑龙江集贤县', 77, 3), (78, '黑龙江大庆市', 78, 3), (79, '黑龙江伊春市', 79, 3), (80, '黑龙江嘉荫县', 80, 3), (81, '黑龙江佳木斯市', 81, 3), (82, '黑龙江桦南县', 82, 3), (83, '黑龙江依兰县', 83, 3), (84, '黑龙江桦川县', 84, 3), (85, '黑龙江省宝清县', 85, 3), (86, '黑龙江汤原县', 86, 3), (87, '黑龙江饶河县', 87, 3), (88, '黑龙江抚远县', 88, 3), (89, '黑龙江友谊县', 89, 3), (90, '黑龙江七台河市', 90, 3), (91, '黑龙江省勃利县', 91, 3), (92, '黑龙江牡丹江市', 92, 3), (93, '黑龙江宁安县', 93, 3), (94, '黑龙江海林县', 94, 3), (95, '黑龙江穆棱县', 95, 3), (96, '黑龙江东宁县', 96, 3), (97, '黑龙江林口县', 97, 3), (98, '黑龙江虎林县', 98, 3), (99, '黑龙江双城市', 99, 3), (100, '黑龙江尚志市', 100, 3), (101, '黑龙江省宾县', 101, 3), (102, '黑龙江五常县', 102, 3), (103, '黑龙江省巴彦县', 103, 3), (104, '黑龙江木兰县', 104, 3), (105, '黑龙江通河县', 105, 3), (106, '黑龙江方正县', 106, 3), (107, '黑龙江延寿县', 107, 3), (108, '黑龙江绥化市', 108, 3), (109, '黑龙江省安达市', 109, 3), (110, '黑龙江肇东市', 110, 3), (111, '黑龙江海伦县', 111, 3), (112, '黑龙江望奎县', 112, 3), (113, '黑龙江兰西县', 113, 3), (114, '黑龙江青冈县', 114, 3), (115, '黑龙江肇源县', 115, 3), (116, '黑龙江肇州县', 116, 3), (117, '黑龙江庆安县', 117, 3), (118, '黑龙江明水县', 118, 3), (119, '黑龙江绥棱县', 119, 3), (120, '黑龙江黑河市', 120, 3), (121, '黑龙江省北安市', 121, 3), (122, '黑龙江五大连池市', 122, 3), (123, '黑龙江嫩江县', 123, 3), (124, '黑龙江省德都县', 124, 3), (125, '黑龙江逊克县', 125, 3), (126, '黑龙江孙吴县', 126, 3), (127, '黑龙江呼玛县', 127, 3), (128, '黑龙江塔河县', 128, 3), (129, '黑龙江漠河县', 129, 3), (130, '黑龙江绥芬河市', 130, 3), (131, '黑龙江省阿城市', 131, 3), (132, '黑龙江同江市', 132, 3), (133, '黑龙江富锦市', 133, 3), (134, '黑龙江铁力市', 134, 3), (135, '黑龙江密山市', 135, 3), (136, '吉林省长春市', 136, 3), (137, '内蒙古呼和浩特市', 137, 3), (138, '内蒙古土默特左旗', 138, 3), (139, '内蒙古托克托县', 139, 3), (140, '内蒙古包头市', 140, 3), (141, '内蒙古土默特右旗', 141, 3), (142, '内蒙古固阳县', 142, 3), (143, '内蒙古乌海市', 143, 3), (144, '内蒙古赤峰市', 144, 3), (145, '内蒙古阿鲁科尔沁旗', 145, 3), (146, '内蒙古巴林左旗', 146, 3), (147, '内蒙古巴林右旗', 147, 3), (148, '内蒙古林西县', 148, 3), (149, '内蒙古克什克腾旗', 149, 3), (150, '内蒙古翁牛特旗', 150, 3), (151, '内蒙古喀喇沁旗', 151, 3), (152, '内蒙古宁城县', 152, 3), (153, '内蒙古敖汉旗', 153, 3), (154, '内蒙古海拉尔市', 154, 3), (155, '内蒙古满州里市', 155, 3), (156, '内蒙古扎兰屯市', 156, 3), (157, '内蒙古牙克石市', 157, 3), (158, '内蒙古阿荣旗', 158, 3), (159, '内蒙古莫力县', 159, 3), (160, '内蒙古额尔古纳右旗', 160, 3), (161, '内蒙古额尔古纳左旗', 161, 3), (162, '内蒙古鄂伦春自治旗', 162, 3), (163, '内蒙古鄂温克族自治旗', 163, 3), (164, '内蒙古新巴尔虎右旗', 164, 3), (165, '内蒙古新巴尔虎左旗', 165, 3), (166, '内蒙古陈巴尔虎旗', 166, 3), (167, '内蒙古乌兰浩特市', 167, 3), (168, '内蒙古科尔沁右翼前旗', 168, 3), (169, '内蒙古科尔沁右翼中旗', 169, 3), (170, '内蒙古扎赉特旗', 170, 3), (171, '内蒙古突泉县', 171, 3), (172, '内蒙古通辽市', 172, 3), (173, '内蒙古霍林郭勒市', 173, 3), (174, '内蒙古科尔沁左翼中旗', 174, 3), (175, '内蒙古科尔沁左翼后旗', 175, 3), (176, '内蒙古开鲁县', 176, 3), (177, '内蒙古库伦旗', 177, 3), (178, '内蒙古奈曼旗', 178, 3), (179, '内蒙古扎鲁特旗', 179, 3), (180, '内蒙古二连浩特市', 180, 3), (181, '内蒙古锡林浩特市', 181, 3), (182, '内蒙古阿巴嘎旗', 182, 3), (183, '内蒙古苏尼特左旗', 183, 3), (184, '内蒙古苏尼特右旗', 184, 3), (185, '内蒙古东乌珠穆沁旗', 185, 3), (186, '内蒙古西乌珠穆沁旗', 186, 3), (187, '内蒙古太仆寺旗', 187, 3), (188, '内蒙古镶黄旗', 188, 3), (189, '内蒙古正镶白旗', 189, 3), (190, '内蒙古正蓝旗', 190, 3), (191, '内蒙古多伦县', 191, 3), (192, '内蒙古集宁市', 192, 3), (193, '内蒙古武川县', 193, 3), (194, '内蒙古和林格尔县', 194, 3), (195, '内蒙古清水河县', 195, 3), (196, '内蒙古卓资县', 196, 3), (197, '内蒙古化德县', 197, 3), (198, '内蒙古商都县', 198, 3), (199, '内蒙古兴和县', 199, 3), (200, '内蒙古丰镇县', 200, 3), (201, '内蒙古凉城县', 201, 3), (202, '内蒙古察哈尔右翼前旗', 202, 3), (203, '内蒙古察哈尔右翼中旗', 203, 3), (204, '内蒙古察哈尔右翼后旗', 204, 3), (205, '内蒙古达尔罕茂明安联', 205, 3), (206, '内蒙古四子王旗', 206, 3), (207, '内蒙古东胜市', 207, 3), (208, '内蒙古达拉特旗', 208, 3), (209, '内蒙古准格尔旗', 209, 3), (210, '内蒙古鄂托克前旗', 210, 3), (211, '内蒙古鄂托克旗', 211, 3), (212, '内蒙古杭锦旗', 212, 3), (213, '内蒙古乌审旗', 213, 3), (214, '内蒙古伊金霍洛旗', 214, 3), (215, '内蒙古临河市', 215, 3), (216, '内蒙古五原县', 216, 3), (217, '内蒙古磴口县', 217, 3), (218, '内蒙古乌拉特前旗', 218, 3), (219, '内蒙古乌拉特中旗', 219, 3), (220, '内蒙古乌拉特后旗', 220, 3), (221, '内蒙古杭锦后旗', 221, 3), (222, '内蒙古阿拉善左旗', 222, 3), (223, '内蒙古阿拉善右旗', 223, 3), (224, '内蒙古额济纳旗', 224, 3), (225, '辽宁省', 225, 3), (226, '辽宁省沈阳市', 226, 3), (227, '辽宁省新民县', 227, 3), (228, '辽宁省辽中县', 228, 3), (229, '辽宁省大连市', 229, 3), (230, '辽宁省新金县', 230, 3), (231, '辽宁省长海县', 231, 3), (232, '辽宁省庄河县', 232, 3), (233, '辽宁省鞍山市', 233, 3), (234, '辽宁省台安县', 234, 3), (235, '辽宁省抚顺市', 235, 3), (236, '辽宁省抚顺县', 236, 3), (237, '辽宁省新宾县', 237, 3), (238, '辽宁省清原县', 238, 3), (239, '辽宁省本溪市', 239, 3), (240, '辽宁省本溪县', 240, 3), (241, '辽宁省桓仁县', 241, 3), (242, '辽宁省丹东市', 242, 3), (243, '辽宁省凤城县', 243, 3), (244, '辽宁省岫岩县', 244, 3), (245, '辽宁省东沟县', 245, 3), (246, '辽宁省宽甸县', 246, 3), (247, '辽宁省锦州市', 247, 3), (248, '辽宁省绥中县', 248, 3), (249, '辽宁省锦  县', 249, 3), (250, '辽宁省北镇县', 250, 3), (251, '辽宁省黑山县', 251, 3), (252, '辽宁省义  县', 252, 3), (253, '辽宁省营口市', 253, 3), (254, '辽宁省营口县', 254, 3), (255, '辽宁省盖  县', 255, 3), (256, '辽宁省阜新市', 256, 3), (257, '辽宁省阜新县', 257, 3), (258, '辽宁省彰武县', 258, 3), (259, '辽宁省辽阳市', 259, 3), (260, '辽宁省辽阳县', 260, 3), (261, '辽宁省灯塔县', 261, 3), (262, '辽宁省盘锦市', 262, 3), (263, '辽宁省大洼县', 263, 3), (264, '辽宁省盘山县', 264, 3), (265, '辽宁省铁岭市', 265, 3), (266, '辽宁省铁岭县', 266, 3), (267, '辽宁省西丰县', 267, 3), (268, '辽宁省昌图县', 268, 3), (269, '辽宁省康平县', 269, 3), (270, '辽宁省法库县', 270, 3), (271, '辽宁省朝阳市', 271, 3), (272, '辽宁省朝阳县', 272, 3), (273, '辽宁省建平县', 273, 3), (274, '辽宁省凌源县', 274, 3), (275, '辽宁省喀喇沁县', 275, 3), (276, '辽宁省建昌县', 276, 3), (277, '辽宁省直辖行政单位', 277, 3), (278, '辽宁省瓦房店市', 278, 3), (279, '辽宁省海城市', 279, 3), (280, '辽宁省锦西市', 280, 3), (281, '辽宁省兴城市', 281, 3), (282, '辽宁省铁法市', 282, 3), (283, '辽宁省北票市', 283, 3), (284, '辽宁省开原市', 284, 3), (285, '吉林省', 285, 3), (286, '吉林省榆树县', 286, 3), (287, '吉林省农安县', 287, 3), (288, '吉林省德惠县', 288, 3), (289, '吉林省双阳县', 289, 3), (290, '吉林省吉林市', 290, 3), (291, '吉林省永吉县', 291, 3), (292, '吉林省舒兰县', 292, 3), (293, '吉林省磐石县', 293, 3), (294, '吉林省蛟河县', 294, 3), (295, '吉林省四平市', 295, 3), (296, '吉林省梨树县', 296, 3), (297, '吉林省伊通县', 297, 3), (298, '吉林省双辽县', 298, 3), (299, '吉林省辽源市', 299, 3), (300, '吉林省东丰县', 300, 3), (301, '吉林省东辽县', 301, 3), (302, '吉林省通化市', 302, 3), (303, '吉林省通化县', 303, 3), (304, '吉林省辉南县', 304, 3), (305, '吉林省柳河县', 305, 3), (306, '吉林省浑江市', 306, 3), (307, '吉林省抚松县', 307, 3), (308, '吉林省靖宇县', 308, 3), (309, '吉林省长白县', 309, 3), (310, '吉林省白城地区', 310, 3), (311, '吉林省白城市', 311, 3), (312, '吉林省洮南市', 312, 3), (313, '吉林省扶余市', 313, 3), (314, '吉林省大安市', 314, 3), (315, '吉林省长岭县', 315, 3), (316, '吉林省前郭尔罗斯县', 316, 3), (317, '吉林省镇赉县', 317, 3), (318, '吉林省通榆县', 318, 3), (319, '吉林省乾安县', 319, 3), (320, '吉林省延吉市', 320, 3), (321, '吉林省图们市', 321, 3), (322, '吉林省敦化市', 322, 3), (323, '吉林省珲春市', 323, 3), (324, '吉林省龙井市', 324, 3), (325, '吉林省和龙县', 325, 3), (326, '吉林省汪清县', 326, 3), (327, '吉林省安图县', 327, 3), (328, '吉林省公主岭市', 328, 3), (329, '吉林省梅河口市', 329, 3), (330, '吉林省集安市', 330, 3), (331, '吉林省桦甸市', 331, 3), (332, '吉林省九台市', 332, 3), (333, '黑龙江省', 333, 3), (334, '黑龙江哈尔滨市', 334, 3), (335, '黑龙江呼兰县', 335, 3), (336, '黑龙江齐齐哈尔市', 336, 3), (337, '黑龙江龙江县', 337, 3), (338, '黑龙江讷河县', 338, 3), (339, '黑龙江依安县', 339, 3), (340, '黑龙江泰来县', 340, 3), (341, '黑龙江甘南县', 341, 3), (342, '男', 342, 4), (343, '女', 343, 4), (344, '未知', 344, 4), (345, '不明', 345, 4), (346, 'string', 346, 5), (347, 'int', 347, 5), (348, 'double', 348, 5), (349, 'DateTime', 349, 5), (350, 'Date', 350, 5), (351, 'bool', 351, 5), (456661440205443072, '1', 1023, 456659310463700992), (456662703420755968, '2', 1026, 456659310463700992);

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
INSERT INTO `cm_option_group` VALUES (1, '民族'), (2, '学历'), (3, '地区'), (4, '性别'), (5, '数据类型'), (456659310463700992, '新组');

-- ----------------------------
-- Table structure for cm_params
-- ----------------------------
DROP TABLE IF EXISTS `cm_params`;
CREATE TABLE `cm_params`  (
  `id` bigint(20) NOT NULL COMMENT '用户参数标识',
  `name` varchar(255) NOT NULL COMMENT '参数名称',
  `value` varchar(255) NOT NULL COMMENT '参数缺省值',
  `note` varchar(255) NOT NULL COMMENT '参数描述',
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
-- Table structure for cm_permission
-- ----------------------------
DROP TABLE IF EXISTS `cm_permission`;
CREATE TABLE `cm_permission`  (
  `id` bigint(20) NOT NULL COMMENT '权限标识',
  `name` varchar(64) NOT NULL COMMENT '权限名称',
  `note` varchar(255) NULL DEFAULT NULL COMMENT '权限描述',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_permission_name`(`name`) USING BTREE COMMENT '不重复'
) ENGINE = InnoDB COMMENT = '权限';

-- ----------------------------
-- Records of cm_permission
-- ----------------------------
INSERT INTO `cm_permission` VALUES (1, '公共文件管理', '禁止删除'), (2, '素材库管理', '禁止删除'), (455253883184238592, '测试1', '');

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
INSERT INTO `cm_role` VALUES (1, '任何人', '所有用户默认都具有该角色，不可删除'), (2, '系统管理员', '系统角色，不可删除'), (22844822693027840, '收发员', ''), (152695933758603264, '市场经理', ''), (152696004814307328, '综合经理', ''), (152696042718232576, '财务经理', '');

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
INSERT INTO `cm_role_menu` VALUES (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (1, 7), (1, 8), (1, 9), (2, 10), (1, 15315637929975808), (2, 18860286065975296), (22844822693027840, 154430055023640576);

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
INSERT INTO `cm_role_per` VALUES (1, 1), (1, 2), (22844822693027840, 455253883184238592), (152696004814307328, 455253883184238592);

-- ----------------------------
-- Table structure for cm_rpt
-- ----------------------------
DROP TABLE IF EXISTS `cm_rpt`;
CREATE TABLE `cm_rpt`  (
  `id` bigint(20) NOT NULL COMMENT '报表标识',
  `name` varchar(64) NOT NULL COMMENT '报表名称',
  `define` varchar(21000) NOT NULL COMMENT '报表模板定义',
  `note` varchar(255) NOT NULL COMMENT '报表描述',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_rpt_name`(`name`) USING BTREE
) ENGINE = InnoDB COMMENT = '报表模板定义';

-- ----------------------------
-- Records of cm_rpt
-- ----------------------------
INSERT INTO `cm_rpt` VALUES (139241259579338752, '测试报表111', '<Rpt cols=\"80,80,80,80,80,80,80\">\r\n  <Params>\r\n    <Param name=\"新参数1\"><![CDATA[<a:CText Title=\"标题1\" />]]></Param>\r\n    <Param name=\"新参数2\"><![CDATA[<a:CText Title=\"标题2\" />]]></Param>\r\n  </Params>\r\n  <Data />\r\n  <Page />\r\n  <Header />\r\n  <Body rows=\"30,30,30,30,30,30,30,30,30,30\">\r\n    <Text row=\"4\" col=\"6\" val=\"文本\" lbs=\"None\" tbs=\"None\" rbs=\"None\" bbs=\"None\" />\r\n    <Text row=\"7\" col=\"6\" rowspan=\"3\" val=\"文本\" lbs=\"None\" tbs=\"None\" rbs=\"None\" bbs=\"None\" />\r\n  </Body>\r\n  <Footer />\r\n  <View />\r\n</Rpt>', '新增测试1', '2020-10-19 13:35:10', '2023-06-28 08:39:08'), (139540400075304960, 'abc1', '<Rpt cols=\"80,80,80,80,80\">\r\n  <Params />\r\n  <Data />\r\n  <Page />\r\n  <Header />\r\n  <Body rows=\"30,30,30,30,30,30,30,30,30,30,30,30,30\">\r\n    <Text row=\"2\" col=\"2\" val=\"文本\" lbs=\"None\" tbs=\"None\" rbs=\"None\" bbs=\"None\" />\r\n    <Text row=\"4\" col=\"3\" colspan=\"2\" val=\"文本\" lbs=\"None\" tbs=\"None\" rbs=\"None\" bbs=\"None\" />\r\n    <Text row=\"7\" col=\"3\" val=\"文本\" lbs=\"None\" tbs=\"None\" rbs=\"None\" bbs=\"None\" />\r\n    <Text row=\"12\" col=\"4\" val=\"文本\" lbs=\"None\" tbs=\"None\" rbs=\"None\" bbs=\"None\" />\r\n  </Body>\r\n  <Footer />\r\n  <View />\r\n</Rpt>', '阿斯顿法定', '2020-10-20 09:24:01', '2023-03-13 16:14:41'), (150118388697264128, 'abc12', '', '', '2020-11-18 13:57:21', '2020-11-18 13:57:21'), (154424288497369088, '新报表abc', '', '', '2020-11-30 11:07:07', '2020-11-30 11:07:07'), (259588273038290944, '新报表3', '', '', '2021-09-16 15:51:31', '2021-09-16 15:51:53');

-- ----------------------------
-- Table structure for cm_user
-- ----------------------------
DROP TABLE IF EXISTS `cm_user`;
CREATE TABLE `cm_user`  (
  `id` bigint(20) NOT NULL COMMENT '用户标识',
  `name` varchar(32) NOT NULL COMMENT '账号，唯一',
  `phone` varchar(16) NOT NULL COMMENT '手机号，唯一',
  `pwd` char(32) NOT NULL COMMENT '密码的md5',
  `photo` varchar(255) NOT NULL DEFAULT '' COMMENT '头像',
  `expired` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否停用',
  `ctime` datetime NOT NULL COMMENT '创建时间',
  `mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_user_name`(`name`) USING BTREE,
  INDEX `idx_user_phone`(`phone`) USING BTREE
) ENGINE = InnoDB COMMENT = '系统用户';

-- ----------------------------
-- Records of cm_user
-- ----------------------------
INSERT INTO `cm_user` VALUES (1, 'Windows', '13511111111', 'b59c67bf196a4758191e42f76670ceba', '[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]', 0, '2019-10-24 09:06:38', '2023-03-16 08:35:39');
INSERT INTO `cm_user` VALUES (2, '安卓', '13522222222', 'b59c67bf196a4758191e42f76670ceba', '[[\"photo/2.jpg\",\"2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]', 0, '2019-10-24 13:03:19', '2023-03-16 08:36:23');
INSERT INTO `cm_user` VALUES (3, '苹果', '13533333333', 'b59c67bf196a4758191e42f76670ceba', '[[\"photo/3.jpg\",\"3\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]', 0, '0001-01-01 00:00:00', '2023-03-16 08:36:46');
INSERT INTO `cm_user` VALUES (149709966847897600, '李市场', '13122222222', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2020-11-17 10:54:29', '2020-11-25 16:37:55');
INSERT INTO `cm_user` VALUES (152695627289198592, '王综合', '13211111111', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2020-11-25 16:38:34', '2020-11-25 16:38:34');
INSERT INTO `cm_user` VALUES (152695790787362816, '张财务', '13866666666', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2020-11-25 16:38:54', '2020-11-25 16:38:54');
INSERT INTO `cm_user` VALUES (184215437633777664, '15955555555', '15955555555', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2021-02-20 16:06:23', '2021-02-20 16:06:23');
INSERT INTO `cm_user` VALUES (185188338092601344, '15912345678', '15912345678', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2021-02-23 08:32:20', '2021-02-23 08:32:20');
INSERT INTO `cm_user` VALUES (185212597401677824, '15912345677', '15912345671', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2021-02-23 10:08:43', '2022-01-19 15:49:43');
INSERT INTO `cm_user` VALUES (192818293676994560, '18543175028', '18543175028', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2021-03-16 09:51:02', '2021-03-16 09:51:02');
INSERT INTO `cm_user` VALUES (196167762048839680, '18843175028', '18843175028', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2021-03-25 15:40:38', '2021-03-25 15:40:38');
INSERT INTO `cm_user` VALUES (224062063923556352, '14411111111', '14411111111', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2021-06-10 15:02:39', '2021-06-10 15:02:39');
INSERT INTO `cm_user` VALUES (227949556179791872, 'WebAssembly', '13612345678', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2021-06-21 08:30:10', '2021-06-21 08:30:34');
INSERT INTO `cm_user` VALUES (229519641138819072, '13311111111', '13311111111', 'b59c67bf196a4758191e42f76670ceba', '[[\"editor/E3/18/452737920958222336.jpg\",\"未标题-2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2023-03-03 15:38\"]]', 0, '2021-06-25 16:29:06', '2021-06-25 16:29:06');
INSERT INTO `cm_user` VALUES (231620526086156288, '13611111111', '13611111111', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2021-07-01 11:37:18', '2021-07-01 11:37:18');
INSERT INTO `cm_user` VALUES (247170018466197504, '15948341892', '15948341897', 'b59c67bf196a4758191e42f76670ceba', '', 0, '2021-08-13 09:25:26', '2021-09-10 09:36:37');

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
-- Records of cm_user_group
-- ----------------------------
INSERT INTO `cm_user_group` VALUES (1, 454483802783240192), (1, 454484924033945600), (149709966847897600, 454484847190102016);

-- ----------------------------
-- Table structure for cm_user_params
-- ----------------------------
DROP TABLE IF EXISTS `cm_user_params`;
CREATE TABLE `cm_user_params`  (
  `user_id` bigint(20) NOT NULL COMMENT '用户标识',
  `param_id` bigint(20) NOT NULL COMMENT '参数标识',
  `value` varchar(255) NOT NULL COMMENT '参数值',
  `mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`user_id`, `param_id`) USING BTREE,
  INDEX `idx_user_params_userid`(`user_id`) USING BTREE,
  INDEX `idx_user_params_paramsid`(`param_id`) USING BTREE,
  CONSTRAINT `fk_user_params_paramsid` FOREIGN KEY (`param_id`) REFERENCES `cm_params` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_user_params_userid` FOREIGN KEY (`user_id`) REFERENCES `cm_user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB COMMENT = '用户参数值';

-- ----------------------------
-- Records of cm_user_params
-- ----------------------------
INSERT INTO `cm_user_params` VALUES (2, 1, 'false', '2020-12-04 13:29:05');

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
INSERT INTO `cm_user_role` VALUES (1, 2), (1, 22844822693027840), (1, 152695933758603264), (1, 152696004814307328), (2, 2), (2, 22844822693027840), (2, 152695933758603264), (3, 2), (149709966847897600, 2), (149709966847897600, 152695933758603264), (152695627289198592, 152696004814307328), (152695790787362816, 152696042718232576), (247170018466197504, 22844822693027840);

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
-- Records of cm_wfd_atv
-- ----------------------------
INSERT INTO `cm_wfd_atv` VALUES (146898715155492864, 146898695127691264, '开始', 1, 0, 0, NULL, 1, 1, 0, 0, 0, 0, '2020-11-09 16:43:10', '2020-11-09 16:43:10'), (146898876447453184, 146898695127691264, '任务项', 0, 0, 0, NULL, 1, 0, 0, 0, 0, 0, '2020-11-09 16:43:48', '2020-11-09 16:43:48'), (146900570585559040, 146900552231284736, '开始', 1, 0, 0, NULL, 1, 1, 0, 0, 0, 0, '2020-11-09 16:50:32', '2020-11-09 16:50:32'), (146900847761944576, 146900823984435200, '开始', 1, 0, 0, NULL, 1, 1, 0, 0, 0, 0, '2020-11-09 16:51:38', '2020-11-09 16:51:38'), (146901433265811456, 146901403339452416, '开始', 1, 0, 0, NULL, 1, 1, 0, 0, 0, 0, '2020-11-09 16:53:58', '2020-11-09 16:53:58'), (147141181158846464, 147141147767992320, '开始', 1, 0, 0, NULL, 1, 1, 0, 0, 0, 0, '2020-11-10 08:46:31', '2020-11-10 08:46:31'), (147141718000398336, 147141147767992320, '任务项', 0, 0, 0, NULL, 1, 0, 0, 0, 0, 0, '2020-11-10 08:48:39', '2020-11-10 08:48:39'), (152588671081775104, 152588581545967616, '接收文件', 1, 0, 0, NULL, 1, 1, 0, 0, 0, 0, '2020-11-25 09:32:55', '2020-12-09 10:45:33'), (152683112727576576, 152588581545967616, '市场部', 0, 0, 0, NULL, 1, 0, 0, 0, 2, 0, '2020-11-25 15:48:12', '2020-12-14 15:36:36'), (152684512937246720, 152588581545967616, '综合部', 0, 2, 0, NULL, 1, 0, 0, 0, 2, 0, '2020-11-25 15:53:46', '2020-12-14 15:33:30'), (152684758027206656, 152588581545967616, '市场部传阅', 0, 0, 0, NULL, 1, 0, 0, 0, 0, 0, '2020-11-25 15:54:44', '2020-11-25 15:56:10'), (152684895835258880, 152588581545967616, '同步', 2, 0, 0, NULL, 1, 0, 0, 0, 0, 2, '2020-11-25 15:55:17', '2020-12-16 08:39:31'), (152685032993193984, 152588581545967616, '综合部传阅', 0, 0, 0, NULL, 1, 0, 0, 0, 0, 0, '2020-11-25 15:55:50', '2020-11-25 15:56:10'), (152685491275431936, 152588581545967616, '返回收文人', 0, 0, 0, NULL, 1, 0, 0, 0, 0, 0, '2020-11-25 15:57:39', '2020-11-25 15:58:18'), (152685608543977472, 152588581545967616, '完成', 3, 0, 0, NULL, 1, 0, 0, 0, 0, 0, '2020-11-25 15:58:07', '2020-11-25 15:58:07');

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
-- Records of cm_wfd_atv_role
-- ----------------------------
INSERT INTO `cm_wfd_atv_role` VALUES (146898715155492864, 1), (146900570585559040, 1), (146900847761944576, 1), (146901433265811456, 1), (146898715155492864, 2), (146900570585559040, 2), (146901433265811456, 2), (152588671081775104, 22844822693027840), (152684758027206656, 22844822693027840), (152685032993193984, 22844822693027840), (152685491275431936, 22844822693027840), (152683112727576576, 152695933758603264), (152684512937246720, 152696004814307328);

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
-- Records of cm_wfd_prc
-- ----------------------------
INSERT INTO `cm_wfd_prc` VALUES (146898695127691264, '555', '<Sketch><Node id=\"146898715155492864\" title=\"开始\" shape=\"开始\" left=\"340\" top=\"100\" width=\"80\" height=\"60\" /><Node id=\"146898876447453184\" title=\"任务项\" shape=\"任务\" left=\"340\" top=\"360\" width=\"120\" height=\"60\" /><Line id=\"146898896794021888\" headerid=\"146898715155492864\" bounds=\"380,160,30,200\" headerport=\"4\" tailid=\"146898876447453184\" tailport=\"0\" /></Sketch>', 0, 0, '', 1, '0001-01-01 00:00:00', '2020-11-19 13:17:25'), (146900552231284736, '666', '<Sketch><Node id=\"146900570585559040\" title=\"开始\" shape=\"开始\" left=\"620\" top=\"120\" width=\"80\" height=\"60\" /></Sketch>', 0, 0, '', 3, '0001-01-01 00:00:00', '2020-11-09 16:50:56'), (146900823984435200, '777', '<Sketch><Node id=\"146900847761944576\" title=\"开始\" shape=\"开始\" left=\"300\" top=\"220\" width=\"80\" height=\"60\" /></Sketch>', 0, 0, '', 4, '0001-01-01 00:00:00', '2020-11-09 16:52:58'), (146901403339452416, '888', '<Sketch><Node id=\"146901433265811456\" title=\"开始\" shape=\"开始\" left=\"340\" top=\"140\" width=\"80\" height=\"60\" /></Sketch>', 0, 0, '', 6, '0001-01-01 00:00:00', '2020-11-09 16:54:39'), (147141147767992320, 'ggg', '<Sketch><Node id=\"147141181158846464\" title=\"开始\" shape=\"开始\" left=\"320\" top=\"40\" width=\"80\" height=\"60\" /><Node id=\"147141718000398336\" title=\"任务项\" shape=\"任务\" left=\"380\" top=\"480\" width=\"120\" height=\"60\" /><Line id=\"147141749642227712\" headerid=\"147141181158846464\" bounds=\"400,100,50,380\" headerport=\"3\" tailid=\"147141718000398336\" tailport=\"0\" /></Sketch>', 1, 0, '', 2, '2020-11-10 08:46:24', '2020-11-10 08:50:03'), (152588581545967616, '收文样例', '<Sketch><Node id=\"152588671081775104\" title=\"接收文件\" shape=\"开始\" left=\"300\" top=\"40\" width=\"80\" height=\"60\" /><Node id=\"152683112727576576\" title=\"市场部\" shape=\"任务\" left=\"160\" top=\"140\" width=\"120\" height=\"60\" /><Line id=\"152683122982649856\" headerid=\"152588671081775104\" bounds=\"210,70,50,70\" headerport=\"6\" tailid=\"152683112727576576\" tailport=\"0\" /><Node id=\"152684512937246720\" title=\"综合部\" shape=\"任务\" left=\"400\" top=\"140\" width=\"120\" height=\"60\" /><Line id=\"152684673721696256\" headerid=\"152588671081775104\" bounds=\"380,70,90,70\" headerport=\"2\" tailid=\"152684512937246720\" tailport=\"0\" /><Node id=\"152684758027206656\" title=\"市场部传阅\" shape=\"任务\" left=\"160\" top=\"260\" width=\"120\" height=\"60\" /><Node id=\"152684895835258880\" title=\"同步\" shape=\"同步\" background=\"#FF9D9D9D\" borderbrush=\"#FF969696\" left=\"280\" top=\"400\" width=\"120\" height=\"60\" /><Line id=\"152684951493672960\" headerid=\"152683112727576576\" bounds=\"210,200,20,60\" headerport=\"4\" tailid=\"152684758027206656\" tailport=\"0\" /><Line id=\"152684981348728832\" headerid=\"152683112727576576\" bounds=\"120,170,160,470\" headerport=\"6\" tailid=\"152685608543977472\" tailport=\"6\" /><Node id=\"152685032993193984\" title=\"综合部传阅\" shape=\"任务\" left=\"400\" top=\"260\" width=\"120\" height=\"60\" /><Line id=\"152685133509689344\" headerid=\"152684512937246720\" bounds=\"450,200,20,60\" headerport=\"4\" tailid=\"152685032993193984\" tailport=\"0\" /><Line id=\"152685169891082240\" headerid=\"152684512937246720\" bounds=\"400,170,160,270\" headerport=\"2\" tailid=\"152684895835258880\" tailport=\"2\" /><Line id=\"152685211767013376\" headerid=\"152684758027206656\" bounds=\"220,320,60,120\" headerport=\"4\" tailid=\"152684895835258880\" tailport=\"6\" /><Line id=\"152685247745753088\" headerid=\"152685032993193984\" bounds=\"400,320,60,120\" headerport=\"4\" tailid=\"152684895835258880\" tailport=\"2\" /><Node id=\"152685491275431936\" title=\"返回收文人\" shape=\"任务\" left=\"280\" top=\"500\" width=\"120\" height=\"60\" /><Line id=\"152685585135566848\" headerid=\"152684895835258880\" bounds=\"330,460,20,40\" headerport=\"4\" tailid=\"152685491275431936\" tailport=\"0\" /><Node id=\"152685608543977472\" title=\"完成\" shape=\"结束\" background=\"#FF9D9D9D\" borderbrush=\"#FF969696\" left=\"300\" top=\"600\" width=\"80\" height=\"60\" /><Line id=\"152685622099968000\" headerid=\"152685491275431936\" bounds=\"330,560,20,40\" headerport=\"4\" tailid=\"152685608543977472\" tailport=\"0\" /></Sketch>', 0, 0, '', 5, '2020-11-25 09:32:33', '2021-08-24 15:45:54');

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
-- Records of cm_wfd_trs
-- ----------------------------
INSERT INTO `cm_wfd_trs` VALUES (146898896794021888, 146898695127691264, 146898715155492864, 146898876447453184, 0, NULL), (147141749642227712, 147141147767992320, 147141181158846464, 147141718000398336, 0, NULL), (152683122982649856, 152588581545967616, 152588671081775104, 152683112727576576, 0, NULL), (152684673721696256, 152588581545967616, 152588671081775104, 152684512937246720, 0, NULL), (152684951493672960, 152588581545967616, 152683112727576576, 152684758027206656, 0, NULL), (152684981348728832, 152588581545967616, 152683112727576576, 152685608543977472, 0, NULL), (152685133509689344, 152588581545967616, 152684512937246720, 152685032993193984, 0, NULL), (152685169891082240, 152588581545967616, 152684512937246720, 152684895835258880, 0, NULL), (152685211767013376, 152588581545967616, 152684758027206656, 152684895835258880, 0, NULL), (152685247745753088, 152588581545967616, 152685032993193984, 152684895835258880, 0, NULL), (152685585135566848, 152588581545967616, 152684895835258880, 152685491275431936, 0, NULL), (152685622099968000, 152588581545967616, 152685491275431936, 152685608543977472, 0, NULL), (160910207789953024, 152588581545967616, 152683112727576576, 152588671081775104, 1, 152683122982649856);

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
-- Records of cm_wfi_atv
-- ----------------------------
INSERT INTO `cm_wfi_atv` VALUES (162025231375790080, 162025231350624256, 152588671081775104, 1, 1, '2020-12-21 10:30:29', '2020-12-21 10:30:31'), (162025255044247552, 162025231350624256, 152683112727576576, 1, 1, '2020-12-21 10:30:31', '2020-12-21 16:45:05'), (162119526644576256, 162025231350624256, 152684758027206656, 1, 1, '2020-12-21 16:45:05', '2020-12-21 16:45:11'), (162119548043915264, 162025231350624256, 152684895835258880, 3, 1, '2020-12-21 16:45:11', '2020-12-21 16:45:11'), (162119548199104512, 162025231350624256, 152685491275431936, 1, 1, '2020-12-21 16:45:11', '2020-12-21 16:45:13'), (162401333625614336, 162401333600448512, 152588671081775104, 1, 1, '2020-12-22 11:25:22', '2023-03-16 10:42:58'), (457374494836674560, 162401333600448512, 152683112727576576, 1, 1, '2023-03-16 10:42:57', '2023-03-16 11:10:31'), (457374495587454976, 162401333600448512, 152684512937246720, 0, 1, '2023-03-16 10:42:57', '2023-03-16 10:42:57'), (457381430491631616, 162401333600448512, 152684758027206656, 0, 1, '2023-03-16 11:10:31', '2023-03-16 11:10:31'), (457384397022187520, 457384396879581184, 152588671081775104, 1, 1, '2023-03-16 11:22:27', '2023-03-16 11:23:30'), (457384696747151360, 457384396879581184, 152683112727576576, 1, 1, '2023-03-16 11:23:29', '2023-03-16 11:27:51'), (457384697418240000, 457384396879581184, 152684512937246720, 1, 1, '2023-03-16 11:23:29', '2023-03-16 11:28:13'), (457385791041064960, 457384396879581184, 152684758027206656, 0, 2, '2023-03-16 11:27:50', '2023-03-16 11:27:50'), (457385885710700544, 457384396879581184, 152685032993193984, 0, 1, '2023-03-16 11:28:13', '2023-03-16 11:28:13'), (457388173628035072, 457388173615452160, 152588671081775104, 1, 1, '2023-03-16 11:37:33', '2023-03-16 11:38:10'), (457388387768225792, 457388173615452160, 152683112727576576, 1, 1, '2023-03-16 11:38:10', '2023-03-16 11:38:50'), (457388561571794944, 457388173615452160, 152684758027206656, 0, 1, '2023-03-16 11:38:49', '2023-03-16 11:38:49');

-- ----------------------------
-- Table structure for cm_wfi_item
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfi_item`;
CREATE TABLE `cm_wfi_item`  (
  `id` bigint(20) NOT NULL COMMENT '工作项标识',
  `atvi_id` bigint(20) NOT NULL COMMENT '活动实例标识',
  `status` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfiItemStatus#工作项状态 0活动 1结束 2终止 3同步活动',
  `assign_kind` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfiItemAssignKind#指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派',
  `sender` varchar(32) NOT NULL COMMENT '发送者',
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
-- Records of cm_wfi_item
-- ----------------------------
INSERT INTO `cm_wfi_item` VALUES (162025231392567296, 162025231375790080, 1, 1, 'daoting', '2020-12-21 10:30:29', 1, '2020-12-21 10:30:29', NULL, 1, '', 157, '2020-12-21 10:30:29', '2020-12-21 10:30:31'), (162025255065219072, 162025255044247552, 1, 0, 'daoting', '2020-12-21 10:30:31', 1, '2020-12-21 13:27:15', NULL, 1, '', 158, '2020-12-21 10:30:31', '2020-12-21 16:45:05'), (162119526686519296, 162119526644576256, 1, 0, 'daoting', '2020-12-21 16:45:05', 1, '2020-12-21 16:45:07', NULL, 1, '', 159, '2020-12-21 16:45:05', '2020-12-21 16:45:11'), (162119548064886784, 162119548043915264, 3, 0, 'daoting', '2020-12-21 16:45:11', 0, NULL, NULL, 1, '', 160, '2020-12-21 16:45:11', '2020-12-21 16:45:11'), (162119548220076032, 162119548199104512, 1, 0, 'daoting', '2020-12-21 16:45:11', 1, '2020-12-21 16:45:12', NULL, 1, '', 161, '2020-12-21 16:45:11', '2020-12-21 16:45:13'), (162401333642391552, 162401333625614336, 1, 1, 'daoting', '2020-12-22 11:25:22', 1, '2020-12-22 11:25:22', NULL, 1, '', 162, '2020-12-22 11:25:22', '2023-03-16 10:42:58'), (457374495021223936, 457374494836674560, 1, 0, '', '2023-03-16 10:42:57', 1, '2023-03-16 10:43:13', NULL, 1, '', 163, '2023-03-16 10:42:57', '2023-03-16 11:10:31'), (457374495696506880, 457374495587454976, 0, 0, '', '2023-03-16 10:42:57', 0, NULL, NULL, 152695627289198592, '', 164, '2023-03-16 10:42:57', '2023-03-16 10:42:57'), (457381430646820864, 457381430491631616, 0, 0, '', '2023-03-16 11:10:31', 1, '2023-03-16 11:11:00', NULL, 1, '', 165, '2023-03-16 11:10:31', '2023-03-16 11:10:31'), (457384397164793856, 457384397022187520, 1, 1, 'Windows', '2023-03-16 11:22:27', 1, '2023-03-16 11:22:27', NULL, 1, '', 167, '2023-03-16 11:22:27', '2023-03-16 11:23:30'), (457384696902340608, 457384696747151360, 1, 0, '', '2023-03-16 11:23:29', 1, '2023-03-16 11:23:45', NULL, 1, '', 168, '2023-03-16 11:23:29', '2023-03-16 11:27:51'), (457384697523097600, 457384697418240000, 1, 0, '', '2023-03-16 11:23:29', 1, '2023-03-16 11:23:46', NULL, 1, '', 169, '2023-03-16 11:23:29', '2023-03-16 11:28:13'), (457385791196254208, 457385791041064960, 1, 0, '', '2023-03-16 11:27:50', 1, '2023-03-16 11:28:02', NULL, 1, '', 170, '2023-03-16 11:27:50', '2023-03-16 11:28:25'), (457385791531798528, 457385791041064960, 0, 0, '', '2023-03-16 11:27:50', 0, NULL, NULL, 247170018466197504, '', 171, '2023-03-16 11:27:50', '2023-03-16 11:27:50'), (457385885811363840, 457385885710700544, 0, 0, '', '2023-03-16 11:28:13', 0, NULL, NULL, 2, '', 172, '2023-03-16 11:28:13', '2023-03-16 11:28:13'), (457388173640617984, 457388173628035072, 1, 1, 'Windows', '2023-03-16 11:37:33', 1, '2023-03-16 11:37:33', NULL, 1, '', 174, '2023-03-16 11:37:33', '2023-03-16 11:38:10'), (457388387776614400, 457388387768225792, 1, 0, '', '2023-03-16 11:38:10', 1, '2023-03-16 11:38:22', NULL, 2, '', 175, '2023-03-16 11:38:10', '2023-03-16 11:38:50'), (457388561714401280, 457388561571794944, 0, 0, '', '2023-03-16 11:38:49', 0, NULL, NULL, 1, '', 176, '2023-03-16 11:38:49', '2023-03-16 11:38:49');

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
-- Records of cm_wfi_prc
-- ----------------------------
INSERT INTO `cm_wfi_prc` VALUES (162025231350624256, 152588581545967616, 'a', 1, 58, '2020-12-21 10:30:29', '2020-12-21 16:45:13'), (162401333600448512, 152588581545967616, '关于新冠疫情的批示', 0, 59, '2020-12-22 11:25:22', '2020-12-22 11:25:22'), (457384396879581184, 152588581545967616, '阿斯蒂芬', 0, 64, '2023-03-16 11:22:27', '2023-03-16 11:22:27'), (457388173615452160, 152588581545967616, '疫情在', 0, 65, '2023-03-16 11:37:33', '2023-03-16 11:37:33');

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
-- Records of cm_wfi_trs
-- ----------------------------
INSERT INTO `cm_wfi_trs` VALUES (162025255165882368, 152683122982649856, 162025231375790080, 162025255044247552, 0, '2020-12-21 10:30:31'), (162119526820737024, 152684951493672960, 162025255044247552, 162119526644576256, 0, '2020-12-21 16:45:05'), (162119548186521600, 152685211767013376, 162119526644576256, 162119548043915264, 0, '2020-12-21 16:45:11'), (162119548320739328, 152685585135566848, 162119548043915264, 162119548199104512, 0, '2020-12-21 16:45:11'), (457374495470014464, 152683122982649856, 162401333625614336, 457374494836674560, 0, '2023-03-16 10:42:57'), (457374496069799936, 152684673721696256, 162401333625614336, 457374495587454976, 0, '2023-03-16 10:42:57'), (457381431104000000, 152684951493672960, 457374494836674560, 457381430491631616, 0, '2023-03-16 11:10:31'), (457384697296605184, 152683122982649856, 457384397022187520, 457384696747151360, 0, '2023-03-16 11:23:29'), (457384697883807744, 152684673721696256, 457384397022187520, 457384697418240000, 0, '2023-03-16 11:23:29'), (457385791921868800, 152684951493672960, 457384696747151360, 457385791041064960, 0, '2023-03-16 11:27:50'), (457385886172073984, 152685133509689344, 457384697418240000, 457385885710700544, 0, '2023-03-16 11:28:13'), (457388387831140352, 152683122982649856, 457388173628035072, 457388387768225792, 0, '2023-03-16 11:38:10'), (457388562041556992, 152684951493672960, 457388387768225792, 457388561571794944, 0, '2023-03-16 11:38:49');

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
-- Records of fsm_file
-- ----------------------------

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
INSERT INTO `sequence` VALUES ('cm_menu_dispidx', 89), ('cm_option_dispidx', 1031), ('cm_wfd_prc_dispidx', 11), ('cm_wfi_item_dispidx', 176), ('cm_wfi_prc_dispidx', 65), ('demo_crud_dispidx', 84), ('demo_基础_序列', 11);

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

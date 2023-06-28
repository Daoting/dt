/*
 Navicat Premium Data Transfer

 Source Server         : server2
 Source Server Type    : MySQL
 Source Server Version : 50721 (5.7.21-log)
 Source Host           : 10.10.1.2:3306
 Source Schema         : dt

 Target Server Type    : MySQL
 Target Server Version : 50721 (5.7.21-log)
 File Encoding         : 65001

 Date: 27/06/2023 16:32:52
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for cm_file_my
-- ----------------------------
DROP TABLE IF EXISTS `cm_file_my`;
CREATE TABLE `cm_file_my`  (
  `ID` bigint(20) NOT NULL COMMENT '文件标识',
  `ParentID` bigint(20) NULL DEFAULT NULL COMMENT '上级目录，根目录的parendid为空',
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '名称',
  `IsFolder` tinyint(1) NOT NULL COMMENT '是否为文件夹',
  `ExtName` varchar(8) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '文件扩展名',
  `Info` varchar(512) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '文件描述信息',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `UserID` bigint(20) NOT NULL COMMENT '所属用户',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fk_myfile_parentid`(`ParentID`) USING BTREE,
  INDEX `fk_user_userid`(`UserID`) USING BTREE,
  CONSTRAINT `fk_myfile_parentid` FOREIGN KEY (`ParentID`) REFERENCES `cm_file_my` (`ID`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_user_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '个人文件';


-- ----------------------------
-- Table structure for cm_file_pub
-- ----------------------------
DROP TABLE IF EXISTS `cm_file_pub`;
CREATE TABLE `cm_file_pub`  (
  `ID` bigint(20) NOT NULL COMMENT '文件标识',
  `ParentID` bigint(20) NULL DEFAULT NULL COMMENT '上级目录，根目录的parendid为空',
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '名称',
  `IsFolder` tinyint(1) NOT NULL COMMENT '是否为文件夹',
  `ExtName` varchar(8) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '文件扩展名',
  `Info` varchar(512) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '文件描述信息',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fk_pubfile_parentid`(`ParentID`) USING BTREE,
  CONSTRAINT `fk_pubfile_parentid` FOREIGN KEY (`ParentID`) REFERENCES `cm_file_pub` (`ID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '公共文件';

-- ----------------------------
-- Records of cm_file_pub
-- ----------------------------
BEGIN;
INSERT INTO `cm_file_pub` (`ID`, `ParentID`, `Name`, `IsFolder`, `ExtName`, `Info`, `Ctime`) VALUES (1, NULL, '公共文件', 1, NULL, '', '2020-10-21 15:19:20'), (2, NULL, '素材库', 1, NULL, '', '2020-10-21 15:20:21');
COMMIT;

-- ----------------------------
-- Table structure for cm_group
-- ----------------------------
DROP TABLE IF EXISTS `cm_group`;
CREATE TABLE `cm_group`  (
  `ID` bigint(20) NOT NULL COMMENT '组标识',
  `Name` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '组名',
  `Note` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '组描述',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `idx_group_name`(`Name`) USING BTREE COMMENT '不重复'
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '分组，与用户和角色多对多';


-- ----------------------------
-- Table structure for cm_group_role
-- ----------------------------
DROP TABLE IF EXISTS `cm_group_role`;
CREATE TABLE `cm_group_role`  (
  `GroupID` bigint(20) NOT NULL COMMENT '组标识',
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`GroupID`, `RoleID`) USING BTREE,
  INDEX `fk_grouprole_roleid`(`RoleID`) USING BTREE,
  INDEX `fk_grouprole_groupid`(`GroupID`) USING BTREE,
  CONSTRAINT `fk_grouprole_groupid` FOREIGN KEY (`GroupID`) REFERENCES `cm_group` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_grouprole_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '组一角色多对多';


-- ----------------------------
-- Table structure for cm_menu
-- ----------------------------
DROP TABLE IF EXISTS `cm_menu`;
CREATE TABLE `cm_menu`  (
  `ID` bigint(20) NOT NULL COMMENT '菜单标识',
  `ParentID` bigint(20) NULL DEFAULT NULL COMMENT '父菜单标识',
  `Name` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '菜单名称',
  `IsGroup` tinyint(1) NOT NULL COMMENT '分组或实例。0表实例，1表分组',
  `ViewName` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '视图名称',
  `Params` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '传递给菜单程序的参数',
  `Icon` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '图标',
  `Note` varchar(512) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '备注',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `IsLocked` tinyint(1) NOT NULL DEFAULT 0 COMMENT '定义了菜单是否被锁定。0表未锁定，1表锁定不可用',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fk_menu_parentid`(`ParentID`) USING BTREE,
  CONSTRAINT `fk_menu_parentid` FOREIGN KEY (`ParentID`) REFERENCES `cm_menu` (`ID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '业务菜单';

-- ----------------------------
-- Records of cm_menu
-- ----------------------------
BEGIN;
INSERT INTO `cm_menu` (`ID`, `ParentID`, `Name`, `IsGroup`, `ViewName`, `Params`, `Icon`, `Note`, `Dispidx`, `IsLocked`, `Ctime`, `Mtime`) VALUES (1, NULL, '工作台', 1, '', '', '搬运工', '', 1, 0, '2019-03-07 10:45:44', '2019-03-07 10:45:43'), (2, 1, '用户账号', 0, '用户账号', '', '钥匙', '', 2, 0, '2019-11-08 11:42:28', '2019-11-08 11:43:53'), (3, 1, '菜单管理', 0, '菜单管理', '', '大图标', '', 3, 0, '2019-03-11 11:35:59', '2019-03-11 11:35:58'), (4, 1, '系统角色', 0, '系统角色', '', '两人', '', 4, 0, '2019-11-08 11:47:21', '2019-11-08 11:48:22'), (5, 1, '分组管理', 0, '分组管理', '', '分组', '', 5, 0, '2023-03-10 08:34:49', '2023-03-10 08:34:49'), (6, 1, '基础权限', 0, '基础权限', '', '审核', '', 6, 0, '2019-03-12 09:11:22', '2019-03-07 11:23:40'), (7, 1, '参数定义', 0, '参数定义', '', '调色板', '', 7, 0, '2019-03-12 15:35:56', '2019-03-12 15:37:10'), (8, 1, '基础选项', 0, '基础选项', '', '修理', '', 8, 0, '2019-11-08 11:49:40', '2019-11-08 11:49:46'), (9, 1, '报表设计', 0, '报表设计', '', '折线图', '', 76, 0, '2020-10-19 11:21:38', '2020-10-19 11:21:38'), (10, 1, '流程设计', 0, '流程设计', '', '双绞线', '', 79, 0, '2020-11-02 16:21:19', '2020-11-02 16:21:19');
COMMIT;

-- ----------------------------
-- Table structure for cm_option
-- ----------------------------
DROP TABLE IF EXISTS `cm_option`;
CREATE TABLE `cm_option`  (
  `ID` bigint(20) NOT NULL COMMENT '标识',
  `Name` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '选项名称',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `GroupID` bigint(20) NOT NULL COMMENT '所属分组',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fk_option_groupid`(`GroupID`) USING BTREE,
  CONSTRAINT `fk_option_groupid` FOREIGN KEY (`GroupID`) REFERENCES `cm_option_group` (`ID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '基础选项';

-- ----------------------------
-- Records of cm_option
-- ----------------------------
BEGIN;
INSERT INTO `cm_option` (`ID`, `Name`, `Dispidx`, `GroupID`) VALUES (2, '汉族', 2, 1), (3, '蒙古族', 3, 1), (4, '回族', 4, 1), (5, '藏族', 5, 1), (6, '维吾尔族', 6, 1), (7, '苗族', 7, 1), (8, '彝族', 8, 1), (9, '壮族', 9, 1), (10, '布依族', 10, 1), (11, '朝鲜族', 11, 1), (12, '满族', 12, 1), (13, '侗族', 13, 1), (14, '瑶族', 14, 1), (15, '白族', 15, 1), (16, '土家族', 16, 1), (17, '哈尼族', 17, 1), (18, '哈萨克族', 18, 1), (19, '傣族', 19, 1), (20, '黎族', 20, 1), (21, '傈僳族', 21, 1), (22, '佤族', 22, 1), (23, '畲族', 23, 1), (24, '高山族', 24, 1), (25, '拉祜族', 25, 1), (26, '水族', 26, 1), (27, '东乡族', 27, 1), (28, '纳西族', 28, 1), (29, '景颇族', 29, 1), (30, '柯尔克孜族', 30, 1), (31, '土族', 31, 1), (32, '达斡尔族', 32, 1), (33, '仫佬族', 33, 1), (34, '羌族', 34, 1), (35, '布朗族', 35, 1), (36, '撒拉族', 36, 1), (37, '毛难族', 37, 1), (38, '仡佬族', 38, 1), (39, '锡伯族', 39, 1), (40, '阿昌族', 40, 1), (41, '普米族', 41, 1), (42, '塔吉克族', 42, 1), (43, '怒族', 43, 1), (44, '乌孜别克族', 44, 1), (45, '俄罗斯族', 45, 1), (46, '鄂温克族', 46, 1), (47, '德昂族', 47, 1), (48, '保安族', 48, 1), (49, '裕固族', 49, 1), (50, '京族', 50, 1), (51, '塔塔尔族', 51, 1), (52, '独龙族', 52, 1), (53, '鄂伦春族', 53, 1), (54, '赫哲族', 54, 1), (55, '门巴族', 55, 1), (56, '珞巴族', 56, 1), (57, '基诺族', 57, 1), (58, '大学', 58, 2), (59, '高中', 59, 2), (60, '中学', 60, 2), (61, '小学', 61, 2), (62, '硕士', 62, 2), (63, '博士', 63, 2), (64, '其他', 64, 2), (65, '黑龙江杜尔伯特县', 65, 3), (66, '黑龙江富裕县', 66, 3), (67, '黑龙江林甸县', 67, 3), (68, '黑龙江克山县', 68, 3), (69, '黑龙江克东县', 69, 3), (70, '黑龙江省拜泉县', 70, 3), (71, '黑龙江鸡西市', 71, 3), (72, '黑龙江鸡东县', 72, 3), (73, '黑龙江鹤岗市', 73, 3), (74, '黑龙江萝北县', 74, 3), (75, '黑龙江绥滨县', 75, 3), (76, '黑龙江双鸭山市', 76, 3), (77, '黑龙江集贤县', 77, 3), (78, '黑龙江大庆市', 78, 3), (79, '黑龙江伊春市', 79, 3), (80, '黑龙江嘉荫县', 80, 3), (81, '黑龙江佳木斯市', 81, 3), (82, '黑龙江桦南县', 82, 3), (83, '黑龙江依兰县', 83, 3), (84, '黑龙江桦川县', 84, 3), (85, '黑龙江省宝清县', 85, 3), (86, '黑龙江汤原县', 86, 3), (87, '黑龙江饶河县', 87, 3), (88, '黑龙江抚远县', 88, 3), (89, '黑龙江友谊县', 89, 3), (90, '黑龙江七台河市', 90, 3), (91, '黑龙江省勃利县', 91, 3), (92, '黑龙江牡丹江市', 92, 3), (93, '黑龙江宁安县', 93, 3), (94, '黑龙江海林县', 94, 3), (95, '黑龙江穆棱县', 95, 3), (96, '黑龙江东宁县', 96, 3), (97, '黑龙江林口县', 97, 3), (98, '黑龙江虎林县', 98, 3), (99, '黑龙江双城市', 99, 3), (100, '黑龙江尚志市', 100, 3), (101, '黑龙江省宾县', 101, 3), (102, '黑龙江五常县', 102, 3), (103, '黑龙江省巴彦县', 103, 3), (104, '黑龙江木兰县', 104, 3), (105, '黑龙江通河县', 105, 3), (106, '黑龙江方正县', 106, 3), (107, '黑龙江延寿县', 107, 3), (108, '黑龙江绥化市', 108, 3), (109, '黑龙江省安达市', 109, 3), (110, '黑龙江肇东市', 110, 3), (111, '黑龙江海伦县', 111, 3), (112, '黑龙江望奎县', 112, 3), (113, '黑龙江兰西县', 113, 3), (114, '黑龙江青冈县', 114, 3), (115, '黑龙江肇源县', 115, 3), (116, '黑龙江肇州县', 116, 3), (117, '黑龙江庆安县', 117, 3), (118, '黑龙江明水县', 118, 3), (119, '黑龙江绥棱县', 119, 3), (120, '黑龙江黑河市', 120, 3), (121, '黑龙江省北安市', 121, 3), (122, '黑龙江五大连池市', 122, 3), (123, '黑龙江嫩江县', 123, 3), (124, '黑龙江省德都县', 124, 3), (125, '黑龙江逊克县', 125, 3), (126, '黑龙江孙吴县', 126, 3), (127, '黑龙江呼玛县', 127, 3), (128, '黑龙江塔河县', 128, 3), (129, '黑龙江漠河县', 129, 3), (130, '黑龙江绥芬河市', 130, 3), (131, '黑龙江省阿城市', 131, 3), (132, '黑龙江同江市', 132, 3), (133, '黑龙江富锦市', 133, 3), (134, '黑龙江铁力市', 134, 3), (135, '黑龙江密山市', 135, 3), (136, '吉林省长春市', 136, 3), (137, '内蒙古呼和浩特市', 137, 3), (138, '内蒙古土默特左旗', 138, 3), (139, '内蒙古托克托县', 139, 3), (140, '内蒙古包头市', 140, 3), (141, '内蒙古土默特右旗', 141, 3), (142, '内蒙古固阳县', 142, 3), (143, '内蒙古乌海市', 143, 3), (144, '内蒙古赤峰市', 144, 3), (145, '内蒙古阿鲁科尔沁旗', 145, 3), (146, '内蒙古巴林左旗', 146, 3), (147, '内蒙古巴林右旗', 147, 3), (148, '内蒙古林西县', 148, 3), (149, '内蒙古克什克腾旗', 149, 3), (150, '内蒙古翁牛特旗', 150, 3), (151, '内蒙古喀喇沁旗', 151, 3), (152, '内蒙古宁城县', 152, 3), (153, '内蒙古敖汉旗', 153, 3), (154, '内蒙古海拉尔市', 154, 3), (155, '内蒙古满州里市', 155, 3), (156, '内蒙古扎兰屯市', 156, 3), (157, '内蒙古牙克石市', 157, 3), (158, '内蒙古阿荣旗', 158, 3), (159, '内蒙古莫力县', 159, 3), (160, '内蒙古额尔古纳右旗', 160, 3), (161, '内蒙古额尔古纳左旗', 161, 3), (162, '内蒙古鄂伦春自治旗', 162, 3), (163, '内蒙古鄂温克族自治旗', 163, 3), (164, '内蒙古新巴尔虎右旗', 164, 3), (165, '内蒙古新巴尔虎左旗', 165, 3), (166, '内蒙古陈巴尔虎旗', 166, 3), (167, '内蒙古乌兰浩特市', 167, 3), (168, '内蒙古科尔沁右翼前旗', 168, 3), (169, '内蒙古科尔沁右翼中旗', 169, 3), (170, '内蒙古扎赉特旗', 170, 3), (171, '内蒙古突泉县', 171, 3), (172, '内蒙古通辽市', 172, 3), (173, '内蒙古霍林郭勒市', 173, 3), (174, '内蒙古科尔沁左翼中旗', 174, 3), (175, '内蒙古科尔沁左翼后旗', 175, 3), (176, '内蒙古开鲁县', 176, 3), (177, '内蒙古库伦旗', 177, 3), (178, '内蒙古奈曼旗', 178, 3), (179, '内蒙古扎鲁特旗', 179, 3), (180, '内蒙古二连浩特市', 180, 3), (181, '内蒙古锡林浩特市', 181, 3), (182, '内蒙古阿巴嘎旗', 182, 3), (183, '内蒙古苏尼特左旗', 183, 3), (184, '内蒙古苏尼特右旗', 184, 3), (185, '内蒙古东乌珠穆沁旗', 185, 3), (186, '内蒙古西乌珠穆沁旗', 186, 3), (187, '内蒙古太仆寺旗', 187, 3), (188, '内蒙古镶黄旗', 188, 3), (189, '内蒙古正镶白旗', 189, 3), (190, '内蒙古正蓝旗', 190, 3), (191, '内蒙古多伦县', 191, 3), (192, '内蒙古集宁市', 192, 3), (193, '内蒙古武川县', 193, 3), (194, '内蒙古和林格尔县', 194, 3), (195, '内蒙古清水河县', 195, 3), (196, '内蒙古卓资县', 196, 3), (197, '内蒙古化德县', 197, 3), (198, '内蒙古商都县', 198, 3), (199, '内蒙古兴和县', 199, 3), (200, '内蒙古丰镇县', 200, 3), (201, '内蒙古凉城县', 201, 3), (202, '内蒙古察哈尔右翼前旗', 202, 3), (203, '内蒙古察哈尔右翼中旗', 203, 3), (204, '内蒙古察哈尔右翼后旗', 204, 3), (205, '内蒙古达尔罕茂明安联', 205, 3), (206, '内蒙古四子王旗', 206, 3), (207, '内蒙古东胜市', 207, 3), (208, '内蒙古达拉特旗', 208, 3), (209, '内蒙古准格尔旗', 209, 3), (210, '内蒙古鄂托克前旗', 210, 3), (211, '内蒙古鄂托克旗', 211, 3), (212, '内蒙古杭锦旗', 212, 3), (213, '内蒙古乌审旗', 213, 3), (214, '内蒙古伊金霍洛旗', 214, 3), (215, '内蒙古临河市', 215, 3), (216, '内蒙古五原县', 216, 3), (217, '内蒙古磴口县', 217, 3), (218, '内蒙古乌拉特前旗', 218, 3), (219, '内蒙古乌拉特中旗', 219, 3), (220, '内蒙古乌拉特后旗', 220, 3), (221, '内蒙古杭锦后旗', 221, 3), (222, '内蒙古阿拉善左旗', 222, 3), (223, '内蒙古阿拉善右旗', 223, 3), (224, '内蒙古额济纳旗', 224, 3), (225, '辽宁省', 225, 3), (226, '辽宁省沈阳市', 226, 3), (227, '辽宁省新民县', 227, 3), (228, '辽宁省辽中县', 228, 3), (229, '辽宁省大连市', 229, 3), (230, '辽宁省新金县', 230, 3), (231, '辽宁省长海县', 231, 3), (232, '辽宁省庄河县', 232, 3), (233, '辽宁省鞍山市', 233, 3), (234, '辽宁省台安县', 234, 3), (235, '辽宁省抚顺市', 235, 3), (236, '辽宁省抚顺县', 236, 3), (237, '辽宁省新宾县', 237, 3), (238, '辽宁省清原县', 238, 3), (239, '辽宁省本溪市', 239, 3), (240, '辽宁省本溪县', 240, 3), (241, '辽宁省桓仁县', 241, 3), (242, '辽宁省丹东市', 242, 3), (243, '辽宁省凤城县', 243, 3), (244, '辽宁省岫岩县', 244, 3), (245, '辽宁省东沟县', 245, 3), (246, '辽宁省宽甸县', 246, 3), (247, '辽宁省锦州市', 247, 3), (248, '辽宁省绥中县', 248, 3), (249, '辽宁省锦  县', 249, 3), (250, '辽宁省北镇县', 250, 3), (251, '辽宁省黑山县', 251, 3), (252, '辽宁省义  县', 252, 3), (253, '辽宁省营口市', 253, 3), (254, '辽宁省营口县', 254, 3), (255, '辽宁省盖  县', 255, 3), (256, '辽宁省阜新市', 256, 3), (257, '辽宁省阜新县', 257, 3), (258, '辽宁省彰武县', 258, 3), (259, '辽宁省辽阳市', 259, 3), (260, '辽宁省辽阳县', 260, 3), (261, '辽宁省灯塔县', 261, 3), (262, '辽宁省盘锦市', 262, 3), (263, '辽宁省大洼县', 263, 3), (264, '辽宁省盘山县', 264, 3), (265, '辽宁省铁岭市', 265, 3), (266, '辽宁省铁岭县', 266, 3), (267, '辽宁省西丰县', 267, 3), (268, '辽宁省昌图县', 268, 3), (269, '辽宁省康平县', 269, 3), (270, '辽宁省法库县', 270, 3), (271, '辽宁省朝阳市', 271, 3), (272, '辽宁省朝阳县', 272, 3), (273, '辽宁省建平县', 273, 3), (274, '辽宁省凌源县', 274, 3), (275, '辽宁省喀喇沁县', 275, 3), (276, '辽宁省建昌县', 276, 3), (277, '辽宁省直辖行政单位', 277, 3), (278, '辽宁省瓦房店市', 278, 3), (279, '辽宁省海城市', 279, 3), (280, '辽宁省锦西市', 280, 3), (281, '辽宁省兴城市', 281, 3), (282, '辽宁省铁法市', 282, 3), (283, '辽宁省北票市', 283, 3), (284, '辽宁省开原市', 284, 3), (285, '吉林省', 285, 3), (286, '吉林省榆树县', 286, 3), (287, '吉林省农安县', 287, 3), (288, '吉林省德惠县', 288, 3), (289, '吉林省双阳县', 289, 3), (290, '吉林省吉林市', 290, 3), (291, '吉林省永吉县', 291, 3), (292, '吉林省舒兰县', 292, 3), (293, '吉林省磐石县', 293, 3), (294, '吉林省蛟河县', 294, 3), (295, '吉林省四平市', 295, 3), (296, '吉林省梨树县', 296, 3), (297, '吉林省伊通县', 297, 3), (298, '吉林省双辽县', 298, 3), (299, '吉林省辽源市', 299, 3), (300, '吉林省东丰县', 300, 3), (301, '吉林省东辽县', 301, 3), (302, '吉林省通化市', 302, 3), (303, '吉林省通化县', 303, 3), (304, '吉林省辉南县', 304, 3), (305, '吉林省柳河县', 305, 3), (306, '吉林省浑江市', 306, 3), (307, '吉林省抚松县', 307, 3), (308, '吉林省靖宇县', 308, 3), (309, '吉林省长白县', 309, 3), (310, '吉林省白城地区', 310, 3), (311, '吉林省白城市', 311, 3), (312, '吉林省洮南市', 312, 3), (313, '吉林省扶余市', 313, 3), (314, '吉林省大安市', 314, 3), (315, '吉林省长岭县', 315, 3), (316, '吉林省前郭尔罗斯县', 316, 3), (317, '吉林省镇赉县', 317, 3), (318, '吉林省通榆县', 318, 3), (319, '吉林省乾安县', 319, 3), (320, '吉林省延吉市', 320, 3), (321, '吉林省图们市', 321, 3), (322, '吉林省敦化市', 322, 3), (323, '吉林省珲春市', 323, 3), (324, '吉林省龙井市', 324, 3), (325, '吉林省和龙县', 325, 3), (326, '吉林省汪清县', 326, 3), (327, '吉林省安图县', 327, 3), (328, '吉林省公主岭市', 328, 3), (329, '吉林省梅河口市', 329, 3), (330, '吉林省集安市', 330, 3), (331, '吉林省桦甸市', 331, 3), (332, '吉林省九台市', 332, 3), (333, '黑龙江省', 333, 3), (334, '黑龙江哈尔滨市', 334, 3), (335, '黑龙江呼兰县', 335, 3), (336, '黑龙江齐齐哈尔市', 336, 3), (337, '黑龙江龙江县', 337, 3), (338, '黑龙江讷河县', 338, 3), (339, '黑龙江依安县', 339, 3), (340, '黑龙江泰来县', 340, 3), (341, '黑龙江甘南县', 341, 3), (342, '男', 342, 4), (343, '女', 343, 4), (344, '未知', 344, 4), (345, '不明', 345, 4), (346, 'string', 346, 5), (347, 'int', 347, 5), (348, 'double', 348, 5), (349, 'DateTime', 349, 5), (350, 'Date', 350, 5), (351, 'bool', 351, 5);
COMMIT;

-- ----------------------------
-- Table structure for cm_option_group
-- ----------------------------
DROP TABLE IF EXISTS `cm_option_group`;
CREATE TABLE `cm_option_group`  (
  `ID` bigint(20) NOT NULL COMMENT '标识',
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '分组名称',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '基础选项分组';

-- ----------------------------
-- Records of cm_option_group
-- ----------------------------
BEGIN;
INSERT INTO `cm_option_group` (`ID`, `Name`) VALUES (1, '民族'), (2, '学历'), (3, '地区'), (4, '性别'), (5, '数据类型');
COMMIT;

-- ----------------------------
-- Table structure for cm_params
-- ----------------------------
DROP TABLE IF EXISTS `cm_params`;
CREATE TABLE `cm_params`  (
  `ID` bigint(20) NOT NULL COMMENT '用户参数标识',
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '参数名称',
  `Value` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '参数缺省值',
  `Note` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '参数描述',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `Name`(`Name`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '用户参数定义';

-- ----------------------------
-- Records of cm_params
-- ----------------------------
BEGIN;
INSERT INTO `cm_params` (`ID`, `Name`, `Value`, `Note`, `Ctime`, `Mtime`) VALUES (1, '接收新任务', 'true', '', '2020-12-01 15:13:49', '2020-12-02 09:23:53'), (2, '接收新发布通知', 'true', '', '2020-12-02 09:25:15', '2020-12-02 09:25:15'), (3, '接收新消息', 'true', '接收通讯录消息推送', '2020-12-02 09:24:28', '2020-12-02 09:24:28');
COMMIT;

-- ----------------------------
-- Table structure for cm_permission
-- ----------------------------
DROP TABLE IF EXISTS `cm_permission`;
CREATE TABLE `cm_permission`  (
  `ID` bigint(20) NOT NULL COMMENT '权限标识',
  `Name` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '权限名称',
  `Note` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '权限描述',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `idx_permission_name`(`Name`) USING BTREE COMMENT '不重复'
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '权限';

-- ----------------------------
-- Records of cm_permission
-- ----------------------------
BEGIN;
INSERT INTO `cm_permission` (`ID`, `Name`, `Note`) VALUES (1, '公共文件管理', '禁止删除'), (2, '素材库管理', '禁止删除');
COMMIT;

-- ----------------------------
-- Table structure for cm_role
-- ----------------------------
DROP TABLE IF EXISTS `cm_role`;
CREATE TABLE `cm_role`  (
  `ID` bigint(20) NOT NULL COMMENT '角色标识',
  `Name` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '角色名称',
  `Note` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '角色描述',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `idx_role_name`(`Name`) USING BTREE COMMENT '不重复'
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '角色';

-- ----------------------------
-- Records of cm_role
-- ----------------------------
BEGIN;
INSERT INTO `cm_role` (`ID`, `Name`, `Note`) VALUES (1, '任何人', '所有用户默认都具有该角色，不可删除'), (2, '系统管理员', '系统角色，不可删除');
COMMIT;

-- ----------------------------
-- Table structure for cm_role_menu
-- ----------------------------
DROP TABLE IF EXISTS `cm_role_menu`;
CREATE TABLE `cm_role_menu`  (
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  `MenuID` bigint(20) NOT NULL COMMENT '菜单标识',
  PRIMARY KEY (`RoleID`, `MenuID`) USING BTREE,
  INDEX `fk_rolemenu_menuid`(`MenuID`) USING BTREE,
  INDEX `fk_rolemenu_roleid`(`RoleID`) USING BTREE,
  CONSTRAINT `fk_rolemenu_menuid` FOREIGN KEY (`MenuID`) REFERENCES `cm_menu` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_rolemenu_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '角色一菜单多对多';

-- ----------------------------
-- Records of cm_role_menu
-- ----------------------------
BEGIN;
INSERT INTO `cm_role_menu` (`RoleID`, `MenuID`) VALUES (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (1, 7), (1, 8), (1, 9), (2, 10);
COMMIT;

-- ----------------------------
-- Table structure for cm_role_per
-- ----------------------------
DROP TABLE IF EXISTS `cm_role_per`;
CREATE TABLE `cm_role_per`  (
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  `PerID` bigint(20) NOT NULL COMMENT '权限标识',
  PRIMARY KEY (`RoleID`, `PerID`) USING BTREE,
  INDEX `fk_roleper_perid`(`PerID`) USING BTREE,
  INDEX `fk_roleper_roleid`(`RoleID`) USING BTREE,
  CONSTRAINT `fk_roleper_perid` FOREIGN KEY (`PerID`) REFERENCES `cm_permission` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_roleper_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '角色一权限多对多';

-- ----------------------------
-- Records of cm_role_per
-- ----------------------------
BEGIN;
INSERT INTO `cm_role_per` (`RoleID`, `PerID`) VALUES (1, 1), (1, 2);
COMMIT;

-- ----------------------------
-- Table structure for cm_rpt
-- ----------------------------
DROP TABLE IF EXISTS `cm_rpt`;
CREATE TABLE `cm_rpt`  (
  `ID` bigint(20) NOT NULL COMMENT '报表标识',
  `Name` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '报表名称',
  `Define` varchar(21000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '报表模板定义',
  `Note` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '报表描述',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `idx_rpt_name`(`Name`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '报表模板定义';


-- ----------------------------
-- Table structure for cm_user
-- ----------------------------
DROP TABLE IF EXISTS `cm_user`;
CREATE TABLE `cm_user`  (
  `ID` bigint(20) NOT NULL COMMENT '用户标识',
  `Phone` char(11) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '手机号，唯一',
  `Name` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '姓名',
  `Pwd` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '密码的md5',
  `Sex` tinyint(4) UNSIGNED NOT NULL DEFAULT 1 COMMENT '#Gender#性别',
  `Photo` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '头像',
  `Expired` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否停用',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `idx_phone`(`Phone`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '系统用户';

-- ----------------------------
-- Records of cm_user
-- ----------------------------
BEGIN;
INSERT INTO `cm_user` (`ID`, `Phone`, `Name`, `Pwd`, `Sex`, `Photo`, `Expired`, `Ctime`, `Mtime`) VALUES (1, '13511111111', 'Windows', 'af3303f852abeccd793068486a391626', 1, '[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]', 0, '2019-10-24 09:06:38', '2023-03-16 08:35:39'), (2, '13522222222', '安卓', 'b59c67bf196a4758191e42f76670ceba', 2, '[[\"photo/2.jpg\",\"2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]', 0, '2019-10-24 13:03:19', '2023-03-16 08:36:23'), (3, '13533333333', '苹果', '674f3c2c1a8a6f90461e8a66fb5550ba', 1, '[[\"photo/3.jpg\",\"3\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]', 0, '0001-01-01 00:00:00', '2023-03-16 08:36:46');
COMMIT;

-- ----------------------------
-- Table structure for cm_user_group
-- ----------------------------
DROP TABLE IF EXISTS `cm_user_group`;
CREATE TABLE `cm_user_group`  (
  `UserID` bigint(20) NOT NULL COMMENT '用户标识',
  `GroupID` bigint(20) NOT NULL COMMENT '组标识',
  PRIMARY KEY (`UserID`, `GroupID`) USING BTREE,
  INDEX `fk_usergroup_groupid`(`GroupID`) USING BTREE,
  INDEX `fk_usergroup_userid`(`UserID`) USING BTREE,
  CONSTRAINT `fk_usergroup_groupid` FOREIGN KEY (`GroupID`) REFERENCES `cm_group` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_usergroup_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '用户一组多对多';


-- ----------------------------
-- Table structure for cm_user_params
-- ----------------------------
DROP TABLE IF EXISTS `cm_user_params`;
CREATE TABLE `cm_user_params`  (
  `UserID` bigint(20) NOT NULL COMMENT '用户标识',
  `ParamID` bigint(20) NOT NULL COMMENT '参数标识',
  `Value` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '参数值',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`UserID`, `ParamID`) USING BTREE,
  INDEX `fk_userparams_userid`(`UserID`) USING BTREE,
  INDEX `fk_userparams_paramsid`(`ParamID`) USING BTREE,
  CONSTRAINT `fk_userparams_paramsid` FOREIGN KEY (`ParamID`) REFERENCES `cm_params` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_userparams_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '用户参数值';


-- ----------------------------
-- Table structure for cm_user_role
-- ----------------------------
DROP TABLE IF EXISTS `cm_user_role`;
CREATE TABLE `cm_user_role`  (
  `UserID` bigint(20) NOT NULL COMMENT '用户标识',
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`UserID`, `RoleID`) USING BTREE,
  INDEX `fk_userrole_userid`(`UserID`) USING BTREE,
  INDEX `fk_userrole_roleid`(`RoleID`) USING BTREE,
  CONSTRAINT `fk_userrole_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_userrole_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '用户一角色多对多';

-- ----------------------------
-- Records of cm_user_role
-- ----------------------------
BEGIN;
INSERT INTO `cm_user_role` (`UserID`, `RoleID`) VALUES (1, 2), (2, 2),(3, 2);
COMMIT;

-- ----------------------------
-- Table structure for cm_wfd_atv
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfd_atv`;
CREATE TABLE `cm_wfd_atv`  (
  `ID` bigint(20) NOT NULL COMMENT '活动标识',
  `PrcID` bigint(20) NOT NULL COMMENT '流程标识',
  `Name` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '活动名称，同时作为状态名称',
  `Type` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfdAtvType#活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动',
  `ExecScope` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfdAtvExecScope#执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户',
  `ExecLimit` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfdAtvExecLimit#执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者',
  `ExecAtvID` bigint(20) NULL DEFAULT NULL COMMENT '在执行者限制为3或4时选择的活动',
  `AutoAccept` tinyint(1) NOT NULL COMMENT '是否自动签收，打开工作流视图时自动签收工作项',
  `CanDelete` tinyint(1) NOT NULL COMMENT '能否删除流程实例和业务数据，0否 1',
  `CanTerminate` tinyint(1) NOT NULL COMMENT '能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能',
  `CanJumpInto` tinyint(1) NOT NULL COMMENT '是否可作为跳转目标，0不可跳转 1可以',
  `TransKind` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfdAtvTransKind#当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择',
  `JoinKind` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfdAtvJoinKind#同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fk_wfdatv_prcid`(`PrcID`) USING BTREE,
  CONSTRAINT `fk_wfdatv_prcid` FOREIGN KEY (`PrcID`) REFERENCES `cm_wfd_prc` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '活动模板';


-- ----------------------------
-- Table structure for cm_wfd_atv_role
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfd_atv_role`;
CREATE TABLE `cm_wfd_atv_role`  (
  `AtvID` bigint(20) NOT NULL COMMENT '活动标识',
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`AtvID`, `RoleID`) USING BTREE,
  INDEX `fk_wfdatvrole_roleid`(`RoleID`) USING BTREE,
  CONSTRAINT `fk_wfdatvrole_atvid` FOREIGN KEY (`AtvID`) REFERENCES `cm_wfd_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfdatvrole_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '活动授权';


-- ----------------------------
-- Table structure for cm_wfd_prc
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfd_prc`;
CREATE TABLE `cm_wfd_prc`  (
  `ID` bigint(20) NOT NULL COMMENT '流程标识',
  `Name` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '流程名称',
  `Diagram` varchar(21000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程图',
  `IsLocked` tinyint(1) NOT NULL COMMENT '锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行',
  `Singleton` tinyint(1) NOT NULL COMMENT '同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例',
  `Note` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '描述',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后修改时间',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程模板';


-- ----------------------------
-- Table structure for cm_wfd_trs
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfd_trs`;
CREATE TABLE `cm_wfd_trs`  (
  `ID` bigint(20) NOT NULL COMMENT '迁移标识',
  `PrcID` bigint(20) NOT NULL COMMENT '流程模板标识',
  `SrcAtvID` bigint(20) NOT NULL COMMENT '起始活动模板标识',
  `TgtAtvID` bigint(20) NOT NULL COMMENT '目标活动模板标识',
  `IsRollback` tinyint(1) NOT NULL COMMENT '是否为回退迁移',
  `TrsID` bigint(20) NULL DEFAULT NULL COMMENT '类别为回退迁移时对应的常规迁移标识',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fk_wfdtrs_prcid`(`PrcID`) USING BTREE,
  CONSTRAINT `fk_wfdtrs_prcid` FOREIGN KEY (`PrcID`) REFERENCES `cm_wfd_prc` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '迁移模板';


-- ----------------------------
-- Table structure for cm_wfi_atv
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfi_atv`;
CREATE TABLE `cm_wfi_atv`  (
  `ID` bigint(20) NOT NULL COMMENT '活动实例标识',
  `PrciID` bigint(20) NOT NULL COMMENT '流程实例标识',
  `AtvdID` bigint(20) NOT NULL COMMENT '活动模板标识',
  `Status` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfiAtvStatus#活动实例的状态 0活动 1结束 2终止 3同步活动',
  `InstCount` int(11) NOT NULL COMMENT '活动实例在流程实例被实例化的次数',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fk_wfiatv_prciid`(`PrciID`) USING BTREE,
  INDEX `fk_wfiatv_atvdid`(`AtvdID`) USING BTREE,
  CONSTRAINT `fk_wfiatv_atvdid` FOREIGN KEY (`AtvdID`) REFERENCES `cm_wfd_atv` (`ID`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_wfiatv_prciid` FOREIGN KEY (`PrciID`) REFERENCES `cm_wfi_prc` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '活动实例';


-- ----------------------------
-- Table structure for cm_wfi_item
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfi_item`;
CREATE TABLE `cm_wfi_item`  (
  `ID` bigint(20) NOT NULL COMMENT '工作项标识',
  `AtviID` bigint(20) NOT NULL COMMENT '活动实例标识',
  `Status` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfiItemStatus#工作项状态 0活动 1结束 2终止 3同步活动',
  `AssignKind` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfiItemAssignKind#指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派',
  `Sender` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '发送者',
  `Stime` datetime NOT NULL COMMENT '发送时间',
  `IsAccept` tinyint(1) NOT NULL COMMENT '是否签收此项任务',
  `AcceptTime` datetime NULL DEFAULT NULL COMMENT '签收时间',
  `RoleID` bigint(20) NULL DEFAULT NULL COMMENT '执行者角色标识',
  `UserID` bigint(20) NULL DEFAULT NULL COMMENT '执行者用户标识',
  `Note` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '工作项备注',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fk_wfiitem_atviid`(`AtviID`) USING BTREE,
  CONSTRAINT `fk_wfiitem_atviid` FOREIGN KEY (`AtviID`) REFERENCES `cm_wfi_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '工作项';


-- ----------------------------
-- Table structure for cm_wfi_prc
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfi_prc`;
CREATE TABLE `cm_wfi_prc`  (
  `ID` bigint(20) NOT NULL COMMENT '流程实例标识，同时为业务数据主键',
  `PrcdID` bigint(20) NOT NULL COMMENT '流程模板标识',
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '流转单名称',
  `Status` tinyint(4) UNSIGNED NOT NULL COMMENT '#WfiPrcStatus#流程实例状态 0活动 1结束 2终止',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fk_wfiprc_prcdid`(`PrcdID`) USING BTREE,
  CONSTRAINT `fk_wfiprc_prcdid` FOREIGN KEY (`PrcdID`) REFERENCES `cm_wfd_prc` (`ID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程实例';


-- ----------------------------
-- Table structure for cm_wfi_trs
-- ----------------------------
DROP TABLE IF EXISTS `cm_wfi_trs`;
CREATE TABLE `cm_wfi_trs`  (
  `ID` bigint(20) NOT NULL COMMENT '迁移实例标识',
  `TrsdID` bigint(20) NOT NULL COMMENT '迁移模板标识',
  `SrcAtviID` bigint(20) NOT NULL COMMENT '起始活动实例标识',
  `TgtAtviID` bigint(20) NOT NULL COMMENT '目标活动实例标识',
  `IsRollback` tinyint(1) NOT NULL COMMENT '是否为回退迁移，1表回退',
  `Ctime` datetime NOT NULL COMMENT '迁移时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `fk_wfitrs_trsdid`(`TrsdID`) USING BTREE,
  INDEX `fk_wfitrs_srcatviid`(`SrcAtviID`) USING BTREE,
  INDEX `fk_wfitrs_tgtatviid`(`TgtAtviID`) USING BTREE,
  CONSTRAINT `fk_wfitrs_srcatviid` FOREIGN KEY (`SrcAtviID`) REFERENCES `cm_wfi_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfitrs_tgtatviid` FOREIGN KEY (`TgtAtviID`) REFERENCES `cm_wfi_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfitrs_trsdid` FOREIGN KEY (`TrsdID`) REFERENCES `cm_wfd_trs` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '迁移实例';


-- ----------------------------
-- Table structure for fsm_file
-- ----------------------------
DROP TABLE IF EXISTS `fsm_file`;
CREATE TABLE `fsm_file`  (
  `ID` bigint(20) UNSIGNED NOT NULL COMMENT '文件标识',
  `Name` varchar(512) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '文件名称',
  `Path` varchar(512) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '存放路径：卷/两级目录/id.ext',
  `Size` bigint(20) UNSIGNED NOT NULL COMMENT '文件长度',
  `Info` varchar(512) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '文件描述',
  `Uploader` bigint(20) UNSIGNED NOT NULL COMMENT '上传人id',
  `Ctime` datetime NOT NULL COMMENT '上传时间',
  `Downloads` bigint(20) UNSIGNED NOT NULL COMMENT '下载次数',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `idx_fsm_file_path`(`Path`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci;


-- ----------------------------
-- Table structure for sequence
-- ----------------------------
DROP TABLE IF EXISTS `sequence`;
CREATE TABLE `sequence`  (
  `id` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '序列名称',
  `val` int(11) NOT NULL COMMENT '序列的当前值',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '模拟Sequence';

-- ----------------------------
-- Records of sequence
-- ----------------------------
BEGIN;
INSERT INTO `sequence` (`id`, `val`) VALUES ('cm_menu+dispidx', 89), ('cm_option+dispidx', 1031), ('cm_pub_post+dispidx', 167), ('cm_wfd_prc+dispidx', 11), ('cm_wfi_item+dispidx', 176), ('cm_wfi_prc+dispidx', 65), ('demo_crud+dispidx', 84), ('demo_基础+序列', 11);
COMMIT;

-- ----------------------------
-- Procedure structure for cm_参数_用户参数列表
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_参数_用户参数列表`;
delimiter ;;
CREATE PROCEDURE `cm_参数_用户参数列表`(`p_userid` bigint)
BEGIN

select paramid,value from cm_user_params where userid =p_userid
	union
select id,value from cm_params a  where
	not exists ( select paramid from cm_user_params b where a.id = b.paramid and userid = p_userid );
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_参数_用户参数值ByID
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_参数_用户参数值ByID`;
delimiter ;;
CREATE PROCEDURE `cm_参数_用户参数值ByID`(`p_userid` bigint,`p_paramid` bigint)
BEGIN
	
select value from cm_user_params where userid = p_userid and paramid = p_paramid
union
select value from cm_params a  where id = p_paramid;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_参数_用户参数值ByName
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_参数_用户参数值ByName`;
delimiter ;;
CREATE PROCEDURE `cm_参数_用户参数值ByName`(`p_userid` bigint,`p_name` varchar(200))
BEGIN
	
select a.value from cm_user_params a, cm_params b where a.paramid=b.id and a.userid = p_userid and b.name = p_name
union
select value from cm_params a  where name = p_name;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_分组_分组列表的用户
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_分组_分组列表的用户`;
delimiter ;;
CREATE PROCEDURE `cm_分组_分组列表的用户`(`p_groupid` VARCHAR(4000))
BEGIN
	
select distinct(userid) from cm_user_group where find_in_set(groupid, p_groupid);
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_编辑活动模板
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_编辑活动模板`;
delimiter ;;
CREATE PROCEDURE `cm_流程_编辑活动模板`(`p_prcid` bigint)
BEGIN

select
	a.*,
	( CASE execscope WHEN 0 THEN '一组用户' WHEN 1 THEN '所有用户' WHEN 2 THEN '单个用户' WHEN 3 THEN '任一用户' END ) execscope_dsp,
	( CASE execlimit WHEN 0 THEN '无限制' WHEN 1 THEN '前一活动的执行者' WHEN 2 THEN '前一活动的同部门执行者' WHEN 3 THEN '已完成活动的执行者' WHEN 4 THEN '已完成活动的同部门执行者' END ) execlimit_dsp,
	( CASE JOINKIND WHEN 0 THEN '全部任务' WHEN 1 THEN '任一任务' WHEN 2 THEN '即时同步' END ) joinkind_dsp,
	( CASE transkind WHEN 0 THEN '自由选择' WHEN 1 THEN '全部' WHEN 2 THEN '独占式选择' END ) transkind_dsp,
	( select name from cm_wfd_atv where id = a.execatvid ) as execatvid_dsp 
from
	cm_wfd_atv a 
where
	prcid = p_prcid;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_参与的流程
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_参与的流程`;
delimiter ;;
CREATE PROCEDURE `cm_流程_参与的流程`(p_userid bigint)
BEGIN

select distinct
	p.id,
	p.name
from
	cm_wfd_prc p,
	cm_wfd_atv a,
	cm_wfd_atv_role r,
	cm_user_role u 
where
	p.id = a.prcid 
	and a.id = r.atvid 
	and ( r.roleid = u.roleid or r.roleid = 1 ) 
	and u.userid = p_userid
order by
	p.dispidx;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_查找实例
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_查找实例`;
delimiter ;;
CREATE PROCEDURE `cm_流程_查找实例`(p_prcdid bigint, p_start datetime, p_end datetime, p_status int, p_title VARCHAR(200))
BEGIN

select
	id,
	PrcdID,
	name,
	Status,
	Ctime,
	Mtime 
from
	cm_wfi_prc 
where
	PrcdID = p_prcdid 
	and ( p_status > 2 or `Status` = p_status ) 
	and ( p_title = '' or name = p_title ) 
	and ( p_start < '1900-01-01' or Mtime >= p_start ) 
	and ( p_end < '1900-01-01' or Mtime <= p_end ) 
order by
	dispidx;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_待办任务
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_待办任务`;
delimiter ;;
CREATE PROCEDURE `cm_流程_待办任务`(p_userid bigint)
BEGIN

select wi.id   itemid,
		 pi.id     prciid,
		 pd.id     prcdid,
		 pd.name   prcname,
		 ad.name   atvname,
		 pi.name   formname,
		 wi.AssignKind,
		 wi.sender,
		 wi.stime,
		 wi.IsAccept
from cm_wfi_atv ai,
		 cm_wfd_atv ad,
		 cm_wfi_prc pi,
		 cm_wfd_prc pd,
		 (select id,
						 atviid,
						 sender,
						 stime,
						 IsAccept,
						 AssignKind
				from cm_wfi_item wi
			 where status = 0
				 and (userid = p_userid or
						 (userid is null and
						 (exists (select 1
													from cm_user_role
												 where wi.roleid = roleid
													 and userid = p_userid)) or
						 roleid = 1))) wi
where ai.id = wi.atviid
 and ai.atvdid = ad.id
 and ai.prciid = pi.id
 and pi.prcdid = pd.id
order by wi.stime desc;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_待办任务总数
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_待办任务总数`;
delimiter ;;
CREATE PROCEDURE `cm_流程_待办任务总数`(`p_userid` bigint)
BEGIN
	
select
	sum( 1 ) allTask 
from
	cm_wfi_prc a,
	cm_wfi_atv b,
	cm_wfi_item c 
where
	a.id = b.prciid 
	and b.id = c.atviid 
	and c.status = 0 
	and 
	(
		c.userid = p_userid 
		or ( userid is null and exists ( select 1 from cm_user_role where c.roleid = roleid and userid = p_userid ) ) 
	);
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_后续活动
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_后续活动`;
delimiter ;;
CREATE PROCEDURE `cm_流程_后续活动`(p_atvid bigint)
BEGIN

select
	atv.* 
from
	cm_wfd_atv atv,
	( select trs.TgtAtvID atvid from cm_wfd_trs trs where trs.SrcAtvID = p_atvid and IsRollback = 0 ) trs 
where
	atv.id = trs.atvid;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_后续活动工作项
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_后续活动工作项`;
delimiter ;;
CREATE PROCEDURE `cm_流程_后续活动工作项`(p_prciid bigint, p_atvdid bigint)
BEGIN

select
	a.IsAccept,
	a.Status,
	b.id atviid 
from
	cm_wfi_item a,
	cm_wfi_atv b 
where
	a.atviid = b.id 
	and b.atvdid in ( select TgtAtvID from cm_wfd_trs d where d.SrcAtvID = p_atvdid and d.IsRollback = 0 ) 
	and b.prciid = p_prciid;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_活动实例的工作项
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_活动实例的工作项`;
delimiter ;;
CREATE PROCEDURE `cm_流程_活动实例的工作项`(`p_atviid` bigint)
BEGIN

select
	status,
	AssignKind,
	concat( sender, ' -> ', usr.name ) sendprc,
	IsAccept,
	wi.mtime 
from
	cm_wfi_item wi
	left join cm_user usr on wi.userid = usr.id 
where
	atviid = p_atviid
order by
	dispidx;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_可启动流程
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_可启动流程`;
delimiter ;;
CREATE PROCEDURE `cm_流程_可启动流程`(p_userid bigint)
BEGIN

select
	pd.id,
	name 
from
	cm_wfd_prc pd,
	(
select distinct
	p.id 
from
	cm_wfd_prc p,
	cm_wfd_atv a,
	cm_wfd_atv_role r,
	cm_user_role u 
where
	p.id = a.prcid 
	and a.id = r.atvid 
	and ( r.roleid = u.roleid or r.roleid = 1 ) 
	and u.userid = p_userid
	and p.islocked = 0 
	and a.type = 1 
	) pa 
where
	pd.id = pa.id 
order by
	dispidx;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_历史任务
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_历史任务`;
delimiter ;;
CREATE PROCEDURE `cm_流程_历史任务`(p_userid bigint, p_start datetime, p_end datetime, p_status int)
BEGIN

select wi.id itemid,
			 pi.id prciid,
			 pd.id prcdid,
			 ad.id atvdid,
			 ai.id atviid,
			 pd.name prcname,
			 ( CASE pi.status WHEN 1 THEN '已结束' WHEN 2 THEN '已终止' ELSE ad.name END ) as atvname,
			 pi.status,
			 pi.name formname,
			 wi.sender,
			 wi.stime,
			 max(wi.mtime) mtime,
			 wi.reCount
	from cm_wfi_atv ai,
			 cm_wfi_prc pi,
			 cm_wfd_atv ad,
			 cm_wfd_prc pd,
			 (select id,
							 atviid,
							 mtime,
							 sender,
							 stime,
							 (select count(1)
									from cm_wfi_item
								 where atviid = t.atviid
									 and AssignKind = 4
									 and id <> t.id) as reCount
					from cm_wfi_item t
				 where status = 1
					 and userid = p_userid
					 and (p_start < '1900-01-01' or mtime >= p_start)
					 and (p_end < '1900-01-01' or mtime <= p_end)
					 order by mtime desc) wi
 where wi.atviid = ai.id
	 and ai.prciid = pi.id
	 and pi.prcdid = pd.id
	 and ai.atvdid = ad.id
	 and wi.reCount = 0
	 and (p_status > 2 or pi.status = p_status)
 group by prciid
 order by wi.stime desc;
 
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_前一活动的同部门执行者
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_前一活动的同部门执行者`;
delimiter ;;
CREATE PROCEDURE `cm_流程_前一活动的同部门执行者`(`p_prciid` bigint,`p_atvdid` bigint)
BEGIN

select distinct userid from cm_xemp
where depid in (
  select distinct depid from cm_xemp
	where userid in (
    select userid from cm_wfi_item
		where atviid in (
      select ID from cm_wfi_atv
			where prciid = p_prciid
	          and atvdid in ( select SrcAtvID from cm_wfd_trs where TgtAtvID = p_atvdid ))));
						
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_前一活动执行者
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_前一活动执行者`;
delimiter ;;
CREATE PROCEDURE `cm_流程_前一活动执行者`(`p_prciid` bigint,`p_atvdid` bigint)
BEGIN

select distinct userid from cm_wfi_item
where atviid in (
  select id from cm_wfi_atv
	where prciid = p_prciid
		and atvdid in (select SrcAtvID from cm_wfd_trs where TgtAtvID=p_atvdid));
		
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_日志目标项
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_日志目标项`;
delimiter ;;
CREATE PROCEDURE `cm_流程_日志目标项`(p_prciid bigint, p_atviid bigint)
BEGIN

select ( CASE username WHEN NULL THEN rolename ELSE username END ) accpname,
			 atvdname,
			 atvdtype,
			 joinkind,
			 atviid
	from (select a.atviid,
							 (select group_concat(name order by a.dispidx separator '、') from cm_user where id = a.userid) as username,
							 (select group_concat(name order by a.dispidx separator '、') from cm_role where id = a.roleid) as rolename,
							 max(a.dispidx) dispidx,
							 c.name as atvdname,
							 c.type as atvdtype,
							 c.joinkind
					from cm_wfi_item a,
							 (select ti.TgtAtviID id
									from cm_wfi_atv ai, cm_wfi_trs ti
								 where ai.id = ti.SrcAtviID
									 and ai.prciid = p_prciid
									 and ti.SrcAtviID = p_atviid) b,
							 cm_wfd_atv c,
							 cm_wfi_atv d
				 where a.atviid = b.id
					 and b.id = d.id
					 and d.atvdid = c.id
				 group by a.atviid, c.name, c.type, c.joinkind) t
 order by dispidx;
 
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_生成日志列表
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_生成日志列表`;
delimiter ;;
CREATE PROCEDURE `cm_流程_生成日志列表`(p_prciid bigint, p_atvdid bigint)
BEGIN

select b.prciid,
			 b.id atviid,
			 c.status prcistatus,
			 d.name atvdname,
			 a.AssignKind,
			 a.IsAccept,
			 a.AcceptTime,
			 a.status itemstatus,
			 ( CASE userid WHEN NULL THEN (select name from cm_role t where t.id = a.roleid) ELSE (select name from cm_user t where t.id = a.userid) END ) username,
			 a.note,
			 a.ctime,
			 a.mtime,
			 c.mtime prcitime,
			 a.sender
from cm_wfi_item a, cm_wfi_atv b, cm_wfi_prc c, cm_wfd_atv d
where a.atviid = b.id
	 and b.prciid = c.id
	 and b.atvdid = d.id
	 and b.prciid = p_prciid
	 and (p_atvdid = 0 or b.atvdid = p_atvdid)
order by a.dispidx;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_所有经办历史任务
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_所有经办历史任务`;
delimiter ;;
CREATE PROCEDURE `cm_流程_所有经办历史任务`(p_userid bigint, p_start datetime, p_end datetime, p_status int)
BEGIN

select wi.id itemid,
			 pi.id prciid,
			 pd.id prcdid,
			 ad.id atvdid,
			 ai.id atviid,
			 pd.name prcname,
			 ad.name atvname,
			 pi.status,
			 pi.name formname,
			 wi.sender,
			 wi.stime,
			 wi.mtime,
			 wi.reCount
	from cm_wfi_atv ai,
			 cm_wfi_prc pi,
			 cm_wfd_atv ad,
			 cm_wfd_prc pd,
			 (select id,
							 atviid,
							 mtime,
							 sender,
							 stime,
							 (select count(1)
									from cm_wfi_item
								 where atviid = t.atviid
									 and AssignKind = 4
									 and id <> t.id) as reCount
					from cm_wfi_item t
				 where status = 1
					 and userid = p_userid
					 and (p_start < '1900-01-01' or mtime >= p_start)
					 and (p_end < '1900-01-01' or mtime <= p_end)) wi
	where wi.atviid = ai.id
	 and ai.prciid = pi.id
	 and pi.prcdid = pd.id
	 and ai.atvdid = ad.id
	 and (p_status > 2 or pi.status = p_status)
	order by wi.stime desc;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_已完成活动同部门执行者
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_已完成活动同部门执行者`;
delimiter ;;
CREATE PROCEDURE `cm_流程_已完成活动同部门执行者`(`p_prciid` bigint,`p_atvdid` bigint)
BEGIN

select distinct userid from cm_xemp
where
	depid in (
  select distinct depid from cm_xemp
  where
	userid in ( select userid from cm_wfi_item where atviid in ( select id from cm_wfi_atv where prciid = p_prciid and atvdid = p_atvdid ) ) 
	);
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_已完成活动执行者
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_已完成活动执行者`;
delimiter ;;
CREATE PROCEDURE `cm_流程_已完成活动执行者`(`p_prciid` bigint,`p_atvdid` bigint)
BEGIN

select distinct
	userid 
from
	cm_wfi_item 
where
	atviid in ( select id from cm_wfi_atv where prciid = p_prciid and atvdid = p_atvdid );
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_流程_最后工作项
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_流程_最后工作项`;
delimiter ;;
CREATE PROCEDURE `cm_流程_最后工作项`(p_prciid bigint)
BEGIN

select
	wi.id itemid,
	pi.PrcdID prcid 
from
	cm_wfi_item wi,
	cm_wfi_atv wa,
	cm_wfi_prc pi 
where
	wi.atviid = wa.id 
	and wa.PrciID = pi.id 
	and pi.id = p_prciid
order by
	wi.mtime desc 
	LIMIT 0,
	1;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_文件_搜索扩展名文件
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_文件_搜索扩展名文件`;
delimiter ;;
CREATE PROCEDURE `cm_文件_搜索扩展名文件`(`p_name` varchar(200),`p_userid` bigint,`p_extname` varchar(200))
BEGIN
	
select info from cm_file_pub
where
	isfolder = 0 
	and locate( extname, p_extname ) 
	and name like p_name
union
select info from cm_file_my
where
	isfolder = 0 
	and locate( extname, p_extname ) 
	and userid = p_userid 
	and name like p_name 
	limit 20;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_文件_搜索所有文件
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_文件_搜索所有文件`;
delimiter ;;
CREATE PROCEDURE `cm_文件_搜索所有文件`(`p_name` varchar(200),`p_userid` bigint)
BEGIN
	
select info from cm_file_pub where isfolder=0 and `name` like p_name
union
select info from cm_file_my
 where
	isfolder = 0 
	and userid = p_userid
	and `name` like p_name 
	limit 20;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_用户_角色列表的用户
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_用户_角色列表的用户`;
delimiter ;;
CREATE PROCEDURE `cm_用户_角色列表的用户`(`p_roleid` varchar(4000))
BEGIN
	
select distinct(userid) from cm_user_role where find_in_set(roleid, p_roleid);
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_用户_具有的权限
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_用户_具有的权限`;
delimiter ;;
CREATE PROCEDURE `cm_用户_具有的权限`(`p_userid` bigint)
BEGIN

select id, name
from
	(
		select distinct ( b.id ),
			b.name
		from
			cm_role_per a
			left join cm_permission b on a.perid = b.id 
		where
			exists (
					select
						roleid 
					from
						cm_user_role c 
					where
						a.roleid = c.roleid 
						and userid = p_userid
				  union
					select
						roleid 
					from
						cm_group_role d 
					where
						a.roleid = d.roleid 
						and exists ( select groupid from cm_user_group e where d.groupid = e.groupid and e.userid = p_userid ) 
			) 
			or a.roleid = 1 
	) t 
order by
	id;
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for cm_用户_可访问的菜单
-- ----------------------------
DROP PROCEDURE IF EXISTS `cm_用户_可访问的菜单`;
delimiter ;;
CREATE PROCEDURE `cm_用户_可访问的菜单`(`p_userid` bigint)
BEGIN

select id,name
  from (select distinct (b.id), b.name, dispidx
          from cm_role_menu a
          left join cm_menu b
            on a.menuid = b.id
         where exists
         (select roleid
                  from cm_user_role c
                 where a.roleid = c.roleid
                   and userid = p_userid
					union
					select roleid
					        from cm_group_role d
									where a.roleid = d.roleid
									  and exists (select groupid from cm_user_group e where d.groupid=e.groupid and e.userid=p_userid)
					) or a.roleid=1
			 ) t
 order by dispidx;
 
END
;;
delimiter ;

-- ----------------------------
-- Function structure for nextval
-- ----------------------------
DROP FUNCTION IF EXISTS `nextval`;
delimiter ;;
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
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;

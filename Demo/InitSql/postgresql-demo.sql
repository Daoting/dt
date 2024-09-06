/*
 Navicat Premium Data Transfer

 Source Server         : dt
 Source Server Type    : PostgreSQL
 Source Server Version : 150003 (150003)
 Source Host           : 10.10.1.2:5432
 Source Catalog        : dt
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 150003 (150003)
 File Encoding         : 65001

 Date: 16/07/2024 15:15:58
*/


-- ----------------------------
-- Sequence structure for cm_menu_dispidx
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."cm_menu_dispidx";
CREATE SEQUENCE "public"."cm_menu_dispidx" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 90
CACHE 1;

-- ----------------------------
-- Sequence structure for cm_option_dispidx
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."cm_option_dispidx";
CREATE SEQUENCE "public"."cm_option_dispidx" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1032
CACHE 1;

-- ----------------------------
-- Sequence structure for cm_wfd_prc_dispidx
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."cm_wfd_prc_dispidx";
CREATE SEQUENCE "public"."cm_wfd_prc_dispidx" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 12
CACHE 1;

-- ----------------------------
-- Sequence structure for cm_wfi_item_dispidx
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."cm_wfi_item_dispidx";
CREATE SEQUENCE "public"."cm_wfi_item_dispidx" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 177
CACHE 1;

-- ----------------------------
-- Sequence structure for cm_wfi_prc_dispidx
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."cm_wfi_prc_dispidx";
CREATE SEQUENCE "public"."cm_wfi_prc_dispidx" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 66
CACHE 1;

-- ----------------------------
-- Sequence structure for crud_基础_序列
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."crud_基础_序列";
CREATE SEQUENCE "public"."crud_基础_序列" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 12
CACHE 1;

-- ----------------------------
-- Table structure for cm_cache
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_cache";
CREATE TABLE "public"."cm_cache" (
  "id" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "val" varchar(512) COLLATE "pg_catalog"."default"
)
;
COMMENT ON TABLE "public"."cm_cache" IS '模拟redis缓存key value数据，直连数据库时用';

-- ----------------------------
-- Records of cm_cache
-- ----------------------------

-- ----------------------------
-- Table structure for cm_file_my
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_file_my";
CREATE TABLE "public"."cm_file_my" (
  "id" int8 NOT NULL,
  "parent_id" int8,
  "name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "is_folder" bool NOT NULL,
  "ext_name" varchar(8) COLLATE "pg_catalog"."default",
  "info" varchar(512) COLLATE "pg_catalog"."default",
  "ctime" timestamp(0) NOT NULL,
  "user_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_file_my"."id" IS '文件标识';
COMMENT ON COLUMN "public"."cm_file_my"."parent_id" IS '上级目录，根目录的parendid为空';
COMMENT ON COLUMN "public"."cm_file_my"."name" IS '名称';
COMMENT ON COLUMN "public"."cm_file_my"."is_folder" IS '是否为文件夹';
COMMENT ON COLUMN "public"."cm_file_my"."ext_name" IS '文件扩展名';
COMMENT ON COLUMN "public"."cm_file_my"."info" IS '文件描述信息';
COMMENT ON COLUMN "public"."cm_file_my"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_file_my"."user_id" IS '所属用户';
COMMENT ON TABLE "public"."cm_file_my" IS '个人文件';

-- ----------------------------
-- Records of cm_file_my
-- ----------------------------

-- ----------------------------
-- Table structure for cm_file_pub
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_file_pub";
CREATE TABLE "public"."cm_file_pub" (
  "id" int8 NOT NULL,
  "parent_id" int8,
  "name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "is_folder" bool NOT NULL,
  "ext_name" varchar(8) COLLATE "pg_catalog"."default",
  "info" varchar(512) COLLATE "pg_catalog"."default",
  "ctime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_file_pub"."id" IS '文件标识';
COMMENT ON COLUMN "public"."cm_file_pub"."parent_id" IS '上级目录，根目录的parendid为空';
COMMENT ON COLUMN "public"."cm_file_pub"."name" IS '名称';
COMMENT ON COLUMN "public"."cm_file_pub"."is_folder" IS '是否为文件夹';
COMMENT ON COLUMN "public"."cm_file_pub"."ext_name" IS '文件扩展名';
COMMENT ON COLUMN "public"."cm_file_pub"."info" IS '文件描述信息';
COMMENT ON COLUMN "public"."cm_file_pub"."ctime" IS '创建时间';
COMMENT ON TABLE "public"."cm_file_pub" IS '公共文件';

-- ----------------------------
-- Records of cm_file_pub
-- ----------------------------
INSERT INTO "public"."cm_file_pub" VALUES (1, NULL, '公共文件', 't', NULL, '', '2020-10-21 15:19:20');
INSERT INTO "public"."cm_file_pub" VALUES (2, NULL, '素材库', 't', NULL, '', '2020-10-21 15:20:21');

-- ----------------------------
-- Table structure for cm_group
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_group";
CREATE TABLE "public"."cm_group" (
  "id" int8 NOT NULL,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "note" varchar(255) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."cm_group"."id" IS '组标识';
COMMENT ON COLUMN "public"."cm_group"."name" IS '组名';
COMMENT ON COLUMN "public"."cm_group"."note" IS '组描述';
COMMENT ON TABLE "public"."cm_group" IS '分组，与用户和角色多对多';

-- ----------------------------
-- Records of cm_group
-- ----------------------------
INSERT INTO "public"."cm_group" VALUES (87370632757800960, '新分组1', NULL);
INSERT INTO "public"."cm_group" VALUES (87370661568475136, '新分组2', NULL);

-- ----------------------------
-- Table structure for cm_group_role
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_group_role";
CREATE TABLE "public"."cm_group_role" (
  "group_id" int8 NOT NULL,
  "role_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_group_role"."group_id" IS '组标识';
COMMENT ON COLUMN "public"."cm_group_role"."role_id" IS '角色标识';
COMMENT ON TABLE "public"."cm_group_role" IS '组一角色多对多';

-- ----------------------------
-- Records of cm_group_role
-- ----------------------------
INSERT INTO "public"."cm_group_role" VALUES (87370661568475136, 87363447483035648);
INSERT INTO "public"."cm_group_role" VALUES (87370661568475136, 87368228331089920);

-- ----------------------------
-- Table structure for cm_menu
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_menu";
CREATE TABLE "public"."cm_menu" (
  "id" int8 NOT NULL,
  "parent_id" int8,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "is_group" bool NOT NULL,
  "view_name" varchar(128) COLLATE "pg_catalog"."default",
  "params" varchar(4000) COLLATE "pg_catalog"."default",
  "icon" varchar(128) COLLATE "pg_catalog"."default",
  "note" varchar(512) COLLATE "pg_catalog"."default",
  "dispidx" int4 NOT NULL,
  "is_locked" bool NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_menu"."id" IS '菜单标识';
COMMENT ON COLUMN "public"."cm_menu"."parent_id" IS '父菜单标识';
COMMENT ON COLUMN "public"."cm_menu"."name" IS '菜单名称';
COMMENT ON COLUMN "public"."cm_menu"."is_group" IS '分组或实例。0表实例，1表分组';
COMMENT ON COLUMN "public"."cm_menu"."view_name" IS '视图名称';
COMMENT ON COLUMN "public"."cm_menu"."params" IS '传递给菜单程序的参数';
COMMENT ON COLUMN "public"."cm_menu"."icon" IS '图标';
COMMENT ON COLUMN "public"."cm_menu"."note" IS '备注';
COMMENT ON COLUMN "public"."cm_menu"."dispidx" IS '显示顺序';
COMMENT ON COLUMN "public"."cm_menu"."is_locked" IS '定义了菜单是否被锁定。0表未锁定，1表锁定不可用';
COMMENT ON COLUMN "public"."cm_menu"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_menu"."mtime" IS '最后修改时间';
COMMENT ON TABLE "public"."cm_menu" IS '业务菜单';

-- ----------------------------
-- Records of cm_menu
-- ----------------------------
INSERT INTO "public"."cm_menu" VALUES (2, 1, '用户账号', 'f', '用户账号', NULL, '钥匙', '账号管理及所属分组、关联角色，查看拥有菜单、已授权限', 2, 'f', '2019-11-08 11:42:28', '2019-11-08 11:43:53');
INSERT INTO "public"."cm_menu" VALUES (3, 1, '菜单管理', 'f', '菜单管理', NULL, '大图标', '菜单、菜单组管理和菜单授权角色', 3, 'f', '2019-03-11 11:35:59', '2019-03-11 11:35:58');
INSERT INTO "public"."cm_menu" VALUES (4, 1, '系统角色', 'f', '系统角色', NULL, '两人', '角色管理及所属分组、关联用户、拥有菜单、授予权限的管理', 4, 'f', '2019-11-08 11:47:21', '2019-11-08 11:48:22');
INSERT INTO "public"."cm_menu" VALUES (5, 1, '分组管理', 'f', '分组管理', NULL, '分组', '分组管理及关联的角色、关联的用户', 5, 'f', '2023-03-10 08:34:49', '2023-03-10 08:34:49');
INSERT INTO "public"."cm_menu" VALUES (6, 1, '基础权限', 'f', '基础权限', NULL, '审核', '按照模块、功能两级目录管理的权限和关联的角色', 6, 'f', '2019-03-12 09:11:22', '2019-03-07 11:23:40');
INSERT INTO "public"."cm_menu" VALUES (7, 1, '参数定义', 'f', '参数定义', NULL, '调色板', '参数名称、默认值的定义管理', 7, 'f', '2019-03-12 15:35:56', '2019-03-12 15:37:10');
INSERT INTO "public"."cm_menu" VALUES (8, 1, '基础选项', 'f', '基础选项', NULL, '修理', '按照分组管理的选项列表，如民族、学历等静态列表', 8, 'f', '2019-11-08 11:49:40', '2019-11-08 11:49:46');
INSERT INTO "public"."cm_menu" VALUES (9, 1, '报表设计', 'f', '报表设计', NULL, '折线图', '报表管理及报表模板设计', 9, 'f', '2020-10-19 11:21:38', '2020-10-19 11:21:38');
INSERT INTO "public"."cm_menu" VALUES (10, 1, '流程设计', 'f', '流程设计', NULL, '双绞线', '流程模板设计及流程实例查询', 10, 'f', '2020-11-02 16:21:19', '2020-11-02 16:21:19');
INSERT INTO "public"."cm_menu" VALUES (93146668397260800, NULL, '基础维护', 't', NULL, NULL, NULL, NULL, 1, 'f', '2024-06-14 08:51:37', '2024-06-14 08:51:37');
INSERT INTO "public"."cm_menu" VALUES (93147399237955584, 93146668397260800, '部门管理', 'f', '部门管理', NULL, '多人', NULL, 115, 'f', '2024-06-14 08:54:32', '2024-06-14 08:54:32');
INSERT INTO "public"."cm_menu" VALUES (93147789455028224, 93146668397260800, '人员管理', 'f', '人员管理', NULL, '个人信息', NULL, 117, 'f', '2024-06-14 08:56:05', '2024-06-14 08:56:05');
INSERT INTO "public"."cm_menu" VALUES (95003376719523840, 93146668397260800, '供应商管理', 'f', '供应商管理', NULL, '全局', NULL, 119, 'f', '2024-06-19 11:49:30', '2024-06-19 11:49:30');
INSERT INTO "public"."cm_menu" VALUES (96885816660619264, NULL, '物资管理', 't', NULL, NULL, NULL, NULL, 122, 'f', '2024-06-24 16:29:45', '2024-06-24 16:29:45');
INSERT INTO "public"."cm_menu" VALUES (1, NULL, '工作台', 't', '', NULL, '搬运工', NULL, 123, 'f', '2019-03-07 10:45:44', '2019-03-07 10:45:43');
INSERT INTO "public"."cm_menu" VALUES (96888679717695488, 96885816660619264, '其他入库管理', 'f', '其他入库管理', NULL, '三角锥', NULL, 125, 'f', '2024-06-24 16:41:07', '2024-06-24 16:41:07');
INSERT INTO "public"."cm_menu" VALUES (96888913680166912, 96885816660619264, '物资领用管理', 'f', '物资领用管理', NULL, '六角锥', NULL, 126, 'f', '2024-06-24 16:42:03', '2024-06-24 16:42:03');
INSERT INTO "public"."cm_menu" VALUES (96889200914493440, 96885816660619264, '其他出库管理', 'f', '其他出库管理', NULL, '接通', NULL, 127, 'f', '2024-06-24 16:43:12', '2024-06-24 16:43:12');
INSERT INTO "public"."cm_menu" VALUES (96889439553613824, 96885816660619264, '物资盘存管理', 'f', '物资盘存管理', NULL, '文件', NULL, 128, 'f', '2024-06-24 16:44:09', '2024-06-24 16:44:09');
INSERT INTO "public"."cm_menu" VALUES (96889910070636544, 96885816660619264, '物资计划管理', 'f', '物资计划管理', NULL, '外设', NULL, 129, 'f', '2024-06-24 16:46:01', '2024-06-24 16:46:01');
INSERT INTO "public"."cm_menu" VALUES (97869834403213312, NULL, '测试组', 't', NULL, NULL, NULL, NULL, 130, 'f', '2024-06-27 09:39:50', '2024-06-27 09:39:50');
INSERT INTO "public"."cm_menu" VALUES (97869954830069760, 97869834403213312, '一级菜单1', 'f', NULL, NULL, '文件', NULL, 131, 'f', '2024-06-27 09:40:18', '2024-06-27 09:40:18');
INSERT INTO "public"."cm_menu" VALUES (97870059381485568, 97869834403213312, '一级菜单2', 'f', NULL, NULL, '文件', NULL, 132, 'f', '2024-06-27 09:40:43', '2024-06-27 09:40:43');
INSERT INTO "public"."cm_menu" VALUES (97870113269903360, 97869834403213312, '二级组', 't', NULL, NULL, NULL, NULL, 133, 'f', '2024-06-27 09:40:56', '2024-06-27 09:40:56');
INSERT INTO "public"."cm_menu" VALUES (97870286377218048, 97870113269903360, '二级菜单1', 'f', NULL, NULL, '文件', NULL, 134, 'f', '2024-06-27 09:41:37', '2024-06-27 09:41:37');
INSERT INTO "public"."cm_menu" VALUES (97870350000615424, 97870113269903360, '二级菜单2', 'f', NULL, NULL, '文件', NULL, 135, 'f', '2024-06-27 09:41:52', '2024-06-27 09:41:52');
INSERT INTO "public"."cm_menu" VALUES (97871217135218688, 97870113269903360, '三级组', 't', NULL, NULL, NULL, NULL, 136, 'f', '2024-06-27 09:45:19', '2024-06-27 09:45:19');
INSERT INTO "public"."cm_menu" VALUES (97871290111913984, 97871217135218688, '三级菜单', 'f', NULL, NULL, '文件', NULL, 137, 'f', '2024-06-27 09:45:37', '2024-06-27 09:45:37');
INSERT INTO "public"."cm_menu" VALUES (96886018188537856, 96885816660619264, '外购入库管理', 'f', '外购入库', NULL, '四面体', NULL, 124, 'f', '2024-06-24 16:30:33', '2024-06-24 16:30:33');
INSERT INTO "public"."cm_menu" VALUES (95004558183657472, 93146668397260800, '物资目录管理', 'f', '物资目录管理', NULL, '文件', NULL, 121, 'f', '2024-06-19 11:54:11', '2024-06-19 11:54:11');

-- ----------------------------
-- Table structure for cm_option
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_option";
CREATE TABLE "public"."cm_option" (
  "id" int8 NOT NULL,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "dispidx" int4 NOT NULL,
  "group_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_option"."id" IS '标识';
COMMENT ON COLUMN "public"."cm_option"."name" IS '选项名称';
COMMENT ON COLUMN "public"."cm_option"."dispidx" IS '显示顺序';
COMMENT ON COLUMN "public"."cm_option"."group_id" IS '所属分组';
COMMENT ON TABLE "public"."cm_option" IS '基础选项';

-- ----------------------------
-- Records of cm_option
-- ----------------------------
INSERT INTO "public"."cm_option" VALUES (2, '汉族', 2, 1);
INSERT INTO "public"."cm_option" VALUES (3, '蒙古族', 3, 1);
INSERT INTO "public"."cm_option" VALUES (4, '回族', 4, 1);
INSERT INTO "public"."cm_option" VALUES (5, '藏族', 5, 1);
INSERT INTO "public"."cm_option" VALUES (6, '维吾尔族', 6, 1);
INSERT INTO "public"."cm_option" VALUES (7, '苗族', 7, 1);
INSERT INTO "public"."cm_option" VALUES (8, '彝族', 8, 1);
INSERT INTO "public"."cm_option" VALUES (9, '壮族', 9, 1);
INSERT INTO "public"."cm_option" VALUES (10, '布依族', 10, 1);
INSERT INTO "public"."cm_option" VALUES (11, '朝鲜族', 11, 1);
INSERT INTO "public"."cm_option" VALUES (12, '满族', 12, 1);
INSERT INTO "public"."cm_option" VALUES (13, '侗族', 13, 1);
INSERT INTO "public"."cm_option" VALUES (14, '瑶族', 14, 1);
INSERT INTO "public"."cm_option" VALUES (15, '白族', 15, 1);
INSERT INTO "public"."cm_option" VALUES (16, '土家族', 16, 1);
INSERT INTO "public"."cm_option" VALUES (17, '哈尼族', 17, 1);
INSERT INTO "public"."cm_option" VALUES (18, '哈萨克族', 18, 1);
INSERT INTO "public"."cm_option" VALUES (19, '傣族', 19, 1);
INSERT INTO "public"."cm_option" VALUES (20, '黎族', 20, 1);
INSERT INTO "public"."cm_option" VALUES (21, '傈僳族', 21, 1);
INSERT INTO "public"."cm_option" VALUES (22, '佤族', 22, 1);
INSERT INTO "public"."cm_option" VALUES (23, '畲族', 23, 1);
INSERT INTO "public"."cm_option" VALUES (24, '高山族', 24, 1);
INSERT INTO "public"."cm_option" VALUES (25, '拉祜族', 25, 1);
INSERT INTO "public"."cm_option" VALUES (26, '水族', 26, 1);
INSERT INTO "public"."cm_option" VALUES (27, '东乡族', 27, 1);
INSERT INTO "public"."cm_option" VALUES (28, '纳西族', 28, 1);
INSERT INTO "public"."cm_option" VALUES (29, '景颇族', 29, 1);
INSERT INTO "public"."cm_option" VALUES (30, '柯尔克孜族', 30, 1);
INSERT INTO "public"."cm_option" VALUES (31, '土族', 31, 1);
INSERT INTO "public"."cm_option" VALUES (32, '达斡尔族', 32, 1);
INSERT INTO "public"."cm_option" VALUES (33, '仫佬族', 33, 1);
INSERT INTO "public"."cm_option" VALUES (34, '羌族', 34, 1);
INSERT INTO "public"."cm_option" VALUES (35, '布朗族', 35, 1);
INSERT INTO "public"."cm_option" VALUES (36, '撒拉族', 36, 1);
INSERT INTO "public"."cm_option" VALUES (37, '毛难族', 37, 1);
INSERT INTO "public"."cm_option" VALUES (38, '仡佬族', 38, 1);
INSERT INTO "public"."cm_option" VALUES (39, '锡伯族', 39, 1);
INSERT INTO "public"."cm_option" VALUES (40, '阿昌族', 40, 1);
INSERT INTO "public"."cm_option" VALUES (41, '普米族', 41, 1);
INSERT INTO "public"."cm_option" VALUES (42, '塔吉克族', 42, 1);
INSERT INTO "public"."cm_option" VALUES (43, '怒族', 43, 1);
INSERT INTO "public"."cm_option" VALUES (44, '乌孜别克族', 44, 1);
INSERT INTO "public"."cm_option" VALUES (45, '俄罗斯族', 45, 1);
INSERT INTO "public"."cm_option" VALUES (46, '鄂温克族', 46, 1);
INSERT INTO "public"."cm_option" VALUES (47, '德昂族', 47, 1);
INSERT INTO "public"."cm_option" VALUES (48, '保安族', 48, 1);
INSERT INTO "public"."cm_option" VALUES (49, '裕固族', 49, 1);
INSERT INTO "public"."cm_option" VALUES (50, '京族', 50, 1);
INSERT INTO "public"."cm_option" VALUES (51, '塔塔尔族', 51, 1);
INSERT INTO "public"."cm_option" VALUES (52, '独龙族', 52, 1);
INSERT INTO "public"."cm_option" VALUES (53, '鄂伦春族', 53, 1);
INSERT INTO "public"."cm_option" VALUES (54, '赫哲族', 54, 1);
INSERT INTO "public"."cm_option" VALUES (55, '门巴族', 55, 1);
INSERT INTO "public"."cm_option" VALUES (56, '珞巴族', 56, 1);
INSERT INTO "public"."cm_option" VALUES (57, '基诺族', 57, 1);
INSERT INTO "public"."cm_option" VALUES (58, '大学', 58, 2);
INSERT INTO "public"."cm_option" VALUES (59, '高中', 59, 2);
INSERT INTO "public"."cm_option" VALUES (60, '中学', 60, 2);
INSERT INTO "public"."cm_option" VALUES (61, '小学', 61, 2);
INSERT INTO "public"."cm_option" VALUES (62, '硕士', 62, 2);
INSERT INTO "public"."cm_option" VALUES (63, '博士', 63, 2);
INSERT INTO "public"."cm_option" VALUES (64, '其他', 64, 2);
INSERT INTO "public"."cm_option" VALUES (342, '男', 342, 4);
INSERT INTO "public"."cm_option" VALUES (343, '女', 343, 4);
INSERT INTO "public"."cm_option" VALUES (344, '未知', 344, 4);
INSERT INTO "public"."cm_option" VALUES (345, '不明', 345, 4);
INSERT INTO "public"."cm_option" VALUES (346, 'string', 346, 5);
INSERT INTO "public"."cm_option" VALUES (347, 'int', 347, 5);
INSERT INTO "public"."cm_option" VALUES (348, 'double', 348, 5);
INSERT INTO "public"."cm_option" VALUES (349, 'DateTime', 349, 5);
INSERT INTO "public"."cm_option" VALUES (350, 'Date', 350, 5);
INSERT INTO "public"."cm_option" VALUES (351, 'bool', 351, 5);
INSERT INTO "public"."cm_option" VALUES (87085657688928256, '1', 1033, 87084798125043712);
INSERT INTO "public"."cm_option" VALUES (87340416941453312, '2', 1041, 87340355956273152);
INSERT INTO "public"."cm_option" VALUES (87340427989250048, '3', 1042, 87340355956273152);
INSERT INTO "public"."cm_option" VALUES (87358859103203328, 'sd', 1046, 87358742535106560);
INSERT INTO "public"."cm_option" VALUES (88109942872240128, '按不出', 1047, 87358742535106560);
INSERT INTO "public"."cm_option" VALUES (88109994864832512, '234', 1048, 87340355956273152);
INSERT INTO "public"."cm_option" VALUES (88111462170783744, '啊啊啊', 1049, 87358742535106560);

-- ----------------------------
-- Table structure for cm_option_group
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_option_group";
CREATE TABLE "public"."cm_option_group" (
  "id" int8 NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_option_group"."id" IS '标识';
COMMENT ON COLUMN "public"."cm_option_group"."name" IS '分组名称';
COMMENT ON TABLE "public"."cm_option_group" IS '基础选项分组';

-- ----------------------------
-- Records of cm_option_group
-- ----------------------------
INSERT INTO "public"."cm_option_group" VALUES (1, '民族');
INSERT INTO "public"."cm_option_group" VALUES (2, '学历');
INSERT INTO "public"."cm_option_group" VALUES (3, '地区');
INSERT INTO "public"."cm_option_group" VALUES (4, '性别');
INSERT INTO "public"."cm_option_group" VALUES (5, '数据类型');
INSERT INTO "public"."cm_option_group" VALUES (87084798125043712, '按不出');
INSERT INTO "public"."cm_option_group" VALUES (87340355956273152, 'b');
INSERT INTO "public"."cm_option_group" VALUES (87358742535106560, 'ds');
INSERT INTO "public"."cm_option_group" VALUES (88144064831320064, '阿斯顿1234');

-- ----------------------------
-- Table structure for cm_params
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_params";
CREATE TABLE "public"."cm_params" (
  "id" int8 NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "value" varchar(255) COLLATE "pg_catalog"."default",
  "note" varchar(255) COLLATE "pg_catalog"."default",
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_params"."id" IS '用户参数标识';
COMMENT ON COLUMN "public"."cm_params"."name" IS '参数名称';
COMMENT ON COLUMN "public"."cm_params"."value" IS '参数缺省值';
COMMENT ON COLUMN "public"."cm_params"."note" IS '参数描述';
COMMENT ON COLUMN "public"."cm_params"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_params"."mtime" IS '修改时间';
COMMENT ON TABLE "public"."cm_params" IS '用户参数定义';

-- ----------------------------
-- Records of cm_params
-- ----------------------------
INSERT INTO "public"."cm_params" VALUES (1, '接收新任务', 'true', '', '2020-12-01 15:13:49', '2020-12-02 09:23:53');
INSERT INTO "public"."cm_params" VALUES (2, '接收新发布通知', 'true', '', '2020-12-02 09:25:15', '2020-12-02 09:25:15');
INSERT INTO "public"."cm_params" VALUES (3, '接收新消息', 'true', '接收通讯录消息推送', '2020-12-02 09:24:28', '2020-12-02 09:24:28');

-- ----------------------------
-- Table structure for cm_permission
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_permission";
CREATE TABLE "public"."cm_permission" (
  "id" int8 NOT NULL,
  "func_id" int8 NOT NULL,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "note" varchar(255) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."cm_permission"."id" IS '权限标识';
COMMENT ON COLUMN "public"."cm_permission"."func_id" IS '所属功能';
COMMENT ON COLUMN "public"."cm_permission"."name" IS '权限名称';
COMMENT ON COLUMN "public"."cm_permission"."note" IS '权限描述';
COMMENT ON TABLE "public"."cm_permission" IS '权限';

-- ----------------------------
-- Records of cm_permission
-- ----------------------------
INSERT INTO "public"."cm_permission" VALUES (1, 1, '公共文件增删', '公共文件的上传、删除等');
INSERT INTO "public"."cm_permission" VALUES (2, 1, '素材库增删', '素材库目录的上传、删除等');
INSERT INTO "public"."cm_permission" VALUES (87428792977747968, 87428740729303040, '新权限1', NULL);
INSERT INTO "public"."cm_permission" VALUES (87433224209592320, 87428740729303040, 'qx1', NULL);
INSERT INTO "public"."cm_permission" VALUES (87433598698024960, 87428740729303040, '阿斯顿', NULL);
INSERT INTO "public"."cm_permission" VALUES (87434002596917248, 87433900117487616, '冲销', NULL);
INSERT INTO "public"."cm_permission" VALUES (87434193769099264, 87428694306746368, '权限1', NULL);

-- ----------------------------
-- Table structure for cm_permission_func
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_permission_func";
CREATE TABLE "public"."cm_permission_func" (
  "id" int8 NOT NULL,
  "module_id" int8 NOT NULL,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "note" varchar(255) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."cm_permission_func"."module_id" IS '所属模块';
COMMENT ON COLUMN "public"."cm_permission_func"."name" IS '功能名称';
COMMENT ON COLUMN "public"."cm_permission_func"."note" IS '功能描述';
COMMENT ON TABLE "public"."cm_permission_func" IS '权限所属功能';

-- ----------------------------
-- Records of cm_permission_func
-- ----------------------------
INSERT INTO "public"."cm_permission_func" VALUES (1, 1, '文件管理', '管理文件的上传、删除等');
INSERT INTO "public"."cm_permission_func" VALUES (87428694306746368, 87427935636844544, '功能1', NULL);
INSERT INTO "public"."cm_permission_func" VALUES (87428740729303040, 87427935636844544, '功能2', NULL);
INSERT INTO "public"."cm_permission_func" VALUES (87433900117487616, 87433840629673984, '外购入库', NULL);

-- ----------------------------
-- Table structure for cm_permission_module
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_permission_module";
CREATE TABLE "public"."cm_permission_module" (
  "id" int8 NOT NULL,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "note" varchar(255) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."cm_permission_module"."id" IS '模块标识';
COMMENT ON COLUMN "public"."cm_permission_module"."name" IS '模块名称';
COMMENT ON COLUMN "public"."cm_permission_module"."note" IS '模块描述';
COMMENT ON TABLE "public"."cm_permission_module" IS '权限所属模块';

-- ----------------------------
-- Records of cm_permission_module
-- ----------------------------
INSERT INTO "public"."cm_permission_module" VALUES (1, '系统预留', '系统内部使用的权限控制，禁止删除');
INSERT INTO "public"."cm_permission_module" VALUES (87427935636844544, '模块1', NULL);
INSERT INTO "public"."cm_permission_module" VALUES (87433840629673984, '物资管理', NULL);

-- ----------------------------
-- Table structure for cm_role
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_role";
CREATE TABLE "public"."cm_role" (
  "id" int8 NOT NULL,
  "name" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "note" varchar(255) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."cm_role"."id" IS '角色标识';
COMMENT ON COLUMN "public"."cm_role"."name" IS '角色名称';
COMMENT ON COLUMN "public"."cm_role"."note" IS '角色描述';
COMMENT ON TABLE "public"."cm_role" IS '角色';

-- ----------------------------
-- Records of cm_role
-- ----------------------------
INSERT INTO "public"."cm_role" VALUES (1, '任何人', '所有用户默认都具有该角色，不可删除');
INSERT INTO "public"."cm_role" VALUES (2, '系统管理员', '系统角色，不可删除');
INSERT INTO "public"."cm_role" VALUES (87363447483035648, '库管员', NULL);
INSERT INTO "public"."cm_role" VALUES (87368228331089920, '库主管', NULL);

-- ----------------------------
-- Table structure for cm_role_menu
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_role_menu";
CREATE TABLE "public"."cm_role_menu" (
  "role_id" int8 NOT NULL,
  "menu_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_role_menu"."role_id" IS '角色标识';
COMMENT ON COLUMN "public"."cm_role_menu"."menu_id" IS '菜单标识';
COMMENT ON TABLE "public"."cm_role_menu" IS '角色一菜单多对多';

-- ----------------------------
-- Records of cm_role_menu
-- ----------------------------
INSERT INTO "public"."cm_role_menu" VALUES (2, 2);
INSERT INTO "public"."cm_role_menu" VALUES (2, 3);
INSERT INTO "public"."cm_role_menu" VALUES (2, 4);
INSERT INTO "public"."cm_role_menu" VALUES (2, 5);
INSERT INTO "public"."cm_role_menu" VALUES (2, 6);
INSERT INTO "public"."cm_role_menu" VALUES (1, 7);
INSERT INTO "public"."cm_role_menu" VALUES (1, 8);
INSERT INTO "public"."cm_role_menu" VALUES (1, 9);
INSERT INTO "public"."cm_role_menu" VALUES (2, 10);
INSERT INTO "public"."cm_role_menu" VALUES (87368228331089920, 4);
INSERT INTO "public"."cm_role_menu" VALUES (87368228331089920, 6);
INSERT INTO "public"."cm_role_menu" VALUES (2, 93147399237955584);
INSERT INTO "public"."cm_role_menu" VALUES (2, 93147789455028224);
INSERT INTO "public"."cm_role_menu" VALUES (2, 95003376719523840);
INSERT INTO "public"."cm_role_menu" VALUES (2, 95004558183657472);
INSERT INTO "public"."cm_role_menu" VALUES (1, 97869954830069760);
INSERT INTO "public"."cm_role_menu" VALUES (1, 97870059381485568);
INSERT INTO "public"."cm_role_menu" VALUES (1, 97870350000615424);
INSERT INTO "public"."cm_role_menu" VALUES (1, 97870286377218048);
INSERT INTO "public"."cm_role_menu" VALUES (1, 97871290111913984);
INSERT INTO "public"."cm_role_menu" VALUES (1, 96886018188537856);
INSERT INTO "public"."cm_role_menu" VALUES (1, 96888679717695488);
INSERT INTO "public"."cm_role_menu" VALUES (1, 96888913680166912);
INSERT INTO "public"."cm_role_menu" VALUES (1, 96889200914493440);
INSERT INTO "public"."cm_role_menu" VALUES (1, 96889439553613824);
INSERT INTO "public"."cm_role_menu" VALUES (1, 96889910070636544);

-- ----------------------------
-- Table structure for cm_role_per
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_role_per";
CREATE TABLE "public"."cm_role_per" (
  "role_id" int8 NOT NULL,
  "per_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_role_per"."role_id" IS '角色标识';
COMMENT ON COLUMN "public"."cm_role_per"."per_id" IS '权限标识';
COMMENT ON TABLE "public"."cm_role_per" IS '角色一权限多对多';

-- ----------------------------
-- Records of cm_role_per
-- ----------------------------
INSERT INTO "public"."cm_role_per" VALUES (2, 1);
INSERT INTO "public"."cm_role_per" VALUES (2, 2);
INSERT INTO "public"."cm_role_per" VALUES (87368228331089920, 1);
INSERT INTO "public"."cm_role_per" VALUES (87363447483035648, 87433224209592320);
INSERT INTO "public"."cm_role_per" VALUES (87368228331089920, 87433224209592320);
INSERT INTO "public"."cm_role_per" VALUES (87363447483035648, 87434002596917248);

-- ----------------------------
-- Table structure for cm_rpt
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_rpt";
CREATE TABLE "public"."cm_rpt" (
  "id" int8 NOT NULL,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "define" varchar(65535) COLLATE "pg_catalog"."default",
  "note" varchar(255) COLLATE "pg_catalog"."default",
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_rpt"."id" IS '报表标识';
COMMENT ON COLUMN "public"."cm_rpt"."name" IS '报表名称';
COMMENT ON COLUMN "public"."cm_rpt"."define" IS '报表模板定义';
COMMENT ON COLUMN "public"."cm_rpt"."note" IS '报表描述';
COMMENT ON COLUMN "public"."cm_rpt"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_rpt"."mtime" IS '修改时间';
COMMENT ON TABLE "public"."cm_rpt" IS '报表模板定义';

-- ----------------------------
-- Records of cm_rpt
-- ----------------------------
INSERT INTO "public"."cm_rpt" VALUES (87360002399170560, '12', NULL, NULL, '2024-05-29 09:37:34', '2024-05-29 09:37:34');
INSERT INTO "public"."cm_rpt" VALUES (87361316415893504, 'ab', NULL, NULL, '2024-05-29 09:42:49', '2024-05-29 09:42:49');
INSERT INTO "public"."cm_rpt" VALUES (96817577268113408, '地方', NULL, NULL, '2024-06-24 11:58:40', '2024-06-24 11:58:40');

-- ----------------------------
-- Table structure for cm_user
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_user";
CREATE TABLE "public"."cm_user" (
  "id" int8 NOT NULL,
  "acc" varchar(32) COLLATE "pg_catalog"."default",
  "phone" varchar(16) COLLATE "pg_catalog"."default",
  "pwd" char(32) COLLATE "pg_catalog"."default" NOT NULL,
  "name" varchar(32) COLLATE "pg_catalog"."default",
  "photo" varchar(255) COLLATE "pg_catalog"."default",
  "expired" bool NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_user"."id" IS '用户标识';
COMMENT ON COLUMN "public"."cm_user"."acc" IS '账号，唯一';
COMMENT ON COLUMN "public"."cm_user"."phone" IS '手机号，唯一';
COMMENT ON COLUMN "public"."cm_user"."pwd" IS '密码的md5';
COMMENT ON COLUMN "public"."cm_user"."name" IS '姓名';
COMMENT ON COLUMN "public"."cm_user"."photo" IS '头像';
COMMENT ON COLUMN "public"."cm_user"."expired" IS '是否停用';
COMMENT ON COLUMN "public"."cm_user"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_user"."mtime" IS '修改时间';
COMMENT ON TABLE "public"."cm_user" IS '系统用户';

-- ----------------------------
-- Records of cm_user
-- ----------------------------
INSERT INTO "public"."cm_user" VALUES (1, 'admin', '13511111111', 'b59c67bf196a4758191e42f76670ceba', NULL, '', 'f', '2019-10-24 09:06:38', '2024-05-30 09:38:24');
INSERT INTO "public"."cm_user" VALUES (87375101197316096, 'kzg1', '13511113333', 'b59c67bf196a4758191e42f76670ceba', '', NULL, 'f', '2024-05-29 10:37:34', '2024-06-25 15:30:49');
INSERT INTO "public"."cm_user" VALUES (97233424511954944, 'kgy2', NULL, 'b59c67bf196a4758191e42f76670ceba', NULL, NULL, 'f', '2024-06-25 15:31:09', '2024-06-25 15:31:09');
INSERT INTO "public"."cm_user" VALUES (97233490068926464, 'kgy3', NULL, 'b59c67bf196a4758191e42f76670ceba', NULL, NULL, 'f', '2024-06-25 15:31:18', '2024-06-25 15:31:18');
INSERT INTO "public"."cm_user" VALUES (97233514383306752, 'kgy4', NULL, 'b59c67bf196a4758191e42f76670ceba', NULL, NULL, 'f', '2024-06-25 15:31:24', '2024-06-25 15:31:24');
INSERT INTO "public"."cm_user" VALUES (97233573971783680, 'kzg2', NULL, 'b59c67bf196a4758191e42f76670ceba', NULL, NULL, 'f', '2024-06-25 15:31:37', '2024-06-25 15:31:37');
INSERT INTO "public"."cm_user" VALUES (87374677803298816, 'kgy1', '13511112222', 'b59c67bf196a4758191e42f76670ceba', NULL, NULL, 'f', '2024-05-29 10:35:53', '2024-07-01 14:55:34');

-- ----------------------------
-- Table structure for cm_user_group
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_user_group";
CREATE TABLE "public"."cm_user_group" (
  "user_id" int8 NOT NULL,
  "group_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_user_group"."user_id" IS '用户标识';
COMMENT ON COLUMN "public"."cm_user_group"."group_id" IS '组标识';
COMMENT ON TABLE "public"."cm_user_group" IS '用户一组多对多';

-- ----------------------------
-- Records of cm_user_group
-- ----------------------------
INSERT INTO "public"."cm_user_group" VALUES (1, 87370632757800960);
INSERT INTO "public"."cm_user_group" VALUES (87374677803298816, 87370661568475136);
INSERT INTO "public"."cm_user_group" VALUES (1, 87370661568475136);
INSERT INTO "public"."cm_user_group" VALUES (87375101197316096, 87370632757800960);

-- ----------------------------
-- Table structure for cm_user_params
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_user_params";
CREATE TABLE "public"."cm_user_params" (
  "user_id" int8 NOT NULL,
  "param_id" int8 NOT NULL,
  "value" varchar(255) COLLATE "pg_catalog"."default",
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_user_params"."user_id" IS '用户标识';
COMMENT ON COLUMN "public"."cm_user_params"."param_id" IS '参数标识';
COMMENT ON COLUMN "public"."cm_user_params"."value" IS '参数值';
COMMENT ON COLUMN "public"."cm_user_params"."mtime" IS '修改时间';
COMMENT ON TABLE "public"."cm_user_params" IS '用户参数值';

-- ----------------------------
-- Records of cm_user_params
-- ----------------------------

-- ----------------------------
-- Table structure for cm_user_role
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_user_role";
CREATE TABLE "public"."cm_user_role" (
  "user_id" int8 NOT NULL,
  "role_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_user_role"."user_id" IS '用户标识';
COMMENT ON COLUMN "public"."cm_user_role"."role_id" IS '角色标识';
COMMENT ON TABLE "public"."cm_user_role" IS '用户一角色多对多';

-- ----------------------------
-- Records of cm_user_role
-- ----------------------------
INSERT INTO "public"."cm_user_role" VALUES (1, 2);
INSERT INTO "public"."cm_user_role" VALUES (87374677803298816, 87363447483035648);
INSERT INTO "public"."cm_user_role" VALUES (87375101197316096, 87368228331089920);
INSERT INTO "public"."cm_user_role" VALUES (97233573971783680, 87368228331089920);
INSERT INTO "public"."cm_user_role" VALUES (97233514383306752, 87363447483035648);
INSERT INTO "public"."cm_user_role" VALUES (97233490068926464, 87363447483035648);
INSERT INTO "public"."cm_user_role" VALUES (97233424511954944, 87363447483035648);
INSERT INTO "public"."cm_user_role" VALUES (1, 87363447483035648);
INSERT INTO "public"."cm_user_role" VALUES (1, 87368228331089920);

-- ----------------------------
-- Table structure for cm_wfd_atv
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_wfd_atv";
CREATE TABLE "public"."cm_wfd_atv" (
  "id" int8 NOT NULL,
  "prc_id" int8 NOT NULL,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "type" int2 NOT NULL,
  "exec_scope" int2 NOT NULL,
  "exec_limit" int2 NOT NULL,
  "exec_atv_id" int8,
  "auto_accept" bool NOT NULL,
  "can_delete" bool NOT NULL,
  "can_terminate" bool NOT NULL,
  "can_jump_into" bool NOT NULL,
  "trans_kind" int2 NOT NULL,
  "join_kind" int2 NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_wfd_atv"."id" IS '活动标识';
COMMENT ON COLUMN "public"."cm_wfd_atv"."prc_id" IS '流程标识';
COMMENT ON COLUMN "public"."cm_wfd_atv"."name" IS '活动名称，同时作为状态名称';
COMMENT ON COLUMN "public"."cm_wfd_atv"."type" IS '#WfdAtvType#活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动';
COMMENT ON COLUMN "public"."cm_wfd_atv"."exec_scope" IS '#WfdAtvExecScope#执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户';
COMMENT ON COLUMN "public"."cm_wfd_atv"."exec_limit" IS '#WfdAtvExecLimit#执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者';
COMMENT ON COLUMN "public"."cm_wfd_atv"."exec_atv_id" IS '在执行者限制为3或4时选择的活动';
COMMENT ON COLUMN "public"."cm_wfd_atv"."auto_accept" IS '是否自动签收，打开工作流视图时自动签收工作项';
COMMENT ON COLUMN "public"."cm_wfd_atv"."can_delete" IS '能否删除流程实例和业务数据，0否 1';
COMMENT ON COLUMN "public"."cm_wfd_atv"."can_terminate" IS '能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能';
COMMENT ON COLUMN "public"."cm_wfd_atv"."can_jump_into" IS '是否可作为跳转目标，0不可跳转 1可以';
COMMENT ON COLUMN "public"."cm_wfd_atv"."trans_kind" IS '#WfdAtvTransKind#当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择';
COMMENT ON COLUMN "public"."cm_wfd_atv"."join_kind" IS '#WfdAtvJoinKind#同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步';
COMMENT ON COLUMN "public"."cm_wfd_atv"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_wfd_atv"."mtime" IS '修改时间';
COMMENT ON TABLE "public"."cm_wfd_atv" IS '活动模板';

-- ----------------------------
-- Records of cm_wfd_atv
-- ----------------------------
INSERT INTO "public"."cm_wfd_atv" VALUES (96767714337779712, 96767646822068224, '完成', 3, 0, 0, NULL, 't', 'f', 'f', 'f', 0, 0, '2024-06-24 08:40:27', '2024-06-24 08:40:27');
INSERT INTO "public"."cm_wfd_atv" VALUES (96767673514618880, 96767646822068224, '填写', 1, 0, 0, NULL, 't', 't', 'f', 'f', 2, 0, '2024-06-24 08:40:17', '2024-06-25 15:32:48');
INSERT INTO "public"."cm_wfd_atv" VALUES (96767684025544704, 96767646822068224, '审核', 0, 2, 0, NULL, 't', 'f', 'f', 'f', 0, 0, '2024-06-24 08:40:20', '2024-06-25 15:33:26');

-- ----------------------------
-- Table structure for cm_wfd_atv_role
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_wfd_atv_role";
CREATE TABLE "public"."cm_wfd_atv_role" (
  "atv_id" int8 NOT NULL,
  "role_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_wfd_atv_role"."atv_id" IS '活动标识';
COMMENT ON COLUMN "public"."cm_wfd_atv_role"."role_id" IS '角色标识';
COMMENT ON TABLE "public"."cm_wfd_atv_role" IS '活动授权';

-- ----------------------------
-- Records of cm_wfd_atv_role
-- ----------------------------
INSERT INTO "public"."cm_wfd_atv_role" VALUES (96767673514618880, 87363447483035648);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (96767684025544704, 87368228331089920);

-- ----------------------------
-- Table structure for cm_wfd_prc
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_wfd_prc";
CREATE TABLE "public"."cm_wfd_prc" (
  "id" int8 NOT NULL,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "diagram" varchar(65535) COLLATE "pg_catalog"."default",
  "is_locked" bool NOT NULL,
  "singleton" bool NOT NULL,
  "note" varchar(255) COLLATE "pg_catalog"."default",
  "dispidx" int4 NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_wfd_prc"."id" IS '流程标识';
COMMENT ON COLUMN "public"."cm_wfd_prc"."name" IS '流程名称';
COMMENT ON COLUMN "public"."cm_wfd_prc"."diagram" IS '流程图';
COMMENT ON COLUMN "public"."cm_wfd_prc"."is_locked" IS '锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行';
COMMENT ON COLUMN "public"."cm_wfd_prc"."singleton" IS '同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例';
COMMENT ON COLUMN "public"."cm_wfd_prc"."note" IS '描述';
COMMENT ON COLUMN "public"."cm_wfd_prc"."dispidx" IS '显示顺序';
COMMENT ON COLUMN "public"."cm_wfd_prc"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_wfd_prc"."mtime" IS '最后修改时间';
COMMENT ON TABLE "public"."cm_wfd_prc" IS '流程模板';

-- ----------------------------
-- Records of cm_wfd_prc
-- ----------------------------
INSERT INTO "public"."cm_wfd_prc" VALUES (96801745670533120, '新流程x', NULL, 't', 'f', NULL, 14, '2024-06-24 10:55:41', '-infinity');
INSERT INTO "public"."cm_wfd_prc" VALUES (96812242725793792, '新流程3', NULL, 't', 'f', NULL, 15, '2024-06-24 11:37:23', '-infinity');
INSERT INTO "public"."cm_wfd_prc" VALUES (96767646822068224, '外购入库', '<Sketch><Node id="96767673514618880" title="填写" shape="开始" left="460" top="60" width="80" height="60" /><Node id="96767684025544704" title="审核" shape="任务" left="440" top="200" width="120" height="60" /><Line id="96767701062807552" headerid="96767673514618880" bounds="490,120,20,80" headerport="4" tailid="96767684025544704" tailport="0" /><Node id="96767714337779712" title="完成" shape="结束" background="#FF9D9D9D" borderbrush="#FF969696" left="460" top="340" width="80" height="60" /><Line id="96767731547009024" headerid="96767684025544704" bounds="490,260,20,80" headerport="4" tailid="96767714337779712" tailport="0" /></Sketch>', 'f', 'f', NULL, 13, '2024-06-24 08:40:11', '2024-07-05 09:20:34');

-- ----------------------------
-- Table structure for cm_wfd_trs
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_wfd_trs";
CREATE TABLE "public"."cm_wfd_trs" (
  "id" int8 NOT NULL,
  "prc_id" int8 NOT NULL,
  "src_atv_id" int8 NOT NULL,
  "tgt_atv_id" int8 NOT NULL,
  "is_rollback" bool NOT NULL,
  "trs_id" int8
)
;
COMMENT ON COLUMN "public"."cm_wfd_trs"."id" IS '迁移标识';
COMMENT ON COLUMN "public"."cm_wfd_trs"."prc_id" IS '流程模板标识';
COMMENT ON COLUMN "public"."cm_wfd_trs"."src_atv_id" IS '起始活动模板标识';
COMMENT ON COLUMN "public"."cm_wfd_trs"."tgt_atv_id" IS '目标活动模板标识';
COMMENT ON COLUMN "public"."cm_wfd_trs"."is_rollback" IS '是否为回退迁移';
COMMENT ON COLUMN "public"."cm_wfd_trs"."trs_id" IS '类别为回退迁移时对应的常规迁移标识';
COMMENT ON TABLE "public"."cm_wfd_trs" IS '迁移模板';

-- ----------------------------
-- Records of cm_wfd_trs
-- ----------------------------
INSERT INTO "public"."cm_wfd_trs" VALUES (96767701062807552, 96767646822068224, 96767673514618880, 96767684025544704, 'f', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (96767731547009024, 96767646822068224, 96767684025544704, 96767714337779712, 'f', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (100764090209955840, 96767646822068224, 96767684025544704, 96767673514618880, 't', 96767701062807552);

-- ----------------------------
-- Table structure for cm_wfi_atv
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_wfi_atv";
CREATE TABLE "public"."cm_wfi_atv" (
  "id" int8 NOT NULL,
  "prci_id" int8 NOT NULL,
  "atvd_id" int8 NOT NULL,
  "status" int2 NOT NULL,
  "inst_count" int4 NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_wfi_atv"."id" IS '活动实例标识';
COMMENT ON COLUMN "public"."cm_wfi_atv"."prci_id" IS '流程实例标识';
COMMENT ON COLUMN "public"."cm_wfi_atv"."atvd_id" IS '活动模板标识';
COMMENT ON COLUMN "public"."cm_wfi_atv"."status" IS '#WfiAtvStatus#活动实例的状态 0活动 1结束 2终止 3同步活动';
COMMENT ON COLUMN "public"."cm_wfi_atv"."inst_count" IS '活动实例在流程实例被实例化的次数';
COMMENT ON COLUMN "public"."cm_wfi_atv"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_wfi_atv"."mtime" IS '最后一次状态改变的时间';
COMMENT ON TABLE "public"."cm_wfi_atv" IS '活动实例';

-- ----------------------------
-- Records of cm_wfi_atv
-- ----------------------------
INSERT INTO "public"."cm_wfi_atv" VALUES (100131245670035456, 100131245460320256, 96767673514618880, 1, 1, '2024-07-03 15:25:59', '2024-07-04 10:43:09');
INSERT INTO "public"."cm_wfi_atv" VALUES (100422497007333376, 100131245460320256, 96767684025544704, 0, 1, '2024-07-04 10:43:09', '2024-07-04 10:43:09');
INSERT INTO "public"."cm_wfi_atv" VALUES (100436235529777152, 100436235504611328, 96767673514618880, 1, 1, '2024-07-04 11:37:51', '2024-07-04 11:38:02');
INSERT INTO "public"."cm_wfi_atv" VALUES (100436307248181248, 100436235504611328, 96767684025544704, 0, 1, '2024-07-04 11:38:02', '2024-07-04 11:38:02');
INSERT INTO "public"."cm_wfi_atv" VALUES (100437023425589248, 100437023215874048, 96767673514618880, 1, 1, '2024-07-04 11:40:59', '2024-07-04 11:41:03');
INSERT INTO "public"."cm_wfi_atv" VALUES (100437067289620480, 100437023215874048, 96767684025544704, 0, 1, '2024-07-04 11:41:03', '2024-07-04 11:41:03');
INSERT INTO "public"."cm_wfi_atv" VALUES (100441222624342016, 100441222339129344, 96767673514618880, 1, 1, '2024-07-04 11:57:37', '2024-07-04 11:57:42');
INSERT INTO "public"."cm_wfi_atv" VALUES (100441258733105152, 100441222339129344, 96767684025544704, 0, 1, '2024-07-04 11:57:42', '2024-07-04 11:57:42');
INSERT INTO "public"."cm_wfi_atv" VALUES (100460630063214592, 100460629845110784, 96767673514618880, 1, 1, '2024-07-04 13:14:46', '2024-07-04 13:14:50');
INSERT INTO "public"."cm_wfi_atv" VALUES (100460669560975360, 100460629845110784, 96767684025544704, 0, 1, '2024-07-04 13:14:50', '2024-07-04 13:14:50');
INSERT INTO "public"."cm_wfi_atv" VALUES (100460833491152896, 100460833314992128, 96767673514618880, 1, 1, '2024-07-04 13:15:33', '2024-07-04 13:15:37');
INSERT INTO "public"."cm_wfi_atv" VALUES (100461502608470016, 100461464012484608, 96767684025544704, 0, 1, '2024-07-04 13:18:09', '2024-07-04 13:18:09');
INSERT INTO "public"."cm_wfi_atv" VALUES (100461464037650432, 100461464012484608, 96767673514618880, 0, 2, '2024-07-04 13:18:05', '2024-07-04 13:43:33');
INSERT INTO "public"."cm_wfi_atv" VALUES (100482448593547264, 100482448404803584, 96767673514618880, 1, 1, '2024-07-04 14:41:34', '2024-07-04 14:41:39');
INSERT INTO "public"."cm_wfi_atv" VALUES (100482517124280320, 100482448404803584, 96767684025544704, 0, 1, '2024-07-04 14:41:39', '2024-07-04 14:41:39');
INSERT INTO "public"."cm_wfi_atv" VALUES (100460864222818304, 100460833314992128, 96767684025544704, 1, 1, '2024-07-04 13:15:36', '2024-07-05 09:21:10');
INSERT INTO "public"."cm_wfi_atv" VALUES (100764247085314048, 100460833314992128, 96767673514618880, 0, 1, '2024-07-05 09:21:10', '2024-07-05 09:21:10');
INSERT INTO "public"."cm_wfi_atv" VALUES (103392972188708864, 103392971937050624, 96767673514618880, 1, 1, '2024-07-12 15:26:54', '2024-07-12 15:27:03');
INSERT INTO "public"."cm_wfi_atv" VALUES (103393041159843840, 103392971937050624, 96767684025544704, 0, 1, '2024-07-12 15:27:03', '2024-07-12 15:27:03');
INSERT INTO "public"."cm_wfi_atv" VALUES (103401551926947840, 103401551914364928, 96767673514618880, 0, 1, '2024-07-12 16:00:55', '2024-07-12 16:00:55');

-- ----------------------------
-- Table structure for cm_wfi_item
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_wfi_item";
CREATE TABLE "public"."cm_wfi_item" (
  "id" int8 NOT NULL,
  "atvi_id" int8 NOT NULL,
  "status" int2 NOT NULL,
  "assign_kind" int2 NOT NULL,
  "sender_id" int8,
  "sender" varchar(32) COLLATE "pg_catalog"."default",
  "stime" timestamp(0) NOT NULL,
  "is_accept" bool NOT NULL,
  "accept_time" timestamp(0),
  "role_id" int8,
  "user_id" int8,
  "note" varchar(255) COLLATE "pg_catalog"."default",
  "dispidx" int4 NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_wfi_item"."id" IS '工作项标识';
COMMENT ON COLUMN "public"."cm_wfi_item"."atvi_id" IS '活动实例标识';
COMMENT ON COLUMN "public"."cm_wfi_item"."status" IS '#WfiItemStatus#工作项状态 0活动 1结束 2终止 3同步活动';
COMMENT ON COLUMN "public"."cm_wfi_item"."assign_kind" IS '#WfiItemAssignKind#指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派';
COMMENT ON COLUMN "public"."cm_wfi_item"."sender_id" IS '发送者标识';
COMMENT ON COLUMN "public"."cm_wfi_item"."sender" IS '发送者姓名';
COMMENT ON COLUMN "public"."cm_wfi_item"."stime" IS '发送时间';
COMMENT ON COLUMN "public"."cm_wfi_item"."is_accept" IS '是否签收此项任务';
COMMENT ON COLUMN "public"."cm_wfi_item"."accept_time" IS '签收时间';
COMMENT ON COLUMN "public"."cm_wfi_item"."role_id" IS '执行者角色标识';
COMMENT ON COLUMN "public"."cm_wfi_item"."user_id" IS '执行者用户标识';
COMMENT ON COLUMN "public"."cm_wfi_item"."note" IS '工作项备注';
COMMENT ON COLUMN "public"."cm_wfi_item"."dispidx" IS '显示顺序';
COMMENT ON COLUMN "public"."cm_wfi_item"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_wfi_item"."mtime" IS '最后一次状态改变的时间';
COMMENT ON TABLE "public"."cm_wfi_item" IS '工作项';

-- ----------------------------
-- Records of cm_wfi_item
-- ----------------------------
INSERT INTO "public"."cm_wfi_item" VALUES (100131245858779136, 100131245670035456, 1, 1, 87374677803298816, '王库管', '2024-07-03 15:25:59', 't', '2024-07-03 15:25:59', NULL, 87374677803298816, NULL, 189, '2024-07-03 15:25:59', '2024-07-04 10:43:09');
INSERT INTO "public"."cm_wfi_item" VALUES (100436235550748672, 100436235529777152, 1, 1, 1, '测试员', '2024-07-04 11:37:51', 't', '2024-07-04 11:37:51', NULL, 1, NULL, 193, '2024-07-04 11:37:51', '2024-07-04 11:38:02');
INSERT INTO "public"."cm_wfi_item" VALUES (100437023626915840, 100437023425589248, 1, 1, 1, '测试员', '2024-07-04 11:40:59', 't', '2024-07-04 11:40:59', NULL, 1, NULL, 196, '2024-07-04 11:40:59', '2024-07-04 11:41:03');
INSERT INTO "public"."cm_wfi_item" VALUES (100441222808891392, 100441222624342016, 1, 1, 1, '测试员', '2024-07-04 11:57:37', 't', '2024-07-04 11:57:37', NULL, 1, NULL, 199, '2024-07-04 11:57:37', '2024-07-04 11:57:42');
INSERT INTO "public"."cm_wfi_item" VALUES (100441258980569088, 100441258733105152, 0, 0, 1, '测试员', '2024-07-04 11:57:42', 't', '2024-07-04 11:58:08', NULL, 1, NULL, 200, '2024-07-04 11:57:42', '2024-07-04 11:57:42');
INSERT INTO "public"."cm_wfi_item" VALUES (100460630272929792, 100460630063214592, 1, 1, 1, '测试员', '2024-07-04 13:14:46', 't', '2024-07-04 13:14:46', NULL, 1, NULL, 202, '2024-07-04 13:14:46', '2024-07-04 13:14:50');
INSERT INTO "public"."cm_wfi_item" VALUES (100460833663119360, 100460833491152896, 1, 1, 1, '测试员', '2024-07-04 13:15:33', 't', '2024-07-04 13:15:33', NULL, 1, NULL, 205, '2024-07-04 13:15:33', '2024-07-04 13:15:37');
INSERT INTO "public"."cm_wfi_item" VALUES (100461464058621952, 100461464037650432, 1, 1, 1, '测试员', '2024-07-04 13:18:05', 't', '2024-07-04 13:18:05', NULL, 1, NULL, 208, '2024-07-04 13:18:05', '2024-07-04 13:18:09');
INSERT INTO "public"."cm_wfi_item" VALUES (100467895117836288, 100461464037650432, 0, 4, 1, '测试员', '2024-07-04 13:18:05', 't', '2024-07-04 13:49:09', NULL, 1, NULL, 210, '2024-07-04 13:43:33', '2024-07-04 13:43:33');
INSERT INTO "public"."cm_wfi_item" VALUES (100422497221242880, 100422497007333376, 0, 0, 87374677803298816, '王库管', '2024-07-04 10:43:09', 't', '2024-07-04 13:59:47', NULL, 1, '按不出', 190, '2024-07-04 10:43:09', '2024-07-04 10:43:09');
INSERT INTO "public"."cm_wfi_item" VALUES (100436307264958464, 100436307248181248, 0, 0, 1, '测试员', '2024-07-04 11:38:02', 't', '2024-07-04 14:38:26', NULL, 1, NULL, 194, '2024-07-04 11:38:02', '2024-07-04 11:38:02');
INSERT INTO "public"."cm_wfi_item" VALUES (100482448794873856, 100482448593547264, 1, 1, 1, '测试员', '2024-07-04 14:41:34', 't', '2024-07-04 14:41:34', NULL, 1, NULL, 212, '2024-07-04 14:41:34', '2024-07-04 14:41:39');
INSERT INTO "public"."cm_wfi_item" VALUES (100482517304635392, 100482517124280320, 0, 0, 1, '测试员', '2024-07-04 14:41:39', 'f', NULL, NULL, 97233573971783680, NULL, 213, '2024-07-04 14:41:39', '2024-07-04 14:41:39');
INSERT INTO "public"."cm_wfi_item" VALUES (100461502621052928, 100461502608470016, 0, 0, 1, '测试员', '2024-07-04 13:18:09', 't', '2024-07-05 09:18:24', NULL, 1, NULL, 209, '2024-07-04 13:18:09', '2024-07-04 13:18:09');
INSERT INTO "public"."cm_wfi_item" VALUES (100460864403173376, 100460864222818304, 1, 0, 1, '测试员', '2024-07-04 13:15:36', 't', '2024-07-04 13:16:04', NULL, 1, NULL, 206, '2024-07-04 13:15:36', '2024-07-05 09:21:10');
INSERT INTO "public"."cm_wfi_item" VALUES (100764247152422912, 100764247085314048, 0, 2, 1, '测试员', '2024-07-05 09:21:10', 'f', NULL, NULL, 1, '没填写清除', 217, '2024-07-05 09:21:10', '2024-07-05 09:21:10');
INSERT INTO "public"."cm_wfi_item" VALUES (100460669821022208, 100460669560975360, 0, 0, 1, '测试员', '2024-07-04 13:14:50', 't', '2024-07-05 09:22:02', NULL, 1, NULL, 203, '2024-07-04 13:14:50', '2024-07-04 13:14:50');
INSERT INTO "public"."cm_wfi_item" VALUES (100437067545473024, 100437067289620480, 0, 0, 1, '测试员', '2024-07-04 11:41:03', 't', '2024-07-12 14:55:59', NULL, 1, NULL, 197, '2024-07-04 11:41:03', '2024-07-04 11:41:03');
INSERT INTO "public"."cm_wfi_item" VALUES (103392972406812672, 103392972188708864, 1, 1, 1, '测试员', '2024-07-12 15:26:54', 't', '2024-07-12 15:26:54', NULL, 1, NULL, 221, '2024-07-12 15:26:54', '2024-07-12 15:27:03');
INSERT INTO "public"."cm_wfi_item" VALUES (103393041377947648, 103393041159843840, 0, 0, 1, '测试员', '2024-07-12 15:27:03', 't', '2024-07-12 15:27:46', NULL, 1, NULL, 222, '2024-07-12 15:27:03', '2024-07-12 15:27:03');
INSERT INTO "public"."cm_wfi_item" VALUES (103401551939530752, 103401551926947840, 0, 1, 1, '测试员', '2024-07-12 16:00:55', 't', '2024-07-12 16:00:55', NULL, 1, NULL, 224, '2024-07-12 16:00:55', '2024-07-12 16:00:55');

-- ----------------------------
-- Table structure for cm_wfi_prc
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_wfi_prc";
CREATE TABLE "public"."cm_wfi_prc" (
  "id" int8 NOT NULL,
  "prcd_id" int8 NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "status" int2 NOT NULL,
  "dispidx" int4 NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_wfi_prc"."id" IS '流程实例标识，同时为业务数据主键';
COMMENT ON COLUMN "public"."cm_wfi_prc"."prcd_id" IS '流程模板标识';
COMMENT ON COLUMN "public"."cm_wfi_prc"."name" IS '流转单名称';
COMMENT ON COLUMN "public"."cm_wfi_prc"."status" IS '#WfiPrcStatus#流程实例状态 0活动 1结束 2终止';
COMMENT ON COLUMN "public"."cm_wfi_prc"."dispidx" IS '显示顺序';
COMMENT ON COLUMN "public"."cm_wfi_prc"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_wfi_prc"."mtime" IS '最后一次状态改变的时间';
COMMENT ON TABLE "public"."cm_wfi_prc" IS '流程实例';

-- ----------------------------
-- Records of cm_wfi_prc
-- ----------------------------
INSERT INTO "public"."cm_wfi_prc" VALUES (100131245460320256, 96767646822068224, '标题', 0, 69, '2024-07-03 15:25:59', '2024-07-03 15:25:59');
INSERT INTO "public"."cm_wfi_prc" VALUES (100436235504611328, 96767646822068224, '标题', 0, 70, '2024-07-04 11:37:51', '2024-07-04 11:37:51');
INSERT INTO "public"."cm_wfi_prc" VALUES (100437023215874048, 96767646822068224, '标题', 0, 71, '2024-07-04 11:40:59', '2024-07-04 11:40:59');
INSERT INTO "public"."cm_wfi_prc" VALUES (100441222339129344, 96767646822068224, '标题', 0, 72, '2024-07-04 11:57:37', '2024-07-04 11:57:37');
INSERT INTO "public"."cm_wfi_prc" VALUES (100460629845110784, 96767646822068224, '标题', 0, 73, '2024-07-04 13:14:46', '2024-07-04 13:14:46');
INSERT INTO "public"."cm_wfi_prc" VALUES (100460833314992128, 96767646822068224, '标题', 0, 74, '2024-07-04 13:15:33', '2024-07-04 13:15:33');
INSERT INTO "public"."cm_wfi_prc" VALUES (100461464012484608, 96767646822068224, '标题', 0, 75, '2024-07-04 13:18:05', '2024-07-04 13:18:05');
INSERT INTO "public"."cm_wfi_prc" VALUES (100482448404803584, 96767646822068224, '标题', 0, 76, '2024-07-04 14:41:34', '2024-07-04 14:41:34');
INSERT INTO "public"."cm_wfi_prc" VALUES (103392971937050624, 96767646822068224, '标题', 0, 77, '2024-07-12 15:26:54', '2024-07-12 15:26:54');
INSERT INTO "public"."cm_wfi_prc" VALUES (103401551914364928, 96767646822068224, '标题', 0, 78, '2024-07-12 16:00:55', '2024-07-12 16:00:55');

-- ----------------------------
-- Table structure for cm_wfi_trs
-- ----------------------------
DROP TABLE IF EXISTS "public"."cm_wfi_trs";
CREATE TABLE "public"."cm_wfi_trs" (
  "id" int8 NOT NULL,
  "trsd_id" int8 NOT NULL,
  "src_atvi_id" int8 NOT NULL,
  "tgt_atvi_id" int8 NOT NULL,
  "is_rollback" bool NOT NULL,
  "ctime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_wfi_trs"."id" IS '迁移实例标识';
COMMENT ON COLUMN "public"."cm_wfi_trs"."trsd_id" IS '迁移模板标识';
COMMENT ON COLUMN "public"."cm_wfi_trs"."src_atvi_id" IS '起始活动实例标识';
COMMENT ON COLUMN "public"."cm_wfi_trs"."tgt_atvi_id" IS '目标活动实例标识';
COMMENT ON COLUMN "public"."cm_wfi_trs"."is_rollback" IS '是否为回退迁移，1表回退';
COMMENT ON COLUMN "public"."cm_wfi_trs"."ctime" IS '迁移时间';
COMMENT ON TABLE "public"."cm_wfi_trs" IS '迁移实例';

-- ----------------------------
-- Records of cm_wfi_trs
-- ----------------------------
INSERT INTO "public"."cm_wfi_trs" VALUES (100422497766502400, 96767701062807552, 100131245670035456, 100422497007333376, 'f', '2024-07-04 10:43:09');
INSERT INTO "public"."cm_wfi_trs" VALUES (100436307336261632, 96767701062807552, 100436235529777152, 100436307248181248, 'f', '2024-07-04 11:38:02');
INSERT INTO "public"."cm_wfi_trs" VALUES (100437068124286976, 96767701062807552, 100437023425589248, 100437067289620480, 'f', '2024-07-04 11:41:03');
INSERT INTO "public"."cm_wfi_trs" VALUES (100441259525828608, 96767701062807552, 100441222624342016, 100441258733105152, 'f', '2024-07-04 11:57:42');
INSERT INTO "public"."cm_wfi_trs" VALUES (100460670395641856, 96767701062807552, 100460630063214592, 100460669560975360, 'f', '2024-07-04 13:14:50');
INSERT INTO "public"."cm_wfi_trs" VALUES (100460864885518336, 96767701062807552, 100460833491152896, 100460864222818304, 'f', '2024-07-04 13:15:36');
INSERT INTO "public"."cm_wfi_trs" VALUES (100461502675578880, 96767701062807552, 100461464037650432, 100461502608470016, 'f', '2024-07-04 13:18:09');
INSERT INTO "public"."cm_wfi_trs" VALUES (100482517837312000, 96767701062807552, 100482448593547264, 100482517124280320, 'f', '2024-07-04 14:41:39');
INSERT INTO "public"."cm_wfi_trs" VALUES (100764247131451392, 100764090209955840, 100460864222818304, 100764247085314048, 't', '2024-07-05 09:21:10');
INSERT INTO "public"."cm_wfi_trs" VALUES (103393041931595776, 96767701062807552, 103392972188708864, 103393041159843840, 'f', '2024-07-12 15:27:03');

-- ----------------------------
-- Table structure for crud_主表
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_主表";
CREATE TABLE "public"."crud_主表" (
  "id" int8 NOT NULL,
  "主表名称" varchar(255) COLLATE "pg_catalog"."default",
  "限长4" varchar(16) COLLATE "pg_catalog"."default",
  "不重复" varchar(255) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."crud_主表"."限长4" IS '限制最大长度4';
COMMENT ON COLUMN "public"."crud_主表"."不重复" IS '列值无重复';

-- ----------------------------
-- Records of crud_主表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_基础
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_基础";
CREATE TABLE "public"."crud_基础" (
  "id" int8 NOT NULL,
  "parent_id" int8,
  "序列" int4 NOT NULL,
  "名称" varchar(255) COLLATE "pg_catalog"."default",
  "限长4" varchar(16) COLLATE "pg_catalog"."default",
  "不重复" varchar(64) COLLATE "pg_catalog"."default",
  "禁止选中" bool NOT NULL,
  "禁止保存" bool NOT NULL,
  "禁止删除" bool NOT NULL,
  "值变事件" varchar(64) COLLATE "pg_catalog"."default",
  "发布插入事件" bool NOT NULL,
  "发布删除事件" bool NOT NULL,
  "创建时间" timestamp(0) NOT NULL,
  "修改时间" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."crud_基础"."id" IS '标识';
COMMENT ON COLUMN "public"."crud_基础"."parent_id" IS '上级id，演示树状结构';
COMMENT ON COLUMN "public"."crud_基础"."序列" IS '序列自动赋值';
COMMENT ON COLUMN "public"."crud_基础"."限长4" IS '限制最大长度4';
COMMENT ON COLUMN "public"."crud_基础"."不重复" IS '列值无重复';
COMMENT ON COLUMN "public"."crud_基础"."禁止选中" IS '始终为false';
COMMENT ON COLUMN "public"."crud_基础"."禁止保存" IS 'true时保存前校验不通过';
COMMENT ON COLUMN "public"."crud_基础"."禁止删除" IS 'true时删除前校验不通过';
COMMENT ON COLUMN "public"."crud_基础"."值变事件" IS '每次值变化时触发领域事件';
COMMENT ON COLUMN "public"."crud_基础"."发布插入事件" IS 'true时允许发布插入事件';
COMMENT ON COLUMN "public"."crud_基础"."发布删除事件" IS 'true时允许发布删除事件';
COMMENT ON COLUMN "public"."crud_基础"."创建时间" IS '初次创建时间';
COMMENT ON COLUMN "public"."crud_基础"."修改时间" IS '最后修改时间';

-- ----------------------------
-- Records of crud_基础
-- ----------------------------
INSERT INTO "public"."crud_基础" VALUES (95713571640733696, NULL, 12, '收到', NULL, NULL, 'f', 'f', 'f', NULL, 'f', 'f', '2024-06-21 10:51:36', '2024-06-21 10:51:36');
INSERT INTO "public"."crud_基础" VALUES (95713754986344448, NULL, 14, '对方的', NULL, NULL, 'f', 'f', 'f', NULL, 'f', 'f', '2024-06-21 10:52:20', '2024-06-21 10:52:20');

-- ----------------------------
-- Table structure for crud_大儿
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_大儿";
CREATE TABLE "public"."crud_大儿" (
  "id" int8 NOT NULL,
  "parent_id" int8 NOT NULL,
  "大儿名" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Records of crud_大儿
-- ----------------------------

-- ----------------------------
-- Table structure for crud_字段类型
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_字段类型";
CREATE TABLE "public"."crud_字段类型" (
  "id" int8 NOT NULL,
  "字符串" varchar(255) COLLATE "pg_catalog"."default",
  "整型" int4 NOT NULL,
  "可空整型" int4,
  "长整型" int8,
  "布尔" bool NOT NULL,
  "可空布尔" bool,
  "日期时间" timestamp(6) NOT NULL,
  "可空时间" timestamp(6),
  "枚举" int2 NOT NULL,
  "可空枚举" int2,
  "单精度" float4 NOT NULL,
  "可空单精度" float4
)
;
COMMENT ON COLUMN "public"."crud_字段类型"."枚举" IS '#Gender#性别';
COMMENT ON COLUMN "public"."crud_字段类型"."可空枚举" IS '#Gender#性别';

-- ----------------------------
-- Records of crud_字段类型
-- ----------------------------
INSERT INTO "public"."crud_字段类型" VALUES (95367994566537216, 'abc', 0, NULL, NULL, 'f', NULL, '-infinity', NULL, 0, NULL, 0, NULL);
INSERT INTO "public"."crud_字段类型" VALUES (95368037096779776, NULL, 23, 1, 123, 't', NULL, '2024-07-10 00:00:00', '2024-07-10 00:00:00', 1, 2, 10.2, NULL);

-- ----------------------------
-- Table structure for crud_小儿
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_小儿";
CREATE TABLE "public"."crud_小儿" (
  "id" int8 NOT NULL,
  "group_id" int8 NOT NULL,
  "小儿名" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Records of crud_小儿
-- ----------------------------

-- ----------------------------
-- Table structure for crud_扩展1
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_扩展1";
CREATE TABLE "public"."crud_扩展1" (
  "id" int8 NOT NULL,
  "扩展1名称" varchar(255) COLLATE "pg_catalog"."default",
  "禁止选中" bool NOT NULL,
  "禁止保存" bool NOT NULL
)
;
COMMENT ON COLUMN "public"."crud_扩展1"."id" IS '标识';
COMMENT ON COLUMN "public"."crud_扩展1"."禁止选中" IS '始终为false';
COMMENT ON COLUMN "public"."crud_扩展1"."禁止保存" IS 'true时保存前校验不通过';

-- ----------------------------
-- Records of crud_扩展1
-- ----------------------------

-- ----------------------------
-- Table structure for crud_扩展2
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_扩展2";
CREATE TABLE "public"."crud_扩展2" (
  "id" int8 NOT NULL,
  "扩展2名称" varchar(255) COLLATE "pg_catalog"."default",
  "禁止删除" bool NOT NULL,
  "值变事件" varchar(255) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."crud_扩展2"."id" IS '标识';
COMMENT ON COLUMN "public"."crud_扩展2"."禁止删除" IS 'true时删除前校验不通过';
COMMENT ON COLUMN "public"."crud_扩展2"."值变事件" IS '每次值变化时触发领域事件';

-- ----------------------------
-- Records of crud_扩展2
-- ----------------------------

-- ----------------------------
-- Table structure for crud_权限
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_权限";
CREATE TABLE "public"."crud_权限" (
  "id" int8 NOT NULL,
  "权限名称" varchar(255) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."crud_权限"."id" IS '权限名称';
COMMENT ON TABLE "public"."crud_权限" IS '权限';

-- ----------------------------
-- Records of crud_权限
-- ----------------------------

-- ----------------------------
-- Table structure for crud_父表
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_父表";
CREATE TABLE "public"."crud_父表" (
  "id" int8 NOT NULL,
  "父名" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Records of crud_父表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_用户
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_用户";
CREATE TABLE "public"."crud_用户" (
  "id" int8 NOT NULL,
  "手机号" char(11) COLLATE "pg_catalog"."default",
  "姓名" varchar(32) COLLATE "pg_catalog"."default",
  "密码" char(32) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."crud_用户"."id" IS '用户标识';
COMMENT ON COLUMN "public"."crud_用户"."手机号" IS '手机号，唯一';
COMMENT ON COLUMN "public"."crud_用户"."姓名" IS '姓名';
COMMENT ON COLUMN "public"."crud_用户"."密码" IS '密码的md5';
COMMENT ON TABLE "public"."crud_用户" IS '系统用户';

-- ----------------------------
-- Records of crud_用户
-- ----------------------------

-- ----------------------------
-- Table structure for crud_用户角色
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_用户角色";
CREATE TABLE "public"."crud_用户角色" (
  "user_id" int8 NOT NULL,
  "role_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."crud_用户角色"."user_id" IS '用户标识';
COMMENT ON COLUMN "public"."crud_用户角色"."role_id" IS '角色标识';
COMMENT ON TABLE "public"."crud_用户角色" IS '用户关联的角色';

-- ----------------------------
-- Records of crud_用户角色
-- ----------------------------

-- ----------------------------
-- Table structure for crud_缓存表
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_缓存表";
CREATE TABLE "public"."crud_缓存表" (
  "id" int8 NOT NULL,
  "手机号" char(11) COLLATE "pg_catalog"."default",
  "姓名" varchar(32) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Records of crud_缓存表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_角色
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_角色";
CREATE TABLE "public"."crud_角色" (
  "id" int8 NOT NULL,
  "角色名称" varchar(32) COLLATE "pg_catalog"."default",
  "角色描述" varchar(255) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."crud_角色"."id" IS '角色标识';
COMMENT ON COLUMN "public"."crud_角色"."角色名称" IS '角色名称';
COMMENT ON COLUMN "public"."crud_角色"."角色描述" IS '角色描述';
COMMENT ON TABLE "public"."crud_角色" IS '角色';

-- ----------------------------
-- Records of crud_角色
-- ----------------------------

-- ----------------------------
-- Table structure for crud_角色权限
-- ----------------------------
DROP TABLE IF EXISTS "public"."crud_角色权限";
CREATE TABLE "public"."crud_角色权限" (
  "role_id" int8 NOT NULL,
  "prv_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."crud_角色权限"."role_id" IS '角色标识';
COMMENT ON COLUMN "public"."crud_角色权限"."prv_id" IS '权限标识';
COMMENT ON TABLE "public"."crud_角色权限" IS '角色关联的权限';

-- ----------------------------
-- Records of crud_角色权限
-- ----------------------------

-- ----------------------------
-- Table structure for fsm_file
-- ----------------------------
DROP TABLE IF EXISTS "public"."fsm_file";
CREATE TABLE "public"."fsm_file" (
  "id" int8 NOT NULL,
  "name" varchar(512) COLLATE "pg_catalog"."default" NOT NULL,
  "path" varchar(512) COLLATE "pg_catalog"."default" NOT NULL,
  "size" int8 NOT NULL,
  "info" varchar(512) COLLATE "pg_catalog"."default",
  "uploader" int8 NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "downloads" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."fsm_file"."id" IS '文件标识';
COMMENT ON COLUMN "public"."fsm_file"."name" IS '文件名称';
COMMENT ON COLUMN "public"."fsm_file"."path" IS '存放路径：卷/两级目录/id.ext';
COMMENT ON COLUMN "public"."fsm_file"."size" IS '文件长度';
COMMENT ON COLUMN "public"."fsm_file"."info" IS '文件描述';
COMMENT ON COLUMN "public"."fsm_file"."uploader" IS '上传人id';
COMMENT ON COLUMN "public"."fsm_file"."ctime" IS '上传时间';
COMMENT ON COLUMN "public"."fsm_file"."downloads" IS '下载次数';

-- ----------------------------
-- Records of fsm_file
-- ----------------------------

-- ----------------------------
-- Table structure for 人员
-- ----------------------------
DROP TABLE IF EXISTS "public"."人员";
CREATE TABLE "public"."人员" (
  "id" int8 NOT NULL,
  "姓名" varchar(32) COLLATE "pg_catalog"."default",
  "出生日期" timestamp(0),
  "性别" int2,
  "工作日期" timestamp(0),
  "办公室电话" varchar(32) COLLATE "pg_catalog"."default",
  "电子邮件" varchar(32) COLLATE "pg_catalog"."default",
  "建档时间" timestamp(0),
  "撤档时间" timestamp(0),
  "撤档原因" varchar(64) COLLATE "pg_catalog"."default",
  "user_id" int8
)
;
COMMENT ON COLUMN "public"."人员"."性别" IS '#Gender#';
COMMENT ON COLUMN "public"."人员"."user_id" IS '账号ID';

-- ----------------------------
-- Records of 人员
-- ----------------------------
INSERT INTO "public"."人员" VALUES (93233663974862848, '王库管', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 87374677803298816);
INSERT INTO "public"."人员" VALUES (93233694710722560, '张主管', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 87375101197316096);
INSERT INTO "public"."人员" VALUES (100436029211963392, '测试员', NULL, NULL, NULL, NULL, NULL, '2024-07-04 11:37:09', NULL, NULL, 1);

-- ----------------------------
-- Table structure for 供应商
-- ----------------------------
DROP TABLE IF EXISTS "public"."供应商";
CREATE TABLE "public"."供应商" (
  "id" int8 NOT NULL,
  "名称" varchar(64) COLLATE "pg_catalog"."default",
  "执照号" varchar(32) COLLATE "pg_catalog"."default",
  "执照效期" timestamp(0),
  "税务登记号" varchar(32) COLLATE "pg_catalog"."default",
  "地址" varchar(128) COLLATE "pg_catalog"."default",
  "电话" varchar(16) COLLATE "pg_catalog"."default",
  "开户银行" varchar(64) COLLATE "pg_catalog"."default",
  "帐号" varchar(32) COLLATE "pg_catalog"."default",
  "联系人" varchar(32) COLLATE "pg_catalog"."default",
  "建档时间" timestamp(0),
  "撤档时间" timestamp(0),
  "备注" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Records of 供应商
-- ----------------------------
INSERT INTO "public"."供应商" VALUES (95034724012290048, '物资东厂', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2024-06-19 13:54:37', NULL, NULL);
INSERT INTO "public"."供应商" VALUES (95034312534290432, '仁和制药', NULL, NULL, NULL, NULL, '13698562456', NULL, NULL, NULL, '2024-06-19 13:53:52', NULL, NULL);

-- ----------------------------
-- Table structure for 物资主单
-- ----------------------------
DROP TABLE IF EXISTS "public"."物资主单";
CREATE TABLE "public"."物资主单" (
  "id" int8 NOT NULL,
  "部门id" int8 NOT NULL,
  "入出类别" int2 NOT NULL,
  "状态" int2 NOT NULL,
  "单号" varchar(8) COLLATE "pg_catalog"."default" NOT NULL,
  "摘要" varchar(64) COLLATE "pg_catalog"."default",
  "填制人" varchar(32) COLLATE "pg_catalog"."default",
  "填制日期" timestamp(0),
  "审核人" varchar(32) COLLATE "pg_catalog"."default",
  "审核日期" timestamp(0),
  "入出系数" int2,
  "供应商id" int8,
  "发料人" varchar(32) COLLATE "pg_catalog"."default",
  "发料日期" timestamp(0),
  "金额" float4,
  "发票金额" float4
)
;
COMMENT ON COLUMN "public"."物资主单"."入出类别" IS '#入出类别# 小于10为入库类型，大于等于10为出库类型';
COMMENT ON COLUMN "public"."物资主单"."状态" IS '#单据状态#0-填写;1-待审核;2-已审核;3-被冲销;4-冲销';
COMMENT ON COLUMN "public"."物资主单"."单号" IS '相同单号可以不同的冲销状态，命名：前缀+连续序号';
COMMENT ON COLUMN "public"."物资主单"."填制人" IS '如果是申领单，表示申领人';
COMMENT ON COLUMN "public"."物资主单"."入出系数" IS '1:物资入,-1:物资出;0-盘点记录单';
COMMENT ON COLUMN "public"."物资主单"."供应商id" IS '外购入库时填写';
COMMENT ON COLUMN "public"."物资主单"."发料人" IS '申请单时用效,主要反应该张单据什么人发的料';
COMMENT ON COLUMN "public"."物资主单"."发料日期" IS '申请单时用效';
COMMENT ON COLUMN "public"."物资主单"."金额" IS '单据内所有详单的金额和';

-- ----------------------------
-- Records of 物资主单
-- ----------------------------

-- ----------------------------
-- Table structure for 物资分类
-- ----------------------------
DROP TABLE IF EXISTS "public"."物资分类";
CREATE TABLE "public"."物资分类" (
  "id" int8 NOT NULL,
  "名称" varchar(64) COLLATE "pg_catalog"."default" NOT NULL
)
;

-- ----------------------------
-- Records of 物资分类
-- ----------------------------
INSERT INTO "public"."物资分类" VALUES (95413444640272384, '电工材料');
INSERT INTO "public"."物资分类" VALUES (95419313314623488, '劳保材料');
INSERT INTO "public"."物资分类" VALUES (95419350320967680, '水暖材料');
INSERT INTO "public"."物资分类" VALUES (95419395929829376, '维修材料');
INSERT INTO "public"."物资分类" VALUES (95419431795322880, '办公材料');
INSERT INTO "public"."物资分类" VALUES (95419477521625088, '低值易耗');
INSERT INTO "public"."物资分类" VALUES (95419514808987648, '易耗材料');
INSERT INTO "public"."物资分类" VALUES (95419598749593600, '其他材料');

-- ----------------------------
-- Table structure for 物资库存
-- ----------------------------
DROP TABLE IF EXISTS "public"."物资库存";
CREATE TABLE "public"."物资库存" (
  "id" int8 NOT NULL,
  "部门id" int8,
  "物资id" int8,
  "批次" varchar(16) COLLATE "pg_catalog"."default",
  "可用数量" float4,
  "可用金额" float4,
  "实际数量" float4,
  "实际金额" float4
)
;
COMMENT ON COLUMN "public"."物资库存"."批次" IS '相同物资ID不同批次的物资独立计算库存，部门ID+物资ID+批次构成唯一索引';
COMMENT ON COLUMN "public"."物资库存"."可用数量" IS '当填写申领单还未审批时只影响可用数量，确保后填写申领单时数量有效';

-- ----------------------------
-- Records of 物资库存
-- ----------------------------

-- ----------------------------
-- Table structure for 物资目录
-- ----------------------------
DROP TABLE IF EXISTS "public"."物资目录";
CREATE TABLE "public"."物资目录" (
  "id" int8 NOT NULL,
  "分类id" int8,
  "名称" varchar(64) COLLATE "pg_catalog"."default",
  "规格" varchar(64) COLLATE "pg_catalog"."default",
  "产地" varchar(64) COLLATE "pg_catalog"."default",
  "成本价" float4,
  "核算方式" int2,
  "摊销月数" int4,
  "建档时间" timestamp(0),
  "撤档时间" timestamp(0)
)
;
COMMENT ON COLUMN "public"."物资目录"."规格" IS '计量单位，如 盒、10个/包、20个/箱、支';
COMMENT ON COLUMN "public"."物资目录"."产地" IS '名称,规格,产地构成唯一索引';
COMMENT ON COLUMN "public"."物资目录"."成本价" IS '预估价格，库存计算金额用';
COMMENT ON COLUMN "public"."物资目录"."核算方式" IS '#物资核算方式#一次性、分期摊销(折旧)';
COMMENT ON COLUMN "public"."物资目录"."摊销月数" IS '当核算方式为分期摊销时的总月数';

-- ----------------------------
-- Records of 物资目录
-- ----------------------------
INSERT INTO "public"."物资目录" VALUES (95434428013375488, 95413444640272384, '测电笔', '只', NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95437661570101248, 95413444640272384, '啊手动阀', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95437943343443968, 95419313314623488, '阿斯顿', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95438090697732096, 95413444640272384, '122', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95438249615716352, 95413444640272384, '所属', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95438724352208896, 95419313314623488, '撒地方', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95439002132574208, 95413444640272384, '55', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95439468451098624, 95413444640272384, '不不不', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95439917585559552, 95419350320967680, '也一样', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95440261258440704, 95419313314623488, '引用', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95440723634319360, 95419350320967680, '23', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95440891901407232, 95419313314623488, '阿斯顿', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95678260751659008, 95419395929829376, '234', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (95779076258951168, 95413444640272384, '啊手动阀', NULL, NULL, NULL, NULL, NULL, '2024-06-21 15:12:07', NULL);
INSERT INTO "public"."物资目录" VALUES (95779372557168640, 95413444640272384, '234234', '的方式', NULL, NULL, NULL, NULL, '2024-06-21 15:13:11', NULL);
INSERT INTO "public"."物资目录" VALUES (95779399979528192, 95413444640272384, '不不不', NULL, NULL, NULL, NULL, NULL, '2024-06-21 15:13:18', NULL);
INSERT INTO "public"."物资目录" VALUES (95439719358558208, 95419313314623488, '烦烦烦333', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO "public"."物资目录" VALUES (104839509410344960, 95413444640272384, '电线', '米', NULL, NULL, NULL, NULL, '2024-07-16 15:15:05', NULL);

-- ----------------------------
-- Table structure for 物资计划
-- ----------------------------
DROP TABLE IF EXISTS "public"."物资计划";
CREATE TABLE "public"."物资计划" (
  "id" int8 NOT NULL,
  "部门id" int8,
  "no" varchar(8) COLLATE "pg_catalog"."default",
  "计划类型" int2,
  "编制方法" int2,
  "摘要" varchar(64) COLLATE "pg_catalog"."default",
  "编制人" varchar(32) COLLATE "pg_catalog"."default",
  "编制日期" timestamp(0),
  "审核人" varchar(32) COLLATE "pg_catalog"."default",
  "审核日期" timestamp(0)
)
;
COMMENT ON COLUMN "public"."物资计划"."计划类型" IS '#计划类型#月;季;年';
COMMENT ON COLUMN "public"."物资计划"."编制方法" IS '#计划编制方法#1-往年同期线性参照法,2-临近期间平均参照法,3-物资储备定额参照法,4-由部门申购计划产生';

-- ----------------------------
-- Records of 物资计划
-- ----------------------------

-- ----------------------------
-- Table structure for 物资计划明细
-- ----------------------------
DROP TABLE IF EXISTS "public"."物资计划明细";
CREATE TABLE "public"."物资计划明细" (
  "计划id" int8 NOT NULL,
  "物资id" int8 NOT NULL,
  "前期数量" float4,
  "上期数量" float4,
  "库存数量" float4,
  "计划数量" float4,
  "审批数量" float4,
  "单价" float4,
  "金额" float4,
  "显示顺序" int4
)
;
COMMENT ON COLUMN "public"."物资计划明细"."前期数量" IS '前年、上上月、前季度数量';
COMMENT ON COLUMN "public"."物资计划明细"."上期数量" IS '去年、上个月、上季度数量';

-- ----------------------------
-- Records of 物资计划明细
-- ----------------------------

-- ----------------------------
-- Table structure for 物资详单
-- ----------------------------
DROP TABLE IF EXISTS "public"."物资详单";
CREATE TABLE "public"."物资详单" (
  "id" int8 NOT NULL,
  "单据id" int8 NOT NULL,
  "物资id" int8,
  "序号" int2,
  "批次" varchar(16) COLLATE "pg_catalog"."default",
  "数量" float4,
  "单价" float4,
  "金额" float4,
  "产地" varchar(64) COLLATE "pg_catalog"."default",
  "随货单号" varchar(128) COLLATE "pg_catalog"."default",
  "发票号" varchar(128) COLLATE "pg_catalog"."default",
  "发票日期" timestamp(0),
  "发票金额" float4,
  "盘点时间" timestamp(0),
  "盘点金额" float4,
  "物资名称" varchar(64) COLLATE "pg_catalog"."default"
)
;
COMMENT ON COLUMN "public"."物资详单"."序号" IS '在一张单据内部从1连续编号，入出类别+冲销状态+单号+序号共同构成唯一索引';
COMMENT ON COLUMN "public"."物资详单"."数量" IS '按散装单位填写';
COMMENT ON COLUMN "public"."物资详单"."单价" IS '售价';
COMMENT ON COLUMN "public"."物资详单"."金额" IS '实际数量与单价的乘积。';
COMMENT ON COLUMN "public"."物资详单"."随货单号" IS '外购入库时填写';
COMMENT ON COLUMN "public"."物资详单"."发票号" IS '外购入库时填写';
COMMENT ON COLUMN "public"."物资详单"."发票日期" IS '外购入库时填写';
COMMENT ON COLUMN "public"."物资详单"."发票金额" IS '外购入库时填写';
COMMENT ON COLUMN "public"."物资详单"."盘点时间" IS '盘点有效';
COMMENT ON COLUMN "public"."物资详单"."盘点金额" IS '盘点有效';
COMMENT ON COLUMN "public"."物资详单"."物资名称" IS '物资名称，以物资id为准，冗余';

-- ----------------------------
-- Records of 物资详单
-- ----------------------------

-- ----------------------------
-- Table structure for 部门
-- ----------------------------
DROP TABLE IF EXISTS "public"."部门";
CREATE TABLE "public"."部门" (
  "id" int8 NOT NULL,
  "上级id" int8,
  "编码" varchar(16) COLLATE "pg_catalog"."default",
  "名称" varchar(32) COLLATE "pg_catalog"."default",
  "说明" varchar(64) COLLATE "pg_catalog"."default",
  "建档时间" timestamp(0),
  "撤档时间" timestamp(0)
)
;
COMMENT ON COLUMN "public"."部门"."说明" IS '位置、环境、备注等';

-- ----------------------------
-- Records of 部门
-- ----------------------------
INSERT INTO "public"."部门" VALUES (93173171340210176, NULL, '01', '设备科', NULL, '2024-06-14 10:37:22', NULL);
INSERT INTO "public"."部门" VALUES (93173345370271744, NULL, '02', '物资科', NULL, '2024-06-14 10:37:41', NULL);
INSERT INTO "public"."部门" VALUES (93174118862843904, NULL, '03', '财务科', NULL, '2024-06-14 10:40:52', NULL);

-- ----------------------------
-- Table structure for 部门人员
-- ----------------------------
DROP TABLE IF EXISTS "public"."部门人员";
CREATE TABLE "public"."部门人员" (
  "部门id" int8 NOT NULL,
  "人员id" int8 NOT NULL,
  "缺省" bool
)
;
COMMENT ON COLUMN "public"."部门人员"."缺省" IS '当一个人员属于多个部门时，当前是否为缺省';

-- ----------------------------
-- Records of 部门人员
-- ----------------------------
INSERT INTO "public"."部门人员" VALUES (93173345370271744, 93233663974862848, 't');
INSERT INTO "public"."部门人员" VALUES (93173345370271744, 100436029211963392, 't');

-- ----------------------------
-- View structure for v_物资目录
-- ----------------------------
DROP VIEW IF EXISTS "public"."v_物资目录";
CREATE VIEW "public"."v_物资目录" AS  SELECT a.id,
    a."分类id",
    a."名称",
    a."规格",
    a."产地",
    a."成本价",
    a."核算方式",
    a."摊销月数",
    a."建档时间",
    a."撤档时间",
    b."名称" AS "物资分类"
   FROM "物资目录" a
     LEFT JOIN "物资分类" b ON a."分类id" = b.id;

-- ----------------------------
-- View structure for v_人员
-- ----------------------------
DROP VIEW IF EXISTS "public"."v_人员";
CREATE VIEW "public"."v_人员" AS  SELECT a.id,
    a."姓名",
    a."出生日期",
    a."性别",
    a."工作日期",
    a."办公室电话",
    a."电子邮件",
    a."建档时间",
    a."撤档时间",
    a."撤档原因",
    a.user_id,
    COALESCE(NULLIF(b.name::text, ''::text), NULLIF(b.acc::text, ''::text), NULLIF(b.phone::text, ''::text)) AS "账号"
   FROM "人员" a
     LEFT JOIN cm_user b ON a.user_id = b.id;

-- ----------------------------
-- View structure for v_物资主单
-- ----------------------------
DROP VIEW IF EXISTS "public"."v_物资主单";
CREATE VIEW "public"."v_物资主单" AS  SELECT a.id,
    a."部门id",
    a."入出类别",
    a."状态",
    a."单号",
    a."摘要",
    a."填制人",
    a."填制日期",
    a."审核人",
    a."审核日期",
    a."入出系数",
    a."供应商id",
    a."发料人",
    a."发料日期",
    a."金额",
    a."发票金额",
    b."名称" AS "部门名称",
    c."名称" AS "供应商"
   FROM "物资主单" a
     LEFT JOIN "部门" b ON a."部门id" = b.id
     LEFT JOIN "供应商" c ON a."供应商id" = c.id;

-- ----------------------------
-- View structure for v_部门
-- ----------------------------
DROP VIEW IF EXISTS "public"."v_部门";
CREATE VIEW "public"."v_部门" AS  SELECT a.id,
    a."上级id",
    a."编码",
    a."名称",
    a."说明",
    a."建档时间",
    a."撤档时间",
    b."名称" AS "上级部门"
   FROM "部门" a
     LEFT JOIN "部门" b ON a."上级id" = b.id;

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
SELECT setval('"public"."cm_menu_dispidx"', 137, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
SELECT setval('"public"."cm_option_dispidx"', 1050, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
SELECT setval('"public"."cm_wfd_prc_dispidx"', 15, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
SELECT setval('"public"."cm_wfi_item_dispidx"', 226, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
SELECT setval('"public"."cm_wfi_prc_dispidx"', 78, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
SELECT setval('"public"."crud_基础_序列"', 85, true);

-- ----------------------------
-- Primary Key structure for table cm_cache
-- ----------------------------
ALTER TABLE "public"."cm_cache" ADD CONSTRAINT "cm_cache_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_file_my
-- ----------------------------
CREATE INDEX "idx_file_my_parentid" ON "public"."cm_file_my" USING btree (
  "parent_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_file_my_userid" ON "public"."cm_file_my" USING btree (
  "user_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_file_my
-- ----------------------------
ALTER TABLE "public"."cm_file_my" ADD CONSTRAINT "cm_file_my_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_file_pub
-- ----------------------------
CREATE INDEX "idx_file_pub_parentid" ON "public"."cm_file_pub" USING btree (
  "parent_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_file_pub
-- ----------------------------
ALTER TABLE "public"."cm_file_pub" ADD CONSTRAINT "cm_file_pub_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_group
-- ----------------------------
CREATE UNIQUE INDEX "idx_group_name" ON "public"."cm_group" USING btree (
  "name" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);
COMMENT ON INDEX "public"."idx_group_name" IS '不重复';

-- ----------------------------
-- Primary Key structure for table cm_group
-- ----------------------------
ALTER TABLE "public"."cm_group" ADD CONSTRAINT "cm_group_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_group_role
-- ----------------------------
CREATE INDEX "idx_group_role_groupid" ON "public"."cm_group_role" USING btree (
  "group_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_group_role_roleid" ON "public"."cm_group_role" USING btree (
  "role_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_group_role
-- ----------------------------
ALTER TABLE "public"."cm_group_role" ADD CONSTRAINT "cm_group_role_pkey" PRIMARY KEY ("group_id", "role_id");

-- ----------------------------
-- Indexes structure for table cm_menu
-- ----------------------------
CREATE INDEX "idx_menu_parentid" ON "public"."cm_menu" USING btree (
  "parent_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_menu
-- ----------------------------
ALTER TABLE "public"."cm_menu" ADD CONSTRAINT "cm_menu_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_option
-- ----------------------------
CREATE INDEX "idx_option_groupid" ON "public"."cm_option" USING btree (
  "group_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_option
-- ----------------------------
ALTER TABLE "public"."cm_option" ADD CONSTRAINT "cm_option_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table cm_option_group
-- ----------------------------
ALTER TABLE "public"."cm_option_group" ADD CONSTRAINT "cm_option_group_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_params
-- ----------------------------
CREATE UNIQUE INDEX "idx_params_name" ON "public"."cm_params" USING btree (
  "name" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_params
-- ----------------------------
ALTER TABLE "public"."cm_params" ADD CONSTRAINT "cm_params_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_permission
-- ----------------------------
CREATE INDEX "fk_permission" ON "public"."cm_permission" USING btree (
  "func_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Uniques structure for table cm_permission
-- ----------------------------
ALTER TABLE "public"."cm_permission" ADD CONSTRAINT "uq_permission" UNIQUE ("func_id", "name");

-- ----------------------------
-- Primary Key structure for table cm_permission
-- ----------------------------
ALTER TABLE "public"."cm_permission" ADD CONSTRAINT "cm_permission_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_permission_func
-- ----------------------------
CREATE INDEX "fk_permission_func" ON "public"."cm_permission_func" USING btree (
  "module_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Uniques structure for table cm_permission_func
-- ----------------------------
ALTER TABLE "public"."cm_permission_func" ADD CONSTRAINT "uq_permission_func" UNIQUE ("module_id", "name");

-- ----------------------------
-- Primary Key structure for table cm_permission_func
-- ----------------------------
ALTER TABLE "public"."cm_permission_func" ADD CONSTRAINT "cm_permission_func_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table cm_permission_module
-- ----------------------------
ALTER TABLE "public"."cm_permission_module" ADD CONSTRAINT "cm_permission_module_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_role
-- ----------------------------
CREATE UNIQUE INDEX "idx_role_name" ON "public"."cm_role" USING btree (
  "name" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);
COMMENT ON INDEX "public"."idx_role_name" IS '不重复';

-- ----------------------------
-- Primary Key structure for table cm_role
-- ----------------------------
ALTER TABLE "public"."cm_role" ADD CONSTRAINT "cm_role_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_role_menu
-- ----------------------------
CREATE INDEX "idx_role_menu_menuid" ON "public"."cm_role_menu" USING btree (
  "menu_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_role_menu_roleid" ON "public"."cm_role_menu" USING btree (
  "role_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_role_menu
-- ----------------------------
ALTER TABLE "public"."cm_role_menu" ADD CONSTRAINT "cm_role_menu_pkey" PRIMARY KEY ("role_id", "menu_id");

-- ----------------------------
-- Indexes structure for table cm_role_per
-- ----------------------------
CREATE INDEX "idx_role_per_perid" ON "public"."cm_role_per" USING btree (
  "per_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_role_per_roleid" ON "public"."cm_role_per" USING btree (
  "role_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_role_per
-- ----------------------------
ALTER TABLE "public"."cm_role_per" ADD CONSTRAINT "cm_role_per_pkey" PRIMARY KEY ("role_id", "per_id");

-- ----------------------------
-- Indexes structure for table cm_rpt
-- ----------------------------
CREATE UNIQUE INDEX "idx_rpt_name" ON "public"."cm_rpt" USING btree (
  "name" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_rpt
-- ----------------------------
ALTER TABLE "public"."cm_rpt" ADD CONSTRAINT "cm_rpt_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_user
-- ----------------------------
CREATE INDEX "idx_user_acc" ON "public"."cm_user" USING btree (
  "acc" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);
CREATE INDEX "idx_user_phone" ON "public"."cm_user" USING btree (
  "phone" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_user
-- ----------------------------
ALTER TABLE "public"."cm_user" ADD CONSTRAINT "cm_user_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_user_group
-- ----------------------------
CREATE INDEX "idx_user_group_groupid" ON "public"."cm_user_group" USING btree (
  "group_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_user_group_userid" ON "public"."cm_user_group" USING btree (
  "user_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_user_group
-- ----------------------------
ALTER TABLE "public"."cm_user_group" ADD CONSTRAINT "cm_user_group_pkey" PRIMARY KEY ("user_id", "group_id");

-- ----------------------------
-- Indexes structure for table cm_user_params
-- ----------------------------
CREATE INDEX "idx_user_params_paramsid" ON "public"."cm_user_params" USING btree (
  "param_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_user_params_userid" ON "public"."cm_user_params" USING btree (
  "user_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_user_params
-- ----------------------------
ALTER TABLE "public"."cm_user_params" ADD CONSTRAINT "cm_user_params_pkey" PRIMARY KEY ("user_id", "param_id");

-- ----------------------------
-- Indexes structure for table cm_user_role
-- ----------------------------
CREATE INDEX "idx_user_role_roleid" ON "public"."cm_user_role" USING btree (
  "role_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_user_role_userid" ON "public"."cm_user_role" USING btree (
  "user_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_user_role
-- ----------------------------
ALTER TABLE "public"."cm_user_role" ADD CONSTRAINT "cm_user_role_pkey" PRIMARY KEY ("user_id", "role_id");

-- ----------------------------
-- Indexes structure for table cm_wfd_atv
-- ----------------------------
CREATE INDEX "idx_wfd_atv_prcid" ON "public"."cm_wfd_atv" USING btree (
  "prc_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_wfd_atv
-- ----------------------------
ALTER TABLE "public"."cm_wfd_atv" ADD CONSTRAINT "cm_wfd_atv_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfd_atv_role
-- ----------------------------
CREATE INDEX "idx_wfd_atv_role_roleid" ON "public"."cm_wfd_atv_role" USING btree (
  "role_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_wfd_atv_role
-- ----------------------------
ALTER TABLE "public"."cm_wfd_atv_role" ADD CONSTRAINT "cm_wfd_atv_role_pkey" PRIMARY KEY ("atv_id", "role_id");

-- ----------------------------
-- Primary Key structure for table cm_wfd_prc
-- ----------------------------
ALTER TABLE "public"."cm_wfd_prc" ADD CONSTRAINT "cm_wfd_prc_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfd_trs
-- ----------------------------
CREATE INDEX "idx_wfd_trs_prcid" ON "public"."cm_wfd_trs" USING btree (
  "prc_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_wfd_trs
-- ----------------------------
ALTER TABLE "public"."cm_wfd_trs" ADD CONSTRAINT "cm_wfd_trs_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfi_atv
-- ----------------------------
CREATE INDEX "idx_wfi_atv_atvdid" ON "public"."cm_wfi_atv" USING btree (
  "atvd_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_wfi_atv_prciid" ON "public"."cm_wfi_atv" USING btree (
  "prci_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_wfi_atv
-- ----------------------------
ALTER TABLE "public"."cm_wfi_atv" ADD CONSTRAINT "cm_wfi_atv_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfi_item
-- ----------------------------
CREATE INDEX "idx_wfi_item_atviid" ON "public"."cm_wfi_item" USING btree (
  "atvi_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_wfi_item
-- ----------------------------
ALTER TABLE "public"."cm_wfi_item" ADD CONSTRAINT "cm_wfi_item_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfi_prc
-- ----------------------------
CREATE INDEX "idx_wfi_prc_prcdid" ON "public"."cm_wfi_prc" USING btree (
  "prcd_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_wfi_prc
-- ----------------------------
ALTER TABLE "public"."cm_wfi_prc" ADD CONSTRAINT "cm_wfi_prc_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfi_trs
-- ----------------------------
CREATE INDEX "idx_wfi_trs_srcatviid" ON "public"."cm_wfi_trs" USING btree (
  "src_atvi_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_wfi_trs_tgtatviid" ON "public"."cm_wfi_trs" USING btree (
  "tgt_atvi_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_wfi_trs_trsdid" ON "public"."cm_wfi_trs" USING btree (
  "trsd_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table cm_wfi_trs
-- ----------------------------
ALTER TABLE "public"."cm_wfi_trs" ADD CONSTRAINT "cm_wfi_trs_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table crud_主表
-- ----------------------------
ALTER TABLE "public"."crud_主表" ADD CONSTRAINT "crud_主表_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table crud_基础
-- ----------------------------
ALTER TABLE "public"."crud_基础" ADD CONSTRAINT "crud_基础_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table crud_大儿
-- ----------------------------
CREATE INDEX "idx_大儿_parendid" ON "public"."crud_大儿" USING btree (
  "parent_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table crud_大儿
-- ----------------------------
ALTER TABLE "public"."crud_大儿" ADD CONSTRAINT "crud_大儿_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table crud_字段类型
-- ----------------------------
ALTER TABLE "public"."crud_字段类型" ADD CONSTRAINT "crud_字段类型_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table crud_小儿
-- ----------------------------
CREATE INDEX "idx_小儿_parentid" ON "public"."crud_小儿" USING btree (
  "group_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table crud_小儿
-- ----------------------------
ALTER TABLE "public"."crud_小儿" ADD CONSTRAINT "crud_小儿_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table crud_扩展1
-- ----------------------------
ALTER TABLE "public"."crud_扩展1" ADD CONSTRAINT "crud_扩展1_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table crud_扩展2
-- ----------------------------
ALTER TABLE "public"."crud_扩展2" ADD CONSTRAINT "crud_扩展2_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table crud_权限
-- ----------------------------
ALTER TABLE "public"."crud_权限" ADD CONSTRAINT "crud_权限_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table crud_父表
-- ----------------------------
ALTER TABLE "public"."crud_父表" ADD CONSTRAINT "crud_父表_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table crud_用户
-- ----------------------------
ALTER TABLE "public"."crud_用户" ADD CONSTRAINT "crud_用户_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table crud_用户角色
-- ----------------------------
CREATE INDEX "idx_crud_用户角色_roleid" ON "public"."crud_用户角色" USING btree (
  "role_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_crud_用户角色_userid" ON "public"."crud_用户角色" USING btree (
  "user_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table crud_用户角色
-- ----------------------------
ALTER TABLE "public"."crud_用户角色" ADD CONSTRAINT "crud_用户角色_pkey" PRIMARY KEY ("user_id", "role_id");

-- ----------------------------
-- Primary Key structure for table crud_缓存表
-- ----------------------------
ALTER TABLE "public"."crud_缓存表" ADD CONSTRAINT "crud_缓存表_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table crud_角色
-- ----------------------------
ALTER TABLE "public"."crud_角色" ADD CONSTRAINT "crud_角色_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table crud_角色权限
-- ----------------------------
CREATE INDEX "idx_crud_角色权限_prvid" ON "public"."crud_角色权限" USING btree (
  "prv_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "idx_crud_角色权限_roleid" ON "public"."crud_角色权限" USING btree (
  "role_id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table crud_角色权限
-- ----------------------------
ALTER TABLE "public"."crud_角色权限" ADD CONSTRAINT "crud_角色权限_pkey" PRIMARY KEY ("role_id", "prv_id");

-- ----------------------------
-- Indexes structure for table fsm_file
-- ----------------------------
CREATE UNIQUE INDEX "idx_fsm_file_path" ON "public"."fsm_file" USING btree (
  "path" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table fsm_file
-- ----------------------------
ALTER TABLE "public"."fsm_file" ADD CONSTRAINT "fsm_file_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table 人员
-- ----------------------------
ALTER TABLE "public"."人员" ADD CONSTRAINT "人员_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table 供应商
-- ----------------------------
ALTER TABLE "public"."供应商" ADD CONSTRAINT "pk_供应商" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table 物资主单
-- ----------------------------
ALTER TABLE "public"."物资主单" ADD CONSTRAINT "pk_物资主单" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table 物资分类
-- ----------------------------
ALTER TABLE "public"."物资分类" ADD CONSTRAINT "pk_物资分类" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table 物资库存
-- ----------------------------
CREATE INDEX "ix_物资库存_物资id" ON "public"."物资库存" USING btree (
  "物资id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "ix_物资库存_部门id" ON "public"."物资库存" USING btree (
  "部门id" "pg_catalog"."int8_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table 物资库存
-- ----------------------------
ALTER TABLE "public"."物资库存" ADD CONSTRAINT "pk_物资库存" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table 物资目录
-- ----------------------------
ALTER TABLE "public"."物资目录" ADD CONSTRAINT "pk_物资目录" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table 物资计划
-- ----------------------------
ALTER TABLE "public"."物资计划" ADD CONSTRAINT "pk_物资计划" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table 物资计划明细
-- ----------------------------
ALTER TABLE "public"."物资计划明细" ADD CONSTRAINT "pk_物资计划明细" PRIMARY KEY ("计划id", "物资id");

-- ----------------------------
-- Primary Key structure for table 物资详单
-- ----------------------------
ALTER TABLE "public"."物资详单" ADD CONSTRAINT "pk_物资详单" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table 部门
-- ----------------------------
ALTER TABLE "public"."部门" ADD CONSTRAINT "部门_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table 部门人员
-- ----------------------------
ALTER TABLE "public"."部门人员" ADD CONSTRAINT "部门人员_pkey" PRIMARY KEY ("部门id", "人员id");

-- ----------------------------
-- Foreign Keys structure for table cm_file_my
-- ----------------------------
ALTER TABLE "public"."cm_file_my" ADD CONSTRAINT "fk_file_my_parentid" FOREIGN KEY ("parent_id") REFERENCES "public"."cm_file_my" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_file_my" ADD CONSTRAINT "fk_file_my_userid" FOREIGN KEY ("user_id") REFERENCES "public"."cm_user" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_file_pub
-- ----------------------------
ALTER TABLE "public"."cm_file_pub" ADD CONSTRAINT "fk_file_pub_parentid" FOREIGN KEY ("parent_id") REFERENCES "public"."cm_file_pub" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_group_role
-- ----------------------------
ALTER TABLE "public"."cm_group_role" ADD CONSTRAINT "fk_group_role_groupid" FOREIGN KEY ("group_id") REFERENCES "public"."cm_group" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_group_role" ADD CONSTRAINT "fk_group_role_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."cm_role" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_menu
-- ----------------------------
ALTER TABLE "public"."cm_menu" ADD CONSTRAINT "fk_menu_parentid" FOREIGN KEY ("parent_id") REFERENCES "public"."cm_menu" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_option
-- ----------------------------
ALTER TABLE "public"."cm_option" ADD CONSTRAINT "fk_option_groupid" FOREIGN KEY ("group_id") REFERENCES "public"."cm_option_group" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_permission
-- ----------------------------
ALTER TABLE "public"."cm_permission" ADD CONSTRAINT "fk_permission_func" FOREIGN KEY ("func_id") REFERENCES "public"."cm_permission_func" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_permission_func
-- ----------------------------
ALTER TABLE "public"."cm_permission_func" ADD CONSTRAINT "fk_permission_module" FOREIGN KEY ("module_id") REFERENCES "public"."cm_permission_module" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_role_menu
-- ----------------------------
ALTER TABLE "public"."cm_role_menu" ADD CONSTRAINT "fk_role_menu_menuid" FOREIGN KEY ("menu_id") REFERENCES "public"."cm_menu" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_role_menu" ADD CONSTRAINT "fk_role_menu_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."cm_role" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_role_per
-- ----------------------------
ALTER TABLE "public"."cm_role_per" ADD CONSTRAINT "fk_role_per_perid" FOREIGN KEY ("per_id") REFERENCES "public"."cm_permission" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_role_per" ADD CONSTRAINT "fk_role_per_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."cm_role" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_user_group
-- ----------------------------
ALTER TABLE "public"."cm_user_group" ADD CONSTRAINT "fk_user_group_groupid" FOREIGN KEY ("group_id") REFERENCES "public"."cm_group" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_user_group" ADD CONSTRAINT "fk_user_group_userid" FOREIGN KEY ("user_id") REFERENCES "public"."cm_user" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_user_params
-- ----------------------------
ALTER TABLE "public"."cm_user_params" ADD CONSTRAINT "fk_user_params_paramsid" FOREIGN KEY ("param_id") REFERENCES "public"."cm_params" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_user_params" ADD CONSTRAINT "fk_user_params_userid" FOREIGN KEY ("user_id") REFERENCES "public"."cm_user" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_user_role
-- ----------------------------
ALTER TABLE "public"."cm_user_role" ADD CONSTRAINT "fk_user_role_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."cm_role" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_user_role" ADD CONSTRAINT "fk_user_role_userid" FOREIGN KEY ("user_id") REFERENCES "public"."cm_user" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_wfd_atv
-- ----------------------------
ALTER TABLE "public"."cm_wfd_atv" ADD CONSTRAINT "fk_wfd_atv_prcid" FOREIGN KEY ("prc_id") REFERENCES "public"."cm_wfd_prc" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_wfd_atv_role
-- ----------------------------
ALTER TABLE "public"."cm_wfd_atv_role" ADD CONSTRAINT "fk_wfd_atv_role_atvid" FOREIGN KEY ("atv_id") REFERENCES "public"."cm_wfd_atv" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_wfd_atv_role" ADD CONSTRAINT "fk_wfd_atv_role_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."cm_role" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_wfd_trs
-- ----------------------------
ALTER TABLE "public"."cm_wfd_trs" ADD CONSTRAINT "fk_wfd_trs_prcid" FOREIGN KEY ("prc_id") REFERENCES "public"."cm_wfd_prc" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_atv
-- ----------------------------
ALTER TABLE "public"."cm_wfi_atv" ADD CONSTRAINT "fk_wfi_atv_atvdid" FOREIGN KEY ("atvd_id") REFERENCES "public"."cm_wfd_atv" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_wfi_atv" ADD CONSTRAINT "fk_wfi_atv_prciid" FOREIGN KEY ("prci_id") REFERENCES "public"."cm_wfi_prc" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_item
-- ----------------------------
ALTER TABLE "public"."cm_wfi_item" ADD CONSTRAINT "fk_wfi_item_atviid" FOREIGN KEY ("atvi_id") REFERENCES "public"."cm_wfi_atv" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_prc
-- ----------------------------
ALTER TABLE "public"."cm_wfi_prc" ADD CONSTRAINT "fk_wfi_prc_prcdid" FOREIGN KEY ("prcd_id") REFERENCES "public"."cm_wfd_prc" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_trs
-- ----------------------------
ALTER TABLE "public"."cm_wfi_trs" ADD CONSTRAINT "fk_wfi_trs_srcatviid" FOREIGN KEY ("src_atvi_id") REFERENCES "public"."cm_wfi_atv" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_wfi_trs" ADD CONSTRAINT "fk_wfi_trs_tgtatviid" FOREIGN KEY ("tgt_atvi_id") REFERENCES "public"."cm_wfi_atv" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."cm_wfi_trs" ADD CONSTRAINT "fk_wfi_trs_trsdid" FOREIGN KEY ("trsd_id") REFERENCES "public"."cm_wfd_trs" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table crud_大儿
-- ----------------------------
ALTER TABLE "public"."crud_大儿" ADD CONSTRAINT "fk_大儿_parendid" FOREIGN KEY ("parent_id") REFERENCES "public"."crud_父表" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table crud_小儿
-- ----------------------------
ALTER TABLE "public"."crud_小儿" ADD CONSTRAINT "fk_小儿_parentid" FOREIGN KEY ("group_id") REFERENCES "public"."crud_父表" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table crud_用户角色
-- ----------------------------
ALTER TABLE "public"."crud_用户角色" ADD CONSTRAINT "fk_crud_用户角色_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."crud_角色" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."crud_用户角色" ADD CONSTRAINT "fk_crud_用户角色_userid" FOREIGN KEY ("user_id") REFERENCES "public"."crud_用户" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table crud_角色权限
-- ----------------------------
ALTER TABLE "public"."crud_角色权限" ADD CONSTRAINT "fk_角色权限_prvid" FOREIGN KEY ("prv_id") REFERENCES "public"."crud_权限" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."crud_角色权限" ADD CONSTRAINT "fk_角色权限_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."crud_角色" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table 物资主单
-- ----------------------------
ALTER TABLE "public"."物资主单" ADD CONSTRAINT "fk_物资主单_供应商" FOREIGN KEY ("供应商id") REFERENCES "public"."供应商" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE "public"."物资主单" ADD CONSTRAINT "fk_物资主单_部门" FOREIGN KEY ("部门id") REFERENCES "public"."部门" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table 物资库存
-- ----------------------------
ALTER TABLE "public"."物资库存" ADD CONSTRAINT "fk_物资库存_物资" FOREIGN KEY ("物资id") REFERENCES "public"."物资目录" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE "public"."物资库存" ADD CONSTRAINT "fk_物资库存_部门" FOREIGN KEY ("部门id") REFERENCES "public"."部门" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table 物资目录
-- ----------------------------
ALTER TABLE "public"."物资目录" ADD CONSTRAINT "fk_物资目录_分类" FOREIGN KEY ("分类id") REFERENCES "public"."物资分类" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table 物资计划
-- ----------------------------
ALTER TABLE "public"."物资计划" ADD CONSTRAINT "fk_物资计划_部门" FOREIGN KEY ("部门id") REFERENCES "public"."部门" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table 物资计划明细
-- ----------------------------
ALTER TABLE "public"."物资计划明细" ADD CONSTRAINT "fk_物资计划明细_物资" FOREIGN KEY ("物资id") REFERENCES "public"."物资目录" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE "public"."物资计划明细" ADD CONSTRAINT "fk_物资计划明细_计划" FOREIGN KEY ("计划id") REFERENCES "public"."物资计划" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table 物资详单
-- ----------------------------
ALTER TABLE "public"."物资详单" ADD CONSTRAINT "fk_物资详单_单据" FOREIGN KEY ("单据id") REFERENCES "public"."物资主单" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE "public"."物资详单" ADD CONSTRAINT "fk_物资详单_物资" FOREIGN KEY ("物资id") REFERENCES "public"."物资目录" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table 部门
-- ----------------------------
ALTER TABLE "public"."部门" ADD CONSTRAINT "fk_部门_上级id" FOREIGN KEY ("上级id") REFERENCES "public"."部门" ("id") ON DELETE RESTRICT ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Keys structure for table 部门人员
-- ----------------------------
ALTER TABLE "public"."部门人员" ADD CONSTRAINT "fk_部门人员_人员" FOREIGN KEY ("人员id") REFERENCES "public"."人员" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;
ALTER TABLE "public"."部门人员" ADD CONSTRAINT "fk_部门人员_部门" FOREIGN KEY ("部门id") REFERENCES "public"."部门" ("id") ON DELETE CASCADE ON UPDATE NO ACTION;

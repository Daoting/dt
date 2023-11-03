/*
Navicat 从 mysql 导出后修改：
1. int2 部分转bool 其余保留，转bool的数据需要加''
2. varchar(21000) varchar(65535)
3. timestamp 转 timestamp(0)
*/

-- ----------------------------
-- Table structure for cm_file_my
-- ----------------------------
CREATE TABLE "public"."cm_file_my" (
  "id" int8 NOT NULL,
  "parent_id" int8,
  "name" varchar(255) NOT NULL,
  "is_folder" bool NOT NULL,
  "ext_name" varchar(8),
  "info" varchar(512),
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
-- Table structure for cm_file_pub
-- ----------------------------
CREATE TABLE "public"."cm_file_pub" (
  "id" int8 NOT NULL,
  "parent_id" int8,
  "name" varchar(255) NOT NULL,
  "is_folder" bool NOT NULL,
  "ext_name" varchar(8),
  "info" varchar(512),
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
INSERT INTO "public"."cm_file_pub" VALUES (1, NULL, '公共文件', '1', NULL, '', '2020-10-21 15:19:20');
INSERT INTO "public"."cm_file_pub" VALUES (2, NULL, '素材库', '1', NULL, '', '2020-10-21 15:20:21');

-- ----------------------------
-- Table structure for cm_group
-- ----------------------------
CREATE TABLE "public"."cm_group" (
  "id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
  "note" varchar(255)
)
;
COMMENT ON COLUMN "public"."cm_group"."id" IS '组标识';
COMMENT ON COLUMN "public"."cm_group"."name" IS '组名';
COMMENT ON COLUMN "public"."cm_group"."note" IS '组描述';
COMMENT ON TABLE "public"."cm_group" IS '分组，与用户和角色多对多';

-- ----------------------------
-- Table structure for cm_group_role
-- ----------------------------
CREATE TABLE "public"."cm_group_role" (
  "group_id" int8 NOT NULL,
  "role_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_group_role"."group_id" IS '组标识';
COMMENT ON COLUMN "public"."cm_group_role"."role_id" IS '角色标识';
COMMENT ON TABLE "public"."cm_group_role" IS '组一角色多对多';

-- ----------------------------
-- Table structure for cm_menu
-- ----------------------------
CREATE TABLE "public"."cm_menu" (
  "id" int8 NOT NULL,
  "parent_id" int8,
  "name" varchar(64) NOT NULL,
  "is_group" bool NOT NULL,
  "view_name" varchar(128),
  "params" varchar(4000),
  "icon" varchar(128),
  "note" varchar(512),
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
INSERT INTO "public"."cm_menu" VALUES (1, NULL, '工作台', '1', '', '', '搬运工', '', 1, '0', '2019-03-07 10:45:44', '2019-03-07 10:45:43');
INSERT INTO "public"."cm_menu" VALUES (2, 1, '用户账号', '0', '用户账号', '', '钥匙', '账号管理及所属分组、关联角色，查看拥有菜单、已授权限', 2, '0', '2019-11-08 11:42:28', '2019-11-08 11:43:53');
INSERT INTO "public"."cm_menu" VALUES (3, 1, '菜单管理', '0', '菜单管理', '', '大图标', '菜单、菜单组管理和菜单授权角色', 3, '0', '2019-03-11 11:35:59', '2019-03-11 11:35:58');
INSERT INTO "public"."cm_menu" VALUES (4, 1, '系统角色', '0', '系统角色', '', '两人', '角色管理及所属分组、关联用户、拥有菜单、授予权限的管理', 4, '0', '2019-11-08 11:47:21', '2019-11-08 11:48:22');
INSERT INTO "public"."cm_menu" VALUES (5, 1, '分组管理', '0', '分组管理', '', '分组', '分组管理及关联的角色、关联的用户', 5, '0', '2023-03-10 08:34:49', '2023-03-10 08:34:49');
INSERT INTO "public"."cm_menu" VALUES (6, 1, '基础权限', '0', '基础权限', '', '审核', '按照模块、功能两级目录管理的权限和关联的角色', 6, '0', '2019-03-12 09:11:22', '2019-03-07 11:23:40');
INSERT INTO "public"."cm_menu" VALUES (7, 1, '参数定义', '0', '参数定义', '', '调色板', '参数名称、默认值的定义管理', 7, '0', '2019-03-12 15:35:56', '2019-03-12 15:37:10');
INSERT INTO "public"."cm_menu" VALUES (8, 1, '基础选项', '0', '基础选项', '', '修理', '按照分组管理的选项列表，如民族、学历等静态列表', 8, '0', '2019-11-08 11:49:40', '2019-11-08 11:49:46');
INSERT INTO "public"."cm_menu" VALUES (9, 1, '报表设计', '0', '报表设计', '', '折线图', '报表管理及报表模板设计', 9, '0', '2020-10-19 11:21:38', '2020-10-19 11:21:38');
INSERT INTO "public"."cm_menu" VALUES (10, 1, '流程设计', '0', '流程设计', '', '双绞线', '流程模板设计及流程实例查询', 10, '0', '2020-11-02 16:21:19', '2020-11-02 16:21:19');

-- ----------------------------
-- Table structure for cm_option
-- ----------------------------
CREATE TABLE "public"."cm_option" (
  "id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
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

-- ----------------------------
-- Table structure for cm_option_group
-- ----------------------------
CREATE TABLE "public"."cm_option_group" (
  "id" int8 NOT NULL,
  "name" varchar(255) NOT NULL
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

-- ----------------------------
-- Table structure for cm_params
-- ----------------------------
CREATE TABLE "public"."cm_params" (
  "id" int8 NOT NULL,
  "name" varchar(255) NOT NULL,
  "value" varchar(255),
  "note" varchar(255),
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
-- Table structure for CM_PERMISSION_MODULE
-- ----------------------------
CREATE TABLE "public"."cm_permission_module" (
  "id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
  "note" varchar(255)
)
;
COMMENT ON COLUMN "public"."cm_permission_module"."id" IS '模块标识';
COMMENT ON COLUMN "public"."cm_permission_module"."name" IS '模块名称';
COMMENT ON COLUMN "public"."cm_permission_module"."note" IS '模块描述';
COMMENT ON TABLE "public"."cm_permission_module" IS '权限所属模块';

-- ----------------------------
-- Records of CM_PERMISSION_MODULE
-- ----------------------------
INSERT INTO "public"."cm_permission_module" VALUES (1, '系统预留', '系统内部使用的权限控制，禁止删除');

-- ----------------------------
-- Table structure for CM_PERMISSION_FUNC
-- ----------------------------
CREATE TABLE "public"."cm_permission_func" (
  "id" int8 NOT NULL,
  "module_id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
  "note" varchar(255)
)
;
COMMENT ON COLUMN "public"."cm_permission_func"."module_id" IS '所属模块';
COMMENT ON COLUMN "public"."cm_permission_func"."name" IS '功能名称';
COMMENT ON COLUMN "public"."cm_permission_func"."note" IS '功能描述';
COMMENT ON TABLE "public"."cm_permission_func" IS '权限所属功能';

-- ----------------------------
-- Records of CM_PERMISSION_FUNC
-- ----------------------------
INSERT INTO "public"."cm_permission_func" VALUES (1, 1, '文件管理', '管理文件的上传、删除等');

-- ----------------------------
-- Table structure for cm_permission
-- ----------------------------
CREATE TABLE "public"."cm_permission" (
  "id" int8 NOT NULL,
  "func_id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
  "note" varchar(255)
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

-- ----------------------------
-- Table structure for cm_role
-- ----------------------------
CREATE TABLE "public"."cm_role" (
  "id" int8 NOT NULL,
  "name" varchar(32) NOT NULL,
  "note" varchar(255)
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

-- ----------------------------
-- Table structure for cm_role_menu
-- ----------------------------
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

-- ----------------------------
-- Table structure for cm_role_per
-- ----------------------------
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

-- ----------------------------
-- Table structure for cm_rpt
-- ----------------------------
CREATE TABLE "public"."cm_rpt" (
  "id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
  "define" varchar(65535),
  "note" varchar(255),
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
-- Table structure for cm_user
-- ----------------------------
CREATE TABLE "public"."cm_user" (
  "id" int8 NOT NULL,
  "name" varchar(32),
  "phone" varchar(16),
  "pwd" char(32) NOT NULL,
  "photo" varchar(255),
  "expired" bool NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_user"."id" IS '用户标识';
COMMENT ON COLUMN "public"."cm_user"."name" IS '账号，唯一';
COMMENT ON COLUMN "public"."cm_user"."phone" IS '手机号，唯一';
COMMENT ON COLUMN "public"."cm_user"."pwd" IS '密码的md5';
COMMENT ON COLUMN "public"."cm_user"."photo" IS '头像';
COMMENT ON COLUMN "public"."cm_user"."expired" IS '是否停用';
COMMENT ON COLUMN "public"."cm_user"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_user"."mtime" IS '修改时间';
COMMENT ON TABLE "public"."cm_user" IS '系统用户';

-- ----------------------------
-- Records of cm_user
-- ----------------------------
INSERT INTO "public"."cm_user" VALUES (1, 'admin', '13511111111', 'b59c67bf196a4758191e42f76670ceba', '', '0', '2019-10-24 09:06:38', '2023-03-16 08:35:39');

-- ----------------------------
-- Table structure for cm_user_group
-- ----------------------------
CREATE TABLE "public"."cm_user_group" (
  "user_id" int8 NOT NULL,
  "group_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_user_group"."user_id" IS '用户标识';
COMMENT ON COLUMN "public"."cm_user_group"."group_id" IS '组标识';
COMMENT ON TABLE "public"."cm_user_group" IS '用户一组多对多';

-- ----------------------------
-- Table structure for cm_user_params
-- ----------------------------
CREATE TABLE "public"."cm_user_params" (
  "user_id" int8 NOT NULL,
  "param_id" int8 NOT NULL,
  "value" varchar(255),
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_user_params"."user_id" IS '用户标识';
COMMENT ON COLUMN "public"."cm_user_params"."param_id" IS '参数标识';
COMMENT ON COLUMN "public"."cm_user_params"."value" IS '参数值';
COMMENT ON COLUMN "public"."cm_user_params"."mtime" IS '修改时间';
COMMENT ON TABLE "public"."cm_user_params" IS '用户参数值';

-- ----------------------------
-- Table structure for cm_user_role
-- ----------------------------
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

-- ----------------------------
-- Table structure for cm_wfd_atv
-- ----------------------------
CREATE TABLE "public"."cm_wfd_atv" (
  "id" int8 NOT NULL,
  "prc_id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
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
-- Table structure for cm_wfd_atv_role
-- ----------------------------
CREATE TABLE "public"."cm_wfd_atv_role" (
  "atv_id" int8 NOT NULL,
  "role_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_wfd_atv_role"."atv_id" IS '活动标识';
COMMENT ON COLUMN "public"."cm_wfd_atv_role"."role_id" IS '角色标识';
COMMENT ON TABLE "public"."cm_wfd_atv_role" IS '活动授权';

-- ----------------------------
-- Table structure for cm_wfd_prc
-- ----------------------------
CREATE TABLE "public"."cm_wfd_prc" (
  "id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
  "diagram" varchar(65535),
  "is_locked" bool NOT NULL,
  "singleton" bool NOT NULL,
  "note" varchar(255),
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
-- Table structure for cm_wfd_trs
-- ----------------------------
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
-- Table structure for cm_wfi_atv
-- ----------------------------
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
-- Table structure for cm_wfi_item
-- ----------------------------
CREATE TABLE "public"."cm_wfi_item" (
  "id" int8 NOT NULL,
  "atvi_id" int8 NOT NULL,
  "status" int2 NOT NULL,
  "assign_kind" int2 NOT NULL,
  "sender" varchar(32),
  "stime" timestamp(0) NOT NULL,
  "is_accept" bool NOT NULL,
  "accept_time" timestamp(0),
  "role_id" int8,
  "user_id" int8,
  "note" varchar(255),
  "dispidx" int4 NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_wfi_item"."id" IS '工作项标识';
COMMENT ON COLUMN "public"."cm_wfi_item"."atvi_id" IS '活动实例标识';
COMMENT ON COLUMN "public"."cm_wfi_item"."status" IS '#WfiItemStatus#工作项状态 0活动 1结束 2终止 3同步活动';
COMMENT ON COLUMN "public"."cm_wfi_item"."assign_kind" IS '#WfiItemAssignKind#指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派';
COMMENT ON COLUMN "public"."cm_wfi_item"."sender" IS '发送者';
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
-- Table structure for cm_wfi_prc
-- ----------------------------
CREATE TABLE "public"."cm_wfi_prc" (
  "id" int8 NOT NULL,
  "prcd_id" int8 NOT NULL,
  "name" varchar(255) NOT NULL,
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
-- Table structure for cm_wfi_trs
-- ----------------------------
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
-- Table structure for fsm_file
-- ----------------------------
CREATE TABLE "public"."fsm_file" (
  "id" int8 NOT NULL,
  "name" varchar(512) NOT NULL,
  "path" varchar(512) NOT NULL,
  "size" int8 NOT NULL,
  "info" varchar(512),
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
-- Indexes structure for table cm_file_my
-- ----------------------------
CREATE INDEX "idx_file_my_parentid" ON "public"."cm_file_my" USING btree (
  "parent_id" ASC
);
CREATE INDEX "idx_file_my_userid" ON "public"."cm_file_my" USING btree (
  "user_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_file_my
-- ----------------------------
ALTER TABLE "public"."cm_file_my" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_file_pub
-- ----------------------------
CREATE INDEX "idx_file_pub_parentid" ON "public"."cm_file_pub" USING btree (
  "parent_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_file_pub
-- ----------------------------
ALTER TABLE "public"."cm_file_pub" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_group
-- ----------------------------
CREATE UNIQUE INDEX "idx_group_name" ON "public"."cm_group" USING btree (
  "name" ASC
);
COMMENT ON INDEX "public"."idx_group_name" IS '不重复';

-- ----------------------------
-- Primary Key structure for table cm_group
-- ----------------------------
ALTER TABLE "public"."cm_group" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_group_role
-- ----------------------------
CREATE INDEX "idx_group_role_groupid" ON "public"."cm_group_role" USING btree (
  "group_id" ASC
);
CREATE INDEX "idx_group_role_roleid" ON "public"."cm_group_role" USING btree (
  "role_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_group_role
-- ----------------------------
ALTER TABLE "public"."cm_group_role" ADD PRIMARY KEY ("group_id", "role_id");

-- ----------------------------
-- Indexes structure for table cm_menu
-- ----------------------------
CREATE INDEX "idx_menu_parentid" ON "public"."cm_menu" USING btree (
  "parent_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_menu
-- ----------------------------
ALTER TABLE "public"."cm_menu" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_option
-- ----------------------------
CREATE INDEX "idx_option_groupid" ON "public"."cm_option" USING btree (
  "group_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_option
-- ----------------------------
ALTER TABLE "public"."cm_option" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table cm_option_group
-- ----------------------------
ALTER TABLE "public"."cm_option_group" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_params
-- ----------------------------
CREATE UNIQUE INDEX "idx_params_name" ON "public"."cm_params" USING btree (
  "name" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_params
-- ----------------------------
ALTER TABLE "public"."cm_params" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_permission
-- ----------------------------
CREATE INDEX "fk_permission" ON "public"."cm_permission" (
  "func_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_permission
-- ----------------------------
ALTER TABLE "public"."cm_permission" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table CM_PERMISSION_FUNC
-- ----------------------------
CREATE INDEX "fk_permission_func" ON "public"."cm_permission_func" (
  "module_id" ASC
);

-- ----------------------------
-- Primary Key structure for table CM_PERMISSION_FUNC
-- ----------------------------
ALTER TABLE "public"."cm_permission_func" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table CM_PERMISSION_MODULE
-- ----------------------------
ALTER TABLE "public"."cm_permission_module" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Uniques structure for table cm_permission
-- ----------------------------
ALTER TABLE "public"."cm_permission" ADD CONSTRAINT "uq_permission" UNIQUE ("func_id", "name");

-- ----------------------------
-- Uniques structure for table cm_permission
-- ----------------------------
ALTER TABLE "public"."cm_permission_func" ADD CONSTRAINT "uq_permission_func" UNIQUE ("module_id", "name");

ALTER TABLE "public"."cm_permission" 
  ADD CONSTRAINT "fk_permission_func" FOREIGN KEY ("func_id") REFERENCES "public"."cm_permission_func" ("id");

ALTER TABLE "public"."cm_permission_func" 
  ADD CONSTRAINT "fk_permission_module" FOREIGN KEY ("module_id") REFERENCES "public"."cm_permission_module" ("id");

-- ----------------------------
-- Indexes structure for table cm_role
-- ----------------------------
CREATE UNIQUE INDEX "idx_role_name" ON "public"."cm_role" USING btree (
  "name" ASC
);
COMMENT ON INDEX "public"."idx_role_name" IS '不重复';

-- ----------------------------
-- Primary Key structure for table cm_role
-- ----------------------------
ALTER TABLE "public"."cm_role" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_role_menu
-- ----------------------------
CREATE INDEX "idx_role_menu_menuid" ON "public"."cm_role_menu" USING btree (
  "menu_id" ASC
);
CREATE INDEX "idx_role_menu_roleid" ON "public"."cm_role_menu" USING btree (
  "role_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_role_menu
-- ----------------------------
ALTER TABLE "public"."cm_role_menu" ADD PRIMARY KEY ("role_id", "menu_id");

-- ----------------------------
-- Indexes structure for table cm_role_per
-- ----------------------------
CREATE INDEX "idx_role_per_roleid" ON "public"."cm_role_per" USING btree (
  "role_id" ASC
);
CREATE INDEX "idx_role_per_perid" ON "public"."cm_role_per" USING btree (
  "per_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_role_per
-- ----------------------------
ALTER TABLE "public"."cm_role_per" ADD PRIMARY KEY ("role_id", "per_id");

-- ----------------------------
-- Indexes structure for table cm_rpt
-- ----------------------------
CREATE UNIQUE INDEX "idx_rpt_name" ON "public"."cm_rpt" USING btree (
  "name" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_rpt
-- ----------------------------
ALTER TABLE "public"."cm_rpt" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_user
-- ----------------------------
CREATE INDEX "idx_user_name" ON "public"."cm_user" USING btree (
  name ASC
);

CREATE INDEX "idx_user_phone" ON "public"."cm_user" USING btree (
  "phone" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_user
-- ----------------------------
ALTER TABLE "public"."cm_user" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_user_group
-- ----------------------------
CREATE INDEX "idx_user_group_userid" ON "public"."cm_user_group" USING btree (
  "user_id" ASC
);
CREATE INDEX "idx_user_group_groupid" ON "public"."cm_user_group" USING btree (
  "group_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_user_group
-- ----------------------------
ALTER TABLE "public"."cm_user_group" ADD PRIMARY KEY ("user_id", "group_id");

-- ----------------------------
-- Indexes structure for table cm_user_params
-- ----------------------------
CREATE INDEX "idx_user_params_userid" ON "public"."cm_user_params" USING btree (
  "user_id" ASC
);
CREATE INDEX "idx_user_params_paramsid" ON "public"."cm_user_params" USING btree (
  "param_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_user_params
-- ----------------------------
ALTER TABLE "public"."cm_user_params" ADD PRIMARY KEY ("user_id", "param_id");

-- ----------------------------
-- Indexes structure for table cm_user_role
-- ----------------------------
CREATE INDEX "idx_user_role_userid" ON "public"."cm_user_role" USING btree (
  "user_id" ASC
);
CREATE INDEX "idx_user_role_roleid" ON "public"."cm_user_role" USING btree (
  "role_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_user_role
-- ----------------------------
ALTER TABLE "public"."cm_user_role" ADD PRIMARY KEY ("user_id", "role_id");

-- ----------------------------
-- Indexes structure for table cm_wfd_atv
-- ----------------------------
CREATE INDEX "idx_wfd_atv_prcid" ON "public"."cm_wfd_atv" USING btree (
  "prc_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_wfd_atv
-- ----------------------------
ALTER TABLE "public"."cm_wfd_atv" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfd_atv_role
-- ----------------------------
CREATE INDEX "idx_wfd_atv_role_roleid" ON "public"."cm_wfd_atv_role" USING btree (
  "role_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_wfd_atv_role
-- ----------------------------
ALTER TABLE "public"."cm_wfd_atv_role" ADD PRIMARY KEY ("atv_id", "role_id");

-- ----------------------------
-- Primary Key structure for table cm_wfd_prc
-- ----------------------------
ALTER TABLE "public"."cm_wfd_prc" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfd_trs
-- ----------------------------
CREATE INDEX "idx_wfd_trs_prcid" ON "public"."cm_wfd_trs" USING btree (
  "prc_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_wfd_trs
-- ----------------------------
ALTER TABLE "public"."cm_wfd_trs" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfi_atv
-- ----------------------------
CREATE INDEX "idx_wfi_atv_prciid" ON "public"."cm_wfi_atv" USING btree (
  "prci_id" ASC
);
CREATE INDEX "idx_wfi_atv_atvdid" ON "public"."cm_wfi_atv" USING btree (
  "atvd_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_wfi_atv
-- ----------------------------
ALTER TABLE "public"."cm_wfi_atv" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfi_item
-- ----------------------------
CREATE INDEX "idx_wfi_item_atviid" ON "public"."cm_wfi_item" USING btree (
  "atvi_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_wfi_item
-- ----------------------------
ALTER TABLE "public"."cm_wfi_item" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfi_prc
-- ----------------------------
CREATE INDEX "idx_wfi_prc_prcdid" ON "public"."cm_wfi_prc" USING btree (
  "prcd_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_wfi_prc
-- ----------------------------
ALTER TABLE "public"."cm_wfi_prc" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table cm_wfi_trs
-- ----------------------------
CREATE INDEX "idx_wfi_trs_trsdid" ON "public"."cm_wfi_trs" USING btree (
  "trsd_id" ASC
);
CREATE INDEX "idx_wfi_trs_srcatviid" ON "public"."cm_wfi_trs" USING btree (
  "src_atvi_id" ASC
);
CREATE INDEX "idx_wfi_trs_tgtatviid" ON "public"."cm_wfi_trs" USING btree (
  "tgt_atvi_id" ASC
);

-- ----------------------------
-- Primary Key structure for table cm_wfi_trs
-- ----------------------------
ALTER TABLE "public"."cm_wfi_trs" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table fsm_file
-- ----------------------------
CREATE UNIQUE INDEX "idx_fsm_file_path" ON "public"."fsm_file" USING btree (
  "path" ASC
);

-- ----------------------------
-- Primary Key structure for table fsm_file
-- ----------------------------
ALTER TABLE "public"."fsm_file" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Foreign Keys structure for table cm_file_my
-- ----------------------------
ALTER TABLE "public"."cm_file_my" ADD CONSTRAINT "fk_file_my_parentid" FOREIGN KEY ("parent_id") REFERENCES "public"."cm_file_my" ("id");
ALTER TABLE "public"."cm_file_my" ADD CONSTRAINT "fk_file_my_userid" FOREIGN KEY ("user_id") REFERENCES "public"."cm_user" ("id");

-- ----------------------------
-- Foreign Keys structure for table cm_file_pub
-- ----------------------------
ALTER TABLE "public"."cm_file_pub" ADD CONSTRAINT "fk_file_pub_parentid" FOREIGN KEY ("parent_id") REFERENCES "public"."cm_file_pub" ("id");

-- ----------------------------
-- Foreign Keys structure for table cm_group_role
-- ----------------------------
ALTER TABLE "public"."cm_group_role" ADD CONSTRAINT "fk_group_role_groupid" FOREIGN KEY ("group_id") REFERENCES "public"."cm_group" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."cm_group_role" ADD CONSTRAINT "fk_group_role_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."cm_role" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_menu
-- ----------------------------
ALTER TABLE "public"."cm_menu" ADD CONSTRAINT "fk_menu_parentid" FOREIGN KEY ("parent_id") REFERENCES "public"."cm_menu" ("id");

-- ----------------------------
-- Foreign Keys structure for table cm_option
-- ----------------------------
ALTER TABLE "public"."cm_option" ADD CONSTRAINT "fk_option_groupid" FOREIGN KEY ("group_id") REFERENCES "public"."cm_option_group" ("id");

-- ----------------------------
-- Foreign Keys structure for table cm_role_menu
-- ----------------------------
ALTER TABLE "public"."cm_role_menu" ADD CONSTRAINT "fk_role_menu_menuid" FOREIGN KEY ("menu_id") REFERENCES "public"."cm_menu" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."cm_role_menu" ADD CONSTRAINT "fk_role_menu_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."cm_role" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_role_per
-- ----------------------------
ALTER TABLE "public"."cm_role_per" ADD CONSTRAINT "fk_role_per_perid" FOREIGN KEY ("per_id") REFERENCES "public"."cm_permission" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."cm_role_per" ADD CONSTRAINT "fk_role_per_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."cm_role" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_user_group
-- ----------------------------
ALTER TABLE "public"."cm_user_group" ADD CONSTRAINT "fk_user_group_groupid" FOREIGN KEY ("group_id") REFERENCES "public"."cm_group" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."cm_user_group" ADD CONSTRAINT "fk_user_group_userid" FOREIGN KEY ("user_id") REFERENCES "public"."cm_user" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_user_params
-- ----------------------------
ALTER TABLE "public"."cm_user_params" ADD CONSTRAINT "fk_user_params_paramsid" FOREIGN KEY ("param_id") REFERENCES "public"."cm_params" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."cm_user_params" ADD CONSTRAINT "fk_user_params_userid" FOREIGN KEY ("user_id") REFERENCES "public"."cm_user" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_user_role
-- ----------------------------
ALTER TABLE "public"."cm_user_role" ADD CONSTRAINT "fk_user_role_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."cm_role" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."cm_user_role" ADD CONSTRAINT "fk_user_role_userid" FOREIGN KEY ("user_id") REFERENCES "public"."cm_user" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfd_atv
-- ----------------------------
ALTER TABLE "public"."cm_wfd_atv" ADD CONSTRAINT "fk_wfd_atv_prcid" FOREIGN KEY ("prc_id") REFERENCES "public"."cm_wfd_prc" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfd_atv_role
-- ----------------------------
ALTER TABLE "public"."cm_wfd_atv_role" ADD CONSTRAINT "fk_wfd_atv_role_atvid" FOREIGN KEY ("atv_id") REFERENCES "public"."cm_wfd_atv" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."cm_wfd_atv_role" ADD CONSTRAINT "fk_wfd_atv_role_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."cm_role" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfd_trs
-- ----------------------------
ALTER TABLE "public"."cm_wfd_trs" ADD CONSTRAINT "fk_wfd_trs_prcid" FOREIGN KEY ("prc_id") REFERENCES "public"."cm_wfd_prc" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_atv
-- ----------------------------
ALTER TABLE "public"."cm_wfi_atv" ADD CONSTRAINT "fk_wfi_atv_atvdid" FOREIGN KEY ("atvd_id") REFERENCES "public"."cm_wfd_atv" ("id");
ALTER TABLE "public"."cm_wfi_atv" ADD CONSTRAINT "fk_wfi_atv_prciid" FOREIGN KEY ("prci_id") REFERENCES "public"."cm_wfi_prc" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_item
-- ----------------------------
ALTER TABLE "public"."cm_wfi_item" ADD CONSTRAINT "fk_wfi_item_atviid" FOREIGN KEY ("atvi_id") REFERENCES "public"."cm_wfi_atv" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_prc
-- ----------------------------
ALTER TABLE "public"."cm_wfi_prc" ADD CONSTRAINT "fk_wfi_prc_prcdid" FOREIGN KEY ("prcd_id") REFERENCES "public"."cm_wfd_prc" ("id");

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_trs
-- ----------------------------
ALTER TABLE "public"."cm_wfi_trs" ADD CONSTRAINT "fk_wfi_trs_srcatviid" FOREIGN KEY ("src_atvi_id") REFERENCES "public"."cm_wfi_atv" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."cm_wfi_trs" ADD CONSTRAINT "fk_wfi_trs_tgtatviid" FOREIGN KEY ("tgt_atvi_id") REFERENCES "public"."cm_wfi_atv" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."cm_wfi_trs" ADD CONSTRAINT "fk_wfi_trs_trsdid" FOREIGN KEY ("trsd_id") REFERENCES "public"."cm_wfd_trs" ("id") ON DELETE CASCADE;

-- ----------------------------
-- 序列
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."cm_menu_dispidx";
DROP SEQUENCE IF EXISTS "public"."cm_option_dispidx";
DROP SEQUENCE IF EXISTS "public"."cm_wfd_prc_dispidx";
DROP SEQUENCE IF EXISTS "public"."cm_wfi_item_dispidx";
DROP SEQUENCE IF EXISTS "public"."cm_wfi_prc_dispidx";
create sequence cm_menu_dispidx start 90;
create sequence cm_option_dispidx start 1032;
create sequence cm_wfd_prc_dispidx start 12;
create sequence cm_wfi_item_dispidx start 177;
create sequence cm_wfi_prc_dispidx start 66;
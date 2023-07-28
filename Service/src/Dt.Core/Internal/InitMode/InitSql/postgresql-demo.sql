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
  "info" varchar(512) NOT NULL,
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
INSERT INTO "public"."cm_file_my" VALUES (140724076930789376, NULL, '新目录1', '1', NULL, '', '2020-10-23 15:47:16', 1);
INSERT INTO "public"."cm_file_my" VALUES (140724154458304512, 140724076930789376, 'b', '1', NULL, '', '2020-10-23 15:47:34', 1);
INSERT INTO "public"."cm_file_my" VALUES (141735914371936256, NULL, '新目录12', '1', NULL, '', '2020-10-26 10:48:01', 2);
INSERT INTO "public"."cm_file_my" VALUES (456284281217503232, NULL, '新Tab', '1', '', '', '2023-03-13 10:30:55', 1);

-- ----------------------------
-- Table structure for cm_file_pub
-- ----------------------------
CREATE TABLE "public"."cm_file_pub" (
  "id" int8 NOT NULL,
  "parent_id" int8,
  "name" varchar(255) NOT NULL,
  "is_folder" bool NOT NULL,
  "ext_name" varchar(8),
  "info" varchar(512) NOT NULL,
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
INSERT INTO "public"."cm_file_pub" VALUES (140015729575325696, 1, '新目录a', '1', NULL, '', '0001-01-01 00:00:00');
INSERT INTO "public"."cm_file_pub" VALUES (140016348063199232, 1, '新目录1111', '1', NULL, '', '2020-10-21 16:55:00');
INSERT INTO "public"."cm_file_pub" VALUES (140244264617373696, 140016348063199232, '新目录q', '1', NULL, '', '2020-10-22 08:00:39');
INSERT INTO "public"."cm_file_pub" VALUES (140253323206717440, 140244264617373696, 'ab', '1', NULL, '', '2020-10-22 08:36:39');
INSERT INTO "public"."cm_file_pub" VALUES (140266906502164480, 140244264617373696, 'aa', '0', 'xlsx', '[["v0/1F/4A/140266906879651840.xlsx","aa","xlsx文件",8236,"daoting","2020-10-22 09:30"]]', '2020-10-22 09:30:37');
INSERT INTO "public"."cm_file_pub" VALUES (142873261784297472, 2, '新目录1', '1', NULL, '', '2020-10-29 14:07:20');
INSERT INTO "public"."cm_file_pub" VALUES (142888903606398976, 2, '12', '0', 'xlsx', '[["v0/52/37/142888904373956608.xlsx","12","xlsx文件",8153,"daoting","2020-10-29 15:09"]]', '2020-10-29 15:09:30');
INSERT INTO "public"."cm_file_pub" VALUES (142913881819181056, 2, '未标题-2', '0', 'jpg', '[["v0/E3/18/142913882284748800.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2020-10-29 16:48"]]', '2020-10-29 16:48:44');
INSERT INTO "public"."cm_file_pub" VALUES (142914110945619968, 2, 'Icon-20@2x', '0', 'png', '[["v0/E3/0D/142914111109197824.png","Icon-20@2x","40 x 40 (.png)",436,"daoting","2020-10-29 16:49"]]', '2020-10-29 16:49:39');
INSERT INTO "public"."cm_file_pub" VALUES (143174605384577024, 140016348063199232, 'Icon-20@3x', '0', 'png', '[["v0/56/59/143174606269575168.png","Icon-20@3x","60 x 60 (.png)",496,"daoting","2020-10-30 10:04"]]', '2020-10-30 10:04:47');
INSERT INTO "public"."cm_file_pub" VALUES (143191060503195648, 1, 'Icon-20@3x', '0', 'png', '[["v0/56/59/143191060947791872.png","Icon-20@3x","60 x 60 (.png)",534,"daoting","2020-10-30 11:10"]]', '2020-10-30 11:10:10');
INSERT INTO "public"."cm_file_pub" VALUES (143192411157164032, 140015729575325696, 'Icon-29@2x', '0', 'png', '[["v0/46/CE/143192411832446976.png","Icon-29@2x","58 x 58 (.png)",624,"daoting","2020-10-30 11:15"]]', '2020-10-30 11:15:32');
INSERT INTO "public"."cm_file_pub" VALUES (143193081423720448, 140015729575325696, '3709740f5c5e4cb4909a6cc79f412734_th', '0', 'png', '[["v0/BF/6D/143193081931231232.png","3709740f5c5e4cb4909a6cc79f412734_th","537 x 302 (.png)",27589,"daoting","2020-10-30 11:18"]]', '2020-10-30 11:18:12');
INSERT INTO "public"."cm_file_pub" VALUES (143195001659977728, 1, '未标题-2', '0', 'jpg', '[["v0/E3/18/143195002217820160.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2020-10-30 11:25"]]', '2020-10-30 11:25:50');
INSERT INTO "public"."cm_file_pub" VALUES (143203944146792448, 1, 'ImageStabilization', '0', 'wmv', '[["v0/EA/34/143203944767549440.wmv","ImageStabilization","00:00:06 (480 x 288)",403671,"daoting","2020-10-30 12:01"]]', '2020-10-30 12:01:22');
INSERT INTO "public"."cm_file_pub" VALUES (172190549410705408, 1, '公司服务器及网络', '0', 'txt', '[["v0/5F/37/172190549775609856.txt","公司服务器及网络","txt文件",435,"daoting","2021-01-18 11:43"]]', '2021-01-18 11:43:37');
INSERT INTO "public"."cm_file_pub" VALUES (185641691419373568, 1, '1', '0', 'png', '[["v0/FC/63/185641725430984704.png","1","1101 x 428 (.png)",47916,"daoting","2021-02-24 14:33"]]', '2021-02-24 14:33:46');
INSERT INTO "public"."cm_file_pub" VALUES (187725770344230912, 1, 'doc1', '0', 'png', '[["v0/D8/28/187725778074333184.png","doc1","1076 x 601 (.png)",59038,"daoting","2021-03-02 08:35"]]', '2021-03-02 08:35:12');
INSERT INTO "public"."cm_file_pub" VALUES (205916917767991296, 140015729575325696, 'state', '0', 'db', '[["v0/DF/F3/205916918690738176.db","state","db文件",90112,"苹果","2021-04-21 13:20"]]', '2021-04-21 13:20:20');
INSERT INTO "public"."cm_file_pub" VALUES (255970120425140224, 456277006646005760, 'abc', '1', '', '', '2021-09-06 16:13:53');
INSERT INTO "public"."cm_file_pub" VALUES (322270820868235264, 1, '172190549775609856', '0', 'txt', '[["editor/57/01/322270823007330304.txt","172190549775609856","txt文件",435,"daoting","2022-03-08 15:09"]]', '2022-03-08 15:09:10');
INSERT INTO "public"."cm_file_pub" VALUES (456276498464133120, 456277006646005760, '未标题-2', '0', 'jpg', '[["editor/E3/18/456276498854203392.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2023-03-13 09:59"]]', '2023-03-13 09:59:59');
INSERT INTO "public"."cm_file_pub" VALUES (456277006646005760, 1, '新Tab', '1', '', '', '2023-03-13 10:02:00');
INSERT INTO "public"."cm_file_pub" VALUES (456281421624922112, 255970120425140224, '未标题-2', '0', 'jpg', '[["editor/E3/18/456281422107267072.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2023-03-13 10:19"]]', '2023-03-13 10:19:33');
INSERT INTO "public"."cm_file_pub" VALUES (456281921225248768, 456277006646005760, 'UserList', '0', 'xaml', '[["editor/C1/45/456281921523044352.xaml","UserList","xaml文件",2682,"daoting","2023-03-13 10:21"]]', '2023-03-13 10:21:32');

-- ----------------------------
-- Table structure for cm_group
-- ----------------------------
CREATE TABLE "public"."cm_group" (
  "id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
  "note" varchar(255) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_group"."id" IS '组标识';
COMMENT ON COLUMN "public"."cm_group"."name" IS '组名';
COMMENT ON COLUMN "public"."cm_group"."note" IS '组描述';
COMMENT ON TABLE "public"."cm_group" IS '分组，与用户和角色多对多';

-- ----------------------------
-- Records of cm_group
-- ----------------------------
INSERT INTO "public"."cm_group" VALUES (454483802783240192, '分组1', '');
INSERT INTO "public"."cm_group" VALUES (454484847190102016, '2', '');
INSERT INTO "public"."cm_group" VALUES (454484924033945600, '3', '');

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
-- Records of cm_group_role
-- ----------------------------
INSERT INTO "public"."cm_group_role" VALUES (454483802783240192, 2);
INSERT INTO "public"."cm_group_role" VALUES (454483802783240192, 22844822693027840);
INSERT INTO "public"."cm_group_role" VALUES (454483802783240192, 152695933758603264);
INSERT INTO "public"."cm_group_role" VALUES (454483802783240192, 152696004814307328);
INSERT INTO "public"."cm_group_role" VALUES (454484847190102016, 152695933758603264);
INSERT INTO "public"."cm_group_role" VALUES (454484924033945600, 22844822693027840);

-- ----------------------------
-- Table structure for cm_menu
-- ----------------------------
CREATE TABLE "public"."cm_menu" (
  "id" int8 NOT NULL,
  "parent_id" int8,
  "name" varchar(64) NOT NULL,
  "is_group" bool NOT NULL,
  "view_name" varchar(128) NOT NULL,
  "params" varchar(4000) NOT NULL,
  "icon" varchar(128) NOT NULL,
  "note" varchar(512) NOT NULL,
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
INSERT INTO "public"."cm_menu" VALUES (2, 1, '用户账号', '0', '用户账号', '', '钥匙', '', 2, '0', '2019-11-08 11:42:28', '2019-11-08 11:43:53');
INSERT INTO "public"."cm_menu" VALUES (3, 1, '菜单管理', '0', '菜单管理', '', '大图标', '', 3, '0', '2019-03-11 11:35:59', '2019-03-11 11:35:58');
INSERT INTO "public"."cm_menu" VALUES (4, 1, '系统角色', '0', '系统角色', '', '两人', '', 4, '0', '2019-11-08 11:47:21', '2019-11-08 11:48:22');
INSERT INTO "public"."cm_menu" VALUES (5, 1, '分组管理', '0', '分组管理', '', '分组', '', 5, '0', '2023-03-10 08:34:49', '2023-03-10 08:34:49');
INSERT INTO "public"."cm_menu" VALUES (6, 1, '基础权限', '0', '基础权限', '', '审核', '', 6, '0', '2019-03-12 09:11:22', '2019-03-07 11:23:40');
INSERT INTO "public"."cm_menu" VALUES (7, 1, '参数定义', '0', '参数定义', '', '调色板', '', 7, '0', '2019-03-12 15:35:56', '2019-03-12 15:37:10');
INSERT INTO "public"."cm_menu" VALUES (8, 1, '基础选项', '0', '基础选项', '', '修理', '', 8, '0', '2019-11-08 11:49:40', '2019-11-08 11:49:46');
INSERT INTO "public"."cm_menu" VALUES (9, 1, '报表设计', '0', '报表设计', '', '折线图', '', 76, '0', '2020-10-19 11:21:38', '2020-10-19 11:21:38');
INSERT INTO "public"."cm_menu" VALUES (10, 1, '流程设计', '0', '流程设计', '', '双绞线', '', 79, '0', '2020-11-02 16:21:19', '2020-11-02 16:21:19');
INSERT INTO "public"."cm_menu" VALUES (15268145234386944, 15315938808373248, '新菜单组22', '1', '', '', '文件夹', '', 25, '0', '2019-11-12 11:10:10', '2019-11-12 11:10:13');
INSERT INTO "public"."cm_menu" VALUES (15315637929975808, 18562741636898816, '新菜单12', '0', '', '', '文件', '', 48, '0', '2019-11-12 14:18:53', '2019-11-12 14:31:38');
INSERT INTO "public"."cm_menu" VALUES (15315938808373248, NULL, '新菜单组额', '1', '', '', '文件夹', '', 67, '0', '2019-11-12 14:20:04', '2019-11-12 14:20:14');
INSERT INTO "public"."cm_menu" VALUES (18562741636898816, 15315938808373248, '新组t', '1', '', '', '文件夹', '', 63, '0', '2019-11-21 13:21:43', '2019-11-21 13:21:43');
INSERT INTO "public"."cm_menu" VALUES (18860286065975296, NULL, '新菜单a123', '0', '报表', '新报表111,abc1', '文件', '', 68, '0', '2019-11-22 09:04:04', '2019-11-22 09:04:04');
INSERT INTO "public"."cm_menu" VALUES (154430055023640576, NULL, '新菜单xxx', '0', '报表', '', '文件', '', 84, '0', '2020-11-30 11:29:56', '2020-11-30 11:29:56');
INSERT INTO "public"."cm_menu" VALUES (259520016549801984, NULL, '新组bcd', '1', '', '', '文件夹', '', 83, '0', '2021-09-16 11:19:54', '2021-09-16 11:19:54');

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
INSERT INTO "public"."cm_option" VALUES (65, '黑龙江杜尔伯特县', 65, 3);
INSERT INTO "public"."cm_option" VALUES (66, '黑龙江富裕县', 66, 3);
INSERT INTO "public"."cm_option" VALUES (67, '黑龙江林甸县', 67, 3);
INSERT INTO "public"."cm_option" VALUES (68, '黑龙江克山县', 68, 3);
INSERT INTO "public"."cm_option" VALUES (69, '黑龙江克东县', 69, 3);
INSERT INTO "public"."cm_option" VALUES (70, '黑龙江省拜泉县', 70, 3);
INSERT INTO "public"."cm_option" VALUES (71, '黑龙江鸡西市', 71, 3);
INSERT INTO "public"."cm_option" VALUES (72, '黑龙江鸡东县', 72, 3);
INSERT INTO "public"."cm_option" VALUES (73, '黑龙江鹤岗市', 73, 3);
INSERT INTO "public"."cm_option" VALUES (74, '黑龙江萝北县', 74, 3);
INSERT INTO "public"."cm_option" VALUES (75, '黑龙江绥滨县', 75, 3);
INSERT INTO "public"."cm_option" VALUES (76, '黑龙江双鸭山市', 76, 3);
INSERT INTO "public"."cm_option" VALUES (77, '黑龙江集贤县', 77, 3);
INSERT INTO "public"."cm_option" VALUES (78, '黑龙江大庆市', 78, 3);
INSERT INTO "public"."cm_option" VALUES (79, '黑龙江伊春市', 79, 3);
INSERT INTO "public"."cm_option" VALUES (80, '黑龙江嘉荫县', 80, 3);
INSERT INTO "public"."cm_option" VALUES (81, '黑龙江佳木斯市', 81, 3);
INSERT INTO "public"."cm_option" VALUES (82, '黑龙江桦南县', 82, 3);
INSERT INTO "public"."cm_option" VALUES (83, '黑龙江依兰县', 83, 3);
INSERT INTO "public"."cm_option" VALUES (84, '黑龙江桦川县', 84, 3);
INSERT INTO "public"."cm_option" VALUES (85, '黑龙江省宝清县', 85, 3);
INSERT INTO "public"."cm_option" VALUES (86, '黑龙江汤原县', 86, 3);
INSERT INTO "public"."cm_option" VALUES (87, '黑龙江饶河县', 87, 3);
INSERT INTO "public"."cm_option" VALUES (88, '黑龙江抚远县', 88, 3);
INSERT INTO "public"."cm_option" VALUES (89, '黑龙江友谊县', 89, 3);
INSERT INTO "public"."cm_option" VALUES (90, '黑龙江七台河市', 90, 3);
INSERT INTO "public"."cm_option" VALUES (91, '黑龙江省勃利县', 91, 3);
INSERT INTO "public"."cm_option" VALUES (92, '黑龙江牡丹江市', 92, 3);
INSERT INTO "public"."cm_option" VALUES (93, '黑龙江宁安县', 93, 3);
INSERT INTO "public"."cm_option" VALUES (94, '黑龙江海林县', 94, 3);
INSERT INTO "public"."cm_option" VALUES (95, '黑龙江穆棱县', 95, 3);
INSERT INTO "public"."cm_option" VALUES (96, '黑龙江东宁县', 96, 3);
INSERT INTO "public"."cm_option" VALUES (97, '黑龙江林口县', 97, 3);
INSERT INTO "public"."cm_option" VALUES (98, '黑龙江虎林县', 98, 3);
INSERT INTO "public"."cm_option" VALUES (99, '黑龙江双城市', 99, 3);
INSERT INTO "public"."cm_option" VALUES (100, '黑龙江尚志市', 100, 3);
INSERT INTO "public"."cm_option" VALUES (101, '黑龙江省宾县', 101, 3);
INSERT INTO "public"."cm_option" VALUES (102, '黑龙江五常县', 102, 3);
INSERT INTO "public"."cm_option" VALUES (103, '黑龙江省巴彦县', 103, 3);
INSERT INTO "public"."cm_option" VALUES (104, '黑龙江木兰县', 104, 3);
INSERT INTO "public"."cm_option" VALUES (105, '黑龙江通河县', 105, 3);
INSERT INTO "public"."cm_option" VALUES (106, '黑龙江方正县', 106, 3);
INSERT INTO "public"."cm_option" VALUES (107, '黑龙江延寿县', 107, 3);
INSERT INTO "public"."cm_option" VALUES (108, '黑龙江绥化市', 108, 3);
INSERT INTO "public"."cm_option" VALUES (109, '黑龙江省安达市', 109, 3);
INSERT INTO "public"."cm_option" VALUES (110, '黑龙江肇东市', 110, 3);
INSERT INTO "public"."cm_option" VALUES (111, '黑龙江海伦县', 111, 3);
INSERT INTO "public"."cm_option" VALUES (112, '黑龙江望奎县', 112, 3);
INSERT INTO "public"."cm_option" VALUES (113, '黑龙江兰西县', 113, 3);
INSERT INTO "public"."cm_option" VALUES (114, '黑龙江青冈县', 114, 3);
INSERT INTO "public"."cm_option" VALUES (115, '黑龙江肇源县', 115, 3);
INSERT INTO "public"."cm_option" VALUES (116, '黑龙江肇州县', 116, 3);
INSERT INTO "public"."cm_option" VALUES (117, '黑龙江庆安县', 117, 3);
INSERT INTO "public"."cm_option" VALUES (118, '黑龙江明水县', 118, 3);
INSERT INTO "public"."cm_option" VALUES (119, '黑龙江绥棱县', 119, 3);
INSERT INTO "public"."cm_option" VALUES (120, '黑龙江黑河市', 120, 3);
INSERT INTO "public"."cm_option" VALUES (121, '黑龙江省北安市', 121, 3);
INSERT INTO "public"."cm_option" VALUES (122, '黑龙江五大连池市', 122, 3);
INSERT INTO "public"."cm_option" VALUES (123, '黑龙江嫩江县', 123, 3);
INSERT INTO "public"."cm_option" VALUES (124, '黑龙江省德都县', 124, 3);
INSERT INTO "public"."cm_option" VALUES (125, '黑龙江逊克县', 125, 3);
INSERT INTO "public"."cm_option" VALUES (126, '黑龙江孙吴县', 126, 3);
INSERT INTO "public"."cm_option" VALUES (127, '黑龙江呼玛县', 127, 3);
INSERT INTO "public"."cm_option" VALUES (128, '黑龙江塔河县', 128, 3);
INSERT INTO "public"."cm_option" VALUES (129, '黑龙江漠河县', 129, 3);
INSERT INTO "public"."cm_option" VALUES (130, '黑龙江绥芬河市', 130, 3);
INSERT INTO "public"."cm_option" VALUES (131, '黑龙江省阿城市', 131, 3);
INSERT INTO "public"."cm_option" VALUES (132, '黑龙江同江市', 132, 3);
INSERT INTO "public"."cm_option" VALUES (133, '黑龙江富锦市', 133, 3);
INSERT INTO "public"."cm_option" VALUES (134, '黑龙江铁力市', 134, 3);
INSERT INTO "public"."cm_option" VALUES (135, '黑龙江密山市', 135, 3);
INSERT INTO "public"."cm_option" VALUES (136, '吉林省长春市', 136, 3);
INSERT INTO "public"."cm_option" VALUES (137, '内蒙古呼和浩特市', 137, 3);
INSERT INTO "public"."cm_option" VALUES (138, '内蒙古土默特左旗', 138, 3);
INSERT INTO "public"."cm_option" VALUES (139, '内蒙古托克托县', 139, 3);
INSERT INTO "public"."cm_option" VALUES (140, '内蒙古包头市', 140, 3);
INSERT INTO "public"."cm_option" VALUES (141, '内蒙古土默特右旗', 141, 3);
INSERT INTO "public"."cm_option" VALUES (142, '内蒙古固阳县', 142, 3);
INSERT INTO "public"."cm_option" VALUES (143, '内蒙古乌海市', 143, 3);
INSERT INTO "public"."cm_option" VALUES (144, '内蒙古赤峰市', 144, 3);
INSERT INTO "public"."cm_option" VALUES (145, '内蒙古阿鲁科尔沁旗', 145, 3);
INSERT INTO "public"."cm_option" VALUES (146, '内蒙古巴林左旗', 146, 3);
INSERT INTO "public"."cm_option" VALUES (147, '内蒙古巴林右旗', 147, 3);
INSERT INTO "public"."cm_option" VALUES (148, '内蒙古林西县', 148, 3);
INSERT INTO "public"."cm_option" VALUES (149, '内蒙古克什克腾旗', 149, 3);
INSERT INTO "public"."cm_option" VALUES (150, '内蒙古翁牛特旗', 150, 3);
INSERT INTO "public"."cm_option" VALUES (151, '内蒙古喀喇沁旗', 151, 3);
INSERT INTO "public"."cm_option" VALUES (152, '内蒙古宁城县', 152, 3);
INSERT INTO "public"."cm_option" VALUES (153, '内蒙古敖汉旗', 153, 3);
INSERT INTO "public"."cm_option" VALUES (154, '内蒙古海拉尔市', 154, 3);
INSERT INTO "public"."cm_option" VALUES (155, '内蒙古满州里市', 155, 3);
INSERT INTO "public"."cm_option" VALUES (156, '内蒙古扎兰屯市', 156, 3);
INSERT INTO "public"."cm_option" VALUES (157, '内蒙古牙克石市', 157, 3);
INSERT INTO "public"."cm_option" VALUES (158, '内蒙古阿荣旗', 158, 3);
INSERT INTO "public"."cm_option" VALUES (159, '内蒙古莫力县', 159, 3);
INSERT INTO "public"."cm_option" VALUES (160, '内蒙古额尔古纳右旗', 160, 3);
INSERT INTO "public"."cm_option" VALUES (161, '内蒙古额尔古纳左旗', 161, 3);
INSERT INTO "public"."cm_option" VALUES (162, '内蒙古鄂伦春自治旗', 162, 3);
INSERT INTO "public"."cm_option" VALUES (163, '内蒙古鄂温克族自治旗', 163, 3);
INSERT INTO "public"."cm_option" VALUES (164, '内蒙古新巴尔虎右旗', 164, 3);
INSERT INTO "public"."cm_option" VALUES (165, '内蒙古新巴尔虎左旗', 165, 3);
INSERT INTO "public"."cm_option" VALUES (166, '内蒙古陈巴尔虎旗', 166, 3);
INSERT INTO "public"."cm_option" VALUES (167, '内蒙古乌兰浩特市', 167, 3);
INSERT INTO "public"."cm_option" VALUES (168, '内蒙古科尔沁右翼前旗', 168, 3);
INSERT INTO "public"."cm_option" VALUES (169, '内蒙古科尔沁右翼中旗', 169, 3);
INSERT INTO "public"."cm_option" VALUES (170, '内蒙古扎赉特旗', 170, 3);
INSERT INTO "public"."cm_option" VALUES (171, '内蒙古突泉县', 171, 3);
INSERT INTO "public"."cm_option" VALUES (172, '内蒙古通辽市', 172, 3);
INSERT INTO "public"."cm_option" VALUES (173, '内蒙古霍林郭勒市', 173, 3);
INSERT INTO "public"."cm_option" VALUES (174, '内蒙古科尔沁左翼中旗', 174, 3);
INSERT INTO "public"."cm_option" VALUES (175, '内蒙古科尔沁左翼后旗', 175, 3);
INSERT INTO "public"."cm_option" VALUES (176, '内蒙古开鲁县', 176, 3);
INSERT INTO "public"."cm_option" VALUES (177, '内蒙古库伦旗', 177, 3);
INSERT INTO "public"."cm_option" VALUES (178, '内蒙古奈曼旗', 178, 3);
INSERT INTO "public"."cm_option" VALUES (179, '内蒙古扎鲁特旗', 179, 3);
INSERT INTO "public"."cm_option" VALUES (180, '内蒙古二连浩特市', 180, 3);
INSERT INTO "public"."cm_option" VALUES (181, '内蒙古锡林浩特市', 181, 3);
INSERT INTO "public"."cm_option" VALUES (182, '内蒙古阿巴嘎旗', 182, 3);
INSERT INTO "public"."cm_option" VALUES (183, '内蒙古苏尼特左旗', 183, 3);
INSERT INTO "public"."cm_option" VALUES (184, '内蒙古苏尼特右旗', 184, 3);
INSERT INTO "public"."cm_option" VALUES (185, '内蒙古东乌珠穆沁旗', 185, 3);
INSERT INTO "public"."cm_option" VALUES (186, '内蒙古西乌珠穆沁旗', 186, 3);
INSERT INTO "public"."cm_option" VALUES (187, '内蒙古太仆寺旗', 187, 3);
INSERT INTO "public"."cm_option" VALUES (188, '内蒙古镶黄旗', 188, 3);
INSERT INTO "public"."cm_option" VALUES (189, '内蒙古正镶白旗', 189, 3);
INSERT INTO "public"."cm_option" VALUES (190, '内蒙古正蓝旗', 190, 3);
INSERT INTO "public"."cm_option" VALUES (191, '内蒙古多伦县', 191, 3);
INSERT INTO "public"."cm_option" VALUES (192, '内蒙古集宁市', 192, 3);
INSERT INTO "public"."cm_option" VALUES (193, '内蒙古武川县', 193, 3);
INSERT INTO "public"."cm_option" VALUES (194, '内蒙古和林格尔县', 194, 3);
INSERT INTO "public"."cm_option" VALUES (195, '内蒙古清水河县', 195, 3);
INSERT INTO "public"."cm_option" VALUES (196, '内蒙古卓资县', 196, 3);
INSERT INTO "public"."cm_option" VALUES (197, '内蒙古化德县', 197, 3);
INSERT INTO "public"."cm_option" VALUES (198, '内蒙古商都县', 198, 3);
INSERT INTO "public"."cm_option" VALUES (199, '内蒙古兴和县', 199, 3);
INSERT INTO "public"."cm_option" VALUES (200, '内蒙古丰镇县', 200, 3);
INSERT INTO "public"."cm_option" VALUES (201, '内蒙古凉城县', 201, 3);
INSERT INTO "public"."cm_option" VALUES (202, '内蒙古察哈尔右翼前旗', 202, 3);
INSERT INTO "public"."cm_option" VALUES (203, '内蒙古察哈尔右翼中旗', 203, 3);
INSERT INTO "public"."cm_option" VALUES (204, '内蒙古察哈尔右翼后旗', 204, 3);
INSERT INTO "public"."cm_option" VALUES (205, '内蒙古达尔罕茂明安联', 205, 3);
INSERT INTO "public"."cm_option" VALUES (206, '内蒙古四子王旗', 206, 3);
INSERT INTO "public"."cm_option" VALUES (207, '内蒙古东胜市', 207, 3);
INSERT INTO "public"."cm_option" VALUES (208, '内蒙古达拉特旗', 208, 3);
INSERT INTO "public"."cm_option" VALUES (209, '内蒙古准格尔旗', 209, 3);
INSERT INTO "public"."cm_option" VALUES (210, '内蒙古鄂托克前旗', 210, 3);
INSERT INTO "public"."cm_option" VALUES (211, '内蒙古鄂托克旗', 211, 3);
INSERT INTO "public"."cm_option" VALUES (212, '内蒙古杭锦旗', 212, 3);
INSERT INTO "public"."cm_option" VALUES (213, '内蒙古乌审旗', 213, 3);
INSERT INTO "public"."cm_option" VALUES (214, '内蒙古伊金霍洛旗', 214, 3);
INSERT INTO "public"."cm_option" VALUES (215, '内蒙古临河市', 215, 3);
INSERT INTO "public"."cm_option" VALUES (216, '内蒙古五原县', 216, 3);
INSERT INTO "public"."cm_option" VALUES (217, '内蒙古磴口县', 217, 3);
INSERT INTO "public"."cm_option" VALUES (218, '内蒙古乌拉特前旗', 218, 3);
INSERT INTO "public"."cm_option" VALUES (219, '内蒙古乌拉特中旗', 219, 3);
INSERT INTO "public"."cm_option" VALUES (220, '内蒙古乌拉特后旗', 220, 3);
INSERT INTO "public"."cm_option" VALUES (221, '内蒙古杭锦后旗', 221, 3);
INSERT INTO "public"."cm_option" VALUES (222, '内蒙古阿拉善左旗', 222, 3);
INSERT INTO "public"."cm_option" VALUES (223, '内蒙古阿拉善右旗', 223, 3);
INSERT INTO "public"."cm_option" VALUES (224, '内蒙古额济纳旗', 224, 3);
INSERT INTO "public"."cm_option" VALUES (225, '辽宁省', 225, 3);
INSERT INTO "public"."cm_option" VALUES (226, '辽宁省沈阳市', 226, 3);
INSERT INTO "public"."cm_option" VALUES (227, '辽宁省新民县', 227, 3);
INSERT INTO "public"."cm_option" VALUES (228, '辽宁省辽中县', 228, 3);
INSERT INTO "public"."cm_option" VALUES (229, '辽宁省大连市', 229, 3);
INSERT INTO "public"."cm_option" VALUES (230, '辽宁省新金县', 230, 3);
INSERT INTO "public"."cm_option" VALUES (231, '辽宁省长海县', 231, 3);
INSERT INTO "public"."cm_option" VALUES (232, '辽宁省庄河县', 232, 3);
INSERT INTO "public"."cm_option" VALUES (233, '辽宁省鞍山市', 233, 3);
INSERT INTO "public"."cm_option" VALUES (234, '辽宁省台安县', 234, 3);
INSERT INTO "public"."cm_option" VALUES (235, '辽宁省抚顺市', 235, 3);
INSERT INTO "public"."cm_option" VALUES (236, '辽宁省抚顺县', 236, 3);
INSERT INTO "public"."cm_option" VALUES (237, '辽宁省新宾县', 237, 3);
INSERT INTO "public"."cm_option" VALUES (238, '辽宁省清原县', 238, 3);
INSERT INTO "public"."cm_option" VALUES (239, '辽宁省本溪市', 239, 3);
INSERT INTO "public"."cm_option" VALUES (240, '辽宁省本溪县', 240, 3);
INSERT INTO "public"."cm_option" VALUES (241, '辽宁省桓仁县', 241, 3);
INSERT INTO "public"."cm_option" VALUES (242, '辽宁省丹东市', 242, 3);
INSERT INTO "public"."cm_option" VALUES (243, '辽宁省凤城县', 243, 3);
INSERT INTO "public"."cm_option" VALUES (244, '辽宁省岫岩县', 244, 3);
INSERT INTO "public"."cm_option" VALUES (245, '辽宁省东沟县', 245, 3);
INSERT INTO "public"."cm_option" VALUES (246, '辽宁省宽甸县', 246, 3);
INSERT INTO "public"."cm_option" VALUES (247, '辽宁省锦州市', 247, 3);
INSERT INTO "public"."cm_option" VALUES (248, '辽宁省绥中县', 248, 3);
INSERT INTO "public"."cm_option" VALUES (249, '辽宁省锦  县', 249, 3);
INSERT INTO "public"."cm_option" VALUES (250, '辽宁省北镇县', 250, 3);
INSERT INTO "public"."cm_option" VALUES (251, '辽宁省黑山县', 251, 3);
INSERT INTO "public"."cm_option" VALUES (252, '辽宁省义  县', 252, 3);
INSERT INTO "public"."cm_option" VALUES (253, '辽宁省营口市', 253, 3);
INSERT INTO "public"."cm_option" VALUES (254, '辽宁省营口县', 254, 3);
INSERT INTO "public"."cm_option" VALUES (255, '辽宁省盖  县', 255, 3);
INSERT INTO "public"."cm_option" VALUES (256, '辽宁省阜新市', 256, 3);
INSERT INTO "public"."cm_option" VALUES (257, '辽宁省阜新县', 257, 3);
INSERT INTO "public"."cm_option" VALUES (258, '辽宁省彰武县', 258, 3);
INSERT INTO "public"."cm_option" VALUES (259, '辽宁省辽阳市', 259, 3);
INSERT INTO "public"."cm_option" VALUES (260, '辽宁省辽阳县', 260, 3);
INSERT INTO "public"."cm_option" VALUES (261, '辽宁省灯塔县', 261, 3);
INSERT INTO "public"."cm_option" VALUES (262, '辽宁省盘锦市', 262, 3);
INSERT INTO "public"."cm_option" VALUES (263, '辽宁省大洼县', 263, 3);
INSERT INTO "public"."cm_option" VALUES (264, '辽宁省盘山县', 264, 3);
INSERT INTO "public"."cm_option" VALUES (265, '辽宁省铁岭市', 265, 3);
INSERT INTO "public"."cm_option" VALUES (266, '辽宁省铁岭县', 266, 3);
INSERT INTO "public"."cm_option" VALUES (267, '辽宁省西丰县', 267, 3);
INSERT INTO "public"."cm_option" VALUES (268, '辽宁省昌图县', 268, 3);
INSERT INTO "public"."cm_option" VALUES (269, '辽宁省康平县', 269, 3);
INSERT INTO "public"."cm_option" VALUES (270, '辽宁省法库县', 270, 3);
INSERT INTO "public"."cm_option" VALUES (271, '辽宁省朝阳市', 271, 3);
INSERT INTO "public"."cm_option" VALUES (272, '辽宁省朝阳县', 272, 3);
INSERT INTO "public"."cm_option" VALUES (273, '辽宁省建平县', 273, 3);
INSERT INTO "public"."cm_option" VALUES (274, '辽宁省凌源县', 274, 3);
INSERT INTO "public"."cm_option" VALUES (275, '辽宁省喀喇沁县', 275, 3);
INSERT INTO "public"."cm_option" VALUES (276, '辽宁省建昌县', 276, 3);
INSERT INTO "public"."cm_option" VALUES (277, '辽宁省直辖行政单位', 277, 3);
INSERT INTO "public"."cm_option" VALUES (278, '辽宁省瓦房店市', 278, 3);
INSERT INTO "public"."cm_option" VALUES (279, '辽宁省海城市', 279, 3);
INSERT INTO "public"."cm_option" VALUES (280, '辽宁省锦西市', 280, 3);
INSERT INTO "public"."cm_option" VALUES (281, '辽宁省兴城市', 281, 3);
INSERT INTO "public"."cm_option" VALUES (282, '辽宁省铁法市', 282, 3);
INSERT INTO "public"."cm_option" VALUES (283, '辽宁省北票市', 283, 3);
INSERT INTO "public"."cm_option" VALUES (284, '辽宁省开原市', 284, 3);
INSERT INTO "public"."cm_option" VALUES (285, '吉林省', 285, 3);
INSERT INTO "public"."cm_option" VALUES (286, '吉林省榆树县', 286, 3);
INSERT INTO "public"."cm_option" VALUES (287, '吉林省农安县', 287, 3);
INSERT INTO "public"."cm_option" VALUES (288, '吉林省德惠县', 288, 3);
INSERT INTO "public"."cm_option" VALUES (289, '吉林省双阳县', 289, 3);
INSERT INTO "public"."cm_option" VALUES (290, '吉林省吉林市', 290, 3);
INSERT INTO "public"."cm_option" VALUES (291, '吉林省永吉县', 291, 3);
INSERT INTO "public"."cm_option" VALUES (292, '吉林省舒兰县', 292, 3);
INSERT INTO "public"."cm_option" VALUES (293, '吉林省磐石县', 293, 3);
INSERT INTO "public"."cm_option" VALUES (294, '吉林省蛟河县', 294, 3);
INSERT INTO "public"."cm_option" VALUES (295, '吉林省四平市', 295, 3);
INSERT INTO "public"."cm_option" VALUES (296, '吉林省梨树县', 296, 3);
INSERT INTO "public"."cm_option" VALUES (297, '吉林省伊通县', 297, 3);
INSERT INTO "public"."cm_option" VALUES (298, '吉林省双辽县', 298, 3);
INSERT INTO "public"."cm_option" VALUES (299, '吉林省辽源市', 299, 3);
INSERT INTO "public"."cm_option" VALUES (300, '吉林省东丰县', 300, 3);
INSERT INTO "public"."cm_option" VALUES (301, '吉林省东辽县', 301, 3);
INSERT INTO "public"."cm_option" VALUES (302, '吉林省通化市', 302, 3);
INSERT INTO "public"."cm_option" VALUES (303, '吉林省通化县', 303, 3);
INSERT INTO "public"."cm_option" VALUES (304, '吉林省辉南县', 304, 3);
INSERT INTO "public"."cm_option" VALUES (305, '吉林省柳河县', 305, 3);
INSERT INTO "public"."cm_option" VALUES (306, '吉林省浑江市', 306, 3);
INSERT INTO "public"."cm_option" VALUES (307, '吉林省抚松县', 307, 3);
INSERT INTO "public"."cm_option" VALUES (308, '吉林省靖宇县', 308, 3);
INSERT INTO "public"."cm_option" VALUES (309, '吉林省长白县', 309, 3);
INSERT INTO "public"."cm_option" VALUES (310, '吉林省白城地区', 310, 3);
INSERT INTO "public"."cm_option" VALUES (311, '吉林省白城市', 311, 3);
INSERT INTO "public"."cm_option" VALUES (312, '吉林省洮南市', 312, 3);
INSERT INTO "public"."cm_option" VALUES (313, '吉林省扶余市', 313, 3);
INSERT INTO "public"."cm_option" VALUES (314, '吉林省大安市', 314, 3);
INSERT INTO "public"."cm_option" VALUES (315, '吉林省长岭县', 315, 3);
INSERT INTO "public"."cm_option" VALUES (316, '吉林省前郭尔罗斯县', 316, 3);
INSERT INTO "public"."cm_option" VALUES (317, '吉林省镇赉县', 317, 3);
INSERT INTO "public"."cm_option" VALUES (318, '吉林省通榆县', 318, 3);
INSERT INTO "public"."cm_option" VALUES (319, '吉林省乾安县', 319, 3);
INSERT INTO "public"."cm_option" VALUES (320, '吉林省延吉市', 320, 3);
INSERT INTO "public"."cm_option" VALUES (321, '吉林省图们市', 321, 3);
INSERT INTO "public"."cm_option" VALUES (322, '吉林省敦化市', 322, 3);
INSERT INTO "public"."cm_option" VALUES (323, '吉林省珲春市', 323, 3);
INSERT INTO "public"."cm_option" VALUES (324, '吉林省龙井市', 324, 3);
INSERT INTO "public"."cm_option" VALUES (325, '吉林省和龙县', 325, 3);
INSERT INTO "public"."cm_option" VALUES (326, '吉林省汪清县', 326, 3);
INSERT INTO "public"."cm_option" VALUES (327, '吉林省安图县', 327, 3);
INSERT INTO "public"."cm_option" VALUES (328, '吉林省公主岭市', 328, 3);
INSERT INTO "public"."cm_option" VALUES (329, '吉林省梅河口市', 329, 3);
INSERT INTO "public"."cm_option" VALUES (330, '吉林省集安市', 330, 3);
INSERT INTO "public"."cm_option" VALUES (331, '吉林省桦甸市', 331, 3);
INSERT INTO "public"."cm_option" VALUES (332, '吉林省九台市', 332, 3);
INSERT INTO "public"."cm_option" VALUES (333, '黑龙江省', 333, 3);
INSERT INTO "public"."cm_option" VALUES (334, '黑龙江哈尔滨市', 334, 3);
INSERT INTO "public"."cm_option" VALUES (335, '黑龙江呼兰县', 335, 3);
INSERT INTO "public"."cm_option" VALUES (336, '黑龙江齐齐哈尔市', 336, 3);
INSERT INTO "public"."cm_option" VALUES (337, '黑龙江龙江县', 337, 3);
INSERT INTO "public"."cm_option" VALUES (338, '黑龙江讷河县', 338, 3);
INSERT INTO "public"."cm_option" VALUES (339, '黑龙江依安县', 339, 3);
INSERT INTO "public"."cm_option" VALUES (340, '黑龙江泰来县', 340, 3);
INSERT INTO "public"."cm_option" VALUES (341, '黑龙江甘南县', 341, 3);
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
INSERT INTO "public"."cm_option" VALUES (456661440205443072, '1', 1023, 456659310463700992);
INSERT INTO "public"."cm_option" VALUES (456662703420755968, '2', 1026, 456659310463700992);

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
INSERT INTO "public"."cm_option_group" VALUES (456659310463700992, '新组');

-- ----------------------------
-- Table structure for cm_params
-- ----------------------------
CREATE TABLE "public"."cm_params" (
  "id" int8 NOT NULL,
  "name" varchar(255) NOT NULL,
  "value" varchar(255) NOT NULL,
  "note" varchar(255) NOT NULL,
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
CREATE TABLE "public"."cm_permission" (
  "id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
  "note" varchar(255)
)
;
COMMENT ON COLUMN "public"."cm_permission"."id" IS '权限标识';
COMMENT ON COLUMN "public"."cm_permission"."name" IS '权限名称';
COMMENT ON COLUMN "public"."cm_permission"."note" IS '权限描述';
COMMENT ON TABLE "public"."cm_permission" IS '权限';

-- ----------------------------
-- Records of cm_permission
-- ----------------------------
INSERT INTO "public"."cm_permission" VALUES (1, '公共文件管理', '禁止删除');
INSERT INTO "public"."cm_permission" VALUES (2, '素材库管理', '禁止删除');
INSERT INTO "public"."cm_permission" VALUES (455253883184238592, '测试1', '');

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
INSERT INTO "public"."cm_role" VALUES (22844822693027840, '收发员', '');
INSERT INTO "public"."cm_role" VALUES (152695933758603264, '市场经理', '');
INSERT INTO "public"."cm_role" VALUES (152696004814307328, '综合经理', '');
INSERT INTO "public"."cm_role" VALUES (152696042718232576, '财务经理', '');

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
INSERT INTO "public"."cm_role_menu" VALUES (1, 15315637929975808);
INSERT INTO "public"."cm_role_menu" VALUES (2, 18860286065975296);
INSERT INTO "public"."cm_role_menu" VALUES (22844822693027840, 154430055023640576);

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
INSERT INTO "public"."cm_role_per" VALUES (1, 1);
INSERT INTO "public"."cm_role_per" VALUES (1, 2);
INSERT INTO "public"."cm_role_per" VALUES (22844822693027840, 455253883184238592);
INSERT INTO "public"."cm_role_per" VALUES (152696004814307328, 455253883184238592);

-- ----------------------------
-- Table structure for cm_rpt
-- ----------------------------
CREATE TABLE "public"."cm_rpt" (
  "id" int8 NOT NULL,
  "name" varchar(64) NOT NULL,
  "define" varchar(65535) NOT NULL,
  "note" varchar(255) NOT NULL,
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
INSERT INTO "public"."cm_rpt" VALUES (139241259579338752, '测试报表111', '<Rpt cols="80,80,80,80,80,80,80">
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
</Rpt>', '新增测试1', '2020-10-19 13:35:10', '2023-06-28 08:39:08');
INSERT INTO "public"."cm_rpt" VALUES (139540400075304960, 'abc1', '<Rpt cols="80,80,80,80,80">
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
</Rpt>', '阿斯顿法定', '2020-10-20 09:24:01', '2023-03-13 16:14:41');
INSERT INTO "public"."cm_rpt" VALUES (150118388697264128, 'abc12', '', '', '2020-11-18 13:57:21', '2020-11-18 13:57:21');
INSERT INTO "public"."cm_rpt" VALUES (154424288497369088, '新报表abc', '', '', '2020-11-30 11:07:07', '2020-11-30 11:07:07');
INSERT INTO "public"."cm_rpt" VALUES (259588273038290944, '新报表3', '', '', '2021-09-16 15:51:31', '2021-09-16 15:51:53');

-- ----------------------------
-- Table structure for cm_user
-- ----------------------------
CREATE TABLE "public"."cm_user" (
  "id" int8 NOT NULL,
  "phone" char(11) NOT NULL,
  "name" varchar(32) NOT NULL,
  "pwd" char(32) NOT NULL,
  "sex" int2 NOT NULL,
  "photo" varchar(255) NOT NULL,
  "expired" bool NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "mtime" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."cm_user"."id" IS '用户标识';
COMMENT ON COLUMN "public"."cm_user"."phone" IS '手机号，唯一';
COMMENT ON COLUMN "public"."cm_user"."name" IS '姓名';
COMMENT ON COLUMN "public"."cm_user"."pwd" IS '密码的md5';
COMMENT ON COLUMN "public"."cm_user"."sex" IS '#Gender#性别';
COMMENT ON COLUMN "public"."cm_user"."photo" IS '头像';
COMMENT ON COLUMN "public"."cm_user"."expired" IS '是否停用';
COMMENT ON COLUMN "public"."cm_user"."ctime" IS '创建时间';
COMMENT ON COLUMN "public"."cm_user"."mtime" IS '修改时间';
COMMENT ON TABLE "public"."cm_user" IS '系统用户';

-- ----------------------------
-- Records of cm_user
-- ----------------------------
INSERT INTO "public"."cm_user" VALUES (1, '13511111111', 'Windows', 'af3303f852abeccd793068486a391626', 1, '[["photo/1.jpg","1","300 x 300 (.jpg)",49179,"daoting","2020-03-13 10:37"]]', '0', '2019-10-24 09:06:38', '2023-03-16 08:35:39');
INSERT INTO "public"."cm_user" VALUES (2, '13522222222', '安卓', 'b59c67bf196a4758191e42f76670ceba', 2, '[["photo/2.jpg","2","300 x 300 (.jpg)",49179,"daoting","2020-03-13 10:37"]]', '0', '2019-10-24 13:03:19', '2023-03-16 08:36:23');
INSERT INTO "public"."cm_user" VALUES (3, '13533333333', '苹果', '674f3c2c1a8a6f90461e8a66fb5550ba', 1, '[["photo/3.jpg","3","300 x 300 (.jpg)",49179,"daoting","2020-03-13 10:37"]]', '0', '0001-01-01 00:00:00', '2023-03-16 08:36:46');
INSERT INTO "public"."cm_user" VALUES (149709966847897600, '13122222222', '李市场', '934b535800b1cba8f96a5d72f72f1611', 1, '', '0', '2020-11-17 10:54:29', '2020-11-25 16:37:55');
INSERT INTO "public"."cm_user" VALUES (152695627289198592, '13211111111', '王综合', 'b59c67bf196a4758191e42f76670ceba', 1, '', '0', '2020-11-25 16:38:34', '2020-11-25 16:38:34');
INSERT INTO "public"."cm_user" VALUES (152695790787362816, '13866666666', '张财务', 'e9510081ac30ffa83f10b68cde1cac07', 1, '', '0', '2020-11-25 16:38:54', '2020-11-25 16:38:54');
INSERT INTO "public"."cm_user" VALUES (184215437633777664, '15955555555', '15955555555', '6074c6aa3488f3c2dddff2a7ca821aab', 1, '', '0', '2021-02-20 16:06:23', '2021-02-20 16:06:23');
INSERT INTO "public"."cm_user" VALUES (185188338092601344, '15912345678', '15912345678', '674f3c2c1a8a6f90461e8a66fb5550ba', 1, '', '0', '2021-02-23 08:32:20', '2021-02-23 08:32:20');
INSERT INTO "public"."cm_user" VALUES (185212597401677824, '15912345671', '15912345677', 'cca8f108b55ec9e39d7885e24f7da0af', 2, '', '0', '2021-02-23 10:08:43', '2022-01-19 15:49:43');
INSERT INTO "public"."cm_user" VALUES (192818293676994560, '18543175028', '18543175028', 'bf8dd8c68d02e161c28dc9ea139d4784', 1, '', '0', '2021-03-16 09:51:02', '2021-03-16 09:51:02');
INSERT INTO "public"."cm_user" VALUES (196167762048839680, '18843175028', '18843175028', 'bf8dd8c68d02e161c28dc9ea139d4784', 1, '', '0', '2021-03-25 15:40:38', '2021-03-25 15:40:38');
INSERT INTO "public"."cm_user" VALUES (224062063923556352, '14411111111', '14411111111', 'b59c67bf196a4758191e42f76670ceba', 1, '', '0', '2021-06-10 15:02:39', '2021-06-10 15:02:39');
INSERT INTO "public"."cm_user" VALUES (227949556179791872, '13612345678', 'WebAssembly', '674f3c2c1a8a6f90461e8a66fb5550ba', 1, '', '0', '2021-06-21 08:30:10', '2021-06-21 08:30:34');
INSERT INTO "public"."cm_user" VALUES (229519641138819072, '13311111111', '13311111111', 'b59c67bf196a4758191e42f76670ceba', 1, '[["editor/E3/18/452737920958222336.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2023-03-03 15:38"]]', '0', '2021-06-25 16:29:06', '2021-06-25 16:29:06');
INSERT INTO "public"."cm_user" VALUES (231620526086156288, '13611111111', '13611111111', 'b59c67bf196a4758191e42f76670ceba', 1, '', '0', '2021-07-01 11:37:18', '2021-07-01 11:37:18');
INSERT INTO "public"."cm_user" VALUES (247170018466197504, '15948341897', '15948341892', 'af3303f852abeccd793068486a391626', 1, '', '0', '2021-08-13 09:25:26', '2021-09-10 09:36:37');

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
-- Records of cm_user_group
-- ----------------------------
INSERT INTO "public"."cm_user_group" VALUES (1, 454483802783240192);
INSERT INTO "public"."cm_user_group" VALUES (1, 454484924033945600);
INSERT INTO "public"."cm_user_group" VALUES (149709966847897600, 454484847190102016);

-- ----------------------------
-- Table structure for cm_user_params
-- ----------------------------
CREATE TABLE "public"."cm_user_params" (
  "user_id" int8 NOT NULL,
  "param_id" int8 NOT NULL,
  "value" varchar(255) NOT NULL,
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
INSERT INTO "public"."cm_user_params" VALUES (2, 1, 'false', '2020-12-04 13:29:05');

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
INSERT INTO "public"."cm_user_role" VALUES (1, 22844822693027840);
INSERT INTO "public"."cm_user_role" VALUES (1, 152695933758603264);
INSERT INTO "public"."cm_user_role" VALUES (1, 152696004814307328);
INSERT INTO "public"."cm_user_role" VALUES (2, 2);
INSERT INTO "public"."cm_user_role" VALUES (2, 22844822693027840);
INSERT INTO "public"."cm_user_role" VALUES (2, 152695933758603264);
INSERT INTO "public"."cm_user_role" VALUES (3, 2);
INSERT INTO "public"."cm_user_role" VALUES (149709966847897600, 2);
INSERT INTO "public"."cm_user_role" VALUES (149709966847897600, 152695933758603264);
INSERT INTO "public"."cm_user_role" VALUES (152695627289198592, 152696004814307328);
INSERT INTO "public"."cm_user_role" VALUES (152695790787362816, 152696042718232576);
INSERT INTO "public"."cm_user_role" VALUES (247170018466197504, 22844822693027840);

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
-- Records of cm_wfd_atv
-- ----------------------------
INSERT INTO "public"."cm_wfd_atv" VALUES (146898715155492864, 146898695127691264, '开始', 1, 0, 0, NULL,       '1', '1', '0', '0', 0, 0, '2020-11-09 16:43:10', '2020-11-09 16:43:10');
INSERT INTO "public"."cm_wfd_atv" VALUES (146898876447453184, 146898695127691264, '任务项', 0, 0, 0, NULL,     '1', '0', '0', '0', 0, 0, '2020-11-09 16:43:48', '2020-11-09 16:43:48');
INSERT INTO "public"."cm_wfd_atv" VALUES (146900570585559040, 146900552231284736, '开始', 1, 0, 0, NULL,       '1', '1', '0', '0', 0, 0, '2020-11-09 16:50:32', '2020-11-09 16:50:32');
INSERT INTO "public"."cm_wfd_atv" VALUES (146900847761944576, 146900823984435200, '开始', 1, 0, 0, NULL,       '1', '1', '0', '0', 0, 0, '2020-11-09 16:51:38', '2020-11-09 16:51:38');
INSERT INTO "public"."cm_wfd_atv" VALUES (146901433265811456, 146901403339452416, '开始', 1, 0, 0, NULL,       '1', '1', '0', '0', 0, 0, '2020-11-09 16:53:58', '2020-11-09 16:53:58');
INSERT INTO "public"."cm_wfd_atv" VALUES (147141181158846464, 147141147767992320, '开始', 1, 0, 0, NULL,       '1', '1', '0', '0', 0, 0, '2020-11-10 08:46:31', '2020-11-10 08:46:31');
INSERT INTO "public"."cm_wfd_atv" VALUES (147141718000398336, 147141147767992320, '任务项', 0, 0, 0, NULL,     '1', '0', '0', '0', 0, 0, '2020-11-10 08:48:39', '2020-11-10 08:48:39');
INSERT INTO "public"."cm_wfd_atv" VALUES (152588671081775104, 152588581545967616, '接收文件', 1, 0, 0, NULL,   '1', '1', '0', '0', 0, 0, '2020-11-25 09:32:55', '2020-12-09 10:45:33');
INSERT INTO "public"."cm_wfd_atv" VALUES (152683112727576576, 152588581545967616, '市场部', 0, 0, 0, NULL,     '1', '0', '0', '0', 2, 0, '2020-11-25 15:48:12', '2020-12-14 15:36:36');
INSERT INTO "public"."cm_wfd_atv" VALUES (152684512937246720, 152588581545967616, '综合部', 0, 2, 0, NULL,     '1', '0', '0', '0', 2, 0, '2020-11-25 15:53:46', '2020-12-14 15:33:30');
INSERT INTO "public"."cm_wfd_atv" VALUES (152684758027206656, 152588581545967616, '市场部传阅', 0, 0, 0, NULL,  '1', '0', '0', '0', 0, 0, '2020-11-25 15:54:44', '2020-11-25 15:56:10');
INSERT INTO "public"."cm_wfd_atv" VALUES (152684895835258880, 152588581545967616, '同步', 2, 0, 0, NULL,       '1', '0', '0', '0', 0, 2, '2020-11-25 15:55:17', '2020-12-16 08:39:31');
INSERT INTO "public"."cm_wfd_atv" VALUES (152685032993193984, 152588581545967616, '综合部传阅', 0, 0, 0, NULL,  '1', '0', '0', '0', 0, 0, '2020-11-25 15:55:50', '2020-11-25 15:56:10');
INSERT INTO "public"."cm_wfd_atv" VALUES (152685491275431936, 152588581545967616, '返回收文人', 0, 0, 0, NULL,  '1', '0', '0', '0', 0, 0, '2020-11-25 15:57:39', '2020-11-25 15:58:18');
INSERT INTO "public"."cm_wfd_atv" VALUES (152685608543977472, 152588581545967616, '完成', 3, 0, 0, NULL,       '1', '0', '0', '0', 0, 0, '2020-11-25 15:58:07', '2020-11-25 15:58:07');

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
-- Records of cm_wfd_atv_role
-- ----------------------------
INSERT INTO "public"."cm_wfd_atv_role" VALUES (146898715155492864, 1);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (146900570585559040, 1);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (146900847761944576, 1);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (146901433265811456, 1);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (146898715155492864, 2);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (146900570585559040, 2);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (146901433265811456, 2);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (152588671081775104, 22844822693027840);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (152684758027206656, 22844822693027840);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (152685032993193984, 22844822693027840);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (152685491275431936, 22844822693027840);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (152683112727576576, 152695933758603264);
INSERT INTO "public"."cm_wfd_atv_role" VALUES (152684512937246720, 152696004814307328);

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
-- Records of cm_wfd_prc
-- ----------------------------
INSERT INTO "public"."cm_wfd_prc" VALUES (146898695127691264, '555', '<Sketch><Node id="146898715155492864" title="开始" shape="开始" left="340" top="100" width="80" height="60" /><Node id="146898876447453184" title="任务项" shape="任务" left="340" top="360" width="120" height="60" /><Line id="146898896794021888" headerid="146898715155492864" bounds="380,160,30,200" headerport="4" tailid="146898876447453184" tailport="0" /></Sketch>', '0', '0', '', 1, '0001-01-01 00:00:00', '2020-11-19 13:17:25');
INSERT INTO "public"."cm_wfd_prc" VALUES (146900552231284736, '666', '<Sketch><Node id="146900570585559040" title="开始" shape="开始" left="620" top="120" width="80" height="60" /></Sketch>', '0', '0', '', 3, '0001-01-01 00:00:00', '2020-11-09 16:50:56');
INSERT INTO "public"."cm_wfd_prc" VALUES (146900823984435200, '777', '<Sketch><Node id="146900847761944576" title="开始" shape="开始" left="300" top="220" width="80" height="60" /></Sketch>', '0', '0', '', 4, '0001-01-01 00:00:00', '2020-11-09 16:52:58');
INSERT INTO "public"."cm_wfd_prc" VALUES (146901403339452416, '888', '<Sketch><Node id="146901433265811456" title="开始" shape="开始" left="340" top="140" width="80" height="60" /></Sketch>', '0', '0', '', 6, '0001-01-01 00:00:00', '2020-11-09 16:54:39');
INSERT INTO "public"."cm_wfd_prc" VALUES (147141147767992320, 'ggg', '<Sketch><Node id="147141181158846464" title="开始" shape="开始" left="320" top="40" width="80" height="60" /><Node id="147141718000398336" title="任务项" shape="任务" left="380" top="480" width="120" height="60" /><Line id="147141749642227712" headerid="147141181158846464" bounds="400,100,50,380" headerport="3" tailid="147141718000398336" tailport="0" /></Sketch>', '1', '0', '', 2, '2020-11-10 08:46:24', '2020-11-10 08:50:03');
INSERT INTO "public"."cm_wfd_prc" VALUES (152588581545967616, '收文样例', '<Sketch><Node id="152588671081775104" title="接收文件" shape="开始" left="300" top="40" width="80" height="60" /><Node id="152683112727576576" title="市场部" shape="任务" left="160" top="140" width="120" height="60" /><Line id="152683122982649856" headerid="152588671081775104" bounds="210,70,50,70" headerport="6" tailid="152683112727576576" tailport="0" /><Node id="152684512937246720" title="综合部" shape="任务" left="400" top="140" width="120" height="60" /><Line id="152684673721696256" headerid="152588671081775104" bounds="380,70,90,70" headerport="2" tailid="152684512937246720" tailport="0" /><Node id="152684758027206656" title="市场部传阅" shape="任务" left="160" top="260" width="120" height="60" /><Node id="152684895835258880" title="同步" shape="同步" background="#FF9D9D9D" borderbrush="#FF969696" left="280" top="400" width="120" height="60" /><Line id="152684951493672960" headerid="152683112727576576" bounds="210,200,20,60" headerport="4" tailid="152684758027206656" tailport="0" /><Line id="152684981348728832" headerid="152683112727576576" bounds="120,170,160,470" headerport="6" tailid="152685608543977472" tailport="6" /><Node id="152685032993193984" title="综合部传阅" shape="任务" left="400" top="260" width="120" height="60" /><Line id="152685133509689344" headerid="152684512937246720" bounds="450,200,20,60" headerport="4" tailid="152685032993193984" tailport="0" /><Line id="152685169891082240" headerid="152684512937246720" bounds="400,170,160,270" headerport="2" tailid="152684895835258880" tailport="2" /><Line id="152685211767013376" headerid="152684758027206656" bounds="220,320,60,120" headerport="4" tailid="152684895835258880" tailport="6" /><Line id="152685247745753088" headerid="152685032993193984" bounds="400,320,60,120" headerport="4" tailid="152684895835258880" tailport="2" /><Node id="152685491275431936" title="返回收文人" shape="任务" left="280" top="500" width="120" height="60" /><Line id="152685585135566848" headerid="152684895835258880" bounds="330,460,20,40" headerport="4" tailid="152685491275431936" tailport="0" /><Node id="152685608543977472" title="完成" shape="结束" background="#FF9D9D9D" borderbrush="#FF969696" left="300" top="600" width="80" height="60" /><Line id="152685622099968000" headerid="152685491275431936" bounds="330,560,20,40" headerport="4" tailid="152685608543977472" tailport="0" /></Sketch>', '0', '0', '', 5, '2020-11-25 09:32:33', '2021-08-24 15:45:54');

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
-- Records of cm_wfd_trs
-- ----------------------------
INSERT INTO "public"."cm_wfd_trs" VALUES (146898896794021888, 146898695127691264, 146898715155492864, 146898876447453184, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (147141749642227712, 147141147767992320, 147141181158846464, 147141718000398336, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (152683122982649856, 152588581545967616, 152588671081775104, 152683112727576576, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (152684673721696256, 152588581545967616, 152588671081775104, 152684512937246720, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (152684951493672960, 152588581545967616, 152683112727576576, 152684758027206656, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (152684981348728832, 152588581545967616, 152683112727576576, 152685608543977472, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (152685133509689344, 152588581545967616, 152684512937246720, 152685032993193984, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (152685169891082240, 152588581545967616, 152684512937246720, 152684895835258880, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (152685211767013376, 152588581545967616, 152684758027206656, 152684895835258880, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (152685247745753088, 152588581545967616, 152685032993193984, 152684895835258880, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (152685585135566848, 152588581545967616, 152684895835258880, 152685491275431936, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (152685622099968000, 152588581545967616, 152685491275431936, 152685608543977472, '0', NULL);
INSERT INTO "public"."cm_wfd_trs" VALUES (160910207789953024, 152588581545967616, 152683112727576576, 152588671081775104, '1', 152683122982649856);

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
-- Records of cm_wfi_atv
-- ----------------------------
INSERT INTO "public"."cm_wfi_atv" VALUES (162025231375790080, 162025231350624256, 152588671081775104, 1, 1, '2020-12-21 10:30:29', '2020-12-21 10:30:31');
INSERT INTO "public"."cm_wfi_atv" VALUES (162025255044247552, 162025231350624256, 152683112727576576, 1, 1, '2020-12-21 10:30:31', '2020-12-21 16:45:05');
INSERT INTO "public"."cm_wfi_atv" VALUES (162119526644576256, 162025231350624256, 152684758027206656, 1, 1, '2020-12-21 16:45:05', '2020-12-21 16:45:11');
INSERT INTO "public"."cm_wfi_atv" VALUES (162119548043915264, 162025231350624256, 152684895835258880, 3, 1, '2020-12-21 16:45:11', '2020-12-21 16:45:11');
INSERT INTO "public"."cm_wfi_atv" VALUES (162119548199104512, 162025231350624256, 152685491275431936, 1, 1, '2020-12-21 16:45:11', '2020-12-21 16:45:13');
INSERT INTO "public"."cm_wfi_atv" VALUES (162401333625614336, 162401333600448512, 152588671081775104, 1, 1, '2020-12-22 11:25:22', '2023-03-16 10:42:58');
INSERT INTO "public"."cm_wfi_atv" VALUES (457374494836674560, 162401333600448512, 152683112727576576, 1, 1, '2023-03-16 10:42:57', '2023-03-16 11:10:31');
INSERT INTO "public"."cm_wfi_atv" VALUES (457374495587454976, 162401333600448512, 152684512937246720, 0, 1, '2023-03-16 10:42:57', '2023-03-16 10:42:57');
INSERT INTO "public"."cm_wfi_atv" VALUES (457381430491631616, 162401333600448512, 152684758027206656, 0, 1, '2023-03-16 11:10:31', '2023-03-16 11:10:31');
INSERT INTO "public"."cm_wfi_atv" VALUES (457384397022187520, 457384396879581184, 152588671081775104, 1, 1, '2023-03-16 11:22:27', '2023-03-16 11:23:30');
INSERT INTO "public"."cm_wfi_atv" VALUES (457384696747151360, 457384396879581184, 152683112727576576, 1, 1, '2023-03-16 11:23:29', '2023-03-16 11:27:51');
INSERT INTO "public"."cm_wfi_atv" VALUES (457384697418240000, 457384396879581184, 152684512937246720, 1, 1, '2023-03-16 11:23:29', '2023-03-16 11:28:13');
INSERT INTO "public"."cm_wfi_atv" VALUES (457385791041064960, 457384396879581184, 152684758027206656, 0, 2, '2023-03-16 11:27:50', '2023-03-16 11:27:50');
INSERT INTO "public"."cm_wfi_atv" VALUES (457385885710700544, 457384396879581184, 152685032993193984, 0, 1, '2023-03-16 11:28:13', '2023-03-16 11:28:13');
INSERT INTO "public"."cm_wfi_atv" VALUES (457388173628035072, 457388173615452160, 152588671081775104, 1, 1, '2023-03-16 11:37:33', '2023-03-16 11:38:10');
INSERT INTO "public"."cm_wfi_atv" VALUES (457388387768225792, 457388173615452160, 152683112727576576, 1, 1, '2023-03-16 11:38:10', '2023-03-16 11:38:50');
INSERT INTO "public"."cm_wfi_atv" VALUES (457388561571794944, 457388173615452160, 152684758027206656, 0, 1, '2023-03-16 11:38:49', '2023-03-16 11:38:49');

-- ----------------------------
-- Table structure for cm_wfi_item
-- ----------------------------
CREATE TABLE "public"."cm_wfi_item" (
  "id" int8 NOT NULL,
  "atvi_id" int8 NOT NULL,
  "status" int2 NOT NULL,
  "assign_kind" int2 NOT NULL,
  "sender" varchar(32) NOT NULL,
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
-- Records of cm_wfi_item
-- ----------------------------
INSERT INTO "public"."cm_wfi_item" VALUES (162025231392567296, 162025231375790080, 1, 1, 'daoting', '2020-12-21 10:30:29', '1', '2020-12-21 10:30:29', NULL, 1, '', 157, '2020-12-21 10:30:29', '2020-12-21 10:30:31');
INSERT INTO "public"."cm_wfi_item" VALUES (162025255065219072, 162025255044247552, 1, 0, 'daoting', '2020-12-21 10:30:31', '1', '2020-12-21 13:27:15', NULL, 1, '', 158, '2020-12-21 10:30:31', '2020-12-21 16:45:05');
INSERT INTO "public"."cm_wfi_item" VALUES (162119526686519296, 162119526644576256, 1, 0, 'daoting', '2020-12-21 16:45:05', '1', '2020-12-21 16:45:07', NULL, 1, '', 159, '2020-12-21 16:45:05', '2020-12-21 16:45:11');
INSERT INTO "public"."cm_wfi_item" VALUES (162119548064886784, 162119548043915264, 3, 0, 'daoting', '2020-12-21 16:45:11', '0', NULL, NULL, 1, '', 160, '2020-12-21 16:45:11', '2020-12-21 16:45:11');
INSERT INTO "public"."cm_wfi_item" VALUES (162119548220076032, 162119548199104512, 1, 0, 'daoting', '2020-12-21 16:45:11', '1', '2020-12-21 16:45:12', NULL, 1, '', 161, '2020-12-21 16:45:11', '2020-12-21 16:45:13');
INSERT INTO "public"."cm_wfi_item" VALUES (162401333642391552, 162401333625614336, 1, 1, 'daoting', '2020-12-22 11:25:22', '1', '2020-12-22 11:25:22', NULL, 1, '', 162, '2020-12-22 11:25:22', '2023-03-16 10:42:58');
INSERT INTO "public"."cm_wfi_item" VALUES (457374495021223936, 457374494836674560, 1, 0, '', '2023-03-16 10:42:57', '1', '2023-03-16 10:43:13', NULL, 1, '', 163, '2023-03-16 10:42:57', '2023-03-16 11:10:31');
INSERT INTO "public"."cm_wfi_item" VALUES (457374495696506880, 457374495587454976, 0, 0, '', '2023-03-16 10:42:57', '0', NULL, NULL, 152695627289198592, '', 164, '2023-03-16 10:42:57', '2023-03-16 10:42:57');
INSERT INTO "public"."cm_wfi_item" VALUES (457381430646820864, 457381430491631616, 0, 0, '', '2023-03-16 11:10:31', '1', '2023-03-16 11:11:00', NULL, 1, '', 165, '2023-03-16 11:10:31', '2023-03-16 11:10:31');
INSERT INTO "public"."cm_wfi_item" VALUES (457384397164793856, 457384397022187520, 1, 1, 'Windows', '2023-03-16 11:22:27', '1', '2023-03-16 11:22:27', NULL, 1, '', 167, '2023-03-16 11:22:27', '2023-03-16 11:23:30');
INSERT INTO "public"."cm_wfi_item" VALUES (457384696902340608, 457384696747151360, 1, 0, '', '2023-03-16 11:23:29', '1', '2023-03-16 11:23:45', NULL, 1, '', 168, '2023-03-16 11:23:29', '2023-03-16 11:27:51');
INSERT INTO "public"."cm_wfi_item" VALUES (457384697523097600, 457384697418240000, 1, 0, '', '2023-03-16 11:23:29', '1', '2023-03-16 11:23:46', NULL, 1, '', 169, '2023-03-16 11:23:29', '2023-03-16 11:28:13');
INSERT INTO "public"."cm_wfi_item" VALUES (457385791196254208, 457385791041064960, 1, 0, '', '2023-03-16 11:27:50', '1', '2023-03-16 11:28:02', NULL, 1, '', 170, '2023-03-16 11:27:50', '2023-03-16 11:28:25');
INSERT INTO "public"."cm_wfi_item" VALUES (457385791531798528, 457385791041064960, 0, 0, '', '2023-03-16 11:27:50', '0', NULL, NULL, 247170018466197504, '', 171, '2023-03-16 11:27:50', '2023-03-16 11:27:50');
INSERT INTO "public"."cm_wfi_item" VALUES (457385885811363840, 457385885710700544, 0, 0, '', '2023-03-16 11:28:13', '0', NULL, NULL, 2, '', 172, '2023-03-16 11:28:13', '2023-03-16 11:28:13');
INSERT INTO "public"."cm_wfi_item" VALUES (457388173640617984, 457388173628035072, 1, 1, 'Windows', '2023-03-16 11:37:33', '1', '2023-03-16 11:37:33', NULL, 1, '', 174, '2023-03-16 11:37:33', '2023-03-16 11:38:10');
INSERT INTO "public"."cm_wfi_item" VALUES (457388387776614400, 457388387768225792, 1, 0, '', '2023-03-16 11:38:10', '1', '2023-03-16 11:38:22', NULL, 2, '', 175, '2023-03-16 11:38:10', '2023-03-16 11:38:50');
INSERT INTO "public"."cm_wfi_item" VALUES (457388561714401280, 457388561571794944, 0, 0, '', '2023-03-16 11:38:49', '0', NULL, NULL, 1, '', 176, '2023-03-16 11:38:49', '2023-03-16 11:38:49');

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
-- Records of cm_wfi_prc
-- ----------------------------
INSERT INTO "public"."cm_wfi_prc" VALUES (162025231350624256, 152588581545967616, 'a', 1, 58, '2020-12-21 10:30:29', '2020-12-21 16:45:13');
INSERT INTO "public"."cm_wfi_prc" VALUES (162401333600448512, 152588581545967616, '关于新冠疫情的批示', 0, 59, '2020-12-22 11:25:22', '2020-12-22 11:25:22');
INSERT INTO "public"."cm_wfi_prc" VALUES (457384396879581184, 152588581545967616, '阿斯蒂芬', 0, 64, '2023-03-16 11:22:27', '2023-03-16 11:22:27');
INSERT INTO "public"."cm_wfi_prc" VALUES (457388173615452160, 152588581545967616, '疫情在', 0, 65, '2023-03-16 11:37:33', '2023-03-16 11:37:33');

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
-- Records of cm_wfi_trs
-- ----------------------------
INSERT INTO "public"."cm_wfi_trs" VALUES (162025255165882368, 152683122982649856, 162025231375790080, 162025255044247552, '0', '2020-12-21 10:30:31');
INSERT INTO "public"."cm_wfi_trs" VALUES (162119526820737024, 152684951493672960, 162025255044247552, 162119526644576256, '0', '2020-12-21 16:45:05');
INSERT INTO "public"."cm_wfi_trs" VALUES (162119548186521600, 152685211767013376, 162119526644576256, 162119548043915264, '0', '2020-12-21 16:45:11');
INSERT INTO "public"."cm_wfi_trs" VALUES (162119548320739328, 152685585135566848, 162119548043915264, 162119548199104512, '0', '2020-12-21 16:45:11');
INSERT INTO "public"."cm_wfi_trs" VALUES (457374495470014464, 152683122982649856, 162401333625614336, 457374494836674560, '0', '2023-03-16 10:42:57');
INSERT INTO "public"."cm_wfi_trs" VALUES (457374496069799936, 152684673721696256, 162401333625614336, 457374495587454976, '0', '2023-03-16 10:42:57');
INSERT INTO "public"."cm_wfi_trs" VALUES (457381431104000000, 152684951493672960, 457374494836674560, 457381430491631616, '0', '2023-03-16 11:10:31');
INSERT INTO "public"."cm_wfi_trs" VALUES (457384697296605184, 152683122982649856, 457384397022187520, 457384696747151360, '0', '2023-03-16 11:23:29');
INSERT INTO "public"."cm_wfi_trs" VALUES (457384697883807744, 152684673721696256, 457384397022187520, 457384697418240000, '0', '2023-03-16 11:23:29');
INSERT INTO "public"."cm_wfi_trs" VALUES (457385791921868800, 152684951493672960, 457384696747151360, 457385791041064960, '0', '2023-03-16 11:27:50');
INSERT INTO "public"."cm_wfi_trs" VALUES (457385886172073984, 152685133509689344, 457384697418240000, 457385885710700544, '0', '2023-03-16 11:28:13');
INSERT INTO "public"."cm_wfi_trs" VALUES (457388387831140352, 152683122982649856, 457388173628035072, 457388387768225792, '0', '2023-03-16 11:38:10');
INSERT INTO "public"."cm_wfi_trs" VALUES (457388562041556992, 152684951493672960, 457388387768225792, 457388561571794944, '0', '2023-03-16 11:38:49');

-- ----------------------------
-- Table structure for demo_cache_tbl1
-- ----------------------------
CREATE TABLE "public"."demo_cache_tbl1" (
  "id" int8 NOT NULL,
  "phone" varchar(255) NOT NULL,
  "name" varchar(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_cache_tbl1
-- ----------------------------
INSERT INTO "public"."demo_cache_tbl1" VALUES (454454068519129088, 'ca4f271212bc4add946c55feed7400bb', '3917');
INSERT INTO "public"."demo_cache_tbl1" VALUES (484620968746045440, '3f435d84c76a46e29002f467a4cd0187', '7425');
INSERT INTO "public"."demo_cache_tbl1" VALUES (484621133057904640, '3329d521b2134b0195083828152cb5b0', '1786');
INSERT INTO "public"."demo_cache_tbl1" VALUES (484624179913576448, 'd80e785d1d44472abe88723e4ed17ca8', '156');

-- ----------------------------
-- Table structure for demo_child_tbl1
-- ----------------------------
CREATE TABLE "public"."demo_child_tbl1" (
  "id" int8 NOT NULL,
  "parent_id" int8 NOT NULL,
  "item_name" varchar(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_child_tbl1
-- ----------------------------
INSERT INTO "public"."demo_child_tbl1" VALUES (443588385740705792, 443588385522601984, '修改370');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588388055961600, 443588385522601984, '修改370');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588388299231232, 443588385522601984, '修改370');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588583695077376, 443588583535693824, '新增0');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588583913181184, 443588583535693824, '新增1');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588584148062208, 443588583535693824, '新增2');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588895562551296, 443588895352836096, '新增0');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588895814209536, 443588895352836096, '新增1');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588896132976640, 443588895352836096, '新增2');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588932807970816, 443588932694724608, '新增0');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588933026074624, 443588932694724608, '新增1');
INSERT INTO "public"."demo_child_tbl1" VALUES (443588933248372736, 443588932694724608, '新增2');
INSERT INTO "public"."demo_child_tbl1" VALUES (445140374660337664, 445140374589034496, '新增0');
INSERT INTO "public"."demo_child_tbl1" VALUES (445140374786166784, 445140374589034496, '新增1');
INSERT INTO "public"."demo_child_tbl1" VALUES (446130095746207744, 446130095742013440, '新增0');
INSERT INTO "public"."demo_child_tbl1" VALUES (446130095754596352, 446130095742013440, '新增1');
INSERT INTO "public"."demo_child_tbl1" VALUES (484622270955802624, 484622270804807680, '新增0');
INSERT INTO "public"."demo_child_tbl1" VALUES (484622271224238080, 484622270804807680, '新增1');
INSERT INTO "public"."demo_child_tbl1" VALUES (484622408784826368, 484622408633831424, '新增0');
INSERT INTO "public"."demo_child_tbl1" VALUES (484622408994541568, 484622408633831424, '新增1');
INSERT INTO "public"."demo_child_tbl1" VALUES (484623850744598528, 484623850568437760, '新增0');
INSERT INTO "public"."demo_child_tbl1" VALUES (484623850987868160, 484623850568437760, '新增1');
INSERT INTO "public"."demo_child_tbl1" VALUES (484623946806743040, 484623946693496832, '新增0');
INSERT INTO "public"."demo_child_tbl1" VALUES (484623947016458240, 484623946693496832, '新增1');

-- ----------------------------
-- Table structure for demo_child_tbl2
-- ----------------------------
CREATE TABLE "public"."demo_child_tbl2" (
  "id" int8 NOT NULL,
  "group_id" int8 NOT NULL,
  "item_name" varchar(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_child_tbl2
-- ----------------------------
INSERT INTO "public"."demo_child_tbl2" VALUES (443588388416671744, 443588385522601984, '修改975');
INSERT INTO "public"."demo_child_tbl2" VALUES (443588583799934976, 443588583535693824, '新增0');
INSERT INTO "public"."demo_child_tbl2" VALUES (443588584039010304, 443588583535693824, '新增1');
INSERT INTO "public"."demo_child_tbl2" VALUES (443588584374554624, 443588583535693824, '新增2');
INSERT INTO "public"."demo_child_tbl2" VALUES (443588895692574720, 443588895352836096, '新增0');
INSERT INTO "public"."demo_child_tbl2" VALUES (443588895931650048, 443588895352836096, '新增1');
INSERT INTO "public"."demo_child_tbl2" VALUES (443588896258805760, 443588895352836096, '新增2');
INSERT INTO "public"."demo_child_tbl2" VALUES (443588932917022720, 443588932694724608, '新增0');
INSERT INTO "public"."demo_child_tbl2" VALUES (443588933135126528, 443588932694724608, '新增1');
INSERT INTO "public"."demo_child_tbl2" VALUES (443588933361618944, 443588932694724608, '新增2');
INSERT INTO "public"."demo_child_tbl2" VALUES (445140374735835136, 445140374589034496, '新增0');
INSERT INTO "public"."demo_child_tbl2" VALUES (445140374819721216, 445140374589034496, '新增1');
INSERT INTO "public"."demo_child_tbl2" VALUES (446130095750402048, 446130095742013440, '新增0');
INSERT INTO "public"."demo_child_tbl2" VALUES (446130095754596353, 446130095742013440, '新增1');
INSERT INTO "public"."demo_child_tbl2" VALUES (484622271115186176, 484622270804807680, '新增0');
INSERT INTO "public"."demo_child_tbl2" VALUES (484622271333289984, 484622270804807680, '新增1');
INSERT INTO "public"."demo_child_tbl2" VALUES (484622408889683968, 484622408633831424, '新增0');
INSERT INTO "public"."demo_child_tbl2" VALUES (484622409107787776, 484622408633831424, '新增1');
INSERT INTO "public"."demo_child_tbl2" VALUES (484623850878816256, 484623850568437760, '新增0');
INSERT INTO "public"."demo_child_tbl2" VALUES (484623851092725760, 484623850568437760, '新增1');
INSERT INTO "public"."demo_child_tbl2" VALUES (484623946907406336, 484623946693496832, '新增0');
INSERT INTO "public"."demo_child_tbl2" VALUES (484623947121315840, 484623946693496832, '新增1');

-- ----------------------------
-- Table structure for demo_crud
-- ----------------------------
CREATE TABLE "public"."demo_crud" (
  "id" int8 NOT NULL,
  "name" varchar(255) NOT NULL,
  "dispidx" int4 NOT NULL,
  "mtime" timestamp(0) NOT NULL,
  "enable_insert_event" bool NOT NULL,
  "enable_name_changed_event" bool NOT NULL,
  "enable_del_event" bool NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_crud"."id" IS '标识';
COMMENT ON COLUMN "public"."demo_crud"."name" IS '名称';
COMMENT ON COLUMN "public"."demo_crud"."dispidx" IS '显示顺序';
COMMENT ON COLUMN "public"."demo_crud"."mtime" IS '最后修改时间';
COMMENT ON COLUMN "public"."demo_crud"."enable_insert_event" IS 'true时允许发布插入事件';
COMMENT ON COLUMN "public"."demo_crud"."enable_name_changed_event" IS 'true时允许发布Name变化事件';
COMMENT ON COLUMN "public"."demo_crud"."enable_del_event" IS 'true时允许发布删除事件';
COMMENT ON TABLE "public"."demo_crud" IS '#demo#基础增删改';

-- ----------------------------
-- Records of demo_crud
-- ----------------------------
INSERT INTO "public"."demo_crud" VALUES (446127712370708480, '批增更944', 50, '2023-02-13 09:52:21', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (446127712387485696, '批量605', 51, '2023-02-13 09:52:21', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (446127744155144192, '批增更887', 52, '2023-02-13 09:52:28', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (446127778095452160, '批增更删501', 53, '2023-02-13 09:52:36', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (446127928557719552, '新增事件9083', 54, '2023-02-13 09:53:12', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (447641397090078720, '领域服务', 61, '2023-02-17 14:07:07', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (447641397589200896, '服务更', 62, '2023-02-17 14:07:08', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484620702760062976, '单个9897', 63, '2023-05-30 15:09:40', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484620769650823168, '批量430', 64, '2023-05-30 15:09:56', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484620769889898496, '批量813', 65, '2023-05-30 15:09:56', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484620770128973824, '批量572', 66, '2023-05-30 15:09:56', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484620773429891072, '批增更218', 67, '2023-05-30 15:09:57', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484623044423208960, '单个5122', 68, '2023-05-30 15:18:58', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484623148454531072, '批量40', 69, '2023-05-30 15:19:23', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484623148689412096, '批量680', 70, '2023-05-30 15:19:23', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484623148932681728, '批量531', 71, '2023-05-30 15:19:23', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484623187683856384, '批增更615', 72, '2023-05-30 15:19:33', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484623231044571136, '批增更删992', 73, '2023-05-30 15:19:43', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484624288650907648, '领域服务', 74, '2023-05-30 15:23:55', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484624288994840576, '服务更', 75, '2023-05-30 15:23:55', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484956889089593344, '单个8461', 76, '2023-05-31 13:25:35', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484957035659546624, '单个8271', 77, '2023-05-31 13:26:09', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484957333266386944, '批量652', 78, '2023-05-31 13:27:20', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484957333782286336, '批量521', 79, '2023-05-31 13:27:21', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484957334516289536, '批量955', 80, '2023-05-31 13:27:21', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (484988812650369024, '批增更778', 81, '2023-05-31 15:32:23', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (486788489460862976, '单个4284', 82, '2023-06-05 14:43:45', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (487086064026013696, '单个1221', 83, '2023-06-06 10:26:08', '0', '0', '0');
INSERT INTO "public"."demo_crud" VALUES (487086286626115584, '单个685', 84, '2023-06-06 10:27:01', '0', '0', '0');

-- ----------------------------
-- Table structure for demo_par_tbl
-- ----------------------------
CREATE TABLE "public"."demo_par_tbl" (
  "id" int8 NOT NULL,
  "name" varchar(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_par_tbl
-- ----------------------------
INSERT INTO "public"."demo_par_tbl" VALUES (443588385522601984, '91471c9846a44fe8a7fc4b76e9f702ea');
INSERT INTO "public"."demo_par_tbl" VALUES (443588583535693824, '新增');
INSERT INTO "public"."demo_par_tbl" VALUES (443588895352836096, '新增');
INSERT INTO "public"."demo_par_tbl" VALUES (443588932694724608, '新增');
INSERT INTO "public"."demo_par_tbl" VALUES (445140374589034496, '新增');
INSERT INTO "public"."demo_par_tbl" VALUES (446130095742013440, '新增');
INSERT INTO "public"."demo_par_tbl" VALUES (484622270804807680, '新增');
INSERT INTO "public"."demo_par_tbl" VALUES (484622408633831424, '新增');
INSERT INTO "public"."demo_par_tbl" VALUES (484623850568437760, '新增');
INSERT INTO "public"."demo_par_tbl" VALUES (484623946693496832, '新增');

-- ----------------------------
-- Table structure for demo_virtbl1
-- ----------------------------
CREATE TABLE "public"."demo_virtbl1" (
  "id" int8 NOT NULL,
  "name1" varchar(255) NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_virtbl1"."name1" IS '名称1';

-- ----------------------------
-- Records of demo_virtbl1
-- ----------------------------
INSERT INTO "public"."demo_virtbl1" VALUES (484613811564728320, '新1');
INSERT INTO "public"."demo_virtbl1" VALUES (484613939734269952, '新1');
INSERT INTO "public"."demo_virtbl1" VALUES (484614242416218112, '批增1');
INSERT INTO "public"."demo_virtbl1" VALUES (484621407772233728, '新1');
INSERT INTO "public"."demo_virtbl1" VALUES (484623466739290112, '新1');

-- ----------------------------
-- Table structure for demo_virtbl2
-- ----------------------------
CREATE TABLE "public"."demo_virtbl2" (
  "id" int8 NOT NULL,
  "name2" varchar(255) NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_virtbl2"."name2" IS '名称2';

-- ----------------------------
-- Records of demo_virtbl2
-- ----------------------------
INSERT INTO "public"."demo_virtbl2" VALUES (484613811564728320, '新2');
INSERT INTO "public"."demo_virtbl2" VALUES (484613939734269952, '新2');
INSERT INTO "public"."demo_virtbl2" VALUES (484614242416218112, '批增2');
INSERT INTO "public"."demo_virtbl2" VALUES (484621407772233728, '新2');
INSERT INTO "public"."demo_virtbl2" VALUES (484623466739290112, '新2');

-- ----------------------------
-- Table structure for demo_virtbl3
-- ----------------------------
CREATE TABLE "public"."demo_virtbl3" (
  "id" int8 NOT NULL,
  "name3" varchar(255) NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_virtbl3"."name3" IS '名称3';

-- ----------------------------
-- Records of demo_virtbl3
-- ----------------------------
INSERT INTO "public"."demo_virtbl3" VALUES (484613811564728320, '新3');
INSERT INTO "public"."demo_virtbl3" VALUES (484613939734269952, '新3');
INSERT INTO "public"."demo_virtbl3" VALUES (484614242416218112, '批增3');
INSERT INTO "public"."demo_virtbl3" VALUES (484621407772233728, '新3');
INSERT INTO "public"."demo_virtbl3" VALUES (484623466739290112, '新3');

-- ----------------------------
-- Table structure for demo_大儿
-- ----------------------------
CREATE TABLE "public"."demo_大儿" (
  "id" int8 NOT NULL,
  "parent_id" int8 NOT NULL,
  "大儿名" varchar(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_大儿
-- ----------------------------
INSERT INTO "public"."demo_大儿" VALUES (453807589999792128, 448686488403595264, '啊北侧');
INSERT INTO "public"."demo_大儿" VALUES (453810847795400704, 453810798449414144, 'bd');
INSERT INTO "public"."demo_大儿" VALUES (453811346175184896, 453810798449414144, 'asdf');
INSERT INTO "public"."demo_大儿" VALUES (453811364621733888, 453810798449414144, 'bde');

-- ----------------------------
-- Table structure for demo_父表
-- ----------------------------
CREATE TABLE "public"."demo_父表" (
  "id" int8 NOT NULL,
  "父名" varchar(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_父表
-- ----------------------------
INSERT INTO "public"."demo_父表" VALUES (448686488403595264, '123');
INSERT INTO "public"."demo_父表" VALUES (449120963746877440, '单位');
INSERT INTO "public"."demo_父表" VALUES (453810798449414144, 'aaaa');

-- ----------------------------
-- Table structure for demo_基础
-- ----------------------------
CREATE TABLE "public"."demo_基础" (
  "id" int8 NOT NULL,
  "序列" int4 NOT NULL,
  "限长4" varchar(16) NOT NULL,
  "不重复" varchar(64) NOT NULL,
  "禁止选中" bool NOT NULL,
  "禁止保存" bool NOT NULL,
  "禁止删除" bool NOT NULL,
  "值变事件" varchar(64) NOT NULL,
  "创建时间" timestamp(0) NOT NULL,
  "修改时间" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_基础"."id" IS '标识';
COMMENT ON COLUMN "public"."demo_基础"."序列" IS '序列自动赋值';
COMMENT ON COLUMN "public"."demo_基础"."限长4" IS '限制最大长度4';
COMMENT ON COLUMN "public"."demo_基础"."不重复" IS '列值无重复';
COMMENT ON COLUMN "public"."demo_基础"."禁止选中" IS '始终为false';
COMMENT ON COLUMN "public"."demo_基础"."禁止保存" IS 'true时保存前校验不通过';
COMMENT ON COLUMN "public"."demo_基础"."禁止删除" IS 'true时删除前校验不通过';
COMMENT ON COLUMN "public"."demo_基础"."值变事件" IS '每次值变化时触发领域事件';
COMMENT ON COLUMN "public"."demo_基础"."创建时间" IS '初次创建时间';
COMMENT ON COLUMN "public"."demo_基础"."修改时间" IS '最后修改时间';

-- ----------------------------
-- Records of demo_基础
-- ----------------------------
INSERT INTO "public"."demo_基础" VALUES (1, 1, 'adb', 'ddd', '1', '1', '1', 'a', '2023-01-17 10:08:10', '2023-01-17 10:08:14');
INSERT INTO "public"."demo_基础" VALUES (447570516976357376, 6, '11', 'dd', '0', '0', '1', 'snv111', '2023-02-17 09:25:27', '2023-02-17 09:25:27');

-- ----------------------------
-- Table structure for demo_角色
-- ----------------------------
CREATE TABLE "public"."demo_角色" (
  "id" int8 NOT NULL,
  "角色名称" varchar(32) NOT NULL,
  "角色描述" varchar(255)
)
;
COMMENT ON COLUMN "public"."demo_角色"."id" IS '角色标识';
COMMENT ON COLUMN "public"."demo_角色"."角色名称" IS '角色名称';
COMMENT ON COLUMN "public"."demo_角色"."角色描述" IS '角色描述';
COMMENT ON TABLE "public"."demo_角色" IS '角色';

-- ----------------------------
-- Records of demo_角色
-- ----------------------------
INSERT INTO "public"."demo_角色" VALUES (449487215124303872, 'xxx', 'df');
INSERT INTO "public"."demo_角色" VALUES (449812931669938176, '管理员', '');
INSERT INTO "public"."demo_角色" VALUES (449812975420723200, '维护1', '');
INSERT INTO "public"."demo_角色" VALUES (449813053959065600, '维护2', '');

-- ----------------------------
-- Table structure for demo_角色权限
-- ----------------------------
CREATE TABLE "public"."demo_角色权限" (
  "role_id" int8 NOT NULL,
  "prv_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_角色权限"."role_id" IS '角色标识';
COMMENT ON COLUMN "public"."demo_角色权限"."prv_id" IS '权限标识';
COMMENT ON TABLE "public"."demo_角色权限" IS '角色关联的权限';

-- ----------------------------
-- Records of demo_角色权限
-- ----------------------------
INSERT INTO "public"."demo_角色权限" VALUES (449487215124303872, 449812884102336512);

-- ----------------------------
-- Table structure for demo_扩展1
-- ----------------------------
CREATE TABLE "public"."demo_扩展1" (
  "id" int8 NOT NULL,
  "扩展1名称" varchar(255) NOT NULL,
  "禁止选中" bool NOT NULL,
  "禁止保存" bool NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_扩展1"."id" IS '标识';
COMMENT ON COLUMN "public"."demo_扩展1"."禁止选中" IS '始终为false';
COMMENT ON COLUMN "public"."demo_扩展1"."禁止保存" IS 'true时保存前校验不通过';

-- ----------------------------
-- Records of demo_扩展1
-- ----------------------------
INSERT INTO "public"."demo_扩展1" VALUES (447555037331214336, 'a', '0', '0');
INSERT INTO "public"."demo_扩展1" VALUES (447577275388416000, '221', '0', '0');
INSERT INTO "public"."demo_扩展1" VALUES (447577372700463104, '', '0', '0');

-- ----------------------------
-- Table structure for demo_扩展2
-- ----------------------------
CREATE TABLE "public"."demo_扩展2" (
  "id" int8 NOT NULL,
  "扩展2名称" varchar(255) NOT NULL,
  "禁止删除" bool NOT NULL,
  "值变事件" varchar(255) NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_扩展2"."id" IS '标识';
COMMENT ON COLUMN "public"."demo_扩展2"."禁止删除" IS 'true时删除前校验不通过';
COMMENT ON COLUMN "public"."demo_扩展2"."值变事件" IS '每次值变化时触发领域事件';

-- ----------------------------
-- Records of demo_扩展2
-- ----------------------------
INSERT INTO "public"."demo_扩展2" VALUES (447555037331214336, 'a', '0', '');
INSERT INTO "public"."demo_扩展2" VALUES (447577275388416000, '', '0', '221');
INSERT INTO "public"."demo_扩展2" VALUES (447577372700463104, '', '0', '');

-- ----------------------------
-- Table structure for demo_权限
-- ----------------------------
CREATE TABLE "public"."demo_权限" (
  "id" int8 NOT NULL,
  "权限名称" varchar(255) NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_权限"."id" IS '权限名称';
COMMENT ON TABLE "public"."demo_权限" IS '权限';

-- ----------------------------
-- Records of demo_权限
-- ----------------------------
INSERT INTO "public"."demo_权限" VALUES (449812852120768512, '删除');
INSERT INTO "public"."demo_权限" VALUES (449812884102336512, '修改');

-- ----------------------------
-- Table structure for demo_收文
-- ----------------------------
CREATE TABLE "public"."demo_收文" (
  "id" int8 NOT NULL,
  "来文单位" varchar(255) NOT NULL,
  "来文时间" date NOT NULL,
  "密级" int2 NOT NULL,
  "文件标题" varchar(255) NOT NULL,
  "文件附件" varchar(512) NOT NULL,
  "市场部经理意见" varchar(255) NOT NULL,
  "综合部经理意见" varchar(255) NOT NULL,
  "收文完成时间" date NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_收文"."密级" IS '#密级#';

-- ----------------------------
-- Records of demo_收文
-- ----------------------------
INSERT INTO "public"."demo_收文" VALUES (162025231350624256, '123', '2020-12-21', 0, 'a', '', '', '', '0001-01-01');
INSERT INTO "public"."demo_收文" VALUES (162401333600448512, 'abc', '2020-12-22', 0, '关于新冠疫情的批示', '', '', '', '0001-01-01');
INSERT INTO "public"."demo_收文" VALUES (457384396879581184, '', '2023-03-16', 0, '阿斯蒂芬', '', '', '', '0001-01-01');
INSERT INTO "public"."demo_收文" VALUES (457388173615452160, '', '2023-03-16', 0, '疫情在', '', '', '', '0001-01-01');

-- ----------------------------
-- Table structure for demo_小儿
-- ----------------------------
CREATE TABLE "public"."demo_小儿" (
  "id" int8 NOT NULL,
  "group_id" int8 NOT NULL,
  "小儿名" varchar(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_小儿
-- ----------------------------
INSERT INTO "public"."demo_小儿" VALUES (449113382156521472, 448686488403595264, 'wwww');
INSERT INTO "public"."demo_小儿" VALUES (453810909078376448, 453810798449414144, '34');
INSERT INTO "public"."demo_小儿" VALUES (453811464773324800, 453810798449414144, 'adgas');

-- ----------------------------
-- Table structure for demo_用户
-- ----------------------------
CREATE TABLE "public"."demo_用户" (
  "id" int8 NOT NULL,
  "手机号" char(11) NOT NULL,
  "姓名" varchar(32) NOT NULL,
  "密码" char(32) NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_用户"."id" IS '用户标识';
COMMENT ON COLUMN "public"."demo_用户"."手机号" IS '手机号，唯一';
COMMENT ON COLUMN "public"."demo_用户"."姓名" IS '姓名';
COMMENT ON COLUMN "public"."demo_用户"."密码" IS '密码的md5';
COMMENT ON TABLE "public"."demo_用户" IS '系统用户';

-- ----------------------------
-- Records of demo_用户
-- ----------------------------
INSERT INTO "public"."demo_用户" VALUES (449772627373871104, '13223333', '阿斯顿', '');
INSERT INTO "public"."demo_用户" VALUES (453805638385946624, '111', '', '');
INSERT INTO "public"."demo_用户" VALUES (453805654500462592, '222', '', '');

-- ----------------------------
-- Table structure for demo_用户角色
-- ----------------------------
CREATE TABLE "public"."demo_用户角色" (
  "user_id" int8 NOT NULL,
  "role_id" int8 NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_用户角色"."user_id" IS '用户标识';
COMMENT ON COLUMN "public"."demo_用户角色"."role_id" IS '角色标识';
COMMENT ON TABLE "public"."demo_用户角色" IS '用户关联的角色';

-- ----------------------------
-- Records of demo_用户角色
-- ----------------------------
INSERT INTO "public"."demo_用户角色" VALUES (449772627373871104, 449487215124303872);
INSERT INTO "public"."demo_用户角色" VALUES (449772627373871104, 449812931669938176);

-- ----------------------------
-- Table structure for demo_主表
-- ----------------------------
CREATE TABLE "public"."demo_主表" (
  "id" int8 NOT NULL,
  "主表名称" varchar(255) NOT NULL,
  "限长4" varchar(16) NOT NULL,
  "不重复" varchar(255) NOT NULL
)
;
COMMENT ON COLUMN "public"."demo_主表"."限长4" IS '限制最大长度4';
COMMENT ON COLUMN "public"."demo_主表"."不重复" IS '列值无重复';

-- ----------------------------
-- Records of demo_主表
-- ----------------------------
INSERT INTO "public"."demo_主表" VALUES (447555037331214336, 'a', '', '');
INSERT INTO "public"."demo_主表" VALUES (447577275388416000, '1', '222222', '121');
INSERT INTO "public"."demo_主表" VALUES (447577372700463104, '', '', '1');

-- ----------------------------
-- Table structure for fsm_file
-- ----------------------------
CREATE TABLE "public"."fsm_file" (
  "id" numeric(20) NOT NULL,
  "name" varchar(512) NOT NULL,
  "path" varchar(512) NOT NULL,
  "size" numeric(20) NOT NULL,
  "info" varchar(512),
  "uploader" numeric(20) NOT NULL,
  "ctime" timestamp(0) NOT NULL,
  "downloads" numeric(20) NOT NULL
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
CREATE UNIQUE INDEX "idx_permission_name" ON "public"."cm_permission" USING btree (
  "name" ASC
);
COMMENT ON INDEX "public"."idx_permission_name" IS '不重复';

-- ----------------------------
-- Primary Key structure for table cm_permission
-- ----------------------------
ALTER TABLE "public"."cm_permission" ADD PRIMARY KEY ("id");

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
CREATE UNIQUE INDEX "idx_user_phone" ON "public"."cm_user" USING btree (
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
-- Primary Key structure for table demo_cache_tbl1
-- ----------------------------
ALTER TABLE "public"."demo_cache_tbl1" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_child_tbl1
-- ----------------------------
ALTER TABLE "public"."demo_child_tbl1" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_child_tbl2
-- ----------------------------
ALTER TABLE "public"."demo_child_tbl2" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_crud
-- ----------------------------
ALTER TABLE "public"."demo_crud" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_par_tbl
-- ----------------------------
ALTER TABLE "public"."demo_par_tbl" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_virtbl1
-- ----------------------------
ALTER TABLE "public"."demo_virtbl1" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_virtbl2
-- ----------------------------
ALTER TABLE "public"."demo_virtbl2" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_virtbl3
-- ----------------------------
ALTER TABLE "public"."demo_virtbl3" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table demo_大儿
-- ----------------------------
CREATE INDEX "idx_大儿_parendid" ON "public"."demo_大儿" USING btree (
  "parent_id" ASC
);

-- ----------------------------
-- Primary Key structure for table demo_大儿
-- ----------------------------
ALTER TABLE "public"."demo_大儿" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_父表
-- ----------------------------
ALTER TABLE "public"."demo_父表" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_基础
-- ----------------------------
ALTER TABLE "public"."demo_基础" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_角色
-- ----------------------------
ALTER TABLE "public"."demo_角色" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table demo_角色权限
-- ----------------------------
CREATE INDEX "idx_demo_角色权限_prvid" ON "public"."demo_角色权限" USING btree (
  "prv_id" ASC
);
CREATE INDEX "idx_demo_角色权限_roleid" ON "public"."demo_角色权限" USING btree (
  "role_id" ASC
);

-- ----------------------------
-- Primary Key structure for table demo_角色权限
-- ----------------------------
ALTER TABLE "public"."demo_角色权限" ADD PRIMARY KEY ("role_id", "prv_id");

-- ----------------------------
-- Primary Key structure for table demo_扩展1
-- ----------------------------
ALTER TABLE "public"."demo_扩展1" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_扩展2
-- ----------------------------
ALTER TABLE "public"."demo_扩展2" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_权限
-- ----------------------------
ALTER TABLE "public"."demo_权限" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_收文
-- ----------------------------
ALTER TABLE "public"."demo_收文" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table demo_小儿
-- ----------------------------
CREATE INDEX "idx_小儿_parentid" ON "public"."demo_小儿" USING btree (
  "group_id" ASC
);

-- ----------------------------
-- Primary Key structure for table demo_小儿
-- ----------------------------
ALTER TABLE "public"."demo_小儿" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table demo_用户
-- ----------------------------
ALTER TABLE "public"."demo_用户" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table demo_用户角色
-- ----------------------------
CREATE INDEX "idx_demo_用户角色_roleid" ON "public"."demo_用户角色" USING btree (
  "role_id" ASC
);
CREATE INDEX "idx_demo_用户角色_userid" ON "public"."demo_用户角色" USING btree (
  "user_id" ASC
);

-- ----------------------------
-- Primary Key structure for table demo_用户角色
-- ----------------------------
ALTER TABLE "public"."demo_用户角色" ADD PRIMARY KEY ("user_id", "role_id");

-- ----------------------------
-- Primary Key structure for table demo_主表
-- ----------------------------
ALTER TABLE "public"."demo_主表" ADD PRIMARY KEY ("id");

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
-- Foreign Keys structure for table demo_大儿
-- ----------------------------
ALTER TABLE "public"."demo_大儿" ADD CONSTRAINT "fk_大儿_parendid" FOREIGN KEY ("parent_id") REFERENCES "public"."demo_父表" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table demo_角色权限
-- ----------------------------
ALTER TABLE "public"."demo_角色权限" ADD CONSTRAINT "fk_角色权限_prvid" FOREIGN KEY ("prv_id") REFERENCES "public"."demo_权限" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."demo_角色权限" ADD CONSTRAINT "fk_角色权限_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."demo_角色" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table demo_小儿
-- ----------------------------
ALTER TABLE "public"."demo_小儿" ADD CONSTRAINT "fk_小儿_parentid" FOREIGN KEY ("group_id") REFERENCES "public"."demo_父表" ("id") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table demo_用户角色
-- ----------------------------
ALTER TABLE "public"."demo_用户角色" ADD CONSTRAINT "fk_demo_用户角色_roleid" FOREIGN KEY ("role_id") REFERENCES "public"."demo_角色" ("id") ON DELETE CASCADE;
ALTER TABLE "public"."demo_用户角色" ADD CONSTRAINT "fk_demo_用户角色_userid" FOREIGN KEY ("user_id") REFERENCES "public"."demo_用户" ("id") ON DELETE CASCADE;

-- ----------------------------
-- 序列
-- ----------------------------
create sequence cm_menu_dispidx start 90;
create sequence cm_option_dispidx start 1032;
create sequence cm_wfd_prc_dispidx start 12;
create sequence cm_wfi_item_dispidx start 177;
create sequence cm_wfi_prc_dispidx start 66;
create sequence demo_crud_dispidx start 86;
create sequence demo_基础_序列 start 12;

-- ----------------------------
-- View structure for demo_child_view
-- ----------------------------
CREATE VIEW "demo_child_view" AS  SELECT c.id,
    c.parent_id,
    c.item_name,
    p.name
   FROM demo_child_tbl1 c
     JOIN demo_par_tbl p ON c.parent_id = p.id;
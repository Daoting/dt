
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
DROP SEQUENCE IF EXISTS "public"."demo_crud_dispidx";
DROP SEQUENCE IF EXISTS "public"."demo_基础_序列";
create sequence demo_crud_dispidx start 86;
create sequence demo_基础_序列 start 12;

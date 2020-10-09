-- MySQL dump 10.13  Distrib 5.6.13, for Win32 (x86)
--
-- Host: 10.10.1.2    Database: bs0
-- ------------------------------------------------------
-- Server version	5.7.21-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cm_menu`
--

DROP TABLE IF EXISTS `cm_menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_menu` (
  `ID` bigint(20) NOT NULL COMMENT '菜单标识',
  `ParentID` bigint(20) DEFAULT NULL COMMENT '父菜单标识',
  `Name` varchar(64) NOT NULL COMMENT '菜单名称',
  `IsGroup` tinyint(1) NOT NULL COMMENT '分组或实例。0表实例，1表分组',
  `ViewName` varchar(128) NOT NULL COMMENT '视图名称',
  `Params` varchar(4000) NOT NULL COMMENT '传递给菜单程序的参数',
  `Icon` varchar(128) NOT NULL COMMENT '图标',
  `SrvName` varchar(32) NOT NULL COMMENT '提供提示信息的服务名称，空表示无提示信息',
  `Note` varchar(512) NOT NULL COMMENT '备注',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `IsLocked` tinyint(1) NOT NULL DEFAULT '0' COMMENT '定义了菜单是否被锁定。0表未锁定，1表锁定不可用',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  KEY `fk_menu_parentid` (`ParentID`),
  CONSTRAINT `fk_menu_parentid` FOREIGN KEY (`parentid`) REFERENCES `cm_menu` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='业务菜单';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_menu`
--

LOCK TABLES `cm_menu` WRITE;
/*!40000 ALTER TABLE `cm_menu` DISABLE KEYS */;
INSERT INTO `cm_menu` VALUES (1,NULL,'工作台',1,'','','搬运工','','',1,0,'2019-03-07 10:45:44','2019-03-07 10:45:43'),(2,1,'用户账号',0,'用户账号','','钥匙','','',2,0,'2019-11-08 11:42:28','2019-11-08 11:43:53'),(3,1,'菜单管理',0,'菜单管理','','大图标','','',3,0,'2019-03-11 11:35:59','2019-03-11 11:35:58'),(4,1,'系统角色',0,'系统角色','','两人','','',4,0,'2019-11-08 11:47:21','2019-11-08 11:48:22'),(5,1,'基础权限',0,'基础权限','','审核','','',5,0,'2019-03-12 09:11:22','2019-03-07 11:23:40'),(6,1,'参数定义',0,'参数定义','','调色板','','',6,0,'2019-03-12 15:35:56','2019-03-12 15:37:10'),(7,1,'基础代码',0,'基础代码','','文件','','',7,0,'2019-11-08 11:49:40','2019-11-08 11:49:46'),(8,1,'控件样例',0,'控件样例','','词典','','客户端常用控件的使用样例，按照容器、编辑器、数据访问、样式资源等分类给出。',8,0,'2019-03-07 11:06:57','2019-03-07 11:06:57'),(15268145234386944,15315938808373248,'新菜单组22',1,'','','文件夹','','',25,0,'2019-11-12 11:10:10','2019-11-12 11:10:13'),(15315637929975808,18562741636898816,'新菜单12',0,'','','文件','','',48,0,'2019-11-12 14:18:53','2019-11-12 14:31:38'),(15315938808373248,NULL,'新菜单组额',1,'','','文件夹','','',67,0,'2019-11-12 14:20:04','2019-11-12 14:20:14'),(18562741636898816,15315938808373248,'新组t',1,'','','文件夹','','',63,0,'2019-11-21 13:21:43','2019-11-21 13:21:43'),(18860286065975296,NULL,'新菜单a',0,'','','文件','','',68,0,'2019-11-22 09:04:04','2019-11-22 09:04:04'),(84192907213271040,NULL,'发布',1,'','','文件夹','','',51,0,'2020-05-20 15:52:35','2020-05-20 15:52:35'),(84193059722358784,84192907213271040,'文章管理',0,'文章管理','','文件','','',69,0,'2020-05-20 15:53:11','2020-05-20 15:53:11'),(84470870081138688,84192907213271040,'素材库',0,'素材库','','词典','','',75,0,'2020-05-21 10:17:06','2020-05-21 10:17:06'),(84471155981676544,84192907213271040,'文章专辑',0,'文章专辑','','书籍','','',73,0,'2020-05-21 10:18:15','2020-05-21 10:18:15'),(84471601248989184,84192907213271040,'文章分类',0,'文章分类','','展开','','',74,0,'2020-05-21 10:20:01','2020-05-21 10:20:01');
/*!40000 ALTER TABLE `cm_menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_params`
--

DROP TABLE IF EXISTS `cm_params`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_params` (
  `ID` varchar(64) NOT NULL COMMENT '用户参数标识',
  `Value` varchar(255) NOT NULL COMMENT '参数缺省值',
  `Note` varchar(255) NOT NULL COMMENT '参数描述',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='用户参数定义';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_params`
--

LOCK TABLES `cm_params` WRITE;
/*!40000 ALTER TABLE `cm_params` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_params` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_prv`
--

DROP TABLE IF EXISTS `cm_prv`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_prv` (
  `ID` varchar(64) NOT NULL COMMENT '权限名称',
  `Note` varchar(255) DEFAULT NULL COMMENT '权限描述',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='权限';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_prv`
--

LOCK TABLES `cm_prv` WRITE;
/*!40000 ALTER TABLE `cm_prv` DISABLE KEYS */;
INSERT INTO `cm_prv` VALUES ('新权限asdfasdfasd','sdf'),('阿斯蒂',''),('阿斯蒂芬5','阿斯蒂');
/*!40000 ALTER TABLE `cm_prv` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_res`
--

DROP TABLE IF EXISTS `cm_res`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_res` (
  `ID` varchar(64) NOT NULL COMMENT '代码名称',
  `Grp` varchar(64) NOT NULL COMMENT '所属分组',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  PRIMARY KEY (`ID`,`Grp`) USING BTREE,
  KEY `fk_res_grp` (`Grp`),
  CONSTRAINT `fk_res_grp` FOREIGN KEY (`Grp`) REFERENCES `cm_resgrp` (`Name`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='基础代码';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_res`
--

LOCK TABLES `cm_res` WRITE;
/*!40000 ALTER TABLE `cm_res` DISABLE KEYS */;
INSERT INTO `cm_res` VALUES ('bool','数据类型',816),('Date','数据类型',350),('DateTime','数据类型',349),('double','数据类型',348),('int','数据类型',347),('string','数据类型',346),('不明','性别',345),('东乡族','民族',27),('中学','学历',60),('乌孜别克族','民族',44),('京族','民族',50),('仡佬族','民族',38),('仫佬族','民族',33),('佤族','民族',22),('侗族','民族',13),('俄罗斯族','民族',45),('保安族','民族',48),('傈僳族','民族',21),('傣族','民族',19),('其他','学历',64),('内蒙古东乌珠穆沁旗','地区',185),('内蒙古东胜市','地区',207),('内蒙古丰镇县','地区',200),('内蒙古临河市','地区',215),('内蒙古乌兰浩特市','地区',167),('内蒙古乌审旗','地区',213),('内蒙古乌拉特中旗','地区',219),('内蒙古乌拉特前旗','地区',218),('内蒙古乌拉特后旗','地区',220),('内蒙古乌海市','地区',143),('内蒙古二连浩特市','地区',180),('内蒙古五原县','地区',216),('内蒙古伊金霍洛旗','地区',214),('内蒙古克什克腾旗','地区',149),('内蒙古兴和县','地区',199),('内蒙古准格尔旗','地区',209),('内蒙古凉城县','地区',201),('内蒙古包头市','地区',140),('内蒙古化德县','地区',197),('内蒙古卓资县','地区',196),('内蒙古呼和浩特市','地区',137),('内蒙古和林格尔县','地区',194),('内蒙古商都县','地区',198),('内蒙古喀喇沁旗','地区',151),('内蒙古四子王旗','地区',206),('内蒙古固阳县','地区',142),('内蒙古土默特右旗','地区',141),('内蒙古土默特左旗','地区',138),('内蒙古多伦县','地区',191),('内蒙古太仆寺旗','地区',187),('内蒙古奈曼旗','地区',178),('内蒙古宁城县','地区',152),('内蒙古察哈尔右翼中旗','地区',203),('内蒙古察哈尔右翼前旗','地区',202),('内蒙古察哈尔右翼后旗','地区',204),('内蒙古巴林右旗','地区',147),('内蒙古巴林左旗','地区',146),('内蒙古库伦旗','地区',177),('内蒙古开鲁县','地区',176),('内蒙古扎兰屯市','地区',156),('内蒙古扎赉特旗','地区',170),('内蒙古扎鲁特旗','地区',179),('内蒙古托克托县','地区',139),('内蒙古敖汉旗','地区',153),('内蒙古新巴尔虎右旗','地区',164),('内蒙古新巴尔虎左旗','地区',165),('内蒙古杭锦后旗','地区',221),('内蒙古杭锦旗','地区',212),('内蒙古林西县','地区',148),('内蒙古正蓝旗','地区',190),('内蒙古正镶白旗','地区',189),('内蒙古武川县','地区',193),('内蒙古海拉尔市','地区',154),('内蒙古清水河县','地区',195),('内蒙古满州里市','地区',155),('内蒙古牙克石市','地区',157),('内蒙古磴口县','地区',217),('内蒙古科尔沁右翼中旗','地区',169),('内蒙古科尔沁右翼前旗','地区',168),('内蒙古科尔沁左翼中旗','地区',174),('内蒙古科尔沁左翼后旗','地区',175),('内蒙古突泉县','地区',171),('内蒙古翁牛特旗','地区',150),('内蒙古苏尼特右旗','地区',184),('内蒙古苏尼特左旗','地区',183),('内蒙古莫力县','地区',159),('内蒙古西乌珠穆沁旗','地区',186),('内蒙古赤峰市','地区',144),('内蒙古达尔罕茂明安联','地区',205),('内蒙古达拉特旗','地区',208),('内蒙古通辽市','地区',172),('内蒙古鄂伦春自治旗','地区',162),('内蒙古鄂托克前旗','地区',210),('内蒙古鄂托克旗','地区',211),('内蒙古鄂温克族自治旗','地区',163),('内蒙古锡林浩特市','地区',181),('内蒙古镶黄旗','地区',188),('内蒙古阿巴嘎旗','地区',182),('内蒙古阿拉善右旗','地区',223),('内蒙古阿拉善左旗','地区',222),('内蒙古阿荣旗','地区',158),('内蒙古阿鲁科尔沁旗','地区',145),('内蒙古陈巴尔虎旗','地区',166),('内蒙古集宁市','地区',192),('内蒙古霍林郭勒市','地区',173),('内蒙古额尔古纳右旗','地区',160),('内蒙古额尔古纳左旗','地区',161),('内蒙古额济纳旗','地区',224),('博士','学历',63),('吉林省','地区',285),('吉林省东丰县','地区',300),('吉林省东辽县','地区',301),('吉林省九台市','地区',332),('吉林省乾安县','地区',319),('吉林省伊通县','地区',297),('吉林省公主岭市','地区',328),('吉林省农安县','地区',287),('吉林省前郭尔罗斯县','地区',316),('吉林省双辽县','地区',298),('吉林省双阳县','地区',289),('吉林省吉林市','地区',290),('吉林省和龙县','地区',325),('吉林省四平市','地区',295),('吉林省图们市','地区',321),('吉林省大安市','地区',314),('吉林省安图县','地区',327),('吉林省延吉市','地区',320),('吉林省德惠县','地区',288),('吉林省扶余市','地区',313),('吉林省抚松县','地区',307),('吉林省敦化市','地区',322),('吉林省柳河县','地区',305),('吉林省桦甸市','地区',331),('吉林省梅河口市','地区',329),('吉林省梨树县','地区',296),('吉林省榆树县','地区',286),('吉林省永吉县','地区',291),('吉林省汪清县','地区',326),('吉林省洮南市','地区',312),('吉林省浑江市','地区',306),('吉林省珲春市','地区',323),('吉林省白城地区','地区',310),('吉林省白城市','地区',311),('吉林省磐石县','地区',293),('吉林省舒兰县','地区',292),('吉林省蛟河县','地区',294),('吉林省辉南县','地区',304),('吉林省辽源市','地区',299),('吉林省通化县','地区',303),('吉林省通化市','地区',302),('吉林省通榆县','地区',318),('吉林省镇赉县','地区',317),('吉林省长岭县','地区',315),('吉林省长春市','地区',136),('吉林省长白县','地区',309),('吉林省集安市','地区',330),('吉林省靖宇县','地区',308),('吉林省龙井市','地区',324),('哈尼族','民族',17),('哈萨克族','民族',18),('回族','民族',4),('土家族','民族',16),('土族','民族',31),('基诺族','民族',57),('塔吉克族','民族',42),('塔塔尔族','民族',51),('壮族','民族',9),('大学','学历',58),('女','性别',343),('小学','学历',61),('布依族','民族',10),('布朗族','民族',35),('彝族','民族',8),('德昂族','民族',47),('怒族','民族',43),('拉祜族','民族',25),('撒拉族','民族',36),('普米族','民族',41),('景颇族','民族',29),('朝鲜族','民族',11),('未知','性别',344),('柯尔克孜族','民族',30),('毛难族','民族',37),('水族','民族',26),('汉族','民族',2),('满族','民族',12),('独龙族','民族',52),('珞巴族','民族',56),('瑶族','民族',14),('男','性别',342),('畲族','民族',23),('白族','民族',15),('硕士','学历',62),('纳西族','民族',28),('维吾尔族','民族',6),('羌族','民族',34),('苗族','民族',7),('蒙古族','民族',3),('藏族','民族',5),('裕固族','民族',49),('赫哲族','民族',54),('辽宁省','地区',225),('辽宁省东沟县','地区',245),('辽宁省丹东市','地区',242),('辽宁省义  县','地区',252),('辽宁省兴城市','地区',281),('辽宁省凌源县','地区',274),('辽宁省凤城县','地区',243),('辽宁省北票市','地区',283),('辽宁省北镇县','地区',250),('辽宁省台安县','地区',234),('辽宁省喀喇沁县','地区',275),('辽宁省大洼县','地区',263),('辽宁省大连市','地区',229),('辽宁省宽甸县','地区',246),('辽宁省岫岩县','地区',244),('辽宁省庄河县','地区',232),('辽宁省康平县','地区',269),('辽宁省建平县','地区',273),('辽宁省建昌县','地区',276),('辽宁省开原市','地区',284),('辽宁省彰武县','地区',258),('辽宁省抚顺县','地区',236),('辽宁省抚顺市','地区',235),('辽宁省新宾县','地区',237),('辽宁省新民县','地区',227),('辽宁省新金县','地区',230),('辽宁省昌图县','地区',268),('辽宁省朝阳县','地区',272),('辽宁省朝阳市','地区',271),('辽宁省本溪县','地区',240),('辽宁省本溪市','地区',239),('辽宁省桓仁县','地区',241),('辽宁省沈阳市','地区',226),('辽宁省法库县','地区',270),('辽宁省海城市','地区',279),('辽宁省清原县','地区',238),('辽宁省灯塔县','地区',261),('辽宁省瓦房店市','地区',278),('辽宁省盖  县','地区',255),('辽宁省盘山县','地区',264),('辽宁省盘锦市','地区',262),('辽宁省直辖行政单位','地区',277),('辽宁省绥中县','地区',248),('辽宁省营口县','地区',254),('辽宁省营口市','地区',253),('辽宁省西丰县','地区',267),('辽宁省辽中县','地区',228),('辽宁省辽阳县','地区',260),('辽宁省辽阳市','地区',259),('辽宁省铁岭县','地区',266),('辽宁省铁岭市','地区',265),('辽宁省铁法市','地区',282),('辽宁省锦  县','地区',249),('辽宁省锦州市','地区',247),('辽宁省锦西市','地区',280),('辽宁省长海县','地区',231),('辽宁省阜新县','地区',257),('辽宁省阜新市','地区',256),('辽宁省鞍山市','地区',233),('辽宁省黑山县','地区',251),('达斡尔族','民族',32),('鄂伦春族','民族',53),('鄂温克族','民族',46),('锡伯族','民族',39),('门巴族','民族',55),('阿昌族','民族',40),('高中','学历',59),('高山族','民族',24),('黎族','民族',20),('黑龙江七台河市','地区',90),('黑龙江东宁县','地区',96),('黑龙江五大连池市','地区',122),('黑龙江五常县','地区',102),('黑龙江伊春市','地区',79),('黑龙江佳木斯市','地区',81),('黑龙江依兰县','地区',83),('黑龙江依安县','地区',339),('黑龙江克东县','地区',69),('黑龙江克山县','地区',68),('黑龙江兰西县','地区',113),('黑龙江友谊县','地区',89),('黑龙江双城市','地区',99),('黑龙江双鸭山市','地区',76),('黑龙江同江市','地区',132),('黑龙江呼兰县','地区',335),('黑龙江呼玛县','地区',127),('黑龙江哈尔滨市','地区',334),('黑龙江嘉荫县','地区',80),('黑龙江塔河县','地区',128),('黑龙江大庆市','地区',78),('黑龙江嫩江县','地区',123),('黑龙江孙吴县','地区',126),('黑龙江宁安县','地区',93),('黑龙江密山市','地区',135),('黑龙江富裕县','地区',66),('黑龙江富锦市','地区',133),('黑龙江尚志市','地区',100),('黑龙江庆安县','地区',117),('黑龙江延寿县','地区',107),('黑龙江抚远县','地区',88),('黑龙江方正县','地区',106),('黑龙江明水县','地区',118),('黑龙江望奎县','地区',112),('黑龙江木兰县','地区',104),('黑龙江杜尔伯特县','地区',65),('黑龙江林口县','地区',97),('黑龙江林甸县','地区',67),('黑龙江桦南县','地区',82),('黑龙江桦川县','地区',84),('黑龙江汤原县','地区',86),('黑龙江泰来县','地区',340),('黑龙江海伦县','地区',111),('黑龙江海林县','地区',94),('黑龙江漠河县','地区',129),('黑龙江牡丹江市','地区',92),('黑龙江甘南县','地区',341),('黑龙江省','地区',333),('黑龙江省勃利县','地区',91),('黑龙江省北安市','地区',121),('黑龙江省安达市','地区',109),('黑龙江省宝清县','地区',85),('黑龙江省宾县','地区',101),('黑龙江省巴彦县','地区',103),('黑龙江省德都县','地区',124),('黑龙江省拜泉县','地区',70),('黑龙江省阿城市','地区',131),('黑龙江穆棱县','地区',95),('黑龙江绥化市','地区',108),('黑龙江绥棱县','地区',119),('黑龙江绥滨县','地区',75),('黑龙江绥芬河市','地区',130),('黑龙江肇东市','地区',110),('黑龙江肇州县','地区',116),('黑龙江肇源县','地区',115),('黑龙江萝北县','地区',74),('黑龙江虎林县','地区',98),('黑龙江讷河县','地区',338),('黑龙江逊克县','地区',125),('黑龙江通河县','地区',105),('黑龙江铁力市','地区',134),('黑龙江集贤县','地区',77),('黑龙江青冈县','地区',114),('黑龙江饶河县','地区',87),('黑龙江鸡东县','地区',72),('黑龙江鸡西市','地区',71),('黑龙江鹤岗市','地区',73),('黑龙江黑河市','地区',120),('黑龙江齐齐哈尔市','地区',336),('黑龙江龙江县','地区',337);
/*!40000 ALTER TABLE `cm_res` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_resgrp`
--

DROP TABLE IF EXISTS `cm_resgrp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_resgrp` (
  `Name` varchar(64) NOT NULL COMMENT '分组名称',
  PRIMARY KEY (`Name`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='基础代码分组';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_resgrp`
--

LOCK TABLES `cm_resgrp` WRITE;
/*!40000 ALTER TABLE `cm_resgrp` DISABLE KEYS */;
INSERT INTO `cm_resgrp` VALUES ('地区'),('学历'),('性别'),('数据类型'),('民族');
/*!40000 ALTER TABLE `cm_resgrp` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_role`
--

DROP TABLE IF EXISTS `cm_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_role` (
  `ID` bigint(20) NOT NULL COMMENT '角色标识',
  `Name` varchar(32) NOT NULL COMMENT '角色名称',
  `Note` varchar(255) DEFAULT NULL COMMENT '角色描述',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='角色';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_role`
--

LOCK TABLES `cm_role` WRITE;
/*!40000 ALTER TABLE `cm_role` DISABLE KEYS */;
INSERT INTO `cm_role` VALUES (1,'任何人','所有用户默认都具有该角色，不可删除'),(2,'系统管理员','系统角色，不可删除'),(22844822693027840,'abc','');
/*!40000 ALTER TABLE `cm_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_rolemenu`
--

DROP TABLE IF EXISTS `cm_rolemenu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_rolemenu` (
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  `MenuID` bigint(20) NOT NULL COMMENT '菜单标识',
  PRIMARY KEY (`RoleID`,`MenuID`) USING BTREE,
  KEY `fk_rolemenu_menuid` (`MenuID`),
  CONSTRAINT `fk_rolemenu_menuid` FOREIGN KEY (`MenuID`) REFERENCES `cm_menu` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_rolemenu_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='角色关联的菜单';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_rolemenu`
--

LOCK TABLES `cm_rolemenu` WRITE;
/*!40000 ALTER TABLE `cm_rolemenu` DISABLE KEYS */;
INSERT INTO `cm_rolemenu` VALUES (1,2),(1,3),(1,4),(1,5),(1,6),(1,7),(1,8),(1,15315637929975808),(2,18860286065975296),(1,84193059722358784),(1,84470870081138688),(1,84471155981676544),(1,84471601248989184);
/*!40000 ALTER TABLE `cm_rolemenu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_roleprv`
--

DROP TABLE IF EXISTS `cm_roleprv`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_roleprv` (
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  `PrvID` varchar(64) NOT NULL COMMENT '权限标识',
  PRIMARY KEY (`RoleID`,`PrvID`) USING BTREE,
  KEY `fk_roleprv_prvid` (`PrvID`),
  CONSTRAINT `fk_roleprv_prvid` FOREIGN KEY (`PrvID`) REFERENCES `cm_prv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_roleprv_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='角色关联的权限';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_roleprv`
--

LOCK TABLES `cm_roleprv` WRITE;
/*!40000 ALTER TABLE `cm_roleprv` DISABLE KEYS */;
INSERT INTO `cm_roleprv` VALUES (22844822693027840,'新权限asdfasdfasd'),(1,'阿斯蒂芬5'),(2,'阿斯蒂芬5'),(22844822693027840,'阿斯蒂芬5');
/*!40000 ALTER TABLE `cm_roleprv` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_rpt`
--

DROP TABLE IF EXISTS `cm_rpt`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_rpt` (
  `ID` bigint(20) NOT NULL COMMENT '报表标识',
  `Name` varchar(64) NOT NULL COMMENT '报表名称',
  `Define` varchar(21000) NOT NULL COMMENT '报表模板定义',
  `Note` varchar(255) NOT NULL COMMENT '报表描述',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='报表模板定义';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_rpt`
--

LOCK TABLES `cm_rpt` WRITE;
/*!40000 ALTER TABLE `cm_rpt` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_rpt` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_sql`
--

DROP TABLE IF EXISTS `cm_sql`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_sql` (
  `id` varchar(128) NOT NULL COMMENT 'sql键值',
  `sql` varchar(20000) NOT NULL COMMENT 'sql内容',
  `note` varchar(255) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='服务的sql语句';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_sql`
--

LOCK TABLES `cm_sql` WRITE;
/*!40000 ALTER TABLE `cm_sql` DISABLE KEYS */;
INSERT INTO `cm_sql` VALUES ('权限-关联用户','select distinct (c.name)\r\n  from cm_roleprv a, cm_userrole b, cm_user c\r\n where a.roleid = b.roleid\r\n   and b.userid = c.id\r\n   and a.prvid = @prvid\r\n order by c.name',NULL),('权限-关联角色','select id as roleid, b.name as rolename, a.prvid\r\n  from cm_roleprv a\r\n  left join cm_role b\r\n    on a.roleid = b.id\r\n where a.prvid = @prvid',NULL),('权限-名称重复','select count(id) from cm_prv where id=@id',NULL),('权限-所有','select * from cm_prv',NULL),('权限-未关联的角色','select id, name, note\r\n  from cm_role a\r\n where not exists (select roleid\r\n          from cm_roleprv b\r\n         where a.id = b.roleid\r\n           and prvid = @prvid)',NULL),('权限-模糊查询','select * from cm_prv where id like @id',NULL),('权限-系统权限','select * from cm_prv where id < 1000',NULL),('用户-关联角色','SELECT\r\n	b.id roleid,\r\n	b.NAME rolename,\r\n	a.userid \r\nFROM\r\n	cm_userrole a,\r\n	cm_role b \r\nWHERE\r\n	a.roleid = b.id \r\n	AND userid = @userid',NULL),('用户-具有的权限','SELECT\r\n	prvid \r\nFROM\r\n	(\r\nSELECT DISTINCT\r\n	( a.prvid ) \r\nFROM\r\n	cm_roleprv a\r\n	LEFT JOIN cm_prv b ON a.prvid = b.id \r\nWHERE\r\n	EXISTS ( SELECT roleid FROM cm_userrole c WHERE a.roleid = c.roleid AND userid = @userid ) \r\n	) t',NULL),('用户-删除用户角色','delete from cm_userrole where userid=@userid and roleid=@roleid',NULL),('用户-可访问的菜单','select name\r\n  from (select distinct (b.id), b.name, dispidx\r\n          from cm_rolemenu a\r\n          left join cm_menu b\r\n            on a.menuid = b.id\r\n         where exists\r\n         (select roleid\r\n                  from cm_userrole c\r\n                 where a.roleid = c.roleid\r\n                   and userid = @userid) or a.roleid=1) t\r\n order by dispidx',NULL),('用户-增加用户角色','insert into cm_userrole (userid, roleid) values (@userid, @roleid)',NULL),('用户-所有','SELECT\r\n	id,\r\n	phone,\r\n	name,\r\n	( CASE sex WHEN 1 THEN \'男\' ELSE \'女\' END ) sex,\r\n	expired,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user',NULL),('用户-最近修改','SELECT\r\n	id,\r\n	phone,\r\n	NAME,\r\n	( CASE sex WHEN 1 THEN \'男\' ELSE \'女\' END ) sex,\r\n	expired,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user \r\nWHERE\r\n	to_days(now()) - to_days(mtime) <= 2',NULL),('用户-未关联的角色','SELECT\r\n	a.id,\r\n	a.NAME\r\nFROM\r\n	cm_role a\r\nWHERE\r\n	NOT EXISTS ( SELECT roleid FROM cm_userrole b WHERE a.id = b.roleid AND userid = @userid )\r\n	AND a.id<>1',NULL),('用户-模糊查询','SELECT\r\n	id,\r\n	phone,\r\n	NAME,\r\n	( CASE sex WHEN 1 THEN \'男\' ELSE \'女\' END ) sex,\r\n	expired,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user \r\nWHERE\r\n	NAME LIKE @input \r\n	OR phone LIKE @input',NULL),('用户-编辑','SELECT\r\n	id,\r\n	phone,\r\n	name,\r\n	sex,\r\n	(case when sex=1 then \'男\' else \'女\' end) as sexname,\r\n	photo,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user \r\nWHERE\r\n	id = @id',NULL),('用户-重复手机号','select count(id) from cm_user where phone=@phone',NULL),('登录-手机号获取用户','select * from cm_user where phone=@phone',NULL),('菜单-id菜单项','SELECT\r\n	a.*,\r\n	b.NAME parentname \r\nFROM\r\n	cm_menu a\r\n	LEFT JOIN cm_menu b ON a.parentid = b.id \r\nWHERE\r\n	a.id = @id',NULL),('菜单-关联的角色','SELECT\r\n	b.id roleid,\r\n	b.NAME rolename,\r\n	a.menuid \r\nFROM\r\n	cm_rolemenu a,\r\n	cm_role b \r\nWHERE\r\n	a.roleid = b.id \r\n	AND menuid = @menuid',NULL),('菜单-分组树','SELECT\r\n	id,\r\n	NAME,\r\n	parentid \r\nFROM\r\n	cm_menu \r\nWHERE\r\n	isgroup = 1 \r\nORDER BY\r\n	dispidx',NULL),('菜单-完整树','SELECT\r\n	id,\r\n	NAME,\r\n	parentid,\r\n	isgroup,\r\n	icon,\r\n	dispidx\r\nFROM\r\n	cm_menu \r\nORDER BY\r\n	dispidx',NULL),('菜单-是否有子菜单','select count(*) from cm_menu where parentid=@parentid',NULL),('菜单-未关联的角色','SELECT\r\n	a.id,\r\n	a.NAME\r\nFROM\r\n	cm_role a\r\nWHERE\r\n	NOT EXISTS ( SELECT roleid FROM cm_rolemenu b WHERE a.id = b.roleid AND menuid = @menuid )',NULL),('角色-关联用户','SELECT\r\n	b.id userid,\r\n	b.NAME username,\r\n	a.roleid \r\nFROM\r\n	cm_userrole a,\r\n	cm_user b \r\nWHERE\r\n	a.userid = b.id\r\n	AND roleid = @roleid',NULL),('角色-关联的权限','select a.prvid, a.roleid\r\n  from cm_roleprv a\r\n  join cm_prv b\r\n    on a.prvid = b.id\r\n where a.roleid = @roleid',NULL),('角色-关联的菜单','select id as menuid, name, a.roleid\r\n  from cm_rolemenu a\r\n  join cm_menu b\r\n    on a.menuid = b.id\r\n where b.isgroup = 0\r\n   and a.roleid = @roleid\r\n order by dispidx',NULL),('角色-名称重复','select count(id) from cm_role where name=@name',NULL),('角色-所有','select * from cm_role',NULL),('角色-未关联的权限','select a.id, a.note\r\n  from cm_prv a\r\n where not exists\r\n (select prvid\r\n          from cm_roleprv b\r\n         where a.id = b.prvid\r\n           and b.roleid = @roleid)',NULL),('角色-未关联的用户','select id, name\r\n  from cm_user a\r\n where not exists (select userid\r\n          from cm_userrole b\r\n         where a.id = b.userid\r\n           and roleid = @roleid)\r\n order by name',NULL),('角色-未关联的菜单','select id, name\r\n  from cm_menu a\r\n where isgroup = 0\r\n   and not exists (select menuid\r\n          from cm_rolemenu b\r\n         where a.id = b.menuid\r\n           and roleid = @roleid)\r\n order by dispidx',NULL),('角色-模糊查询','select * from cm_role where name like @name',NULL),('角色-系统角色','select * from cm_role where id < 1000',NULL),('角色-编辑','SELECT\r\n	id,\r\n	name,\r\n	note\r\nFROM\r\n	cm_role\r\nWHERE\r\n	id = @id',NULL);
/*!40000 ALTER TABLE `cm_sql` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_user`
--

DROP TABLE IF EXISTS `cm_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_user` (
  `ID` bigint(20) NOT NULL COMMENT '用户标识',
  `Phone` char(11) NOT NULL COMMENT '手机号，唯一',
  `Name` varchar(32) NOT NULL COMMENT '姓名',
  `Pwd` char(32) NOT NULL COMMENT '密码的md5',
  `Sex` tinyint(1) NOT NULL DEFAULT '1' COMMENT '性别，0女1男',
  `Photo` varchar(255) NOT NULL DEFAULT '' COMMENT '头像',
  `Expired` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否停用',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `idx_phone` (`Phone`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='系统用户';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_user`
--

LOCK TABLES `cm_user` WRITE;
/*!40000 ALTER TABLE `cm_user` DISABLE KEYS */;
INSERT INTO `cm_user` VALUES (1,'15948371897','daoting','af3303f852abeccd793068486a391626',1,'[[\"v0/E3/18/63458646655102976.jpg\",\"未标题-2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-24 10:42\"]]',0,'2019-10-24 09:06:38','2020-03-24 10:42:01'),(8411237852585984,'15911111111','安卓','b59c67bf196a4758191e42f76670ceba',0,'[[\"v0/7E/55/81569009808306176.jpg\",\"75832742034403328\",\"300 x 300 (jpg)\",49179,\"安卓\",\"2020-05-13 02:06\"]]',0,'2019-10-24 13:03:19','2020-05-13 10:06:08'),(52998151791833088,'13312345678','苹果','674f3c2c1a8a6f90461e8a66fb5550ba',1,'[[\"v0/E9/B4/76177735114682368.jpg\",\"IMG_0002\",\"4288 x 2848 (jpg)\",2604768,\"苹果\",\"2020-04-28 13:03\"]]',0,'0001-01-01 00:00:00','2020-04-28 13:03:11'),(53353150145228800,'13332165498','13332165498','a081c174f5913958ba8c6443bacffcb9',1,'',0,'0001-01-01 00:00:00','0001-01-01 00:00:00'),(55829335999639552,'13512345678','13512345678','674f3c2c1a8a6f90461e8a66fb5550ba',1,'',0,'0001-01-01 00:00:00','0001-01-01 00:00:00'),(76214103471681536,'13099999999','13099999999','fa246d0262c3925617b0c72bb20eeb1d',1,'',0,'0001-01-01 00:00:00','0001-01-01 00:00:00');
/*!40000 ALTER TABLE `cm_user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_userparams`
--

DROP TABLE IF EXISTS `cm_userparams`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_userparams` (
  `UserID` bigint(11) NOT NULL COMMENT '用户标识',
  `ParamID` varchar(64) NOT NULL COMMENT '参数标识',
  `Value` varchar(255) NOT NULL COMMENT '参数值',
  PRIMARY KEY (`UserID`,`ParamID`) USING BTREE,
  KEY `fk_userparams_paramsid` (`ParamID`),
  CONSTRAINT `fk_userparams_paramsid` FOREIGN KEY (`ParamID`) REFERENCES `cm_params` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_userparams_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='用户参数值';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_userparams`
--

LOCK TABLES `cm_userparams` WRITE;
/*!40000 ALTER TABLE `cm_userparams` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_userparams` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_userrole`
--

DROP TABLE IF EXISTS `cm_userrole`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_userrole` (
  `UserID` bigint(20) NOT NULL COMMENT '用户标识',
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`UserID`,`RoleID`) USING BTREE,
  KEY `fk_userrole_userid` (`UserID`),
  KEY `fk_userrole_roleid` (`RoleID`),
  CONSTRAINT `fk_userrole_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_userrole_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='用户关联的角色';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_userrole`
--

LOCK TABLES `cm_userrole` WRITE;
/*!40000 ALTER TABLE `cm_userrole` DISABLE KEYS */;
INSERT INTO `cm_userrole` VALUES (1,2),(8411237852585984,2);
/*!40000 ALTER TABLE `cm_userrole` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `fsm_file`
--

DROP TABLE IF EXISTS `fsm_file`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fsm_file` (
  `ID` bigint(20) unsigned NOT NULL COMMENT '文件标识',
  `Name` varchar(512) NOT NULL COMMENT '文件名称',
  `Path` varchar(512) NOT NULL COMMENT '存放路径：卷/两级目录/id.ext',
  `Size` bigint(20) unsigned NOT NULL COMMENT '文件长度',
  `Info` varchar(512) DEFAULT NULL COMMENT '文件描述',
  `Uploader` bigint(20) unsigned NOT NULL COMMENT '上传人id',
  `Ctime` datetime NOT NULL COMMENT '上传时间',
  `Downloads` bigint(20) unsigned NOT NULL COMMENT '下载次数',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `idx_fsm_file_path` (`Path`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fsm_file`
--

LOCK TABLES `fsm_file` WRITE;
/*!40000 ALTER TABLE `fsm_file` DISABLE KEYS */;
INSERT INTO `fsm_file` VALUES (59189634018439168,'1.jpg','photo/CC/D2/59189634018439168.jpg',40589,'334 x 297 (.jpg)',1,'2020-03-12 15:58:28',1),(59190827587334144,'IMG_20200228_073347.jpg','photo/40/05/59190827587334144.jpg',200090,'960 x 1280 (jpg)',8411237852585984,'2020-03-12 16:03:13',1),(59435697681854464,'MySql_bs0_202001211220.sql','photo/30/D1/59435697681854464.sql',43717,'sql文件',1,'2020-03-13 08:16:15',1),(59471299324276736,'1.jpg','photo/CC/D2/59471299324276736.jpg',49179,'300 x 300 (.jpg)',1,'2020-03-13 10:37:43',0),(59471299760484352,'Logon.wav','photo/AD/FF/59471299760484352.wav',384496,'00:04',1,'2020-03-13 10:37:43',4),(59471299831787520,'mov.mp4','photo/CB/D1/59471299831787520.mp4',788493,'00:00:10 (320 x 176)',1,'2020-03-13 10:37:43',3),(59471299907284992,'profilephoto.jpg','photo/08/64/59471299907284992.jpg',17891,'300 x 300 (.jpg)',1,'2020-03-13 10:37:43',3),(59471299978588160,'苍蝇.wmv','photo/D4/6B/59471299978588160.wmv',403671,'00:00:06 (480 x 288)',1,'2020-03-13 10:37:43',3),(59471300041502720,'文本文档.txt','photo/DB/D6/59471300041502720.txt',8,'txt文件',1,'2020-03-13 10:37:43',1),(59471300070862848,'项目文档.docx','photo/5D/26/59471300070862848.docx',13071,'docx文件',1,'2020-03-13 10:37:43',1),(62011895247138816,'无标题1.png','v0/D3/43/62011895247138816.png',24425,'401 x 665 (.png)',1,'2020-03-20 10:53:10',1),(62013122181722112,'未标题-2.jpg','v0/E3/18/62013122181722112.jpg',49179,'300 x 300 (.jpg)',1,'2020-03-20 10:58:02',0),(62101043526103040,'IMG_20200228_073347.jpg','v0/40/05/62101043526103040.jpg',200090,'960 x 1280 (jpg)',8411237852585984,'2020-03-20 16:47:25',0),(63446669690007552,'1.jpg','v0/CC/D2/63446669690007552.jpg',49179,'300 x 300 (.jpg)',1,'2020-03-24 09:54:25',0),(63454870955225088,'未标题-2.jpg','v0/E3/18/63454870955225088.jpg',49179,'300 x 300 (.jpg)',1,'2020-03-24 10:27:01',0),(63458646655102976,'未标题-2.jpg','v0/E3/18/63458646655102976.jpg',49179,'300 x 300 (.jpg)',1,'2020-03-24 10:42:01',13),(66364004327354368,'mov.mp4','v1/CB/D1/66364004327354368.mp4',788493,'00:00:10 (320 x 176)',8411237852585984,'2020-04-01 11:06:53',0),(66364069729136640,'mov.mp4','v1/CB/D1/66364069729136640.mp4',788493,'00:00:10 (320 x 176)',8411237852585984,'2020-04-01 11:07:08',0),(66364122896134144,'mov.mp4','v1/CB/D1/66364122896134144.mp4',788493,'00:00:10 (320 x 176)',8411237852585984,'2020-04-01 11:07:21',0),(66367788520697856,'Docker for Windows Installer.exe','v1/88/6D/66367788520697856.exe',567050280,'exe文件',1,'2020-04-01 11:22:09',0),(66738006560468992,'1.jpg','chat/CC/D2/66738006560468992.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-02 11:53:02',0),(66738149242302464,'mono-wasm-f5cfc67c8ed.zip','chat/AE/40/66738149242302464.zip',40418077,'zip文件',1,'2020-04-02 11:53:37',0),(66739208513777664,'1.jpg','chat/CC/D2/66739208513777664.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-02 11:57:49',1),(66739283596013568,'Bs.Kehu.Droid.apk','chat/04/36/66739283596013568.apk',70594488,'apk文件',1,'2020-04-02 11:58:08',0),(66766700469415936,'abc.jpg','chat/DD/5D/66766700469415936.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-02 13:47:03',0),(67164400922783744,'abc.jpg','chat/DD/5D/67164400922783744.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-03 16:07:20',0),(67166199503253504,'icon.txt','chat/E3/A6/67166199503253504.txt',1215,'txt文件',1,'2020-04-03 16:14:29',0),(67166750076956672,'ddd.jpg','chat/9F/7E/67166750076956672.jpg',17808,'350 x 311 (.jpg)',1,'2020-04-03 16:16:40',0),(67169438420299776,'Bs.Kehu.Droid.apk','chat/04/36/67169438420299776.apk',70594488,'apk文件',1,'2020-04-03 16:27:22',1),(67176187961405440,'abc.jpg','chat/DD/5D/67176187961405440.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-03 16:54:10',0),(74743280340692992,'1.jpg','chat/CC/D2/74743280340692992.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-24 14:03:06',1),(75475011070980096,'abc.jpg','chat/DD/5D/75475011070980096.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-26 14:30:44',0),(75475154491011072,'1.jpg','chat/CC/D2/75475154491011072.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:31:18',0),(75475459660181504,'1.jpg','chat/CC/D2/75475459660181504.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:32:31',0),(75475573405511680,'1.jpg','chat/CC/D2/75475573405511680.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:32:58',0),(75475736769458176,'1.jpg','chat/CC/D2/75475736769458176.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:33:37',0),(75476112453267456,'1.jpg','chat/CC/D2/75476112453267456.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:35:06',0),(75477724177494016,'1.jpg','chat/CC/D2/75477724177494016.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:41:30',0),(75477785003290624,'abc.jpg','chat/DD/5D/75477785003290624.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-26 14:41:45',0),(75478180089950208,'Chat.xaml','chat/C2/3C/75478180089950208.xaml',11664,'xaml文件',1,'2020-04-26 14:43:19',1),(75478180182224896,'ChatDetail.cs','chat/49/C1/75478180182224896.cs',9813,'cs文件',1,'2020-04-26 14:43:19',1),(75478180270305280,'ChatInputBar.cs','chat/0D/BB/75478180270305280.cs',6433,'cs文件',1,'2020-04-26 14:43:19',0),(75479061795565568,'Chat.xaml','chat/C2/3C/75479061795565568.xaml',11664,'xaml文件',1,'2020-04-26 14:46:49',0),(75479061917200384,'ChatDetail.cs','chat/49/C1/75479061917200384.cs',9813,'cs文件',1,'2020-04-26 14:46:49',0),(75479062026252288,'ChatInputBar.cs','chat/0D/BB/75479062026252288.cs',6433,'cs文件',1,'2020-04-26 14:46:49',0),(75479250497302528,'ChatInputBar.cs','chat/0D/BB/75479250497302528.cs',6433,'cs文件',1,'2020-04-26 14:47:34',0),(75479607776505856,'1.jpg','chat/CC/D2/75479607776505856.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-26 14:49:00',1),(75479847657140224,'1.jpg','chat/CC/D2/75479847657140224.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:49:57',0),(75480158085967872,'1.jpg','chat/CC/D2/75480158085967872.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:51:11',0),(75480389250838528,'1.jpg','chat/CC/D2/75480389250838528.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:52:06',0),(75486424963346432,'Chat.xaml','chat/C2/3C/75486424963346432.xaml',11664,'xaml文件',1,'2020-04-26 15:16:05',0),(75486425064009728,'ChatDetail.cs','chat/49/C1/75486425064009728.cs',9813,'cs文件',1,'2020-04-26 15:16:05',0),(75486425156284416,'ChatInputBar.cs','chat/0D/BB/75486425156284416.cs',6433,'cs文件',1,'2020-04-26 15:16:05',1),(75487019367526400,'1.jpg','chat/CC/D2/75487019367526400.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-26 15:18:26',0),(75745284512935936,'1.jpg','chat/CC/D2/75745284512935936.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-27 08:24:44',0),(75772133746012160,'Chat.xaml','chat/C2/3C/75772133746012160.xaml',11664,'xaml文件',1,'2020-04-27 10:11:25',0),(75772133846675456,'ChatDetail.cs','chat/49/C1/75772133846675456.cs',9813,'cs文件',1,'2020-04-27 10:11:25',0),(75772134421295104,'ChatInputBar.cs','chat/0D/BB/75772134421295104.cs',6433,'cs文件',1,'2020-04-27 10:11:25',1),(75776994612998144,'1.jpg','chat/CC/D2/75776994612998144.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-27 10:30:44',1),(75793731341381632,'1.jpg','chat/CC/D2/75793731341381632.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-27 11:37:14',0),(75793731458822144,'ChatDetail.cs','chat/49/C1/75793731458822144.cs',9813,'cs文件',1,'2020-04-27 11:37:14',0),(75793731551096832,'ChatInputBar.cs','chat/0D/BB/75793731551096832.cs',6433,'cs文件',1,'2020-04-27 11:37:14',1),(75832741728219136,'66739208513777664.jpg','chat/22/93/75832741728219136.jpg',40589,'334 x 297 (jpg)',8411237852585984,'2020-04-27 14:12:14',0),(75832742034403328,'1.jpg','chat/CC/D2/75832742034403328.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-27 14:12:14',1),(75833059291557888,'ChatDetail.cs','chat/49/C1/75833059291557888.cs',9813,'cs文件',1,'2020-04-27 14:13:30',0),(75833059392221184,'ChatInputBar.cs','chat/0D/BB/75833059392221184.cs',6433,'cs文件',1,'2020-04-27 14:13:30',0),(75839635486273536,'75832742034403328.jpg','chat/7E/55/75839635486273536.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-27 14:39:40',2),(75839636979445760,'66739208513777664.jpg','chat/22/93/75839636979445760.jpg',40589,'334 x 297 (jpg)',8411237852585984,'2020-04-27 14:39:40',2),(75844728772227072,'ChatDetail.cs','chat/49/C1/75844728772227072.cs',9813,'cs文件',1,'2020-04-27 14:59:54',0),(75844728864501760,'ChatInputBar.cs','chat/0D/BB/75844728864501760.cs',6433,'cs文件',1,'2020-04-27 14:59:54',2),(76111316666675200,'IMG_0006.HEIC','chat/B7/45/76111316666675200.heic',2808983,'4032 x 3024 (heic)',0,'2020-04-28 08:39:16',0),(76111551325401088,'IMG_0006.HEIC','chat/B7/45/76111551325401088.heic',2808983,'4032 x 3024 (heic)',0,'2020-04-28 08:40:12',1),(76111675015426048,'IMG_0002.JPG','chat/E9/B4/76111675015426048.jpg',2604768,'4288 x 2848 (jpg)',0,'2020-04-28 08:40:41',1),(76113076420472832,'IMG_0001.JPG','chat/98/FE/76113076420472832.jpg',1896240,'4288 x 2848 (jpg)',0,'2020-04-28 08:46:15',0),(76113185799532544,'1.jpg','chat/CC/D2/76113185799532544.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-28 08:46:39',0),(76177735114682368,'IMG_0002.JPG','v0/E9/B4/76177735114682368.jpg',2604768,'4288 x 2848 (jpg)',0,'2020-04-28 13:03:09',10),(76214375992389632,'FullSizeRender.jpg','chat/B9/03/76214375992389632.jpg',2190497,'1242 x 1661 (jpg)',0,'2020-04-28 15:28:44',1),(76214714330116096,'Chat.xaml','chat/C2/3C/76214714330116096.xaml',11664,'xaml文件',1,'2020-04-28 15:30:03',0),(76214714409807872,'ChatDetail.cs','chat/49/C1/76214714409807872.cs',9813,'cs文件',1,'2020-04-28 15:30:03',0),(76214714485305344,'ChatInputBar.cs','chat/0D/BB/76214714485305344.cs',6433,'cs文件',1,'2020-04-28 15:30:03',0),(76486706589462528,'Screenshot_2020-04-28-16-14-02-127_com.miui.packageinstaller.jpg','chat/3E/79/76486706589462528.jpg',168528,'1080 x 2340 (jpg)',8411237852585984,'2020-04-29 09:30:52',1),(76486743985876992,'IMG_0382.JPG','chat/0C/E4/76486743985876992.jpg',32162,'464 x 413 (jpg)',0,'2020-04-29 09:31:01',1),(76486802945208320,'FullSizeRender.jpg','chat/B9/03/76486802945208320.jpg',2190497,'1242 x 1661 (jpg)',0,'2020-04-29 09:31:17',0),(76487071930118144,'Chat.xaml','chat/C2/3C/76487071930118144.xaml',11664,'xaml文件',1,'2020-04-29 09:32:19',0),(76487072018198528,'ChatDetail.cs','chat/49/C1/76487072018198528.cs',9813,'cs文件',1,'2020-04-29 09:32:19',0),(76487072102084608,'ChatInputBar.cs','chat/0D/BB/76487072102084608.cs',6433,'cs文件',1,'2020-04-29 09:32:19',0),(76487229128437760,'Demo修改20190804.doc','chat/66/CE/76487229128437760.doc',572416,'doc文件',1,'2020-04-29 09:32:57',1),(76521621724983296,'IMG_0002.JPG','chat/E9/B4/76521621724983296.jpg',2604768,'4288 x 2848 (jpg)',0,'2020-04-29 11:49:40',0),(76874175394738176,'2c1403a82e214682a53fec9576bae3de.wav','chat/7C/CC/76874175394738176.wav',717164,'wav文件',8411237852585984,'2020-04-30 11:10:32',1),(76874744842809344,'c68ec31ff884453dba2fc388b6269c83.wav','chat/3F/01/76874744842809344.wav',1114604,'wav文件',8411237852585984,'2020-04-30 11:12:48',1),(76879465364189184,'28e292213cb54669a4fb9bda4625e75b.wav','chat/CA/CF/76879465364189184.wav',1287360,'wav文件',8411237852585984,'2020-04-30 11:31:33',1),(76902487953371136,'5b8a06348f814a65bf5654b703442208.m4a','chat/A8/5F/76902487953371136.m4a',13568,'m4a文件',8411237852585984,'2020-04-30 13:03:02',2),(76908769213018112,'3674472da5a74da69c5d05138352785e.m4a','chat/1D/22/76908769213018112.m4a',20791,'m4a文件',8411237852585984,'2020-04-30 13:27:59',0),(76913523729231872,'1.jpg','chat/CC/D2/76913523729231872.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-30 13:46:53',0),(76915286083497984,'1.jpg','chat/CC/D2/76915286083497984.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-30 13:53:53',0),(76917565465423872,'aa.jpg','chat/8F/98/76917565465423872.jpg',17891,'300 x 300 (.jpg)',1,'2020-04-30 14:02:56',0),(76918305026076672,'0dd7d38b5e9840b7a25966fc5bc758e0.m4a','chat/3D/38/76918305026076672.m4a',10445,'m4a文件',8411237852585984,'2020-04-30 14:05:53',0),(76918518470012928,'75832742034403328.jpg','chat/7E/55/76918518470012928.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-30 14:06:44',0),(76923472400216064,'8a98b769cd954a83abd7ecb728ca99b6.m4a','chat/3C/DD/76923472400216064.m4a',35947,'m4a文件',0,'2020-04-30 14:26:25',1),(76926849905455104,'da4ec27f8d0d411fa3adf84a2ac81c3d.m4a','chat/A5/55/76926849905455104.m4a',36346,'m4a文件',0,'2020-04-30 14:39:50',1),(76927511338807296,'2c1cce1c7a5a4e4fa64e619bc89e8845.m4a','chat/7C/21/76927511338807296.m4a',83825,'m4a文件',0,'2020-04-30 14:42:28',0),(76928825422639104,'ed7ffe1c86134f4590208773c09c5c99.m4a','chat/9B/2C/76928825422639104.m4a',81828,'m4a文件',0,'2020-04-30 14:47:41',0),(76929997265039360,'c2ae4eb043554b548c08e540a68b501f.m4a','chat/A8/0E/76929997265039360.m4a',39713,'m4a文件',0,'2020-04-30 14:52:20',1),(76931805316247552,'1194246890104d23952194bda64b7f35.m4a','chat/6A/E7/76931805316247552.m4a',34413,'m4a文件',0,'2020-04-30 14:59:31',1),(76932645045268480,'aaa4d99f180345c3912935e89c3781a1.m4a','chat/C3/1A/76932645045268480.m4a',28588,'m4a文件',0,'2020-04-30 15:02:51',1),(76933570514251776,'f34cd6be374244649d554544bbbf6b70.m4a','chat/DA/47/76933570514251776.m4a',28365,'m4a文件',0,'2020-04-30 15:06:32',1),(76935318695964672,'075d8c8c663248b5bb016a15a67a411a.m4a','chat/FA/FF/76935318695964672.m4a',44631,'m4a文件',0,'2020-04-30 15:13:29',0),(76938013909577728,'edef0e47e31b46d08fb6c552906cf151.m4a','chat/FD/B7/76938013909577728.m4a',40191,'m4a文件',0,'2020-04-30 15:24:11',0),(76943264389656576,'2a7b181ad26448fe93b08a9d002f01f3.m4a','chat/76/1E/76943264389656576.m4a',21854,'m4a文件',8411237852585984,'2020-04-30 15:45:03',1),(76943404798177280,'1280a7e0bcd14ebf9c994166e18d7ea2.m4a','chat/67/3D/76943404798177280.m4a',16385,'m4a文件',8411237852585984,'2020-04-30 15:45:37',1),(76943756880637952,'1035dd18b5c047fcaeb451cbee28ff58.m4a','chat/3A/AC/76943756880637952.m4a',41850,'m4a文件',0,'2020-04-30 15:47:01',1),(76956131226677248,'e13927305ee84574b67a809fb039be87.m4a','chat/8E/2B/76956131226677248.m4a',10250,'m4a文件',8411237852585984,'2020-04-30 16:36:11',0),(76958313313333248,'3d9e10ac3f124ee08990b192c6e88144.m4a','v0/85/86/76958313313333248.m4a',9859,'m4a文件',8411237852585984,'2020-04-30 16:44:51',0),(76958849588654080,'ab6efee7ee594b3dab9b43b5505f72e2.m4a','v0/A4/1F/76958849588654080.m4a',8688,'m4a文件',8411237852585984,'2020-04-30 16:46:59',0),(79006578783416320,'abc.m4a','v0/3E/08/79006578783416320.m4a',28365,'00:06',1,'2020-05-06 08:24:04',1),(79010543533158400,'a87ccdb44a78497996c89b5ba341759c.m4a','chat/5E/F7/79010543533158400.m4a',10250,'00:04',8411237852585984,'2020-05-06 08:39:49',0),(79010668305313792,'916bde9e1d1049e6b070ecc08fad5522.m4a','chat/00/69/79010668305313792.m4a',13178,'00:06',8411237852585984,'2020-05-06 08:40:19',0),(79015215551606784,'011a20c88db94b4283da0094cd7ad2e2.m4a','chat/A0/16/79015215551606784.m4a',8688,'00:03',8411237852585984,'2020-05-06 08:58:23',0),(79015275014254592,'1a9fcfcc7b5a47869438651e4cc9c8e1.m4a','chat/FD/07/79015275014254592.m4a',20595,'00:11',8411237852585984,'2020-05-06 08:58:37',1),(79023211941851136,'abc.m4a','chat/3E/08/79023211941851136.m4a',28365,'00:06',1,'2020-05-06 09:30:09',1),(79023291511992320,'Chat.xaml','chat/C2/3C/79023291511992320.xaml',11664,'xaml文件',1,'2020-05-06 09:30:28',0),(79023895038783488,'e8adb811c0ec4fb689619a56974d889f.m4a','chat/C5/7D/79023895038783488.m4a',8688,'00:03',8411237852585984,'2020-05-06 09:32:52',0),(79025346381213696,'23c5bf765f1b4b7a8e601e5dc34399e3.m4a','chat/0B/EC/79025346381213696.m4a',11031,'00:05',8411237852585984,'2020-05-06 09:38:38',1),(79029054041092096,'mov_bbb.mp4','chat/7A/1B/79029054041092096.mp4',788493,'00:00:10 (320 x 176)',1,'2020-05-06 09:53:22',2),(79035688339501056,'00_04.wav','chat/ED/60/79035688339501056.wav',384496,'00:04',1,'2020-05-06 10:19:44',1),(79046986980782080,'0cad6a4134b14b51bd553f8ab0394beb.m4a','chat/85/2E/79046986980782080.m4a',41324,'00:05',0,'2020-05-06 11:04:38',0),(79103159838830592,'d9c0858bf5a04e6d95640a3150dbe73b.m4a','chat/01/FF/79103159838830592.m4a',8688,'00:03',8411237852585984,'2020-05-06 14:47:42',1),(79104313280819200,'b945e8d778dd491486f1e2a2f0a55bc2.m4a','chat/6C/9C/79104313280819200.m4a',9664,'00:04',8411237852585984,'2020-05-06 14:52:17',0),(79104348408115200,'74a2a0e1890140559763efbb99f1043e.m4a','chat/2C/3F/79104348408115200.m4a',12983,'00:06',8411237852585984,'2020-05-06 14:52:25',1),(80179499258212352,'ef3e4a98c2b740fb9b2c9669494fce30.mp4','v0/CE/3A/80179499258212352.mp4',538460,'00:05 (480 x 360)',0,'2020-05-09 14:04:44',0),(80179624273637376,'09977e213270434bafd297831a344979.jpg','chat/54/92/80179624273637376.jpg',5179868,'4032 x 3024 (jpg)',0,'2020-05-09 14:05:15',1),(80179802284093440,'0f4003d774ea4bfc9db17a5b5437dbaf.mp4','chat/BE/A5/80179802284093440.mp4',475298,'00:04 (480 x 360)',0,'2020-05-09 14:05:55',1),(80180869143064576,'IMG_0779.MOV','chat/DB/CC/80180869143064576.mov',5353019,'1080 x 1920 (mov)',0,'2020-05-09 14:10:14',2),(80184072756654080,'IMG_0779.MOV','chat/DB/CC/80184072756654080.mov',5353019,'1080 x 1920 (mov)',0,'2020-05-09 14:23:01',1),(80184336418992128,'f1ad3e0d5f5643179865617ff20cc4be.mp4','chat/E9/33/80184336418992128.mp4',446906,'00:04 (360 x 480)',0,'2020-05-09 14:23:56',0),(80184570809282560,'da55422eef0845da8ef7b055234067ca.mp4','chat/40/14/80184570809282560.mp4',354452,'00:03 (360 x 480)',0,'2020-05-09 14:24:52',1),(80187945395286016,'3f51cfc9fb664e899e36d3e183e58e63.mp4','chat/F0/5E/80187945395286016.mp4',8148024,'00:04 (1080 x 1920)',0,'2020-05-09 14:38:21',1),(80205717391142912,'3d2916be9d24421b899151683e943cc0.mp4','chat/18/B0/80205717391142912.mp4',11453919,'00:00:04 (1920 x 1080)',8411237852585984,'2020-05-09 15:48:59',1),(80205991891562496,'15a83ed344e84a1eb7c8af7f62c3e668.jpg','chat/98/15/80205991891562496.jpg',4987986,'4032 x 3024 (jpg)',0,'2020-05-09 15:50:03',1),(80206131020820480,'bbab9e607b664d32b689d6d88251d0e7.mp4','chat/C3/70/80206131020820480.mp4',7084191,'00:03 (1080 x 1920)',0,'2020-05-09 15:50:36',1),(80206528108163072,'fc6dbfefaded46bf802925f2ba689733.mp4','v0/9C/02/80206528108163072.mp4',7433745,'00:00:02 (1920 x 1080)',8411237852585984,'2020-05-09 15:52:10',0),(80212384711307264,'62b0b11df62b456abae5cfe34caefc21.jpg','chat/56/F0/80212384711307264.jpg',71303,'720 x 1280 (jpg)',8411237852585984,'2020-05-09 16:15:23',0),(80819234523705344,'3edf58c1385e44f8bfd8b083ed0d746c.jpg','v0/21/06/80819234523705344.jpg',36129,'640 x 480 (jpg)',1,'2020-05-11 08:26:50',0),(80819439436427264,'354e62bc31064830b38da77fe0fd85c7.jpg','v0/78/AE/80819439436427264.jpg',36129,'640 x 480 (jpg)',1,'2020-05-11 08:27:39',0),(80826125681291264,'e1343db5a8da4ac9a312a47a4d495af2.mp4','v0/00/06/80826125681291264.mp4',1414027,'00:00:04 (640 x 480)',1,'2020-05-11 08:54:13',0),(81563700230483968,'1facf6c9ecfa4e509902c66aa79951b0.m4a','chat/1E/57/81563700230483968.m4a',44357,'00:06',0,'2020-05-13 09:45:02',0),(81563797735469056,'a5ec2a5795aa44e6a5da00a1a29356f7.jpg','chat/8D/4E/81563797735469056.jpg',4970312,'4032 x 3024 (jpg)',0,'2020-05-13 09:45:29',1),(81569009808306176,'75832742034403328.jpg','v0/7E/55/81569009808306176.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-05-13 10:06:08',44),(88875538962051072,'u285.jpg','v0/A1/62/88875538962051072.jpg',263911,'1024 x 900 (.jpg)',1,'2020-06-02 13:59:39',4),(91373962043191296,'2.jpg','v0/1D/89/91373962043191296.jpg',25960,'640 x 769 (.jpg)',1,'2020-06-09 11:27:30',1),(91374008964870144,'IMG_20150518_124337.jpg','v0/74/1E/91374008964870144.jpg',517556,'1080 x 811 (.jpg)',1,'2020-06-09 11:27:41',1),(91786038024728576,'IMG_20160818_145515.jpg','v0/CF/B2/91786038024728576.jpg',2691194,'4160 x 2336 (.jpg)',1,'2020-06-10 14:44:56',0),(91786445488779264,'IMG_20150518_125023.jpg','v0/3D/E6/91786445488779264.jpg',510138,'1080 x 801 (.jpg)',1,'2020-06-10 14:46:34',0),(91789217609150464,'VID_20160930_100110.mp4','v0/26/2D/91789217609150464.mp4',25116782,'00:00:24 (1920 x 1080)',1,'2020-06-10 14:57:35',2),(91795724492992512,'VID_20160930_101031.mp4','v0/8F/9E/91795724492992512.mp4',3959865,'00:00:03 (1920 x 1080)',1,'2020-06-10 15:23:26',1),(91795768751288320,'IMG_20160921_134610.jpg','v0/4E/93/91795768751288320.jpg',2662560,'4160 x 2336 (.jpg)',1,'2020-06-10 15:23:36',0),(91800378303967232,'IMG_20150518_124913.jpg','v0/60/55/91800378303967232.jpg',541402,'1080 x 805 (.jpg)',1,'2020-06-10 15:41:55',1),(91800491223019520,'6.jpg','v0/61/62/91800491223019520.jpg',46138,'960 x 540 (.jpg)',1,'2020-06-10 15:42:22',0),(91800491357237248,'7.jpg','v0/B2/18/91800491357237248.jpg',34263,'540 x 960 (.jpg)',1,'2020-06-10 15:42:22',1),(91800491449511936,'IMG_20150518_125051.jpg','v0/F4/5B/91800491449511936.jpg',472877,'1080 x 802 (.jpg)',1,'2020-06-10 15:42:22',0),(91803792962351104,'2-5.JPG','v0/09/EE/91803792962351104.jpg',2190807,'4000 x 3000 (.jpg)',1,'2020-06-10 15:55:29',0),(91803835261906944,'CIMG5332.JPG','v0/B8/94/91803835261906944.jpg',2226785,'4000 x 3000 (.jpg)',1,'2020-06-10 15:55:39',0),(91803936436908032,'IMG_20160906_145630.jpg','v0/65/1F/91803936436908032.jpg',3020173,'4160 x 2336 (.jpg)',1,'2020-06-10 15:56:04',0),(91803998420332544,'IMG_20160906_145451.jpg','v0/EA/34/91803998420332544.jpg',2513845,'4160 x 2336 (.jpg)',1,'2020-06-10 15:56:18',0),(91804086945312768,'IMG_20160818_150302.jpg','v0/21/92/91804086945312768.jpg',2578809,'4160 x 2336 (.jpg)',1,'2020-06-10 15:56:39',0),(92529784261570560,'u354.png','v0/00/F7/92529784261570560.png',296599,'553 x 291 (.png)',1,'2020-06-12 16:00:18',0),(105124916462743552,'公司服务器及网络.txt','chat/5F/37/105124916462743552.txt',435,'txt文件',1,'2020-07-17 10:08:54',1);
/*!40000 ALTER TABLE `fsm_file` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `fsm_sql`
--

DROP TABLE IF EXISTS `fsm_sql`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fsm_sql` (
  `id` varchar(128) NOT NULL COMMENT 'sql键值',
  `sql` varchar(20000) NOT NULL COMMENT 'sql内容',
  `note` varchar(255) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fsm_sql`
--

LOCK TABLES `fsm_sql` WRITE;
/*!40000 ALTER TABLE `fsm_sql` DISABLE KEYS */;
INSERT INTO `fsm_sql` VALUES ('上传文件','INSERT INTO fsm_file ( id, NAME, path, size, uploader, info, ctime, downloads )\r\nVALUES\r\n	( @id, @NAME, @path, @size, @uploader, @info, now( ), 0 )',NULL),('增加下载次数','update fsm_file set downloads=downloads+1 where path=@path',NULL);
/*!40000 ALTER TABLE `fsm_sql` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pub_category`
--

DROP TABLE IF EXISTS `pub_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pub_category` (
  `ID` bigint(20) NOT NULL COMMENT '分类标识',
  `Name` varchar(255) NOT NULL COMMENT '名称',
  `Desc` varchar(1024) NOT NULL COMMENT '详细说明',
  `ParentID` bigint(20) DEFAULT NULL COMMENT '父分类标识',
  PRIMARY KEY (`ID`),
  KEY `fk_category_parentid` (`ParentID`),
  CONSTRAINT `fk_category_parentid` FOREIGN KEY (`ParentID`) REFERENCES `pub_category` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='文章分类';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pub_category`
--

LOCK TABLES `pub_category` WRITE;
/*!40000 ALTER TABLE `pub_category` DISABLE KEYS */;
/*!40000 ALTER TABLE `pub_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pub_comment`
--

DROP TABLE IF EXISTS `pub_comment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pub_comment` (
  `ID` bigint(20) NOT NULL COMMENT '评论标识',
  `PostID` bigint(20) NOT NULL COMMENT '文章标识',
  `Content` varchar(4000) NOT NULL COMMENT '评论内容',
  `UserID` bigint(20) NOT NULL COMMENT '评论人标识',
  `UserName` varchar(64) NOT NULL COMMENT '评论人',
  `Ctime` datetime NOT NULL COMMENT '评论时间',
  `IsSpam` tinyint(1) NOT NULL COMMENT '是否为垃圾评论',
  `ParentID` bigint(20) DEFAULT NULL COMMENT '上级评论标识',
  `Support` int(11) NOT NULL COMMENT '对该评论的支持数',
  `Oppose` int(11) NOT NULL COMMENT '对该评论的反对数',
  PRIMARY KEY (`ID`),
  KEY `fk_comment_postid` (`PostID`),
  KEY `fk_comment_parentid` (`ParentID`),
  CONSTRAINT `fk_comment_parentid` FOREIGN KEY (`ParentID`) REFERENCES `pub_comment` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_comment_postid` FOREIGN KEY (`PostID`) REFERENCES `pub_post` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='评论信息';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pub_comment`
--

LOCK TABLES `pub_comment` WRITE;
/*!40000 ALTER TABLE `pub_comment` DISABLE KEYS */;
/*!40000 ALTER TABLE `pub_comment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pub_meta`
--

DROP TABLE IF EXISTS `pub_meta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pub_meta` (
  `ID` bigint(20) NOT NULL COMMENT '素材标识',
  `Name` varchar(255) NOT NULL COMMENT '素材名称',
  `Type` tinyint(4) NOT NULL COMMENT '类型，0分组 1图片 2视频 3音频 4模板',
  `Content` varchar(8000) NOT NULL COMMENT '素材内容',
  `ParentID` bigint(20) DEFAULT NULL COMMENT '上级目录',
  `CreatorID` bigint(20) NOT NULL COMMENT '创建人',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`ID`),
  KEY `fk_meta_parentid` (`ParentID`),
  CONSTRAINT `fk_meta_parentid` FOREIGN KEY (`ParentID`) REFERENCES `pub_meta` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='素材库';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pub_meta`
--

LOCK TABLES `pub_meta` WRITE;
/*!40000 ALTER TABLE `pub_meta` DISABLE KEYS */;
/*!40000 ALTER TABLE `pub_meta` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pub_post`
--

DROP TABLE IF EXISTS `pub_post`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pub_post` (
  `ID` bigint(20) NOT NULL COMMENT '文章标识',
  `Title` varchar(255) NOT NULL COMMENT '标题',
  `Cover` varchar(1024) NOT NULL COMMENT '封面',
  `Summary` varchar(512) NOT NULL COMMENT '摘要',
  `Content` text NOT NULL COMMENT '内容',
  `TempType` tinyint(4) NOT NULL COMMENT '在列表中显示时的模板类型',
  `IsPublish` tinyint(1) NOT NULL COMMENT '是否发布',
  `AllowCoverClick` tinyint(1) NOT NULL COMMENT '封面可点击',
  `AllowComment` tinyint(1) NOT NULL COMMENT '是否允许评论',
  `AddAlbumLink` tinyint(1) NOT NULL COMMENT '文章末尾是否添加同专辑链接',
  `AddCatLink` tinyint(1) NOT NULL COMMENT '文章末尾是否添加同分类链接',
  `Url` varchar(128) NOT NULL COMMENT '文章地址',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `CreatorID` bigint(20) NOT NULL COMMENT '创建人ID',
  `Creator` varchar(32) NOT NULL COMMENT '创建人',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `LastEditorID` bigint(20) DEFAULT NULL COMMENT '最后编辑人ID',
  `LastEditor` varchar(32) DEFAULT NULL COMMENT '最后编辑人',
  `Mtime` datetime DEFAULT NULL COMMENT '最后修改时间',
  `ReadCount` int(11) NOT NULL COMMENT '阅读次数',
  `CommentCount` int(11) NOT NULL COMMENT '评论总数',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='文章/帖子';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pub_post`
--

LOCK TABLES `pub_post` WRITE;
/*!40000 ALTER TABLE `pub_post` DISABLE KEYS */;
INSERT INTO `pub_post` VALUES (84827705988476928,'介护福祉士','[[\"v0/CF/B2/91786038024728576.jpg\",\"IMG_20160818_145515\",\"4160 x 2336 (.jpg)\",2691194,\"daoting\",\"2020-06-10 14:44\"],[\"v0/3D/E6/91786445488779264.jpg\",\"IMG_20150518_125023\",\"1080 x 801 (.jpg)\",510138,\"daoting\",\"2020-06-10 14:46\"]]','福祉堂以创建“每个人都向往的养老生活！”为己任，通过养老教育，培养专业人才，通过专业人才提供“预防失能、失能康复”健康养老服务，延长健康寿命，提高本人及家庭生活质量。希望得到政府的关注和扶持，加快企业发展步伐，服务老人、福祉中国。','<p style=\"text-align: center;\"><span style=\"font-size:21px;font-family:宋体;background:#D9D9D9;\">介护福祉士</span><span style=\'font-size:21px;font-family:\"Century\",\"serif\";background:#D9D9D9;\'>1</span></p><ul style=\"list-style-type: undefined;margin-left:26px;\"><li><span style=\"font-size:19px;font-family:宋体;\">人间的尊严与自立</span></li></ul><ul style=\"list-style-type: undefined;margin-left:62px;\"><li><span style=\"font-size:19px;font-family:宋体;\">人间的尊严与自立的意义</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80px;\"><li><span style=\"font-size:19px;font-family:宋体;\">所谓的理解人间是</span><span style=\"font-size:19px;font-family:宋体;\">┈</span></li><li><span style=\"font-size:19px;font-family:宋体;\">人间的尊严的意义</span></li><li><span style=\"font-size:19px;font-family:宋体;\">自立的意义</span></li><li><span style=\"font-size:19px;font-family:宋体;\">自立与自律</span></li><li><span style=\"font-size:19px;font-family:宋体;\">人间的尊严与自立</span></li></ol><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;background:#D9D9D9;\">演习</span><span style=\"font-size:19px;\">&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">所谓的理解人间究竟有什么样的理解方式呢，每个人从自己的经验进行讨论交流</span></p><p><span class=\"fr-video fr-deletable fr-fvc fr-dvb fr-draggable\" contenteditable=\"false\" draggable=\"true\"><video controls=\"controls\" height=\"360\" poster=\"../../fsm/photo/mov-t.jpg\" preload=\"none\" src=\"../../fsm/photo/mov.mp4\" width=\"640\"><br></video></span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;background:#D9D9D9;\">演习</span><span style=\"font-size:19px;\">&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">对生命的敬畏与其</span><span style=\"font-size:19px;font-family:宋体;\">说</span><span style=\"font-size:19px;font-family:宋体;\">作为知识的理解，倒不如说是从共感的感动的理解中诞生的，举出身边的事例进行讨论交流</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;background:#D9D9D9;\">演习</span><span style=\"font-size:19px;\">&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">从第</span><span style=\"font-size:19px;\">3</span><span style=\"font-size:19px;font-family:宋体;\">页的小</span><span style=\"font-size:19px;\">A</span><span style=\"font-size:19px;font-family:宋体;\">与小</span><span style=\"font-size:19px;\">B</span><span style=\"font-size:19px;font-family:宋体;\">的事例学习，每个的个别的状况，就自立具有的意义进行讨论交流</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;background:#D9D9D9;\">演习</span><span style=\"font-size:19px;\">&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">明确自立与自律的分别具有的意义，就两者的关系进行思考</span></p><ul style=\"list-style-type: undefined;margin-left:62px;\"><li><span style=\"font-size:19px;font-family:宋体;\">围绕尊严和自立的历史与内容</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80px;\"><li><span style=\"font-size:19px;font-family:宋体;\">人权，尊严与自立的思想</span></li></ol><p><span style=\"font-size:19px;font-family:宋体;\">人权，围绕尊严与自立的历史的经过</span></p>',0,1,0,0,0,0,'202006/91793303565889536.html',6,1,'daoting','2020-05-22 09:55:01',NULL,'',NULL,0,0),(84831009241952256,'人体的构造和机能及疾病V1223','[[\"v0/8F/9E/91795724492992512.mp4\",\"VID_20160930_101031\",\"00:00:03 (1920 x 1080)\",3959865,\"daoting\",\"2020-06-10 15:23\"],[\"v0/4E/93/91795768751288320.jpg\",\"IMG_20160921_134610\",\"4160 x 2336 (.jpg)\",2662560,\"daoting\",\"2020-06-10 15:23\"]]','共十三章389页','<p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:24px;background:silver;\">21</span><span style=\"font-size:24px;font-family:宋体;background:silver;\">世纪健康养老服务专业系列教材之</span><span style=\"font-size:24px;background:silver;\">&mdash;&mdash;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:29px;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">人体的构造、机能、疾病</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:19px;font-family:宋体;\">刘 &nbsp;纯 等编著</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:24px;font-family:宋体;\">百岁福祉教育科技有限公司</span></strong></p><p><span style=\"font-size:16px;font-family:宋体;\"><br>&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:16px;font-family:宋体;\">前 &nbsp;言</span></p><p><span style=\"font-size:16px;font-family:宋体;\"><br>&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:16px;font-family:宋体;\">目 &nbsp;录</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><div style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><ul style=\"margin-bottom:0cm;list-style-type: undefined;margin-left:0cmundefined;\"><li style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">人的成长、发育和衰老</span></li></ul></div><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第一节 身体的成长、发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、胎儿期的成长和发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、幼儿期的成长和发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、儿童期的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">4</span><span style=\"font-size:19px;font-family:宋体;\">、青年、中年期的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第二节 精神的成长、发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、胎儿期的知觉和脑、神经的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、幼儿期的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、儿童期的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">4</span><span style=\"font-size:19px;font-family:宋体;\">、青年、中年期的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第三节 衰老</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、高龄期的生活</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、体力和营养</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、衰老的前兆</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">4</span><span style=\"font-size:19px;font-family:宋体;\">、衰老伴随的身体、生理方面的变化</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">5</span><span style=\"font-size:19px;font-family:宋体;\">、衰老伴随的精神方面的变化</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">第二章 身体构造和身心的机能</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第一节 身体部位的名称</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、身体的名称</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、骨骼的名称</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第二节 各器官的构造及机能</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、水份和脱水</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、血液的成分</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、心脏的构造和循环</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">4</span><span style=\"font-size:19px;font-family:宋体;\">、肾脏的构造和泌尿系统</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">5</span><span style=\"font-size:19px;font-family:宋体;\">、呼吸器官的构造和呼吸</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">6</span><span style=\"font-size:19px;font-family:宋体;\">、消化和吸收</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">7</span><span style=\"font-size:19px;font-family:宋体;\">、神经的构造和机能</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">8</span><span style=\"font-size:19px;font-family:宋体;\">、身体机能的调节</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">第三章 疾病的概要</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第一节 生活习惯病和亚健康</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、生活习惯病</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、亚健康</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第二节 恶性肿瘤</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、主要的恶性肿瘤的统计</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、治疗的基本</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、缓解治理和告知</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第三节 脑血管疾病</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、脑梗塞</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、脑出血和其他的脑血管疾病</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第四节 心脏疾病</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、缺血性心疾病</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、心力衰竭</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、心律不齐</span></p>',0,1,0,1,1,1,'202006/91795143548334080.html',7,1,'daoting','2020-05-22 10:08:09',NULL,'',NULL,0,0),(87098359274139648,'杨先生脑卒中康复','[[\"v0/26/2D/91789217609150464.mp4\",\"VID_20160930_100110\",\"00:00:24 (1920 x 1080)\",25116782,\"daoting\",\"2020-06-10 14:57\"]]','','<p><span class=\"fr-video fr-deletable fr-fvc fr-dvb fr-draggable\" contenteditable=\"false\" draggable=\"true\"><video class=\"fr-draggable\" controls=\"controls\" height=\"360\" poster=\"../../fsm/v0/26/2D/91789217609150464-t.jpg\" preload=\"none\" src=\"../../fsm/v0/26/2D/91789217609150464.mp4\" width=\"640\"><br></video></span></p>',0,1,1,0,0,0,'202006/91789935531388928.html',75,1,'daoting','2020-05-28 16:17:47',NULL,'',NULL,0,0),(88875481210679296,'废用综合症','[[\"v0/A1/62/88875538962051072.jpg\",\"u285\",\"1024 x 900 (.jpg)\",263911,\"daoting\",\"2020-06-02 13:59\"],[\"v0/1D/89/91373962043191296.jpg\",\"2\",\"640 x 769 (.jpg)\",25960,\"daoting\",\"2020-06-09 11:27\"],[\"v0/74/1E/91374008964870144.jpg\",\"IMG_20150518_124337\",\"1080 x 811 (.jpg)\",517556,\"daoting\",\"2020-06-09 11:27\"]]','在康复的世界，一个最大的目标就是预防废用综合症为前提的。','<p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Calibri\",\"sans-serif\";font-size:14px;\'><strong><span style=\"font-size:48px;font-family:宋体;\">「废用综合症」</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">在康复的世界，一个最大的目标就是预防废用综合症为前提的。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">废用综合症是指「由于身体的不活动引起的二次性障碍的総称」。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-size:21px;font-family:宋体;\">废用综合症的症状</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">首先，最易想到的就是对筋肉和骨、关节等运动器官的障碍。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">运动器官的障碍</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">&bull;筋萎縮（筋力低下）</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">&bull;筋肉的伸張性的低下＝易外伤</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">怪我</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">&bull;骨密度的低下＝易骨折</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">&bull;关节挛缩</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">可动区域的低下</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">等。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">当然、废用综合症、不仅运动器官、会在全身各处出现症状的。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">人体的内臓，很多都是通过重力，施加适当的负荷，方能维持正常的功能。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">心臓、在站立時、将血液充分送到头部进行收缩、起到泵的作用。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">总是处于卧床的状态的话、这个泵功能没能被充分使用、会逐步变弱。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">其結果、就会出现起立性低血圧等的症状。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">人体，在排尿</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">排便时，膀胱等的排尿</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">排便器官也是利用重力的効果、将尿和便排出体外。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">卧床不起的话、这些功能会低下、出现排尿障碍和便秘等的症状。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">同时、卧床不起、从外界来的刺激（光</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">味</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">音等）的量极度低下，对大脑的刺激减少。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">而且不运动、从关节和筋肉来的体性感觉（触觉和圧觉）等刺激大脑的机会也減少。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">平时、由外界来的刺激成为脑的恰当的负荷、通过对其的反应而保持正常的功能。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">长期卧床不起会导致认知功能的低下和认知症、精神状态的変化、抑郁、幻想和妄想等。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Calibri\",\"sans-serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">废用综合症常见症状的原因</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">关节挛缩</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">由于麻痺和疼痛等为原因，会发生长时间不活动身体。持续相同姿势的话，关节会变硬，逐步变得不能活动。也就是关节挛缩。由于筋肉和靭帯、失去軟部组织的长度和柔软性、可动性，关节会固硬、会伴有一活动就疼痛的状态。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">在废用综合症中，关节挛缩是最大的問題。关节发生挛缩的话，由于关节可动区域受限、不能进行的日常生活的动作增加感到既不方便、介助者也会因此产生额外的负担。因此，需要一点点地活动固硬的关节、即使症状严重也要尽可能地一点一点地活动关节预防关节挛缩。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">肺炎</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">由于进食机会减少、脑梗塞和注意力的低下、免疫力的低下等，发生吞咽障碍、引起误咽性肺炎的危险率极高。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">肺炎的症状，会出现发烧、咳嗽、痰増加等、但高齢者自觉症状少，意思表示也不充分、常使肺炎严重化。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">观察項目、能够安全地食用的，食物大小、形状、保持座位容易吞咽的姿势，确认是否有呛的反应。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">食欲不振</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">废用综合症、对以前喜欢感兴趣的，一切的意欲会逐步減退（一般除烟、酒外）。这样的有气无力状态会导致对进食意欲减退、常出现食欲不振。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">食欲不振的观察項目、是否比平时吃得少或询问本人，确认体重等。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">便秘</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">由于不活动身体、腸的蠕动运动低下，发生便秘。也有由于进餐量减少，咀嚼力低落和唾液和消化液的分泌量低下等原因，引起便秘。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">观察項目</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>：</span><span style=\"font-family:宋体;\">进食</span><span style=\"font-family:宋体;\">量、是否偏食、</span>1<span style=\"font-family:宋体;\">天</span>25<span style=\"font-family:宋体;\">～</span>30<span style=\"font-family:宋体;\">品目食食用否、是否保持</span>3<span style=\"font-family:宋体;\">餐。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">尿路感染症</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">废用综合症，由于身体的活动状态处于不活発，血液中的钙质要从骨中补充。</span><span style=\"font-family:宋体;\">这样，很多钙被排出到尿液中と。</span><span style=\"font-family:宋体;\">由于卧床的姿势，尿液滞留尿管易形成結石、引起尿路感染</span> &nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">尿路感染的观察項目，确认「是否发烧、排尿时是否有痛感或不适感、能否憋住尿、是否頻尿、是否有下腹部的疼痛感或不适感、是否腰痛、是否有残尿感」。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">浮腫</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">筋肉持续不用的状态话、運搬血液的泵的作用低下。并且、由于食欲低下，营养不足的状态持续、調整血管内的浸透圧的、血液中的蛋白質的白蛋白降低的话，从血管内来的组织液増加、引起浮腫。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">另外，静脈血栓症和腎功能低下也会发生浮腫。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">浮腫的观察項目，用手指按压仙骨或胫骨的皮膚下的位置約</span>1<span style=\"font-family:宋体;\">分左右进行测试。血液中的白蛋白低下引起的浮腫时，指压痕迹会残留</span>40<span style=\"font-family:宋体;\">秒后恢复。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">认知症</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">废用综合症的不活発性生活、几乎的患者都存在认知功能低下的症状。认知功能的低下重症化时，会变成重度的认知症、由于给日常生活和康复带来障碍，所以废用综合症、同身体功能同样，要有意识地进行精神功能的介护</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">认知症的观察項目，使用長谷川式簡易知能评价基准。「能否说出年齢、能否知道現在的時間、场所、是否能够记忆</span>3<span style=\"font-family:宋体;\">个单词、能否进行简单的計算、数字是否能倒数、过一会儿是否能说出这</span>3<span style=\"font-family:宋体;\">个单词、是否能记住</span>5<span style=\"font-family:宋体;\">个物品的地点、言语流畅否」进行评价。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">抑郁</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">高齢者本人很少提到的抑郁，郁闷。由于废用综合症引起的认知功能的低下和行动力的低下，成为慢性的压抑是抑郁的誘因。并且、不活动和失去了对以前感兴趣的事、生活整体意欲減退、心情沉闷也会变成抑郁，郁闷的。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">抑郁的观察項目，「心情是否沉落、是否有至今一直感兴趣的事不感兴趣了、食欲是否減退、睡眠充足否、是否说话和动作变得缓慢、是否易疲劳、是否感到罪悪感、是否有集中力、是否有自殺願望」等。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">心功能低下</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">心臓的泵功能，由于長期卧床的姿势会逐步衰退、特别是高齢者的衰退极为显著。同时、心臓的泵功能衰退，再加上上下肢手脚的骨格筋的泵功能低下的话，返回心臓的血液量减少、其結果是从心臓被送出的血液量也减少。废用综合症，特别是下肢的易明显衰退，导致心功能的低下。</span></p>',0,1,0,0,0,0,'202006/91788569173946368.html',142,1,'daoting','2020-06-02 13:59:27',NULL,'',NULL,0,0),(91797036454506496,'社会福祉原論','','所谓的健康是什么「所谓的需要介护的状态」，是由于「心」或「身体」具有某种不自由「障碍」给生活上带来困难的状态。','<p style=\"text-align: center;\"><span style=\"font-size: 30px;\">养成福祉士教材14</span></p><p>&nbsp;&nbsp;心与身体的机理</p><p>序章</p><p>所谓的健康是什么「所谓的需要介护的状态」，是由于「心」或「身体」具有某种不自由「障碍」给生活上带来困难的状态。</p><p>本章首先就不需要介护的状态也就是「健康的状态」进行理解，在理解了精神上的、身体上的、社会性的健康之后，学习判断利用者健康状态的基准，并且作为介护福祉士学习判断利用者生活困难程度的基准的知识。</p><p>①健康的定义</p><p>●身体、心、社会性的健康</p><p>「何为健康」，如果被问了，何为健康呢？你会怎样回答呢？恐怕我想会回答「蛮精神的！」「没有病的状态」等的回答会是一般性的。这样的回答也绝不是错误的，但是这是否可以说是100%的说对了还多少不一样。实际上，所谓的健康是具有很多很多，多方面的很深奥的东西的。</p><p>把健康，坦率的说的话，那么就是WHO宪法的前文，这里的健康的定义如下：「所谓的健康就是身体上、精神上、社会上、适应上完全处于良好的状态，而不是单纯的指疾病或病弱」。加上「心灵的」「动态的」这两个语言，再一看似乎觉得不太懂的语言，但是这是在考虑「所谓的健康是什么」时是具有非常重要的意义的。</p><p>「心灵的」原文是「Spartitual」也就是心灵一般来讲是由心灵的体验或宗教的印象。在WHO中被解释成各种各样的，但是有一个是可以说的都包含着「人类的尊严」和「<strong>生活质量</strong>」的意思。</p><p>P2旁边：WHO:World Health Organization，日语被意为世界保健机构。是国联机构之一，是进行灭绝疾病的研究和医疗普及等，将健康作为人权之一来认为，并且以达成为目的。生活的质量：并不是「只活着」而是「如何活着」，其质量是非常重要的。主要在医疗和福祉现场被使用，与延命处置同时和解除痛苦，将患者能够平稳的度过的方法探取等中诞生。</p><p>&nbsp; &nbsp;&nbsp;例如说，在介护机构中，有时会给在半夜失禁的高龄者穿尿不湿，与此介护者会轻松简单的。但是利用者本人呢，有事会感到「屈辱的」，失去了尊严「人的尊严」。如果站在重视人的尊严的立场上的话，我们可以认为尿不湿尽可能地不要给其穿为好。</p><p>&nbsp; &nbsp;&nbsp;并且对于癌症患者来说，为了治疗癌症会投入很多抗癌药物，即使是面向好转，但另一方面会发生头发、眉毛脱落的副作用的现象，尤其是女性由于容姿的变化所带来的非常难受的体验，为了不降低其「生活质量」，我们要考虑化妆啊或准备头套等的想法。</p><p>&nbsp; &nbsp;&nbsp;这些被保证了的「尊严」和「生活质量」也当然可以说是健康的条件。</p><p>●思考继续性，那么对另一个「动态的」包含着什么样的意义呢</p><p>&nbsp; &nbsp;&nbsp;「动态的」的原文是「eynamic」。日本也经常使用称为动态。WHO的动态的意思上，其中有一个就是「继续」。</p><p>&nbsp; &nbsp;&nbsp;这里即使进行健康诊断也没发现异常。尽管被判断为健康，但是由于不断重复吸烟、暴饮暴食的结果。某天会突然由于发生脑梗死病倒的人。</p><p>&nbsp; &nbsp;&nbsp;这个人在某一时期内确实是健康的，但是是否每日度过「健康的生活」那就不一样了。本来是健康的，但由于过着不健康的生活的结果，就会发病。</p><p>&nbsp; &nbsp;&nbsp;也就是「健康」和「不健康」，绝不是分别的东西，是在一条直线上并列存在的。这就是健康的身体突然得病的缘故。</p><p>&nbsp; &nbsp;&nbsp;重要的是维持健康，也就是将维持称为「继续」。具有健康的身心，并将这种状态能够持续，尽可能的每天每日注意预防，可以说是健康的条件。</p><p>&nbsp; &nbsp; 仅仅身体健康并不是「健康」。身体、心、社会上的人的尊严、生活质量、健康的继续，将这些满足的东西称为健康。从事介护工作的人，面对利用者可以说是包括这么多样意义的健康为目标是理想的。</p>',0,1,0,0,0,0,'202006/91797540882477056.html',141,1,'daoting','2020-06-10 15:28:40',NULL,'',NULL,0,0),(91800034912104448,'康复论','[[\"v0/60/55/91800378303967232.jpg\",\"IMG_20150518_124913\",\"1080 x 805 (.jpg)\",541402,\"daoting\",\"2020-06-10 15:41\"],[\"v0/61/62/91800491223019520.jpg\",\"6\",\"960 x 540 (.jpg)\",46138,\"daoting\",\"2020-06-10 15:42\"],[\"v0/B2/18/91800491357237248.jpg\",\"7\",\"540 x 960 (.jpg)\",34263,\"daoting\",\"2020-06-10 15:42\"],[\"v0/F4/5B/91800491449511936.jpg\",\"IMG_20150518_125051\",\"1080 x 802 (.jpg)\",472877,\"daoting\",\"2020-06-10 15:42\"],[\"v0/09/EE/91803792962351104.jpg\",\"2-5\",\"4000 x 3000 (.jpg)\",2190807,\"daoting\",\"2020-06-10 15:55\"],[\"v0/B8/94/91803835261906944.jpg\",\"CIMG5332\",\"4000 x 3000 (.jpg)\",2226785,\"daoting\",\"2020-06-10 15:55\"],[\"v0/65/1F/91803936436908032.jpg\",\"IMG_20160906_145630\",\"4160 x 2336 (.jpg)\",3020173,\"daoting\",\"2020-06-10 15:56\"],[\"v0/EA/34/91803998420332544.jpg\",\"IMG_20160906_145451\",\"4160 x 2336 (.jpg)\",2513845,\"daoting\",\"2020-06-10 15:56\"],[\"v0/21/92/91804086945312768.jpg\",\"IMG_20160818_150302\",\"4160 x 2336 (.jpg)\",2578809,\"daoting\",\"2020-06-10 15:56\"]]','','<p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:24px;font-family:宋体;background:lightgrey;\">健康养老服务专业系列教材之</span><span style=\"font-size:24px;background:lightgrey;\">&mdash;&mdash;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:29px;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:29px;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">康 复 论</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:19px;font-family:宋体;\">XX &nbsp;XX&nbsp;</span></strong><strong><span style=\"font-size:19px;font-family:宋体;\">编著</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:24px;font-family:宋体;\">长春百岁福祉教育科技有限公司</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;\">&nbsp;</span></p><p><span style=\'font-size:14px;font-family:\"Century\",\"serif\";\'><br>&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\'font-size:19px;font-family:\"MS Mincho\",\"serif\";\'>前</span><span style=\"font-size:19px;\">&nbsp;&nbsp;</span><span style=\'font-size:19px;font-family:\"MS Mincho\",\"serif\";\'>言</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;\">&nbsp;</span></p><p><span style=\'font-size:19px;font-family:\"Century\",\"serif\";\'><br>&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">目</span><span style=\"font-size:19px;\">&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">录</span></p><div style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><ul style=\"margin-bottom:0cm;list-style-type: undefined;margin-left:26px;\"><li style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">康复的基础知识</span></li></ul></div><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";text-indent:84.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">学习的目标</span></p><ul style=\"list-style-type: undefined;margin-left:62.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">何为康复</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复概论</span></li><li><span style=\"font-size:19px;font-family:宋体;\">康复的<span style=\"background:yellow;\">语源</span></span></li><li><span style=\"font-size:19px;font-family:宋体;\">康复的定义</span></li></ol><ul style=\"list-style-type: undefined;margin-left:62.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复的对象和目的</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复的对象</span></li><li><span style=\"font-size:19px;font-family:宋体;\">康复的目的和历史发展</span></li></ol><ul style=\"list-style-type: undefined;margin-left:62.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复的基本概念</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">自立生活运动</span></li><li><span style=\"font-size:19px;font-family:宋体;\">普通生活</span></li><li><span style=\"font-size:19px;font-family:宋体;\">国际障碍者年</span></li><li><span style=\"font-size:19px;font-family:宋体;\">障碍者计划</span><span lang=\"EN-US\" style=\"font-size:19px;\">&mdash;&mdash;</span><span style=\"font-size:19px;font-family:宋体;\">普通生活七年战略</span></li><li><span lang=\"EN-US\" style=\"font-size:19px;\">QOL</span><span style=\"font-size:19px;font-family:宋体;\">是什么</span></li><li><span lang=\"EN-US\" style=\"font-size:19px;\">ADL</span><span style=\"font-size:19px;font-family:宋体;\">是什么</span></li><li><span lang=\"EN-US\" style=\"font-size:19px;\">QOL</span><span style=\"font-size:19px;font-family:宋体;\">和</span><span lang=\"EN-US\" style=\"font-size:19px;\">ADL</span><span style=\"font-size:19px;font-family:宋体;\">的关系</span></li><li><span lang=\"EN-US\" style=\"font-size:19px;\">Empowerment</span></li><li><span style=\"font-size:19px;font-family:宋体;\">生活机能与障碍的国际分类</span></li></ol><ul style=\"list-style-type: undefined;margin-left:26px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍的类种与社会保障制度</span></li></ul><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";text-indent:84.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">学习的目标</span></p><ul style=\"list-style-type: undefined;margin-left:52.25px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍者</span></li></ul><ol style=\"list-style-type: undefined;margin-left:70.25px;\"><li><span style=\"font-size:19px;font-family:宋体;\">身体障碍者</span></li><li><span style=\"font-size:19px;font-family:宋体;\">精神障碍者</span></li><li><span style=\"font-size:19px;font-family:宋体;\">认知障碍者</span></li></ol><ul style=\"list-style-type: undefined;margin-left:52.25px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍者（儿童）与社会保障制度</span></li></ul><ol style=\"list-style-type: undefined;margin-left:70.25px;\"><li><span style=\"font-size:19px;font-family:宋体;\">身体障碍者福祉法</span></li><li><span style=\"font-size:19px;font-family:宋体;\">精神保健及精神障碍者福祉的法律</span></li><li><span style=\"font-size:19px;font-family:宋体;\">认知障碍者福祉法</span></li><li><span style=\"font-size:19px;font-family:宋体;\">儿童福祉法</span></li></ol><ul style=\"list-style-type: undefined;margin-left:26px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复分类与其过程</span></li></ul><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";text-indent:84.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">学习目标</span></p><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复的种类</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span style=\"font-size:19px;font-family:宋体;\">医学的康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">职业的康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">教育的康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">社会的康复</span></li></ol><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">地域康复与地域福祉</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span lang=\"EN-US\" style=\"font-size:19px;\">community based rehabilitation:CBR</span></li><li><span style=\"font-size:19px;font-family:宋体;\">地域康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">地域福祉</span></li></ol><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复的过程</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span style=\"font-size:19px;font-family:宋体;\">评价</span></li><li><span style=\"font-size:19px;font-family:宋体;\">康复与多职种团队</span></li><li><span style=\"font-size:19px;font-family:宋体;\">多职种团队与各专业</span></li></ol><ul style=\"list-style-type: undefined;margin-left:26px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍者、高龄者与康复</span></li></ul><p style=\'margin-top:0cm;margin-right:0cm;margin-bottom:.0001pt;margin-left:36.0pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";text-indent:49.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">学习的目标</span></p><ul style=\"list-style-type: undefined;margin-left:62px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍者（儿童）与康复</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80px;\"><li><span style=\"font-size:19px;font-family:宋体;\">身体障碍者与康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">精神障碍者与康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">认知障碍者与康复</span></li></ol><ul style=\"list-style-type: undefined;margin-left:62px;\"><li><span style=\"font-size:19px;font-family:宋体;\">高龄者与康复</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80px;\"><li><span style=\"font-size:19px;font-family:宋体;\">我国的平均寿命与高龄化</span></li><li><span style=\"font-size:19px;font-family:宋体;\">我国的高龄者比率</span></li><li><span style=\"font-size:19px;font-family:宋体;\">高龄者与身体障碍者</span></li><li><span style=\"font-size:19px;font-family:宋体;\">高龄者卧床不起的防止</span></li><li><span style=\"font-size:19px;font-family:宋体;\">痴呆高龄者</span></li><li><span style=\"font-size:19px;font-family:宋体;\">帕金森综合症</span></li><li><span style=\"font-size:19px;font-family:宋体;\">高龄者福祉服务</span></li></ol><ul style=\"list-style-type: undefined;margin-left:26px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复对象的理解与支援器械</span></li></ul><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";text-indent:84.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">学习的目标</span></p><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">从运动学的角度理解康复对象</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span style=\"font-size:19px;font-family:宋体;\">身体运动的基准点</span></li><li><span style=\"font-size:19px;font-family:宋体;background:yellow;\">体节及</span><span lang=\"EN-US\" style=\"font-size:19px;background:yellow;\">guanjie</span></li><li><span style=\"font-size:19px;font-family:宋体;\">身体的骨骼</span></li><li><span style=\"font-size:19px;font-family:宋体;\">关节运动</span></li><li><span style=\"font-size:19px;font-family:宋体;\">筋力（肌肉和周围的软组织的力量）</span></li><li><span style=\"font-size:19px;font-family:宋体;\">末梢神经和中枢神经</span></li></ol><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复支援器械</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span style=\"font-size:19px;font-family:宋体;\">福祉用具和假肢装具（辅装具）</span></li><li><span style=\"font-size:19px;font-family:宋体;\">假肢装具</span></li><li><span style=\"font-size:19px;font-family:宋体;\">福祉用具</span></li><li><span style=\"font-size:19px;font-family:宋体;\">介护保险与福祉用具</span></li><li><span style=\"font-size:19px;font-family:宋体;\">辅装具与日常生活用具</span></li></ol><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍的理解</span><span lang=\"EN-US\" style=\"font-size:19px;\">&mdash;&mdash;</span><span style=\"font-size:19px;font-family:宋体;\">示例</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span style=\"font-size:19px;font-family:宋体;\">脑血管障碍的康复与中枢神经</span></li><li><span style=\"font-size:19px;font-family:宋体;\">截肢者的康复与社会保障制度</span></li></ol><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">附件：参考文献</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: right;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">共</span><span style=\"font-size:19px;\">167</span><span style=\"font-size:19px;font-family:宋体;\">页</span></p><p style=\'margin-top:0cm;margin-right:0cm;margin-bottom:.0001pt;margin-left:57.0pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;\">&nbsp;</span></p>',0,1,0,0,0,0,'202006/91804406198956032.html',140,1,'daoting','2020-06-10 15:40:34',NULL,'',NULL,0,0),(92497574754119680,'维持提高QOL，维持提高心身健康','[[\"v0/00/F7/92529784261570560.png\",\"u354\",\"553 x 291 (.png)\",296599,\"daoting\",\"2020-06-12 16:00\"]]','','<p>养老服务核心目标</p><p>维持、提高QOL；维持、提高心身健康。</p>',1,1,0,0,0,0,'202006/92498226599292928.html',148,1,'daoting','2020-06-12 13:52:22',NULL,'',NULL,0,0);
/*!40000 ALTER TABLE `pub_post` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pub_postcat`
--

DROP TABLE IF EXISTS `pub_postcat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pub_postcat` (
  `PostID` bigint(20) NOT NULL COMMENT '文章标识',
  `CatID` bigint(20) NOT NULL COMMENT '分类标识',
  PRIMARY KEY (`PostID`,`CatID`) USING BTREE,
  KEY `fk_postcat_catid` (`CatID`),
  KEY `fk_postcat_postid` (`PostID`) USING BTREE,
  CONSTRAINT `fk_postcat_catid` FOREIGN KEY (`CatID`) REFERENCES `pub_category` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_postcat_postid` FOREIGN KEY (`PostID`) REFERENCES `pub_post` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pub_postcat`
--

LOCK TABLES `pub_postcat` WRITE;
/*!40000 ALTER TABLE `pub_postcat` DISABLE KEYS */;
/*!40000 ALTER TABLE `pub_postcat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pub_sql`
--

DROP TABLE IF EXISTS `pub_sql`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pub_sql` (
  `id` varchar(128) NOT NULL COMMENT 'sql键值',
  `sql` varchar(20000) NOT NULL COMMENT 'sql内容',
  `note` varchar(255) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pub_sql`
--

LOCK TABLES `pub_sql` WRITE;
/*!40000 ALTER TABLE `pub_sql` DISABLE KEYS */;
INSERT INTO `pub_sql` VALUES ('文章-模糊查询','select\r\n	ID,\r\n	Title,\r\n	Cover,\r\n	IsPublish,\r\n	Dispidx,\r\n	Creator,\r\n	Ctime,\r\n	ReadCount,\r\n	CommentCount \r\nfrom\r\n	pub_post \r\norder by\r\n	Dispidx desc \r\nwhere\r\n	Title like @input',NULL),('文章-管理列表','select\r\n	ID,\r\n	Title,\r\n	Cover,\r\n	Summary,\r\n	IsPublish,\r\n	Dispidx,\r\n	Creator,\r\n	Ctime,\r\n	ReadCount,\r\n	CommentCount \r\nfrom\r\n	pub_post \r\norder by\r\n	Dispidx desc',NULL),('文章-编辑','select\r\n	* \r\nfrom\r\n	pub_post \r\nwhere\r\n	id = @id',NULL);
/*!40000 ALTER TABLE `pub_sql` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sequence`
--

DROP TABLE IF EXISTS `sequence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sequence` (
  `id` varchar(64) NOT NULL COMMENT '序列名称',
  `val` int(11) NOT NULL COMMENT '序列的当前值',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='模拟Sequence';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sequence`
--

LOCK TABLES `sequence` WRITE;
/*!40000 ALTER TABLE `sequence` DISABLE KEYS */;
INSERT INTO `sequence` VALUES ('sq_menu',75),('sq_post',148),('sq_res',1022);
/*!40000 ALTER TABLE `sequence` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-09-30 12:20:56

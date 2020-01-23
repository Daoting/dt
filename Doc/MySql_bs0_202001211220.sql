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
INSERT INTO `cm_menu` VALUES (1,NULL,'工作台',1,'','','搬运工','','',1,0,'2019-03-07 10:45:44','2019-03-07 10:45:43'),(2,1,'用户账号',0,'用户账号','','钥匙','','',2,0,'2019-11-08 11:42:28','2019-11-08 11:43:53'),(3,1,'菜单管理',0,'菜单管理','','大图标','','',3,0,'2019-03-11 11:35:59','2019-03-11 11:35:58'),(4,1,'系统角色',0,'系统角色','','两人','','',4,0,'2019-11-08 11:47:21','2019-11-08 11:48:22'),(5,1,'基础权限',0,'基础权限','','审核','','',5,0,'2019-03-12 09:11:22','2019-03-07 11:23:40'),(6,1,'参数定义',0,'参数定义','','调色板','','',6,0,'2019-03-12 15:35:56','2019-03-12 15:37:10'),(7,1,'基础代码',0,'基础代码','','文件','','',7,0,'2019-11-08 11:49:40','2019-11-08 11:49:46'),(8,1,'控件样例',0,'控件样例','','词典','','客户端常用控件的使用样例，按照容器、编辑器、数据访问、样式资源等分类给出。',8,0,'2019-03-07 11:06:57','2019-03-07 11:06:57'),(15268145234386944,NULL,'新菜单组22',1,'','','文件夹','','',25,0,'2019-11-12 11:10:10','2019-11-12 11:10:13'),(15315637929975808,18562741636898816,'新菜单12',0,'','','文件','','',48,0,'2019-11-12 14:18:53','2019-11-12 14:31:38'),(15315938808373248,NULL,'新菜单组额',1,'','','文件夹','','',51,0,'2019-11-12 14:20:04','2019-11-12 14:20:14'),(18562741636898816,15315938808373248,'新组t',1,'','','文件夹','','',63,0,'2019-11-21 13:21:43','2019-11-21 13:21:43'),(18860286065975296,NULL,'新菜单a',0,'','','文件','','',67,0,'2019-11-22 09:04:04','2019-11-22 09:04:04');
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
INSERT INTO `cm_rolemenu` VALUES (1,2),(1,3),(1,4),(1,5),(1,6),(1,7),(1,8),(1,15315637929975808),(2,18860286065975296);
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
INSERT INTO `cm_sql` VALUES ('权限-关联用户','select distinct (c.name)\r\n  from cm_roleprv a, cm_userrole b, cm_user c\r\n where a.roleid = b.roleid\r\n   and b.userid = c.id\r\n   and a.prvid = @prvid\r\n order by c.name',NULL),('权限-关联角色','select id as roleid, b.name as rolename, a.prvid\r\n  from cm_roleprv a\r\n  left join cm_role b\r\n    on a.roleid = b.id\r\n where a.prvid = @prvid',NULL),('权限-名称重复','select count(id) from cm_prv where id=@id',NULL),('权限-所有','select * from cm_prv',NULL),('权限-未关联的角色','select id, name, note\r\n  from cm_role a\r\n where not exists (select roleid\r\n          from cm_roleprv b\r\n         where a.id = b.roleid\r\n           and prvid = @prvid)',NULL),('权限-模糊查询','select * from cm_prv where id like @id',NULL),('权限-系统权限','select * from cm_prv where id < 1000',NULL),('用户-关联角色','SELECT\r\n	b.id roleid,\r\n	b.NAME rolename,\r\n	a.userid \r\nFROM\r\n	cm_userrole a,\r\n	cm_role b \r\nWHERE\r\n	a.roleid = b.id \r\n	AND userid = @userid',NULL),('用户-具有的权限','SELECT\r\n	prvid \r\nFROM\r\n	(\r\nSELECT DISTINCT\r\n	( a.prvid ) \r\nFROM\r\n	cm_roleprv a\r\n	LEFT JOIN cm_prv b ON a.prvid = b.id \r\nWHERE\r\n	EXISTS ( SELECT roleid FROM cm_userrole c WHERE a.roleid = c.roleid AND userid = @userid ) \r\n	) t',NULL),('用户-删除用户角色','delete from cm_userrole where userid=@userid and roleid=@roleid',NULL),('用户-可访问的菜单','select name\r\n  from (select distinct (b.id), b.name, dispidx\r\n          from cm_rolemenu a\r\n          left join cm_menu b\r\n            on a.menuid = b.id\r\n         where exists\r\n         (select roleid\r\n                  from cm_userrole c\r\n                 where a.roleid = c.roleid\r\n                   and userid = @userid) or a.roleid=1) t\r\n order by dispidx',NULL),('用户-增加用户角色','insert into cm_userrole (userid, roleid) values (@userid, @roleid)',NULL),('用户-所有','SELECT\r\n	id,\r\n	phone,\r\n	name,\r\n	( CASE sex WHEN 1 THEN \'男\' ELSE \'女\' END ) sex,\r\n	expired,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user',NULL),('用户-最近修改','SELECT\r\n	id,\r\n	phone,\r\n	NAME,\r\n	( CASE sex WHEN 1 THEN \'男\' ELSE \'女\' END ) sex,\r\n	expired,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user \r\nWHERE\r\n	to_days(now()) - to_days(mtime) <= 2',NULL),('用户-未关联的角色','SELECT\r\n	a.id,\r\n	a.NAME\r\nFROM\r\n	cm_role a\r\nWHERE\r\n	NOT EXISTS ( SELECT roleid FROM cm_userrole b WHERE a.id = b.roleid AND userid = @userid )\r\n	AND a.id<>1',NULL),('用户-模糊查询','SELECT\r\n	id,\r\n	phone,\r\n	NAME,\r\n	( CASE sex WHEN 1 THEN \'男\' ELSE \'女\' END ) sex,\r\n	expired,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user \r\nWHERE\r\n	NAME LIKE @input \r\n	OR phone LIKE @input',NULL),('用户-编辑','SELECT\r\n	id,\r\n	phone,\r\n	name,\r\n	sex,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user \r\nWHERE\r\n	id = @id',NULL),('用户-重复手机号','select count(id) from cm_user where phone=@phone',NULL),('登录-手机号获取用户','select * from cm_user where phone=@phone',NULL),('菜单-id菜单项','SELECT\r\n	a.*,\r\n	b.NAME parentname \r\nFROM\r\n	cm_menu a\r\n	LEFT JOIN cm_menu b ON a.parentid = b.id \r\nWHERE\r\n	a.id = @id',NULL),('菜单-关联的角色','SELECT\r\n	b.id roleid,\r\n	b.NAME rolename,\r\n	a.menuid \r\nFROM\r\n	cm_rolemenu a,\r\n	cm_role b \r\nWHERE\r\n	a.roleid = b.id \r\n	AND menuid = @menuid',NULL),('菜单-分组树','SELECT\r\n	id,\r\n	NAME,\r\n	parentid \r\nFROM\r\n	cm_menu \r\nWHERE\r\n	isgroup = 1 \r\nORDER BY\r\n	dispidx',NULL),('菜单-完整树','SELECT\r\n	id,\r\n	NAME,\r\n	parentid,\r\n	isgroup,\r\n	icon,\r\n	dispidx\r\nFROM\r\n	cm_menu \r\nORDER BY\r\n	dispidx',NULL),('菜单-是否有子菜单','select count(*) from cm_menu where parentid=@parentid',NULL),('菜单-未关联的角色','SELECT\r\n	a.id,\r\n	a.NAME\r\nFROM\r\n	cm_role a\r\nWHERE\r\n	NOT EXISTS ( SELECT roleid FROM cm_rolemenu b WHERE a.id = b.roleid AND menuid = @menuid )',NULL),('角色-关联用户','SELECT\r\n	b.id userid,\r\n	b.NAME username,\r\n	a.roleid \r\nFROM\r\n	cm_userrole a,\r\n	cm_user b \r\nWHERE\r\n	a.userid = b.id\r\n	AND roleid = @roleid',NULL),('角色-关联的权限','select a.prvid, a.roleid\r\n  from cm_roleprv a\r\n  join cm_prv b\r\n    on a.prvid = b.id\r\n where a.roleid = @roleid',NULL),('角色-关联的菜单','select id as menuid, name, a.roleid\r\n  from cm_rolemenu a\r\n  join cm_menu b\r\n    on a.menuid = b.id\r\n where b.isgroup = 0\r\n   and a.roleid = @roleid\r\n order by dispidx',NULL),('角色-名称重复','select count(id) from cm_role where name=@name',NULL),('角色-所有','select * from cm_role',NULL),('角色-未关联的权限','select a.id, a.note\r\n  from cm_prv a\r\n where not exists\r\n (select prvid\r\n          from cm_roleprv b\r\n         where a.id = b.prvid\r\n           and b.roleid = @roleid)',NULL),('角色-未关联的用户','select id, name\r\n  from cm_user a\r\n where not exists (select userid\r\n          from cm_userrole b\r\n         where a.id = b.userid\r\n           and roleid = @roleid)\r\n order by name',NULL),('角色-未关联的菜单','select id, name\r\n  from cm_menu a\r\n where isgroup = 0\r\n   and not exists (select menuid\r\n          from cm_rolemenu b\r\n         where a.id = b.menuid\r\n           and roleid = @roleid)\r\n order by dispidx',NULL),('角色-模糊查询','select * from cm_role where name like @name',NULL),('角色-系统角色','select * from cm_role where id < 1000',NULL),('角色-编辑','SELECT\r\n	id,\r\n	name,\r\n	note\r\nFROM\r\n	cm_role\r\nWHERE\r\n	id = @id',NULL);
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
INSERT INTO `cm_user` VALUES (1,'15948371897','daoting','af3303f852abeccd793068486a391626',1,0,'2019-10-24 09:06:38','2019-10-24 09:06:41'),(8411237852585984,'15911111111','15911111113','b59c67bf196a4758191e42f76670ceba',0,0,'2019-10-24 13:03:19','2019-12-03 11:22:00');
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
  `UserType` tinyint(4) NOT NULL COMMENT '上传人类型',
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
INSERT INTO `fsm_file` VALUES (58806081590337536,'1.png','v0/FC/63/58806081590337536.png',47916,'1101 x 428 (.png)',0,3,'2019-10-10 14:34:21',0),(58806081670029312,'2.png','v0/4D/1A/58806081670029312.png',37213,'891 x 422 (.png)',0,3,'2019-10-10 14:34:21',0),(58806799604850688,'icon.png','v0/94/95/58806799604850688.png',595,'72 x 72 (.png)',0,3,'2019-10-10 14:37:12',0),(58808696365588480,'icon.png','v0/94/95/58808696365588480.png',595,'72 x 72 (.png)',0,3,'2019-10-10 14:44:44',0),(58817739587002368,'IMG_20190929_012732.jpg','v0/8D/6B/58817739587002368.jpg',69941,'720 x 1280 (jpg)',110,3,'2019-10-10 15:20:40',0),(58818326697287680,'IMG_20190929_012732.jpg','v0/8D/6B/58818326697287680.jpg',69941,'720 x 1280 (jpg)',110,3,'2019-10-10 15:23:00',0),(58829572431618048,'IMG_20191010_080620.jpg','v0/3A/3B/58829572431618048.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-10 16:07:42',0),(58830350181408768,'IMG_20191010_080620.jpg','v0/3A/3B/58830350181408768.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-10 16:10:47',0),(58831556010254336,'IMG_20191010_080620.jpg','v0/3A/3B/58831556010254336.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-10 16:15:34',0),(58834189722787840,'IMG_20191010_080620.jpg','v0/3A/3B/58834189722787840.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-10 16:26:02',0),(58837598811136000,'IMG_20191010_080620.jpg','v0/3A/3B/58837598811136000.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-10 16:39:35',0),(58838876534546432,'IMG_20191010_080620.jpg','v0/3A/3B/58838876534546432.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-10 16:44:40',0),(58842403029852160,'IMG_20191010_080620.jpg','v0/3A/3B/58842403029852160.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-10 16:58:42',0),(59068911744917504,'1.png','v0/FC/63/59068911744917504.png',47916,'1101 x 428 (.png)',0,3,'2019-10-11 07:58:46',0),(59069355028324352,'IMG_20191010_080620.jpg','v0/3A/3B/59069355028324352.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 08:00:32',0),(59070589504266240,'IMG_20191010_080617.jpg','v0/FE/EB/59070589504266240.jpg',71775,'720 x 1280 (jpg)',110,3,'2019-10-11 08:05:27',0),(59070590166966272,'IMG_20191010_080620.jpg','v0/3A/3B/59070590166966272.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 08:05:27',0),(59071030845710336,'IMG_20191010_080620.jpg','v0/3A/3B/59071030845710336.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 08:07:12',0),(59071464176033792,'IMG_20191010_080617.jpg','v0/FE/EB/59071464176033792.jpg',71775,'720 x 1280 (jpg)',110,3,'2019-10-11 08:08:55',0),(59071465128140800,'IMG_20191010_080620.jpg','v0/3A/3B/59071465128140800.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 08:08:55',0),(59071578588258304,'IMG_20191010_080617.jpg','v0/FE/EB/59071578588258304.jpg',71775,'720 x 1280 (jpg)',110,3,'2019-10-11 08:09:22',0),(59071579607474176,'IMG_20191010_080620.jpg','v0/3A/3B/59071579607474176.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 08:09:23',0),(59071623408590848,'IMG_20191010_080617.jpg','v0/FE/EB/59071623408590848.jpg',71775,'720 x 1280 (jpg)',110,3,'2019-10-11 08:09:33',0),(59071624624939008,'IMG_20191010_080620.jpg','v0/3A/3B/59071624624939008.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 08:09:33',0),(59076939344977920,'VID_20191010_080630.mp4','v0/5B/14/59076939344977920.mp4',3555116,'00:00:09 (720 x 1280)',110,3,'2019-10-11 08:30:42',0),(59089667258261504,'VID_20191010_080630.mp4','v0/5B/14/59089667258261504.mp4',3555116,'00:00:09 (720 x 1280)',110,3,'2019-10-11 09:21:16',0),(59090223058067456,'VID_20191010_080630.mp4','v0/5B/14/59090223058067456.mp4',3555116,'00:00:09 (720 x 1280)',110,3,'2019-10-11 09:23:29',0),(59091590451503104,'VID_20191010_080630.mp4','v0/5B/14/59091590451503104.mp4',3555116,'00:00:09 (1280 x 720)',110,3,'2019-10-11 09:28:55',0),(59092208691912704,'VID_20191010_080630.mp4','v0/5B/14/59092208691912704.mp4',3555116,'00:00:09 (1280 x 720)',110,3,'2019-10-11 09:31:23',0),(59092753406173184,'IMG_20191010_080617.jpg','v0/FE/EB/59092753406173184.jpg',71775,'720 x 1280 (jpg)',110,3,'2019-10-11 09:33:31',0),(59092753993375744,'IMG_20191010_080620.jpg','v0/3A/3B/59092753993375744.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 09:33:31',0),(59092785349992448,'IMG_20191010_080620.jpg','v0/3A/3B/59092785349992448.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 09:33:38',0),(59092826349314048,'IMG_20191010_080617.jpg','v0/FE/EB/59092826349314048.jpg',71775,'720 x 1280 (jpg)',110,3,'2019-10-11 09:33:48',0),(59092827032985600,'IMG_20191010_080620.jpg','v0/3A/3B/59092827032985600.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 09:33:48',0),(59093426071871488,'1.png','v0/FC/63/59093426071871488.png',47916,'1101 x 428 (.png)',0,3,'2019-10-11 09:36:11',0),(59093426155757568,'2.png','v0/4D/1A/59093426155757568.png',37213,'891 x 422 (.png)',0,3,'2019-10-11 09:36:11',0),(59093426210283520,'icon.png','v0/94/95/59093426210283520.png',595,'72 x 72 (.png)',0,3,'2019-10-11 09:36:11',0),(59093512138989568,'ImageStabilization.wmv','v0/EA/34/59093512138989568.wmv',403671,'00:00:06 (480 x 288)',0,3,'2019-10-11 09:36:31',0),(59096452685832192,'ImageStabilization.wmv','v0/EA/34/59096452685832192.wmv',403671,'00:00:06 (480 x 288)',0,3,'2019-10-11 09:48:12',0),(59098691743719424,'VID_20191010_080630.mp4','v0/5B/14/59098691743719424.mp4',3555116,'00:00:09 (1280 x 720)',110,3,'2019-10-11 09:57:08',0),(59098789659746304,'IMG_20191010_080620.jpg','v0/3A/3B/59098789659746304.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 09:57:30',0),(59098836594008064,'IMG_20191010_080617.jpg','v0/FE/EB/59098836594008064.jpg',71775,'720 x 1280 (jpg)',110,3,'2019-10-11 09:57:41',0),(59098837390925824,'IMG_20191010_080620.jpg','v0/3A/3B/59098837390925824.jpg',67692,'720 x 1280 (jpg)',110,3,'2019-10-11 09:57:41',0),(59098908887031808,'VID_20191010_080630.mp4','v0/5B/14/59098908887031808.mp4',3555116,'00:00:09 (1280 x 720)',110,3,'2019-10-11 09:57:59',0),(59100497186070528,'2.png','v0/4D/1A/59100497186070528.png',37213,'891 x 422 (.png)',0,3,'2019-10-11 10:04:17',0),(59100539833753600,'icon.png','v0/94/95/59100539833753600.png',595,'72 x 72 (.png)',0,3,'2019-10-11 10:04:27',0),(59100656250855424,'ImageStabilization.wmv','v0/EA/34/59100656250855424.wmv',403671,'00:00:06 (480 x 288)',0,3,'2019-10-11 10:04:55',0),(59100846353489920,'Windows Logon.wav','v0/88/4F/59100846353489920.wav',384496,'00:04',0,3,'2019-10-11 10:05:40',0),(59102736164265984,'IMG_0003.JPG','v0/3A/6B/59102736164265984.jpg',2505426,'3000 x 2002 (jpg)',0,3,'2019-10-11 10:13:12',0),(59102743898562560,'IMG_0002.JPG','v0/E9/B4/59102743898562560.jpg',2604768,'4288 x 2848 (jpg)',0,3,'2019-10-11 10:13:15',0),(59102753616764928,'IMG_0005.JPG','v0/DC/D7/59102753616764928.jpg',1852262,'3000 x 2002 (jpg)',0,3,'2019-10-11 10:13:16',0),(59102761082626048,'IMG_0001.JPG','v0/98/FE/59102761082626048.jpg',1896240,'4288 x 2848 (jpg)',0,3,'2019-10-11 10:13:18',0),(59102767571214336,'IMG_0004.JPG','v0/8B/21/59102767571214336.jpg',1268382,'1668 x 2500 (jpg)',0,3,'2019-10-11 10:13:19',0);
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
INSERT INTO `fsm_sql` VALUES ('上传文件','INSERT INTO fsm_file ( id, NAME, path, size, uploader, usertype, info, ctime, downloads )\r\nVALUES\r\n	( @id, @NAME, @path, @size, @uploader, @usertype, @info, now( ), 0 )',NULL),('增加下载次数','update fsm_file set downloads=downloads+1 where path=@path',NULL);
/*!40000 ALTER TABLE `fsm_sql` ENABLE KEYS */;
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
INSERT INTO `sequence` VALUES ('sq_menu',67),('sq_res',1022);
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

-- Dump completed on 2020-01-21 12:20:55

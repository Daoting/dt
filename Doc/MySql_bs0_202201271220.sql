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
  `SvcName` varchar(32) NOT NULL COMMENT '提供提示信息的服务Api名称，格式如：cm:UserRelated.GetMenuTip，空表示无提示信息',
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
INSERT INTO `cm_menu` VALUES (1,NULL,'工作台',1,'','','搬运工','','',1,0,'2019-03-07 10:45:44','2019-03-07 10:45:43'),(2,1,'用户账号',0,'用户账号','','钥匙','','',2,0,'2019-11-08 11:42:28','2019-11-08 11:43:53'),(3,1,'菜单管理',0,'菜单管理','','大图标','','',3,0,'2019-03-11 11:35:59','2019-03-11 11:35:58'),(4,1,'系统角色',0,'系统角色','','两人','','',4,0,'2019-11-08 11:47:21','2019-11-08 11:48:22'),(5,1,'基础权限',0,'基础权限','','审核','','',5,0,'2019-03-12 09:11:22','2019-03-07 11:23:40'),(6,1,'参数定义',0,'参数定义','','调色板','','',6,0,'2019-03-12 15:35:56','2019-03-12 15:37:10'),(7,1,'基础选项',0,'基础选项','','文件','','',7,0,'2019-11-08 11:49:40','2019-11-08 11:49:46'),(15268145234386944,15315938808373248,'新菜单组22',1,'','','文件夹','','',25,0,'2019-11-12 11:10:10','2019-11-12 11:10:13'),(15315637929975808,18562741636898816,'新菜单12',0,'','','文件','','',48,0,'2019-11-12 14:18:53','2019-11-12 14:31:38'),(15315938808373248,NULL,'新菜单组额',1,'','','文件夹','','',67,0,'2019-11-12 14:20:04','2019-11-12 14:20:14'),(18562741636898816,15315938808373248,'新组t',1,'','','文件夹','','',63,0,'2019-11-21 13:21:43','2019-11-21 13:21:43'),(18860286065975296,NULL,'新菜单a123',0,'报表','新报表111,abc1','文件','','',68,0,'2019-11-22 09:04:04','2019-11-22 09:04:04'),(139207663032332288,1,'报表设计',0,'报表设计','','折线图','','',8,0,'2020-10-19 11:21:38','2020-10-19 11:21:38'),(142179186714210304,1,'发布管理',0,'发布管理','','书籍','','',79,0,'2020-10-27 16:09:27','2020-10-27 16:09:27'),(144356540693737472,1,'流程设计',0,'流程设计','','双绞线','','',76,0,'2020-11-02 16:21:19','2020-11-02 16:21:19'),(154430055023640576,NULL,'新菜单xxx',0,'报表','','文件','','',83,0,'2020-11-30 11:29:56','2020-11-30 11:29:56'),(154784874422861824,NULL,'abcz',0,'','','文件','','',84,0,'2020-12-01 10:59:50','2020-12-01 10:59:50'),(259520016549801984,NULL,'新组bcd',1,'','','文件夹','','',85,0,'2021-09-16 11:19:54','2021-09-16 11:19:54');
/*!40000 ALTER TABLE `cm_menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_myfile`
--

DROP TABLE IF EXISTS `cm_myfile`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_myfile` (
  `ID` bigint(20) NOT NULL COMMENT '文件标识',
  `ParentID` bigint(20) DEFAULT NULL COMMENT '上级目录，根目录的parendid为空',
  `Name` varchar(255) NOT NULL COMMENT '名称',
  `IsFolder` tinyint(1) NOT NULL COMMENT '是否为文件夹',
  `ExtName` varchar(8) DEFAULT NULL COMMENT '文件扩展名',
  `Info` varchar(512) NOT NULL COMMENT '文件描述信息',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `UserID` bigint(20) NOT NULL COMMENT '所属用户',
  PRIMARY KEY (`ID`),
  KEY `fk_myfile_parentid` (`ParentID`),
  KEY `fk_user_userid` (`UserID`),
  CONSTRAINT `fk_myfile_parentid` FOREIGN KEY (`ParentID`) REFERENCES `cm_myfile` (`ID`),
  CONSTRAINT `fk_user_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='个人文件';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_myfile`
--

LOCK TABLES `cm_myfile` WRITE;
/*!40000 ALTER TABLE `cm_myfile` DISABLE KEYS */;
INSERT INTO `cm_myfile` VALUES (140724076930789376,NULL,'新目录1',1,NULL,'','2020-10-23 15:47:16',1),(140724154458304512,140724076930789376,'b',1,NULL,'','2020-10-23 15:47:34',1),(140724322477928448,NULL,'12',0,NULL,'[[\"v0/52/37/140724323039965184.xlsx\",\"12\",\"xlsx文件\",8153,\"daoting\",\"2020-10-23 15:48\"]]','2020-10-23 15:48:14',1),(141735914371936256,NULL,'新目录12',1,NULL,'','2020-10-26 10:48:01',8411237852585984);
/*!40000 ALTER TABLE `cm_myfile` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_option`
--

DROP TABLE IF EXISTS `cm_option`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_option` (
  `Name` varchar(64) NOT NULL COMMENT '选项名称',
  `Category` varchar(64) NOT NULL COMMENT '所属分类',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  PRIMARY KEY (`Name`,`Category`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='基础选项';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_option`
--

LOCK TABLES `cm_option` WRITE;
/*!40000 ALTER TABLE `cm_option` DISABLE KEYS */;
INSERT INTO `cm_option` VALUES ('bool','数据类型',816),('Date','数据类型',350),('DateTime','数据类型',349),('double','数据类型',348),('int','数据类型',347),('string','数据类型',346),('不明','性别',345),('东乡族','民族',27),('中学','学历',60),('乌孜别克族','民族',44),('京族','民族',50),('仡佬族','民族',38),('仫佬族','民族',33),('佤族','民族',22),('侗族','民族',13),('俄罗斯族','民族',45),('保安族','民族',48),('傈僳族','民族',21),('傣族','民族',19),('其他','学历',64),('内蒙古东乌珠穆沁旗','地区',185),('内蒙古东胜市','地区',207),('内蒙古丰镇县','地区',200),('内蒙古临河市','地区',215),('内蒙古乌兰浩特市','地区',167),('内蒙古乌审旗','地区',213),('内蒙古乌拉特中旗','地区',219),('内蒙古乌拉特前旗','地区',218),('内蒙古乌拉特后旗','地区',220),('内蒙古乌海市','地区',143),('内蒙古二连浩特市','地区',180),('内蒙古五原县','地区',216),('内蒙古伊金霍洛旗','地区',214),('内蒙古克什克腾旗','地区',149),('内蒙古兴和县','地区',199),('内蒙古准格尔旗','地区',209),('内蒙古凉城县','地区',201),('内蒙古包头市','地区',140),('内蒙古化德县','地区',197),('内蒙古卓资县','地区',196),('内蒙古呼和浩特市','地区',137),('内蒙古和林格尔县','地区',194),('内蒙古商都县','地区',198),('内蒙古喀喇沁旗','地区',151),('内蒙古四子王旗','地区',206),('内蒙古固阳县','地区',142),('内蒙古土默特右旗','地区',141),('内蒙古土默特左旗','地区',138),('内蒙古多伦县','地区',191),('内蒙古太仆寺旗','地区',187),('内蒙古奈曼旗','地区',178),('内蒙古宁城县','地区',152),('内蒙古察哈尔右翼中旗','地区',203),('内蒙古察哈尔右翼前旗','地区',202),('内蒙古察哈尔右翼后旗','地区',204),('内蒙古巴林右旗','地区',147),('内蒙古巴林左旗','地区',146),('内蒙古库伦旗','地区',177),('内蒙古开鲁县','地区',176),('内蒙古扎兰屯市','地区',156),('内蒙古扎赉特旗','地区',170),('内蒙古扎鲁特旗','地区',179),('内蒙古托克托县','地区',139),('内蒙古敖汉旗','地区',153),('内蒙古新巴尔虎右旗','地区',164),('内蒙古新巴尔虎左旗','地区',165),('内蒙古杭锦后旗','地区',221),('内蒙古杭锦旗','地区',212),('内蒙古林西县','地区',148),('内蒙古正蓝旗','地区',190),('内蒙古正镶白旗','地区',189),('内蒙古武川县','地区',193),('内蒙古海拉尔市','地区',154),('内蒙古清水河县','地区',195),('内蒙古满州里市','地区',155),('内蒙古牙克石市','地区',157),('内蒙古磴口县','地区',217),('内蒙古科尔沁右翼中旗','地区',169),('内蒙古科尔沁右翼前旗','地区',168),('内蒙古科尔沁左翼中旗','地区',174),('内蒙古科尔沁左翼后旗','地区',175),('内蒙古突泉县','地区',171),('内蒙古翁牛特旗','地区',150),('内蒙古苏尼特右旗','地区',184),('内蒙古苏尼特左旗','地区',183),('内蒙古莫力县','地区',159),('内蒙古西乌珠穆沁旗','地区',186),('内蒙古赤峰市','地区',144),('内蒙古达尔罕茂明安联','地区',205),('内蒙古达拉特旗','地区',208),('内蒙古通辽市','地区',172),('内蒙古鄂伦春自治旗','地区',162),('内蒙古鄂托克前旗','地区',210),('内蒙古鄂托克旗','地区',211),('内蒙古鄂温克族自治旗','地区',163),('内蒙古锡林浩特市','地区',181),('内蒙古镶黄旗','地区',188),('内蒙古阿巴嘎旗','地区',182),('内蒙古阿拉善右旗','地区',223),('内蒙古阿拉善左旗','地区',222),('内蒙古阿荣旗','地区',158),('内蒙古阿鲁科尔沁旗','地区',145),('内蒙古陈巴尔虎旗','地区',166),('内蒙古集宁市','地区',192),('内蒙古霍林郭勒市','地区',173),('内蒙古额尔古纳右旗','地区',160),('内蒙古额尔古纳左旗','地区',161),('内蒙古额济纳旗','地区',224),('博士','学历',63),('吉林省','地区',285),('吉林省东丰县','地区',300),('吉林省东辽县','地区',301),('吉林省九台市','地区',332),('吉林省乾安县','地区',319),('吉林省伊通县','地区',297),('吉林省公主岭市','地区',328),('吉林省农安县','地区',287),('吉林省前郭尔罗斯县','地区',316),('吉林省双辽县','地区',298),('吉林省双阳县','地区',289),('吉林省吉林市','地区',290),('吉林省和龙县','地区',325),('吉林省四平市','地区',295),('吉林省图们市','地区',321),('吉林省大安市','地区',314),('吉林省安图县','地区',327),('吉林省延吉市','地区',320),('吉林省德惠县','地区',288),('吉林省扶余市','地区',313),('吉林省抚松县','地区',307),('吉林省敦化市','地区',322),('吉林省柳河县','地区',305),('吉林省桦甸市','地区',331),('吉林省梅河口市','地区',329),('吉林省梨树县','地区',296),('吉林省榆树县','地区',286),('吉林省永吉县','地区',291),('吉林省汪清县','地区',326),('吉林省洮南市','地区',312),('吉林省浑江市','地区',306),('吉林省珲春市','地区',323),('吉林省白城地区','地区',310),('吉林省白城市','地区',311),('吉林省磐石县','地区',293),('吉林省舒兰县','地区',292),('吉林省蛟河县','地区',294),('吉林省辉南县','地区',304),('吉林省辽源市','地区',299),('吉林省通化县','地区',303),('吉林省通化市','地区',302),('吉林省通榆县','地区',318),('吉林省镇赉县','地区',317),('吉林省长岭县','地区',315),('吉林省长春市','地区',136),('吉林省长白县','地区',309),('吉林省集安市','地区',330),('吉林省靖宇县','地区',308),('吉林省龙井市','地区',324),('哈尼族','民族',17),('哈萨克族','民族',18),('回族','民族',4),('土家族','民族',16),('土族','民族',31),('基诺族','民族',57),('塔吉克族','民族',42),('塔塔尔族','民族',51),('壮族','民族',9),('大学','学历',58),('女','性别',343),('小学','学历',61),('布依族','民族',10),('布朗族','民族',35),('彝族','民族',8),('德昂族','民族',47),('怒族','民族',43),('拉祜族','民族',25),('撒拉族','民族',36),('普米族','民族',41),('景颇族','民族',29),('朝鲜族','民族',11),('未知','性别',344),('柯尔克孜族','民族',30),('毛难族','民族',37),('水族','民族',26),('汉族','民族',2),('满族','民族',12),('独龙族','民族',52),('珞巴族','民族',56),('瑶族','民族',14),('男','性别',342),('畲族','民族',23),('白族','民族',15),('硕士','学历',62),('纳西族','民族',28),('维吾尔族','民族',6),('羌族','民族',34),('苗族','民族',7),('蒙古族','民族',3),('藏族','民族',5),('裕固族','民族',49),('赫哲族','民族',54),('辽宁省','地区',225),('辽宁省东沟县','地区',245),('辽宁省丹东市','地区',242),('辽宁省义  县','地区',252),('辽宁省兴城市','地区',281),('辽宁省凌源县','地区',274),('辽宁省凤城县','地区',243),('辽宁省北票市','地区',283),('辽宁省北镇县','地区',250),('辽宁省台安县','地区',234),('辽宁省喀喇沁县','地区',275),('辽宁省大洼县','地区',263),('辽宁省大连市','地区',229),('辽宁省宽甸县','地区',246),('辽宁省岫岩县','地区',244),('辽宁省庄河县','地区',232),('辽宁省康平县','地区',269),('辽宁省建平县','地区',273),('辽宁省建昌县','地区',276),('辽宁省开原市','地区',284),('辽宁省彰武县','地区',258),('辽宁省抚顺县','地区',236),('辽宁省抚顺市','地区',235),('辽宁省新宾县','地区',237),('辽宁省新民县','地区',227),('辽宁省新金县','地区',230),('辽宁省昌图县','地区',268),('辽宁省朝阳县','地区',272),('辽宁省朝阳市','地区',271),('辽宁省本溪县','地区',240),('辽宁省本溪市','地区',239),('辽宁省桓仁县','地区',241),('辽宁省沈阳市','地区',226),('辽宁省法库县','地区',270),('辽宁省海城市','地区',279),('辽宁省清原县','地区',238),('辽宁省灯塔县','地区',261),('辽宁省瓦房店市','地区',278),('辽宁省盖  县','地区',255),('辽宁省盘山县','地区',264),('辽宁省盘锦市','地区',262),('辽宁省直辖行政单位','地区',277),('辽宁省绥中县','地区',248),('辽宁省营口县','地区',254),('辽宁省营口市','地区',253),('辽宁省西丰县','地区',267),('辽宁省辽中县','地区',228),('辽宁省辽阳县','地区',260),('辽宁省辽阳市','地区',259),('辽宁省铁岭县','地区',266),('辽宁省铁岭市','地区',265),('辽宁省铁法市','地区',282),('辽宁省锦  县','地区',249),('辽宁省锦州市','地区',247),('辽宁省锦西市','地区',280),('辽宁省长海县','地区',231),('辽宁省阜新县','地区',257),('辽宁省阜新市','地区',256),('辽宁省鞍山市','地区',233),('辽宁省黑山县','地区',251),('达斡尔族','民族',32),('鄂伦春族','民族',53),('鄂温克族','民族',46),('锡伯族','民族',39),('门巴族','民族',55),('阿昌族','民族',40),('高中','学历',59),('高山族','民族',24),('黎族','民族',20),('黑龙江七台河市','地区',90),('黑龙江东宁县','地区',96),('黑龙江五大连池市','地区',122),('黑龙江五常县','地区',102),('黑龙江伊春市','地区',79),('黑龙江佳木斯市','地区',81),('黑龙江依兰县','地区',83),('黑龙江依安县','地区',339),('黑龙江克东县','地区',69),('黑龙江克山县','地区',68),('黑龙江兰西县','地区',113),('黑龙江友谊县','地区',89),('黑龙江双城市','地区',99),('黑龙江双鸭山市','地区',76),('黑龙江同江市','地区',132),('黑龙江呼兰县','地区',335),('黑龙江呼玛县','地区',127),('黑龙江哈尔滨市','地区',334),('黑龙江嘉荫县','地区',80),('黑龙江塔河县','地区',128),('黑龙江大庆市','地区',78),('黑龙江嫩江县','地区',123),('黑龙江孙吴县','地区',126),('黑龙江宁安县','地区',93),('黑龙江密山市','地区',135),('黑龙江富裕县','地区',66),('黑龙江富锦市','地区',133),('黑龙江尚志市','地区',100),('黑龙江庆安县','地区',117),('黑龙江延寿县','地区',107),('黑龙江抚远县','地区',88),('黑龙江方正县','地区',106),('黑龙江明水县','地区',118),('黑龙江望奎县','地区',112),('黑龙江木兰县','地区',104),('黑龙江杜尔伯特县','地区',65),('黑龙江林口县','地区',97),('黑龙江林甸县','地区',67),('黑龙江桦南县','地区',82),('黑龙江桦川县','地区',84),('黑龙江汤原县','地区',86),('黑龙江泰来县','地区',340),('黑龙江海伦县','地区',111),('黑龙江海林县','地区',94),('黑龙江漠河县','地区',129),('黑龙江牡丹江市','地区',92),('黑龙江甘南县','地区',341),('黑龙江省','地区',333),('黑龙江省勃利县','地区',91),('黑龙江省北安市','地区',121),('黑龙江省安达市','地区',109),('黑龙江省宝清县','地区',85),('黑龙江省宾县','地区',101),('黑龙江省巴彦县','地区',103),('黑龙江省德都县','地区',124),('黑龙江省拜泉县','地区',70),('黑龙江省阿城市','地区',131),('黑龙江穆棱县','地区',95),('黑龙江绥化市','地区',108),('黑龙江绥棱县','地区',119),('黑龙江绥滨县','地区',75),('黑龙江绥芬河市','地区',130),('黑龙江肇东市','地区',110),('黑龙江肇州县','地区',116),('黑龙江肇源县','地区',115),('黑龙江萝北县','地区',74),('黑龙江虎林县','地区',98),('黑龙江讷河县','地区',338),('黑龙江逊克县','地区',125),('黑龙江通河县','地区',105),('黑龙江铁力市','地区',134),('黑龙江集贤县','地区',77),('黑龙江青冈县','地区',114),('黑龙江饶河县','地区',87),('黑龙江鸡东县','地区',72),('黑龙江鸡西市','地区',71),('黑龙江鹤岗市','地区',73),('黑龙江黑河市','地区',120),('黑龙江齐齐哈尔市','地区',336),('黑龙江龙江县','地区',337);
/*!40000 ALTER TABLE `cm_option` ENABLE KEYS */;
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
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='用户参数定义';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_params`
--

LOCK TABLES `cm_params` WRITE;
/*!40000 ALTER TABLE `cm_params` DISABLE KEYS */;
INSERT INTO `cm_params` VALUES ('接收新任务','true','','2020-12-01 15:13:49','2020-12-02 09:23:53'),('接收新发布通知','true','','2020-12-02 09:25:15','2020-12-02 09:25:15'),('接收新消息','true','接收通讯录消息推送','2020-12-02 09:24:28','2020-12-02 09:24:28');
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
INSERT INTO `cm_prv` VALUES ('公共文件管理','禁止删除'),('素材库管理','禁止删除');
/*!40000 ALTER TABLE `cm_prv` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_album`
--

DROP TABLE IF EXISTS `cm_pub_album`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_album` (
  `ID` bigint(20) NOT NULL COMMENT '专辑标识',
  `Name` varchar(255) NOT NULL COMMENT '名称',
  `Creator` varchar(32) NOT NULL COMMENT '创建人',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='文章专辑';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_album`
--

LOCK TABLES `cm_pub_album` WRITE;
/*!40000 ALTER TABLE `cm_pub_album` DISABLE KEYS */;
INSERT INTO `cm_pub_album` VALUES (142460430710599680,'新专辑1','daoting','2020-10-28 10:46:54'),(142460798655918080,'新专辑3','daoting','2020-10-28 10:48:22'),(142460807388459008,'新专辑2','daoting','2020-10-28 10:48:24'),(142471178862063616,'abc专辑','daoting','2020-10-28 11:29:37'),(142471215763550208,'123新专辑','daoting','2020-10-28 11:29:45'),(259929892446924800,'123','daoting','2021-09-17 14:28:37');
/*!40000 ALTER TABLE `cm_pub_album` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_comment`
--

DROP TABLE IF EXISTS `cm_pub_comment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_comment` (
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
  CONSTRAINT `fk_comment_parentid` FOREIGN KEY (`ParentID`) REFERENCES `cm_pub_comment` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_comment_postid` FOREIGN KEY (`PostID`) REFERENCES `cm_pub_post` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='评论信息';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_comment`
--

LOCK TABLES `cm_pub_comment` WRITE;
/*!40000 ALTER TABLE `cm_pub_comment` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_pub_comment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_keyword`
--

DROP TABLE IF EXISTS `cm_pub_keyword`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_keyword` (
  `ID` varchar(32) NOT NULL COMMENT '关键字',
  `Creator` varchar(32) NOT NULL COMMENT '创建人',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='关键字';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_keyword`
--

LOCK TABLES `cm_pub_keyword` WRITE;
/*!40000 ALTER TABLE `cm_pub_keyword` DISABLE KEYS */;
INSERT INTO `cm_pub_keyword` VALUES ('啊大大123','daoting','2020-10-28 09:19:10'),('新关键字12','daoting','2021-09-17 14:13:17'),('新关键字2','daoting','2020-10-28 09:05:13'),('新关键字3','daoting','2020-10-28 09:05:48'),('新关键字4','daoting','2020-10-28 09:05:53'),('新关键字撒旦','daoting','2021-09-17 15:30:16'),('脑卒中1','admin','2020-10-27 13:45:50');
/*!40000 ALTER TABLE `cm_pub_keyword` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_post`
--

DROP TABLE IF EXISTS `cm_pub_post`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_post` (
  `ID` bigint(20) NOT NULL COMMENT '文章标识',
  `Title` varchar(255) NOT NULL COMMENT '标题',
  `Cover` varchar(1024) NOT NULL COMMENT '封面',
  `Summary` varchar(512) NOT NULL COMMENT '摘要',
  `Content` text NOT NULL COMMENT '内容',
  `TempType` tinyint(4) unsigned NOT NULL COMMENT '#PostTempType#在列表中显示时的模板类型',
  `IsPublish` tinyint(1) NOT NULL COMMENT '是否发布',
  `AllowCoverClick` tinyint(1) NOT NULL COMMENT '封面可点击',
  `AllowComment` tinyint(1) NOT NULL COMMENT '是否允许评论',
  `AddAlbumLink` tinyint(1) NOT NULL COMMENT '文章末尾是否添加同专辑链接',
  `AddKeywordLink` tinyint(1) NOT NULL COMMENT '文章末尾是否添加同关键字链接',
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
-- Dumping data for table `cm_pub_post`
--

LOCK TABLES `cm_pub_post` WRITE;
/*!40000 ALTER TABLE `cm_pub_post` DISABLE KEYS */;
INSERT INTO `cm_pub_post` VALUES (84827705988476928,'介护福祉士','[[\"v0/CF/B2/91786038024728576.jpg\",\"IMG_20160818_145515\",\"4160 x 2336 (.jpg)\",2691194,\"daoting\",\"2020-06-10 14:44\"],[\"v0/3D/E6/91786445488779264.jpg\",\"IMG_20150518_125023\",\"1080 x 801 (.jpg)\",510138,\"daoting\",\"2020-06-10 14:46\"]]','福祉堂以创建“每个人都向往的养老生活！”为己任，通过养老教育，培养专业人才，通过专业人才提供“预防失能、失能康复”健康养老服务，延长健康寿命，提高本人及家庭生活质量。希望得到政府的关注和扶持，加快企业发展步伐，服务老人、福祉中国。','<p style=\"text-align: center;\"><span style=\"font-size:21px;font-family:宋体;background:#D9D9D9;\">介护福祉士</span></p><ul style=\"list-style-type: undefined;margin-left:26px;\"><li><span style=\"font-size:19px;font-family:宋体;\">人间的尊严与自立</span></li></ul><ul style=\"list-style-type: undefined;margin-left:62px;\"><li><span style=\"font-size:19px;font-family:宋体;\">人间的尊严与自立的意义</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80px;\"><li><span style=\"font-size:19px;font-family:宋体;\">所谓的理解人间是</span><span style=\"font-size:19px;font-family:宋体;\">┈</span></li><li><span style=\"font-size:19px;font-family:宋体;\">人间的尊严的意义</span></li><li><span style=\"font-size:19px;font-family:宋体;\">自立的意义</span></li><li><span style=\"font-size:19px;font-family:宋体;\">自立与自律</span></li><li><span style=\"font-size:19px;font-family:宋体;\">人间的尊严与自立</span></li></ol><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;background:#D9D9D9;\">演习</span><span style=\"font-size:19px;\">&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">所谓的理解人间究竟有什么样的理解方式呢，每个人从自己的经验进行讨论交流</span></p><p><br></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;background:#D9D9D9;\">演习</span><span style=\"font-size:19px;\">&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">对生命的敬畏与其</span><span style=\"font-size:19px;font-family:宋体;\">说</span><span style=\"font-size:19px;font-family:宋体;\">作为知识的理解，倒不如说是从共感的感动的理解中诞生的，举出身边的事例进行讨论交流</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;background:#D9D9D9;\">演习</span><span style=\"font-size:19px;\">&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">从第</span><span style=\"font-size:19px;\">3</span><span style=\"font-size:19px;font-family:宋体;\">页的小</span><span style=\"font-size:19px;\">A</span><span style=\"font-size:19px;font-family:宋体;\">与小</span><span style=\"font-size:19px;\">B</span><span style=\"font-size:19px;font-family:宋体;\">的事例学习，每个的个别的状况，就自立具有的意义进行讨论交流</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;background:#D9D9D9;\">演习</span><span style=\"font-size:19px;\">&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">明确自立与自律的分别具有的意义，就两者的关系进行思考</span></p><ul style=\"list-style-type: undefined;margin-left:62px;\"><li><span style=\"font-size:19px;font-family:宋体;\">围绕尊严和自立的历史与内容</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80px;\"><li><span style=\"font-size:19px;font-family:宋体;\">人权，尊严与自立的思想</span></li></ol><p><span style=\"font-size:19px;font-family:宋体;\">人<img class=\"fr-fic fr-dib\" style=\"width: 300px;\" src=\"../../../fsm/v0/E3/18/143195002217820160.jpg\">权，围绕尊严与自立的历史的经过</span></p><p><br></p><p><br><br></p>',0,1,0,0,0,0,'202112/297166874453786624.html',6,1,'daoting','2020-05-22 09:55:01',NULL,'',NULL,0,0),(84831009241952256,'人体的构造和机能及疾病V1223','[[\"v0/8F/9E/91795724492992512.mp4\",\"VID_20160930_101031\",\"00:00:03 (1920 x 1080)\",3959865,\"daoting\",\"2020-06-10 15:23\"],[\"v0/4E/93/91795768751288320.jpg\",\"IMG_20160921_134610\",\"4160 x 2336 (.jpg)\",2662560,\"daoting\",\"2020-06-10 15:23\"]]','共十三章389页','<p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:24px;background:silver;\">21</span><span style=\"font-size:24px;font-family:宋体;background:silver;\">世纪健康养老服务专业系列教材之</span><span style=\"font-size:24px;background:silver;\">&mdash;&mdash;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:29px;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">人体的构造、机能、疾病</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:19px;font-family:宋体;\">刘 &nbsp;纯 等编著</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p><br></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:24px;font-family:宋体;\">百岁福祉教育科技有限公司</span></strong></p><p><span style=\"font-size:16px;font-family:宋体;\"><br>&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:16px;font-family:宋体;\">前 &nbsp;言</span></p><p><span style=\"font-size:16px;font-family:宋体;\"><br>&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:16px;font-family:宋体;\">目 &nbsp;录</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Times New Roman\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><div style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><ul style=\"margin-bottom:0cm;list-style-type: undefined;margin-left:0cmundefined;\"><li style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">人的成长、发育和衰老</span></li></ul></div><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第一节 身体的成长、发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、胎儿期的成长和发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、幼儿期的成长和发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、儿童期的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">4</span><span style=\"font-size:19px;font-family:宋体;\">、青年、中年期的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第二节 精神的成长、发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、胎儿期的知觉和脑、神经的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、幼儿期的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、儿童期的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">4</span><span style=\"font-size:19px;font-family:宋体;\">、青年、中年期的发育</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第三节 衰老</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、高龄期的生活</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、体力和营养</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、衰老的前兆</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">4</span><span style=\"font-size:19px;font-family:宋体;\">、衰老伴随的身体、生理方面的变化</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">5</span><span style=\"font-size:19px;font-family:宋体;\">、衰老伴随的精神方面的变化</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">第二章 身体构造和身心的机能</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第一节 身体部位的名称</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、身体的名称</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、骨骼的名称</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第二节 各器官的构造及机能</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、水份和脱水</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、血液的成分</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、心脏的构造和循环</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">4</span><span style=\"font-size:19px;font-family:宋体;\">、肾脏的构造和泌尿系统</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">5</span><span style=\"font-size:19px;font-family:宋体;\">、呼吸器官的构造和呼吸</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">6</span><span style=\"font-size:19px;font-family:宋体;\">、消化和吸收</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">7</span><span style=\"font-size:19px;font-family:宋体;\">、神经的构造和机能</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">8</span><span style=\"font-size:19px;font-family:宋体;\">、身体机能的调节</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">第三章 疾病的概要</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第一节 生活习惯病和亚健康</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、生活习惯病</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、亚健康</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第二节 恶性肿瘤</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、主要的恶性肿瘤的统计</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、治疗的基本</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、缓解治理和告知</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第三节 脑血管疾病</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、脑梗塞</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、脑出血和其他的脑血管疾病</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">第四节 心脏疾病</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">1</span><span style=\"font-size:19px;font-family:宋体;\">、缺血性心疾病</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">2</span><span style=\"font-size:19px;font-family:宋体;\">、心力衰竭</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Times New Roman\",\"serif\";text-indent:28.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">3</span><span style=\"font-size:19px;font-family:宋体;\">、心律不齐</span></p>',0,1,0,1,1,1,'202102/185620460645249024.html',7,1,'daoting','2020-05-22 10:08:09',NULL,'',NULL,0,0),(87098359274139648,'杨先生脑卒中康复','[[\"v0/26/2D/91789217609150464.mp4\",\"VID_20160930_100110\",\"00:00:24 (1920 x 1080)\",25116782,\"daoting\",\"2020-06-10 14:57\"]]','','<p><span class=\"fr-video fr-deletable fr-fvc fr-dvb fr-draggable\" contenteditable=\"false\" draggable=\"true\"><video class=\"fr-draggable\" controls=\"controls\" height=\"360\" poster=\"../../../fsm/v0/26/2D/91789217609150464.mp4-t.jpg\" preload=\"none\" src=\"../../../fsm/v0/26/2D/91789217609150464.mp4\" width=\"640\"><br></video></span></p><p><br></p>',0,1,1,0,0,0,'202102/185619865137967104.html',75,1,'daoting','2020-05-28 16:17:47',NULL,'',NULL,0,0),(88875481210679296,'废用综合症','[[\"v0/A1/62/88875538962051072.jpg\",\"u285\",\"1024 x 900 (.jpg)\",263911,\"daoting\",\"2020-06-02 13:59\"],[\"v0/1D/89/91373962043191296.jpg\",\"2\",\"640 x 769 (.jpg)\",25960,\"daoting\",\"2020-06-09 11:27\"],[\"v0/74/1E/91374008964870144.jpg\",\"IMG_20150518_124337\",\"1080 x 811 (.jpg)\",517556,\"daoting\",\"2020-06-09 11:27\"]]','在康复的世界，一个最大的目标就是预防废用综合症为前提的。','<p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Calibri\",\"sans-serif\";font-size:14px;\'><strong><span style=\"font-size:48px;font-family:宋体;\">「废用综合症」</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">在康复的世界，一个最大的目标就是预防废用综合症为前提的。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">废用综合症是指「由于身体的不活动引起的二次性障碍的総称」。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-size:21px;font-family:宋体;\">废用综合症的症状</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">首先，最易想到的就是对筋肉和骨、关节等运动器官的障碍。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">运动器官的障碍</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">&bull;筋萎縮（筋力低下）</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">&bull;筋肉的伸張性的低下＝易外伤</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">怪我</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">&bull;骨密度的低下＝易骨折</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">&bull;关节挛缩</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">可动区域的低下</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">等。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">当然、废用综合症、不仅运动器官、会在全身各处出现症状的。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">人体的内臓，很多都是通过重力，施加适当的负荷，方能维持正常的功能。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">心臓、在站立時、将血液充分送到头部进行收缩、起到泵的作用。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">总是处于卧床的状态的话、这个泵功能没能被充分使用、会逐步变弱。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">其結果、就会出现起立性低血圧等的症状。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">人体，在排尿</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">排便时，膀胱等的排尿</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">排便器官也是利用重力的効果、将尿和便排出体外。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">卧床不起的话、这些功能会低下、出现排尿障碍和便秘等的症状。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">同时、卧床不起、从外界来的刺激（光</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">味</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>・</span><span style=\"font-family:宋体;\">音等）的量极度低下，对大脑的刺激减少。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">而且不运动、从关节和筋肉来的体性感觉（触觉和圧觉）等刺激大脑的机会也減少。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">平时、由外界来的刺激成为脑的恰当的负荷、通过对其的反应而保持正常的功能。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">长期卧床不起会导致认知功能的低下和认知症、精神状态的変化、抑郁、幻想和妄想等。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Calibri\",\"sans-serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">废用综合症常见症状的原因</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">关节挛缩</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">由于麻痺和疼痛等为原因，会发生长时间不活动身体。持续相同姿势的话，关节会变硬，逐步变得不能活动。也就是关节挛缩。由于筋肉和靭帯、失去軟部组织的长度和柔软性、可动性，关节会固硬、会伴有一活动就疼痛的状态。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">在废用综合症中，关节挛缩是最大的問題。关节发生挛缩的话，由于关节可动区域受限、不能进行的日常生活的动作增加感到既不方便、介助者也会因此产生额外的负担。因此，需要一点点地活动固硬的关节、即使症状严重也要尽可能地一点一点地活动关节预防关节挛缩。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">肺炎</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">由于进食机会减少、脑梗塞和注意力的低下、免疫力的低下等，发生吞咽障碍、引起误咽性肺炎的危险率极高。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">肺炎的症状，会出现发烧、咳嗽、痰増加等、但高齢者自觉症状少，意思表示也不充分、常使肺炎严重化。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">观察項目、能够安全地食用的，食物大小、形状、保持座位容易吞咽的姿势，确认是否有呛的反应。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">食欲不振</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">废用综合症、对以前喜欢感兴趣的，一切的意欲会逐步減退（一般除烟、酒外）。这样的有气无力状态会导致对进食意欲减退、常出现食欲不振。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">食欲不振的观察項目、是否比平时吃得少或询问本人，确认体重等。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">便秘</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">由于不活动身体、腸的蠕动运动低下，发生便秘。也有由于进餐量减少，咀嚼力低落和唾液和消化液的分泌量低下等原因，引起便秘。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">观察項目</span><span style=\'font-family:\"MS Mincho\",\"serif\";\'>：</span><span style=\"font-family:宋体;\">进食</span><span style=\"font-family:宋体;\">量、是否偏食、</span>1<span style=\"font-family:宋体;\">天</span>25<span style=\"font-family:宋体;\">～</span>30<span style=\"font-family:宋体;\">品目食食用否、是否保持</span>3<span style=\"font-family:宋体;\">餐。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">尿路感染症</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">废用综合症，由于身体的活动状态处于不活発，血液中的钙质要从骨中补充。</span><span style=\"font-family:宋体;\">这样，很多钙被排出到尿液中と。</span><span style=\"font-family:宋体;\">由于卧床的姿势，尿液滞留尿管易形成結石、引起尿路感染</span> &nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">尿路感染的观察項目，确认「是否发烧、排尿时是否有痛感或不适感、能否憋住尿、是否頻尿、是否有下腹部的疼痛感或不适感、是否腰痛、是否有残尿感」。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">浮腫</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">筋肉持续不用的状态话、運搬血液的泵的作用低下。并且、由于食欲低下，营养不足的状态持续、調整血管内的浸透圧的、血液中的蛋白質的白蛋白降低的话，从血管内来的组织液増加、引起浮腫。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">另外，静脈血栓症和腎功能低下也会发生浮腫。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">浮腫的观察項目，用手指按压仙骨或胫骨的皮膚下的位置約</span>1<span style=\"font-family:宋体;\">分左右进行测试。血液中的白蛋白低下引起的浮腫时，指压痕迹会残留</span>40<span style=\"font-family:宋体;\">秒后恢复。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">认知症</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">废用综合症的不活発性生活、几乎的患者都存在认知功能低下的症状。认知功能的低下重症化时，会变成重度的认知症、由于给日常生活和康复带来障碍，所以废用综合症、同身体功能同样，要有意识地进行精神功能的介护</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">认知症的观察項目，使用長谷川式簡易知能评价基准。「能否说出年齢、能否知道現在的時間、场所、是否能够记忆</span>3<span style=\"font-family:宋体;\">个单词、能否进行简单的計算、数字是否能倒数、过一会儿是否能说出这</span>3<span style=\"font-family:宋体;\">个单词、是否能记住</span>5<span style=\"font-family:宋体;\">个物品的地点、言语流畅否」进行评价。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">抑郁</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">高齢者本人很少提到的抑郁，郁闷。由于废用综合症引起的认知功能的低下和行动力的低下，成为慢性的压抑是抑郁的誘因。并且、不活动和失去了对以前感兴趣的事、生活整体意欲減退、心情沉闷也会变成抑郁，郁闷的。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">抑郁的观察項目，「心情是否沉落、是否有至今一直感兴趣的事不感兴趣了、食欲是否減退、睡眠充足否、是否说话和动作变得缓慢、是否易疲劳、是否感到罪悪感、是否有集中力、是否有自殺願望」等。</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><strong><span style=\"font-family:宋体;\">心功能低下</span></strong></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'>&nbsp;</p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Calibri\",\"sans-serif\";\'><span style=\"font-family:宋体;\">心臓的泵功能，由于長期卧床的姿势会逐步衰退、特别是高齢者的衰退极为显著。同时、心臓的泵功能衰退，再加上上下肢手脚的骨格筋的泵功能低下的话，返回心臓的血液量减少、其結果是从心臓被送出的血液量也减少。废用综合症，特别是下肢的易明显衰退，导致心功能的低下。</span></p>',0,1,0,0,0,0,'202102/185619406948003840.html',142,1,'daoting','2020-06-02 13:59:27',NULL,'',NULL,0,0),(91797036454506496,'社会福祉原論','','所谓的健康是什么「所谓的需要介护的状态」，是由于「心」或「身体」具有某种不自由「障碍」给生活上带来困难的状态。','<p style=\"text-align: center;\"><span style=\"font-size: 30px;\">养成福祉士教材</span></p><p>&nbsp; 心与身体的机理</p><p>序章</p><p>所谓的健康是什么「所谓的需要介护的状态」，是由于「心」或「身体」具有某种不自由「障碍」给生活上带来困难的状态。</p><p>本章首先就不需要介护的状态也就是「健康的状态」进行理解，在理解了精神上的、身体上的、社会性的健康之后，学习判断利用者健康状态的基准，并且作为介护福祉士学习判断利用者生活困难程度的基准的知识。</p><p>①健康的定义</p><p>●身体、心、社会性的健康</p><p>「何为健康」，如果被问了，何为健康呢？你会怎样回答呢？恐怕我想会回答「蛮精神的！」「没有病的状态」等的回答会是一般性的。这样的回答也绝不是错误的，但是这是否可以说是100%的说对了还多少不一样。实际上，所谓的健康是具有很多很多，多方面的很深奥的东西的。</p><p>把健康，坦率的说的话，那么就是WHO宪法的前文，这里的健康的定义如下：「所谓的健康就是身体上、精神上、社会上、适应上完全处于良好的状态，而不是单纯的指疾病或病弱」。加上「心灵的」「动态的」这两个语言，再一看似乎觉得不太懂的语言，但是这是在考虑「所谓的健康是什么」时是具有非常重要的意义的。</p><p>「心灵的」原文是「Spartitual」也就是心灵一般来讲是由心灵的体验或宗教的印象。在WHO中被解释成各种各样的，但是有一个是可以说的都包含着「人类的尊严」和「<strong>生活质量</strong>」的意思。</p><p>P2旁边：WHO:World Health Organization，日语被意为世界保健机构。是国联机构之一，是进行灭绝疾病的研究和医疗普及等，将健康作为人权之一来认为，并且以达成为目的。生活的质量：并不是「只活着」而是「如何活着」，其质量是非常重要的。主要在医疗和福祉现场被使用，与延命处置同时和解除痛苦，将患者能够平稳的度过的方法探取等中诞生。</p><p>&nbsp; &nbsp; 例如说，在介护机构中，有时会给在半夜失禁的高龄者穿尿不湿，与此介护者会轻松简单的。但是利用者本人呢，有事会感到「屈辱的」，失去了尊严「人的尊严」。如果站在重视人的尊严的立场上的话，我们可以认为尿不湿尽可能地不要给其穿为好。</p><p>&nbsp; &nbsp; 并且对于癌症患者来说，为了治疗癌症会投入很多抗癌药物，即使是面向好转，但另一方面会发生头发、眉毛脱落的副作用的现象，尤其是女性由于容姿的变化所带来的非常难受的体验，为了不降低其「生活质量」，我们要考虑化妆啊或准备头套等的想法。</p><p>&nbsp; &nbsp; 这些被保证了的「尊严」和「生活质量」也当然可以说是健康的条件。</p><p>●思考继续性，那么对另一个「动态的」包含着什么样的意义呢</p><p>&nbsp; &nbsp; 「动态的」的原文是「eynamic」。日本也经常使用称为动态。WHO的动态的意思上，其中有一个就是「继续」。</p><p>&nbsp; &nbsp; 这里即使进行健康诊断也没发现异常。尽管被判断为健康，但是由于不断重复吸烟、暴饮暴食的结果。某天会突然由于发生脑梗死病倒的人。</p><p>&nbsp; &nbsp; 这个人在某一时期内确实是健康的，但是是否每日度过「健康的生活」那就不一样了。本来是健康的，但由于过着不健康的生活的结果，就会发病。</p><p>&nbsp; &nbsp; 也就是「健康」和「不健康」，绝不是分别的东西，是在一条直线上并列存在的。这就是健康的身体突然得病的缘故。</p><p>&nbsp; &nbsp; 重要的是维持健康，也就是将维持称为「继续」。具有健康的身心，并将这种状态能够持续，尽可能的每天每日注意预防，可以说是健康的条件。</p><p>&nbsp; &nbsp; 仅仅身体健康并不是「健康」。身体、心、社会上的人的尊严、生活质量、健康的继续，将这些满足的东西称为健康。从事介护工作的人，面对利用者可以说是包括这么多样意义的健康为目标是理想的。</p>',0,1,0,0,0,0,'202201/305542627759169536.html',141,1,'daoting','2020-06-10 15:28:40',NULL,'',NULL,0,0),(91800034912104448,'康复论','[[\"v0/60/55/91800378303967232.jpg\",\"IMG_20150518_124913\",\"1080 x 805 (.jpg)\",541402,\"daoting\",\"2020-06-10 15:41\"],[\"v0/61/62/91800491223019520.jpg\",\"6\",\"960 x 540 (.jpg)\",46138,\"daoting\",\"2020-06-10 15:42\"],[\"v0/B2/18/91800491357237248.jpg\",\"7\",\"540 x 960 (.jpg)\",34263,\"daoting\",\"2020-06-10 15:42\"],[\"v0/F4/5B/91800491449511936.jpg\",\"IMG_20150518_125051\",\"1080 x 802 (.jpg)\",472877,\"daoting\",\"2020-06-10 15:42\"],[\"v0/09/EE/91803792962351104.jpg\",\"2-5\",\"4000 x 3000 (.jpg)\",2190807,\"daoting\",\"2020-06-10 15:55\"],[\"v0/B8/94/91803835261906944.jpg\",\"CIMG5332\",\"4000 x 3000 (.jpg)\",2226785,\"daoting\",\"2020-06-10 15:55\"],[\"v0/65/1F/91803936436908032.jpg\",\"IMG_20160906_145630\",\"4160 x 2336 (.jpg)\",3020173,\"daoting\",\"2020-06-10 15:56\"],[\"v0/EA/34/91803998420332544.jpg\",\"IMG_20160906_145451\",\"4160 x 2336 (.jpg)\",2513845,\"daoting\",\"2020-06-10 15:56\"],[\"v0/21/92/91804086945312768.jpg\",\"IMG_20160818_150302\",\"4160 x 2336 (.jpg)\",2578809,\"daoting\",\"2020-06-10 15:56\"]]','','<p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:24px;font-family:宋体;background:lightgrey;\">健康养老服务专业系列教材之</span><span style=\"font-size:24px;background:lightgrey;\">&mdash;&mdash;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:29px;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:29px;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">康 复 论</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:29px;font-family:宋体;\">&nbsp;</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:19px;font-family:宋体;\">XX &nbsp;XX&nbsp;</span></strong><strong><span style=\"font-size:19px;font-family:宋体;\">编著</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><strong><span style=\"font-size:24px;font-family:宋体;\">长春百岁福祉教育科技有限公司</span></strong></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;\">&nbsp;</span></p><p><span style=\'font-size:14px;font-family:\"Century\",\"serif\";\'><br>&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\'font-size:19px;font-family:\"MS Mincho\",\"serif\";\'>前</span><span style=\"font-size:19px;\">&nbsp;&nbsp;</span><span style=\'font-size:19px;font-family:\"MS Mincho\",\"serif\";\'>言</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;\">&nbsp;</span></p><p><span style=\'font-size:19px;font-family:\"Century\",\"serif\";\'><br>&nbsp;</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: center;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">目</span><span style=\"font-size:19px;\">&nbsp;</span><span style=\"font-size:19px;font-family:宋体;\">录</span></p><div style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><ul style=\"margin-bottom:0cm;list-style-type: undefined;margin-left:26px;\"><li style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">康复的基础知识</span></li></ul></div><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";text-indent:84.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">学习的目标</span></p><ul style=\"list-style-type: undefined;margin-left:62.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">何为康复</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复概论</span></li><li><span style=\"font-size:19px;font-family:宋体;\">康复的<span style=\"background:yellow;\">语源</span></span></li><li><span style=\"font-size:19px;font-family:宋体;\">康复的定义</span></li></ol><ul style=\"list-style-type: undefined;margin-left:62.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复的对象和目的</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复的对象</span></li><li><span style=\"font-size:19px;font-family:宋体;\">康复的目的和历史发展</span></li></ol><ul style=\"list-style-type: undefined;margin-left:62.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复的基本概念</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80.75px;\"><li><span style=\"font-size:19px;font-family:宋体;\">自立生活运动</span></li><li><span style=\"font-size:19px;font-family:宋体;\">普通生活</span></li><li><span style=\"font-size:19px;font-family:宋体;\">国际障碍者年</span></li><li><span style=\"font-size:19px;font-family:宋体;\">障碍者计划</span><span lang=\"EN-US\" style=\"font-size:19px;\">&mdash;&mdash;</span><span style=\"font-size:19px;font-family:宋体;\">普通生活七年战略</span></li><li><span lang=\"EN-US\" style=\"font-size:19px;\">QOL</span><span style=\"font-size:19px;font-family:宋体;\">是什么</span></li><li><span lang=\"EN-US\" style=\"font-size:19px;\">ADL</span><span style=\"font-size:19px;font-family:宋体;\">是什么</span></li><li><span lang=\"EN-US\" style=\"font-size:19px;\">QOL</span><span style=\"font-size:19px;font-family:宋体;\">和</span><span lang=\"EN-US\" style=\"font-size:19px;\">ADL</span><span style=\"font-size:19px;font-family:宋体;\">的关系</span></li><li><span lang=\"EN-US\" style=\"font-size:19px;\">Empowerment</span></li><li><span style=\"font-size:19px;font-family:宋体;\">生活机能与障碍的国际分类</span></li></ol><ul style=\"list-style-type: undefined;margin-left:26px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍的类种与社会保障制度</span></li></ul><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";text-indent:84.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">学习的目标</span></p><ul style=\"list-style-type: undefined;margin-left:52.25px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍者</span></li></ul><ol style=\"list-style-type: undefined;margin-left:70.25px;\"><li><span style=\"font-size:19px;font-family:宋体;\">身体障碍者</span></li><li><span style=\"font-size:19px;font-family:宋体;\">精神障碍者</span></li><li><span style=\"font-size:19px;font-family:宋体;\">认知障碍者</span></li></ol><ul style=\"list-style-type: undefined;margin-left:52.25px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍者（儿童）与社会保障制度</span></li></ul><ol style=\"list-style-type: undefined;margin-left:70.25px;\"><li><span style=\"font-size:19px;font-family:宋体;\">身体障碍者福祉法</span></li><li><span style=\"font-size:19px;font-family:宋体;\">精神保健及精神障碍者福祉的法律</span></li><li><span style=\"font-size:19px;font-family:宋体;\">认知障碍者福祉法</span></li><li><span style=\"font-size:19px;font-family:宋体;\">儿童福祉法</span></li></ol><ul style=\"list-style-type: undefined;margin-left:26px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复分类与其过程</span></li></ul><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";text-indent:84.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">学习目标</span></p><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复的种类</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span style=\"font-size:19px;font-family:宋体;\">医学的康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">职业的康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">教育的康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">社会的康复</span></li></ol><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">地域康复与地域福祉</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span lang=\"EN-US\" style=\"font-size:19px;\">community based rehabilitation:CBR</span></li><li><span style=\"font-size:19px;font-family:宋体;\">地域康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">地域福祉</span></li></ol><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复的过程</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span style=\"font-size:19px;font-family:宋体;\">评价</span></li><li><span style=\"font-size:19px;font-family:宋体;\">康复与多职种团队</span></li><li><span style=\"font-size:19px;font-family:宋体;\">多职种团队与各专业</span></li></ol><ul style=\"list-style-type: undefined;margin-left:26px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍者、高龄者与康复</span></li></ul><p style=\'margin-top:0cm;margin-right:0cm;margin-bottom:.0001pt;margin-left:36.0pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";text-indent:49.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">学习的目标</span></p><ul style=\"list-style-type: undefined;margin-left:62px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍者（儿童）与康复</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80px;\"><li><span style=\"font-size:19px;font-family:宋体;\">身体障碍者与康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">精神障碍者与康复</span></li><li><span style=\"font-size:19px;font-family:宋体;\">认知障碍者与康复</span></li></ol><ul style=\"list-style-type: undefined;margin-left:62px;\"><li><span style=\"font-size:19px;font-family:宋体;\">高龄者与康复</span></li></ul><ol style=\"list-style-type: undefined;margin-left:80px;\"><li><span style=\"font-size:19px;font-family:宋体;\">我国的平均寿命与高龄化</span></li><li><span style=\"font-size:19px;font-family:宋体;\">我国的高龄者比率</span></li><li><span style=\"font-size:19px;font-family:宋体;\">高龄者与身体障碍者</span></li><li><span style=\"font-size:19px;font-family:宋体;\">高龄者卧床不起的防止</span></li><li><span style=\"font-size:19px;font-family:宋体;\">痴呆高龄者</span></li><li><span style=\"font-size:19px;font-family:宋体;\">帕金森综合症</span></li><li><span style=\"font-size:19px;font-family:宋体;\">高龄者福祉服务</span></li></ol><ul style=\"list-style-type: undefined;margin-left:26px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复对象的理解与支援器械</span></li></ul><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";text-indent:84.0pt;\'><span style=\"font-size:19px;font-family:宋体;\">学习的目标</span></p><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">从运动学的角度理解康复对象</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span style=\"font-size:19px;font-family:宋体;\">身体运动的基准点</span></li><li><span style=\"font-size:19px;font-family:宋体;background:yellow;\">体节及</span><span lang=\"EN-US\" style=\"font-size:19px;background:yellow;\">guanjie</span></li><li><span style=\"font-size:19px;font-family:宋体;\">身体的骨骼</span></li><li><span style=\"font-size:19px;font-family:宋体;\">关节运动</span></li><li><span style=\"font-size:19px;font-family:宋体;\">筋力（肌肉和周围的软组织的力量）</span></li><li><span style=\"font-size:19px;font-family:宋体;\">末梢神经和中枢神经</span></li></ol><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">康复支援器械</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span style=\"font-size:19px;font-family:宋体;\">福祉用具和假肢装具（辅装具）</span></li><li><span style=\"font-size:19px;font-family:宋体;\">假肢装具</span></li><li><span style=\"font-size:19px;font-family:宋体;\">福祉用具</span></li><li><span style=\"font-size:19px;font-family:宋体;\">介护保险与福祉用具</span></li><li><span style=\"font-size:19px;font-family:宋体;\">辅装具与日常生活用具</span></li></ol><ul style=\"list-style-type: undefined;margin-left:47px;\"><li><span style=\"font-size:19px;font-family:宋体;\">障碍的理解</span><span lang=\"EN-US\" style=\"font-size:19px;\">&mdash;&mdash;</span><span style=\"font-size:19px;font-family:宋体;\">示例</span></li></ul><ol style=\"list-style-type: undefined;margin-left:65px;\"><li><span style=\"font-size:19px;font-family:宋体;\">脑血管障碍的康复与中枢神经</span></li><li><span style=\"font-size:19px;font-family:宋体;\">截肢者的康复与社会保障制度</span></li></ol><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;font-family:宋体;\">附件：参考文献</span></p><p style=\'margin: 0cm 0cm 0pt;text-align: right;font-family: \"Century\",\"serif\";font-size:14px;\'><span style=\"font-size:19px;font-family:宋体;\">共</span><span style=\"font-size:19px;\">167</span><span style=\"font-size:19px;font-family:宋体;\">页</span></p><p style=\'margin-top:0cm;margin-right:0cm;margin-bottom:.0001pt;margin-left:57.0pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;\">&nbsp;</span></p><p style=\'margin:0cm;margin-bottom:.0001pt;text-align:justify;font-size:14px;font-family:\"Century\",\"serif\";\'><span style=\"font-size:19px;\">&nbsp;</span></p>',0,1,0,0,0,0,'202102/185619586971725824.html',140,1,'daoting','2020-06-10 15:40:34',NULL,'',NULL,0,0),(92497574754119680,'维持提高QOL，维持提高心身健康','[[\"v0/00/F7/92529784261570560.png\",\"u354\",\"553 x 291 (.png)\",296599,\"daoting\",\"2020-06-12 16:00\"]]','','<p>养老服务核心目标&nbsp;</p><p>维持、提高QOL；维持、提高心身健康。<img src=\"../../v0/56/59/143191060947791872.png\" style=\"width: 300px;\" class=\"fr-fic fr-dib\"></p>',1,1,0,0,0,0,'202201/301998163539206144.html',148,1,'daoting','2020-06-12 13:52:22',NULL,'',NULL,0,0);
/*!40000 ALTER TABLE `cm_pub_post` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_postalbum`
--

DROP TABLE IF EXISTS `cm_pub_postalbum`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_postalbum` (
  `PostID` bigint(20) NOT NULL COMMENT '文章标识',
  `AlbumID` bigint(20) NOT NULL COMMENT '专辑标识',
  PRIMARY KEY (`PostID`,`AlbumID`) USING BTREE,
  KEY `fk_postalbum_albumid` (`AlbumID`),
  KEY `fk_postalbum_postid` (`PostID`) USING BTREE,
  CONSTRAINT `fk_postalbum_albumid` FOREIGN KEY (`AlbumID`) REFERENCES `cm_pub_album` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_postalbum_postid` FOREIGN KEY (`PostID`) REFERENCES `cm_pub_post` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_postalbum`
--

LOCK TABLES `cm_pub_postalbum` WRITE;
/*!40000 ALTER TABLE `cm_pub_postalbum` DISABLE KEYS */;
INSERT INTO `cm_pub_postalbum` VALUES (84831009241952256,142460430710599680),(84831009241952256,142460798655918080),(84831009241952256,142460807388459008),(84831009241952256,142471215763550208),(84827705988476928,259929892446924800);
/*!40000 ALTER TABLE `cm_pub_postalbum` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_postkeyword`
--

DROP TABLE IF EXISTS `cm_pub_postkeyword`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_postkeyword` (
  `PostID` bigint(20) NOT NULL COMMENT '文章标识',
  `Keyword` varchar(32) NOT NULL COMMENT '关键字',
  PRIMARY KEY (`PostID`,`Keyword`) USING BTREE,
  KEY `fk_postkw_postid` (`PostID`) USING BTREE,
  KEY `fk_postkw_keyword` (`Keyword`),
  CONSTRAINT `fk_postkw_keyword` FOREIGN KEY (`Keyword`) REFERENCES `cm_pub_keyword` (`ID`),
  CONSTRAINT `fk_postkw_postid` FOREIGN KEY (`PostID`) REFERENCES `cm_pub_post` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_postkeyword`
--

LOCK TABLES `cm_pub_postkeyword` WRITE;
/*!40000 ALTER TABLE `cm_pub_postkeyword` DISABLE KEYS */;
INSERT INTO `cm_pub_postkeyword` VALUES (84827705988476928,'脑卒中1'),(84831009241952256,'啊大大123'),(84831009241952256,'新关键字2'),(84831009241952256,'新关键字3'),(84831009241952256,'新关键字4');
/*!40000 ALTER TABLE `cm_pub_postkeyword` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pubfile`
--

DROP TABLE IF EXISTS `cm_pubfile`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pubfile` (
  `ID` bigint(20) NOT NULL COMMENT '文件标识',
  `ParentID` bigint(20) DEFAULT NULL COMMENT '上级目录，根目录的parendid为空',
  `Name` varchar(255) NOT NULL COMMENT '名称',
  `IsFolder` tinyint(1) NOT NULL COMMENT '是否为文件夹',
  `ExtName` varchar(8) DEFAULT NULL COMMENT '文件扩展名',
  `Info` varchar(512) NOT NULL COMMENT '文件描述信息',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`ID`),
  KEY `fk_pubfile_parentid` (`ParentID`),
  CONSTRAINT `fk_pubfile_parentid` FOREIGN KEY (`ParentID`) REFERENCES `cm_pubfile` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='公共文件';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pubfile`
--

LOCK TABLES `cm_pubfile` WRITE;
/*!40000 ALTER TABLE `cm_pubfile` DISABLE KEYS */;
INSERT INTO `cm_pubfile` VALUES (1,NULL,'公共文件',1,NULL,'','2020-10-21 15:19:20'),(2,NULL,'素材库',1,NULL,'','2020-10-21 15:20:21'),(140015729575325696,1,'新目录a',1,NULL,'','0001-01-01 00:00:00'),(140016348063199232,1,'新目录1111',1,NULL,'','2020-10-21 16:55:00'),(140244264617373696,140016348063199232,'新目录q',1,NULL,'','2020-10-22 08:00:39'),(140253323206717440,140244264617373696,'ab',1,NULL,'','2020-10-22 08:36:39'),(140266906502164480,140244264617373696,'aa',0,'xlsx','[[\"v0/1F/4A/140266906879651840.xlsx\",\"aa\",\"xlsx文件\",8236,\"daoting\",\"2020-10-22 09:30\"]]','2020-10-22 09:30:37'),(142873261784297472,2,'新目录1',1,NULL,'','2020-10-29 14:07:20'),(142888903606398976,2,'12',0,'xlsx','[[\"v0/52/37/142888904373956608.xlsx\",\"12\",\"xlsx文件\",8153,\"daoting\",\"2020-10-29 15:09\"]]','2020-10-29 15:09:30'),(142913881819181056,2,'未标题-2',0,'jpg','[[\"v0/E3/18/142913882284748800.jpg\",\"未标题-2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-10-29 16:48\"]]','2020-10-29 16:48:44'),(142914110945619968,2,'Icon-20@2x',0,'png','[[\"v0/E3/0D/142914111109197824.png\",\"Icon-20@2x\",\"40 x 40 (.png)\",436,\"daoting\",\"2020-10-29 16:49\"]]','2020-10-29 16:49:39'),(143174605384577024,140016348063199232,'Icon-20@3x',0,'png','[[\"v0/56/59/143174606269575168.png\",\"Icon-20@3x\",\"60 x 60 (.png)\",496,\"daoting\",\"2020-10-30 10:04\"]]','2020-10-30 10:04:47'),(143191060503195648,1,'Icon-20@3x',0,'png','[[\"v0/56/59/143191060947791872.png\",\"Icon-20@3x\",\"60 x 60 (.png)\",534,\"daoting\",\"2020-10-30 11:10\"]]','2020-10-30 11:10:10'),(143192411157164032,140015729575325696,'Icon-29@2x',0,'png','[[\"v0/46/CE/143192411832446976.png\",\"Icon-29@2x\",\"58 x 58 (.png)\",624,\"daoting\",\"2020-10-30 11:15\"]]','2020-10-30 11:15:32'),(143193081423720448,140015729575325696,'3709740f5c5e4cb4909a6cc79f412734_th',0,'png','[[\"v0/BF/6D/143193081931231232.png\",\"3709740f5c5e4cb4909a6cc79f412734_th\",\"537 x 302 (.png)\",27589,\"daoting\",\"2020-10-30 11:18\"]]','2020-10-30 11:18:12'),(143195001659977728,1,'未标题-2',0,'jpg','[[\"v0/E3/18/143195002217820160.jpg\",\"未标题-2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-10-30 11:25\"]]','2020-10-30 11:25:50'),(143203944146792448,1,'ImageStabilization',0,'wmv','[[\"v0/EA/34/143203944767549440.wmv\",\"ImageStabilization\",\"00:00:06 (480 x 288)\",403671,\"daoting\",\"2020-10-30 12:01\"]]','2020-10-30 12:01:22'),(172190549410705408,1,'公司服务器及网络',0,'txt','[[\"v0/5F/37/172190549775609856.txt\",\"公司服务器及网络\",\"txt文件\",435,\"daoting\",\"2021-01-18 11:43\"]]','2021-01-18 11:43:37'),(185641691419373568,1,'1',0,'png','[[\"v0/FC/63/185641725430984704.png\",\"1\",\"1101 x 428 (.png)\",47916,\"daoting\",\"2021-02-24 14:33\"]]','2021-02-24 14:33:46'),(187725770344230912,1,'doc1',0,'png','[[\"v0/D8/28/187725778074333184.png\",\"doc1\",\"1076 x 601 (.png)\",59038,\"daoting\",\"2021-03-02 08:35\"]]','2021-03-02 08:35:12'),(205916917767991296,140015729575325696,'state',0,'db','[[\"v0/DF/F3/205916918690738176.db\",\"state\",\"db文件\",90112,\"苹果\",\"2021-04-21 13:20\"]]','2021-04-21 13:20:20'),(255970120425140224,1,'abc',1,'','','2021-09-06 16:13:53');
/*!40000 ALTER TABLE `cm_pubfile` ENABLE KEYS */;
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
INSERT INTO `cm_role` VALUES (1,'任何人','所有用户默认都具有该角色，不可删除'),(2,'系统管理员','系统角色，不可删除'),(22844822693027840,'收发员',''),(152695933758603264,'市场经理',''),(152696004814307328,'综合经理',''),(152696042718232576,'财务经理',''),(155563622655066112,'新角色1','123');
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
INSERT INTO `cm_rolemenu` VALUES (1,2),(152696042718232576,2),(1,3),(1,4),(152695933758603264,4),(1,5),(152695933758603264,5),(1,6),(1,7),(1,15315637929975808),(2,18860286065975296),(1,139207663032332288),(2,142179186714210304),(2,144356540693737472),(22844822693027840,154430055023640576);
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
INSERT INTO `cm_roleprv` VALUES (2,'公共文件管理'),(152696004814307328,'公共文件管理'),(2,'素材库管理'),(152696004814307328,'素材库管理');
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
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `idx_rpt_name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='报表模板定义';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_rpt`
--

LOCK TABLES `cm_rpt` WRITE;
/*!40000 ALTER TABLE `cm_rpt` DISABLE KEYS */;
INSERT INTO `cm_rpt` VALUES (139241259579338752,'测试报表111','<Rpt cols=\"80,80,80,80,80,80,80\">\r\n  <Params>\r\n    <Param name=\"新参数1\"><![CDATA[<a:CText Title=\"标题1\" />]]></Param>\r\n    <Param name=\"新参数2\"><![CDATA[<a:CText Title=\"标题2\" />]]></Param>\r\n  </Params>\r\n  <Data />\r\n  <Page />\r\n  <Header />\r\n  <Body rows=\"30,30,30,30,30\">\r\n    <Text row=\"4\" col=\"6\" val=\"文本\" lbs=\"None\" tbs=\"None\" rbs=\"None\" bbs=\"None\" />\r\n  </Body>\r\n  <Footer />\r\n  <View />\r\n</Rpt>','新增测试1','2020-10-19 13:35:10','2020-10-20 13:34:54'),(139540400075304960,'abc1','<Rpt cols=\"80,80,80,80,80\">\r\n  <Params />\r\n  <Data />\r\n  <Page />\r\n  <Header />\r\n  <Body rows=\"30,30,30,30,30,30,30,30\">\r\n    <Text row=\"2\" col=\"2\" val=\"文本\" lbs=\"None\" tbs=\"None\" rbs=\"None\" bbs=\"None\" />\r\n    <Text row=\"4\" col=\"3\" colspan=\"2\" val=\"文本\" lbs=\"None\" tbs=\"None\" rbs=\"None\" bbs=\"None\" />\r\n    <Text row=\"7\" col=\"3\" val=\"文本\" lbs=\"None\" tbs=\"None\" rbs=\"None\" bbs=\"None\" />\r\n  </Body>\r\n  <Footer />\r\n  <View />\r\n</Rpt>','阿斯顿法定','2020-10-20 09:24:01','2020-11-30 11:04:28'),(150118388697264128,'abc12','','','2020-11-18 13:57:21','2020-11-18 13:57:21'),(154424288497369088,'新报表abc','','','2020-11-30 11:07:07','2020-11-30 11:07:07'),(259588273038290944,'新报表3','','','2021-09-16 15:51:31','2021-09-16 15:51:53');
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
INSERT INTO `cm_sql` VALUES ('个人文件-子级文件夹','select * from cm_myfile where isfolder=1 and parentid=@parentid',NULL),('个人文件-子项个数','select count(*) from cm_myfile where parentid=@parentid',NULL),('个人文件-所有子级','select * from cm_myfile where parentid=@parentid',NULL),('个人文件-扩展名过滤子级','select\r\n	* \r\nfrom\r\n	cm_myfile \r\nwhere\r\n	parentid = @parentid \r\n	and ( isfolder = 1 or locate( extname, @extname ) )',NULL),('个人文件-扩展名过滤根目录','select\r\n	* \r\nfrom\r\n	cm_myfile \r\nwhere\r\n	parentid is null \r\n	and userid = @userid \r\n	and ( isfolder = 1 or locate( extname, @extname ) )',NULL),('个人文件-搜索文件','select * from cm_myfile where isfolder=0 and userid=@userid and name like @name limit 20',NULL),('个人文件-根目录','select * from cm_myfile where parentid is null and userid=@userid',NULL),('参数-最近修改','select * from cm_params where to_days(now()) - to_days(mtime) <= 2',NULL),('参数-模糊查询','select * from cm_params where id like @id',NULL),('参数-用户设置','select p.*, u.name from cm_userparams p, cm_user u where p.ParamID=@ParamID',NULL),('参数-用户设置数','select count(*) from cm_userparams where ParamID=@ParamID',NULL),('参数-重复名称','select count(*) from cm_params where id=@id',NULL),('发布-专辑引用数','select count(*) from cm_pub_postalbum where albumid=@albumid',NULL),('发布-关键字引用数','select count(*) from cm_pub_postkeyword where keyword=@keyword',NULL),('发布-所有专辑','select * from cm_pub_album',NULL),('发布-所有关键字','select * from cm_pub_keyword',NULL),('发布-模糊查询专辑','select * from cm_pub_album where name like @name',NULL),('发布-模糊查询关键字','select * from cm_pub_keyword where id like @id',NULL),('报表-id','SELECT\r\n	id,name,note,ctime,mtime\r\nFROM\r\n	cm_rpt\r\nWHERE\r\n	id=@id',NULL),('报表-所有','select id,name,note,ctime,mtime from cm_rpt',NULL),('报表-最近修改','SELECT\r\n	id,name,note,ctime,mtime\r\nFROM\r\n	cm_rpt\r\nWHERE\r\n	to_days(now()) - to_days(mtime) <= 2',NULL),('报表-模板','select define from cm_rpt where id=@id',NULL),('报表-模糊查询',' SELECT\r\n	id,name,note,ctime,mtime\r\nFROM\r\n	cm_rpt\r\nWHERE\r\n	NAME LIKE @input',NULL),('报表-重复名称','select count(*) from cm_rpt where name=@name',NULL),('文件-子级文件夹','select * from cm_pubfile where isfolder=1 and parentid=@parentid',NULL),('文件-子项个数','select count(*) from cm_pubfile where parentid=@parentid',NULL),('文件-所有子级','select * from cm_pubfile where parentid=@parentid',NULL),('文件-扩展名过滤子级','select\r\n	* \r\nfrom\r\n	cm_pubfile \r\nwhere\r\n	parentid = @parentid \r\n	and ( isfolder = 1 or locate( extname, @extname ) )',NULL),('文件-搜索所有文件','select\r\n	info \r\nfrom\r\n	cm_pubfile \r\nwhere\r\n	isfolder = 0 \r\n	and name like @name union\r\nselect\r\n	info \r\nfrom\r\n	cm_myfile \r\nwhere\r\n	isfolder = 0 \r\n	and userid = @userid \r\n	and name like @name \r\n	limit 20',NULL),('文件-搜索扩展名文件','select\r\n	info \r\nfrom\r\n	cm_pubfile \r\nwhere\r\n	isfolder = 0 \r\n	and locate( extname, @extname ) \r\n	and name like @name union\r\nselect\r\n	info \r\nfrom\r\n	cm_myfile \r\nwhere\r\n	isfolder = 0 \r\n	and locate( extname, @extname ) \r\n	and userid = @userid \r\n	and name like @name \r\n	limit 20',NULL),('文件-搜索文件','select * from cm_pubfile where isfolder=0 and name like @name limit 20',NULL),('文章-已选专辑','select\r\n	a.id,\r\n	a.name \r\nfrom\r\n	cm_pub_album a\r\n	inner join cm_pub_postalbum b on a.id = b.albumid \r\nwhere\r\n	b.postid = @postid',NULL),('文章-已选关键字','select keyword from cm_pub_postkeyword where postid=@postid',NULL),('文章-未选专辑','select\r\n	id,\r\n	name\r\nfrom\r\n	cm_pub_album a \r\nwhere\r\n	not exists ( select albumid from cm_pub_postalbum where albumid = a.id and postid = @postid )',NULL),('文章-未选关键字','select\r\n	id \r\nfrom\r\n	cm_pub_keyword a \r\nwhere\r\n	not exists ( select keyword from cm_pub_postkeyword where keyword = a.id and postid = @postid )',NULL),('文章-模糊查询','select\r\n	ID,\r\n	Title,\r\n	IsPublish,\r\n	Dispidx,\r\n	Creator,\r\n	Ctime,\r\n	ReadCount,\r\n	CommentCount \r\nfrom\r\n	cm_pub_post\r\nwhere\r\n	Title like @input\r\norder by\r\n	Dispidx desc',NULL),('文章-管理列表','select\r\n	ID,\r\n	Title,\r\n	IsPublish,\r\n	Dispidx,\r\n	Creator,\r\n	Ctime,\r\n	ReadCount,\r\n	CommentCount \r\nfrom\r\n	cm_pub_post \r\norder by\r\n	Dispidx desc',NULL),('文章-编辑','select\r\n	a.*,\r\n	( CASE a.TempType WHEN 0 THEN \'普通\' WHEN 1 THEN \'封面标题混合\' ELSE \'普通\' END ) TempType_dsp \r\nfrom\r\n	cm_pub_post a \r\nwhere\r\n	id = @id',NULL),('权限-关联用户','select distinct (c.name)\r\n  from cm_roleprv a, cm_userrole b, cm_user c\r\n where a.roleid = b.roleid\r\n   and b.userid = c.id\r\n   and a.prvid = @prvid\r\n order by c.name',NULL),('权限-关联角色','select id as roleid, b.name as rolename, a.prvid\r\n  from cm_roleprv a\r\n  left join cm_role b\r\n    on a.roleid = b.id\r\n where a.prvid = @prvid',NULL),('权限-名称重复','select count(id) from cm_prv where id=@id',NULL),('权限-所有','select * from cm_prv',NULL),('权限-未关联的角色','select id, name, note\r\n  from cm_role a\r\n where not exists (select roleid\r\n          from cm_roleprv b\r\n         where a.id = b.roleid\r\n           and prvid = @prvid)',NULL),('权限-模糊查询','select * from cm_prv where id like @id',NULL),('权限-系统权限','select * from cm_prv where id < 1000',NULL),('流程-前一活动执行者','select distinct\r\n	userid \r\nfrom\r\n	cm_wfi_item \r\nwhere\r\n	atviid in ( select id from cm_wfi_atv where prciid = @prciId and atvdid in ( select SrcAtvID from cm_wfd_trs where TgtAtvID = @atvdid ) )',NULL),('流程-前一活动的同部门执行者','select distinct\r\n	userid \r\nfrom\r\n	cm_xemp \r\nwhere\r\n	depid in (\r\nselect distinct\r\n	depid \r\nfrom\r\n	cm_xemp \r\nwhere\r\n	userid in (\r\nselect\r\n	userid \r\nfrom\r\n	cm_wfi_item \r\nwhere\r\n	atviid in ( select ID from cm_wfi_atv where prciid = @prciId and atvdid in ( select SrcAtvID from cm_wfd_trs where TgtAtvID = @atvdid ) ) \r\n	) \r\n	)',NULL),('流程-历史任务','select wi.id itemid,\r\n			 pi.id prciid,\r\n			 pd.id prcdid,\r\n			 ad.id atvdid,\r\n			 ai.id atviid,\r\n			 pd.name prcname,\r\n			 ( CASE pi.status WHEN 1 THEN \'已结束\' WHEN 2 THEN \'已终止\' ELSE ad.name END ) as atvname,\r\n			 pi.status,\r\n			 pi.name formname,\r\n			 wi.sender,\r\n			 wi.stime,\r\n			 max(wi.mtime) mtime,\r\n			 wi.reCount\r\n	from cm_wfi_atv ai,\r\n			 cm_wfi_prc pi,\r\n			 cm_wfd_atv ad,\r\n			 cm_wfd_prc pd,\r\n			 (select id,\r\n							 atviid,\r\n							 mtime,\r\n							 sender,\r\n							 stime,\r\n							 (select count(1)\r\n									from cm_wfi_item\r\n								 where atviid = t.atviid\r\n									 and AssignKind = 4\r\n									 and id <> t.id) as reCount\r\n					from cm_wfi_item t\r\n				 where status = 1\r\n					 and userid = @userID\r\n					 and (@start < \'1900-01-01\' or mtime >= @start)\r\n					 and (@end < \'1900-01-01\' or mtime <= @end)\r\n					 order by mtime desc) wi\r\n where wi.atviid = ai.id\r\n	 and ai.prciid = pi.id\r\n	 and pi.prcdid = pd.id\r\n	 and ai.atvdid = ad.id\r\n	 and wi.reCount = 0\r\n	 and (@status > 2 or pi.status = @status)\r\n group by prciid\r\n order by wi.stime desc',NULL),('流程-参与的流程','select distinct\r\n	p.id,\r\n	p.name,\r\n	p.ListType\r\nfrom\r\n	cm_wfd_prc p,\r\n	cm_wfd_atv a,\r\n	cm_wfd_atvrole r,\r\n	cm_userrole u \r\nwhere\r\n	p.id = a.prcid \r\n	and a.id = r.atvid \r\n	and ( r.roleid = u.roleid or r.roleid = 1 ) \r\n	and u.userid = @userID\r\norder by\r\n	p.dispidx',NULL),('流程-可启动流程','select\r\n	pd.id,\r\n	name \r\nfrom\r\n	cm_wfd_prc pd,\r\n	(\r\nselect distinct\r\n	p.id \r\nfrom\r\n	cm_wfd_prc p,\r\n	cm_wfd_atv a,\r\n	cm_wfd_atvrole r,\r\n	cm_userrole u \r\nwhere\r\n	p.id = a.prcid \r\n	and a.id = r.atvid \r\n	and ( r.roleid = u.roleid or r.roleid = 1 ) \r\n	and u.userid = @userid \r\n	and p.islocked = 0 \r\n	and a.type = 1 \r\n	) pa \r\nwhere\r\n	pd.id = pa.id \r\norder by\r\n	dispidx;',NULL),('流程-同步活动实例数','select\r\n	count( * ) \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	prciid = @prciid \r\n	and atvdid = @atvdid',NULL),('流程-后续活动','select\r\n	atv.* \r\nfrom\r\n	cm_wfd_atv atv,\r\n	( select trs.TgtAtvID atvid from cm_wfd_trs trs where trs.SrcAtvID = @atvid and IsRollback = 0 ) trs \r\nwhere\r\n	atv.id = trs.atvid',NULL),('流程-后续活动工作项','select\r\n	a.IsAccept,\r\n	a.Status,\r\n	b.id atviid \r\nfrom\r\n	cm_wfi_item a,\r\n	cm_wfi_atv b \r\nwhere\r\n	a.atviid = b.id \r\n	and b.atvdid in ( select TgtAtvID from cm_wfd_trs d where d.SrcAtvID = @atvdid and d.IsRollback = 0 ) \r\n	and b.prciid = @prciid',NULL),('流程-回退活动实例','select\r\n	* \r\nfrom\r\n	cm_wfi_atv a \r\nwhere\r\n	prciid = @prciid \r\n	and exists ( select TgtAtvID from cm_wfd_trs b where SrcAtvID = @SrcAtvID and b.IsRollback = 1 and a.atvdid = b.TgtAtvID ) \r\norder by\r\n	mtime desc',NULL),('流程-实例id获取模板id','select PrcdID from cm_wfi_prc where id=@id',NULL),('流程-工作项个数','select\r\n	count( * ) \r\nfrom\r\n	cm_wfi_item \r\nwhere\r\n	atviid = @atviid \r\n	and status = 1',NULL),('流程-工作项的活动实例','select\r\n	* \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	id = ( select atviid from cm_wfi_item where id = @itemid )',NULL),('流程-工作项的流程实例','select\r\n	* \r\nfrom\r\n	cm_wfi_prc \r\nwhere\r\n	id = ( select prciid from cm_wfi_atv where id = ( select atviid from cm_wfi_item where id = @itemid ) )',NULL),('流程-已完成活动同部门执行者','select distinct\r\n	userid \r\nfrom\r\n	cm_xemp \r\nwhere\r\n	depid in (\r\nselect distinct\r\n	depid \r\nfrom\r\n	cm_xemp \r\nwhere\r\n	userid in ( select userid from cm_wfi_item where atviid in ( select id from cm_wfi_atv where prciid = @prciId and atvdid = @atvdid ) ) \r\n	)',NULL),('流程-已完成活动执行者','select distinct\r\n	userid \r\nfrom\r\n	cm_wfi_item \r\nwhere\r\n	atviid in ( select id from cm_wfi_atv where prciid = @prciId and atvdid = @atvdid )',NULL),('流程-待办任务','select wi.id   itemid,\r\n		 pi.id     prciid,\r\n		 pd.id     prcdid,\r\n		 pd.name   prcname,\r\n		 ad.name   atvname,\r\n		 pi.name   formname,\r\n		 wi.AssignKind,\r\n		 wi.sender,\r\n		 wi.stime,\r\n		 wi.IsAccept\r\nfrom cm_wfi_atv ai,\r\n		 cm_wfd_atv ad,\r\n		 cm_wfi_prc pi,\r\n		 cm_wfd_prc pd,\r\n		 (select id,\r\n						 atviid,\r\n						 sender,\r\n						 stime,\r\n						 IsAccept,\r\n						 AssignKind\r\n				from cm_wfi_item wi\r\n			 where status = 0\r\n				 and (userid = @userID or\r\n						 (userid is null and\r\n						 (exists (select 1\r\n													from cm_userrole\r\n												 where wi.roleid = roleid\r\n													 and userid = @userID)) or\r\n						 roleid = 1))) wi\r\nwhere ai.id = wi.atviid\r\n and ai.atvdid = ad.id\r\n and ai.prciid = pi.id\r\n and pi.prcdid = pd.id\r\norder by wi.stime desc',NULL),('流程-待办任务总数','select\r\n	sum( 1 ) allTask \r\nfrom\r\n	cm_wfi_prc a,\r\n	cm_wfi_atv b,\r\n	cm_wfi_item c \r\nwhere\r\n	a.id = b.prciid \r\n	and b.id = c.atviid \r\n	and c.status = 0 \r\n	and (\r\n	c.userid = @userid \r\n	or ( userid is null and exists ( select 1 from cm_userrole where c.roleid = roleid and userid = @userid ) ) \r\n	)',NULL),('流程-所有未过期用户','select id, name from cm_user where expired = 0',NULL),('流程-所有流程模板','select ID,Name,IsLocked,Singleton,Note,Dispidx,Ctime,Mtime from cm_wfd_prc order by Dispidx',NULL),('流程-所有流程模板名称','select\r\n	id,\r\n	name \r\nfrom\r\n	cm_wfd_prc \r\norder by\r\n	dispidx;',NULL),('流程-所有经办历史任务','select wi.id itemid,\r\n			 pi.id prciid,\r\n			 pd.id prcdid,\r\n			 ad.id atvdid,\r\n			 ai.id atviid,\r\n			 pd.name prcname,\r\n			 ad.name atvname,\r\n			 pi.status,\r\n			 pi.name formname,\r\n			 wi.sender,\r\n			 wi.stime,\r\n			 wi.mtime,\r\n			 wi.reCount\r\n	from cm_wfi_atv ai,\r\n			 cm_wfi_prc pi,\r\n			 cm_wfd_atv ad,\r\n			 cm_wfd_prc pd,\r\n			 (select id,\r\n							 atviid,\r\n							 mtime,\r\n							 sender,\r\n							 stime,\r\n							 (select count(1)\r\n									from cm_wfi_item\r\n								 where atviid = t.atviid\r\n									 and AssignKind = 4\r\n									 and id <> t.id) as reCount\r\n					from cm_wfi_item t\r\n				 where status = 1\r\n					 and userid = @userID\r\n					 and (@start < \'1900-01-01\' or mtime >= @start)\r\n					 and (@end < \'1900-01-01\' or mtime <= @end)) wi\r\n	where wi.atviid = ai.id\r\n	 and ai.prciid = pi.id\r\n	 and pi.prcdid = pd.id\r\n	 and ai.atvdid = ad.id\r\n	 and (@status > 2 or pi.status = @status)\r\n	order by wi.stime desc',NULL),('流程-日志目标项','select ( CASE username WHEN NULL THEN rolename ELSE username END ) accpname,\r\n			 atvdname,\r\n			 atvdtype,\r\n			 joinkind,\r\n			 atviid\r\n	from (select a.atviid,\r\n							 (select group_concat(name order by a.dispidx separator \'、\') from cm_user where id = a.userid) as username,\r\n							 (select group_concat(name order by a.dispidx separator \'、\') from cm_role where id = a.roleid) as rolename,\r\n							 max(a.dispidx) dispidx,\r\n							 c.name as atvdname,\r\n							 c.type as atvdtype,\r\n							 c.joinkind\r\n					from cm_wfi_item a,\r\n							 (select ti.TgtAtviID id\r\n									from cm_wfi_atv ai, cm_wfi_trs ti\r\n								 where ai.id = ti.SrcAtviID\r\n									 and ai.prciid = @prciid\r\n									 and ti.SrcAtviID = @atviid) b,\r\n							 cm_wfd_atv c,\r\n							 cm_wfi_atv d\r\n				 where a.atviid = b.id\r\n					 and b.id = d.id\r\n					 and d.atvdid = c.id\r\n				 group by a.atviid, c.name, c.type, c.joinkind) t\r\n order by dispidx',NULL),('流程-是否活动授权任何人','select\r\n	count(*) \r\nfrom\r\n	cm_wfd_atvrole \r\nwhere\r\n	roleid = 1 \r\n	and atvid = @atvid',NULL),('流程-最后工作项','select\r\n	wi.id itemid,\r\n	pi.PrcdID prcid \r\nfrom\r\n	cm_wfi_item wi,\r\n	cm_wfi_atv wa,\r\n	cm_wfi_prc pi \r\nwhere\r\n	wi.atviid = wa.id \r\n	and wa.PrciID = pi.id \r\n	and pi.id = @prciID \r\norder by\r\n	wi.mtime desc \r\n	LIMIT 0,\r\n	1',NULL),('流程-最后已完成活动ID','select\r\n	id \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	prciid = @prciid \r\n	and atvdid = @atvdid \r\n	and status = 1 \r\norder by\r\n	mtime desc',NULL),('流程-最近修改','select ID,Name,IsLocked,Singleton,Note,Dispidx,Ctime,Mtime from cm_wfd_prc WHERE to_days(now()) - to_days(mtime) <= 2',NULL),('流程-查找实例','select\r\n	id,\r\n	PrcdID,\r\n	name,\r\n	Status,\r\n	Ctime,\r\n	Mtime \r\nfrom\r\n	cm_wfi_prc \r\nwhere\r\n	PrcdID = @PrcdID \r\n	and ( @Status > 2 or `Status` = @Status ) \r\n	and ( @title = \'\' or name = @title ) \r\n	and ( @start < \'1900-01-01\' or Mtime >= @start ) \r\n	and ( @end < \'1900-01-01\' or Mtime <= @end ) \r\norder by\r\n	dispidx',NULL),('流程-模糊查询','select ID,Name,IsLocked,Singleton,Note,Dispidx,Ctime,Mtime from cm_wfd_prc WHERE NAME LIKE @input',NULL),('流程-活动前的迁移','select\r\n	* \r\nfrom\r\n	cm_wfd_trs \r\nwhere\r\n	TgtAtvID = @TgtAtvID',NULL),('流程-活动发送者','select\r\n	sender \r\nfrom\r\n	cm_wfi_item \r\nwhere\r\n	atviid = @atviid \r\norder by\r\n	mtime desc',NULL),('流程-活动实例状态','select\r\n	atvdid,\r\n	status \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	prciid = @prciid \r\norder by\r\n	ctime',NULL),('流程-活动实例的工作项','select\r\n	status,\r\n	AssignKind,\r\n	concat( sender, \' -> \', usr.name ) sendprc,\r\n	IsAccept,\r\n	wi.mtime \r\nfrom\r\n	cm_wfi_item wi\r\n	left join cm_user usr on wi.userid = usr.id \r\nwhere\r\n	atviid = @atviID \r\norder by\r\n	dispidx',NULL),('流程-活动实例的状态','select status \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	atvdid = @atvdid \r\n	and prciid = @prciid',NULL),('流程-活动未关联的角色','SELECT\r\n	a.id,\r\n	a.NAME\r\nFROM\r\n	cm_role a\r\nWHERE\r\n	NOT EXISTS ( SELECT roleid FROM cm_wfd_atvrole b WHERE a.id = b.roleid AND atvid = @atvid )',NULL),('流程-活动的所有执行者','select\r\n	id,\r\nname \r\nfrom\r\n	cm_user u \r\nwhere\r\n	exists (\r\nselect distinct\r\n	( userid ) \r\nfrom\r\n	cm_userrole ur \r\nwhere\r\n	exists ( select roleid from cm_wfd_atvrole ar where ur.roleid = ar.roleid and atvid = @atvid ) \r\n	and u.id = ur.userid \r\n	) \r\norder by\r\nname',NULL),('流程-活动的所有授权角色','select\r\n	id,\r\nname \r\nfrom\r\n	cm_role r \r\nwhere\r\n	exists ( select distinct ( roleid ) from cm_wfd_atvrole ar where r.id = ar.roleid and atvid = @atvid )',NULL),('流程-活动结束的实例数','select\r\n	count( * ) \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	atvdid = @atvdid \r\n	and prciid = @prciid \r\n	and status = 1',NULL),('流程-流程实例数','select count(*) from cm_wfi_prc where PrcdID=@PrcdID',NULL),('流程-流程实例的活动实例','select\r\n	atvi.id,\r\n	atvd.name,\r\n	status,\r\n	instcount \r\nfrom\r\n	cm_wfi_atv atvi,\r\n	cm_wfd_atv atvd \r\nwhere\r\n	atvi.atvdid = atvd.id \r\n	and atvi.prciid = @prciID \r\norder by\r\n	atvi.ctime',NULL),('流程-生成日志列表','select b.prciid,\r\n			 b.id atviid,\r\n			 c.status prcistatus,\r\n			 d.name atvdname,\r\n			 a.AssignKind,\r\n			 a.IsAccept,\r\n			 a.AcceptTime,\r\n			 a.status itemstatus,\r\n			 ( CASE userid WHEN NULL THEN (select name from cm_role t where t.id = a.roleid) ELSE (select name from cm_user t where t.id = a.userid) END ) username,\r\n			 a.note,\r\n			 a.ctime,\r\n			 a.mtime,\r\n			 c.mtime prcitime,\r\n			 a.sender\r\nfrom cm_wfi_item a, cm_wfi_atv b, cm_wfi_prc c, cm_wfd_atv d\r\nwhere a.atviid = b.id\r\n	 and b.prciid = c.id\r\n	 and b.atvdid = d.id\r\n	 and b.prciid = @prciid\r\n	 and (@atvdid = 0 or b.atvdid = @atvdid)\r\norder by a.dispidx',NULL),('流程-编辑活动授权','select\r\n	a.*,\r\n	b.name as role \r\nfrom\r\n	cm_wfd_atvrole a,\r\n	cm_role b \r\nwhere\r\n	a.roleid = b.id \r\n	and atvid in ( select id from cm_wfd_atv where prcid = @prcid )',NULL),('流程-编辑活动模板','select\r\n	a.*,\r\n	( CASE execscope WHEN 0 THEN \'一组用户\' WHEN 1 THEN \'所有用户\' WHEN 2 THEN \'单个用户\' WHEN 3 THEN \'任一用户\' END ) execscope_dsp,\r\n	( CASE execlimit WHEN 0 THEN \'无限制\' WHEN 1 THEN \'前一活动的执行者\' WHEN 2 THEN \'前一活动的同部门执行者\' WHEN 3 THEN \'已完成活动的执行者\' WHEN 4 THEN \'已完成活动的同部门执行者\' END ) execlimit_dsp,\r\n	( CASE JOINKIND WHEN 0 THEN \'全部任务\' WHEN 1 THEN \'任一任务\' WHEN 2 THEN \'即时同步\' END ) joinkind_dsp,\r\n	( CASE transkind WHEN 0 THEN \'自由选择\' WHEN 1 THEN \'全部\' WHEN 2 THEN \'独占式选择\' END ) transkind_dsp,\r\n	( select name from cm_wfd_atv where id = a.execatvid ) as execatvid_dsp \r\nfrom\r\n	cm_wfd_atv a \r\nwhere\r\n	prcid = @prcid',NULL),('流程-编辑流程模板','select * from cm_wfd_prc where id=@prcid',NULL),('流程-编辑迁移模板','select * from cm_wfd_trs where prcid=@prcid',NULL),('流程-获取用户ID','select id from cm_user where name = @name',NULL),('流程-起始活动','select * from cm_wfd_atv where prcid=@prcid and type=1',NULL),('流程-迁移模板ID','select\r\n	ID \r\nfrom\r\n	cm_wfd_trs \r\nwhere\r\n	prcid = @prcid \r\n	and SrcAtvID = @SrcAtvID \r\n	and TgtAtvID = @TgtAtvID \r\n	and IsRollback = @IsRollback',NULL),('流程-重复名称','select count(*) from cm_wfd_prc where name=@name',NULL),('用户-关联角色','SELECT\r\n	b.id roleid,\r\n	b.NAME rolename,\r\n	a.userid \r\nFROM\r\n	cm_userrole a,\r\n	cm_role b \r\nWHERE\r\n	a.roleid = b.id \r\n	AND userid = @userid',NULL),('用户-具有的权限','SELECT\r\n	prvid \r\nFROM\r\n	(\r\nSELECT DISTINCT\r\n	( a.prvid ) \r\nFROM\r\n	cm_roleprv a\r\n	LEFT JOIN cm_prv b ON a.prvid = b.id \r\nWHERE\r\n	EXISTS ( SELECT roleid FROM cm_userrole c WHERE a.roleid = c.roleid AND userid = @userid ) \r\n	or a.roleid = 1 \r\n	) t \r\nORDER BY\r\n	prvid',NULL),('用户-可访问的菜单','select id,name\r\n  from (select distinct (b.id), b.name, dispidx\r\n          from cm_rolemenu a\r\n          left join cm_menu b\r\n            on a.menuid = b.id\r\n         where exists\r\n         (select roleid\r\n                  from cm_userrole c\r\n                 where a.roleid = c.roleid\r\n                   and userid = @userid) or a.roleid=1) t\r\n order by dispidx',NULL),('用户-所有','SELECT\r\n	id,\r\n	phone,\r\n	name,\r\n	sex,\r\n	expired,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user',NULL),('用户-最近修改','SELECT\r\n	id,\r\n	phone,\r\n	NAME,\r\n	sex,\r\n	expired,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user \r\nWHERE\r\n	to_days(now()) - to_days(mtime) <= 2',NULL),('用户-未关联的角色','SELECT\r\n	a.id,\r\n	a.NAME\r\nFROM\r\n	cm_role a\r\nWHERE\r\n	NOT EXISTS ( SELECT roleid FROM cm_userrole b WHERE a.id = b.roleid AND userid = @userid )\r\n	AND a.id<>1',NULL),('用户-模糊查询','SELECT\r\n	id,\r\n	phone,\r\n	NAME,\r\n	sex,\r\n	expired,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user \r\nWHERE\r\n	NAME LIKE @input \r\n	OR phone LIKE @input',NULL),('用户-编辑','SELECT\r\n	id,\r\n	phone,\r\n	name,\r\n	sex,\r\n	photo,\r\n	ctime,\r\n	mtime \r\nFROM\r\n	cm_user \r\nWHERE\r\n	id = @id',NULL),('用户-角色列表的用户','SELECT DISTINCT(userid) FROM cm_userrole where FIND_IN_SET(roleid, @roleid)',NULL),('用户-重复手机号','select count(id) from cm_user where phone=@phone',NULL),('登录-手机号获取用户','select * from cm_user where phone=@phone',NULL),('菜单-id菜单项','SELECT\r\n	a.*,\r\n	b.NAME parentname \r\nFROM\r\n	cm_menu a\r\n	LEFT JOIN cm_menu b ON a.parentid = b.id \r\nWHERE\r\n	a.id = @id',NULL),('菜单-关联的角色','SELECT\r\n	b.id roleid,\r\n	b.NAME rolename,\r\n	a.menuid \r\nFROM\r\n	cm_rolemenu a,\r\n	cm_role b \r\nWHERE\r\n	a.roleid = b.id \r\n	AND menuid = @menuid',NULL),('菜单-分组树','SELECT\r\n	id,\r\n	NAME,\r\n	parentid \r\nFROM\r\n	cm_menu \r\nWHERE\r\n	isgroup = 1 \r\nORDER BY\r\n	dispidx',NULL),('菜单-完整树','SELECT\r\n	id,\r\n	NAME,\r\n	parentid,\r\n	isgroup,\r\n	icon,\r\n	dispidx\r\nFROM\r\n	cm_menu \r\nORDER BY\r\n	dispidx',NULL),('菜单-是否有子菜单','select count(*) from cm_menu where parentid=@parentid',NULL),('菜单-未关联的角色','SELECT\r\n	a.id,\r\n	a.NAME\r\nFROM\r\n	cm_role a\r\nWHERE\r\n	NOT EXISTS ( SELECT roleid FROM cm_rolemenu b WHERE a.id = b.roleid AND menuid = @menuid )',NULL),('角色-关联用户','SELECT\r\n	b.id userid,\r\n	b.NAME username,\r\n	a.roleid \r\nFROM\r\n	cm_userrole a,\r\n	cm_user b \r\nWHERE\r\n	a.userid = b.id\r\n	AND roleid = @roleid',NULL),('角色-关联的权限','select a.prvid, a.roleid\r\n  from cm_roleprv a\r\n  join cm_prv b\r\n    on a.prvid = b.id\r\n where a.roleid = @roleid',NULL),('角色-关联的菜单','select id as menuid, name, a.roleid\r\n  from cm_rolemenu a\r\n  join cm_menu b\r\n    on a.menuid = b.id\r\n where b.isgroup = 0\r\n   and a.roleid = @roleid\r\n order by dispidx',NULL),('角色-名称重复','select count(id) from cm_role where name=@name',NULL),('角色-所有','select * from cm_role',NULL),('角色-未关联的权限','select a.id, a.note\r\n  from cm_prv a\r\n where not exists\r\n (select prvid\r\n          from cm_roleprv b\r\n         where a.id = b.prvid\r\n           and b.roleid = @roleid)',NULL),('角色-未关联的用户','select id, name\r\n  from cm_user a\r\n where not exists (select userid\r\n          from cm_userrole b\r\n         where a.id = b.userid\r\n           and roleid = @roleid)\r\n order by name',NULL),('角色-未关联的菜单','select id, name\r\n  from cm_menu a\r\n where isgroup = 0\r\n   and not exists (select menuid\r\n          from cm_rolemenu b\r\n         where a.id = b.menuid\r\n           and roleid = @roleid)\r\n order by dispidx',NULL),('角色-模糊查询','select * from cm_role where name like @name',NULL),('角色-系统角色','select * from cm_role where id < 1000',NULL),('角色-编辑','SELECT\r\n	id,\r\n	name,\r\n	note\r\nFROM\r\n	cm_role\r\nWHERE\r\n	id = @id',NULL),('选项-分类选项','SELECT * FROM cm_option where Category=@Category order by Dispidx',NULL),('选项-所有分类','SELECT DISTINCT(Category) as name FROM cm_option order by Dispidx',NULL),('选项-选项','SELECT * FROM cm_option where Category=@Category and Name=@Name',NULL);
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
  `Sex` tinyint(4) unsigned NOT NULL DEFAULT '1' COMMENT '#Gender#性别',
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
INSERT INTO `cm_user` VALUES (1,'15948371897','daoting','af3303f852abeccd793068486a391626',1,'[[\"v0/E3/18/63458646655102976.jpg\",\"未标题-2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-24 10:42\"]]',0,'2019-10-24 09:06:38','2021-09-10 11:26:55'),(8411237852585984,'15911111111','安卓','b59c67bf196a4758191e42f76670ceba',2,'[[\"v0/7E/55/81569009808306176.jpg\",\"75832742034403328\",\"300 x 300 (jpg)\",49179,\"安卓\",\"2020-05-13 02:06\"]]',0,'2019-10-24 13:03:19','2020-05-13 10:06:08'),(52998151791833088,'13312345678','苹果','674f3c2c1a8a6f90461e8a66fb5550ba',1,'[[\"v0/E9/B4/76177735114682368.jpg\",\"IMG_0002\",\"4288 x 2848 (jpg)\",2604768,\"苹果\",\"2020-04-28 13:03\"]]',0,'0001-01-01 00:00:00','2020-04-28 13:03:11'),(149709966847897600,'13122222222','李市场','934b535800b1cba8f96a5d72f72f1611',1,'',0,'2020-11-17 10:54:29','2020-11-25 16:37:55'),(152695627289198592,'13211111111','王综合','b59c67bf196a4758191e42f76670ceba',1,'',0,'2020-11-25 16:38:34','2020-11-25 16:38:34'),(152695790787362816,'13866666666','张财务','e9510081ac30ffa83f10b68cde1cac07',1,'',0,'2020-11-25 16:38:54','2020-11-25 16:38:54'),(184215437633777664,'15955555555','15955555555','6074c6aa3488f3c2dddff2a7ca821aab',1,'',0,'2021-02-20 16:06:23','2021-02-20 16:06:23'),(185188338092601344,'15912345678','15912345678','674f3c2c1a8a6f90461e8a66fb5550ba',1,'',0,'2021-02-23 08:32:20','2021-02-23 08:32:20'),(185212597401677824,'15912345671','15912345677','cca8f108b55ec9e39d7885e24f7da0af',2,'',0,'2021-02-23 10:08:43','2022-01-19 15:49:43'),(192818293676994560,'18543175028','18543175028','bf8dd8c68d02e161c28dc9ea139d4784',1,'',0,'2021-03-16 09:51:02','2021-03-16 09:51:02'),(196167762048839680,'18843175028','18843175028','bf8dd8c68d02e161c28dc9ea139d4784',1,'',0,'2021-03-25 15:40:38','2021-03-25 15:40:38'),(224062063923556352,'14411111111','14411111111','b59c67bf196a4758191e42f76670ceba',1,'',0,'2021-06-10 15:02:39','2021-06-10 15:02:39'),(227949556179791872,'13612345678','WebAssembly','674f3c2c1a8a6f90461e8a66fb5550ba',1,'',0,'2021-06-21 08:30:10','2021-06-21 08:30:34'),(229519641138819072,'13311111111','13311111111','b59c67bf196a4758191e42f76670ceba',1,'',0,'2021-06-25 16:29:06','2021-06-25 16:29:06'),(231620526086156288,'13611111111','13611111111','b59c67bf196a4758191e42f76670ceba',1,'',0,'2021-07-01 11:37:18','2021-07-01 11:37:18'),(237124390994440192,'13332323232','adasd','12e086066892a311b752673a28583d3f',1,'',0,'2021-07-16 16:08:10','2021-07-16 16:08:10'),(238206296410419200,'13312345432','新用户111','2e92962c0b6996add9517e4242ea9bdc',1,'[[\"v0/E3/0D/142914111109197824.png\",\"Icon-20@2x\",\"40 x 40 (.png)\",436,\"daoting\",\"2020-10-29 16:49\"]]',0,'2021-07-19 15:47:35','2021-09-10 09:20:45'),(247170018466197504,'15948341897','15948341892','af3303f852abeccd793068486a391626',1,'',0,'2021-08-13 09:25:26','2021-09-10 09:36:37');
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
  `Mtime` datetime NOT NULL COMMENT '修改时间',
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
INSERT INTO `cm_userparams` VALUES (8411237852585984,'接收新任务','false','2020-12-04 13:29:05');
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
INSERT INTO `cm_userrole` VALUES (1,2),(1,22844822693027840),(1,152695933758603264),(8411237852585984,2),(8411237852585984,22844822693027840),(52998151791833088,2),(149709966847897600,2),(149709966847897600,152695933758603264),(152695627289198592,152696004814307328),(152695790787362816,152696042718232576),(238206296410419200,22844822693027840),(247170018466197504,22844822693027840);
/*!40000 ALTER TABLE `cm_userrole` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfd_atv`
--

DROP TABLE IF EXISTS `cm_wfd_atv`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfd_atv` (
  `ID` bigint(20) NOT NULL COMMENT '活动标识',
  `PrcID` bigint(20) NOT NULL COMMENT '流程标识',
  `Name` varchar(64) NOT NULL COMMENT '活动名称，同时作为状态名称',
  `Type` tinyint(4) unsigned NOT NULL COMMENT '#WfdAtvType#活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动',
  `ExecScope` tinyint(4) unsigned NOT NULL COMMENT '#WfdAtvExecScope#执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户',
  `ExecLimit` tinyint(4) unsigned NOT NULL COMMENT '#WfdAtvExecLimit#执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者',
  `ExecAtvID` bigint(20) DEFAULT NULL COMMENT '在执行者限制为3或4时选择的活动',
  `AutoAccept` tinyint(1) NOT NULL COMMENT '是否自动签收，打开工作流视图时自动签收工作项',
  `CanDelete` tinyint(1) NOT NULL COMMENT '能否删除流程实例和业务数据，0否 1',
  `CanTerminate` tinyint(1) NOT NULL COMMENT '能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能',
  `CanJumpInto` tinyint(1) NOT NULL COMMENT '是否可作为跳转目标，0不可跳转 1可以',
  `TransKind` tinyint(4) unsigned NOT NULL COMMENT '#WfdAtvTransKind#当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择',
  `JoinKind` tinyint(4) unsigned NOT NULL COMMENT '#WfdAtvJoinKind#同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`),
  KEY `fk_wfdatv_prcid` (`PrcID`),
  CONSTRAINT `fk_wfdatv_prcid` FOREIGN KEY (`PrcID`) REFERENCES `cm_wfd_prc` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='活动模板';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfd_atv`
--

LOCK TABLES `cm_wfd_atv` WRITE;
/*!40000 ALTER TABLE `cm_wfd_atv` DISABLE KEYS */;
INSERT INTO `cm_wfd_atv` VALUES (146898715155492864,146898695127691264,'开始',1,0,0,NULL,1,1,0,0,0,0,'2020-11-09 16:43:10','2020-11-09 16:43:10'),(146898876447453184,146898695127691264,'任务项',0,0,0,NULL,1,0,0,0,0,0,'2020-11-09 16:43:48','2020-11-09 16:43:48'),(146900570585559040,146900552231284736,'开始',1,0,0,NULL,1,1,0,0,0,0,'2020-11-09 16:50:32','2020-11-09 16:50:32'),(146900847761944576,146900823984435200,'开始',1,0,0,NULL,1,1,0,0,0,0,'2020-11-09 16:51:38','2020-11-09 16:51:38'),(146901433265811456,146901403339452416,'开始',1,0,0,NULL,1,1,0,0,0,0,'2020-11-09 16:53:58','2020-11-09 16:53:58'),(147141181158846464,147141147767992320,'开始',1,0,0,NULL,1,1,0,0,0,0,'2020-11-10 08:46:31','2020-11-10 08:46:31'),(147141718000398336,147141147767992320,'任务项',0,0,0,NULL,1,0,0,0,0,0,'2020-11-10 08:48:39','2020-11-10 08:48:39'),(152588671081775104,152588581545967616,'接收文件',1,0,0,NULL,1,1,0,0,0,0,'2020-11-25 09:32:55','2020-12-09 10:45:33'),(152683112727576576,152588581545967616,'市场部',0,0,0,NULL,1,0,0,0,2,0,'2020-11-25 15:48:12','2020-12-14 15:36:36'),(152684512937246720,152588581545967616,'综合部',0,2,0,NULL,1,0,0,0,2,0,'2020-11-25 15:53:46','2020-12-14 15:33:30'),(152684758027206656,152588581545967616,'市场部传阅',0,0,0,NULL,1,0,0,0,0,0,'2020-11-25 15:54:44','2020-11-25 15:56:10'),(152684895835258880,152588581545967616,'同步',2,0,0,NULL,1,0,0,0,0,2,'2020-11-25 15:55:17','2020-12-16 08:39:31'),(152685032993193984,152588581545967616,'综合部传阅',0,0,0,NULL,1,0,0,0,0,0,'2020-11-25 15:55:50','2020-11-25 15:56:10'),(152685491275431936,152588581545967616,'返回收文人',0,0,0,NULL,1,0,0,0,0,0,'2020-11-25 15:57:39','2020-11-25 15:58:18'),(152685608543977472,152588581545967616,'完成',3,0,0,NULL,1,0,0,0,0,0,'2020-11-25 15:58:07','2020-11-25 15:58:07');
/*!40000 ALTER TABLE `cm_wfd_atv` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfd_atvrole`
--

DROP TABLE IF EXISTS `cm_wfd_atvrole`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfd_atvrole` (
  `AtvID` bigint(20) NOT NULL COMMENT '活动标识',
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`AtvID`,`RoleID`),
  KEY `fk_wfdatvrole_roleid` (`RoleID`),
  CONSTRAINT `fk_wfdatvrole_atvid` FOREIGN KEY (`AtvID`) REFERENCES `cm_wfd_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfdatvrole_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='活动授权';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfd_atvrole`
--

LOCK TABLES `cm_wfd_atvrole` WRITE;
/*!40000 ALTER TABLE `cm_wfd_atvrole` DISABLE KEYS */;
INSERT INTO `cm_wfd_atvrole` VALUES (146898715155492864,1),(146900570585559040,1),(146900847761944576,1),(146901433265811456,1),(146898715155492864,2),(146900570585559040,2),(146901433265811456,2),(152588671081775104,22844822693027840),(152684758027206656,22844822693027840),(152685032993193984,22844822693027840),(152685491275431936,22844822693027840),(152683112727576576,152695933758603264),(152684512937246720,152696004814307328);
/*!40000 ALTER TABLE `cm_wfd_atvrole` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfd_prc`
--

DROP TABLE IF EXISTS `cm_wfd_prc`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfd_prc` (
  `ID` bigint(20) NOT NULL COMMENT '流程标识',
  `Name` varchar(64) NOT NULL COMMENT '流程名称',
  `Diagram` varchar(21000) DEFAULT NULL COMMENT '流程图',
  `IsLocked` tinyint(1) NOT NULL COMMENT '锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行',
  `Singleton` tinyint(1) NOT NULL COMMENT '同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例',
  `FormType` varchar(255) NOT NULL COMMENT '表单类型',
  `ListType` varchar(255) NOT NULL COMMENT '表单查询类型',
  `Note` varchar(255) DEFAULT NULL COMMENT '描述',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后修改时间',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='流程模板';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfd_prc`
--

LOCK TABLES `cm_wfd_prc` WRITE;
/*!40000 ALTER TABLE `cm_wfd_prc` DISABLE KEYS */;
INSERT INTO `cm_wfd_prc` VALUES (146898695127691264,'555','<Sketch><Node id=\"146898715155492864\" title=\"开始\" shape=\"开始\" left=\"340\" top=\"100\" width=\"80\" height=\"60\" /><Node id=\"146898876447453184\" title=\"任务项\" shape=\"任务\" left=\"340\" top=\"360\" width=\"120\" height=\"60\" /><Line id=\"146898896794021888\" headerid=\"146898715155492864\" bounds=\"380,160,30,200\" headerport=\"4\" tailid=\"146898876447453184\" tailport=\"0\" /></Sketch>',0,0,'Dt.App.Workflow.WfDemoForm,Dt.App','','',1,'0001-01-01 00:00:00','2020-11-19 13:17:25'),(146900552231284736,'666','<Sketch><Node id=\"146900570585559040\" title=\"开始\" shape=\"开始\" left=\"620\" top=\"120\" width=\"80\" height=\"60\" /></Sketch>',0,0,'','','',3,'0001-01-01 00:00:00','2020-11-09 16:50:56'),(146900823984435200,'777','<Sketch><Node id=\"146900847761944576\" title=\"开始\" shape=\"开始\" left=\"300\" top=\"220\" width=\"80\" height=\"60\" /></Sketch>',0,0,'','','',4,'0001-01-01 00:00:00','2020-11-09 16:52:58'),(146901403339452416,'888','<Sketch><Node id=\"146901433265811456\" title=\"开始\" shape=\"开始\" left=\"340\" top=\"140\" width=\"80\" height=\"60\" /></Sketch>',0,0,'','','',6,'0001-01-01 00:00:00','2020-11-09 16:54:39'),(147141147767992320,'ggg','<Sketch><Node id=\"147141181158846464\" title=\"开始\" shape=\"开始\" left=\"320\" top=\"40\" width=\"80\" height=\"60\" /><Node id=\"147141718000398336\" title=\"任务项\" shape=\"任务\" left=\"380\" top=\"480\" width=\"120\" height=\"60\" /><Line id=\"147141749642227712\" headerid=\"147141181158846464\" bounds=\"400,100,50,380\" headerport=\"3\" tailid=\"147141718000398336\" tailport=\"0\" /></Sketch>',1,0,'','','',2,'2020-11-10 08:46:24','2020-11-10 08:50:03'),(152588581545967616,'收文样例','<Sketch><Node id=\"152588671081775104\" title=\"接收文件\" shape=\"开始\" left=\"300\" top=\"40\" width=\"80\" height=\"60\" /><Node id=\"152683112727576576\" title=\"市场部\" shape=\"任务\" left=\"160\" top=\"140\" width=\"120\" height=\"60\" /><Line id=\"152683122982649856\" headerid=\"152588671081775104\" bounds=\"210,70,50,70\" headerport=\"6\" tailid=\"152683112727576576\" tailport=\"0\" /><Node id=\"152684512937246720\" title=\"综合部\" shape=\"任务\" left=\"400\" top=\"140\" width=\"120\" height=\"60\" /><Line id=\"152684673721696256\" headerid=\"152588671081775104\" bounds=\"380,70,90,70\" headerport=\"2\" tailid=\"152684512937246720\" tailport=\"0\" /><Node id=\"152684758027206656\" title=\"市场部传阅\" shape=\"任务\" left=\"160\" top=\"260\" width=\"120\" height=\"60\" /><Node id=\"152684895835258880\" title=\"同步\" shape=\"同步\" background=\"#FF9D9D9D\" borderbrush=\"#FF969696\" left=\"280\" top=\"400\" width=\"120\" height=\"60\" /><Line id=\"152684951493672960\" headerid=\"152683112727576576\" bounds=\"210,200,20,60\" headerport=\"4\" tailid=\"152684758027206656\" tailport=\"0\" /><Line id=\"152684981348728832\" headerid=\"152683112727576576\" bounds=\"120,170,160,470\" headerport=\"6\" tailid=\"152685608543977472\" tailport=\"6\" /><Node id=\"152685032993193984\" title=\"综合部传阅\" shape=\"任务\" left=\"400\" top=\"260\" width=\"120\" height=\"60\" /><Line id=\"152685133509689344\" headerid=\"152684512937246720\" bounds=\"450,200,20,60\" headerport=\"4\" tailid=\"152685032993193984\" tailport=\"0\" /><Line id=\"152685169891082240\" headerid=\"152684512937246720\" bounds=\"400,170,160,270\" headerport=\"2\" tailid=\"152684895835258880\" tailport=\"2\" /><Line id=\"152685211767013376\" headerid=\"152684758027206656\" bounds=\"220,320,60,120\" headerport=\"4\" tailid=\"152684895835258880\" tailport=\"6\" /><Line id=\"152685247745753088\" headerid=\"152685032993193984\" bounds=\"400,320,60,120\" headerport=\"4\" tailid=\"152684895835258880\" tailport=\"2\" /><Node id=\"152685491275431936\" title=\"返回收文人\" shape=\"任务\" left=\"280\" top=\"500\" width=\"120\" height=\"60\" /><Line id=\"152685585135566848\" headerid=\"152684895835258880\" bounds=\"330,460,20,40\" headerport=\"4\" tailid=\"152685491275431936\" tailport=\"0\" /><Node id=\"152685608543977472\" title=\"完成\" shape=\"结束\" background=\"#FF9D9D9D\" borderbrush=\"#FF969696\" left=\"300\" top=\"600\" width=\"80\" height=\"60\" /><Line id=\"152685622099968000\" headerid=\"152685491275431936\" bounds=\"330,560,20,40\" headerport=\"4\" tailid=\"152685608543977472\" tailport=\"0\" /></Sketch>',0,0,'Dt.Sample.收文表单,Dt.Sample','Dt.Sample.收文查询,Dt.Sample','',5,'2020-11-25 09:32:33','2021-08-24 15:45:54');
/*!40000 ALTER TABLE `cm_wfd_prc` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfd_trs`
--

DROP TABLE IF EXISTS `cm_wfd_trs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfd_trs` (
  `ID` bigint(20) NOT NULL COMMENT '迁移标识',
  `PrcID` bigint(20) NOT NULL COMMENT '流程模板标识',
  `SrcAtvID` bigint(20) NOT NULL COMMENT '起始活动模板标识',
  `TgtAtvID` bigint(20) NOT NULL COMMENT '目标活动模板标识',
  `IsRollback` tinyint(1) NOT NULL COMMENT '是否为回退迁移',
  `TrsID` bigint(20) DEFAULT NULL COMMENT '类别为回退迁移时对应的常规迁移标识',
  PRIMARY KEY (`ID`),
  KEY `fk_wfdtrs_prcid` (`PrcID`),
  CONSTRAINT `fk_wfdtrs_prcid` FOREIGN KEY (`PrcID`) REFERENCES `cm_wfd_prc` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='迁移模板';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfd_trs`
--

LOCK TABLES `cm_wfd_trs` WRITE;
/*!40000 ALTER TABLE `cm_wfd_trs` DISABLE KEYS */;
INSERT INTO `cm_wfd_trs` VALUES (146898896794021888,146898695127691264,146898715155492864,146898876447453184,0,NULL),(147141749642227712,147141147767992320,147141181158846464,147141718000398336,0,NULL),(152683122982649856,152588581545967616,152588671081775104,152683112727576576,0,NULL),(152684673721696256,152588581545967616,152588671081775104,152684512937246720,0,NULL),(152684951493672960,152588581545967616,152683112727576576,152684758027206656,0,NULL),(152684981348728832,152588581545967616,152683112727576576,152685608543977472,0,NULL),(152685133509689344,152588581545967616,152684512937246720,152685032993193984,0,NULL),(152685169891082240,152588581545967616,152684512937246720,152684895835258880,0,NULL),(152685211767013376,152588581545967616,152684758027206656,152684895835258880,0,NULL),(152685247745753088,152588581545967616,152685032993193984,152684895835258880,0,NULL),(152685585135566848,152588581545967616,152684895835258880,152685491275431936,0,NULL),(152685622099968000,152588581545967616,152685491275431936,152685608543977472,0,NULL),(160910207789953024,152588581545967616,152683112727576576,152588671081775104,1,152683122982649856);
/*!40000 ALTER TABLE `cm_wfd_trs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfi_atv`
--

DROP TABLE IF EXISTS `cm_wfi_atv`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfi_atv` (
  `ID` bigint(20) NOT NULL COMMENT '活动实例标识',
  `PrciID` bigint(20) NOT NULL COMMENT '流程实例标识',
  `AtvdID` bigint(20) NOT NULL COMMENT '活动模板标识',
  `Status` tinyint(4) unsigned NOT NULL COMMENT '#WfiAtvStatus#活动实例的状态 0活动 1结束 2终止 3同步活动',
  `InstCount` int(11) NOT NULL COMMENT '活动实例在流程实例被实例化的次数',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`ID`),
  KEY `fk_wfiatv_prciid` (`PrciID`),
  KEY `fk_wfiatv_atvdid` (`AtvdID`),
  CONSTRAINT `fk_wfiatv_atvdid` FOREIGN KEY (`AtvdID`) REFERENCES `cm_wfd_atv` (`ID`),
  CONSTRAINT `fk_wfiatv_prciid` FOREIGN KEY (`PrciID`) REFERENCES `cm_wfi_prc` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='活动实例';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfi_atv`
--

LOCK TABLES `cm_wfi_atv` WRITE;
/*!40000 ALTER TABLE `cm_wfi_atv` DISABLE KEYS */;
INSERT INTO `cm_wfi_atv` VALUES (162025231375790080,162025231350624256,152588671081775104,1,1,'2020-12-21 10:30:29','2020-12-21 10:30:31'),(162025255044247552,162025231350624256,152683112727576576,1,1,'2020-12-21 10:30:31','2020-12-21 16:45:05'),(162119526644576256,162025231350624256,152684758027206656,1,1,'2020-12-21 16:45:05','2020-12-21 16:45:11'),(162119548043915264,162025231350624256,152684895835258880,3,1,'2020-12-21 16:45:11','2020-12-21 16:45:11'),(162119548199104512,162025231350624256,152685491275431936,1,1,'2020-12-21 16:45:11','2020-12-21 16:45:13'),(162401333625614336,162401333600448512,152588671081775104,0,1,'2020-12-22 11:25:22','2020-12-22 11:25:22');
/*!40000 ALTER TABLE `cm_wfi_atv` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfi_item`
--

DROP TABLE IF EXISTS `cm_wfi_item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfi_item` (
  `ID` bigint(20) NOT NULL COMMENT '工作项标识',
  `AtviID` bigint(20) NOT NULL COMMENT '活动实例标识',
  `Status` tinyint(4) unsigned NOT NULL COMMENT '#WfiItemStatus#工作项状态 0活动 1结束 2终止 3同步活动',
  `AssignKind` tinyint(4) unsigned NOT NULL COMMENT '#WfiItemAssignKind#指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派',
  `Sender` varchar(32) NOT NULL COMMENT '发送者',
  `Stime` datetime NOT NULL COMMENT '发送时间',
  `IsAccept` tinyint(1) NOT NULL COMMENT '是否签收此项任务',
  `AcceptTime` datetime DEFAULT NULL COMMENT '签收时间',
  `RoleID` bigint(20) DEFAULT NULL COMMENT '执行者角色标识',
  `UserID` bigint(20) DEFAULT NULL COMMENT '执行者用户标识',
  `Note` varchar(255) DEFAULT NULL COMMENT '工作项备注',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`ID`),
  KEY `fk_wfiitem_atviid` (`AtviID`),
  CONSTRAINT `fk_wfiitem_atviid` FOREIGN KEY (`AtviID`) REFERENCES `cm_wfi_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='工作项';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfi_item`
--

LOCK TABLES `cm_wfi_item` WRITE;
/*!40000 ALTER TABLE `cm_wfi_item` DISABLE KEYS */;
INSERT INTO `cm_wfi_item` VALUES (162025231392567296,162025231375790080,1,1,'daoting','2020-12-21 10:30:29',1,'2020-12-21 10:30:29',NULL,1,'',157,'2020-12-21 10:30:29','2020-12-21 10:30:31'),(162025255065219072,162025255044247552,1,0,'daoting','2020-12-21 10:30:31',1,'2020-12-21 13:27:15',NULL,1,'',158,'2020-12-21 10:30:31','2020-12-21 16:45:05'),(162119526686519296,162119526644576256,1,0,'daoting','2020-12-21 16:45:05',1,'2020-12-21 16:45:07',NULL,1,'',159,'2020-12-21 16:45:05','2020-12-21 16:45:11'),(162119548064886784,162119548043915264,3,0,'daoting','2020-12-21 16:45:11',0,NULL,NULL,1,'',160,'2020-12-21 16:45:11','2020-12-21 16:45:11'),(162119548220076032,162119548199104512,1,0,'daoting','2020-12-21 16:45:11',1,'2020-12-21 16:45:12',NULL,1,'',161,'2020-12-21 16:45:11','2020-12-21 16:45:13'),(162401333642391552,162401333625614336,0,1,'daoting','2020-12-22 11:25:22',1,'2020-12-22 11:25:22',NULL,1,'',162,'2020-12-22 11:25:22','2020-12-22 11:25:22');
/*!40000 ALTER TABLE `cm_wfi_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfi_prc`
--

DROP TABLE IF EXISTS `cm_wfi_prc`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfi_prc` (
  `ID` bigint(20) NOT NULL COMMENT '流程实例标识，同时为业务数据主键',
  `PrcdID` bigint(20) NOT NULL COMMENT '流程模板标识',
  `Name` varchar(255) NOT NULL COMMENT '流转单名称',
  `Status` tinyint(4) unsigned NOT NULL COMMENT '#WfiPrcStatus#流程实例状态 0活动 1结束 2终止',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`ID`),
  KEY `fk_wfiprc_prcdid` (`PrcdID`),
  CONSTRAINT `fk_wfiprc_prcdid` FOREIGN KEY (`PrcdID`) REFERENCES `cm_wfd_prc` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='流程实例';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfi_prc`
--

LOCK TABLES `cm_wfi_prc` WRITE;
/*!40000 ALTER TABLE `cm_wfi_prc` DISABLE KEYS */;
INSERT INTO `cm_wfi_prc` VALUES (162025231350624256,152588581545967616,'a',1,58,'2020-12-21 10:30:29','2020-12-21 16:45:13'),(162401333600448512,152588581545967616,'关于新冠疫情的批示',0,59,'2020-12-22 11:25:22','2020-12-22 11:25:22');
/*!40000 ALTER TABLE `cm_wfi_prc` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfi_trs`
--

DROP TABLE IF EXISTS `cm_wfi_trs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfi_trs` (
  `ID` bigint(20) NOT NULL COMMENT '迁移实例标识',
  `TrsdID` bigint(20) NOT NULL COMMENT '迁移模板标识',
  `SrcAtviID` bigint(20) NOT NULL COMMENT '起始活动实例标识',
  `TgtAtviID` bigint(20) NOT NULL COMMENT '目标活动实例标识',
  `IsRollback` tinyint(1) NOT NULL COMMENT '是否为回退迁移，1表回退',
  `Ctime` datetime NOT NULL COMMENT '迁移时间',
  PRIMARY KEY (`ID`),
  KEY `fk_wfitrs_trsdid` (`TrsdID`),
  KEY `fk_wfitrs_srcatviid` (`SrcAtviID`),
  KEY `fk_wfitrs_tgtatviid` (`TgtAtviID`),
  CONSTRAINT `fk_wfitrs_srcatviid` FOREIGN KEY (`SrcAtviID`) REFERENCES `cm_wfi_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfitrs_tgtatviid` FOREIGN KEY (`TgtAtviID`) REFERENCES `cm_wfi_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfitrs_trsdid` FOREIGN KEY (`TrsdID`) REFERENCES `cm_wfd_trs` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='迁移实例';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfi_trs`
--

LOCK TABLES `cm_wfi_trs` WRITE;
/*!40000 ALTER TABLE `cm_wfi_trs` DISABLE KEYS */;
INSERT INTO `cm_wfi_trs` VALUES (162025255165882368,152683122982649856,162025231375790080,162025255044247552,0,'2020-12-21 10:30:31'),(162119526820737024,152684951493672960,162025255044247552,162119526644576256,0,'2020-12-21 16:45:05'),(162119548186521600,152685211767013376,162119526644576256,162119548043915264,0,'2020-12-21 16:45:11'),(162119548320739328,152685585135566848,162119548043915264,162119548199104512,0,'2020-12-21 16:45:11');
/*!40000 ALTER TABLE `cm_wfi_trs` ENABLE KEYS */;
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
INSERT INTO `fsm_file` VALUES (59189634018439168,'1.jpg','photo/CC/D2/59189634018439168.jpg',40589,'334 x 297 (.jpg)',1,'2020-03-12 15:58:28',1),(59190827587334144,'IMG_20200228_073347.jpg','photo/40/05/59190827587334144.jpg',200090,'960 x 1280 (jpg)',8411237852585984,'2020-03-12 16:03:13',1),(59435697681854464,'MySql_bs0_202001211220.sql','photo/30/D1/59435697681854464.sql',43717,'sql文件',1,'2020-03-13 08:16:15',1),(59471299324276736,'1.jpg','photo/CC/D2/59471299324276736.jpg',49179,'300 x 300 (.jpg)',1,'2020-03-13 10:37:43',0),(59471299760484352,'Logon.wav','photo/AD/FF/59471299760484352.wav',384496,'00:04',1,'2020-03-13 10:37:43',4),(59471299831787520,'mov.mp4','photo/CB/D1/59471299831787520.mp4',788493,'00:00:10 (320 x 176)',1,'2020-03-13 10:37:43',3),(59471299907284992,'profilephoto.jpg','photo/08/64/59471299907284992.jpg',17891,'300 x 300 (.jpg)',1,'2020-03-13 10:37:43',3),(59471299978588160,'苍蝇.wmv','photo/D4/6B/59471299978588160.wmv',403671,'00:00:06 (480 x 288)',1,'2020-03-13 10:37:43',3),(59471300041502720,'文本文档.txt','photo/DB/D6/59471300041502720.txt',8,'txt文件',1,'2020-03-13 10:37:43',1),(59471300070862848,'项目文档.docx','photo/5D/26/59471300070862848.docx',13071,'docx文件',1,'2020-03-13 10:37:43',1),(62011895247138816,'无标题1.png','v0/D3/43/62011895247138816.png',24425,'401 x 665 (.png)',1,'2020-03-20 10:53:10',1),(62013122181722112,'未标题-2.jpg','v0/E3/18/62013122181722112.jpg',49179,'300 x 300 (.jpg)',1,'2020-03-20 10:58:02',0),(62101043526103040,'IMG_20200228_073347.jpg','v0/40/05/62101043526103040.jpg',200090,'960 x 1280 (jpg)',8411237852585984,'2020-03-20 16:47:25',0),(63446669690007552,'1.jpg','v0/CC/D2/63446669690007552.jpg',49179,'300 x 300 (.jpg)',1,'2020-03-24 09:54:25',0),(63454870955225088,'未标题-2.jpg','v0/E3/18/63454870955225088.jpg',49179,'300 x 300 (.jpg)',1,'2020-03-24 10:27:01',0),(63458646655102976,'未标题-2.jpg','v0/E3/18/63458646655102976.jpg',49179,'300 x 300 (.jpg)',1,'2020-03-24 10:42:01',136),(66364004327354368,'mov.mp4','v1/CB/D1/66364004327354368.mp4',788493,'00:00:10 (320 x 176)',8411237852585984,'2020-04-01 11:06:53',0),(66364069729136640,'mov.mp4','v1/CB/D1/66364069729136640.mp4',788493,'00:00:10 (320 x 176)',8411237852585984,'2020-04-01 11:07:08',0),(66364122896134144,'mov.mp4','v1/CB/D1/66364122896134144.mp4',788493,'00:00:10 (320 x 176)',8411237852585984,'2020-04-01 11:07:21',0),(66367788520697856,'Docker for Windows Installer.exe','v1/88/6D/66367788520697856.exe',567050280,'exe文件',1,'2020-04-01 11:22:09',0),(66738006560468992,'1.jpg','chat/CC/D2/66738006560468992.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-02 11:53:02',0),(66738149242302464,'mono-wasm-f5cfc67c8ed.zip','chat/AE/40/66738149242302464.zip',40418077,'zip文件',1,'2020-04-02 11:53:37',0),(66739208513777664,'1.jpg','chat/CC/D2/66739208513777664.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-02 11:57:49',1),(66739283596013568,'Bs.Kehu.Droid.apk','chat/04/36/66739283596013568.apk',70594488,'apk文件',1,'2020-04-02 11:58:08',0),(66766700469415936,'abc.jpg','chat/DD/5D/66766700469415936.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-02 13:47:03',0),(67164400922783744,'abc.jpg','chat/DD/5D/67164400922783744.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-03 16:07:20',0),(67166199503253504,'icon.txt','chat/E3/A6/67166199503253504.txt',1215,'txt文件',1,'2020-04-03 16:14:29',0),(67166750076956672,'ddd.jpg','chat/9F/7E/67166750076956672.jpg',17808,'350 x 311 (.jpg)',1,'2020-04-03 16:16:40',0),(67169438420299776,'Bs.Kehu.Droid.apk','chat/04/36/67169438420299776.apk',70594488,'apk文件',1,'2020-04-03 16:27:22',1),(67176187961405440,'abc.jpg','chat/DD/5D/67176187961405440.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-03 16:54:10',0),(74743280340692992,'1.jpg','chat/CC/D2/74743280340692992.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-24 14:03:06',1),(75475011070980096,'abc.jpg','chat/DD/5D/75475011070980096.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-26 14:30:44',0),(75475154491011072,'1.jpg','chat/CC/D2/75475154491011072.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:31:18',0),(75475459660181504,'1.jpg','chat/CC/D2/75475459660181504.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:32:31',0),(75475573405511680,'1.jpg','chat/CC/D2/75475573405511680.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:32:58',0),(75475736769458176,'1.jpg','chat/CC/D2/75475736769458176.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:33:37',0),(75476112453267456,'1.jpg','chat/CC/D2/75476112453267456.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:35:06',0),(75477724177494016,'1.jpg','chat/CC/D2/75477724177494016.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:41:30',0),(75477785003290624,'abc.jpg','chat/DD/5D/75477785003290624.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-26 14:41:45',0),(75478180089950208,'Chat.xaml','chat/C2/3C/75478180089950208.xaml',11664,'xaml文件',1,'2020-04-26 14:43:19',1),(75478180182224896,'ChatDetail.cs','chat/49/C1/75478180182224896.cs',9813,'cs文件',1,'2020-04-26 14:43:19',1),(75478180270305280,'ChatInputBar.cs','chat/0D/BB/75478180270305280.cs',6433,'cs文件',1,'2020-04-26 14:43:19',0),(75479061795565568,'Chat.xaml','chat/C2/3C/75479061795565568.xaml',11664,'xaml文件',1,'2020-04-26 14:46:49',0),(75479061917200384,'ChatDetail.cs','chat/49/C1/75479061917200384.cs',9813,'cs文件',1,'2020-04-26 14:46:49',0),(75479062026252288,'ChatInputBar.cs','chat/0D/BB/75479062026252288.cs',6433,'cs文件',1,'2020-04-26 14:46:49',0),(75479250497302528,'ChatInputBar.cs','chat/0D/BB/75479250497302528.cs',6433,'cs文件',1,'2020-04-26 14:47:34',0),(75479607776505856,'1.jpg','chat/CC/D2/75479607776505856.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-26 14:49:00',1),(75479847657140224,'1.jpg','chat/CC/D2/75479847657140224.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:49:57',0),(75480158085967872,'1.jpg','chat/CC/D2/75480158085967872.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:51:11',0),(75480389250838528,'1.jpg','chat/CC/D2/75480389250838528.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-26 14:52:06',0),(75486424963346432,'Chat.xaml','chat/C2/3C/75486424963346432.xaml',11664,'xaml文件',1,'2020-04-26 15:16:05',0),(75486425064009728,'ChatDetail.cs','chat/49/C1/75486425064009728.cs',9813,'cs文件',1,'2020-04-26 15:16:05',0),(75486425156284416,'ChatInputBar.cs','chat/0D/BB/75486425156284416.cs',6433,'cs文件',1,'2020-04-26 15:16:05',1),(75487019367526400,'1.jpg','chat/CC/D2/75487019367526400.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-26 15:18:26',0),(75745284512935936,'1.jpg','chat/CC/D2/75745284512935936.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-27 08:24:44',0),(75772133746012160,'Chat.xaml','chat/C2/3C/75772133746012160.xaml',11664,'xaml文件',1,'2020-04-27 10:11:25',0),(75772133846675456,'ChatDetail.cs','chat/49/C1/75772133846675456.cs',9813,'cs文件',1,'2020-04-27 10:11:25',0),(75772134421295104,'ChatInputBar.cs','chat/0D/BB/75772134421295104.cs',6433,'cs文件',1,'2020-04-27 10:11:25',1),(75776994612998144,'1.jpg','chat/CC/D2/75776994612998144.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-27 10:30:44',1),(75793731341381632,'1.jpg','chat/CC/D2/75793731341381632.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-27 11:37:14',0),(75793731458822144,'ChatDetail.cs','chat/49/C1/75793731458822144.cs',9813,'cs文件',1,'2020-04-27 11:37:14',0),(75793731551096832,'ChatInputBar.cs','chat/0D/BB/75793731551096832.cs',6433,'cs文件',1,'2020-04-27 11:37:14',1),(75832741728219136,'66739208513777664.jpg','chat/22/93/75832741728219136.jpg',40589,'334 x 297 (jpg)',8411237852585984,'2020-04-27 14:12:14',0),(75832742034403328,'1.jpg','chat/CC/D2/75832742034403328.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-27 14:12:14',1),(75833059291557888,'ChatDetail.cs','chat/49/C1/75833059291557888.cs',9813,'cs文件',1,'2020-04-27 14:13:30',0),(75833059392221184,'ChatInputBar.cs','chat/0D/BB/75833059392221184.cs',6433,'cs文件',1,'2020-04-27 14:13:30',0),(75839635486273536,'75832742034403328.jpg','chat/7E/55/75839635486273536.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-27 14:39:40',2),(75839636979445760,'66739208513777664.jpg','chat/22/93/75839636979445760.jpg',40589,'334 x 297 (jpg)',8411237852585984,'2020-04-27 14:39:40',2),(75844728772227072,'ChatDetail.cs','chat/49/C1/75844728772227072.cs',9813,'cs文件',1,'2020-04-27 14:59:54',0),(75844728864501760,'ChatInputBar.cs','chat/0D/BB/75844728864501760.cs',6433,'cs文件',1,'2020-04-27 14:59:54',2),(76111316666675200,'IMG_0006.HEIC','chat/B7/45/76111316666675200.heic',2808983,'4032 x 3024 (heic)',0,'2020-04-28 08:39:16',0),(76111551325401088,'IMG_0006.HEIC','chat/B7/45/76111551325401088.heic',2808983,'4032 x 3024 (heic)',0,'2020-04-28 08:40:12',1),(76111675015426048,'IMG_0002.JPG','chat/E9/B4/76111675015426048.jpg',2604768,'4288 x 2848 (jpg)',0,'2020-04-28 08:40:41',1),(76113076420472832,'IMG_0001.JPG','chat/98/FE/76113076420472832.jpg',1896240,'4288 x 2848 (jpg)',0,'2020-04-28 08:46:15',0),(76113185799532544,'1.jpg','chat/CC/D2/76113185799532544.jpg',40589,'334 x 297 (.jpg)',1,'2020-04-28 08:46:39',0),(76177735114682368,'IMG_0002.JPG','v0/E9/B4/76177735114682368.jpg',2604768,'4288 x 2848 (jpg)',0,'2020-04-28 13:03:09',66),(76214375992389632,'FullSizeRender.jpg','chat/B9/03/76214375992389632.jpg',2190497,'1242 x 1661 (jpg)',0,'2020-04-28 15:28:44',1),(76214714330116096,'Chat.xaml','chat/C2/3C/76214714330116096.xaml',11664,'xaml文件',1,'2020-04-28 15:30:03',0),(76214714409807872,'ChatDetail.cs','chat/49/C1/76214714409807872.cs',9813,'cs文件',1,'2020-04-28 15:30:03',0),(76214714485305344,'ChatInputBar.cs','chat/0D/BB/76214714485305344.cs',6433,'cs文件',1,'2020-04-28 15:30:03',0),(76486706589462528,'Screenshot_2020-04-28-16-14-02-127_com.miui.packageinstaller.jpg','chat/3E/79/76486706589462528.jpg',168528,'1080 x 2340 (jpg)',8411237852585984,'2020-04-29 09:30:52',1),(76486743985876992,'IMG_0382.JPG','chat/0C/E4/76486743985876992.jpg',32162,'464 x 413 (jpg)',0,'2020-04-29 09:31:01',1),(76486802945208320,'FullSizeRender.jpg','chat/B9/03/76486802945208320.jpg',2190497,'1242 x 1661 (jpg)',0,'2020-04-29 09:31:17',0),(76487071930118144,'Chat.xaml','chat/C2/3C/76487071930118144.xaml',11664,'xaml文件',1,'2020-04-29 09:32:19',0),(76487072018198528,'ChatDetail.cs','chat/49/C1/76487072018198528.cs',9813,'cs文件',1,'2020-04-29 09:32:19',0),(76487072102084608,'ChatInputBar.cs','chat/0D/BB/76487072102084608.cs',6433,'cs文件',1,'2020-04-29 09:32:19',0),(76487229128437760,'Demo修改20190804.doc','chat/66/CE/76487229128437760.doc',572416,'doc文件',1,'2020-04-29 09:32:57',1),(76521621724983296,'IMG_0002.JPG','chat/E9/B4/76521621724983296.jpg',2604768,'4288 x 2848 (jpg)',0,'2020-04-29 11:49:40',0),(76874175394738176,'2c1403a82e214682a53fec9576bae3de.wav','chat/7C/CC/76874175394738176.wav',717164,'wav文件',8411237852585984,'2020-04-30 11:10:32',1),(76874744842809344,'c68ec31ff884453dba2fc388b6269c83.wav','chat/3F/01/76874744842809344.wav',1114604,'wav文件',8411237852585984,'2020-04-30 11:12:48',1),(76879465364189184,'28e292213cb54669a4fb9bda4625e75b.wav','chat/CA/CF/76879465364189184.wav',1287360,'wav文件',8411237852585984,'2020-04-30 11:31:33',1),(76902487953371136,'5b8a06348f814a65bf5654b703442208.m4a','chat/A8/5F/76902487953371136.m4a',13568,'m4a文件',8411237852585984,'2020-04-30 13:03:02',2),(76908769213018112,'3674472da5a74da69c5d05138352785e.m4a','chat/1D/22/76908769213018112.m4a',20791,'m4a文件',8411237852585984,'2020-04-30 13:27:59',0),(76913523729231872,'1.jpg','chat/CC/D2/76913523729231872.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-30 13:46:53',0),(76915286083497984,'1.jpg','chat/CC/D2/76915286083497984.jpg',49179,'300 x 300 (.jpg)',1,'2020-04-30 13:53:53',0),(76917565465423872,'aa.jpg','chat/8F/98/76917565465423872.jpg',17891,'300 x 300 (.jpg)',1,'2020-04-30 14:02:56',0),(76918305026076672,'0dd7d38b5e9840b7a25966fc5bc758e0.m4a','chat/3D/38/76918305026076672.m4a',10445,'m4a文件',8411237852585984,'2020-04-30 14:05:53',0),(76918518470012928,'75832742034403328.jpg','chat/7E/55/76918518470012928.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-04-30 14:06:44',0),(76923472400216064,'8a98b769cd954a83abd7ecb728ca99b6.m4a','chat/3C/DD/76923472400216064.m4a',35947,'m4a文件',0,'2020-04-30 14:26:25',1),(76926849905455104,'da4ec27f8d0d411fa3adf84a2ac81c3d.m4a','chat/A5/55/76926849905455104.m4a',36346,'m4a文件',0,'2020-04-30 14:39:50',1),(76927511338807296,'2c1cce1c7a5a4e4fa64e619bc89e8845.m4a','chat/7C/21/76927511338807296.m4a',83825,'m4a文件',0,'2020-04-30 14:42:28',0),(76928825422639104,'ed7ffe1c86134f4590208773c09c5c99.m4a','chat/9B/2C/76928825422639104.m4a',81828,'m4a文件',0,'2020-04-30 14:47:41',0),(76929997265039360,'c2ae4eb043554b548c08e540a68b501f.m4a','chat/A8/0E/76929997265039360.m4a',39713,'m4a文件',0,'2020-04-30 14:52:20',1),(76931805316247552,'1194246890104d23952194bda64b7f35.m4a','chat/6A/E7/76931805316247552.m4a',34413,'m4a文件',0,'2020-04-30 14:59:31',1),(76932645045268480,'aaa4d99f180345c3912935e89c3781a1.m4a','chat/C3/1A/76932645045268480.m4a',28588,'m4a文件',0,'2020-04-30 15:02:51',1),(76933570514251776,'f34cd6be374244649d554544bbbf6b70.m4a','chat/DA/47/76933570514251776.m4a',28365,'m4a文件',0,'2020-04-30 15:06:32',1),(76935318695964672,'075d8c8c663248b5bb016a15a67a411a.m4a','chat/FA/FF/76935318695964672.m4a',44631,'m4a文件',0,'2020-04-30 15:13:29',0),(76938013909577728,'edef0e47e31b46d08fb6c552906cf151.m4a','chat/FD/B7/76938013909577728.m4a',40191,'m4a文件',0,'2020-04-30 15:24:11',0),(76943264389656576,'2a7b181ad26448fe93b08a9d002f01f3.m4a','chat/76/1E/76943264389656576.m4a',21854,'m4a文件',8411237852585984,'2020-04-30 15:45:03',1),(76943404798177280,'1280a7e0bcd14ebf9c994166e18d7ea2.m4a','chat/67/3D/76943404798177280.m4a',16385,'m4a文件',8411237852585984,'2020-04-30 15:45:37',1),(76943756880637952,'1035dd18b5c047fcaeb451cbee28ff58.m4a','chat/3A/AC/76943756880637952.m4a',41850,'m4a文件',0,'2020-04-30 15:47:01',1),(76956131226677248,'e13927305ee84574b67a809fb039be87.m4a','chat/8E/2B/76956131226677248.m4a',10250,'m4a文件',8411237852585984,'2020-04-30 16:36:11',0),(76958313313333248,'3d9e10ac3f124ee08990b192c6e88144.m4a','v0/85/86/76958313313333248.m4a',9859,'m4a文件',8411237852585984,'2020-04-30 16:44:51',0),(76958849588654080,'ab6efee7ee594b3dab9b43b5505f72e2.m4a','v0/A4/1F/76958849588654080.m4a',8688,'m4a文件',8411237852585984,'2020-04-30 16:46:59',0),(79006578783416320,'abc.m4a','v0/3E/08/79006578783416320.m4a',28365,'00:06',1,'2020-05-06 08:24:04',1),(79010543533158400,'a87ccdb44a78497996c89b5ba341759c.m4a','chat/5E/F7/79010543533158400.m4a',10250,'00:04',8411237852585984,'2020-05-06 08:39:49',0),(79010668305313792,'916bde9e1d1049e6b070ecc08fad5522.m4a','chat/00/69/79010668305313792.m4a',13178,'00:06',8411237852585984,'2020-05-06 08:40:19',0),(79015215551606784,'011a20c88db94b4283da0094cd7ad2e2.m4a','chat/A0/16/79015215551606784.m4a',8688,'00:03',8411237852585984,'2020-05-06 08:58:23',0),(79015275014254592,'1a9fcfcc7b5a47869438651e4cc9c8e1.m4a','chat/FD/07/79015275014254592.m4a',20595,'00:11',8411237852585984,'2020-05-06 08:58:37',1),(79023211941851136,'abc.m4a','chat/3E/08/79023211941851136.m4a',28365,'00:06',1,'2020-05-06 09:30:09',1),(79023291511992320,'Chat.xaml','chat/C2/3C/79023291511992320.xaml',11664,'xaml文件',1,'2020-05-06 09:30:28',0),(79023895038783488,'e8adb811c0ec4fb689619a56974d889f.m4a','chat/C5/7D/79023895038783488.m4a',8688,'00:03',8411237852585984,'2020-05-06 09:32:52',0),(79025346381213696,'23c5bf765f1b4b7a8e601e5dc34399e3.m4a','chat/0B/EC/79025346381213696.m4a',11031,'00:05',8411237852585984,'2020-05-06 09:38:38',1),(79029054041092096,'mov_bbb.mp4','chat/7A/1B/79029054041092096.mp4',788493,'00:00:10 (320 x 176)',1,'2020-05-06 09:53:22',2),(79035688339501056,'00_04.wav','chat/ED/60/79035688339501056.wav',384496,'00:04',1,'2020-05-06 10:19:44',1),(79046986980782080,'0cad6a4134b14b51bd553f8ab0394beb.m4a','chat/85/2E/79046986980782080.m4a',41324,'00:05',0,'2020-05-06 11:04:38',0),(79103159838830592,'d9c0858bf5a04e6d95640a3150dbe73b.m4a','chat/01/FF/79103159838830592.m4a',8688,'00:03',8411237852585984,'2020-05-06 14:47:42',1),(79104313280819200,'b945e8d778dd491486f1e2a2f0a55bc2.m4a','chat/6C/9C/79104313280819200.m4a',9664,'00:04',8411237852585984,'2020-05-06 14:52:17',0),(79104348408115200,'74a2a0e1890140559763efbb99f1043e.m4a','chat/2C/3F/79104348408115200.m4a',12983,'00:06',8411237852585984,'2020-05-06 14:52:25',1),(80179499258212352,'ef3e4a98c2b740fb9b2c9669494fce30.mp4','v0/CE/3A/80179499258212352.mp4',538460,'00:05 (480 x 360)',0,'2020-05-09 14:04:44',0),(80179624273637376,'09977e213270434bafd297831a344979.jpg','chat/54/92/80179624273637376.jpg',5179868,'4032 x 3024 (jpg)',0,'2020-05-09 14:05:15',1),(80179802284093440,'0f4003d774ea4bfc9db17a5b5437dbaf.mp4','chat/BE/A5/80179802284093440.mp4',475298,'00:04 (480 x 360)',0,'2020-05-09 14:05:55',1),(80180869143064576,'IMG_0779.MOV','chat/DB/CC/80180869143064576.mov',5353019,'1080 x 1920 (mov)',0,'2020-05-09 14:10:14',2),(80184072756654080,'IMG_0779.MOV','chat/DB/CC/80184072756654080.mov',5353019,'1080 x 1920 (mov)',0,'2020-05-09 14:23:01',1),(80184336418992128,'f1ad3e0d5f5643179865617ff20cc4be.mp4','chat/E9/33/80184336418992128.mp4',446906,'00:04 (360 x 480)',0,'2020-05-09 14:23:56',0),(80184570809282560,'da55422eef0845da8ef7b055234067ca.mp4','chat/40/14/80184570809282560.mp4',354452,'00:03 (360 x 480)',0,'2020-05-09 14:24:52',1),(80187945395286016,'3f51cfc9fb664e899e36d3e183e58e63.mp4','chat/F0/5E/80187945395286016.mp4',8148024,'00:04 (1080 x 1920)',0,'2020-05-09 14:38:21',1),(80205717391142912,'3d2916be9d24421b899151683e943cc0.mp4','chat/18/B0/80205717391142912.mp4',11453919,'00:00:04 (1920 x 1080)',8411237852585984,'2020-05-09 15:48:59',1),(80205991891562496,'15a83ed344e84a1eb7c8af7f62c3e668.jpg','chat/98/15/80205991891562496.jpg',4987986,'4032 x 3024 (jpg)',0,'2020-05-09 15:50:03',1),(80206131020820480,'bbab9e607b664d32b689d6d88251d0e7.mp4','chat/C3/70/80206131020820480.mp4',7084191,'00:03 (1080 x 1920)',0,'2020-05-09 15:50:36',1),(80206528108163072,'fc6dbfefaded46bf802925f2ba689733.mp4','v0/9C/02/80206528108163072.mp4',7433745,'00:00:02 (1920 x 1080)',8411237852585984,'2020-05-09 15:52:10',0),(80212384711307264,'62b0b11df62b456abae5cfe34caefc21.jpg','chat/56/F0/80212384711307264.jpg',71303,'720 x 1280 (jpg)',8411237852585984,'2020-05-09 16:15:23',0),(80819234523705344,'3edf58c1385e44f8bfd8b083ed0d746c.jpg','v0/21/06/80819234523705344.jpg',36129,'640 x 480 (jpg)',1,'2020-05-11 08:26:50',0),(80819439436427264,'354e62bc31064830b38da77fe0fd85c7.jpg','v0/78/AE/80819439436427264.jpg',36129,'640 x 480 (jpg)',1,'2020-05-11 08:27:39',0),(80826125681291264,'e1343db5a8da4ac9a312a47a4d495af2.mp4','v0/00/06/80826125681291264.mp4',1414027,'00:00:04 (640 x 480)',1,'2020-05-11 08:54:13',0),(81563700230483968,'1facf6c9ecfa4e509902c66aa79951b0.m4a','chat/1E/57/81563700230483968.m4a',44357,'00:06',0,'2020-05-13 09:45:02',1),(81563797735469056,'a5ec2a5795aa44e6a5da00a1a29356f7.jpg','chat/8D/4E/81563797735469056.jpg',4970312,'4032 x 3024 (jpg)',0,'2020-05-13 09:45:29',1),(81569009808306176,'75832742034403328.jpg','v0/7E/55/81569009808306176.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2020-05-13 10:06:08',89),(88875538962051072,'u285.jpg','v0/A1/62/88875538962051072.jpg',263911,'1024 x 900 (.jpg)',1,'2020-06-02 13:59:39',4),(91373962043191296,'2.jpg','v0/1D/89/91373962043191296.jpg',25960,'640 x 769 (.jpg)',1,'2020-06-09 11:27:30',1),(91374008964870144,'IMG_20150518_124337.jpg','v0/74/1E/91374008964870144.jpg',517556,'1080 x 811 (.jpg)',1,'2020-06-09 11:27:41',1),(91786038024728576,'IMG_20160818_145515.jpg','v0/CF/B2/91786038024728576.jpg',2691194,'4160 x 2336 (.jpg)',1,'2020-06-10 14:44:56',0),(91786445488779264,'IMG_20150518_125023.jpg','v0/3D/E6/91786445488779264.jpg',510138,'1080 x 801 (.jpg)',1,'2020-06-10 14:46:34',0),(91789217609150464,'VID_20160930_100110.mp4','v0/26/2D/91789217609150464.mp4',25116782,'00:00:24 (1920 x 1080)',1,'2020-06-10 14:57:35',16),(91795724492992512,'VID_20160930_101031.mp4','v0/8F/9E/91795724492992512.mp4',3959865,'00:00:03 (1920 x 1080)',1,'2020-06-10 15:23:26',1),(91795768751288320,'IMG_20160921_134610.jpg','v0/4E/93/91795768751288320.jpg',2662560,'4160 x 2336 (.jpg)',1,'2020-06-10 15:23:36',0),(91800378303967232,'IMG_20150518_124913.jpg','v0/60/55/91800378303967232.jpg',541402,'1080 x 805 (.jpg)',1,'2020-06-10 15:41:55',1),(91800491223019520,'6.jpg','v0/61/62/91800491223019520.jpg',46138,'960 x 540 (.jpg)',1,'2020-06-10 15:42:22',0),(91800491357237248,'7.jpg','v0/B2/18/91800491357237248.jpg',34263,'540 x 960 (.jpg)',1,'2020-06-10 15:42:22',2),(91800491449511936,'IMG_20150518_125051.jpg','v0/F4/5B/91800491449511936.jpg',472877,'1080 x 802 (.jpg)',1,'2020-06-10 15:42:22',0),(91803792962351104,'2-5.JPG','v0/09/EE/91803792962351104.jpg',2190807,'4000 x 3000 (.jpg)',1,'2020-06-10 15:55:29',0),(91803835261906944,'CIMG5332.JPG','v0/B8/94/91803835261906944.jpg',2226785,'4000 x 3000 (.jpg)',1,'2020-06-10 15:55:39',0),(91803936436908032,'IMG_20160906_145630.jpg','v0/65/1F/91803936436908032.jpg',3020173,'4160 x 2336 (.jpg)',1,'2020-06-10 15:56:04',0),(91803998420332544,'IMG_20160906_145451.jpg','v0/EA/34/91803998420332544.jpg',2513845,'4160 x 2336 (.jpg)',1,'2020-06-10 15:56:18',0),(91804086945312768,'IMG_20160818_150302.jpg','v0/21/92/91804086945312768.jpg',2578809,'4160 x 2336 (.jpg)',1,'2020-06-10 15:56:39',0),(92529784261570560,'u354.png','v0/00/F7/92529784261570560.png',296599,'553 x 291 (.png)',1,'2020-06-12 16:00:18',2),(105124916462743552,'公司服务器及网络.txt','chat/5F/37/105124916462743552.txt',435,'txt文件',1,'2020-07-17 10:08:54',1),(140723461135659008,'12.xlsx','v0/52/37/140723461135659008.xlsx',8153,'xlsx文件',1,'2020-10-23 15:44:49',0),(142888904373956608,'12.xlsx','v0/52/37/142888904373956608.xlsx',8153,'xlsx文件',1,'2020-10-29 15:09:29',1),(142914111109197824,'Icon-20@2x.png','v0/E3/0D/142914111109197824.png',436,'40 x 40 (.png)',1,'2020-10-29 16:49:38',9),(143174606269575168,'Icon-20@3x.png','v0/56/59/143174606269575168.png',496,'60 x 60 (.png)',1,'2020-10-30 10:04:47',6),(143191060947791872,'Icon-20@3x.png','v0/56/59/143191060947791872.png',534,'60 x 60 (.png)',1,'2020-10-30 11:10:10',20),(143192411832446976,'Icon-29@2x.png','v0/46/CE/143192411832446976.png',624,'58 x 58 (.png)',1,'2020-10-30 11:15:32',6),(143193081931231232,'3709740f5c5e4cb4909a6cc79f412734_th.png','v0/BF/6D/143193081931231232.png',27589,'537 x 302 (.png)',1,'2020-10-30 11:18:12',1),(143195002217820160,'未标题-2.jpg','v0/E3/18/143195002217820160.jpg',49179,'300 x 300 (.jpg)',1,'2020-10-30 11:25:50',25),(143203944767549440,'ImageStabilization.wmv','v0/EA/34/143203944767549440.wmv',403671,'00:00:06 (480 x 288)',1,'2020-10-30 12:01:22',4),(159556679775813632,'公司服务器及网络.txt','v0/5F/37/159556679775813632.txt',435,'txt文件',1,'2020-12-14 15:01:15',1),(172190549775609856,'公司服务器及网络.txt','v0/5F/37/172190549775609856.txt',435,'txt文件',1,'2021-01-18 11:43:37',11),(172613740583055360,'Kitematic-Windows.zip','v0/7C/43/172613740583055360.zip',68582887,'zip文件',1,'2021-01-19 15:45:24',0),(185641725430984704,'1.png','v0/FC/63/185641725430984704.png',47916,'1101 x 428 (.png)',1,'2021-02-24 14:33:54',3),(187725778074333184,'doc1.png','v0/D8/28/187725778074333184.png',59038,'1076 x 601 (.png)',1,'2021-03-02 08:35:14',3),(195351968117288960,'IMG_1174.JPG','chat/94/8D/195351968117288960.jpg',2372862,'4032 x 3024 (jpg)',0,'2021-03-23 09:39:15',2),(195354540781727744,'e083c794e5a04b4db7b8a82b99828736.m4a','chat/B4/8C/195354540781727744.m4a',41620,'00:05',0,'2021-03-23 09:49:18',1),(195354884563660800,'705da00b023d4cff82f43ba62b0bb411.m4a','chat/9B/78/195354884563660800.m4a',47960,'00:07',0,'2021-03-23 09:50:39',0),(195692840486825984,'IMG_1174.JPG','chat/94/8D/195692840486825984.jpg',2372862,'4032 x 3024 (jpg)',0,'2021-03-24 08:13:42',0),(195693129310793728,'IMG_1174.JPG','chat/94/8D/195693129310793728.jpg',2372862,'4032 x 3024 (jpg)',0,'2021-03-24 08:14:51',0),(195694580921659392,'IMG_1174.JPG','chat/94/8D/195694580921659392.jpg',2372862,'4032 x 3024 (jpg)',0,'2021-03-24 08:20:39',1),(195696978054475776,'b20e6c7053e14b6db4c22dd6dd7a6487.m4a','chat/CD/15/195696978054475776.m4a',39061,'00:04',0,'2021-03-24 08:29:57',0),(195697423770578944,'6d6acc516f8444a3be9a3bd99d663c44.m4a','chat/1D/6B/195697423770578944.m4a',15064,'00:07',192818293676994560,'2021-03-24 08:31:43',1),(195697688133365760,'IMG_20201026_024819.jpg','chat/89/6F/195697688133365760.jpg',0,'0 x 0 (jpg)',8411237852585984,'2021-03-24 08:32:46',2),(195698088278355968,'IMG_20201026_024819.jpg','chat/89/6F/195698088278355968.jpg',0,'0 x 0 (jpg)',8411237852585984,'2021-03-24 08:34:21',2),(195698534673936384,'IMG_20201026_024819.jpg','chat/89/6F/195698534673936384.jpg',0,'0 x 0 (jpg)',8411237852585984,'2021-03-24 08:36:08',2),(196148954584182784,'IMG_20201026_024819.jpg','chat/89/6F/196148954584182784.jpg',200427,'960 x 1280 (jpg)',8411237852585984,'2021-03-25 14:25:55',0),(196160610236231680,'92dfc942ff8e4785955c499dfb5c3c3b.m4a','chat/7B/4B/196160610236231680.m4a',13764,'00:06',8411237852585984,'2021-03-25 15:12:13',0),(196161161258725376,'4bcb4823ab62442a843b10343751660f.m4a','chat/17/23/196161161258725376.m4a',7908,'00:02',8411237852585984,'2021-03-25 15:14:24',0),(196168091893100544,'13d18645c5354f089b9fc64d3aa297e4.m4a','chat/55/C6/196168091893100544.m4a',12112,'00:05',196167762048839680,'2021-03-25 15:41:57',0),(196168454671036416,'63e47c931fe0401b9e3fef25e1b3ab10.m4a','chat/43/2C/196168454671036416.m4a',5467,'00:01',196167762048839680,'2021-03-25 15:43:23',0),(196168796318068736,'Screenshot_2021-03-24-20-09-20-304_com.ss.android.article.video.jpg','chat/E0/32/196168796318068736.jpg',260078,'2340 x 1080 (jpg)',196167762048839680,'2021-03-25 15:44:45',1),(196169482460065792,'工作流系统使用手册.doc','chat/EA/EE/196169482460065792.doc',2233856,'doc文件',1,'2021-03-25 15:47:33',1),(196170017900720128,'wx_camera_1616118077177.jpg','chat/EE/D7/196170017900720128.jpg',1381259,'1920 x 887 (jpg)',196167762048839680,'2021-03-25 15:49:39',0),(196170169919074304,'IMG_20210225_111848.jpg','chat/CF/43/196170169919074304.jpg',8168873,'4000 x 3000 (jpg)',196167762048839680,'2021-03-25 15:50:36',1),(196170394020737024,'e780d8a4a35b2a259cff8fcd49778937.mp4','chat/40/09/196170394020737024.mp4',10842597,'00:03:06 (854 x 480)',196167762048839680,'2021-03-25 15:51:53',1),(198242511948214272,'IMG_20210325_055419.jpg','chat/03/90/198242511948214272.jpg',200427,'960 x 1280 (jpg)',8411237852585984,'2021-03-31 09:04:58',0),(198242804110848000,'ef14e6326140452e8a9c826cbe5cf943.jpg','chat/27/11/198242804110848000.jpg',200427,'960 x 1280 (jpg)',8411237852585984,'2021-03-31 09:06:07',0),(198260189991661568,'IMG_20210325_055419.jpg','chat/03/90/198260189991661568.jpg',200427,'960 x 1280 (jpg)',8411237852585984,'2021-03-31 10:15:12',0),(198260269800878080,'IMG_20210325_055419.jpg','chat/03/90/198260269800878080.jpg',200427,'960 x 1280 (jpg)',8411237852585984,'2021-03-31 10:15:31',0),(198260270102867968,'IMG_20201026_024819.jpg','chat/89/6F/198260270102867968.jpg',200427,'960 x 1280 (jpg)',8411237852585984,'2021-03-31 10:15:31',0),(198266554311110656,'504d28b7b4b347c281455e01beb3c133.m4a','chat/00/D5/198266554311110656.m4a',37194,'00:03',0,'2021-03-31 10:40:30',0),(198270589348999168,'IMG_0002.JPG','chat/E9/B4/198270589348999168.jpg',2604768,'4288 x 2848 (jpg)',0,'2021-03-31 10:56:32',0),(199062303022116864,'64571bb7f628430b9f07fbee88f704af.m4a','chat/6F/A8/199062303022116864.m4a',8697,'00:03',8411237852585984,'2021-04-02 15:22:31',0),(199062434618404864,'b5b91426a2fc469380b5ebeda13a9f25.jpg','chat/3B/C3/199062434618404864.jpg',200453,'960 x 1280 (jpg)',8411237852585984,'2021-04-02 15:23:02',0),(199063071552827392,'4eb4d110eebf4c62bea518db51e27cb4.m4a','chat/81/47/199063071552827392.m4a',8892,'00:03',8411237852585984,'2021-04-02 15:25:34',0),(199063904826814464,'ace2ce015c3044fe9afcfc7b4675a65c.m4a','chat/38/1C/199063904826814464.m4a',9283,'00:04',8411237852585984,'2021-04-02 15:28:52',0),(199065343775076352,'16ad68f9749e401fa2fd3b28493192ef.m4a','chat/D4/59/199065343775076352.m4a',10064,'00:04',8411237852585984,'2021-04-02 15:34:36',0),(199066951527297024,'43873d0247154b43b2dd140591143413.m4a','chat/23/05/199066951527297024.m4a',10844,'00:05',8411237852585984,'2021-04-02 15:40:59',0),(199068852893380608,'1502fe0981294270846d1ba6dfbc85f8.jpg','chat/22/C5/199068852893380608.jpg',200683,'960 x 1280 (jpg)',8411237852585984,'2021-04-02 15:48:32',0),(205527361709273088,'IMG_20210413_063902.jpg','chat/8D/E2/205527361709273088.jpg',145365,'960 x 1280 (jpg)',8411237852585984,'2021-04-20 11:32:22',1),(205584104271245312,'3709740f5c5e4cb4909a6cc79f412734_th.png','chat/BF/6D/205584104271245312.png',27589,'537 x 302 (.png)',1,'2021-04-20 15:17:50',1),(205584262643970048,'Windows Logon.wav','chat/88/4F/205584262643970048.wav',384496,'00:04',1,'2021-04-20 15:18:27',1),(205584822994595840,'icon.png','chat/94/95/205584822994595840.png',595,'72 x 72 (.png)',1,'2021-04-20 15:20:41',3),(205586704345460736,'未标题-2.jpg','chat/E3/18/205586704345460736.jpg',49179,'300 x 300 (.jpg)',1,'2021-04-20 15:28:09',3),(205588627148632064,'Stub.cs','chat/8C/FE/205588627148632064.cs',8024,'cs文件',1,'2021-04-20 15:35:48',1),(205588924898078720,'205586704345460736.jpg','chat/88/8D/205588924898078720.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2021-04-20 15:36:59',3),(205591090354319360,'model-40e73a94.db','chat/25/CB/205591090354319360.db',81920,'db文件',8411237852585984,'2021-04-20 15:45:35',1),(205593309275353088,'IMG_20210420_072538.jpg','chat/7E/DE/205593309275353088.jpg',145926,'960 x 1280 (jpg)',8411237852585984,'2021-04-20 15:54:24',0),(205593366909284352,'205584822994595840.png','chat/B4/B7/205593366909284352.png',595,'72 x 72 (png)',8411237852585984,'2021-04-20 15:54:38',3),(205600547603869696,'未标题-2.jpg','chat/E3/18/205600547603869696.jpg',49179,'300 x 300 (.jpg)',1,'2021-04-20 16:23:10',3),(205601611057065984,'205586704345460736.jpg','chat/88/8D/205601611057065984.jpg',49179,'300 x 300 (jpg)',8411237852585984,'2021-04-20 16:27:23',3),(205603140681986048,'205584822994595840.png','chat/B4/B7/205603140681986048.png',595,'72 x 72 (png)',8411237852585984,'2021-04-20 16:33:28',3),(205603316255551488,'205584104271245312.png','chat/31/4B/205603316255551488.png',27589,'537 x 302 (png)',8411237852585984,'2021-04-20 16:34:10',0),(205603810206150656,'doc2.png','chat/29/DF/205603810206150656.png',110019,'670 x 532 (.png)',1,'2021-04-20 16:36:08',1),(205603876752977920,'state.db','chat/DF/F3/205603876752977920.db',90112,'db文件',1,'2021-04-20 16:36:23',2),(205603985855213568,'model-40e73a94.db','chat/25/CB/205603985855213568.db',81920,'db文件',8411237852585984,'2021-04-20 16:36:49',1),(205860650894618624,'state.db','chat/DF/F3/205860650894618624.db',90114,'db文件',0,'2021-04-21 09:36:45',0),(205860866376986624,'Stub.cs','chat/8C/FE/205860866376986624.cs',8024,'cs文件',1,'2021-04-21 09:37:36',2),(205861747298267136,'205860866376986624.cs','chat/C0/EA/205861747298267136.cs',8026,'cs文件',0,'2021-04-21 09:41:06',0),(205862303681081344,'state.db','chat/DF/F3/205862303681081344.db',90114,'db文件',0,'2021-04-21 09:43:19',0),(205862778044280832,'205860866376986624.cs','chat/C0/EA/205862778044280832.cs',8026,'cs文件',0,'2021-04-21 09:45:12',0),(205863146711019520,'state.db','chat/DF/F3/205863146711019520.db',90114,'db文件',0,'2021-04-21 09:46:40',0),(205863590044758016,'205860866376986624.cs','chat/C0/EA/205863590044758016.cs',8026,'cs文件',0,'2021-04-21 09:48:26',0),(205866724880150528,'205860866376986624.cs','chat/C0/EA/205866724880150528.cs',8026,'cs文件',0,'2021-04-21 10:00:53',0),(205867083392479232,'state.db','chat/DF/F3/205867083392479232.db',90114,'db文件',0,'2021-04-21 10:02:19',0),(205871027992784896,'state.db','chat/DF/F3/205871027992784896.db',90114,'db文件',0,'2021-04-21 10:17:59',0),(205878287125049344,'state.db','chat/DF/F3/205878287125049344.db',90114,'db文件',0,'2021-04-21 10:46:50',0),(205883012461490176,'state.db','chat/DF/F3/205883012461490176.db',90114,'db文件',0,'2021-04-21 11:05:36',0),(205884144625774592,'state.db','chat/DF/F3/205884144625774592.db',90114,'db文件',0,'2021-04-21 11:10:06',0),(205884816842682368,'state.db','v0/DF/F3/205884816842682368.db',90114,'db文件',0,'2021-04-21 11:12:46',0),(205889490907951104,'state.db','chat/DF/F3/205889490907951104.db',90114,'db文件',0,'2021-04-21 11:31:21',0),(205890524870995968,'state.db','chat/DF/F3/205890524870995968.db',90114,'db文件',0,'2021-04-21 11:35:27',0),(205891917933572096,'state.db','chat/DF/F3/205891917933572096.db',90114,'db文件',0,'2021-04-21 11:40:59',0),(205895131001122816,'state.db','chat/DF/F3/205895131001122816.db',90114,'db文件',0,'2021-04-21 11:53:45',0),(205896396607188992,'state.db','chat/DF/F3/205896396607188992.db',90114,'db文件',0,'2021-04-21 11:58:47',1),(205916918690738176,'state.db','v0/DF/F3/205916918690738176.db',90114,'db文件',0,'2021-04-21 13:20:20',1),(208762000041267200,'养老项目建议书.docx','chat/6B/9A/208762000041267200.docx',20170,'docx文件',1,'2021-04-29 09:45:40',2),(211644071852068864,'未标题-2.jpg','chat/E3/18/211644071852068864.jpg',49179,'300 x 300 (.jpg)',1,'2021-05-07 08:37:58',2),(211647952552153088,'未标题-2.jpg','chat/E3/18/211647952552153088.jpg',49179,'300 x 300 (.jpg)',1,'2021-05-07 08:53:23',1),(211648064154193920,'未标题-2.jpg','chat/E3/18/211648064154193920.jpg',49179,'300 x 300 (.jpg)',1,'2021-05-07 08:53:50',2),(211649132401479680,'960a510a-d632-11ea-94d4-eadd54424171.mp4','chat/92/80/211649132401479680.mp4',565000,'00:00:14 (848 x 480)',1,'2021-05-07 08:58:05',1),(211650057664942080,'截屏2021-05-06 上午11.29.30.png','chat/2C/97/211650057664942080.png',41640,'777 x 304 (.png)',1,'2021-05-07 09:01:45',2),(211661686293090304,'IMG_1212.JPG','chat/19/F8/211661686293090304.jpg',207682,'837 x 1488 (jpg)',0,'2021-05-07 09:47:58',2),(211676271704231936,'960a510a-d632-11ea-94d4-eadd54424171.mp4','chat/92/80/211676271704231936.mp4',565000,'00:00:14 (848 x 480)',1,'2021-05-07 10:45:55',1),(211676310509932544,'未标题-2.jpg','chat/E3/18/211676310509932544.jpg',49179,'300 x 300 (.jpg)',1,'2021-05-07 10:46:04',1),(211677962780495872,'未标题-2.jpg','chat/E3/18/211677962780495872.jpg',49179,'300 x 300 (.jpg)',1,'2021-05-07 10:52:38',1),(211678059324985344,'960a510a-d632-11ea-94d4-eadd54424171.mp4','chat/92/80/211678059324985344.mp4',565000,'00:00:14 (848 x 480)',1,'2021-05-07 10:53:01',1),(211736783456522240,'960a510a-d632-11ea-94d4-eadd54424171.mp4','chat/92/80/211736783456522240.mp4',565000,'00:00:14 (848 x 480)',1,'2021-05-07 14:46:22',2),(211737220217786368,'IMG_0002.JPG','chat/E9/B4/211737220217786368.jpg',2604768,'4288 x 2848 (jpg)',0,'2021-05-07 14:48:06',1),(228403236632915968,'Uno_Logo_Uno_Generic-blog-cover.png','chat/BD/BD/228403236632915968.png',113645,'4500 x 1736 (.png)',1,'2021-06-22 14:32:56',0),(231679691211010048,'IMG_0002.JPG','chat/E9/B4/231679691211010048.jpg',2604768,'4288 x 2848 (jpg)',0,'2021-07-01 15:32:22',0),(233073052786487296,'1c1ec3a5cfc84481a20608fd343b13fe.jpg','chat/EC/F0/233073052786487296.jpg',200756,'960 x 1280 (jpg)',227949556179791872,'2021-07-05 11:49:05',0),(233073213751291904,'52873dd14d564d5ea5642861af0a3cec.jpg','chat/DD/82/233073213751291904.jpg',200526,'960 x 1280 (jpg)',227949556179791872,'2021-07-05 11:49:44',0),(233073272895172608,'4991d5716e1b47fba213286debcbc858.mp4','chat/A9/FE/233073272895172608.mp4',1329665,'00:00:02 (1280 x 720)',227949556179791872,'2021-07-05 11:49:58',0),(243637220816502784,'e526d8b50fdf409090f81e99f6128f69.m4a','chat/C9/C6/243637220816502784.m4a',11422,'00:05',243635822079033344,'2021-08-03 15:27:20',0),(244361180335697920,'qrcode-224417791112769536.png','chat/05/B8/244361180335697920.png',1170,'980 x 980 (png)',224417791112769536,'2021-08-05 15:24:05',1),(246817028177784832,'公司服务器及网络.txt','v0/5F/37/246817028177784832.txt',435,'txt文件',1,'2021-08-12 10:02:46',0),(254566934062088192,'infzm_1630567602.jpeg','chat/E7/CE/254566934062088192.jpeg',769971,'1080 x 1221 (jpeg)',254566454892216320,'2021-09-02 19:18:07',0),(287476098484453376,'snapshot.png','v0/F7/78/287476098484453376.png',17725,'1118 x 929 (.png)',0,'2021-12-02 14:47:23',0),(287476447131779072,'snapshot2.png','v0/C1/47/287476447131779072.png',17197,'622 x 689 (.png)',0,'2021-12-02 14:48:49',0);
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
-- Table structure for table `lob_agent`
--

DROP TABLE IF EXISTS `lob_agent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `lob_agent` (
  `ID` bigint(20) NOT NULL COMMENT '代理人标识',
  `Phone` char(11) NOT NULL COMMENT '手机号，唯一',
  `Name` varchar(32) NOT NULL COMMENT '姓名',
  `Pwd` char(32) NOT NULL COMMENT '密码的md5',
  `Sex` tinyint(4) unsigned NOT NULL DEFAULT '1' COMMENT '#Gender#性别',
  `Photo` varchar(255) NOT NULL DEFAULT '' COMMENT '头像',
  `AccountName` varchar(32) NOT NULL COMMENT '开户姓名',
  `Bank` varchar(255) NOT NULL COMMENT '开户银行',
  `Account` varchar(255) NOT NULL COMMENT '银行账号',
  `BankPhone` varchar(255) NOT NULL COMMENT '联系电话',
  `Expired` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否停用',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  `LeaderPhone` char(11) NOT NULL COMMENT '推荐人手机号',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `idx_agent_phone` (`Phone`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代理人';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lob_agent`
--

LOCK TABLES `lob_agent` WRITE;
/*!40000 ALTER TABLE `lob_agent` DISABLE KEYS */;
INSERT INTO `lob_agent` VALUES (224367919756079104,'15948371897','洪道亭','c44503fdeb660738ecf033007b437e9f',1,'','','','','',0,'2021-06-11 11:18:00','2021-06-11 11:18:00','13377777777'),(224417791112769536,'13377777777','测试','f63f4fbc9f8c85d409f2f59f2b9e12d5',1,'','李强','工商银行·牡丹卡普卡','6212264200012966703','18686479123',0,'2021-06-11 14:36:11','2021-06-11 14:36:11',''),(243635822079033344,'13578696916','张勋','a94ad9e7adae68af2e81d23e60dab2c7',1,'','','','','',0,'2021-08-03 15:21:47','2021-08-03 15:21:47',''),(254811010451685376,'15104489118','15104489118','f860e2f2738b017c370fd3cf5de79dae',1,'','','','','',0,'2021-09-03 11:27:59','2021-09-03 11:27:59',''),(254857567955894272,'15948304068','15948304068','aff436ac586227aa2c17cb715c921fd6',1,'','','','','',0,'2021-09-03 14:32:59','2021-09-03 14:32:59','15948371897'),(254860856827101184,'13294383336','13294383336','676312efbad35c9a2bbe22bd6a0e61de',1,'','','','','',0,'2021-09-03 14:46:03','2021-09-03 14:46:03','13578696916'),(257303201296203776,'13043333152','13043333152','c27daa3d45ee95f13888d68550eb6019',1,'','','','','',0,'2021-09-10 08:31:03','2021-09-10 08:31:03','13294383336'),(258101798598131712,'18686479123','18686479123','2e79a2ae13a9e992504fd3776f6e8723',1,'','','','','',0,'2021-09-12 13:24:23','2021-09-12 13:24:23',''),(258806962111954944,'13843646689','13843646689','ae9586ada632a35ee545ba75edf788f0',1,'','','','','',0,'2021-09-14 12:06:28','2021-09-14 12:06:28','13578696916'),(259488846328414208,'13693542898','13693542898','a9d8941dab991e4958b649c0645908aa',1,'','','','','',0,'2021-09-16 09:16:02','2021-09-16 09:16:02',''),(260274754136883200,'13621667509','13621667509','75477c455955407bd165845289bbf93c',1,'','','','','',0,'2021-09-18 13:18:57','2021-09-18 13:18:57',''),(260284648621654016,'15946603325','15946603325','07c866601f3db42bb2d834262f4385f2',1,'','','','','',0,'2021-09-18 13:58:15','2021-09-18 13:58:15',''),(260285708077678592,'13251527583','13251527583','0959d38d1e5cd4e6889147ddc8654145',1,'','','','','',0,'2021-09-18 14:02:28','2021-09-18 14:02:28',''),(260294959554674688,'13833046002','13833046002','763ac67a58917087614609b739c96e90',1,'','','','','',0,'2021-09-18 14:39:14','2021-09-18 14:39:14',''),(260295091532644352,'17305366922','17305366922','7df88598a79556b8e638711240f7d19a',1,'','','','','',0,'2021-09-18 14:39:46','2021-09-18 14:39:46',''),(260343217559748608,'13367127750','13367127750','3c4ea0decf84de49212d86cc1ed81ec2',1,'','','','','',0,'2021-09-18 17:51:00','2021-09-18 17:51:00',''),(260416930728689664,'17603200038','17603200038','cfd7aa3a1381b07de42347b89852dec5',1,'','','','','',0,'2021-09-18 22:43:54','2021-09-18 22:43:54',''),(260420117594423296,'15750367263','15750367263','cd49cd09eb067bab90723da89d39469c',1,'','','','','',0,'2021-09-18 22:56:34','2021-09-18 22:56:34',''),(260436375761108992,'18037706956','18037706956','da79dcb2a4b5f02348dc9d2498d0747a',1,'','','','','',0,'2021-09-19 00:01:11','2021-09-19 00:01:11',''),(260451705904480256,'18271715904','18271715904','152359cb12d7a6fc6f792ce3c72b3a4c',1,'','','','','',0,'2021-09-19 01:02:06','2021-09-19 01:02:06',''),(260617057535836160,'15682325859','15682325859','1d881bfa71c365f27eb52ca4e5d50873',1,'','','','','',0,'2021-09-19 11:59:08','2021-09-19 11:59:08',''),(260618901385428992,'13680096668','13680096668','74fd88052fd28baa056912cb51b75830',1,'','','','','',0,'2021-09-19 12:06:28','2021-09-19 12:06:28',''),(260799632573251584,'18589305217','18589305217','6b39a6950e1eaf5a893421aebef93af2',1,'','','','','',0,'2021-09-20 00:04:38','2021-09-20 00:04:38',''),(260949503540248576,'13138506195','13138506195','6f4c82bacda47440581a42805f6b9d2a',1,'','','','','',0,'2021-09-20 10:00:09','2021-09-20 10:00:09',''),(260991092186726400,'15093597119','15093597119','201b23829a9f13fe7e552fcec01385fe',1,'','','','','',0,'2021-09-20 12:45:25','2021-09-20 12:45:25',''),(261344558310539264,'13375003650','13375003650','a5aba31c4edcf67eff02662a9783cfdc',1,'','','','','',0,'2021-09-21 12:09:58','2021-09-21 12:09:58',''),(261493011116244992,'13037690297','13037690297','2bdb0906ed99b1d7c023b6c8d46ad5d7',1,'','','','','',0,'2021-09-21 21:59:52','2021-09-21 21:59:52',''),(261897924371595264,'17877709330','17877709330','36e95cef52a67ee097aad7021d276f35',1,'','','','','',0,'2021-09-23 00:48:51','2021-09-23 00:48:51',''),(261921105736155136,'17389195347','17389195347','04ad1dacf04aaa697d7ac8f45fda8441',1,'','','','','',0,'2021-09-23 02:20:57','2021-09-23 02:20:57',''),(262049457834868736,'15904411147','15904411147','39887165e72d6f5bd2fae1db72d194ac',1,'','','','','',0,'2021-09-23 10:50:59','2021-09-23 10:50:59','18686479123'),(262576865553989632,'18370859733','18370859733','d7c8036b23d7004a2d0dd7ecbda796dd',1,'','','','','',0,'2021-09-24 21:46:43','2021-09-24 21:46:43',''),(263829502727340032,'15996258117','15996258117','d2f6c7cc7b1b38e3cb40be533a7d4699',1,'','','','','',0,'2021-09-28 08:44:14','2021-09-28 08:44:14','18686479123'),(264284588888866816,'13791975688','13791975688','657ddb3a68f2bd0d89f14c380d9365f9',1,'','','','','',0,'2021-09-29 14:52:35','2021-09-29 14:52:35',''),(269282178931933184,'13133333333','13133333333','1a100d2c0dab19c4430e7d73762b3423',1,'','','','','',0,'2021-10-13 09:51:14','2021-10-13 09:51:14','15948371897'),(269282862850949120,'13122222222','13122222222','e3ceb5881a0a1fdaad01296d7554868d',1,'','','','','',0,'2021-10-13 09:53:57','2021-10-13 09:53:57',''),(269284361731174400,'13144444444','','73882ab1fa529d7273da0db6b49cc4f3',1,'','','','','',0,'2021-10-13 09:59:56','2021-10-13 09:59:56','15948371897'),(269289729450885120,'13155555555','','5b1b68a9abf4d2cd155c81a9225fd158',1,'','','','','',0,'2021-10-13 10:21:14','2021-10-13 10:21:14','15948371897'),(269290449440915456,'13366666666','','f379eaf3c831b04de153469d1bec345e',1,'','','','','',0,'2021-10-13 10:24:06','2021-10-13 10:24:06',''),(272942340897030144,'13624320435','','c67b64e6647c581e06ce17f6545037bd',1,'','','','','',0,'2021-10-23 12:15:24','2021-10-23 12:15:24','18686479123'),(272946389184724992,'18505530467','','35bb892533566f0f52167b2ad212ac9b',1,'','','','','',0,'2021-10-23 12:31:29','2021-10-23 12:31:29','18686479123'),(273518872841469952,'13911236861','','558fecbaa840aff96e74a3238d968837',1,'','','','','',0,'2021-10-25 02:26:21','2021-10-25 02:26:21',''),(274732498835259392,'13012345678','AppStore测试','5bd2026f128662763c532f2f4b6f2476',1,'','','','','',0,'2021-10-28 10:48:52','2021-10-28 10:48:52','15948371897'),(274734549472428032,'13811111111','','96e79218965eb72c92a549dd5a330112',1,'','','','','',0,'2021-10-28 10:57:01','2021-10-28 10:57:01',''),(274784153177997312,'13827442810','','c56527b92d0bdc261f7d6263ed31932d',1,'','','','','',0,'2021-10-28 14:14:06','2021-10-28 14:14:06','13377777777'),(275055302055084032,'13039114473','','3984f923640c429238b2e5dd5cece912',1,'','','','','',0,'2021-10-29 08:11:34','2021-10-29 08:11:34','13578696916'),(275222463197265920,'13604332721','','837e0653b38856006c807d87e4b076db',1,'','','','','',0,'2021-10-29 19:15:49','2021-10-29 19:15:49','15948371897'),(275404791634771968,'13039114370','','4bfb1bab88868ee638b208b90e811443',1,'','','','','',0,'2021-10-30 07:20:19','2021-10-30 07:20:19','13039114473'),(275405106077548544,'18543101169','','cb8f30ef0e2bb1948deb92e0fd0ce0cc',1,'','','','','',0,'2021-10-30 07:21:34','2021-10-30 07:21:34','13039114473'),(275516071498530816,'18843013763','','cefc4e78243039937790afd014f3bbca',1,'','','','','',0,'2021-10-30 14:42:30','2021-10-30 14:42:30',''),(275934944160956416,'18444055551','','6014056ad00f177df70d9e17ab9b3535',1,'','','','','',0,'2021-10-31 18:26:57','2021-10-31 18:26:57',''),(276506770469601280,'13926911625','','dadc242e748fed2effec9b9a15ec57bd',1,'','','','','',0,'2021-11-02 08:19:11','2021-11-02 08:19:11',''),(278883822443216896,'13904317788','','d079acd676de87f3222cb9514c76fb62',1,'','','','','',0,'2021-11-08 21:44:44','2021-11-08 21:44:44','18686479123'),(281965162914955264,'13843189525','','2540885ae5a4b59308d5720273b058f9',1,'','','','','',0,'2021-11-17 09:48:53','2021-11-17 09:48:53','13578696916'),(293906625890402304,'13894031149','','7a345060957ea1b0691a47f863233a1b',1,'','','','','',0,'2021-12-20 08:39:59','2021-12-20 08:39:59',''),(295023405102841856,'13666698433','','52dd4bddd923b106aa257e0de6311d9e',1,'','','','','',0,'2021-12-23 10:37:41','2021-12-23 10:37:41',''),(301656900587798528,'18639302986','','4cfe05110e7adefd18621cf84c894997',1,'','','','','',0,'2022-01-10 17:56:49','2022-01-10 17:56:49',''),(307731076755013632,'18682262512','','a570bac1f50089ae02593276bc67eb60',1,'','','','','',0,'2022-01-27 12:13:26','2022-01-27 12:13:26','');
/*!40000 ALTER TABLE `lob_agent` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lob_agent_extra`
--

DROP TABLE IF EXISTS `lob_agent_extra`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `lob_agent_extra` (
  `ID` bigint(20) NOT NULL COMMENT '流水标识',
  `AgentID` bigint(20) NOT NULL COMMENT '代理人标识',
  `LowerID` bigint(20) NOT NULL COMMENT '下级代理人标识',
  `PatientID` bigint(20) NOT NULL COMMENT '患者标识',
  `Income` double(10,2) NOT NULL COMMENT '收入金额',
  `Ctime` datetime NOT NULL COMMENT '收入时间',
  `LowerName` varchar(255) NOT NULL COMMENT '下级代理人标题：手机号(姓名)，为显示的冗余',
  `PatientFeesID` bigint(20) NOT NULL COMMENT '患者收费流水标识',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='躺平收入';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lob_agent_extra`
--

LOCK TABLES `lob_agent_extra` WRITE;
/*!40000 ALTER TABLE `lob_agent_extra` DISABLE KEYS */;
INSERT INTO `lob_agent_extra` VALUES (1,224417791112769536,1,1,200.00,'2021-08-04 15:44:50','15589096785(王军)',0),(269659703738679296,224417791112769536,224367919756079104,0,200.00,'2021-10-13 00:00:00','15948371897 (洪道亭)',0);
/*!40000 ALTER TABLE `lob_agent_extra` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lob_agent_income`
--

DROP TABLE IF EXISTS `lob_agent_income`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `lob_agent_income` (
  `ID` bigint(20) NOT NULL COMMENT '流水标识',
  `AgentID` bigint(20) NOT NULL COMMENT '代理人标识',
  `PatientID` bigint(20) NOT NULL COMMENT '患者标识',
  `Income` double(10,2) NOT NULL COMMENT '收入金额',
  `Ctime` datetime NOT NULL COMMENT '收入时间',
  `PatientName` varchar(255) NOT NULL COMMENT '患者标题：手机号(姓名)，为显示的冗余',
  `PatientFeesID` bigint(20) NOT NULL COMMENT '患者收费流水标识',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='直接推荐收入';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lob_agent_income`
--

LOCK TABLES `lob_agent_income` WRITE;
/*!40000 ALTER TABLE `lob_agent_income` DISABLE KEYS */;
INSERT INTO `lob_agent_income` VALUES (1,224417791112769536,1,1000.00,'2021-06-29 15:39:47','15312342345(赵先生)',0),(2,224417791112769536,23,1100.00,'2021-07-25 15:42:06','13598089768',0);
/*!40000 ALTER TABLE `lob_agent_income` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lob_patient`
--

DROP TABLE IF EXISTS `lob_patient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `lob_patient` (
  `ID` bigint(20) NOT NULL COMMENT '患者标识',
  `Phone` char(11) NOT NULL COMMENT '手机号，唯一',
  `Name` varchar(32) NOT NULL COMMENT '姓名',
  `Sex` tinyint(4) unsigned NOT NULL COMMENT '#Gender#性别',
  `State` tinyint(4) unsigned NOT NULL COMMENT '#PatientState#患者状态',
  `Info` varchar(4000) NOT NULL COMMENT '描述信息',
  `Ctime` datetime NOT NULL COMMENT '注册时间',
  `AgentID` bigint(20) NOT NULL COMMENT '推荐人标识',
  PRIMARY KEY (`ID`),
  KEY `idx_patient_agentid` (`AgentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='患者';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lob_patient`
--

LOCK TABLES `lob_patient` WRITE;
/*!40000 ALTER TABLE `lob_patient` DISABLE KEYS */;
INSERT INTO `lob_patient` VALUES (241080948845572096,'13511111111','未知',1,0,'','2021-07-27 14:10:32',224417791112769536),(241103763959312384,'13322222222','未知1',1,1,'','2021-07-27 15:40:30',224417791112769536),(241103859329396736,'13533333333','王军',1,2,'','2021-07-27 15:40:56',224417791112769536),(241103959460016128,'13677777777','未知',1,3,'','2021-07-27 15:41:15',224417791112769536),(241104312167428096,'15388888888','未知',1,3,'','2021-07-27 15:43:11',224417791112769536),(241461871571234816,'13898765432','赵先生',1,3,'','2021-07-28 15:24:00',224417791112769536),(241748930873651200,'13266666666','张三',1,3,'','2021-07-29 10:24:56',224417791112769536),(241749283803361280,'13366666666','李先生',1,3,'','2021-07-29 10:26:00',224417791112769536),(241749492855861248,'13466666666','李女士',1,3,'','2021-07-29 10:26:59',224417791112769536),(241829997731246080,'15948371897','',0,0,'','2021-07-29 15:46:05',224417791112769536),(241843802989260800,'13811111111','',0,0,'','2021-07-29 16:40:54',224417791112769536),(242078216620994560,'15411111111','',0,0,'','2021-07-30 08:12:25',224417791112769536),(242094170117632000,'13212345678','',0,0,'','2021-07-30 09:15:49',224417791112769536),(242101148298637312,'13578696916','',0,0,'','2021-07-30 09:43:33',224417791112769536),(242157728222277632,'13355878979','',0,0,'','2021-07-30 13:28:22',224417791112769536),(243582523111501824,'13888888888','',0,0,'','2021-08-03 11:50:00',224417791112769536),(243638945937932288,'宋先生(13888)','',0,0,'','2021-08-03 15:34:11',243635822079033344),(243656880010084352,'13888888881','',0,0,'','2021-08-03 16:45:27',224417791112769536),(244360420705746944,'13258694567','',0,0,'','2021-08-05 15:21:03',224417791112769536),(244720013881425920,'15712345678','',0,0,'','2021-08-06 15:09:58',224417791112769536),(244720200972550144,'15611111111','',0,0,'','2021-08-06 15:10:42',224417791112769536),(244999209681731584,'13801132450','',0,0,'','2021-08-07 09:39:23',0),(244999440917905408,'18686479123','',0,0,'','2021-08-07 09:40:19',224417791112769536),(245711568654090240,'13222222222','',0,0,'','2021-08-09 08:50:02',224417791112769536),(245791413241167872,'13812345678','',0,0,'','2021-08-09 14:07:19',224417791112769536),(245791534997618688,'13578695644','',0,0,'','2021-08-09 14:07:48',224417791112769536),(245791634599755776,'13578694444','',0,0,'','2021-08-09 14:08:12',224417791112769536),(246546726391169024,'15604483365','',0,0,'','2021-08-11 16:08:40',224417791112769536),(249413818970849280,'13578696988','',0,0,'','2021-08-19 14:01:28',249412886019227648),(249783206987218944,'13234305588','',0,0,'','2021-08-20 14:29:17',249782495759089664),(249783301149343744,'13245883588','',0,0,'','2021-08-20 14:29:39',249782495759089664),(251176335246147584,'13478978956','',0,0,'','2021-08-24 10:45:05',224417791112769536),(253781699850452992,'13185231456','',0,0,'','2021-08-31 15:17:52',224417791112769536),(254069759699894272,'13025420401','',0,0,'','2021-09-01 10:22:31',254069006742634496),(254566678746415104,'17875821602','',0,0,'','2021-09-02 19:17:05',254566454892216320),(254848808135213056,'13801183520','',0,0,'','2021-09-03 13:58:10',243635822079033344),(255528057523453952,'15714405555','',0,0,'','2021-09-05 10:57:16',224417791112769536),(258102393325273088,'13894941330','',0,0,'','2021-09-12 13:26:45',258101798598131712),(258746776093507584,'13696325689','',0,0,'','2021-09-14 08:07:18',224417791112769536),(258768162874769408,'15584464333','',0,0,'','2021-09-14 09:32:17',257303201296203776),(260417376566427648,'17603200038','',0,0,'','2021-09-18 22:45:40',260416930728689664),(260619138166472704,'13680096638','',0,0,'','2021-09-19 12:07:24',260618901385428992),(261921532867297280,'17389195347','',0,0,'','2021-09-23 02:22:39',261921105736155136),(263827066440695808,'15996258117','',0,0,'','2021-09-28 08:34:33',258101798598131712),(272951867704918016,'13514484505','',0,0,'','2021-10-23 12:53:15',272946389184724992);
/*!40000 ALTER TABLE `lob_patient` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lob_patient_fees`
--

DROP TABLE IF EXISTS `lob_patient_fees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `lob_patient_fees` (
  `ID` bigint(20) NOT NULL COMMENT '流水标识',
  `PatientID` bigint(20) NOT NULL COMMENT '患者标识',
  `Fees` double(10,2) NOT NULL COMMENT '费用',
  `FeesTime` datetime NOT NULL COMMENT '缴费日期',
  `Info` varchar(255) NOT NULL COMMENT '缴费说明',
  PRIMARY KEY (`ID`),
  KEY `idx_fees_PatientID` (`PatientID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='患者收费';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lob_patient_fees`
--

LOCK TABLES `lob_patient_fees` WRITE;
/*!40000 ALTER TABLE `lob_patient_fees` DISABLE KEYS */;
/*!40000 ALTER TABLE `lob_patient_fees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lob_sql`
--

DROP TABLE IF EXISTS `lob_sql`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `lob_sql` (
  `id` varchar(128) NOT NULL COMMENT 'sql键值',
  `sql` varchar(20000) NOT NULL COMMENT 'sql内容',
  `note` varchar(255) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lob_sql`
--

LOCK TABLES `lob_sql` WRITE;
/*!40000 ALTER TABLE `lob_sql` DISABLE KEYS */;
INSERT INTO `lob_sql` VALUES ('agent-卡信息','select\r\n	AccountName,\r\n	Bank,\r\n	Account,\r\n	BankPhone \r\nfrom\r\n	lob_agent \r\nWHERE\r\n	id = @id',NULL),('agent-总收入','select\r\n	sum( income ) total,\r\n	count( 1 ) cnt\r\nfrom\r\n	lob_agent_income \r\nwhere\r\n	AgentID = @AgentID',NULL),('agent-收入明细','select\r\n	* \r\nfrom\r\n	lob_agent_income \r\nwhere\r\n	AgentID = @AgentID \r\norder by\r\n	ctime desc',NULL),('agent-编辑','select\r\n	id,\r\n	AccountName,\r\n	Bank,\r\n	Account,\r\n	BankPhone \r\nfrom\r\n	lob_agent \r\nWHERE\r\n	id = @id',NULL),('agent-躺平收入','select\r\n	sum( income ) \r\nfrom\r\n	lob_agent_extra\r\nwhere\r\n	AgentID = @AgentID',NULL),('agent-躺平收入分组','select\r\n	LowerName,\r\n	CONCAT( \'共推荐\', cast(count( 1 ) as CHAR), \'位患者，贡献收入¥\', cast(sum( ceil(income) )as CHAR) ) info,\r\n	LowerID\r\nfrom\r\n	lob_agent_extra \r\nwhere\r\n	AgentID = @AgentID \r\nGROUP BY\r\n	LowerID',NULL),('agent-躺平收入明细','select\r\n	Ctime \r\nfrom\r\n	lob_agent_extra \r\nwhere\r\n	AgentID = @AgentID \r\n	and LowerID = @LowerID \r\nORDER BY\r\n	Ctime desc',NULL),('agent-近30天收入','select\r\n	sum( income ) \r\nfrom\r\n	lob_agent_income \r\nwhere\r\n	AgentID = @AgentID \r\n	and ctime >= DATE_SUB( CURDATE( ), INTERVAL 30 DAY )',NULL),('agent-近30天收入明细','select\r\n	* \r\nfrom\r\n	lob_agent_income \r\nwhere\r\n	AgentID = @AgentID \r\n	and ctime >= DATE_SUB( CURDATE( ), INTERVAL 30 DAY ) \r\norder by\r\n	ctime desc',NULL),('代理人-全部','select\r\n	*\r\nfrom\r\n	lob_agent',NULL),('代理人-关联下线','select\r\n	b.* \r\nfrom\r\n	lob_agent a,\r\n	lob_agent b \r\nWHERE\r\n	a.Phone = b.LeaderPhone \r\n	and a.id = @id',NULL),('代理人-关联患者','select id,Phone,Name from lob_patient where AgentID=@AgentID',NULL),('代理人-关联推荐收入','select * from lob_agent_income where AgentID=@AgentID',NULL),('代理人-关联监控','select * from log_usertrace where AgentID=@AgentID',NULL),('代理人-关联躺平收入','select * from lob_agent_extra where AgentID=@AgentID',NULL),('代理人-无下级代理','select\r\n	* \r\nfrom\r\n	lob_agent a \r\nwhere\r\n	NOT EXISTS ( SELECT ID FROM `lob_agent` b WHERE b.LeaderPhone = a.Phone )',NULL),('代理人-无收入','select\r\n	* \r\nfrom\r\n	lob_agent a \r\nwhere\r\n	NOT EXISTS ( SELECT AgentID FROM `lob_agent_income` b WHERE a.ID = b.AgentID )',NULL),('代理人-无银行账号','select\r\n	* \r\nfrom\r\n	lob_agent \r\nWHERE\r\n	Account = \'\'',NULL),('代理人-有收入','select\r\n	* \r\nfrom\r\n	lob_agent a \r\nwhere\r\n	EXISTS ( SELECT AgentID FROM `lob_agent_income` b WHERE a.ID = b.AgentID )',NULL),('代理人-未推荐患者','select\r\n	* \r\nfrom\r\n	lob_agent a \r\nwhere\r\n	NOT EXISTS ( SELECT AgentID FROM `lob_patient` b WHERE a.ID = b.AgentID )',NULL),('代理人-模糊查询','SELECT\r\n	*\r\nFROM\r\n	lob_agent\r\nWHERE\r\n	NAME LIKE @input \r\n	OR phone LIKE @input',NULL),('代理人-编辑','\r\nselect * from lob_agent \r\nWHERE\r\n	id = @id',NULL),('代理人-重复手机号','select count(id) from lob_agent where phone=@phone',NULL),('患者-全部','select * from lob_patient',NULL),('患者-关联收费','select * from lob_patient_fees where PatientID=@PatientID order by FeesTime',NULL),('患者-模糊查询','select * from lob_patient\r\nWHERE\r\n	phone LIKE @input',NULL),('患者-状态人数','select count(*) from lob_patient where state=@state',NULL),('患者-状态查询','select * from lob_patient where state=@state',NULL),('患者-编辑','select\r\n	a.*,\r\n	b.phone agentphone,\r\n	b.name agentname \r\nfrom\r\n	lob_patient a\r\n	LEFT JOIN lob_agent b on a.AgentID = b.id \r\nwhere\r\n	a.id = @id',NULL),('患者-重复手机号','select count(id) from lob_patient where phone=@phone',NULL),('推荐收入-编辑','select * from lob_agent_income where ID=@ID',NULL);
/*!40000 ALTER TABLE `lob_sql` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `log_usertrace`
--

DROP TABLE IF EXISTS `log_usertrace`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `log_usertrace` (
  `AgentID` bigint(20) NOT NULL COMMENT '代理人标识',
  `Trace` tinyint(4) unsigned NOT NULL COMMENT '#TraceKind#跟踪类别',
  `Clicks` bigint(20) NOT NULL COMMENT '使用次数',
  `LastTime` datetime NOT NULL COMMENT '最近使用时间',
  PRIMARY KEY (`AgentID`,`Trace`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `log_usertrace`
--

LOCK TABLES `log_usertrace` WRITE;
/*!40000 ALTER TABLE `log_usertrace` DISABLE KEYS */;
INSERT INTO `log_usertrace` VALUES (224367919756079104,0,108,'2021-11-10 15:38:04'),(224367919756079104,1,5,'2021-11-10 15:38:06'),(224367919756079104,2,6,'2021-10-29 08:08:24'),(224367919756079104,3,48,'2021-11-10 15:38:08'),(224367919756079104,4,4,'2021-10-29 08:08:32'),(224367919756079104,5,1,'2021-08-13 16:31:03'),(224367919756079104,6,27,'2021-10-29 09:32:34'),(224417791112769536,0,371,'2021-11-15 15:22:42'),(224417791112769536,1,130,'2021-10-28 10:19:30'),(224417791112769536,2,168,'2021-11-15 15:17:45'),(224417791112769536,3,254,'2021-11-15 15:22:48'),(224417791112769536,4,174,'2021-10-28 10:23:10'),(224417791112769536,5,50,'2021-10-22 16:52:39'),(224417791112769536,6,90,'2021-10-28 13:00:46'),(243635822079033344,0,25,'2022-01-20 12:13:40'),(243635822079033344,1,13,'2021-11-23 20:39:01'),(243635822079033344,2,28,'2022-01-04 10:08:23'),(243635822079033344,3,44,'2022-01-04 10:08:30'),(243635822079033344,4,15,'2021-11-23 20:39:20'),(243635822079033344,5,4,'2021-09-03 09:37:23'),(243635822079033344,6,9,'2021-11-23 20:39:28'),(246105722575515648,0,7,'2021-08-10 11:46:49'),(246105722575515648,1,1,'2021-08-10 10:56:50'),(246105722575515648,2,1,'2021-08-10 10:56:52'),(246105722575515648,3,6,'2021-08-10 11:39:12'),(246105722575515648,6,2,'2021-08-10 11:46:56'),(246118538003398656,0,1,'2021-08-10 11:56:59'),(246118538003398656,3,1,'2021-08-10 11:47:49'),(246118538003398656,6,1,'2021-08-10 11:57:06'),(246533377737670656,0,7,'2021-08-13 16:25:13'),(246533377737670656,1,1,'2021-08-13 16:09:02'),(246533377737670656,2,3,'2021-08-13 16:16:03'),(246533377737670656,3,3,'2021-08-13 16:17:25'),(246533377737670656,4,2,'2021-08-13 16:16:48'),(246533377737670656,5,1,'2021-08-13 16:16:56'),(246533377737670656,6,2,'2021-08-13 16:25:22'),(248974790999535616,1,1,'2021-08-18 08:57:26'),(248974790999535616,2,3,'2021-08-18 08:57:28'),(248974790999535616,3,10,'2021-08-18 11:17:43'),(248974790999535616,4,1,'2021-08-18 09:32:25'),(249010538368856064,0,30,'2021-08-24 08:15:12'),(249010538368856064,1,6,'2021-08-23 10:44:32'),(249010538368856064,2,9,'2021-08-23 10:44:35'),(249010538368856064,3,31,'2021-08-24 08:16:10'),(249010538368856064,4,4,'2021-08-23 10:45:00'),(249010538368856064,5,7,'2021-08-23 10:45:07'),(249010538368856064,6,8,'2021-08-24 08:16:13'),(249412886019227648,0,7,'2021-08-19 15:00:05'),(249412886019227648,1,4,'2021-08-19 14:18:05'),(249412886019227648,2,2,'2021-08-19 15:00:08'),(249412886019227648,3,8,'2021-08-19 14:51:08'),(249412886019227648,4,3,'2021-08-19 14:16:36'),(249412886019227648,5,2,'2021-08-19 14:36:15'),(249412886019227648,6,2,'2021-08-19 14:58:58'),(249782495759089664,0,2,'2021-08-20 15:13:47'),(249782495759089664,1,1,'2021-08-20 14:28:44'),(249782495759089664,2,2,'2021-08-20 14:29:42'),(249782495759089664,3,1,'2021-08-20 14:29:45'),(249782495759089664,4,2,'2021-08-20 14:31:01'),(249782495759089664,5,1,'2021-08-20 14:31:10'),(249782495759089664,6,2,'2021-08-20 15:13:54'),(249794494031249408,0,2,'2021-08-22 16:43:12'),(249794494031249408,1,3,'2021-08-21 07:54:48'),(249794494031249408,2,3,'2021-08-22 16:43:22'),(249794494031249408,3,4,'2021-08-22 16:43:38'),(249794494031249408,6,1,'2021-08-22 16:43:54'),(250833019686141952,0,2,'2021-08-24 13:22:50'),(250833019686141952,2,1,'2021-08-23 12:01:19'),(250833019686141952,3,1,'2021-08-23 12:01:14'),(250833019686141952,4,1,'2021-08-23 12:01:18'),(251228090524811264,6,1,'2021-08-24 14:11:04'),(251651375339192320,0,1,'2021-08-25 18:12:43'),(253384432689659904,0,1,'2021-08-30 12:59:16'),(253384432689659904,3,1,'2021-08-30 13:00:21'),(253384432689659904,6,1,'2021-08-30 13:00:48'),(253424751166668800,6,1,'2021-08-30 15:40:04'),(253428440090591232,2,1,'2021-08-30 15:54:57'),(253428440090591232,3,1,'2021-08-30 15:54:39'),(253429796109074432,2,1,'2021-08-30 16:00:40'),(253429796109074432,3,2,'2021-08-30 16:00:46'),(253429796109074432,6,2,'2021-08-30 16:00:53'),(254069006742634496,0,1,'2021-09-01 10:19:31'),(254069006742634496,1,9,'2021-09-01 10:28:45'),(254069006742634496,2,7,'2021-09-01 10:27:29'),(254069006742634496,3,5,'2021-09-01 10:28:47'),(254069006742634496,4,7,'2021-09-01 10:28:53'),(254069006742634496,5,4,'2021-09-01 10:29:02'),(254069006742634496,6,4,'2021-09-01 10:29:00'),(254566454892216320,0,1,'2021-09-02 19:16:12'),(254566454892216320,1,3,'2021-09-02 19:18:52'),(254566454892216320,2,3,'2021-09-02 19:17:10'),(254566454892216320,3,3,'2021-09-02 19:19:00'),(254566454892216320,4,3,'2021-09-02 19:18:29'),(254566454892216320,5,2,'2021-09-02 19:18:43'),(254566454892216320,6,3,'2021-09-02 19:19:05'),(254811010451685376,0,1,'2021-09-03 11:27:59'),(254857567955894272,0,1,'2021-09-03 14:32:59'),(254857567955894272,1,1,'2021-09-03 14:34:48'),(254857567955894272,2,1,'2021-09-03 14:36:14'),(254857567955894272,3,3,'2021-09-03 14:35:21'),(254857567955894272,4,1,'2021-09-03 14:35:49'),(254857567955894272,5,1,'2021-09-03 14:35:46'),(254857567955894272,6,3,'2021-09-03 14:35:57'),(254860856827101184,0,12,'2021-11-14 20:23:06'),(254860856827101184,1,5,'2021-09-10 08:11:49'),(254860856827101184,2,10,'2021-09-22 10:10:21'),(254860856827101184,3,16,'2021-11-12 10:17:51'),(254860856827101184,4,7,'2021-09-22 10:10:23'),(254860856827101184,5,3,'2021-09-22 10:10:27'),(254860856827101184,6,4,'2021-09-13 08:25:41'),(257303201296203776,0,8,'2021-12-10 10:01:48'),(257303201296203776,1,4,'2021-11-03 09:12:10'),(257303201296203776,2,5,'2021-12-10 10:01:51'),(257303201296203776,3,6,'2021-11-03 09:12:04'),(257303201296203776,4,7,'2021-11-03 09:12:02'),(257303201296203776,5,4,'2021-11-03 09:12:00'),(257303201296203776,6,4,'2021-11-03 09:11:59'),(258101798598131712,0,11,'2021-11-09 22:40:40'),(258101798598131712,1,9,'2021-10-23 12:10:51'),(258101798598131712,2,5,'2021-10-23 12:10:30'),(258101798598131712,3,10,'2021-10-23 12:11:13'),(258101798598131712,4,1,'2021-09-15 08:51:47'),(258101798598131712,5,1,'2021-09-26 13:04:43'),(258101798598131712,6,2,'2021-09-15 08:52:04'),(258806962111954944,0,2,'2021-09-14 14:26:46'),(258806962111954944,1,2,'2021-09-14 14:27:26'),(258806962111954944,2,3,'2021-09-14 14:29:03'),(258806962111954944,3,3,'2021-09-14 14:29:10'),(258806962111954944,4,4,'2021-09-14 14:29:20'),(258806962111954944,5,3,'2021-09-14 14:29:18'),(258806962111954944,6,3,'2021-09-14 14:29:15'),(259488846328414208,0,1,'2021-09-16 09:16:02'),(260274754136883200,0,1,'2021-09-18 13:18:57'),(260284648621654016,0,2,'2021-09-18 13:58:59'),(260284648621654016,1,1,'2021-09-18 13:59:03'),(260284648621654016,2,1,'2021-09-18 13:59:13'),(260284648621654016,5,1,'2021-09-18 13:59:08'),(260285708077678592,0,3,'2021-10-17 00:31:37'),(260285708077678592,1,1,'2021-10-13 09:36:01'),(260285708077678592,2,1,'2021-10-13 09:36:07'),(260285708077678592,5,1,'2021-10-13 09:36:05'),(260294959554674688,0,2,'2021-09-18 14:41:38'),(260294959554674688,1,1,'2021-09-18 14:41:44'),(260294959554674688,2,1,'2021-09-18 14:41:50'),(260294959554674688,5,1,'2021-09-18 14:41:59'),(260294959554674688,6,1,'2021-09-18 14:42:02'),(260295091532644352,0,1,'2021-09-18 14:39:45'),(260343217559748608,0,2,'2021-09-18 17:51:29'),(260343217559748608,2,1,'2021-09-18 17:51:39'),(260343217559748608,3,1,'2021-09-18 17:51:47'),(260343217559748608,6,1,'2021-09-18 17:51:34'),(260416930728689664,0,2,'2021-09-18 22:45:07'),(260416930728689664,1,2,'2021-09-18 22:45:50'),(260416930728689664,2,2,'2021-09-18 22:45:44'),(260416930728689664,3,1,'2021-09-18 22:45:23'),(260416930728689664,4,2,'2021-09-18 22:46:02'),(260416930728689664,5,1,'2021-09-18 22:45:48'),(260420117594423296,0,1,'2021-09-18 22:56:34'),(260436375761108992,0,1,'2021-09-19 00:01:10'),(260451705904480256,0,1,'2021-09-19 01:02:05'),(260617057535836160,0,1,'2021-09-19 11:59:08'),(260618901385428992,0,2,'2021-09-19 12:07:07'),(260618901385428992,1,1,'2021-09-19 12:07:10'),(260618901385428992,2,1,'2021-09-19 12:07:14'),(260618901385428992,3,1,'2021-09-19 12:07:27'),(260618901385428992,4,1,'2021-09-19 12:07:31'),(260799632573251584,0,1,'2021-09-20 00:04:37'),(260949503540248576,0,1,'2021-09-20 10:00:09'),(260991092186726400,0,1,'2021-09-20 12:45:25'),(261344558310539264,0,1,'2021-09-21 12:09:58'),(261493011116244992,0,2,'2021-09-21 22:01:09'),(261493011116244992,1,1,'2021-09-21 22:01:16'),(261493011116244992,2,1,'2021-09-21 22:01:27'),(261493011116244992,4,1,'2021-09-21 22:01:35'),(261493011116244992,5,1,'2021-09-21 22:01:31'),(261493011116244992,6,3,'2021-09-21 22:01:48'),(261897924371595264,0,1,'2021-09-23 00:48:51'),(261921105736155136,0,3,'2021-09-23 02:23:14'),(261921105736155136,1,1,'2021-09-23 02:21:55'),(261921105736155136,2,1,'2021-09-23 02:22:05'),(261921105736155136,3,1,'2021-09-23 02:22:00'),(261921105736155136,4,1,'2021-09-23 02:22:20'),(261921105736155136,5,1,'2021-09-23 02:22:15'),(261921105736155136,6,2,'2021-09-23 02:22:19'),(262049457834868736,0,2,'2021-09-24 03:41:01'),(262049457834868736,2,1,'2021-09-23 10:51:17'),(262049457834868736,3,2,'2021-09-23 10:51:28'),(262576865553989632,0,3,'2021-09-24 21:50:08'),(262576865553989632,1,2,'2021-09-24 21:47:41'),(262576865553989632,2,1,'2021-09-24 21:47:31'),(262576865553989632,3,1,'2021-09-24 21:47:23'),(262576865553989632,4,2,'2021-09-24 21:47:54'),(262576865553989632,5,1,'2021-09-24 21:47:37'),(262576865553989632,6,2,'2021-09-24 21:50:11'),(263829502727340032,0,2,'2021-09-30 21:36:18'),(263829502727340032,1,1,'2021-09-28 08:44:57'),(263829502727340032,3,1,'2021-09-28 08:44:46'),(264284588888866816,0,2,'2021-09-29 14:53:05'),(264284588888866816,1,1,'2021-09-29 14:53:10'),(264284588888866816,2,1,'2021-09-29 14:53:12'),(269282178931933184,0,1,'2021-10-13 09:51:14'),(269282178931933184,6,1,'2021-10-13 09:53:24'),(269282862850949120,0,2,'2021-10-13 09:59:30'),(269282862850949120,6,1,'2021-10-13 09:59:32'),(269284361731174400,0,2,'2021-10-13 10:20:43'),(269284361731174400,6,1,'2021-10-13 10:20:48'),(269289729450885120,0,2,'2021-10-13 10:23:41'),(269289729450885120,6,1,'2021-10-13 10:23:43'),(269290449440915456,0,4,'2021-10-14 08:59:40'),(269290449440915456,3,1,'2021-10-14 08:59:42'),(269290449440915456,6,1,'2021-10-14 08:59:49'),(272942340897030144,0,2,'2021-12-31 09:33:18'),(272942340897030144,4,1,'2021-12-31 09:33:32'),(272946389184724992,0,2,'2021-10-24 08:15:43'),(272946389184724992,1,2,'2021-10-24 08:17:04'),(272946389184724992,2,3,'2021-10-24 08:17:07'),(272946389184724992,3,5,'2021-10-24 08:17:00'),(272946389184724992,4,2,'2021-10-24 08:16:50'),(272946389184724992,5,1,'2021-10-23 12:54:22'),(272946389184724992,6,1,'2021-10-24 08:16:47'),(273518872841469952,0,4,'2021-10-25 02:30:53'),(273518872841469952,1,1,'2021-10-25 02:27:39'),(273518872841469952,2,1,'2021-10-25 02:28:27'),(273518872841469952,3,2,'2021-10-25 02:31:18'),(273518872841469952,4,4,'2021-10-25 02:31:08'),(273518872841469952,5,1,'2021-10-25 02:27:50'),(273518872841469952,6,2,'2021-10-25 02:31:01'),(274732498835259392,0,5,'2021-10-29 09:32:45'),(274732498835259392,1,2,'2021-10-28 13:01:09'),(274732498835259392,2,6,'2021-10-28 21:20:18'),(274732498835259392,3,11,'2021-10-29 09:32:55'),(274732498835259392,4,3,'2021-10-28 21:20:14'),(274732498835259392,5,2,'2021-10-28 13:01:23'),(274732498835259392,6,5,'2021-10-29 09:33:02'),(274734549472428032,0,2,'2021-10-28 11:25:27'),(274734549472428032,3,1,'2021-10-28 11:25:50'),(274734549472428032,6,1,'2021-10-28 11:25:54'),(274784153177997312,0,1,'2021-10-28 14:14:07'),(274784153177997312,1,1,'2021-10-28 14:14:52'),(274784153177997312,3,1,'2021-10-28 14:14:50'),(274784153177997312,4,2,'2021-10-28 14:14:56'),(274784153177997312,5,1,'2021-10-28 14:14:48'),(274784153177997312,6,1,'2021-10-28 14:14:45'),(275055302055084032,0,2,'2021-10-30 07:20:41'),(275055302055084032,1,1,'2021-10-29 08:14:11'),(275055302055084032,2,2,'2021-10-29 08:15:19'),(275055302055084032,3,3,'2021-10-29 08:15:44'),(275055302055084032,4,4,'2021-10-29 08:15:49'),(275055302055084032,5,2,'2021-10-29 08:15:51'),(275055302055084032,6,4,'2021-10-30 07:20:43'),(275222463197265920,0,1,'2021-10-29 19:15:48'),(275222463197265920,1,1,'2021-10-29 19:16:18'),(275222463197265920,2,1,'2021-10-29 19:16:26'),(275222463197265920,3,1,'2021-10-29 19:16:35'),(275404791634771968,0,1,'2021-10-30 07:20:19'),(275404791634771968,1,1,'2021-10-30 07:23:29'),(275404791634771968,2,1,'2021-10-30 07:22:43'),(275404791634771968,3,4,'2021-10-30 07:23:31'),(275404791634771968,4,3,'2021-10-30 07:23:41'),(275404791634771968,5,2,'2021-10-30 07:23:35'),(275404791634771968,6,1,'2021-10-30 07:23:24'),(275405106077548544,0,2,'2021-11-17 21:29:38'),(275405106077548544,1,1,'2021-11-17 21:29:40'),(275405106077548544,2,1,'2021-11-17 21:29:44'),(275405106077548544,3,2,'2021-11-17 21:29:47'),(275405106077548544,4,1,'2021-11-17 21:29:52'),(275405106077548544,5,1,'2021-11-17 21:29:56'),(275405106077548544,6,1,'2021-11-17 21:29:59'),(275516071498530816,0,1,'2021-10-30 14:42:30'),(275934944160956416,0,3,'2021-10-31 19:24:41'),(275934944160956416,1,2,'2021-10-31 19:24:51'),(275934944160956416,2,1,'2021-10-31 18:30:40'),(275934944160956416,3,2,'2021-10-31 19:24:54'),(275934944160956416,4,2,'2021-10-31 18:29:35'),(275934944160956416,5,1,'2021-10-31 19:25:01'),(275934944160956416,6,2,'2021-10-31 19:24:58'),(276506770469601280,0,1,'2021-11-02 08:19:11'),(278883822443216896,0,1,'2021-11-08 21:44:44'),(278883822443216896,1,1,'2021-11-08 21:49:36'),(278883822443216896,4,2,'2021-11-08 21:49:55'),(278883822443216896,6,1,'2021-11-08 21:49:49'),(281965162914955264,0,2,'2022-01-09 05:25:16'),(281965162914955264,1,2,'2021-11-17 09:50:44'),(281965162914955264,2,1,'2021-11-17 09:50:22'),(281965162914955264,3,1,'2021-11-17 09:50:33'),(281965162914955264,4,2,'2021-11-17 09:50:48'),(281965162914955264,5,1,'2021-11-17 09:50:36'),(281965162914955264,6,1,'2021-11-17 09:50:42'),(293906625890402304,0,2,'2021-12-20 08:41:06'),(293906625890402304,1,1,'2021-12-20 08:41:10'),(293906625890402304,3,1,'2021-12-20 08:41:18'),(293906625890402304,4,1,'2021-12-20 08:42:10'),(293906625890402304,5,1,'2021-12-20 08:42:23'),(293906625890402304,6,1,'2021-12-20 08:42:27'),(295023405102841856,0,2,'2021-12-23 10:38:45'),(295023405102841856,1,1,'2021-12-23 10:39:48'),(295023405102841856,2,1,'2021-12-23 10:39:37'),(295023405102841856,3,1,'2021-12-23 10:39:40'),(295023405102841856,6,1,'2021-12-23 10:39:33'),(301656900587798528,0,2,'2022-01-10 18:04:38'),(301656900587798528,1,1,'2022-01-10 18:04:44'),(301656900587798528,2,1,'2022-01-10 18:04:46'),(301656900587798528,3,2,'2022-01-10 18:04:51'),(301656900587798528,5,1,'2022-01-10 18:04:54'),(301656900587798528,6,1,'2022-01-10 18:04:42'),(307731076755013632,0,3,'2022-01-27 12:15:33');
/*!40000 ALTER TABLE `log_usertrace` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `oa_收文`
--

DROP TABLE IF EXISTS `oa_收文`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `oa_收文` (
  `ID` bigint(20) NOT NULL,
  `来文单位` varchar(255) NOT NULL,
  `来文时间` date NOT NULL,
  `密级` tinyint(4) unsigned NOT NULL COMMENT '#密级#',
  `文件标题` varchar(255) NOT NULL,
  `文件附件` varchar(512) NOT NULL,
  `市场部经理意见` varchar(255) NOT NULL,
  `综合部经理意见` varchar(255) NOT NULL,
  `收文完成时间` date NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `oa_收文`
--

LOCK TABLES `oa_收文` WRITE;
/*!40000 ALTER TABLE `oa_收文` DISABLE KEYS */;
INSERT INTO `oa_收文` VALUES (162025231350624256,'','2020-12-21',0,'a','','','','0001-01-01'),(162401333600448512,'','2020-12-22',0,'关于新冠疫情的批示','','','','0001-01-01');
/*!40000 ALTER TABLE `oa_收文` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `oa_goods`
--

DROP TABLE IF EXISTS `oa_goods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `oa_goods` (
  `ID` bigint(20) NOT NULL,
  `ParentID` bigint(20) NOT NULL COMMENT '所属购物',
  `Name` varchar(255) NOT NULL COMMENT '物品名称',
  PRIMARY KEY (`ID`),
  KEY `pk_shoppingid` (`ParentID`),
  CONSTRAINT `pk_shoppingid` FOREIGN KEY (`ParentID`) REFERENCES `oa_shopping` (`ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `oa_goods`
--

LOCK TABLES `oa_goods` WRITE;
/*!40000 ALTER TABLE `oa_goods` DISABLE KEYS */;
INSERT INTO `oa_goods` VALUES (260252331202703360,260235672052166656,'ddd'),(260252353671589888,260235672052166656,'bfd'),(260252716277559296,260252635210051584,'bder23');
/*!40000 ALTER TABLE `oa_goods` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `oa_shopping`
--

DROP TABLE IF EXISTS `oa_shopping`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `oa_shopping` (
  `ID` bigint(20) NOT NULL,
  `Reason` varchar(255) NOT NULL COMMENT '事由',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `oa_shopping`
--

LOCK TABLES `oa_shopping` WRITE;
/*!40000 ALTER TABLE `oa_shopping` DISABLE KEYS */;
INSERT INTO `oa_shopping` VALUES (260235275409420288,'abcd'),(260235672052166656,'222'),(260252635210051584,'a2324');
/*!40000 ALTER TABLE `oa_shopping` ENABLE KEYS */;
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
INSERT INTO `sequence` VALUES ('sq_menu',87),('sq_option',1020),('sq_post',161),('sq_res',1022),('sq_wfd_prc',11),('sq_wfi_item',162),('sq_wfi_prc',59);
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

-- Dump completed on 2022-01-27 12:20:53

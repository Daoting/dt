#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
using System.ComponentModel;
#endregion

namespace Dt.Sample
{
    public static class TvData
    {
        static Table _tbl;
        static MedTree _tree;

        public static Table GetTbl()
        {
            if (_tbl != null)
                return _tbl;

            _tbl = new Table
            {
                { "id" },
                { "parentid" },
                { "name" },
                { "code" },
            };

            _tbl.NewRow("421", "", "标价", "BJ");
            _tbl.NewRow("722", "", "凯旋院区老干部用药", "KXYQLGBYY");
            _tbl.NewRow("788", "", "其它", "QT");
            _tbl.NewRow("803", "", "解热镇痛抗炎抗风湿药及抗变态反应药", "JRZTKYKFSY");
            _tbl.NewRow("807", "", "内脏各系统药物", "NZGXTYW");
            _tbl.NewRow("839", "", "解毒药", "JDY");
            _tbl.NewRow("842", "", "专科用药", "ZKYY");
            _tbl.NewRow("849", "", "诊断用药", "ZDYY");
            _tbl.NewRow("869", "", "激素类及相关药物", "JSLJXGYW");
            _tbl.NewRow("887", "", "水、电解质和酸碱平衡药物及透析液", "SDJZHSJPHY");
            _tbl.NewRow("102", "", "测试", "CS");
            _tbl.NewRow("916", "", "营养与微量元素", "YYYWLYS");
            _tbl.NewRow("930", "", "抗肿瘤.抗病毒及免疫抑制药", "KZL.KBDJMY");
            _tbl.NewRow("104", "", "抗生素", "KSS");
            _tbl.NewRow("1161", "", "测试用药", "CSYY");
            _tbl.NewRow("115", "", "博思专用", "BSZY");
            _tbl.NewRow("181", "", "测试部专用", "CSBZY");
            _tbl.NewRow("201", "", "实施专用", "SSZY");
            _tbl.NewRow("241", "", "仙丹类", "XDL");
            _tbl.NewRow("281", "", "心脏病神药", "XZBSY");
            _tbl.NewRow("401", "", "外周神经系统药", "WZSJXTY");
            _tbl.NewRow("405", "", "全身麻醉药", "QSMZY");
            _tbl.NewRow("404", "", "中枢神经系统药", "ZSSJXTY");
            _tbl.NewRow("422", "421", "抗微生物药物", "KWSWYW");
            _tbl.NewRow("804", "803", "解热镇痛抗炎抗风湿药", "JRZTKYKFSY");
            _tbl.NewRow("805", "803", "抗变态反应药", "KBTFYY");
            _tbl.NewRow("806", "803", "抗痛风药", "KTFY");
            _tbl.NewRow("808", "807", "调脂药", "DZY");
            _tbl.NewRow("809", "807", "治疗心衰药、抗休克药", "ZLXSYKXKY");
            _tbl.NewRow("810", "807", "抗血栓药", "KXSY");
            _tbl.NewRow("815", "807", "抗高血压药", "KGXYY");
            _tbl.NewRow("821", "807", "抗心律失常类", "KXLSCL");
            _tbl.NewRow("822", "807", "防治冠心病、心绞痛药及其他心脑血管用药", "FZGXBXJTYJ");
            _tbl.NewRow("823", "807", "呼吸系统药", "HXXTY");
            _tbl.NewRow("827", "807", "消化系统药", "XHXTY");
            _tbl.NewRow("837", "807", "血液系统药", "XYXTY");
            _tbl.NewRow("856", "807", "口服降血糖药", "KFJXTY");
            _tbl.NewRow("862", "807", "注射降血糖药", "ZSJXTY");
            _tbl.NewRow("868", "807", "利尿药", "LNY");
            _tbl.NewRow("840", "839", "有机磷中毒解毒药", "YJLZDJDY");
            _tbl.NewRow("841", "839", "其他解毒药", "QTJDY");
            _tbl.NewRow("843", "842", "皮肤科用药", "PFKYY");
            _tbl.NewRow("844", "842", "眼科用药", "YKYY");
            _tbl.NewRow("845", "842", "耳鼻喉科用药", "EBHKYY");
            _tbl.NewRow("846", "842", "妇科", "FK");
            _tbl.NewRow("847", "842", "肛肠科", "GCK");
            _tbl.NewRow("848", "842", "骨科", "GK");
            _tbl.NewRow("934", "842", "外科用药", "WKYY");
            _tbl.NewRow("935", "842", "妇产科用药", "FCKYY");
            _tbl.NewRow("936", "842", "解毒药物", "JDYW");
            _tbl.NewRow("937", "842", "消毒防腐药", "XDFFY");
            _tbl.NewRow("870", "869", "肾上腺皮质激素类", "SSXPZJSL");
            _tbl.NewRow("871", "869", "甲状旁腺激素.降钙素.钙剂及二磷酸盐", "JZPXJS.JGS");
            _tbl.NewRow("875", "869", "生殖系统药", "SZXTY");
            _tbl.NewRow("883", "869", "下丘脑和垂体激素类药物", "XQNHCTJSLY");
            _tbl.NewRow("886", "869", "甲状腺激素及抗甲状腺药", "JZXJSJKJZX");
            _tbl.NewRow("888", "887", "水、电解质和酸碱平衡药物", "SDJZHSJPHY");
            _tbl.NewRow("893", "887", "透析液", "TXY");
            _tbl.NewRow("917", "916", "维生素类", "WSSL");
            _tbl.NewRow("920", "916", "全肠道内营养药", "QCDNYYY");
            _tbl.NewRow("921", "916", "全肠道外营养药", "QCDWYYY");
            _tbl.NewRow("922", "916", "微量元素", "WLYS");
            _tbl.NewRow("923", "916", "生物制品", "SWZP");
            _tbl.NewRow("928", "916", "生化药", "SHY");
            _tbl.NewRow("105", "104", "青霉素类", "QMSL");
            _tbl.NewRow("106", "104", "头孢菌素类", "TBJSL");
            _tbl.NewRow("107", "104", "四环素类", "SHSL");
            _tbl.NewRow("423", "104", "单环B-内酰胺", "DHB-NXA");
            _tbl.NewRow("424", "104", "氨基糖苷类", "AJTGL");
            _tbl.NewRow("425", "104", "糖肽类", "TTL");
            _tbl.NewRow("426", "104", "其他抗生素类", "QTKSSL");
            _tbl.NewRow("898", "104", "碳青霉烯类", "TQMXL");
            _tbl.NewRow("899", "104", "头霉素类", "TMSL");
            _tbl.NewRow("931", "930", "抗病毒药", "KBDY");
            _tbl.NewRow("945", "930", "免疫抑制剂", "MYYZJ");
            _tbl.NewRow("900", "104", "抗真菌类", "KZJL");
            _tbl.NewRow("904", "104", "B-内酰胺酶抑制剂", "B-NXAMYZJ");
            _tbl.NewRow("905", "104", "氨基苷类", "AJGL");
            _tbl.NewRow("906", "104", "大环内酯类", "DHNZL");
            _tbl.NewRow("907", "104", "肽类", "TL");
            _tbl.NewRow("908", "104", "林可霉素类", "LKMSL");
            _tbl.NewRow("909", "104", "磷霉素类", "LMSL");
            _tbl.NewRow("910", "104", "硝基咪唑类", "XJMZL");
            _tbl.NewRow("911", "104", "磺胺类", "HAL");
            _tbl.NewRow("912", "104", "喹诺酮类", "KNTL");
            _tbl.NewRow("913", "104", "抗疟药", "KNY");
            _tbl.NewRow("914", "104", "羧链胞酸酯类", "SLBSZL");
            _tbl.NewRow("915", "104", "唑烷酮类", "ZWTL");
            _tbl.NewRow("402", "401", "肌肉松弛药", "JRSCY");
            _tbl.NewRow("403", "401", "局部麻醉药", "JBMZY");
            _tbl.NewRow("789", "401", "拟胆碱药", "NDJY");
            _tbl.NewRow("790", "401", "抗胆碱药", "KDJY");
            _tbl.NewRow("791", "401", "拟肾上腺素药", "NSSXSY");
            _tbl.NewRow("792", "401", "肾上腺素受体阻断药", "SSXSSTZDY");
            _tbl.NewRow("406", "405", "静脉麻醉药", "JMMZY");
            _tbl.NewRow("407", "405", "吸入全身麻醉药", "XRQSMZY");
            _tbl.NewRow("793", "404", "抗痴呆药", "KCDY");
            _tbl.NewRow("794", "404", "抗癫痫药", "KDXY");
            _tbl.NewRow("795", "404", "抗焦虑药", "KJLY");
            _tbl.NewRow("796", "404", "抗精神病药", "KJSBY");
            _tbl.NewRow("797", "404", "抗抑郁药", "KYYY");
            _tbl.NewRow("798", "404", "影响脑代谢及促智药", "YXNDXJCZY");
            _tbl.NewRow("799", "404", "镇静催眠药", "ZJCMY");
            _tbl.NewRow("800", "404", "镇痛药", "ZTY");
            _tbl.NewRow("801", "404", "治疗帕金森病药", "ZLPJSBY");
            _tbl.NewRow("802", "404", "中枢兴奋药物", "ZSXFYW");
            _tbl.NewRow("427", "422", "合成抗菌药物", "HCKJYW");
            _tbl.NewRow("429", "422", "抗病毒类", "KBDL");
            _tbl.NewRow("850", "422", "抗结核类", "KJHL");
            _tbl.NewRow("811", "810", "血小板抑制剂", "XXBYZJ");
            _tbl.NewRow("812", "810", "抗凝剂", "KNJ");
            _tbl.NewRow("813", "810", "溶栓药", "RSY");
            _tbl.NewRow("814", "810", "其他", "QT");
            _tbl.NewRow("816", "815", "B肾上腺素能受体阻滞剂", "BSSXSNSTZZ");
            _tbl.NewRow("817", "815", "钙拮抗剂", "GJKJ");
            _tbl.NewRow("818", "815", "血管紧张素II受体拮抗剂", "XGJZSIISTJ");
            _tbl.NewRow("819", "815", "血管紧张素转换酶抑制剂", "XGJZSZHMYZ");
            _tbl.NewRow("820", "815", "血管扩张剂", "XGKZJ");
            _tbl.NewRow("824", "823", "祛痰药", "QTY");
            _tbl.NewRow("825", "823", "平喘药", "PCY");
            _tbl.NewRow("826", "823", "镇咳药", "ZKY");
            _tbl.NewRow("828", "827", "抗酸及治疗消化性溃疡药", "KSJZLXHXKY");
            _tbl.NewRow("829", "827", "助消化药", "ZXHY");
            _tbl.NewRow("830", "827", "胃肠解痉药", "WCJJY");
            _tbl.NewRow("831", "827", "微生态制剂", "WSTZJ");
            _tbl.NewRow("832", "827", "止吐药", "ZTY");
            _tbl.NewRow("833", "827", "胃肠动力药", "WCDLY");
            _tbl.NewRow("834", "827", "止泻药与通便药", "ZXYYTBY");
            _tbl.NewRow("835", "827", "肝胆疾病辅助药", "GDJBFZY");
            _tbl.NewRow("836", "827", "其他消化系统用药", "QTXHXTYY");
            _tbl.NewRow("838", "837", "促凝血和止血药", "CNXHZXY");
            _tbl.NewRow("851", "837", "抗凝血、溶栓药", "KNXRSY");
            _tbl.NewRow("852", "837", "抗贫血药", "KPXY");
            _tbl.NewRow("853", "837", "抗血小板药", "KXXBY");
            _tbl.NewRow("854", "837", "升白细胞及血小板药", "SBXBJXXBY");
            _tbl.NewRow("855", "837", "抗肿瘤药", "KZLY");
            _tbl.NewRow("857", "856", "a-葡萄糖苷酶抑制剂", "A-PTTGMYZJ");
            _tbl.NewRow("858", "856", "非磺酰脲类促胰岛素分泌剂", "FHXNLCYDSF");
            _tbl.NewRow("859", "856", "磺酰脲类", "HXNL");
            _tbl.NewRow("860", "856", "噻唑烷二酮", "SZWET");
            _tbl.NewRow("861", "856", "双胍类", "SGL");
            _tbl.NewRow("863", "862", "短效胰岛素", "DXYDS");
            _tbl.NewRow("864", "862", "中效胰岛素", "ZXYDS");
            _tbl.NewRow("865", "862", "预混胰岛素", "YHYDS");
            _tbl.NewRow("866", "862", "超长效人胰岛素", "CCXRYDS");
            _tbl.NewRow("867", "862", "超短效人胰岛素", "CDXRYDS");
            _tbl.NewRow("872", "871", "钙剂", "GJ");
            _tbl.NewRow("873", "871", "降钙素", "JGS");
            _tbl.NewRow("874", "871", "二磷酸盐类", "ELSYL");
            _tbl.NewRow("876", "875", "避孕药", "BYY");
            _tbl.NewRow("877", "875", "促性腺激素类", "CXXJSL");
            _tbl.NewRow("878", "875", "抗早孕药", "KZYY");
            _tbl.NewRow("879", "875", "雄激素类", "XJSL");
            _tbl.NewRow("880", "875", "孕激素类", "YJSL");
            _tbl.NewRow("881", "875", "雌激素类", "CJSL");
            _tbl.NewRow("882", "875", "其他妇科用药", "QTFKYY");
            _tbl.NewRow("884", "883", "垂体激素", "CTJS");
            _tbl.NewRow("885", "883", "下丘脑激素", "XQNJS");
            _tbl.NewRow("889", "888", "电解质输液", "DJZSY");
            _tbl.NewRow("890", "888", "胶体输液", "JTSY");
            _tbl.NewRow("891", "888", "盐类输液", "YLSY");
            _tbl.NewRow("892", "888", "营养输液", "YYSY");
            _tbl.NewRow("918", "917", "水溶性维生素", "SRXWSS");
            _tbl.NewRow("919", "917", "脂溶性维生素", "ZRXWSS");
            _tbl.NewRow("924", "923", "菌苗", "JM");
            _tbl.NewRow("925", "923", "类毒素", "LDS");
            _tbl.NewRow("926", "923", "人血液制品", "RXYZP");
            _tbl.NewRow("927", "923", "其他用药", "QTYY");
            _tbl.NewRow("929", "928", "酶类", "ML");
            _tbl.NewRow("894", "106", "第一代头孢菌素", "DYDTBJS");
            _tbl.NewRow("895", "106", "第二代头孢菌素", "DEDTBJS");
            _tbl.NewRow("896", "106", "第三代头孢菌素", "DSDTBJS");
            _tbl.NewRow("897", "106", "第四代头孢菌素", "DSDTBJS");
            _tbl.NewRow("901", "900", "多烯类", "DXL");
            _tbl.NewRow("902", "900", "烯丙胺类", "XBAL");
            _tbl.NewRow("903", "900", "唑类", "ZL");
            _tbl.NewRow("932", "931", "核苷类", "HGL");
            _tbl.NewRow("933", "931", "非核苷类", "FHGL");
            _tbl.NewRow("428", "427", "抗感染中草药", "KGRZCY");
            _tbl.NewRow("938", "855", "激素类", "JSL");
            _tbl.NewRow("939", "855", "抗代谢类", "KDXL");
            _tbl.NewRow("940", "855", "抗生素类", "KSSL");
            _tbl.NewRow("941", "855", "抗体类", "KTL");
            _tbl.NewRow("942", "855", "其他抗肿瘤药及抗肿瘤辅助药", "QTKZLYJKZL");
            _tbl.NewRow("943", "855", "天然来源类", "TRLYL");
            _tbl.NewRow("944", "855", "烷化剂类", "WHJL");
            _tbl.NewRow("108", "", "内科用药", "NKYY");
            _tbl.NewRow("738", "", "脑血栓 冠心病", "NXSGXB");
            _tbl.NewRow("739", "", "镇静 安神", "ZJAS");
            _tbl.NewRow("740", "", "妇科", "FK");
            _tbl.NewRow("741", "", "五官", "WG");
            _tbl.NewRow("742", "", "皮肤", "PF");
            _tbl.NewRow("743", "", "外用", "WY");
            _tbl.NewRow("744", "", "滋补", "ZB");
            _tbl.NewRow("745", "", "降糖", "JT");
            _tbl.NewRow("746", "", "抗肿瘤", "KZL");
            _tbl.NewRow("747", "", "中药汤剂", "ZYTJ");
            _tbl.NewRow("748", "", "清热解毒", "QRJD");
            _tbl.NewRow("110", "", "儿科用药", "EKYY");
            _tbl.NewRow("111", "", "妇科用药", "FKYY");
            _tbl.NewRow("182", "", "测试部专用", "CSBZY");
            _tbl.NewRow("408", "", "呼吸", "HX");
            _tbl.NewRow("723", "", "其他", "QT");
            _tbl.NewRow("735", "", "消化类", "XHL");
            _tbl.NewRow("736", "", "祛风除湿", "QFCS");
            _tbl.NewRow("737", "", "补肾壮阳", "BSZY");
            _tbl.NewRow("112", "108", "泌尿系统类", "MNXTL");
            _tbl.NewRow("113", "108", "心脑血管疾病类", "XNXGJBL");
            _tbl.NewRow("114", "108", "安神类", "ASL");
            _tbl.NewRow("725", "108", "化痰止咳平喘类", "HTZKPCL");
            _tbl.NewRow("726", "108", "抗肿瘤及肿瘤辅助治疗类", "KZLJZLFZZL");
            _tbl.NewRow("727", "108", "虚症类", "XZL");
            _tbl.NewRow("728", "108", "消化系统类", "XHXTL");
            _tbl.NewRow("729", "108", "清热解毒（感冒）类", "QRJDGML");
            _tbl.NewRow("730", "108", "肝胆类", "GDL");
            _tbl.NewRow("731", "108", "降压类", "JYL");
            _tbl.NewRow("732", "108", "降糖类", "JTL");
            _tbl.NewRow("733", "110", "内服类", "NFL");
            _tbl.NewRow("734", "111", "外用类", "WYL");
            _tbl.NewRow("724", "112", "注射液", "ZSY");
            _tbl.NewRow("116", "", "根茎类", "GJL");
            _tbl.NewRow("762", "", "温里药", "WLY");
            _tbl.NewRow("763", "", "理气药", "LQY");
            _tbl.NewRow("764", "", "理血药", "LXY");
            _tbl.NewRow("767", "", "安神药", "ASY");
            _tbl.NewRow("770", "", "平肝熄风药", "PGXFY");
            _tbl.NewRow("771", "", "芳香开窍药", "FXKQY");
            _tbl.NewRow("774", "", "补养药", "BYY");
            _tbl.NewRow("779", "", "固涩药", "GSY");
            _tbl.NewRow("780", "", "消导药", "XDY");
            _tbl.NewRow("781", "", "泻下药", "XXY");
            _tbl.NewRow("785", "", "驱虫药", "QCY");
            _tbl.NewRow("786", "", "其他药", "QTY");
            _tbl.NewRow("787", "", "未分类", "WFL");
            _tbl.NewRow("117", "", "草叶类", "CYL");
            _tbl.NewRow("183", "", "测试部专用", "CSBZY");
            _tbl.NewRow("409", "", "枝叶", "ZY");
            _tbl.NewRow("749", "", "解表药", "JBY");
            _tbl.NewRow("752", "", "止咳平喘化痰药", "ZKPCHTY");
            _tbl.NewRow("756", "", "清热药", "QRY");
            _tbl.NewRow("760", "", "芳香化湿药", "FXHSY");
            _tbl.NewRow("761", "", "利水渗湿药", "LSSSY");
            _tbl.NewRow("765", "764", "止血药", "ZXY");
            _tbl.NewRow("766", "764", "活血化瘀药", "HXHYY");
            _tbl.NewRow("768", "767", "重镇安神药", "ZZASY");
            _tbl.NewRow("769", "767", "养心安神药", "YXASY");
            _tbl.NewRow("772", "771", "凉开药", "LKY");
            _tbl.NewRow("773", "771", "温开药", "WKY");
            _tbl.NewRow("775", "774", "补气药", "BQY");
            _tbl.NewRow("776", "774", "补血药", "BXY");
            _tbl.NewRow("777", "774", "补阴药", "BYY");
            _tbl.NewRow("778", "774", "助阳药", "ZYY");
            _tbl.NewRow("782", "781", "润下药", "RXY");
            _tbl.NewRow("783", "781", "攻下药", "GXY");
            _tbl.NewRow("784", "781", "逐水药", "ZSY");
            _tbl.NewRow("750", "749", "辛温解表药", "XWJBY");
            _tbl.NewRow("751", "749", "辛凉解表药", "XLJBY");
            _tbl.NewRow("753", "752", "温化寒痰药", "WHHTY");
            _tbl.NewRow("754", "752", "清化热痰药", "QHRTY");
            _tbl.NewRow("755", "752", "止咳平喘药", "ZKPCY");
            _tbl.NewRow("757", "756", "清热泻火药", "QRXHY");
            _tbl.NewRow("758", "756", "清热解毒药", "QRJDY");
            _tbl.NewRow("759", "756", "清热燥湿药", "QRZSY");

            return _tbl;
        }

        public static MedTree GetTreeData()
        {
            if (_tree != null)
                return _tree;

            Table tbl = GetTbl();
            _tree = new MedTree();

            foreach (var item in ((ITreeData)tbl).GetTreeRoot())
            {
                Row row = (Row)item;
                MedTreeItem ti = new MedTreeItem { Name = row.Str("name"), Code = row.Str("code") };
                BuildChildren(ti, row);
                _tree.Add(ti);
            }
            return _tree;
        }

        public static Table GetRootTbl()
        {
            Table tbl = GetTbl();
            var root = new Table
            {
                { "id" },
                { "parentid" },
                { "name" },
                { "code" },
            };
            foreach (var item in ((ITreeData)tbl).GetTreeRoot())
            {
                Row row = (Row)item;
                root.NewRow(row.Str("id"), "", row.Str("name"), row.Str("code"));
            }
            return root;
        }

        static void BuildChildren(MedTreeItem p_parent, Row p_row)
        {
            foreach (var item in ((ITreeData)_tbl).GetTreeItemChildren(p_row))
            {
                Row row = (Row)item;
                MedTreeItem ti = new MedTreeItem { Name = row.Str("name"), Code = row.Str("code") };
                BuildChildren(ti, row);
                p_parent.Children.Add(ti);
            }
        }
    }

    public class MedTree : List<MedTreeItem>, ITreeData
    {
        IEnumerable<object> ITreeData.GetTreeRoot()
        {
            return this;
        }

        IEnumerable<object> ITreeData.GetTreeItemChildren(object p_parent)
        {
            return ((MedTreeItem)p_parent).Children;
        }
    }

    public class MedTreeItem : INotifyPropertyChanged
    {
        string _name;

        public string Name
        {
            get { return _name; }
            internal set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public string Code { get; set; }

        public List<MedTreeItem> Children { get; } = new List<MedTreeItem>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
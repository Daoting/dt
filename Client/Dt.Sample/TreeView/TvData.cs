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

            _tbl.AddRow(new Medic("421", "", "标价", "BJ"));
            _tbl.AddRow(new Medic("722", "", "凯旋院区老干部用药", "KXYQLGBYY"));
            _tbl.AddRow(new Medic("788", "", "其它", "QT"));
            _tbl.AddRow(new Medic("803", "", "解热镇痛抗炎抗风湿药及抗变态反应药", "JRZTKYKFSY"));
            _tbl.AddRow(new Medic("807", "", "内脏各系统药物", "NZGXTYW"));
            _tbl.AddRow(new Medic("839", "", "解毒药", "JDY"));
            _tbl.AddRow(new Medic("842", "", "专科用药", "ZKYY"));
            _tbl.AddRow(new Medic("849", "", "诊断用药", "ZDYY"));
            _tbl.AddRow(new Medic("869", "", "激素类及相关药物", "JSLJXGYW"));
            _tbl.AddRow(new Medic("887", "", "水、电解质和酸碱平衡药物及透析液", "SDJZHSJPHY"));
            _tbl.AddRow(new Medic("102", "", "测试", "CS"));
            _tbl.AddRow(new Medic("916", "", "营养与微量元素", "YYYWLYS"));
            _tbl.AddRow(new Medic("930", "", "抗肿瘤.抗病毒及免疫抑制药", "KZL.KBDJMY"));
            _tbl.AddRow(new Medic("104", "", "抗生素", "KSS"));
            _tbl.AddRow(new Medic("1161", "", "测试用药", "CSYY"));
            _tbl.AddRow(new Medic("115", "", "博思专用", "BSZY"));
            _tbl.AddRow(new Medic("181", "", "测试部专用", "CSBZY"));
            _tbl.AddRow(new Medic("201", "", "实施专用", "SSZY"));
            _tbl.AddRow(new Medic("241", "", "仙丹类", "XDL"));
            _tbl.AddRow(new Medic("281", "", "心脏病神药", "XZBSY"));
            _tbl.AddRow(new Medic("401", "", "外周神经系统药", "WZSJXTY"));
            _tbl.AddRow(new Medic("405", "", "全身麻醉药", "QSMZY"));
            _tbl.AddRow(new Medic("404", "", "中枢神经系统药", "ZSSJXTY"));
            _tbl.AddRow(new Medic("422", "421", "抗微生物药物", "KWSWYW"));
            _tbl.AddRow(new Medic("804", "803", "解热镇痛抗炎抗风湿药", "JRZTKYKFSY"));
            _tbl.AddRow(new Medic("805", "803", "抗变态反应药", "KBTFYY"));
            _tbl.AddRow(new Medic("806", "803", "抗痛风药", "KTFY"));
            _tbl.AddRow(new Medic("808", "807", "调脂药", "DZY"));
            _tbl.AddRow(new Medic("809", "807", "治疗心衰药、抗休克药", "ZLXSYKXKY"));
            _tbl.AddRow(new Medic("810", "807", "抗血栓药", "KXSY"));
            _tbl.AddRow(new Medic("815", "807", "抗高血压药", "KGXYY"));
            _tbl.AddRow(new Medic("821", "807", "抗心律失常类", "KXLSCL"));
            _tbl.AddRow(new Medic("822", "807", "防治冠心病、心绞痛药及其他心脑血管用药", "FZGXBXJTYJ"));
            _tbl.AddRow(new Medic("823", "807", "呼吸系统药", "HXXTY"));
            _tbl.AddRow(new Medic("827", "807", "消化系统药", "XHXTY"));
            _tbl.AddRow(new Medic("837", "807", "血液系统药", "XYXTY"));
            _tbl.AddRow(new Medic("856", "807", "口服降血糖药", "KFJXTY"));
            _tbl.AddRow(new Medic("862", "807", "注射降血糖药", "ZSJXTY"));
            _tbl.AddRow(new Medic("868", "807", "利尿药", "LNY"));
            _tbl.AddRow(new Medic("840", "839", "有机磷中毒解毒药", "YJLZDJDY"));
            _tbl.AddRow(new Medic("841", "839", "其他解毒药", "QTJDY"));
            _tbl.AddRow(new Medic("843", "842", "皮肤科用药", "PFKYY"));
            _tbl.AddRow(new Medic("844", "842", "眼科用药", "YKYY"));
            _tbl.AddRow(new Medic("845", "842", "耳鼻喉科用药", "EBHKYY"));
            _tbl.AddRow(new Medic("846", "842", "妇科", "FK"));
            _tbl.AddRow(new Medic("847", "842", "肛肠科", "GCK"));
            _tbl.AddRow(new Medic("848", "842", "骨科", "GK"));
            _tbl.AddRow(new Medic("934", "842", "外科用药", "WKYY"));
            _tbl.AddRow(new Medic("935", "842", "妇产科用药", "FCKYY"));
            _tbl.AddRow(new Medic("936", "842", "解毒药物", "JDYW"));
            _tbl.AddRow(new Medic("937", "842", "消毒防腐药", "XDFFY"));
            _tbl.AddRow(new Medic("870", "869", "肾上腺皮质激素类", "SSXPZJSL"));
            _tbl.AddRow(new Medic("871", "869", "甲状旁腺激素.降钙素.钙剂及二磷酸盐", "JZPXJS.JGS"));
            _tbl.AddRow(new Medic("875", "869", "生殖系统药", "SZXTY"));
            _tbl.AddRow(new Medic("883", "869", "下丘脑和垂体激素类药物", "XQNHCTJSLY"));
            _tbl.AddRow(new Medic("886", "869", "甲状腺激素及抗甲状腺药", "JZXJSJKJZX"));
            _tbl.AddRow(new Medic("888", "887", "水、电解质和酸碱平衡药物", "SDJZHSJPHY"));
            _tbl.AddRow(new Medic("893", "887", "透析液", "TXY"));
            _tbl.AddRow(new Medic("917", "916", "维生素类", "WSSL"));
            _tbl.AddRow(new Medic("920", "916", "全肠道内营养药", "QCDNYYY"));
            _tbl.AddRow(new Medic("921", "916", "全肠道外营养药", "QCDWYYY"));
            _tbl.AddRow(new Medic("922", "916", "微量元素", "WLYS"));
            _tbl.AddRow(new Medic("923", "916", "生物制品", "SWZP"));
            _tbl.AddRow(new Medic("928", "916", "生化药", "SHY"));
            _tbl.AddRow(new Medic("105", "104", "青霉素类", "QMSL"));
            _tbl.AddRow(new Medic("106", "104", "头孢菌素类", "TBJSL"));
            _tbl.AddRow(new Medic("107", "104", "四环素类", "SHSL"));
            _tbl.AddRow(new Medic("423", "104", "单环B-内酰胺", "DHB-NXA"));
            _tbl.AddRow(new Medic("424", "104", "氨基糖苷类", "AJTGL"));
            _tbl.AddRow(new Medic("425", "104", "糖肽类", "TTL"));
            _tbl.AddRow(new Medic("426", "104", "其他抗生素类", "QTKSSL"));
            _tbl.AddRow(new Medic("898", "104", "碳青霉烯类", "TQMXL"));
            _tbl.AddRow(new Medic("899", "104", "头霉素类", "TMSL"));
            _tbl.AddRow(new Medic("931", "930", "抗病毒药", "KBDY"));
            _tbl.AddRow(new Medic("945", "930", "免疫抑制剂", "MYYZJ"));
            _tbl.AddRow(new Medic("900", "104", "抗真菌类", "KZJL"));
            _tbl.AddRow(new Medic("904", "104", "B-内酰胺酶抑制剂", "B-NXAMYZJ"));
            _tbl.AddRow(new Medic("905", "104", "氨基苷类", "AJGL"));
            _tbl.AddRow(new Medic("906", "104", "大环内酯类", "DHNZL"));
            _tbl.AddRow(new Medic("907", "104", "肽类", "TL"));
            _tbl.AddRow(new Medic("908", "104", "林可霉素类", "LKMSL"));
            _tbl.AddRow(new Medic("909", "104", "磷霉素类", "LMSL"));
            _tbl.AddRow(new Medic("910", "104", "硝基咪唑类", "XJMZL"));
            _tbl.AddRow(new Medic("911", "104", "磺胺类", "HAL"));
            _tbl.AddRow(new Medic("912", "104", "喹诺酮类", "KNTL"));
            _tbl.AddRow(new Medic("913", "104", "抗疟药", "KNY"));
            _tbl.AddRow(new Medic("914", "104", "羧链胞酸酯类", "SLBSZL"));
            _tbl.AddRow(new Medic("915", "104", "唑烷酮类", "ZWTL"));
            _tbl.AddRow(new Medic("402", "401", "肌肉松弛药", "JRSCY"));
            _tbl.AddRow(new Medic("403", "401", "局部麻醉药", "JBMZY"));
            _tbl.AddRow(new Medic("789", "401", "拟胆碱药", "NDJY"));
            _tbl.AddRow(new Medic("790", "401", "抗胆碱药", "KDJY"));
            _tbl.AddRow(new Medic("791", "401", "拟肾上腺素药", "NSSXSY"));
            _tbl.AddRow(new Medic("792", "401", "肾上腺素受体阻断药", "SSXSSTZDY"));
            _tbl.AddRow(new Medic("406", "405", "静脉麻醉药", "JMMZY"));
            _tbl.AddRow(new Medic("407", "405", "吸入全身麻醉药", "XRQSMZY"));
            _tbl.AddRow(new Medic("793", "404", "抗痴呆药", "KCDY"));
            _tbl.AddRow(new Medic("794", "404", "抗癫痫药", "KDXY"));
            _tbl.AddRow(new Medic("795", "404", "抗焦虑药", "KJLY"));
            _tbl.AddRow(new Medic("796", "404", "抗精神病药", "KJSBY"));
            _tbl.AddRow(new Medic("797", "404", "抗抑郁药", "KYYY"));
            _tbl.AddRow(new Medic("798", "404", "影响脑代谢及促智药", "YXNDXJCZY"));
            _tbl.AddRow(new Medic("799", "404", "镇静催眠药", "ZJCMY"));
            _tbl.AddRow(new Medic("800", "404", "镇痛药", "ZTY"));
            _tbl.AddRow(new Medic("801", "404", "治疗帕金森病药", "ZLPJSBY"));
            _tbl.AddRow(new Medic("802", "404", "中枢兴奋药物", "ZSXFYW"));
            _tbl.AddRow(new Medic("427", "422", "合成抗菌药物", "HCKJYW"));
            _tbl.AddRow(new Medic("429", "422", "抗病毒类", "KBDL"));
            _tbl.AddRow(new Medic("850", "422", "抗结核类", "KJHL"));
            _tbl.AddRow(new Medic("811", "810", "血小板抑制剂", "XXBYZJ"));
            _tbl.AddRow(new Medic("812", "810", "抗凝剂", "KNJ"));
            _tbl.AddRow(new Medic("813", "810", "溶栓药", "RSY"));
            _tbl.AddRow(new Medic("814", "810", "其他", "QT"));
            _tbl.AddRow(new Medic("816", "815", "B肾上腺素能受体阻滞剂", "BSSXSNSTZZ"));
            _tbl.AddRow(new Medic("817", "815", "钙拮抗剂", "GJKJ"));
            _tbl.AddRow(new Medic("818", "815", "血管紧张素II受体拮抗剂", "XGJZSIISTJ"));
            _tbl.AddRow(new Medic("819", "815", "血管紧张素转换酶抑制剂", "XGJZSZHMYZ"));
            _tbl.AddRow(new Medic("820", "815", "血管扩张剂", "XGKZJ"));
            _tbl.AddRow(new Medic("824", "823", "祛痰药", "QTY"));
            _tbl.AddRow(new Medic("825", "823", "平喘药", "PCY"));
            _tbl.AddRow(new Medic("826", "823", "镇咳药", "ZKY"));
            _tbl.AddRow(new Medic("828", "827", "抗酸及治疗消化性溃疡药", "KSJZLXHXKY"));
            _tbl.AddRow(new Medic("829", "827", "助消化药", "ZXHY"));
            _tbl.AddRow(new Medic("830", "827", "胃肠解痉药", "WCJJY"));
            _tbl.AddRow(new Medic("831", "827", "微生态制剂", "WSTZJ"));
            _tbl.AddRow(new Medic("832", "827", "止吐药", "ZTY"));
            _tbl.AddRow(new Medic("833", "827", "胃肠动力药", "WCDLY"));
            _tbl.AddRow(new Medic("834", "827", "止泻药与通便药", "ZXYYTBY"));
            _tbl.AddRow(new Medic("835", "827", "肝胆疾病辅助药", "GDJBFZY"));
            _tbl.AddRow(new Medic("836", "827", "其他消化系统用药", "QTXHXTYY"));
            _tbl.AddRow(new Medic("838", "837", "促凝血和止血药", "CNXHZXY"));
            _tbl.AddRow(new Medic("851", "837", "抗凝血、溶栓药", "KNXRSY"));
            _tbl.AddRow(new Medic("852", "837", "抗贫血药", "KPXY"));
            _tbl.AddRow(new Medic("853", "837", "抗血小板药", "KXXBY"));
            _tbl.AddRow(new Medic("854", "837", "升白细胞及血小板药", "SBXBJXXBY"));
            _tbl.AddRow(new Medic("855", "837", "抗肿瘤药", "KZLY"));
            _tbl.AddRow(new Medic("857", "856", "a-葡萄糖苷酶抑制剂", "A-PTTGMYZJ"));
            _tbl.AddRow(new Medic("858", "856", "非磺酰脲类促胰岛素分泌剂", "FHXNLCYDSF"));
            _tbl.AddRow(new Medic("859", "856", "磺酰脲类", "HXNL"));
            _tbl.AddRow(new Medic("860", "856", "噻唑烷二酮", "SZWET"));
            _tbl.AddRow(new Medic("861", "856", "双胍类", "SGL"));
            _tbl.AddRow(new Medic("863", "862", "短效胰岛素", "DXYDS"));
            _tbl.AddRow(new Medic("864", "862", "中效胰岛素", "ZXYDS"));
            _tbl.AddRow(new Medic("865", "862", "预混胰岛素", "YHYDS"));
            _tbl.AddRow(new Medic("866", "862", "超长效人胰岛素", "CCXRYDS"));
            _tbl.AddRow(new Medic("867", "862", "超短效人胰岛素", "CDXRYDS"));
            _tbl.AddRow(new Medic("872", "871", "钙剂", "GJ"));
            _tbl.AddRow(new Medic("873", "871", "降钙素", "JGS"));
            _tbl.AddRow(new Medic("874", "871", "二磷酸盐类", "ELSYL"));
            _tbl.AddRow(new Medic("876", "875", "避孕药", "BYY"));
            _tbl.AddRow(new Medic("877", "875", "促性腺激素类", "CXXJSL"));
            _tbl.AddRow(new Medic("878", "875", "抗早孕药", "KZYY"));
            _tbl.AddRow(new Medic("879", "875", "雄激素类", "XJSL"));
            _tbl.AddRow(new Medic("880", "875", "孕激素类", "YJSL"));
            _tbl.AddRow(new Medic("881", "875", "雌激素类", "CJSL"));
            _tbl.AddRow(new Medic("882", "875", "其他妇科用药", "QTFKYY"));
            _tbl.AddRow(new Medic("884", "883", "垂体激素", "CTJS"));
            _tbl.AddRow(new Medic("885", "883", "下丘脑激素", "XQNJS"));
            _tbl.AddRow(new Medic("889", "888", "电解质输液", "DJZSY"));
            _tbl.AddRow(new Medic("890", "888", "胶体输液", "JTSY"));
            _tbl.AddRow(new Medic("891", "888", "盐类输液", "YLSY"));
            _tbl.AddRow(new Medic("892", "888", "营养输液", "YYSY"));
            _tbl.AddRow(new Medic("918", "917", "水溶性维生素", "SRXWSS"));
            _tbl.AddRow(new Medic("919", "917", "脂溶性维生素", "ZRXWSS"));
            _tbl.AddRow(new Medic("924", "923", "菌苗", "JM"));
            _tbl.AddRow(new Medic("925", "923", "类毒素", "LDS"));
            _tbl.AddRow(new Medic("926", "923", "人血液制品", "RXYZP"));
            _tbl.AddRow(new Medic("927", "923", "其他用药", "QTYY"));
            _tbl.AddRow(new Medic("929", "928", "酶类", "ML"));
            _tbl.AddRow(new Medic("894", "106", "第一代头孢菌素", "DYDTBJS"));
            _tbl.AddRow(new Medic("895", "106", "第二代头孢菌素", "DEDTBJS"));
            _tbl.AddRow(new Medic("896", "106", "第三代头孢菌素", "DSDTBJS"));
            _tbl.AddRow(new Medic("897", "106", "第四代头孢菌素", "DSDTBJS"));
            _tbl.AddRow(new Medic("901", "900", "多烯类", "DXL"));
            _tbl.AddRow(new Medic("902", "900", "烯丙胺类", "XBAL"));
            _tbl.AddRow(new Medic("903", "900", "唑类", "ZL"));
            _tbl.AddRow(new Medic("932", "931", "核苷类", "HGL"));
            _tbl.AddRow(new Medic("933", "931", "非核苷类", "FHGL"));
            _tbl.AddRow(new Medic("428", "427", "抗感染中草药", "KGRZCY"));
            _tbl.AddRow(new Medic("938", "855", "激素类", "JSL"));
            _tbl.AddRow(new Medic("939", "855", "抗代谢类", "KDXL"));
            _tbl.AddRow(new Medic("940", "855", "抗生素类", "KSSL"));
            _tbl.AddRow(new Medic("941", "855", "抗体类", "KTL"));
            _tbl.AddRow(new Medic("942", "855", "其他抗肿瘤药及抗肿瘤辅助药", "QTKZLYJKZL"));
            _tbl.AddRow(new Medic("943", "855", "天然来源类", "TRLYL"));
            _tbl.AddRow(new Medic("944", "855", "烷化剂类", "WHJL"));
            _tbl.AddRow(new Medic("108", "", "内科用药", "NKYY"));
            _tbl.AddRow(new Medic("738", "", "脑血栓 冠心病", "NXSGXB"));
            _tbl.AddRow(new Medic("739", "", "镇静 安神", "ZJAS"));
            _tbl.AddRow(new Medic("740", "", "妇科", "FK"));
            _tbl.AddRow(new Medic("741", "", "五官", "WG"));
            _tbl.AddRow(new Medic("742", "", "皮肤", "PF"));
            _tbl.AddRow(new Medic("743", "", "外用", "WY"));
            _tbl.AddRow(new Medic("744", "", "滋补", "ZB"));
            _tbl.AddRow(new Medic("745", "", "降糖", "JT"));
            _tbl.AddRow(new Medic("746", "", "抗肿瘤", "KZL"));
            _tbl.AddRow(new Medic("747", "", "中药汤剂", "ZYTJ"));
            _tbl.AddRow(new Medic("748", "", "清热解毒", "QRJD"));
            _tbl.AddRow(new Medic("110", "", "儿科用药", "EKYY"));
            _tbl.AddRow(new Medic("111", "", "妇科用药", "FKYY"));
            _tbl.AddRow(new Medic("182", "", "测试部专用", "CSBZY"));
            _tbl.AddRow(new Medic("408", "", "呼吸", "HX"));
            _tbl.AddRow(new Medic("723", "", "其他", "QT"));
            _tbl.AddRow(new Medic("735", "", "消化类", "XHL"));
            _tbl.AddRow(new Medic("736", "", "祛风除湿", "QFCS"));
            _tbl.AddRow(new Medic("737", "", "补肾壮阳", "BSZY"));
            _tbl.AddRow(new Medic("112", "108", "泌尿系统类", "MNXTL"));
            _tbl.AddRow(new Medic("113", "108", "心脑血管疾病类", "XNXGJBL"));
            _tbl.AddRow(new Medic("114", "108", "安神类", "ASL"));
            _tbl.AddRow(new Medic("725", "108", "化痰止咳平喘类", "HTZKPCL"));
            _tbl.AddRow(new Medic("726", "108", "抗肿瘤及肿瘤辅助治疗类", "KZLJZLFZZL"));
            _tbl.AddRow(new Medic("727", "108", "虚症类", "XZL"));
            _tbl.AddRow(new Medic("728", "108", "消化系统类", "XHXTL"));
            _tbl.AddRow(new Medic("729", "108", "清热解毒（感冒）类", "QRJDGML"));
            _tbl.AddRow(new Medic("730", "108", "肝胆类", "GDL"));
            _tbl.AddRow(new Medic("731", "108", "降压类", "JYL"));
            _tbl.AddRow(new Medic("732", "108", "降糖类", "JTL"));
            _tbl.AddRow(new Medic("733", "110", "内服类", "NFL"));
            _tbl.AddRow(new Medic("734", "111", "外用类", "WYL"));
            _tbl.AddRow(new Medic("724", "112", "注射液", "ZSY"));
            _tbl.AddRow(new Medic("116", "", "根茎类", "GJL"));
            _tbl.AddRow(new Medic("762", "", "温里药", "WLY"));
            _tbl.AddRow(new Medic("763", "", "理气药", "LQY"));
            _tbl.AddRow(new Medic("764", "", "理血药", "LXY"));
            _tbl.AddRow(new Medic("767", "", "安神药", "ASY"));
            _tbl.AddRow(new Medic("770", "", "平肝熄风药", "PGXFY"));
            _tbl.AddRow(new Medic("771", "", "芳香开窍药", "FXKQY"));
            _tbl.AddRow(new Medic("774", "", "补养药", "BYY"));
            _tbl.AddRow(new Medic("779", "", "固涩药", "GSY"));
            _tbl.AddRow(new Medic("780", "", "消导药", "XDY"));
            _tbl.AddRow(new Medic("781", "", "泻下药", "XXY"));
            _tbl.AddRow(new Medic("785", "", "驱虫药", "QCY"));
            _tbl.AddRow(new Medic("786", "", "其他药", "QTY"));
            _tbl.AddRow(new Medic("787", "", "未分类", "WFL"));
            _tbl.AddRow(new Medic("117", "", "草叶类", "CYL"));
            _tbl.AddRow(new Medic("183", "", "测试部专用", "CSBZY"));
            _tbl.AddRow(new Medic("409", "", "枝叶", "ZY"));
            _tbl.AddRow(new Medic("749", "", "解表药", "JBY"));
            _tbl.AddRow(new Medic("752", "", "止咳平喘化痰药", "ZKPCHTY"));
            _tbl.AddRow(new Medic("756", "", "清热药", "QRY"));
            _tbl.AddRow(new Medic("760", "", "芳香化湿药", "FXHSY"));
            _tbl.AddRow(new Medic("761", "", "利水渗湿药", "LSSSY"));
            _tbl.AddRow(new Medic("765", "764", "止血药", "ZXY"));
            _tbl.AddRow(new Medic("766", "764", "活血化瘀药", "HXHYY"));
            _tbl.AddRow(new Medic("768", "767", "重镇安神药", "ZZASY"));
            _tbl.AddRow(new Medic("769", "767", "养心安神药", "YXASY"));
            _tbl.AddRow(new Medic("772", "771", "凉开药", "LKY"));
            _tbl.AddRow(new Medic("773", "771", "温开药", "WKY"));
            _tbl.AddRow(new Medic("775", "774", "补气药", "BQY"));
            _tbl.AddRow(new Medic("776", "774", "补血药", "BXY"));
            _tbl.AddRow(new Medic("777", "774", "补阴药", "BYY"));
            _tbl.AddRow(new Medic("778", "774", "助阳药", "ZYY"));
            _tbl.AddRow(new Medic("782", "781", "润下药", "RXY"));
            _tbl.AddRow(new Medic("783", "781", "攻下药", "GXY"));
            _tbl.AddRow(new Medic("784", "781", "逐水药", "ZSY"));
            _tbl.AddRow(new Medic("750", "749", "辛温解表药", "XWJBY"));
            _tbl.AddRow(new Medic("751", "749", "辛凉解表药", "XLJBY"));
            _tbl.AddRow(new Medic("753", "752", "温化寒痰药", "WHHTY"));
            _tbl.AddRow(new Medic("754", "752", "清化热痰药", "QHRTY"));
            _tbl.AddRow(new Medic("755", "752", "止咳平喘药", "ZKPCY"));
            _tbl.AddRow(new Medic("757", "756", "清热泻火药", "QRXHY"));
            _tbl.AddRow(new Medic("758", "756", "清热解毒药", "QRJDY"));
            _tbl.AddRow(new Medic("759", "756", "清热燥湿药", "QRZSY"));

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
                root.AddRow(new { id = row.Str("id"), parentid = "", name = row.Str("name"), code = row.Str("code") });
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

        class Medic
        {
            public Medic(string p_id, string p_parentID, string p_name, string p_code)
            {
                ID = p_id;
                ParentID = p_parentID;
                Name = p_name;
                Code = p_code;
            }

            public string ID { get; }
            public string ParentID { get; }
            public string Name { get; }
            public string Code { get; }
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
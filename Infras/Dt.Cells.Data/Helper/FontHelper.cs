#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    internal static class FontHelper
    {
        static Dictionary<string, string> EastAsiaFontMapping = new Dictionary<string, string>();
        static HashSet<string> EastAsiaFontNames = new HashSet<string>();

        static FontHelper()
        {
            EastAsiaFontNames.Add("MS UI GOTHIC");
            EastAsiaFontNames.Add("MS PGOTHIC");
            EastAsiaFontNames.Add("ＭＳ Ｐゴシック");
            EastAsiaFontNames.Add("MS GOTHIC");
            EastAsiaFontNames.Add("ＭＳ ゴシック");
            EastAsiaFontNames.Add("HGGOTHICE");
            EastAsiaFontNames.Add("ＨＧゴシックE");
            EastAsiaFontNames.Add("HGGOTHICM");
            EastAsiaFontNames.Add("ＨＧゴシックM");
            EastAsiaFontNames.Add("HGSGOTHICE");
            EastAsiaFontNames.Add("HGSゴシックE");
            EastAsiaFontNames.Add("HGSGOTHICM");
            EastAsiaFontNames.Add("HGSSゴシックM");
            EastAsiaFontNames.Add("HGPGOTHICE");
            EastAsiaFontNames.Add("HGPゴシックE");
            EastAsiaFontNames.Add("HGPGOTHICM");
            EastAsiaFontNames.Add("HGPゴシックM");
            EastAsiaFontNames.Add("HGMaruGothicMPRO");
            EastAsiaFontNames.Add("HG丸ｺﾞｼｯｸM-PRO");
            EastAsiaFontNames.Add("HGSeikaishotaiPRO");
            EastAsiaFontNames.Add("HG正楷書体-PRO");
            EastAsiaFontNames.Add("HGSeikaishotai");
            EastAsiaFontNames.Add("ＨＧ正楷書体");
            EastAsiaFontNames.Add("MS MINCHO");
            EastAsiaFontNames.Add("ＭＳ 明朝");
            EastAsiaFontNames.Add("MS PMINCHO");
            EastAsiaFontNames.Add("ＭＳ Ｐ明朝");
            EastAsiaFontNames.Add("GULIMCHE");
            EastAsiaFontNames.Add("굴림체");
            EastAsiaFontNames.Add("BATANGCHE");
            EastAsiaFontNames.Add("바탕체");
            EastAsiaFontNames.Add("DOTUMCHE");
            EastAsiaFontNames.Add("돋움체");
            EastAsiaFontNames.Add("GUNGSUHCHE");
            EastAsiaFontNames.Add("궁서체");
            EastAsiaFontNames.Add("BATANG");
            EastAsiaFontNames.Add("바탕");
            EastAsiaFontNames.Add("GULIM");
            EastAsiaFontNames.Add("굴림");
            EastAsiaFontNames.Add("DOTUM");
            EastAsiaFontNames.Add("돋움");
            EastAsiaFontNames.Add("GUNGSUH");
            EastAsiaFontNames.Add("궁서");
            EastAsiaFontNames.Add("AMI R");
            EastAsiaFontNames.Add("휴먼아미체");
            EastAsiaFontNames.Add("HYPMOKGAK-BOLD");
            EastAsiaFontNames.Add("HY목각파임B");
            EastAsiaFontNames.Add("HYSHORTSAMUL-MEDIUM");
            EastAsiaFontNames.Add("HY얕은샘물M");
            EastAsiaFontNames.Add("HYPOST-MEDIUM");
            EastAsiaFontNames.Add("HY엽서M");
            EastAsiaFontNames.Add("HEADLINE R");
            EastAsiaFontNames.Add("휴먼둥근헤드라인");
            EastAsiaFontNames.Add("PYUNJI R");
            EastAsiaFontNames.Add("휴먼편지체");
            EastAsiaFontNames.Add("HYGOTHIC-EXTRA");
            EastAsiaFontNames.Add("HY견고딕");
            EastAsiaFontNames.Add("HYSINMUN-MYEONGJO");
            EastAsiaFontNames.Add("HY신문명조");
            EastAsiaFontNames.Add("HYMYEONGJO-EXTRA");
            EastAsiaFontNames.Add("HY견명조");
            EastAsiaFontNames.Add("HYTAJA-MEDIUM");
            EastAsiaFontNames.Add("HY타자M");
            EastAsiaFontNames.Add("HEADLINE SANS R");
            EastAsiaFontNames.Add("휴먼각진헤드라인");
            EastAsiaFontNames.Add("YET R");
            EastAsiaFontNames.Add("휴먼옛체");
            EastAsiaFontNames.Add("SIMSUN");
            EastAsiaFontNames.Add("宋体");
            EastAsiaFontNames.Add("SIMHEI");
            EastAsiaFontNames.Add("黑体");
            EastAsiaFontNames.Add("NSIMSUN");
            EastAsiaFontNames.Add("新宋体");
            EastAsiaFontNames.Add("KAITI");
            EastAsiaFontNames.Add("楷体");
            EastAsiaFontNames.Add("FANGSONG");
            EastAsiaFontNames.Add("仿宋");
            EastAsiaFontNames.Add("MICROSOFT JHENGHEI");
            EastAsiaFontNames.Add("MICROSOFT YAHEI");
            EastAsiaFontNames.Add("微软雅黑");
            EastAsiaFontNames.Add("LISU");
            EastAsiaFontNames.Add("隶书");
            EastAsiaFontNames.Add("YOUYUAN");
            EastAsiaFontNames.Add("幼圆");
            EastAsiaFontNames.Add("PMINGLIU");
            EastAsiaFontNames.Add("新細明體");
            EastAsiaFontNames.Add("MINGLIU");
            EastAsiaFontNames.Add("細明體");
            EastAsiaFontNames.Add("DFLIHEIBOLD(P)");
            EastAsiaFontNames.Add("華康儷粗黑");
            EastAsiaFontNames.Add("DFLIHEIBOLD");
            EastAsiaFontNames.Add("華康新儷粗黑");
            EastAsiaFontNames.Add("DFKAI-SB");
            EastAsiaFontNames.Add("標楷體");
            EastAsiaFontMapping.Add("新細明體", "PMingLiu");
            EastAsiaFontMapping.Add("細明體", "MingLiu");
            EastAsiaFontMapping.Add("華康儷粗黑", "DFLiHeiBold(P)");
            EastAsiaFontMapping.Add("華康新儷粗黑", "DFLiHeiBold");
            EastAsiaFontMapping.Add("標楷體", "DFKai-SB");
            EastAsiaFontMapping.Add("ＭＳ Ｐゴシック", "MS PGothic");
            EastAsiaFontMapping.Add("ＭＳ ゴシック", "MS Gothic");
            EastAsiaFontMapping.Add("ＨＧゴシックE", "HGGothicE");
            EastAsiaFontMapping.Add("ＨＧゴシックM", "HGGothicM");
            EastAsiaFontMapping.Add("HGSゴシックE", "HGSGothicE");
            EastAsiaFontMapping.Add("HGSゴシックM", "HGSGothicM");
            EastAsiaFontMapping.Add("HGPゴシックE", "HGPGothicE");
            EastAsiaFontMapping.Add("HGPゴシックM", "HGPGothicM");
            EastAsiaFontMapping.Add("HG丸ｺﾞｼｯｸM-PRO", "HGMaruGothicMPRO");
            EastAsiaFontMapping.Add("ＨＧ正楷書体-PRO", "HGSeikaishotaiPRO");
            EastAsiaFontMapping.Add("ＨＧ正楷書体", "HGSeikaishotai");
            EastAsiaFontMapping.Add("ＭＳ 明朝", "MS Mincho");
            EastAsiaFontMapping.Add("ＭＳ Ｐ明朝", "MS PMincho");
            EastAsiaFontMapping.Add("宋体", "SimSun");
            EastAsiaFontMapping.Add("黑体", "SimHei");
            EastAsiaFontMapping.Add("仿宋", "FangSong");
            EastAsiaFontMapping.Add("新宋体", "NSimSun");
            EastAsiaFontMapping.Add("楷体", "KaiTi");
            EastAsiaFontMapping.Add("微软雅黑", "Microsoft YaHei");
            EastAsiaFontMapping.Add("隶书", "LiSu");
            EastAsiaFontMapping.Add("幼圆", "YouYuan");
            EastAsiaFontMapping.Add("굴림체", "GulimChe");
            EastAsiaFontMapping.Add("바탕체", "BatangChe");
            EastAsiaFontMapping.Add("돋움체", "DotumChe");
            EastAsiaFontMapping.Add("궁서체", "GungsuhChe");
            EastAsiaFontMapping.Add("바탕", "Batang");
            EastAsiaFontMapping.Add("굴림", "Gulim");
            EastAsiaFontMapping.Add("돋움", "Dotum");
            EastAsiaFontMapping.Add("궁서", "Gungsuh");
            EastAsiaFontMapping.Add("휴먼아미체", "Ami R");
            EastAsiaFontMapping.Add("HY목각파임B", "HYPMokGak-Bold");
            EastAsiaFontMapping.Add("HY얕은샘물M", "HYShortSamul-Medium");
            EastAsiaFontMapping.Add("HY엽서M", "HYPost-Medium");
            EastAsiaFontMapping.Add("휴먼둥근헤드라인", "Headline R");
            EastAsiaFontMapping.Add("휴먼편지체", "Pyunji R");
            EastAsiaFontMapping.Add("HY견고딕", "HYGothic-Extra");
            EastAsiaFontMapping.Add("HY신문명조", "HYSinMun-MyeongJo");
            EastAsiaFontMapping.Add("HY견명조", "HYMyeongJo-Extra");
            EastAsiaFontMapping.Add("HY타자M", "HYTaJa-Medium");
            EastAsiaFontMapping.Add("휴먼각진헤드라인", "Headline Sans R");
            EastAsiaFontMapping.Add("휴먼옛체", "Yet R");
        }

        internal static string GetEquivalentEnglishFontName(string fontName)
        {
            string str;
            if (EastAsiaFontMapping.TryGetValue(fontName, out str))
            {
                return str;
            }
            return fontName;
        }
    }
}


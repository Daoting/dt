#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ArticlePage : PageWin
    {

        public ArticlePage(long p_id)
        {
            InitializeComponent();

            if (p_id == 1)
            {
                Title = "社区康养";
                _tbTitle.Text = "赵阿姨脑卒中的康复";
                _img.Source = new BitmapImage(new Uri("ms-appx:///Bs.Kehu/Assets/u285.jpg"));
                _tbContent.Text = "对脑卒中患者而言，康复治疗是一个长期的过程，在不同的时期采用一切有效的措施均可改善受损的功能障碍，预防并发症，提高脑卒中患者的生活质量。\r\n抗痉挛体位又称良肢位，以保持肢体的良好功能为目的，防止或对抗痉挛模式的出现，预防继发性关节挛缩、畸形或肌肉萎缩，防止压疮、肺炎及深静脉血栓的出现。对脑卒中患者而言，康复治疗是一个长期的过程，在不同的时期采用一切有效的措施均可改善受损的功能障碍，预防并发症，提高脑卒中患者的生活质量。\r\n抗痉挛体位又称良肢位，以保持肢体的良好功能为目的，防止或对抗痉挛模式的出现，预防继发性关节挛缩、畸形或肌肉萎缩，防止压疮、肺炎及深静脉血栓的出现。对脑卒中患者而言，康复治疗是一个长期的过程，在不同的时期采用一切有效的措施均可改善受损的功能障碍，预防并发症，提高脑卒中患者的生活质量。\r\n抗痉挛体位又称良肢位，以保持肢体的良好功能为目的，防止或对抗痉挛模式的出现，预防继发性关节挛缩、畸形或肌肉萎缩，防止压疮、肺炎及深静脉血栓的出现。对脑卒中患者而言，康复治疗是一个长期的过程，在不同的时期采用一切有效的措施均可改善受损的功能障碍，预防并发症，提高脑卒中患者的生活质量。\r\n抗痉挛体位又称良肢位，以保持肢体的良好功能为目的，防止或对抗痉挛模式的出现，预防继发性关节挛缩、畸形或肌肉萎缩，防止压疮、肺炎及深静脉血栓的出现。";

            }
            else
            {
                Title = "疾患解读";
                _tbTitle.Text = "虚弱的防治";
                _img.Source = new BitmapImage(new Uri("ms-appx:///Bs.Kehu/Assets/u287.png"));
                _tbContent.Text = "虚弱フレイル「Frailty」是指身身体和精神（筋力和精神面衰退）对ストレス的能力变弱的状态。但是、如果这种状态如果尽早介入的话，就有恢复到原来状态的可能性。高龄者フレイル「Frailty」、不仅仅是降低了生活质量、而是有引起各种各样的合并症的危险。根据フレイル的基准、フレイル状态，会发生什么进行简单易懂的解释。";

            }
        }

    }
}
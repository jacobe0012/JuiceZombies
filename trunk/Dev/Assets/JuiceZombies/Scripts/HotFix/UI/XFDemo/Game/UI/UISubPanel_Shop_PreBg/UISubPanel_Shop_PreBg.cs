//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using cfg.config;
using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_PreBg)]
    internal sealed class UISubPanel_Shop_PreBgEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_PreBg;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_PreBg>();
        }
    }

    public partial class UISubPanel_Shop_PreBg : UI, IAwake<List<int>>
    {
        private Tbdraw_banner tbdraw_Banner;
        private Tblanguage tblanguage;
        private Tbquality tbquality;
        public List<UI> uiList = new List<UI>();
        private int itemDeltaH = 35;

        public void Initialize(List<int> BoxInt)
        {
            tbdraw_Banner = ConfigManager.Instance.Tables.Tbdraw_banner;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbquality = ConfigManager.Instance.Tables.Tbquality;
            List<draw_banner> banners = new List<draw_banner>();
            foreach (var banner in tbdraw_Banner.DataList)
            {
                if (banner.id == BoxInt[0] && banner.quality == BoxInt[1])
                {
                    banners.Add(banner);
                }
            }

            string pHelp = banners[0].power.ToString();
            int pCount = pHelp.Count<char>();
            string poHelp = "";
            if (pCount == 1)
            {
                poHelp = "0.0" + pHelp;
            }
            else if (pCount == 2)
            {
                poHelp = "0." + pHelp;
            }
            else if (pCount < 5)
            {
                for (int i = 0; i < pCount - 2; i++)
                {
                    poHelp += pHelp[i];
                }

                poHelp += ".";
                for (int i = pCount - 2; i < pCount; i++)
                {
                    poHelp += pHelp[i];
                }
            }
            else
            {
                poHelp = "100.00";
            }

            this.GetFromReference(KTitleText).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbquality[banners[0].quality].name).current);
            this.GetFromReference(KTitleText).GetTextMeshPro().SetColor("#" + tbquality[banners[0].quality].fontColor);
            this.GetFromReference(KProbabilityText).GetTextMeshPro().SetTMPText(
                tblanguage.Get("drawbox_preview_text").current
                + poHelp + "%");
            //this.GetFromReference(KBg).GetImage().SetSprite(tbquality[banners[0].quality].previewBg, false);
           // this.GetFromReference(KTitleImg).GetImage().SetSprite(tbquality[banners[0].quality].previewTitle, false);

            //int quality = banners[0].quality;   
            //if (banners[0].quality > 50)
            //{
            //    quality -= 50;
            //}


            //List<Vector3> reList = new List<Vector3>();
            //int count = banners[0].info.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    int ihelp = i;
            //    Vector3 vector3 = new Vector3();
            //    vector3[0] = 11;
            //    vector3[1] = banners[0].info[ihelp];
            //    vector3[2] = quality;
            //    reList.Add(vector3);
            //}

            //for (int i = 0; i < count; i++)
            //{
            //    int ihelp = i;
            //    var uiRe = UIHelper.Create(UIType.UICommon_RewardItem, reList[ihelp],
            //            this.GetFromReference(KBg).GameObject.transform);
            //    JiYuUIHelper.SetDefaultRect(uiRe);

            //    uiList.Add(uiRe);
            //}
            //PosInit();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}
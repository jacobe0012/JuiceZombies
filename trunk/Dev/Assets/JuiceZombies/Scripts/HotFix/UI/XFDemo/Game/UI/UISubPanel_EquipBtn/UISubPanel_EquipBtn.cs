//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_EquipBtn)]
    internal sealed class UISubPanel_EquipBtnEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_EquipBtn;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_EquipBtn>();
        }
    }

    public partial class UISubPanel_EquipBtn : UI, IAwake<int>
    {
        public int tagFuncId;

        //字体大小
        public int initSize = 50;
        public int ChageSize = 60;


        //颜色通道
        public float InitAlaph = 190 / 255.0f;
        public float ChageAlaph = 255 / 255.0f;
        private Color InitColor;
        public int ButtonIndex;
        public int posId;

        public void Initialize(int args)
        {
            this.posId = posId;
            tagFuncId = posId;
        }

        void InitNode()
        {
            var KBtn = GetFromReference(UISubPanel_EquipBtn.KBtn);
            var KText_Name = GetFromReference(UISubPanel_EquipBtn.KText_Name);
            var KImg_Bottom = GetFromReference(UISubPanel_EquipBtn.KImg_Bottom);
            var KImg_icon = GetFromReference(UISubPanel_EquipBtn.KImg_icon);
        }

        public void ChageState2(bool enableUnderLine, float fontSize)
        {
            var KText_Name = GetFromReference(UICommon_SubBtn.KText_Name);
            var KImg_Bottom = GetFromReference(UICommon_SubBtn.KImg_Bottom);
            if (enableUnderLine)
            {
                KImg_Bottom.GetImage().SetSpriteAsync("img_equip_selected", true);
            }
            else
            {
                KImg_Bottom.GetImage().SetSpriteAsync("img_equip_unselected", true);
            }
            // if (isSelected)
            // {
            //     // KText_Name.GetTextMeshPro().SetFontSize(fontSize);
            //     // KText_Name.GetTextMeshPro().SetAlpha(ChageAlaph);
            //     // KText_Name.GetTextMeshPro().SetFontStyle(FontStyles.Bold);
            //     // KImg_Bottom.GetImage().SetAlpha(ChageAlaph);
            // }
            // else
            // {
            //     // KText_Name.GetTextMeshPro().SetFontSize(fontSize);
            //     // KText_Name.GetTextMeshPro().SetAlpha(InitAlaph);
            //     // KText_Name.GetTextMeshPro().SetFontStyle(FontStyles.Normal);
            //     // KImg_Bottom.GetImage().SetAlpha(InitAlaph);
            // }

            //KImg_Bottom.SetActive(enableUnderLine);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}
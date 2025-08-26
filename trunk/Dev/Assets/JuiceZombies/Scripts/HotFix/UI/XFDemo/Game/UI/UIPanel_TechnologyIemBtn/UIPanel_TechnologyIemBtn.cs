//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Main;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_TechnologyIemBtn)]
    internal sealed class UIPanel_TechnologyIemBtnEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIPanel_TechnologyIemBtn;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_TechnologyIemBtn>();
        }
    }

    public partial class UIPanel_TechnologyIemBtn : UI, IAwake<int>
    {
        private Tbbattletech tbBattletech;
        private Tblanguage tbLanguage;
        private Tbskill_quality tbSkill_quality;

        public void Initialize(int technologyID)
        {
            tbBattletech = ConfigManager.Instance.Tables.Tbbattletech;
            tbLanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbSkill_quality = ConfigManager.Instance.Tables.Tbskill_quality;
            //var color = SetColorForQuality();


            var qulityStr = tbBattletech.Get(technologyID).typeIcon;
            this.GetImage(KImg_icon_type)?.SetSpriteAsync(qulityStr, false);
            this.GetTextMeshPro(KText_type)?.SetTMPText(tbLanguage.Get("text_type").current);
            this.GetTextMeshPro(KText_typeName)?.SetTMPText(tbLanguage.Get(tbBattletech.Get(technologyID).typeName).current);
            this.GetTextMeshPro(KTxt_describe)?.SetTMPText(tbLanguage.Get(tbBattletech.Get(technologyID).desc).current);
            this.GetTextMeshPro(KTxt_name_Technology)
                ?.SetTMPText(tbLanguage.Get(tbBattletech.Get(technologyID).name).current);
            //this.GetTextMeshPro(KTxt_Descrip_Technology)?.SetTMPText(tbLanguage.Get(tbBattletech.Get(technologyID).desc).current);
            this.GetImage(KImg_icon_technology)?.SetSprite(tbBattletech.Get(technologyID).icon, false);
            //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Tip),
            //    async () => OnTipBtnClickAsync(technologyID));
        }

        private async UniTaskVoid OnTipBtnClickAsync(int technologyID)
        {
            //if (UnicornUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui))
            //{
            //      UnicornUIHelper.DestoryAllTips();;
            //    return;
            //}

            //var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips, new UICommon_ItemTips.ItemTipsData
            //{
            //    itemUI = this,
            //    KTxt_Title = null,
            //    KTxt_Des = tbLanguage.Get(tbBattletech.Get(technologyID).desc).current,
            //    //ArrowType = UICommon_ItemTips.ArrowType.Default
            //    ArrowType = UICommon_ItemTips.ArrowType.Up
            //}) as UICommon_ItemTips;

            //var KTxt_Des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
            //KTxt_Des.GetTextMeshPro().Get().alignment = TextAlignmentOptions.Center;

           
            //// tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title).SetActive(false);
            //// var des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
            //// var content = tipUI.GetFromReference(UICommon_ItemTips.KContent);
            ////
            //// des.GetTextMeshPro().SetTMPText(tbLanguage.Get(tbBattletech.Get(technologyID).desc).current);
            //// var preferredHeight = des.GetTextMeshPro().Get().preferredHeight;
            //// des.GetRectTransform().SetAnchoredPositionY(-preferredHeight);
            //// des.GetRectTransform().SetHeight(preferredHeight);
            //// des.GetRectTransform().SetWidth(des.GetTextMeshPro().Get().preferredWidth);
            ////
            //// SetTipPos(this, tipUI, UICommon_ItemTips.KContent,
            ////     UICommon_ItemTips.KImg_ArrowDown, UICommon_ItemTips.KImg_ArrowUp);
            //// content.GetRectTransform().SetWidth(des.GetTextMeshPro().Get().preferredWidth + 20);
            ////
            //// var oldHeight = content.GetRectTransform().Height();
            //// content.GetRectTransform().SetHeight(preferredHeight * 3);
            //// var offsetY = (oldHeight - content.GetRectTransform().Height()) / 2;
            //// content.GetRectTransform().SetAnchoredPositionY(offsetY);
        }

      

        private void SetTipPos(UI itemUI, UI tipUI, string contentKey, string arrowDownKey, string arrowUpKey,
            float contentGap = 30f)
        {
            var arrowDown = tipUI.GetFromReference(arrowDownKey);
            var arrowUp = tipUI.GetFromReference(arrowUpKey);


            var itemRect = itemUI.GetRectTransform();
            var tipRect = tipUI.GetRectTransform();


            ScrollRect scrollRect = itemUI.GameObject.transform.GetComponentInParent<ScrollRect>();

            Canvas canvas = itemUI.GameObject.transform.GetComponentInParent<Canvas>();

            // var rewardPosX = itemRect.AnchoredPosition().x;
            // var parentPos = scrollRect.content.anchoredPosition;


            RectTransform canvasRect = canvas.transform.GetComponent<RectTransform>();
            Vector3 loadpos = canvas.transform.InverseTransformPoint(itemUI.GameObject.transform.position);


            if (scrollRect != null)
            {
                Vector3 scrollRectPos =
                    canvas.transform.InverseTransformPoint(scrollRect.transform.position);
                var scrollRectWidth = scrollRect.transform.GetComponent<RectTransform>().rect.width;
                var scrollRectLeftPos = scrollRectPos.x - scrollRectWidth / 2f;
                var scrollRectRightPos = scrollRectPos.x + scrollRectWidth / 2f;

                loadpos.x = math.clamp(loadpos.x, scrollRectLeftPos, scrollRectRightPos);
            }

            var itemUpOffset = itemRect.Width() / 2f * itemRect.Scale().x;
            var tipUpOffset = tipRect.Height() / 2f * tipRect.Scale().y;

            float offsetY = itemUpOffset + tipUpOffset;
            offsetY -= itemRect.Width() * itemRect.Scale().x + tipRect.Height() * tipRect.Scale().y;
            arrowDown.SetActive(false);
            arrowUp.SetActive(true);


            var tipPos = new Vector3(loadpos.x, loadpos.y + offsetY + 100);


            tipUI.GetRectTransform().SetAnchoredPosition(tipPos);
            var tipWidth = tipRect.Width() * tipRect.Scale().x;

            var arrow = tipUI.GetFromReference(arrowDownKey).GetRectTransform();
            var arrowWidth = arrow.Width() * arrow.Scale().x;


            var screenPosL = -(Screen.width / 2f);
            var screenPosR = Screen.width / 2f;
            var tipPosL = loadpos.x - tipWidth / 2;
            var tipPosR = loadpos.x + tipWidth / 2;
            var contentRect = tipUI.GetFromReference(contentKey).GetRectTransform();

            if (tipPosL < screenPosL + contentGap)
            {
                var contentPos = math.length(tipPosL) - math.length(screenPosL + contentGap);
                contentPos = math.min(contentPos, tipWidth / 2f - arrowWidth / 2f);

                contentRect.SetAnchoredPosition(new Vector2(contentPos, 0));
            }
            else if (tipPosR > screenPosR - contentGap)
            {
                var contentPos = math.length(tipPosR) - math.length(screenPosR - contentGap);
                contentPos = math.min(contentPos, tipWidth / 2f - arrowWidth / 2f);

                contentRect.SetAnchoredPosition(new Vector2(-contentPos, 0));
            }
            else
            {
                contentRect.SetAnchoredPosition(Vector2.zero);
            }
        }


        private string SetColorForQuality(int quality)
        {

            tbSkill_quality = ConfigManager.Instance.Tables.Tbskill_quality;

            string color = "";
            switch (quality)
            {
                case 1:
                    color = "#357AEA";
                    break;
                case 2:
                    color = "#E7B008";
                    break;
                case 3:
                    color = "#9234EA";
                    break;
            }

            return color;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}
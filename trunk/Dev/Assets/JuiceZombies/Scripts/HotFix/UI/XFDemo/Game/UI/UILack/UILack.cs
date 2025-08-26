//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UILack)]
    internal sealed class UILackEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UILack;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UILack>();
        }
    }

    public partial class UILack : UI, IAwake<int>
    {
        public void Initialize(int args)
        {
            var maskBtn = GetFromReference(KMaskBtn);
            maskBtn.GetXButton().OnClick.Add(() => { this.Close(); });
            //����1 ��ʯ2 ���3
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var chapterConfig = ConfigManager.Instance.Tables.Tbchapter;

            var titleText = GetFromReference(KTitleText);
            titleText.GetTextMeshPro().SetTMPText(language.Get(user_varibles.Get(args).lackTitle).current);

            chapter chapter = default;
            foreach (var chap in chapterConfig.DataList)
            {
                if (chap.levelId == ResourcesSingletonOld.Instance.levelInfo.levelId)
                {
                    chapter = chap;
                    break;
                }
            }

            int costNum = default;
            if (chapter == default)
            {
                return;
            }

            foreach (var cost in chapter.cost)
            {
                if (cost.x == 1)
                {
                    costNum = (int)cost.z;
                    break;
                }
            }


            long count = 0;
            string iconBG = default;
            switch (args)
            {
                //TODO:������������
                case 1:
                    count = costNum - ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy;
                    iconBG = "pic_equip_quality_frame_3";
                    break;
                case 2:
                    //count = costNum - ResourcesSingletonOld.Instance.gems;
                    iconBG = "pic_equip_quality_frame_4";
                    break;
                case 3:
                    //count = costNum - ResourcesSingletonOld.Instance.gold;
                    iconBG = "pic_equip_quality_frame_3";
                    break;
            }

            var contentText = GetFromReference(KContentText);
            contentText.GetTextMeshPro()
                .SetTMPText(
                    string.Format(language.Get(user_varibles.Get(args).lackDesc).current, count));

            var buyText = GetFromReference(KBuyText);
            buyText.GetTextMeshPro().SetTMPText(language.Get(user_varibles.Get(args).lackGain[0]).current);

            var gainText = GetFromReference(KGainText);
            gainText.GetTextMeshPro().SetTMPText(language.Get("common_gain_way").current);

            var gotoText = GetFromReference(KGotoText);
            gotoText.GetTextMeshPro().SetTMPText(language.Get("common_state_goto").current);


            var itemIcon = GetFromReference(KItemIcon);
            itemIcon.GetImage().SetSpriteAsync(user_varibles.Get(args).icon, false).Forget();
            var itemBG = GetFromReference(KItemBG);
            itemBG.GetImage().SetSpriteAsync(iconBG, false).Forget();


            var closeBtn = GetFromReference(KCloseBtn);

            var gotoBtn = GetFromReference(KGotoBtn);
            closeBtn.GetRectTransform().SetScale(Vector2.one);
            gotoBtn.GetRectTransform().SetScale(Vector2.one);

            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(gotoBtn, () => { });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(closeBtn, () => { this.Close(); });
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}
//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using Common;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIBag)]
    internal sealed class UIBagEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIBag;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UIBag>();
        }
    }

    public partial class UIBag : UI, IAwake<List<BagItem>>
    {
        public void Initialize(List<BagItem> args)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var tag_func = ConfigManager.Instance.Tables.Tbtag_func;
            List<bool> asdf = new List<bool>();
            asdf.Sort();

            foreach (var bagItem in args)
            {
                Log.Debug($"{bagItem}", Color.red);
            }

           
            //func_3604_name£¬item_page_2£¬item_page_1£¬item_page_0
            var bagText = GetFromReference(KBagText);
            var consumeText = GetFromReference(KConsumeText);
            var propText = GetFromReference(KPropText);
            var allText = GetFromReference(KAllText);
            var closeBtn = GetFromReference(KCloseBtn);
            var maskBtn = GetFromReference(KMaskBtn);
            var consumeBG = GetFromReference(KConsumeBG);
            var propBG = GetFromReference(KPropBG);
            var allBG = GetFromReference(KAllBG);
            int bagid = 3606;
            bagText.GetTextMeshPro().SetTMPText($"{language.Get(tag_func.Get(bagid).name).current}");
            consumeText.GetTextMeshPro().SetTMPText($"{language.Get("item_page_2").current}");
            propText.GetTextMeshPro().SetTMPText($"{language.Get("item_page_1").current}");
            allText.GetTextMeshPro().SetTMPText($"{language.Get("item_page_0").current}");

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(closeBtn, () =>
            {
                this.GetChild<UIBagThingList>(KItemView).DisableTipPanel();

                this.Close();
            });
            maskBtn.GetXButton().OnClick.Add(() =>
            {
                this.GetChild<UIBagThingList>(KItemView).DisableTipPanel();

                this.Close();
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(consumeBG,
                () => { this.GetChild<UIBagThingList>(KItemView).DisableTipPanel(); });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(propBG,
                () => { this.GetChild<UIBagThingList>(KItemView).DisableTipPanel(); });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(allBG,
                () => { this.GetChild<UIBagThingList>(KItemView).DisableTipPanel(); });

            var bGMask = GetFromReference(KBGMask);
            bGMask.GetXButton().OnClick.Add(() => { this.GetChild<UIBagThingList>(KItemView).DisableTipPanel(); });
            this.RemoveChild(KItemView);
            this.AddChild(typeof(UIBagThingList), KItemView, args, true);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}
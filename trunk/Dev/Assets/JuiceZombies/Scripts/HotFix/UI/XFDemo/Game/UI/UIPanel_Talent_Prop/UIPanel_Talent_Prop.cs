//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Talent_Prop)]
    internal sealed class UIPanel_Talent_PropEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Talent_Prop;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Talent_Prop>();
        }
    }

    public partial class UIPanel_Talent_Prop : UI, IAwake<int>, ILoopScrollRectProvide<UIContainer_Bar>
    {
        private List<talent_level> tanlentLevelList;
        private Tbtalent tbtanlent;
        private Tblanguage lang;
        private List<talent> talentList;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private int type;

        public async void Initialize(int type)
        {
            await UnicornUIHelper.InitBlur(this);
            this.type = type;

            InitJson();
            talentList = tbtanlent.DataList.Where(a => a.type == 1).GroupBy(a => a.level)
                .Select(a => a.First()).ToList();


            if (type == 1)
            {
                ShowPropView().Forget();
            }
            else if (type == 2)
            {
                ShowSkillView().Forget();
            }

            InitNode().Forget();
            //UpdatePropView();
        }

        async UniTaskVoid InitNode()
        {
            var KCommon_Bottom = GetFromReference(UIPanel_Talent_Prop.KCommon_Bottom);
            var KScrollView_Item0 = KCommon_Bottom.GetFromReference(UICommon_Bottom.KScrollView_Item0);
            var KBtn_Close = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_Close);
            var KBtn_TitleInfo = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_TitleInfo);
            var KText_BottomTitle = KCommon_Bottom.GetFromReference(UICommon_Bottom.KText_BottomTitle);
            KBtn_TitleInfo.SetActive(false);
            KText_BottomTitle.SetActive(false);
            KBtn_Close.GetXButton().RemoveAllListeners();
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, () => { this.Close(); });

            var content = KScrollView_Item0.GetScrollRect().Content;
            var bottomList = content.GetList();
            bottomList.Clear();
            for (int i = 1; i <= 2; i++)
            {
                var index = i;
                var ui = await bottomList.CreateWithUITypeAsync(UIType.UICommon_BottomBtn, index, false);
                var KText_Btn = ui.GetFromReference(UICommon_BottomBtn.KText_Btn);
                var KBg_Mask = ui.GetFromReference(UICommon_BottomBtn.KBg_Mask);
                var KBg_Mask1 = ui.GetFromReference(UICommon_BottomBtn.KBg_Mask1);
                var KImg_RedDot = ui.GetFromReference(UICommon_BottomBtn.KImg_RedDot);
                KBg_Mask.SetActive(true);
                KBg_Mask1.SetActive(true);

                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ui, () =>
                {
                    if (type == index)
                    {
                        return;
                    }

                    foreach (var child in bottomList.Children)
                    {
                        var uichild = child as UICommon_BottomBtn;

                        if (uichild.id == type)
                        {
                            var KBg_Mask = uichild.GetFromReference(UICommon_BottomBtn.KBg_Mask);
                            var KBg_Mask1 = uichild.GetFromReference(UICommon_BottomBtn.KBg_Mask1);
                            KBg_Mask.SetActive(true);
                            KBg_Mask1.SetActive(true);
                            break;
                        }
                    }

                    type = index;
                    KBg_Mask.SetActive(false);
                    KBg_Mask1.SetActive(false);
                    this.GetFromReference(KContainer_Info_Prop).SetActive(true);
                    this.GetFromReference(KContainer_Info_Skill).SetActive(false);
                    switch (type)
                    {
                        case 1:
                            ShowPropView().Forget();
                            break;
                        case 2:
                            ShowSkillView().Forget();
                            break;
                    }
                }, 1101);
                if (type == index)
                {
                    KBg_Mask.SetActive(false);
                    KBg_Mask1.SetActive(false);
                }

                string str = "";
                switch (index)
                {
                    case 1:
                        str = lang.Get("talent_attr_tab").current;
                        break;
                    case 2:
                        str = lang.Get("talent_skill_tab").current;
                        break;
                }

                KText_Btn.GetTextMeshPro().SetTMPText(str);
            }

            bottomList.Sort((a, b) =>
            {
                var uia = a as UICommon_BottomBtn;
                var uib = b as UICommon_BottomBtn;
                return uia.id.CompareTo(uib.id);
            });
        }

        private void InitJson()
        {
            tanlentLevelList = ConfigManager.Instance.Tables.Tbtalent_level.DataList;
            tbtanlent = ConfigManager.Instance.Tables.Tbtalent;
            lang = ConfigManager.Instance.Tables.Tblanguage;
            //tanlent = ConfigManager.Instance.Tables.Tbtalent;
        }


        private async UniTaskVoid ShowSkillView()
        {
            this.GetFromReference(KContainer_Info_Skill).SetActive(true);
            this.GetFromReference(KContainer_Info_Prop).SetActive(false);

            // this.GetFromReference(KBtn_Prop).GetImage().SetSprite("icon_switch_btn_black", false);
            // this.GetFromReference(KBtn_Skill).GetImage().SetSprite("icon_switch_btn", false);

            //var skillText = this.GetFromReference(KTxt_Prop).GetTextMeshPro();
            //skillText.SetFontSize(50);
            //skillText.SetColor(new Color(47f / 255f, 55f / 255f, 69f / 255f));
            //skillText.SetTMPText(lang.Get("talent_attr_tab").current);

            //var propText = this.GetFromReference(KTxt_Skill).GetTextMeshPro();
            //propText.SetFontSize(55);
            //propText.SetColor(new Color(0x94 / 255f, 0xA3 / 255f, 0xB8 / 255f));
            //propText.SetTMPText("<u>" + lang.Get("talent_skill_tab").current + "<u>");

            var grid = GetFromReference(KSkillItemList).GetComponent<GridLayoutGroup>();
            grid.padding.left = 0;
            grid.padding.right = 0;
            grid.padding.top = 30;
            grid.padding.bottom = 0;
            grid.cellSize = new Vector2(350, 400);
            grid.spacing = new Vector2(15, 15);
            //grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.constraintCount = 3;
            var KCommon_Bottom = GetFromReference(UIPanel_Talent_Prop.KCommon_Bottom);
            var KBtn_Close = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_Close);

            KBtn_Close.GetButton().SetEnabled(false);
            //var loopRect = this.GetFromReference(KContainer_Info_Skill).GetLoopScrollRect<UICommon_Btn2>();
            var loopList = this.GetFromReference(KContainer_Info_Skill).GetScrollRect().Content.GetList();
            loopList.Clear();
            for (int i = 0; i < tbtanlent.DataList.Count; i++)
            {
                if (tbtanlent.DataList[i].type == 2)
                {
                    var ihelp = i;

                    var ui = loopList.CreateWithUIType<int>(
                        UIType.UICommon_Btn2, tbtanlent.DataList[i].id, false) as UICommon_Btn2;
                    ui.index = ihelp;

                    UnicornTweenHelper.SetEaseAlphaAndPosB2U(ui.GetFromReference(UICommon_Btn2.KMid), 0, 20,
                        cancellationToken: cts.Token,
                        0.35f, false,
                        false);


                    //UnicornUIHelper.ChangePaddingLR(this, 50, 0.2f);
                    UnicornTweenHelper.ChangeSoftness(ui, 300, 0.35f, cancellationToken: cts.Token);
                }
            }

            loopList.Sort((a, b) =>
            {
                var uia = a as UICommon_Btn2;
                var uib = b as UICommon_Btn2;
                return uia.index.CompareTo(uib.index);
            });

            KBtn_Close.GetButton().SetEnabled(true);
        }

        private async UniTaskVoid ShowPropView()
        {
            var KContainer_Info_Prop = GetFromReference(UIPanel_Talent_Prop.KContainer_Info_Prop);
            this.GetFromReference(KContainer_Info_Skill).SetActive(false);
            KContainer_Info_Prop.SetActive(true);

            // this.GetFromReference(KBtn_Skill).GetImage().SetSprite("icon_switch_btn_black", false);
            // this.GetFromReference(KBtn_Prop).GetImage().SetSprite("icon_switch_btn", false);

            Log.Debug($"maxTanlentLevel:{talentList.Count}");
            //var loopRect = this.GetFromReference(KContainer_Info_Prop).GetLoopScrollRect<UIContainer_Bar>();

            var loopRect = KContainer_Info_Prop.GetLoopScrollRect<UIContainer_Bar>();

            loopRect.SetProvideData(UIPathSet.UIContainer_Bar, this);
            loopRect.SetTotalCount(talentList.Count);
            loopRect.RefillCells();
        }


        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }


        public async void ProvideData(UIContainer_Bar ui, int index)
        {
            var talent = talentList[index];
            ObjectHelper.Awake(ui, talent.level);
            if (talent.level <= 7)
            {
                UnicornTweenHelper.SetEaseAlphaAndPosB2U(ui.GetFromReference(UIContainer_Bar.KBg), 0, 50,
                    cancellationToken: cts.Token,
                    0.35f + 0.02f * (talent.level), true, true);
            }
        }
    }
}
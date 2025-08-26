//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using HotFix_UI;
using Main;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;


namespace XFramework
{
    [UIEvent(UIType.UIPanel_Sweep)]
    internal sealed class UIPanel_SweepEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Sweep;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Sweep>();
        }
    }

    public partial class UIPanel_Sweep : UI, IAwake
    {
        private Tblanguage tblanguage;
        private Tbmonthly tbmonthly;
        private Tbuser_variable tbuser_variable;
        private Tbchapter tbchapter;
        private Tbmonster_type tbmonster_type;
        private Tbsweep_times tbsweep_times;
        private int maxChapter;
        private int curSelect;
        private UI loading;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public async void Initialize()
        {
            await UnicornUIHelper.InitBlur(this);
            maxChapter = ResourcesSingletonOld.Instance.levelInfo.maxPassChapterID;
            maxChapter = 10;
            //var cardMap = ResourcesSingletonOld.Instance.a

            InitNode();
            InitBtn();
            UpdateCardState();
            InitText();


            curSelect = 1;
            ChangeMagnification(curSelect).Forget();


            //var top = GetFromReference(KTop);
            UnicornTweenHelper.SetEaseAlphaAndPosLtoR(this.GetFromReference(UIPanel_Sweep.KTop), 0, 100, cts.Token, 0.15f,
                false);
            UnicornTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(UIPanel_Sweep.KBg_TV),
                cancellationToken: cts.Token);

            var height1 = this.GetFromReference(UIPanel_Sweep.KMid).GetRectTransform().AnchoredPosition().y;
            UnicornTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UIPanel_Sweep.KMid), height1, 100, cts.Token,
                0.3f, false, true);
            var height2 = this.GetFromReference(UIPanel_Sweep.KBottom).GetRectTransform().AnchoredPosition().y;
            UnicornTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UIPanel_Sweep.KBottom), height2, 100, cts.Token,
                0.3f, false, true);
            var height3 = this.GetFromReference(UIPanel_Sweep.KSelectBottom).GetRectTransform().AnchoredPosition().y;
            UnicornTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UIPanel_Sweep.KSelectBottom), height3, 20,
                cts.Token, 0.3f, false, true);
            var height4 = this.GetFromReference(UIPanel_Sweep.KBtnGet).GetRectTransform().AnchoredPosition().y;
            UnicornTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UIPanel_Sweep.KBtnGet), height4, 20, cts.Token,
                0.3f, false, true);
            await UniTask.Delay(200, cancellationToken: cts.Token);
        }

        private void InitText()
        {
            this.GetTextMeshPro(KText_Mid).SetTMPText(tblanguage.Get("sweep_title").current);
            this.GetTextMeshPro(KText_Bottom).SetTMPText(tblanguage.Get("sweep_defeat_text").current);
            this.GetTextMeshPro(KText_KillCount).SetTMPText(tblanguage.Get("common_total").current);
            this.GetTextMeshPro(KText_Get).SetTMPText(tblanguage.Get("sweep_button").current);
            //this.GetTextMeshPro(KText_1).SetTMPText(tbsweep_times.Get(1).times.ToString());
            //this.GetTextMeshPro(KText_2).SetTMPText(tbsweep_times.Get(2).times.ToString());
            //this.GetTextMeshPro(KText_3).SetTMPText(tbsweep_times.Get(3).times.ToString());
            //this.GetTextMeshPro(KText_4).SetTMPText(tbsweep_times.Get(4).times.ToString());
            //this.GetTextMeshPro(KText_5).SetTMPText(tbsweep_times.Get(5).times.ToString());


            int id = 2;
            this.GetTextMeshPro(KTxtNotActive)?.SetTMPText(tblanguage.Get("common_state_unactivated").current);
            string num = ReturnBigGreenText(tbmonthly.Get(id).userVariable[0].y.ToString());
            string str = string.Format(tblanguage.Get("monthly_acc_unactivated_text").current, num,
                tblanguage.Get("sweep_title").current);
            str = str.Replace("%", ReturnBigGreenText("%"));
            this.GetTextMeshPro(KTextLauchDes).SetTMPText(str);

            this.GetTextMeshPro(KText_Launch).SetTMPText(tblanguage.Get("common_state_activatenow").current);


            str = string.Format(tblanguage.Get("monthly_acc_tips").current,
                ReturnBigGreenText(tblanguage.Get(tbuser_variable.Get(3).name).current),
                ReturnBigGreenText(tblanguage.Get(tbuser_variable.Get(4).name).current));
            this.GetTextMeshPro(KTextTipUp).SetTMPText(str);
            str = UnicornUIHelper.GetRewardTextIconName(tbuser_variable.Get(3).icon)
                  + ReturnBigGreenText(GetSweepReward(tbchapter.Get(maxChapter).sweepOnce, 3).ToString())
                  + UnicornUIHelper.GetRewardTextIconName(tbuser_variable.Get(4).icon)
                  + ReturnBigGreenText(GetSweepReward(tbchapter.Get(maxChapter).sweepOnce, 4).ToString());
            this.GetTextMeshPro(KTextTip).SetTMPText(str);

            this.GetTextMeshPro(KTxtActive).SetTMPText(tblanguage.Get("monthly_activated_text").current);

            str = string.Format(tblanguage.Get("common_acc_obj_text").current, num,
                tblanguage.Get("sweep_title").current);
            str = str.Replace("%", ReturnBigGreenText("%"));
            this.GetTextMeshPro(KTextLauchDesActive).SetTMPText(str);


            str = string.Format(tblanguage.Get("common_acc_text").current,
                tblanguage.Get("sweep_title").current);
            str += "\n";
            str += UnicornUIHelper.GetRewardTextIconName(UnicornUIHelper.GetVector3(UnicornUIHelper.Vector3Type.DOLLARS))
                   + ReturnBigGreenText(GetSweepReward(tbchapter.Get(maxChapter).sweepOnce, 3).ToString())
                   + UnicornUIHelper.GetRewardTextIconName(UnicornUIHelper.GetVector3(UnicornUIHelper.Vector3Type.EXP))
                   + ReturnBigGreenText(GetSweepReward(tbchapter.Get(maxChapter).sweepOnce, 4).ToString());
            this.GetTextMeshPro(KTxtAdditonAward).SetTMPText(str);

            var width = this.GetTextMeshPro(KTextTip).Get().preferredWidth;
            var width1 = this.GetTextMeshPro(KTextTipUp).Get().preferredWidth;
            width = width > width1 ? width1 : width1;
            var height = this.GetTextMeshPro(KTextTip).Get().preferredHeight +
                         this.GetTextMeshPro(KTextTipUp).Get().preferredWidth;

            GetFromReference(KImgTip)?.GetRectTransform().SetWidth(width + 20);
            GetFromReference(KImgTip)?.GetRectTransform().SetWidth(height + 30);
        }

        private string ReturnBigGreenText(string text, int size = 34, string color = "67CC4D")
        {
            return UnityHelper.RichTextSize(UnityHelper.RichTextColor(text, color), size);
        }


        private async void UpdateCardState()
        {
            var cardMap = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1404].SpecialCard.Unclaimed;
            UnReward cardData = new UnReward();
            foreach (var cm in cardMap)
            {
                if (cm.Value.Id == 2)
                {
                    cardData = cm.Value;
                }
            }

            bool isBuy = false;
            if (cardData.EndTime <= TimeHelper.ClientNowSeconds())
            {
                isBuy = false;
            }
            else
            {
                isBuy = true;
            }

            ResourcesSingletonOld.Instance.monthBuy = isBuy;

            if (ResourcesSingletonOld.Instance.monthBuy)
            {
                GetFromReference(KCardActive)?.SetActive(true);
                GetFromReference(KImgCard).GetXImage().SetGrayed(false);
                GetFromReference(KCardNotActive)?.SetActive(false);
                this.GetImage(KBtnLaunch)?.SetSprite("", false);
                //this.GetImage(KBtnLaunch)?.SetSprite(" icon_btn_yellow", false);
                GetFromReference(KBG_Money)?.GetImage().SetSprite("bg_sweep_yellow_big", false);
                GetFromReference(KBG_Exp)?.GetImage().SetSprite("bg_sweep_yellow", false);
                GetFromReference(KBG_Equip)?.GetImage().SetSprite("bg_sweep_yellow", false);
                await UniTask.Delay(200, cancellationToken: cts.Token);
                UnicornTweenHelper.PlayUIImageSweepFX(this.GetFromReference(UIPanel_Sweep.KImgCard),
                    cancellationToken: cts.Token);
            }
            else
            {
                GetFromReference(KCardActive)?.SetActive(false);
                GetFromReference(KImgCard).GetXImage().SetGrayed(true);
                GetFromReference(KCardNotActive)?.SetActive(true);

                GetFromReference(KBG_Money)?.GetImage().SetSprite("bg_sweep_gray_big", false);
                GetFromReference(KBG_Exp)?.GetImage().SetSprite("bg_sweep_gray", false);
                GetFromReference(KBG_Equip)?.GetImage().SetSprite("bg_sweep_gray", false);
                //this.GetImage(KBtnLaunch)?.SetSprite("icon_button_gray", false);
                await UniTask.Delay(200, cancellationToken: cts.Token);
                UnicornTweenHelper.PlayUIImageSweepFX(this.GetFromReference(UIPanel_Sweep.KBtnLaunch),
                    cancellationToken: cts.Token);
            }
        }

        private int GetSweepReward(List<Vector3> sweepOnce, int type)
        {
            for (int i = 0; i < sweepOnce.Count; i++)
            {
                if (sweepOnce[i].x == type)
                {
                    return (int)sweepOnce[i].z;
                }
            }

            return -1;
        }


        /// <summary>
        /// ȥ�¿�����
        /// </summary>
        private void GotoMonth()
        {
            var str = "type=1;para=[143]";
            UnicornUIHelper.GoToSomePanel(str);
            Close();
            UnicornUIHelper.DestroyAllSubPanel();
        }

        private void InitBtn()
        {
            this.GetButton(KTop).OnClick.Add( () => { GetFromReference(KImgTip)?.SetActive(false); });
            this.GetButton(KMid).OnClick.Add(() => { GetFromReference(KImgTip)?.SetActive(false); });
            this.GetButton(KBottom).OnClick.Add(() => { GetFromReference(KImgTip)?.SetActive(false); });
            this.GetXButton(KBg).OnClick.Add(async () => await ClosePanel());
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KBtnTipNot), () => { OpenTip(); });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KBtnTipYes), () => { OpenTip(); });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KBtnLaunch), () => { GotoMonth(); });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KBtnGet), () => { OnBtnGetClick(); });
            this.GetXButton(KContainer1).OnClick.Add(() => ChangeMagnification(1).Forget());
            this.GetXButton(KContainer2).OnClick.Add(() => ChangeMagnification(2).Forget());
            this.GetXButton(KContainer3).OnClick.Add(() => ChangeMagnification(3).Forget());
            this.GetXButton(KContainer4).OnClick.Add(() => ChangeMagnification(4).Forget());
            this.GetXButton(KContainer5).OnClick.Add(() => ChangeMagnification(5).Forget());
        }

        private async UniTask ClosePanel()
        {
            //UnicornTweenHelper.SetEaseAlphaAndPosUtoB(GetFromReference(UIPanel_Sweep.KContainer), 0 - 100, 100, 0.15f, false);
            //UnicornTweenHelper.SetEaseAlphaAndPosRtoL(GetFromReference(UIPanel_Sweep.KContainer), 0 - 100, 100, 0.15f, false);
            //GetFromReference(UIPanel_Sweep.KContainer).GetComponent<CanvasGroup>().alpha = 1f;
            //GetFromReference(UIPanel_Sweep.KContainer).GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetEase(Ease.InQuad);
            //await UniTask.Delay(150);
            Close();
        }

        private async UniTaskVoid ChangeMagnification(int param)
        {
            for (int i = 0; i < 5; i++)
            {
                this.GetFromReference(KContainerSelect).GetRectTransform().GetChild(i).GetChild(0)
                    .GetComponent<XImage>()
                    .SetSprite(ResourcesManager.LoadAsset<Sprite>("icon_sweep_item"), false);
                this.GetFromReference(KContainerSelect).GetRectTransform().GetChild(curSelect - 1).GetChild(0)
                    .SetScale2(1f);
                this.GetFromReference(KContainerSelect).GetRectTransform().GetChild(curSelect - 1).GetChild(1)
                    .GetComponent<TMP_Text>()
                    .SetFontSize(34);
            }

            this.GetXButton(KBtnL).OnClick.RemoveAllListeners();
            this.GetXButton(KBtnR).OnClick.RemoveAllListeners();
            //Log.Debug($"{"Container" + curSelect.ToString()}");
            //this.GetFromReference(KContainerSelect).GetRectTransform().GetChild(curSelect - 1).GetComponent<XImage>().SetSprite(ResourcesManager.LoadAsset<Sprite>("Ellipse 156 (1)"), false);

            curSelect = param;
            this.GetFromReference(KContainerSelect).GetRectTransform().GetChild(curSelect - 1).GetChild(0)
                .GetComponent<XImage>()
                .SetSprite(ResourcesManager.LoadAsset<Sprite>("icon_sweep_item_clicked"), false);
            this.GetFromReference(KContainerSelect).GetRectTransform().GetChild(curSelect - 1).GetChild(0)
                .SetScale2(1.5f);
            this.GetFromReference(KContainerSelect).GetRectTransform().GetChild(curSelect - 1).GetChild(1)
                .GetComponent<TMP_Text>()
                .SetFontSize(50);
            var tempL = (curSelect - 1 > 0) ? curSelect - 1 : 5;
            this.GetXButton(KBtnL).OnClick.Add(() => ChangeMagnification(tempL).Forget());

            var tempR = (curSelect + 1 < 6) ? curSelect + 1 : 1;
            this.GetXButton(KBtnR).OnClick.Add(() => ChangeMagnification(tempR).Forget());
            param = tbsweep_times.Get(param).times;
            Log.Debug($"���ʣ�{param}", Color.cyan);

            UpdateDisplay(param);

            long moneyCount = GetSweepReward(tbchapter.Get(maxChapter).sweepOnce, 3) * param;
            this.GetTextMeshPro(KText_MoneyCount).SetTMPText(moneyCount.ToString("#,0"));

            int expCount = GetSweepReward(tbchapter.Get(maxChapter).sweepOnce, 4) * param;
            this.GetTextMeshPro(KText_ExpCount).SetTMPText(UnicornUIHelper.FormatNumber(expCount));

            int equipCount = GetSweepReward(tbchapter.Get(maxChapter).sweepOnce, 5) * param;
            this.GetTextMeshPro(KText_EquipCount).SetTMPText(UnicornUIHelper.FormatNumber(equipCount));

            var list = GetFromReference(KEnemy).GetList();
            list.Clear();
            var monster = tbchapter.Get(maxChapter).sweepMonsterOnce;
            int sumCount = 0;
            for (int i = 0; i < 3; i++)
            {
                var ihelp = i;
                int temp = (int)(monster[0][i] * param);
                sumCount += temp;
                var ui = await list.CreateWithUITypeAsync(UIType.UISubPanel_SweepItem, false);
                ui.GetImage(UISubPanel_SweepItem.KImgMonster).SetSprite(tbmonster_type.Get(ihelp + 1).icon, false);
                ui.GetTextMeshPro(UISubPanel_SweepItem.KText_EnemyName).SetTMPText(
                    UnityHelper.RichTextColor(tblanguage.Get(tbmonster_type.Get(ihelp + 1).name).current,
                        tbmonster_type.Get(ihelp + 1).color));
                ui.GetTextMeshPro(UISubPanel_SweepItem.KText_EnemyCount)
                    .SetTMPText(UnityHelper.RichTextColor(UnicornUIHelper.FormatNumber(temp).ToString(),
                        tbmonster_type.Get(ihelp + 1).color));
            }

            list.Sort((a, b) =>
            {
                var uia = a as UISubPanel_SweepItem;
                var uib = b as UISubPanel_SweepItem;
                return uia.index.CompareTo(uib.index);
            });
            var content = tblanguage.Get("common_total").current + sumCount.ToString("#,0");
            this.GetTextMeshPro(KText_KillCount).SetTMPText(content);
            UnicornUIHelper.ForceRefreshLayout(GetFromReference(KEnemy));
            //this.GetXButton(KContainer1).OnClick.Invoke();
        }

        public int GetCurretSelect()
        {
            return tbsweep_times.Get(curSelect).times;
        }

        public void UpdateDisplay(int param)
        {
            var needEnergy = tbchapter.Get(maxChapter).cost[0].z * param;
            var str = needEnergy.ToString();
            if (ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy - needEnergy < 0)
            {
                str = UnityHelper.RichTextColor(str, "FF0000");
            }
            else
            {
                str = UnityHelper.RichTextColor(str, "FFFFFF");
            }

            this.GetTextMeshPro(KText_Consume).SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_energy") + str);
        }

        private async void OnBtnGetClick()
        {
            var cost = tbchapter.Get(maxChapter).cost[0];
            cost.z = cost.z * tbsweep_times.Get(curSelect).times;

            if (UnicornUIHelper.TryReduceReward(cost))
            {
                Log.Debug("OnBtnGetClick", Color.cyan);
                WebMessageHandlerOld.Instance.AddHandler(CMDOld.GETSWEEPREWARD, OnSweepGet);
                NetWorkManager.Instance.SendMessage(CMDOld.GETSWEEPREWARD,
                    new IntValue { Value = tbsweep_times.Get(curSelect).times });
                //UnicornTweenHelper.EnableLoading(true);
            }
            else
            {
                //UnicornUIHelper.ClearCommonResource();
                //var str = string.Format(tblanguage.Get("common_lack").current, tblanguage.Get(tbuser_variable.Get((int)cost.x).name).current);
                //UIHelper.CreateAsync(UIType.UICommon_Resource, str);
                UIHelper.CreateAsync(UIType.UIPanel_BuyEnergy);
            }
        }

        private void OnSweepGet(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.GETSWEEPREWARD, OnSweepGet);
            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(stringValueList);
            if (e.data.IsEmpty)
            {
                //await UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("text_reward_expire").current);
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            //UnicornTweenHelper.EnableLoading(false);

            List<Vector3> reList = new List<Vector3>();
            foreach (var itemstr in stringValueList.Values)
            {
                reList.Add(UnityHelper.StrToVector3(itemstr));
            }

            var param = tbsweep_times.Get(curSelect).times;
            UpdateDisplay(param);
            UIHelper.Create(UIType.UICommon_Reward, reList);
        }

        private void OpenTip()
        {
            GetFromReference(KImgTip)?.SetActive(!GetFromReference(KImgTip).GameObject.activeSelf);
        }

        void InitNode()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbmonthly = ConfigManager.Instance.Tables.Tbmonthly;
            tbuser_variable = ConfigManager.Instance.Tables.Tbuser_variable;
            tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            tbmonster_type = ConfigManager.Instance.Tables.Tbmonster_type;
            tbsweep_times = ConfigManager.Instance.Tables.Tbsweep_times;
        }

        protected override void OnClose()
        {
            curSelect = 1;
            GetFromReference(KImgTip)?.SetActive(false);
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
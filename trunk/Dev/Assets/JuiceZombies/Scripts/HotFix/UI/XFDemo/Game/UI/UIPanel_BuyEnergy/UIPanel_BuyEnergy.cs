//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Common;
using Google.Protobuf;
using HotFix_UI;
using Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_BuyEnergy)]
    internal sealed class UIPanel_BuyEnergyEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_BuyEnergy;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_BuyEnergy>();
        }
    }

    public partial class UIPanel_BuyEnergy : UI, IAwake
    {
        private Tblanguage tbLanguage;
        private Tbconstant tbconstant;
        private int raminAdTimes;
        private int raminDiamondTimes;
        private float currtime;
        private long timerId;
        private int curClickType;

        private CancellationTokenSource cts = new CancellationTokenSource();


        public async void Initialize()
        {
            await UnicornUIHelper.InitBlur(this);
            var KContainer = this.GetFromReference(UIPanel_BuyEnergy.KContainer);
            KContainer.GetRectTransform().SetAnchoredPositionY(-822.55f);
            this.GetRectTransform().SetAnchoredPositionY(0f);

            InitNode();
            InitTable();
            InitText();
            InitData();
            InitBtn();
            Anim().Forget();
        }

        async UniTaskVoid Anim()
        {
            var KContainer = this.GetFromReference(UIPanel_BuyEnergy.KContainer);

            UnicornTweenHelper.SetEaseAlphaAndPosB2U(KContainer, 0, cancellationToken: cts.Token);
            await UniTask.Delay(100, cancellationToken: cts.Token);

            float incremental = 200f;
            UnicornTweenHelper.SetEaseAlphaAndPosLtoR(this.GetFromReference(UIPanel_BuyEnergy.KContainerDiamondBuy), 0,
                200,
                cts.Token, 0.35f);
            //await UniTask.Delay(200);
            UnicornTweenHelper.SetEaseAlphaAndPosLtoR(this.GetFromReference(UIPanel_BuyEnergy.KContainerAdvertiseBuy), 0,
                400f,
                cts.Token, 0.35f);
        }

        private void InitData()
        {
            //��ѯ�������
            WebMessageHandlerOld.Instance.AddHandler(6, 5, OnQueryBuyTimes);
            NetWorkManager.Instance.SendMessage(6, 5);
        }

        private void OnQueryBuyTimes(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(6, 5, OnQueryBuyTimes);
            var configTimes = new PatrolConfig();
            configTimes.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            raminAdTimes = configTimes.AdEnergyTimes;
            raminDiamondTimes = configTimes.BuyEnergyTimes;
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.INITPLAYER, OnRequreTimeDownResponse);
            NetWorkManager.Instance.SendMessage(CMDOld.INITPLAYER);
        }

        private void InitTable()
        {
            tbLanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbconstant = ConfigManager.Instance.Tables.Tbconstant;
        }

        private void InitText()
        {
            GetFromReference(UIPanel_BuyEnergy.KText_Title).GetTextMeshPro()
                .SetTMPText(tbLanguage.GetOrDefault("common_lack_1_gain_1").current);
            GetFromReference(UIPanel_BuyEnergy.KTxtBtnAdvertiseNum).GetTextMeshPro()
                .SetTMPText(tbLanguage.GetOrDefault("common_free_text").current);
            GetFromReference(UIPanel_BuyEnergy.KTxtEnergyNum).GetTextMeshPro().SetTMPText(
                $"{ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy}/{ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyMax}");
            GetFromReference(UIPanel_BuyEnergy.KFillImage_Energy).GetImage().SetFillAmount(
                ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy * 1f /
                ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyMax);
        }

        private void InitBtn()
        {
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KBtnClose), () => { ClosePanel(); });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KCloseMask), () => { ClosePanel(); });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KBtnDiamondBuy),
                () => { OnClickBuyEnergy(0); });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KBtnAdvertiseBuy),
                () => { OnClickBuyEnergy(1); });
            StartTimer();
        }

        private void OnClickBuyEnergy(int buyType)
        {
            //curClickType=buyType;

            IntValue type = new IntValue()
            {
                Value = buyType
            };
            //���������Ľӿ�
            WebMessageHandlerOld.Instance.AddHandler(13, 7, OnBuyEnergyResponse);
            NetWorkManager.Instance.SendMessage(13, 7, type);
        }

        private async void OnBuyEnergyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(13, 7, OnBuyEnergyResponse);
            var str = new StringValue();
            str.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }
            //if (curClickType == 0)
            //{

            //}

            int adToEnergy = 5, diamondToEnergy = 15;
            var strList = UnicornUIHelper.TurnStrReward2List(str.Value);
            foreach (var item in strList)
            {
                if ((int)item.z == adToEnergy)
                {
                    raminAdTimes--;
                }
                else
                {
                    raminDiamondTimes--;
                }

                UnicornUIHelper.AddReward(item, true);
            }

            WebMessageHandlerOld.Instance.AddHandler(CMDOld.INITPLAYER, OnRequreTimeDownResponse);
            NetWorkManager.Instance.SendMessage(CMDOld.INITPLAYER);
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Patrol, out UI ui))
            {
                var ui1 = ui as UIPanel_Patrol;
                ui1.Initialize();
            }

            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Sweep, out UI uisweep))
            {
                var uisweep1 = uisweep as UIPanel_Sweep;
                uisweep1.UpdateDisplay(uisweep1.GetCurretSelect());
            }
        }

        private void OnRequreTimeDownResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.INITPLAYER, OnRequreTimeDownResponse);
            var gameRole = new GameRole();
            gameRole.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingletonOld.Instance.UserInfo = gameRole;

            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            var maxDiamondTimes = tbconstant.GetOrDefault("energy_bite_num").constantValue;
            var maxAdvertiseTimes = tbconstant.GetOrDefault("energy_ad_num").constantValue;
            var purPrice = tbconstant.GetOrDefault("energy_price").constantValue;

            GetFromReference(UIPanel_BuyEnergy.KText_DiamondDes).GetTextMeshPro().SetTMPText(string.Format(
                tbLanguage.Get("common_energy_pay_text").current,
                raminDiamondTimes.ToString()));
            GetFromReference(UIPanel_BuyEnergy.KText_AdvertiseDes).GetTextMeshPro()
                .SetTMPText(string.Format(tbLanguage.Get("common_energy_pay_text").current, raminAdTimes.ToString()));
            var truePrice = purPrice * (maxDiamondTimes - raminDiamondTimes + 1);
            Debug.Log($"truePrice:{truePrice}");
            GetFromReference(UIPanel_BuyEnergy.KTxtBtnDiamondNum).GetTextMeshPro().SetTMPText(truePrice.ToString());
            if (raminDiamondTimes > 0)
            {
                GetFromReference(KDiamondMask)?.SetActive(false);
            }
            else
            {
                GetFromReference(KDiamondMask)?.SetActive(true);
            }


            if (raminAdTimes > 0)
            {
                GetFromReference(KAderverMask)?.SetActive(false);

                GetFromReference(UIPanel_BuyEnergy.KTxtBtnAdvertiseNum).GetTextMeshPro()
                    .SetTMPText(tbLanguage.GetOrDefault("common_free_text").current);
            }
            else
            {
                GetFromReference(KAderverMask)?.SetActive(true);
            }

            var curEnergy = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy;
            var maxEnergy = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyMax;

            GetFromReference(UIPanel_BuyEnergy.KText_Recovery).SetActive(true);
            if (curEnergy >= maxEnergy)
            {
                GetFromReference(UIPanel_BuyEnergy.KText_Recovery).SetActive(false);
            }
            else
            {
                CloseTimeView();
                StartTimer();
            }
        }

        private void CloseTimeView()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }


        public void StartTimer()
        {
            if (ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy <
                ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyMax)
            {
                currtime = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyCountdown;
            }

            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(1000, this.Update);
        }

        private void Update()
        {
            GetFromReference(UIPanel_BuyEnergy.KTxtEnergyNum)?.GetTextMeshPro()
                ?.SetTMPText(
                    $"{ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy}/{ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyMax}");
            GetFromReference(UIPanel_BuyEnergy.KFillImage_Energy).GetImage().SetFillAmount(
                ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy * 1f /
                ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyMax);
            
            if (currtime < 0)
            {
                //ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy += 1;
                Log.Debug(
                    $"GetFromReference(UIPanel_BuyEnergy.KTxtEnergyNum):{GetFromReference(UIPanel_BuyEnergy.KTxtEnergyNum)}",
                    Color.cyan);
                if (ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy <
                    ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyMax)
                {
                    currtime = tbconstant.Get("energy_restore").constantValue;
                }
                else
                {
                    CloseTimeView();
                    return;
                }
            }

            currtime -= 0.2f;
            GetFromReference(UIPanel_BuyEnergy.KText_Recovery)?.GetTextMeshPro()?.SetTMPText(string.Format(
                tbLanguage.Get("common_energy_refresh_text").current,
                UnityHelper.RichTextColor(ToTimeFormat(currtime), "75FA53"),
                UnityHelper.RichTextColor(1.ToString(), "75FA53")));
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui))
            {
                var uis = ui as UIPanel_Main;
                uis?.RefreshResourceUI();
            }
        }

        public string ToTimeFormat(float time)
        {
            //����ȡ��
            int seconds = (int)time;
            //һСʱΪ3600�� ������3600ȡ����ΪСʱ
            int hour = seconds / 3600;
            //һ����Ϊ60�� ������3600ȡ���ٶ�60ȡ����Ϊ����
            int minute = seconds % 3600 / 60;
            //��3600ȡ���ٶ�60ȡ�༴Ϊ����
            seconds = seconds % 3600 % 60;
            //����00:00:00ʱ���ʽ
            return string.Format("{0:D2}:{1:D2}", minute, seconds);
        }


        private void ClosePanel()
        {
            CloseTimeView();
            Close();
        }

        void InitNode()
        {
        }

        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
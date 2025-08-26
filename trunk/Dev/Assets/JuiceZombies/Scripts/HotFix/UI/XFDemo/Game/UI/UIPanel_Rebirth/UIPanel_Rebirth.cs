//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using cfg.config;
using Common;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf;
using Main;
using Unity.Collections;
using Unity.Entities;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Rebirth)]
    internal sealed class UIPanel_RebirthEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Rebirth;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Rebirth>();
        }
    }

    public partial class UIPanel_Rebirth : UI, IAwake
    {
        private Tblanguage tblanguage;
        private Tblevel tblevel;
        private long timerId;

        private int curTime = 10;

        //private bool isExit = false;
        private CancellationTokenSource  cts = new CancellationTokenSource();

        public async void Initialize()
        {
            await UnicornUIHelper.InitBlur(this);
            this.SetActive(false);
           
            Log.Debug($"UIPanel_Rebirth Initialize");
            InitJson();

            UnicornUIHelper.StartStopTime(false);

            InitNode();
            UnicornTweenHelper.SetScaleWithBounce(GetFromReference(UIPanel_Rebirth.KImgBg));
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tblevel = ConfigManager.Instance.Tables.Tblevel;
        }

        async void InitNode()
        {
            if (ResourcesSingletonOld.Instance.levelInfo.rebirthNum <= 0)
            {
                UnicornTweenHelper.SetScaleWithBounceClose(GetFromReference(UIPanel_Rebirth.KImgBg));
                await UniTask.Delay(200, true);
                Close();
                UIHelper.CreateAsync(UIType.UIPanel_Fail);
                return;
            }

            this.SetActive(true);
            int reviveNum = (int)tblevel.Get(ResourcesSingletonOld.Instance.levelInfo.levelId).reviveNum[0].y;
            var KText_Title = GetFromReference(UIPanel_Rebirth.KText_Title);
            var KBtn_Close = GetFromReference(UIPanel_Rebirth.KBtn_Close);
            var KText_Time = GetFromReference(UIPanel_Rebirth.KText_Time);
            var KBtn_Advertise = GetFromReference(UIPanel_Rebirth.KBtn_Advertise);
            var KBtn_RebirthCoin = GetFromReference(UIPanel_Rebirth.KBtn_RebirthCoin);
            var KText_Confirm = GetFromReference(UIPanel_Rebirth.KText_Confirm);
            var KText_RebirthCoinNum = GetFromReference(UIPanel_Rebirth.KText_RebirthCoinNum);
            var KText_Desc = GetFromReference(UIPanel_Rebirth.KText_Desc);
            var KText_RemainNum = GetFromReference(UIPanel_Rebirth.KText_RemainNum);
            KText_Title.GetTextMeshPro().SetTMPText(tblanguage.Get("level_revive_title").current);
            KText_Desc.GetTextMeshPro().SetTMPText(tblanguage.Get("level_revive_desc").current);
            KText_Confirm.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_confirm").current);

            // var remainStr =
            //     $"{tblanguage.Get("level_revive_num").current}{ResourcesSingletonOld.Instance.levelInfo.rebirthNum}";
            var remainStr = string.Format(tblanguage.Get("level_revive_num").current,
                ResourcesSingletonOld.Instance.levelInfo.rebirthNum);
            KText_RemainNum.GetTextMeshPro().SetTMPText(remainStr);

            var rewardCount = UnicornUIHelper.GetRewardCount(new Vector3(5, 1010002, 0));
            var textMeshPro = KText_RebirthCoinNum.GetTextMeshPro();

            textMeshPro.SetTMPText($"{rewardCount}/{1}");

            if (rewardCount <= 0)
            {
                textMeshPro.SetColor($"FF0000");
            }
            else
            {
                textMeshPro.SetColor($"FFFFFF");
            }


            GetFromReference(UIPanel_Rebirth.KTimeFill)?.GetImage().DoFillAmount(1, 0, 10f);
            KBtn_Advertise.GetXImage().SetGrayed(ResourcesSingletonOld.Instance.levelInfo.adRebirthNum <= 0);

            KBtn_Advertise.GetXButton().SetEnabled(ResourcesSingletonOld.Instance.levelInfo.adRebirthNum > 0);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Advertise, async () =>
            {
                if (ResourcesSingletonOld.Instance.levelInfo.adRebirthNum > 0)
                {
                    ResourcesSingletonOld.Instance.levelInfo.adRebirthNum--;
                    ResourcesSingletonOld.Instance.levelInfo.rebirthNum--;
                    //TODO:���

                    UnicornTweenHelper.SetScaleWithBounceClose(GetFromReference(UIPanel_Rebirth.KImgBg));
                    await UniTask.Delay(200, true);


                    UnicornUIHelper.RebirthPlayer();
                    Close();
                }
                // else
                // {
                //     Close();
                // }
            });

            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_RebirthCoin,
                async () =>
                {
                    if (rewardCount <= 0)
                    {
                        UnicornUIHelper.ClearCommonResource();
                        UIHelper.CreateAsync( UIType.UICommon_Resource,
                            tblanguage.Get("common_lack_6_title").current).Forget();
                        return;
                    }

                    ResourcesSingletonOld.Instance.levelInfo.rebirthNum--;


                    WebMessageHandlerOld.Instance.AddHandler(CMDOld.CONSUMEREBIRTHCOIN, OnConsumeCoinResponse);
                    NetWorkManager.Instance.SendMessage(CMDOld.CONSUMEREBIRTHCOIN, new StringValue
                    {
                        Value = $"{5};{1010002};{1}"
                    });

                    UnicornUIHelper.TryReduceReward(new Vector3(5, 1010002, 1));
                    UnicornTweenHelper.SetScaleWithBounceClose(GetFromReference(UIPanel_Rebirth.KImgBg));
                    await UniTask.Delay(200, true);
                    UnicornUIHelper.RebirthPlayer();
                    Close();
                });
            //UIHelper.CreateAsync()
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close,
                async () =>
                {
                    UnicornTweenHelper.SetScaleWithBounceClose(GetFromReference(UIPanel_Rebirth.KImgBg));
                    await UniTask.Delay(200, true);
                    UIHelper.CreateAsync(UIType.UIPanel_Fail);
                    Close();
                });
            //StartTimer();
            SetTxtTime(cts.Token).Forget();
        }

        private async UniTaskVoid SetTxtTime(CancellationToken cct)
        {
            var KText_Time = GetFromReference(UIPanel_Rebirth.KText_Time);
            while (curTime > 0)
            {
                KText_Time.GetTextMeshPro().SetTMPText($"{curTime}");
                curTime -= 1;
                await UniTask.Delay(1000, true, PlayerLoopTiming.Update, cct);
            }

            UIHelper.CreateAsync(UIType.UIPanel_Fail);
            Close();
        }

        /// <summary>
        /// ������ʱ��
        /// </summary>
        // public void StartTimer()
        // {
        //     //����һ��ÿִ֡�е������൱��Update
        //     var timerMgr = TimerManager.Instance;
        //     //timerId = timerMgr.StartRepeatedTimer(updateInternal, this.Update);
        //     timerId = timerMgr.StartRepeatedTimer(100, this.Update);
        // }


        /// <summary>
        /// �Ƴ���ʱ��
        /// </summary>
        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }

        private void OnConsumeCoinResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.CONSUMEREBIRTHCOIN, OnConsumeCoinResponse);
            var response = new StringValue();
            response.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                UIHelper.CreateAsync(UIType.UIPanel_Fail);
                Close();
                Debug.Log("game pass is empty");
                return;
            }

            if (response.Value.Contains("true"))
            {
                UnicornUIHelper.RebirthPlayer();
                Close();
                return;
            }

            UIHelper.CreateAsync(UIType.UIPanel_Fail);
            Close();
        }


        protected override void OnClose()
        {
            //RemoveTimer();
            if (cts != null)
            {
                cts.Cancel();
            }

            curTime = 0;
            base.OnClose();
        }
    }
}
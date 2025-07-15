//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static HotFix_UI.JiYuUIHelper;
using static XFramework.UIManager;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Bank)]
    internal sealed class UIPanel_BankEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Bank;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Bank>();
        }
    }

    public partial class UIPanel_Bank : UI, IAwake
    {
        #region property

        private Tblanguage tblanguage;
        private Tbpiggy_bank tbpiggy_Bank;
        private Tbprice tbprice;
        private long timerId = 0;

        //test
        private int BankID = 1;
        private float DiamondNum = 200f;
        private bool CanBuy = true;
        private long BankCountDownTime = 0;
        private long BankEndTime = 0;
        private bool HaveSendMessage = false;
        private CancellationTokenSource cts = new CancellationTokenSource();

        #endregion

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);

            this.GetFromReference(KBtn_Close).GetXButton().OnClick?.Add(() => { Close(); });
            DataInit();
            TextInit();
            SetProgressbar();
            BtnInit();
            SetImg();
            SetState();
            SetUpdate();

            Anim().Forget();
        }

        async UniTaskVoid Anim()
        {
            JiYuTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(KImg_Bank), cancellationToken: cts.Token);
            JiYuTweenHelper.SetEaseAlphaAndPosLtoR(this.GetFromReference(KProgressbar), -52, 100, cts.Token, 0.15f,
                false);
            JiYuTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(KProgressbar), cts.Token, "FFF5C2", JiYuTweenHelper.UIDir.Right,
                0.5f, 1f);


            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(KBg), 0, 100, cts.Token, 0.3f, true, true);
            await UniTask.Delay(200, cancellationToken: cts.Token);
            JiYuTweenHelper.PlayUIImageSweepFX(this.GetFromReference(KImg_Diamond), cts.Token);
        }

        public void ReSet()
        {
            Debug.Log("panel bank reset");
            RemoveTimer();
            DataInit();
            TextInit();
            SetProgressbar();
            BtnInit();
            SetImg();
            SetState();
            SetUpdate();
        }

        public void CloseBank()
        {
            Close();
        }

        private void OnBankResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERYBANK, OnBankResponse);
            var gameBank = new GoldPig();
            gameBank.MergeFrom(e.data);
            Debug.Log(gameBank);

            if (e.data.IsEmpty)
            {
                Debug.Log("e is empty");
                return;
            }

            Debug.Log("panel bank game bank:" + gameBank);
            gameBank.PigLevel = gameBank.PigLevel % 100;
            ResourcesSingleton.Instance.goldPig = gameBank;

            Debug.Log("OnBankResponse gameBank.PigLevel" + gameBank.PigLevel);
            Debug.Log("OnBankResponse gameBank.Unlock" + gameBank.Unlock);

            if (gameBank.PigLevel == 0 || gameBank.Unlock == 0)
            {
                //var parent = this.GetParent<UICommonFunButton>();
                //this.SetActive(false);
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var ui))
                {
                    var uis = ui as UIPanel_Main;
                    uis.SetIconBtnEnable(3405, false);
                }

                Close();
            }
            else
            {
                ReSet();
            }
        }

        private void SetUpdate()
        {
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(1000, Update);
        }

        private void Update()
        {
            var KCommon_CloseInfo = GetFromReference(UIPanel_Bank.KCommon_CloseInfo);
            KCommon_CloseInfo.GetTextMeshPro().SetTMPText(tblanguage.Get("text_window_close").current);

            KCommon_CloseInfo.GetTextMeshPro().DoFade(1, 0.1f, 1f).AddOnCompleted(() =>
            {
                KCommon_CloseInfo.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });
            SetTextByTime();
        }
    
        private void DataInit()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbpiggy_Bank = ConfigManager.Instance.Tables.Tbpiggy_bank;
            tbprice = ConfigManager.Instance.Tables.Tbprice;
            var gameBank = ResourcesSingleton.Instance.goldPig;
            //Debug.Log(gameBank);
            DiamondNum = gameBank.GoldBank;

            foreach (var tbpb in tbpiggy_Bank.DataList)
            {
                if (tbpb.type == gameBank.PigType && tbpb.sort == gameBank.PigLevel)
                {
                    BankID = tbpb.id;
                    break;
                }
            }

            if (DiamondNum >= tbpiggy_Bank.Get(BankID).full)
            {
                CanBuy = true;
                BankCountDownTime = gameBank.Countdown;
                BankEndTime = gameBank.Countdown - ResourcesSingleton.Instance.serverDeltaTime / 1000 + 1;
                //Debug.Log("BankEndTime" + BankEndTime);
            }
            else
            {
                CanBuy = false;
            }
        }

        private void TextInit()
        {
            this.GetFromReference(KText_Top).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbpiggy_Bank.Get(BankID).name).current);
            this.GetFromReference(KText_FullNum).GetTextMeshPro().SetTMPText(tbpiggy_Bank.Get(BankID).full.ToString());
            this.GetFromReference(KText_Limite).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("piggy_bank_tips_type_2").current);
            this.GetFromReference(KText_Complete).GetTextMeshPro().SetTMPText(tblanguage.Get("text_success").current);
            this.GetFromReference(KText_Num).GetTextMeshPro().SetTMPText(JiYuUIHelper.GetRewardTextIconName("icon_diamond")+ DiamondNum.ToString());
            this.GetFromReference(KText_Btn).GetTextMeshPro().SetTMPText(
                tbprice.Get(tbpiggy_Bank.Get(BankID).price).rmb.ToString() +
                tblanguage.Get("common_coin_unit").current);
            this.GetFromReference(KText_Multiplier).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("gift_ratio_text").current);

            float priceCount = tbprice.Get(tbpiggy_Bank.Get(BankID).price).diamond;
            float nowDiamondCount = DiamondNum;
            float ratio = nowDiamondCount / priceCount;
            this.GetFromReference(KText_Over).GetTextMeshPro().SetTMPText("X" + ratio.ToString("F1"));
        }

        private void SetProgressbar()
        {
            int leftNum = tbpiggy_Bank.Get(BankID).base0;
            float rightNum = tbpiggy_Bank.Get(BankID).full;
            if (DiamondNum > rightNum)
            {
                DiamondNum = rightNum;
            }

            if (DiamondNum < leftNum)
            {
                DiamondNum = leftNum;
            }

            float progressRightOffset = (rightNum - DiamondNum) / rightNum;
            this.GetFromReference(KProgressbar).GetComponent<Slider>().value = progressRightOffset;

            if (DiamondNum == rightNum)
            {
                this.GetFromReference(KText_Complete).SetActive(true);
                this.GetFromReference(KText_Num).SetActive(false);
                this.GetFromReference(KText_Notice).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("piggy_bank_desc_2").current);
            }
            else
            {
                this.GetFromReference(KText_Complete).SetActive(false);
                this.GetFromReference(KText_Num).SetActive(true);
                this.GetFromReference(KText_Notice).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("piggy_bank_desc_1").current);
            }
        }

        private void BtnInit()
        {
            var KBtn_Buy = this.GetFromReference(UIPanel_Bank.KBtn_Buy);
            if (CanBuy)
            {
                this.GetFromReference(KImg_Grey).SetActive(false);
            }
            else
            {
                this.GetFromReference(KImg_Grey).SetActive(true);
            }

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Buy, () =>
            {
                if (CanBuy)
                {
                    const string Fund1 = "B03";
                    JiYuUIHelper.SendBuyMessage(Fund1, BankID);
                    /*
                    WebMessageHandler.Instance.AddHandler(15, 2, OnBuyBankResponse);
                    NetWorkManager.Instance.SendMessage(15, 2);*/
                }
                else
                {
                    var str = $"{tblanguage.Get("piggy_bank_pay_limit_tips").current}";
                    JiYuUIHelper.ClearCommonResource();
                    UIHelper.CreateAsync(UIType.UICommon_Resource, str);
                }
            });
        }

        private void OnBuyBankResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(15, 2, OnBuyBankResponse);

            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                return;
            }

            ResourcesSingleton.Instance.haveBuyBank = true;

            foreach (var itemstr in stringValueList.Values)
            {
                JiYuUIHelper.AddReward(UnityHelper.StrToVector3(itemstr), true);
            }

            Debug.Log("15, 2, OnBuyBankResponse");

            WebMessageHandler.Instance.AddHandler(CMD.QUERYBANK, OnBankResponse);
            NetWorkManager.Instance.SendMessage(CMD.QUERYBANK);
        }

        private void SetState()
        {
            if (CanBuy)
            {
                this.GetFromReference(KText_Time).SetActive(true);
                this.GetFromReference(KImg_Grey).SetActive(false);
            }
            else
            {
                this.GetFromReference(KText_Time).SetActive(false);
                this.GetFromReference(KImg_Grey).SetActive(true);
            }

            if (tbpiggy_Bank.Get(BankID).type == 2)
            {
                this.GetFromReference(KImg_Limite).SetActive(true);
            }
            else
            {
                this.GetFromReference(KImg_Limite).SetActive(false);
            }
        }

        private void SetImg()
        {
            this.GetFromReference(KImg_Bank).GetImage().SetSpriteAsync(tbpiggy_Bank.Get(BankID).pic, true);
        }

        private void SetTextByTime()
        {
            //this.GetFromReference()
            if (CanBuy)
            {
                if (BankEndTime - TimeHelper.ClientNowSeconds() >= 0)
                {
                    string str1 = tblanguage.Get("text_remain_time").current;
                    //have time
                    long deltaTime = BankEndTime - TimeHelper.ClientNowSeconds();
                    string str2 = UnityHelper.RichTextColor(
                        JiYuUIHelper.GeneralTimeFormat(new Unity.Mathematics.int4(1, 4, 1, 1), deltaTime), "F8F709");
                    this.GetFromReference(KText_Time).GetTextMeshPro().SetTMPText(str1 + str2);
                }
                else
                {
                    if (!HaveSendMessage)
                    {
                        HaveSendMessage = true;
                        WebMessageHandler.Instance.AddHandler(CMD.QUERYBANK, OnBankResponse);
                        NetWorkManager.Instance.SendMessage(CMD.QUERYBANK);
                    }
                }
            }
            else
            {
            }
        }

        private void RemoveTimer()
        {
            if (this.timerId != 0)
            {
                var timerMgr = TimerManager.Instance;
                timerMgr?.RemoveTimerId(ref this.timerId);
                this.timerId = 0;
            }
        }


        protected override void OnClose()
        {
            RemoveTimer();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
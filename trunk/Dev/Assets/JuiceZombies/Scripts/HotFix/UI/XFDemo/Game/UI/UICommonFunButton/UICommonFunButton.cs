//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommonFunButton)]
    internal sealed class UICommonFunButtonEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommonFunButton;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommonFunButton>();
        }
    }

    public partial class UICommonFunButton : UI, IAwake<int>
    {
        const float animTime = 0.15f;
        private int currentFucID;
        private CancellationTokenSource ccs = new CancellationTokenSource();

        public void Initialize(int id)
        {
            this.GetFromReference(KImg_RedDot).SetActive(false);
            var lang = ConfigManager.Instance.Tables.Tblanguage;
            currentFucID = id;
            var funcTable = ConfigManager.Instance.Tables.Tbtag_func.DataMap;
            //Log.Debug("Initialize");
            this.GetFromReference(KImg_Fun).GetImage().SetSprite(funcTable[id].icon, false);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Fuc), () => { OnBtnClick(); });

            // JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KBtn_Fuc), OnBtnClick);

            this.GetFromReference(KTxt_Name).GetTextMeshPro().SetTMPText(lang.Get(funcTable[id].name).current);

            if (id == 3401)
            {
                if (ResourcesSingleton.Instance.firstChargeInt == 0)
                {
                    this.SetActive(false);
                }
                else
                {
                    this.SetActive(true);
                }
            }

            if (id == 3405)
            {
                var gameBank = ResourcesSingleton.Instance.goldPig;
                Debug.Log("bank level :" + gameBank.PigLevel);
                Debug.Log(gameBank);
                Debug.Log("gameBank.Countdown" +
                          (gameBank.Countdown - (ResourcesSingleton.Instance.serverDeltaTime / 1000) + 1).ToString());
                //gameBank.Unlock
                if (ResourcesSingleton.Instance.goldPig.PigLevel == 0 ||
                    ResourcesSingleton.Instance.goldPig.Unlock == 0)
                {
                    this.SetActive(false);
                }
                else
                {
                    this.SetActive(true);
                    int bankID = 0;
                    foreach (var tbpb in ConfigManager.Instance.Tables.Tbpiggy_bank.DataList)
                    {
                        if (tbpb.type == ResourcesSingleton.Instance.goldPig.PigType &&
                            tbpb.sort == ResourcesSingleton.Instance.goldPig.PigLevel)
                        {
                            bankID = tbpb.id;
                            break;
                        }
                    }

                    if (ResourcesSingleton.Instance.goldPig.GoldBank >=
                        ConfigManager.Instance.Tables.Tbpiggy_bank.Get(bankID).full)
                    {
                        if (ResourcesSingleton.Instance.haveSetBankWeb == false)
                        {
                            ResourcesSingleton.Instance.haveSetBankWeb = true;
                            Set3405BankWeb(ResourcesSingleton.Instance.goldPig.Countdown -
                                (ResourcesSingleton.Instance.serverDeltaTime / 1000) + 1).Forget();
                        }
                    }
                    else
                    {
                        if (ResourcesSingleton.Instance.goldPig.GoldBank <
                            ConfigManager.Instance.Tables.Tbpiggy_bank.Get(bankID).base0)
                        {
                            this.SetActive(false);
                        }
                    }
                }
            }
        }

        private async UniTaskVoid Set3405BankWeb(long endTime)
        {
            await UniTask.Delay(1000 * (int)endTime);
            if (ResourcesSingleton.Instance.haveBuyBank)
            {
            }
            else
            {
                WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYBANK, On3405BankResponse);
                NetWorkManager.Instance.SendMessage(CMD.QUERYBANK);
            }
        }

        private void On3405BankResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYBANK, On3405BankResponse);
            var gameBank = new GoldPig();
            gameBank.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Debug.Log("e is empty");
                return;
            }

            Debug.Log("common fun btn game bank:" + gameBank);
            ResourcesSingleton.Instance.goldPig = gameBank;
            if (this != null)
            {
                if (gameBank.PigLevel == 0 || gameBank.Unlock == 0)
                {
                    this.SetActive(false);
                }
                else
                {
                    this.SetActive(true);
                }
            }

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Bank, out UI uis))
            {
                var ui = uis as UIPanel_Bank;
                if (gameBank.PigLevel == 0 || gameBank.Unlock == 0)
                {
                    ui.CloseBank();
                }
                else
                {
                    ui.ReSet();
                }
            }
        }
        //private async UniTask()

        private void OnBtnClick()
        {
            Log.Debug("OnBtnClick", Color.cyan);
            if (currentFucID == 3606)
            {
                OnBagButtonClick();
            }

            #region 黄金国新增

            if (currentFucID == 3601)
            {
                //每日签到
                UIHelper.Create(UIType.UIPanel_Sign);
            }

            if (currentFucID == 5105)
            {
                //成就
                UIHelper.Create(UIType.UIPanel_Achieve);
            }

            if (currentFucID == 3401)
            {
                //first 
                var ui = UIHelper.Create(UIType.UIPanel_First_Charge);
                ui.SetParent(this, false);
            }

            if (currentFucID == 3405)
            {
                //bank
                var ui = UIHelper.Create(UIType.UIPanel_Bank);
                ui.SetParent(this, false);
            }

            #endregion
        }

        private void OnBagButtonClick()
        {
            NetWorkManager.Instance.SendMessage(CMD.OPENBAG);
        }

        public void OnDestroyUI()
        {
        }

        protected override void OnClose()
        {
            ccs.Cancel();
            base.OnClose();
        }
    }
}
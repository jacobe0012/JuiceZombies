//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_IconBtnItem)]
    internal sealed class UISubPanel_IconBtnItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_IconBtnItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_IconBtnItem>();
        }
    }

    public partial class UISubPanel_IconBtnItem : UI, IAwake<int>
    {
        public int id;

        public void Initialize(int args)
        {
            id = args;
            InitNode();
        }

        void InitNode()
        {
            var KImg_IconBtn = GetFromReference(UISubPanel_IconBtnItem.KImg_IconBtn);
            var KText_IconBtn = GetFromReference(UISubPanel_IconBtnItem.KText_IconBtn);

            this.GetFromReference(KTip).SetActive(false);

            if (id == 3405)
            {
                module3405Set();
            }
        }

        #region tag id 3405; huangjinguo add

        private void module3405Set()
        {
            var gameBank = ResourcesSingletonOld.Instance.goldPig;
            if (ResourcesSingletonOld.Instance.goldPig.PigLevel == 0 || ResourcesSingletonOld.Instance.goldPig.Unlock == 0)
            {
                this.SetActive(false);
            }
            else
            {
                this.SetActive(true);
                int bankID = 0;
                foreach (var tbpb in ConfigManager.Instance.Tables.Tbpiggy_bank.DataList)
                {
                    if (tbpb.type == ResourcesSingletonOld.Instance.goldPig.PigType &&
                        tbpb.sort == ResourcesSingletonOld.Instance.goldPig.PigLevel)
                    {
                        bankID = tbpb.id;
                        break;
                    }
                }

                if (ResourcesSingletonOld.Instance.goldPig.GoldBank >=
                    ConfigManager.Instance.Tables.Tbpiggy_bank.Get(bankID).full)
                {
                    if (ResourcesSingletonOld.Instance.haveSetBankWeb == false)
                    {
                        ResourcesSingletonOld.Instance.haveSetBankWeb = true;
                        Set3405BankWeb(ResourcesSingletonOld.Instance.goldPig.Countdown -
                            (ResourcesSingletonOld.Instance.serverDeltaTime / 1000) + 1).Forget();
                    }
                }
                else
                {
                    if (ResourcesSingletonOld.Instance.goldPig.GoldBank <
                        ConfigManager.Instance.Tables.Tbpiggy_bank.Get(bankID).base0)
                    {
                        this.SetActive(false);
                    }
                }
            }
        }

        private async UniTaskVoid Set3405BankWeb(long endTime)
        {
            await UniTask.Delay(1000 * (int)endTime);
            if (ResourcesSingletonOld.Instance.haveBuyBank)
            {
            }
            else
            {
                WebMessageHandlerOld.Instance.AddHandler(CMDOld.QUERYBANK, On3405BankResponse);
                NetWorkManager.Instance.SendMessage(CMDOld.QUERYBANK);
            }
        }

        private void On3405BankResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.QUERYBANK, On3405BankResponse);
            var gameBank = new GoldPig();
            gameBank.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Debug.Log("e is empty");
                return;
            }

            //Debug.Log("common fun btn game bank:" + gameBank);
            ResourcesSingletonOld.Instance.goldPig = gameBank;
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

            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Bank, out UI uis))
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

        #endregion

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}
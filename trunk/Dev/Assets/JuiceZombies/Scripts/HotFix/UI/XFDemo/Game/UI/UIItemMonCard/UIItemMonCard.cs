//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2025-08-23 18:46:52
//---------------------------------------------------------------------

using cfg.config;
using Codice.Client.Common;
using HotFix_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIItemMonCard)]
    internal sealed class UIItemMonCardEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIItemMonCard;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIItemMonCard>();
        }
    }

    public partial class UIItemMonCard : UI, IAwake<int>
    {
        const string monthColorBg = "FF7DF7";
        const string monthColorBorder = "FDB2FD";
        const string noAdColorBorder = "A7CAF2";
        const string noAdColorBg = "4D9CF4";
        private TbmonthCard tbmonthCard;
        private Tblanguage tblanguage;
        private TbItems tbItems;

        enum MonCardState
        {
            //未购买=0，剩余30天以上=1，剩余5-30天=2，剩余5天以下=3
            NotBuy = 0,
            BuyLeftOver30 = 1,
            BuyLeftOver5 = 2,
            BuyLeftOnly5
        }

        public void Initialize(int type)
        {
            InitNode();
            InitView(type);
        }

        private void InitView(int type)
        {
            var KBg = GetFromReference(UIItemMonCard.KBg);
            var KBorder = GetFromReference(UIItemMonCard.KBorder);
            var KBgTime = GetFromReference(UIItemMonCard.KBgTime);
            var KTextLeftTime = GetFromReference(UIItemMonCard.KTextLeftTime);
            var KText_Title = GetFromReference(UIItemMonCard.KText_Title);

            var KImage_Card = GetFromReference(UIItemMonCard.KImage_Card);
            var KGradient_Bottom = GetFromReference(UIItemMonCard.KGradient_Bottom);
            var KTextDiscord = GetFromReference(UIItemMonCard.KTextDiscord);

            var KBuyBtn = GetFromReference(UIItemMonCard.KBuyBtn);
            var KTextBtn = GetFromReference(UIItemMonCard.KTextBtn);
            var KTextGet = GetFromReference(UIItemMonCard.KTextGet);
            tbmonthCard = ConfigManager.Instance.Tables.TbmonthCard;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbItems = ConfigManager.Instance.Tables.TbItems;
            switch (type)
            {
                //type 1 月卡 2免广告卡
                case 1:
                    KBg.GetImage().SetColor(monthColorBg);
                    KGradient_Bottom.GetImage().SetColor(monthColorBg);
                    KBorder.GetImage().SetColor(monthColorBorder);
                    KText_Title.GetTextMeshPro().SetTMPText(tblanguage.Get("title_moncard").current);
                    SetContainerItem(type);
                    //WebMsgHandler.Instance.AddHandler(CMD.Shop.C2S_QUERYSHOP, OnRequreMonthTime);
                    NetWorkManager.Instance.SendMsg(new S2C_ShopData());
                    Log.Debug("SendMsg", Color.cyan);
                    break;
                case 2:
                    KBg.GetImage().SetColor(noAdColorBg);
                    KGradient_Bottom.GetImage().SetColor(noAdColorBg);
                    KBorder.GetImage().SetColor(noAdColorBorder);
                    KText_Title.GetTextMeshPro().SetTMPText(tblanguage.Get("title_noad").current);
                    break;
            }
        }

        private async Task SetContainerItem(int type)
        {
            var desc = tbmonthCard.Get(type).desc;
            var KItemContainer = GetFromReference(UIItemMonCard.KItemContainer);
            var list = KItemContainer.GetList();

            var KGroup_Items = GetFromReference(UIItemMonCard.KGroup_Items);
            var iconList = KGroup_Items.GetList();
            for (int i = 0; i < desc.Count; i++)
            {
                int index = i;
                var ui = await list.CreateWithUITypeAsync<int>(UIType.UIItemMonCardIDes, index, false);
                var KText_Description = ui.GetFromReference(UIItemMonCardIDes.KText_Description);
                KText_Description.GetTextMeshPro().SetTMPText(desc[i]);
                int iconId = (int)tbmonthCard.Get(type).descpara[i].x;
                Log.Debug("iconId:" + iconId);
                var iconUI = iconList.Create(iconList.Get().GetChild(i).gameObject);
                iconUI.GetImage().SetSpriteAsync(tbItems.Get(iconId).icon, false);
            }

            list.Sort((a, b) =>
            {
                var uiA = a as UIItemMonCardIDes;
                var uiB = b as UIItemMonCardIDes;
                return uiA.index.CompareTo(uiB.index);
            });
        }

        [MsgHandler(typeof(S2C_ShopData))]
        private void OnRequreMonthTime(S2C_ShopData data)
        {
            ResourcesSingletonOld.Instance.monCardTime = data.buyedMonthCardms;
            //var receivedMessage = MessagePackSerializer.Deserialize<MyMessage>(e.data, options);

            Log.Debug($"OnRequreMonthTime:{data.buyedMonthCardms}");
            var time = ResourcesSingletonOld.Instance.monCardTime;
            if (time <= 0)
            {
                SetStateOfBuy(time, MonCardState.NotBuy);
                return;
            }
            else
            {
                if (time > 30)
                {
                    SetStateOfBuy(time, MonCardState.BuyLeftOver30);
                }
                else if (time <= 30 && time > 5)
                {
                    SetStateOfBuy(time, MonCardState.BuyLeftOver5);
                }
                else if (time <= 5 && time > 0)
                {
                    SetStateOfBuy(time, MonCardState.BuyLeftOnly5);
                }
            }
        }

        private void SetStateOfBuy(long time, MonCardState notBuy = MonCardState.NotBuy)
        {
            InitBtnState();
            var KTextLeftTime = GetFromReference(UIItemMonCard.KTextLeftTime);
            var KBgTime = KTextLeftTime.GetFromReference(UIItemMonCard.KBgTime);
            var KBuyBtn = GetFromReference(UIItemMonCard.KBuyBtn);
            var KTextBtn = GetFromReference(UIItemMonCard.KTextBtn);
            KBuyBtn.SetActive(true);
            switch (notBuy)
            {
                case MonCardState.NotBuy:
                    //未购买
                    KTextLeftTime.GetTextMeshPro()
                        .SetTMPText(string.Format(tblanguage.Get("month_card_time").current, 30));
                    KBgTime.GetImage().SetColor("FFF046");
                    KBuyBtn.GetFromReference(UICommonBtn.KYellow).SetActive(true);
                    KTextBtn.GetTextMeshPro().SetTMPText(tblanguage.Get("buy_now").current);
                    break;
                case MonCardState.BuyLeftOver30:
                    KTextLeftTime.GetTextMeshPro()
                        .SetTMPText(string.Format(tblanguage.Get("month_card_left_time").current, time));
                    KBgTime.GetImage().SetColor("88FF46");
                    KBuyBtn.GetFromReference(UICommonBtn.KGreen).SetActive(true);
                    KTextBtn.GetTextMeshPro().SetTMPText(tblanguage.Get("renew_now").current);
                    break;
                case MonCardState.BuyLeftOver5:
                    KTextLeftTime.GetTextMeshPro()
                        .SetTMPText(string.Format(tblanguage.Get("month_card_left_time").current, time));
                    KBgTime.GetImage().SetColor("FFF046");
                    KBuyBtn.GetFromReference(UICommonBtn.KYellow).SetActive(true);
                    KTextBtn.GetTextMeshPro().SetTMPText(tblanguage.Get("renew_now").current);
                    break;
                case MonCardState.BuyLeftOnly5:
                    KTextLeftTime.GetTextMeshPro()
                        .SetTMPText(string.Format(tblanguage.Get("month_card_left_time").current, time));
                    KBgTime.GetImage().SetColor("FF5946");
                    KBuyBtn.GetFromReference(UICommonBtn.KRed).SetActive(true);
                    KTextBtn.GetTextMeshPro().SetTMPText(tblanguage.Get("renew_now").current);
                    break;
            }
        }

        private void InitBtnState()
        {
            var KBuyBtn = GetFromReference(UIItemMonCard.KBuyBtn);
            KBuyBtn.GetFromReference(UICommonBtn.KYellow).SetActive(false);
            KBuyBtn.GetFromReference(UICommonBtn.KGreen).SetActive(false);
            KBuyBtn.GetFromReference(UICommonBtn.KBlue).SetActive(false);
            KBuyBtn.GetFromReference(UICommonBtn.KPink).SetActive(false);
            KBuyBtn.GetFromReference(UICommonBtn.KRed).SetActive(false);
        }

        void InitNode()
        {
            var KBg = GetFromReference(UIItemMonCard.KBg);
            var KBorder = GetFromReference(UIItemMonCard.KBorder);
            var KBgTime = GetFromReference(UIItemMonCard.KBgTime);
            var KTextLeftTime = GetFromReference(UIItemMonCard.KTextLeftTime);
            var KText_Title = GetFromReference(UIItemMonCard.KText_Title);
            var KItemContainer = GetFromReference(UIItemMonCard.KItemContainer);
            var KImage_Card = GetFromReference(UIItemMonCard.KImage_Card);
            var KGradient_Bottom = GetFromReference(UIItemMonCard.KGradient_Bottom);
            var KTextDiscord = GetFromReference(UIItemMonCard.KTextDiscord);
            var KGroup_Items = GetFromReference(UIItemMonCard.KGroup_Items);
            var KBuyBtn = GetFromReference(UIItemMonCard.KBuyBtn);
            var KTextBtn = GetFromReference(UIItemMonCard.KTextBtn);
            var KTextGet = GetFromReference(UIItemMonCard.KTextGet);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}
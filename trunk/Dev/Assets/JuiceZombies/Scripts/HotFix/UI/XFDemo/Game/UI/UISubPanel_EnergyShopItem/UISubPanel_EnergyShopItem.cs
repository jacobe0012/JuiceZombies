//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using SuperScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_EnergyShopItem)]
    internal sealed class UISubPanel_EnergyShopItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_EnergyShopItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_EnergyShopItem>();
        }
    }

    public partial class UISubPanel_EnergyShopItem : UI, IAwake<int>
    {
        private int uId;
        public int sort;
        private Tbuser_variable tbUser_variable;
        private Tbenergy_shop_goods tbEnergy_shop_goods;
        private Tblanguage tblanguage;
        private TbitemOld tbItem;
        private UI btn_Buy;
        private UI img_Saled;
        private UI txt_Saled;

        private UI reward_Pos;

        //private UI img_Item;
        //private UI txt_ItemCount;
        private UI txt_Name;
        private UI img_Lock;
        private CancellationTokenSource cts;

        void InitNode()
        {
            btn_Buy = GetFromReference(UISubPanel_EnergyShopItem.KBtn_Buy);
            img_Saled = GetFromReference(UISubPanel_EnergyShopItem.KImg_Saled);
            txt_Saled = GetFromReference(UISubPanel_EnergyShopItem.KTxt_Saled);
            reward_Pos = GetFromReference(UISubPanel_EnergyShopItem.KReward_Pos);
            //img_Item = GetFromReference(UISubPanel_EnergyShopItem.KImg_Item);
            //txt_ItemCount = GetFromReference(UISubPanel_EnergyShopItem.KTxt_ItemCount);
            txt_Name = GetFromReference(UISubPanel_EnergyShopItem.KTxt_Name);
            img_Lock = GetFromReference(UISubPanel_EnergyShopItem.KImg_Lock);
        }


        public void Initialize(int id)
        {

            cts = new CancellationTokenSource();
            InitJsonConfig();
            sort = tbEnergy_shop_goods.GetOrDefault(id).sort;
            uId = id;
            InitNode();
            //InitItemDisplay(id);
            InitBtnDisplay(id);
            txt_Saled.GetTextMeshPro().SetTMPText(tblanguage.GetOrDefault("active_sell_out_text").current);
            DisplayLock(true);
            DisplaySaled(false);
            SetReward();
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn_Buy,
                () => OnBtnBuyOnClick());
            InitEffect();
        }

        private void InitEffect()
        {
            //UniTask.Delay(500,cancellationToken:cts.Token);
          
        }

        private async UniTaskVoid SetReward()
        {
            var list = reward_Pos.GetList();
            list.Clear();
            var reward = tbEnergy_shop_goods.GetOrDefault(uId).reward[0];
            var ui = await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, reward, false) as UICommon_RewardItem;
            ui.SetActive(true);
            SetRewardInfo(ui);
            UnicornUIHelper.SetRewardOnClick(reward, ui);
            ui.GetRectTransform().SetAnchoredPositionX(0f);
            ui.GetRectTransform().SetAnchoredPositionY(0f);
        }

        private void SetRewardInfo(UICommon_RewardItem ui)
        {
            var itemStr = tbEnergy_shop_goods.GetOrDefault(uId).reward;
            var itemId = (int)itemStr[0].x;
            var count = (int)itemStr[0].z;
            if (itemId != 5)
            {
                var icon = tbUser_variable.GetOrDefault(itemId).icon;
                var name = tbUser_variable.GetOrDefault(itemId).name;
                name = tblanguage.GetOrDefault(name).current;
                ui.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage().SetSpriteAsync(icon, false);
                ui.GetFromReference(UICommon_RewardItem.KText_ItemCount).GetTextMeshPro().SetTMPText(count.ToString());
                txt_Name.GetTextMeshPro().SetTMPText(name.ToString());
            }
            else
            {
                var icon = tbItem.GetOrDefault(itemId).icon;
                var name = tbItem.GetOrDefault(itemId).name;
                name = tblanguage.GetOrDefault(name).current;
                ui.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage().SetSpriteAsync(icon, false);
                ui.GetFromReference(UICommon_RewardItem.KText_ItemCount).GetTextMeshPro().SetTMPText(count.ToString());
                txt_Name.GetTextMeshPro().SetTMPText(name.ToString());
            }
        }

        private async UniTaskVoid OnBtnBuyOnClick()
        {
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Activity_EnergyShop, out UI ui))
            {
                var uiEnergyShop = ui as UIPanel_Activity_EnergyShop;
                bool isCanBuy = true;
                //if (!uiEnergyShop.isCanBuy)
                if (!isCanBuy)
                {
                    Log.Debug("!uiEnergyShop.isCanBuy");
                    var str = tblanguage.Get("energy_warning_text").current;
                    var itemId = (int)tbEnergy_shop_goods.GetOrDefault(uId).cost[0].y;
                    str = str.Replace("{0}", tblanguage.GetOrDefault(tbItem.GetOrDefault(itemId).name).current);
                    UnicornUIHelper.ClearCommonResource();
                    await UIHelper.CreateAsync(UIType.UICommon_Resource, str);
                }
                else
                {
                    if (JudgeItemOrNot(tbEnergy_shop_goods.GetOrDefault(uId).cost, out int itemId))
                    {
                        if (ResourcesSingletonOld.Instance.items.ContainsKey(itemId))
                        {
                            var playerHas = ResourcesSingletonOld.Instance.items[itemId];
                            var needCount = tbEnergy_shop_goods.GetOrDefault(uId).cost[0].z;
                            if (needCount > playerHas)
                            {
                                Log.Debug("needCount > playerHas");
                                //TODO:
                                UnicornUIHelper.ClearCommonResource();
                                await UIHelper.CreateAsync(UIType.UICommon_Resource,
                                    tblanguage.Get("common_lack_6_title").current);
                            }
                            else
                            {
                                var uis = await UIHelper.CreateAsync(UIType.UICommon_Sec_Confirm,
                                    tbEnergy_shop_goods.GetOrDefault(uId).cost[0],
                                    tbEnergy_shop_goods.GetOrDefault(uId).reward[0]);
                                //textHelp = textHelp.Replace("{1}", rewardHelp);
                                var secBuyBtn = uis.GetFromReference(UICommon_Sec_Confirm.KBtn_Buy);
                                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(secBuyBtn, () =>
                                {
                                    IntValue intValue = new IntValue();
                                    intValue.Value = uId;
                                    //WebMessageHandlerOld.Instance.AddHandler(CMDOld.SHOP1201, On1201BuyResponse);
                                    WebMessageHandlerOld.Instance.AddHandler(8, 6, OnBuyResponse);
                                    NetWorkManager.Instance.SendMessage(8, 6, intValue);
                                });
                            }
                        }
                        else
                        {
                            Log.Debug("isCanBuy----else");
                            //TODO:
                            UnicornUIHelper.ClearCommonResource();
                            await UIHelper.CreateAsync(UIType.UICommon_Resource,
                                tblanguage.Get("common_lack_6_title").current);
                        }
                    }
                    else
                    {
                    }
                }
            }
        }

        private void OnBuyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(8, 6, OnBuyResponse);

            ActivityRes res = new ActivityRes();

            res.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            OnSecBtnClick().Forget();
        }

        private async UniTaskVoid OnSecBtnClick()
        {
            var rewards = tbEnergy_shop_goods.GetOrDefault(uId).reward;
            var cost = tbEnergy_shop_goods.GetOrDefault(uId).cost[0];
            UnicornUIHelper.AddReward(rewards);
            UnicornUIHelper.TryReduceReward(cost);

            await UIHelper.CreateAsync(UIType.UICommon_Reward, rewards);

            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Activity_EnergyShop, out UI ui))
            {
                var uiEnergyShop = ui as UIPanel_Activity_EnergyShop;
                uiEnergyShop.UpdateGoldNum();
            }

            if (UnicornUIHelper.TryGetUI(UIType.UICommon_Sec_Confirm, out UI ui1))
            {
                var uiCommon_Sec_Confirm = ui1 as UICommon_Sec_Confirm;
                uiCommon_Sec_Confirm.Dispose();
            }
        }

        private void InitBtnDisplay(int id)
        {
            var itemStr = tbEnergy_shop_goods.Get(id).cost;
            var count = (int)itemStr[0].z;
            string icon = "";
            if (JudgeItemOrNot(itemStr, out int itemId))
            {
                icon = tbItem.Get(itemId).icon;
            }
            else
            {
                icon = tbUser_variable.GetOrDefault(itemId).icon;
            }
            GetFromReference(KText_Mid).GetTextMeshPro().SetTMPText(UnicornUIHelper.GetRewardTextIconName(icon) + count.ToString());
        }

        private bool JudgeItemOrNot(List<Vector3> itemStr, out int itemId)
        {
            itemId = (int)itemStr[0].x;
            if (itemId != 5)
            {
                return false;
            }

            itemId = (int)itemStr[0].y;
            return true;
        }

        private void InitItemDisplay(int id)
        {
            //var itemStr = tbEnergy_shop_goods.GetOrDefault(id).reward;
            //var itemId = (int)itemStr[0].x;
            //var count= (int)itemStr[0].z;
            //if (itemId!=5)
            //{
            //    var icon = tbUser_variable.GetOrDefault(itemId).icon;
            //    var name = tbUser_variable.GetOrDefault(itemId).name;
            //    name = tblanguage.GetOrDefault(name).current;
            //    img_Item.GetImage().SetSpriteAsync(icon, false);
            //    txt_ItemCount.GetTextMeshPro().SetTMPText(count.ToString());
            //    txt_Name.GetTextMeshPro().SetTMPText(name.ToString());

            //}
            //else
            //{
            //    var icon = tbItem.GetOrDefault(itemId).icon;
            //    var name = tbItem.GetOrDefault(itemId).name;
            //    name = tblanguage.GetOrDefault(name).current;
            //    img_Item.GetImage().SetSpriteAsync(icon, false);
            //    txt_ItemCount.GetTextMeshPro().SetTMPText(count.ToString());
            //    txt_Name.GetTextMeshPro().SetTMPText(name.ToString());
            //}
        }

        public void DisplayLock(bool isDisplay)
        {
            img_Lock.SetActive(isDisplay);
        }

        public void DisplaySaled(bool isDisplay)
        {
            img_Saled.SetActive(isDisplay);
        }

        private void InitJsonConfig()
        {
            tbUser_variable = ConfigManager.Instance.Tables.Tbuser_variable;
            tbEnergy_shop_goods = ConfigManager.Instance.Tables.Tbenergy_shop_goods;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            //tbConstant = ConfigManager.Instance.Tables.Tbconstant;
            tbItem = ConfigManager.Instance.Tables.TbitemOld;
        }

        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
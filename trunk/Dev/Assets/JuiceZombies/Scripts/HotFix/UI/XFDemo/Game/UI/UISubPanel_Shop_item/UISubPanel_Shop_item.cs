//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using cfg.config;
using Common;
using DG.Tweening;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Newtonsoft.Json;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_item)]
    internal sealed class UISubPanel_Shop_itemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_item;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_item>();
        }
    }

    public partial class UISubPanel_Shop_item : UI, IAwake<int3>
    {
        private int tagID = 0;
        public int ID = 0;
        private UI SecConfirmUI = null;
        private Tbrecharge tbrecharge;
        private Tblanguage tblanguage;
        private Tbchapter tbchapter;
        private Tbuser_variable tbuser_Variable;
        private Tbspecials tbspecials;
        private Tbgift tbgift;
        private Tbprice tbprice;
        private Tbshop_daily tbshop_Daily;
        private Tbconstant tbconstant;
        private int boxString;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public void Initialize(int3 a)
        {
            DataInit();

            this.GetFromReference(KBtn_CloseTip).GetXButton().OnClick?.Add(CloseAllTip);

            tagID = a.x;
            ID = a.y;
            SecConfirmUI = null;
            this.GetFromReference(KImg_Green).SetActive(false);
            var btn = this.GetFromReference(KBtn_Common);
            this.GetFromReference(KText_RemainingTimes).SetActive(false);
            this.GetFromReference(KImg_RedPoint).SetActive(false);
            this.GetFromReference(KStateText).SetActive(false);
            string shopStr = default;


            if (a.z == 0)
            {
                UnicornTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UISubPanel_Shop_item.KBg), 0, 50, cts.Token,
                    0.35f,
                    false,
                    false);
                UnicornTweenHelper.ChangeSoftness(this, 300, 0.2f, cancellationToken: cts.Token);
            }


            //return;
            //每日特供
            if (tagID == 1201)
            {
                var redDotStr = NodeNames.GetTagFuncRedDotName(tagID);
                RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
                GetFromReference(KImg_RedPoint)?.SetActive(false);


                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn, async () =>
                {
                    var dbList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList;

                    int index = 0;
                    foreach (var db in dbList)
                    {
                        if (tbshop_Daily.Get(db.Sign).pos == ID)
                        {
                            break;
                        }

                        index++;
                    }

                    var a = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList[index];
                    int sign = a.Sign;
                    int pos = tbshop_Daily.Get(sign).pos;

                    if (pos == 1)
                    {
                        IntValue intValue = new IntValue();
                        intValue.Value = sign;
                        WebMessageHandlerOld.Instance.AddHandler(CMDOld.SHOP1201, On1201BuyResponse);
                        NetWorkManager.Instance.SendMessage(CMDOld.SHOP1201, intValue);
                    }
                    else
                    {
                        int needCostInt = (int)tbshop_Daily[sign].cost[0][2];
                        long playerHave = 0;
                        if (tbshop_Daily[sign].cost[0][0] == 3)
                        {
                            playerHave = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.UsBill;
                        }

                        if (tbshop_Daily[sign].cost[0][0] == 2)
                        {
                            playerHave = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin;
                        }

                        Debug.Log("needCostInt" + needCostInt);
                        Debug.Log("playerHave" + playerHave);

                        if (needCostInt > playerHave)
                        {
                            Vector3 v3 = tbshop_Daily[sign].cost[0];
                            v3.z = v3.z - playerHave;
                            var uiTip = UIHelper.Create<Vector3>(UIType.UICommon_ResourceNotEnough, v3);
                            UnicornUIHelper.SetResourceNotEnoughTip(uiTip, btn);
                        }
                        else
                        {
                            var uis = await UIHelper.CreateAsync(UIType.UICommon_Sec_Confirm,
                                tbshop_Daily[sign].cost[0],
                                tbshop_Daily[sign].reward[0]);

                            SecConfirmUI = uis;

                            var secBuyBtn = uis.GetFromReference(UICommon_Sec_Confirm.KBtn_Buy);
                            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(secBuyBtn, () =>
                            {
                                IntValue intValue = new IntValue();
                                intValue.Value = sign;
                                WebMessageHandlerOld.Instance.AddHandler(CMDOld.SHOP1201, On1201BuyResponse);
                                NetWorkManager.Instance.SendMessage(CMDOld.SHOP1201, intValue);
                            });

                            // uis.GetFromReference(UICommon_Sec_Confirm.KText_Return).GetTextMeshPro()
                            //     .SetTMPText(tblanguage.Get("common_state_cancel").current);
                            // uis.GetFromReference(UICommon_Sec_Confirm.KText_Buy).GetTextMeshPro()
                            //     .SetTMPText(tblanguage.Get("common_state_buy").current);
                            // string textHelp = tblanguage.Get("text_purchase_confirm").current;
                            // string rewardHelp = "";
                            // foreach (var r in tbshop_Daily[sign].reward)
                            // {
                            //     rewardHelp += UnicornUIHelper.GetRewardName(r);
                            // }
                            //
                            // if (tbshop_Daily[sign].cost[0][0] == 3)
                            // {
                            //     textHelp = textHelp.Replace("{0}",
                            //         "<sprite=1>" + UnityHelper.RichTextColor(tbshop_Daily[sign].cost[0][2].ToString(),
                            //             "475569"));
                            // }
                            //
                            // if (tbshop_Daily[sign].cost[0][0] == 2)
                            // {
                            //     textHelp = textHelp.Replace("{0}",
                            //         "<sprite=0>" + UnityHelper.RichTextColor(tbshop_Daily[sign].cost[0][2].ToString(),
                            //             "475569"));
                            // }
                            //
                            // textHelp = textHelp.Replace("{1}", rewardHelp);
                            // uis.GetFromReference(UICommon_Sec_Confirm.KText).GetTextMeshPro().SetTMPText(textHelp);
                            //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(uis.GetFromReference(UICommon_Sec_Confirm.KBtn_Return), () => OnConfirmClose());
                            //uis.GetFromReference(UICommon_Sec_Confirm.KBg_Blur)? .GetXButton().OnClick.Add(() => { OnConfirmClose(); });
                        }
                    }
                });
            }

            ///特惠
            if (tagID == 1202)
            {
                if (tbspecials.Get(ID).price > 0)
                {
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
                    {
                        UnicornUIHelper.SendBuyMessage(tagID, ID);
                        OnBuyBtnClick(btn);
                    });
                }
                else
                {
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
                    {
                        Log.Debug("DFDFDSFDFDSF", Color.cyan);
                        IntValue intValue = new IntValue();
                        intValue.Value = ID;
                        WebMessageHandlerOld.Instance.AddHandler(CMDOld.SHOP1202, On1202BuyResponse);
                        NetWorkManager.Instance.SendMessage(CMDOld.SHOP1202, intValue);
                    });
                }
            }

            if (tagID == 1302)
            {
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
                {
                    IntValue intValue = new IntValue();
                    intValue.Value = ID;
                    WebMessageHandlerOld.Instance.AddHandler(11, 5, On1302BuyResponse);
                    NetWorkManager.Instance.SendMessage(11, 5, intValue);
                });
            }

            if (tagID == 1401)
            {
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
                {
                    UnicornUIHelper.SendBuyMessage(tagID, ID);
                    OnBuyBtnClick(btn);
                });
                //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () => { UnicornUIHelper.SendBuyMessage(tagID, ID); });
            }

            if (tagID == 1402)
            {
                // UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
                // {
                //     UnicornUIHelper.SendBuyMessage(tagID, ID);
                //     OnBuyBtnClick(btn);
                // });
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
                {
                    IntValue intValue = new IntValue();
                    intValue.Value = ID;
                    WebMessageHandlerOld.Instance.AddHandler(CMDOld.SHOP1402, On1402BuyResponse);
                    NetWorkManager.Instance.SendMessage(CMDOld.SHOP1402, intValue);
                });
            }
        }

        private void OnBuyBtnClick(UI btn)
        {
            Log.Debug($"id:{ID}", Color.cyan);
            JiYuEventManager.Instance.UnregisterEvent("OnShopResponse");
            JiYuEventManager.Instance.RegisterEvent("OnShopResponse", (a) => OnShopResponse(ID, a));
        }

        private void OnShopResponse(int id, string a)

        {
            Log.Debug($"id1111:{id}", Color.cyan);
            var shopNum = UnicornUIHelper.GetShopNum(a);

            switch (shopNum)
            {
                case "A01":
                    var cList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1401].ChargeInfoList;
                    ChargeInfo chargeInfo = new ChargeInfo();
                    int i = 0;
                    foreach (var c in cList)
                    {
                        if (c.Id == id)
                        {
                            chargeInfo = c;
                            break;
                        }

                        i++;
                    }

                    var rc = tbrecharge.Get(id);
                    ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1401].ChargeInfoList[i].ChargeSum += 1;
                    this.GetFromReference(KTitleTextTop)?.GetTextMeshPro().SetTMPText(tblanguage
                        .Get("recharge_extra_text")
                        .current.Replace("{0}", rc.valueExtra.ToString()));
                    ResourcesSingletonOld.Instance.UpdateResourceUI();
                    break;
                case "B02":
                    var gsList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1202].GameSpecialsList;
                    GameSpecials gameSpecials = new GameSpecials();
                    i = 0;
                    foreach (var gs in gsList)
                    {
                        if (gs.SpecialId == id)
                        {
                            gameSpecials = gs;
                            break;
                        }

                        i++;
                    }

                    var sp = tbspecials.Get(id);
                    if (sp.freeRule.Count == 0)
                    {
                        ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1202].GameSpecialsList[i].BuyCount = 0;
                    }
                    else
                    {
                        var redDotStr = NodeNames.GetTagFuncRedDotName(tagID);
                        RedDotManager.Instance.SetRedPointCnt(redDotStr + sp.type.ToString(), 0);
                        GetFromReference(KImg_RedDot)?.SetActive(false);

                        ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1202].GameSpecialsList[i].FreeCount = 0;
                    }

                    this.GetFromReference(KImg_Green)?.SetActive(false);
                    this.GetFromReference(KBtn_Common).SetActive(false);
                    this.GetFromReference(KStateText).SetActive(true);
                    this.GetFromReference(KStateText).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("common_state_purchased").current);
                    ResourcesSingletonOld.Instance.UpdateResourceUI();
                    break;
                case "B01":
                    var thisGift = tbgift.Get(id);

                    var giList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1302].GiftInfoList;
                    GiftInfo giftInfo = new GiftInfo();

                    int index = 0;
                    foreach (var gi in giList)
                    {
                        if (gi.GiftId == id)
                        {
                            giftInfo = gi;
                            break;
                        }

                        index++;
                    }
                    //Log.Debug($"times000:{ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1302].GiftInfoList[index].Times}",Color.cyan);
                    //Log.Debug($"freetimes000:{giftInfo.FreeTimes}",Color.cyan);

                    var itemNodeStr = NodeNames.GetTagFuncRedDotName(1302) + '|' + thisGift.id.ToString();
                    if (giftInfo.FreeTimes > 0)
                    {
                        ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1302].GiftInfoList[index].FreeTimes -= 1;
                    }

                    else
                    {
                        ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1302].GiftInfoList[index].Times -= 1;
                    }

                    //Log.Debug($"times111:{ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1302].GiftInfoList[index].Times}", Color.cyan);
                    //Log.Debug($"freetimes111:{giftInfo.FreeTimes}",Color.cyan);

                    if (thisGift.freeRule.Count == 0)
                    {
                        RedDotManager.Instance.SetRedPointCnt(itemNodeStr, 0);
                        this.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(false);

                        if (giftInfo.Times > 0)
                        {
                            this.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).SetActive(true);
                            string giftTimesStr = tblanguage.Get("gift_chances_left_text").current;
                            var timesStr = giftTimesStr.Replace("{0}", giftInfo.Times.ToString());
                            this.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).GetTextMeshPro()
                                .SetTMPText(timesStr);
                            //can buy
                            this.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
                            this.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                            this.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro().SetTMPText(
                                tbprice.Get(thisGift.price).rmb.ToString() +
                                tblanguage.Get("common_coin_unit").current);
                        }
                        else
                        {
                            //can not buy

                            this.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
                            this.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
                            this.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
                                .SetTMPText(tblanguage.Get("common_state_purchased").current);
                            this.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).SetActive(false);
                        }
                    }
                    else
                    {
                        this.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(true);
                        this.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_green", false);
                        this.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
                        this.GetFromReference(UISubPanel_Shop_item.KItemTitleTopBg).SetActive(false);
                        if (giftInfo.FreeTimes > 0)
                        {
                            RedDotManager.Instance.SetRedPointCnt(itemNodeStr, 1);
                            this.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);

                            //RedPointMgr.instance.SetState(ShopRoot, "module1302" + thisGift.id.ToString(),
                            //    RedPointState.Show);
                            this.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).SetActive(true);
                            string giftTimesStr = tblanguage.Get("gift_chances_left_text").current;
                            var timesStr = giftTimesStr.Replace("{0}", giftInfo.FreeTimes.ToString());
                            this.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).GetTextMeshPro()
                                .SetTMPText(timesStr);
                            //can buy
                            if (thisGift.freeRule[0][0] - giftInfo.FreeTimes <
                                thisGift.freeRule[0][0] - thisGift.freeRule[0][1])
                            {
                                //free
                                this.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);

                                this.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                                //itemUI.GetFromReference(UISubPanel_Shop_item.KImg_Left).GetImage().SetSprite("Advert", false);
                                this.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                                    .SetTMPText(tblanguage.Get("common_free_text").current);
                            }
                            else
                            {
                                //advert
                                this.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);

                                this.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);

                                this.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                                    .SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                                                tblanguage.Get("common_free_text").current);
                            }
                        }
                        else
                        {
                            RedDotManager.Instance.SetRedPointCnt(itemNodeStr, 0);
                            this.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(false);
                            //RedPointMgr.instance.SetState("ShopRoot", "module1302" + thisGift.id.ToString(),
                            //   RedPointState.Hide);
                            //can not buy
                            this.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
                            this.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
                            this.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
                                .SetTMPText(tblanguage.Get("common_state_purchased").current);
                            this.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).SetActive(false);
                        }
                    }

                    ResourcesSingletonOld.Instance.UpdateResourceUI();
                    break;
            }
        }


        public void CloseAllTip()
        {
            UIHelper.Remove(UIType.UICommon_EquipTips);
            UIHelper.Remove(UIType.UICommon_ItemTips);
            UIHelper.Remove(UIType.UICommon_ResourceNotEnough);
        }

        private void DataInit()
        {
            tbrecharge = ConfigManager.Instance.Tables.Tbrecharge;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            tbuser_Variable = ConfigManager.Instance.Tables.Tbuser_variable;
            tbspecials = ConfigManager.Instance.Tables.Tbspecials;
            tbgift = ConfigManager.Instance.Tables.Tbgift;
            tbprice = ConfigManager.Instance.Tables.Tbprice;
            tbshop_Daily = ConfigManager.Instance.Tables.Tbshop_daily;
            tbconstant = ConfigManager.Instance.Tables.Tbconstant;
        }


        private void On1201BuyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.SHOP1201, On1201BuyResponse);
            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);
            Log.Debug($"购买1201物品:{stringValueList}", Color.green);


            StringValue stringValue = new StringValue();
            //stringValue.MergeFrom(e.data);


            if (e.data.IsEmpty)
            {
                return;
            }

            if (SecConfirmUI != null)
            {
                SecConfirmUI.Dispose();
            }

            var rewards = UnicornUIHelper.TurnStrReward2List(stringValueList.Values);
            UIHelper.Create(UIType.UICommon_Reward, rewards);
            //ResourcesSingletonOld.Instance.UpdateResourceUI();

            
            if (stringValueList.Values.Count > 0)
            {
                var dbList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList;

                int index = 0;
                foreach (var db in dbList)
                {
                    if (tbshop_Daily.Get(db.Sign).pos == ID)
                    {
                        break;
                    }

                    index++;
                }

                ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList[index].UpTime =
                    TimeHelper.ClientNowSeconds();
                ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList[index].BuyCount += 1;


                var a = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList[index];
                int sign = a.Sign;
                int times = a.BuyCount;
                long upTime = a.UpTime;

                if (true)
                {
                    // List<Vector3> vector3s = new List<Vector3>();
                    // foreach (var r in tbshop_Daily.Get(sign).reward)
                    // {
                    //     vector3s.Add(r);
                    // }
                    //
                    // UIHelper.Create(UIType.UICommon_Reward, vector3s);
                    // vector3s.Clear();

                    if (tbshop_Daily[sign].cost[0][0] == 3)
                    {
                        //gold
                        ResourcesSingletonOld.Instance.UserInfo.RoleAssets.UsBill -= (int)tbshop_Daily[sign].cost[0][2];
                    }

                    else if (tbshop_Daily[sign].cost[0][0] == 2)
                    {
                        //gems
                        ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin -= (int)tbshop_Daily[sign].cost[0][2];
                    }
                }
                else
                {
                    // foreach (var r in tbshop_Daily.Get(sign).reward)
                    // {
                    //     UnicornUIHelper.AddReward(r, true);
                    // }
                }

                if (this != null)
                {
                    //this.GetFromReference(KBtn_Common).SetActive(false);
                    //this.GetFromReference(KStateText).SetActive(true);
                    //this.GetFromReference(KStateText).GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_purchased").current);

                    if (tbshop_Daily.DataMap.ContainsKey(sign))
                    {
                        if (times < tbshop_Daily.Get(sign).times)
                        {
                            if (tbshop_Daily.Get(sign).pos == 1)
                            {
                                //pos 1
                                if (TimeHelper.ClientNowSeconds() - upTime >
                                    tbconstant.Get("shop_daily_ad_cd").constantValue)
                                {
                                    //不在CD

                                    var redDotStr = NodeNames.GetTagFuncRedDotName(1201);
                                    RedDotManager.Instance.SetRedPointCnt(redDotStr, 1);
                                    GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);


                                    //RedPointMgr.instance.SetState("ShopRoot", "12011", RedPointState.Show);
                                    this.GetFromReference(KStateText).SetActive(false);
                                    //RedPointMgr.instance.SetState("ShopRoot", "module1201", RedPointState.Show);
                                    //判断是广告免费还是免费
                                    this.GetFromReference(KImg_Green).SetActive(true);
                                    if (times < tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1])
                                    {
                                        //免费
                                        if (tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1] -
                                            times > 1)
                                        {
                                            this.GetFromReference(KText_Mid).GetTextMeshPro()
                                                .SetTMPText(tblanguage.Get("common_free_text").current + "(" +
                                                            (tbshop_Daily[sign].freeRule[0][0] -
                                                             tbshop_Daily[sign].freeRule[0][1] - times).ToString()
                                                            + ")");
                                        }
                                        else
                                        {
                                            this.GetFromReference(KText_Mid).GetTextMeshPro()
                                                .SetTMPText(tblanguage.Get("common_free_text").current);
                                        }
                                    }
                                    else
                                    {
                                        //广告免费
                                        this.GetFromReference(KText_Mid).SetActive(true);
                                        if (tbshop_Daily[sign].freeRule[0][1] -
                                            (times - (tbshop_Daily[sign].freeRule[0][0] -
                                                      tbshop_Daily[sign].freeRule[0][1])) > 1)
                                        {
                                            this.GetFromReference(KText_Mid).GetTextMeshPro()
                                                .SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                                                            tblanguage.Get("common_free_text").current + "(" +
                                                            (tbshop_Daily[sign].freeRule[0][1]
                                                             - (times - (tbshop_Daily[sign].freeRule[0][0] -
                                                                         tbshop_Daily[sign].freeRule[0][1]))).ToString()
                                                            + ")");
                                        }
                                        else
                                        {
                                            this.GetFromReference(KText_Mid).GetTextMeshPro()
                                                .SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                                                            tblanguage.Get("common_free_text").current);
                                        }
                                    }

                                    this.GetFromReference(KBtn_Common).SetActive(true);
                                }
                                else
                                {
                                    var redDotStr = NodeNames.GetTagFuncRedDotName(1201);
                                    RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
                                    GetFromReference(KImg_RedDot)?.SetActive(false);

                                    //在CD
                                    //RedPointMgr.instance.SetState("ShopRoot", "12011", RedPointState.Hide);
                                    //RedPointMgr.instance.SetState("ShopRoot", "module1201", RedPointState.Hide);
                                    long cdTime = tbconstant.Get("shop_daily_ad_cd").constantValue -
                                        TimeHelper.ClientNowSeconds() + upTime;
                                    //cdTime = -cdTime;
                                    this.GetFromReference(KImg_Green).SetActive(false);
                                    this.GetFromReference(KBtn_Common).SetActive(false);
                                    this.GetFromReference(KStateText).SetActive(true);
                                    this.GetFromReference(KStateText).GetTextMeshPro()
                                        .SetTMPText(UnicornUIHelper.GeneralTimeFormat(new int4(1, 2, 1, 1), cdTime) + "\n"
                                            + tblanguage.Get("shop_daily_1_countdown_text").current);
                                }
                            }
                            else
                            {
                                //除了1之外的其他位置

                                if (tbshop_Daily[sign].cost[0][0] == 3)
                                {
                                    //消耗游戏币
                                    this.GetFromReference(KText_Mid).GetTextMeshPro()
                                        .SetTMPText(UnicornUIHelper.GetRewardTextIconName(tbuser_Variable[3].icon) +
                                                    tbshop_Daily[sign].cost[0][2].ToString());
                                }

                                if (tbshop_Daily[sign].cost[0][0] == 2)
                                {
                                    //消耗充值货币
                                    this.GetFromReference(KText_Mid).GetTextMeshPro()
                                        .SetTMPText(UnicornUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon) +
                                                    tbshop_Daily[sign].cost[0][2].ToString());
                                }

                                this.GetFromReference(KText_Mid).SetActive(true);
                                this.GetFromReference(KBtn_Common).SetActive(true);
                            }
                        }
                        else
                        {
                            //没有购买次数
                            if (tbshop_Daily[sign].pos == 1)
                            {
                                var redDotStr = NodeNames.GetTagFuncRedDotName(1201);
                                RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
                                GetFromReference(KImg_RedDot)?.SetActive(false);


                                //当前位置位1
                                //RedPointMgr.instance.SetState("ShopRoot", "12011", RedPointState.Hide);
                                this.GetFromReference(KStateText).GetTextMeshPro().SetTMPText(
                                    tblanguage.Get("shop_daily_gained_text").current);
                                this.GetFromReference(KImg_Green).SetActive(false);
                            }
                            else
                            {
                                //当前位置为除了1之外的位置
                                this.GetFromReference(KStateText).GetTextMeshPro().SetTMPText(
                                    tblanguage.Get("shop_daily_purchased_text").current);
                            }

                            this.GetFromReference(KStateText).SetActive(true);
                            this.GetFromReference(KBtn_Common).SetActive(false);
                        }
                    }
                }

                var parent = this.GetParent<UIPanel_Shop>();
                parent.UpDateNowInterface();
            }


            //ResourcesSingletonOld.Instance.UpdateResourceUI();
        }

        private void On1202BuyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.SHOP1202, On1202BuyResponse);

            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                return;
            }

            
            var rewards = UnicornUIHelper.TurnStrReward2List(stringValueList.Values);
            UIHelper.Create(UIType.UICommon_Reward, rewards);
            
            
            // foreach (var itemstr in stringValueList.Values)
            // {
            //     UnicornUIHelper.AddReward(UnityHelper.StrToVector3(itemstr), true);
            // }

            if (this != null)
            {
                var gsList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1202].GameSpecialsList;
                GameSpecials gameSpecials = new GameSpecials();
                int i = 0;
                foreach (var gs in gsList)
                {
                    if (gs.SpecialId == ID)
                    {
                        gameSpecials = gs;
                        break;
                    }

                    i++;
                }

                var sp = tbspecials.Get(ID);
                if (sp.freeRule.Count == 0)
                {
                    ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1202].GameSpecialsList[i].BuyCount = 0;
                }
                else
                {
                    var redDotStr = NodeNames.GetTagFuncRedDotName(tagID);
                    RedDotManager.Instance.SetRedPointCnt(redDotStr + sp.type.ToString(), 0);
                    GetFromReference(KImg_RedDot)?.SetActive(false);

                    ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1202].GameSpecialsList[i].FreeCount = 0;
                    //RedPointMgr.instance.SetState("ShopRoot", "module1202" + tbspecials.Get(ID).id.ToString(),
                    //RedPointState.Hide);
                }

                this.GetFromReference(KImg_Green).SetActive(false);
                this.GetFromReference(KBtn_Common).SetActive(false);
                this.GetFromReference(KStateText).SetActive(true);
                this.GetFromReference(KStateText).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_purchased").current);
                ResourcesSingletonOld.Instance.UpdateResourceUI();
            }
        }

        private void On1302BuyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 5, On1302BuyResponse);
            GiftResult giftResult = new GiftResult();
            giftResult.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(giftResult);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            List<Vector3> reList = new List<Vector3>();
            reList.Clear();
            foreach (var itemstr in giftResult.Reward)
            {
                reList.Add(UnityHelper.StrToVector3(itemstr));
            }

            UIHelper.Create(UIType.UICommon_Reward, reList);

            if (this != null)
            {
            }

            ResourcesSingletonOld.Instance.UpdateResourceUI();
        }

        private void On1401BuyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 4, On1401BuyResponse);
            var intvalue = new IntValue();
            intvalue.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(intvalue);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            Vector3 vector3 = new Vector3();
            vector3.x = 2;
            vector3.y = 0;
            int zHelp = 0;

            var cList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1401].ChargeInfoList;
            ChargeInfo chargeInfo = new ChargeInfo();
            int i = 0;
            foreach (var c in cList)
            {
                if (c.Id == ID)
                {
                    chargeInfo = c;
                    break;
                }

                i++;
            }

            recharge rc = null;
            foreach (var rec in tbrecharge.DataList)
            {
                if (rec.id == ID)
                {
                    rc = rec;
                }
            }

            if (chargeInfo.ChargeSum == 0)
            {
                zHelp = rc.value * 2;
            }
            else
            {
                zHelp = rc.value + rc.valueExtra;
            }

            vector3.z = zHelp;

            UnicornUIHelper.AddReward(vector3, true).Forget();

            if (this != null)
            {
                ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1401].ChargeInfoList[i].ChargeSum += 1;
                this.GetFromReference(KTitleTextTop).GetTextMeshPro().SetTMPText(tblanguage.Get("recharge_extra_text")
                    .current.Replace("{0}", rc.valueExtra.ToString()));
            }

            ResourcesSingletonOld.Instance.UpdateResourceUI();
        }

        private void On1402BuyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 4, On1402BuyResponse);
            var intvalue = new IntValue();
            intvalue.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(intvalue);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            Vector3 vector3 = new Vector3();
            vector3.x = 3;
            vector3.y = 0;
            int zHelp = 0;

            recharge rc = null;
            foreach (var rec in tbrecharge.DataList)
            {
                if (rec.id == ID)
                {
                    rc = rec;
                }
            }

            int moneyHelp = ResourcesSingletonOld.Instance.levelInfo.maxPassChapterID;
            if (moneyHelp == 0)
            {
                moneyHelp = tbchapter[1].money;
            }
            else
            {
                foreach (var chapter in tbchapter.DataList)
                {
                    if (moneyHelp == chapter.id)
                    {
                        moneyHelp = chapter.money;
                        break;
                    }
                }
            }

            moneyHelp = moneyHelp * 6 * rc.value;
            moneyHelp = (moneyHelp * (ResourcesSingletonOld.Instance.UserInfo.PatrolGainName + 100)) / 100;

            zHelp = moneyHelp;

            vector3.z = zHelp;

            UnicornUIHelper.AddReward(vector3, true).Forget();

            if (this != null)
            {
                var cList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1402].ChargeInfoList;
                ChargeInfo chargeInfo = new ChargeInfo();
                int i = 0;
                foreach (var c in cList)
                {
                    if (c.Id == ID)
                    {
                        chargeInfo = c;
                        break;
                    }

                    i++;
                }

                if (chargeInfo.FreeSum > 0)
                {
                    ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1402].ChargeInfoList[i].FreeSum -= 1;
                }
                else if (chargeInfo.AdSum > 0)
                {
                    ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1402].ChargeInfoList[i].AdSum -= 1;
                }
                else if (rc.unit == 2)
                {
                    ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin -= rc.price;
                }

                var redDotStr = NodeNames.GetTagFuncRedDotName(tagID);
                RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
                GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(false);
                if (chargeInfo.FreeSum > 0)
                {
                    RedDotManager.Instance.SetRedPointCnt(redDotStr, 1);
                    GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);


                    this.GetFromReference(KText_Mid).SetActive(true);
                    this.GetFromReference(KText_Mid).GetTextMeshPro().SetTMPText(
                        tblanguage.Get("common_free_text").current + "(" + chargeInfo.FreeSum.ToString() + ")");
                    if (rc.freeRule.Count > 0)
                    {
                        //RedPointMgr.instance.SetState("ShopRoot", "module1402", RedPointState.Show);
                    }
                }
                else if (chargeInfo.AdSum > 0)
                {
                    RedDotManager.Instance.SetRedPointCnt(redDotStr, 1);
                    GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);


                    this.GetFromReference(KText_Mid).SetActive(true);
                    this.GetFromReference(KText_Mid).GetTextMeshPro().SetTMPText(
                        UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                        tblanguage.Get("common_free_text").current + "(" + chargeInfo.AdSum.ToString() + ")");
                    if (rc.freeRule.Count > 0)
                    {
                        //RedPointMgr.instance.SetState("ShopRoot", "module1402", RedPointState.Show);
                    }
                }
                else if (rc.unit == 2)
                {
                    this.GetFromReference(KText_Mid).SetActive(true);
                    this.GetFromReference(KText_Mid).GetTextMeshPro().SetTMPText(
                        UnicornUIHelper.GetRewardTextIconName(tbuser_Variable.Get(2).icon) + rc.price.ToString());
                    if (rc.freeRule.Count > 0)
                    {
                        //RedPointMgr.instance.SetState("ShopRoot", "module1402", RedPointState.Hide);
                    }
                }
            }

            ResourcesSingletonOld.Instance.UpdateResourceUI();
        }


        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
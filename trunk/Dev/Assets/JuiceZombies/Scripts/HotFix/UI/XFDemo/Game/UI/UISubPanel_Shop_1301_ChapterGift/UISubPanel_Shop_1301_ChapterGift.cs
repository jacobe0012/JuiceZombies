//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_1301_ChapterGift)]
    internal sealed class UISubPanel_Shop_1301_ChapterGiftEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_1301_ChapterGift;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_1301_ChapterGift>();
        }
    }

    public partial class UISubPanel_Shop_1301_ChapterGift : UI, IAwake
    {
        #region property

        private Tbgift tbgift;
        private Tbgift_group tbgift_Group;
        private Tblanguage tblanguage;
        private Tbprice tbprice;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private bool NetLock = true;
        private const int ThisModuleID = 1301;
        private const int Module1301_One_Gift_Width = 1142;
        private int ModuleGiftCount = 0;
        private int NowGiftID = 0;
        private int NowIndex = 0;
        private Dictionary<int, UI> IDandUIDic = new Dictionary<int, UI>();
        private float LastScrollValue = 1;

        #endregion

        public void Initialize()
        {
            DataInit();
            ImgSet().Forget();
            CalculateCountAndCreateItem(true);
            BtnInit();
            NowIndex = 0;
            DoToMove();
            InitEffect();
        }

        private void InitEffect()
        {
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(GetFromReference(KImg), -20,cancellationToken:cts.Token);
          
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(GetFromReference(KImgHand),0,cancellationToken:cts.Token);
            var giftList = this.GetFromReference(KContent).GetList();
            //foreach (var gift in giftList.Children)
            //{
            //    var item = gift as UISubPanel_Shop_Gift_Item;
            //    JiYuTweenHelper.SetEaseAlphaAndPosLtoR(item.GetFromReference(UISubPanel_Shop_Gift_Item.KBg), 0, 100, cts.Token, 0.15f,
            //    false, true);
            //}

        }

        private void DataInit()
        {
            tbgift = ConfigManager.Instance.Tables.Tbgift;
            tbgift_Group = ConfigManager.Instance.Tables.Tbgift_group;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbprice = ConfigManager.Instance.Tables.Tbprice;
        }

        private async UniTaskVoid ImgSet()
        {
            await this.GetFromReference(KImg).GetImage().SetSpriteAsync(tbgift_Group.Get(100).pic, false);
        }

        private void SetContenWidth()
        {
            float w = Module1301_One_Gift_Width * ModuleGiftCount;
            this.GetFromReference(KContent).GetRectTransform().SetWidth(w);
        }

        private void BtnInit()
        {
            var leftBtn = this.GetFromReference(KBtn_Left);
            var rightBtn = this.GetFromReference(KBtn_Right);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(leftBtn, () =>
            {
                NowIndex -= 1;
                DoToMove();
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(rightBtn, () =>
            {
                NowIndex += 1;
                DoToMove();
            });

            var scroll = this.GetFromReference(KScrollView).GetXScrollRect();
            scroll.OnEndDrag.Add(() => { GetIndexAndMove(); });
            //this.GetFromReference(KScrollView).GetScrollRect().OnValueChanged

            scroll.OnBeginDrag.Add(JiYuUIHelper.DestoryAllTips);

            // JiYuScrollRect scroll = this.GetFromReference(KScrollView).GetComponent<JiYuScrollRect>();
            // scroll.SetOnEndDragAcrion(() =>
            // {
            //     GetIndexAndMove();
            // });
        }

        private void CalculateCountAndCreateItem(bool createItem = false)
        {
            Dictionary<int, GiftInfo> IDGiftInfoDic = new Dictionary<int, GiftInfo>();
            var shopGiftList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ThisModuleID].GiftInfoList;
            //Debug.Log(ResourcesSingleton.Instance.shopMap.IndexModuleMap[ThisModuleID].GiftInfoList);

            //fund times > 0 gift by server
            Log.Debug($"shopGiftList {shopGiftList}");
            foreach (var shopGift in shopGiftList)
            {
                if (shopGift.Times > 0)
                {
                    IDGiftInfoDic.Add(shopGift.GiftId, shopGift);
                }
            }

            //fund group = 100 gift by excal
            List<int> giftIDList = new List<int>();
            foreach (var tgf in tbgift.DataList)
            {
                if (tgf.group == 100)
                {
                    giftIDList.Add(tgf.id);
                }
            }

            giftIDList.Sort();

            //compare times > 0 gift and fund group = 100 gift
            int giftCount = 0;
            foreach (var id in giftIDList)
            {
                if (IDGiftInfoDic.ContainsKey(id))
                {
                    giftCount++;
                }
            }

            ModuleGiftCount = giftCount;
            SetContenWidth();
            if(ModuleGiftCount<= 0){
                GetFromReference(KScrollView).SetActive(false);
                GetFromReference(KNoBox).SetActive(true);
                GetFromReference(KText_Sold).GetTextMeshPro().SetTMPText(tblanguage.Get("active_sell_out_text").current);
            }
            else
            {
                GetFromReference(KScrollView).SetActive(true);
                GetFromReference(KNoBox).SetActive(false);
                if (createItem)
                {
                    CreateItem(giftIDList, IDGiftInfoDic).Forget();
                }
            }


        }

        private async UniTaskVoid CreateItem(List<int> giftIDList, Dictionary<int, GiftInfo> IDGiftInfoDic)
        {
            var giftList = this.GetFromReference(KContent).GetList();
            foreach (var id in giftIDList)
            {
                if (IDGiftInfoDic.ContainsKey(id))
                {
                    Log.Debug($"礼包id:{id},次数:{IDGiftInfoDic[id].Times}", Color.cyan);
                    if (IDGiftInfoDic[id].Times <= 0)
                    {
                        //Log.Debug($"礼包id:{id},次数:{IDGiftInfoDic[id].Times}",Color.cyan);
                        continue;
                    }

                    gift g = tbgift.Get(id);
                    var ui = await giftList.CreateWithUITypeAsync(UIType.UISubPanel_Shop_Gift_Item, false, cts.Token);
                    IDandUIDic.Add(id, ui);

                    ui.GetFromReference(UISubPanel_Shop_Gift_Item.KText_Title).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get(g.name).current.Replace("{0}", g.namePara.ToString()));
                    if (g.opYn == 1)
                    {
                        ui.GetFromReference(UISubPanel_Shop_Gift_Item.KText_Btn_Left).SetActive(true);
                        ui.GetFromReference(UISubPanel_Shop_Gift_Item.KText_Btn_Left).GetTextMeshPro()
                            .SetTMPText((tbprice.Get(g.price).rmb * g.ratio / 100).ToString() +
                                        tblanguage.Get("common_coin_unit").current);
                    }
                    else
                    {
                        ui.GetFromReference(UISubPanel_Shop_Gift_Item.KText_Btn_Left).SetActive(false);
                    }

                    ui.GetFromReference(UISubPanel_Shop_Gift_Item.KText_Btn_Right).GetTextMeshPro()
                        .SetTMPText(tbprice.Get(g.price).rmb.ToString() + tblanguage.Get("common_coin_unit").current);

                    ui.GetFromReference(UISubPanel_Shop_Gift_Item.KText_Discount).GetRectTransform()
                        .SetAnchoredPosition(190, 104);
                    ui.GetFromReference(UISubPanel_Shop_Gift_Item.KText_Discount).GetTextMeshPro()
                        .SetTMPText(g.ratio + "%" + tblanguage.Get("gift_ratio_text").current);

                    List<Vector3> vector3s = new List<Vector3>();
                    vector3s = g.reward;

                    JiYuUIHelper.SortRewards(vector3s);

                    #region set reward

                    var re0 = ui.GetFromReference(UISubPanel_Shop_Gift_Item.KRe0);
                    var re1 = ui.GetFromReference(UISubPanel_Shop_Gift_Item.KRe1);
                    var re2 = ui.GetFromReference(UISubPanel_Shop_Gift_Item.KRe2);
                    var re3 = ui.GetFromReference(UISubPanel_Shop_Gift_Item.KRe3);
                    var re4 = ui.GetFromReference(UISubPanel_Shop_Gift_Item.KRe4);
                    var re5 = ui.GetFromReference(UISubPanel_Shop_Gift_Item.KRe5);

                    if (vector3s.Count <= 6)
                    {
                        switch (vector3s.Count)
                        {
                            case 0:
                                re0.SetActive(false);
                                re1.SetActive(false);
                                re2.SetActive(false);
                                re3.SetActive(false);
                                re4.SetActive(false);
                                re5.SetActive(false);
                                break;
                            case 1:
                                re0.SetActive(true);
                                re1.SetActive(false);
                                re2.SetActive(false);
                                re3.SetActive(false);
                                re4.SetActive(false);
                                re5.SetActive(false);

                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[0], re0);

                                SetRewardOnclickIn1301(re0, vector3s[0]);
                                break;
                            case 2:
                                re0.SetActive(true);
                                re1.SetActive(true);
                                re2.SetActive(false);
                                re3.SetActive(false);
                                re4.SetActive(false);
                                re5.SetActive(false);

                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[0], re0);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[1], re1);

                                SetRewardOnclickIn1301(re0, vector3s[0]);
                                SetRewardOnclickIn1301(re1, vector3s[1]);
                                break;
                            case 3:
                                re0.SetActive(true);
                                re1.SetActive(true);
                                re2.SetActive(true);
                                re3.SetActive(false);
                                re4.SetActive(false);
                                re5.SetActive(false);

                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[0], re0);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[1], re1);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[2], re2);

                                SetRewardOnclickIn1301(re0, vector3s[0]);
                                SetRewardOnclickIn1301(re1, vector3s[1]);
                                SetRewardOnclickIn1301(re2, vector3s[2]);
                                break;
                            case 4:
                                re0.SetActive(true);
                                re1.SetActive(true);
                                re2.SetActive(true);
                                re3.SetActive(true);
                                re4.SetActive(false);
                                re5.SetActive(false);

                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[0], re0);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[1], re1);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[2], re2);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[3], re3);

                                SetRewardOnclickIn1301(re0, vector3s[0]);
                                SetRewardOnclickIn1301(re1, vector3s[1]);
                                SetRewardOnclickIn1301(re2, vector3s[2]);
                                SetRewardOnclickIn1301(re3, vector3s[3]);
                                break;
                            case 5:
                                re0.SetActive(true);
                                re1.SetActive(true);
                                re2.SetActive(true);
                                re3.SetActive(true);
                                re4.SetActive(true);
                                re5.SetActive(false);

                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[0], re0);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[1], re1);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[2], re2);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[3], re3);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[4], re4);

                                SetRewardOnclickIn1301(re0, vector3s[0]);
                                SetRewardOnclickIn1301(re1, vector3s[1]);
                                SetRewardOnclickIn1301(re2, vector3s[2]);
                                SetRewardOnclickIn1301(re3, vector3s[3]);
                                SetRewardOnclickIn1301(re4, vector3s[4]);
                                break;
                            case 6:
                                re0.SetActive(true);
                                re1.SetActive(true);
                                re2.SetActive(true);
                                re3.SetActive(true);
                                re4.SetActive(true);
                                re5.SetActive(true);

                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[0], re0);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[1], re1);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[2], re2);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[3], re3);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[4], re4);
                                JiYuUIHelper.SetRewardIconAndCountText(vector3s[5], re5);

                                SetRewardOnclickIn1301(re0, vector3s[0]);
                                SetRewardOnclickIn1301(re1, vector3s[1]);
                                SetRewardOnclickIn1301(re2, vector3s[2]);
                                SetRewardOnclickIn1301(re3, vector3s[3]);
                                SetRewardOnclickIn1301(re4, vector3s[4]);
                                SetRewardOnclickIn1301(re5, vector3s[5]);
                                break;
                        }
                    }
                    else
                    {
                        re0.SetActive(true);
                        re1.SetActive(true);
                        re2.SetActive(true);
                        re3.SetActive(true);
                        re4.SetActive(true);
                        re5.SetActive(true);

                        JiYuUIHelper.SetRewardIconAndCountText(vector3s[0], re0);
                        JiYuUIHelper.SetRewardIconAndCountText(vector3s[1], re1);
                        JiYuUIHelper.SetRewardIconAndCountText(vector3s[2], re2);
                        JiYuUIHelper.SetRewardIconAndCountText(vector3s[3], re3);
                        JiYuUIHelper.SetRewardIconAndCountText(vector3s[4], re4);
                        JiYuUIHelper.SetRewardIconAndCountText(vector3s[5], re5);

                        SetRewardOnclickIn1301(re0, vector3s[0]);
                        SetRewardOnclickIn1301(re1, vector3s[1]);
                        SetRewardOnclickIn1301(re2, vector3s[2]);
                        SetRewardOnclickIn1301(re3, vector3s[3]);
                        SetRewardOnclickIn1301(re4, vector3s[4]);
                        SetRewardOnclickIn1301(re5, vector3s[5]);
                    }

                    #endregion

                    var buyBtn = ui.GetFromReference(UISubPanel_Shop_Gift_Item.KBtn);
                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(buyBtn, () =>
                    {
                        IntValue intValue = new IntValue();
                        intValue.Value = g.id;
                        if (NetLock)
                        {
                            NowGiftID = g.id;
                            NetLock = false;
                            WebMessageHandler.Instance.AddHandler(11, 5, OnBuyGiftResponse);
                            NetWorkManager.Instance.SendMessage(11, 5, intValue);
                        }
                    });
                }
            }
        }

        private void SetRewardOnclickIn1301(UI rewardUI, Vector3 rewardV3)
        {
            if (rewardV3.x == 11)
            {
                int equipId = (int)rewardV3.y;
                var btn = rewardUI.GetFromReference(UICommon_RewardItem.KBtn_Item);
                btn.GetXButton().OnClick.Add(() =>
                {
                    MyGameEquip equip = new MyGameEquip()
                    {
                        reward =rewardV3
                    };

                    var tipUI = UIHelper.Create(UIType.UICommon_EquipTips, equip);
                    tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle).SetActive(false);
                    tipUI.GetFromReference(UICommon_EquipTips.KBottom).SetActive(false);
                    tipUI.GetFromReference(UICommon_EquipTips.KBtn_Decrease).SetActive(false);

                    var itemPos = JiYuUIHelper.GetUIPos(rewardUI);

                    float tipMidH = tipUI.GetFromReference(UICommon_EquipTips.KMid).GetRectTransform().Height();
                    float tipTopH = tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform()
                        .Height();
                    float tipTopHelp = tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle).GetRectTransform()
                        .Height();
                    float tipBottomH = tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).GetRectTransform()
                        .Height();
                    float tipBottomHelp =
                        tipUI.GetFromReference(UICommon_EquipTips.KBottom).GetRectTransform().Height();
                    float screeenH = Screen.height;
                    float itemH = rewardUI.GetRectTransform().Height();

                    if (itemPos.y - tipTopH - tipMidH - itemH > -screeenH / 2)
                    {
                        //down
                        tipUI.GetRectTransform()
                            .SetAnchoredPositionY(itemPos.y - tipMidH / 2 - tipTopH - itemH + tipTopHelp - tipTopH);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(true);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(false);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform()
                            .SetAnchoredPositionX(itemPos.x);
                    }
                    else
                    {
                        //up
                        tipUI.GetRectTransform().SetAnchoredPositionY(itemPos.y + tipMidH / 2 + tipBottomH - itemH -
                            tipBottomHelp + tipBottomH);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(false);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(true);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).GetRectTransform()
                            .SetAnchoredPositionX(itemPos.x);
                    }
                });
            }
            else
            {
                JiYuUIHelper.SetRewardOnClick(rewardV3, rewardUI);
            }
        }

        private void OnBuyGiftResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(11, 5, OnBuyGiftResponse);
            GiftResult giftResult = new GiftResult();
            giftResult.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            List<Vector3> reList = new List<Vector3>();
            foreach (var itemstr in giftResult.Reward)
            {
                reList.Add(UnityHelper.StrToVector3(itemstr));
            }

            UIHelper.Create(UIType.UICommon_Reward, reList);

            int index = 0;
            for (int i = 0;
                 i < ResourcesSingleton.Instance.shopMap.IndexModuleMap[ThisModuleID].GiftInfoList.Count;
                 i++)
            {
                if (NowGiftID == ResourcesSingleton.Instance.shopMap.IndexModuleMap[ThisModuleID].GiftInfoList[i]
                        .GiftId)
                {
                    index = i;
                    break;
                }
            }

            ResourcesSingleton.Instance.shopMap.IndexModuleMap[ThisModuleID].GiftInfoList[index].Times -= 1;
            Log.Debug(
                $"礼包id:{ResourcesSingleton.Instance.shopMap.IndexModuleMap[ThisModuleID].GiftInfoList[index].GiftId},次数:{ResourcesSingleton.Instance.shopMap.IndexModuleMap[ThisModuleID].GiftInfoList[index].Times}",
                Color.cyan);
            if (IDandUIDic[NowGiftID] != null)
            {
                IDandUIDic[NowGiftID].Dispose();
            }

            if (IDandUIDic.Count == 0)
            {
                this.Dispose();
            }
            else
            {
                CalculateCountAndCreateItem(false);
            }

            ResourcesSingleton.Instance.UpdateResourceUI();
            NetLock = true;
        }

        private void GetIndexAndMove()
        {
            float contentX = this.GetFromReference(KContent).GetRectTransform().AnchoredPosition().x;
            contentX = math.abs(contentX);
            int posHelpIndex = (int)(contentX / Module1301_One_Gift_Width);
            if (contentX > posHelpIndex * Module1301_One_Gift_Width + Module1301_One_Gift_Width / 2)
            {
                posHelpIndex += 1;
            }

            NowIndex = posHelpIndex;
            DoToMove();
        }

        private void DoToMove()
        {
            this.GetFromReference(KContent).GetRectTransform()
                .DoAnchoredPositionX(-NowIndex * Module1301_One_Gift_Width, 0.2f);
            //NowIndex = indexHelp;
            SetLeftAndRightBtnActive();
        }

        private void SetLeftAndRightBtnActive()
        {
            if (ModuleGiftCount > 0)
            {
                if (NowIndex == 0)
                {
                    this.GetFromReference(KBtn_Left).SetActive(false);
                    if (ModuleGiftCount > 1)
                    {
                        this.GetFromReference(KBtn_Right).SetActive(true);
                    }
                    else
                    {
                        this.GetFromReference(KBtn_Right).SetActive(false);
                    }
                }
                else if (NowIndex == ModuleGiftCount - 1)
                {
                    this.GetFromReference(KBtn_Left).SetActive(true);
                    this.GetFromReference(KBtn_Right).SetActive(false);
                }
                else
                {
                    this.GetFromReference(KBtn_Left).SetActive(true);
                    this.GetFromReference(KBtn_Right).SetActive(true);
                }
            }
            else
            {
                this.GetFromReference(KBtn_Left).SetActive(false);
                this.GetFromReference(KBtn_Right).SetActive(false);

            }
        }


        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
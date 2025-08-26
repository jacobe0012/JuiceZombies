//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using Google.Protobuf.Collections;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_SelectBoxNomal)]
    internal sealed class UIPanel_SelectBoxNomalEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_SelectBoxNomal;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_SelectBoxNomal>();
        }
    }

    public partial class UIPanel_SelectBoxNomal : UI, IAwake<Vector3>
    {
        private Tblanguage tblanguage;
        private Tbitem tbitem;
        private Tbdrop tbdrop;
        public Vector3 reward;
        public int itemCount = 1;

        public List<Vector3> rewardsList = new List<Vector3>();

        public void Initialize(Vector3 args)
        {
            //this.SetActive(false);
            reward = args;
            InitJson();
            InitNode().Forget();
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbitem = ConfigManager.Instance.Tables.Tbitem;
            tbdrop = ConfigManager.Instance.Tables.Tbdrop;
        }

        async UniTaskVoid InitNode()
        {
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.SELFCHOOSEBOX, OnSelfChooseBoxResponse);

            var KImg_Bg = GetFromReference(UIPanel_SelectBoxNomal.KImg_Bg);
            var KText_Tittle = GetFromReference(UIPanel_SelectBoxNomal.KText_Tittle);
            var KScrollView = GetFromReference(UIPanel_SelectBoxNomal.KScrollView);
            var KBtnGet = GetFromReference(UIPanel_SelectBoxNomal.KBtnGet);
            var KText_Name = GetFromReference(UIPanel_SelectBoxNomal.KText_Name);
            var KText_BoxName = GetFromReference(UIPanel_SelectBoxNomal.KText_BoxName);
            var KText_Number = GetFromReference(UIPanel_SelectBoxNomal.KText_Number);
            var KBtnAddOne = GetFromReference(UIPanel_SelectBoxNomal.KBtnAddOne);
            var KBtnAddMax = GetFromReference(UIPanel_SelectBoxNomal.KBtnAddMax);
            var KBtnDealOne = GetFromReference(UIPanel_SelectBoxNomal.KBtnDealOne);
            var KBtnDealMin = GetFromReference(UIPanel_SelectBoxNomal.KBtnDealMin);
            var KCommon_RewardItem = GetFromReference(UIPanel_SelectBoxNomal.KCommon_RewardItem);
            var KCommon_RewardItem0 = GetFromReference(UIPanel_SelectBoxNomal.KCommon_RewardItem0);
            var KText_BoxName0 = GetFromReference(UIPanel_SelectBoxNomal.KText_BoxName0);
            var KUp = GetFromReference(UIPanel_SelectBoxNomal.KUp);
            var KUpMulti = GetFromReference(UIPanel_SelectBoxNomal.KUpMulti);
            var KBg_Container = GetFromReference(UIPanel_SelectBoxNomal.KBg_Container);
            var KBtn_CloseTips = GetFromReference(UIPanel_SelectBoxNomal.KBtn_CloseTips);
            var KBtn_CloseTips0 = GetFromReference(UIPanel_SelectBoxNomal.KBtn_CloseTips0);

            //var KCommon_Blur = GetFromReference(UIPanel_SelectBoxNomal.KCommon_Blur);
            await UnicornUIHelper.InitBlur(this);


            UnicornUIHelper.SetRewardIconAndCountText(reward, KCommon_RewardItem);
            UnicornUIHelper.SetRewardIconAndCountText(reward, KCommon_RewardItem0);
            var item = tbitem.Get((int)reward.y);
            var boxCount = (int)reward.z;
            boxCount = Mathf.Min(boxCount, 999);
            var dropId = (int)item.useEffect[0].y;

            var dropList = tbdrop.DataList.Where(a => a.id == dropId).ToList();
            var chooseMaxCount = dropList[0].packPower;
            if (item.quality <= 5)
            {
                KUpMulti.SetActive(true);
                KUp.SetActive(false);
            }
            else
            {
                KUpMulti.SetActive(false);
                KUp.SetActive(true);
            }

            KText_BoxName.GetTextMeshPro().SetTMPText(UnicornUIHelper.GetRewardName(reward));
            KText_BoxName0.GetTextMeshPro().SetTMPText(UnicornUIHelper.GetRewardName(reward));

            KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_confirm").current);
            KText_Number.GetTextMeshPro().SetTMPText($"{itemCount}/{boxCount}");

            var titleStr = string.Format(tblanguage.Get("text_box_choose").current, chooseMaxCount);
            KText_Tittle.GetTextMeshPro().SetTMPText(titleStr);

            Log.Debug($"Count{dropList.Count}");

            var scrollRect = KScrollView.GetXScrollRect();
            scrollRect.SetVerticalNormalizedPosition(0);
            scrollRect.OnBeginDrag.Add(() => { UnicornUIHelper.DestoryAllTips(); });
            var list = scrollRect.Content.GetList();
            list.Clear();
            foreach (var dropItem in dropList)
            {
                var ui =
                    await list.CreateWithUITypeAsync(UIType.UISubPanel_SelectBoxItem, dropItem.reward[0], false) as
                        UISubPanel_SelectBoxItem;
                var KBtn_Selected = ui.GetFromReference(UISubPanel_SelectBoxItem.KBtn_Selected);
                var KImg_Selected = ui.GetFromReference(UISubPanel_SelectBoxItem.KImg_Selected);
                var KImg_UnSelected = ui.GetFromReference(UISubPanel_SelectBoxItem.KImg_UnSelected);
                var KCommon_RewardItemDrop = ui.GetFromReference(UISubPanel_SelectBoxItem.KCommon_RewardItem);

                var KImg_Selected0 = KCommon_RewardItemDrop.GetFromReference(UICommon_RewardItem.KImg_Selected);
                UnicornUIHelper.SetRewardOnClick(dropItem.reward[0], KCommon_RewardItemDrop);


                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Selected, async () =>
                {
                    if (KImg_Selected.GameObject.activeSelf)
                    {
                        rewardsList.Remove(dropItem.reward[0]);
                        KImg_Selected.SetActive(false);
                        KImg_UnSelected.SetActive(true);
                        KImg_Selected0.SetActive(false);
                    }
                    else
                    {
                        if (rewardsList.Count() >= chooseMaxCount)
                        {
                            UnicornUIHelper.ClearCommonResource();
                            UIHelper.CreateAsync(UIType.UICommon_Resource,
                                tblanguage.Get("text_item_selection_limit_exceeded").current).Forget();

                            return;
                        }

                        rewardsList.Add(dropItem.reward[0]);
                        KImg_Selected.SetActive(true);
                        KImg_UnSelected.SetActive(false);
                        KImg_Selected0.SetActive(true);
                    }
                });
            }

            list.Sort(UnicornUIHelper.SelectedBoxItemUIComparer);


            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtnGet, () =>
            {
                if (rewardsList.Count() < chooseMaxCount)
                {
                    UnicornUIHelper.ClearCommonResource();
                    var str = string.Format(tblanguage.Get("text_item_choose_more").current,
                        chooseMaxCount - rewardsList.Count());
                    UIHelper.CreateAsync(UIType.UICommon_Resource, str).Forget();
                    return;
                }

                var bagItem = new BagItem();
                var useItemParam = new UseItemParam();
                bagItem.ItemId = item.id;
                bagItem.Count = itemCount;
                useItemParam.ItemVal = bagItem;
                UnicornUIHelper.TurnList2StrReward(useItemParam.Reward, rewardsList);

                Log.Debug($"Use:useItemParam {useItemParam}", Color.green);
                NetWorkManager.Instance.SendMessage(CMDOld.SELFCHOOSEBOX, useItemParam);
                //rewards.Add()
                //useItemParam.Reward
                //useItemParam.
            });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtnAddOne, () =>
            {
                if (itemCount >= boxCount)
                {
                    UnicornUIHelper.ClearCommonResource();
                    var str = tblanguage.Get("text_item_selection_limit_exceeded").current;
                    UIHelper.CreateAsync(UIType.UICommon_Resource, str).Forget();
                    return;
                }

                itemCount++;
                KText_Number.GetTextMeshPro().SetTMPText($"{itemCount}/{boxCount}");
            });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtnAddMax, () =>
            {
                itemCount = boxCount;
                KText_Number.GetTextMeshPro().SetTMPText($"{itemCount}/{boxCount}");
            });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtnDealOne, () =>
            {
                if (itemCount <= 1)
                {
                    return;
                }

                itemCount--;
                KText_Number.GetTextMeshPro().SetTMPText($"{itemCount}/{boxCount}");
            });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtnDealMin, () =>
            {
                itemCount = 1;
                KText_Number.GetTextMeshPro().SetTMPText($"{itemCount}/{boxCount}");
            });
            KImg_Bg.GetButton().OnClick.Add(() => { Close(); });
            KBg_Container.GetButton().OnClick.Add(() => { UnicornUIHelper.DestoryAllTips(); });
            KBtn_CloseTips.GetButton().OnClick.Add(() => { UnicornUIHelper.DestoryAllTips(); });
            KBtn_CloseTips0.GetButton().OnClick.Add(() => { Close(); });
            await UniTask.Yield();
            var totalH = scrollRect.Content.GetRectTransform().Height() + KBtn_CloseTips.GetRectTransform().Height();
            totalH = Mathf.Min(750f, totalH);
            KBg_Container.GetRectTransform().SetHeight(totalH);
            //this.SetActive(true);
        }

        private async void OnSelfChooseBoxResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var rawList = new StringValueList();
            rawList.MergeFrom(e.data);

            Log.Debug($"OnSelfChooseBoxResponse:{rawList}", Color.green);
            if (e.data.IsEmpty)
            {
                Log.Debug("OnSelfChooseBoxResponse IsEmpty", Color.red);
                return;
            }

            var reward = UnicornUIHelper.TurnStrReward2List(rawList.Values);

            var ui = await UIHelper.CreateAsync(UIType.UICommon_Reward, reward);
            //var KBtn_Close = ui.GetFromReference(UICommon_Reward.KBtn_Close);
            //var KBg_Img = ui.GetFromReference(UICommon_Reward.KBg_Img);

            //KBtn_Close.GetButton().OnClick.Add(() => { Refresh().Forget(); });
            //KBg_Img.GetButton().OnClick.Add(() => { Refresh().Forget(); });


            //Log.Debug($"OnExchangeResponse:{buyDice.Value}", Color.green);
            //Close();
        }

        protected override void OnClose()
        {
            UnicornUIHelper.DestoryAllTips();
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.SELFCHOOSEBOX, OnSelfChooseBoxResponse);
            rewardsList.Clear();
            base.OnClose();
        }
    }
}
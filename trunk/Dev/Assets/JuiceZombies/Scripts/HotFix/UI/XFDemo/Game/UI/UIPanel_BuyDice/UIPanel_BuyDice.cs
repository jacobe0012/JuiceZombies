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

namespace XFramework
{
    [UIEvent(UIType.UIPanel_BuyDice)]
    internal sealed class UIPanel_BuyDiceEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_BuyDice;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_BuyDice>();
        }
    }

    public partial class UIPanel_BuyDice : UI, IAwake<int>
    {
        private Tbmonopoly tbmonopoly;
        private Tbmonopoly_cell tbmonopoly_cell;
        private Tbmonopoly_event tbmonopoly_event;
        private Tbmonopoly_shop tbmonopoly_shop;
        private Tbmonopoly_event_lotto tbmonopoly_event_lotto;
        private TbitemOld tbitem;
        private Tblanguage tblanguage;
        private Tbactivity tbactivity;
        public int activityId;
        public int canBuyCount;
        private Vector3 dimonad;

        public async void Initialize(int args)
        {
            await UnicornUIHelper.InitBlur(this);
            activityId = args;
            InitJson();
            InitNode();
        }

        void InitJson()
        {
            tbmonopoly = ConfigManager.Instance.Tables.Tbmonopoly;
            tbmonopoly_cell = ConfigManager.Instance.Tables.Tbmonopoly_cell;
            tbmonopoly_event = ConfigManager.Instance.Tables.Tbmonopoly_event;
            tbmonopoly_shop = ConfigManager.Instance.Tables.Tbmonopoly_shop;
            tbmonopoly_event_lotto = ConfigManager.Instance.Tables.Tbmonopoly_event_lotto;
            tbitem = ConfigManager.Instance.Tables.TbitemOld;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbactivity = ConfigManager.Instance.Tables.Tbactivity;
        }

        void Refresh()
        {
            var KText_Name = GetFromReference(UIPanel_BuyDice.KText_Name);
            var KReward_Pos = GetFromReference(UIPanel_BuyDice.KReward_Pos);
            var KBtn_Left = GetFromReference(UIPanel_BuyDice.KBtn_Left);
            var KBtn_Right = GetFromReference(UIPanel_BuyDice.KBtn_Right);
            var KSlider = GetFromReference(UIPanel_BuyDice.KSlider);
            var KBtn_Buy = GetFromReference(UIPanel_BuyDice.KBtn_Buy);
            var KText_BuyCount = GetFromReference(UIPanel_BuyDice.KText_BuyCount);
            var KText_NowCount = GetFromReference(UIPanel_BuyDice.KText_NowCount);
            var KBtn_Mask = GetFromReference(UIPanel_BuyDice.KBtn_Mask);
            var KText_Buy = GetFromReference(UIPanel_BuyDice.KText_Buy);
            var KText_Price = GetFromReference(UIPanel_BuyDice.KText_Price);
            if (!ResourcesSingletonOld.Instance.activity.activityMap.ActivityMap_.TryGetValue(activityId,
                    out var gameActivity))
            {
                Close();
            }

            var activity = tbactivity.Get(activityId);
            var monopoly = tbmonopoly.Get(activity.link);
            var diceItem = tbitem.Get(monopoly.diceItem);
            dimonad = new Vector3(2, 0, 1);
            Vector3 initDiceItem = new Vector3(5, diceItem.id, 1);
            KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get(diceItem.name).current);
            KText_Buy.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_buy").current);

            KText_Price.GetTextMeshPro().SetTMPText($"{UnicornUIHelper.GetRewardTextIconName(dimonad)}{diceItem.value}");

            //

            var buyCountStr =
                $"{tblanguage.Get("text_buy_num").current}{1}";

            KText_BuyCount.GetTextMeshPro().SetTMPText(buyCountStr);

            var nowCountStr =
                $"{tblanguage.Get("text_quantity_on_hand").current}{UnicornUIHelper.GetRewardCount(initDiceItem)}";
            KText_NowCount.GetTextMeshPro().SetTMPText(nowCountStr);

            var slider = KSlider.GetSlider();
            slider.Get().minValue = 1;
            canBuyCount = (int)(UnicornUIHelper.GetRewardCount(new Vector3(2, 0, 1)) / diceItem.value);

            int maxNum = Mathf.Min(100, canBuyCount);

            slider.Get().maxValue = Mathf.Max(maxNum, 1.5f);
            slider.SetValue(1);
            slider.SetEnabled(true);

            if (canBuyCount <= 1)
            {
                slider.SetValue(slider.Get().maxValue);
                slider.SetEnabled(false);
            }
        }

        async void InitNode()
        {
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.BUYDICE, OnBuyDiceResponse);
            var KText_Name = GetFromReference(UIPanel_BuyDice.KText_Name);
            var KReward_Pos = GetFromReference(UIPanel_BuyDice.KReward_Pos);
            var KBtn_Left = GetFromReference(UIPanel_BuyDice.KBtn_Left);
            var KBtn_Right = GetFromReference(UIPanel_BuyDice.KBtn_Right);
            var KSlider = GetFromReference(UIPanel_BuyDice.KSlider);
            var KBtn_Buy = GetFromReference(UIPanel_BuyDice.KBtn_Buy);
            var KText_BuyCount = GetFromReference(UIPanel_BuyDice.KText_BuyCount);
            var KText_NowCount = GetFromReference(UIPanel_BuyDice.KText_NowCount);
            var KBtn_Mask = GetFromReference(UIPanel_BuyDice.KBtn_Mask);
            var KText_Buy = GetFromReference(UIPanel_BuyDice.KText_Buy);
            var KText_Price = GetFromReference(UIPanel_BuyDice.KText_Price);


            if (!ResourcesSingletonOld.Instance.activity.activityMap.ActivityMap_.TryGetValue(activityId,
                    out var gameActivity))
            {
                Close();
            }

            var activity = tbactivity.Get(activityId);
            var monopoly = tbmonopoly.Get(activity.link);
            var diceItem = tbitem.Get(monopoly.diceItem);
            dimonad = new Vector3(2, 0, 1);
            Vector3 initDiceItem = new Vector3(5, diceItem.id, 1);
            KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get(diceItem.name).current);
            KText_Buy.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_buy").current);

            KText_Price.GetTextMeshPro().SetTMPText($"{UnicornUIHelper.GetRewardTextIconName(dimonad)}{diceItem.value}");

            //

            var buyCountStr =
                $"{tblanguage.Get("text_buy_num").current}{1}";

            KText_BuyCount.GetTextMeshPro().SetTMPText(buyCountStr);

            var nowCountStr =
                $"{tblanguage.Get("text_quantity_on_hand").current}{UnicornUIHelper.GetRewardCount(initDiceItem)}";
            KText_NowCount.GetTextMeshPro().SetTMPText(nowCountStr);

            var slider = KSlider.GetSlider();
            slider.Get().minValue = 1;
            canBuyCount = (int)(UnicornUIHelper.GetRewardCount(new Vector3(2, 0, 1)) / diceItem.value);

            int maxNum = Mathf.Min(100, canBuyCount);

            slider.Get().maxValue = Mathf.Max(maxNum, 1.5f);
            slider.SetValue(1);
            slider.SetEnabled(true);

            if (canBuyCount <= 1)
            {
                slider.SetValue(slider.Get().maxValue);
                slider.SetEnabled(false);
            }

            var uiCommon_RewardItem = await UIHelper.CreateAsync(this, UIType.UICommon_RewardItem, initDiceItem,
                KReward_Pos.GetRectTransform().Get());
            uiCommon_RewardItem.GetRectTransform().SetAnchoredPosition(Vector2.zero);
            uiCommon_RewardItem.SetParent(this, true);
            var KText_ItemCount = uiCommon_RewardItem.GetFromReference(UICommon_RewardItem.KText_ItemCount);
            KText_ItemCount.SetActive(true);

            slider.OnValueChanged.Add((value) =>
            {
                int intValue = (int)value;
                var rewardItemui = this.GetChild(UIType.UICommon_RewardItem);
                var KText_ItemCount = rewardItemui.GetFromReference(UICommon_RewardItem.KText_ItemCount);
                KText_ItemCount.GetTextMeshPro().SetTMPText($"x{intValue}");
                KText_ItemCount.SetActive(intValue >= 1);
                var buyCountStr =
                    $"{tblanguage.Get("text_buy_num").current}{intValue}";
                KText_BuyCount.GetTextMeshPro().SetTMPText(buyCountStr);
                KText_Price.GetTextMeshPro()
                    .SetTMPText($"{UnicornUIHelper.GetRewardTextIconName(dimonad)}{diceItem.value * intValue}");
            });
            var btnRight = KBtn_Right.GetXButton();
            btnRight.OnClick.Add(() =>
            {
                if (canBuyCount <= 1)
                {
                    return;
                }

                slider.SetValue((int)slider.Value + 1);
            });
            btnRight.OnLongPress.Add((a) =>
            {
                if (canBuyCount <= 1)
                {
                    return;
                }

                //Log.Debug($"{a}");
                slider.SetValue((int)slider.Value + 1);
            });
            btnRight.SetPointerActive(true);
            btnRight.SetLongPressInterval(0.12f);
            btnRight.SetMaxLongPressCount(-1);

            var btnLeft = KBtn_Left.GetXButton();
            btnLeft.OnClick.Add(() =>
            {
                if (canBuyCount <= 1)
                {
                    return;
                }

                slider.SetValue((int)slider.Value - 1);
            });
            btnLeft.OnLongPress.Add((a) =>
            {
                if (canBuyCount <= 1)
                {
                    return;
                }

                //Log.Debug($"{a}");
                slider.SetValue((int)slider.Value - 1);
            });
            btnLeft.SetPointerActive(true);
            btnLeft.SetLongPressInterval(0.12f);
            btnLeft.SetMaxLongPressCount(-1);


            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Buy, async () =>
            {
                var cost = new Vector3(2, 0, (int)slider.Value * diceItem.value);
                var ui = await UIHelper.CreateAsync(UIType.UICommon_Sec_Confirm, cost, initDiceItem);

                var KBg = ui.GetFromReference(UICommon_Sec_Confirm.KBg);
                KBg.GetImage().SetColor("CDCDCD");

                var KBtn_Buy = ui.GetFromReference(UICommon_Sec_Confirm.KBtn_Buy);


                UnicornTweenHelper.JiYuOnClickNoAnim(KBtn_Buy, () =>
                {
                    //Log.Error($"TryReduceReward1 {cost}");
                    //var cost = new Vector3(2, 0, (int)(slider.Value * diceItem.value));
                    if (!UnicornUIHelper.TryReduceReward(cost))
                    {
                        // Vector3 v3 = tbshop_Daily[sign].cost[0];
                        // v3.z = v3.z - playerHave;
                        // var uiTip = UIHelper.Create<Vector3>(UIType.UICommon_ResourceNotEnough, v3);
                        // UnicornUIHelper.SetResourceNotEnoughTip(uiTip, KBtn_Buy);
                        //
                        //
                        // UIHelper.CreateAsync(UIType.UILack, 2);
                        // ui.Dispose();
                        return;
                    }

                    NetWorkManager.Instance.SendMessage(CMDOld.BUYDICE, new IntValue
                    {
                        Value = (int)slider.Value
                    });

                    ui.Dispose();
                    // var get = initDiceItem;
                    // get.z = (int)slider.Value;
                    // UnicornUIHelper.AddReward(get);
                });
            });


            KBtn_Mask.GetButton().OnClick.Add(() =>
            {
                if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Activity_Monopoly, out var ui))
                {
                    var uis = ui as UIPanel_Activity_Monopoly;
                    uis.Refresh();
                }

                Close();
            });
        }

        async public void OnBuyDiceResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            StringValue buyDice = new StringValue();
            buyDice.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            var reward = UnicornUIHelper.TurnStrReward2List(buyDice.Value);

            var ui = await UIHelper.CreateAsync(UIType.UICommon_Reward, reward);
            var KBtn_Close = ui.GetFromReference(UICommon_Reward.KBtn_Close);
            var KBg_Img = ui.GetFromReference(UICommon_Reward.KBg_Img);
            KBtn_Close.GetButton().OnClick.Add(Refresh);
            KBg_Img.GetButton().OnClick.Add(Refresh);


            Log.Debug($"OnBuyDiceResponse:{buyDice.Value}", Color.green);
            //Close();
        }

        protected override void OnClose()
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.BUYDICE, OnBuyDiceResponse);
            base.OnClose();
        }
    }
}
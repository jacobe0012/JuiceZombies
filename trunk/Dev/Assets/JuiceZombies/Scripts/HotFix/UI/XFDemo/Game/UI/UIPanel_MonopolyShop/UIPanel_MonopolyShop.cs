//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Main;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf;
using DG.Tweening;
using Unity.Entities.UniversalDelegates;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_MonopolyShop)]
    internal sealed class UIPanel_MonopolyShopEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_MonopolyShop;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_MonopolyShop>();
        }
    }

    public partial class UIPanel_MonopolyShop : UI, IAwake<int>, ILoopScrollRectProvide<UISubPanel_MonopolyShopItem>
    {
        private Tblanguage tblanguage;
        private Tbtag_func tbtag_func;
        private Tbblock tbblock;
        private Tbchapter tbchapter;
        private Tblevel tblevel;
        private Tbuser_avatar tbuser_avatar;
        private Tbuser_variable tbuser_variable;
        private Tbuser_level tbuserLevel;
        private Tbdraw_box tbdraw_box;
        private Tbtag tbtag;
        private Tbconstant tbconstant;
        private Tbchapter_box tbchapter_box;
        private Tbactivity tbactivity;
        private Tbdays_challenge tbdays_challenge;
        private Tbbattlepass tbbattlepass;
        private Tbpiggy_bank tbpiggy_bank;
        private Tbdays_sign tbdays_sign;
        private Tbenergy_shop tbenergy_shop;
        private Tbmonopoly tbmonopoly;
        private Tbmonopoly_cell tbmonopoly_cell;
        private Tbmonopoly_event tbmonopoly_event;
        private Tbmonopoly_shop tbmonopoly_shop;
        private Tbmonopoly_event_lotto tbmonopoly_event_lotto;

        public int activityId;

        private long timerId;
        private long endTime;
        private Vector3 shopItemReward;
        private List<monopoly_shop> shopList;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public async void Initialize(int args)
        {
            await JiYuUIHelper.InitBlur(this);
            activityId = args;
            InitJson();
            InitNode();
            JiYuTweenHelper.SetEaseAlphaAndScaleWithFour(GetFromReference(KTip), cancellationToken: cts.Token);
            GetFromReference(KTip).GetComponent<CanvasGroup>().alpha = 0f;
            GetFromReference(KTip).GetComponent<CanvasGroup>().DOFade(1f, 1.5f).SetEase(Ease.InQuad).SetUpdate(true);
            JiYuTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(KImg_Title),cts.Token,"A1DD01",JiYuTweenHelper.UIDir.UpRight);
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            tbblock = ConfigManager.Instance.Tables.Tbblock;
            tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            tbuser_avatar = ConfigManager.Instance.Tables.Tbuser_avatar;
            tbuser_variable = ConfigManager.Instance.Tables.Tbuser_variable;
            tbuserLevel = ConfigManager.Instance.Tables.Tbuser_level;
            tbdraw_box = ConfigManager.Instance.Tables.Tbdraw_box;
            tbtag = ConfigManager.Instance.Tables.Tbtag;
            tbconstant = ConfigManager.Instance.Tables.Tbconstant;
            tbchapter_box = ConfigManager.Instance.Tables.Tbchapter_box;
            tbactivity = ConfigManager.Instance.Tables.Tbactivity;
            tblevel = ConfigManager.Instance.Tables.Tblevel;
            tbdays_challenge = ConfigManager.Instance.Tables.Tbdays_challenge;
            tbbattlepass = ConfigManager.Instance.Tables.Tbbattlepass;
            tbpiggy_bank = ConfigManager.Instance.Tables.Tbpiggy_bank;
            tbdays_sign = ConfigManager.Instance.Tables.Tbdays_sign;
            tbenergy_shop = ConfigManager.Instance.Tables.Tbenergy_shop;
            tbmonopoly = ConfigManager.Instance.Tables.Tbmonopoly;
            tbmonopoly_cell = ConfigManager.Instance.Tables.Tbmonopoly_cell;
            tbmonopoly_event = ConfigManager.Instance.Tables.Tbmonopoly_event;
            tbmonopoly_shop = ConfigManager.Instance.Tables.Tbmonopoly_shop;
            tbmonopoly_event_lotto = ConfigManager.Instance.Tables.Tbmonopoly_event_lotto;
        }

        async UniTaskVoid InitNode()
        {
            WebMessageHandler.Instance.AddHandler(CMD.MONOEXCHANGE, OnExchangeResponse);
            var KScrollView = GetFromReference(UIPanel_MonopolyShop.KScrollView);
            var KBtn_Desc = GetFromReference(UIPanel_MonopolyShop.KBtn_Desc);
            var KText_Title = GetFromReference(UIPanel_MonopolyShop.KText_Title);
            var KText_Time = GetFromReference(UIPanel_MonopolyShop.KText_Time);
            var KBtn_Close = GetFromReference(UIPanel_MonopolyShop.KBtn_Close);
            var KImg_Title = GetFromReference(UIPanel_MonopolyShop.KImg_Title);
            var KCommon_ItemTips = GetFromReference(UIPanel_MonopolyShop.KCommon_ItemTips);
            var KText_Des = GetFromReference(UIPanel_MonopolyShop.KText_Des);
            if (!ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.TryGetValue(activityId,
                    out var gameActivity))
            {
                Close();
            }

            endTime = gameActivity.MonopolyRecord.EndTime;
            var activity = tbactivity.Get(activityId);
            var monopoly = tbmonopoly.Get(activity.link);
            Log.Debug($"ExchangeRecord", Color.green);
            //sort  Count
            foreach (var VARIABLE in gameActivity.MonopolyRecord.ExchangeRecord)
            {
                Log.Debug($"{VARIABLE}", Color.green);
            }

            shopItemReward = new Vector3(5, monopoly.shopItem, 1);
            var diceReward = new Vector3(5, monopoly.diceItem, 1);
            //string diceStr = $"x{JiYuUIHelper.GetRewardCount(diceReward)}

            string shopItemStr =
                $"{JiYuUIHelper.GetRewardTextIconName(shopItemReward)}x{JiYuUIHelper.GetRewardCount(shopItemReward)}";


            KText_Title.GetTextMeshPro().SetTMPText(shopItemStr);
            KImg_Title.GetImage().SetSpriteAsync(monopoly.shopPic, true).Forget();

            string shopDes = string.Format(tblanguage.Get("monopoly_shop_desc").current,
                JiYuUIHelper.GetRewardName(shopItemReward, false));

            KText_Des.GetTextMeshPro().SetTMPText(shopDes);

            KCommon_ItemTips.SetActive(false);

            StartTimer();
            shopList = tbmonopoly_shop.DataList.Where((a) => a.id == monopoly.monopolyShop).OrderBy(item => item.sort)
                .ToList();

            // var scrollRect = KScrollView.GetXScrollRect();
            // var content = scrollRect.Content;
            // var list = content.GetList();
            // list.Clear();
            //scrollRect.OnValueChanged.AddListener((value) => { ScrollRectListener(value); });
            var loopRect = KScrollView.GetLoopScrollRect<UISubPanel_MonopolyShopItem>();
            loopRect.OnBeginDrag.Add(JiYuUIHelper.DestoryAllTips);
            loopRect.Content.GetRectTransform().SetWidth(Screen.width - 30f);
            //loopRect.Dispose();
            // loopRect.OnGetObject = (id) =>
            // {
            //     string str = "";
            //     if (id > 3)
            //     {
            //         str = UIPathSet.UISubPanel_MonopolyShopItem;
            //     }
            //     else
            //     {
            //         str = UIPathSet.UISubPanel_MonopolyTaskShopItem;
            //     }
            //
            //
            //     return str;
            // };

            loopRect.SetProvideData(UIPathSet.UISubPanel_MonopolyShopItem, this);

            loopRect.SetTotalCount(shopList.Count);
            loopRect.RefillCells();
            //loopRect.Get().ClearCells();
            // foreach (var monopolyShop in shopList)
            // {
            //     var ui = await list.CreateWithUITypeAsync(UIType.UISubPanel_MonopolyShopItem, monopolyShop.sort, false);
            //     var KRewardPos = ui.GetFromReference(UISubPanel_MonopolyShopItem.KRewardPos);
            //     var KText_Desc = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_Desc);
            //     var KText_Name = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_Name);
            //     var KText_Cost = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_Cost);
            //     var KBtn_Buy = ui.GetFromReference(UISubPanel_MonopolyShopItem.KBtn_Buy);
            //     var KText_Buy = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_Buy);
            //     var KText_BuyFinish = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_BuyFinish);
            //
            //     int curCount = monopolyShop.limit;
            //     if (gameActivity.MonopolyRecord.ExchangeRecord.TryGetValue(monopolyShop.sort, out var count))
            //     {
            //         curCount = count;
            //     }
            //
            //     var countStr =
            //         UnityHelper.RichTextColor($"{curCount}/{monopolyShop.limit}",
            //             UnityHelper.Color2HexRGB(curCount <= 0 ? Color.red : Color.white));
            //     var desc = $"{tblanguage.Get("monopoly_shop_bug_num_text").current}:{countStr}";
            //
            //     KText_Desc.GetTextMeshPro().SetTMPText(desc);
            //     KText_BuyFinish.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_soldout").current);
            //     KText_Buy.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_exchange").current);
            //
            //     var reward = monopolyShop.reward[0];
            //     var rewardui = await UIHelper.CreateAsync(ui, UIType.UICommon_RewardItem, reward,
            //         KRewardPos.GetRectTransform().Get());
            //     rewardui.SetParent(ui, true);
            //     rewardui.GetRectTransform().SetAnchoredPosition(Vector2.zero);
            //
            //     var name = JiYuUIHelper.GetRewardName(reward);
            //     KText_Name.GetTextMeshPro().SetTMPText(name);
            //     var costCount = JiYuUIHelper.GetRewardCount(shopItemReward);
            //
            //     var costNumStr1 =
            //         UnityHelper.RichTextColor($"x{monopolyShop.costNum}",
            //             UnityHelper.Color2HexRGB(monopolyShop.costNum < costCount ? Color.white : Color.red));
            //
            //     string costNumStr = $"{JiYuUIHelper.GetRewardTextIconName(shopItemReward)}{costNumStr1}";
            //
            //     KText_Cost.GetTextMeshPro().SetTMPText(costNumStr);
            //     KBtn_Buy.SetActive(true);
            //     KText_BuyFinish.SetActive(false);
            //     KBtn_Buy.GetXButton().SetEnabled(monopolyShop.costNum < costCount);
            //
            //     if (curCount <= 0)
            //     {
            //         KBtn_Buy.SetActive(false);
            //         KText_BuyFinish.SetActive(true);
            //     }
            //
            //
            //     JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Buy, () =>
            //     {
            //         KCommon_ItemTips.SetActive(false);
            //
            //         var cost = shopItemReward;
            //         cost.z = monopolyShop.costNum;
            //
            //         if (!JiYuUIHelper.TryReduceReward(cost))
            //         {
            //             //UIHelper.CreateAsync(UIType.UILack, 2);
            //             return;
            //         }
            //
            //         if (gameActivity.MonopolyRecord.ExchangeRecord.ContainsKey(monopolyShop.sort))
            //         {
            //             gameActivity.MonopolyRecord.ExchangeRecord[monopolyShop.sort]--;
            //         }
            //         else
            //         {
            //             gameActivity.MonopolyRecord.ExchangeRecord.Add(monopolyShop.sort, monopolyShop.limit - 1);
            //         }
            //
            //
            //         NetWorkManager.Instance.SendMessage(CMD.MONOEXCHANGE, new IntValue
            //         {
            //             Value = monopolyShop.sort
            //         });
            //     });
            // }

            // list.Sort((a, b) =>
            // {
            //     var uia = a as UISubPanel_MonopolyShopItem;
            //     var uib = b as UISubPanel_MonopolyShopItem;
            //     return uia.sort.CompareTo(uib.sort);
            // });

            //loopRect.SetVerticalNormalizedPosition(0);
            KBtn_Close.GetButton().OnClick.Add(() =>
            {
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Activity_Monopoly, out var ui))
                {
                    var uis = ui as UIPanel_Activity_Monopoly;
                    uis.Refresh();
                }

                Close();
            });

            // scrollRect.OnDrag.Add((a) =>
            // {
            //     //var KCommon_ItemTips = GetFromReference(UIPanel_MonopolyShop.KCommon_ItemTips);
            //     KCommon_ItemTips.SetActive(false);
            // });
        }

        // async void ScrollRectListener(Vector2 value)
        // {
        //     var KScrollView = GetFromReference(UIPanel_MonopolyShop.KScrollView);
        //     var scrollRect = KScrollView.GetXScrollRect();
        //     var content = scrollRect.Content;
        //     var list = content.GetList();
        //
        //     if (list.Count <= 0)
        //         return;
        //     foreach (var child in list.Children)
        //     {
        //         var childui = child as UISubPanel_MonopolyShopItem;
        //         var pos = child.GetRectTransform().AnchoredPosition();
        //         float rangePos = pos.y;
        //         if (IsOutRange(rangePos, child.GetRectTransform()))
        //         {
        //             //把超出范围的cell 扔进 poolsObj里
        //             Log.Debug($"IsOutRange{child.Name} {childui.sort}");
        //
        //             childui.Dispose();
        //             // if (obj != null)
        //             // {
        //             //     SetPoolsObj(obj);
        //             //     m_CellInfos[i].obj = null;
        //             // }
        //         }
        //         else
        //         {
        //             if (list.Count <= 5)
        //             {
        //                 var ui = await list.CreateWithUITypeAsync(UIType.UISubPanel_MonopolyShopItem, 1, false);
        //             }
        //
        //             //Log.Debug($"IsOutRange{child.Name} {childui.sort}");
        //             // if (obj == null)
        //             // {
        //             //     //优先从 poolsObj中 取出 （poolsObj为空则返回 实例化的cell）
        //             //     GameObject cell = GetPoolsObj();
        //             //     cell.transform.localPosition = pos;
        //             //     cell.gameObject.name = i.ToString();
        //             //     m_CellInfos[i].obj = cell;
        //             //
        //             //     Func(m_FuncCallBackFunc, cell);
        //             // }
        //         }
        //     }
        // }

        //判断是否超出显示范围
        // protected bool IsOutRange(float pos, RectTransformComponent cellRect, int direction = 1)
        // {
        //     var KScrollView = GetFromReference(UIPanel_MonopolyShop.KScrollView);
        //     var scrollRect = KScrollView.GetXScrollRect();
        //     var rectTrans = KScrollView.GetRectTransform();
        //     var content = scrollRect.Content;
        //     var list = content.GetList();
        //
        //     Vector3 listP = content.GetRectTransform().AnchoredPosition();
        //
        //     if (direction == 1)
        //     {
        //         if (pos + listP.y > cellRect.Height() || pos + listP.y < -rectTrans.Height())
        //         {
        //             return true;
        //         }
        //     }
        //     else
        //     {
        //         if (pos + listP.x < -cellRect.Width() || pos + listP.x > rectTrans.Width())
        //         {
        //             return true;
        //         }
        //     }
        //
        //     return false;
        // }

        async public void OnExchangeResponse(object sender, WebMessageHandler.Execute e)
        {
            StringValue buyDice = new StringValue();
            buyDice.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            var reward = JiYuUIHelper.TurnStrReward2List(buyDice.Value);

            var ui = await UIHelper.CreateAsync(UIType.UICommon_Reward, reward);
            var KBtn_Close = ui.GetFromReference(UICommon_Reward.KBtn_Close);
            var KBg_Img = ui.GetFromReference(UICommon_Reward.KBg_Img);

            KBtn_Close.GetButton().OnClick.Add(() => { Refresh().Forget(); });
            KBg_Img.GetButton().OnClick.Add(() => { Refresh().Forget(); });


            Log.Debug($"OnExchangeResponse:{buyDice.Value}", Color.green);
            //Close();
        }

        private async UniTaskVoid Refresh()
        {
            var KText_Title = GetFromReference(UIPanel_MonopolyShop.KText_Title);
            var KScrollView = GetFromReference(UIPanel_MonopolyShop.KScrollView);
            string shopItemStr =
                $"{JiYuUIHelper.GetRewardTextIconName(shopItemReward)}x{JiYuUIHelper.GetRewardCount(shopItemReward)}";

            KText_Title.GetTextMeshPro().SetTMPText(shopItemStr);

            //var scrollRect = KScrollView.GetXScrollRect();
            var loopRect = KScrollView.GetLoopScrollRect<UISubPanel_MonopolyShopItem>();
            var content = loopRect.Content;
            var list = content.GetList();
            var activity = tbactivity.Get(activityId);
            var monopoly = tbmonopoly.Get(activity.link);
            int i = 0;
            foreach (var uia in list.Children)
            {
                i++;
                var ui = uia as UISubPanel_MonopolyShopItem;

                var monopolyShop = tbmonopoly_shop.DataList
                    .Where((a) => a.id == monopoly.monopolyShop && a.sort == ui.sort)
                    .ToList()[0];
    //            JiYuTweenHelper.SetEaseAlphaAndPosB2U(ui.GetFromReference(UISubPanel_MonopolyShopItem.KMid), 0,
    //60, cts.Token, 0.3F, true, true);
    //            await UniTask.Delay(2 * i, cancellationToken: cts.Token);
                var KRewardPos = ui.GetFromReference(UISubPanel_MonopolyShopItem.KRewardPos);
                var KText_Desc = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_Desc);
                var KText_Name = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_Name);

                var KCommon_CostBtn = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KCommon_CostBtn);
                var KText_Cost = KCommon_CostBtn.GetFromReference(UICommon_CostBtn.KText_Cost);
                var KCommon_Btn = KCommon_CostBtn.GetFromReference(UICommon_CostBtn.KCommon_Btn);
                var KText_Btn = KCommon_Btn.GetFromReference(UICommon_Btn.KText_Btn);
                var KImg_RedDotRight = KCommon_Btn.GetFromReference(UICommon_Btn.KImg_RedDotRight);

                // var KText_Cost = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_Cost);
                // var KBtn_Buy = ui.GetFromReference(UISubPanel_MonopolyShopItem.KBtn_Buy);
                // var KText_Buy = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_Buy);

                var KText_BuyFinish = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_BuyFinish);
                var KImg_TextName = ui.GetFromReference(UISubPanel_MonopolyShopItem.KImg_TextName);

                if (!ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.TryGetValue(activityId,
                        out var gameActivity))
                {
                    Close();
                }

                int curCount = monopolyShop.limit;
                if (gameActivity.MonopolyRecord.ExchangeRecord.TryGetValue(monopolyShop.sort, out var count))
                {
                    curCount = count;
                }

                var countStr =
                    UnityHelper.RichTextColor($"{curCount}/{monopolyShop.limit}",
                        UnityHelper.Color2HexRGB(curCount <= 0 ? Color.red : Color.white));
                var desc = $"{tblanguage.Get("monopoly_shop_bug_num_text").current}:{countStr}";

                KText_Desc.GetTextMeshPro().SetTMPText(desc);
                KText_BuyFinish.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_soldout").current);
                KText_Btn.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_exchange").current);

                // var reward = monopolyShop.reward[0];
                // var rewardui = await UIHelper.CreateAsync(ui, UIType.UICommon_RewardItem, reward,
                //     KRewardPos.GetRectTransform().Get());
                // rewardui.SetParent(ui, true);
                // rewardui.GetRectTransform().SetAnchoredPosition(Vector2.zero);

                //var name = JiYuUIHelper.GetRewardName(reward);
                //KText_Name.GetTextMeshPro().SetTMPText(name);
                var costCount = JiYuUIHelper.GetRewardCount(shopItemReward);

                var costNumStr =
                    UnityHelper.RichTextColor($"x{monopolyShop.costNum}",
                        UnityHelper.Color2HexRGB(monopolyShop.costNum < costCount ? Color.white : Color.red));
                // string costNumStr =
                //     $"{JiYuUIHelper.GetRewardTextIconName(shopItemReward)}x{JiYuUIHelper.GetRewardCount(shopItemReward)}";


                KText_Cost.GetTextMeshPro().SetTMPText(costNumStr);
                KCommon_Btn.SetActive(true);
                KText_BuyFinish.SetActive(false);
                KCommon_Btn.GetXButton().SetEnabled(monopolyShop.costNum < costCount);

                if (curCount <= 0)
                {
                    KCommon_Btn.SetActive(false);
                    KText_BuyFinish.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            //timerId = timerMgr.StartRepeatedTimer(updateInternal, this.Update);
            timerId = timerMgr.StartRepeatedTimer(1000, this.Update);
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }


        private void Update()
        {
            var KText_Time = GetFromReference(UIPanel_MonopolyShop.KText_Time);
            string cdStr = tblanguage.Get("active_countdown_text").current;
            long clientT = JiYuUIHelper.GetServerTimeStamp(true);
            if (!JiYuUIHelper.TryGetRemainingTime(clientT, endTime, out var timeStr))
            {
                Close();
            }

            cdStr += timeStr;

            KText_Time.GetTextMeshPro().SetTMPText(cdStr);
        }

        async public void ProvideData(UISubPanel_MonopolyShopItem ui, int index)
        {
            if (!ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.TryGetValue(activityId,
                    out var gameActivity))
            {
                Close();
            }

            var activity = tbactivity.Get(activityId);
            var monopoly = tbmonopoly.Get(activity.link);

            var monopolyShop = shopList[index];
            //return;
            ObjectHelper.Awake(ui, monopolyShop.sort);

            //var ui = await list.CreateWithUITypeAsync(UIType.UISubPanel_MonopolyShopItem, monopolyShop.sort, false);
            var KRewardPos = ui.GetFromReference(UISubPanel_MonopolyShopItem.KRewardPos);
            KRewardPos.GetRectTransform().SetAnchoredPositionX(80f);
            var KText_Desc = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_Desc);
            var KText_Name = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_Name);
            var KText_BuyFinish = ui.GetFromReference(UISubPanel_MonopolyShopItem.KText_BuyFinish);
            var KImg_TextName = ui.GetFromReference(UISubPanel_MonopolyShopItem.KImg_TextName);

            var KCommon_CostBtn = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KCommon_CostBtn);
            var KText_Cost = KCommon_CostBtn.GetFromReference(UICommon_CostBtn.KText_Cost);
            var KCommon_Btn = KCommon_CostBtn.GetFromReference(UICommon_CostBtn.KCommon_Btn);
            var KText_Btn = KCommon_Btn.GetFromReference(UICommon_Btn.KText_Btn);
            var KImg_RedDotRight = KCommon_Btn.GetFromReference(UICommon_Btn.KImg_RedDotRight);


            int curCount = monopolyShop.limit;
            if (gameActivity.MonopolyRecord.ExchangeRecord.TryGetValue(monopolyShop.sort, out var count))
            {
                curCount = count;
            }

            var countStr =
                UnityHelper.RichTextColor($"{curCount}/{monopolyShop.limit}", curCount <= 0 ? "FF0006" : "525F76");


            var desc = $"{tblanguage.Get("monopoly_shop_bug_num_text").current}:{countStr}";

            KText_Desc.GetTextMeshPro().SetTMPText(desc);
            KText_BuyFinish.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_soldout").current);
            KText_Btn.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_exchange").current);

            var reward = monopolyShop.reward[0];

            var rewardui = await UIHelper.CreateAsync(ui, UIType.UICommon_RewardItem, reward,
                KRewardPos.GetRectTransform().Get());
            var KImg_Pos = rewardui.GetFromReference(UICommon_RewardItem.KImg_Pos);
            KImg_Pos.SetActive(false);
            JiYuUIHelper.SetRewardOnClick(reward, rewardui);
            rewardui.SetParent(ui, true);
            rewardui.GetRectTransform().SetAnchoredPosition(Vector2.zero);
            var scale = 220f / 156f;
            rewardui.GetRectTransform().SetScale(new Vector2(scale, scale));
            var name = JiYuUIHelper.GetRewardName(reward);
            KText_Name.GetTextMeshPro().SetTMPText(name);
            var width = KText_Name.GetTextMeshPro().Get().preferredWidth;
            KImg_TextName.GetRectTransform().SetWidth(width + 30f);


            var costCount = JiYuUIHelper.GetRewardCount(shopItemReward);

            var costNumStr1 =
                UnityHelper.RichTextColor($"x{monopolyShop.costNum}",
                    UnityHelper.Color2HexRGB(monopolyShop.costNum < costCount ? Color.white : Color.red));

            string costNumStr = $"{JiYuUIHelper.GetRewardTextIconName(shopItemReward)}{costNumStr1}";

            KText_Cost.GetTextMeshPro().SetTMPText(costNumStr);
            KCommon_Btn.SetActive(true);
            KText_BuyFinish.SetActive(false);
            KCommon_Btn.GetXButton().SetEnabled(monopolyShop.costNum < costCount);

            // KText_Buy.GetTextMeshPro().SetAlpha(1);
            // KBtn_Buy.GetXImage().SetAlpha(1);
            KCommon_Btn.GetXImage().SetGrayed(false);
            if (monopolyShop.costNum > costCount)
            {
                // KText_Buy.GetTextMeshPro().SetAlpha(0.5f);
                // KBtn_Buy.GetXImage().SetAlpha(0.5f);
                KCommon_Btn.GetXImage().SetGrayed(true);
            }


            if (curCount <= 0)
            {
                KCommon_Btn.SetActive(false);
                KText_BuyFinish.SetActive(true);
            }


            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KCommon_Btn, () =>
            {
                var KCommon_ItemTips = GetFromReference(UIPanel_MonopolyShop.KCommon_ItemTips);
                KCommon_ItemTips.SetActive(false);

                var cost = shopItemReward;
                cost.z = monopolyShop.costNum;

                if (!JiYuUIHelper.TryReduceReward(cost))
                {
                    //UIHelper.CreateAsync(UIType.UILack, 2);
                    return;
                }

                if (gameActivity.MonopolyRecord.ExchangeRecord.ContainsKey(monopolyShop.sort))
                {
                    gameActivity.MonopolyRecord.ExchangeRecord[monopolyShop.sort]--;
                }
                else
                {
                    gameActivity.MonopolyRecord.ExchangeRecord.Add(monopolyShop.sort, monopolyShop.limit - 1);
                }


                NetWorkManager.Instance.SendMessage(CMD.MONOEXCHANGE, new IntValue
                {
                    Value = monopolyShop.sort
                });
            });


            if (index < 4)
            {
                JiYuTweenHelper.SetEaseAlphaAndPosB2U(ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KMid), 0, 50,
         cancellationToken: cts.Token,
         0.35f + 0.02f * index, true, true);
            }
        }

        protected override void OnClose()
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.MONOEXCHANGE, OnExchangeResponse);
            RemoveTimer();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;


namespace XFramework
{
    [UIEvent(UIType.UIPanel_Pass)]
    internal sealed class UIPanel_PassEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Pass;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Pass>();
        }
    }

    public partial class UIPanel_Pass : UI, IAwake<int>
    {
        #region properties

        private Tblanguage tblanguage;
        private Tbbattlepass tbbattlepass;
        private Tbtag_func tbtag_func;
        private Tbbattlepass_exp tbbattlepass_exp;
        private Tbbattlepass_reward tbbattlepass_reward;
        private Tbconstant tbconstant;
        private Tbuser_variable tbuser_variable;
        private Tbactivity tbactivity;

        private UI boxUI = null;
        private Dictionary<int, battlepass_reward> rewardDic = new Dictionary<int, battlepass_reward>();
        private List<UI> getUIList = new List<UI>();
        private UI unLockBtn = null;
        private CancellationTokenSource cts = new CancellationTokenSource();


        private int battlePassExp;
        public int activityId;
        private long endTime;
        public int battlePassId;


        private long timerId = 0;
        private bool isInit = false;
        private int tagFunc;
        private string m_RedDotName;

        #endregion

        public async void Initialize(int args)
        {
            await UnicornUIHelper.InitBlur(this);
            activityId = args;
            InitJson();
            var pass = tbactivity.Get(activityId);
            battlePassId = pass.link;
            var battlePass = tbbattlepass.Get(battlePassId);
            tagFunc = battlePass.tagFunc;
            InitRedDot();

            WebInit();

            NetWorkManager.Instance.SendMessage(CMDOld.QUERYPASS);

            this.GetFromReference(KBtn_Close_Tip).SetActive(false);
            this.GetFromReference(KBtn_Close_Tip).GetXButton().OnClick?.Add(() =>
            {
                CloseTip1();
                this.GetFromReference(KBtn_Close_Tip).SetActive(false);
            });
            this.GetFromReference(KBtn_Close).GetXButton().OnClick?.Add(() =>
            {
                CloseAllTip();
                Close();
            });
            UnicornTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(KImg_SeasonBg));
            await UniTask.Delay(200, cancellationToken: cts.Token);
            //UnicornTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(KImg_ActivationBg));

            UnicornTweenHelper.PlayUIImageSweepFX(this.GetFromReference(KImg_Activation), cts.Token, "A1DD01",
                UnicornTweenHelper.UIDir.Left);
        }

        void InitRedDot()
        {
            m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);
            for (int i = 0; i < 2; i++)
            {
                var itemStr = $"{m_RedDotName}|Pos{i}";
                RedDotManager.Instance.InsterNode(itemStr);
            }
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbbattlepass = ConfigManager.Instance.Tables.Tbbattlepass;
            tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            tbbattlepass_exp = ConfigManager.Instance.Tables.Tbbattlepass_exp;
            tbbattlepass_reward = ConfigManager.Instance.Tables.Tbbattlepass_reward;
            tbconstant = ConfigManager.Instance.Tables.Tbconstant;
            tbuser_variable = ConfigManager.Instance.Tables.Tbuser_variable;
            tbactivity = ConfigManager.Instance.Tables.Tbactivity;
        }

        private void WebInit()
        {
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.UNLOCKNEXTPASSLEVEL, OnUnLockNowResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.QUERYPASS, OnPassInfoResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.GETPASSREWARD, OnGetRewardResponse);
        }

        private void WebRemove()
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.UNLOCKNEXTPASSLEVEL, OnUnLockNowResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.QUERYPASS, OnPassInfoResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.GETPASSREWARD, OnGetRewardResponse);
        }

        private void CloseTip1()
        {
            UnicornUIHelper.DestoryItemTips();
            UIHelper.Remove(UIType.UICommon_PlainTextTip);
        }

        private void CloseTip2()
        {
            UIHelper.Remove(UIType.UICommon_Reward_Tip2);
            UIHelper.Remove(UIType.UICommon_PlainTextTip);
        }

        private void CloseTip3()
        {
            UIHelper.Remove(UIType.UICommon_PlainTextTip);
        }

        private void CloseAllTip()
        {
            UnicornUIHelper.DestoryItemTips();
            GetFromReference(KCommon_ItemTips).SetActive(false);
            CloseTip1();
            CloseTip2();
            CloseTip3();
        }

        private void CloseTipBtnSet1()
        {
            var KImg_SeasonBg = GetFromReference(UIPanel_Pass.KImg_SeasonBg);
            var KImg_Time = GetFromReference(UIPanel_Pass.KImg_Time);
            var KProgress_Bg = GetFromReference(UIPanel_Pass.KProgress_Bg);
            var KBg_Pass = GetFromReference(UIPanel_Pass.KBg_Pass);
            var KScrollView = GetFromReference(UIPanel_Pass.KScrollView);
            KImg_SeasonBg.GetXButton()?.RemoveAllListeners();
            KImg_Time.GetXButton()?.RemoveAllListeners();
            KProgress_Bg.GetXButton()?.RemoveAllListeners();
            KBg_Pass.GetXButton()?.RemoveAllListeners();
            KScrollView.GetXScrollRect()?.RemoveAllListeners();

            KImg_SeasonBg.GetXButton()?.OnClick?.Add(() => { CloseAllTip(); });
            KImg_Time.GetXButton()?.OnClick?.Add(() => { CloseAllTip(); });
            KProgress_Bg.GetXButton()?.OnClick?.Add(() => { CloseAllTip(); });
            KBg_Pass.GetXButton()?.OnClick?.Add(() => { CloseAllTip(); });

            KScrollView.GetXScrollRect().OnDrag.Add((a) => { CloseAllTip(); });
        }

        private void BtnInit()
        {
            var promptBtn = this.GetFromReference(KBtn_Prompt);
            var activationBtn = this.GetFromReference(KBtn_Activation);
            promptBtn.GetXButton().RemoveAllListeners();
            activationBtn.GetXButton().RemoveAllListeners();
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(promptBtn, async () =>
            {
                var KText_Des = GetFromReference(UIPanel_Pass.KText_Des);
                var desc = tblanguage.Get("battlepass_tips").current;
                GetFromReference(KCommon_ItemTips).SetActive(!GetFromReference(KCommon_ItemTips).GameObject.activeSelf);
                KText_Des.GetTextMeshPro().SetTMPText(desc);
                var height = KText_Des.GetTextMeshPro().Get().preferredHeight;
                var width = KText_Des.GetTextMeshPro().Get().preferredWidth;
                KText_Des.GetRectTransform().SetHeight(height);
                GetFromReference(KContent).GetRectTransform().SetHeight(height + 76 * 2);
            });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(activationBtn, async () =>
            {
                //CloseAllTip();
                var tokenUI = await UIHelper.CreateAsync(UIType.UISubPanel_Pass_Token,
                    ResourcesSingletonOld.Instance.gamePasses);
                var tokenBuyBtn = tokenUI.GetFromReference(UISubPanel_Pass_Token.KBtn_Buy);
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(tokenBuyBtn,
                    () =>
                    {
                        const string ShopNum = "C01";
                        UnicornUIHelper.SendBuyMessage(ShopNum, battlePassId);

                        //NetWorkManager.Instance.SendMessage(CMDOld.GETPASSTOKEN);
                        OnBuyBtnClick();
                    });
            });
        }

        private void OnBuyBtnClick()
        {
            JiYuEventManager.Instance.RegisterEvent("OnShopResponse", (a) =>
            {
                var shopNum = UnicornUIHelper.GetShopNum(a);
                if (shopNum == "C01")
                {
                    NetWorkManager.Instance.SendMessage(CMDOld.QUERYPASS);
                    UIHelper.Remove(UIType.UISubPanel_Pass_Token);
                }
            });
        }

        // private void OnGetTokenResponse(object sender, WebMessageHandlerOld.Execute e)
        // {
        //     StringValue stringValue = new StringValue();
        //     stringValue.MergeFrom(e.data);
        //
        //     if (e.data.IsEmpty)
        //     {
        //         return;
        //     }
        //
        //     Debug.Log(stringValue);
        //     UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("text_activate_success").current);
        //     NetWorkManager.Instance.SendMessage(CMDOld.QUERYPASS);
        //     UIHelper.Remove(UIType.UISubPanel_Pass_Token);
        // }


        private void TextInit()
        {
            this.GetFromReference(KText_Title).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbbattlepass.Get(battlePassId).name).current);
            this.GetFromReference(KText_Passport).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbtag_func.Get(tagFunc).name).current);
            this.GetFromReference(KText_Free).GetTextMeshPro().SetTMPText(tblanguage.Get("common_free_text").current);


            TextSet();
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
            var KText_Time = GetFromReference(UIPanel_Pass.KText_Time);
            string cdStr = tblanguage.Get("battlepass_season_time").current;
            long clientT = UnicornUIHelper.GetServerTimeStamp(true);
            if (!UnicornUIHelper.TryGetRemainingTime(clientT, endTime, out var timeStr))
            {
                Close();
            }

            cdStr += timeStr;
            KText_Time.GetTextMeshPro().SetTMPText(cdStr);
        }


        private void TextSet()
        {
            //int level = PassLevel(battlePassExp);
            var KText_Actiivation = this.GetFromReference(UIPanel_Pass.KText_Actiivation);
            var KBtn_Activation = this.GetFromReference(UIPanel_Pass.KBtn_Activation);
            if (ResourcesSingletonOld.Instance.gamePasses[0].Type == 0)
            {
                KText_Actiivation.GetTextMeshPro().SetTMPText(
                    tblanguage.Get("common_state_activate").current +
                    tblanguage.Get("battlepass_activate_name").current);
                KBtn_Activation.GetXButton().SetEnabled(true);
            }
            else
            {
                KText_Actiivation.GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("battlepass_activate_name").current);
                KBtn_Activation.GetXButton().SetEnabled(false);
            }

            var KImg_Progress = GetFromReference(UIPanel_Pass.KImg_Progress);
            var KText_Level = GetFromReference(UIPanel_Pass.KText_Level);

            var KText_Progress = GetFromReference(UIPanel_Pass.KText_Progress);
            foreach(var pass in ResourcesSingletonOld.Instance.gamePasses)
            {
                Log.Debug($"gamePass:id{pass.Level},exp:{pass.Exp}",Color.cyan);
            }
           
            UnicornUIHelper.ShowPassLevelExp(ResourcesSingletonOld.Instance.gamePasses[0].Exp, KImg_Progress, KText_Level,
                KText_Progress);
            // string expStr;
            // if (level < tbbattlepass_exp.DataList.Count)
            // {
            //     expStr = battlePassExp + "/" + tbbattlepass_exp.Get(level + 1).exp;
            // }
            // else
            // {
            //     expStr = battlePassExp + "/" + tbbattlepass_exp.Get(level).exp;
            // }
            //
            // this.GetFromReference(KText_Progress).GetTextMeshPro().SetTMPText(expStr);
            //var width = this.GetFromReference(KProgress_Bg).GetRectTransform().Width();

            //this.GetFromReference(KImg_Progress).GetRectTransform().SetOffsetWithRight(0);
            //ProgressSet(level);
        }


        private void ProgressSet(int level)
        {
            float progressProportion = 0;
            if (level < tbbattlepass_exp.DataList.Count)
            {
                float leftExp = tbbattlepass_exp.Get(level).exp;
                float rightExp = tbbattlepass_exp.Get(level + 1).exp;
                progressProportion = (battlePassExp - leftExp) / (rightExp - leftExp);
            }
            else
            {
                progressProportion = 1f;
            }

            float progressWidth = this.GetFromReference(KProgress_Bg).GetRectTransform().Width();
            this.GetFromReference(KImg_Progress).GetImage().SetFillAmount(progressProportion);
        }

        private int PassLevel(int exp)
        {
            int level = 1;
            foreach (var bpExp in tbbattlepass_exp.DataList)
            {
                if (battlePassExp >= bpExp.exp)
                {
                    level = bpExp.id;
                }
                else
                {
                    break;
                }
            }

            return level;
        }

        private async UniTaskVoid CreateItem1(CancellationToken cct)
        {
            var KScrollView = GetFromReference(UIPanel_Pass.KScrollView);
            var sr = KScrollView.GetXScrollRect();

            var itemList = sr.Content.GetList();
            itemList.Clear();
            getUIList.Clear();
            Dictionary<UI, int> levelUIDic = new Dictionary<UI, int>();

            int level = PassLevel(battlePassExp);

            int itemCount = tbbattlepass_exp.DataList.Count;
            float itemListH = itemCount * 267 + (itemCount - 1) * 36;
            // var posTransform = this.GetFromReference(KPos_Item).GetRectTransform();
            // posTransform.SetHeight(itemListH);
            // var contentTransform = this.GetFromReference(KContent).GetRectTransform();
            // contentTransform.SetHeight(itemListH + 201 + 681 + 36 * 2);
            // contentTransform.SetOffsetWithLeft(0);
            // contentTransform.SetOffsetWithRight(0);
            unLockBtn = null;
            for (int i = 0; i < itemCount; i++)
            {
                var itemUI = await itemList.CreateWithUITypeAsync(UIType.UISubPanel_Pass_Item, false, cct);
                ItemSet(itemUI, i + 1, level);
                levelUIDic.Add(itemUI, i);
            }

            SetUnLockNow();
            boxUI = await itemList.CreateWithUITypeAsync(UIType.UISubPanel_Pass_Box, false, cct);
            levelUIDic.Add(boxUI, itemCount);
            itemList.Sort((UI ui1, UI ui2) =>
            {
                if (levelUIDic[ui1] < levelUIDic[ui2])
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });

            BoxSet();
            //UnicornUIHelper.ForceRefreshLayout(this.GetFromReference(KPos_Item));
        }

        private void AllItemAndBoxSet()
        {
            if (this.cts != null)
            {
                this.cts.Cancel();
                this.cts.Dispose();
            }

            this.cts = new CancellationTokenSource();
            TextSet();
            CreateItem1(cts.Token).Forget();
        }

        private void ItemSet(UI itemui, int thisLevel, int level)
        {
            UnicornUIHelper.SetRewardOnClick(rewardDic[thisLevel].reward1[0],
                itemui.GetFromReference(UISubPanel_Pass_Item.KReward_Left));
            // itemui.GetFromReference(UISubPanel_Pass_Item.KReward_Left), () =>
            // {
            //     CloseTip2();
            //     if (level >= thisLevel)
            //     {
            //         if (gamePassList[thisLevel - 1].ItemStatus == 0)
            //         {
            //             CloseAllTip();
            //             IntValueList intValueList = new IntValueList();
            //             intValueList.Values.Add(thisLevel);
            //             intValueList.Values.Add(2);
            //             NetWorkManager.Instance.SendMessage(14, 3, intValueList);
            //         }
            //     }
            // });
            UnicornUIHelper.SetRewardOnClick(rewardDic[thisLevel].reward2[0],
                itemui.GetFromReference(UISubPanel_Pass_Item.KReward_Right));
            // {
            //     CloseTip2();
            //     if (level >= thisLevel)
            //     {
            //         if (gamePassList[0].Type == 1)
            //         {
            //             if (gamePassList[thisLevel - 1].VipStatus == 0)
            //             {
            //                 CloseAllTip();
            //                 IntValueList intValueList = new IntValueList();
            //                 intValueList.Values.Add(thisLevel);
            //                 intValueList.Values.Add(1);
            //                 NetWorkManager.Instance.SendMessage(14, 3, intValueList);
            //             }
            //         }
            //     }
            // });

            if (thisLevel == 1)
            {
                itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_Start).SetActive(false);
                itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_NoLock_Start).SetActive(false);
            }
            else
            {
                if (level >= thisLevel)
                {
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_Start).SetActive(true);
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Level_Lock).GetXImage().SetGrayed(false);
                }
                else
                {
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Level_Lock).GetXImage().SetGrayed(true);
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_Start).SetActive(false);
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_End).SetActive(false);
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_NoLock_Start).SetActive(true);
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_NoLock_End).SetActive(true);

                    //通行证最大等级
                    if (thisLevel == 30)
                    {
                        itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_NoLock_End).SetActive(false);
                        itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_End).SetActive(false);
                    }
                }
            }

            if (thisLevel == level + 1)
            {
                itemui.GetFromReference(UISubPanel_Pass_Item.KBtn_UnlockNow).SetActive(true);
                unLockBtn = itemui.GetFromReference(UISubPanel_Pass_Item.KBtn_UnlockNow);
            }
            else
            {
                itemui.GetFromReference(UISubPanel_Pass_Item.KBtn_UnlockNow).SetActive(false);
            }

            if (level >= thisLevel)
            {
                //itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Level_Lock).SetActive(false);
                if (ResourcesSingletonOld.Instance.gamePasses[thisLevel - 1].ItemStatus == 0)
                {
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Received_Left).SetActive(false);
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Left).SetActive(true);
                    getUIList.Add(itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Left));
                }
                else
                {
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Received_Left).SetActive(true);
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Left).SetActive(false);
                }

                itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Lock_Left).SetActive(false);
                if (ResourcesSingletonOld.Instance.gamePasses[0].Type == 0)
                {
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Lock_Right).SetActive(true);
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Received_Right).SetActive(false);
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Right).SetActive(false);
                }
                else
                {
                    itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Lock_Right).SetActive(false);
                    if (ResourcesSingletonOld.Instance.gamePasses[thisLevel - 1].VipStatus == 0)
                    {
                        itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Received_Right).SetActive(false);
                        itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Right).SetActive(true);
                        getUIList.Add(itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Right));
                    }
                    else
                    {
                        itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Received_Right).SetActive(true);
                        itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Right).SetActive(false);
                    }
                }
            }
            else
            {
                itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Received_Left).SetActive(false);
                itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Received_Right).SetActive(false);
                itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Level_Lock).SetActive(true);
                itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Lock_Left).SetActive(true);
                itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Lock_Right).SetActive(true);
                itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Left).SetActive(false);
                itemui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Right).SetActive(false);
            }

            itemui.GetFromReference(UISubPanel_Pass_Item.KText_Level).GetTextMeshPro().SetTMPText(thisLevel.ToString());
            itemui.GetFromReference(UISubPanel_Pass_Item.KText_Remind_Left).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_gain").current);
            itemui.GetFromReference(UISubPanel_Pass_Item.KText_Remind_Right).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_gain").current);
            itemui.GetFromReference(UISubPanel_Pass_Item.KText_Received_Left).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_gained").current);
            itemui.GetFromReference(UISubPanel_Pass_Item.KText_Received_Right).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_gained").current);


            int battlepass_exp_diamond_ratio = tbconstant.Get("battlepass_exp_diamond_ratio").constantValue;
            var cost = new Vector3(2, 0, battlepass_exp_diamond_ratio * 20);
            string costStr =
                $"{UnicornUIHelper.GetRewardTextIconName(cost)}{(int)cost.z}";

            itemui.GetFromReference(UISubPanel_Pass_Item.KText_UnlockNow).GetTextMeshPro()
                .SetTMPText(costStr);

            UnicornUIHelper.SetRewardIconAndCountText(rewardDic[thisLevel].reward1[0],
                itemui.GetFromReference(UISubPanel_Pass_Item.KReward_Left));
            UnicornUIHelper.SetRewardIconAndCountText(rewardDic[thisLevel].reward2[0],
                itemui.GetFromReference(UISubPanel_Pass_Item.KReward_Right));
        }

        private void OnGetRewardResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            //WebMessageHandlerOld.Instance.RemoveHandler(14, 3, OnGetRewardResponse);
            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);
            Log.Debug($"OnGetRewardResponse:{stringValueList.Values}", Color.green);
            if (e.data.IsEmpty)
            {
                return;
            }

            var rewards = UnicornUIHelper.TurnStrReward2List(stringValueList.Values);
            UnicornUIHelper.AddReward(rewards, true);

            NetWorkManager.Instance.SendMessage(CMDOld.QUERYPASS);
        }

        private void BoxSet()
        {
            bool canGetBox = false;
            boxUI.GetFromReference(UISubPanel_Pass_Box.KText_Title).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("text_reward_more").current);
            boxUI.GetFromReference(UISubPanel_Pass_Box.KText_Introduce).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("battlepass_reward_more_tips").current);

            int moreOnceExp = tbconstant.Get("battlepass_reward_more_once_exp").constantValue;
            int timeMax = tbconstant.Get("battlepass_reward_more_times_upperlimitp").constantValue;
            int normalExpMax = tbbattlepass_exp.DataList[tbbattlepass_exp.DataList.Count - 1].exp;
            float progressHelp = 1.0f;
            int leftExpHelp = 0;
            int receiveCount = ResourcesSingletonOld.Instance.gamePasses[ResourcesSingletonOld.Instance.gamePasses.Count - 1]
                .ReceiveCount;

            int rightExpHelp = tbconstant.Get("battlepass_reward_more_once_exp").constantValue;
            if (battlePassExp < normalExpMax)
            {
                leftExpHelp = 0;
                progressHelp = 0;
            }
            else
            {
                int moreExp = battlePassExp - normalExpMax;
                //if (moreExp - receiveCount * moreOnceExp >= timeMax * moreOnceExp)
                if (receiveCount >= timeMax)
                {
                    progressHelp = 1.0f;
                }
                else
                {
                    if (moreExp - receiveCount * moreOnceExp < moreOnceExp)
                    {
                        progressHelp = (float)((moreExp - receiveCount * moreOnceExp) % moreOnceExp) /
                                       (float)moreOnceExp;
                    }
                    else
                    {
                        progressHelp = 1.0f;
                        canGetBox = true;
                    }
                }

                leftExpHelp = moreExp - receiveCount * moreOnceExp;
            }

            boxUI.GetFromReference(UISubPanel_Pass_Box.KText_Progress).GetTextMeshPro()
                .SetTMPText(leftExpHelp.ToString() + "/" + rightExpHelp.ToString());
            boxUI.GetFromReference(UISubPanel_Pass_Box.KImg_Progress).GetRectTransform().SetOffsetWithRight(
                -boxUI.GetFromReference(UISubPanel_Pass_Box.KProgress_Bg).GetRectTransform().Width() *
                (1 - progressHelp));

            var boxBtn = boxUI.GetFromReference(UISubPanel_Pass_Box.KBtn_Box);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(boxBtn, async () =>
            {
                if (!canGetBox)
                {
                    CloseTip1();
                    var tipUI = await UIHelper.CreateAsync(UIType.UICommon_Reward_Tip2);
                    List<Vector3> vector3s = new List<Vector3>();
                    var version = tbactivity.Get(ResourcesSingletonOld.Instance.gamePasses[0].Version).link;
                    if (ResourcesSingletonOld.Instance.gamePasses[0].Type == 0)
                    {
                        //free
                        vector3s = tbbattlepass.Get(version).rewardMore1;
                    }
                    else
                    {
                        //vip
                        vector3s = tbbattlepass.Get(version).rewardMore2;
                    }

                    int reCount = vector3s.Count;
                    tipUI.GetRectTransform().SetWidth(reCount * 156);
                    UnicornUIHelper.SetTipPos(boxUI.GetFromReference(UISubPanel_Pass_Box.KPos_Btn)
                        , tipUI, UICommon_Reward_Tip2.KBg, UICommon_Reward_Tip2.KImg_PolygonDown,
                        UICommon_Reward_Tip2.KImg_PolygonUp);
                    var bgList = tipUI.GetFromReference(UICommon_Reward_Tip2.KBg).GetList();
                    bgList.Clear();
                    for (int i = 0; i < reCount; i++)
                    {
                        var reui = await bgList.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, vector3s[i],
                            false);
                        reui.GetRectTransform().SetScale2(0.85f);
                        UnicornUIHelper.SetRewardOnClick(vector3s[i], reui);
                        //() => { this.GetFromReference(KBtn_Close_Tip).SetActive(true); });
                    }

                    UnicornUIHelper.ForceRefreshLayout(tipUI.GetFromReference(UICommon_Reward_Tip2.KBg));
                }
                else
                {
                    int freeOrVip = 2;
                    int moreLevel = 0;
                    moreLevel = (battlePassExp - normalExpMax) / moreOnceExp;
                    if (moreLevel > timeMax)
                    {
                        moreLevel = timeMax;
                    }

                    if (ResourcesSingletonOld.Instance.gamePasses[0].Type == 0)
                    {
                        //free
                        freeOrVip = 2;
                    }
                    else
                    {
                        //vip
                        freeOrVip = 1;
                    }

                    IntValueList intValueList = new IntValueList();
                    intValueList.Values.Add(moreLevel +
                                            tbbattlepass_exp.DataList[tbbattlepass_exp.DataList.Count - 1].id);
                    intValueList.Values.Add(freeOrVip);
                    NetWorkManager.Instance.SendMessage(CMDOld.GETPASSREWARD, intValueList);
                }
            });
        }

        private void UIUpAndDownIn1Sec(UI input)
        {
            input.GetRectTransform().DoAnchoredPositionY(-10, 0.5f).AddOnCompleted(() =>
            {
                input.GetRectTransform().DoAnchoredPositionY(10, 0.5f);
            });
        }

        private void SetUnLockNow()
        {
            int level = PassLevel(battlePassExp);
            if (level < tbbattlepass_exp.DataList.Count)
            {
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(unLockBtn, () =>
                {
                    CloseTip1();
                    var confirmUI = UIHelper.Create(UIType.UICommon_Sec_Confirm);
                    int level = PassLevel(battlePassExp);
                    int tokenExp = tbbattlepass_exp.Get(level + 1).exp - battlePassExp;
                    string confirmStr = tblanguage.Get("text_battlepass_exp_confirm").current;
                    int battlepass_exp_diamond_ratio = tbconstant.Get("battlepass_exp_diamond_ratio").constantValue;
                    var str0 = confirmStr.Replace("{0}", "<sprite=0>" + (tokenExp * battlepass_exp_diamond_ratio).ToString());
                    var str1 = str0.Replace("{1}",
                        tokenExp.ToString() + tblanguage.Get(tbuser_variable.Get(14).name).current);
                    confirmUI.GetFromReference(UICommon_Sec_Confirm.KText).GetTextMeshPro().SetTMPText(str1);
                    var buyBtn = confirmUI.GetFromReference(UICommon_Sec_Confirm.KBtn_Buy);
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(buyBtn, () =>
                    {
                        NetWorkManager.Instance.SendMessage(CMDOld.UNLOCKNEXTPASSLEVEL);
                        confirmUI.Dispose();
                    });
                });
            }
        }

        private void OnUnLockNowResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var unLockStr = new StringValue();
            unLockStr.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                return;
            }

            if (unLockStr.Value == "success")
            {
                // int level = PassLevel(battlePassExp);
                // int changeGems = (tbbattlepass_exp.Get(level + 1).exp - battlePassExp) * 2;
                NetWorkManager.Instance.SendMessage(CMDOld.QUERYPASS);
                NetWorkManager.Instance.SendMessage(CMDOld.PASSEXP);
                //ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin -= changeGems;
                //ResourcesSingletonOld.Instance.UpdateResourceUI();
            }
            else
            {
                //defeat
            }
        }

        private void OnPassInfoResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var passList = new ByteValueList();
            passList.MergeFrom(e.data);
            Log.Debug($"OnPassInfoResponse {passList}", Color.green);
            if (e.data.IsEmpty)
            {
                return;
            }

            ResourcesSingletonOld.Instance.gamePasses.Clear();
            foreach (var pass in passList.Values)
            {
                GamePass gamePass = new GamePass();
                gamePass.MergeFrom(pass);
                ResourcesSingletonOld.Instance.gamePasses.Add(gamePass);
                Log.Debug($"OnPass {gamePass}", Color.green);
            }

            battlePassExp = ResourcesSingletonOld.Instance.gamePasses[ResourcesSingletonOld.Instance.gamePasses.Count - 1]
                .Exp;

            endTime = ResourcesSingletonOld.Instance.gamePasses[0].EndTime;
            if (!isInit)
            {
                isInit = true;
                InitNode();
                StartTimer();
            }
            else
            {
                InitNode();
            }

            //AllItemAndBoxSet();
            // if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui))
            // {
            //     var uis = ui as UIPanel_Main;
            //     uis.PassUpdate();
            // }
        }

        private void InitNode()
        {
            TextInit();
            BtnInit();
            CloseTipBtnSet1();
            CreateItem().Forget();
        }

        private async UniTaskVoid CreateItem()
        {
            var KScrollView = GetFromReference(UIPanel_Pass.KScrollView);
            var scrollRect = KScrollView.GetXScrollRect();
            var content = scrollRect.Content;
            var list = content.GetList();
            list.Clear();
            var rewardList = tbbattlepass_reward.DataList.Where(a => a.id == battlePassId).ToList();
            var upStr = $"{m_RedDotName}|Pos{0}";
            int redDotNum = 0;
            int curMaxLevel = ResourcesSingletonOld.Instance.gamePasses.OrderByDescending(x => x.Level).First().Level;

            foreach (var battlepass in rewardList)
            {
                var ui = await list.CreateWithUITypeAsync(UIType.UISubPanel_Pass_Item, battlepass.level, false);

                var KText_Level = ui.GetFromReference(UISubPanel_Pass_Item.KText_Level);
                var KBtn_Left = ui.GetFromReference(UISubPanel_Pass_Item.KBtn_Left);
                var KImg_Received_Left = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Received_Left);
                var KText_Received_Left = ui.GetFromReference(UISubPanel_Pass_Item.KText_Received_Left);
                var KImg_Remind_Left = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Left);
                var KText_Remind_Left = ui.GetFromReference(UISubPanel_Pass_Item.KText_Remind_Left);
                var KBtn_Right = ui.GetFromReference(UISubPanel_Pass_Item.KBtn_Right);
                var KImg_Received_Right = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Received_Right);
                var KText_Received_Right = ui.GetFromReference(UISubPanel_Pass_Item.KText_Received_Right);
                var KImg_Remind_Right = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Remind_Right);
                var KText_Remind_Right = ui.GetFromReference(UISubPanel_Pass_Item.KText_Remind_Right);
                var KImg_Level_Lock = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Level_Lock);
                var KBtn_UnlockNow = ui.GetFromReference(UISubPanel_Pass_Item.KBtn_UnlockNow);
                var KText_UnlockNow = ui.GetFromReference(UISubPanel_Pass_Item.KText_UnlockNow);
                var KImg_Lock_Left = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Lock_Left);
                var KImg_Lock_Right = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Lock_Right);
                var KReward_Left = ui.GetFromReference(UISubPanel_Pass_Item.KReward_Left);
                var KReward_Right = ui.GetFromReference(UISubPanel_Pass_Item.KReward_Right);
                var KImg_Link_NoLock_End = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_NoLock_End);
                var KImg_Link_NoLock_Start = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_NoLock_Start);
                var KImg_Link_End = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_End);
                var KImg_Link_Start = ui.GetFromReference(UISubPanel_Pass_Item.KImg_Link_Start);

                KText_Level.GetTextMeshPro().SetTMPText(battlepass.level.ToString());
                KText_Remind_Left.GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gain").current);
                KText_Remind_Right.GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gain").current);
                KText_Received_Left.GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gained").current);
                KText_Received_Right.GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gained").current);
                int battlepass_exp_diamond_ratio = tbconstant.Get("battlepass_exp_diamond_ratio").constantValue;

                var exp = ResourcesSingletonOld.Instance.gamePassExp;
                var explist = tbbattlepass_exp.DataList;
                var curLevel = -1;
                foreach (var item in explist)
                {
                    if (item.exp > exp)
                    {
                        curLevel = item.id;
                        break;
                    }
                }

                if (curLevel == -1)
                {
                    curLevel = tbbattlepass_exp.DataList[tbbattlepass_exp.DataList.Count - 1].id;
                }

                var cost = new Vector3(2, 0, battlepass_exp_diamond_ratio * (tbbattlepass_exp.Get(curLevel).exp - exp));
                string costStr =
                    $"{UnicornUIHelper.GetRewardTextIconName(cost)}{(int)cost.z}";

                KText_UnlockNow.GetTextMeshPro()
                    .SetTMPText(costStr);
                var rewardL = battlepass.reward1[0];
                var rewardR = battlepass.reward2[0];
                UnicornUIHelper.SetRewardIconAndCountText(rewardL, KReward_Left);
                UnicornUIHelper.SetRewardIconAndCountText(rewardR, KReward_Right);
                var gamePass = ResourcesSingletonOld.Instance.gamePasses.Where(a => a.Level == battlepass.level)
                    .FirstOrDefault();


                KBtn_UnlockNow.SetActive(false);
                if (battlepass.level - 1 == ResourcesSingletonOld.Instance.gamePasses.Count)
                {
                    KBtn_UnlockNow.SetActive(true);
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_UnlockNow, async () =>
                    {
                        UnicornUIHelper.DestoryItemTips();
                        var langStr = tblanguage.Get($"battlepass_unlocknow_tips").current;

                        int battlepass_exp_diamond_ratio = tbconstant.Get("battlepass_exp_diamond_ratio").constantValue;

                        string str = string.Format(langStr, cost.z);


                        //var cost = new Vector3(2, 0, battlepass_exp_diamond_ratio * lackNum);
                        string costStr =
                            $"{UnicornUIHelper.GetRewardTextIconName(cost)}{(int)cost.z}";
                        //costStr += $" {tblanguage.Get($"common_state_buy").current}";


                        // var name = tblanguage.Get(tbuser_variable.Get(14).name).current;
                        // var get = UnityHelper.RichTextColor($"{lackNum}{name}", "8CD15A");


                        var ui = await UIHelper.CreateAsync(UIType.UICommon_Sec_Confirm);
                        var KBtn_Buy = ui.GetFromReference(UICommon_Sec_Confirm.KBtn_Buy);

                        var KBtn_Return = ui.GetFromReference(UICommon_Sec_Confirm.KBtn_Return);
                        var KText_Return = ui.GetFromReference(UICommon_Sec_Confirm.KText_Return);
                        var KText_Buy = ui.GetFromReference(UICommon_Sec_Confirm.KText_Buy);
                        var KText = ui.GetFromReference(UICommon_Sec_Confirm.KText);
                        KText_Buy.GetTextMeshPro().SetTMPText(costStr);
                        KText_Return.GetTextMeshPro().SetTMPText(tblanguage.Get($"common_state_cancel").current);
                        KText.GetTextMeshPro().SetTMPText(str);
                        UnicornTweenHelper.JiYuOnClickNoAnim(KBtn_Return, () => { ui.Dispose(); });
                        UnicornTweenHelper.JiYuOnClickNoAnim(KBtn_Buy,
                            () =>
                            {
                                if (!UnicornUIHelper.TryReduceReward(cost))
                                {
                                    return;
                                }

                                NetWorkManager.Instance.SendMessage(CMDOld.UNLOCKNEXTPASSLEVEL);
                                ui.Dispose();
                            });
                    });
                }


                var KImg_Link_StartImage = KImg_Link_Start.GetXImage();
                var KImg_Link_EndImage = KImg_Link_End.GetXImage();
                var KImg_Level_LockImage = KImg_Level_Lock.GetXImage();

                if (gamePass != null)
                {
                    var KBtn_ItemL = KReward_Left.GetFromReference(UICommon_RewardItem.KBtn_Item);
                    if (gamePass.ItemStatus == 0)
                    {
                        redDotNum++;
                        UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_ItemL, () =>
                        {
                            IntValueList intValueList = new IntValueList();
                            intValueList.Values.Add(battlepass.level);
                            //1：vip 2：免费
                            intValueList.Values.Add(2);

                            NetWorkManager.Instance.SendMessage(CMDOld.GETPASSREWARD, intValueList);
                        });
                    }
                    else
                    {
                        UnicornUIHelper.SetRewardOnClick(rewardL, KReward_Left);
                    }

                    var KBtn_ItemR = KReward_Right.GetFromReference(UICommon_RewardItem.KBtn_Item);
                    if (gamePass.Type == 1 && gamePass.VipStatus == 0)
                    {
                        redDotNum++;
                        UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_ItemR, () =>
                        {
                            IntValueList intValueList = new IntValueList();
                            intValueList.Values.Add(battlepass.level);
                            //1：vip 2：免费
                            intValueList.Values.Add(1);

                            NetWorkManager.Instance.SendMessage(CMDOld.GETPASSREWARD, intValueList);
                        });
                    }
                    else
                    {
                        UnicornUIHelper.SetRewardOnClick(rewardR, KReward_Right);
                    }


                    KImg_Remind_Left.SetActive(gamePass.ItemStatus == 0);
                    KImg_Remind_Right.SetActive(gamePass.Type == 1 && gamePass.VipStatus == 0);
                    KImg_Lock_Left.SetActive(false);
                    KImg_Lock_Right.SetActive(gamePass.Type == 0);
                    KImg_Received_Left.SetActive(gamePass.ItemStatus == 1);
                    KImg_Received_Right.SetActive(gamePass.VipStatus == 1);
                    KImg_Link_StartImage.SetGrayed(false);
                    KImg_Link_EndImage.SetGrayed(false);
                    KImg_Level_LockImage.SetGrayed(false);
                    if (curMaxLevel == gamePass.Level)
                    {
                        KImg_Link_EndImage.SetGrayed(true);
                    }
                }
                else
                {
                    UnicornUIHelper.SetRewardOnClick(rewardL, KReward_Left);
                    UnicornUIHelper.SetRewardOnClick(rewardR, KReward_Right);
                    KImg_Remind_Left.SetActive(false);
                    KImg_Remind_Right.SetActive(false);
                    KImg_Lock_Left.SetActive(true);
                    KImg_Lock_Right.SetActive(true);
                    KImg_Received_Left.SetActive(false);
                    KImg_Received_Right.SetActive(false);

                    KImg_Link_StartImage.SetGrayed(true);
                    KImg_Link_EndImage.SetGrayed(true);
                    KImg_Level_LockImage.SetGrayed(true);
                }


                KImg_Link_Start.SetActive(true);
                KImg_Link_End.SetActive(true);
                if (battlepass.level == 1)
                {
                    KImg_Link_Start.SetActive(false);
                }

                if (battlepass.level ==
                    tbbattlepass_reward.DataList[^1].level)
                {
                    KImg_Link_End.SetActive(false);
                }
            }

            RedDotManager.Instance.SetRedPointCnt(upStr, redDotNum);
            list.Sort((a, b) =>
            {
                var uia = a as UISubPanel_Pass_Item;
                var uib = b as UISubPanel_Pass_Item;
                return uia.level.CompareTo(uib.level);
            });

            var boxui = await list.CreateWithUITypeAsync(UIType.UISubPanel_Pass_Box, false);
            var KImg_Bg = boxui.GetFromReference(UISubPanel_Pass_Box.KImg_Bg);
            var KBtn_Box = boxui.GetFromReference(UISubPanel_Pass_Box.KBtn_Box);
            var KProgress_Bg = boxui.GetFromReference(UISubPanel_Pass_Box.KProgress_Bg);
            var KImg_Progress = boxui.GetFromReference(UISubPanel_Pass_Box.KImg_Progress);
            var KText_Progress = boxui.GetFromReference(UISubPanel_Pass_Box.KText_Progress);
            var KImg_Level_Bg = boxui.GetFromReference(UISubPanel_Pass_Box.KImg_Level_Bg);
            var KText_Introduce = boxui.GetFromReference(UISubPanel_Pass_Box.KText_Introduce);
            var KImg_Title = boxui.GetFromReference(UISubPanel_Pass_Box.KImg_Title);
            var KText_Title = boxui.GetFromReference(UISubPanel_Pass_Box.KText_Title);
            var KPos_Btn = boxui.GetFromReference(UISubPanel_Pass_Box.KPos_Btn);
            //text_reward_more
            KText_Title.GetTextMeshPro().SetTMPText(tblanguage.Get($"text_reward_more").current);
            KText_Introduce.GetTextMeshPro().SetTMPText(tblanguage.Get($"battlepass_reward_more_tips").current);

            int battlepass_reward_more_once_exp = tbconstant.Get("battlepass_reward_more_once_exp").constantValue;

            // int totalOverExp = battlePassExp - tbbattlepass_exp.DataList[^1].exp;
            // totalOverExp = Mathf.Max(0, totalOverExp);
            int receiveCount = ResourcesSingletonOld.Instance.gamePasses[0].ReceiveCount;
            int curOverExp = battlePassExp - tbbattlepass_exp.DataList[^1].exp -
                             receiveCount * battlepass_reward_more_once_exp;
            curOverExp = Mathf.Max(0, curOverExp);
            KText_Progress.GetTextMeshPro().SetTMPText($"{curOverExp}/{battlepass_reward_more_once_exp}");
            //constant表battlepass_reward_more_once_exp
            UnicornUIHelper.ForceRefreshLayout(content);
        }


        protected override void OnClose()
        {
            WebRemove();
            RemoveTimer();
            base.OnClose();
        }
    }
}
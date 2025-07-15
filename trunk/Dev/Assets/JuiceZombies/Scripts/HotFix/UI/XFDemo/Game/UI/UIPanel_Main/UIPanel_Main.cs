//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Spine.Unity;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Main)]
    internal sealed class UIPanel_MainEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Main;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Main>();
        }
    }

    public partial class UIPanel_Main : UI, IAwake
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
        private Tbtask_group tbtask_group;
        private Tbbattlepass_exp tbbattlepass_exp;

        private Tbguide tbguide;

        //private Tbt tbdays_sign;
        public int tagId = 3;
        public XScrollRectComponent scrollRect;

        private float currtime;
        private long timerId;

        private long timerId0;

        //private bool isBoxPanelDisplay;
        private bool isBlock;
        private int curChapterID = 1;

        private float enableTime = 3f;
        private float curEnableTime;

        private float internalTime;
        private float curInternalTime;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private float curWidth;

        private float defaultWidth = -Screen.width * 2f;

        /// <summary>
        /// 0:idle
        /// 
        /// </summary>
        private int playerState = 0;

        const float VerticalTextGap = 40f;
        const float HorizentalTextGap = 60f;

        public int panelId = 311;
        private int guideId;
        private List<guide> _guides = new List<guide>();

        public void Initialize()
        {
            InitJson();
            InitNode();
            StartTimer();
        }

        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(1000, this.Update);
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
            tbbattlepass_exp = ConfigManager.Instance.Tables.Tbbattlepass_exp;

            tbdays_challenge = ConfigManager.Instance.Tables.Tbdays_challenge;
            tbbattlepass = ConfigManager.Instance.Tables.Tbbattlepass;
            tbpiggy_bank = ConfigManager.Instance.Tables.Tbpiggy_bank;
            tbdays_sign = ConfigManager.Instance.Tables.Tbdays_sign;
            tbenergy_shop = ConfigManager.Instance.Tables.Tbenergy_shop;
            tbmonopoly = ConfigManager.Instance.Tables.Tbmonopoly;
            tbtask_group = ConfigManager.Instance.Tables.Tbtask_group;
            tbguide = ConfigManager.Instance.Tables.Tbguide;
        }

        public void RefreshText()
        {
            var KText_StartTitle = GetFromReference(UIPanel_Main.KText_StartTitle);
            // var KTaskItemPos = GetFromReference(UIPanel_Main.KTaskItemPos);
            KText_StartTitle.GetTextMeshPro().SetTMPText(tblanguage.Get("func_3801_name").current);
            // UpdateLevelChooseItem(curChapterID);
        }

        public void PassUpdate(bool isFirst = false)
        {
            var KPass = this.GetFromReference(UIPanel_Main.KPass);

            KPass.SetActive(ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(3201));

            //this.GetFromReference(KImg_PassIcon).GetImage().SetSpriteAsync(tbtag_func.Get(3201).icon, false);
            this.GetFromReference(KText_PassTitle).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbtag_func.Get(3201).name).current);

            // Log.Debug($"gamePassStart {ResourcesSingleton.Instance.gamePassStart}", Color.green);
            //
            // if (ResourcesSingleton.Instance.gamePassStart)
            // {
            //     KPass.SetActive(true);
            //     var gamePassList = ResourcesSingleton.Instance.gamePasses;
            //     int passEXP = gamePassList[gamePassList.Count - 1].Exp;
            //
            //     int passLevel = 1;
            //     foreach (var bpExp in ConfigManager.Instance.Tables.Tbbattlepass_exp.DataList)
            //     {
            //         if (passEXP >= bpExp.exp)
            //         {
            //             passLevel = bpExp.id;
            //         }
            //         else
            //         {
            //             break;
            //         }
            //     }
            //
            //     this.GetFromReference(KText_PassLevel).GetTextMeshPro().SetTMPText(passLevel.ToString());
            //
            //     string expStr;
            //     float progressProportion = 0;
            //     if (passLevel < ConfigManager.Instance.Tables.Tbbattlepass_exp.DataList.Count)
            //     {
            //         expStr = passEXP + "/" + ConfigManager.Instance.Tables.Tbbattlepass_exp.Get(passLevel + 1).exp;
            //         float leftExp = ConfigManager.Instance.Tables.Tbbattlepass_exp.Get(passLevel).exp;
            //         float rightExp = ConfigManager.Instance.Tables.Tbbattlepass_exp.Get(passLevel + 1).exp;
            //         progressProportion = (passEXP - leftExp) / (rightExp - leftExp);
            //     }
            //     else
            //     {
            //         expStr = passEXP + "/" + ConfigManager.Instance.Tables.Tbbattlepass_exp.Get(passLevel).exp;
            //         progressProportion = 1;
            //     }
            //
            //     this.GetFromReference(KText_PassExp).GetTextMeshPro().SetTMPText(expStr);
            //     if (isFirst)
            //     {
            //         this.GetFromReference(KImg_PassFilledImg).GetXImage().SetFillAmount(progressProportion);
            //     }
            //     else
            //     {
            //         this.GetFromReference(KImg_PassFilledImg).GetXImage().DoFillAmount(progressProportion, 0.2f);
            //     }
            // }
            // else
            // {
            //     KPass.SetActive(false);
            // }
        }

        async UniTaskVoid InitNode()
        {
            //WebMessageHandler.Instance.AddHandler(CMD.QUERYACTIVITYTASK, OnQueryMonopolyTaskResponse);

            var KScrollView = GetFromReference(UIPanel_Main.KScrollView);
            var KIconBtnPos = GetFromReference(UIPanel_Main.KIconBtnPos);
            var KTaskItemPos = GetFromReference(UIPanel_Main.KTaskItemPos);
            var KBtn_Start = GetFromReference(UIPanel_Main.KBtn_Start);
            var KText_StartTitle = GetFromReference(UIPanel_Main.KText_StartTitle);
            var KText_StartNum = GetFromReference(UIPanel_Main.KText_StartNum);
            var KImg_StartIcon = GetFromReference(UIPanel_Main.KImg_StartIcon);
            var KBtn_PlayerImage = GetFromReference(UIPanel_Main.KBtn_PlayerImage);

            var KText_PlayerLevel = GetFromReference(UIPanel_Main.KText_PlayerLevel);

            var KImg_FilledImgExp = GetFromReference(UIPanel_Main.KImg_FilledImgExp);
            var KText_Energy = GetFromReference(UIPanel_Main.KText_Energy);

            var KText_Diamond = GetFromReference(UIPanel_Main.KText_Diamond);
            var KText_Money = GetFromReference(UIPanel_Main.KText_Money);

            var KText_PassTitle = GetFromReference(UIPanel_Main.KText_PassTitle);
            var KActivityPos = GetFromReference(UIPanel_Main.KActivityPos);
            var KBtn_Energy = GetFromReference(UIPanel_Main.KBtn_Energy);
            var KBtn_Money = GetFromReference(UIPanel_Main.KBtn_Money);
            var KBtn_Diamond = GetFromReference(UIPanel_Main.KBtn_Diamond);
            var KBtn_Treasure = GetFromReference(UIPanel_Main.KBtn_Treasure);
            var KImg_TreasureRedDot = GetFromReference(UIPanel_Main.KImg_TreasureRedDot);
            var KText_TreasureRedDot = GetFromReference(UIPanel_Main.KText_TreasureRedDot);
            var KPlayer = GetFromReference(UIPanel_Main.KPlayer);
            var KSpine_Player = GetFromReference(UIPanel_Main.KSpine_Player);
            var KImg_PlayerTip = GetFromReference(UIPanel_Main.KImg_PlayerTip);

            //var KPlayerTip = GetFromReference(UIPanel_Main.KPlayerTip);
            var KText_PlayerTip = GetFromReference(UIPanel_Main.KText_PlayerTip);
            var KContainer_Treasure = GetFromReference(UIPanel_Main.KContainer_Treasure);
            var KImg_Arrow = GetFromReference(UIPanel_Main.KImg_Arrow);
            var KBtn_Player = GetFromReference(UIPanel_Main.KBtn_Player);
            //var KBg_MainMask = GetFromReference(UIPanel_Main.KBg_MainMask);
            var KBtn_Test = GetFromReference(UIPanel_Main.KBtn_Test);
            var KBg_TestGuid = GetFromReference(UIPanel_Main.KBg_TestGuid);

            var KPass = GetFromReference(UIPanel_Main.KPass);
            var KImg_PassFilledImg = GetFromReference(UIPanel_Main.KImg_PassFilledImg);
            var KText_PassExp = GetFromReference(UIPanel_Main.KText_PassExp);
            var KText_PassLevel = GetFromReference(UIPanel_Main.KText_PassLevel);
            var KImg_PassRedDot = GetFromReference(UIPanel_Main.KImg_PassRedDot);


            JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var jiyuUI);
            var KBg_JiYuMask = jiyuUI.GetFromReference(UIPanel_JiyuGame.KBg_JiYuMask);

            // var guidRect = KBg_TestGuid.GetXImage().Get().hollowArea;
            //
            // var parRect = KBg_TestGuid.GetRectTransform();
            // await UniTask.Yield();
            // Log.Debug($"parRect {parRect.Width()} {parRect.Height()}");
            // // 全屏矩形的定义
            // guidRect = new Rect(parRect.Width() / 2f, parRect.Height() / 2f, parRect.Width(), parRect.Height());
            // KBg_TestGuid.GetXImage().Get().hollowArea = guidRect;
            //
            // KBg_TestGuid.GetXImage().Get().material.SetVector("_Rect",
            //     new Vector4(guidRect.x, guidRect.y, guidRect.width, guidRect.height));
            KPass.SetActive(false);
            KActivityPos.SetActive(false);
            JiYuTweenHelper.JiYuOnClickNoAnim(KBtn_Player, async () =>
            {
                if (playerState != 0)
                {
                    return;
                }

                if (this.cts != null)
                {
                    this.cts.Cancel();
                    this.cts.Dispose();
                }

                cts = new CancellationTokenSource();
                Log.Error($"KBtn_PlayerOnclick");
                // for (int i = 0; i < 888; i++)
                // {
                //     var index = i;
                //     if (index == 0)
                //     {
                //         KPlayerTip.SetActive(true);
                //         await UniTask.Delay((int)(3f * 1000f), false,
                //             PlayerLoopTiming.Update, cts.Token);
                //         KPlayerTip.SetActive(false);
                //     }
                //     else
                //     {
                //         await UniTask.Delay((int)(Random.Range(3f, 8f) * 1000f), false,
                //             PlayerLoopTiming.Update, cts.Token);
                //         KPlayerTip.SetActive(true);
                //         await UniTask.Delay((int)(3f * 1000f), false,
                //             PlayerLoopTiming.Update, cts.Token);
                //         KPlayerTip.SetActive(false);
                //     }
                // }
            }, 0.2f);


            curInternalTime = Random.Range(3f, 8f);
            curEnableTime = enableTime;
            //StartUpdate();

            //KImg_EnergyIcon.GetImage().SetSpriteAsync(tbuser_variable.Get(1).icon, true);
            //KImg_GemIcon.GetImage().SetSpriteAsync(tbuser_variable.Get(2).icon, true);
            //KImg_GoldIcon.GetImage().SetSpriteAsync(tbuser_variable.Get(3).icon, true);


            scrollRect = KScrollView.GetXScrollRect();
            var content = scrollRect.Content;


            scrollRect.OnDrag.Add(OnScrollRectDragging);
            scrollRect.OnEndDrag.Add(() =>
            {
                curWidth = 0;
                scrollRect.Get().vertical = true;
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                {
                    var uis = ui as UIPanel_JiyuGame;
                    uis.OnEndDrag();
                }
            });
            // scrollRect.OnBeginDrag.Add(() =>
            // {
            //     //Log.Error($"OnBeginDrag");
            //     Log.Debug($" {m_ScrollRect.Get().horizontalNormalizedPosition}");
            //     m_IsEndMove = false;
            //     m_IsDrag = true;
            // });


            ChangePlayerInfo();

            KBg_JiYuMask.SetActive(false);

            //content.GetRectTransform().Get().childCount
            //content.GetRectTransform().GetChild(0);
            // var listPos = new List<Vector3>();
            // var points = content.GetComponent<EdgeCollider2D>().points;
            // foreach (var VARIABLE in points)
            // {
            //     listPos.Add(new Vector3(VARIABLE.x, VARIABLE.y, 0));
            // }

            // content.GetRectTransform().GetChild(0).DOLocalPath(listPos.ToArray(), 5).onWaypointChange += (a) =>
            // {
            //     //Debug.LogError($"{a}");
            // };


            KText_StartTitle.GetTextMeshPro().SetTMPText(tblanguage.Get("func_3801_name").current);


            ShowLevelExp(ResourcesSingleton.Instance.UserInfo, KImg_FilledImgExp,
                KText_PlayerLevel);
            ShowPassLevelExp(ResourcesSingleton.Instance.gamePassExp);

            KImg_Arrow.SetActive(false);
            KContainer_Treasure.SetActive(false);
            //KPlayerTip.SetActive(false);
            //KTaskItemPos.AddChild<>()
            var taskItemPosList = KTaskItemPos.GetList();
            taskItemPosList.Clear();
            var downItemList = tbtag_func.DataList.Where((x) => x.tagId == 3 && x.posType == 2).ToList();

            downItemList = downItemList
                .GroupBy(p => p.sub2Pos) // 按 Age 分组
                .Select(g => g.First()) // 保留每个 Age 第一项
                .ToList();


            bool enableTaskItem = false;
            foreach (var downItem in downItemList)
            {
                Log.Debug($"downItem:{downItem.id}");
                //int index = i;
                int tagId = downItem.id;
                // switch (index + 1)
                // {
                //     case 1:
                //         tagId = 3502;
                //         break;
                //     case 2:
                //         tagId = 3602;
                //         break;
                //     case 3:
                //         tagId = 3604;
                //         break;
                // }

                if (!ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(tagId))
                {
                    continue;
                }

                enableTaskItem = true;
                var ui = await taskItemPosList.CreateWithUITypeAsync(
                        UIType.UISubPanel_TaskBtnItem, tagId, false)
                    as UISubPanel_TaskBtnItem;

                var KImg_IconBtn = ui.GetFromReference(UISubPanel_TaskBtnItem.KImg_IconBtn);
                var KText_IconBtn = ui.GetFromReference(UISubPanel_TaskBtnItem.KText_IconBtn);
                var KImg_RedDot = ui.GetFromReference(UISubPanel_TaskBtnItem.KImg_RedDot);

                KText_IconBtn.GetTextMeshPro().SetTMPText(tblanguage.Get(tbtag_func.Get(tagId).name).current);
                KImg_IconBtn.GetImage().SetSpriteAsync(tbtag_func.Get(tagId).icon, false).Forget();
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KImg_IconBtn, () => { OnClickTagFunc(tagId, ui); },
                    1104);
                var m_RedDotName = NodeNames.GetTagFuncRedDotName(tagId);
                RedDotManager.Instance.AddListener(m_RedDotName, num =>
                {
                    Log.Debug($"{m_RedDotName} {num}");
                    KImg_RedDot.SetActive(num > 0);
                });
            }

            KTaskItemPos.SetActive(enableTaskItem);
            // for (int i = 0; i < 3; i++)
            // {
            //     int index = i;
            //     int tagId = 0;
            //     switch (index + 1)
            //     {
            //         case 1:
            //             tagId = 3502;
            //             break;
            //         case 2:
            //             tagId = 3602;
            //             break;
            //         case 3:
            //             tagId = 3604;
            //             break;
            //     }
            //
            //     if (!ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(tagId))
            //     {
            //         continue;
            //     }
            //
            //     var ui = await taskItemPosList.CreateWithUITypeAsync(
            //             UIType.UISubPanel_TaskBtnItem, tagId, false)
            //         as UISubPanel_TaskBtnItem;
            //
            //     var KImg_IconBtn = ui.GetFromReference(UISubPanel_TaskBtnItem.KImg_IconBtn);
            //     var KText_IconBtn = ui.GetFromReference(UISubPanel_TaskBtnItem.KText_IconBtn);
            //     var KImg_RedDot = ui.GetFromReference(UISubPanel_TaskBtnItem.KImg_RedDot);
            //
            //     KText_IconBtn.GetTextMeshPro().SetTMPText(tblanguage.Get(tbtag_func.Get(tagId).name).current);
            //     KImg_IconBtn.GetImage().SetSpriteAsync(tbtag_func.Get(tagId).icon, false).Forget();
            //     JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KImg_IconBtn, () => { OnClickTagFunc(tagId, ui); });
            //     var m_RedDotName = NodeNames.GetTagFuncRedDotName(tagId);
            //     RedDotManager.Instance.AddListener(m_RedDotName, num =>
            //     {
            //         Log.Debug($"{m_RedDotName} {num}");
            //         KImg_RedDot.SetActive(num > 0);
            //     });
            // }

            #region Activity

            var activityList = KActivityPos.GetList();
            activityList.Clear();
            var activityFlag = ResourcesSingleton.Instance.activity.ActivityFlag;
            // long nowTime = TimeHelper.ClientNowSeconds() -
            //                ResourcesSingleton.Instance.ServerDeltaTime / 1000;

            KPass.SetActive(false);
            bool activityEnable = false;
            foreach (var ac in activityFlag.ActivityFlagMap)
            {
                var activity = tbactivity.Get(ac.Key);

                switch (activity.type)
                {
                    //通行证	
                    case 11:
                        var battlepass = tbbattlepass.Get(activity.link);
                        if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(battlepass.tagFunc))
                        {
                            // var KPass = GetFromReference(UIPanel_Main.KPass);
                            // var KImg_PassFilledImg = GetFromReference(UIPanel_Main.KImg_PassFilledImg);
                            // var KText_PassExp = GetFromReference(UIPanel_Main.KText_PassExp);
                            // var KText_PassLevel = GetFromReference(UIPanel_Main.KText_PassLevel);
                            // var KImg_PassRedDot = GetFromReference(UIPanel_Main.KImg_PassRedDot);

                            var KImg_RedDot11 = KImg_PassRedDot;
                            var m_RedDotName11 = NodeNames.GetTagFuncRedDotName(battlepass.tagFunc);
                            Log.Debug($"通行证 {ac.Value}", Color.cyan);
                            KImg_RedDot11.SetActive(ac.Value > 0 ? true : false);
                            RedDotManager.Instance.AddListener(m_RedDotName11, (num) =>
                            {
                                Log.Debug($"{m_RedDotName11} {num}", Color.cyan);
                                KImg_RedDot11?.SetActive(num > 0);
                            });
                            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KPass,
                                () => { UIHelper.CreateAsync(UIType.UIPanel_Pass, ac.Key); }, 1104);

                            KPass.SetActive(true);
                            Log.Debug($"通行证{battlepass.tagFunc}");
                        }

                        break;
                    //活跃度挑战			
                    case 21:

                        var daysChallenge = tbdays_challenge.Get(activity.link);
                        if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(daysChallenge.tagFunc))
                        {
                            activityEnable = true;
                            var ac21ui =
                                await activityList.CreateWithUITypeAsync(UIType.UISubPanel_ActivityItem,
                                    daysChallenge.tagFunc,
                                    false);
                            var KImg_RedDot21 =
                                ac21ui.GetFromReference(UISubPanel_ActivityItem.KImg_RedDot);
                            var m_RedDotName21 = NodeNames.GetTagFuncRedDotName(daysChallenge.tagFunc);
                            KImg_RedDot21.SetActive(ac.Value > 0 ? true : false);
                            RedDotManager.Instance.AddListener(m_RedDotName21, (num) =>
                            {
                                Log.Debug($"{m_RedDotName21} {num}", Color.cyan);
                                KImg_RedDot21?.SetActive(num > 0);
                            });

                            ac21ui.GetFromReference(UISubPanel_ActivityItem.KText_ActivityItem).GetTextMeshPro()
                                .SetTMPText(tblanguage.Get(tbtag_func.Get(daysChallenge.tagFunc).name).current);
                            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(ac21ui,
                                () => { UIHelper.CreateAsync(UIType.UIPanel_Activity_Challenge, ac.Key); }, 1104);
                            Log.Debug($"活跃度挑战{daysChallenge.tagFunc}");
                        }

                        break;
                    //签到			
                    case 22:


                        var daysSign = tbdays_sign.Get(activity.link);
                        if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(daysSign.tagFunc))
                        {
                            activityEnable = true;
                            var ac22ui =
                                await activityList.CreateWithUITypeAsync(UIType.UISubPanel_ActivityItem,
                                    daysSign.tagFunc, false);

                            ac22ui.GetFromReference(UISubPanel_ActivityItem.KText_ActivityItem).GetTextMeshPro()
                                .SetTMPText(tblanguage.Get(tbtag_func.Get(daysSign.tagFunc).name).current);
                            var KImg_RedDot22 =
                                ac22ui.GetFromReference(UISubPanel_ActivityItem.KImg_RedDot);

                            var m_RedDotName22 = NodeNames.GetTagFuncRedDotName(daysSign.tagFunc);

                            //KImg_RedDot22.SetActive(RedDotManager.Instance.GetRedPointCnt(m_RedDotName) > 0);
                            KImg_RedDot22.SetActive(ac.Value > 0 ? true : false);
                            // var itemStr = $"{m_RedDotName}|Up{i}";
                            // RedDotManager.Instance.InsterNode(itemStr);

                            RedDotManager.Instance.AddListener(m_RedDotName22, (num) =>
                            {
                                Log.Debug($"KImg_RedDot22 {m_RedDotName22} {num}", Color.cyan);
                                KImg_RedDot22?.SetActive(num > 0);
                            });


                            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(ac22ui,
                                () => { UIHelper.CreateAsync<int>(UIType.UIPanel_Activity_NewSign, ac.Key); }, 1104);
                        }

                        break;
                    //大富翁			
                    case 23:


                        var monopoly = tbmonopoly.Get(activity.link);
                        if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(monopoly.tagFunc))
                        {
                            activityEnable = true;
                            var ac23ui =
                                await activityList.CreateWithUITypeAsync(UIType.UISubPanel_ActivityItem,
                                    monopoly.tagFunc, false);
                            var KText_ActivityItem23 =
                                ac23ui.GetFromReference(UISubPanel_ActivityItem.KText_ActivityItem);
                            var KImg_RedDot23 =
                                ac23ui.GetFromReference(UISubPanel_ActivityItem.KImg_RedDot);
                            var m_RedDotName23 = NodeNames.GetTagFuncRedDotName(monopoly.tagFunc);
                            KText_ActivityItem23.GetTextMeshPro()
                                .SetTMPText(tblanguage.Get(tbtag_func.Get(monopoly.tagFunc).name).current);
                            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(ac23ui,
                                () =>
                                {
                                    //TODO：
                                    UIHelper.CreateAsync(UIType.UIPanel_Activity_Monopoly, ac.Key);
                                }, 1104);

                            // var diceReward = new Vector3(5, monopoly.diceItem, 1);
                            // var diceStr = $"{m_RedDotName}|Pos0";
                            //
                            // RedDotManager.Instance.SetRedPointCnt(diceStr,
                            //     (int)JiYuUIHelper.GetRewardCount(diceReward));

                            //KImg_RedDot23.SetActive(RedDotManager.Instance.GetRedPointCnt(m_RedDotName) > 0);
                            KImg_RedDot23.SetActive(ac.Value > 0 ? true : false);
                            RedDotManager.Instance.AddListener(m_RedDotName23, (num) =>
                            {
                                Log.Debug($"{m_RedDotName23} {num}", Color.cyan);

                                KImg_RedDot23?.SetActive(num > 0);
                            });

                            Log.Debug($"gameTaskInfo111", Color.green);
                            // var node = RedDotManager.Instance.GetNode("Root");
                            // node.PrintTree();
                        }

                        break;
                    //体力商店			
                    case 24:


                        var energyShop = tbenergy_shop.Get(activity.link);
                        if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(energyShop.tagFunc))
                        {
                            activityEnable = true;
                            var ac24ui =
                                await activityList.CreateWithUITypeAsync(UIType.UISubPanel_ActivityItem,
                                    energyShop.tagFunc, false);
                            var KText_ActivityItem24 =
                                ac24ui.GetFromReference(UISubPanel_ActivityItem.KText_ActivityItem);
                            var KImg_RedDot24 =
                                ac24ui.GetFromReference(UISubPanel_ActivityItem.KImg_RedDot);

                            KText_ActivityItem24.GetTextMeshPro()
                                .SetTMPText(tblanguage.Get(tbtag_func.Get(energyShop.tagFunc).name).current);
                            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(ac24ui,
                                () => { UIHelper.CreateAsync(UIType.UIPanel_Activity_EnergyShop, ac.Key); }, 1104);
                            KImg_RedDot24.SetActive(false);
                            // var m_RedDotName = NodeNames.GetTagFuncRedDotName(energyShop.tagFunc);
                            //
                            //
                            // KImg_RedDot.SetActive(RedDotManager.Instance.GetRedPointCnt(m_RedDotName) > 0);
                            //
                            // RedDotManager.Instance.AddListener(m_RedDotName, (num) =>
                            // {
                            //     //Log.Error($"RedDotManager {itemStr} {num}");
                            //     KImg_RedDot?.SetActive(num > 0);
                            // });
                        }

                        break;
                }
            }

            KActivityPos.SetActive(activityFlag.ActivityFlagMap.Count > 0 && activityEnable);
            activityList.Sort((a, b) =>
            {
                var uia = a as UISubPanel_ActivityItem;
                var uib = b as UISubPanel_ActivityItem;
                return tbtag_func.Get(uia.tagFuncId).sort.CompareTo(tbtag_func.Get(uib.tagFuncId).sort);
            });

            #endregion

            var upItemList = tbtag_func.DataList.Where((x) => x.tagId == 3 && x.posType == 1).ToList();
            var KIconBtnPosList = KIconBtnPos.GetList();
            KIconBtnPosList.Clear();
            foreach (var tag in upItemList)
            {
                if (!ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(tag.id))
                {
                    continue;
                }

                if (tag.id == 3401)
                {
                    int chargeInt = ResourcesSingleton.Instance.firstChargeInt;
                    if (chargeInt == 4)
                    {
                        continue;
                    }
                }

                int tagId = tag.id;
                var ui = await KIconBtnPosList.CreateWithUITypeAsync(
                        UIType.UISubPanel_IconBtnItem, tag.id, false)
                    as UISubPanel_IconBtnItem;
                var KImg_IconBtn = ui.GetFromReference(UISubPanel_IconBtnItem.KImg_IconBtn);
                var KText_IconBtn = ui.GetFromReference(UISubPanel_IconBtnItem.KText_IconBtn);
                KText_IconBtn.GetTextMeshPro().SetTMPText(tblanguage.Get(tbtag_func.Get(tagId).name).current);
                KImg_IconBtn.GetImage().SetSpriteAsync(tbtag_func.Get(tagId).icon, false).Forget();

                InitMainPanelRedDot(ui, tagId);


                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KImg_IconBtn, () => { OnClickTagFunc(tagId, ui); },
                    1104);

                #region huangjinguo add

                if (tagId == 3405)
                {
                    ui.GetFromReference(UISubPanel_IconBtnItem.KTip).SetActive(true);
                    ui.GetFromReference(UISubPanel_IconBtnItem.KText_Diamond).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName("icon_diamond") +
                                    ResourcesSingleton.Instance.goldPig.GoldBank.ToString());
                }

                #endregion
            }

            KIconBtnPosList.Sort((a, b) =>
            {
                var uia = a as UISubPanel_IconBtnItem;
                var uib = b as UISubPanel_IconBtnItem;
                var tagA = tbtag_func.Get(uia.id);
                var tagB = tbtag_func.Get(uib.id);
                return tagA.sort.CompareTo(tagB.sort);
            });

            taskItemPosList.Sort((a, b) =>
            {
                var uia = a as UISubPanel_TaskBtnItem;
                var uib = b as UISubPanel_TaskBtnItem;
                var tagA = tbtag_func.Get(uia.id);
                var tagB = tbtag_func.Get(uib.id);
                return tagA.sort.CompareTo(tagB.sort);
            });

            var maxChapterID = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID;
            var maxBlockID = ResourcesSingleton.Instance.levelInfo.maxMainBlockID;

            var savedChapterId = JsonManager.Instance.userData.chapterId;
            var savedBlockId = JsonManager.Instance.userData.blockId;

            Log.Debug(
                $"maxChapterID{maxChapterID} maxBlockID{maxBlockID} savedChapterId{savedChapterId} savedBlockId{savedBlockId}",
                Color.green);
            var cost = tbchapter.Get(maxChapterID).cost;
            KText_StartNum.GetTextMeshPro().SetTMPText(cost[0].z.ToString());


            var contentList = content.GetList();
            contentList.Clear();

            float pageWidth = KScrollView.GetRectTransform().Width();
            float pageHeight = KScrollView.GetRectTransform().Height();
            pageWidth = Screen.width;
            pageHeight = Screen.height;
            var contentRect = scrollRect.Content.GetRectTransform();
            contentRect.SetWidth(pageWidth);
            contentRect.SetHeight(pageHeight * (tbblock.DataList.Count));

            curChapterID = JsonManager.Instance.userData.lastChapterId;
            //curChapterID = lastChapterId;

            foreach (var block in tbblock.DataList)
            {
                var blockId = block.id;
                var ui = await contentList.CreateWithUITypeAsync(
                        UIType.UISubPanel_MainMap, blockId, false)
                    as UISubPanel_MainMap;
                var uiRect = ui.GetRectTransform();
                var mainMapPos = new Vector2(0, (blockId - 1) * pageHeight);
                uiRect.SetAnchoredPosition(mainMapPos);
                uiRect.SetWidth(pageWidth);
                uiRect.SetHeight(pageHeight);
                var KImg_UnlockMain = ui.GetFromReference(UISubPanel_MainMap.KImg_UnlockMain);
                var KText_UnlockMain = ui.GetFromReference(UISubPanel_MainMap.KText_UnlockMain);
                KImg_UnlockMain.SetActive(false);
                if (blockId == tbblock.DataList[tbblock.DataList.Count - 1].id)
                {
                    KText_UnlockMain.GetTextMeshPro().SetTMPText(tblanguage.Get("text_version_unlock").current);
                    KImg_UnlockMain.SetActive(true);
                }

                ui.GetImage().SetSpriteAsync(block.bg, true).Forget();


                var KBtn_CloudMask = ui.GetFromReference(UISubPanel_MainMap.KBtn_CloudMask);
                var KSplineAnimate_MoveEntity = ui.GetFromReference(UISubPanel_MainMap.KSplineAnimate_MoveEntity);
                KSplineAnimate_MoveEntity.SetActive(false);
                var KNodePos = ui.GetFromReference(UISubPanel_MainMap.KNodePos);
                //var KBtn_Mask = ui.GetFromReference(UISubPanel_MainMap.KBtn_Mask);
                var KBtn_TreasureMask = ui.GetFromReference(UISubPanel_MainMap.KBtn_TreasureMask);
                KBtn_TreasureMask.GetButton().OnClick.Add(() =>
                {
                    var KContainer_Treasure = GetFromReference(UIPanel_Main.KContainer_Treasure);
                    var KImg_Arrow = GetFromReference(UIPanel_Main.KImg_Arrow);

                    KImg_Arrow.SetActive(false);
                    KContainer_Treasure.SetActive(false);

                    JiYuUIHelper.DestoryAllTips();
                });

                //KBtn_CloudMask.SetActive(!(blockId == 1));
                //KBtn_Mask.SetActive(false);

                KBtn_CloudMask.SetActive(blockId > maxBlockID);

                var nodeList = KNodePos.GetList();
                nodeList.Clear();

                //var KSpline = ui.GetFromReference(UISubPanel_MainMap.KSpline);
                var splineStr = $"Spline{blockId - 1}";
                var KSpline = ui.GetFromReference(splineStr);
                KSpline.SetActive(true);
                var parentPos = KSpline.GetRectTransform().AnchoredPosition();
                var splineIndex = blockId - 1;
                //TODO:加数据
                splineIndex = 0;
                var splineList = KSpline.GetComponent<SplineContainer>()[splineIndex];
                //var sum = 0;

                for (int i = 0; i < splineList.Count; i++)
                {
                    var index = i;
                    var node = splineList[index];
                    int chapterId = ((blockId - 1) * 10) + index + 1;
                    var nodePos = (float2)parentPos + node.Position.xy;
                    var nodeui = await nodeList.CreateWithUITypeAsync(
                            UIType.UISubPanel_LevelChooseItem, chapterId, false)
                        as UISubPanel_LevelChooseItem;

                    var KBg_LevelItemDown = nodeui.GetFromReference(UISubPanel_LevelChooseItem.KBg_LevelItemDown);
                    var KText_LevelId = nodeui.GetFromReference(UISubPanel_LevelChooseItem.KText_LevelId);

                    var KText_LevelItemTitle =
                        nodeui.GetFromReference(UISubPanel_LevelChooseItem.KText_LevelItemTitle);
                    var KText_LevelItemContent =
                        nodeui.GetFromReference(UISubPanel_LevelChooseItem.KText_LevelItemContent);
                    var KBg_LevelItemUp = nodeui.GetFromReference(UISubPanel_LevelChooseItem.KBg_LevelItemUp);

                    KText_LevelId.GetTextMeshPro().SetTMPText(chapterId.ToString());


                    nodeui.GetRectTransform().SetAnchoredPosition(nodePos);

                    string title = $"{tbchapter.Get(chapterId).num}";
                    var timeInSeconds = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterSurviveTime;
                    var minutes = timeInSeconds / 60;
                    var seconds = timeInSeconds % 60;
                    string cont = $"{tblanguage.Get("chapter_survival_maxtime_text").current}{minutes}m{seconds}s";
                    KText_LevelItemTitle.GetTextMeshPro().SetTMPText(title);
                    KText_LevelItemContent.SetActive(false);


                    var nodeWorldPos = (float2)nodePos + (float2)mainMapPos;
                    nodeWorldPos.y += pageHeight / 2f;
                    if (ResourcesSingleton.Instance.levelInfo.maxPassChapterID == 0 && chapterId == 1)
                    {
                        KPlayer.GetRectTransform().SetAnchoredPosition(nodeWorldPos);
                    }
                    else if (chapterId == curChapterID)
                    {
                        KPlayer.GetRectTransform().SetAnchoredPosition(nodeWorldPos);
                    }
                    // if (maxChapterID != savedChapterId)
                    // {
                    //    
                    // }
                    // else
                    // {
                    //     // var nodeLast = splineList[index <= 0 ? 0 : index - 1];
                    //     // var nodePosTemp = (float2)parentPos + nodeLast.Position.xy;
                    //     // var nodeWorldPosTemp = (float2)nodePosTemp + (float2)mainMapPos;
                    //     // nodeWorldPosTemp.y += pageHeight / 2f;
                    //
                    //     if (ResourcesSingleton.Instance.levelInfo.maxPassChapterID == 0 && chapterId == 1)
                    //     {
                    //         KPlayer.GetRectTransform().SetAnchoredPosition(nodeWorldPos);
                    //     }
                    //     else if (chapterId == curChapterID)
                    //     {
                    //         KPlayer.GetRectTransform().SetAnchoredPosition(nodeWorldPos);
                    //     }
                    // }

                    KBg_LevelItemDown.GetRectTransform()
                        .SetHeight(KText_LevelItemTitle.GetTextMeshPro().Get().preferredHeight + VerticalTextGap);
                    KBg_LevelItemDown.GetRectTransform()
                        .SetWidth(Mathf.Max(KText_LevelItemTitle.GetTextMeshPro().Get().preferredWidth, 80f) +
                                  HorizentalTextGap);
                    KBg_LevelItemDown.SetActive(false);
                    //上次选的关卡
                    if (chapterId == curChapterID)
                    {
                        KBg_LevelItemDown.SetActive(true);
                        //curChapterID = chapterId;
                        title =
                            $"{tbchapter.Get(chapterId).num} {tblanguage.Get(tbchapter.Get(chapterId).name).current}";
                        KText_LevelItemTitle.GetTextMeshPro().SetTMPText(title);
                        KText_LevelItemContent.GetTextMeshPro().SetTMPText(cont);
                        KText_LevelItemContent.SetActive(false);

                        KBg_LevelItemDown.GetRectTransform()
                            .SetHeight(KText_LevelItemTitle.GetTextMeshPro().Get().preferredHeight + VerticalTextGap);

                        KBg_LevelItemDown.GetRectTransform()
                            .SetWidth(
                                Mathf.Max(KText_LevelItemTitle.GetTextMeshPro().Get().preferredWidth, 80f) +
                                HorizentalTextGap);
                    }


                    // KBg_LevelItemDown.GetRectTransform()
                    //     .SetHeight(KText_LevelItemTitle.GetTextMeshPro().Get().preferredHeight + 20);
                    // KBg_LevelItemDown.GetRectTransform()
                    //     .SetWidth(Mathf.Max(KText_LevelItemTitle.GetTextMeshPro().Get().preferredWidth, 80f) +
                    //               20);

                    if (curChapterID == ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID &&
                        curChapterID == chapterId)
                    {
                        KBg_LevelItemDown.SetActive(true);
                        //初始化位置和首个选中
                        KText_PlayerTip.GetTextMeshPro()
                            .SetTMPText(tblanguage.Get(tbchapter.Get(curChapterID).desc).current);
                        var tipHeight = math.max(100f, KText_PlayerTip.GetTextMeshPro().Get().preferredHeight);
                        KImg_PlayerTip.GetRectTransform()
                            .SetHeight(tipHeight);

                        title =
                            $"{tbchapter.Get(chapterId).num} {tblanguage.Get(tbchapter.Get(chapterId).name).current}";
                        KText_LevelItemTitle.GetTextMeshPro().SetTMPText(title);
                        KText_LevelItemContent.GetTextMeshPro().SetTMPText(cont);
                        KText_LevelItemContent.SetActive(true);
                        KBg_LevelItemDown.GetRectTransform()
                            .SetHeight(KText_LevelItemTitle.GetTextMeshPro().Get().preferredHeight +
                                       KText_LevelItemContent.GetTextMeshPro().Get().preferredHeight + VerticalTextGap);
                        KBg_LevelItemDown.GetRectTransform()
                            .SetWidth(Mathf.Max(KText_LevelItemTitle.GetTextMeshPro().Get().preferredWidth,
                                KText_LevelItemContent.GetTextMeshPro().Get().preferredWidth) + HorizentalTextGap);
                    }


                    //已解锁未通关
                    if (chapterId == ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID)
                    {
                        //curChapterID = chapterId;

                        //KBg_LevelItemDown.GetImage().SetColor("FFEDD6");
                        //KBg_LevelItemUp.GetImage().SetColor("FFEDD6");
                        KBg_LevelItemUp.GetImage().SetSpriteAsync("pic_levelchooseitem_unlocked", true).Forget();

                        // KText_LevelItemContent.GetTextMeshPro().SetColor("F67416");
                        // KText_LevelItemTitle.GetTextMeshPro().SetColor("F67416");


                        KBg_LevelItemUp.GetButton().OnClick.Add(async () =>
                        {
                            if (curChapterID == chapterId)
                            {
                                return;
                            }

                            AudioManager.Instance.PlayFModAudio(1107);

                            var KContainer_Treasure = GetFromReference(UIPanel_Main.KContainer_Treasure);
                            var KImg_Arrow = GetFromReference(UIPanel_Main.KImg_Arrow);
                            KContainer_Treasure.SetActive(false);
                            KImg_Arrow.SetActive(false);
                            playerState = 1;
                            curChapterID = chapterId;

                            var cost = tbchapter.Get(curChapterID).cost;
                            KText_StartNum.GetTextMeshPro().SetTMPText(cost[0].z.ToString());

                            KText_PlayerTip.GetTextMeshPro()
                                .SetTMPText(tblanguage.Get(tbchapter.Get(curChapterID).desc).current);
                            var tipHeight = math.max(100f, KText_PlayerTip.GetTextMeshPro().Get().preferredHeight);
                            KImg_PlayerTip.GetRectTransform()
                                .SetHeight(tipHeight);
                            //int redDotCount = GetRedDotCount();

                            DisplayRedRot();
                            foreach (var content in contentList.Children)
                            {
                                var contents = content as UISubPanel_MainMap;

                                if (contents.blockId <= ResourcesSingleton.Instance.levelInfo.maxMainBlockID)
                                {
                                    var KNodePos = contents.GetFromReference(UISubPanel_MainMap.KNodePos);
                                    var nodeList = KNodePos.GetList();
                                    foreach (var child in nodeList.Children)
                                    {
                                        var childs = child as UISubPanel_LevelChooseItem;

                                        var KBg_LevelItemDown =
                                            childs.GetFromReference(UISubPanel_LevelChooseItem.KBg_LevelItemDown);
                                        var KText_LevelItemTitle =
                                            childs.GetFromReference(UISubPanel_LevelChooseItem
                                                .KText_LevelItemTitle);
                                        var KText_LevelItemContent =
                                            childs.GetFromReference(UISubPanel_LevelChooseItem
                                                .KText_LevelItemContent);
                                        var KBg_LevelItemUp =
                                            childs.GetFromReference(UISubPanel_LevelChooseItem.KBg_LevelItemUp);

                                        KBg_LevelItemDown.SetActive(false);
                                        string title = $"{tbchapter.Get(childs.chapterId).num}";
                                        KText_LevelItemTitle.GetTextMeshPro().SetTMPText(title);
                                        KText_LevelItemContent.SetActive(false);
                                        KBg_LevelItemDown.GetRectTransform()
                                            .SetHeight(KText_LevelItemTitle.GetTextMeshPro().Get().preferredHeight +
                                                       VerticalTextGap);
                                        KBg_LevelItemDown.GetRectTransform()
                                            .SetWidth(Mathf.Max(
                                                          KText_LevelItemTitle.GetTextMeshPro().Get()
                                                              .preferredWidth, 80f) +
                                                      HorizentalTextGap);
                                    }
                                }
                            }


                            title =
                                $"{tbchapter.Get(chapterId).num} {tblanguage.Get(tbchapter.Get(chapterId).name).current}";
                            KText_LevelItemTitle.GetTextMeshPro().SetTMPText(title);
                            KText_LevelItemContent.GetTextMeshPro().SetTMPText(cont);
                            KText_LevelItemContent.SetActive(true);
                            KBg_LevelItemDown.SetActive(true);
                            KBg_LevelItemDown.GetRectTransform()
                                .SetHeight(KText_LevelItemTitle.GetTextMeshPro().Get().preferredHeight +
                                           KText_LevelItemContent.GetTextMeshPro().Get().preferredHeight +
                                           VerticalTextGap);

                            KBg_LevelItemDown.GetRectTransform()
                                .SetWidth(Mathf.Max(KText_LevelItemTitle.GetTextMeshPro().Get().preferredWidth,
                                    KText_LevelItemContent.GetTextMeshPro().Get().preferredWidth) + HorizentalTextGap);


                            KPlayer.GetRectTransform().SetAnchoredPosition(nodeWorldPos);
                            var playerPosY = -math.abs(KPlayer.GetRectTransform().AnchoredPosition().y);
                            content.GetRectTransform().Get().DOAnchorPosY(playerPosY + Screen.height / 2f, 0.3f);
                            //TODO:动画：

                            //KPlayerTip.SetActive(false);
                            var skeletonGraphic = KSpine_Player.GetComponent<SkeletonGraphic>();
                            skeletonGraphic.AnimationState.ClearTracks();
                            skeletonGraphic.AnimationState.SetAnimation(0, "flash", false);

                            if (this.cts != null)
                            {
                                this.cts.Cancel();
                                this.cts.Dispose();
                            }

                            cts = new CancellationTokenSource();
                            await UniTask.Delay(JiYuUIHelper.GetAnimaionDuration(skeletonGraphic, "flash"), false,
                                PlayerLoopTiming.Update, cts.Token);
                            //Debug.LogError($"111111");
                            skeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
                            //Debug.LogError($"222222");
                            //await UniTask.Delay(2000);
                            //Debug.LogError($"333333");
                            playerState = 0;
                            //await PlayerTipOccurAsyc(cts.Token);
                        });
                    }
                    //未解锁
                    else if (chapterId > ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID)
                    {
                        //KBg_LevelItemDown.GetImage().SetColor("9CA3B0");
                        //KBg_LevelItemUp.GetImage().SetColor("9CA3B0");
                        KBg_LevelItemUp.GetImage().SetSpriteAsync("pic_levelchooseitem_locked", true).Forget();
                        // KText_LevelItemContent.GetTextMeshPro().SetColor("FFFFFF");
                        // KText_LevelItemTitle.GetTextMeshPro().SetColor("FFFFFF");

                        KBg_LevelItemUp.GetButton().OnClick.Add(() =>
                        {
                            JiYuUIHelper.ClearCommonResource();
                            UIHelper.CreateAsync(UIType.UICommon_Resource,
                                tblanguage.Get("challenge_2_unlock_text").current);
                        });
                    }
                    //已解锁已通关
                    else
                    {
                        //KBg_LevelItemDown.GetImage().SetColor("BBF7D0");
                        //KBg_LevelItemUp.GetImage().SetColor("BBF7D0");
                        KBg_LevelItemUp.GetImage().SetSpriteAsync("pic_levelchooseitem_unlocked", true).Forget();

                        // KText_LevelItemContent.GetTextMeshPro().SetColor("16A34A");
                        // KText_LevelItemTitle.GetTextMeshPro().SetColor("16A34A");

                        KBg_LevelItemUp.GetButton().OnClick.Add(async () =>
                        {
                            if (curChapterID == chapterId)
                            {
                                return;
                            }

                            AudioManager.Instance.PlayFModAudio(1107);

                            var KContainer_Treasure = GetFromReference(UIPanel_Main.KContainer_Treasure);
                            var KImg_Arrow = GetFromReference(UIPanel_Main.KImg_Arrow);
                            KContainer_Treasure.SetActive(false);
                            KImg_Arrow.SetActive(false);
                            playerState = 1;
                            curChapterID = chapterId;
                            var cost = tbchapter.Get(curChapterID).cost;
                            KText_StartNum.GetTextMeshPro().SetTMPText(cost[0].z.ToString());

                            KText_PlayerTip.GetTextMeshPro()
                                .SetTMPText(tblanguage.Get(tbchapter.Get(curChapterID).desc).current);
                            var tipHeight = math.max(100f, KText_PlayerTip.GetTextMeshPro().Get().preferredHeight);
                            KImg_PlayerTip.GetRectTransform()
                                .SetHeight(tipHeight);

                            //int redDotCount = GetRedDotCount();

                            DisplayRedRot();


                            foreach (var content in contentList.Children)
                            {
                                var contents = content as UISubPanel_MainMap;

                                if (contents.blockId <= ResourcesSingleton.Instance.levelInfo.maxMainBlockID)
                                {
                                    var KNodePos = contents.GetFromReference(UISubPanel_MainMap.KNodePos);
                                    var nodeList = KNodePos.GetList();
                                    foreach (var child in nodeList.Children)
                                    {
                                        var childs = child as UISubPanel_LevelChooseItem;

                                        var KBg_LevelItemDown =
                                            childs.GetFromReference(UISubPanel_LevelChooseItem.KBg_LevelItemDown);
                                        var KText_LevelItemTitle =
                                            childs.GetFromReference(UISubPanel_LevelChooseItem
                                                .KText_LevelItemTitle);
                                        var KText_LevelItemContent =
                                            childs.GetFromReference(UISubPanel_LevelChooseItem
                                                .KText_LevelItemContent);
                                        var KBg_LevelItemUp =
                                            childs.GetFromReference(UISubPanel_LevelChooseItem.KBg_LevelItemUp);
                                        KBg_LevelItemDown.SetActive(false);
                                        string title = $"{tbchapter.Get(childs.chapterId).num}";
                                        KText_LevelItemTitle.GetTextMeshPro().SetTMPText(title);
                                        KText_LevelItemContent.SetActive(false);
                                        KBg_LevelItemDown.GetRectTransform()
                                            .SetHeight(KText_LevelItemTitle.GetTextMeshPro().Get().preferredHeight +
                                                       VerticalTextGap);
                                        KBg_LevelItemDown.GetRectTransform()
                                            .SetWidth(Mathf.Max(
                                                          KText_LevelItemTitle.GetTextMeshPro().Get()
                                                              .preferredWidth, 80f) +
                                                      HorizentalTextGap);
                                    }
                                }
                            }


                            title =
                                $"{tbchapter.Get(chapterId).num} {tblanguage.Get(tbchapter.Get(chapterId).name).current}";
                            KText_LevelItemTitle.GetTextMeshPro().SetTMPText(title);
                            KText_LevelItemContent.GetTextMeshPro().SetTMPText(cont);
                            KText_LevelItemContent.SetActive(false);
                            KBg_LevelItemDown.SetActive(true);
                            KBg_LevelItemDown.GetRectTransform()
                                .SetHeight(
                                    KText_LevelItemTitle.GetTextMeshPro().Get().preferredHeight + VerticalTextGap);

                            KBg_LevelItemDown.GetRectTransform()
                                .SetWidth(
                                    Mathf.Max(KText_LevelItemTitle.GetTextMeshPro().Get().preferredWidth, 80f) +
                                    HorizentalTextGap);


                            KPlayer.GetRectTransform().SetAnchoredPosition(nodeWorldPos);
                            var playerPosY = -math.abs(KPlayer.GetRectTransform().AnchoredPosition().y);
                            content.GetRectTransform().Get().DOAnchorPosY(playerPosY + Screen.height / 2f, 0.3f);
                            //TODO:动画：
                            //KPlayerTip.SetActive(false);
                            var skeletonGraphic = KSpine_Player.GetComponent<SkeletonGraphic>();
                            skeletonGraphic.AnimationState.ClearTracks();
                            skeletonGraphic.AnimationState.SetAnimation(0, "flash", false);

                            if (this.cts != null)
                            {
                                this.cts.Cancel();
                                this.cts.Dispose();
                            }

                            cts = new CancellationTokenSource();

                            await UniTask.Delay(JiYuUIHelper.GetAnimaionDuration(skeletonGraphic, "flash"), false,
                                PlayerLoopTiming.Update, cts.Token);
                            skeletonGraphic?.AnimationState?.SetAnimation(0, "idle", true);
                            playerState = 0;
                            //await PlayerTipOccurAsyc(cts.Token);
                        });
                    }


                    //KImg_UnlockIcon.SetActive(chapterId > ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID);
                }
            }

            //int redDotCount = GetRedDotCount();

            DisplayRedRot();
            KPlayer.GetRectTransform().Get().SetSiblingIndex(content.GetRectTransform().Get().childCount - 1);


            //KImg_PlayerTip

            //KSpine_Player.GetRectTransform().SetSiblingIndex(content.GetRectTransform().ChildCount-2);
            // contentList.Sort((a, b) =>
            // {
            //     var uia = a as UISubPanel_MainMap;
            //     var uib = b as UISubPanel_MainMap;
            //     return uia.blockId.CompareTo(uib.blockId);
            // });


            var playerPosY = -math.abs(KPlayer.GetRectTransform().AnchoredPosition().y);
            content.GetRectTransform().Get().DOAnchorPosY(playerPosY + Screen.height / 2f, 0.3f);
            //content.GetRectTransform().DoAnchoredPositionY(playerPosY + Screen.height / 2f, 2f);
            //街区动画
            //bool isCrossBlock = false
            if (savedBlockId == maxBlockID)
            {
                if (ResourcesSingleton.Instance.FromRunTimeScene)
                {
                    await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXExit);
                }
                else
                {
                    await JiYuTweenHelper.EnableLoading(false);
                }

                //isCrossBlock = true;
                var ui = await UIHelper.CreateAsync(UIType.UIPanel_UnlockBlock, new Vector2(0, maxBlockID));
                var KBtn_Mask = ui.GetFromReference(UIPanel_UnlockBlock.KBtn_Mask);

                await UniTask.Delay(1000);
                KBtn_Mask.GetButton().OnClick.Add(async () =>
                {
                    AudioManager.Instance.PlayFModAudio(1105);
                    JsonManager.Instance.userData.blockId++;
                    JsonManager.Instance.userData.chapterId++;
                    JsonManager.Instance.SavePlayerData(JsonManager.Instance.userData);

                    ui.SetActive(false);
                    var uiManager = UIHelper.TryGetUIManager();
                    uiManager.BlurVolume?.SetViewActive(false);
                    //TODO:动画
                    foreach (var child in contentList.Children)
                    {
                        var childs = child as UISubPanel_MainMap;
                        //var KBtn_Mask = childs.GetFromReference(UISubPanel_MainMap.KBtn_Mask);

                        if (maxBlockID == childs.blockId)
                        {
                            KBg_JiYuMask.SetActive(true);
                            playerState = 1;
                            Log.Debug($"maxBlockID{maxBlockID}");

                            content.GetRectTransform().DoAnchoredPositionY(-pageHeight * (maxBlockID - 1), 2f);

                            //content.GetRectTransform().DoLocalMoveY(-pageHeight * (maxBlockID - 1), 2f);
                            await UniTask.Delay(2000);

                            var KBtn_CloudMask = childs.GetFromReference(UISubPanel_MainMap.KBtn_CloudMask);
                            var KImg_Cloud1 = childs.GetFromReference(UISubPanel_MainMap.KImg_Cloud1);
                            // KBtn_CloudMask.GetRectTransform().GetChild(0).DOLocalMoveX(-pageWidth, 2f);

                            KBtn_CloudMask.GetRectTransform().GetChild(2).DOLocalMoveX(-2000 / 1.5f, 1f);
                            KBtn_CloudMask.GetRectTransform().GetChild(3).DOLocalMoveX(2000 / 1.5f, 1f).onComplete +=
                                () =>
                                {
                                    //KBtn_CloudMask.GetRectTransform().GetChild(1).GetComponent<Image>().
                                    KImg_Cloud1.GetImage().DoFade(1, 0, 1f).AddOnCompleted(() =>
                                    {
                                        KBtn_CloudMask.SetActive(false);
                                    });
                                };

                            #region PlayerMove

                            var splineStr = $"Spline{maxBlockID - 1}";
                            var splineStr1 = $"Spline{maxBlockID}";
                            var KSpline = childs.GetFromReference(splineStr);
                            var KSpline1 = childs.GetFromReference(splineStr1);
                            //KSpline.SetActive(true);
                            //var KSpline = childs.GetFromReference(UISubPanel_MainMap.KSpline);
                            //var KBtn_Mask = childs.GetFromReference(UISubPanel_MainMap.KBtn_Mask);

                            var splineContainer = KSpline.GetComponent<SplineContainer>();
                            var oldknots = splineContainer.Spline.Knots.ToList();
                            var splineContainer1 = KSpline1.GetComponent<SplineContainer>();
                            var oldknots1 = splineContainer.Spline.Knots.ToList();
                            var knots = new List<BezierKnot>(2);


                            knots.Add(oldknots[0]);
                            knots.Add(oldknots1[9]);
                            splineContainer.Spline = new Spline
                            {
                                Knots = knots,
                                Closed = false
                            };

                            await UniTask.Yield();
                            var KSplineAnimate_MoveEntity =
                                childs.GetFromReference(UISubPanel_MainMap.KSplineAnimate_MoveEntity);
                            var splineAnimate = KSplineAnimate_MoveEntity.GetComponent<SplineAnimate>();
                            splineAnimate.Container = splineContainer;
                            KSplineAnimate_MoveEntity.SetActive(true);
                            splineAnimate.Restart(false);

                            splineAnimate.Play();


                            var playerRect = KPlayer.GetRectTransform().Get();
                            var moveEntityRect = KSplineAnimate_MoveEntity.GetRectTransform().Get();
                            var offset = playerRect.AnchoredPosition() - moveEntityRect.AnchoredPosition();

                            // playerRect.DOAnchorPos(moveEntityRect.AnchoredPosition() + offset, splineAnimate.Duration)
                            //     .ChangeEndValue(moveEntityRect.AnchoredPosition() + offset);
                            var skeletonGraphic = KSpine_Player.GetComponent<SkeletonGraphic>();

                            skeletonGraphic.AnimationState.ClearTracks();
                            skeletonGraphic.AnimationState.SetAnimation(0, "run", true);
                            var scaleX = math.abs(KSpine_Player.GetRectTransform().Scale2().x);

                            splineAnimate.Updated += (a, b) =>
                            {
                                playerRect.SetAnchoredPosition(moveEntityRect.AnchoredPosition() + offset);

                                var bz = NormalizeAngle(b.eulerAngles.z);
                                bool isRight = bz <= 0;
                                if (isRight)
                                {
                                    KSpine_Player.GetRectTransform().SetScaleX(scaleX);
                                }
                                else
                                {
                                    KSpine_Player.GetRectTransform().SetScaleX(-scaleX);
                                }


                                //Log.Error($"{bz}");
                            };
                            await UniTask.Delay((int)(splineAnimate.Duration * 1000f));
                            KSpine_Player.GetRectTransform().SetScaleX(scaleX);
                            skeletonGraphic.AnimationState.SetAnimation(0, "idle", true);


                            KBg_JiYuMask.SetActive(false);
                            playerState = 0;
                            ui.Dispose();
                            UpdateLevelChooseItem(maxChapterID);
                            //await PlayerTipOccurAsyc(cts.Token);

                            #endregion

                            // KBg_JiYuMask.SetActive(false);
                            // playerState = 0;
                            // ui.Dispose();
                            // await PlayerTipOccurAsyc(cts.Token);
                            break;
                        }
                    }

                    ui.Dispose();
                });
            }
            //章节动画
            else if (savedChapterId == maxChapterID)
            {
                if (ResourcesSingleton.Instance.FromRunTimeScene)
                {
                    await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXExit);
                }
                else
                {
                    await JiYuTweenHelper.EnableLoading(false);
                }

                var ui = await UIHelper.CreateAsync(UIType.UIPanel_UnlockBlock, new Vector2(1, maxChapterID));
                //var ui = await UIHelper.CreateAsync(UIType.UIPanel_UnlockBlock, maxBlockID);
                var KBtn_Mask = ui.GetFromReference(UIPanel_UnlockBlock.KBtn_Mask);

                await UniTask.Delay(1000);
                KBtn_Mask.GetButton().OnClick.Add(async () =>
                {
                    AudioManager.Instance.PlayFModAudio(1105);

                    JsonManager.Instance.userData.chapterId++;
                    JsonManager.Instance.SavePlayerData(JsonManager.Instance.userData);
                    ui.SetActive(false);
                    var uiManager = UIHelper.TryGetUIManager();
                    uiManager.BlurVolume?.SetViewActive(false);
                    //TODO:动画

                    foreach (var child in contentList.Children)
                    {
                        var childs = child as UISubPanel_MainMap;
                        if (maxBlockID == childs.blockId)
                        {
                            //return;
                            KBg_JiYuMask.SetActive(true);
                            playerState = 1;
                            var splineStr = $"Spline{maxBlockID - 1}";
                            var KSpline = childs.GetFromReference(splineStr);
                            KSpline.SetActive(true);
                            //var KSpline = childs.GetFromReference(UISubPanel_MainMap.KSpline);
                            //var KBtn_Mask = childs.GetFromReference(UISubPanel_MainMap.KBtn_Mask);

                            var splineContainer = KSpline.GetComponent<SplineContainer>();
                            var oldknots = splineContainer.Spline.Knots.ToList();
                            var knots = new List<BezierKnot>(2);

                            knots.Add(oldknots[(maxChapterID - 1 - 1) % 10]);
                            knots.Add(oldknots[(maxChapterID - 1) % 10]);
                            splineContainer.Spline = new Spline
                            {
                                Knots = knots,
                                Closed = false
                            };

                            await UniTask.Yield();
                            var KSplineAnimate_MoveEntity =
                                childs.GetFromReference(UISubPanel_MainMap.KSplineAnimate_MoveEntity);
                            var splineAnimate = KSplineAnimate_MoveEntity.GetComponent<SplineAnimate>();
                            splineAnimate.Container = splineContainer;
                            KSplineAnimate_MoveEntity.SetActive(true);
                            splineAnimate.Restart(false);

                            splineAnimate.Play();


                            var playerRect = KPlayer.GetRectTransform().Get();
                            var moveEntityRect = KSplineAnimate_MoveEntity.GetRectTransform().Get();
                            var offset = playerRect.AnchoredPosition() - moveEntityRect.AnchoredPosition();

                            // playerRect.DOAnchorPos(moveEntityRect.AnchoredPosition() + offset, splineAnimate.Duration)
                            //     .ChangeEndValue(moveEntityRect.AnchoredPosition() + offset);
                            var skeletonGraphic = KSpine_Player.GetComponent<SkeletonGraphic>();

                            skeletonGraphic.AnimationState.ClearTracks();
                            skeletonGraphic.AnimationState.SetAnimation(0, "run", true);
                            var scaleX = math.abs(KSpine_Player.GetRectTransform().Scale2().x);

                            splineAnimate.Updated += (a, b) =>
                            {
                                playerRect.SetAnchoredPosition(moveEntityRect.AnchoredPosition() + offset);

                                var bz = NormalizeAngle(b.eulerAngles.z);
                                bool isRight = bz <= 0;
                                if (isRight)
                                {
                                    KSpine_Player.GetRectTransform().SetScaleX(scaleX);
                                }
                                else
                                {
                                    KSpine_Player.GetRectTransform().SetScaleX(-scaleX);
                                }


                                //Log.Error($"{bz}");
                            };
                            await UniTask.Delay((int)(splineAnimate.Duration * 1000f));
                            KSpine_Player.GetRectTransform().SetScaleX(scaleX);
                            skeletonGraphic.AnimationState.SetAnimation(0, "idle", true);


                            KBg_JiYuMask.SetActive(false);
                            playerState = 0;
                            ui.Dispose();
                            UpdateLevelChooseItem(maxChapterID);
                            //await PlayerTipOccurAsyc(cts.Token);
                            //playerState = 0;
                        }
                    }
                });
            }
            else
            {
                if (ResourcesSingleton.Instance.FromRunTimeScene)
                {
                    JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXExit);
                }
                else
                {
                    JiYuTweenHelper.EnableLoading(false);
                }
            }

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Start,
                () => { OnStartButtonClick(curChapterID); }, 0, 1);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Test,
                () => { OnStartButtonClick(curChapterID, true); });
            //ShowTimeView();
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_PlayerImage,
                () =>
                {
                    if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                        return;
                    var uis = ui as UIPanel_JiyuGame;
                    uis.GoToTagId(5);

                    //OpenPlayerInfo();
                });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Energy,
                () => { OnBuyEnergyBtnClick(); });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Money,
                () => { GoToChongZhi(); });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Diamond,
                () => { GoToChongZhi(); });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Treasure,
                () => OnBtnTreasure());
            // content.GetRectTransform().GetChild(0).GetComponent<SplineAnimate>()
            //     .Container.Spline.RemoveAt();
            // content.GetRectTransform().GetChild(0).GetComponent<SplineAnimate>().Updated
            // foreach (var VARIABLE in content.GetRectTransform().GetChild(0).GetComponent<SplineAnimate>()
            //              .Container.Spline)
            // {
            //     Debug.LogError($"{VARIABLE.Position}");
            // }
            //PassSet();
            //AudioManager.Instance.SetFModSFXMute(false);
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var uijiyu))
            {
                var uis = uijiyu as UIPanel_JiyuGame;
                uis.ForceRefreshToggles();
            }


            JiYuUIHelper.InitUIPosInfo();
            ResourcesSingleton.Instance.isUIInit = true;
            //await UniTask.Delay(1000);
            JiYuUIHelper.InitAudioSettings();

            Guide().Forget();
        }

        public void ChangePlayerInfo()
        {
            var KImg_HeadImage = GetFromReference(UIPanel_Main.KImg_HeadImage);
            var KImg_Frame = GetFromReference(UIPanel_Main.KImg_Frame);
            var KText_PlayerName = GetFromReference(UIPanel_Main.KText_PlayerName);

            var gamerole = ResourcesSingleton.Instance.UserInfo;
            Log.Debug($" gamerole.RoleAvatar{gamerole.RoleAvatar} gamerole.RoleAvatarFrame{gamerole.RoleAvatarFrame}");

            KImg_HeadImage.GetImage()
                .SetSpriteAsync(tbuser_avatar.Get(gamerole.RoleAvatar).icon, true);
            KImg_Frame.GetImage()
                .SetSpriteAsync(tbuser_avatar.Get(gamerole.RoleAvatarFrame).icon, true);
            KText_PlayerName.GetTextMeshPro().SetTMPText(gamerole.Nickname);
        }

        public async void OnGuideIdFinished(int guideId, bool isFinished = true)
        {
            Log.Debug($"开始guide.id:{guideId}");
            if (isFinished)
            {
                JiYuUIHelper.TryFinishGuide(guideId);
            }

            await UniTask.Delay(500);

            _guides = _guides.Where(a => a.id != guideId).ToList();
            if (_guides.Count > 0)
            {
                var guide = _guides[0];
                //Log.Debug($"开始guide.id:{guide.id}");
                IntroGuideOrder(guide.id);
            }
        }


        async UniTask IntroGuideOrder(int guideId)
        {
            this.guideId = guideId;
            var guide = tbguide.Get(guideId);

            Log.Debug($"尝试主界面引导guide.guideType:{guide.guideType}");
            switch (guide.guideType)
            {
                case 315:
                    WebMessageHandler.Instance.AddHandler(CMD.CHANGESTATUS, OnChangeNameStatusResponse);
                    NetWorkManager.Instance.SendMessage(CMD.CHANGESTATUS);
                    break;
                case 316:

                    if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(2))
                    {
                        var guideUI = await UIHelper.CreateAsync(UIType.UISubPanel_Guid, guide.id, 1);
                        //guideUI.GetFromReference(UISubPanel_Guid.)
                    }
                    else
                    {
                        OnGuideIdFinished(guide.id, false);
                    }

                    break;
                case 317:
                    if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(1))
                    {
                        var guideUI = await UIHelper.CreateAsync(UIType.UISubPanel_Guid, guide.id, 1);
                    }
                    else
                    {
                        OnGuideIdFinished(guide.id, false);
                    }

                    break;
            }
        }


        async UniTaskVoid Guide()
        {
            if (ResourcesSingleton.Instance.settingData.GuideList.Contains(1))
            {
                return;
            }

            _guides.Clear();
            // var guide315 = tbguide.DataList.Where(a => a.guideType == 315).FirstOrDefault();
            // var guide316 = tbguide.DataList.Where(a => a.guideType == 316).FirstOrDefault();
            // var guide317 = tbguide.DataList.Where(a => a.guideType == 317).FirstOrDefault();
            List<int> guideTempList = new List<int>() { 315, 316, 317 };

            foreach (var temp in guideTempList)
            {
                var guide = tbguide.DataList.Where(a => a.guideType == temp).FirstOrDefault();
                if (ResourcesSingleton.Instance.settingData.GuideList.Contains(guide.group))
                {
                    _guides.Add(guide);
                }
            }


            if (_guides.Count > 0)
            {
                var guide = _guides[0];
                Log.Debug($"开始guide.id:{guide.id}");
                IntroGuideOrder(guide.id);
            }
        }

        private async void OnChangeNameStatusResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.CHANGESTATUS, OnChangeNameStatusResponse);
            var checkResult = new CheckResult();
            checkResult.MergeFrom(e.data);
            Debug.Log(checkResult);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }


            var ui = await UIHelper.CreateAsync(UIType.UISubPanel_Person_ChangeNameCenter, checkResult, guideId);
            // var ui = await UIHelper.CreateAsync<CheckResult>(UIType.UISubPanel_Person_ChangeName, checkResult,
            //     UILayer.Mid);
            ui.SetParent(this, false);
        }

        private void GoToChongZhi()
        {
            var str = "type=1;para=[141]";
            JiYuUIHelper.GoToSomePanel(str);
            //Close();
        }

        private void InitMainPanelRedDot(UISubPanel_IconBtnItem ui, int tagId)
        {
            var nodeName = NodeNames.GetTagFuncRedDotName(tagId);
            Log.Debug(nodeName, Color.cyan);
            switch (tagId)
            {
                case 3601:
                    var dailyCanSignOrNot = ResourcesSingleton.Instance.signData.OnDayStatus;
                    if (dailyCanSignOrNot == 0)
                    {
                        Log.Debug("dailyCanSignOrNot==0", Color.cyan);
                        RedDotManager.Instance.SetRedPointCnt(nodeName, 1);
                        ui.GetFromReference(UISubPanel_IconBtnItem.KImgRedDot).SetActive(true);
                    }

                    break;
                default:
                    break;
            }
        }


        private async UniTaskVoid OnBuyEnergyBtnClick()
        {
            var ui = await UIHelper.CreateAsync(UIType.UIPanel_BuyEnergy);


            // JiYuTweenHelper.SetEaseAlphaAndPosB2U(ui, 0);
            // await UniTask.Delay(100);
            //
            // float incremental = 200f;
            // JiYuTweenHelper.SetEaseAlphaAndPosLtoR(ui.GetFromReference(UIPanel_BuyEnergy.KContainerDiamondBuy), 0, 200,
            //     cts.Token, 0.35f);
            // //await UniTask.Delay(200);
            // JiYuTweenHelper.SetEaseAlphaAndPosLtoR(ui.GetFromReference(UIPanel_BuyEnergy.KContainerAdvertiseBuy), 0, 400f,
            //     cts.Token, 0.35f);
            //ui.GetFromReference(UIPanel_BuyEnergy.KContainerAdvertiseBuy).GetRectTransform().SetAnchoredPositionX(-incremental);
            //ui.GetFromReference(UIPanel_BuyEnergy.KContainerAdvertiseBuy).GetComponent<CanvasGroup>().alpha = 0f;
            //ui.GetFromReference(UIPanel_BuyEnergy.KContainerAdvertiseBuy).GetComponent<RectTransform>().DOAnchorPosX(0, 0.25f).SetEase(Ease.InQuad);
            //ui.GetFromReference(UIPanel_BuyEnergy.KContainerAdvertiseBuy).GetComponent<CanvasGroup>().DOFade(1, 0.25f).SetEase(Ease.InQuad);
            //ui.GetFromReference(UIPanel_BuyEnergy.KContainerDiamondBuy).GetRectTransform().SetAnchoredPositionX(-incremental);
            //ui.GetFromReference(UIPanel_BuyEnergy.KContainerDiamondBuy).GetComponent<CanvasGroup>().alpha = 0f;
            //ui.GetFromReference(UIPanel_BuyEnergy.KContainerDiamondBuy).GetComponent<RectTransform>().DOAnchorPosX(0, 0.25f).SetEase(Ease.InQuad);
            //ui.GetFromReference(UIPanel_BuyEnergy.KContainerDiamondBuy).GetComponent<CanvasGroup>().DOFade(1, 0.25f).SetEase(Ease.InQuad);
        }


        void UpdateLevelChooseItem(int chapterID)
        {
            curChapterID = chapterID;

            var KScrollView = GetFromReference(UIPanel_Main.KScrollView);
            var KIconBtnPos = GetFromReference(UIPanel_Main.KIconBtnPos);
            var KTaskItemPos = GetFromReference(UIPanel_Main.KTaskItemPos);
            var KBtn_Start = GetFromReference(UIPanel_Main.KBtn_Start);
            var KText_StartTitle = GetFromReference(UIPanel_Main.KText_StartTitle);
            var KText_StartNum = GetFromReference(UIPanel_Main.KText_StartNum);
            var KImg_StartIcon = GetFromReference(UIPanel_Main.KImg_StartIcon);
            var KBtn_PlayerImage = GetFromReference(UIPanel_Main.KBtn_PlayerImage);
            var KImg_HeadImage = GetFromReference(UIPanel_Main.KImg_HeadImage);
            var KImg_Frame = GetFromReference(UIPanel_Main.KImg_Frame);
            var KText_PlayerLevel = GetFromReference(UIPanel_Main.KText_PlayerLevel);
            var KText_PlayerName = GetFromReference(UIPanel_Main.KText_PlayerName);
            var KImg_FilledImgExp = GetFromReference(UIPanel_Main.KImg_FilledImgExp);
            var KText_Energy = GetFromReference(UIPanel_Main.KText_Energy);
            var KText_Diamond = GetFromReference(UIPanel_Main.KText_Diamond);
            var KText_Money = GetFromReference(UIPanel_Main.KText_Money);

            var KImg_PassFilledImg = GetFromReference(UIPanel_Main.KImg_PassFilledImg);
            var KText_PassLevel = GetFromReference(UIPanel_Main.KText_PassLevel);
            var KText_PassTitle = GetFromReference(UIPanel_Main.KText_PassTitle);
            var KActivityPos = GetFromReference(UIPanel_Main.KActivityPos);
            var KBtn_Energy = GetFromReference(UIPanel_Main.KBtn_Energy);
            var KBtn_Money = GetFromReference(UIPanel_Main.KBtn_Money);

            var KBtn_Treasure = GetFromReference(UIPanel_Main.KBtn_Treasure);
            var KImg_TreasureRedDot = GetFromReference(UIPanel_Main.KImg_TreasureRedDot);
            var KText_TreasureRedDot = GetFromReference(UIPanel_Main.KText_TreasureRedDot);
            var KPlayer = GetFromReference(UIPanel_Main.KPlayer);
            var KSpine_Player = GetFromReference(UIPanel_Main.KSpine_Player);
            var KImg_PlayerTip = GetFromReference(UIPanel_Main.KImg_PlayerTip);

            //var KPlayerTip = GetFromReference(UIPanel_Main.KPlayerTip);
            var KText_PlayerTip = GetFromReference(UIPanel_Main.KText_PlayerTip);
            var KContainer_Treasure = GetFromReference(UIPanel_Main.KContainer_Treasure);
            var KImg_Arrow = GetFromReference(UIPanel_Main.KImg_Arrow);
            var KBtn_Player = GetFromReference(UIPanel_Main.KBtn_Player);

            var KBtn_Test = GetFromReference(UIPanel_Main.KBtn_Test);
            var KBg_TestGuid = GetFromReference(UIPanel_Main.KBg_TestGuid);


            KContainer_Treasure.SetActive(false);
            KImg_Arrow.SetActive(false);


            var cost = tbchapter.Get(curChapterID).cost;
            KText_StartNum.GetTextMeshPro().SetTMPText(cost[0].z.ToString());

            KText_PlayerTip.GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbchapter.Get(curChapterID).desc).current);
            var tipHeight = math.max(100f, KText_PlayerTip.GetTextMeshPro().Get().preferredHeight);
            KImg_PlayerTip.GetRectTransform()
                .SetHeight(tipHeight);
            //int redDotCount = GetRedDotCount();

            DisplayRedRot();


            var content0 = scrollRect.Content;
            var contentList = content0.GetList();

            foreach (var content in contentList.Children)
            {
                var contents = content as UISubPanel_MainMap;

                if (contents.blockId <= ResourcesSingleton.Instance.levelInfo.maxMainBlockID)
                {
                    var KNodePos = contents.GetFromReference(UISubPanel_MainMap.KNodePos);
                    var nodeList = KNodePos.GetList();
                    foreach (var child in nodeList.Children)
                    {
                        var childs = child as UISubPanel_LevelChooseItem;

                        var KBg_LevelItemDown =
                            childs.GetFromReference(UISubPanel_LevelChooseItem.KBg_LevelItemDown);
                        var KText_LevelItemTitle =
                            childs.GetFromReference(UISubPanel_LevelChooseItem
                                .KText_LevelItemTitle);
                        var KText_LevelItemContent =
                            childs.GetFromReference(UISubPanel_LevelChooseItem
                                .KText_LevelItemContent);
                        var KBg_LevelItemUp =
                            childs.GetFromReference(UISubPanel_LevelChooseItem.KBg_LevelItemUp);
                        KBg_LevelItemDown.SetActive(false);
                        string title = $"{tbchapter.Get(childs.chapterId).num}";
                        KText_LevelItemTitle.GetTextMeshPro().SetTMPText(title);
                        KText_LevelItemContent.SetActive(false);
                        KBg_LevelItemDown.GetRectTransform()
                            .SetHeight(KText_LevelItemTitle.GetTextMeshPro().Get().preferredHeight +
                                       VerticalTextGap);
                        KBg_LevelItemDown.GetRectTransform()
                            .SetWidth(Mathf.Max(
                                          KText_LevelItemTitle.GetTextMeshPro().Get()
                                              .preferredWidth, 80f) +
                                      HorizentalTextGap);

                        if (childs.chapterId == chapterID)
                        {
                            var timeInSeconds = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterSurviveTime;
                            var minutes = timeInSeconds / 60;
                            var seconds = timeInSeconds % 60;
                            string cont =
                                $"{tblanguage.Get("chapter_survival_maxtime_text").current}{minutes}m{seconds}s";
                            title =
                                $"{tbchapter.Get(chapterID).num} {tblanguage.Get(tbchapter.Get(chapterID).name).current}";
                            KText_LevelItemTitle.GetTextMeshPro().SetTMPText(title);
                            KText_LevelItemContent.GetTextMeshPro().SetTMPText(cont);
                            KText_LevelItemContent.SetActive(true);
                            KBg_LevelItemDown.SetActive(true);
                            KBg_LevelItemDown.GetRectTransform()
                                .SetHeight(KText_LevelItemTitle.GetTextMeshPro().Get().preferredHeight +
                                           KText_LevelItemContent.GetTextMeshPro().Get().preferredHeight +
                                           VerticalTextGap);

                            KBg_LevelItemDown.GetRectTransform()
                                .SetWidth(Mathf.Max(KText_LevelItemTitle.GetTextMeshPro().Get().preferredWidth,
                                    KText_LevelItemContent.GetTextMeshPro().Get().preferredWidth) + HorizentalTextGap);


                            //KPlayer.GetRectTransform().SetAnchoredPosition(nodeWorldPos);
                            var playerPosY = -math.abs(KPlayer.GetRectTransform().AnchoredPosition().y);
                            content0.GetRectTransform().Get().DOAnchorPosY(playerPosY + Screen.height / 2f, 0.3f);
                        }
                    }
                }
            }
        }

        async public void OnQueryMonopolyTaskResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERYACTIVITYTASK, OnQueryMonopolyTaskResponse);

            ByteValueList taskList = new ByteValueList();

            taskList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            //23:活动类型
            if (!JiYuUIHelper.TryGetActivityLink(23, out var activityId, out var link))
            {
                return;
            }

            ResourcesSingleton.Instance.activity.activityTaskDic.TryRemove(activityId, out var list);

            var tasks = new List<GameTaskInfo>();
            Log.Debug($"gameTaskInfo", Color.green);
            foreach (var taskBytes in taskList.Values)
            {
                GameTaskInfo gameTaskInfo = new GameTaskInfo();
                gameTaskInfo.MergeFrom(taskBytes);
                tasks.Add(gameTaskInfo);

                Log.Debug($"{gameTaskInfo.ToString()}", Color.green);
            }

            var tbtask = ConfigManager.Instance.Tables.Tbtask;
            var tbtask_group = ConfigManager.Instance.Tables.Tbtask_group;
            var tbtask_type = ConfigManager.Instance.Tables.Tbtask_type;
            var tbmonopoly = ConfigManager.Instance.Tables.Tbmonopoly;


            int redDotNum = 0;

            var m_RedDotName = NodeNames.GetTagFuncRedDotName(tbmonopoly.Get(link).tagFunc);

            var taskListStr =
                $"{m_RedDotName}|Pos1";

            foreach (var gameTask in tasks)
            {
                //RedDotManager.Instance.ClearChildrenListeners(pos1);
                var task = tbtask.Get(gameTask.Id);
                var task_group = tbtask_group.Get(task.group);
                var task_type = tbtask_type.Get(task.type);
                if (gameTask.Para >= task.para[0] && gameTask.Status == 0)
                {
                    redDotNum++;
                }
            }

            Log.Debug($"isRedDot{redDotNum}", Color.green);
            RedDotManager.Instance.SetRedPointCnt(taskListStr, redDotNum);

            // var node = RedDotManager.Instance.GetNode(m_RedDotName);
            // node.PrintTree();

            ResourcesSingleton.Instance.activity.activityTaskDic.TryAdd(activityId, tasks);
        }


        private int GetRedDotCount()
        {
            //var chapterBoxConfig = tbchapter_box.DataList;
            int minNotLockBoxID = ResourcesSingleton.Instance.levelInfo.levelBox.minNotLockBoxID;
            int minNotGetBoxID = ResourcesSingleton.Instance.levelInfo.levelBox.minNotGetBoxID;
            int boxID = 0, redDOtCount = 0;
            if (minNotGetBoxID < minNotLockBoxID)
            {
                redDOtCount++;
            }
            //for (int i = 0; i < chapterBoxConfig.Count; i++)
            //{
            //    if (chapterBoxConfig[i].chapterId == curChapterID)
            //    {
            //        boxID = chapterBoxConfig[i].id;
            //        if(boxID >= minNotGetBoxID)
            //        {
            //            redDOtCount++;
            //        }
            //        //if (!ResourcesSingleton.Instance.levelInfo.levelBox.boxStateDic.ContainsKey(boxID))
            //        //{
            //        //    continue;
            //        //}
            //        //else
            //        //{
            //        //    if (ResourcesSingleton.Instance.levelInfo.levelBox.boxStateDic[boxID])
            //        //    {
            //        //        continue;
            //        //    }
            //        //    else
            //        //    {
            //        //        redDOtCount++;
            //        //    }
            //        //}
            //    }
            //}

            return redDOtCount;
        }


        // async UniTask PlayerTipOccurAsyc(CancellationToken cct)
        // {
        //     var KPlayerTip = GetFromReference(UIPanel_Main.KPlayerTip);
        //     for (int i = 0; i < 888; i++)
        //     {
        //         var index = i;
        //         if (index == 0)
        //         {
        //             await UniTask.Delay((int)(2f * 1000f), false,
        //                 PlayerLoopTiming.Update, cct);
        //             KPlayerTip.SetActive(true);
        //             await UniTask.Delay((int)(3f * 1000f), false,
        //                 PlayerLoopTiming.Update, cct);
        //             KPlayerTip.SetActive(false);
        //         }
        //         else
        //         {
        //             await UniTask.Delay((int)(Random.Range(3f, 8f) * 1000f), false,
        //                 PlayerLoopTiming.Update, cct);
        //             KPlayerTip.SetActive(true);
        //             await UniTask.Delay((int)(3f * 1000f), false,
        //                 PlayerLoopTiming.Update, cct);
        //             KPlayerTip.SetActive(false);
        //         }
        //     }
        // }

        public void SetIconBtnEnable(int tag_funcId, bool enable)
        {
            var KIconBtnPos = GetFromReference(UIPanel_Main.KIconBtnPos);
            var KIconBtnPosList = KIconBtnPos.GetList();

            foreach (var ui in KIconBtnPosList.Children)
            {
                var uis = ui as UISubPanel_IconBtnItem;
                if (uis.id == tag_funcId)
                {
                    uis.SetActive(enable);
                    break;
                }
            }
        }


        float NormalizeAngle(float angle)
        {
            while (angle > 180)
            {
                angle -= 360;
            }

            while (angle < -180)
            {
                angle += 360;
            }

            return angle;
        }

        private async UniTaskVoid OnBtnTreasure()
        {
            if (JiYuUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui))
            {
                UIHelper.Remove(UIType.UICommon_ItemTips);
            }

            if (JiYuUIHelper.TryGetUI(UIType.UICommon_Reward_Tip, out var ui1))
            {
                UIHelper.Remove(UIType.UICommon_Reward_Tip);
            }

            UIHelper.Remove(UIType.UICommon_EquipTips);
            UIHelper.Remove(UIType.UICommon_ResourceNotEnough);
            var kContainer_Treasure = GetFromReference(UIPanel_Main.KContainer_Treasure);
            var KImg_Arrow = GetFromReference(UIPanel_Main.KImg_Arrow);

            KImg_Arrow.SetActive(!KImg_Arrow.GameObject.activeSelf);
            kContainer_Treasure.SetActive(!kContainer_Treasure.GameObject.activeSelf);


            UpdateTreasure().Forget();

            // if (JiYuUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui))
            // {
            //       JiYuUIHelper.DestoryAllTips();;
            // }

            //int redDotCount = 0;

            //ac.Value.MonopolyRecord.MonopolyGridNum;


            //GetFromReference(UIPanel_Main.KContainer_Treasure)?.GetComponent<RectTransform>().SetWidth(maxWidth);
            //\  if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Battle, out UI ui))
            //    //     {
            //    //         var ui1 = ui as UIPanel_Battle;
            //    //         ui1.RedPointSetState();
            //    //     }
            //    if (JiYuUIHelper.TryGetUI(UIType.cont))
        }

        public void BtnTreasure()
        {
            Log.Debug($"CloseTreasure");
            var kContainer_Treasure = GetFromReference(UIPanel_Main.KContainer_Treasure);
            var KImg_Arrow = GetFromReference(UIPanel_Main.KImg_Arrow);

            KImg_Arrow.SetActive(false);
            kContainer_Treasure.SetActive(false);
        }

        private const float TreasureAnimPosX = 300f;
        private const float TreasureAnimDuration = 0.2f;

        public async UniTask PlayTreasureAnim()
        {
            var kPos_Treasure = GetFromReference(UIPanel_Main.KPosTreasure);
            var treasureInfoList = kPos_Treasure.GetList();
            foreach (var ui in treasureInfoList.Children)
            {
                //var curRect = ui.GetRectTransform();
                var KBar = ui.GetFromReference(UIContainerBoxBar.KBar);
                //KBar.GetRectTransform().SetAnchoredPositionX(0);
                KBar.GetRectTransform().DoAnchoredPositionX(-TreasureAnimPosX, TreasureAnimDuration);
            }

            await UniTask.Delay(1000);
        }

        public async UniTaskVoid UpdateTreasure()
        {
            int minNotLockBoxID = ResourcesSingleton.Instance.levelInfo.levelBox.minNotLockBoxID;
            int minNotGetBoxID = ResourcesSingleton.Instance.levelInfo.levelBox.minNotGetBoxID;
            var kPos_Treasure = GetFromReference(UIPanel_Main.KPosTreasure);
            var treasureInfoList = kPos_Treasure.GetList();

            var chapterBoxConfig = tbchapter_box.DataList;


            treasureInfoList.Clear();

            //显示的宝箱并非当前关卡 而是最小未领取的那一个页面
            if (minNotLockBoxID > minNotGetBoxID)
            {
                var displayID = tbchapter_box.Get(minNotGetBoxID).chapterId;
                this.GetFromReference(KTxtTitle)?.GetTextMeshPro()
                    .SetTMPText(string.Format(tblanguage.GetOrDefault("chapter_box_title").current, displayID));
                for (int i = 0; i < chapterBoxConfig.Count; i++)
                {
                    if (chapterBoxConfig[i].chapterId == displayID)
                    {
                        var boxID = chapterBoxConfig[i].id;
                        var ui = await treasureInfoList.CreateWithUITypeAsync<int>(
                            UIType.UIContainerBoxBar, boxID, false) as UIContainerBoxBar;
                        ui.index = i;
                        var KBar = ui.GetFromReference(UIContainerBoxBar.KBar);
                        KBar.GetRectTransform().SetAnchoredPositionX(TreasureAnimPosX);
                        KBar.GetRectTransform().DoAnchoredPositionX(0, TreasureAnimDuration);
                        //Debug.LogError($"maxWidth:{maxWidth}");
                    }
                }
            }
            //显示当前关卡的
            else
            {
                for (int i = 0; i < chapterBoxConfig.Count; i++)
                {
                    if (chapterBoxConfig[i].chapterId == curChapterID)
                    {
                        this.GetFromReference(KTxtTitle)?.GetTextMeshPro().SetTMPText(
                            string.Format(tblanguage.GetOrDefault("chapter_box_title").current, curChapterID));
                        var boxID = chapterBoxConfig[i].id;
                        var ui = await treasureInfoList.CreateWithUITypeAsync<int>(
                            UIType.UIContainerBoxBar, boxID, false) as UIContainerBoxBar;
                        ui.index = i;
                        var KBar = ui.GetFromReference(UIContainerBoxBar.KBar);
                        KBar.GetRectTransform().SetAnchoredPositionX(TreasureAnimPosX);
                        KBar.GetRectTransform().DoAnchoredPositionX(0, TreasureAnimDuration);
                        //Debug.LogError($"maxWidth:{maxWidth}");
                    }
                }

                // treasureInfoList.Sort((a, b) =>
                // {
                //     var uia = a as UIContainerBoxBar;
                //     var uib = b as UIContainerBoxBar;
                //     return uia.index.CompareTo(uib.index);
                // });
            }

            //await UniTask.Delay(1050);
            treasureInfoList.Sort((a, b) =>
            {
                var uia = a as UIContainerBoxBar;
                var uib = b as UIContainerBoxBar;
                return uia.index.CompareTo(uib.index);
            });
            //JiYuUIHelper.ForceRefreshLayout(kPos_Treasure);
        }

        public void DisplayRedRot()
        {
            int minNotLockBoxID = ResourcesSingleton.Instance.levelInfo.levelBox.minNotLockBoxID;
            int minNotGetBoxID = ResourcesSingleton.Instance.levelInfo.levelBox.minNotGetBoxID;
            int boxID = 0, redDotCount = 0;
            if (minNotGetBoxID < minNotLockBoxID)
            {
                redDotCount++;
            }

            var m_RedDotName = NodeNames.GetTagFuncRedDotName(3501);


            var diceStr = $"{m_RedDotName}";

            RedDotManager.Instance.SetRedPointCnt(diceStr, redDotCount);

            var kImgTreasureRedDot = GetFromReference(KImg_TreasureRedDot);
            kImgTreasureRedDot.SetActive(redDotCount > 0);
            var kText_TreasureRedDot = GetFromReference(KText_TreasureRedDot);
            kText_TreasureRedDot.SetActive(false);
            //kText_TreasureRedDot.GetTextMeshPro().SetTMPText(redDotCount.ToString());
            //RedDotManager.Instance.AddListener(m_RedDotName, (num) =>
            //{
            //    //Log.Debug($"{KImg_RedDot.Name} {m_RedDotName} {num}", Color.cyan);

            //    KImg_RedDot?.SetActive(num > 0);
            //});
            //Log.Debug($"gameTaskInfo111", Color.green);
            var node = RedDotManager.Instance.GetNode(m_RedDotName);
            node.PrintTree();
        }

        public void SetAsMaxWidth(float width)
        {
            float curWidth = this.GetFromReference(KContainer_Treasure).GetRectTransform().Width();
            if (width > curWidth)
            {
                this.GetFromReference(KContainer_Treasure)?.GetRectTransform().SetWidth(width);
            }
        }

        async void OnClickTagFunc(int func_id, UI ui)
        {
            switch (func_id)
            {
                case 3502:

                    var chapterID = ResourcesSingleton.Instance.levelInfo.maxPassChapterID;
                    if (chapterID <= 0)
                    {
                        string value = string.Format("未解锁");
                        UIHelper.Create(UIType.UIPanel_Tips, value, UILayer.Mid);
                        return;
                    }

                    var uiPatrol = await UIHelper.CreateAsync(UIType.UIPanel_Patrol);
                    //JiYuTweenHelper.SetEaseAlphaAndPosB2U(uiPatrol.GetFromReference(UIPanel_Patrol.KPos_Patrol), 0, 100, 0.15f, false);
                    //JiYuTweenHelper.SetEaseAlphaAndPosLtoR(uiPatrol.GetFromReference(UIPanel_Patrol.KPos_Patrol), 0, 100, 0.15f, false);
                    //uiPatrol.GetFromReference(UIPanel_Patrol.KPos_Patrol).GetComponent<CanvasGroup>().alpha = 0f;
                    //uiPatrol.GetFromReference(UIPanel_Patrol.KPos_Patrol).GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetEase(Ease.InQuad);

                    break;
                case 3602:
                    await UIHelper.CreateAsync(UIType.UIPanel_Task_DailyAndWeekly);

                    break;
                //case 3604:
                //成就
                //UIHelper.CreateAsync(UIType.UIPanel_Achieve);

                //break;
                case 3605:
                    var uiSweep = await UIHelper.CreateAsync(UIType.UIPanel_Sweep);
                    //JiYuTweenHelper.SetEaseAlphaAndPosB2U(uiSweep.GetFromReference(UIPanel_Sweep.KContainer), 0, 100, 0.15f, false);
                    //JiYuTweenHelper.SetEaseAlphaAndPosLtoR(uiSweep.GetFromReference(UIPanel_Sweep.KContainer), 0, 100, 0.15f, false);
                    //uiSweep.GetFromReference(UIPanel_Sweep.KContainer).GetComponent<CanvasGroup>().alpha = 0f;
                    // uiSweep.GetFromReference(UIPanel_Sweep.KContainer).GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetEase(Ease.InQuad);
                    break;
                case 3401:
                    //首充
                    UIHelper.CreateAsync(UIType.UIPanel_First_Charge);

                    break;
                case 3405:
                    //存钱罐
                    UIHelper.CreateAsync(UIType.UIPanel_Bank);

                    break;
                case 3601:
                    //签到

                    UIHelper.CreateAsync<UISubPanel_IconBtnItem>(UIType.UIPanel_Sign, ui as UISubPanel_IconBtnItem)
                        .Forget();

                    break;
            }
        }


        private void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }

        void Update()
        {
            //NetWorkManager.Instance.SendMessage(CMD.INITPLAYER);
            UpdateTimeView();
        }

        private void UpdateTimeView()
        {
            if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy <
                ResourcesSingleton.Instance.UserInfo.RoleAssets.EnergyMax)
            {
                int energy_restore = tbconstant.Get("energy_restore").constantValue;
                long clientT = JiYuUIHelper.GetServerTimeStamp(true);

                if (clientT - ResourcesSingleton.Instance.UserInfo.RoleAssets.EnergyUpdate > energy_restore)
                {
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy++;
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.EnergyUpdate = clientT;
                    RefreshResourceUI();
                }
            }
        }


        public string ToTimeFormat(float time)
        {
            //秒数取整
            int seconds = (int)time;
            //一小时为3600秒 秒数对3600取整即为小时
            int hour = seconds / 3600;
            //一分钟为60秒 秒数对3600取余再对60取整即为分钟
            int minute = seconds % 3600 / 60;
            //对3600取余再对60取余即为秒数
            seconds = seconds % 3600 % 60;
            //返回00:00:00时间格式
            return string.Format("{0:D2}:{1:D2}", minute, seconds);
        }

        public async void OpenPlayerInfo()
        {
            var ui = await UIHelper.CreateAsync(UIType.UIPlayerInformtion, ResourcesSingleton.Instance.UserInfo);
            ui.SetParent(this, false);
        }

        public void RefreshResourceUI()
        {
            var KImg_FilledImgExp = GetFromReference(UIPanel_Main.KImg_FilledImgExp);
            var KText_PlayerLevel = GetFromReference(UIPanel_Main.KText_PlayerLevel);
            var KText_Diamond = GetFromReference(UIPanel_Main.KText_Diamond);
            var KText_Money = GetFromReference(UIPanel_Main.KText_Money);
            var KText_Energy = GetFromReference(UIPanel_Main.KText_Energy);

            var icon = JiYuUIHelper.GetRewardTextIconName("icon_money_add");
            // icon= UnityHelper.RichTextSize(icon, 50);
            KText_Money.GetTextMeshPro().SetTMPText(icon + JiYuUIHelper.ReturnFormatResourceNum(2));

            icon = JiYuUIHelper.GetRewardTextIconName("icon_diamond_add");
            /// icon = UnityHelper.RichTextSize(icon, 50);
            KText_Diamond.GetTextMeshPro().SetTMPText(icon + JiYuUIHelper.ReturnFormatResourceNum(1));

            icon = JiYuUIHelper.GetRewardTextIconName("icon_energy_add");
            // icon = UnityHelper.RichTextSize(icon, 50);
            KText_Energy.GetTextMeshPro()
                .SetTMPText(icon + JiYuUIHelper.ReturnFormatResourceNum(3) + "/" +
                            $"{ResourcesSingleton.Instance.UserInfo.RoleAssets.EnergyMax}");
            ShowLevelExp(ResourcesSingleton.Instance.UserInfo, KImg_FilledImgExp,
                KText_PlayerLevel);
            //ResourcesSingleton.Instance.UserInfo.RoleAssets.Level
        }

        public string ReturnFormatResourceNum(int resourceType)
        {
            long num = 0;
            //比特币
            if (resourceType == 1)
            {
                num = ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin;
            }
            else if (resourceType == 2)
            {
                num = ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill;
            }

            if (num / 1000 == 0)
            {
                return num.ToString();
            }
            else if (num / 1000 > 0 && num / 1000000 == 0)
            {
                var high = num / 1000;
                var low = num / 100 % 10;
                if (low == 0)
                {
                    return high.ToString() + "K";
                }

                return high.ToString() + "." + low.ToString() + "K";
            }
            else
            {
                var high = num / 1000000;
                var low = num / 100000 % 10;
                if (low == 0)
                {
                    return high.ToString() + "M";
                }

                return high.ToString() + "." + low.ToString() + "M";
            }
        }


        public void SetPlayerName(string name)
        {
            var KText_PlayerName = GetFromReference(UIPanel_Main.KText_PlayerName);
            KText_PlayerName.GetTextMeshPro().SetTMPText(name);
        }

        public void ShowLevelExp(GameRole userInfo, UI slideUI, UI levelTextUI)
        {
            long exp = userInfo.RoleAssets.Exp;
            int level = userInfo.RoleAssets.Level;

            List<long> arr = new List<long>();
            foreach (var levellist in tbuserLevel.DataList)
            {
                arr.Add(levellist.exp);
            }

            int result = JiYuUIHelper.FindFirstGreaterThan(arr, exp);
            long curExp = 0;
            long levelUpNeedExp = 0;
            //int level = 0;
            double expRatios;

            if (result != -1)
            {
                //level = tbuserLevel.DataList[result - 1].id;
            }
            else
            {
                //level = tbuserLevel.DataList[tbuserLevel.DataList.Count - 1].id;
                expRatios = 1;
                levelTextUI.GetTextMeshPro().SetTMPText(level.ToString());
                slideUI.GetImage().DoFillAmount((float)expRatios, 0.3f);


                //ResourcesSingleton.Instance.UserInfo.RoleAssets.Level = level;
                arr.Clear();
                return;
            }

            if (tbuserLevel.DataList[result] != null)
            {
                levelUpNeedExp = tbuserLevel.DataList[result].exp - tbuserLevel.DataList[result - 1].exp;
                curExp = exp - tbuserLevel.DataList[result - 1].exp;
            }

            expRatios = curExp / (double)levelUpNeedExp;
            if (!double.IsNaN(expRatios))
            {
                slideUI.GetImage().DoFillAmount((float)expRatios, 0.3f);
            }

            //Log.Error($"fsafsf {expRatios}  {curExp}  {(double)levelUpNeedExp}");
            levelTextUI.GetTextMeshPro().SetTMPText(level.ToString());
            //ResourcesSingleton.Instance.UserInfo.RoleAssets.Level = level;
            arr.Clear();
        }


        public void ShowPassLevelExp(int exp)
        {
            var KImg_PassFilledImg = GetFromReference(UIPanel_Main.KImg_PassFilledImg);
            var KText_PassLevel = GetFromReference(UIPanel_Main.KText_PassLevel);
            var KText_PassExp = GetFromReference(UIPanel_Main.KText_PassExp);
            JiYuUIHelper.ShowPassLevelExp(exp, KImg_PassFilledImg, KText_PassLevel, KText_PassExp);
        }

        private UI GetToggle(int index)
        {
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out UI ui))
            {
                var uis = ui as UIPanel_JiyuGame;
                if (uis.toggleUIList.Count >= index)
                {
                    return uis.toggleUIList[tbtag[index].sort - 1];
                }
            }


            return default;
        }


        #region InitBlackBoardEntity

        private EntityManager entityManager;
        private EntityQuery switchSceneQuery;
        private long timerActionId = -1;

        public async void OnStartButtonClick(int curChapterID, bool isTest = false)
        {
            if (isTest)
            {
                curChapterID = 999999;
            }


            var cost = tbchapter.Get(curChapterID).cost;

            //KText_StartNum.GetTextMeshPro().SetTMPText(cost[0].z.ToString());
            //var cost = tbchapter.Get(ResourcesSingleton.Instance.levelInfo.chapterID).cost;

            if (!JiYuUIHelper.IsRewardsEnough(cost))
            {
                //UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("common_lack_1_text").current);
                var ui = await UIHelper.CreateAsync(UIType.UIPanel_BuyEnergy);
                //ui.GameObject.GetComponent<RectTransform>().DoAnchoredPosition()
                AudioManager.Instance.PlayFModAudio(1102);
                return;
            }
            else
            {
                AudioManager.Instance.PlayFModAudio(1231);
            }


            var global = Common.Instance.Get<Global>();

            //PlayerPrefs.DeleteAll();
            // var viewList = UnityEngine.GameObject.FindGameObjectsWithTag("handelDestroy");
            // if (viewList.Length > 0)
            // {
            //     foreach (var view in viewList)
            //     {
            //         GameObject.Destroy(view.gameObject);
            //     }
            // }


            JsonManager.Instance.userData.tagId = 3;
            JsonManager.Instance.userData.lastChapterId = curChapterID;
            JsonManager.Instance.SavePlayerData(JsonManager.Instance.userData);
            JiYuUIHelper.EnableISystem<SpawnEnemySystem>(false);
            JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXEnter);
            await UniTask.Delay(1000, true);
            if (global.isStandAlone)
            {
                //curChapterID = 2;
                var levelId = tbchapter.Get(curChapterID).levelId;
                Log.Debug($"curChapterID {curChapterID} {levelId}");
                ResourcesSingleton.Instance.levelInfo.levelId = tbchapter.Get(curChapterID).levelId;
                int adNum = (int)tblevel.Get(levelId).reviveNum[0].x;
                int reviveNum = (int)tblevel.Get(levelId).reviveNum[0].y;

                ResourcesSingleton.Instance.levelInfo.rebirthNum = reviveNum;
                ResourcesSingleton.Instance.levelInfo.adRebirthNum = adNum;

                // JiYuSceneHelper.LoadRunTime();

                var sceneController = Common.Instance.Get<SceneController>();
                var sceneObj = sceneController.LoadSceneAsync<RunTimeScene>(SceneName.RunTime, LoadSceneMode.Additive);
                SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();
            }
            else
            {
                var levelId = tbchapter.Get(curChapterID).levelId;
                ResourcesSingleton.Instance.levelInfo.levelId = tbchapter.Get(curChapterID).levelId;
                int adNum = (int)tblevel.Get(levelId).reviveNum[0].x;
                int reviveNum = (int)tblevel.Get(levelId).reviveNum[0].y;
                ResourcesSingleton.Instance.levelInfo.rebirthNum = reviveNum;
                ResourcesSingleton.Instance.levelInfo.adRebirthNum = adNum;

                //
                // var tbguide = ConfigManager.Instance.Tables.Tbguide;
                // foreach (var guide in tbguide.DataList)
                // {
                //     if (guide.closeType == 2 && guide.buttonId == 31101)
                //     {
                //         JiYuUIHelper.FinishGuide(guide.id);
                //     }
                // }
            }

            //WebMessageHandler.Instance.AddHandler(CMD.REQUESTBATTLEID, OnClickPlayBtnBeforeResponse);
            WebMessageHandler.Instance.AddHandler(CMD.QUERYCANSTART, OnClickPlayBtnResponse);
            var battleGain = new BattleGain
            {
                LevelId = ResourcesSingleton.Instance.levelInfo.levelId
            };

            NetWorkManager.Instance.SendMessage(CMD.QUERYCANSTART, battleGain);
        }

        void OnClickPlayBtnResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERYCANSTART, OnClickPlayBtnResponse);
            var longValue = new LongValue();
            longValue.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                //return;
            }

            Log.Debug($"验证对局是否可以开始:{longValue.Value}", Color.green);
            if (longValue.Value != null && longValue.Value > 0)
            {
                ResourcesSingleton.Instance.battleData.battleId = longValue.Value;
                //this.GetParent<UIPanel_JiyuGame>().DestoryAllToggle();
                entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                switchSceneQuery = entityManager.CreateEntityQuery(typeof(SwitchSceneData));

                Log.Debug($"switch:{switchSceneQuery.CalculateEntityCount()}");

                //this.Close();
                //Close();
                var sceneController = Common.Instance.Get<SceneController>();
                var sceneObj = sceneController.LoadSceneAsync<RunTimeScene>(SceneName.RunTime, LoadSceneMode.Additive);
                SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();


                // var switchSceneData = switchSceneQuery.ToComponentDataArray<SwitchSceneData>(Allocator.Temp)[0];
                //  switchSceneData.mainScene.LoadAsync(new ContentSceneParameters()
                //  {
                //      loadSceneMode = LoadSceneMode.Single
                //  });
            }
            else
            {
                Log.Debug($"对局不可以开始{longValue.Value}", Color.green);
            }
        }

        #endregion


        void OnScrollRectDragging(Vector2 delta)
        {
            bool preferHorizontal = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y); // 判断水平滑动

            if (preferHorizontal)
            {
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                {
                    var uis = ui as UIPanel_JiyuGame;
                    uis.m_IsEndMove = false;
                    uis.m_IsDrag = true;
                    float maxWidth = -Screen.width * (uis.m_AllPageCount - 1);
                    curWidth += delta.x;
                    defaultWidth = -Screen.width * uis.unlockTags.FindIndex(a => a == tagId);

                    //float targetPosX = curWidth + delta.x;
                    uis.SetScrollRectHorNorPos((curWidth + defaultWidth) / maxWidth);
                    scrollRect.Get().vertical = false;
                    //右+ 左-
                }


                //Log.Debug($"delta:{delta}");
                // 处理水平滑动
                //scrollRect.SetEnabled(false);
            }
        }


        protected override void OnClose()
        {
            _guides.Clear();
            if (this.cts != null)
            {
                this.cts.Cancel();
                this.cts.Dispose();
            }

            RemoveTimer();
            base.OnClose();
        }
    }
}
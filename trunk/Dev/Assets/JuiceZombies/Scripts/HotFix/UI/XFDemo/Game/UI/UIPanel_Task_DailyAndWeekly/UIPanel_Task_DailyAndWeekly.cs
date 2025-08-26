//---------------------------------------------------------------------
// UnicornStudio
// Author: huangjinguo
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
using Main;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

//using System.Numerics;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Task_DailyAndWeekly)]
    internal sealed class UIPanel_Task_DailyAndWeeklyEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Task_DailyAndWeekly;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Task_DailyAndWeekly>();
        }
    }

    public partial class UIPanel_Task_DailyAndWeekly : UI, IAwake
    {
        // private UI LastBottom;
        // private UI BottomUI;
        //private List<UI> BottomList = new List<UI>();

        private float TaskH = 168;
        private float DeltaH = 20;

        private Tblanguage tblanguage;
        private Tbtask tbtask;
        private Tbtask_type tbtask_type;
        private Tbtask_score tbtask_Score;
        private Tbtask_group tbtask_group;
        private Tbtag_func tbtag_Func;
        private Tbchapter tbchapter;
        private Tbquality tbquality;
        private Tbequip_quality tbequip_Quality;


        private long timerId = 0;

        //private int timeUpdateGroup = 100;
        private int clickBoxID = 0;
        //private List<Vector2> tasks = new List<Vector2>();

        private int clickTaskID = 0;

        //private const string BattleRoot = UIPanel_Battle.Battle_Red_Point_Root;
        private bool BottomChange = false;
        private Vector2 lastScrollValue;

        private bool isInit;

        private int achieveGroupId;

        private string m_RedDotName;

        private int tagFunc = 3602;

        //private int tagFunc1 = 3603;
        private int scoreSum;
        private CancellationTokenSource cts = new CancellationTokenSource();

        private bool isDataSuccess=false;

        public async void Initialize()
        {
            await UnicornUIHelper.InitBlur(this);
            InitJson();
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.QUERYDAYANDWEEKTASK, OnDayAndWeekResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.GETTASKSCORE, OnGetTaskResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.GETALLDAILY, OnGetAllDailyResponse);
            

            
            bool hasReddot = ResourcesSingletonOld.Instance.taskRedFlag.TagFuncMap.Where((a) =>
            {
                return (a.Key == 3602 || a.Key == 3603) && a.Value == 2;
            }).Count() > 0;
            hasReddot = true;
            if (hasReddot)
            {
                NetWorkManager.Instance.SendMessage(CMDOld.GETALLDAILY);
                NetWorkManager.Instance.SendMessage(CMDOld.PASSEXP);
            }
            else
            {
                NetWorkManager.Instance.SendMessage(CMDOld.QUERYDAYANDWEEKTASK);
                Debug.Log($"3-1 QUERYDAYANDWEEKTASK");
            }
            GetFromReference(KBg).GetButton().OnClick?.Add(() => { UnicornUIHelper.DestoryAllTips(); });




        }

       


        private async UniTask<bool> IsGetData()
        {
            while (!isDataSuccess)
            {
                await UniTask.Delay(500, cancellationToken: cts.Token);
            }

            return true;
        }

        void InitNode()
        {
            InitRedDot();
            //this.GetFromReference(KBtn_Tip_Close).SetActive(false);
            //DataInit();
            BottomInit().Forget();
            //SetContentH(DailyNum);
            //CreateTask(100).Forget();
            // if (ResourcesSingletonOld.Instance.gamePassStart)
            // {
            //     WebMessageHandlerOld.Instance.AddHandler(3, 1, OnRoleTaskInfoResponse);
            //     NetWorkManager.Instance.SendMessage(3, 1);
            // }

            TopScoreInit();
            this.GetFromReference(KScrollView).GetXButton().OnClick.Add(() => { UnicornUIHelper.DestoryAllTips(); });
            this.GetFromReference(KScrollView).GetXScrollRect().OnDrag.Add((f) => { UnicornUIHelper.DestoryAllTips(); });
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtask = ConfigManager.Instance.Tables.Tbtask;
            tbtask_type = ConfigManager.Instance.Tables.Tbtask_type;
            tbtask_Score = ConfigManager.Instance.Tables.Tbtask_score;
            tbquality = ConfigManager.Instance.Tables.Tbquality;
            tbtag_Func = ConfigManager.Instance.Tables.Tbtag_func;
            tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            tbequip_Quality = ConfigManager.Instance.Tables.Tbequip_quality;
            tbtask_group = ConfigManager.Instance.Tables.Tbtask_group;
        }

        private void InitRedDot()
        {
            m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);
            var itemStr1 = $"{m_RedDotName}|Bottom100";
            var itemStr2 = $"{m_RedDotName}|Bottom200";
            RedDotManager.Instance.InsterNode(itemStr1);
            RedDotManager.Instance.InsterNode(itemStr2);
        }

        private void OnGetTaskResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            TaskResult taskResult = new TaskResult();
            taskResult.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            if (taskResult.Res == "true")
            {
                if (taskResult.Reward.Count != 0)
                {
                    List<Vector3> reList = new List<Vector3>();
                    foreach (var itemstr in taskResult.Reward)
                    {
                        reList.Add(UnityHelper.StrToVector3(itemstr));
                    }

                    UIHelper.Create(UIType.UICommon_Reward, reList);
                }

                NetWorkManager.Instance.SendMessage(CMDOld.QUERYDAYANDWEEKTASK);
                Debug.LogError($"3-1 QUERYDAYANDWEEKTASK");
            }
        }

        /// <summary>
        /// init data
        /// </summary>
        private void DataInit()
        {
            return;
            int i = 0;
            int j = 0;
            foreach (task t in tbtask.DataList)
            {
                if (t.group == 100)
                {
                    i++;
                }

                if (t.group == 200)
                {
                    j++;
                }
            }

            //DailyNum = i;
            //WeeklyNum = j;
        }

        public void Refresh()
        {
            //this.GetFromReference(KBtn_Tip_Close).SetActive(false);
            //DataInit();
            BottomInit().Forget();
            //SetContentH(DailyNum);
            //CreateTask(100).Forget();
            // if (ResourcesSingletonOld.Instance.gamePassStart)
            // {
            //     WebMessageHandlerOld.Instance.AddHandler(3, 1, OnRoleTaskInfoResponse);
            //     NetWorkManager.Instance.SendMessage(3, 1);
            // }

            //TopScoreInit();
        }

        private async void OnGetAllDailyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            //WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.QUERYDAYANDWEEKTASK, OnDayAndWeekResponse);
            ClickClaimRes clickClaimRes = new ClickClaimRes();
            clickClaimRes.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                isDataSuccess = false;
                Log.Debug("dayAndWeekInfo IsEmpty", Color.red);
                return;
            }
            isDataSuccess=true;

            GetFromReference(KTop).GetComponent<CanvasGroup>().alpha = 0;
            if (ResourcesSingletonOld.Instance.taskRedFlag.TagFuncMap.ContainsKey(3602))
            {
                ResourcesSingletonOld.Instance.taskRedFlag.TagFuncMap[3602] = 1;
            }

            if (ResourcesSingletonOld.Instance.taskRedFlag.TagFuncMap.ContainsKey(3603))
            {
                ResourcesSingletonOld.Instance.taskRedFlag.TagFuncMap[3603] = 1;
            }


            ResourcesSingletonOld.Instance.dayAndWeekInfo = clickClaimRes.TaskInfo;
            Log.Debug($"OnGetAllDailyResponse:{ResourcesSingletonOld.Instance.dayAndWeekInfo.ToString()}", Color.green);
            if (!isInit)
            {
                isInit = true;
                InitNode();
            }
            else
            {
                Refresh();
            }

            if (clickClaimRes.Rewards.Count > 0)
            {
                await UniTask.Delay(1000, cancellationToken: cts.Token);
                var reward = UnicornUIHelper.TurnStrReward2List(clickClaimRes.Rewards);
                var ui = await UIHelper.CreateAsync(UIType.UICommon_Reward, reward);
            }
        }

        private void OnDayAndWeekResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            //WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.QUERYDAYANDWEEKTASK, OnDayAndWeekResponse);
            RoleTaskInfo roleTaskInfo = new RoleTaskInfo();
            roleTaskInfo.MergeFrom(e.data);

            ResourcesSingletonOld.Instance.dayAndWeekInfo = roleTaskInfo;
            if (e.data.IsEmpty)
            {
                Log.Debug("dayAndWeekInfo IsEmpty", Color.red);
                return;
            }
            Log.Debug($"OnDayAndWeekResponse:{roleTaskInfo.ToString()}", Color.green);
            if (!isInit)
            {
                isInit = true;
                InitNode();
            }
            else
            {
                Refresh();
            }


            // foreach (var t in roleTaskInfo.TaskInfoList)
            // {
            //     //status为领取状态0未领取，1领取， para任务参数
            //     ResourcesSingletonOld.Instance.dayWeekTask.tasks.Add(t.Id, new Vector2(t.Status, t.Para));
            // }
            //
            // foreach (var s in roleTaskInfo.TaskScoreList)
            // {
            //     //status为领取状态0未领取，1领取， valid生效与否0未生效1生效
            //     ResourcesSingletonOld.Instance.dayWeekTask.boxes.Add(s.Id, new Vector2(s.Status, s.Valid));
            // }
        }

        /// <summary>
        /// create task ui
        /// </summary>
        /// <param name="groupID"></param>
        public async UniTaskVoid CreateTask(int groupID)
        {
            scoreSum = 0;
            //UIDic.Clear();

            //timeUpdateGroup = groupID;
            switch (groupID)
            {
                case 100:
                    this.GetFromReference(KText_Activation).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("task_daily_name").current);
                    break;
                case 200:
                    this.GetFromReference(KText_Activation).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("task_weekly_name").current);
                    break;
            }

            var KContent = GetFromReference(KScrollView).GetXScrollRect().Content;
            var contentList = KContent.GetList();
            float contentW = KContent.GetRectTransform().Width();

            //TaskSortList(groupID);
            contentList.Clear();
            var list = ResourcesSingletonOld.Instance.dayAndWeekInfo.TaskInfoList.Where(a =>
            {
                return a.Group == groupID;
            });
            foreach (var gameTask in list)
            {
                //Log.Debug($"gameTask {gameTask.ToString()}");
                var task = tbtask.Get(gameTask.Id);
                int state = 0;
                if (gameTask.Status == 1)
                {
                    state = 2;
                }
                else if (gameTask.Status == 0)
                {
                    if (gameTask.Para >= task.para[0])
                    {
                        state = 0;
                    }
                    else
                    {
                        state = 1;
                    }
                }

                var taskUI =
                    await contentList.CreateWithUITypeAsync(UIType.UISubPanel_Task_DAndW, gameTask.Id, state,
                        false);

                taskUI.GetRectTransform().SetAnchoredPositionZ(0);
                var KImg_LeftBg = taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_LeftBg);
                var KPos_Right = taskUI.GetFromReference(UISubPanel_Task_DAndW.KPos_Right);
                var KBg_Mask = taskUI.GetFromReference(UISubPanel_Task_DAndW.KBg_Mask);
                var reUI = taskUI.GetFromReference(UISubPanel_Task_DAndW.KCommon_RewardItem);
                reUI.SetActive(false);
                KBg_Mask.SetActive(false);
                taskUI.GetRectTransform().SetWidth(contentW);
                string nameStr = nameString(gameTask.Id);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KPos).SetActive(false);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KPos2).SetActive(true);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_Task_Name2).GetTextMeshPro().SetTMPText(nameStr);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_Task_Num2).SetActive(false);


                // UnicornUIHelper.SetRewardIconAndCountText(task.reward[0], reUI);
                // UnicornUIHelper.SetRewardOnClick(task.reward[0], reUI);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_ScoreNum).GetTextMeshPro()
                    .SetTMPText(task.score.ToString());
                // taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_ScoreNum2).GetTextMeshPro()
                //     .SetTMPText(task.score.ToString());

                int serverPara = gameTask.Para;
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_ProgressBar2).GetTextMeshPro()
                    .SetTMPText(gameTask.Para.ToString() + "/" + tbtask.Get(gameTask.Id).para[0]);

                // if (task.Para > tbtask.Get(task.TaskId).para[0])
                // {
                //     task.Para = tbtask.Get(task.TaskId).para[0];
                // }

                float rightOffsetHelp =
                    (float)(tbtask.Get(gameTask.Id).para[0] - serverPara) /
                    (float)tbtask.Get(gameTask.Id).para[0];
                //float wHelp = taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar_Bg2).GetRectTransform().Width();

                //            taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar2).GetRectTransform().SetOffsetWithRight(-wHelp * rightOffsetHelp);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar2).GetXImage()
                    .SetFillAmount(1 - rightOffsetHelp);

                bool canReceive = gameTask.Para >= task.para[0] && gameTask.Status == 0;

                Log.Debug($"state{state} gameTask{gameTask}");
                switch (state)
                {
                    case 0:
                        //可领取
                        //taskUI.GetFromReference(UISubPanel_Task_DAndW.KBg).GetXImage().SetAlpha(1);
                        //UnicornUIHelper.SetRewardIconAndCountText(task.reward[0], reUI);
                        //SetTaskUIAlpha(taskUI);
                        var itemStr = $"{m_RedDotName}|Bottom{achieveGroupId}";
                        RedDotManager.Instance.SetRedPointCnt(itemStr, 1);

                        taskUI.GetFromReference(UISubPanel_Task_DAndW.KBtn).SetActive(true);
                        taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_Dui).SetActive(false);
                        taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_Receive).GetTextMeshPro()
                            .SetTMPText(tblanguage.Get("common_state_gain").current);
                        KPos_Right.GetImage().SetSpriteAsync("daily_scrollview_finished_icon", false).Forget();
                        KImg_LeftBg.GetImage().SetSpriteAsync("daily_scrollview_finished", false).Forget();
                        break;
                    case 1:
                        //前往
                        // taskUI.GetFromReference(UISubPanel_Task_DAndW.KBg).GetXImage()
                        //     .SetSpriteAsync(picConnotClaim, false);
                        //taskUI.GetFromReference(UISubPanel_Task_DAndW.KBg).GetXImage().SetAlpha(1);
                        //UnicornUIHelper.SetRewardIconAndCountText(task.reward[0], reUI);
                        //SetTaskUIAlpha(taskUI);
                        taskUI.GetFromReference(UISubPanel_Task_DAndW.KBtn).SetActive(true);
                        taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_Dui).SetActive(false);
                        taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_Receive).GetTextMeshPro()
                            .SetTMPText(tblanguage.Get("common_state_goto").current);

                        KPos_Right.GetImage().SetSpriteAsync("daily_scrollview_goto_icon", false).Forget();
                        KImg_LeftBg.GetImage().SetSpriteAsync("daily_scrollview", false).Forget();
                        break;
                    case 2:
                        //已领取
                        // taskUI.GetFromReference(UISubPanel_Task_DAndW.KBg).GetXImage()
                        //     .SetSpriteAsync(picClaimed, false);
                        //taskUI.GetFromReference(UISubPanel_Task_DAndW.KBg).GetXImage().SetAlpha(0.3f);
                        //UnicornUIHelper.SetRewardIconAndCountText(task.reward[0], reUI, 0.3f);
                        scoreSum += gameTask.Score;
                        KBg_Mask.SetActive(true);
                        taskUI.GetFromReference(UISubPanel_Task_DAndW.KBtn).SetActive(false);
                        taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_Dui).SetActive(true);
                        KPos_Right.GetImage().SetSpriteAsync("daily_scrollview_goto_icon", false).Forget();
                        KImg_LeftBg.GetImage().SetSpriteAsync("daily_scrollview", false).Forget();
                        break;
                    // default:
                    //     taskUI.GetFromReference(UISubPanel_Task_DAndW.KBg).GetXImage()
                    //         .SetSpriteAsync(picConnotClaim, false);
                    //     taskUI.GetFromReference(UISubPanel_Task_DAndW.KBg).GetXImage().SetAlpha(1);
                    //     UnicornUIHelper.SetRewardIconAndCountText(task.reward[0], reUI);
                    //     SetTaskUIAlpha(taskUI);
                    //     taskUI.GetFromReference(UISubPanel_Task_DAndW.KBtn).SetActive(true);
                    //     taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_Dui).SetActive(false);
                    //     taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_Receive).GetTextMeshPro()
                    //         .SetTMPText(tblanguage.Get("common_state_goto").current);
                    //     break;
                }

                var btn = taskUI.GetFromReference(UISubPanel_Task_DAndW.KBtn);
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
                {
                    UnicornUIHelper.DestoryAllTips();
                    switch (state)
                    {
                        case 0:
                            //可领取
                            NetWorkManager.Instance.SendMessage(CMDOld.GETTASKSCORE, new IntValue
                            {
                                Value = gameTask.Id
                            });
                            break;
                        case 1:
                            //前往

                            var taskType = tbtask_type.Get(tbtask.Get(gameTask.Id).type);
                            Log.Debug($"前往Type{taskType.goto0} {taskType.id}");

                            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Task_DailyAndWeekly, out var ui))
                            {
                                ui.Dispose();
                            }

                            Close();
                            UnicornUIHelper.GoToSomePanel(taskType.goto0);
                            break;
                        case 2:

                            //已领取
                            break;
                    }
                });
            }

            contentList.Sort((a, b) =>
            {
                var uia = a as UISubPanel_Task_DAndW;
                var uib = b as UISubPanel_Task_DAndW;
                var taska = tbtask.Get(uia.taskId);
                var taskb = tbtask.Get(uib.taskId);
                if (uia.state != uib.state)
                {
                    return uia.state.CompareTo(uib.state);
                }

                return taska.sort.CompareTo(taskb.sort);
            });

            int i = 0;
            foreach (var task in contentList.Children)
            {
                i++;
                UnicornTweenHelper.SetEaseAlphaAndPosB2U(task.GetFromReference(UISubPanel_Task_DAndW.KBg), 0, 50,
                    cts.Token,
                    0.35f + 0.02f * i, true, true);
            }


            TopScoreBoxSet(groupID).Forget();
            UnicornTweenHelper.SetEaseAlphaAndPosUtoB(GetFromReference(KTop), -220, 500, cts.Token,
                0.35f, true, true);

            //UIandTaskDic.Add(taskUI, t);
            //TODO:Red
            // RedPointMgr.instance.Init(thisRedRoot, "task" + t.id.ToString(),
            //     (RedPointState state, int data) =>
            //     {
            //         if (state == RedPointState.Show)
            //         {
            //             taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_RedPoint).SetActive(true);
            //         }
            //         else
            //         {
            //             taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_RedPoint).SetActive(false);
            //         }
            //     });
            //List<UI> uilist = new List<UI>();
            // foreach (var t in tasks)
            // {
            //     var ui = await list.CreateWithUITypeAsync(UIType.UISubPanel_Task_DAndW, false);
            //     var KImg_LeftBg = ui.GetFromReference(UISubPanel_Task_DAndW.KImg_LeftBg);
            //     var KPos_Right = ui.GetFromReference(UISubPanel_Task_DAndW.KPos_Right);
            //     var KBg_Mask = ui.GetFromReference(UISubPanel_Task_DAndW.KBg_Mask);
            //     KBg_Mask.SetActive(false);
            //
            //     uilist.Add(ui);
            //     // ui.GetRectTransform().SetWidth(this.GetFromReference(KScrollView).GetScrollRect().Content
            //     //     .GetRectTransform().Width());
            //     ui.GetFromReference(UISubPanel_Task_DAndW.KText_Task_Name).GetTextMeshPro()
            //         .SetTMPText(nameString((int)t.x));
            //     ui.GetFromReference(UISubPanel_Task_DAndW.KText_Task_Num).GetTextMeshPro()
            //         .SetTMPText((ResourcesSingletonOld.Instance.dayWeekTask.tasks[(int)t.x].y).ToString() + "/" +
            //                     tbtask.Get((int)t.x).para[0]);
            //     ui.GetFromReference(UISubPanel_Task_DAndW.KText_ProgressBar).GetTextMeshPro()
            //         .SetTMPText((ResourcesSingletonOld.Instance.dayWeekTask.tasks[(int)t.x].y).ToString() + "/" +
            //                     tbtask.Get((int)t.x).para[0]);
            //     ui.GetFromReference(UISubPanel_Task_DAndW.KText_ScoreNum).GetTextMeshPro()
            //         .SetTMPText(tbtask.Get((int)t.x).score.ToString());
            //     ui.GetFromReference(UISubPanel_Task_DAndW.KText_ScoreNum2).GetTextMeshPro()
            //         .SetTMPText(tbtask.Get((int)t.x).score.ToString());
            //     ui.GetFromReference(UISubPanel_Task_DAndW.KText_Receive).GetTextMeshPro()
            //         .SetTMPText(tblanguage.Get("common_state_gain").current);
            //
            //     var claimBtn = ui.GetFromReference(UISubPanel_Task_DAndW.KBtn);
            //     //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn);
            //
            //     //add every daily task's red point
            //     //RedPointMgr.instance.Add(Battle_Red_Point_Root, "task" + t.id.ToString(), "Task_Center_Daily", RedPointType.Once);
            //
            //     //string taskHelpStr = "";
            //     if (groupID == 100)
            //     {
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KProgressBar_Bg).SetActive(false);
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KText_Task_Num).SetActive(true);
            //     }
            //     else
            //     {
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KProgressBar_Bg).SetActive(true);
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KText_Task_Num).SetActive(false);
            //     }
            //
            //     //set daily task's red point performance
            //     //RedPointMgr.instance.Init(BattleRoot, "task" + ((int)t.x).ToString(), (RedPointState state, int data) =>
            //     //{
            //     //    if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Task_DailyAndWeekly, out UI uuii))
            //     //    {
            //     //        if (timeUpdateGroup == groupID)
            //     //        {
            //     //            ui.GetFromReference(UISubPanel_Task_DAndW.KImg_RedPoint)
            //     //                .SetActive(state == RedPointState.Show);
            //     //        }
            //     //    }
            //     //}, ui.GetFromReference(UISubPanel_Task_DAndW.KBtn).GetXButton());
            //
            //     if (ResourcesSingletonOld.Instance.dayWeekTask.tasks[(int)t.x].y < tbtask.Get((int)t.x).para[0])
            //     {
            //         float bgw = ui.GetFromReference(UISubPanel_Task_DAndW.KProgressBar_Bg).GetRectTransform().Width();
            //         float rogressBarW = bgw *
            //                             (tbtask.Get((int)t.x).para[0] -
            //                              ResourcesSingletonOld.Instance.dayWeekTask.tasks[(int)t.x].y) /
            //                             tbtask.Get((int)t.x).para[0];
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KProgressBar).GetRectTransform()
            //             .SetOffsetWithRight(-rogressBarW);
            //     }
            //     else
            //     {
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KProgressBar).GetRectTransform().SetOffsetWithRight(0);
            //     }
            //
            //     //Debug.Log(t.y);
            //     //set image by status
            //     if ((int)t.y == 0)
            //     {
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KBtn).SetActive(true);
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KImg_Dui).SetActive(false);
            //         //Unclaimed
            //         if (ResourcesSingletonOld.Instance.dayWeekTask.tasks[(int)t.x].y >=
            //             tbtask.Get((int)t.x).para[0])
            //         {
            //             //Can be claimed
            //             //await ui.GetFromReference(UISubPanel_Task_DAndW.KBg).GetImage().SetSpriteAsync(picClaim, false);
            //             ui.GetFromReference(UISubPanel_Task_DAndW.KBtn).GetXButton().SetEnabled(true);
            //             //RedPointMgr.instance.SetState(BattleRoot, "task" + ((int)t.x).ToString(), RedPointState.Show);
            //             UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
            //             {
            //                 //claim
            //                 ClearTip();
            //                 WebMessageHandlerOld.Instance.AddHandler(3, 2, OnCliamTaskResponse);
            //                 clickTaskID = (int)t.x;
            //                 IntValue intValue = new IntValue();
            //                 intValue.Value = (int)t.x;
            //                 NetWorkManager.Instance.SendMessage(3, 2, intValue);
            //             });
            //         }
            //         else
            //         {
            //             //RedPointMgr.instance.SetState(BattleRoot, "task" + ((int)t.x).ToString(), RedPointState.Hide);
            //             ui.GetFromReference(UISubPanel_Task_DAndW.KText_Receive).GetTextMeshPro()
            //                 .SetTMPText(tblanguage.Get("common_state_goto").current);
            //             //Can not be claimed
            //
            //             ui.GetFromReference(UISubPanel_Task_DAndW.KBtn).GetXButton().SetEnabled(true);
            //             UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
            //             {
            //                 //jump
            //                 ClearTip();
            //             });
            //         }
            //
            //         #region Change image resource
            //
            //         //SetTaskUIAlpha(ui);
            //
            //         #endregion
            //     }
            //     else
            //     {
            //         //Received
            //         //RedPointMgr.instance.SetState(BattleRoot, "task" + ((int)t.x).ToString(), RedPointState.Hide);
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KBtn).GetXButton().SetEnabled(false);
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KBtn).SetActive(false);
            //         ui.GetFromReference(UISubPanel_Task_DAndW.KImg_Dui).SetActive(true);
            //         //scoreSum += tbtask.Get((int)t.x).score;
            //
            //         #region Change image resource
            //
            //         //SetTaskUIAlpha(ui, 0.3f);
            //
            //         #endregion
            //     }
            //
            //     await ui.GetFromReference(UISubPanel_Task_DAndW.KImg_Task).GetImage()
            //         .SetSpriteAsync(tbtask_type.Get(tbtask.Get((int)t.x).type).icon, false);
            // }

            // for (int i = 0; i < uilist.Count; i++)
            // {
            //     uilist[i].GameObject.transform.SetSiblingIndex(i);
            // }
            //
            // uilist.Clear();

            //UnicornUIHelper.ForceRefreshLayout(this.GetFromReference(KScrollView).GetScrollRect().Content);
        }

        private void SetTaskUIAlpha(UI taskUI, float alpha = 1)
        {
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_Task).GetXImage().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar_Bg).GetXImage().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar).GetXImage().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar).GetXImage().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar2).GetXImage().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar_Bg2).GetXImage().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_ProgressBar).GetTextMeshPro().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_ProgressBar2).GetTextMeshPro().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_Task_Name).GetTextMeshPro().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_Task_Name2).GetTextMeshPro().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_Dui).GetXImage().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_ScoreNum2).GetTextMeshPro().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_Score2).GetXImage().SetAlpha(alpha);
        }


        /// <summary>
        /// Claim task response
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCliamTaskResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(3, 2, OnCliamTaskResponse);
            TaskResult taskResult = new TaskResult();
            //StringValueList stringValueList = new StringValueList();
            taskResult.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(taskResult);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            #region change data

            //ResourcesSingletonOld.Instance.dayWeekTask.tasks[clickTaskID] = new Vector2(1,
            //    ResourcesSingletonOld.Instance.dayWeekTask.tasks[clickTaskID].y);
            //TaskCreate(timeUpdateGroup);
            //foreach (var itemstr in stringValueList.Values)
            //{
            //    UnicornUIHelper.AddReward(UnityHelper.StrToVector3(itemstr));
            //}

            if (taskResult.Res == "true")
            {
                if (taskResult.Reward.Count != 0)
                {
                    List<Vector3> reList = new List<Vector3>();
                    foreach (var itemstr in taskResult.Reward)
                    {
                        reList.Add(UnityHelper.StrToVector3(itemstr));
                    }

                    UIHelper.Create(UIType.UICommon_Reward, reList);
                }

                int delayTime = 800;

                UIHelper.CreateAsync(UIType.UIResource, new Vector3(-1, 0, 10));

                await UniTask.Delay(delayTime);

                WebMessageHandlerOld.Instance.AddHandler(3, 1, OnRoleTaskInfoResponse);
                NetWorkManager.Instance.SendMessage(3, 1);
            }

            //ResourcesSingletonOld.Instance.UpdateResourceUI();

            #endregion
        }


        /// <summary>
        /// Information change response
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRoleTaskInfoResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(3, 1, OnRoleTaskInfoResponse);
            RoleTaskInfo roleTaskInfo = new RoleTaskInfo();
            roleTaskInfo.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(roleTaskInfo);
            if (e.data.IsEmpty)
            {
                Log.Debug("3-1RoleTaskInfo.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingletonOld.Instance.dayWeekTask.tasks.Clear();
            ResourcesSingletonOld.Instance.dayWeekTask.boxes.Clear();

            foreach (var t in roleTaskInfo.TaskInfoList)
            {
                //status为领取状态0未领取，1领取， para任务参数
                ResourcesSingletonOld.Instance.dayWeekTask.tasks.Add(t.Id, new Vector2(t.Status, t.Para));
            }

            foreach (var s in roleTaskInfo.TaskScoreList)
            {
                //status为领取状态0未领取，1领取， valid生效与否0未生效1生效
                ResourcesSingletonOld.Instance.dayWeekTask.boxes.Add(s.Id, new Vector2(s.Status, s.Valid));
            }

            //CreateTask(timeUpdateGroup);
        }

        /// <summary>
        /// set task name
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        private string nameString(int taskID)
        {
            string nameHelpStr = tblanguage.Get(tbtask_type.Get(tbtask.Get(taskID).type).desc[0]).current;
            string namestr = nameHelpStr.Replace("{0}", tbtask.Get(taskID).para[0].ToString());
            if (tbtask.Get(taskID).para.Count != 1)
            {
                switch (tbtask.Get(taskID).type)
                {
                    case 1022:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbtag_Func.Get(tbtask.Get(taskID).para[1]).name).current);
                        break;

                    case 2015:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbchapter.Get(tbtask.Get(taskID).para[1]).name).current);
                        break;

                    case 2025:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbchapter.Get(tbtask.Get(taskID).para[1]).name).current);
                        break;

                    case 3044:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbquality.Get(tbequip_Quality.Get(tbtask.Get(taskID).para[1]).type).name)
                                .current);
                        break;

                    case 3051:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbquality.Get(tbequip_Quality.Get(tbtask.Get(taskID).para[1]).type).name)
                                .current);
                        break;

                    case 3052:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbquality.Get(tbequip_Quality.Get(tbtask.Get(taskID).para[1]).type).name)
                                .current);
                        break;

                    case 3053:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbquality.Get(tbequip_Quality.Get(tbtask.Get(taskID).para[1]).type).name)
                                .current);
                        break;

                    case 3061:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbquality.Get(tbequip_Quality.Get(tbtask.Get(taskID).para[1]).type).name)
                                .current);
                        break;

                    case 3062:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbquality.Get(tbequip_Quality.Get(tbtask.Get(taskID).para[1]).type).name)
                                .current);
                        break;

                    case 3063:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbquality.Get(tbequip_Quality.Get(tbtask.Get(taskID).para[1]).type).name)
                                .current);
                        break;

                    default:
                        Debug.Log("task type para is wrong");
                        break;
                }
            }

            return namestr;
        }

        /// <summary>
        /// Set content height based on the number of tasks
        /// </summary>
        /// <param name="num">任务数量</param>
        private void SetContentH(int num)
        {
            // return;
            // this.GetFromReference(KScrollView).GetXScrollRect().Content.GetRectTransform()
            //     .SetHeight((TaskH + 20) * num - DeltaH);
            // this.GetFromReference(KScrollView).GetXScrollRect().Content.GetRectTransform().SetOffsetWithLeft(0);
            // this.GetFromReference(KScrollView).GetXScrollRect().Content.GetRectTransform().SetOffsetWithRight(0);
            // SetMovementType((TaskH + DeltaH) * num - DeltaH);
        }

        /// <summary>
        /// Set whether the scrolling interface can scroll based on the height of the content
        /// </summary>
        /// <param name="ContentH">content的高度</param>
        private void SetMovementType(float ContentH)
        {
            // if (ContentH > this.GetFromReference(KScrollView).GetRectTransform().Height())
            // {
            //     this.GetFromReference(KScrollView).GetComponent<XScrollRect>().movementType =
            //         ScrollRect.MovementType.Elastic;
            // }
            // else
            // {
            //     this.GetFromReference(KScrollView).GetComponent<XScrollRect>().movementType =
            //         ScrollRect.MovementType.Clamped;
            // }
        }

        /// <summary>
        /// Initialize bottom selection and return buttons
        /// </summary>
        private async UniTaskVoid BottomInit()
        {
            var KCommon_Bottom = GetFromReference(UIPanel_Task_DailyAndWeekly.KCommon_Bottom);

            var KScrollView_Item0 = KCommon_Bottom.GetFromReference(UICommon_Bottom.KScrollView_Item0);
            var KBtn_Close = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_Close);
            var KBtn_TitleInfo = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_TitleInfo);
            var KText_BottomTitle = KCommon_Bottom.GetFromReference(UICommon_Bottom.KText_BottomTitle);
            KBtn_TitleInfo.SetActive(false);
            KText_BottomTitle.SetActive(false);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, () => { Close(); });
            var content = KScrollView_Item0.GetScrollRect().Content;
            var bottomList = content.GetList();
            bottomList.Clear();
            UnicornTweenHelper.SetEaseAlphaAndPosUtoB(KScrollView_Item0, 0, 50f, cts.Token);

            achieveGroupId = 100;
            for (int i = 0; i < 2; i++)
            {
                int index = i;
                int groupId = (index + 1) * 100;
                var taskGroup = tbtask_group.Get(groupId);
                var ui =
                    await bottomList.CreateWithUITypeAsync(UIType.UICommon_BottomBtn, groupId, false);
                ui.GetRectTransform().SetAnchoredPositionZ(0);

                var KText_Btn = ui.GetFromReference(UICommon_BottomBtn.KText_Btn);
                var KBg_Mask = ui.GetFromReference(UICommon_BottomBtn.KBg_Mask);
                var KBg_Mask1 = ui.GetFromReference(UICommon_BottomBtn.KBg_Mask1);
                var KImg_RedDot = ui.GetFromReference(UICommon_BottomBtn.KImg_RedDot);
                var itemStr = $"{m_RedDotName}|Bottom{groupId}";

                KImg_RedDot.SetActive(HasRedDot(groupId));

                // RedDotManager.Instance.AddListener(itemStr, a =>
                // {
                //     KImg_RedDot.SetActive(a > 0);
                // });

                KBg_Mask.SetActive(true);
                KBg_Mask1.SetActive(true);
                KText_Btn.GetTextMeshPro()
                    .SetTMPText(tblanguage.Get($"func_{3602 + i}_name").current);

                if (achieveGroupId == groupId)
                {
                    KBg_Mask.SetActive(false);
                    KBg_Mask1.SetActive(false);
                    CreateTask(achieveGroupId).Forget();
                }

                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ui, () =>
                {
                    cts.Cancel();
                    cts = new CancellationTokenSource();
                    UnicornUIHelper.DestoryAllTips();
                    if (achieveGroupId == groupId)
                    {
                        return;
                    }

                    foreach (var child in bottomList.Children)
                    {
                        var uichild = child as UICommon_BottomBtn;

                        if (uichild.id == achieveGroupId)
                        {
                            var KBg_Mask = uichild.GetFromReference(UICommon_BottomBtn.KBg_Mask);
                            var KBg_Mask1 = uichild.GetFromReference(UICommon_BottomBtn.KBg_Mask1);
                            KBg_Mask.SetActive(true);
                            KBg_Mask1.SetActive(true);
                            //uichild.GetRectTransform().DoScale2(new Vector2(1f, 1f), 0.2f);
                            break;
                        }
                    }

                    KBg_Mask.SetActive(false);
                    KBg_Mask1.SetActive(false);
                    achieveGroupId = groupId;

                    CreateTask(achieveGroupId).Forget();
                });

                // itemUI.GetXButton().OnClick.Add(() =>
                // {
                //     ClearTip();
                //     if (LastBottom != itemUI)
                //     {
                //         LastBottom.GetRectTransform().DoScale2(new Vector2(1f, 1f), 0.2f);
                //         itemUI.GetRectTransform().DoScale2(new Vector2(1.1f, 1.1f), 0.2f);
                //         LastBottom = itemUI;
                //         BottomChange = true;
                //         //DestroyTaskList();
                //     }
                // });
            }

            bottomList.Sort((a, b) =>
            {
                var uia = a as UICommon_BottomBtn;
                var uib = b as UICommon_BottomBtn;
                return uia.id.CompareTo(uib.id);
            });
        }

        private bool HasRedDot(int groupId)
        {
            bool taskRed = ResourcesSingletonOld.Instance.dayAndWeekInfo.TaskInfoList.Where(gameTask =>
            {
                var task = tbtask.Get(gameTask.Id);
                int state = 0;
                if (gameTask.Status == 1)
                {
                    state = 2;
                }
                else if (gameTask.Status == 0)
                {
                    if (gameTask.Para >= task.para[0])
                    {
                        state = 0;
                    }
                    else
                    {
                        state = 1;
                    }
                }

                return gameTask.Group == groupId && state == 0;
            }).Count() > 0;
            int tagFunc = 0;
            if (groupId == 100)
            {
                tagFunc = 3602;
            }
            else if (groupId == 200)
            {
                tagFunc = 3603;
            }

            bool scoreRed = ResourcesSingletonOld.Instance.dayAndWeekInfo.TaskScoreList
                .Where(a => a.TagFunc == tagFunc && a.Status == 2)
                .Count() > 0;


            return taskRed || scoreRed;
        }

        /// <summary>
        /// Initialize the top point treasure chest
        /// </summary>
        private void TopScoreInit()
        {
            this.GetFromReference(KText_Activation).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("task_daily_name").current);
            //Set timer
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(200, Update);
        }

        /// <summary>
        /// Refresh Countdown
        /// </summary>
        private void Update()
        {
            long timeS = 0;
            string timeStr = "";
            switch (achieveGroupId)
            {
                case 100:
                    timeS = TimeHelper.GetToTomorrowTime();
                    timeStr = UnicornUIHelper.GeneralTimeFormat(new int4(2, 3, 2, 1), timeS);
                    this.GetFromReference(KText_Count_Down).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("shop_daily_countdown_text").current + timeStr);
                    break;
                case 200:
                    timeS = TimeHelper.GetToNextWeekTime();
                    timeStr = UnicornUIHelper.GeneralTimeFormat(new int4(2, 4, 2, 2), timeS);
                    this.GetFromReference(KText_Count_Down).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("shop_daily_countdown_text").current + timeStr);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Change Top Point Treasure Box Related
        /// </summary>
        /// <param name="groupID"></param>
        private async UniTaskVoid TopScoreBoxSet(int groupID)
        {
            //return;
            int tagFunc = 0;
            if (groupID == 100)
            {
                tagFunc = 3602;
            }
            else if (groupID == 200)
            {
                tagFunc = 3603;
            }

            var rewardList = ResourcesSingletonOld.Instance.dayAndWeekInfo.TaskScoreList.Where(a =>
            {
                return a.TagFunc == tagFunc;
            }).ToList();
            rewardList.Sort((a, b) => { return a.Id.CompareTo(b.Id); });

            var sumScore = ResourcesSingletonOld.Instance.dayAndWeekInfo.TaskInfoList.Where(a =>
                a.TagFunc == tagFunc && a.Status == 1).Sum(a => a.Score);


            // foreach (var taskScore in ResourcesSingletonOld.Instance.dayAndWeekInfo.TaskScoreList)
            // {
            //     taskScore.
            //     if (tbtask[t.Key].group == groupID)
            //     {
            //         if (t.Value.x != 0)
            //         {
            //             scoreSum += tbtask.Get(t.Key).score;
            //         }
            //     }
            // }


            //var tslist = tbtask_Score.DataList.Where(a => a.tagFunc == tagFunc).ToList();

            float scoreMax = 0;
            scoreMax = rewardList.Max(a => tbtask_Score.Get(a.Id).score);
            //tslist.Sort(delegate(task_score ts1, task_score ts2) { return ts1.score.CompareTo(ts2.score); });
            float ProgressBarW = this.GetFromReference(KProgressBar_Bg).GetRectTransform().Width();
            //int scoreSum = 0;
            var scoreBoxList = this.GetFromReference(KPos_ScoreList).GetList();
            scoreBoxList.Clear();
            foreach (var VARIABLE in rewardList)
            {
                Log.Debug($"rewardList {VARIABLE.ToString()}", Color.green);
            }

            foreach (var gameTaskScore in rewardList)
            {
                var taskScore = tbtask_Score.Get(gameTaskScore.Id);
                if (taskScore.score <= sumScore && gameTaskScore.Status != 1)
                {
                    gameTaskScore.Status = 2;
                }

                var uibox = await scoreBoxList.CreateWithUITypeAsync(UIType.UISubPanel_Task_Score_Box, gameTaskScore.Id,
                    false);

                uibox.GetRectTransform().SetAnchoredPositionY(0);
                uibox.GetRectTransform()
                    .SetAnchoredPositionX(ProgressBarW * (-0.5f + (float)taskScore.score / scoreMax) - 30f);
                uibox.GetFromReference(UISubPanel_Task_Score_Box.KText_Box_Score).GetTextMeshPro()
                    .SetTMPText(taskScore.score.ToString());

                var claimBtn = uibox.GetFromReference(UISubPanel_Task_Score_Box.KBtn_Score_Box);
                var KImg_Claimed = uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed);
                KImg_Claimed.SetActive(false);
                if (gameTaskScore.Status == 0 || gameTaskScore.Status == null)
                {
                    uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box).GetImage()
                        .SetSprite("icon_score_box_close", false);
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
                    {
                        List<Vector3> thisBtnReList = taskScore.reward;
                        CreateBoxTip(thisBtnReList, uibox);
                    });
                }
                else if (gameTaskScore.Status == 1)
                {
                    //scoreSum += taskScore.score;
                    uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box).GetImage()
                        .SetSprite("icon_score_box_open", false);
                    //弹tips
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
                    {
                        List<Vector3> thisBtnReList = taskScore.reward;
                        CreateBoxTip(thisBtnReList, uibox);
                    });
                }
                else if (gameTaskScore.Status == 2)
                {
                    KImg_Claimed.SetActive(true);
                    uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box).GetImage()
                        .SetSprite("icon_score_box_close", false);
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
                    {
                        WebMessageHandlerOld.Instance.AddHandler(3, 3, OnCliamScoreBoxResponse);
                        IntValue intValue = new IntValue();
                        intValue.Value = taskScore.id;
                        NetWorkManager.Instance.SendMessage(3, 3, intValue);
                        foreach (var VARIABLE in ResourcesSingletonOld.Instance.dayAndWeekInfo.TaskScoreList)
                        {
                            if (VARIABLE.Id == taskScore.id)
                            {
                                VARIABLE.Status = 1;
                                break;
                            }
                        }
                    });
                }


                // if (taskScore.score > scoreSum)
                // {
                //     //RedPointMgr.instance.SetState(BattleRoot, "score" + tslist[i].id.ToString(), RedPointState.Hide);
                //     //uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed).SetActive(false);
                //     uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box).GetImage()
                //         .SetSprite("icon_score_box_close", false);
                //     //tips
                //     UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
                //     {
                //         List<Vector3> thisBtnReList = taskScore.reward;
                //         CreateBoxPreTip(thisBtnReList, uibox);
                //     });
                // }
                // else
                // {
                //     if (ResourcesSingletonOld.Instance.dayWeekTask.boxes[tslist[i].id].x == 0)
                //     {
                //         //未领取
                //         // RedPointMgr.instance.SetState(BattleRoot, "score" + tslist[i].id.ToString(),
                //         //     RedPointState.Show);
                //         //uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed).SetActive(true);
                //         //TODO Directly claim
                //         uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box).GetImage()
                //             .SetSprite("icon_score_box_close", false);
                //         // UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
                //         // {
                //         //     WebMessageHandlerOld.Instance.AddHandler(3, 3, OnCliamScoreBoxResponse);
                //         //     //clickBoxUI = uibox;
                //         //     clickBoxID = tslist[ihelp].id;
                //         //     IntValue intValue = new IntValue();
                //         //     intValue.Value = tslist[ihelp].id;
                //         //     NetWorkManager.Instance.SendMessage(3, 3, intValue);
                //         // });
                //         //WebMessageHandlerOld.Instance.AddHandler(3, 3, OnCliamScoreBoxResponse);
                //         //clickBoxUI = uibox;
                //         clickBoxID = tslist[ihelp].id;
                //         IntValue intValue = new IntValue();
                //         intValue.Value = tslist[ihelp].id;
                //         //NetWorkManager.Instance.SendMessage(3, 3, intValue);
                //     }
                //     else
                //     {
                //         //已领取
                //         // RedPointMgr.instance.SetState(BattleRoot, "score" + tslist[i].id.ToString(),
                //         //     RedPointState.Hide);
                //         //uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed).SetActive(false);
                //         uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box).GetImage()
                //             .SetSprite("icon_score_box_open", false);
                //         //弹tips
                //         UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
                //         {
                //             List<Vector3> thisBtnReList = tslist[ihelp].reward;
                //             CreateBoxPreTip(thisBtnReList, uibox);
                //         });
                //     }
                //     //uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed).SetActive(true);
                // }
            }

            scoreBoxList.Sort((a, b) =>
            {
                var uia = a as UISubPanel_Task_Score_Box;
                var uib = b as UISubPanel_Task_Score_Box;
                return uia.id.CompareTo(uib.id);
            });
            this.GetFromReference(KText_Score).GetTextMeshPro().SetTMPText(scoreSum.ToString());
            var KProgressBar = this.GetFromReference(UIPanel_Task_DailyAndWeekly.KProgressBar);
            KProgressBar.GetImage().SetFillAmount(scoreSum / scoreMax);


            // for (int i = 0; i < tslist.Count; i++)
            // {
            //     int ihelp = i;
            //     var uibox = await scoreBoxList.CreateWithUITypeAsync(UIType.UISubPanel_Task_Score_Box, false);
            //     // RedPointMgr.instance.Init(BattleRoot, "score" + tslist[i].id.ToString(),
            //     //     (RedPointState state, int data) => { },
            //     //     uibox.GetFromReference(UISubPanel_Task_Score_Box.KBtn_Score_Box).GetXButton());
            //     uibox.GetRectTransform()
            //         .SetAnchoredPositionX(ProgressBarW * (-0.5f + (float)tslist[i].score / scoreMax));
            //     uibox.GetFromReference(UISubPanel_Task_Score_Box.KText_Box_Score).GetTextMeshPro()
            //         .SetTMPText(tslist[i].score.ToString());
            //     //Debug.Log(ResourcesSingletonOld.Instance.dayWeekTask.boxes[tslist[i].id].x.ToString());
            //
            //     var claimBtn = uibox.GetFromReference(UISubPanel_Task_Score_Box.KBtn_Score_Box);
            //
            //     if (tslist[i].score > scoreSum)
            //     {
            //         //RedPointMgr.instance.SetState(BattleRoot, "score" + tslist[i].id.ToString(), RedPointState.Hide);
            //         //uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed).SetActive(false);
            //         uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box).GetImage()
            //             .SetSprite("icon_score_box_close", true);
            //         //tips
            //         UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
            //         {
            //             List<Vector3> thisBtnReList = tslist[ihelp].reward;
            //             CreateBoxPreTip(thisBtnReList, uibox);
            //         });
            //     }
            //     else
            //     {
            //         if (ResourcesSingletonOld.Instance.dayWeekTask.boxes[tslist[i].id].x == 0)
            //         {
            //             //未领取
            //             // RedPointMgr.instance.SetState(BattleRoot, "score" + tslist[i].id.ToString(),
            //             //     RedPointState.Show);
            //             //uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed).SetActive(true);
            //             //TODO Directly claim
            //             uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box).GetImage()
            //                 .SetSprite("icon_score_box_close", true);
            //             // UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
            //             // {
            //             //     WebMessageHandlerOld.Instance.AddHandler(3, 3, OnCliamScoreBoxResponse);
            //             //     //clickBoxUI = uibox;
            //             //     clickBoxID = tslist[ihelp].id;
            //             //     IntValue intValue = new IntValue();
            //             //     intValue.Value = tslist[ihelp].id;
            //             //     NetWorkManager.Instance.SendMessage(3, 3, intValue);
            //             // });
            //             //WebMessageHandlerOld.Instance.AddHandler(3, 3, OnCliamScoreBoxResponse);
            //             //clickBoxUI = uibox;
            //             clickBoxID = tslist[ihelp].id;
            //             IntValue intValue = new IntValue();
            //             intValue.Value = tslist[ihelp].id;
            //             //NetWorkManager.Instance.SendMessage(3, 3, intValue);
            //         }
            //         else
            //         {
            //             //已领取
            //             // RedPointMgr.instance.SetState(BattleRoot, "score" + tslist[i].id.ToString(),
            //             //     RedPointState.Hide);
            //             //uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed).SetActive(false);
            //             uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box).GetImage()
            //                 .SetSprite("icon_score_box_open", true);
            //             //弹tips
            //             UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(claimBtn, () =>
            //             {
            //                 List<Vector3> thisBtnReList = tslist[ihelp].reward;
            //                 CreateBoxPreTip(thisBtnReList, uibox);
            //             });
            //         }
            //         //uibox.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed).SetActive(true);
            //     }
            // }


            //this.GetFromReference(KProgressBar).GetRectTransform().do
        }

        /// <summary>
        /// create box pre tip
        /// </summary>
        /// <param name="rewardList"></param>
        /// <param name="transform">box transform</param>
        //private async void CreateBoxPreTip(List<Vector3> rewardList, UI boxUI)
        //{
        //    var tipUI = await UIHelper.CreateAsync(UIType.UICommon_Reward_Tip, rewardList);
        //    // var tipPosList = this.GetFromReference(KPos_Tip).GetList();
        //    // tipPosList.Clear();
        //    // var tipUI = await tipPosList.CreateWithUITypeAsync<List<Vector3>>(UIType.UICommon_Reward_Tip, rewardList,
        //    //     false);
        //    var KImg_ArrowDownUp = tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownUp);
        //    tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownRight).SetActive(false);
        //    tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownLeft).SetActive(false);
        //    KImg_ArrowDownUp.SetActive(true);
        //    var itemPos = UnicornUIHelper.GetUIPos(boxUI);
        //    //itemPos.y -= 95f;
        //    tipUI.GetRectTransform().SetAnchoredPosition(itemPos);

        //    // var tipY = tipUI.GetRectTransform().AnchoredPosition().y;
        //    // var tipH = tipUI.GetRectTransform().Height();
        //    // tipUI.GetRectTransform().SetAnchoredPositionY(tipY - tipH / 2);
        //    // tipUI.GetRectTransform().SetAnchoredPositionX(boxUI.GetRectTransform().AnchoredPosition().x + 20);
        //    // //screen boundary
        //    // var tipW = tipUI.GetRectTransform().Width();
        //    // if (boxUI.GetRectTransform().AnchoredPosition().x + 20 + tipW / 2 > Screen.width / 2)
        //    // {
        //    //     var tipWHelp = boxUI.GetRectTransform().AnchoredPosition().x + 20 + tipW / 2 - Screen.width / 2;
        //    //     tipUI.GetRectTransform()
        //    //         .SetAnchoredPositionX(boxUI.GetRectTransform().AnchoredPosition().x + 20 - tipWHelp);
        //    //     tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownUp).GetRectTransform()
        //    //         .SetAnchoredPositionX(tipWHelp);
        //    // }

        //    //this.GetFromReference(KBtn_Tip_Close).SetActive(true);
        //}
        private void CreateBoxTip(List<Vector3> rewardList, UI boxUI)
        {
            UnicornUIHelper.DestoryItemTips();
            var tipUI = UIHelper.Create<List<Vector3>>(UIType.UICommon_Reward_Tip, rewardList);
            tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownLeft).SetActive(false);
            tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownRight).SetActive(false);
            tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownUp).SetActive(true);
            var uiPos = UnicornUIHelper.GetUIPos(boxUI);
            float boxH = boxUI.GetRectTransform().Height();
            float tipH = tipUI.GetRectTransform().Height();
            tipUI.GetRectTransform().SetAnchoredPositionY(uiPos.y - tipH / 2 - boxH / 2);
            var screenPosL = -(Screen.width / 2f);
            var screenPosR = Screen.width / 2f;
            float posx = uiPos.x;
            Log.Debug($"width:{tipUI.GetRectTransform().Width()}");
            if ((uiPos.x + tipUI.GetRectTransform().Width() / 2f) > screenPosR)
            {
                posx = screenPosR - (tipUI.GetRectTransform().Width() / 2f) - 20;
            }
            else if ((uiPos.x - tipUI.GetRectTransform().Width() / 2f) < screenPosL)
            {
                posx = screenPosL + (tipUI.GetRectTransform().Width() / 2f) + 20;
            }

            tipUI.GetRectTransform().SetAnchoredPositionX(posx);

            tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownUp).GetRectTransform()
                .SetAnchoredPositionX(uiPos.x - posx);
        }

        private void ClearTip()
        {
            var tipPosList = this.GetFromReference(KPos_Tip).GetList();
            tipPosList.Clear();
        }

        /// <summary>
        /// Claim treasure box response
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCliamScoreBoxResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(3, 3, OnCliamScoreBoxResponse);
            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);
            Debug.Log(stringValueList);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            #region change data

            //ResourcesSingletonOld.Instance.dayWeekTask.boxes[clickBoxID] = new Vector2(1, 0);
            TopScoreBoxSet(achieveGroupId);
            //foreach (var itemstr in stringValueList.Values)
            //{
            //    UnicornUIHelper.AddReward(UnityHelper.StrToVector3(itemstr));
            //}

            List<Vector3> reList = new List<Vector3>();
            foreach (var itemstr in stringValueList.Values)
            {
                reList.Add(UnityHelper.StrToVector3(itemstr));
            }

            UIHelper.Create(UIType.UICommon_Reward, reList);


            // WebMessageHandlerOld.Instance.AddHandler(3, 1, OnRoleTaskInfoResponse);
            // NetWorkManager.Instance.SendMessage(3, 1);
            //ResourcesSingletonOld.Instance.UpdateResourceUI();

            #endregion
        }

        /// <summary>
        /// Delete all task UI
        /// </summary>
        public void DestroyTaskList()
        {
            //UnicornUIHelper.DestroyAllChildren(this.GetFromReference(KScrollView).GetScrollRect().Content);
            var KContent = GetFromReference(KScrollView).GetXScrollRect().Content;
            var list = KContent.GetList();
            list.Clear();
        }

        private void SwapUI(Transform transform1, Transform transform2)
        {
            int index1 = transform1.GetSiblingIndex();
            int index2 = transform2.GetSiblingIndex();

            transform1.SetSiblingIndex(index2);
            transform2.SetSiblingIndex(index1);
        }

        private void ReSortTaskUI(int taskID)
        {
        }

        private void TaskSortList(int groupID)
        {
            //0 id 1 是否领取 2 参数
            List<Vector2> vector2s = new List<Vector2>();

            //可领取但是未领取的部分
            foreach (var t in ResourcesSingletonOld.Instance.dayWeekTask.tasks)
            {
                if (tbtask[t.Key].group == groupID)
                {
                    if (t.Value.x == 0)
                    {
                        if (t.Value.y >= tbtask[t.Key].para[0])
                        {
                            //Debug.Log(t.Key);
                            vector2s.Add(new Vector2(t.Key, 0));
                        }
                    }
                }
            }

            vector2s.Sort(delegate(Vector2 v21, Vector2 v22)
            {
                return tbtask[(int)v21.x].sort.CompareTo(tbtask[(int)v21.x].sort);
            });
            //Debug.Log("vector2s.Count :" + vector2s.Count.ToString());

            //不可领取的部分
            List<Vector2> vector2s1 = new List<Vector2>();
            foreach (var t in ResourcesSingletonOld.Instance.dayWeekTask.tasks)
            {
                if (tbtask[t.Key].group == groupID)
                {
                    if (t.Value.x == 0)
                    {
                        if (t.Value.y < tbtask[t.Key].para[0])
                        {
                            //Debug.Log(t.Key);
                            vector2s1.Add(new Vector2(t.Key, 0));
                        }
                    }
                }
            }

            vector2s1.Sort(delegate(Vector2 v21, Vector2 v22)
            {
                return tbtask[(int)v21.x].sort.CompareTo(tbtask[(int)v21.x].sort);
            });
            foreach (var v in vector2s1)
            {
                vector2s.Add(v);
            }
            //Debug.Log("vector2s.Count :" + vector2s.Count.ToString());

            //领取完的部分
            List<Vector2> vector2s2 = new List<Vector2>();
            foreach (var t in ResourcesSingletonOld.Instance.dayWeekTask.tasks)
            {
                if (tbtask[t.Key].group == groupID)
                {
                    if (t.Value.x != 0)
                    {
                        //Debug.Log(t.Key);
                        vector2s2.Add(new Vector2(t.Key, 1));
                    }
                }
            }

            vector2s2.Sort(delegate(Vector2 v21, Vector2 v22)
            {
                return tbtask[(int)v21.x].sort.CompareTo(tbtask[(int)v21.x].sort);
            });
            foreach (var v in vector2s2)
            {
                vector2s.Add(v);
            }

            //Debug.Log("vector2s.Count :" + vector2s.Count.ToString());

            //tasks = vector2s;
            //foreach (var t in tasks)
            //{
            //    Debug.Log("task id :" + t.x + "; task type :" + tbtask.Get((int)t.x).type + "; " + "task state :" + t.y);
            //}
        }

        /// <summary>
        /// Delete bottom UI
        /// </summary>
        private void DestroyBottomUI()
        {
            //int BottomListCount = BottomList.Count;
            //for (int i = 0; i < BottomListCount; i++)
            //{
            //    UnityEngine.GameObject.Destroy(BottomList[i].GameObject);
            //}
            //BottomList.Clear();
            //UnityEngine.GameObject.Destroy(BottomUI.GameObject);
            //BottomUI.Dispose();
            //BottomUI.Dispose();
        }

        public void RemoveTimer()
        {
            if (this.timerId != 0)
            {
                //Debug.Log("RemoveTimer");
                var timerMgr = TimerManager.Instance;
                timerMgr?.RemoveTimerId(ref this.timerId);
                this.timerId = 0;
            }
        }

        protected override void OnClose()
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.GETALLDAILY, OnGetAllDailyResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.QUERYDAYANDWEEKTASK, OnDayAndWeekResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.GETTASKSCORE, OnGetTaskResponse);
            //DestroyTaskList();
            RemoveTimer();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
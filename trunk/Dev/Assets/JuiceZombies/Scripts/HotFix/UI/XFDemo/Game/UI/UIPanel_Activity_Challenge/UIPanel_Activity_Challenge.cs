//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using Common;
using Google.Protobuf;
using Main;
using Unity.Entities.UniversalDelegates;
using DG.Tweening;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Activity_Challenge)]
    internal sealed class UIPanel_Activity_ChallengeEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Activity_Challenge;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Activity_Challenge>();
        }
    }

    //活跃度挑战
    public partial class UIPanel_Activity_Challenge : UI, IAwake<int>
    {
        #region property

        private Tblanguage tblanguage;
        private Tbtask_score tbtask_Score;
        private Tbactivity tbactivity;
        private Tbdays_challenge tbdays_Challenge;
        private Tbtask_group tbtask_Group;
        private Tbtask_type tbtask_type;
        private Tbtask tbtask;
        private Tbtag_func tbtag_Func;
        private Tbchapter tbchapter;
        private Tbquality tbquality;
        private Tbequip_quality tbequip_Quality;
        private Tbdays_challenge tbdays_challenge;
        private Tbtask_group tbtask_group;

        private long timerId = 0;


        //private UI LastBottomBtn;
        private int tagFunc;
        private float TaskH = 168;

        private float DeltaH = 20;

        //private string picClaim = "pic_daily_task_claim";
        private string picConnotClaim = "pic_daily_task_cannot_claim";
        private string picClaimed = "pic_daily_task_claimed";

        private string thisRedRoot = "module3301RedRoot";

        //private float LastScrollValue = 1;
        private int lastDayId;

        #endregion

        #region Test

        // private int days = 7;
        private int lockDay;

        private int scores = 0;
        private long endTime;
        //  = 0;

        #endregion

        private int activityId;
        private string m_RedDotName;
        private bool isInit;

        private CancellationTokenSource cts = new CancellationTokenSource();

        public async void Initialize(int acID)
        {
            await JiYuUIHelper.InitBlur(this);

            GetFromReference(KBg_ScoreBox).GetComponent<CanvasGroup>().alpha = 0;

            activityId = acID;
            InitJson();

            WebMessageHandler.Instance.AddHandler(CMD.QUERTSINGLEACTIVITY, OnActivityChallengeResponse);
            WebMessageHandler.Instance.AddHandler(CMD.GETTASKSCORE, OnGetTaskResponse);

            var activity = tbactivity.Get(acID);
            var daysChallenge = tbdays_challenge.Get(activity.link);
            tagFunc = daysChallenge.tagFunc;
            var KImg_Pic = GetFromReference(UIPanel_Activity_Challenge.KImg_Pic);
            KImg_Pic.GetImage().SetSpriteAsync(daysChallenge.pic, false).Forget();
            //m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);

            NetWorkManager.Instance.SendMessage(CMD.QUERTSINGLEACTIVITY, new IntValue()
            {
                Value = activityId
            });
        }

        private void InitEffect()
        {
            JiYuTweenHelper.SetEaseAlphaAndPosUtoB(GetFromReference(KBg_ScoreBox), 0, cancellationToken: cts.Token);
            var posY = GetFromReference(KPos_Select).GetRectTransform().AnchoredPosition().y;
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(GetFromReference(KPos_Select), posY, 100,
                cancellationToken: cts.Token);
            GetFromReference(KPos_Select).GetComponent<CanvasGroup>().alpha = 0f;
            GetFromReference(KPos_Select).GetComponent<CanvasGroup>().DOFade(1, 0.4f).SetEase(Ease.InQuad);
        }

        async UniTaskVoid InitNode()
        {
            DataInit();
            InitRedDot();
            UpSet();
            DownSelectSet().Forget();
            SetCloseTip();
            GetFromReference(KBg).GetButton().OnClick?.Add(() => { Close(); });
            StartTimer();
            InitEffect();
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtask_Score = ConfigManager.Instance.Tables.Tbtask_score;
            tbactivity = ConfigManager.Instance.Tables.Tbactivity;
            tbdays_Challenge = ConfigManager.Instance.Tables.Tbdays_challenge;
            tbtask_Group = ConfigManager.Instance.Tables.Tbtask_group;
            tbtask = ConfigManager.Instance.Tables.Tbtask;
            tbtask_type = ConfigManager.Instance.Tables.Tbtask_type;
            tbtag_Func = ConfigManager.Instance.Tables.Tbtag_func;
            tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            tbquality = ConfigManager.Instance.Tables.Tbquality;
            tbequip_Quality = ConfigManager.Instance.Tables.Tbequip_quality;
            tbdays_challenge = ConfigManager.Instance.Tables.Tbdays_challenge;
            tbtask_group = ConfigManager.Instance.Tables.Tbtask_group;
        }

        private void SetCloseTip()
        {
            //GetFromReference(KBg_ScoreBox).GetXButton()?.OnClick?.Add(CloseAllTip);
            GetFromReference(KScrollView).GetXScrollRect().OnDrag.Add((f) => { JiYuUIHelper.DestoryAllTips(); });
            var KBtn_Tip_Close = GetFromReference(UIPanel_Activity_Challenge.KBtn_Tip_Close);
            KBtn_Tip_Close.GetButton().OnClick.Add(JiYuUIHelper.DestoryAllTips);
        }


        // private void CloseAllTip()
        // {
        //     UIHelper.Remove(UIType.UICommon_Reward_Tip);
        //     UIHelper.Remove(UIType.UICommon_EquipTips);
        //     UIHelper.Remove(UIType.UICommon_ItemTips);
        // }

        private void AllPanelUpdate()
        {
            DataInit();
            UpSet(true);
            CreateTasks(lastDayId, true).Forget();
            //RedPointUpdate();
        }


        // private void InitRedPoint()
        // {
        //     RedPointMgr.instance.Remove(thisRedRoot, thisRedRoot);
        //     RedPointMgr.instance.Add(thisRedRoot, null, null, RedPointType.Enternal);
        //     List<int> taskGroupIDList = new List<int>();
        //     foreach (var tbtg in tbtask_Group.DataList)
        //     {
        //         if (tbtg.tagFunc == tagFunc)
        //         {
        //             taskGroupIDList.Add(tbtg.id);
        //         }
        //     }
        //
        //     List<task> taskList = new List<task>();
        //     foreach (var taskGroupID in taskGroupIDList)
        //     {
        //         foreach (var tbtk in tbtask.DataList)
        //         {
        //             if (tbtk.group == taskGroupID)
        //             {
        //                 taskList.Add(tbtk);
        //             }
        //         }
        //     }
        //
        //     for (int i = 1; i <= days; i++)
        //     {
        //         RedPointMgr.instance.Add(thisRedRoot, "day" + i.ToString(), thisRedRoot, RedPointType.Enternal);
        //     }
        //
        //     foreach (var t in taskList)
        //     {
        //         RedPointMgr.instance.Add(thisRedRoot, "task" + t.id.ToString(),
        //             "day" + tbtask_Group.Get(t.group).day.ToString(), RedPointType.Once);
        //     }
        //
        //     for (int i = 0; i < 8; i++)
        //     {
        //         RedPointMgr.instance.Add(thisRedRoot, "box" + i.ToString(), thisRedRoot, RedPointType.Enternal);
        //     }
        //
        //     RedPointUpdate();
        // }

        // private void RedPointUpdate()
        // {
        //     List<int> taskGroupIDList = new List<int>();
        //     foreach (var tbtg in tbtask_Group.DataList)
        //     {
        //         if (tbtg.tagFunc == tagFunc)
        //         {
        //             taskGroupIDList.Add(tbtg.id);
        //         }
        //     }
        //
        //     List<task> taskList = new List<task>();
        //     Dictionary<int, int> IDandStateDic = new Dictionary<int, int>();
        //     foreach (var taskGroupID in taskGroupIDList)
        //     {
        //         foreach (var tbtk in tbtask.DataList)
        //         {
        //             if (tbtk.group == taskGroupID)
        //             {
        //                 taskList.Add(tbtk);
        //                 var acTask = GetTaskByID(tbtk.id);
        //                 int a = 0;
        //                 if (acTask.Status == 1)
        //                 {
        //                     a = 2;
        //                 }
        //                 else
        //                 {
        //                     if (acTask.Para >= tbtk.para[0])
        //                     {
        //                         a = 0;
        //                     }
        //                     else
        //                     {
        //                         a = 1;
        //                     }
        //                 }
        //
        //                 IDandStateDic.Add((int)tbtk.id, a);
        //             }
        //         }
        //     }
        //
        //     foreach (var t in taskList)
        //     {
        //         if (IDandStateDic[t.id] == 0 && tbtask_Group.Get(t.group).day <= lockDay)
        //         {
        //             RedPointMgr.instance.SetState(thisRedRoot, "task" + t.id.ToString(), RedPointState.Show);
        //         }
        //         else
        //         {
        //             RedPointMgr.instance.SetState(thisRedRoot, "task" + t.id.ToString(), RedPointState.Hide);
        //         }
        //     }
        // }

        private void DataInit()
        {
            //days = maxDay;

            //lockDay = 0;
            //test_Scores = 0;
            lockDay = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId]
                .OpenServerActivity.TaskList.Max(p => p.Day);
            // foreach (var VARIABLE in ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityID]
            //              .OpenServerActivity.TaskList)
            // {
            //     Log.Debug($"TaskList:{VARIABLE.ToString()}", Color.green);
            // }
            scores = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId]
                .OpenServerActivity.TaskList.Where(a => a.Status == 1).Sum(p => tbtask.Get(p.TaskId).score);
            var scorList = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId]
                .OpenServerActivity.TaskScoreList;
            Log.Debug($"scorList :{scorList.ToString()}");

            // scores = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityID]
            //     .OpenServerActivity.TaskScoreList.Where(a => a.TagFunc == tagFunc && a.Valid == 1)
            //     .Sum(p => tbtask_Score.Get(p.Id).score);


            // foreach (var VARIABLE in ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityID]
            //              .OpenServerActivity.TaskScoreList)
            // {
            //     Log.Debug($"TaskScoreList:{VARIABLE.ToString()}", Color.green);
            //     scores += tbtask_Score.Get(VARIABLE.Id).score;
            // }

            // 找到得分最高的玩家
            // var activityTask = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityID]
            //     .OpenServerActivity.TaskList.FirstOrDefault(p => p.Day == maxDay);

            // foreach (var severTask in ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityID]
            //              .OpenServerActivity.TaskList)
            // {
            //     if (lockDay < severTask.Day)
            //     {
            //         lockDay = severTask.Day;
            //     }
            //
            //     // if (severTask.Status == 1)
            //     // {
            //     //     test_Scores += tbtask.Get(severTask.TaskId).score;
            //     // }
            // }
            endTime = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId].OpenServerActivity
                .EndTime;
            // endTime = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityID].OpenServerActivity
            //               .EndTime -
            //           (ResourcesSingleton.Instance.ServerDeltaTime / 1000);
        }

        private void Update()
        {
            var KText_Time = GetFromReference(UIPanel_Activity_Challenge.KText_Time);
            string cdStr = tblanguage.Get("active_countdown_text").current;
            long clientT = JiYuUIHelper.GetServerTimeStamp(true);
            if (!JiYuUIHelper.TryGetRemainingTime(clientT, endTime, out var timeStr))
            {
                Close();
            }

            cdStr += timeStr;

            KText_Time.GetTextMeshPro().SetTMPText(cdStr);
        }

        private void UpSet(bool isRefresh = false)
        {
            UpTextSet();
            if (isRefresh)
            {
                UpOtherSetRefresh();
            }
            else
            {
                UpOtherSet();
            }
        }

        private void UpTextSet()
        {
            //SetCountDownText();
            string descStr = tblanguage.Get(tbdays_Challenge.Get(tbactivity.Get(activityId).link).desc).current;
            GetFromReference(KText_Desc).GetTextMeshPro().SetTMPText(descStr);
            GetFromReference(KText_Score).GetTextMeshPro().SetTMPText(scores.ToString());
        }

        private void UpOtherSet()
        {
            var tbtsList = tbtask_Score.DataList.Where(a => a.tagFunc == tagFunc).ToList();
            tbtsList.Sort(delegate(task_score t1, task_score t2) { return t1.score.CompareTo(t2.score); });
            int num = 0;
            foreach (var tp in tbtsList)
            {
                if (tp.score <= scores)
                {
                    num++;
                }
            }

            float fillHelp = 0;
            if (num >= 8)
            {
                fillHelp = 1;
            }
            else if (num == 0)
            {
                fillHelp = 0.125f * scores / tbtsList[0].score;
            }
            else
            {
                fillHelp = 0.125f * num + 0.125f * (scores - tbtsList[num - 1].score) /
                    (tbtsList[num].score - tbtsList[num - 1].score);
            }

            // float pWidth = GetFromReference(KProgressBar_Bg).GetRectTransform().Width();
            // GetFromReference(KProgressBar).GetRectTransform().SetOffsetWithRight((1 - fillHelp) * pWidth);
            GetFromReference(KProgressBar).GetImage().SetFillAmount(fillHelp);

            if (tbtsList.Count != 8)
            {
                Debug.Log("tbtsList.Count != 8");
                Debug.Log(tbtsList.Count);
                Close();
            }

            for (int i = 0; i < 8; i++)
            {
                int ihelp = i;

                var ui = GetFromReference($"SubPanel_BoxRe{ihelp}");

                // RedPointMgr.instance.Init(thisRedRoot, "box" + i.ToString(),
                //     (RedPointState state, int data) =>
                //     {
                //         var imgRed1 = boxreUI.GetFromReference(UISubPanel_Activity_BoxRe.KSubPanel_Activity_Equip_Btn)
                //             .GetFromReference(UISubPanel_Activity_Equip_Btn.KImg_RedPoint);
                //         var imgRed2 = boxreUI.GetFromReference(UISubPanel_Activity_BoxRe.KSubPanel_Task_Score_Box)
                //             .GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed);
                //         if (state == RedPointState.Show)
                //         {
                //             imgRed1.SetActive(true);
                //             imgRed2.SetActive(true);
                //         }
                //         else
                //         {
                //             imgRed1.SetActive(false);
                //             imgRed2.SetActive(false);
                //         }
                //     });

                var taskScoreData = SelectBoxServerData(tbtsList[ihelp].id);
                int state = taskScoreData.Status;
                var KPos_Box = ui.GetFromReference($"Pos_Box");
                var KPos_Re = ui.GetFromReference($"Pos_Re");
                var KSubPanel_Task_Score_Box = ui.GetFromReference($"SubPanel_Task_Score_Box");
                var KSubPanel_Activity_Equip_Btn =
                    ui.GetFromReference($"SubPanel_Activity_Equip_Btn");


                var itemStr = $"{m_RedDotName}|Up{i}";
                //RedDotManager.Instance.InsterNode(itemStr);
                RedDotManager.Instance.SetRedPointCnt(itemStr,
                    state == 2 ? 1 : 0);
                // if (tbtsList[ihelp].score > scores)
                // {
                //     //未完成
                // }
                // else
                // {
                //     if (taskScoreData.Status == 1)
                //     {
                //         //已领取
                //         state = 1;
                //     }
                //     else
                //     {
                //         //可领取
                //         state = 2;
                //     }
                // }

                if (tbtsList[ihelp].reward[0][0] == 11)
                {
                    KPos_Re.SetActive(true);
                    KPos_Box.SetActive(false);
                    var scoreBox = KSubPanel_Activity_Equip_Btn;
                    var reUI = scoreBox.GetFromReference(UISubPanel_Activity_Equip_Btn.KCommon_RewardItem);
                    var KBtn_Item = reUI.GetFromReference(UICommon_RewardItem.KBtn_Item);
                    scoreBox.GetFromReference(UISubPanel_Activity_Equip_Btn.KText_Reward_Score).GetTextMeshPro()
                        .SetTMPText(tbtsList[ihelp].score.ToString());
                    var KImg_Claimed =
                        KSubPanel_Activity_Equip_Btn.GetFromReference(UISubPanel_Activity_Equip_Btn.KImg_Claimed);
                    KImg_Claimed.SetActive(false);

                    JiYuUIHelper.SetRewardIconAndCountText(tbtsList[ihelp].reward[0], reUI);

                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Item, () =>
                    {
                        var taskScoreData = SelectBoxServerData(tbtsList[ihelp].id);
                        int state = taskScoreData.Status;
                        switch (state)
                        {
                            case 0:
                            case 1:
                                JiYuUIHelper.SetRewardOnClickWithNoBtn(tbtsList[ihelp].reward[0], reUI);
                                break;
                            case 2:
                                KImg_Claimed.SetActive(true);
                                WebMessageHandler.Instance.AddHandler(CMD.GETTASKBOX, OnGetBoxResponse);
                                IntValue intValue = new IntValue();
                                intValue.Value = tbtsList[ihelp].id;
                                NetWorkManager.Instance.SendMessage(CMD.GETTASKBOX, intValue);

                                break;
                        }
                    });
                }
                else
                {
                    KPos_Re.SetActive(false);
                    KPos_Box.SetActive(true);
                    var KImg_Score_Box =
                        KSubPanel_Task_Score_Box.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box);
                    var KImg_Claimed =
                        KSubPanel_Task_Score_Box.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed);
                    KImg_Claimed.SetActive(state == 2);
                    var imgStr = state == 1 ? $"icon_score_box_open" : "icon_score_box_close";

                    KImg_Score_Box.GetImage().SetSpriteAsync(imgStr, false).Forget();
                    //icon_score_box_close
                    var boxBtn = KSubPanel_Task_Score_Box.GetFromReference(UISubPanel_Task_Score_Box.KBtn_Score_Box);
                    KSubPanel_Task_Score_Box.GetFromReference(UISubPanel_Task_Score_Box.KText_Box_Score)
                        .GetTextMeshPro()
                        .SetTMPText(tbtsList[ihelp].score.ToString());

                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(boxBtn, () =>
                    {
                        var taskScoreData = SelectBoxServerData(tbtsList[ihelp].id);
                        int state = taskScoreData.Status;
                        //Log.Error($"boxBtn ");
                        JiYuUIHelper.DestoryAllTips();
                        switch (state)
                        {
                            case 0:
                            case 1:
                                List<Vector3> a1 = new List<Vector3>();
                                a1.Add(tbtsList[ihelp].reward[0]);
                                CreateBoxTip(a1, ui);
                                break;
                            case 2:
                                WebMessageHandler.Instance.AddHandler(CMD.GETTASKBOX, OnGetBoxResponse);
                                IntValue intValue = new IntValue();
                                intValue.Value = tbtsList[ihelp].id;
                                NetWorkManager.Instance.SendMessage(CMD.GETTASKBOX, intValue);
                                //Log.Error($"send mess boxBtn ");
                                break;
                        }
                    });
                }
            }
        }

        private void UpOtherSetRefresh()
        {
            var tbtsList = tbtask_Score.DataList.Where(a => a.tagFunc == tagFunc).ToList();
            tbtsList.Sort(delegate(task_score t1, task_score t2) { return t1.score.CompareTo(t2.score); });
            int num = 0;
            foreach (var tp in tbtsList)
            {
                if (tp.score <= scores)
                {
                    num++;
                }
            }

            float fillHelp = 0;
            if (num >= 8)
            {
                fillHelp = 1;
            }
            else if (num == 0)
            {
                fillHelp = 0.125f * scores / tbtsList[0].score;
            }
            else
            {
                fillHelp = 0.125f * num + 0.125f * (scores - tbtsList[num - 1].score) /
                    (tbtsList[num].score - tbtsList[num - 1].score);
            }

            // float pWidth = GetFromReference(KProgressBar_Bg).GetRectTransform().Width();
            // GetFromReference(KProgressBar).GetRectTransform().SetOffsetWithRight((1 - fillHelp) * pWidth);
            GetFromReference(KProgressBar).GetImage().SetFillAmount(fillHelp);

            if (tbtsList.Count != 8)
            {
                Debug.Log("tbtsList.Count != 8");
                Debug.Log(tbtsList.Count);
                Close();
            }

            for (int i = 0; i < 8; i++)
            {
                int ihelp = i;

                var ui = GetFromReference($"SubPanel_BoxRe{ihelp}");

                // RedPointMgr.instance.Init(thisRedRoot, "box" + i.ToString(),
                //     (RedPointState state, int data) =>
                //     {
                //         var imgRed1 = boxreUI.GetFromReference(UISubPanel_Activity_BoxRe.KSubPanel_Activity_Equip_Btn)
                //             .GetFromReference(UISubPanel_Activity_Equip_Btn.KImg_RedPoint);
                //         var imgRed2 = boxreUI.GetFromReference(UISubPanel_Activity_BoxRe.KSubPanel_Task_Score_Box)
                //             .GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed);
                //         if (state == RedPointState.Show)
                //         {
                //             imgRed1.SetActive(true);
                //             imgRed2.SetActive(true);
                //         }
                //         else
                //         {
                //             imgRed1.SetActive(false);
                //             imgRed2.SetActive(false);
                //         }
                //     });

                var taskScoreData = SelectBoxServerData(tbtsList[ihelp].id);

                var KPos_Box = ui.GetFromReference($"Pos_Box");
                var KPos_Re = ui.GetFromReference($"Pos_Re");
                var KSubPanel_Task_Score_Box = ui.GetFromReference($"SubPanel_Task_Score_Box");
                var KSubPanel_Activity_Equip_Btn =
                    ui.GetFromReference($"SubPanel_Activity_Equip_Btn");

                int state = taskScoreData.Status;
                var itemStr = $"{m_RedDotName}|Up{i}";
                //RedDotManager.Instance.InsterNode(itemStr);
                RedDotManager.Instance.SetRedPointCnt(itemStr,
                    state == 2 ? 1 : 0);
                // if (tbtsList[ihelp].score > scores)
                // {
                //     //未完成
                // }
                // else
                // {
                //     if (taskScoreData.Status == 1)
                //     {
                //         //已领取
                //         state = 1;
                //     }
                //     else
                //     {
                //         //可领取
                //         state = 2;
                //     }
                // }

                if (tbtsList[ihelp].reward[0][0] == 11)
                {
                    KPos_Re.SetActive(true);
                    KPos_Box.SetActive(false);
                    var scoreBox = KSubPanel_Activity_Equip_Btn;
                    var reUI = scoreBox.GetFromReference(UISubPanel_Activity_Equip_Btn.KCommon_RewardItem);
                    scoreBox.GetFromReference(UISubPanel_Activity_Equip_Btn.KText_Reward_Score).GetTextMeshPro()
                        .SetTMPText(tbtsList[ihelp].score.ToString());
                    var KImg_Claimed =
                        KSubPanel_Activity_Equip_Btn.GetFromReference(UISubPanel_Activity_Equip_Btn.KImg_Claimed);
                    KImg_Claimed.SetActive(false);

                    JiYuUIHelper.SetRewardIconAndCountText(tbtsList[ihelp].reward[0], reUI);
                    switch (state)
                    {
                        case 0:
                            //JiYuUIHelper.SetRewardOnClick(tbtsList[ihelp].reward[0], reUI);
                            break;
                        case 1:

                            //JiYuUIHelper.SetRewardOnClick(tbtsList[ihelp].reward[0], reUI);
                            break;
                        case 2:
                            KImg_Claimed.SetActive(true);

                            // JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(
                            //     reUI.GetFromReference(UICommon_RewardItem.KBtn_Item), () =>
                            //     {
                            //         WebMessageHandler.Instance.AddHandler(CMD.GETTASKBOX, OnGetBoxResponse);
                            //         IntValue intValue = new IntValue();
                            //         intValue.Value = tbtsList[ihelp].id;
                            //         NetWorkManager.Instance.SendMessage(CMD.GETTASKBOX, intValue);
                            //     });
                            break;
                    }
                }
                else
                {
                    KPos_Re.SetActive(false);
                    KPos_Box.SetActive(true);
                    var KImg_Score_Box =
                        KSubPanel_Task_Score_Box.GetFromReference(UISubPanel_Task_Score_Box.KImg_Score_Box);
                    var KImg_Claimed =
                        KSubPanel_Task_Score_Box.GetFromReference(UISubPanel_Task_Score_Box.KImg_Claimed);
                    KImg_Claimed.SetActive(state == 2);
                    var imgStr = state == 1 ? $"icon_score_box_open" : "icon_score_box_close";

                    KImg_Score_Box.GetImage().SetSpriteAsync(imgStr, false).Forget();
                    //icon_score_box_close
                    var boxBtn = KSubPanel_Task_Score_Box.GetFromReference(UISubPanel_Task_Score_Box.KBtn_Score_Box);
                    KSubPanel_Task_Score_Box.GetFromReference(UISubPanel_Task_Score_Box.KText_Box_Score)
                        .GetTextMeshPro()
                        .SetTMPText(tbtsList[ihelp].score.ToString());

                    // JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(boxBtn, () =>
                    // {
                    //     Log.Error($"boxBtn ");
                    //     JiYuUIHelper.DestoryAllTips();
                    //     switch (state)
                    //     {
                    //         case 0:
                    //             List<Vector3> a = new List<Vector3>();
                    //             a.Add(tbtsList[ihelp].reward[0]);
                    //             CreateBoxTip(a, ui);
                    //             break;
                    //         case 1:
                    //             List<Vector3> a1 = new List<Vector3>();
                    //             a1.Add(tbtsList[ihelp].reward[0]);
                    //             CreateBoxTip(a1, ui);
                    //             break;
                    //         case 2:
                    //             WebMessageHandler.Instance.AddHandler(CMD.GETTASKBOX, OnGetBoxResponse);
                    //             IntValue intValue = new IntValue();
                    //             intValue.Value = tbtsList[ihelp].id;
                    //             NetWorkManager.Instance.SendMessage(CMD.GETTASKBOX, intValue);
                    //             Log.Error($"send mess boxBtn ");
                    //             break;
                    //     }
                    // });
                }
            }
        }

        private GameTaskScore SelectBoxServerData(int input)
        {
            return ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId].OpenServerActivity
                .TaskScoreList.Where(a => a.Id == input).FirstOrDefault();
        }

        private void CreateBoxTip(List<Vector3> rewardList, UI boxUI)
        {
            JiYuUIHelper.DestoryAllTips();
            var tipUI = UIHelper.Create<List<Vector3>>(UIType.UICommon_Reward_Tip, rewardList);
            tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownLeft).SetActive(false);
            tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownRight).SetActive(false);
            tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownUp).SetActive(true);
            var uiPos = JiYuUIHelper.GetUIPos(boxUI);
            tipUI.GetRectTransform().SetAnchoredPositionX(uiPos.x);
            float boxH = boxUI.GetFromReference($"Pos_Box").GetRectTransform().Height();
            float tipH = tipUI.GetRectTransform().Height();
            tipUI.GetRectTransform().SetAnchoredPositionY(uiPos.y - tipH- boxH / 2);
        }

        private async UniTaskVoid DownSelectSet()
        {
            var select = GetFromReference(KPos_Select);
            var downList = select.GetList();
            downList.Clear();
            //Dictionary<UI, int> uiSortDic = new Dictionary<UI, int>();
            var taskGroupsList = tbtask_Group.DataList.Where((a) => a.tagFunc == tagFunc).ToList();

            for (int i = 0; i < taskGroupsList.Count; i++)
            {
                int index = i;
                var btnUI = await downList.CreateWithUITypeAsync(UIType.UISubPanel_Select_Tab_Btn2, index + 1, false,
                    cts.Token);
                // RedPointMgr.instance.Init(thisRedRoot, "day" + (ihelp + 1).ToString(),
                //     (RedPointState state, int data) =>
                //     {
                //         var red = btnUI.GetFromReference(UISubPanel_Select_Tab_Btn2.KImg_RedPoint);
                //         if (state == RedPointState.Show)
                //         {
                //             red.SetActive(true);
                //         }
                //         else
                //         {
                //             red.SetActive(false);
                //         }
                //     });
                var KBtn = btnUI.GetFromReference(UISubPanel_Select_Tab_Btn2.KBtn);
                var KText = btnUI.GetFromReference(UISubPanel_Select_Tab_Btn2.KText);
                var KImg = btnUI.GetFromReference(UISubPanel_Select_Tab_Btn2.KImg);
                //btnUI.GetFromReference(UISubPanel_Select_Tab_Btn2.KImg_RedPoint).SetActive(false);
                KText.GetTextMeshPro()
                    .SetTMPText((index + 1).ToString());
                KImg.SetActive(false);
                await KBtn.GetImage().SetSpriteAsync("activity_days_challenge_tip_1", true);
                KBtn.GetRectTransform().SetAnchoredPositionY(-7);
                btnUI.GetRectTransform().SetWidth(KBtn.GetRectTransform().Width());


                if (index == 0)
                {
                    lastDayId = index + 1;
                    //
                    //btnUI.GetFromReference(UISubPanel_Select_Tab_Btn2.KLine).SetActive(true);
                    KText.GetTextMeshPro()
                        .SetTMPText((index + 1).ToString());
                    await KBtn.GetImage().SetSpriteAsync("activity_days_challenge_tip_2", true);
                    KBtn.GetRectTransform().SetAnchoredPositionY(0);
                    btnUI.GetRectTransform().SetWidth(KBtn.GetRectTransform().Width());
                    CreateTasks(index + 1).Forget();
                }

                if (index + 1 <= lockDay)
                {
                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn, () =>
                    {
                        //var LastBottomBtnUI = LastBottomBtn as UISubPanel_Select_Tab_Btn2;
                        if (lastDayId == index + 1)
                            return;

                        JiYuUIHelper.DestoryAllTips();

                        foreach (var child in downList.Children)
                        {
                            var childUI = child as UISubPanel_Select_Tab_Btn2;
                            var KText = childUI.GetFromReference(UISubPanel_Select_Tab_Btn2.KText);
                            var KBtn = childUI.GetFromReference(UISubPanel_Select_Tab_Btn2.KBtn);
                            KBtn.GetImage().SetSpriteAsync("activity_days_challenge_tip_1", true);
                            KBtn.GetRectTransform().SetAnchoredPositionY(-7f);
                            childUI.GetRectTransform().SetWidth(KBtn.GetRectTransform().Width());
                            //childUI.GetRectTransform().SetScale2(1);
                            //childUI.GetFromReference(UISubPanel_Select_Tab_Btn2.KLine).SetActive(false);
                            KText.GetTextMeshPro()
                                .SetTMPText((childUI.id).ToString());
                        }

                        KBtn.GetImage().SetSpriteAsync("activity_days_challenge_tip_2", true);
                        btnUI.GetRectTransform().SetWidth(KBtn.GetRectTransform().Width());
                        KText.GetTextMeshPro()
                            .SetTMPText((index + 1).ToString());
                        lastDayId = index + 1;
                        KBtn.GetRectTransform().SetAnchoredPositionY(0);
                        JiYuUIHelper.ForceRefreshLayout(select);
                        CreateTasks(index + 1).Forget();
                    }, 1101);
                }
                else
                {
                    KImg.SetActive(true);
                }
            }

            downList.Sort((a, b) =>
            {
                var uia = a as UISubPanel_Select_Tab_Btn2;
                var uib = b as UISubPanel_Select_Tab_Btn2;
                return uia.id.CompareTo(uib.id);
            });
        }

        private async UniTaskVoid CreateTasks(int dayId, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                GetFromReference(KContent).GetRectTransform().SetAnchoredPositionY(0);
            }

            Log.Debug($"CreateTasks", Color.green);
            if (!ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.TryGetValue(activityId,
                    out var activityMap))
            {
                Log.Error($"拿不到活动数据activityID:{activityId}");
            }

            var taskList = activityMap.OpenServerActivity.TaskList.Where((a) => a.Day == dayId).ToList();
            var KScrollView = GetFromReference(UIPanel_Activity_Challenge.KScrollView);

            var scrollRect = KScrollView.GetXScrollRect();

            //taskList[0].
            //List<task> taskList = new List<task>();
            // foreach (var tbtk in tbtask.DataList)
            // {
            //     if (tbtk.group == taskGroupID)
            //     {
            //         taskList.Add(tbtk);
            //     }
            // }

            //0, 1, 2
            //Dictionary<int, int> IDandStateDic = new Dictionary<int, int>();
            //Dictionary<task, ActivityTask> TaskandACTaskDic = new Dictionary<task, ActivityTask>();

            // #region test
            //
            // foreach (var ttt in taskList)
            // {
            //     //int a = ttt.id % 3;
            //     //IDandStateDic.Add((int)ttt.id, a);
            //     var acTask = GetTaskByID(ttt.id);
            //     TaskandACTaskDic.Add(ttt, acTask);
            //     int a = 0;
            //     if (acTask.Status == 1)
            //     {
            //         a = 2;
            //     }
            //     else
            //     {
            //         if (acTask.Para >= ttt.para[0])
            //         {
            //             a = 0;
            //         }
            //         else
            //         {
            //             a = 1;
            //         }
            //     }
            //
            //     IDandStateDic.Add((int)ttt.id, a);
            // }
            //
            // #endregion

            // taskList.Sort(delegate(task t1, task t2)
            // {
            //     if (IDandStateDic[t1.id] < IDandStateDic[t2.id])
            //     {
            //         return -1;
            //     }
            //     else if (IDandStateDic[t1.id] > IDandStateDic[t2.id])
            //     {
            //         return 1;
            //     }
            //     else
            //     {
            //         return t1.sort.CompareTo(t2.sort);
            //     }
            // });

            Log.Debug($"{activityMap.OpenServerActivity.TaskList.ToString()}");
            float contentW = GetFromReference(KContent).GetRectTransform().Width();

            //Dictionary<UI, task> UIandTaskDic = new Dictionary<UI, task>();
            var contentList = scrollRect.Content.GetList();
            contentList.Clear();
            //TODO:
            int i = 0;
            foreach (var gameTask in taskList)
            {
                i++;
                //Log.Debug($"gameTask {gameTask.ToString()}");
                var task = tbtask.Get(gameTask.TaskId);
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
                    await contentList.CreateWithUITypeAsync(UIType.UISubPanel_Task_DAndW, gameTask.TaskId, state,
                        false);


                var KImg_LeftBg = taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_LeftBg);
                var KPos_Right = taskUI.GetFromReference(UISubPanel_Task_DAndW.KPos_Right);
                var KBg_Mask = taskUI.GetFromReference(UISubPanel_Task_DAndW.KBg_Mask);
                KBg_Mask.SetActive(false);
                taskUI.GetRectTransform().SetWidth(contentW);
                string nameStr = nameString(gameTask.TaskId);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KPos).SetActive(false);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KPos2).SetActive(true);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_Task_Name2).GetTextMeshPro().SetTMPText(nameStr);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_Task_Num2).SetActive(false);
                var reUI = taskUI.GetFromReference(UISubPanel_Task_DAndW.KCommon_RewardItem);
                reUI.SetActive(true);
                JiYuUIHelper.SetRewardIconAndCountText(task.reward[0], reUI);
                JiYuUIHelper.SetRewardOnClick(task.reward[0], reUI);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_ScoreNum).GetTextMeshPro()
                    .SetTMPText(task.score.ToString());
                // taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_ScoreNum2).GetTextMeshPro()
                //     .SetTMPText(task.score.ToString());

                int serverPara = gameTask.Para;
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_ProgressBar2).GetTextMeshPro()
                    .SetTMPText(gameTask.Para.ToString() + "/" + tbtask.Get(gameTask.TaskId).para[0]);

                // if (task.Para > tbtask.Get(task.TaskId).para[0])
                // {
                //     task.Para = tbtask.Get(task.TaskId).para[0];
                // }

                float rightOffsetHelp =
                    (float)(tbtask.Get(gameTask.TaskId).para[0] - serverPara) /
                    (float)tbtask.Get(gameTask.TaskId).para[0];
                //float wHelp = taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar_Bg2).GetRectTransform().Width();

                //            taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar2).GetRectTransform().SetOffsetWithRight(-wHelp * rightOffsetHelp);
                taskUI.GetFromReference(UISubPanel_Task_DAndW.KProgressBar2).GetXImage()
                    .SetFillAmount(1 - rightOffsetHelp);

                bool canReceive = gameTask.Para >= task.para[0] && gameTask.Status == 0;

                var itemStr = $"{m_RedDotName}|Bottom{dayId}|Task{gameTask.TaskId}";
                RedDotManager.Instance.SetRedPointCnt(itemStr, canReceive ? 1 : 0);

                switch (state)
                {
                    case 0:
                        //可领取
                        //taskUI.GetFromReference(UISubPanel_Task_DAndW.KBg).GetXImage().SetAlpha(1);
                        JiYuUIHelper.SetRewardIconAndCountText(task.reward[0], reUI);
                        //SetTaskUIAlpha(taskUI);
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
                        JiYuUIHelper.SetRewardIconAndCountText(task.reward[0], reUI);
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
                        JiYuUIHelper.SetRewardIconAndCountText(task.reward[0], reUI, 0.3f);
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
                    //     JiYuUIHelper.SetRewardIconAndCountText(task.reward[0], reUI);
                    //     SetTaskUIAlpha(taskUI);
                    //     taskUI.GetFromReference(UISubPanel_Task_DAndW.KBtn).SetActive(true);
                    //     taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_Dui).SetActive(false);
                    //     taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_Receive).GetTextMeshPro()
                    //         .SetTMPText(tblanguage.Get("common_state_goto").current);
                    //     break;
                }

                var btn = taskUI.GetFromReference(UISubPanel_Task_DAndW.KBtn);
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
                {
                    JiYuUIHelper.DestoryAllTips();
                    switch (state)
                    {
                        case 0:
                            //可领取
                            NetWorkManager.Instance.SendMessage(CMD.GETTASKSCORE, new IntValue
                            {
                                Value = gameTask.TaskId
                            });
                            break;
                        case 1:
                            //前往
                            Log.Debug($"前往");
                            var taskType = tbtask_type.Get(tbtask.Get(gameTask.TaskId).type);

                            JiYuUIHelper.GoToSomePanel(taskType.goto0);
                            Close();
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


            foreach (var task in contentList.Children)
            {
                i++;
                //JiYuTweenHelper.SetEaseAlphaAndPosB2U(task.GetFromReference(UISubPanel_Task_DAndW.KBg), 0, 40, 0.35f, true, false);
                if (!isInit)
                {
                    JiYuTweenHelper.SetEaseAlphaAndPosB2U(task.GetFromReference(UISubPanel_Task_DAndW.KBg), 0, 50,
                        cancellationToken: cts.Token, 0.35f + 0.02f * i, true, true);
                }
                //await UniTask.Delay(2 * i);
            }
            // contentList.Sort(delegate(UI ui1, UI ui2)
            // {
            //     var t1 = UIandTaskDic[ui1];
            //     var t2 = UIandTaskDic[ui2];
            //     if (IDandStateDic[t1.id] < IDandStateDic[t2.id])
            //     {
            //         return -1;
            //     }
            //     else if (IDandStateDic[t1.id] > IDandStateDic[t2.id])
            //     {
            //         return 1;
            //     }
            //     else
            //     {
            //         return t1.sort.CompareTo(t2.sort);
            //     }
            // });

            //SetContentH(taskList.Count);
            //JiYuUIHelper.ForceRefreshLayout(GetFromReference(KContent));
        }

        private ActivityTask GetTaskByID(int input)
        {
            ActivityTask task = new ActivityTask();
            foreach (var severTask in ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId]
                         .OpenServerActivity.TaskList)
            {
                if (severTask.TaskId == input)
                {
                    task = severTask;
                    break;
                }
            }

            return task;
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
            //taskUI.GetFromReference(UISubPanel_Task_DAndW.KText_ScoreNum2).GetTextMeshPro().SetAlpha(alpha);
            taskUI.GetFromReference(UISubPanel_Task_DAndW.KImg_Score2).GetXImage().SetAlpha(alpha);
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

        private void SetContentH(int num)
        {
            GetFromReference(KContent).GetRectTransform().SetHeight((TaskH + 20) * num - DeltaH);
            GetFromReference(KContent).GetRectTransform().SetOffsetWithLeft(0);
            GetFromReference(KContent).GetRectTransform().SetOffsetWithRight(0);
            SetMovementType((TaskH + DeltaH) * num - DeltaH);
        }

        private void SetMovementType(float ContentH)
        {
            if (ContentH > this.GetFromReference(KScrollView).GetRectTransform().Height())
            {
                this.GetFromReference(KScrollView).GetComponent<ScrollRect>().movementType =
                    ScrollRect.MovementType.Elastic;
            }
            else
            {
                this.GetFromReference(KScrollView).GetComponent<ScrollRect>().movementType =
                    ScrollRect.MovementType.Clamped;
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
            timerId = timerMgr.StartRepeatedTimer(500, this.Update);
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


        private void OnGetTaskResponse(object sender, WebMessageHandler.Execute e)
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


                NetWorkManager.Instance.SendMessage(CMD.QUERTSINGLEACTIVITY, new IntValue()
                {
                    Value = activityId
                });
            }
        }

        private void OnGetBoxResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.GETTASKBOX, OnGetBoxResponse);
            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);
            Log.Debug("OnGetBoxResponse", Color.red);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            List<Vector3> reList = new List<Vector3>();
            foreach (var itemstr in stringValueList.Values)
            {
                reList.Add(UnityHelper.StrToVector3(itemstr));
            }

            UIHelper.Create(UIType.UICommon_Reward, reList);

            //WebMessageHandler.Instance.AddHandler(CMD.QUERTSINGLEACTIVITY, OnAllDataResponse);
            NetWorkManager.Instance.SendMessage(CMD.QUERTSINGLEACTIVITY, new IntValue()
            {
                Value = activityId
            });
        }

        private void OnActivityChallengeResponse(object sender, WebMessageHandler.Execute e)
        {
            //var s=sender as GameActivity;
            //WebMessageHandler.Instance.RemoveHandler(CMD.QUERTSINGLEACTIVITY, OnInitResponse);
            GameActivity gameActivity = new GameActivity();
            gameActivity.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug($"OnAllDataResponse is empty");
                //return;
            }
            //gameActivity.OpenServerActivity.

            if (ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.ContainsKey(activityId))
            {
                ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId] = gameActivity;
            }
            else
            {
                ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.Add(activityId,
                    gameActivity);
            }

            if (!isInit)
            {
                InitNode();
                isInit = true;
            }
            else
            {
                AllPanelUpdate();
            }
        }

        private void OnAllDataResponse(object sender, WebMessageHandler.Execute e)
        {
            //WebMessageHandler.Instance.RemoveHandler(CMD.QUERTSINGLEACTIVITY, OnAllDataResponse);
            GameActivity gameActivity = new GameActivity();
            gameActivity.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug($"OnAllDataResponse is empty");
                return;
            }

            if (ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.ContainsKey(activityId))
            {
                ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId] = gameActivity;
            }
            else
            {
                ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.Add(activityId,
                    gameActivity);
            }

            AllPanelUpdate();
        }

        // private void OnOnlyUpDataResponse(object sender, WebMessageHandler.Execute e)
        // {
        //     GameActivity gameActivity = new GameActivity();
        //     gameActivity.MergeFrom(e.data);
        //     if (e.data.IsEmpty)
        //     {
        //         Debug.Log(activityId.ToString() + "is empty");
        //         return;
        //     }
        //
        //     if (ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.ContainsKey(activityId))
        //     {
        //         ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId] = gameActivity;
        //     }
        //     else
        //     {
        //         ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.Add(activityId, gameActivity);
        //     }
        //
        //     //OnlyUpdateUp();
        // }

        protected override void OnClose()
        {
            RemoveTimer();
            JiYuUIHelper.DestoryAllTips();
            cts.Cancel();
            cts.Dispose();
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERTSINGLEACTIVITY, OnActivityChallengeResponse);
            WebMessageHandler.Instance.RemoveHandler(CMD.GETTASKSCORE, OnGetTaskResponse);
            RedDotManager.Instance.ClearChildrenListeners(m_RedDotName);
            base.OnClose();
        }

        private void InitRedDot()
        {
            m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);
            //bool hasRedDot = false;
            var scorList = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId]
                .OpenServerActivity.TaskScoreList;
            var activityTasks = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId]
                .OpenServerActivity.TaskList;
            for (int i = 0; i < scorList.Count; i++)
            {
                var itemStr = $"{m_RedDotName}|Up{i}";
                RedDotManager.Instance.InsterNode(itemStr);
                // RedDotManager.Instance.SetRedPointCnt(itemStr,
                //     scorList[i].Status == 2 ? 1 : 0);
            }

            var taskGroupsList = tbtask_group.DataList.Where((a) => a.tagFunc == tagFunc).ToList();

            for (int i = 0; i < taskGroupsList.Count; i++)
            {
                var itemStr = $"{m_RedDotName}|Bottom{i + 1}";
                RedDotManager.Instance.InsterNode(itemStr);
                var taskList = activityTasks.Where((a) => a.Day == i + 1)
                    .ToList();
                foreach (var task in taskList)
                {
                    var taskStr = $"{m_RedDotName}|Bottom{i + 1}|Task{task.TaskId}";
                    RedDotManager.Instance.InsterNode(taskStr);
                }
            }
        }
    }
}
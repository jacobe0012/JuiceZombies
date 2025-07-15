//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Main;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Google.Protobuf;
using System.Threading;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Activity_NewSign)]
    internal sealed class UIPanel_Activity_NewSignEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Activity_NewSign;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Activity_NewSign>();
        }
    }

    public partial class UIPanel_Activity_NewSign : UI, IAwake<int>
    {
        #region property

        private Tblanguage tblanguage;
        private Tbtag_func tbtag_Func;
        private Tbdays_sign tbdays_sign;
        private Tbactivity tbactivity;
        private Tbtask tbtask;
        private Tbtask_group tbtask_Group;
        private Tbart tbart;


        private int activityId;
        private int tagFunc;
        private string thisModuleRoot = "module3302RedRooT";
        private long timerId = 0;
        private string subKeyHelp = "3302day";
        private int days = 0;
        private UI TextCountDownUI;
        private string m_RedDotName;
        private long endTime;
        private bool isInit;
        private int today = -1;

        #endregion

        #region test

        private Vector3 testV3 = new Vector3(5, 0, 200);
        private CancellationTokenSource cts = new CancellationTokenSource();

        #endregion

        public async void Initialize(int acID)
        {

          
            await JiYuUIHelper.InitBlur(this);
            GetFromReference(KBtn_Bg).GetXButton().OnClick?.Add(() => { CloseThisPanel(); });
            if (!ResourcesSingleton.Instance.isConnectSuccess)
            {
                GetFromReference(KConnectSuc).SetActive(false);
                GetFromReference(KConnectFail).SetActive(true);
                return ;
            }
            GetFromReference(KConnectSuc).SetActive(true);
            GetFromReference(KConnectFail).SetActive(false);
            this.GetFromReference(KImg_TitleBg).GetComponent<CanvasGroup>().alpha = 0;


            InitJson();
            activityId = acID;
            WebMessageHandler.Instance.AddHandler(CMD.QUERTSINGLEACTIVITY, OnInitNewSignResponse);
            var activity = tbactivity.Get(activityId);
            var daysSign = tbdays_sign.Get(activity.link);
            tagFunc = daysSign.tagFunc;

            //m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);

            NetWorkManager.Instance.SendMessage(CMD.QUERTSINGLEACTIVITY, new IntValue
            {
                Value = activityId
            });
        }

        void InitRedDot()
        {
            m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);
            for (int i = 1; i <= days; i++)
            {
                var itemStr = $"{m_RedDotName}|Item{i}";
                RedDotManager.Instance.InsterNode(itemStr);
            }
        }

        private async void InitNode()
        {
            DataInit();
            InitRedDot();
            TextInit();
            CreateOneDaySign().Forget();
            StartTimer();
            //InitRedPoint();
            SetCloseTip(GetFromReference(KImg_Mask));
    
            //JiYuTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(KImg_Tittle));
            JiYuTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(KImg), cancellationToken: cts.Token);
            await UniTask.Delay(200, cancellationToken: cts.Token);
            await JiYuTweenHelper.SetEaseAlphaAndPosLtoR(this.GetFromReference(KImg_TitleBg),0,cancellationToken:cts.Token);
            
            JiYuTweenHelper.PlayUIImageSweepFX(this.GetFromReference(KImg_Tittle), cts.Token, "FFFFFF",
                JiYuTweenHelper.UIDir.Right, 2f);
        }
        public  void SetCloseTip(UI ui)
        {
            ui.GetButton().OnClick?.Add(()=> { JiYuUIHelper.DestoryAllTips();ui.SetActive(false); });
        }
        private async void OnInitNewSignResponse(object sender, WebMessageHandler.Execute e)
        {
            //WebMessageHandler.Instance.RemoveHandler(CMD.QUERTSINGLEACTIVITY, OnInitNewSignResponse);
            GameActivity gameActivity = new GameActivity();
            gameActivity.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug($"OnInitNewSignResponse is empty");
                //return;
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

            if (!isInit)
            {
                InitNode();
                isInit = true;
            }
            else
            {
                DataInit();
                TextInit();
                CreateOneDaySign().Forget();
            }
        }

        
    
       

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtag_Func = ConfigManager.Instance.Tables.Tbtag_func;
            tbdays_sign = ConfigManager.Instance.Tables.Tbdays_sign;
            tbactivity = ConfigManager.Instance.Tables.Tbactivity;
            tbtask = ConfigManager.Instance.Tables.Tbtask;
            tbtask_Group = ConfigManager.Instance.Tables.Tbtask_group;
            tbart = ConfigManager.Instance.Tables.Tbart;
        }

        private void DataInit()
        {
            days = tbtask_Group.DataList.Where(a => a.tagFunc == tagFunc).Count();


            ActivitySign activitySign =
                ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId].ActivitySign;


            //var todayActivityTask = activitySign.TaskList.FirstOrDefault((a) => a.Status == 0 && a.Score == 0);


            today = activitySign.TaskList.Where(a => a.Score != 0).Count();
            today = Mathf.Max(1, today);
            // if (todayTask == null)
            // {
            //     today = activitySign.TaskList.Count();
            // }
            // else
            // {
            //     today = todayTask.Day - 1;
            // }

            Log.Debug($"firstLockedDay {today} activitySign.TaskList{activitySign.TaskList.ToString()}",
                Color.green);

            endTime = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId].ActivitySign
                .EndTime;
        }

        private void UpdateThisPanel()
        {
            //UpdateRedPoint();
            CreateOneDaySign().Forget();
        }

        // private void InitRedPoint()
        // {
        //     RedPointMgr.instance.Remove(thisModuleRoot, thisModuleRoot);
        //     RedPointMgr.instance.Add(thisModuleRoot, null, null, RedPointType.Enternal);
        //     for (int i = 1; i <= days; i++)
        //     {
        //         int ihelp = i;
        //         RedPointMgr.instance.Add(thisModuleRoot, subKeyHelp + ihelp.ToString(), thisModuleRoot,
        //             RedPointType.Once);
        //     }
        //
        //     UpdateRedPoint();
        // }
        //
        // private void UpdateRedPoint()
        // {
        //     for (int i = 1; i <= days; i++)
        //     {
        //         int ihelp = i;
        //         var aT = GetActivityTaskByDay(ihelp);
        //         if (ihelp <= today && aT.Status == 0)
        //         {
        //             RedPointMgr.instance.SetState(thisModuleRoot, subKeyHelp + ihelp.ToString(), RedPointState.Show);
        //         }
        //         else
        //         {
        //             RedPointMgr.instance.SetState(thisModuleRoot, subKeyHelp + ihelp.ToString(), RedPointState.Hide);
        //         }
        //     }
        // }

        private ActivityTask GetActivityTaskByDay(int day)
        {
            // ActivityTask task = new ActivityTask();
            //
            // foreach (var aT in ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId].ActivitySign
            //              .TaskList)
            // {
            //     if (input == aT.Day)
            //     {
            //         task = aT;
            //         break;
            //     }
            // }
            var singTask = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId].ActivitySign
                .TaskList;
            if (day > singTask.Count)
            {
                return null;
            }

            return singTask.FirstOrDefault(a => a.Day == day);
        }


        private void TextInit()
        {
            var daysSign = tbdays_sign.Get(tbactivity.Get(activityId).link);
            // GetFromReference(KText_Name).GetTextMeshPro()
            //     .SetTMPText(tblanguage.Get(daysSign.name).current);
            var picName = JiYuUIHelper.GetL10NPicName("days_sign_title");
            //Log.Error($"picName {picName}");
            var KText_Name = GetFromReference(UIPanel_Activity_NewSign.KImg_Tittle);
            KText_Name.GetImage().SetSprite(picName, true);
            GetFromReference(KText_Desc).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(daysSign.desc).current);
            GetFromReference(KText_Desc_1).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(daysSign.desc).current);
            var KImg = GetFromReference(UIPanel_Activity_NewSign.KImg);
            KImg.GetImage().SetSpriteAsync(daysSign.pic, false).Forget();
        }

        private void OnGetTaskResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.GETTASKSCORE, OnGetTaskResponse);
            TaskResult taskResult = new TaskResult();
            taskResult.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                //return;
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

        private async UniTaskVoid CreateOneDaySign()
        {
            var posList = GetFromReference(KPos_EveryDay).GetList();
            posList.Clear();
            float layoutWidth = GetFromReference(KPos_EveryDay).GetRectTransform().Width();


            //Dictionary<UI, int> UIandSortDic = new Dictionary<UI, int>();

            for (int i = 1; i <= days; i++)
            {
                int day = i;
                var aT = GetActivityTaskByDay(day);
                var ui =
                    posList.CreateWithUIType(UIType.UISubPanel_Activity_OneDaySign, day, false) as
                        UISubPanel_Activity_OneDaySign;
                //var KImg_DayBgSp = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KImg_DayBgSp);
                //var KImg_ItemBgSp = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KImg_ItemBgSp);
                var KUnlockTime = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KUnlockTime);
                var KCommon_Reward1 = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KCommon_Reward1);
                var KCommon_Reward2 = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KCommon_Reward2);
                var KCommon_Reward3 = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KCommon_Reward3);
                var KCommon_Reward4 = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KCommon_Reward4);
                var KBtn_Get = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KBtn_Get);
                var KImg_Got = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KImg_Got);
                var KText_Get = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KText_Get);
                var KImg_ItemBg = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KImg_ItemBg);
                var KText_Locked = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KText_Locked);
                var KText_Day = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KText_Day);
                var KText_DayNum = ui.GetFromReference(UISubPanel_Activity_OneDaySign.KText_DayNum);
                KText_Get.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gain").current);

                ui.GetRectTransform().SetWidth(layoutWidth);
                //第<size=90><color=#4555>5</color></size>天
                //active_days_text
                //var numStr = UnityHelper.RichTextSize(day.ToString(), 90);
                //var dayStr = string.Format(tblanguage.Get("active_days_text").current, numStr);
                //var dayStr = tblanguage.Get("active_days_text").current;

                //KText_Day.GetTextMeshPro().SetTMPText(dayStr);
                KText_DayNum.GetTextMeshPro().SetTMPText($"No.{day.ToString()}");

                //KImg_DayBgSp.SetActive(JudgeDayEqualSp(day));
                //KImg_ItemBgSp.SetActive(JudgeDayEqualSp(day));
                if (JudgeDayEqualSp(day))
                {
                    KImg_ItemBg.GetXImage().SetSpriteAsync($"activity_days_sign_item_received_container", true)
                        .Forget();
                }
                else
                {
                    KImg_ItemBg.GetXImage().SetSpriteAsync($"activity_days_sign_item_container", true).Forget();
                }

                KUnlockTime.SetActive(false);
                KText_Locked.SetActive(false);


                KBtn_Get.SetActive(false);
                KImg_Got.SetActive(false);
                var itemStr = $"{m_RedDotName}|Item{day}";
                RedDotManager.Instance.SetRedPointCnt(itemStr, 0);
                if (day <= today)
                {
                    if (aT.Status == 1)
                    {
                        KBtn_Get.SetActive(false);
                        KImg_Got.SetActive(true);
                    }
                    else if (aT != null && (aT.Status == 0 || aT.Status == null))
                    {
                        KBtn_Get.SetActive(true);
                        KImg_Got.SetActive(false);

                        RedDotManager.Instance.SetRedPointCnt(itemStr, 1);

                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Get, () =>
                        {
                            JiYuUIHelper.DestoryAllTips();
                            WebMessageHandler.Instance.AddHandler(CMD.GETTASKSCORE, OnGetTaskResponse);
                            NetWorkManager.Instance.SendMessage(CMD.GETTASKSCORE, new IntValue
                            {
                                Value = aT.TaskId
                            });
                        });
                    }
                }
                else if (today + 1 <= days && day == today + 1)
                {
                    KUnlockTime.SetActive(true);

                    ui.StartTimer();
                    // var str =
                    //     $"{tblanguage.Get("active_lock_text").current}\n{timeStr}";

                    //KText_Lock.GetTextMeshPro().SetTMPText(str);

                    KBtn_Get.SetActive(false);
                    KImg_Got.SetActive(false);
                }
                else
                {
                    KText_Locked.SetActive(true);
                    KText_Locked.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_locking").current);
                    KBtn_Get.SetActive(false);
                    KImg_Got.SetActive(false);
                }


                //set reward

                List<Vector3> rewardList = FindV3ByDay(day);
                switch (rewardList.Count)
                {
                    case 0:
                        KCommon_Reward1.SetActive(false);
                        KCommon_Reward2.SetActive(false);
                        KCommon_Reward3.SetActive(false);
                        KCommon_Reward4.SetActive(false);
                        break;
                    case 1:
                        KCommon_Reward1.SetActive(true);
                        KCommon_Reward2.SetActive(false);
                        KCommon_Reward3.SetActive(false);
                        KCommon_Reward4.SetActive(false);
                        JiYuUIHelper.SetRewardIconAndCountText(rewardList[0], KCommon_Reward1);
                        JiYuUIHelper.SetRewardOnClick(rewardList[0], KCommon_Reward1, GetFromReference(KImg_Mask));
                        break;
                    case 2:
                        KCommon_Reward1.SetActive(true);
                        KCommon_Reward2.SetActive(true);
                        KCommon_Reward3.SetActive(false);
                        KCommon_Reward4.SetActive(false);
                        JiYuUIHelper.SetRewardIconAndCountText(rewardList[0], KCommon_Reward1);
                        JiYuUIHelper.SetRewardIconAndCountText(rewardList[1], KCommon_Reward2);
                        JiYuUIHelper.SetRewardOnClick(rewardList[0], KCommon_Reward1, GetFromReference(KImg_Mask));
                        JiYuUIHelper.SetRewardOnClick(rewardList[1], KCommon_Reward2, GetFromReference(KImg_Mask));
                        break;
                    case 3:
                        KCommon_Reward1.SetActive(true);
                        KCommon_Reward2.SetActive(true);
                        KCommon_Reward3.SetActive(true);
                        KCommon_Reward4.SetActive(false);
                        JiYuUIHelper.SetRewardIconAndCountText(rewardList[0], KCommon_Reward1);
                        JiYuUIHelper.SetRewardIconAndCountText(rewardList[1], KCommon_Reward2);
                        JiYuUIHelper.SetRewardIconAndCountText(rewardList[2], KCommon_Reward3);
                        JiYuUIHelper.SetRewardOnClick(rewardList[0], KCommon_Reward1, GetFromReference(KImg_Mask));
                        JiYuUIHelper.SetRewardOnClick(rewardList[1], KCommon_Reward2, GetFromReference(KImg_Mask));
                        JiYuUIHelper.SetRewardOnClick(rewardList[2], KCommon_Reward3, GetFromReference(KImg_Mask));
                        break;
                    case 4:
                        KCommon_Reward1.SetActive(true);
                        KCommon_Reward2.SetActive(true);
                        KCommon_Reward3.SetActive(true);
                        KCommon_Reward4.SetActive(true);
                        JiYuUIHelper.SetRewardIconAndCountText(rewardList[0], KCommon_Reward1);
                        JiYuUIHelper.SetRewardIconAndCountText(rewardList[1], KCommon_Reward2);
                        JiYuUIHelper.SetRewardIconAndCountText(rewardList[2], KCommon_Reward3);
                        JiYuUIHelper.SetRewardIconAndCountText(rewardList[3], KCommon_Reward4);
                        JiYuUIHelper.SetRewardOnClick(rewardList[0], KCommon_Reward1, GetFromReference(KImg_Mask));
                        JiYuUIHelper.SetRewardOnClick(rewardList[1], KCommon_Reward2, GetFromReference(KImg_Mask));
                        JiYuUIHelper.SetRewardOnClick(rewardList[2], KCommon_Reward3, GetFromReference(KImg_Mask));
                        JiYuUIHelper.SetRewardOnClick(rewardList[3], KCommon_Reward4, GetFromReference(KImg_Mask));
                        break;
                    default:
                        KCommon_Reward1.SetActive(false);
                        KCommon_Reward2.SetActive(false);
                        KCommon_Reward3.SetActive(false);
                        KCommon_Reward4.SetActive(false);
                        break;
                }

                JiYuTweenHelper.SetEaseAlphaAndPosB2U(ui.GetFromReference(UISubPanel_Activity_OneDaySign.KImg_ItemBg), 0,
                    60, cts.Token, 0.3F, true, true);
                await UniTask.Delay(2 * i, cancellationToken: cts.Token);
            }

            posList.Sort(delegate(UI a, UI b)
            {
                var uia = a as UISubPanel_Activity_OneDaySign;
                var uib = b as UISubPanel_Activity_OneDaySign;
                return uia.day.CompareTo(uib.day);
            });


            //for (int i=0;i<posList.Children.Count;i++)
            //{
            //    var ui=posList.Children[i];
            //    ui.GetFromReference(UISubPanel_Activity_OneDaySign.KImg_ItemBg).GetComponent<CanvasGroup>().alpha = 0.8f-i*0.1f;
            //    await UniTask.Delay(10 * i);
            //    ui.GetFromReference(UISubPanel_Activity_OneDaySign.KImg_ItemBg).GetComponent<CanvasGroup>().DOFade(1, 0.45f).SetEase(Ease.OutQuad);
            //}
        }

        private bool JudgeDayEqualSp(int input)
        {
            return tbdays_sign.Get(tbactivity.Get(activityId).link).specialDate.Contains(input);
        }

        private List<Vector3> FindV3ByDay(int input)
        {
            int groupID = 0;
            foreach (var tbtg in tbtask_Group.DataList)
            {
                if (tbtg.tagFunc == tagFunc && tbtg.day == input)
                {
                    groupID = tbtg.id;
                    break;
                }
            }

            List<Vector3> v3s = new List<Vector3>();
            foreach (var tbtk in tbtask.DataList)
            {
                if (tbtk.group == groupID && tbtk.sort == 1)
                {
                    v3s = tbtk.reward;
                    break;
                }
            }

            return v3s;
        }

        private void CloseThisPanel()
        {
            JiYuUIHelper.DestoryAllTips();
            RemoveTimer();
            Close();
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
            var KText_Time = GetFromReference(UIPanel_Activity_NewSign.KText_Time);
            string cdStr = tblanguage.Get("active_countdown_text").current;
            long clientT = JiYuUIHelper.GetServerTimeStamp(true);
            if (!JiYuUIHelper.TryGetRemainingTime(clientT, endTime, out var timeStr))
            {
                Close();
            }

            cdStr += timeStr;

            KText_Time.GetTextMeshPro().SetTMPText(cdStr);
        }


        protected override void OnClose()
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERTSINGLEACTIVITY, OnInitNewSignResponse);
            RedDotManager.Instance.ClearChildrenListeners(m_RedDotName);
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
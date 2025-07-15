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
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Achieve)]
    internal sealed class UIPanel_AchieveEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Achieve;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Achieve>();
        }
    }

    public partial class UIPanel_Achieve : UI, IAwake
    {
        private Tbachieve_group tbachieve_group;
        private Tblanguage tblanguage;
        private Tbtask tbtask;

        private Tbachieve tbachieve;

        //private Tbachieve_list tbachieve_list;
        private int groupNum = 0;

        private int NowLevel = 1;

        //private Dictionary<int, UI> groupDic = new Dictionary<int, UI>();
        private Dictionary<int, List<int>> groupIDs = new Dictionary<int, List<int>>();
        private bool StrongTipIsActive = false;
        private int NOWEXP = 0;
        private int tagFunc = 5105;
        public string m_RedDotName;
        private bool isInit;

        private CancellationTokenSource cts = new CancellationTokenSource();
        private long timerId0;

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
            InitJson();
            StartTimer();
            var KImg_Achieve = GetFromReference(UIPanel_Achieve.KImg_Achieve);
            KImg_Achieve.SetActive(false);
            WebMessageHandler.Instance.AddHandler(CMD.QUERYACHIEVEMENT, OnQueryAchievementResponse);

            //tagFunc = daysSign.tagFunc;

            //JiYuUIHelper.EnableLoading(true);
            NetWorkManager.Instance.SendMessage(CMD.QUERYACHIEVEMENT);

            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UIPanel_Achieve.KMid), 180, 100, cts.Token, 0.15f,
                false);
            JiYuTweenHelper.SetEaseAlphaAndPosLtoR(GetFromReference(UIPanel_Achieve.KMid), 0, 100, cts.Token, 0.15f,
                false);
            GetFromReference(UIPanel_Achieve.KMid).GetComponent<CanvasGroup>().alpha = 0f;
            GetFromReference(UIPanel_Achieve.KMid).GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetEase(Ease.InQuad);
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtask = ConfigManager.Instance.Tables.Tbtask;
            tbachieve_group = ConfigManager.Instance.Tables.Tbachieve_group;
            tbachieve = ConfigManager.Instance.Tables.Tbachieve;
            //tbachieve_list = ConfigManager.Instance.Tables.Tbachieve_list;
        }

        async UniTask InitNode()
        {
            var KBg = GetFromReference(UIPanel_Achieve.KBg);
            var KPos_Up = GetFromReference(UIPanel_Achieve.KPos_Up);
            var KImg_Achieve = GetFromReference(UIPanel_Achieve.KImg_Achieve);
            var KProgressBar_Bg = GetFromReference(UIPanel_Achieve.KProgressBar_Bg);
            var KProgressBar = GetFromReference(UIPanel_Achieve.KProgressBar);
            var KText_ProgressBar = GetFromReference(UIPanel_Achieve.KText_ProgressBar);
            var KImg_Level = GetFromReference(UIPanel_Achieve.KImg_Level);
            var KText_Level_Num = GetFromReference(UIPanel_Achieve.KText_Level_Num);
            var KText_Level = GetFromReference(UIPanel_Achieve.KText_Level);
            var KBtn_Strong = GetFromReference(UIPanel_Achieve.KBtn_Strong);
            var KText_Strong = GetFromReference(UIPanel_Achieve.KText_Strong);
            var KText_Achieve = GetFromReference(UIPanel_Achieve.KText_Achieve);
            var KBtn_Achieve_Gift = GetFromReference(UIPanel_Achieve.KBtn_Achieve_Gift);
            var KImg_Gift_RedPoint = GetFromReference(UIPanel_Achieve.KImg_Gift_RedPoint);
            var KScrollView = GetFromReference(UIPanel_Achieve.KScrollView);
            var KTip_Strong = GetFromReference(UIPanel_Achieve.KTip_Strong);
            var KImg_Strong = GetFromReference(UIPanel_Achieve.KImg_Strong);
            var KPos_Layout = GetFromReference(UIPanel_Achieve.KPos_Layout);
            var KPos_Gift_Tip = GetFromReference(UIPanel_Achieve.KPos_Gift_Tip);

            //NowLevel = level;

            //TODO:等级

            JiYuTweenHelper.PlayUIImageTranstionFX(KImg_Achieve, cancellationToken: cts.Token);
            DataInit();
            ShowLevelExp(ResourcesSingleton.Instance.achieveInfo.ScoreSum);
            InitRedDot();
            ScrollConteneInit();
            await CreateGroup();
            PosUpSet();
            //SetStrongTip();
            //SetBtnCloseTip();
            KImg_Level.GetXButton().OnClick.Add(() => { CloseStrongTip(); });
            KBg.GetXButton().OnClick.Add(async () => { ClosePanel(); });

            // var KBtn_Strong = GetFromReference(UIPanel_Achieve.KBtn_Strong);
            // var KBtn_Achieve_Gift = this.GetFromReference(UIPanel_Achieve.KBtn_Achieve_Gift);
            //DoTweenEffect.DoScaleTweenOnClickAndLongPress(KBtn_Strong, () => { CreateTips(); });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Achieve_Gift, async () =>
            {
                var lastAchieve = ResourcesSingleton.Instance.achieveInfo.Achieve;
                CloseStrongTip();
                JiYuUIHelper.DestoryItemTips();
                if (NowLevel - lastAchieve.Id >= 2)
                {
                    GetGift(NowLevel);
                }
                else
                {
                    //create item tip
                    //var tipList = this.GetFromReference(KPos_Gift_Tip).GetList();
                    var child = GetChild(UIType.UICommon_Reward_Tip);
                    if (child == null)
                    {
                        var tipUI = await UIHelper.CreateAsync(this, UIType.UICommon_Reward_Tip,
                            tbachieve.Get(NowLevel).reward, KBtn_Achieve_Gift.GetRectTransform().Get());
                        tipUI.SetParent(this, true);

                        tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownLeft).SetActive(false);
                        tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownUp).SetActive(true);
                        tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownRight).SetActive(false);
                        tipUI.GetRectTransform().SetAnchoredPosition(0,
                            -tipUI.GetRectTransform().Height() / 2f-40f);
                    }
                    else
                    {
                        child.SetActive(!child.GameObject.activeSelf);
                    }
                }
            });
        }

        private void StartTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerId0 = timerMgr.StartRepeatedTimer(2000, UpdateTimer);
        }

        private void UpdateTimer()
        {
            var KCommon_CloseInfo = GetFromReference(UIPanel_Patrol.KCommon_CloseInfo);
            KCommon_CloseInfo.GetTextMeshPro().SetTMPText(tblanguage.Get("text_window_close").current);

            KCommon_CloseInfo.GetTextMeshPro().DoFade(1, 0.1f, 1f).AddOnCompleted(() =>
            {
                KCommon_CloseInfo.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });
        }
        private void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId0);
            this.timerId0 = 0;
            //FrameNum = 49;
        }
        private async UniTask ClosePanel()
        {
            RemoveTimer();
            JiYuTweenHelper.SetEaseAlphaAndPosUtoB(GetFromReference(UIPanel_Achieve.KMid), 0 - 100, 100, cts.Token, 0.15f,
                false);
            JiYuTweenHelper.SetEaseAlphaAndPosRtoL(GetFromReference(UIPanel_Achieve.KMid), 0 - 100, 100, cts.Token, 0.15f,
                false);
            GetFromReference(UIPanel_Achieve.KMid).GetComponent<CanvasGroup>().alpha = 1f;
            GetFromReference(UIPanel_Achieve.KMid).GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetEase(Ease.InQuad);
            await UniTask.Delay(150);
            Close();
        }

        public void Update()
        {
            DataInit();
            ShowLevelExp(ResourcesSingleton.Instance.achieveInfo.ScoreSum);
            //InitRedDot();
            //ScrollConteneInit();
            CreateGroup().Forget();
            PosUpSet();
            //SetStrongTip();
        }

        public void ShowLevelExp(long exp)
        {
            var KProgressBar = GetFromReference(UIPanel_Achieve.KProgressBar);
            var KText_ProgressBar = GetFromReference(UIPanel_Achieve.KText_ProgressBar);
            var KText_Level_Num = GetFromReference(UIPanel_Achieve.KText_Level_Num);
            List<long> arr = new List<long>();
            foreach (var levellist in tbachieve.DataList)
            {
                arr.Add(levellist.score);
            }

            int result = FindFirstGreaterThan(arr, exp);
            long curExp = 0;
            long levelUpNeedExp = 0;
            int level = 0;
            double expRatios;

            if (result != -1)
            {
                level = tbachieve.DataList[result].id;
            }
            else
            {
                level = tbachieve.DataList[tbachieve.DataList.Count - 1].id;
                expRatios = 1;
                KText_Level_Num.GetTextMeshPro().SetTMPText(level.ToString());
                KProgressBar.GetImage().DoFillAmount((float)expRatios, 0.3f);
                KText_ProgressBar.GetTextMeshPro()
                    .SetTMPText($"{exp}/{tbachieve.DataList[tbachieve.DataList.Count - 1].score}");
                NOWEXP = (int)exp;
                NowLevel = level;
                arr.Clear();
                return;
            }

            // if (tbuserLevel.DataList[result + 1] != null)
            // {
            //     levelUpNeedExp = tbuserLevel.DataList[result + 1].exp - tbuserLevel.DataList[result].exp;
            //     curExp = exp - tbuserLevel.DataList[result - 1].exp;
            // }
            //
            // expRatios = curExp / (double)levelUpNeedExp;
            // if (!double.IsNaN(expRatios))
            // {
            //     slideUI.GetImage().DoFillAmount((float)expRatios, 0.3f);
            // }

            //Log.Error($"fsafsf {expRatios}  {curExp}  {(double)levelUpNeedExp}");
            expRatios = (float)exp / tbachieve.DataList[result].score;

            KText_Level_Num.GetTextMeshPro().SetTMPText(level.ToString());
            //Log.Error($"expRatios{expRatios}");
            KProgressBar.GetImage().DoFillAmount((float)expRatios, 0.3f);
            KText_ProgressBar.GetTextMeshPro()
                .SetTMPText($"{exp}/{tbachieve.DataList[result].score}");
            NOWEXP = (int)exp;
            NowLevel = level;
            arr.Clear();
        }

        private int FindFirstGreaterThan(List<long> list, long target)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] > target)
                {
                    return i;
                }
            }

            return -1;
        }

        // private void SetStrongTip()
        // {
        //     var textList = this.GetFromReference(KPos_Layout).GetList();
        //     textList.Clear();
        //     for (int i = 1; i <= 3; i++)
        //     {
        //         var textUI = textList.CreateWithUIType(UIType.UISubPanel_Achieve_Text, false);
        //         textUI.GetFromReference(UISubPanel_Achieve_Text.KText).GetTextMeshPro()
        //             .SetTMPText(tblanguage.Get(tbachieve_list.Get(i).name).current + "0");
        //     }
        // }

        private void SetBtnCloseTip()
        {
            //this.GetFromReference(KProgressBar_Bg).GetXButton().OnClick.Add(() => { CloseStrongTip(); });
        }

        private void DataInit()
        {
            StrongTipIsActive = false;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbachieve_group = ConfigManager.Instance.Tables.Tbachieve_group;
            tbtask = ConfigManager.Instance.Tables.Tbtask;
            tbachieve = ConfigManager.Instance.Tables.Tbachieve;

            groupNum = tbachieve_group.DataList.Count;
            return;

            #region 排序

            foreach (var t in tbtask.DataList)
            {
                if (groupIDs.TryGetValue(t.group, out List<int> intList))
                {
                    //intList.Add(t.id);
                    groupIDs[t.group].Add(t.id);
                }
                else
                {
                    groupIDs[t.group] = new List<int> { t.id };
                }
            }

            Dictionary<int, List<int>> helpDic = new Dictionary<int, List<int>>();
            foreach (var gId in groupIDs)
            {
                List<int> ints = new List<int>();
                int i = 0;
                foreach (var id in gId.Value)
                {
                    if (tbtask.Get(id).pre == 0)
                    {
                        i++;
                    }
                }

                if (i == 1)
                {
                    Dictionary<int, int> reDependent = new Dictionary<int, int>();
                    foreach (var id in gId.Value)
                    {
                        reDependent[tbtask.Get(id).pre] = id;
                    }

                    for (int j = 0; j < gId.Value.Count; j++)
                    {
                        if (j == 0)
                        {
                            ints.Add(reDependent[j]);
                        }
                        else
                        {
                            ints.Add(reDependent[ints[j - 1]]);
                        }
                    }

                    helpDic.Add(gId.Key, ints);
                }
            }

            groupIDs = helpDic;

            ResourcesSingleton.Instance.groupIDs = helpDic;

            #endregion
        }

        void InitRedDot()
        {
            m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);
            foreach (var t in tbachieve_group.DataList)
            {
                var itemStr = $"{m_RedDotName}|Group{t.id}";
                RedDotManager.Instance.InsterNode(itemStr);
            }

            var itemStrUp = $"{m_RedDotName}|Up";
            RedDotManager.Instance.InsterNode(itemStrUp);
        }

        private void ScrollConteneInit()
        {
            //return;
            var scrollView = GetFromReference(UIPanel_Achieve.KScrollView).GetScrollRect();
            var gridLayoutGroup = scrollView.Content.GetComponent<GridLayoutGroup>();
            //var cellSize = gridLayoutGroup.cellSize;
            //cellSize.x = (Screen.width - 180) / 2f;

            //gridLayoutGroup.cellSize = cellSize;
        }

        public int GetGroupRedDot(int groupid)
        {
            var achieveGroup = tbachieve_group.Get(groupid);
            int num = 0;
            int redDotNum = 0;
            foreach (var groupId in achieveGroup.groupId)
            {
                //Log.Debug($"{groupId} is null", Color.green);
                var list = ResourcesSingleton.Instance.achieveInfo.TaskInfoList.Where(a => a.Group == groupId)
                    .ToList();
                if (list.Count <= 0)
                {
                    Log.Debug($"{groupId} is null", Color.green);
                    continue;
                }

                var groupTaskInfo = list[0];

                var task = tbtask.Get(groupTaskInfo.Id);

                bool isRedDot = groupTaskInfo.Para >= task.para[0] && groupTaskInfo.Status == 0;
                redDotNum += isRedDot ? 1 : 0;
            }

            return redDotNum;
        }

        private async UniTask CreateGroup()
        {
            var contentList = this.GetFromReference(KScrollView).GetScrollRect().Content.GetList();
            contentList.Clear();

            for (int i = 0; i < groupNum; i++)
            {
                int index = i;
                var achieveGroup = tbachieve_group.DataList[index];
                var ui = await contentList.CreateWithUITypeAsync(UIType.UISubPanel_Achieve_Group,
                    achieveGroup.id, false);
                //groupDic.Add(tbachieve_group.DataList[index].id, ui);
                var KBtn = ui.GetFromReference(UISubPanel_Achieve_Group.KBtn);
                var KText_Group_Name = ui.GetFromReference(UISubPanel_Achieve_Group.KText_Group_Name);
                var KImg_Achieve_Group = ui.GetFromReference(UISubPanel_Achieve_Group.KImg_Achieve_Group);
                var KImg_RedPoint = ui.GetFromReference(UISubPanel_Achieve_Group.KImg_RedPoint);


                KText_Group_Name.GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(achieveGroup.name).current);
                KImg_Achieve_Group.GetImage()
                    .SetSpriteAsync(achieveGroup.icon, false);
                var KText_Group_Para = ui.GetFromReference(UISubPanel_Achieve_Group.KText_Group_Para);


                var maxNum = tbtask.DataList.Where(a => achieveGroup.groupId.Exists(b => b == a.group))
                    .ToList().Count;

                int num = 0;
                int redDotNum = 0;
                foreach (var groupId in achieveGroup.groupId)
                {
                    //Log.Debug($"{groupId} is null", Color.green);
                    var list = ResourcesSingleton.Instance.achieveInfo.TaskInfoList.Where(a => a.Group == groupId)
                        .ToList();
                    if (list.Count <= 0)
                    {
                        Log.Debug($"{groupId} is null", Color.green);
                        continue;
                    }

                    var groupTaskInfo = list[0];

                    int lastThreeDigits = groupTaskInfo.Status == 1
                        ? groupTaskInfo.Id % 1000
                        : groupTaskInfo.Id % 1000 - 1;
                    num += lastThreeDigits;

                    var task = tbtask.Get(groupTaskInfo.Id);

                    bool isRedDot = groupTaskInfo.Para >= task.para[0] && groupTaskInfo.Status == 0;
                    redDotNum += isRedDot ? 1 : 0;
                }

                KImg_RedPoint.SetActive(redDotNum > 0);
                var itemStr = $"{m_RedDotName}|Group{achieveGroup.id}";
                RedDotManager.Instance.SetRedPointCnt(itemStr, redDotNum);

                // RedDotManager.Instance.AddListener(itemStr, (a) =>
                // {
                //     KImg_RedPoint.SetActive(a > 0);
                // });


                //bool canReceive = gameTask.Para >= task.para[0] && gameTask.Status == 0;

                var paraStr = $"{num}/{maxNum}";
                //num.ToString() + "/" + maxNum.ToString();
                KText_Group_Para.GetTextMeshPro()
                    .SetTMPText(paraStr);

                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn, async () =>
                {
                    //Debug.Log(ihelp);
                    CloseStrongTip();
                    var ui = await UIHelper.CreateAsync(UIType.UIPanel_Achieve_List, achieveGroup.id);
                    ui.SetParent(this, false);
                });
                //GroupSet(achieveGroup.id, ui);
            }

            contentList.Sort((a, b) =>
            {
                var uia = a as UISubPanel_Achieve_Group;
                var uib = b as UISubPanel_Achieve_Group;

                return uia.id.CompareTo(uib.id);
            });
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void UpdateGroup()
        {
            Debug.Log("update group and ResourcesSingleton.Instance.achieve.boxes.count" +
                      ResourcesSingleton.Instance.achieve.boxes.Count);
            // foreach (var d in groupDic)
            // {
            //     GroupSet(d.Key, d.Value);
            // }

            PosDownSet();
        }

        /// <summary>
        /// 设置每个单独的group的状态
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ui"></param>
        public void GroupSet(int ID, UI ui)
        {
            int num = 0;
            int maxNum = 0;
            foreach (var gid in tbachieve_group.Get(ID).groupId)
            {
                for (int i = 0; i < groupIDs[gid].Count; i++)
                {
                    maxNum++;
                }
            }

            foreach (var gid in tbachieve_group.Get(ID).groupId)
            {
                for (int i = 0; i < groupIDs[gid].Count; i++)
                {
                    int taskID = groupIDs[gid][i];
                    if (ResourcesSingleton.Instance.achieve.tasks[taskID][0] == 1)
                    {
                        num++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            ui.GetFromReference(UISubPanel_Achieve_Group.KText_Group_Para).GetTextMeshPro()
                .SetTMPText(num.ToString() + "/" + maxNum.ToString());
        }

        private void PosUpSet()
        {
            //#region 设置不需要刷新的部分

            //设置红点表现
            var KImg_Gift_RedPoint = GetFromReference(UIPanel_Achieve.KImg_Gift_RedPoint);


            this.GetFromReference(KText_Strong).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbachieve.Get(NowLevel).name).current);
            this.GetFromReference(KText_Level).GetTextMeshPro().SetTMPText(tblanguage.Get("achieve_level").current);


            var itemStrUp = $"{m_RedDotName}|Up";

            var lastAchieve = ResourcesSingleton.Instance.achieveInfo.Achieve;

            if (NowLevel - lastAchieve.Id >= 2)
            {
                RedDotManager.Instance.SetRedPointCnt(itemStrUp, 1);
                KImg_Gift_RedPoint.SetActive(true);
                //GetGift(NowLevel);
            }
            else
            {
                RedDotManager.Instance.SetRedPointCnt(itemStrUp, 0);
                KImg_Gift_RedPoint.SetActive(false);
            }


            // if (NOWEXP < tbachieve.Get(NowLevel).score)
            // {
            //     RedDotManager.Instance.SetRedPointCnt(itemStrUp, 0);
            // }
            // else
            // {
            //     RedDotManager.Instance.SetRedPointCnt(itemStrUp, 1);
            //     //GetGift(NowLevel);
            // }
            //RedPointMgr.instance.Init(UIPanel_Battle.Battle_Red_Point_Root, KBtn_Achieve_Gift,
            //    (RedPointState state, int data) =>
            //    {
            //        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Achieve, out UI ui))
            //        {
            //            this.GetFromReference(KImg_Gift_RedPoint).SetActive(state == RedPointState.Show);
            //        }
            //    });

            // #endregion
            //
            // #region 设置需要刷新的部分，后面单独出一个函数给更新用
            //
            // //PosDownSet();
            //
            // #endregion
        }

        /// <summary>
        /// 设置上半部分需要刷新的部分
        /// </summary>
        private async void PosDownSet()
        {
            return;
            //先计算当前成就点数
            int exp = 0;
            foreach (var acgroup in tbachieve_group.DataList)
            {
                foreach (int id in acgroup.groupId)
                {
                    for (int i = 0; i < groupIDs[id].Count; i++)
                    {
                        int taskID = groupIDs[id][i];
                        //如果已领取，那么获得经验
                        if (ResourcesSingleton.Instance.achieve.tasks[taskID][0] == 1)
                        {
                            exp += tbtask.Get(taskID).score;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            //根据成就点数和领取状态判断等级红点状态
            int level = 1;
            for (int i = 0; i < tbachieve.DataList.Count; i++)
            {
                if (i == 0)
                {
                    level = tbachieve.DataList[i].id;
                }
                else
                {
                    var a = tbachieve.DataList[i - 1].score;
                    //Debug.Log("i" + i);
                    //Debug.Log("i - 1:" + (i - 1).ToString());
                    //Debug.Log("tbachieve.DataList[i - 1].id" + tbachieve.DataList[i - 1].id);
                    //Debug.Log("ResourcesSingleton.Instance.achieve.boxes.count" + ResourcesSingleton.Instance.achieve.boxes.Count);
                    //foreach (var box in ResourcesSingleton.Instance.achieve.boxes)
                    //{
                    //    Debug.Log("box.key" + box.Key + "; box.value.x" + box.Value.x + "; box.value.y" + box.Value.y);
                    //}
                    var b = ResourcesSingleton.Instance.achieve.boxes[tbachieve.DataList[i - 1].id][0] == 1;
                    if (exp >= tbachieve.DataList[i - 1].score &&
                        ResourcesSingleton.Instance.achieve.boxes[tbachieve.DataList[i - 1].id][0] == 1)
                    {
                        level = tbachieve.DataList[i].id;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            NowLevel = level;
            this.GetFromReference(KText_Level_Num).GetTextMeshPro().SetTMPText(level.ToString());
            this.GetFromReference(KText_Achieve).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbachieve.Get(level).name).current);
            //根据等级和成就点数设置等级进度条
            //Debug.Log(exp);
            float pWidth = this.GetFromReference(KProgressBar_Bg).GetRectTransform().Width();
            int levelNeed = 0;
            int expDisplay = 0;

            if (level >= 2)
            {
                levelNeed = tbachieve.DataList[level - 1].score - tbachieve.DataList[level - 2].score;
                expDisplay = exp - tbachieve.DataList[level - 2].score;
            }
            else
            {
                levelNeed = tbachieve.DataList[level - 1].score - 0;
                expDisplay = exp;
            }

            this.GetFromReference(KText_ProgressBar).GetTextMeshPro()
                .SetTMPText(expDisplay.ToString() + "/" + levelNeed.ToString());

            if (exp < tbachieve.Get(level).score)
            {
                //Debug.Log("exp < tbachieve.Get(level).score");
                float wHelp = pWidth * (levelNeed - expDisplay) / levelNeed;
                //Debug.Log("levelNeed" + levelNeed);
                //Debug.Log("expDisplay" + expDisplay);
                //Debug.Log("tbachieve.Get(level).score" + tbachieve.Get(level).score);
                if (wHelp < 0)
                {
                    wHelp = 0;
                }

                this.GetFromReference(KProgressBar).GetRectTransform().SetOffsetWithRight(-wHelp);
                //this.GetFromReference(KBtn_Achieve_Gift).GetXButton().SetEnabled(false);
                //RedPointMgr.instance.SetState(UIPanel_Battle.Battle_Red_Point_Root, KBtn_Achieve_Gift,
                //    RedPointState.Hide);
            }
            else
            {
                //Debug.Log("exp >= tbachieve.Get(level).score");
                this.GetFromReference(KProgressBar).GetRectTransform().SetOffsetWithRight(0);
                //this.GetFromReference(KBtn_Achieve_Gift).GetXButton().SetEnabled(true);
                //RedPointMgr.instance.SetState(UIPanel_Battle.Battle_Red_Point_Root, KBtn_Achieve_Gift,
                //    RedPointState.Show);
                //设置红点状态为show
            }

            NOWEXP = exp;
            await this.GetFromReference(KImg_Achieve).GetImage().SetSpriteAsync(tbachieve.Get(level).pic, true);
        }

        private void CreateTips()
        {
            //ResourcesSingleton
            if (StrongTipIsActive == false)
            {
                StrongTipIsActive = true;
                this.GetFromReference(KTip_Strong).SetActive(true);
            }
            else
            {
                StrongTipIsActive = false;
                this.GetFromReference(KTip_Strong).SetActive(false);
            }
        }

        private void CloseStrongTip()
        {
            StrongTipIsActive = false;
            //this.GetFromReference(KTip_Strong).SetActive(false);
            //this.GetFromReference(KPos_Gift_Tip).GetList().Clear();
        }

        private void GetGift(int level)
        {
            WebMessageHandler.Instance.AddHandler(3, 5, OnGetGiftResponse);
            IntValue intValue = new IntValue();
            intValue.Value = level - 1;
            NetWorkManager.Instance.SendMessage(3, 5, intValue);
        }

        private void OnGetGiftResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(3, 5, OnGetGiftResponse);
            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(stringValueList);
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

            //WebMessageHandler.Instance.AddHandler(3, 4, OnQueryAchievementResponse);
            NetWorkManager.Instance.SendMessage(CMD.QUERYACHIEVEMENT);
        }

        private async void OnQueryAchievementResponse(object sender, WebMessageHandler.Execute e)
        {
            //WebMessageHandler.Instance.RemoveHandler(3, 4, OnQueryAchievementResponse);
            RoleTaskInfo roleTaskInfo = new RoleTaskInfo();
            roleTaskInfo.MergeFrom(e.data);
            //Debug.Log(e);
            //this.roleTaskInfo = roleTaskInfo;
            ResourcesSingleton.Instance.achieveInfo = roleTaskInfo;
            Log.Debug($"roleTaskInfo:{roleTaskInfo.ToString()}", Color.green);
            //roleTaskInfo.
            if (e.data.IsEmpty)
            {
                Log.Debug("achieveInfo IsEmpty", Color.red);
                return;
            }

            if (!isInit)
            {
                isInit = true;
                await InitNode();
            }
            else
            {
                Update();
            }

            //JiYuUIHelper.EnableLoading(false);
            return;
            //Log.Debug($"TaskScoreList:{ResourcesSingleton.Instance.achieveInfo.TaskScoreList.ToString()}", Color.green);
            //Log.Debug($"AchieveList:{ResourcesSingleton.Instance.achieveInfo.AchieveList.ToString()}", Color.green);


            ResourcesSingleton.Instance.achieve.tasks.Clear();
            ResourcesSingleton.Instance.achieve.boxes.Clear();

            foreach (var t in roleTaskInfo.TaskInfoList)
            {
                //status为领取状态0未领取，1领取， para任务参数
                ResourcesSingleton.Instance.achieve.tasks.Add(t.Id, new Vector2(t.Status, t.Para));
            }

            // foreach (var s in roleTaskInfo.AchieveList)
            // {
            //     //status为领取状态0未领取，1领取， valid生效与否0未生效1生效
            //     ResourcesSingleton.Instance.achieve.boxes.Add(s.Id, new Vector2(s.Status, 0));
            // }

            UpdateGroup();
        }

        protected override void OnClose()
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERYACHIEVEMENT, OnQueryAchievementResponse);
            CloseStrongTip();
            //groupDic.Clear();
            //this.GetFromReference(KScrollView).GetScrollRect().Content.GetList().Clear();
            RedDotManager.Instance.ClearChildrenListeners(m_RedDotName);
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
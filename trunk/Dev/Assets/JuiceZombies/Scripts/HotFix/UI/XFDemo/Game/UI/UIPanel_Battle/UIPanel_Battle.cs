//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Battle)]
    internal sealed class UIPanel_BattleEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Battle;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Battle>();
        }
    }

    public partial class UIPanel_Battle : UI, IAwake
    {
        //private long timerId;
        //private bool isTimer;

        ////private bool isTimer;
        //int maxLevelTime = 9999;
        //UI levelPanel;

        //private Tbconstant tbconstant;
        //private int chapterID;
        //private UI KcruiserButtonUI;
        //public const string Battle_Red_Point_Root = "Battle_Red_Point_Root";
        //public int tagId = 4;

        public void Initialize()
        {
            ////isTalentRequest = false;
            //return;
            //isTimer = false;
           
            //InitFunctionButtonsAsync().Forget();
            //InitMainView();

            //GetFromReference(KchapterBox).GetRectTransform().DoEulerAngleZ(0f, 0f, 0.02f);

            //// Log.Debug($"{roleInfo}", Color.red);

            //UILevelScrollView.ChangeEvent += ChangeLevelSprite;


            //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KextendButton),
            //    () => { OnExtendButtonClick(); });

            //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_pass),
            //    () => { OnPassButtonClick(); });
            //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_specialLevel),
            //    () => { OnSpecialLevelButtonClick(); });


            //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KLevelImage),
            //    () =>
            //    {
            //        DisableTipPanel();
            //        onLevelImageClicked();
            //    });
            ////this.GetFromReference(KLevelImage).GetComponent<Button>()
            ////    ?.onClick.Add(async () => onLevelImageClicked());

            //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_chapterBox),
            //    () => { onTresureButtonClick(); });

            //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_cruiser),
            //    () => { onCruiserButtonClick(); });

            //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_task),
            //    () => { onTaskButtonClick(); });

            //#region bottom panel

            //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_dailyChallege),
            //    () => { OnDailyChallegeClick(); });

            //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_start),
            //    () => { OnStartButtonClick(); });

            //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_dailyActive),
            //    () => { OnDailyActiveButtonClick(); });

            //#endregion

            //UpdateLevelShow();

            //#region �ƽ����Ӻ���ʼ��

            //// RedPointInit();
            //// RedPointSetState();

            //#endregion

            //#region Ѹ��з��ӵ��߼�

            //RedPoint();

            //#endregion
        }

        //private void ChangeLevelSprite(int levelID)
        //{
        //    Log.Debug($"ChangeLevelSprite:{levelID}");
        //    UpdateLevelShow();
        //}

        //private void SetLevelImage(int levelID)
        //{
        //    int chapterID = 0;
        //    var chapterList = ConfigManager.Instance.Tables.Tbchapter.DataList;
        //    var chapterMap = ConfigManager.Instance.Tables.Tbchapter;
        //    for (int i = 0; i < chapterList.Count; i++)
        //    {
        //        if (chapterList[i].levelId == levelID)
        //        {
        //            chapterID = chapterList[i].id;
        //        }
        //    }

        //    this.GetFromReference(KLevelImage).GetImage().SetSprite(chapterMap.GetOrDefault(chapterID).icon, false);
        //}


        //public void SetChapterBoxInfo(RoleChapters roleInfo)
        //{
        //    //�õ���Сδ��ȡ��boxID
        //    int minNoGetBoxID = 2999;
        //    //�õ���Сδ������boxID
        //    int minNotLockBoxID = 2999;
        //    for (int i = 0; i < roleInfo.ChapterList.Count; i++)
        //    {
        //        for (int j = 0; j < roleInfo.ChapterList[i].BoxStatusList.Count; j++)
        //        {
        //            string boxInput = roleInfo.ChapterList[i].BoxStatusList[j];
        //            //  Log.Debug(boxInput,Color.blue);
        //            if (!boxInput.Contains("["))
        //            {
        //                continue;
        //            }

        //            int startIndex = boxInput.IndexOf("[") + 1;
        //            int endIndex = boxInput.IndexOf("]", startIndex);
        //            string result = boxInput.Substring(startIndex, endIndex - startIndex);
        //            //Log.Debug(
        //            //    $"roleInfo.ChapterList[{i}].BoxStatusList[{j}]:{roleInfo.ChapterList[i].BoxStatusList[j]}",
        //            //    Color.yellow);
        //            var boxArray = result.Split(":");

        //            if (boxArray.Length <= 0)
        //            {
        //                Log.Debug($"result:{result}");
        //                boxArray = result.Split(',');
        //            }

        //            if (boxArray[2] == "false" ? true : false)
        //            {
        //                if (minNoGetBoxID > int.Parse(boxArray[0]))
        //                {
        //                    minNoGetBoxID = int.Parse(boxArray[0]);
        //                }
        //            }

        //            if (boxArray[1] == "lock" ? true : false)
        //            {
        //                if (minNotLockBoxID > int.Parse(boxArray[0]))
        //                {
        //                    minNotLockBoxID = int.Parse(boxArray[0]);
        //                }
        //            }
        //        }
        //    }

        //    //ResourcesSingleton.Instance.levelInfo.levelBox.minNotGetBoxID = minNoGetBoxID;
        //    ResourcesSingleton.Instance.levelInfo.levelBox.minNotLockBoxID = minNotLockBoxID;

        //    Log.Debug($"minNoGetBoxID{minNoGetBoxID},minNotLockBoxID{minNotLockBoxID}", Color.yellow);
        //}


        //public void UpdateLevelShow()
        //{
        //    var chapterTable = ConfigManager.Instance.Tables.Tbchapter;

        //    Log.Debug("UpdateLevelShow", Color.cyan);
        //    var levelInfo = ResourcesSingleton.Instance.levelInfo;
        //    var maxLockChapterID = levelInfo.maxUnLockChapterID;
        //    var currentID = ResourcesSingleton.Instance.levelInfo.levelId;
        //    var popLevelID = JsonManager.Instance.userData.chapterId;
        //    Log.Debug($"PopLevelID:{popLevelID}", Color.yellow);
        //    if (currentID == popLevelID)
        //    {
        //        UIHelper.Create(UIType.UILockNewChapter, currentID);
        //    }


        //    //SetChapterBoxInfo(roleInfo);
        //    InitLevelDesciptionText(currentID, maxLockChapterID, levelInfo.maxUnLockChapterSurviveTime);
        //    InitLevelNameText(currentID);
        //    SetLevelImage(currentID);
        //    //Log.Debug(
        //    //    $"minNotLockBoxID{levelInfo.levelBox.minNotLockBoxID},minNotGetBoxID{levelInfo.levelBox.minNotGetBoxID}",
        //    //    Color.green);
        //    //SetBoxRedPot(levelInfo.levelBox.minNotLockBoxID, levelInfo.levelBox.minNotGetBoxID);
        //}


        //private void SetBoxRedPot(int minNotLockBoxID, int minNoGetBoxID)
        //{
        //    int count = GetBoxCount(minNotLockBoxID, minNoGetBoxID);
        //    Log.Debug($"maxLockBoxID{minNotLockBoxID}minNoGetBoxID{minNoGetBoxID}count{count}");
        //    if (count > 0)
        //    {
        //        GetFromReference(KchapterBoxRedPoint).SetActive(true);
        //        GetFromReference(KTxt_RedPoint).GetTextMeshPro().SetTMPText(count.ToString());
        //        //GetFromReference(KRedPointText).GetComponent<TMP_Text>().SetText();
        //        if (!isTimer)
        //        {
        //            StartTimer();
        //        }
        //    }
        //    else
        //    {
        //        GetFromReference(KchapterBoxRedPoint).SetActive(false);
        //        //GetFromReference(KchapterBox).GetRectTransform().DoEulerAngleZ(0f, 0f, 2f);
        //        if (isTimer)
        //        {
        //            RemoveTimer();
        //        }
        //    }
        //}

        //public int GetBoxCount(int minNotLockBoxID, int minNoGetBoxID)
        //{
        //    int count = 0;
        //    int tempIndex = 0;
        //    var boxTable = ConfigManager.Instance.Tables.Tbchapter_box.DataList;
        //    for (int i = 0; i < boxTable.Count; i++)
        //    {
        //        if (boxTable[i].id == minNoGetBoxID)
        //        {
        //            tempIndex = i;
        //        }

        //        if (boxTable[i].id == minNotLockBoxID)
        //        {
        //            count = i - tempIndex;
        //            break;
        //        }
        //    }

        //    return count;
        //}

        //private void StartTimer()
        //{
        //    isTimer = true;
        //    //����һ��ÿִ֡�е������൱��Update
        //    var timerMgr = TimerManager.Instance;
        //    timerId = timerMgr.StartRepeatedTimer(2000, this.BoxShake);
        //}

        //private void BoxShake()
        //{
        //    // var newLoc1 = GetFromReference(KchapterBox).GetComponent<LocalTransform>().RotateZ(math.radians(10));
        //    // var newLoc2= GetFromReference(KchapterBox).GetComponent<LocalTransform>().RotateZ(math.radians(-10));
        //    GetFromReference(KchapterBox).GetRectTransform().DoEulerAngleZ(0f, -10f, 0.5f).AddOnCompleted(() =>
        //        GetFromReference(KchapterBox).GetRectTransform().DoEulerAngleZ(-10f, 10f, 1f)
        //            .AddOnCompleted(() =>
        //                GetFromReference(KchapterBox).GetRectTransform().DoEulerAngleZ(10f, 0f, 0.5f)));
        //}

        //public void RemoveTimer()
        //{
        //    //GetFromReference(KchapterBox).GetRectTransform().DoEulerAngleZ(0f, 0f, 0.02f);

        //    var timerMgr = TimerManager.Instance;
        //    timerMgr?.RemoveTimerId(ref this.timerId);
        //    this.timerId = 0;
        //    Log.Error($"RemoveTimer()");
        //    isTimer = false;
        //}


        //#region �ƽ����������ճ��ܳ�

        //private async void onTaskButtonClick()
        //{
        //    Debug.Log($"onTaskButtonClick");
        //    //��������
        //    WebMessageHandler.Instance.AddHandler(3, 1, OnRoleTaskInfoResponse);
        //    NetWorkManager.Instance.SendMessage(3, 1);
        //    //await UIHelper.CreateAsync(UIType.UIPanel_Task_DailyAndWeekly);
        //}

        //private async void OnRoleTaskInfoResponse(object sender, WebMessageHandler.Execute e)
        //{
        //    WebMessageHandler.Instance.RemoveHandler(3, 1, OnRoleTaskInfoResponse);
        //    RoleTaskInfo roleTaskInfo = new RoleTaskInfo();
        //    roleTaskInfo.MergeFrom(e.data);
        //    Debug.Log(e);
        //    Debug.Log(roleTaskInfo);
        //    if (e.data.IsEmpty)
        //    {
        //        Log.Debug("3-1RoleTaskInfo.data.IsEmpty", Color.red);
        //        return;
        //    }

        //    ResourcesSingleton.Instance.dayWeekTask.tasks.Clear();
        //    ResourcesSingleton.Instance.dayWeekTask.boxes.Clear();

        //    foreach (var t in roleTaskInfo.TaskInfoList)
        //    {
        //        //statusΪ��ȡ״̬0δ��ȡ��1��ȡ�� para�������
        //        ResourcesSingleton.Instance.dayWeekTask.tasks.Add(t.Id, new Vector2(t.Status, t.Para));
        //    }

        //    foreach (var s in roleTaskInfo.TaskScoreList)
        //    {
        //        //statusΪ��ȡ״̬0δ��ȡ��1��ȡ�� valid��Ч���0δ��Ч1��Ч
        //        ResourcesSingleton.Instance.dayWeekTask.boxes.Add(s.Id, new Vector2(s.Status, s.Valid));
        //    }

        //    await UIHelper.CreateAsync<bool>(UIType.UIPanel_Task_DailyAndWeekly, false);
        //}

        //private void RedPointInit()
        //{
        //    //��Ӹ����
        //    RedPointMgr.instance.Add(Battle_Red_Point_Root, null, null, RedPointType.Enternal);
        //    //��Ӹ����ı���
        //    RedPointMgr.instance.Init(Battle_Red_Point_Root, Battle_Red_Point_Root,
        //        (RedPointState state, int data) => { });
        //    //����������ĺ��
        //    RedPointMgr.instance.Add(Battle_Red_Point_Root, "Task_Center", Battle_Red_Point_Root,
        //        RedPointType.Enternal);
        //    //����������ĺ�����
        //    RedPointMgr.instance.Init(Battle_Red_Point_Root, "Task_Center",
        //        (RedPointState state, int data) =>
        //        {
        //            this.GetFromReference(Ktask_reddot).SetActive(state == RedPointState.Show);
        //        });
        //    //������������ճ����
        //    RedPointMgr.instance.Add(Battle_Red_Point_Root, "Task_Center_Daily", "Task_Center", RedPointType.Enternal);
        //    //������������ܳ����
        //    RedPointMgr.instance.Add(Battle_Red_Point_Root, "Task_Center_Weekly", "Task_Center", RedPointType.Enternal);
        //    //���ÿ������ĺ��
        //    foreach (var t in ConfigManager.Instance.Tables.Tbtask.DataList)
        //    {
        //        if (t.group == 100)
        //        {
        //            //���ÿ���ճ�����ĺ��
        //            RedPointMgr.instance.Add(Battle_Red_Point_Root, "task" + t.id.ToString(), "Task_Center_Daily",
        //                RedPointType.Once);
        //        }

        //        if (t.group == 200)
        //        {
        //            //���ÿ���ܳ�����ĺ��
        //            RedPointMgr.instance.Add(Battle_Red_Point_Root, "task" + t.id.ToString(), "Task_Center_Daily",
        //                RedPointType.Once);
        //        }
        //    }

        //    RedPointMgr.instance.Add(Battle_Red_Point_Root, "achieve", Battle_Red_Point_Root, RedPointType.Enternal);
        //    RedPointMgr.instance.Add(Battle_Red_Point_Root, UIPanel_Achieve.KBtn_Achieve_Gift, "achieve",
        //        RedPointType.Once);
        //    foreach (var ag in ConfigManager.Instance.Tables.Tbachieve_group.DataList)
        //    {
        //        RedPointMgr.instance.Add(Battle_Red_Point_Root, "achieve_group" + ag.id.ToString(), "achieve",
        //            RedPointType.Enternal);
        //        foreach (int taskGroupID in ag.groupId)
        //        {
        //            RedPointMgr.instance.Add(Battle_Red_Point_Root, "task_group" + taskGroupID.ToString(),
        //                "achieve_group" + ag.id.ToString(), RedPointType.Once);
        //        }
        //    }

        //    //���ǩ�����
        //    RedPointMgr.instance.Add(Battle_Red_Point_Root, "sign", Battle_Red_Point_Root, RedPointType.Enternal);
        //    RedPointMgr.instance.Add(Battle_Red_Point_Root, "sign_daily", "sign", RedPointType.Once);
        //    RedPointMgr.instance.Add(Battle_Red_Point_Root, "sign_acc", "sign", RedPointType.Once);

        //    //init notice red points
        //    // RedPointMgr.instance.Remove(Battle_Red_Point_Root, "notice");
        //    // RedPointMgr.instance.Add(Battle_Red_Point_Root, "notice", Battle_Red_Point_Root, RedPointType.Enternal);
        //    // if (JsonManager.Instance.sharedData.noticesList == null)
        //    // {
        //    //     
        //    // }
        //    // else
        //    // {
        //    //     if (JsonManager.Instance.sharedData.noticesList.notices == null)
        //    //     {
        //    //     }
        //    //     else
        //    //     {
        //    //         if (JsonManager.Instance.sharedData.noticesList.notices.Count == 0)
        //    //         {
        //    //         }
        //    //         else
        //    //         {
        //    //             foreach (var nt in JsonManager.Instance.sharedData.noticesList.notices)
        //    //             {
        //    //                 RedPointMgr.instance.Add(Battle_Red_Point_Root, "notice" + nt.id.ToString(), "notice",
        //    //                     RedPointType.None);
        //    //             }
        //    //         }
        //    //     }
        //    // }

        //    // RedPointMgr.instance.Remove(Battle_Red_Point_Root, "notice");
        //    // RedPointMgr.instance.Add(Battle_Red_Point_Root, "notice", Battle_Red_Point_Root, RedPointType.Enternal);
        //    // if (JsonManager.Instance.sharedData.noticesList == null)
        //    // {
        //    //     
        //    // }
        //    // else
        //    // {
        //    //     if (JsonManager.Instance.sharedData.noticesList.notices == null)
        //    //     {
        //    //         
        //    //     }
        //    //     else
        //    //     {
        //    //         if (JsonManager.Instance.sharedData.noticesList.notices.Count == 0)
        //    //         {
        //    //             
        //    //         }
        //    //         else
        //    //         {
        //    //             foreach (var nt in JsonManager.Instance.sharedData.noticesList.notices)
        //    //             {
        //    //                 RedPointMgr.instance.Add(Battle_Red_Point_Root, "notice" + nt.id.ToString(), "notice", RedPointType.None);
        //    //             }
        //    //         }
        //    //     }
        //    // }
        //}

        //public void RedPointSetState()
        //{
        //    #region set red point state

        //    foreach (var t in ResourcesSingleton.Instance.dayWeekTask.tasks)
        //    {
        //        if (t.Value.x == 0)
        //        {
        //            //x = 0 Unclaimed
        //            if (t.Value.y >= ConfigManager.Instance.Tables.Tbtask[t.Key].para[0])
        //            {
        //                //set day or week task redpoint show
        //                RedPointMgr.instance.SetState(Battle_Red_Point_Root, "task" + t.Key.ToString(),
        //                    RedPointState.Show);
        //            }
        //            else
        //            {
        //                //set day or week task redpoint hide
        //                RedPointMgr.instance.SetState(Battle_Red_Point_Root, "task" + t.Key.ToString(),
        //                    RedPointState.Hide);
        //            }
        //        }
        //        else
        //        {
        //            //x = 1 Received
        //            RedPointMgr.instance.SetState(Battle_Red_Point_Root, "task" + t.Key.ToString(), RedPointState.Hide);
        //        }
        //    }

        //    //����
        //    Dictionary<int, List<int>> groupIDs = new Dictionary<int, List<int>>();
        //    foreach (var t in ConfigManager.Instance.Tables.Tbtask.DataList)
        //    {
        //        if (groupIDs.TryGetValue(t.group, out List<int> intList))
        //        {
        //            //intList.Add(t.id);
        //            groupIDs[t.group].Add(t.id);
        //        }
        //        else
        //        {
        //            groupIDs[t.group] = new List<int> { t.id };
        //        }
        //    }

        //    Dictionary<int, List<int>> helpDic = new Dictionary<int, List<int>>();
        //    foreach (var gId in groupIDs)
        //    {
        //        List<int> ints = new List<int>();
        //        int k = 0;
        //        foreach (var id in gId.Value)
        //        {
        //            if (ConfigManager.Instance.Tables.Tbtask.Get(id).pre == 0)
        //            {
        //                k++;
        //            }
        //        }

        //        if (k == 1)
        //        {
        //            Dictionary<int, int> reDependent = new Dictionary<int, int>();
        //            foreach (var id in gId.Value)
        //            {
        //                reDependent[ConfigManager.Instance.Tables.Tbtask.Get(id).pre] = id;
        //            }

        //            for (int l = 0; l < gId.Value.Count; l++)
        //            {
        //                if (l == 0)
        //                {
        //                    ints.Add(reDependent[l]);
        //                }
        //                else
        //                {
        //                    ints.Add(reDependent[ints[l - 1]]);
        //                }
        //            }

        //            helpDic.Add(gId.Key, ints);
        //        }
        //    }

        //    ResourcesSingleton.Instance.groupIDs = helpDic;

        //    //ÿ���ɾ͵ĺ��
        //    Dictionary<int, List<Vector3>> SortDic = new Dictionary<int, List<Vector3>>();
        //    SortDic.Clear();
        //    bool isLast = false;
        //    bool isCompleted = false;
        //    Debug.Log("ResourcesSingleton.Instance.achieve.tasks.Count" +
        //              ResourcesSingleton.Instance.achieve.tasks.Count.ToString());
        //    if (ResourcesSingleton.Instance.achieve.tasks.Count == 0)
        //    {
        //        //û�ӵ����ݣ��Լ����ɿ�����
        //        ResourcesSingleton.Instance.achieve.tasks.Clear();
        //        ResourcesSingleton.Instance.achieve.boxes.Clear();
        //        foreach (var t in ConfigManager.Instance.Tables.Tbtask.DataList)
        //        {
        //            ResourcesSingleton.Instance.achieve.tasks.Add(t.id, new Vector2(0, 0));
        //        }

        //        foreach (var ts in ConfigManager.Instance.Tables.Tbtask_score.DataList)
        //        {
        //            ResourcesSingleton.Instance.achieve.boxes.Add(ts.id, new Vector2(0, 0));
        //        }

        //        Debug.Log("û�ӵ����ݣ��Լ����ɿ�����");
        //    }

        //    foreach (var achieveGroup in ConfigManager.Instance.Tables.Tbachieve_group.DataList)
        //    {
        //        //��һ���ҵ�ÿ��ҳǩ�����������
        //        List<Vector3> idList = new List<Vector3>();

        //        foreach (int taskGroupID in achieveGroup.groupId)
        //        {
        //            //�ڶ����ҵ�ÿ��������
        //            //��ѯ����ǰ������Ӧ����ʾ��task��id��������Ӧ�����ж�״̬
        //            int helpID = 0;
        //            for (int i = 0; i < ResourcesSingleton.Instance.groupIDs[taskGroupID].Count; i++)
        //            {
        //                //����������ÿ���������ڲ���������б������ö�Ӧ״̬
        //                //��ǰ����id
        //                int thisTaskID = ResourcesSingleton.Instance.groupIDs[taskGroupID][i];
        //                if (ResourcesSingleton.Instance.achieve.tasks[thisTaskID][0] == 1)
        //                {
        //                    helpID = thisTaskID;
        //                    if (i == ResourcesSingleton.Instance.groupIDs[taskGroupID].Count - 1)
        //                    {
        //                        //��ǰ������ȫ����ɣ���Ӧ״̬2
        //                        isLast = true;
        //                    }
        //                }
        //                else
        //                {
        //                    helpID = thisTaskID;
        //                    if (ResourcesSingleton.Instance.achieve.tasks[thisTaskID][1] >=
        //                        ConfigManager.Instance.Tables.Tbtask.Get(helpID).para[0])
        //                    {
        //                        //��ǰ��������ʾ�����Ѿ���ɣ�����δ��ȡ����Ӧ״̬0
        //                        isCompleted = true;
        //                    }

        //                    break;
        //                }
        //            }

        //            //����״̬����list
        //            if (isLast)
        //            {
        //                idList.Add(new Vector3(taskGroupID, 2, helpID));
        //            }
        //            else
        //            {
        //                if (isCompleted)
        //                {
        //                    idList.Add(new Vector3(taskGroupID, 0, helpID));
        //                }
        //                else
        //                {
        //                    idList.Add(new Vector3(taskGroupID, 1, helpID));
        //                }
        //            }

        //            isLast = false;
        //            isCompleted = false;
        //        }

        //        //����״̬�������򣬲������ֵ�
        //        idList.Sort(delegate(Vector3 v1, Vector3 v2)
        //        {
        //            if (v1.y == v2.y)
        //            {
        //                return v1.x.CompareTo(v2.x);
        //            }
        //            else
        //            {
        //                return v1.y.CompareTo(v2.y);
        //            }
        //        });
        //        SortDic.Add(achieveGroup.id, idList);
        //    }

        //    //����dic��ʾ�����������״̬
        //    foreach (var Sortpair in SortDic)
        //    {
        //        foreach (var v3 in Sortpair.Value)
        //        {
        //            if (v3.y == 0)
        //            {
        //                RedPointMgr.instance.SetState(Battle_Red_Point_Root, "task_group" + ((int)v3.x).ToString(),
        //                    RedPointState.Show);
        //            }
        //            else
        //            {
        //                RedPointMgr.instance.SetState(Battle_Red_Point_Root, "task_group" + ((int)v3.x).ToString(),
        //                    RedPointState.Hide);
        //            }
        //        }
        //    }

        //    int achieveExp = 0;
        //    foreach (var acgroup in ConfigManager.Instance.Tables.Tbachieve_group.DataList)
        //    {
        //        foreach (int id in acgroup.groupId)
        //        {
        //            for (int i = 0; i < groupIDs[id].Count; i++)
        //            {
        //                int taskID = groupIDs[id][i];
        //                //�������ȡ����ô��þ���
        //                if (ResourcesSingleton.Instance.achieve.tasks[taskID][0] == 1)
        //                {
        //                    achieveExp += ConfigManager.Instance.Tables.Tbtask.Get(taskID).score;
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    int achieveLevel = 1;
        //    for (int i = 0; i < ConfigManager.Instance.Tables.Tbachieve.DataList.Count; i++)
        //    {
        //        if (i == 0)
        //        {
        //            achieveLevel = ConfigManager.Instance.Tables.Tbachieve.DataList[i].id;
        //        }
        //        else
        //        {
        //            if (achieveExp >= ConfigManager.Instance.Tables.Tbachieve.DataList[i - 1].score &&
        //                ResourcesSingleton.Instance.achieve.boxes[
        //                    ConfigManager.Instance.Tables.Tbachieve.DataList[i - 1].id][0] == 1)
        //            {
        //                achieveLevel = ConfigManager.Instance.Tables.Tbachieve.DataList[i].id;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }

        //    if (achieveExp < ConfigManager.Instance.Tables.Tbachieve.Get(achieveLevel).score)
        //    {
        //        RedPointMgr.instance.SetState(Battle_Red_Point_Root, UIPanel_Achieve.KBtn_Achieve_Gift,
        //            RedPointState.Hide);
        //    }
        //    else
        //    {
        //        RedPointMgr.instance.SetState(Battle_Red_Point_Root, UIPanel_Achieve.KBtn_Achieve_Gift,
        //            RedPointState.Show);
        //    }

        //    //����ǩ�����
        //    if (ResourcesSingleton.Instance.signData.OnDayStatus == 0)
        //    {
        //        RedPointMgr.instance.SetState(Battle_Red_Point_Root, "sign_daily", RedPointState.Show);
        //    }
        //    else
        //    {
        //        RedPointMgr.instance.SetState(Battle_Red_Point_Root, "sign_daily", RedPointState.Hide);
        //    }

        //    if (accCanBeGetOrNot())
        //    {
        //        RedPointMgr.instance.SetState(Battle_Red_Point_Root, "sign_acc", RedPointState.Show);
        //    }
        //    else
        //    {
        //        RedPointMgr.instance.SetState(Battle_Red_Point_Root, "sign_acc", RedPointState.Hide);
        //    }

        //    //set notice red point
        //    // if (JsonManager.Instance.sharedData.noticesList == null)
        //    // {
        //    //     
        //    // }
        //    // else
        //    // {
        //    //     if (JsonManager.Instance.sharedData.noticesList.notices == null)
        //    //     {
        //    //         
        //    //     }
        //    //     else
        //    //     {
        //    //         if (JsonManager.Instance.sharedData.noticesList.notices.Count == 0)
        //    //         {
        //    //             
        //    //         }
        //    //         else
        //    //         {
        //    //             foreach (var nt in JsonManager.Instance.sharedData.noticesList.notices)
        //    //             {
        //    //                 if (nt.readStatus == 0)
        //    //                 {
        //    //                     //unread
        //    //                     RedPointMgr.instance.SetState(Battle_Red_Point_Root, "notice" + nt.id.ToString(), RedPointState.Show);
        //    //                 }
        //    //                 else
        //    //                 {
        //    //                     //read
        //    //                     RedPointMgr.instance.SetState(Battle_Red_Point_Root, "notice" + nt.id.ToString(), RedPointState.Hide);
        //    //                 }
        //    //             }
        //    //         }
        //    //     }
        //    // }

        //    Debug.Log("red point setting completed from panel battle");

        //    #endregion
        //}

        //#endregion

        ///// <summary>
        ///// Determine whether acc sign can be claimed
        ///// </summary>
        ///// <returns></returns>
        //private bool accCanBeGetOrNot()
        //{
        //    bool accRe = false;
        //    if (ResourcesSingleton.Instance.signData.OnDayStatus == 1)
        //    {
        //        if (ResourcesSingleton.Instance.signData.BoxOnDayStatus == 0)
        //        {
        //            int CheckInDayHelp = (int)(ResourcesSingleton.Instance.signData.CheckInDay
        //                                       % ConfigManager.Instance.Tables.Tbsign_acc
        //                                           .DataList[
        //                                               ConfigManager.Instance.Tables.Tbsign_acc.DataList.Count - 1]
        //                                           .day);
        //            for (int i = 0; i < ConfigManager.Instance.Tables.Tbsign_acc.DataList.Count; i++)
        //            {
        //                if (ConfigManager.Instance.Tables.Tbsign_acc.DataList[i].day == CheckInDayHelp)
        //                {
        //                    accRe = true;
        //                    break;
        //                }
        //                else
        //                {
        //                    continue;
        //                }
        //            }

        //            return accRe;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //private void InitMainView()
        //{
        //    GetFromReference(KRoot_ExtendSetting)?.SetActive(false);
        //    InitExtendSetting();

        //    var funcMap = ConfigManager.Instance.Tables.Tbtag_func.DataMap;
        //    var language = ConfigManager.Instance.Tables.Tblanguage;

        //    string spritPath = "";
        //    spritPath = funcMap[3301].icon;
        //    GetFromReference(KBtn_specialLevel)?.GetRectTransform().GetChild(0).GetComponent<XImage>()
        //        .SetSprite(ResourcesManager.LoadAsset<Sprite>(spritPath), false);
        //    GetFromReference(KBtn_specialLevel)?.GetRectTransform().GetChild(1).GetComponent<TMP_Text>()
        //        .SetText(language.Get(funcMap[3301].name).current);

        //    spritPath = funcMap[3201].icon;
        //    GetFromReference(KBtn_pass)?.GetRectTransform().GetChild(0).GetComponent<XImage>()
        //        .SetSprite(ResourcesManager.LoadAsset<Sprite>(spritPath), false);
        //    GetFromReference(KBtn_pass)?.GetRectTransform().GetChild(1).GetComponent<TMP_Text>()
        //        .SetText(language.Get(funcMap[3201].name).current);

        //    this.GetFromReference(KextendButton).GetImage().SetSprite("icon_extend button", false);

        //    spritPath = funcMap[3501].icon;
        //    this.GetFromReference(KBtn_chapterBox)?.GetImage().SetSprite(spritPath, false);
        //    GetFromReference(KBtn_chapterBox)?.GetRectTransform().GetChild(0).GetComponent<TMP_Text>()
        //        .SetText(language.Get(funcMap[3501].name).current);

        //    spritPath = funcMap[3502].icon;
        //    this.GetFromReference(KBtn_cruiser)?.GetImage().SetSprite(spritPath, false);
        //    GetFromReference(KBtn_cruiser)?.GetRectTransform().GetChild(0).GetComponent<TMP_Text>()
        //        .SetText(language.Get(funcMap[3502].name).current);
        //    GetFromReference(Kcruiser_reddot)?.SetActive(false);

        //    spritPath = funcMap[3701].icon;
        //    GetFromReference(KBtn_dailyChallege)?.GetRectTransform().GetChild(0).GetComponent<XImage>()
        //        .SetSprite(ResourcesManager.LoadAsset<Sprite>(spritPath), false);
        //    GetFromReference(KBtn_dailyChallege)?.GetRectTransform().GetChild(1).GetComponent<TMP_Text>()
        //        .SetText(language.Get(funcMap[3701].name).current);

        //    spritPath = funcMap[3801].icon;
        //    GetFromReference(KBtn_start)?.GetImage().SetSprite(spritPath, false);
        //    GetFromReference(KBtn_start)?.GetRectTransform().GetChild(0).GetComponent<TMP_Text>()
        //        .SetText(language.Get(funcMap[3801].name).current);

        //    spritPath = funcMap[3901].icon;
        //    GetFromReference(KBtn_dailyActive)?.GetRectTransform().GetChild(0).GetComponent<XImage>()
        //        .SetSprite(ResourcesManager.LoadAsset<Sprite>(spritPath), false);
        //    GetFromReference(KBtn_dailyActive)?.GetRectTransform().GetChild(1).GetComponent<TMP_Text>()
        //        .SetText(language.Get(funcMap[3901].name).current);
        //}

        //private async UniTaskVoid InitFunctionButtonsAsync()
        //{
        //    var funcTable = ConfigManager.Instance.Tables.Tbtag_func.DataList;
        //    var funcMap = ConfigManager.Instance.Tables.Tbtag_func.DataMap;
        //    var leftPanel = this.GetFromReference(KLeftPanel);
        //    var rightPanel = this.GetFromReference(KRightPanel);


        //    var leftList = leftPanel.GetList();
        //    leftList.Clear();
        //    var rightList = rightPanel.GetList();
        //    rightList.Clear();
        //    for (int i = 0; i < funcTable.Count; i++)
        //    {
        //        if (funcTable[i].tagId == 3)
        //        {
        //            if (funcTable[i].posType == 4)
        //            {
        //                //Log.Error("InitFunctionButtonsAsync");
        //                var ui =
        //                    await leftList.CreateWithUITypeAsync<int>(UIType.UICommonFunButton, funcTable[i].id, false)
        //                        as UICommonFunButton;
        //                //var button = UIHelper.Create<int>(UIType.UICommonFunButton, funcTable[i].id,
        //                //    leftPanel.GetComponent<Transform>()) as UICommonFunButton;
        //                //leftButtons.Add(funcTable[i].sort, button);
        //            }

        //            if (funcTable[i].posType == 6)
        //            {
        //                var ui = await rightList.CreateWithUITypeAsync<int>(UIType.UICommonFunButton, funcTable[i].id,
        //                    false) as UICommonFunButton;

        //                //rightButtons.Add(funcTable[i].sort, button);

        //                #region ��������

        //                if (funcTable[i].id == 3606)
        //                {
        //                    var pos = JiYuUIHelper.GetUIPos(ui);
        //                    var global = Common.Instance.Get<Global>();
        //                    global.GameObjects.BagPos = pos;
        //                }

        //                #endregion

        //                #region �ƽ������

        //                if (funcTable[i].id == 3601)
        //                {
        //                }

        //                if (funcTable[i].id == 3604)
        //                {
        //                }

        //                #endregion
        //            }
        //        }
        //    }

        //    JiYuUIHelper.ForceRefreshLayout(leftPanel);
        //    JiYuUIHelper.ForceRefreshLayout(rightPanel);
        //}

        ///// <summary>
        ///// 天赋的红点
        ///// </summary>
        ///// <returns></returns>
        //public bool isDisplayRedDot()
        //{
        //    var talentMap = ConfigManager.Instance.Tables.Tbtalent.DataMap;
        //    var talentList = ConfigManager.Instance.Tables.Tbtalent.DataList;
        //    int maxTalentProp = 0;
        //    int maxTalentSkill = 0;
        //    for (int i = 0; i < talentList.Count; i++)
        //    {
        //        if (talentList[i].type == 1 && maxTalentProp < talentList[i].id)
        //        {
        //            maxTalentProp = talentList[i].id;
        //        }
        //        else if (talentList[i].type == 2 && maxTalentSkill < talentList[i].id)
        //        {
        //            maxTalentSkill = talentList[i].id;
        //        }
        //    }

        //    //Log.Debug($"lockTalentProp:{ResourcesSingleton.Instance.talentID.talentPropID}");


        //    int nextPropID = 0, nexSkillID = 0;

        //    for (int i = 0; i < talentList.Count; i++)
        //    {
        //        if (talentList[i].preId == ResourcesSingleton.Instance.talentID.talentPropID)
        //        {
        //            nextPropID = talentList[i].id;
        //        }

        //        if (talentList[i].preId == ResourcesSingleton.Instance.talentID.talentSkillID)
        //        {
        //            nexSkillID = talentList[i].id;
        //        }
        //    }

        //    int initSkillID = 0, initPropID = 0;
        //    for (int i = 0; i < talentList.Count; i++)
        //    {
        //        if (talentList[i].type == 2 && talentList[i].preId == 0)
        //        {
        //            initSkillID = talentList[i].id;
        //            break;
        //        }

        //        if (talentList[i].type == 1 && talentList[i].preId == 0)
        //        {
        //            initPropID = talentList[i].id;
        //            break;
        //        }
        //    }

        //    if (nextPropID == 0)
        //    {
        //        nextPropID = initPropID;
        //    }

        //    if (nexSkillID == 0)
        //    {
        //        nexSkillID = initSkillID;
        //    }

        //    //Log.Debug($"nextPropID:{nextPropID},level:{ResourcesSingleton.Instance.level},currentLevel{talentMap[nextPropID].level}", Color.cyan);
        //    if (nextPropID <= maxTalentProp)
        //    {
        //        if (ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill >= talentMap[nextPropID].cost[0].z &&
        //            ResourcesSingleton.Instance.UserInfo.RoleAssets.Level >= talentMap[nextPropID].level)
        //        {
        //            return true;
        //        }
        //    }

        //    if (nexSkillID <= maxTalentSkill)
        //    {
        //        if (ResourcesSingleton.Instance.items.ContainsKey((int)talentMap[nexSkillID].cost[0].y))
        //        {
        //            if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Level >= talentMap[nexSkillID].level &&
        //                ResourcesSingleton.Instance.items[(int)talentMap[nexSkillID].cost[0].y] >=
        //                talentMap[nexSkillID].cost[0].z)
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //public void DisableTipPanel()
        //{
        //    levelPanel?.Dispose();
        //}

        //#region ����߼�:Ѹ��з

        //public void RedPoint()
        //{
        //    InitJsonFile();
        //    CancelRedPonit();
        //    onWebResponse();
        //}

        ///// <summary>
        ///// ����߼�,Ѹ��з
        ///// </summary>
        //public void onWebResponse()
        //{
        //    WebMessageHandler.Instance.AddHandler(6, 3, WebMessagePatrol);

        //    long timeValue = TimeHelper.ClientNowSeconds();
        //    var partorlValue = new LongValue();
        //    partorlValue.Value = timeValue;

        //    NetWorkManager.Instance.SendMessage(6, 3, partorlValue);
        //}

        //private void WebMessagePatrol(object sender, WebMessageHandler.Execute e)
        //{
        //    var Proclass = new PatrolRes();
        //    Proclass.MergeFrom(e.data);

        //    if (e.data.IsEmpty)
        //    {
        //        WebMessageHandler.Instance.RemoveHandler(6, 3, WebMessagePatrol);
        //        Log.Debug("e.data.IsEmpty", Color.blue);
        //        return;
        //    }

        //    var backTimeLine = Proclass.PatrolGain.PatrolTime;
        //    //��ȡ��ǰʱ���
        //    long nowTime = TimeHelper.ClientNowSeconds();
        //    var timeLine = nowTime - backTimeLine;
        //    var AdCount = Proclass.PatrolConfig.PatrolOnceAdTimes;
        //    if (timeLine >= tbconstant.Get("patrol_time_upperlimit").constantValue
        //        || AdCount >= tbconstant.Get("patrol_once_ad_times").constantValue)
        //    {
        //        //��ʾ���
        //        ShowPatrolRedPoint();
        //    }
        //    else if (timeLine < tbconstant.Get("patrol_time_upperlimit").constantValue)
        //    {
        //        if (timerActionId != -1) TimerManager.Instance.RemoveTimerId(ref timerActionId);
        //        //����һ���ӳ�����
        //        //timerActionId = TimerManager.Instance.WaitTill(
        //        //    (backTimeLine + tbconstant.Get("patrol_time_upperlimit").constantValue) * 1000, ShowPatrolRedPoint);
        //        timerActionId = TimerManager.Instance.WaitTill(
        //            (tbconstant.Get("patrol_time_upperlimit").constantValue - backTimeLine) * 1000, ShowPatrolRedPoint);
        //    }
        //    else if (AdCount < tbconstant.Get("patrol_once_ad_times").constantValue) //����һ����ʱ����,Ϊ��ˢ�¹�����
        //    {
        //        long WillTime = TimeHelper.GetToTomorrowTime();
        //        //����һ���ӳ����� 
        //        TimerManager.Instance.WaitTill(WillTime * 1000, ShowPatrolRedPoint);
        //    }

        //    WebMessageHandler.Instance.RemoveHandler(6, 3, WebMessagePatrol);
        //}


        //private void InitJsonFile()
        //{
        //    tbconstant = ConfigManager.Instance.Tables.Tbconstant;
        //}

        //public void ShowPatrolRedPoint()
        //{
        //    GetFromReference(KchapterBoxRedPoint)?.SetActive(true);
        //}

        //public void CancelRedPonit()
        //{
        //    GetFromReference(KchapterBoxRedPoint)?.SetActive(false);
        //}

        //#endregion

        //private async void onCruiserButtonClick()
        //{
        //    Debug.Log($"cruiserButtonClick");
        //    chapterID = ResourcesSingleton.Instance.levelInfo.maxPassChapterID;
        //    if (chapterID <= 0)
        //    {
        //        string value = string.Format("未解锁");
        //        UIHelper.Create(UIType.UIPanel_Tips, value, UILayer.Mid);
        //        return;
        //    }

        //    await UIHelper.CreateAsync(UIType.UIPanel_Patrol, UILayer.Mid);
        //}

        //void DoScaleTween(UI ui, float aniTime)
        //{
        //    ui.GetRectTransform().DoScale(new Vector3(1.3f, 1.3f, 1), aniTime).AddOnCompleted(() =>
        //    {
        //        ui.GetRectTransform().DoScale(new Vector3(1, 1, 1), aniTime);
        //    });
        //}


        //private void InitLevelNameText(int levelID)
        //{
        //    var language = ConfigManager.Instance.Tables.Tblanguage;
        //    var chapterTable = ConfigManager.Instance.Tables.Tbchapter;
        //    var chapterList = ConfigManager.Instance.Tables.Tbchapter.DataList;
        //    var levelNameText = this.GetTextMeshPro(KTxt_levelName);
        //    Log.Debug($"levelID--------------------------:{levelID}", Color.cyan);
        //    var chapterID = 0;
        //    for (int i = 0; i < chapterList.Count; i++)
        //    {
        //        if (levelID == chapterList[i].levelId)
        //        {
        //            chapterID = chapterList[i].id;
        //            break;
        //        }
        //    }

        //    int levelNum = chapterTable.GetOrDefault(chapterID).num;
        //    string levelName = chapterTable.GetOrDefault(chapterID).name;


        //    levelNameText.SetTMPText(levelNum.ToString() + "." + language.Get(levelName).current);
        //}

        //private void InitLevelDesciptionText(int currentID, int maxLockID, float overTime)
        //{
        //    var chapter = ConfigManager.Instance.Tables.Tbchapter;
        //    maxLockID = chapter.GetOrDefault(maxLockID).levelId;
        //    var language = ConfigManager.Instance.Tables.Tblanguage;

        //    var levelDescriptionText = this.GetFromReference(KTxt_LevelDescription).GetTextMeshPro();

        //    string text = "";
        //    if (currentID < maxLockID)
        //    {
        //        text = language["chapter_passed_text"].current;
        //    }
        //    else
        //    {
        //        if (overTime == 0)
        //        {
        //            text = language["chapter_survival_maxtime_text"].current + "0" + language["time_second_1"].current;
        //            levelDescriptionText.SetTMPText(text);
        //            return;
        //        }

        //        text = language["chapter_survival_maxtime_text"].current + GetFormatTime(overTime);
        //    }

        //    levelDescriptionText.SetTMPText(text);
        //}

        //private string GetFormatTime(float overTime)
        //{
        //    UnityHelper.OutTime(overTime, out int hour, out int minute, out int seconds);
        //    var language = ConfigManager.Instance.Tables.Tblanguage;
        //    string secondText = language["time_second_1"].current;
        //    string minuteText = language["time_minute_1"].current;
        //    string hourText = language["time_hour_1"].current;
        //    string text = "";
        //    text += seconds.ToString() + secondText;
        //    if (minute > 0)
        //    {
        //        text = minute.ToString() + minuteText + text;
        //    }

        //    if (hour > 0)
        //    {
        //        text = hour.ToString() + hourText + text;
        //    }

        //    return text;
        //}


        //private async void onTresureButtonClick()
        //{
        //    Debug.Log($"treasureButtonClick");
        //    var boxPanel =
        //        await UIHelper.CreateAsync(this, UIType.UILevelBox, this.GetComponent<RectTransform>(), true);
        //    this?.AddChild(boxPanel);
        //}

        //private async void onLevelImageClicked()
        //{
        //    Debug.Log($"levelButtonClick");
        //    UIHelper.Create(UIType.UILevelScrollView);
        //    //children.Add(uiLevelScrollView.GameObject);
        //    //this.AddChild(uiLevelScrollView);
        //}

        //private void OnPassButtonClick()
        //{
        //    Debug.Log($"passButtonClick");
        //    UIHelper.Create(UIType.UIPanel_Pass);
        //}


        //private void OnDailyActiveButtonClick()
        //{
        //    Debug.Log($"DailyActiveButtonClick");
        //}


        //private void OnDailyChallegeClick()
        //{
        //    Debug.Log($"challengeButtonClick");
        //}

        //private void OnSpecialLevelButtonClick()
        //{
        //    Debug.Log($"speciallevelButtonClick");
        //}

        //private void OnExtendButtonClick()
        //{
        //    Debug.Log($"extendButtonClick");
        //    //UIHelper.CreateAsyncNew(this, UIType.UIExtendSettings, this.GetRectTransform().Find("extendPanelPos"));
        //    GetFromReference(KRoot_ExtendSetting)?.SetActive(true);
        //}

        //private void InitExtendSetting()
        //{
        //    this.GetButton(KBtn_ExtendSettingClose)?.OnClick.Add(OnExtendClose);

        //    int funcID = 0;
        //    funcID = 5204;
        //    InitExtendDetails(KsettingsButton_BG, funcID);
        //    funcID = 5201;
        //    InitExtendDetails(KemailButton_BG, funcID);
        //    funcID = 5202;
        //    InitExtendDetails(KNoteButton_BG, funcID);
        //    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KemailButton_BG), OnMailClick);
        //    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KsettingsButton_BG),
        //        OnSettingsButtonClick);
        //    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KNoteButton_BG), OnNoteButtonClick);

        //    var emailRedDot = GetFromReference(KemailButton_BG)?.GetRectTransform().GetChild(0).gameObject;
        //    if (JiYuUIHelper.IsMailRedDot())
        //    {
        //        emailRedDot.SetActive(true);
        //    }
        //    else
        //    {
        //        emailRedDot.SetActive(false);
        //    }
        //}

        //private void OnExtendClose()
        //{
        //    GetFromReference(KRoot_ExtendSetting)?.SetActive(false);
        //}

        //private void OnNoteButtonClick()
        //{
        //    Log.Debug("OnNoteButtonClick", Color.cyan);
        //}

        //private void OnSettingsButtonClick()
        //{
        //    Log.Debug("OnSettingsButtonClick", Color.cyan);
        //    UIHelper.CreateAsync(UIType.UIPanel_Settings);
        //    OnExtendClose();
        //}

        //private void OnMailClick()
        //{
        //    Log.Debug("OnSettingsButtonClick", Color.cyan);
        //    UIHelper.CreateAsync(UIType.UIPanel_Mail);
        //    OnExtendClose();
        //}

        //private void InitExtendDetails(string kRoot, int funcID)
        //{
        //    var language = ConfigManager.Instance.Tables.Tblanguage;
        //    var func = ConfigManager.Instance.Tables.Tbtag_func.DataMap;
        //    var parentUI = this.GetFromReference(kRoot);
        //    parentUI.GetRectTransform().GetChild(0).gameObject.SetActive(false);
        //    parentUI.GetRectTransform().GetChild(2).gameObject.GetComponent<TMP_Text>()
        //        .SetTMPText(language.Get(func[funcID].name).current);
        //    parentUI.GetRectTransform().GetChild(1).gameObject.GetComponent<XImage>()
        //        .SetSprite(ResourcesManager.LoadAsset<Sprite>(func[funcID].icon), true);
        //}

        //protected override void OnClose()
        //{
        //    //WebMessageHandler.Instance.RemoveHandler(2, 4, LevelResponse);
        //    RedPointMgr.instance.Remove(Battle_Red_Point_Root, Battle_Red_Point_Root);
        //    UILevelScrollView.ChangeEvent -= ChangeLevelSprite;
        //    //ClearView();
        //    // this.RemoveChild(KCenterPanel);

        //    if (isTimer)
        //    {
        //        RemoveTimer();
        //    }


        //    #region Ѹ��з

        //    if (timerActionId != -1)
        //    {
        //        TimerManager.Instance.RemoveTimerId(ref timerActionId);
        //    }

        //    #endregion

        //    base.OnClose();
        //}


        //#region InitBlackBoardEntity

        //private EntityManager entityManager;
        //private EntityQuery switchSceneQuery;
        //private long timerActionId = -1;

        //private async void OnStartButtonClick()
        //{
        //    var global = Common.Instance.Get<Global>();
        //    if (isTimer)
        //    {
        //        RemoveTimer();
        //        isTimer = false;
        //    }


        //    //PlayerPrefs.DeleteAll();
        //    // var viewList = UnityEngine.GameObject.FindGameObjectsWithTag("handelDestroy");
        //    // if (viewList.Length > 0)
        //    // {
        //    //     foreach (var view in viewList)
        //    //     {
        //    //         GameObject.Destroy(view.gameObject);
        //    //     }
        //    // }
        //    if (global.isStandAlone)
        //    {
        //        var sceneController = Common.Instance.Get<SceneController>();
        //        var sceneObj = sceneController.LoadSceneAsync<RunTimeScene>(SceneName.RunTime);
        //        SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();
        //    }

        //    WebMessageHandler.Instance.AddHandler(2, 3, OnClickPlayBtnBeforeResponse);
        //    WebMessageHandler.Instance.AddHandler(2, 5, OnClickPlayBtnResponse);


        //    NetWorkManager.Instance.SendMessage(2, 3);
        //}


        //void OnClickPlayBtnBeforeResponse(object sender, WebMessageHandler.Execute e)
        //{
        //    WebMessageHandler.Instance.RemoveHandler(2, 3, OnClickPlayBtnBeforeResponse);
        //    var longValue = new LongValue();
        //    longValue.MergeFrom(e.data);


        //    if (e.data.IsEmpty)
        //    {
        //        Log.Debug("e.data.IsEmpty", Color.red);
        //        return;
        //    }

        //    Log.Debug($"战局ID:{longValue.Value}", Color.green);
        //    var battleGain = new BattleGain
        //    {
        //        BattleId = longValue.Value,
        //        LevelId = ResourcesSingleton.Instance.levelInfo.levelId,
        //        Type = 0,
        //        RoleId = 0,
        //        PassStatus = "",
        //        LiveTime = 0,
        //        KillMobs = 0,
        //        KillElite = 0,
        //        KillBoss = 0
        //    };


        //    NetWorkManager.Instance.SendMessage(2, 5, battleGain);
        //}

        //void OnClickPlayBtnResponse(object sender, WebMessageHandler.Execute e)
        //{
        //    WebMessageHandler.Instance.RemoveHandler(2, 5, OnClickPlayBtnResponse);
        //    var longValue = new BoolValue();
        //    longValue.MergeFrom(e.data);
        //    if (e.data.IsEmpty)
        //    {
        //        Log.Debug("e.data.IsEmpty", Color.red);
        //        return;
        //    }

        //    Log.Debug($"验证对局是否可以开始:{longValue.Value}", Color.green);
        //    if (longValue.Value)
        //    {
        //        //this.GetParent<UIPanel_JiyuGame>().DestoryAllToggle();
        //        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //        switchSceneQuery = entityManager.CreateEntityQuery(typeof(SwitchSceneData));

        //        Log.Debug($"switch:{switchSceneQuery.CalculateEntityCount()}");

        //        //this.Close();
        //        var sceneController = Common.Instance.Get<SceneController>();
        //        var sceneObj = sceneController.LoadSceneAsync<RunTimeScene>(SceneName.RunTime);
        //        SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();

        //        //TODO:ʹ��ecs���ݹ�����س���
        //        // var switchSceneData = switchSceneQuery.ToComponentDataArray<SwitchSceneData>(Allocator.Temp)[0];
        //        //  switchSceneData.mainScene.LoadAsync(new ContentSceneParameters()
        //        //  {
        //        //      loadSceneMode = LoadSceneMode.Single
        //        //  });
        //    }
        //    else
        //    {
        //        //TODO:ͨ����ʾ��
        //        Log.Debug("longValue is false,maybe resources are not enough", Color.red);
        //    }
        //}

        //#endregion
    }
}
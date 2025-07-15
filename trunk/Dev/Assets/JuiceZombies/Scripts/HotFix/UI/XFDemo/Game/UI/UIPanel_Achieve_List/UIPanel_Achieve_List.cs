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
using Google.Protobuf;
using HotFix_UI;
using Main;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Achieve_List)]
    internal sealed class UIPanel_Achieve_ListEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Achieve_List;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Achieve_List>();
        }
    }

    public partial class UIPanel_Achieve_List : UI, IAwake<int>
    {
        #region 参数

        private int achieveGroupId = -1;
        private Tbachieve_group tbachieve_Group;
        private Tblanguage tblanguage;
        private Tbtask tbtask;
        private Tbtask_group tbtask_group;
        private Tbtask_type tbtask_type;
        //private int LastBottom = -1;

        //private int BottomID = 0;
        private Dictionary<int, List<int>> groupIDs = new Dictionary<int, List<int>>();
        private Dictionary<int, List<Vector3>> SortDic = new Dictionary<int, List<Vector3>>();

        #endregion

        private CancellationTokenSource cts = new CancellationTokenSource();

        public async void Initialize(int id)
        {
            await JiYuUIHelper.InitBlur(this);
            achieveGroupId = id;
            //Debug.Log(thisID);
            InitJson();
            //GroupSort();
            //ContentHSet(tbachieve_Group.DataList[achieveGroupId - 1].groupId.Count);
            //BottomID = achieveGroupId;
            //CreateTask(achieveGroupId).Forget();
            BottomInit().Forget();

            InitEffect();
        }

        private void InitEffect()
        {


            JiYuTweenHelper.SetEaseAlphaAndPosUtoB(this.GetFromReference(UIPanel_Achieve_List.KScrollView_Item0), 0, 50f,
    cancellationToken: cts.Token);

            int i = 0;
            var contentList = this.GetFromReference(KScrollView).GetScrollRect().Content.GetList();
            foreach (UI item in contentList.Children)
            {
                i++;
                var items = item as UISubPanel_Achieve_Gift;
                JiYuTweenHelper.SetEaseAlphaAndPosB2U(items.GetFromReference(UISubPanel_Achieve_Gift.KBg),
                    0, 50,cancellationToken: cts.Token,0.35f + 0.02f * i, true, true);
            }
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        private void InitJson()
        {
            tbachieve_Group = ConfigManager.Instance.Tables.Tbachieve_group;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtask = ConfigManager.Instance.Tables.Tbtask;
            tbtask_type = ConfigManager.Instance.Tables.Tbtask_type;
            tbtask_group = ConfigManager.Instance.Tables.Tbtask_group;
        }

        private async UniTaskVoid CreateTask(int achieveGroupId)
        {
            var contentList = this.GetFromReference(KScrollView).GetScrollRect().Content.GetList();
            contentList.Clear();

            //foreach (int i in tbachieve_Group.Get(id).groupId)
            //{
            //    //i是任务组的id
            //    var item = await contentList.CreateWithUITypeAsync<List<int>>(UIType.UISubPanel_Achieve_Gift, groupIDs[i], false);
            //    item.GetRectTransform().SetWidth(this.GetFromReference(KScrollView).GetRectTransform().Width());
            //}
            // Dictionary<UI, int> uiSortDic = new Dictionary<UI, int>();
            var achieveGroup = tbachieve_Group.Get(achieveGroupId);
            var taskList = ResourcesSingleton.Instance.achieveInfo.TaskInfoList
                .Where(a => achieveGroup.groupId.Exists(b => b == a.Group))
                .ToList();

            bool hasRedDot = false;
            for (int i = 0; i < taskList.Count; i++)
            {
                int index = i;
                var gameTaskInfo = taskList[i];
                var task = tbtask.Get(gameTaskInfo.Id);
                var ui = await contentList.CreateWithUITypeAsync(UIType.UISubPanel_Achieve_Gift,
                    gameTaskInfo, false);
                //uiSortDic.Add(item, ihelp);
                ui.GetRectTransform().SetWidth(this.GetFromReference(KScrollView).GetRectTransform().Width());
                //对每个成就的按钮进行设置
                var KBtn = ui.GetFromReference(UISubPanel_Achieve_Gift.KBtn);

                var isRedDot = gameTaskInfo.Para >= task.para[0] && gameTaskInfo.Status == 0;

                if (isRedDot)
                {
                    hasRedDot = true;
                    KBtn.GetImage().SetSpriteAsync("common_yellow_button_5", false).Forget();
                }
                else
                {
                    KBtn.GetImage().SetSpriteAsync("common_blue_button_6", false).Forget();
                }

                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn, () =>
                {
                    if (gameTaskInfo.Status == 0)
                    {
                        var task = tbtask.Get(gameTaskInfo.Id);
                        bool isRedDot = gameTaskInfo.Para >= task.para[0] && gameTaskInfo.Status == 0;


                        if (isRedDot)
                        {
                            WebMessageHandler.Instance.AddHandler(CMD.GETTASKSCORE, OnClaimResponse);
                            NetWorkManager.Instance.SendMessage(CMD.GETTASKSCORE, new IntValue
                            {
                                Value = gameTaskInfo.Id
                            });
                        }
                        else
                        {
                            Log.Debug($"前往");
                            var taskType = tbtask_type.Get(tbtask.Get(gameTaskInfo.Id).type);
                            var str = taskType.goto0;
                            //str = $"type=1;para=[411]";

                            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Achieve, out var ui))
                            {
                                ui.Dispose();
                            }

                            Close();
                            JiYuUIHelper.GoToSomePanel(str);
                        }
                    }
                });


                //RedPointMgr.instance.Init(UIPanel_Battle.Battle_Red_Point_Root, "task_group" + ((int)SortDic[id][i].x).ToString(),
                //    (RedPointState state, int data) =>
                //    {
                //        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Achieve_List, out UI uuii))
                //        {
                //            item.GetFromReference(UISubPanel_Achieve_Gift.KImg_RedPoint).SetActive(state == RedPointState.Show);
                //        }
                //    });

                //item.GetFromReference(UISubPanel_Achieve_Gift.KImg_RedPoint).SetActive(true);
            }

            contentList.Sort((UI a, UI b) =>
            {
                var uia = a as UISubPanel_Achieve_Gift;
                var uib = b as UISubPanel_Achieve_Gift;

                var groupa = tbtask_group.Get(uia.gameTaskInfo.Group);
                var groupb = tbtask_group.Get(uib.gameTaskInfo.Group);
                return groupa.sort.CompareTo(groupb.sort);
            });

          
            // var KCommon_Bottom = GetFromReference(UIPanel_Achieve_List.KCommon_Bottom);
            // var KScrollView_Item0 = KCommon_Bottom.GetFromReference(UICommon_Bottom.KScrollView_Item0);
            // var content = KScrollView_Item0.GetScrollRect().Content;
            // var bottomList = content.GetList();
            // foreach (var ui in bottomList.Children)
            // {
            //     var uis = ui as UICommon_BottomBtn;
            //     if (uis.id == achieveGroupId)
            //     {
            //         var KImg_RedDot = ui.GetFromReference(UICommon_BottomBtn.KImg_RedDot);
            //         KImg_RedDot.SetActive(hasRedDot);
            //         break;
            //     }
            // }


            //JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KScrollView).GetScrollRect().Content);
        }

        private void OnClaimResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.GETTASKSCORE, OnClaimResponse);
            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);
            //Debug.Log(e);
            //Debug.Log(stringValueList);
            if (e.data.IsEmpty)
            {
                Log.Debug("OnClaimResponse e.data.IsEmpty", Color.red);
                return;
            }

            //List<Vector3> reList = new List<Vector3>();
            var reList = JiYuUIHelper.TurnStrReward2List(stringValueList.Values);
            // foreach (var itemstr in stringValueList.Values)
            // {
            //     reList.Add(UnityHelper.StrToVector3(itemstr));
            // }

            UIHelper.CreateAsync(UIType.UICommon_Reward, reList);

            WebMessageHandler.Instance.AddHandler(CMD.QUERYACHIEVEMENT, OnAchieveResponse);
            NetWorkManager.Instance.SendMessage(CMD.QUERYACHIEVEMENT);
        }

        private void OnAchieveResponse(object sender, WebMessageHandler.Execute e)
        {
            //TODO:
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERYACHIEVEMENT, OnAchieveResponse);
            RoleTaskInfo roleTaskInfo = new RoleTaskInfo();
            roleTaskInfo.MergeFrom(e.data);
            Log.Debug($"roleTaskInfo:{roleTaskInfo}", Color.green);
            if (e.data.IsEmpty)
            {
                Log.Debug("3-4RoleTaskInfo.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingleton.Instance.achieveInfo = roleTaskInfo;
            Initialize(achieveGroupId);
            //
            // ResourcesSingleton.Instance.achieve.tasks.Clear();
            // ResourcesSingleton.Instance.achieve.boxes.Clear();
            //
            // foreach (var t in roleTaskInfo.TaskInfoList)
            // {
            //     //status为领取状态0未领取，1领取， para任务参数
            //     ResourcesSingleton.Instance.achieve.tasks.Add(t.Id, new Vector2(t.Status, t.Para));
            // }
            //
            // Debug.Log("s.roleTaskInfo.TaskScoreList.count:" + roleTaskInfo.TaskScoreList.Count);
            //
            // foreach (var s in roleTaskInfo.AchieveList)
            // {
            //     //status为领取状态0未领取，1领取， valid生效与否0未生效1生效
            //     ResourcesSingleton.Instance.achieve.boxes.Add(s.Id, new Vector2(s.Status, 0));
            //     Debug.Log("get one achieve then update:" + ResourcesSingleton.Instance.achieve.boxes.Count);
            // }

            // GroupSort();
            // CreateTask(LastBottom).Forget();
            // var parent = this.GetParent<UIPanel_Achieve>();
            // parent.UpdateGroup();
        }

        private async UniTaskVoid BottomInit()
        {
            //this.GetChild(UIType.UICommon_BottomContainer)
            var KCommon_Bottom = GetFromReference(UIPanel_Achieve_List.KCommon_Bottom);

            var KScrollView_Item0 = KCommon_Bottom.GetFromReference(UICommon_Bottom.KScrollView_Item0);
            var KBtn_Close = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_Close);
            var KBtn_TitleInfo = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_TitleInfo);
            var KText_BottomTitle = KCommon_Bottom.GetFromReference(UICommon_Bottom.KText_BottomTitle);
            KText_BottomTitle.SetActive(false);
            KBtn_TitleInfo.SetActive(false);
            var content = KScrollView_Item0.GetScrollRect().Content;
            var bottomList = content.GetList();
            bottomList.Clear();
            KBtn_Close.GetXButton().RemoveAllListeners();
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, () =>
            {
                // bottomList.Clear();
                // bottomUi.Dispose();
                Close();
            });

            for (int i = 0; i < tbachieve_Group.DataList.Count; i++)
            {
                int index = i;
                var achieveGroup = tbachieve_Group.DataList[index];
                var ui = await bottomList.CreateWithUITypeAsync(UIType.UICommon_BottomBtn, achieveGroup.id, false);

                var KText_Btn = ui.GetFromReference(UICommon_BottomBtn.KText_Btn);
                var KBg_Mask = ui.GetFromReference(UICommon_BottomBtn.KBg_Mask);
                var KBg_Mask1 = ui.GetFromReference(UICommon_BottomBtn.KBg_Mask1);
                var KImg_RedDot = ui.GetFromReference(UICommon_BottomBtn.KImg_RedDot);
                KBg_Mask.SetActive(true);
                KBg_Mask1.SetActive(true);
                //KImg_RedDot.SetActive(false);
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Achieve, out var achieve))
                {
                    var uis = achieve as UIPanel_Achieve;
                    var redDot = uis.GetGroupRedDot(achieveGroup.id);
                    KImg_RedDot.SetActive(redDot > 0);
                }


                KText_Btn.GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(achieveGroup.name).current);


                if (achieveGroupId == achieveGroup.id)
                {
                    KBg_Mask.SetActive(false);
                    KBg_Mask1.SetActive(false);
                    CreateTask(achieveGroupId).Forget();
                }

                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(ui, () =>
                {
                    if (achieveGroupId == achieveGroup.id)
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
                    //ui.GetRectTransform().DoScale2(new Vector2(1.1f, 1.1f), 0.2f);
                    achieveGroupId = achieveGroup.id;
                    //GroupSort();
                    //ContentHSet(achieveGroup.groupId.Count);
                    CreateTask(achieveGroup.id).Forget();
                    //RedPointMgr.instance.PrintRedPointTree(UIPanel_Battle.Battle_Red_Point_Root, UIPanel_Battle.Battle_Red_Point_Root);
                }, 1101);
                //ui.GetButton().OnClick.Add();
            }

            bottomList.Sort((a, b) =>
            {
                var uia = a as UICommon_BottomBtn;
                var uib = b as UICommon_BottomBtn;
                return uia.id.CompareTo(uib.id);
            });
        }

        protected override void OnClose()
        {
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Achieve, out var ui))
            {
                var uia = ui as UIPanel_Achieve;
                uia.Update();
            }

            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}
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

namespace XFramework
{
    [UIEvent(UIType.UIPanel_MonopolyTaskShop)]
    internal sealed class UIPanel_MonopolyTaskShopEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_MonopolyTaskShop;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_MonopolyTaskShop>();
        }
    }

    public partial class UIPanel_MonopolyTaskShop : UI, IAwake<int, float>,
        ILoopScrollRectProvide<UISubPanel_MonopolyTaskShopItem>
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
        private Tbtask tbtask;
        private Tbtask_type tbtask_type;
        private Tbtask_group tbtask_group;
        private Tbtask_score tbtask_score;

        public int activityId;
        public float pos;
        private long timerId;
        private long endTime;
        private Vector3 diceItemReward;
        private string m_RedDotName;
        public int redDotNum = 0;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public async void Initialize(int args, float pos)
        {
            await JiYuUIHelper.InitBlur(this);
            activityId = args;
            this.pos = pos;
            InitJson();
            InitNode();
            JiYuTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(KImg_Title), cts.Token);
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
            tbtask = ConfigManager.Instance.Tables.Tbtask;
            tbtask_type = ConfigManager.Instance.Tables.Tbtask_type;
            tbtask_group = ConfigManager.Instance.Tables.Tbtask_group;
            tbtask_score = ConfigManager.Instance.Tables.Tbtask_score;
        }

        async UniTaskVoid InitNode()
        {
            WebMessageHandler.Instance.AddHandler(CMD.GETTASKSCORE, OnGetTaskResponse);

            var KScrollView = GetFromReference(UIPanel_MonopolyShop.KScrollView);
            var KBtn_Desc = GetFromReference(UIPanel_MonopolyShop.KBtn_Desc);
            var KText_Title = GetFromReference(UIPanel_MonopolyShop.KText_Title);
            var KText_Time = GetFromReference(UIPanel_MonopolyShop.KText_Time);
            var KBtn_Close = GetFromReference(UIPanel_MonopolyShop.KBtn_Close);

            var KImg_Title = GetFromReference(UIPanel_MonopolyShop.KImg_Title);
            //var KCommon_ItemTips = GetFromReference(UIPanel_MonopolyShop.KCommon_ItemTips);
            //var KText_Des = GetFromReference(UIPanel_MonopolyShop.KText_Des);
            if (!ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.TryGetValue(activityId,
                    out var gameActivity))
            {
                Close();
            }

            endTime = gameActivity.MonopolyRecord.EndTime;
            var activity = tbactivity.Get(activityId);
            var monopoly = tbmonopoly.Get(activity.link);

            m_RedDotName = NodeNames.GetTagFuncRedDotName(monopoly.tagFunc);
            diceItemReward = new Vector3(5, monopoly.diceItem, 1);
            //string diceStr = $"x{JiYuUIHelper.GetRewardCount(diceReward)}";

            string shopItemStr =
                $"{JiYuUIHelper.GetRewardTextIconName(diceItemReward)}x{JiYuUIHelper.GetRewardCount(diceItemReward)}";

            KText_Title.GetTextMeshPro().SetTMPText(shopItemStr);
            KImg_Title.GetImage().SetSpriteAsync(monopoly.taskPic, true).Forget();
            //KText_Des.GetTextMeshPro().SetTMPText(tblanguage.Get("monopoly_shop_desc").current);

            //KCommon_ItemTips.SetActive(false);
            var descStr = tblanguage.Get("monopoly_task_desc").current;

            descStr = string.Format(descStr, JiYuUIHelper.GetRewardName(diceItemReward));
            KText_Time.GetTextMeshPro().SetTMPText(descStr);
            if (!ResourcesSingleton.Instance.activity.activityTaskDic.TryGetValue(activityId, out var taskList))
            {
                return;
            }

            taskList.Sort((a, b) =>
            {
                var taska = tbtask.Get(a.Id);
                var taskb = tbtask.Get(b.Id);
                return taska.sort.CompareTo(taskb.sort);
            });

            var loopRect = KScrollView.GetLoopScrollRect<UISubPanel_MonopolyTaskShopItem>();
            loopRect.Content.GetRectTransform().SetWidth(Screen.width - 30f);

            loopRect.SetProvideData(UIPathSet.UISubPanel_MonopolyTaskShopItem, this);
            loopRect.SetTotalCount(taskList.Count);
            loopRect.RefillCells();

            // var scrollRect = KScrollView.GetXScrollRect();
            // var content = scrollRect.Content;
            // var list = content.GetList();
            // list.Clear();


            //RedDotManager.Instance.InsterNode(itemStr);


            // int redDotNum = 0;
            // foreach (var gameTask in taskList)
            // {
            //     //RedDotManager.Instance.ClearChildrenListeners(pos1);
            //     var task = tbtask.Get(gameTask.Id);
            //     var task_group = tbtask_group.Get(task.group);
            //     var task_type = tbtask_type.Get(task.type);
            //
            //     var ui = await list.CreateWithUITypeAsync(UIType.UISubPanel_MonopolyTaskShopItem, gameTask.Id, false);
            //     var KRewardPos = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KRewardPos);
            //     var KText_Name = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_Name);
            //     var KText_Cost = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_Cost);
            //     var KBtn_Buy = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KBtn_Buy);
            //     var KText_Buy = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_Buy);
            //     var KText_BuyFinish = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_BuyFinish);
            //     var KImg_Fill = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KImg_Fill);
            //     var KText_Fill = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_Fill);
            //
            //
            //     KText_BuyFinish.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gained").current);
            //     int serverPara = Mathf.Min(gameTask.Para, task.para[0]);
            //     bool canReceive = gameTask.Para >= task.para[0] && gameTask.Status == 0;
            //
            //     KRewardPos.GetImage().SetSpriteAsync(task_type.icon, false).Forget();
            //     var desc = string.Format(tblanguage.Get(task_type.desc[0]).current, task.para[0]);
            //     KText_Name.GetTextMeshPro().SetTMPText(desc);
            //     KText_Cost.GetTextMeshPro()
            //         .SetTMPText($"{JiYuUIHelper.GetRewardTextIconName(diceItemReward)}x{(int)task.reward[0].z}");
            //     KText_Fill.GetTextMeshPro().SetTMPText(serverPara.ToString() + "/" + task.para[0]);
            //     float fillRatio =
            //         (float)serverPara / (float)task.para[0];
            //     KImg_Fill.GetImage().SetFillAmount(fillRatio);
            //     KText_BuyFinish.SetActive(false);
            //     KBtn_Buy.SetActive(true);
            //     if (gameTask.Status == 1)
            //     {
            //         KText_BuyFinish.SetActive(true);
            //         KBtn_Buy.SetActive(false);
            //     }
            //
            //
            //     var receiveStr = tblanguage.Get("common_state_goto").current;
            //     if (canReceive)
            //     {
            //         receiveStr = tblanguage.Get("common_state_gain").current;
            //         redDotNum++;
            //     }
            //
            //     KText_Buy.GetTextMeshPro().SetTMPText(receiveStr);
            //
            //     JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Buy, () =>
            //     {
            //         //TODO:Ç°Íù£º
            //         if (canReceive)
            //         {
            //             NetWorkManager.Instance.SendMessage(CMD.GETTASKSCORE, new IntValue
            //             {
            //                 Value = gameTask.Id
            //             });
            //         }
            //     });
            // }

            // list.Sort((a, b) =>
            // {
            //     var uia = a as UISubPanel_MonopolyTaskShopItem;
            //     var uib = b as UISubPanel_MonopolyTaskShopItem;
            //     var taska = tbtask.Get(uia.taskId);
            //     var taskb = tbtask.Get(uib.taskId);
            //
            //     return taska.sort.CompareTo(taskb.sort);
            // });
            //
            loopRect.SetVerticalNormalizedPosition(pos);
            KBtn_Close.GetButton().OnClick.Add(() =>
            {
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Activity_Monopoly, out var ui))
                {
                    var uis = ui as UIPanel_Activity_Monopoly;
                    uis.Refresh();
                }

                Close();
            });
            await UniTask.Yield();
            Log.Debug($"redDotNum {redDotNum}");
            var taskListStr =
                $"{m_RedDotName}|Pos1";
            RedDotManager.Instance.SetRedPointCnt(taskListStr, redDotNum);
        }

        async private void OnGetTaskResponse(object sender, WebMessageHandler.Execute e)
        {
            TaskResult taskResult = new TaskResult();
            taskResult.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            if (taskResult.Res == "false")
            {
                return;
            }

            var reward = JiYuUIHelper.TurnStrReward2List(taskResult.Reward);
            var ui = await UIHelper.CreateAsync(UIType.UICommon_Reward, reward);
            var KBtn_Close = ui.GetFromReference(UICommon_Reward.KBtn_Close);
            var KBg_Img = ui.GetFromReference(UICommon_Reward.KBg_Img);
            KBtn_Close.GetButton().OnClick.Add(() => { Refresh().Forget(); });
            KBg_Img.GetButton().OnClick.Add(() => { Refresh().Forget(); });
        }

        private async UniTaskVoid Refresh()
        {
            NetWorkManager.Instance.SendMessage(CMD.QUERYACTIVITYTASK, new IntValue
            {
                Value = activityId
            });
        }

        public void ProvideData(UISubPanel_MonopolyTaskShopItem ui, int index)
        {
            //Log.Debug($"index {index}");
            if (!ResourcesSingleton.Instance.activity.activityTaskDic.TryGetValue(activityId, out var taskList))
            {
                return;
            }

            var gameTask = taskList[index];
            ObjectHelper.Awake(ui, gameTask.Id);

            var task = tbtask.Get(gameTask.Id);
            var task_group = tbtask_group.Get(task.group);
            var task_type = tbtask_type.Get(task.type);
            var KRewardPos = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KRewardPos);
            var KText_Name = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_Name);
            //var KText_Cost = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_Cost);
            //var KBtn_Buy = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KBtn_Buy);
            //var KText_Buy = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_Buy);
            var KText_BuyFinish = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_BuyFinish);
            var KImg_Fill = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KImg_Fill);
            var KText_Fill = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_Fill);
            var KCommon_CostBtn = ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KCommon_CostBtn);
            var KText_Cost = KCommon_CostBtn.GetFromReference(UICommon_CostBtn.KText_Cost);
            var KCommon_Btn = KCommon_CostBtn.GetFromReference(UICommon_CostBtn.KCommon_Btn);
            var KText_Btn = KCommon_Btn.GetFromReference(UICommon_Btn.KText_Btn);
            var KImg_RedDotRight = KCommon_Btn.GetFromReference(UICommon_Btn.KImg_RedDotRight);

            KImg_RedDotRight.SetActive(false);
            //var KBtn_Buy = KCommon_CostBtn.GetFromReference(UICommon_CostBtn.KCommon_Btn);

            KText_BuyFinish.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gained").current);
            int serverPara = Mathf.Min(gameTask.Para, task.para[0]);
            bool canReceive = gameTask.Para >= task.para[0] && gameTask.Status == 0;
            KRewardPos.GetImage().SetSpriteAsync(task_type.icon, false).Forget();
            var desc = string.Format(tblanguage.Get(task_type.desc[0]).current, task.para[0]);
            KText_Name.GetTextMeshPro().SetTMPText(desc);
            KText_Cost.GetTextMeshPro()
                .SetTMPText($"{JiYuUIHelper.GetRewardTextIconName(diceItemReward)}x{(int)task.reward[0].z}");
            KText_Fill.GetTextMeshPro().SetTMPText(serverPara.ToString() + "/" + task.para[0]);
            float fillRatio =
                (float)serverPara / (float)task.para[0];
            KImg_Fill.GetImage().SetFillAmount(fillRatio);
            KText_BuyFinish.SetActive(false);
            KCommon_Btn.SetActive(true);
            if (gameTask.Status == 1)
            {
                KText_BuyFinish.SetActive(true);
                KCommon_Btn.SetActive(false);
            }


            var receiveStr = tblanguage.Get("common_state_goto").current;
            if (canReceive)
            {
                receiveStr = tblanguage.Get("common_state_gain").current;

                redDotNum++;
                ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KMid). GetImage().SetSpriteAsync("activity_common_task_item_container_2", true).Forget();
                KCommon_Btn.GetImage().SetSpriteAsync("icon_btn_green", true).Forget();
            }
            else
            {
                ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KMid).GetImage().SetSpriteAsync("activity_common_task_item_container_1", true).Forget();
                KCommon_Btn.GetImage().SetSpriteAsync("common_yellow_button_9", true).Forget();
            }

            KText_Btn.GetTextMeshPro().SetTMPText(receiveStr);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KCommon_Btn, () =>
            {
                //TODO:Ç°Íù£º
                if (canReceive)
                {
                    NetWorkManager.Instance.SendMessage(CMD.GETTASKSCORE, new IntValue
                    {
                        Value = gameTask.Id
                    });
                }
                else
                {
                    Log.Debug($"Ç°Íù");
                    var taskType = tbtask_type.Get(tbtask.Get(gameTask.Id).type);

                    JiYuUIHelper.GoToSomePanel(taskType.goto0);
                }
            });
            if (index <5)
            {
                JiYuTweenHelper.SetEaseAlphaAndPosB2U(ui.GetFromReference(UISubPanel_MonopolyTaskShopItem.KMid), 0, 50,
         cancellationToken: cts.Token,
         0.35f + 0.02f * index, true, true);
            }


        }

        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();
            WebMessageHandler.Instance.RemoveHandler(CMD.GETTASKSCORE, OnGetTaskResponse);
            base.OnClose();
        }
    }
}
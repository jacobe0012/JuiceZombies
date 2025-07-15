//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using Common;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Achieve_Gift)]
    internal sealed class UISubPanel_Achieve_GiftEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Achieve_Gift;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Achieve_Gift>();
        }
    }

    public partial class UISubPanel_Achieve_Gift : UI, IAwake<GameTaskInfo>
    {
        #region 参数

        private Tblanguage tblanguage;
        private Tbtask tbtask;
        private Tbtag_func tbtag_Func;
        private Tbchapter tbchapter;
        private Tbquality tbquality;
        private Tbtask_type tbtask_Type;
        private Tbequip_quality tbequip_Quality;
        private Tbpower tbpower;

        private List<int> GroupsIDs = new();

        //private int thisTaskID;
        private bool isLast;

        private bool isCompleted;

        //private int thisPara;
        private int iHelp;
        public GameTaskInfo gameTaskInfo;

        #endregion

        public void Initialize(GameTaskInfo gameTaskInfo)
        {
            //GroupsIDs = groupIDs;
            this.gameTaskInfo = gameTaskInfo;
            DataInit();
            // RedPointMgr.instance.Init(UIPanel_Battle.Battle_Red_Point_Root, "task_group" + tbtask.Get(thisTaskID).group,
            //     (state, data) =>
            //     {
            //         if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Achieve_List, out var uuii))
            //             GetFromReference(KImg_RedPoint).SetActive(state == RedPointState.Show);
            //         //Debug.Log("task_group" + tbtask.Get(thisTaskID).group.ToString() + "state is " + state.ToString());
            //     });
            TextInit();
            ImgInit();
            RewardInit();
        }

        private void DataInit()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtask = ConfigManager.Instance.Tables.Tbtask;
            tbtag_Func = ConfigManager.Instance.Tables.Tbtag_func;
            tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            tbquality = ConfigManager.Instance.Tables.Tbquality;
            tbtask_Type = ConfigManager.Instance.Tables.Tbtask_type;
            tbequip_Quality = ConfigManager.Instance.Tables.Tbequip_quality;
            tbpower = ConfigManager.Instance.Tables.Tbpower;
            return;
            for (var i = 0; i < GroupsIDs.Count; i++)
            {
                iHelp = i;
                if (ResourcesSingleton.Instance.achieve.tasks[GroupsIDs[i]][0] == 1)
                {
                    gameTaskInfo.Id = GroupsIDs[i];
                    if (i == GroupsIDs.Count - 1) isLast = true;
                }
                //KProgressBar
                else
                {
                    gameTaskInfo.Id = GroupsIDs[i];
                    if (ResourcesSingleton.Instance.achieve.tasks[GroupsIDs[i]][1] >=
                        tbtask.Get(gameTaskInfo.Id).para[0])
                        isCompleted = true;
                    break;
                }
            }

            // if (iHelp == 0)
            //     thisPara = (int)ResourcesSingleton.Instance.achieve.tasks[GroupsIDs[iHelp]][1] - 0;
            // else
            //     //thisPara = (int)ResourcesSingleton.Instance.achieve.tasks[GroupsIDs[iHelp]][1] - tbtask.Get(GroupsIDs[iHelp - 1]).para[0];
            //     thisPara = (int)ResourcesSingleton.Instance.achieve.tasks[GroupsIDs[iHelp]][1];
        }

        private void TextInit()
        {
            //Debug.Log("thisTaskID" + thisTaskID);
            GetFromReference(KText_Name).GetTextMeshPro()
                .SetTMPText(NameString(gameTaskInfo.Id) + "(" + gameTaskInfo.Para + "/" +
                            tbtask.Get(gameTaskInfo.Id).para[0] +
                            ")");
            //Debug.Log(nameString(thisTaskID));
            GetFromReference(KText_Received).GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gained").current);
            GetFromReference(KBtn).SetActive(true);
            if (gameTaskInfo.Status == 1)
            {
                GetFromReference(KBtn).SetActive(false);
                GetFromReference(KText_Received).SetActive(true);

                GetFromReference(KProgressBar_Can_Claim).SetActive(false);
                GetFromReference(KProgressBar_Cannot_Claim).SetActive(false);
            }
            else if (gameTaskInfo.Status == null || gameTaskInfo.Status == 0)
            {
                var task = tbtask.Get(gameTaskInfo.Id);
                bool isRedDot = gameTaskInfo.Para >= task.para[0] && gameTaskInfo.Status == 0;

                if (isRedDot)
                {
                    GetFromReference(KText_Btn).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("common_state_gain").current);
                    GetFromReference(KProgressBar_Can_Claim).SetActive(true);
                    GetFromReference(KProgressBar_Cannot_Claim).SetActive(false);
                }
                else
                {
                    GetFromReference(KText_Btn).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("common_state_goto").current);

                    GetFromReference(KProgressBar_Cannot_Claim).SetActive(true);
                    GetFromReference(KProgressBar_Can_Claim).SetActive(false);
                    var thisW = GetFromReference(KProgressBar_Bg).GetRectTransform().Width();
                    if (gameTaskInfo.Para <= 0)
                    {
                        GetFromReference(KProgressBar_Cannot_Claim).GetRectTransform().SetOffsetWithRight(-thisW);
                    }
                    else
                    {
                        var whelp = thisW * (tbtask.Get(gameTaskInfo.Id).para[0] - gameTaskInfo.Para) /
                                    tbtask.Get(gameTaskInfo.Id).para[0];
                        GetFromReference(KProgressBar_Cannot_Claim).GetRectTransform().SetOffsetWithRight(-whelp);
                    }
                }
            }

            this.GetFromReference(KText_Star).GetTextMeshPro().SetTMPText(tbtask.Get(gameTaskInfo.Id).score.ToString());
        }

        private void ImgInit()
        {
            return;
            if (isLast)
            {
                //全体变透明
            }
            else
            {
                if (isCompleted)
                {
                    //RedPointMgr.instance.SetState(UIPanel_Battle.Battle_Red_Point_Root,
                    //"task_group" + tbtask.Get(thisTaskID).group, RedPointState.Show);
                }
                else
                {
                    // RedPointMgr.instance.SetState(UIPanel_Battle.Battle_Red_Point_Root,
                    //     "task_group" + tbtask.Get(thisTaskID).group, RedPointState.Hide);
                }
            }
        }

        private void RewardInit()
        {
            var itemsList = GetFromReference(KBg_Items).GetList();
            itemsList.Clear();
            var BgW = GetFromReference(KBg_Items).GetRectTransform().Width();
            var reW = BgW / 6;
            var rewardCount = tbtask.Get(gameTaskInfo.Id).reward.Count;
            for (var i = 0; i < rewardCount; i++)
            {
                var ihelp = i;
                var itemUI = itemsList.CreateWithUIType(UIType.UICommon_RewardItem,
                    tbtask.Get(gameTaskInfo.Id).reward[ihelp], false);
                //JiYuUIHelper.SetDefaultRect(itemUI);
            }

            JiYuUIHelper.ForceRefreshLayout(GetFromReference(KBg_Items));
        }

        private string NameString(int taskID)
        {
            var nameHelpStr = tblanguage.Get(tbtask_Type.Get(tbtask.Get(taskID).type).desc[0]).current;
            var namestr = nameHelpStr.Replace("{0}", tbtask.Get(taskID).para[0].ToString());
            if (tbtask.Get(taskID).para.Count != 1)
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

                    case 2041:
                        namestr = namestr.Replace("{1}",
                            tblanguage.Get(tbpower.Get(tbtask.Get(taskID).para[1]).name).current);
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

            return namestr;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}
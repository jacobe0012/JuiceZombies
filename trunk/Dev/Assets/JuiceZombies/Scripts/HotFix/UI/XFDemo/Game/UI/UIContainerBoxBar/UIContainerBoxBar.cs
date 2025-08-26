//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIContainerBoxBar)]
    internal sealed class UIContainerBoxBarEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIContainerBoxBar;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIContainerBoxBar>();
        }
    }

    public partial class UIContainerBoxBar : UI, IAwake<int>
    {
        private Tblanguage tblanguage;
        private Tbtag_func tbtag_func;
        private Tbblock tbblock;
        private Tbchapter tbchapter;
        private Tbuser_avatar tbuser_avatar;
        private Tbuser_variable tbuser_variable;
        private Tbuser_level tbuserLevel;
        private Tbdraw_box tbdraw_box;
        private Tbtag tbtag;
        private Tbconstant tbconstant;
        private Tbchapter_box tbchapter_box;
        public int index;
        public UI kRewardPos;
        private UI kBtn_Display_Root;
        private UI kTxt_Display_Root;
        private UI kTxt_Btn_Display;
        private UI kTxtCondition;
        private UI kTxtDisplay;
        private bool isDisplay;
        private int boxClickedID;

        void InitNode()
        {
            kRewardPos = GetFromReference(UIContainerBoxBar.KRewardPos);
            kBtn_Display_Root = GetFromReference(UIContainerBoxBar.KBtn_Display_Root);
            kTxt_Display_Root = GetFromReference(UIContainerBoxBar.KTxt_Display_Root);
            kTxt_Btn_Display = GetFromReference(UIContainerBoxBar.KTxt_Btn_Display);
            kTxtCondition = GetFromReference(UIContainerBoxBar.KTxtCondition);
            kTxtDisplay = GetFromReference(UIContainerBoxBar.KTxt_Display);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        public async void Initialize(int boxID)
        {
            InitJson();
            InitNode();
            InitCondition(boxID);
            InitRewardInfo(boxID);
            var adaptWidth = await CreateReward(boxID) + this.GetComponent<RectTransform>().Width();
            this.GetComponent<RectTransform>().SetWidth(adaptWidth);

            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui))
            {
                var uis = ui as UIPanel_Main;
                uis.SetAsMaxWidth(adaptWidth + 20);
            }
        }

        public bool DisplayRed()
        {
            return isDisplay;
        }


        private void InitRewardInfo(int boxID)
        {
            isDisplay = false;
            int minNotLockBoxID = ResourcesSingletonOld.Instance.levelInfo.levelBox.minNotLockBoxID;
            if (!ResourcesSingletonOld.Instance.levelInfo.levelBox.boxStateDic.ContainsKey(boxID))
            {
                kBtn_Display_Root.SetActive(false);
                kTxt_Display_Root.SetActive(true);
                kTxtDisplay.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_unfinished").current);
            }
            else
            {
                if (ResourcesSingletonOld.Instance.levelInfo.levelBox.boxStateDic[boxID])
                {
                    SetAsHaved();
                }
                else
                {
                    isDisplay = true;
                    kBtn_Display_Root.SetActive(true);
                    kTxt_Display_Root.SetActive(false);
                    kTxt_Btn_Display.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gain").current);
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(kBtn_Display_Root,
                        () => { onGetButtonClicked(boxID); });
                }
            }
            //if (boxID >= minNotLockBoxID)
            //{
            //    kBtn_Display_Root.SetActive(false);
            //    kTxt_Display_Root.SetActive(true);
            //    kTxtDisplay.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_unfinished").current);
            //}
            //else
            //{
            //    if (ResourcesSingletonOld.Instance.levelInfo.levelBox.boxStateDic.ContainsKey(boxID))
            //    {

            //    }
            //    if (boxID >= minNotGetBoxID)
            //    {
            //        kBtn_Display_Root.SetActive(true);
            //        kTxt_Display_Root.SetActive(false);
            //        kTxt_Btn_Display.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gain").current);
            //        UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(kBtn_Display_Root,
            //            () => { onGetButtonClicked(boxID); });
            //    }
            //    else
            //    {
            //        SetAsHaved();
            //    }
            //}
        }

        private void InitCondition(int boxID)
        {
            string boxDescription = tblanguage.Get(tbchapter_box[boxID].desc).current;
            float ovetTime = tbchapter_box[boxID].overTime;
            kTxtCondition.GetTextMeshPro()
                .SetTMPText(string.Format(boxDescription, (ovetTime / 60).ToString() + tblanguage.Get("time_minute_2").current));
            if (tbchapter_box[boxID].passYn == 1)
            {
                kTxtCondition.GetTextMeshPro().SetTMPText(tblanguage.Get("level_commn_reward").current);
            }
        }

        private void SetAsHaved()
        {
            kBtn_Display_Root.SetActive(false);
            kTxt_Display_Root.SetActive(true);
            kTxtDisplay.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gained").current);
        }

        private void onGetButtonClicked(int boxID)
        {
            boxClickedID = boxID;
            //ResourcesSingletonOld.Instance.levelInfo.levelBox.boxStateDic[boxID] = true;
            WebMessageHandlerOld.Instance.AddHandler(2, 8, OnBoxGetResponce);
            NetWorkManager.Instance.SendMessage(2, 8, new IntValue { Value = boxID });
        }

        private async void OnBoxGetResponce(object sender, WebMessageHandlerOld.Execute e)
        {
            Log.Debug("OnBoxGetResponce", Color.yellow);
            WebMessageHandlerOld.Instance.RemoveHandler(2, 8, OnBoxGetResponce);

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
            //获得资源
          
            ResourcesSingletonOld.Instance.UpdateResourceUI();


            ResourcesSingletonOld.Instance.levelInfo.levelBox.boxStateDic[boxClickedID] = true;
            var boxDic = ResourcesSingletonOld.Instance.levelInfo.levelBox.boxStateDic;

            var keys = boxDic.Keys.ToArray();
            int minNotGetBoxID = ResourcesSingletonOld.Instance.levelInfo.levelBox.minNotLockBoxID,
                oldMinNotGetBoxID = ResourcesSingletonOld.Instance.levelInfo.levelBox.minNotGetBoxID;
            for (int i = 0; i < keys.Length; i++)
            {
                if (!boxDic[keys[i]] && minNotGetBoxID > keys[i])
                {
                    minNotGetBoxID = keys[i];
                }
            }

            if (oldMinNotGetBoxID < minNotGetBoxID)
            {
                ResourcesSingletonOld.Instance.levelInfo.levelBox.minNotGetBoxID = minNotGetBoxID;
            }

            SetAsHaved();
            //需要进行翻页更新
            if (tbchapter_box.Get(oldMinNotGetBoxID).chapterId != tbchapter_box.Get(minNotGetBoxID).chapterId)
            {
                //更新红点和翻页
                if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui))
                {
                    var uiMain = ui as UIPanel_Main;
                    await uiMain.PlayTreasureAnim();
                    uiMain?.DisplayRedRot();
                    uiMain?.UpdateTreasure();
                }
            }


            //ResourcesSingletonOld.Instance.levelInfo.levelBox.boxStateDic.Clear();
            ////拿到最小未解锁的boxID
            //int minNotLockBoxID = 2999;
            ////拿到最小未领取的boxID
            //int minNoGetBoxID = 2999;
            //for (int i = 0; i < roleInfo.ChapterList.Count; i++)
            //{
            //    for (int j = 0; j < roleInfo.ChapterList[i].BoxStatusList.Count; j++)
            //    {
            //        string boxInput = roleInfo.ChapterList[i].BoxStatusList[j];
            //        //  Log.Debug(boxInput,Color.blue);
            //        if (!boxInput.Contains("["))
            //        {
            //            continue;
            //        }

            //        int startIndex = boxInput.IndexOf("[") + 1;
            //        int endIndex = boxInput.IndexOf("]", startIndex);
            //        string result = boxInput.Substring(startIndex, endIndex - startIndex);
            //        Log.Debug(
            //            $"roleInfo.ChapterList[{i}].BoxStatusList[{j}]:{roleInfo.ChapterList[i].BoxStatusList[j]}",
            //            Color.yellow);
            //        var boxArray = result.Split(':');

            //        if (boxArray[2] == "false" ? true : false)
            //        {
            //            // Debug.Log($"{boxArray[0]}");
            //            if (minNoGetBoxID > int.Parse(boxArray[0]))
            //            {
            //                minNoGetBoxID = int.Parse(boxArray[0]);
            //            }
            //        }
            //        if (boxArray[1] == "lock" ? true : false)
            //        {
            //            if (minNotLockBoxID > int.Parse(boxArray[0]))
            //            {
            //                minNotLockBoxID = int.Parse(boxArray[0]);
            //            }
            //        }
            //    }
            //}

            //Log.Debug(
            //    $"minNotLockBoxID：{minNotLockBoxID},minNotGetBoxID:{minNoGetBoxID}", Color.blue);

            //ResourcesSingletonOld.Instance.levelInfo.levelBox.minNotLockBoxID = minNotLockBoxID;
            //ResourcesSingletonOld.Instance.levelInfo.levelBox.minNotGetBoxID = minNoGetBoxID;
            //for (int i = 0; i < roleInfo.ChapterList.Count; i++)
            //{
            //    for (int j = 0; j < roleInfo.ChapterList[i].BoxStatusList.Count; j++)
            //    {
            //        string boxInput = roleInfo.ChapterList[i].BoxStatusList[j];
            //        //  Log.Debug(boxInput,Color.blue);
            //        if (!boxInput.Contains("["))
            //        {
            //            continue;
            //        }

            //        int startIndex = boxInput.IndexOf("[") + 1;
            //        int endIndex = boxInput.IndexOf("]", startIndex);
            //        string result = boxInput.Substring(startIndex, endIndex - startIndex);
            //        Log.Debug(
            //            $"roleInfo.ChapterList[{i}].BoxStatusList[{j}]:{roleInfo.ChapterList[i].BoxStatusList[j]}",
            //            Color.yellow);
            //        var boxArray = result.Split(':');
            //        if (int.Parse(boxArray[0]) < minNotLockBoxID)
            //        {
            //            ResourcesSingletonOld.Instance.levelInfo.levelBox.boxStateDic.Add(int.Parse(boxArray[0]),
            //                boxArray[2] == "false" ? false : true);
            //        }
            //    }
            //}
        }

        private async UniTask<float> CreateReward(int boxID)
        {
            float spacing = kRewardPos.GetComponent<HorizontalLayoutGroup>().spacing;
            var rewardList = kRewardPos.GetList();
            rewardList.Clear();

            var rewardArray = tbchapter_box.Get(boxID).reward;
            UnicornUIHelper.MergeRewardList(rewardArray);
            float width = 0;
            var rewardItems = new List<UICommon_RewardItem>(rewardArray.Count);
            foreach (var reward in rewardArray)
            {
                var ui = await rewardList.CreateWithUITypeAsync(UIType.UICommon_RewardItem, reward, false) as
                    UICommon_RewardItem;
                rewardItems.Add(ui);

                UnicornUIHelper.SetRewardOnClick(reward, ui);
                ui.GetComponent<RectTransform>().SetScale(new float2(1.3f,1.3f));
                width = ui.GetComponent<RectTransform>().Width() * ui.GetComponent<RectTransform>().Scale().x;
            }

            rewardList.Sort(UnicornUIHelper.RewardUIComparer);
            width += spacing;
            //int width=156*0.6f+20
            return width * (rewardArray.Count - 2);
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
        }
    }
}
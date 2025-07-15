//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using Common;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UILevelBox)]
    internal sealed class UILevelBoxEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UILevelBox;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UILevelBox>();
        }
    }

    public partial class UILevelBox : UI, IAwake
    {
        enum DroupItem
        {
            experience = 1,
            dollars,
            gold,
            stamina
        }

        public List<GameObject> rewardItems;

        public void Initialize()
        {
            rewardItems = new List<GameObject>(4);
            WebMessageHandler.Instance.AddHandler(2, 4, GetChapterBoxInfo);
            //GetBoxShow(ResourcesSingleton.Instance.levelInfo.levelBox.minNotGetBoxID);

            this.GetButton(KBackButton)?.OnClick.Add(Close);
            this.GetButton(KGetButton)?.OnClick.Add(onGetButtonClicked);
        }


        private void GetChapterBoxInfo(object sender, WebMessageHandler.Execute e)
        {
            Log.Debug("233333333333333333");
            var roleInfo = new RoleChapters();
            roleInfo.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.yellow);
            }


            //拿到最小未领取的boxID
            int minNoGetBoxID = 2999;
            //拿到最小未解锁的boxID
            int minNotLockBoxID = 2999;
            for (int i = 0; i < roleInfo.ChapterList.Count; i++)
            {
                for (int j = 0; j < roleInfo.ChapterList[i].BoxStatusList.Count; j++)
                {
                    string boxInput = roleInfo.ChapterList[i].BoxStatusList[j];
                    //  Log.Debug(boxInput,Color.blue);
                    if (!boxInput.Contains("["))
                    {
                        continue;
                    }

                    int startIndex = boxInput.IndexOf("[") + 1;
                    int endIndex = boxInput.IndexOf("]", startIndex);
                    string result = boxInput.Substring(startIndex, endIndex - startIndex);
                    Log.Debug(
                        $"roleInfo.ChapterList[{i}].BoxStatusList[{j}]:{roleInfo.ChapterList[i].BoxStatusList[j]}",
                        Color.yellow);
                    var boxArray = result.Split(':');

                    if (boxArray[2] == "false" ? true : false)
                    {
                        // Debug.Log($"{boxArray[0]}");
                        if (minNoGetBoxID > int.Parse(boxArray[0]))
                        {
                            minNoGetBoxID = int.Parse(boxArray[0]);
                        }
                    }

                    if (boxArray[1] == "lock" ? true : false)
                    {
                        if (minNotLockBoxID > int.Parse(boxArray[0]))
                        {
                            minNotLockBoxID = int.Parse(boxArray[0]);
                        }
                    }
                }
            }

            Log.Debug(
                $"boxID:{minNoGetBoxID}", Color.blue);
            //ResourcesSingleton.Instance.levelInfo.levelBox.minNotGetBoxID = minNoGetBoxID;
            ResourcesSingleton.Instance.levelInfo.levelBox.minNotLockBoxID = minNotLockBoxID;
            UIHelper.Create(UIType.UIAwardGet, minNoGetBoxID);
            GetBoxShow(minNoGetBoxID);
        }

        private async void onGetButtonClicked()
        {
            //int lastBoxID = ResourcesSingleton.Instance.levelInfo.levelBox.minNotGetBoxID;
            // Log.Debug($"lastBoxID:{lastBoxID}");
            // int boxLockID= ResourcesSingleton.Instance.levelInfo.levelBox.minNotLockBoxID;
            //NetWorkManager.Instance.SendMessage(2, 8, new IntValue { Value = lastBoxID });
            //NetWorkManager.Instance.SendMessage(2, 4);

            //前端计算下个一宝箱id
            //int minNoGetBoxID = lastBoxID;
            //var boxTable = ConfigManager.Instance.Tables.Tbchapter_box.DataList;
            //for (int i = 0; i < boxTable.Count; i++)
            //{
            //    if (boxTable[i].id == 1003 || i + 1 >= boxTable.Count)
            //    {
            //        Log.Debug("无可领取宝箱 请检查索引", Color.red);
            //        return;
            //    }

            //    if (boxTable[i].id == lastBoxID)
            //    {
            //        minNoGetBoxID = boxTable[i + 1].id;
            //        break;
            //    }
            //}

            //ResourcesSingleton.Instance.levelInfo.levelBox.minNotGetBoxID = minNoGetBoxID;


            //UIHelper.Create(UIType.UIAwardGet, minNoGetBoxID);
            //GetBoxShow(minNoGetBoxID);

            //var boxMap = ConfigManager.Instance.Tables.Tbchapter_box.DataMap;
            //var rewards = boxMap[minNoGetBoxID].reward;

            //JiYuUIHelper.AddReward(rewards, true);


            ResourcesSingleton.Instance.UpdateResourceUI();
        }

        private void GetBoxShow(int nextBoxID)
        {
            Log.Debug($"5555555555555555555555555", Color.green);

            // int minNoGetBoxID = ResourcesSingleton.Instance.levelInfo.levelBox.minNotGetBoxID;
            int minNotLockBoxID = ResourcesSingleton.Instance.levelInfo.levelBox.minNotLockBoxID;
            // Log.Debug($"minNoGetBoxID{minNoGetBoxID}minNotLockBoxID{minNotLockBoxID}",Color.green);
            //通过没领取的boxID设置box界面
            //int boxID = 101;
            UpdateAwardBoxInfo(nextBoxID, minNotLockBoxID);
        }

        private void UpdateAwardBoxInfo(int minNoGetBoxID, int minNotLockBoxID)
        {
            bool isLock = true;
            if (minNoGetBoxID < minNotLockBoxID)
            {
                isLock = false;
            }

            if (this.GetFromReference(KLevelAwardGroup).Children.Count > 0)
            {
                this.GetFromReference(KLevelAwardGroup).RemoveChild("Award");
            }

            if (rewardItems.Count > 0)
            {
                rewardItems.Clear();
            }


            InitBoxPanelText(minNoGetBoxID, isLock);
            InitAwards(minNoGetBoxID);
        }

        //public bool isPass(string status)
        //{
        //    return status == "true" ? true : false;
        //}
        private void InitAwards(int boxID)
        {
            var boxMap = ConfigManager.Instance.Tables.Tbchapter_box.DataMap;
            var rewardArray = boxMap[boxID].reward;

            JiYuUIHelper.SortRewards(rewardArray);
            foreach (var reward in rewardArray)
            {
                float rewardID = reward.x;
                float rewardCount = reward.z;
                InitAwardItem(rewardID, rewardCount);
            }
        }

        private void InitBoxPanelText(int boxID, bool isLock)
        {
            var boxTable = ConfigManager.Instance.Tables.Tbchapter_box.DataList;
            int leftBoxID = -1, rightBoxID = -1;
            for (int i = 0; i < boxTable.Count; i++)
            {
                //最大和最小的单独判断
                if (boxID == 101)
                {
                    rightBoxID = boxID + 1;
                    break;
                }

                if (boxID == 1003)
                {
                    leftBoxID = boxID - 1;
                    break;
                }

                if (boxID == boxTable[i].id)
                {
                    leftBoxID = boxTable[i - 1].id;
                    rightBoxID = boxTable[i + 1].id;
                    break;
                }
            }

            var funcTable = ConfigManager.Instance.Tables.Tbtag_func.DataMap;
            var language = ConfigManager.Instance.Tables.Tblanguage;
            GetFromReference(KTxt_Title).GetTextMeshPro().SetTMPText(language.Get(funcTable[3501].name).current);
            GetFromReference(KAwardTitle).GetTextMeshPro().SetTMPText(language["common_reward_name"].current);

            var boxMap = ConfigManager.Instance.Tables.Tbchapter_box.DataMap;
            string boxDescription = language.Get(boxMap[boxID].desc).current;
            float ovetTime = boxMap[boxID].overTime;

            GetFromReference(KTextAwardDescription).GetTextMeshPro()
                .SetTMPText(boxDescription + (ovetTime / 60).ToString() + language["time_minute_2"].current);

            var conditionCount = GetFromReference(KConditionEffect).GetRectTransform().ChildCount;

            for (int i = 0; i < conditionCount; ++i)
            {
                var go = GetFromReference(KConditionEffect).GetRectTransform().GetChild(i).gameObject;

                if (go.name == "ConditionCenter")
                {
                    var texts = go.GetComponentsInChildren<Text>();
                    foreach (var text in texts)
                    {
                        if (text.name == "TextTime")
                        {
                            text.SetText((boxMap[boxID].overTime / 60).ToString() + language["time_minute_2"].current);
                        }
                        else
                        {
                            text.SetText(language["common_chapter_name"].current + boxMap[boxID].chapterId.ToString());
                        }
                    }
                }
                else if (go.name == "ConditionLeft")
                {
                    go.SetActive(true);
                    if (leftBoxID == -1)
                    {
                        go.SetActive(false);
                        continue;
                    }

                    var texts = go.GetComponentsInChildren<Text>();
                    foreach (var text in texts)
                    {
                        if (text.name == "TextTime")
                        {
                            text.SetText((boxMap[leftBoxID].overTime / 60).ToString() +
                                         language["time_minute_2"].current);
                        }
                        else
                        {
                            text.SetText(language["common_chapter_name"].current +
                                         boxMap[leftBoxID].chapterId.ToString());
                        }
                    }
                }
                else
                {
                    go.SetActive(true);
                    if (rightBoxID == -1)
                    {
                        go.SetActive(false);
                        continue;
                    }

                    var texts = go.GetComponentsInChildren<Text>();
                    foreach (var text in texts)
                    {
                        if (text.name == "TextTime")
                        {
                            text.SetText((boxMap[rightBoxID].overTime / 60).ToString() +
                                         language["time_minute_2"].current);
                        }
                        else
                        {
                            text.SetText(language["common_chapter_name"].current +
                                         boxMap[rightBoxID].chapterId.ToString());
                        }
                    }
                }
            }


            if (!isLock)
            {
                GetFromReference(KGetButton)?.SetActive(true);
                GetFromReference(KNoGetText)?.SetActive(false);
            }
            else if (isLock)
            {
                GetFromReference(KGetButton)?.SetActive(false);
                GetFromReference(KNoGetText)?.SetActive(true);
                GetFromReference(KNoGetText).GetComponent<Text>().SetText(boxDescription + (ovetTime / 60).ToString() +
                                                                          language["time_minute_2"].current +
                                                                          language["chapter_box_desc_end"].current);
            }
        }

        protected override void OnClose()
        {
            //WebMessageHandler.Instance.RemoveHandler(2, 4, NoGetBoxIDResponce);
            WebMessageHandler.Instance.RemoveHandler(2, 4, GetChapterBoxInfo);


            rewardItems.Clear();

            //this.GetParent<UIPanel_Battle>().UpdateLevelShow();

            //if (this.GetFromReference(KLevelAwardGroup).Children.Count > 0)
            //{
            //    this.GetFromReference(KLevelAwardGroup).RemoveChild("Award");
            //}
            // Log.Debug(this.GetFromReference(KLevelAwardGroup).Children.Count.ToString(), Color.blue);
            //if(awards.Capacity > 0)
            //{
            //    Debug.Log(awards.Capacity);
            //}
            //awards.Clear();
            //if (awards.Count > 0)
            //{
            //    foreach (var award in awards)
            //    {
            //        GameObject.Destroy(award);
            //    }
            //}

            base.OnClose();
        }

        bool JudegeCanGetBox()
        {
            return false;
        }

        void InitAwardItem(float rewardID, float rewardCount)
        {
            var itemMap = ConfigManager.Instance.Tables.Tbuser_variable.DataMap;
            var parentUI = GetFromReference(KLevelAwardGroup);
            var item = UIHelper.Create(UIType.UIAward, parentUI,
                GetFromReference(KLevelAwardGroup).GetComponent<Transform>());
            //var item= GameObject.Instantiate(awardGo, GetFromReference(KLevelAwardGroup).GetRectTransform().Get()); 
            Image awardImage = item.GetComponent<Transform>().Find("AwardTypeImage").GetComponent<Image>();
            Text awardCount = item.GetComponent<Transform>().Find("AwardNumText").GetComponent<Text>();
            awardCount.text = rewardCount.ToString();
            int itemID = (int)rewardID;
            string spritePath = itemMap[itemID].icon.ToString();

            awardImage.sprite = ResourcesManager.LoadAsset<Sprite>(spritePath);
            rewardItems.Add(item.GameObject);
        }
    }
}
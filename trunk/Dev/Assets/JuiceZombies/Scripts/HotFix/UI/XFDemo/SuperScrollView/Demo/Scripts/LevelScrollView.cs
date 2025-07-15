using System.Collections.Generic;
using System.Text.RegularExpressions;
using HotFix_UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XFramework;
using static XFramework.UILevelScrollView;

namespace SuperScrollView
{
    public class LevelScrollScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 100;
        DataSourceMgr<LevelItemData> mDataSourceMgr;
        public GameObject mTitle;
        public GameObject mDescription;
        public GameObject mSelectRoot;
        public ScrollRect mScrollRect;
        TMP_Text mTitleText;
        TMP_Text mDescriptionText;
        Button selectButton;
        TMP_Text unselectText;
        Dictionary<string, cfg.config.language> languageDic;

        List<cfg.config.chapter> levelList;
        int maxLcokItemID = 0;
        public int chapterID = -1;

        public static event ChangeMissionSpriteHandle SelectChangeEvent;

        // Use this for initialization
        void Start()
        {
            //WebMessageHandler.Instance.AddHandler(2, 4, LevelLockResponse);
            ////请求一次关卡数据
            //NetWorkManager.Instance.SendMessage(2, 4);
            //maxLcokItemID = ResourcesSingleton.Instance.levelInfo.maxLockChapterID;

            Log.Debug($"maxLcokItemID:{maxLcokItemID}~~~~~~~~~~~~", Color.green);

            mDataSourceMgr = new DataSourceMgr<LevelItemData>(mTotalDataCount);
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount, OnGetItemByIndex);
            mTitleText = mTitle.GetComponentInChildren<TMP_Text>();
            mDescriptionText = mDescription.GetComponentInChildren<TMP_Text>();
            selectButton = mSelectRoot.GetComponentInChildren<Button>();
            unselectText = mSelectRoot.transform.Find("UnSelectText").GetComponent<TMP_Text>();

            selectButton.gameObject.SetActive(false);
            unselectText.gameObject.SetActive(false);
            selectButton.onClick.Add(OnSelectButtonClick);
            languageDic = ConfigManager.Instance.Tables.Tblanguage.DataMap;
            levelList = ConfigManager.Instance.Tables.Tbchapter.DataList;
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= mDataSourceMgr.TotalItemCount)
            {
                return null;
            }

            LevelItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }


            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            if (item.ItemId == 1 || item.ItemId == 2 || item.ItemId == 3)
            {
                item.gameObject.SetActive(false);
            }

            LevelItem itemScript = item.GetComponent<LevelItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }

            itemScript.SetLevelItemData(itemData, index);
            itemScript.ShowLock(!isLock(index - 2));
            return item;
        }

        //拿到一个已经解锁的levelID
        bool isLock(int currentLevelID)
        {
            var isStandAlone = XFramework.Common.Instance.Get<Global>().isStandAlone;
            if (isStandAlone)
            {
                return true;
            }


            int judgeID;
            if (maxLcokItemID == 0)
            {
                judgeID = 1;
            }
            else
            {
                judgeID = maxLcokItemID;
            }

            if (judgeID >= currentLevelID)
                return true;


            return false;
        }

        string ReplaceBetweenBraces(string input, string replacement)
        {
            Regex regex = new Regex("{(.*?)}");
            MatchCollection matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                string content = match.Groups[1].Value;
                input = input.Replace(match.Value, replacement);
            }

            return input;
        }

        void LateUpdate()
        {
            mLoopListView.UpdateAllShownItemSnapData();
            float v = Mathf.Abs(mScrollRect.velocity.x);
            if (v > 0)
            {
                if (mTitle.gameObject.activeSelf)
                {
                    mTitle.gameObject.SetActive(false);
                }

                if (mDescription.gameObject.activeSelf)
                {
                    mDescription.gameObject.SetActive(false);
                }

                if (mSelectRoot.gameObject.activeSelf)
                {
                    mSelectRoot.gameObject.SetActive(false);
                }
            }

            int count = mLoopListView.ShownItemCount;
            for (int i = 0; i < count; ++i)
            {
                LoopListViewItem2 item = mLoopListView.GetShownItemByIndex(i);
                if (item.ItemIndex == 0 || item.ItemIndex == 1 || item.ItemIndex == 2)
                {
                    item.gameObject.SetActive(false);
                }

                if (item.ItemIndex > 103)
                {
                    item.gameObject.SetActive(false);
                }

                LevelItem itemScript = item.GetComponent<LevelItem>();
                float scale = 1 - Mathf.Abs(item.DistanceWithViewPortSnapCenter) / 800f;


                scale = Mathf.Clamp(scale, 0.4f, 1);
                //拿到处于中心位置的index
                if (Mathf.Abs(item.DistanceWithViewPortSnapCenter - 0) < 3)
                {
                    //拿到当前的关卡index
                    chapterID = item.ItemIndex - 2;
                    foreach (var level in levelList)
                    {
                        if (level.num == chapterID)
                        {
                            mDescriptionText.SetTMPText(languageDic[level.desc].current);
                            mTitleText.SetTMPText(chapterID.ToString() + "." + languageDic[level.name].current);
                            if (isLock(level.num))
                            {
                                unselectText.gameObject.SetActive(false);
                                selectButton.gameObject.SetActive(true);
                                itemScript.ShowLock(false);
                            }

                            if (!isLock(level.num))
                            {
                                unselectText.gameObject.SetActive(true);
                                selectButton.gameObject.SetActive(false);
                                string temp = languageDic["chapter_unlock_desc"].current;
                                int displayID = level.num - 1;
                                unselectText.SetText(ReplaceBetweenBraces(temp, displayID.ToString()));
                                itemScript.ShowLock(true);
                            }

                            break;
                        }

                        mTitleText.SetTMPText(chapterID.ToString() + "." + "表没配");
                    }

                    if (v == 0)
                    {
                        if (!mTitle.gameObject.activeSelf)
                        {
                            mTitle.gameObject.SetActive(true);
                        }

                        if (!mDescription.gameObject.activeSelf)
                        {
                            mDescription.gameObject.SetActive(true);
                        }

                        if (!mSelectRoot.gameObject.activeSelf)
                        {
                            mSelectRoot.gameObject.SetActive(true);
                        }
                        //Debug.Log("找到中心点");
                    }
                }

                //  itemScript.mContentRootObj.GetComponent<CanvasGroup>().alpha = scale;
                //加个NaN判断
                if (!float.IsNaN(scale))
                {
                    itemScript.mContentRootObj.transform.localScale = new Vector3(scale, scale, 1);
                }
            }
        }

        private void OnSelectButtonClick()
        {
            Debug.Log(chapterID);
            SelectChangeEvent(chapterID);
        }

        private void OnDestroy()
        {
            //WebMessageHandler.Instance.RemoveHandler(2, 4, LevelLockResponse);
        }
    }
}
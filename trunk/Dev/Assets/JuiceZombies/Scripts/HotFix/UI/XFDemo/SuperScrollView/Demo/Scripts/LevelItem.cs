using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;
using XFramework;

namespace SuperScrollView
{
    public class LevelItem : MonoBehaviour
    {
        public Image mIcon;
        public Image mLock;
        public GameObject mContentRootObj;
        LevelItemData mLevelItemData;
        bool isActive;

        public void Init()
        {
            //ClickEventListener listener = ClickEventListener.Get(mStarIcon.gameObject);
            //listener.SetClickEventHandler(OnStarClicked);
        }

        public void ShowLock(bool isLock)
        {
            if (!isActive)
            {
                mLock.gameObject.SetActive(isActive);
            }
            else
            {
                mLock.gameObject.SetActive(isLock);
            }
        }

        public void SetLevelItemData(LevelItemData itemData, int itemIndex)
        {
            mLock.sprite = ResourcesManager.LoadAsset<Sprite>("MainUIBottom_Icon_Lock");
            mLevelItemData = itemData;
            var chapterTable = ConfigManager.Instance.Tables.Tbchapter.DataMap;
            var chapterID = itemIndex - 2;
            if (chapterID <= 0)
            {
                //mIcon.sprite = ResourcesManager.LoadAsset<Sprite>(chapterTable[1].icon);
            }
            else if (chapterID > 99)
            {
                isActive = false;
                //mIcon.sprite = ResourcesManager.LoadAsset<Sprite>(chapterTable[1].icon);
                mIcon.SetAlpha(0f);
            }
            else
            {
                isActive = true;
                //mIcon.sprite = ResourcesManager.LoadAsset<Sprite>(chapterTable[chapterID].icon);
            }
        }
    }
}
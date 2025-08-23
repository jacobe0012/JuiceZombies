using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class BaseHorizontalItem : MonoBehaviour
    {
        private static int uid;
        private string struid = "000";
        public Text mNameText;
        public Image mIcon;
        public Image mStarIcon;
        public Text mStarCount;
        public Text mDesc;
        public Color32 mRedStarColor = new Color32(236, 217, 103, 255);
        public Color32 mGrayStarColor = new Color32(215, 215, 215, 255);

        int mItemDataIndex = -1;
        ItemData mItemData;

        public static int equipDataIndex = 0;

        public void Init()
        {
            //WebMessageHandlerOld.Instance.AddHandler(9, 5, OnInitEquipmentResponse);


            //从缓存中拿取数据进行排序
            //uid =uid+Random.Range(1,5);
            EquipItemBtnTest btnTest = this.GetComponent<EquipItemBtnTest>();
            //给一个随机UID
            btnTest.UID = System.Convert.ToInt32(struid + uid.ToString());
            //btnTest.InitTableAsync(Random.Range(18, 47));
            btnTest.InitEquipMent();
        }


        public void Init(List<GameEquip> gameEquipments)
        {
            EquipItemBtnTest btnTest = this.GetComponent<EquipItemBtnTest>();


            if (equipDataIndex >= gameEquipments.Count) return;
            Debug.Log(equipDataIndex + "equipDataIndex" + gameEquipments[equipDataIndex].EquipId +
                      "gameEquipments[equipDataIndex].EquipId");

            //如果是通用材料暂时不显示
            if (gameEquipments[equipDataIndex].EquipId % 100 == 0)
            {
                equipDataIndex++;
                return;
            }

            btnTest.InitTableAsync(gameEquipments[equipDataIndex].EquipId, gameEquipments[equipDataIndex].Quality,
                gameEquipments[equipDataIndex].PartId, gameEquipments[equipDataIndex]);
            btnTest.InitEquipMent();
            equipDataIndex++;
        }


        public void SetStarCount(int count)
        {
            mStarCount.text = count.ToString();
            if (count == 0)
            {
                mStarIcon.color = mGrayStarColor;
            }
            else
            {
                mStarIcon.color = mRedStarColor;
            }
        }

        public void SetItemData(ItemData itemData, int itemIndex)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            //mNameText.text = itemData.mName;
            //mDesc.text = itemData.mDesc;
            //mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
            //SetStarCount(itemData.mStarCount);
        }
    }
}
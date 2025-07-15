using System.Collections.Generic;
using Common;
using UnityEngine;

namespace SuperScrollView
{
    public class BaseHorizontalItemList : MonoBehaviour
    {
        public List<BaseHorizontalItem> mItemList;

        public void Init()
        {
            foreach (BaseHorizontalItem item in mItemList)
            {
                item.Init();
            }
        }


        public void Init(List<GameEquip> gameEquipments)
        {
            foreach (BaseHorizontalItem item in mItemList)
            {
                item.Init(gameEquipments);
            }
        }
    }
}
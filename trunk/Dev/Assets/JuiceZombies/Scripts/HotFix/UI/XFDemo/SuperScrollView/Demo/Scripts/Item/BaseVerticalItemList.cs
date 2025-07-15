using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class BaseVerticalItemList : MonoBehaviour
    {
        public List<BaseVerticalItem> mItemList;

        public void Init()
        {
            foreach (BaseVerticalItem item in mItemList)
            {
                item.Init();
            }
        }
    }
}
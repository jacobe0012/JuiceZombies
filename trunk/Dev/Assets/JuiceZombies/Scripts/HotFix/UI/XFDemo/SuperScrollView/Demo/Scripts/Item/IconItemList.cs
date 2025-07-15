using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class IconItemList : MonoBehaviour
    {
        public List<IconItem> mItemList;

        public void Init()
        {
            foreach (IconItem item in mItemList)
            {
                item.Init();
            }
        }
    }
}
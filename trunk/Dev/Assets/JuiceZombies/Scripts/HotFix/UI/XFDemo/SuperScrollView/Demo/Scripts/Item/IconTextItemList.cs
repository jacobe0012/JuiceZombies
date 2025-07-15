using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class IconTextItemList : MonoBehaviour
    {
        public List<IconTextItem> mItemList;

        public void Init()
        {
            foreach (IconTextItem item in mItemList)
            {
                item.Init();
            }
        }
    }
}
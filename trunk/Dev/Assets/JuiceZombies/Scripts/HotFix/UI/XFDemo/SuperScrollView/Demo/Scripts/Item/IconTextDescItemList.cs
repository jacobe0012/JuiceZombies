using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class IconTextDescItemList : MonoBehaviour
    {
        public List<IconTextDescItem> mItemList;

        public void Init()
        {
            foreach (IconTextDescItem item in mItemList)
            {
                item.Init();
            }
        }
    }
}
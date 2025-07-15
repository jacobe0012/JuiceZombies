using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class ToggleRowColItemList : MonoBehaviour
    {
        public List<ToggleRowColItem> mItemList;

        public void Init()
        {
            foreach (ToggleRowColItem item in mItemList)
            {
                item.Init();
            }
        }
    }
}
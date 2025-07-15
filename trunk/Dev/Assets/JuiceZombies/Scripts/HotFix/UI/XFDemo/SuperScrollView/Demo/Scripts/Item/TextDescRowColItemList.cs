using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class TextDescRowColItemList : MonoBehaviour
    {
        public List<TextDescRowColItem> mItemList;

        public void Init()
        {
            foreach (TextDescRowColItem item in mItemList)
            {
                item.Init();
            }
        }
    }
}
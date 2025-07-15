using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class SliderComplexItemList : MonoBehaviour
    {
        public List<SliderComplexItem> mItemList;

        public void Init()
        {
            foreach (SliderComplexItem item in mItemList)
            {
                item.Init();
            }
        }
    }
}
using UnityEngine;

namespace SuperScrollView
{
    public class BaseHorizontalToggleItemList : MonoBehaviour
    {
        public BaseHorizontalToggleItem[] mItemList;

        public void Init()
        {
            foreach (BaseHorizontalToggleItem item in mItemList)
            {
                item.Init();
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class InputFieldItemList : MonoBehaviour
    {
        public List<InputFieldItem> mItemList;

        public void Init()
        {
            foreach (InputFieldItem item in mItemList)
            {
                item.Init();
            }
        }
    }
}
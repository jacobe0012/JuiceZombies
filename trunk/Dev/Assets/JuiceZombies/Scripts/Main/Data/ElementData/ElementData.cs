//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-19 11:51:46
//---------------------------------------------------------------------

using System;
using Unity.Entities;

namespace Main
{
    [Serializable]
    public struct ElementData : IComponentData
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public int type;

        /// <summary>
        /// 元素id
        /// </summary>
        public int id;
    }
}
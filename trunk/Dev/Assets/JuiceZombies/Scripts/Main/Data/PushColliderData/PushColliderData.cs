//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: 2023-07-20 16:19:12
//---------------------------------------------------------------------


using Unity.Entities;

namespace Main
{
    public struct PushColliderData : IComponentData
    {
        public int tick;
        public bool toBeSmall;
        public float initScale;
        public float targetScale;
    }
}
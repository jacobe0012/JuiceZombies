//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: 2024-05-09 19:11:18
//---------------------------------------------------------------------


using Unity.Entities;
using Unity.Rendering;

namespace Main
{
    [MaterialProperty("_StartTime")]
    public struct DamageNumberStartTimeComponent : IComponentData
    {
        public float time;
    }
}
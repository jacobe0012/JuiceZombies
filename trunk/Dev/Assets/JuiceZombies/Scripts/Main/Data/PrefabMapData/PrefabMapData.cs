//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: 2023-07-20 16:19:12
//---------------------------------------------------------------------


using Unity.Collections;
using Unity.Entities;

namespace Main
{
    public struct PrefabMapData : IComponentData
    {
        public NativeHashMap<FixedString128Bytes, Entity> prefabHashMap;
    }
}
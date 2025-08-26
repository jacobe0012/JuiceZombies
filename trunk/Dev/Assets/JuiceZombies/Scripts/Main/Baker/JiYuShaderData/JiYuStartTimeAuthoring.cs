//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2024-03-12 14:27:55
//---------------------------------------------------------------------

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    public class JiYuStartTimeAuthoring : MonoBehaviour
    {
        public class JiYuStartTimeAuthoringBaker : Baker<JiYuStartTimeAuthoring>
        {
            public override void Bake(JiYuStartTimeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuStartTime
                {
                    value = 0
                });
            }
        }
    }

    [MaterialProperty("_JiYuStartTime")]
    public struct JiYuStartTime : IComponentData
    {
        public float value;
    }
}
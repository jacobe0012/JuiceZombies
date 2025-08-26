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
    public class JiYuOverlayColorAuthoring : MonoBehaviour
    {
        public class JiYuOverlayColorAuthoringBaker : Baker<JiYuOverlayColorAuthoring>
        {
            public override void Bake(JiYuOverlayColorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuOverlayColor
                {
                    value = new float4(1, 1, 1, 1)
                });
            }
        }
    }

    [MaterialProperty("_OverlayColor")]
    public struct JiYuOverlayColor : IComponentData
    {
        public float4 value;
    }
}
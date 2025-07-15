//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: 2024-03-12 14:27:55
//---------------------------------------------------------------------

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    public class JiYuScaleXYAuthoring : MonoBehaviour
    {
        public class JiYuScaleXYAuthoringBaker : Baker<JiYuScaleXYAuthoring>
        {
            public override void Bake(JiYuScaleXYAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuScaleXY
                {
                    value = 1
                });
            }
        }
    }

    [MaterialProperty("_JiYuScaleXY")]
    public struct JiYuScaleXY : IComponentData
    {
        public float4 value;
    }
}
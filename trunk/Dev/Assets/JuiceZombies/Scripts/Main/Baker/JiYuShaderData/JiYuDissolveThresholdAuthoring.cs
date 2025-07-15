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
    public class JiYuDissolveThresholdAuthoring : MonoBehaviour
    {
        public class JiYuDissolveThresholdAuthoringBaker : Baker<JiYuDissolveThresholdAuthoring>
        {
            public override void Bake(JiYuDissolveThresholdAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuDissolveThreshold
                {
                    value = 0
                });
            }
        }
    }

    [MaterialProperty("_JiYuDissolveThreshold")]
    public struct JiYuDissolveThreshold : IComponentData
    {
        public float value;
    }
}
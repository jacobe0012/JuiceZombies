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
    public class JiYuAnimIndexAuthoring : MonoBehaviour
    {
        public class JiYuAnimIndexAuthoringBaker : Baker<JiYuAnimIndexAuthoring>
        {
            public override void Bake(JiYuAnimIndexAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuAnimIndex
                {
                    value = 1
                });
            }
        }
    }

    [MaterialProperty("_JiYuAnimIndex")]
    public struct JiYuAnimIndex : IComponentData
    {
        public int value;
    }
}
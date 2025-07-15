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
    public class JiYuMainTexSTAuthoring : MonoBehaviour
    {
        public class JiYuMainTexSTAuthoringBaker : Baker<JiYuMainTexSTAuthoring>
        {
            public override void Bake(JiYuMainTexSTAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new JiYuMainTexST
                {
                    value = new float4(1, 1, 0, 0)
                });
            }
        }
    }

    [MaterialProperty("_MainTex_ST")]
    public struct JiYuMainTexST : IComponentData
    {
        public float4 value;
    }
}
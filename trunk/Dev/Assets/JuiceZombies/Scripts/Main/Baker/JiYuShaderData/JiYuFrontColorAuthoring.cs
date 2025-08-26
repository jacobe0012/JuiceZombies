//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2024-03-12 14:27:55
//---------------------------------------------------------------------

using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    [DisallowMultipleComponent]
    public class JiYuFrontColorAuthoring : MonoBehaviour
    {
        public class JiYuFrontColorAuthoringBaker : Baker<JiYuFrontColorAuthoring>
        {
            public override void Bake(JiYuFrontColorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuFrontColor
                {
                    value = new float4(1, 1, 1, 1)
                });
                AddComponent(entity, new JiYuFrontColor1
                {
                    value = new float4(1, 1, 1, 1)
                });
                AddComponent(entity, new JiYuFrontColor2
                {
                    value = new float4(1, 1, 1, 1)
                });
            }
        }
    }

    [MaterialProperty("_JiYuFrontColor1")]
    public struct JiYuFrontColor1 : IComponentData
    {
        public float4 value;
    }

    [MaterialProperty("_JiYuFrontColor2")]
    public struct JiYuFrontColor2 : IComponentData
    {
        public float4 value;
    }

    [MaterialProperty("_JiYuFrontColor")]
    public struct JiYuFrontColor : IComponentData
    {
        public float4 value;
    }
}
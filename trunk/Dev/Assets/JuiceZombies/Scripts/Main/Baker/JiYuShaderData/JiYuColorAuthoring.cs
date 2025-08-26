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
    public class JiYuColorAuthoring : MonoBehaviour
    {
        public class JiYuColorAuthoringBaker : Baker<JiYuColorAuthoring>
        {
            public override void Bake(JiYuColorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuColor
                {
                    value = new float4(1)
                });
            }
        }
    }

    [MaterialProperty("_Color")]
    public struct JiYuColor : IComponentData
    {
        public float4 value;
    }

    public struct JiYuColorCommand : IComponentData
    {
        //0-  1+
        public int type;
        public float curDuration;
    }
    
    [MaterialProperty("_Color")]
    public struct JiYuAlphaTo0 : IComponentData
    {
        public float4 value;
    }
}
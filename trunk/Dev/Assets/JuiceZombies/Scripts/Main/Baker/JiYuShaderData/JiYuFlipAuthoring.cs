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
    public class JiYuFlipAuthoring : MonoBehaviour
    {
        // [SerializeField] public JiYuLayer layer;
        // [SerializeField] public int sortIndex;

        [SerializeField] public int2 flip;

        public class JiYuFlipAuthoringBaker : Baker<JiYuFlipAuthoring>
        {
            public override void Bake(JiYuFlipAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuFlip
                {
                    value = authoring.flip
                });
            }
        }
    }

    [MaterialProperty("_JiYuFlip")]
    public struct JiYuFlip : IComponentData
    {
        public int2 value;
    }

    public struct FlipData : IComponentData
    {
        public int2 value;
    }
}
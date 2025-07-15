using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    public class BulletTagAuthoring : MonoBehaviour
    {
        class BulletTagAuthoringBaker : Baker<BulletTagAuthoring>
        {
            public override void Bake(BulletTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<BulletData>(entity);
                AddComponent(entity, new TargetData
                {
                    tick = 0
                });
                AddComponent(entity, new JiYuColor
                {
                    value = new float4(1,1,1,0)
                });
            }
        }
    }

    public struct BulletData : IComponentData
    {
        public Entity caster;
    }
}
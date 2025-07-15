using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class ObstacleTagAuthoring : MonoBehaviour
    {
        class ObstacleTagBaker : Baker<ObstacleTagAuthoring>
        {
            public override void Bake(ObstacleTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<ObstacleTag>(entity);
                AddComponent<PushColliderData>(entity);
                AddComponent<BulletSonData>(entity);
                AddComponent(entity, new TargetData
                {
                    tick = 0,
                    BelongsTo = (uint)BuffHelper.TargetEnum.Obstacle,
                    AttackWith = 0
                });
                AddComponent<LinkedEntityGroup>(entity);
            }
        }
    }
}
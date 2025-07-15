using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class AreaTagAuthoring : MonoBehaviour
    {
        class AreaTagBaker : Baker<AreaTagAuthoring>
        {
            public override void Bake(AreaTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<AreaTag>(entity);
                AddComponent(entity, new TargetData
                {
                    tick = 0,
                    BelongsTo = (uint)BuffHelper.TargetEnum.Area,
                    AttackWith = 0
                });
                // AddComponent(entity, new SkillAttackData
                // {
                //     data = new SkillAttack_9999
                //     {
                //         duration = 9999f,
                //     }.ToSkillAttack()
                // });
            }
        }
    }
}
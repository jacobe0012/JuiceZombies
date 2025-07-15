using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class SkillTagAuthoring : MonoBehaviour
    {
        class SkillTagAuthoringBaker : Baker<SkillTagAuthoring>
        {
            public override void Bake(SkillTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<SkillTag>(entity);
                AddComponent(entity, new TargetData
                {
                    tick = 0
                });
            }
        }
    }

    public struct SkillTag : IComponentData
    {
    }
}
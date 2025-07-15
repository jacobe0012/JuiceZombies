using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class SkillAttackDataAuthoring : MonoBehaviour
    {
        // public int id;
        // public float duration;

        class SkillAttackDataBaker : Baker<SkillAttackDataAuthoring>
        {
            public override void Bake(SkillAttackDataAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new SkillAttackData
                {
                });
            }
        }
    }
}
//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-09-14 15:48:52
//---------------------------------------------------------------------

using Unity.Entities;
using UnityEngine;

namespace Main
{
    //伤害实体附带Buff的动态数组
    public class SkillHitEffectBufferAuthoring : MonoBehaviour
    {
        public class SkillHitEffectBufferBaker : Baker<SkillHitEffectBufferAuthoring>
        {
            public override void Bake(SkillHitEffectBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var buffer = AddBuffer<SkillHitEffectBuffer>(entity);
                AddBuffer<LinkedEntityGroup>(entity);
                AddComponent<ElementData>(entity);
            }
        }
    }
}
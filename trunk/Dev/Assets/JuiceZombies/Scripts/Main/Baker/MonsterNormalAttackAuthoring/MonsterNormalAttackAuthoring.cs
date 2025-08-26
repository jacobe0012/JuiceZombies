//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2023-08-10 16:56:00
//---------------------------------------------------------------------

using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class MonsterNormalAttackAuthoring : MonoBehaviour
    {
        public class MonsterNormalAttackBaker : Baker<MonsterNormalAttackAuthoring>
        {
            public override void Bake(MonsterNormalAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<MonsterGeneralAttackData>(entity);
            }
        }
    }
}
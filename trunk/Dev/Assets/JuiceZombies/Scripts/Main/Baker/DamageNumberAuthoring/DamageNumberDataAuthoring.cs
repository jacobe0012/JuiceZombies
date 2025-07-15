//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: 2024-05-10 11:19:42
//---------------------------------------------------------------------

using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class DamageNumberDataAuthoring : MonoBehaviour
    {
        public class DamageNumberDataAuthoringBaker : Baker<DamageNumberDataAuthoring>
        {
            public override void Bake(DamageNumberDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new DamageNumberStartTimeComponent
                {
                    time = 0
                });
                AddComponent<DamageUnManagedData>(entity);
            }
        }
    }
}
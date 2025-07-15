using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class EntityGroupDataAuthoring : MonoBehaviour
    {
        class EntityGroupDataBaker : Baker<EntityGroupDataAuthoring>
        {
            public override void Bake(EntityGroupDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntityGroupData
                {
                });
            }
        }
    }

    public struct EntityGroupData : IComponentData
    {
        public Entity renderingEntity;
        public Entity weaponEntity;
    }

    // public struct RenderingEntityTag : IComponentData
    // {
    // }
}
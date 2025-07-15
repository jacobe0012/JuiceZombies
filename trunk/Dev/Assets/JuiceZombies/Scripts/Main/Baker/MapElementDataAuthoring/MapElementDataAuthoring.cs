using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class MapElementDataAuthoring : MonoBehaviour
    {
        class MapElementDataBaker : Baker<MapElementDataAuthoring>
        {
            public override void Bake(MapElementDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MapElementData
                {
                });
                AddBuffer<Buff>(entity);
            }
        }
    }
}
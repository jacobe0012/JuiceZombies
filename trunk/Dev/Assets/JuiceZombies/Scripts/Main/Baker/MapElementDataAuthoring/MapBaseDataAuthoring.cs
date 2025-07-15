using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class MapBaseDataAuthoring : MonoBehaviour
    {
        class MapBaseDataBaker : Baker<MapBaseDataAuthoring>
        {
            public override void Bake(MapBaseDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MapBaseData()
                {
                });
            }
        }
    }
}
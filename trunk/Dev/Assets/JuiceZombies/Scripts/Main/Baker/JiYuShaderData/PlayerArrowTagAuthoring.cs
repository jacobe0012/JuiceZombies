using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    public class PlayerArrowTagAuthoring : MonoBehaviour
    {
        class PlayerArrowTagAuthoringBaker : Baker<PlayerArrowTagAuthoring>
        {
            public override void Bake(PlayerArrowTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new PlayerArrowTag());
            }
        }
    }

    public struct PlayerArrowTag : IComponentData
    {
    }
}
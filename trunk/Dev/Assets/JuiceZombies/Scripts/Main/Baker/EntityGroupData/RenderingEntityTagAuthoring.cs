using Unity.Entities;
using UnityEngine;

namespace Main
{
    [DisallowMultipleComponent]
    public class RenderingEntityTagAuthoring : MonoBehaviour
    {
        class RenderingEntityTagBaker : Baker<RenderingEntityTagAuthoring>
        {
            public override void Bake(RenderingEntityTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RenderingEntityTag
                {
                });
            }
        }
    }


    public struct RenderingEntityTag : IComponentData
    {
    }
}
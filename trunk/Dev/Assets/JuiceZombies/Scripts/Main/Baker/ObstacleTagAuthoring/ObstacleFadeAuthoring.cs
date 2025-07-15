using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    public class ObstacleFadeAuthoring : MonoBehaviour
    {
        public GameObject parent;

        class ObstacleFadeBaker : Baker<ObstacleFadeAuthoring>
        {
            public override void Bake(ObstacleFadeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ObstacleFadeData
                {
                    Parent = GetEntity(authoring.parent)
                });

                //AddComponent<Parent>(entity);
            }
        }
    }


    public struct ObstacleFadeData : IComponentData
    {
        public Entity Parent;
    }
}
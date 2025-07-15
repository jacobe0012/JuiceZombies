using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    public class GetHitAnimDataAuthoring : MonoBehaviour
    {
        class GetHitAnimDataBaker : Baker<GetHitAnimDataAuthoring>
        {
            public override void Bake(GetHitAnimDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new GetHitAnimData
                {
                });
                // var scale = authoring.gameObject.transform.localScale;
                // scale.x += 0.1f;
                // authoring.gameObject.transform.localScale = scale;
            }
        }
    }

    public struct GetHitAnimData : IComponentData
    {
        public float duration;
        public int tick;
    }

    public struct GetHitAnimObstacleData : IComponentData
    {
        public float duration;
        public int tick;
        public int type;
        public LocalTransform orginalTransform;
      
    }
}
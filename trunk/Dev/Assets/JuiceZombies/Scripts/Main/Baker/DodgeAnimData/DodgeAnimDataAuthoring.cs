using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class DodgeAnimDataAuthoring : MonoBehaviour
    {
        class DodgeAnimDataAuthoringBaker : Baker<DodgeAnimDataAuthoring>
        {
            public override void Bake(DodgeAnimDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DodgeAnimData
                {
                });
                // var scale = authoring.gameObject.transform.localScale;
                // scale.x += 0.1f;
                // authoring.gameObject.transform.localScale = scale;
            }
        }
    }

    public struct DodgeAnimData : IComponentData
    {
        public float duration;
        public int tick;
    }
}
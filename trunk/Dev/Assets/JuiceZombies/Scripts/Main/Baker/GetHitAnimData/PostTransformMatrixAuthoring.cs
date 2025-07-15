using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    public class PostTransformMatrixAuthoring : MonoBehaviour
    {
        class PostTransformMatrixBaker : Baker<PostTransformMatrixAuthoring>
        {
            public override void Bake(PostTransformMatrixAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var scale = authoring.gameObject.transform.localScale;
                scale.x += 0.001f;
                AddComponent(entity, new PostTransformMatrix()
                {
                    Value = float4x4.Scale(scale)
                });
            }
        }
    }
}
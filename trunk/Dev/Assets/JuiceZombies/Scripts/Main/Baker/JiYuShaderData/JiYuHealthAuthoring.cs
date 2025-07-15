using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    public class JiYuHealthAuthoring : MonoBehaviour
    {
        class JiYuHealthAuthoringBaker : Baker<JiYuHealthAuthoring>
        {
            public override void Bake(JiYuHealthAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuHealth
                {
                    value = 1
                });
            }
        }
    }

    [MaterialProperty("_JiYuHealth")]
    public struct JiYuHealth : IComponentData
    {
        public float value;
    }
}
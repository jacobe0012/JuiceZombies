using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;


namespace Main
{
    public class JiYuNumberAuthoring : MonoBehaviour
    {
        public long number;

        public class JiYuNumberAuthoringBaker : Baker<JiYuNumberAuthoring>
        {
            public override void Bake(JiYuNumberAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var damage = new float4x4
                {
                    c0 = UnityHelper.FormatDamage(authoring.number).c0,
                    c1 = UnityHelper.FormatDamage(authoring.number).c1,
                    c2 = default,
                    c3 = default
                };
                AddComponent(entity, new JiYuNumber
                {
                    value = damage
                });
                //Debug.Log(damage);
            }
        }
    }

    [MaterialProperty("_JiYuNumber")]
    public struct JiYuNumber : IComponentData
    {
        public float4x4 value;
    }
}
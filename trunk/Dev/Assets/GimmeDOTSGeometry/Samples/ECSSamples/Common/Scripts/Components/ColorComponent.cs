using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace GimmeDOTSGeometry.Samples.ECS
{
    [MaterialProperty("_Color")]
    public struct ColorComponent : IComponentData
    {
        public float4 value;
    }
}

using Unity.Entities;
using Unity.Mathematics;

namespace GimmeDOTSGeometry.Samples.ECS
{
    public struct Velocity3DComponent : IComponentData
    {
        public float3 velocity;
    }
}

using Unity.Entities;
using Unity.Mathematics;

namespace GimmeDOTSGeometry.Samples.ECS
{
    public struct Velocity2DComponent : IComponentData
    {
        public float2 velocity;
    }
}

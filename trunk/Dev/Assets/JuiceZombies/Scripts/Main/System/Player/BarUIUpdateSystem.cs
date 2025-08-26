//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-31 12:09:51
//---------------------------------------------------------------------


using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct BarUIUpdateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<ChaStats>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var cdfeBarData = SystemAPI.GetComponentLookup<JiYuHealth>();
            var cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>();
            var cdfePlayerArrowTag = SystemAPI.GetComponentLookup<PlayerArrowTag>(true);

            foreach (var (chaStats, childs, e) in SystemAPI
                         .Query<RefRO<ChaStats>, DynamicBuffer<Child>>()
                         .WithEntityAccess())
            {
                foreach (var child in childs)
                {
                    if (cdfeBarData.HasComponent(child.Value))
                    {
                        float hpRatios = chaStats.ValueRO.chaResource.hp / (float)chaStats.ValueRO.chaProperty.maxHp;
                        var temp = cdfeBarData[child.Value];
                        temp.value = hpRatios;
                        cdfeBarData[child.Value] = temp;
                    }

                    if (cdfePlayerArrowTag.HasComponent(child.Value))
                    {
                        var temp = cdfeLocalTransform[child.Value];
                        float needAngel =
                            MathHelper.SignedAngle(math.normalizesafe(chaStats.ValueRO.chaResource.direction),
                                new float3(0, 1, 0));
                        var dir = math.normalizesafe(chaStats.ValueRO.chaResource.direction);
                        var radius = math.length(temp.Position);

                        temp.Position = dir * radius;
                        
                        //temp = MathHelper.RotateAround(temp, float3.zero, needAngel);
                        var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));

                        temp.Rotation = qua;
                        cdfeLocalTransform[child.Value] = temp;
                    }
                }
            }
        }
    }
}
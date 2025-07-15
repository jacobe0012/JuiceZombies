using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace Main
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    [BurstCompile]
    public partial class TransformHybridUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<WorldBlackBoardTag>();
            RequireForUpdate<TransformHybridUpdateData>();
        }


        protected override void OnUpdate()
        {
            // var cdfeTran = SystemAPI.GetComponentLookup<LocalTransform>(true);
            // var strogInfo = SystemAPI.GetEntityStorageInfoLookup();
            foreach (var (hybrid, ltw, e) in SystemAPI
                         .Query<RefRW<TransformHybridUpdateData>, RefRO<LocalToWorld>>()
                         .WithEntityAccess())
            {
                if (hybrid.ValueRW.go.Value == null) return;
                hybrid.ValueRW.go.Value.transform.localPosition = ltw.ValueRO.Position;

                // We need to use the safe version as the vectors will not be normalized if there is some scale
                hybrid.ValueRW.go.Value.transform.localRotation =
                    quaternion.LookRotationSafe(ltw.ValueRO.Forward, ltw.ValueRO.Up);

                //var mat = *(UnityEngine.Matrix4x4*)&ltw;
                //Debug.Log($"");
                hybrid.ValueRW.go.Value.transform.localScale = ltw.ValueRO.Value.Scale();
            }
        }
    }
}
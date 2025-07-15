//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-09-05 12:00:25
//---------------------------------------------------------------------

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Main
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial struct ModifyRestitutionSystem : ISystem
    {
        //private const float normalRestitution = 0.3f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            //state.Enabled = false;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            new ChangeBoxColliderSizeJob
            {
                cdfeHitBackData = SystemAPI.GetComponentLookup<HitBackData>(true),
                globalConfigData = globalConfigData
            }.ScheduleParallel();
        }

        [BurstCompile]
        public partial struct ChangeBoxColliderSizeJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<HitBackData> cdfeHitBackData;
            [ReadOnly] public GlobalConfigData globalConfigData;

            public void Execute(Entity e, ref PhysicsCollider collider, in PlayerData playerData)
            {
                ref var constant = ref globalConfigData.value.Value.configTbbattle_constants.configTbbattle_constants;
                int battle_elastic = 0;
                for (int i = 0; i < constant.Length; i++)
                {
                    if (constant[i].constantName == (FixedString128Bytes)"battle_elastic")
                    {
                        battle_elastic = constant[i].constantValue;
                        break;
                    }
                }

                unsafe
                {
                    var colliderPtr = collider.ColliderPtr;

                    //colliderPtr[0].
                    //colliderPtr.
                    if (cdfeHitBackData.HasComponent(e))
                    {
                        if (colliderPtr->GetRestitution() != battle_elastic / 10000f)
                        {
                            colliderPtr->SetRestitution(battle_elastic / 10000f);
                        }
                    }
                    else
                    {
                        // var colliderPtr = collider.ColliderPtr;
                        if (colliderPtr->GetRestitution() != -100)
                        {
                            colliderPtr->SetRestitution(-100);
                        }
                    }
                }
            }
        }
    }
}
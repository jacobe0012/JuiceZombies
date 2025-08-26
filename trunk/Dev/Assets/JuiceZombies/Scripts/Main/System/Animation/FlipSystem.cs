//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-09-14 10:50:51
//---------------------------------------------------------------------

//using NSprites;

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main
{
    /// <summary>
    /// 控制敌人相对于玩家位置  贴图是否翻转
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(StateMachineSystem))]
    public partial struct FlipSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PlayerData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new FlipAccordingToPlayerPosJob
            {
                fDT = SystemAPI.Time.fixedDeltaTime,
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
                player = SystemAPI.GetSingletonEntity<PlayerData>(),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(true),
                entityStorageInfoLookup = SystemAPI.GetEntityStorageInfoLookup(),
                cdfeStateMachine = SystemAPI.GetComponentLookup<StateMachine>(true),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
                cdfeBossTag = SystemAPI.GetComponentLookup<BossTag>(true),
                bfeBuff = SystemAPI.GetBufferLookup<Buff>(true),
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct FlipAccordingToPlayerPosJob : IJobEntity
        {
            [ReadOnly] public float fDT;
            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [ReadOnly] public Entity player;
            [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
            public EntityStorageInfoLookup entityStorageInfoLookup;

            [ReadOnly] public ComponentLookup<StateMachine> cdfeStateMachine;

            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
            [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
            [ReadOnly] public ComponentLookup<BossTag> cdfeBossTag;
            [ReadOnly] public BufferLookup<Buff> bfeBuff;

            public void Execute(Entity e, ref FlipData flip)
            {
                if (cdfeEnemyData.HasComponent(e))
                {
                    bool canFlip = true;
                    bool bossCanMoveAndFlip = cdfeChaStats[e].chaControlState.bossCanFlip;
                    bool isAttack0 = cdfeStateMachine[e].currentState.CurrentTypeId ==
                                     State.TypeId.LittleMonsterAttack;
                    bool isAttack1 = cdfeStateMachine[e].currentState.CurrentTypeId ==
                                     State.TypeId.CommonBossAttack;

                    bool isAttack = isAttack0 || isAttack1;

                    canFlip = (isAttack && bossCanMoveAndFlip) || !isAttack;

                    var enemyData = cdfeEnemyData[e];
                    var chaStats = cdfeChaStats[e];
                    if (enemyData.changeDirYn == 0)
                    {
                        return;
                    }

                    if (chaStats.chaResource.hp > 0 && canFlip &&
                        (!chaStats.chaImmuneState.immuneControl) && !chaStats.chaControlState.cantMove)
                    {
                        if (entityStorageInfoLookup.Exists(enemyData.target))
                        {
                            if (cdfeLocalTransform[e].Position.x <
                                cdfeLocalTransform[enemyData.target].Position.x)
                            {
                                flip.value.x = 1;
                                // var tran = cdfeLocalTransform[e];
                                //
                                // tran.Position.x = -math.abs(tran.Position.x);
                                //
                                // cdfeLocalTransform[e] = tran;
                            }
                            else
                            {
                                flip.value.x = 0;
                                // var tran = cdfeLocalTransform[e];
                                //
                                // tran.Position.x = math.abs(tran.Position.x);
                                //
                                // cdfeLocalTransform[e] = tran;
                            }
                        }
                    }
                }
                else if (cdfePlayerData.HasComponent(e))
                {
                    var chaStats = cdfeChaStats[e];
                    if (!chaStats.chaControlState.cantMove)
                    {
                        if (chaStats.chaResource.direction.x > 0)
                        {
                            flip.value.x = 1;
                        }
                        else if (chaStats.chaResource.direction.x < 0)
                        {
                            flip.value.x = 0;
                        }
                    }
                }
            }
        }
    }
}
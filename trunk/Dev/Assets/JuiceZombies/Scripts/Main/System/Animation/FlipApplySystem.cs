//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-09-14 10:50:51
//---------------------------------------------------------------------

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Main
{
    /// <summary>
    /// 控制敌人相对于玩家位置  贴图是否翻转
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(FlipSystem))]
    public partial struct FlipApplySystem : ISystem
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
            new FlipApplyJob
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
                cdfeFlipData = SystemAPI.GetComponentLookup<FlipData>(true),
                bfeBuff = SystemAPI.GetBufferLookup<Buff>(true),
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct FlipApplyJob : IJobEntity
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
            [ReadOnly] public ComponentLookup<FlipData> cdfeFlipData;
            [ReadOnly] public BufferLookup<Buff> bfeBuff;

            public void Execute(Entity e, ref JiYuFlip flip, in Parent parent)
            {
                if (cdfeFlipData.HasComponent(parent.Value))
                {
                    flip.value = cdfeFlipData[parent.Value].value;
                }

                // if (cdfeEnemyData.HasComponent(parent.Value))
                // {
                //     // bool canMove = false;
                //     // if (cdfeBossTag.HasComponent(parent.Value))
                //     // {
                //     //     var buffs = bfeBuff[parent.Value];
                //     //
                //     //     foreach (var buff in buffs)
                //     //     {
                //     //         if (buff.CurrentTypeId == Buff.TypeId.Buff_LetBossMove)
                //     //         {
                //     //             canMove = true;
                //     //         }
                //     //     }
                //     // }
                //     bool bossCanMoveAndFlip = cdfeChaStats[parent.Value].chaControlState.bossCanMoveAndFlip;
                //     bool isAttack0 = cdfeStateMachine[parent.Value].currentState.CurrentTypeId ==
                //                      State.TypeId.LittleMonsterAttack;
                //     bool isAttack1 = cdfeStateMachine[parent.Value].currentState.CurrentTypeId ==
                //                      State.TypeId.CommonBossAttack;
                //
                //     bool isAttack = isAttack0 || isAttack1;
                //     if (isAttack && bossCanMoveAndFlip)
                //     {
                //         isAttack = true;
                //     }
                //     else
                //     {
                //         isAttack = false;
                //     }
                //
                //
                //     var enemyData = cdfeEnemyData[parent.Value];
                //     var chaStats = cdfeChaStats[parent.Value];
                //
                //     if (chaStats.chaResource.hp > 0 && !isAttack &&
                //         (!chaStats.chaImmuneState.immuneControl) && !chaStats.chaControlState.cantMove)
                //     {
                //         if (entityStorageInfoLookup.Exists(enemyData.target))
                //         {
                //             if (cdfeLocalTransform[parent.Value].Position.x <
                //                 cdfeLocalTransform[enemyData.target].Position.x)
                //             {
                //                 flip.value.x = 1;
                //                 var tran = cdfeLocalTransform[e];
                //
                //                 tran.Position.x = -math.abs(tran.Position.x);
                //
                //                 cdfeLocalTransform[e] = tran;
                //             }
                //             else
                //             {
                //                 flip.value.x = 0;
                //                 var tran = cdfeLocalTransform[e];
                //
                //                 tran.Position.x = math.abs(tran.Position.x);
                //
                //                 cdfeLocalTransform[e] = tran;
                //             }
                //         }
                //     }
                // }
                // else if (cdfePlayerData.HasComponent(parent.Value))
                // {
                //     var chaStats = cdfeChaStats[parent.Value];
                //     if (!chaStats.chaControlState.cantMove)
                //     {
                //         if (chaStats.chaResource.direction.x > 0)
                //         {
                //             flip.value.x = 1;
                //         }
                //         else if (chaStats.chaResource.direction.x < 0)
                //         {
                //             flip.value.x = 0;
                //         }
                //     }
                // }

                //enemy.survivalTick++;
                // if (cdfeLocalTransform[e].Position.x < cdfeLocalTransform[player].Position.x &&
                //     )
                // {
                //    
                // }
                // else
                // {
                //     flip.Value.x = 0;
                // }
            }
        }
    }
}
//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2025-04-21 10:50:51
//---------------------------------------------------------------------

//using NSprites;

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    /// <summary>
    ///闪避效果系统
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct DodgeAnimSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<DodgeAnimData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var singleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);

            new DodgeAnimSystemJob
            {
                ecb = ecb,
                dT = SystemAPI.Time.DeltaTime,
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
                player = SystemAPI.GetSingletonEntity<PlayerData>(),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(true),
                entityStorageInfoLookup = SystemAPI.GetEntityStorageInfoLookup(),
                cdfeStateMachine = SystemAPI.GetComponentLookup<StateMachine>(true),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
                cdfeBossTag = SystemAPI.GetComponentLookup<BossTag>(true),
                bfeBuff = SystemAPI.GetBufferLookup<Buff>(true),
                gameTimeData = gameTimeData,
                cdfeJiYuChromAberrAmount = SystemAPI.GetComponentLookup<JiYuChromAberrAmount>(),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct DodgeAnimSystemJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public float dT;
            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [ReadOnly] public Entity player;
            [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
            public EntityStorageInfoLookup entityStorageInfoLookup;

            [ReadOnly] public ComponentLookup<StateMachine> cdfeStateMachine;

            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
            [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
            [ReadOnly] public ComponentLookup<BossTag> cdfeBossTag;
            [ReadOnly] public BufferLookup<Buff> bfeBuff;
            [ReadOnly] public GameTimeData gameTimeData;
            [NativeDisableParallelForRestriction] public ComponentLookup<JiYuChromAberrAmount> cdfeJiYuChromAberrAmount;
            public EntityStorageInfoLookup storageInfoFromEntity;

            public const float AnimDuration = 0.6f;
            public const float MaxChromAberrAmount = 0.15f;

            public void Execute([EntityIndexInQuery] int sortKey, Entity e, ref DodgeAnimData dodgeAnimData,
                in EntityGroupData entityGroupData, in ChaStats chaStats)
            {
                if (!storageInfoFromEntity.Exists(entityGroupData.renderingEntity))
                {
                    return;
                }

                if (dodgeAnimData.tick == 0)
                {
                    dodgeAnimData.duration = AnimDuration;
                    dodgeAnimData.tick++;
                    ecb.AddComponent(sortKey, entityGroupData.renderingEntity, new JiYuChromAberrAmount()
                    {
                        value = 0
                    });

                    UnityHelper.EnableShaderFX(ref ecb, sortKey, entityGroupData.renderingEntity, 999001, true);
                    return;
                }

                var jiYuChromAberrAmount = cdfeJiYuChromAberrAmount[entityGroupData.renderingEntity];


                if (dodgeAnimData.duration < 0)
                {
                    // jiYuChromAberrAmount.value = 1;
                    // cdfeJiYuChromAberrAmount[entityGroupData.renderingEntity] = jiYuChromAberrAmount;
                    UnityHelper.EnableShaderFX(ref ecb, sortKey, entityGroupData.renderingEntity, 999001, false);
                    ecb.RemoveComponent<DodgeAnimData>(sortKey, e);
                    return;
                }

                if (chaStats.chaResource.hp <= 0)
                {
                    // jiYuChromAberrAmount.value = 1;
                    // cdfeJiYuChromAberrAmount[entityGroupData.renderingEntity] = jiYuChromAberrAmount;
                    UnityHelper.EnableShaderFX(ref ecb, sortKey, entityGroupData.renderingEntity, 999001, false);
                    ecb.RemoveComponent<DodgeAnimData>(sortKey, e);
                    return;
                }

                var oneTime = AnimDuration / 3f;
                var curTime = AnimDuration - dodgeAnimData.duration;

                var clamp1 = math.clamp(curTime, 0,
                    oneTime);
                float t1 = clamp1 / oneTime;
                var value = math.lerp(0, MaxChromAberrAmount, t1);

                var clamp2 = math.clamp(curTime - oneTime, 0,
                    oneTime);
                float t2 = clamp2 / oneTime;
                value = math.lerp(value, MaxChromAberrAmount, t2);

                var clamp3 = math.clamp(curTime - oneTime * 2f, 0,
                    oneTime);
                float t3 = clamp3 / oneTime;
                value = math.lerp(value, 0, t3);


                jiYuChromAberrAmount.value = value;


                Debug.Log($"jiYuScaleXY{jiYuChromAberrAmount.value} curTime{curTime}");
                //entityGroupData.renderingEntity
                cdfeJiYuChromAberrAmount[entityGroupData.renderingEntity] = jiYuChromAberrAmount;
                dodgeAnimData.duration -= dT;
                dodgeAnimData.tick++;
            }
        }
    }
}
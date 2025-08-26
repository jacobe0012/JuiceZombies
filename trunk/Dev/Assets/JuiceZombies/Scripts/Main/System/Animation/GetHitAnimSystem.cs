//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2025-04-18 10:50:51
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
    /// 受击效果系统
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct GetHitAnimSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GetHitAnimData>();
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
            var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);
            new GetHitAnimJob
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
                cdfeJiYuScaleXY = SystemAPI.GetComponentLookup<JiYuScaleXY>(),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                cdfeJiYuOverlayColor = SystemAPI.GetComponentLookup<JiYuOverlayColor>(),
                gameOthersData = gameOthersData,
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct GetHitAnimJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public float dT;
            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [ReadOnly] public Entity player;
            [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
            public EntityStorageInfoLookup entityStorageInfoLookup;
            [ReadOnly] public GameOthersData gameOthersData;
            [ReadOnly] public ComponentLookup<StateMachine> cdfeStateMachine;

            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
            [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
            [ReadOnly] public ComponentLookup<BossTag> cdfeBossTag;
            [ReadOnly] public BufferLookup<Buff> bfeBuff;
            [ReadOnly] public GameTimeData gameTimeData;
            [NativeDisableParallelForRestriction] public ComponentLookup<JiYuScaleXY> cdfeJiYuScaleXY;
            [NativeDisableParallelForRestriction] public ComponentLookup<JiYuOverlayColor> cdfeJiYuOverlayColor;
            public EntityStorageInfoLookup storageInfoFromEntity;

            // public const float AnimDuration = 0.4f;
            // public const float Offset = 0.25f;

            public void Execute([EntityIndexInQuery] int sortKey, Entity e, ref GetHitAnimData getHitAnimData,
                in EntityGroupData entityGroupData, in ChaStats chaStats)
            {
                var AnimDuration = gameOthersData.gameOtherParas.getHitDuration;
                var Offset = gameOthersData.gameOtherParas.getHitOffset;
                if (!storageInfoFromEntity.Exists(entityGroupData.renderingEntity))
                {
                    return;
                }

                if (getHitAnimData.tick == 0)
                {
                    getHitAnimData.duration = AnimDuration;
                    getHitAnimData.tick++;
                    ecb.AddComponent(sortKey, entityGroupData.renderingEntity, new JiYuScaleXY
                    {
                        value = 1
                    });
                    UnityHelper.EnableShaderFX(ref ecb, sortKey, entityGroupData.renderingEntity, 999101, true);
                    return;
                }


                var oneTime = AnimDuration / 3f;
                var curTime = AnimDuration - getHitAnimData.duration;
                if (cdfeJiYuScaleXY.HasComponent(entityGroupData.renderingEntity))
                {
                    var jiYuScaleXY = cdfeJiYuScaleXY[entityGroupData.renderingEntity];


                    if (getHitAnimData.duration < 0)
                    {
                        jiYuScaleXY.value = 1;
                        cdfeJiYuScaleXY[entityGroupData.renderingEntity] = jiYuScaleXY;

                        ecb.RemoveComponent<GetHitAnimData>(sortKey, e);
                        return;
                    }

                    // if (chaStats.chaResource.hp <= 0)
                    // {
                    //     jiYuScaleXY.value = 1;
                    //     cdfeJiYuScaleXY[entityGroupData.renderingEntity] = jiYuScaleXY;
                    //     UnityHelper.EnableShaderFX(ref ecb, sortKey, entityGroupData.renderingEntity, 999101, false);
                    //     ecb.RemoveComponent<GetHitAnimData>(sortKey, e);
                    //     return;
                    // }


                    var clamp1 = math.clamp(curTime, 0,
                        oneTime);
                    float t1 = clamp1 / oneTime;
                    var scaleX = math.lerp(jiYuScaleXY.value.x, 1 + Offset, t1);
                    var scaleY = math.lerp(jiYuScaleXY.value.y, 1 - Offset, t1);

                    var clamp2 = math.clamp(curTime - oneTime, 0,
                        oneTime);
                    float t2 = clamp2 / oneTime;
                    scaleX = math.lerp(scaleX, 1 - Offset, t2);
                    scaleY = math.lerp(scaleY, 1 + Offset, t2);

                    var clamp3 = math.clamp(curTime - oneTime * 2f, 0,
                        oneTime);
                    float t3 = clamp3 / oneTime;
                    scaleX = math.lerp(scaleX, 1, t3);
                    scaleY = math.lerp(scaleY, 1, t3);

                    jiYuScaleXY.value.x = scaleX;
                    jiYuScaleXY.value.y = scaleY;

                    //Debug.Log($"jiYuScaleXY{jiYuScaleXY.value} curTime{curTime}");
                    //entityGroupData.renderingEntity
                    cdfeJiYuScaleXY[entityGroupData.renderingEntity] = jiYuScaleXY;
                }

                if (cdfeJiYuOverlayColor.HasComponent(entityGroupData.renderingEntity))
                {
                    var jiYuOverlayColor = cdfeJiYuOverlayColor[entityGroupData.renderingEntity];

                    var clamp = math.clamp(curTime, 0,
                        AnimDuration);
                    float t = clamp / AnimDuration;
                    jiYuOverlayColor.value.w = math.lerp(0, 1, t);

                    cdfeJiYuOverlayColor[entityGroupData.renderingEntity] = jiYuOverlayColor;
                }

                if (curTime > 0.1f)
                {
                    UnityHelper.EnableShaderFX(ref ecb, sortKey, entityGroupData.renderingEntity, 999101, false);
                }

                getHitAnimData.duration -= dT;
                getHitAnimData.tick++;
            }
        }
    }
}
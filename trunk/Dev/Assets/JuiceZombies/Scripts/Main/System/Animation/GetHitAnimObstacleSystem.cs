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
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct GetHitAnimObstacleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GetHitAnimObstacleData>();
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
            new GetHitAnimObstacleSystemJob
            {
                ecb = ecb,
                dT = SystemAPI.Time.DeltaTime,
                eT = (float)SystemAPI.Time.ElapsedTime,
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
                cdfeJiYuDissolveThreshold = SystemAPI.GetComponentLookup<JiYuDissolveThreshold>(true),
                cdfeJiYuOverlayColor = SystemAPI.GetComponentLookup<JiYuOverlayColor>(),
                gameOthersData = gameOthersData,
                globalConfigData = globalConfigData,
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct GetHitAnimObstacleSystemJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public float dT;
            [ReadOnly] public float eT;
            [ReadOnly] public Entity player;
            [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
            public EntityStorageInfoLookup entityStorageInfoLookup;
            [ReadOnly] public GameOthersData gameOthersData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public ComponentLookup<StateMachine> cdfeStateMachine;

            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
            [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
            [ReadOnly] public ComponentLookup<BossTag> cdfeBossTag;
            [ReadOnly] public BufferLookup<Buff> bfeBuff;
            [ReadOnly] public GameTimeData gameTimeData;
            [NativeDisableParallelForRestriction] public ComponentLookup<JiYuScaleXY> cdfeJiYuScaleXY;
            [NativeDisableParallelForRestriction] public ComponentLookup<JiYuOverlayColor> cdfeJiYuOverlayColor;
            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            public EntityStorageInfoLookup storageInfoFromEntity;

            [ReadOnly] public ComponentLookup<JiYuDissolveThreshold> cdfeJiYuDissolveThreshold;
            // public const float AnimDuration = 0.4f;
            // public const float Offset = 0.25f;

            public void Execute([EntityIndexInQuery] int sortKey, Entity e,
                ref GetHitAnimObstacleData getHitAnimData, in EntityGroupData entityGroupData, in ChaStats chaStats,
                in MapElementData mapData, in ObstacleTag _)
            {
                var AnimDuration = gameOthersData.gameOtherParas.getHitDuration;
                var Offset = gameOthersData.gameOtherParas.getHitOffset;
                if (!storageInfoFromEntity.Exists(entityGroupData.renderingEntity))
                {
                    return;
                }

                var localTransform = cdfeLocalTransform[entityGroupData.renderingEntity];
                ref var configTbscene_modules =
                    ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
                int sceneModulesIndex = 0;
                for (int i = 0; i < configTbscene_modules.Length; i++)
                {
                    if (configTbscene_modules[i].id == mapData.elementID)
                    {
                        sceneModulesIndex = i;
                        break;
                    }
                }

                ref var tbscene_modules =
                    ref configTbscene_modules[sceneModulesIndex];

                if (getHitAnimData.tick == 0)
                {
                    if (tbscene_modules.hitAnimationPara.Length >= 5 || tbscene_modules.picHitAnimationPara.Length >= 5)
                    {
                        var para1 = getHitAnimData.type == 1
                            ? tbscene_modules.picHitAnimationPara[0]
                            : tbscene_modules.hitAnimationPara[0];

                        getHitAnimData.duration = para1 / 1000f;
                    }

                    getHitAnimData.orginalTransform = localTransform;
                    getHitAnimData.orginalTransform.Rotation = quaternion.identity;
                    getHitAnimData.tick++;

                    UnityHelper.EnableShaderFX(ref ecb, sortKey, entityGroupData.renderingEntity, 999101, true);
                    return;
                }

                if (tbscene_modules.hitAnimationPara.Length >= 5 || tbscene_modules.picHitAnimationPara.Length >= 5)
                {
                    var para1 = tbscene_modules.hitAnimationPara[0];
                    var para2 = tbscene_modules.hitAnimationPara[1];
                    var para3 = tbscene_modules.hitAnimationPara[2];
                    var para4 = tbscene_modules.hitAnimationPara[3];
                    var para5 = tbscene_modules.hitAnimationPara[4];

                    if (getHitAnimData.type == 1)
                    {
                        para1 = tbscene_modules.picHitAnimationPara[0];
                        para2 = tbscene_modules.picHitAnimationPara[1];
                        para3 = tbscene_modules.picHitAnimationPara[2];
                        para4 = tbscene_modules.picHitAnimationPara[3];
                        para5 = tbscene_modules.picHitAnimationPara[4];
                    }

                    para2 *= 4;

                    var curTime = (para1 / 1000f) - getHitAnimData.duration;
                    var singleTime = (para1 / 1000f) / para2;
                    float frequency = para2 / (para1 / 1000f); // 每秒抖动次数
                    float offsetX = math.sin(eT * frequency * 2 * math.PI) * (para4 / 1000f); // 毫米转为米
                    float rotationZ = math.sin(eT * frequency * 2 * math.PI) * para3;
                    // 应用左右抖动（仅影响 X 轴）
                    localTransform.Position.x += offsetX;
                    localTransform.Rotation = getHitAnimData.orginalTransform.Rotation; // 先重置旋转
                    var pos = getHitAnimData.orginalTransform.Position + ((getHitAnimData.orginalTransform.Scale / 2f) -
                                                                          getHitAnimData.orginalTransform.Scale *
                                                                          (para5 / 10000f));


                    localTransform.Rotation =
                        MathHelper.RotateAround(localTransform, pos, math.radians(rotationZ)).Rotation;
                    Debug.Log($"localTransform.Position.x{localTransform.Position.x}");

                    if (curTime > 0.1f)
                    {
                        UnityHelper.EnableShaderFX(ref ecb, sortKey, entityGroupData.renderingEntity, 999101, false);
                    }
                }

                if (getHitAnimData.duration < 0)
                {
                    localTransform = getHitAnimData.orginalTransform;
                    cdfeLocalTransform[entityGroupData.renderingEntity] = localTransform;
                    UnityHelper.EnableShaderFX(ref ecb, sortKey, entityGroupData.renderingEntity, 999101, false);
                    ecb.RemoveComponent<GetHitAnimObstacleData>(sortKey, e);

                    return;
                }

                if (chaStats.chaResource.hp <= 0 && !cdfeJiYuDissolveThreshold.HasComponent(e))
                {
                    ecb.AddComponent(sortKey, e,
                        new JiYuAlphaTo0()
                        {
                            value = UnityHelper.Color2Float4(Color.white)
                        });
                    ecb.AddComponent(sortKey, e,
                        new JiYuDissolveThreshold());
                }

                cdfeLocalTransform[entityGroupData.renderingEntity] = localTransform;
                getHitAnimData.duration -= dT;
                getHitAnimData.tick++;
            }
        }
    }
}
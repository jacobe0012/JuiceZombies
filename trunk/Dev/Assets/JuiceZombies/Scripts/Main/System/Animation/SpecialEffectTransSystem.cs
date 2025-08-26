//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-11 10:30:51
//---------------------------------------------------------------------

using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    /// <summary>
    /// 控制特效变换的系统
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SkillAttackSystem))]
    public partial struct SpecialEffectTransSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SpecialEffectData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);


            new SpecialEffectTransJob
            {
                ecb = ecb,
                fdT = SystemAPI.Time.fixedDeltaTime,
                gameTimeData = gameTimeData,
                bfeSkill = SystemAPI.GetBufferLookup<Skill>(true),
                bfeSpecialEffectChainBuffer = SystemAPI.GetBufferLookup<SpecialEffectChainBuffer>(),
                bfeChild = SystemAPI.GetBufferLookup<Child>(),
                bfeLinkedEntityGroup = SystemAPI.GetBufferLookup<LinkedEntityGroup>(),
                eT = SystemAPI.Time.ElapsedTime,
                prefabMapData = prefabMapData,
                gameSetUpData = gameSetUpData,
                globalConfigData = globalConfigData,
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                cdfePostTransformMatrix = SystemAPI.GetComponentLookup<PostTransformMatrix>(),
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
                cdfeJiYuAnimIndex = SystemAPI.GetComponentLookup<JiYuAnimIndex>(),
                cdfeParent = SystemAPI.GetComponentLookup<Parent>(true),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(true),
                cdfeFlipData = SystemAPI.GetComponentLookup<FlipData>(true)
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct SpecialEffectTransJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public float fdT;
            [ReadOnly] public double eT;
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GameSetUpData gameSetUpData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public GameTimeData gameTimeData;
            [ReadOnly] public BufferLookup<Skill> bfeSkill;

            [NativeDisableParallelForRestriction]
            public BufferLookup<SpecialEffectChainBuffer> bfeSpecialEffectChainBuffer;

            [NativeDisableParallelForRestriction] public BufferLookup<Child> bfeChild;

            [NativeDisableParallelForRestriction] public BufferLookup<LinkedEntityGroup> bfeLinkedEntityGroup;
            public EntityStorageInfoLookup storageInfoFromEntity;
            [NativeDisableParallelForRestriction] public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;
            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [NativeDisableParallelForRestriction] public ComponentLookup<JiYuAnimIndex> cdfeJiYuAnimIndex;

            [ReadOnly] public ComponentLookup<Parent> cdfeParent;
            [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
            [ReadOnly] public ComponentLookup<FlipData> cdfeFlipData;


            private void Execute([EntityIndexInQuery] int sortKey, Entity e, ref SpecialEffectData specialEffectData)
            {
                if (cdfeParent.HasComponent(e))
                {
                    var parent = cdfeParent[e].Value;
                    if (cdfeChaStats.HasComponent(parent))
                    {
                        if (cdfeChaStats[parent].chaResource.hp <= 0)
                        {
                            ecb.DestroyEntity(sortKey, e);
                        }
                    }
                }

                if (!cdfeLocalTransform.HasComponent(e))
                {
                    return;
                }

                var tran = cdfeLocalTransform[e];
                ref var tbspecial_effects =
                    ref globalConfigData.value.Value.configTbspecial_effects.configTbspecial_effects;

                int specialEffectIndex = -1;
                for (int j = 0; j < tbspecial_effects.Length; j++)
                {
                    if (tbspecial_effects[j].id == specialEffectData.id)
                    {
                        specialEffectIndex = j;
                        break;
                    }
                }

                specialEffectIndex = specialEffectData.type == 2 ? 0 : specialEffectIndex;
                if (specialEffectIndex == -1)
                {
                    //Debug.LogError($"vfx ID:{specialEffectId} is not exist!");
                    return;
                }

                ref var specialEffect =
                    ref tbspecial_effects[specialEffectIndex];
                if (specialEffectData.type == 0)
                {
                    switch (specialEffect.type)
                    {
                        case 1:

                            if (specialEffect.para1 != 0)
                            {
                                var radians = math.radians(specialEffect.para1);
                                radians /= (1f / fdT);
                                tran = tran.RotateZ(-radians * gameTimeData.logicTime.gameTimeScale);
                            }

                            break;
                        case 2:

                            var p0 = specialEffectData.startPos;
                            var p2 = specialEffectData.targetPos;
                            float2 v = (specialEffectData.targetPos - specialEffectData.startPos).xy;
                            float2 vPerp = math.normalizesafe(-new float2(-v.y, v.x)); // 垂直向量

                            var p1 = MathHelper.Get2PointRatiosPoint(p0, p2, specialEffect.para2 / 10000f);
                            vPerp = p0.x < p2.x ? -vPerp : vPerp;
                            p1.z = 0;
                            p1.xy += vPerp * (specialEffect.para1 / 1000f);


                            var maxDuration = specialEffect.para4 / 1000f;
                            var duration = maxDuration - specialEffectData.tick * fdT;
                            var pos1 = PhysicsHelper.QuardaticBezier(
                                (maxDuration - duration) / maxDuration, p0,
                                p1, p2);

                            if (specialEffect.para3 == 1)
                            {
                                var pos2 = PhysicsHelper.QuardaticBezier(
                                    (maxDuration - (duration - fdT)) / maxDuration, p0,
                                    p1, p2);
                                tran.Rotation = MathHelper.LookRotation2D(pos2 - pos1);
                            }

                            tran.Position = pos1;
                            //specialEffect.para1;
                            // if (storageInfoFromEntity.Exists(specialEffectData.caster))
                            // {
                            //     float3 targetPos = default;
                            //     float3 casterPos = cdfeLocalTransform[specialEffectData.caster].Position;
                            //     var skills = bfeSkill[specialEffectData.caster];
                            //     foreach (var skill in skills)
                            //     {
                            //         if (skill.Int32_0 == specialEffectData.skillId)
                            //         {
                            //             targetPos = skill.float3_15;
                            //             if (skill.Single_8 <= 0)
                            //             {
                            //                 ecb.DestroyEntity(sortKey, e);
                            //             }
                            //
                            //             break;
                            //         }
                            //     }
                            //
                            //     var vector = targetPos - casterPos;
                            //
                            //     tran.Rotation = MathHelper.LookRotation2D(vector);
                            //     var forward = MathHelper.Forward(tran.Rotation);
                            //     var carrierTran = cdfeLocalTransform[specialEffectData.caster];
                            //   
                            // }


                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        case 5:
                            break;
                        case 6:
                            float3 offset = default;
                            float scaleMulti = 1f;
                            switch (specialEffect.para1)
                            {
                                case 1:
                                    if (storageInfoFromEntity.Exists(specialEffectData.caster) &&
                                        cdfePostTransformMatrix.HasComponent(e))
                                    {
                                        var temp = cdfePostTransformMatrix[e];

                                        float3 targetPos = default;
                                        float scale = 1;
                                        float3 casterPos = cdfeLocalTransform[specialEffectData.caster].Position;
                                        switch (specialEffect.offset)
                                        {
                                            case 0:
                                                offset = float3.zero;
                                                offset *= specialEffect.offsetPara[0] / 1000f;
                                                break;
                                            case 1:
                                                offset.y = 1;
                                                offset *= specialEffect.offsetPara[0] / 1000f;
                                                break;
                                            case 2:
                                                offset.x = -1;
                                                offset *= specialEffect.offsetPara[0] / 1000f;
                                                break;
                                            case 3:
                                                offset.x = 1;
                                                offset *= specialEffect.offsetPara[0] / 1000f;
                                                break;
                                            case 4:
                                                offset.y = -1;
                                                offset *= specialEffect.offsetPara[0] / 1000f;
                                                break;

                                            case 6:

                                                offset = new float3(specialEffect.offsetPara[0] / 1000f,
                                                    specialEffect.offsetPara[1] / 1000f, 0);
                                                if (cdfeFlipData.HasComponent(specialEffectData.caster))
                                                {
                                                    var flipData = cdfeFlipData[specialEffectData.caster];
                                                    offset.x = flipData.value.x < 0.5f ? -offset.x : offset.x;
                                                }

                                                break;
                                        }

                                        casterPos += offset;

                                        var skills = bfeSkill[specialEffectData.caster];
                                        foreach (var skill in skills)
                                        {
                                            if (skill.Int32_0 == specialEffectData.skillId)
                                            {
                                                targetPos = skill.LocalTransform_15.Position;
                                                scale = skill.LocalTransform_15.Scale;
                                                if (skill.Single_8 <= 0)
                                                {
                                                    ecb.DestroyEntity(sortKey, e);
                                                }

                                                break;
                                            }
                                        }

                                        float3 dir = math.normalizesafe(targetPos - casterPos);
                                        // Normalize direction


                                        // Calculate point: end + normalized direction * scale
                                        targetPos -= dir * (scale / 2f * (3f / 4f));
                                        //targetPos = targetPos - dir * (scale);
                                        float3 vector = targetPos - casterPos;

                                        temp = new PostTransformMatrix
                                        {
                                            Value = float4x4.Scale(specialEffect.sizeX / 10000f,
                                                math.length(vector), 1)
                                        };
                                        var ratios = math.length(vector) / (specialEffect.sizeY / 10000f);

                                        //oldTran.Scale = 4;
                                        tran.Position = (targetPos + casterPos) / 2f;
                                        tran.Rotation = MathHelper.LookRotation2D(vector);
                                        if (specialEffect.para2 == 1)
                                        {
                                            ecb.SetComponent(sortKey, e, new JiYuMainTexST
                                            {
                                                value = new float4(1, ratios, 0, 0)
                                            });
                                        }

                                        cdfePostTransformMatrix[e] = temp;
                                    }

                                    break;
                                case 2:
                                    if (storageInfoFromEntity.Exists(specialEffectData.caster))
                                    {
                                        float3 targetPos = default;
                                        float3 casterPos = cdfeLocalTransform[specialEffectData.caster].Position;
                                        var skills = bfeSkill[specialEffectData.caster];
                                        foreach (var skill in skills)
                                        {
                                            if (skill.Int32_0 == specialEffectData.skillId)
                                            {
                                                targetPos = skill.LocalTransform_15.Position;
                                                if (skill.Single_8 <= 0)
                                                {
                                                    ecb.DestroyEntity(sortKey, e);
                                                }

                                                break;
                                            }
                                        }

                                        var vector = targetPos - casterPos;

                                        tran.Rotation = MathHelper.LookRotation2D(vector);
                                        var forward = math.normalizesafe(vector);
                                        var carrierTran = cdfeLocalTransform[specialEffectData.caster];
                                        switch (specialEffect.para2)
                                        {
                                            case 1:
                                                scaleMulti = 1f;
                                                break;
                                            case 2:
                                                scaleMulti = 1f / carrierTran.Scale;
                                                break;
                                        }

                                        switch (specialEffect.offset)
                                        {
                                            case 0:
                                                offset = float3.zero;
                                                break;
                                            case 1:
                                                offset.y = 1;
                                                break;
                                            case 2:
                                                offset.x = -1;
                                                break;
                                            case 3:
                                                offset.x = 1;
                                                break;
                                            case 4:
                                                offset.y = -1;
                                                break;
                                            case 5:
                                                offset = forward;
                                                //offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                                break;
                                        }

                                        //oldTran.Position = offset * (carrierTran.Scale / 2f);
                                        var offsetPara = specialEffect.offsetPara.Length > 0
                                            ? specialEffect.offsetPara[0] / 1000f
                                            : 0;

                                        //oldTran.Position = offset * (carrierTran.Scale / 2f);

                                        tran.Position = offset * scaleMulti * offsetPara;
                                    }

                                    break;
                                // case 3:
                                //     //闪电链生成请求
                                //     if (specialEffectData.tick == 0 && bfeSpecialEffectChainBuffer.HasBuffer(e))
                                //     {
                                //         var buffer = bfeSpecialEffectChainBuffer[e];
                                //         Debug.Log($"排序前");
                                //         foreach (var VARIABLE in buffer)
                                //         {
                                //             Debug.Log(VARIABLE.Entity.Index);
                                //         }
                                //
                                //         UnityHelper.TrySortChainList(ref buffer, cdfeLocalTransform,
                                //             specialEffectData.chainCenterEntity,storageInfoFromEntity);
                                //         Debug.Log($"排序后");
                                //         foreach (var VARIABLE in buffer)
                                //         {
                                //             Debug.Log(VARIABLE.Entity.Index);
                                //         }
                                //
                                //         var count = buffer.Length - 1;
                                //         if (!prefabMapData.prefabHashMap.TryGetValue($"special_effect_skill_130414040",
                                //                 out var prefab0))
                                //         {
                                //         }
                                //
                                //         if (!prefabMapData.prefabHashMap.TryGetValue($"special_effect_skill_130414041",
                                //                 out var prefab1))
                                //         {
                                //         }
                                //
                                //         specialEffectData.duration = (specialEffect.loopSpeed / 1000f) /
                                //                                      gameTimeData.logicTime.gameTimeScale;
                                //         var singleDuration = specialEffectData.duration / count;
                                //         for (int i = 0; i < count; i++)
                                //         {
                                //             var ins = ecb.Instantiate(sortKey, prefab0);
                                //             var prefab0SpecialEffect = new SpecialEffectData
                                //             {
                                //                 type = 1,
                                //                 id = specialEffectData.id * 10,
                                //                 stateId = 0,
                                //                 groupId = 0,
                                //                 sort = 0,
                                //                 caster = default,
                                //                 tick = 0,
                                //                 skillId = 0,
                                //                 startPos = default,
                                //                 targetPos = default,
                                //                 duration = 0,
                                //                 chainCenterEntity = default,
                                //                 chainLastEntity = buffer[i].Entity,
                                //                 chainNextEntity = buffer[i + 1].Entity
                                //             };
                                //             ecb.AddComponent(sortKey, ins, new TimeToDieData
                                //             {
                                //                 duration = singleDuration + 5f
                                //             });
                                //             ecb.SetComponent(sortKey, ins, new JiYuFrameAnimSpeed
                                //             {
                                //                 value = singleDuration
                                //             });
                                //             ecb.SetComponent(sortKey, ins, new JiYuFrameAnimLoop
                                //             {
                                //                 value = specialEffect.loopType == 1 ? 0 : 1
                                //             });
                                //             ecb.SetComponent(sortKey, ins, new JiYuSort
                                //             {
                                //                 value = new int2(specialEffect.zSort, specialEffect.zIndex)
                                //             });
                                //             ecb.SetComponent(sortKey, ins, prefab0SpecialEffect);
                                //
                                //             ecb.AddComponent(sortKey, ins, new PostTransformMatrix
                                //             {
                                //                 Value = float4x4.Scale(specialEffect.sizeX / 10000f * scaleMulti,
                                //                     math.length(2) * scaleMulti, 1)
                                //             });
                                //         }
                                //
                                //         for (int i = 0; i < buffer.Length; i++)
                                //         {
                                //             var ins1 = ecb.Instantiate(sortKey, prefab1);
                                //             var prefab1SpecialEffect = new SpecialEffectData
                                //             {
                                //                 type = 1,
                                //                 id = specialEffectData.id * 10 + 1,
                                //                 stateId = 0,
                                //                 groupId = 0,
                                //                 sort = 0,
                                //                 caster = default,
                                //                 tick = 0,
                                //                 skillId = 0,
                                //                 startPos = default,
                                //                 targetPos = default,
                                //                 duration = 0,
                                //                 chainCenterEntity = default,
                                //                 chainLastEntity = buffer[i].Entity,
                                //                 chainNextEntity = buffer[i + 1].Entity
                                //             };
                                //             ecb.AddComponent(sortKey, ins1, new TimeToDieData
                                //             {
                                //                 duration = 5
                                //             });
                                //             ecb.SetComponent(sortKey, ins1, new JiYuFrameAnimSpeed
                                //             {
                                //                 value = 2
                                //             });
                                //             ecb.SetComponent(sortKey, ins1, new JiYuFrameAnimLoop
                                //             {
                                //                 value = 0
                                //             });
                                //             ecb.SetComponent(sortKey, ins1, new JiYuSort
                                //             {
                                //                 value = new int2(specialEffect.zSort, specialEffect.zIndex)
                                //             });
                                //             ecb.SetComponent(sortKey, ins1, prefab1SpecialEffect);
                                //
                                //             // ecb.AddComponent(sortKey, ins1, new PostTransformMatrix
                                //             // {
                                //             //     Value = float4x4.Scale(specialEffect.sizeX / 10000f * scaleMulti,
                                //             //         math.length(2) * scaleMulti, 1)
                                //             // });
                                //         }
                                //
                                //         //bfeSpecialEffectChainBuffer[e] = buffer;
                                //     }
                                //
                                //     //闪电链链子本体
                                //     if (!bfeSpecialEffectChainBuffer.HasBuffer(e))
                                //     {
                                //         if (cdfePostTransformMatrix.HasComponent(e))
                                //         {
                                //             var temp = cdfePostTransformMatrix[e];
                                //
                                //             float3 targetPos = cdfeLocalTransform[specialEffectData.chainNextEntity]
                                //                 .Position;
                                //             float3 casterPos = cdfeLocalTransform[specialEffectData.chainLastEntity]
                                //                 .Position;
                                //
                                //             var vector = targetPos - casterPos;
                                //
                                //             temp = new PostTransformMatrix
                                //             {
                                //                 Value = float4x4.Scale(temp.Value.Scale().x,
                                //                     math.length(vector), 1)
                                //             };
                                //             //oldTran.Scale = 4;
                                //             tran.Position = (targetPos + casterPos) / 2f;
                                //             tran.Rotation = MathHelper.LookRotation2D(vector);
                                //
                                //             cdfePostTransformMatrix[e] = temp;
                                //         }
                                //     }
                                //
                                //     break;
                            }


                            break;
                        case 8:

                            var duration1 = (specialEffect.maxTime / 1000f);
                            if (duration1 < (specialEffectData.tick - 1) * 0.02f)
                            {
                                var eventE = ecb.CreateEntity(sortKey);
                                ecb.AddComponent(sortKey, eventE, new HybridEventData()
                                {
                                    args = new float4(0, specialEffect.para1, specialEffect.sizeX, 0),
                                    bossEntity = specialEffectData.caster,
                                    strArgs = $"{specialEffect.model}"
                                });
                            }


                            break;
                    }
                }
                else if (specialEffectData.type == 1)
                {
                    switch (specialEffect.type)
                    {
                        case 6:
                            float3 offset = default;
                            float scaleMulti = 1f;
                            switch (specialEffect.para1)
                            {
                                case 3:
                                    //闪电链链子本体
                                    if (!bfeSpecialEffectChainBuffer.HasBuffer(e))
                                    {
                                        if (specialEffectData.id == 130414040 &&
                                            storageInfoFromEntity.Exists(specialEffectData.chainNextEntity) &&
                                            storageInfoFromEntity.Exists(specialEffectData.chainLastEntity) &&
                                            specialEffectData.tick >= specialEffectData.startTick)
                                        {
                                            //Debug.LogError($"闪电链链子本体");
                                            var temp = cdfePostTransformMatrix[e];

                                            float3 targetPos = cdfeLocalTransform[specialEffectData.chainNextEntity]
                                                .Position;
                                            float3 casterPos = cdfeLocalTransform[specialEffectData.chainLastEntity]
                                                .Position;

                                            var vector = targetPos - casterPos;

                                            temp = new PostTransformMatrix
                                            {
                                                Value = float4x4.Scale(specialEffect.sizeX / 10000f,
                                                    math.length(vector), 1)
                                            };
                                            //oldTran.Scale = 4;
                                            tran.Position = (targetPos + casterPos) / 2f;
                                            tran.Rotation = MathHelper.LookRotation2D(vector);

                                            cdfePostTransformMatrix[e] = temp;

                                            if (specialEffectData.tick == specialEffectData.startTick)
                                            {
                                                ecb.SetComponent(sortKey, e, new JiYuFrameAnimSpeed
                                                {
                                                    value = specialEffectData.duration
                                                });

                                                ecb.SetComponent(sortKey, e, new JiYuStartTime()
                                                {
                                                    value = 0
                                                });
                                            }
                                        }

                                        if (specialEffectData.id == 130414041)
                                        {
                                            if (specialEffectData.tick == specialEffectData.startTick)
                                            {
                                                ecb.SetComponent(sortKey, e, new JiYuFrameAnimSpeed
                                                {
                                                    value = specialEffectData.duration
                                                });

                                                ecb.SetComponent(sortKey, e, new JiYuStartTime()
                                                {
                                                    value = 0
                                                });
                                            }
                                        }
                                    }

                                    // if (expr)
                                    // {
                                    //     
                                    // }
                                    break;
                            }


                            break;
                    }
                }
                else if (specialEffectData.type == 2)
                {
                    //闪电链生成请求
                    if (specialEffectData.tick == 0 && bfeSpecialEffectChainBuffer.HasBuffer(e))
                    {
                        int specialEffectIndex0 = -1;
                        int specialEffectIndex1 = -1;
                        for (int j = 0; j < tbspecial_effects.Length; j++)
                        {
                            if (tbspecial_effects[j].id == specialEffectData.id * 10)
                            {
                                specialEffectIndex0 = j;
                            }

                            if (tbspecial_effects[j].id == specialEffectData.id * 10 + 1)
                            {
                                specialEffectIndex1 = j;
                            }
                        }

                        ref var specialEffect0 =
                            ref tbspecial_effects[specialEffectIndex0];
                        ref var specialEffect1 =
                            ref tbspecial_effects[specialEffectIndex1];
                        var buffer = bfeSpecialEffectChainBuffer[e];
                        //
                        // Debug.Log($"排序中心{specialEffectData.chainCenterEntity.Index}"); 
                        // Debug.Log($"排序前");
                        // foreach (var VARIABLE in buffer)
                        // {
                        //     Debug.Log(VARIABLE.Entity.Index);
                        // }

                        UnityHelper.TrySortChainList(ref buffer, cdfeLocalTransform,
                            specialEffectData.chainCenterEntity, storageInfoFromEntity);
                        // Debug.Log($"排序后");
                        // foreach (var VARIABLE in buffer)
                        // {
                        //     Debug.Log(VARIABLE.Entity.Index);
                        // }

                        var count = buffer.Length - 1;
                        if (!prefabMapData.prefabHashMap.TryGetValue($"special_effect_skill_130414040",
                                out var prefab0))
                        {
                        }

                        if (!prefabMapData.prefabHashMap.TryGetValue($"special_effect_skill_130414041",
                                out var prefab1))
                        {
                        }

                        specialEffectData.duration = (specialEffect0.loopSpeed / 1000f) /
                                                     gameTimeData.logicTime.gameTimeScale;
                        var singleDuration = specialEffectData.duration / count;
                        //singleDuration = specialEffectData.duration;
                        var scaleMulti = 1f;
                        for (int i = 0; i < count; i++)
                        {
                            var ins = ecb.Instantiate(sortKey, prefab0);
                            var prefab0SpecialEffect = new SpecialEffectData
                            {
                                type = 1,
                                id = specialEffectData.id * 10,
                                stateId = 0,
                                groupId = 0,
                                sort = 0,
                                caster = default,
                                tick = 0,
                                skillId = 0,
                                startPos = default,
                                targetPos = default,
                                duration = singleDuration,
                                chainCenterEntity = default,
                                chainLastEntity = buffer[i].Entity,
                                chainNextEntity = buffer[i + 1].Entity,
                                startTick = i * (int)(singleDuration / fdT)
                            };
                            ecb.AddComponent(sortKey, ins, new TimeToDieData
                            {
                                duration = specialEffectData.duration
                            });
                            // ecb.SetComponent(sortKey, ins, new JiYuFrameAnimSpeed
                            // {
                            //     value = singleDuration
                            // });
                            ecb.SetComponent(sortKey, ins, new JiYuFrameAnimLoop
                            {
                                // value = specialEffect0.loopType == 1 ? 0 : 1
                                value = 0
                            });
                            ecb.SetComponent(sortKey, ins, new JiYuSort
                            {
                                value = new int2(specialEffect0.zSort, specialEffect0.zIndex)
                            });
                            ecb.SetComponent(sortKey, ins, prefab0SpecialEffect);
                            ecb.AddComponent(sortKey, ins, new PostTransformMatrix
                            {
                                Value = float4x4.Scale(specialEffect0.sizeX / 10000f * scaleMulti,
                                    math.length(0.01f) * scaleMulti, 1)
                            });
                        }

                        for (int i = 0; i < buffer.Length; i++)
                        {
                            if (!storageInfoFromEntity.Exists(buffer[i].Entity))
                            {
                                continue;
                            }

                            var ins1 = ecb.Instantiate(sortKey, prefab1);
                            var carrierTran = cdfeLocalTransform[buffer[i].Entity];
                            var prefabTran = cdfeLocalTransform[prefab1];
                            float3 offset = default;
                            //float scaleMulti = 1f;


                            switch (specialEffect1.para2)
                            {
                                case 1:
                                    scaleMulti = 1f;
                                    break;
                                case 2:
                                    scaleMulti = 1f / carrierTran.Scale;
                                    break;
                            }


                            ecb.AddComponent(sortKey, ins1, new Parent
                            {
                                Value = buffer[i].Entity
                            });
                            ecb.AppendToBuffer(sortKey, buffer[i].Entity, new LinkedEntityGroup
                            {
                                Value = ins1
                            });

                            switch (specialEffect1.offset)
                            {
                                case 0:
                                    offset = float3.zero;
                                    break;
                                case 1:
                                    offset.y = 1;
                                    break;
                                case 2:
                                    offset.x = -1;
                                    break;
                                case 3:
                                    offset.x = 1;
                                    break;
                                case 4:
                                    offset.y = -1;
                                    break;
                                case 5:
                                    offset = MathHelper.Forward(carrierTran.Rotation);
                                    //offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                            }

                            //oldTran.Position = offset * (carrierTran.Scale / 2f);
                            var offsetPara = specialEffect1.offsetPara.Length > 0
                                ? specialEffect1.offsetPara[0] / 1000f
                                : 0;
                            if (specialEffect1.para3 == 1)
                            {
                                prefabTran.Rotation = carrierTran.Rotation;
                            }

                            prefabTran.Position = offset * (offsetPara) * scaleMulti;
                            //ecb.SetComponent(sortkey, ins, oldTran);


                            var prefab1SpecialEffect = new SpecialEffectData
                            {
                                type = 1,
                                id = specialEffectData.id * 10 + 1,
                                stateId = 0,
                                groupId = 0,
                                sort = 0,
                                caster = default,
                                tick = 0,
                                skillId = 0,
                                startPos = default,
                                targetPos = default,
                                duration = (specialEffect1.loopSpeed / 1000f) / gameTimeData.logicTime.gameTimeScale,
                                chainCenterEntity = default,
                                chainLastEntity = default,
                                chainNextEntity = default,
                                startTick = i * (int)(singleDuration / fdT)
                            };
                            ecb.AddComponent(sortKey, ins1, new TimeToDieData
                            {
                                duration = singleDuration * i + specialEffect1.maxTime / 1000f
                            });
                            ecb.SetComponent(sortKey, ins1, new JiYuFrameAnimSpeed
                            {
                                value = 0
                            });
                            ecb.SetComponent(sortKey, ins1, new JiYuFrameAnimLoop
                            {
                                value = specialEffect1.loopType == 1 ? 0 : 1
                            });
                            ecb.SetComponent(sortKey, ins1, new JiYuSort
                            {
                                value = new int2(specialEffect1.zSort, specialEffect1.zIndex)
                            });
                            ecb.SetComponent(sortKey, ins1, prefab1SpecialEffect);
                            if (specialEffect1.sizeX != specialEffect1.sizeY)
                            {
                                ecb.AddComponent(sortKey, ins1, new PostTransformMatrix
                                {
                                    Value = float4x4.Scale(specialEffect1.sizeX / 10000f * scaleMulti,
                                        specialEffect1.sizeY / 10000f * scaleMulti, 1)
                                });
                            }
                            else
                            {
                                prefabTran.Scale = specialEffect1.sizeX / 10000f * scaleMulti;
                                ecb.SetComponent(sortKey, ins1, prefabTran);
                            }


                            // ecb.AddComponent(sortKey, ins1, new PostTransformMatrix
                            // {
                            //     Value = float4x4.Scale(specialEffect.sizeX / 10000f * scaleMulti,
                            //         math.length(2) * scaleMulti, 1)
                            // });
                        }

                        ecb.AddComponent(sortKey, e, new TimeToDieData
                        {
                            duration = 5
                        });
                        //bfeSpecialEffectChainBuffer[e] = buffer;
                    }
                }

                if (specialEffect.loopType == 3)
                {
                    var list = new NativeList<AnimationSegment>(Allocator.Temp);
                    int loopIndex = -1;
                    for (int i = 0; i < specialEffect.loopExtraPara.Length; i++)
                    {
                        var para = specialEffect.loopExtraPara[i];
                        if (para.y == 1 && loopIndex == -1)
                        {
                            loopIndex = i;
                        }
                    }

                    for (int i = 0; i < specialEffect.loopExtraPara.Length; i++)
                    {
                        var para = specialEffect.loopExtraPara[i];

                        list.Add(new AnimationSegment
                        {
                            StartFrame = i == 0 ? 1 : specialEffect.loopExtraPara[i - 1].x,
                            EndFrame = para.x,
                            Loop = para.y == 1 ? true : false,
                            DurationMs = para.z
                        });
                    }

                    var index = GetCurrentFrame(list, specialEffectData.tick);
                    list.Dispose();
                    var animIndex = cdfeJiYuAnimIndex[e];
                    animIndex.value = index;
                    cdfeJiYuAnimIndex[e] = animIndex;
                }


                cdfeLocalTransform[e] = tran;
                specialEffectData.tick++;
            }
        }


        /// <summary>
        /// 根据给定的tick值和动画序列，计算当前应该播放的序列帧图int值。
        /// </summary>
        /// <param name="animationSegments">解析后的动画段列表。</param>
        /// <param name="currentInputTick">当前的int型tick值，每0.02秒增加一次。</param>
        /// <returns>当前应该播放的帧数，如果超出范围则返回-1或最后一帧。</returns>
        public static int GetCurrentFrame(NativeList<AnimationSegment> animationSegments, int currentInputTick)
        {
            // 将输入的tick值转换为毫秒
            // 每0.02秒 = 20毫秒
            long currentTickMs = (long)currentInputTick * 20;

            long accumulatedTimeMs = 0;

            foreach (var segment in animationSegments)
            {
                // 如果当前tick在当前非循环段内
                if (!segment.Loop)
                {
                    if (currentTickMs < accumulatedTimeMs + segment.DurationMs)
                    {
                        // 计算在该段内已经过去的时间
                        long timeInSegment = currentTickMs - accumulatedTimeMs;
                        // 计算每帧的持续时间
                        double msPerFrame = (double)segment.DurationMs / segment.FrameCount;
                        // 计算当前段内的帧索引
                        int frameIndexInSegment = (int)(timeInSegment / msPerFrame);
                        // 避免浮点数精度问题导致的越界
                        if (frameIndexInSegment >= segment.FrameCount)
                        {
                            frameIndexInSegment = segment.FrameCount - 1;
                        }

                        return segment.StartFrame + frameIndexInSegment;
                    }

                    accumulatedTimeMs += segment.DurationMs; // 累加非循环段的持续时间
                }
                // 如果是循环段
                else
                {
                    // 如果当前tick已经到达或超过此循环段的开始时间
                    if (currentTickMs >= accumulatedTimeMs)
                    {
                        // 计算进入循环段的时间点
                        long timeIntoLoopSegment = currentTickMs - accumulatedTimeMs;

                        // 计算每帧的持续时间
                        double msPerFrame = (double)segment.DurationMs / segment.FrameCount;

                        // 计算在循环周期内的时间
                        long timeInCycle = timeIntoLoopSegment % segment.DurationMs;

                        // 计算当前循环周期内的帧索引
                        int frameIndexInSegment = (int)(timeInCycle / msPerFrame);

                        // 避免浮点数精度问题导致的越界
                        if (frameIndexInSegment >= segment.FrameCount)
                        {
                            frameIndexInSegment = segment.FrameCount - 1;
                        }

                        return segment.StartFrame + frameIndexInSegment;
                    }
                }
            }

            // 如果所有动画段都播放完毕，返回最后一个动画段的最后一帧
            if (animationSegments.Length > 0)
            {
                return animationSegments[animationSegments.Length - 1].EndFrame;
            }

            return -1; // 没有动画段
        }
    }

    // 定义动画段的数据结构
    public struct AnimationSegment
    {
        public int StartFrame;
        public int EndFrame;
        public bool Loop;
        public int DurationMs; // 毫秒
        public int StartFrameTime;
        public int EndFrameTime;

        public AnimationSegment(int startFrame, int endFrame, bool loop, int durationMs, int startFrameTime,
            int endFrameTime)
        {
            StartFrame = startFrame;
            EndFrame = endFrame;
            Loop = loop;
            DurationMs = durationMs;
            StartFrameTime = startFrameTime;
            EndFrameTime = endFrameTime;
        }

        // 获取该段的总帧数
        public int FrameCount => EndFrame - StartFrame + 1;
    }
}
//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-25 11:00:25
//---------------------------------------------------------------------

using cfg.blobstruct;
using cfg.config;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;
using UnityEngine;


namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct BulletCastSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<Skill>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var timeLineQuery = SystemAPI.QueryBuilder().WithAll<Skill>().Build();
            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
            var stateQuery = SystemAPI.QueryBuilder().WithAll<PhysicsCollider>().WithNone<Prefab>().Build();
            var targetDataQuery = SystemAPI.QueryBuilder().WithAll<TargetData>().WithNone<Prefab>()
                .WithNone<SkillAttackData>().Build();

            var skillAtackQuery = SystemAPI.QueryBuilder().WithAll<SkillAttackData>().WithNone<Prefab>().Build();
            var player = SystemAPI.GetSingletonEntity<PlayerData>();
            //UnityEngine.Time.timeSinceLevelLoad
            new BulletCastJob
            {
                fdT = SystemAPI.Time.fixedDeltaTime,
                eT = (float)SystemAPI.Time.ElapsedTime,
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(true),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
                cdfeSpiritData = SystemAPI.GetComponentLookup<SpiritData>(true),
                ecb = ecb,
                player = player,
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(),
                cdfePhysicsCollider = SystemAPI.GetComponentLookup<PhysicsCollider>(true),
                gameRandomData = gameRandomData,
                prefabMapData = prefabMapData,
                gameSetUpData = gameSetUpData,
                bfeStatefulTriggerEvent = SystemAPI.GetBufferLookup<StatefulTriggerEvent>(true),
                allEntities = stateQuery.ToEntityArray(Allocator.TempJob),
                targetEntities = targetDataQuery.ToEntityArray(Allocator.TempJob),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                globalConfigData = globalConfigData,
                gameTimeData = gameTimeData,
                cdfeTargetData = SystemAPI.GetComponentLookup<TargetData>(true),
                cdfeBulletTempTag = SystemAPI.GetComponentLookup<BulletTempTag>(true),
                cdfeBulletSonData = SystemAPI.GetComponentLookup<BulletSonData>(true),
                cdfePostTransformMatrix = SystemAPI.GetComponentLookup<PostTransformMatrix>(true),
                cdfeJiYuFrameAnimLoop = SystemAPI.GetComponentLookup<JiYuFrameAnimLoop>(true),
                skillAtackDatas = skillAtackQuery.ToComponentDataArray<SkillAttackData>(Allocator.TempJob),
                skillAtackDatasEntity = skillAtackQuery.ToEntityArray(Allocator.TempJob),
                cdfeBuff = SystemAPI.GetBufferLookup<Buff>(true),
                gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe),
                cdfeFlipData = SystemAPI.GetComponentLookup<FlipData>(true),
            }.ScheduleParallel();
        }


        [BurstCompile]
        public partial struct BulletCastJob : IJobEntity
        {
            public float fdT;
            public float eT;
            [ReadOnly] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
            [ReadOnly] public ComponentLookup<SpiritData> cdfeSpiritData;
            public EntityCommandBuffer.ParallelWriter ecb;

            [ReadOnly] public Entity player;
            public GameRandomData gameRandomData;
            [ReadOnly] public GameOthersData gameOthersData;
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GameSetUpData gameSetUpData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public GameTimeData gameTimeData;
            [NativeDisableParallelForRestriction] public ComponentLookup<ChaStats> cdfeChaStats;
            [ReadOnly] public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
            [ReadOnly] public ComponentLookup<TargetData> cdfeTargetData;
            [ReadOnly] public ComponentLookup<BulletTempTag> cdfeBulletTempTag;
            [ReadOnly] public ComponentLookup<BulletSonData> cdfeBulletSonData;
            [ReadOnly] public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;
            [ReadOnly] public ComponentLookup<JiYuFrameAnimLoop> cdfeJiYuFrameAnimLoop;
            [ReadOnly] public BufferLookup<StatefulTriggerEvent> bfeStatefulTriggerEvent;
            [ReadOnly] public ComponentLookup<FlipData> cdfeFlipData;

            [ReadOnly] public BufferLookup<Buff> cdfeBuff;
            //[ReadOnly] public DynamicBuffer<PlayerEquipSkillBuffer> bufferSkillInfo;

            [DeallocateOnJobCompletion] [NativeDisableParallelForRestriction]
            public NativeArray<SkillAttackData> skillAtackDatas;

            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> skillAtackDatasEntity;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> allEntities;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> targetEntities;

            public EntityStorageInfoLookup storageInfoFromEntity;

            public void Execute([EntityIndexInQuery] int index, Entity entity,
                ref DynamicBuffer<BulletCastData> bulletBuffer)
            {
                ref var bulletsConfig =
                    ref globalConfigData.value.Value.configTbbullets.configTbbullets;
                ref var skillEffectConfig =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                int curTick = gameTimeData.logicTime.tick;

                for (int j = 0; j < bulletBuffer.Length; j++)
                {
                    var bulletData = bulletBuffer[j];
                    int bulletIndex = -1;
                    for (int i = 0; i < bulletsConfig.Length; i++)
                    {
                        if (bulletsConfig[i].id == bulletData.id)
                        {
                            bulletIndex = i;
                            break;
                        }
                    }

                    if (bulletIndex == -1)
                    {
                        Debug.Log($"当前弹幕ID 弹幕表不存在 bulletId:{bulletData.id}");
                        continue;
                    }

                    ref var bullets =
                        ref bulletsConfig[bulletIndex];

                    int skillEffectIndex = -1;
                    for (int i = 0; i < skillEffectConfig.Length; i++)
                    {
                        if (skillEffectConfig[i].id == bullets.skillEffect)
                        {
                            skillEffectIndex = i;
                            break;
                        }
                    }


                    if (bulletIndex == -1)
                    {
                        Debug.Log($"当前skillEffect skillEffectId:{bullets.skillEffect}");
                        //continue;
                    }

                    if (skillEffectIndex < 0)
                    {
                        for (int i = 0; i < skillEffectConfig.Length; i++)
                        {
                            if (skillEffectConfig[i].id == bullets.deadEffect)
                            {
                                skillEffectIndex = i;
                                break;
                            }
                        }
                    }

                    if (skillEffectIndex < 0)
                    {
                        Debug.Log($"弹幕表双效果在skillNew表找不到，配置错误");
                        continue;
                    }

                    ref var skillEffect =
                        ref skillEffectConfig[skillEffectIndex];


                    if (bulletData.tick == 0)
                    {
                        //bulletData.duration = math.max(0.1f, bulletData.duration);
                        if (bullets.startType == 1)
                        {
                            var delay = bullets.startPara.Length >= 1 ? bullets.startPara[0] / 1000f : 0;
                            bulletData.duration += delay;
                            bulletData.delay = delay;
                            bulletBuffer[j] = bulletData;
                        }
                        //bulletData.duration = math.clamp((math.ceil(bullets.num / (float)bullets.groupNum) *
                        //                                  (bullets.interval / 1000f)), 0, bulletData.duration);
                    }

                    if (bulletData.duration < 0f)
                    {
                        Debug.Log($"命令id: {bulletData.id} 销毁 ");

                        bulletBuffer[j] = bulletData;
                        bulletBuffer.RemoveAt(j);
                        continue;
                    }

                    if (bulletData.delay > 0)
                    {
                        bulletData.delay -= 0.02f;
                        bulletBuffer[j] = bulletData;
                        continue;
                    }

                    //Entity prefab = default;
                    //LocalTransform prefabTran = default;
                    //float duration = default;

                    //var prefab = prefabMapData.prefabHashMap[(FixedString128Bytes)bullets.model];
                    if (!prefabMapData.prefabHashMap.TryGetValue((FixedString128Bytes)bullets.model, out var prefab))
                    {
                        continue;
                    }


                    var prefabTran = cdfeLocalTransform[prefab];
                    int rotationStart = bullets.rotationStart;
                    float duration = bullets.time / 1000f;
                    int hp = bullets.hp == 0 ? MathHelper.MaxNum : bullets.hp;
                    float speed = bullets.speed / 1000f;
                    float groupNum = bullets.groupNum;
                    float bulletFireInternal = bullets.interval / 1000f;
                    float degree = bullets.degree;
                    int triggerID = bullets.skillEffect;
                    int trackType = bullets.trackType;
                    int isAbsorb = bullets.absorbYn;
                    if (cdfeBuff.HasBuffer(entity))
                    {
                        var buffs = cdfeBuff[entity];
                        foreach (var buff in buffs)
                        {
                            if (buff.CurrentTypeId == Buff.TypeId.Buff_999112)
                            {
                                isAbsorb = 0;
                                break;
                            }
                        }
                    }

                    //int targetInt = bullets.target[0];
                    ref var targetParlist = ref bullets.target;
                    float targetInt = 0;
                    for (int k = 0; k < targetParlist.Length; k++)
                    {
                        targetInt += math.pow(2, targetParlist[k]);
                    }

                    int size = bullets.size;
                    var isStayTrigger = bullets.atkType > 0 ? true : false;
                    var rotateSpeed = bullets.rotationSpeed;
                    int3 trackPar = new int3(bullets.trackTypePara1, bullets.trackTypePara2, bullets.trackTypePara3);
                    var searthType = bullets.searchType;
                    var startType = bullets.startType;
                    var speedAdd = bullets.speedAdd;
                    ref var startPar = ref bullets.startPara;
                    ref var searchPar = ref bullets.searchPara;
                    var random = gameRandomData.rand;
                    const float MaxAngle = 80f;
                    const float MaxAmmo = 8;
                    const float SpreadDegrees = 20f;
                    //float angleInterval = MaxAngle / (MaxAmmo - 1);
                    float angleInterval = default;

                    float bulletRatios = 1f;
                    if (cdfeChaStats.HasComponent(bulletData.caster))
                    {
                        bulletRatios = (1 + cdfeChaStats[bulletData.caster].chaProperty.defaultBulletRangeRatios /
                            10000f);
                    }

                    var skillAttackData = new SkillAttackData
                    {
                        data = new SkillAttack
                        {
                            CurrentTypeId = (SkillAttack.TypeId)trackType,
                            Int32_0 = bulletData.id,
                            Single_1 = duration,
                            Int32_2 = default,
                            Entity_3 = bulletData.caster,
                            Boolean_4 = true,
                            Int32_5 = hp,
                            Entity_6 = default,
                            int4_7 = default,
                            Single_8 = speed,
                            Int32_9 = triggerID,
                            Boolean_10 = isAbsorb == 0
                                ? false
                                : true,
                            Int32_11 = (int)targetInt,
                            Boolean_12 = isStayTrigger,
                            Single_13 = rotateSpeed,
                            float3_14 = trackPar,
                            float3_15 = bulletData.localTransform.Position,
                            Entity_16 = bulletData.followedEntity,
                            Int32_22 = bullets.deadEffect,
                            Single_23 = bullets.atkType / 1000f,
                            Int32_25 = curTick,
                            Int32_29 = speedAdd,
                        }
                    };
                    //分裂弹幕根弹幕
                    if (trackType == 6 && !cdfeBulletTempTag.HasComponent(entity))
                    {
                        Debug.Log($"分裂弹幕根弹幕 {skillEffect.skillId} {(int)math.round(eT)}");
                        skillAttackData = new SkillAttackData
                        {
                            data = new SkillAttack_6
                            {
                                id = bulletData.id,
                                duration = duration,
                                tick = 0,
                                caster = bulletData.caster,
                                isBullet = true,
                                hp = hp,
                                target = default,
                                args = default,
                                speed = speed,
                                triggerID = triggerID,
                                isAbsorb = isAbsorb == 0
                                    ? false
                                    : true,
                                targetInt = (int)targetInt,
                                isOnStayTrigger = isStayTrigger,
                                rotateSpeed = rotateSpeed,
                                trackPar = trackPar,
                                targetPos = bulletData.localTransform.Position,
                                skillID = skillEffect.skillId,
                                addTime = gameTimeData.logicTime.tick,
                                deadEffectID = bullets.deadEffect,
                                onStayTriggerCd = bullets.atkType / 1000f,
                                curTick = curTick,
                                acceleration = speedAdd,
                            }.ToSkillAttack()
                        };
                    }

                    //分裂弹幕所发射的小弹幕
                    if (trackType == 6 && cdfeBulletTempTag.HasComponent(entity))
                    {
                        skillAttackData = new SkillAttackData
                        {
                            data = new SkillAttack_6
                            {
                                id = bulletData.id,
                                duration = duration,
                                tick = 0,
                                caster = bulletData.caster,
                                isBullet = true,
                                hp = hp,
                                target = default,
                                args = default,
                                speed = speed,
                                triggerID = triggerID,
                                isAbsorb = isAbsorb == 0
                                    ? false
                                    : true,
                                targetInt = (int)targetInt,
                                isOnStayTrigger = isStayTrigger,
                                rotateSpeed = rotateSpeed,
                                trackPar = trackPar,
                                targetPos = bulletData.localTransform.Position,
                                skillID = bulletData.skillId,
                                addTime = bulletData.addTime,
                                deadEffectID = bullets.deadEffect,
                                onStayTriggerCd = bullets.atkType / 1000f,
                                curTick = curTick,
                                acceleration = speedAdd,
                            }.ToSkillAttack()
                        };

                        int sonBulletNum = bulletData.sonBulletNum;
                        float sonBulletRange = bulletData.sonBulletRange;
                        foreach (var targetEntity in targetEntities)
                        {
                            if (sonBulletNum <= 0)
                            {
                                break;
                            }

                            if (!storageInfoFromEntity.Exists(targetEntity))
                                continue;

                            var targetData = cdfeTargetData[targetEntity];
                            if (cdfeBulletSonData.HasComponent(targetEntity))
                            {
                                var bulletSonData = cdfeBulletSonData[targetEntity];
                                if (bulletSonData.Equals(bulletData.caster, bulletData.skillId, bulletData.addTime))
                                {
                                    continue;
                                }
                            }

                            if (!PhysicsHelper.IsTargetEnabled((uint)targetInt, targetData.BelongsTo))
                            {
                                continue;
                            }

                            var targetTran = cdfeLocalTransform[targetEntity];
                            var entityTran = cdfeLocalTransform[entity];
                            var dis = math.length(targetTran.Position -
                                                  entityTran.Position);

                            if (dis <= sonBulletRange)
                            {
                                Debug.Log($"dis{dis} sonBulletRange{sonBulletRange}");
                                //Debug.Log($"sonBulletNum {sonBulletNum}  bullets.model{bullets.model}");

                                sonBulletNum--;
                                var dir = math.normalizesafe(targetTran.Position -
                                                             entityTran.Position);
                                // if (gameTimeData.logicTime.gameTimeScale < math.EPSILON)
                                //     continue;
                                var ins = ecb.Instantiate(index, prefab);
                                ecb.AddComponent(index, ins, new BulletData
                                {
                                    caster = skillAttackData.data.Entity_3
                                });
                                ecb.AddBuffer<LinkedEntityGroup>(index, entity);
                                ecb.AppendToBuffer(index, entity, new LinkedEntityGroup
                                {
                                    Value = ins
                                });
                                if (cdfeJiYuFrameAnimLoop.HasComponent(prefab))
                                {
                                    ecb.SetComponent(index, ins, new JiYuFrameAnimSpeed
                                    {
                                        value = (bullets.videoTime / 1000f) / gameTimeData.logicTime.gameTimeScale
                                    });
                                    ecb.SetComponent(index, ins, new JiYuFrameAnimLoop
                                    {
                                        value = 1
                                    });
                                }

                                var qua = MathHelper.LookRotation2D(dir);

                                if (cdfePostTransformMatrix.HasComponent(prefab))
                                {
                                    var postTransform = cdfePostTransformMatrix[prefab];
                                    postTransform.Value.c0.x *= (size / 1000f) * bulletRatios;
                                    postTransform.Value.c1.y *= (size / 1000f) * bulletRatios;
                                    ecb.SetComponent(index, ins, new PostTransformMatrix
                                    {
                                        Value = postTransform.Value
                                    });
                                }

                                var tran = new LocalTransform
                                {
                                    Position = cdfeLocalTransform[entity].Position,
                                    Scale = (size / 1000f) * bulletRatios,
                                    Rotation = qua
                                };
                                skillAttackData.data.float3_30 = math.mul(qua, MathHelper.picForward);
                                tran = tran.RotateZ(math.radians(-rotationStart));
                                ecb.SetComponent(index, ins, tran);

                                ecb.AddComponent(index, ins, skillAttackData);
                                ecb.AddComponent(index, ins, new TargetData
                                {
                                    tick = 0,
                                    AttackWith = (uint)(targetInt),
                                    BelongsTo = 64
                                });
                            }
                        }
                    }
                    else
                    {
                        if (math.abs(degree) > 0)
                        {
                            angleInterval = degree;
                        }

                        var intervalCount = groupNum - 1;
                        var beginAngel = intervalCount * (-angleInterval / 2f);

                        if (!ReturnDirAndTargetPos(ref bulletData, ref bullets, entity,
                                out float3 targetPos, out float3 dir, out float3 castPos))
                        {
                            bulletData.duration -= fdT;
                            bulletData.tick++;
                            Debug.LogError("dfdfdffffffffffffffffffffff");
                            bulletBuffer[j] = bulletData;
                            continue;
                        }

                        skillAttackData.data.Entity_16 = bulletData.followedEntity;
                        skillAttackData.data.float3_15 = bulletData.localTransform.Position;
                        dir = math.normalizesafe(dir);

                        var qua = MathHelper.LookRotation2D(dir);
                        UnityEngine.Debug.Log($"fllowentity11111:{skillAttackData.data.Entity_16.Index}");
                        if (bulletFireInternal <= 0)
                        {
                            //skillAttackData.data.float3_15 = targetPos;
                            //skillAttackData.data.Entity_16 = entity;

                            var loc = new LocalTransform
                            {
                                Position = castPos,
                                Scale = (size / 1000f) * bulletRatios,
                            };

                            // Debug.Log($"123123 {bulletData.duration}");
                            for (int i = 0; i < groupNum; i++)
                            {
                                if (startType == 3)
                                {
                                    var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(eT * 1000 + i));
                                    var dis = gameRandomData.rand.NextFloat(bullets.startPara[2] / 1000f,
                                        bullets.startPara[1] / 1000f);
                                    var pos = cdfeLocalTransform[entity].Position + dis * math.normalize(dir);
                                    float randomR = rand.NextFloat(0, bullets.startPara[0] / 1000f / 2);
                                    float randomAngle = rand.NextFloat(0f, 180.1f); // 生成随机角度
                                    float randomX = pos.x + randomR * math.cos(randomAngle); // 计算随机点的 x 坐标
                                    float randomY = pos.y + randomR * math.sin(randomAngle); // 计算随机点的 y 坐标
                                    loc.Position = new float3(randomX, randomY, 0);
                                }
                                else if (startType == 4)
                                {
                                    loc.Position = BuffHelper.GetRandomPointInCircle(targetPos,
                                        bullets.startPara[0] / 1000f, bullets.startPara[1] / 1000f,
                                        (uint)(eT * 1000 + i));
                                }

                                // Debug.Log($"groupNumgroupNum{groupNum},id:{bullets.id}");
                                // if (gameTimeData.logicTime.gameTimeScale < math.EPSILON)
                                //     continue;
                                var ins = ecb.Instantiate(index, prefab);
                                if (trackType == 11)
                                {
                                    var centerPos = cdfeLocalTransform[entity].Position;
                                    //float angel = 360f / groupNum;
                                    var tempPos = centerPos + new float3(0, 1, 0) * trackPar[0] / 1000f;
                                    loc.Position = centerPos +
                                                   new float3(
                                                       MathHelper.RotateVector(new float2(tempPos.xy),
                                                           (360 / groupNum) * i).xy, 0) * trackPar[0] / 1000f;
                                    //Debug.Log($"position:{loc.Position}");
                                    //ecb.AppendToBuffer<Child>(index, entity, new Child { Value=ins });
                                    //ecb.AddComponent(index,ins,new Parent { Value=entity });
                                }

                                ecb.AddComponent(index, ins, new BulletData
                                {
                                    caster = skillAttackData.data.Entity_3
                                });
                                ecb.AppendToBuffer(index, entity, new LinkedEntityGroup
                                {
                                    Value = ins
                                });
                                if (cdfeJiYuFrameAnimLoop.HasComponent(prefab))
                                {
                                    ecb.SetComponent(index, ins, new JiYuFrameAnimSpeed
                                    {
                                        value = (bullets.videoTime / 1000f) / gameTimeData.logicTime.gameTimeScale
                                    });
                                    ecb.SetComponent(index, ins, new JiYuFrameAnimLoop
                                    {
                                        value = 1
                                    });
                                }

                                quaternion bulletQ =
                                    quaternion.EulerXYZ(0, 0, math.radians(beginAngel + i * angleInterval));

                                loc.Rotation = math.mul(bulletQ, qua);
                                skillAttackData.data.float3_30 = math.mul(loc.Rotation, MathHelper.picForward);

                                loc = loc.RotateZ(math.radians(-rotationStart));
                                if (cdfePostTransformMatrix.HasComponent(prefab))
                                {
                                    var postTransform = cdfePostTransformMatrix[prefab];
                                    postTransform.Value.c0.x *= (size / 1000f) * bulletRatios;
                                    postTransform.Value.c1.y *= (size / 1000f) * bulletRatios;
                                    ecb.SetComponent(index, ins, new PostTransformMatrix
                                    {
                                        Value = postTransform.Value
                                    });
                                }

                                ecb.SetComponent(index, ins, loc);

                                ecb.AddComponent(index, ins, skillAttackData);
                                ecb.AddComponent(index, ins, new TargetData
                                {
                                    tick = 0,
                                    AttackWith = (uint)(targetInt),
                                    BelongsTo = 64
                                });
                            }
                        }
                        else
                        {
                            var loc = new LocalTransform
                            {
                                Position = castPos,
                                Scale = (size / 1000f) * bulletRatios
                            };
                            //if (startType == 5)
                            //{
                            //    //弧度
                            //    var angle = startPar[1] * 1f / startPar[0];
                            //    loc = loc.RotateZ(-angle);
                            //    Debug.Log($"castPos{loc.Position}");
                            //}

                            if (bulletData.tick % (uint)(bulletFireInternal / fdT) == 0)
                            {
                                if (bullets.groupSearchYn == 1)
                                {
                                    bulletData.isTargeted = false;
                                }

                                bulletData.curGroup++;
                                if (searthType == 3)
                                {
                                    loc.RotateZ(-math.radians(bulletData.curGroup * searchPar[0]));
                                }

                                if (startType == 5)
                                {
                                    var dis = bullets.startPara[1] / 1000f;
                                    var dirtoPlayer = math.normalize(targetPos - castPos);
                                    loc.Position = castPos + (dis * dirtoPlayer * bulletData.curGroup);
                                    //var radians = (bullets.startPara[1] * 1f) / bullets.startPara[0];

                                    //loc = MathHelper.RotateAround(loc, targetPos,-radians);
                                    //dir=new float3(0,-1,0);
                                    Debug.Log($"castPos{loc.Position},targetpos:{targetPos},dirto:{dirtoPlayer}");
                                }

                                for (int i = 0; i < groupNum; i++)
                                {
                                    // if (gameTimeData.logicTime.gameTimeScale < math.EPSILON)
                                    //     continue;
                                    var ins = ecb.Instantiate(index, prefab);
                                    ecb.AddComponent(index, ins, new BulletData
                                    {
                                        caster = skillAttackData.data.Entity_3
                                    });
                                    ecb.AppendToBuffer(index, entity, new LinkedEntityGroup
                                    {
                                        Value = ins
                                    });
                                    if (cdfeJiYuFrameAnimLoop.HasComponent(prefab))
                                    {
                                        ecb.SetComponent(index, ins, new JiYuFrameAnimSpeed
                                        {
                                            value = (bullets.videoTime / 1000f) / gameTimeData.logicTime.gameTimeScale
                                        });
                                        ecb.SetComponent(index, ins, new JiYuFrameAnimLoop
                                        {
                                            value = 1
                                        });
                                    }

                                    quaternion bulletQ =
                                        quaternion.EulerXYZ(0, 0, math.radians(beginAngel + i * angleInterval));
                                    loc.Rotation = math.mul(bulletQ, qua);
                                    skillAttackData.data.float3_30 = math.mul(loc.Rotation, MathHelper.picForward);
                                    loc = loc.RotateZ(math.radians(-rotationStart));
                                    if (cdfePostTransformMatrix.HasComponent(prefab))
                                    {
                                        var postTransform = cdfePostTransformMatrix[prefab];
                                        postTransform.Value.c0.x *= (size / 1000f) * bulletRatios;
                                        postTransform.Value.c1.y *= (size / 1000f) * bulletRatios;
                                        ecb.SetComponent(index, ins, new PostTransformMatrix
                                        {
                                            Value = postTransform.Value
                                        });
                                    }

                                    ecb.SetComponent(index, ins, loc);
                                    Debug.Log($"castPos9999:{loc.Position}");

                                    ecb.AddComponent(index, ins, skillAttackData);
                                    ecb.AddComponent(index, ins, new TargetData
                                    {
                                        tick = 0,
                                        AttackWith = (uint)(targetInt),
                                        BelongsTo = 64
                                    });
                                }
                            }
                        }
                    }

                    bulletData.duration -= fdT;
                    bulletData.tick++;

                    bulletBuffer[j] = bulletData;
                }
            }

            private float3 ReturnCastPos(ref ConfigTbbullet bullets, ref float3 targetDir, Entity caster,
                float3 targetPos, ComponentLookup<FlipData> cdfeFlipData)
            {
                float3 castPos = default;
                int type = bullets.startType;
                ref var posPar = ref bullets.startPara;
                targetDir = math.normalizesafe(targetDir);
                if (posPar.Length <= 0)
                {
                    UnityEngine.Debug.Log("请检查配置表！！！");
                }

                switch (type)
                {
                    case 0:
                        castPos = cdfeLocalTransform[caster].Position + targetDir * posPar[0] / 1000f;


                        break;
                    case 1:
                        //case1有延迟
                        castPos = targetPos;
                        break;
                    case 2:
                        if (bullets.startPara.Length <= 1)
                        {
                            UnityEngine.Debug.Log("请检查配置表！！！");
                            return castPos;
                        }

                        int mapType = gameOthersData.mapData.mapType;

                        float mapheight = gameOthersData.mapData.mapSize.y;
                        float mapWidth = gameOthersData.mapData.mapSize.x;
                        bool isinBoss = cdfePlayerData[player].playerOtherData.isBossFight;
                        //TODO:这两个值跟地图有关
                        float3 startPos = float3.zero;
                        float3 mapCenterPos = float3.zero;
                        if (isinBoss)
                        {
                            switch (mapType)
                            {
                                case 1:
                                    mapCenterPos = new float3(1999, 0, 0);
                                    break;
                                case 2:
                                    mapCenterPos = new float3(0, 1999, 0);
                                    break;
                                case 3:
                                    mapCenterPos = new float3(1999, 0, 0);

                                    break;
                                case 4:
                                    //TODO:全开放的boss场景选点
                                    mapCenterPos = new float3(1999, 1999, 0);
                                    break;
                            }

                            mapWidth = mapheight = cdfePlayerData[player].playerOtherData.bossScenePos.y;
                            startPos = new float3(mapCenterPos.x - mapWidth / 2f, mapCenterPos.y + mapheight / 2f, 0);
                        }
                        else
                        {
                            switch (mapType)
                            {
                                case 1:
                                    var curMapIndex = (int)((cdfeLocalTransform[player].Position.y + mapWidth * 3) /
                                                            mapheight) + 1;
                                    startPos = new float3(-mapWidth / 2f, -(mapWidth * 3) + mapheight * curMapIndex, 0);
                                    mapCenterPos = new float3(startPos.x + mapWidth / 2f, startPos.y - mapheight / 2f,
                                        0);
                                    break;
                                case 3:
                                    startPos = new float3(-mapWidth / 2f, mapheight / 2f, 0);
                                    break;
                                case 4:
                                    //还没想好
                                    break;
                            }
                        }

                        UnityEngine.Debug.Log($"位置：{mapCenterPos},start:{startPos}");
                        var angle = gameRandomData.rand.NextFloat(bullets.startPara[1], bullets.startPara[0]);

                        var tempPos = MathHelper.GetPointOnCircle(mapCenterPos, mapWidth, angle);
                        if (tempPos.x < startPos.x)
                        {
                            tempPos.x = startPos.x;
                        }

                        if (tempPos.x > (startPos.x + mapWidth))
                        {
                            tempPos.x = startPos.x + mapWidth;
                        }

                        castPos = tempPos;
                        //bullets.startType[0]*math.normalizesafe()+mapPos
                        break;
                    case 3:
                        if (bullets.startPara.Length <= 2)
                        {
                            UnityEngine.Debug.Log("请检查配置表！！！");
                            return castPos;
                        }

                        var dis = gameRandomData.rand.NextFloat(bullets.startPara[2] / 1000f,
                            bullets.startPara[1] / 1000f);
                        var pos = cdfeLocalTransform[caster].Position + dis * math.normalize(targetDir);

                        var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(eT * 1000));
                        float randomR = rand.NextFloat(0, bullets.startPara[0] / 1000f / 2);
                        float randomAngle = rand.NextFloat(0f, 180.1f); // 生成随机角度

                        float randomX = pos.x + randomR * math.cos(randomAngle); // 计算随机点的 x 坐标
                        float randomY = pos.y + randomR * math.sin(randomAngle); // 计算随机点的 y 坐标
                        castPos = new float3(randomX, randomY, 0);
                        UnityEngine.Debug.Log($"castPos:{castPos}");
                        break;
                    case 4:
                        if (bullets.startPara.Length <= 1)
                        {
                            UnityEngine.Debug.Log("请检查配置表！！！");
                            return castPos;
                        }

                        castPos = BuffHelper.GetRandomPointInCircle(targetPos,
                            bullets.startPara[0] / 1000f, bullets.startPara[1] / 1000f, 1);
                        break;
                    //发射起点为5的每一组弹幕都得改一遍位置
                    case 5:
                        rand = Unity.Mathematics.Random.CreateFromIndex((uint)(eT * 1000));
                        angle = rand.NextInt(0, 360);
                        castPos = MathHelper.GetPointOnCircle(targetPos, bullets.startPara[0] / 1000f, angle);
                        var dir = math.normalizesafe(castPos - targetPos);
                        castPos = castPos + dir * bullets.startPara[2] / 1000f;
                        //发射方向垂直向下

                        targetDir = new float3(0, -1, 0);
                        //Debug.DrawLine(castPos, targetPos);
                        //Debug.Log($"castPos{castPos},targetPos:{targetPos}");
                        break;
                    case 6:
                        var horPos = (cdfeFlipData[caster].value.x > 0 ? 1 : -1) * cdfeLocalTransform[caster].Scale *
                            posPar[0] / 10000f;
                        var verPos = cdfeLocalTransform[caster].Scale * posPar[1] / 10000f;
                        castPos = new float3(cdfeLocalTransform[caster].Position.x + horPos,
                            cdfeLocalTransform[caster].Position.y + verPos, 0);
                        break;
                }

                return castPos;
            }

            private bool ReturnDirAndTargetPos(ref BulletCastData bulletData, ref ConfigTbbullet bullets, Entity entity,
                out float3 targetPos, out float3 targetDir, out float3 castPos)
            {
                targetDir = default;
                targetPos = default;
                castPos = default;
                //followedEntity = default;
                var searthType = bullets.searchType;
                ref var searchPara = ref bullets.searchPara;
                //int groupCount = bullets.num / bullets.groupNum;

                Entity followedEntity = bulletData.followedEntity;
                ref var targetParlist = ref bullets.targetLockOn;
                float lockInt = 0;
                for (int k = 0; k < targetParlist.Length; k++)
                {
                    lockInt += math.pow(2, targetParlist[k]);
                }

                if (!bulletData.isTargeted)
                {
                    float distance = 9999;
                    //Debug.Log($"searthType{searthType}");
                    bool islocatedFirst = false;
                    float3 locatedPos = Vector3.zero;
                    if (bullets.startType == 6)
                    {
                        islocatedFirst = true;
                        var horPos = (cdfeFlipData[entity].value.x > 0 ? 1 : -1) * cdfeLocalTransform[entity].Scale *
                            bullets.startPara[0] / 10000f;
                        var verPos = cdfeLocalTransform[entity].Scale * bullets.startPara[1] / 10000f;
                        locatedPos = new float3(cdfeLocalTransform[entity].Position.x + horPos,
                            cdfeLocalTransform[entity].Position.y + verPos, 0);
                    }

                    var targetLoc = BuffHelper.SeekTargets(ref searchPara, ref searchPara, searthType, -1, -1,
                        ref searchPara,
                        allEntities, entity, entity,
                        cdfeLocalTransform, cdfeTargetData, cdfeChaStats, (uint)lockInt, out targetPos,
                        out targetDir, out followedEntity, cdfeBuff, cdfeBulletTempTag, (uint)(eT * 1000), 1,
                        islocatedFirst, locatedPos);
                    bulletData.isTargeted = true;


                    //targetPos = targetLoc.Position;
                    castPos = ReturnCastPos(ref bullets, ref targetDir, entity, targetPos, cdfeFlipData);

                    //Debug.LogError($"targetPos：{targetPos}，casterpos：{castPos}");
                    bulletData.localTransform.Position = targetPos;
                    bulletData.bulletPos = castPos;
                    bulletData.followedEntity = followedEntity;

                    UnityEngine.Debug.Log($"fllowentity:{followedEntity.Index}");
                }
                else
                {
                    targetDir = bulletData.localTransform.Position - cdfeLocalTransform[entity].Position;
                    targetPos = bulletData.localTransform.Position;
                    castPos = bulletData.bulletPos;
                    //发射类型为5的强制向下发
                    if (bullets.startType == 5)
                    {
                        targetDir = new float3(0, -1, 0);
                    }
                    if (bulletData.isOrderDir && bullets.trackType == 14)
                    {
                        targetDir = bulletData.dir;
                    }
                }

                if (searthType == 1 && searchPara.Length > 1 && searchPara[1] != 1 &&
                    !storageInfoFromEntity.Exists(followedEntity))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
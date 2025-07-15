using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(HitBackAfterSystem))]
    public partial struct ConsumeDamageInfoSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<DamageInfo>();
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var mapModuleQuery = SystemAPI.QueryBuilder().WithAll<MapElementData>().Build();
            var chaQuery = SystemAPI.QueryBuilder().WithAll<ChaStats>().Build();
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);
            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            // var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer().AsParallelWriter();
            // var BulletMoveAndHitSystemJobHandle =
            //     World.GetExistingSystem<BulletMoveAndHitSystem>().BulletMoveAndHitSystemJobHandle;
            new DamageHandleJob
            {
                timeTick = (uint)(SystemAPI.Time.ElapsedTime * 1000f),
                bfeTrigger = SystemAPI.GetBufferLookup<Trigger>(),
                bfeBuff = SystemAPI.GetBufferLookup<Buff>(),
                bfeGameEvent = SystemAPI.GetBufferLookup<GameEvent>(),
                gameRandomData = gameRandomData,
                prefabMapData = prefabMapData,
                gameSetUpData = gameSetUpData,
                ecb = ecb,
                globalConfigData = globalConfigData,
                gameOthersData = gameOthersData,
                gameTimeData = gameTimeData,
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(true),
                cdfeObstacleData = SystemAPI.GetComponentLookup<ObstacleTag>(true),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
                cdfeTimeToDieData = SystemAPI.GetComponentLookup<TimeToDieData>(true),
                cdfeTargetData = SystemAPI.GetComponentLookup<TargetData>(true),
                cdfeWeaponData = SystemAPI.GetComponentLookup<WeaponData>(true),
                cdfePhysicsMass = SystemAPI.GetComponentLookup<PhysicsMass>(),
                cdfeAgentLocomotion = SystemAPI.GetComponentLookup<AgentLocomotion>(true),
                bfeDropsBuffer = SystemAPI.GetBufferLookup<DropsBuffer>(true),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                eT = (float)SystemAPI.Time.ElapsedTime,
                fdT = SystemAPI.Time.fixedDeltaTime,
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(),
                player = SystemAPI.GetSingletonEntity<PlayerData>(),
                wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>(),
                //bfeCalliByte = SystemAPI.GetBufferLookup<CalliByte>(),
                mapModules = mapModuleQuery.ToEntityArray(Allocator.TempJob),
                cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
                bfeSkill = SystemAPI.GetBufferLookup<Skill>(true),
                allEntities = chaQuery.ToEntityArray(Allocator.TempJob),
                cdfeStateMachine = SystemAPI.GetComponentLookup<StateMachine>(),
                cdfeAreaTag = SystemAPI.GetComponentLookup<AreaTag>(true),
                cdfeEntityGroupData = SystemAPI.GetComponentLookup<EntityGroupData>(true),
            }.ScheduleParallel();
        }


        [BurstCompile]
        partial struct DamageHandleJob : IJobEntity
        {
            [ReadOnly] public uint timeTick;

            [NativeDisableParallelForRestriction] public BufferLookup<Buff> bfeBuff;
            [NativeDisableParallelForRestriction] public BufferLookup<Trigger> bfeTrigger;
            [NativeDisableParallelForRestriction] public BufferLookup<GameEvent> bfeGameEvent;
            public GameRandomData gameRandomData;
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GameSetUpData gameSetUpData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public GameOthersData gameOthersData;
            [ReadOnly] public GameTimeData gameTimeData;


            [ReadOnly] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [ReadOnly] public ComponentLookup<ObstacleTag> cdfeObstacleData;
            [ReadOnly] public ComponentLookup<AreaTag> cdfeAreaTag;
            [ReadOnly] public BufferLookup<Skill> bfeSkill;
            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
            [ReadOnly] public ComponentLookup<TimeToDieData> cdfeTimeToDieData;
            [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;
            [ReadOnly] public ComponentLookup<TargetData> cdfeTargetData;
            [ReadOnly] public ComponentLookup<WeaponData> cdfeWeaponData;
            [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsMass> cdfePhysicsMass;
            [ReadOnly] public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
            [ReadOnly] public BufferLookup<DropsBuffer> bfeDropsBuffer;
            [NativeDisableParallelForRestriction] public ComponentLookup<StateMachine> cdfeStateMachine;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> mapModules;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> allEntities;
            [NativeDisableParallelForRestriction] public ComponentLookup<ChaStats> cdfeChaStats;
            public EntityCommandBuffer.ParallelWriter ecb;

            public EntityStorageInfoLookup storageInfoFromEntity;
            [ReadOnly] public float eT;
            [ReadOnly] public float fdT;
            [NativeDisableParallelForRestriction] public ComponentLookup<PlayerData> cdfePlayerData;

            [ReadOnly] public Entity player;

            [ReadOnly] public Entity wbe;

            //[ReadOnly] public ComponentLookup<MaterialMeshInfo> cdfeMaterialMeshInfo;
            [ReadOnly] public ComponentLookup<EntityGroupData> cdfeEntityGroupData;
            //[NativeDisableParallelForRestriction] public BufferLookup<CalliByte> bfeCalliByte;

            public void Execute([EntityIndexInQuery] int sortKey, Entity entity, ref DynamicBuffer<DamageInfo> damages,
                in WorldBlackBoardTag wbt)
            {
                ref var skillEffectNew =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;

                for (int i = 0; i < damages.Length; i++)
                {
                    var damage = damages[i];
                    if (!storageInfoFromEntity.Exists(damage.defender)) continue;
                    if (!cdfeChaStats.HasComponent(damage.defender)) continue;
                    //无敌时判断
                    if (cdfeChaStats[damage.defender].chaImmuneState.immuneDamage && !damage.tags.IsHeal()) continue;

                    if (cdfeChaStats[damage.defender].chaResource.hp <= 0) continue;
                    if (cdfeChaStats[damage.defender].chaProperty.shieldCount > 0)
                    {
                        var defenderCha = cdfeChaStats[damage.defender];
                        defenderCha.chaProperty.shieldCount--;
                        cdfeChaStats[damage.defender] = defenderCha;
                        continue;
                    }


                    BuffData_ReadWrite refData = new BuffData_ReadWrite
                    {
                        ecb = ecb,
                        damageInfo = damage,
                        cdfeChaStats = cdfeChaStats,
                        cdfePlayerData = cdfePlayerData,
                        skill = default,
                        defenderChaStats = cdfeChaStats[damage.defender],
                        cdfeAgentBody = default,
                        cdfeAgentLocomotion = cdfeAgentLocomotion,
                        cdfeSkill = default,
                        //sumDamage = 0,
                        cdfePhysicsCollider = default,
                        cdfePhysicsVolocity = default,
                        //cdfechaBullet = cdfechaBullet,
                        storageInfoFromEntity = storageInfoFromEntity,
                        cdfeDropsData = default,
                        buff = bfeBuff.HasBuffer(damage.defender)
                            ? bfeBuff[damage.defender]
                            : default,
                        cdfeStateMachine = cdfeStateMachine,
                        cdfeEnemyData = cdfeEnemyData,
                        cdfeLocalTransform = cdfeLocalTransform,
                        cdfePhysicsMass = cdfePhysicsMass
                    };
                    BuffData_ReadOnly inData = new BuffData_ReadOnly
                    {
                        sortkey = sortKey,
                        entity = default,
                        prefabMapData = prefabMapData,
                        cdfeLocalTransform = cdfeLocalTransform,
                        globalConfigData = globalConfigData,
                        player = player,
                        wbe = wbe,
                        cdfeSpiritData = default,
                        cdfeHitBackData = default,
                        cdfeObstacleTag = default,
                        fdT = fdT,
                        eT = 0,
                        cdfeMapElementData = cdfeMapElementData,
                        mapModules = mapModules,
                        bfeSkill = bfeSkill,
                        entities = allEntities,
                        dropsEntities = default,
                        gameRandomData = gameRandomData,
                        cdfeSpecialEffectData = default,
                        linkedEntityGroup = default,
                        cdfeWeaponData = cdfeWeaponData,
                        gameTimeData = gameTimeData,
                        gameOthersData = default,
                        enviromentData = default,
                        timeTick = timeTick,
                    };


                    //GameEventData_ReadWrite eventData_ReadWrite = new GameEventData_ReadWrite
                    //{
                    //    cdfeBuffs = bufferFromEntity,
                    //    cdfeSkills = default,
                    //    ecb = ecb
                    //};

                    //GameEventData_ReadOnly eventData_ReadOnly = new GameEventData_ReadOnly
                    //{
                    //    sortKey = 0,
                    //    config = globalConfigData,
                    //    entities = default,
                    //    selefEntity = default,
                    //    prefabMapData = prefabMapData,
                    //    player = player,
                    //    fDT = inData.fdT,
                    //    rand = gameRandomData.rand,
                    //    cdfeLocalTransform = cdfeLocalTransform,
                    //    cdfeAllStats = cdfeChaStats,
                    //    storageInfoFromEntity = storageInfoFromEntity,
                    //    cdfeEnemyData = cdfeEnemyData,
                    //    cdfePlayerData = cdfePlayerData,
                    //    currentMapStartPos = default,
                    //    currentMapIndex = 0
                    //};

                    //triggerFromEntity[damage.defender]
                    var rand = Unity.Mathematics.Random.CreateFromIndex(
                        (uint)(refData.damageInfo.defender.Index + refData.damageInfo.defender.Version +
                               sortKey + timeTick));
                    const int MaxPower = 10000;
                    int dodgeRate = math.clamp(cdfeChaStats[refData.damageInfo.defender].chaProperty.dodge, 0,
                        MaxPower);
                    bool isDodge = rand.NextInt(0, MaxPower + 1) <= dodgeRate;
                    if (isDodge)
                    {
                        var triggers = bfeTrigger[refData.damageInfo.defender];
                        for (int j = 0; j < triggers.Length; j++)
                        {
                            if (triggers[j].TriggerConditionType_7 == TriggerConditionType.Dodging)
                            {
                                var temp = triggers[j];
                                switch (temp.TriggerType_5)
                                {
                                    case TriggerType.Halo:

                                        float gap = temp.float4_6.x == 0 ? 0.5f : temp.float4_6.x;
                                        //
                                        temp.Single_15 = gap;
                                        break;
                                    case TriggerType.PerNum:
                                        temp.float4_6.z--;
                                        break;
                                    case TriggerType.PerTime:

                                        break;
                                }

                                temp.Boolean_4 = true;
                                triggers[j] = temp;
                            }
                        }

                        if (gameSetUpData.enableDamageNumber && !refData.damageInfo.disableDamageNumber)
                        {
                            //TODO:miss飘字
                            Debug.Log($"闪避成功");
                            ecb.AddComponent<DodgeAnimData>(sortKey, refData.damageInfo.defender);
                        }

                        continue;
                    }


                    // if (damage.hitRate < 1 && damage.hitRate >= 0)
                    // {
                    //     // var randNum = storageInfoFromEntity.Exists(damage.attacker)
                    //     //     ? damage.attacker.Index
                    //     //     : 0;
                    //     //TODO:闪避行为
                    //     var isHit = rand.NextFloat(0f, 1f) < damage.hitRate;
                    //     if (!isHit)
                    //     {
                    //         if (gameSetUpData.enableDamageNumber && !damage.disableDamageNumber)
                    //         {
                    //             var damEntity = prefabMapData.prefabHashMap["DamageNumber"];
                    //             var damIns = ecb.Instantiate(index, damEntity);
                    //
                    //
                    //             ecb.SetComponent(index, damIns, new DamageUnManagedData
                    //             {
                    //                 damage = new float2x4
                    //                 {
                    //                     c0 = default,
                    //                     c1 = default,
                    //                     c2 = new float2(0, eT),
                    //                     c3 = default
                    //                 }
                    //             });
                    //             ecb.SetComponent(index, damIns, new LocalTransform
                    //             {
                    //                 Position = math.length(damage.pos) < math.EPSILON
                    //                     ? cdfeLocalTransform[entity].Position
                    //                     : damage.pos,
                    //                 Scale = 0.3f,
                    //                 Rotation = quaternion.identity
                    //             });
                    //             ecb.AddComponent(index, damIns, new TimeToDieData
                    //             {
                    //                 duration = 1
                    //             });
                    //         }
                    //
                    //         continue;
                    //     }
                    // }


                    var beforeDamage = refData.damageInfo.damage;


                    var damagerefData = new DamageData_ReadWrite
                    {
                        ecb = ecb,
                        storageInfoFromEntity = storageInfoFromEntity,
                        rand = rand,
                        cdfePlayerData = cdfePlayerData,
                        cdfeChaStats = cdfeChaStats,
                    };
                    var damageinData = new DamageData_ReadOnly
                    {
                        sortkey = sortKey,
                        seed = 0,
                        player = player,
                        beforeDamage = beforeDamage,
                        bfeLinkedEntityGroup = default,
                        prefabMapData = prefabMapData,
                        globalConfigData = globalConfigData,
                        cdfeElementData = cdfeMapElementData,
                        cdfeObstacleTag = cdfeObstacleData,
                        cdfeLocalTransform = cdfeLocalTransform,

                        cdfeEnemyData = cdfeEnemyData,
                        cdfeTargetData = cdfeTargetData,
                    };
                    // if (cdfePlayerData.HasComponent(entity))
                    // {
                    //     Debug.Log($"subDamage{refData.subDamage}");
                    //     Debug.Log($"hp{  cdfeChaStats[damage.defender].chaResource.hp}");
                    // }

                    refData.damageInfo.SumDamage(ref damagerefData, in damageinData, out var isCritical);

                    //击中时
                    if (storageInfoFromEntity.Exists(refData.damageInfo.attacker) &&
                        bfeBuff.HasBuffer(refData.damageInfo.attacker))
                    {
                        for (var j = 0; j < bfeBuff[refData.damageInfo.attacker].Length; j++)
                        {
                            var Temp = bfeBuff[refData.damageInfo.attacker];
                            var buffcom = Temp[j];

                            buffcom.OnHit(ref refData, in inData);
                            Temp[j] = buffcom;
                            // var GameEvent = eventsFromEntity[refData.damageInfo.attacker];
                        }
                    }


                    if (storageInfoFromEntity.Exists(refData.damageInfo.attacker) &&
                        bfeTrigger.HasBuffer(refData.damageInfo.attacker) &&
                        refData.damageInfo.tags.weaponDamage)
                    {
                        // Debug.Log($"武器命中时");
                        var triggers = bfeTrigger[refData.damageInfo.attacker];
                        for (int j = 0; j < triggers.Length; j++)
                        {
                            if (triggers[j].TriggerType_5 == TriggerType.WeaponAttack)
                            {
                                var temp = triggers[j];
                                temp.Boolean_18 = true;
                                temp.Entity_20 = refData.damageInfo.defender;
                                triggers[j] = temp;
                            }
                        }
                    }

                    //受击时
                    if (bfeBuff.HasBuffer(refData.damageInfo.defender))
                    {
                        for (var j = 0; j < bfeBuff[refData.damageInfo.defender].Length; j++)
                        {
                            var Temp = bfeBuff[refData.damageInfo.defender];
                            var buffcom = Temp[j];
                            buffcom.OnBeHurt(ref refData, in inData);
                            Temp[j] = buffcom;
                        }
                    }

                    if (bfeTrigger.HasBuffer(refData.damageInfo.defender))
                    {
                        var triggers = bfeTrigger[refData.damageInfo.defender];
                        for (int j = 0; j < triggers.Length; j++)
                        {
                            if (triggers[j].TriggerConditionType_7 == TriggerConditionType.OnBeHert &&
                                !refData.damageInfo.tags.directHeal)
                            {
                                var totalDamage = cdfeChaStats[refData.damageInfo.defender].chaResource.totalDamage;
                                if (totalDamage <= cdfeChaStats[refData.damageInfo.defender].chaProperty.maxHp *
                                    triggers[j].int4_8.y)
                                {
                                    var temp = triggers[j];
                                    switch (temp.TriggerType_5)
                                    {
                                        case TriggerType.Halo:

                                            float gap = temp.float4_6.x == 0 ? 0.5f : temp.float4_6.x;
                                            temp.Single_15 = gap;
                                            break;
                                        case TriggerType.PerNum:
                                            //temp.float4_6.z--;
                                            break;
                                        case TriggerType.PerTime:

                                            break;
                                    }

                                    Debug.Log("触发受击");
                                    temp.Boolean_4 = true;
                                    triggers[j] = temp;
                                }
                            }
                        }
                    }

                    //if (eventsFromEntity.HasBuffer(damage.defender))
                    //{
                    //    for (var j = 0; j < eventsFromEntity[damage.defender].Length; j++)
                    //    {
                    //        //eventData_ReadOnly.hitTarget=damage.defender;
                    //        eventData_ReadOnly.selefEntity = damage.defender;
                    //        var events = eventsFromEntity[damage.defender];
                    //        var temp = events[j];
                    //        temp.OnBeHurt(ref eventData_ReadWrite, in eventData_ReadOnly);
                    //        events[j] = temp;
                    //    }
                    //}


                    if (storageInfoFromEntity.Exists(refData.damageInfo.attacker) &&
                        refData.cdfeChaStats.HasComponent(refData.damageInfo.attacker) && isCritical)
                    {
                        var attackerChaStat = refData.cdfeChaStats[refData.damageInfo.attacker];
                        attackerChaStat.chaProperty.tmpCritical = 0;
                        refData.cdfeChaStats[refData.damageInfo.attacker] = attackerChaStat;
                    }
                    // if (cdfePlayerData.HasComponent(entity))
                    // {
                    //     Debug.Log($"subDamage{refData.subDamage}");
                    //     Debug.Log($"hp{  cdfeChaStats[damage.defender].chaResource.hp}");
                    // }


                    if (refData.damageInfo.damage.normal >= refData.defenderChaStats.chaResource.hp &&
                        bfeTrigger.HasBuffer(refData.damageInfo.defender))
                    {
                        if (bfeTrigger.HasBuffer(refData.damageInfo.defender))
                        {
                            var triggers = bfeTrigger[refData.damageInfo.defender];
                            for (int j = 0; j < triggers.Length; j++)
                            {
                                if (triggers[j].TriggerConditionType_7 == TriggerConditionType.AfterRebirth)
                                {
                                    bool isBinDead = false;

                                    if (bfeBuff.HasBuffer(refData.damageInfo.defender))
                                    {
                                        var buffs = bfeBuff[refData.damageInfo.defender];
                                        foreach (var buff in buffs)
                                        {
                                            if (buff.CurrentTypeId == Buff.TypeId.Buff_BeforeBeKilled)
                                            {
                                                isBinDead = true;
                                                break;
                                            }
                                        }
                                    }


                                    //TODO:需要判断复活次数>0
                                    int sumCount = 0;
                                    if (cdfePlayerData.HasComponent(refData.damageInfo.defender))
                                    {
                                        sumCount = cdfeChaStats[refData.damageInfo.defender].chaProperty.rebirthCount +
                                                   cdfeChaStats[refData.damageInfo.defender].chaProperty.rebirthCount1 +
                                                   cdfePlayerData[refData.damageInfo.defender].playerOtherData
                                                       .rebirthCount;
                                    }
                                    else
                                    {
                                        sumCount = cdfeChaStats[refData.damageInfo.defender].chaProperty.rebirthCount +
                                                   cdfeChaStats[refData.damageInfo.defender].chaProperty.rebirthCount1;
                                    }

                                    if (sumCount > 0 && !isBinDead)
                                    {
                                        Debug.Log("触发复活");
                                        var temp = triggers[j];
                                        switch (temp.TriggerType_5)
                                        {
                                            case TriggerType.Halo:

                                                float gap = temp.float4_6.x == 0 ? 0.5f : temp.float4_6.x;
                                                temp.Single_15 = gap;
                                                break;
                                            case TriggerType.PerNum:
                                                //temp.float4_6.z--;
                                                break;
                                            case TriggerType.PerTime:
                                                //temp.Boolean_4 = true;
                                                break;
                                        }

                                        temp.Boolean_4 = true;
                                        triggers[j] = temp;
                                    }
                                }
                            }
                        }
                    }

                    //如有复活buff 可能不死 所以不执行扣血 被杀前
                    if (refData.damageInfo.damage.normal >= refData.defenderChaStats.chaResource.hp &&
                        bfeBuff.HasBuffer(refData.damageInfo.defender))
                    {
                        for (var j = 0; j < bfeBuff[refData.damageInfo.defender].Length; j++)
                        {
                            var Temp = bfeBuff[refData.damageInfo.defender];
                            var buffcom = Temp[j];
                            //复活buff 死后立即恢复20%血量 则1.修改sumDamage=0； 2.修改hp=0.2 * maxhp；

                            buffcom.OnBeforeBeKilled(ref refData, in inData);
                            Temp[j] = buffcom;
                        }
                    }


                    //确认杀敌时
                    if (refData.damageInfo.damage.normal >= cdfeChaStats[damage.defender].chaResource.hp)
                    {
                        if (storageInfoFromEntity.Exists(damage.attacker) &&
                            bfeBuff.HasBuffer(damage.attacker))
                        {
                            for (var j = 0; j < bfeBuff[damage.attacker].Length; j++)
                            {
                                var Temp = bfeBuff[damage.attacker];
                                var buffcom = Temp[j];

                                buffcom.OnKill(ref refData, in inData);
                                Temp[j] = buffcom;
                            }
                        }
                    }

                    //确认被杀时
                    if (refData.damageInfo.damage.normal >= cdfeChaStats[damage.defender].chaResource.hp)
                    {
                        if (storageInfoFromEntity.Exists(damage.defender) &&
                            bfeBuff.HasBuffer(damage.defender))
                        {
                            for (var j = 0; j < bfeBuff[damage.defender].Length; j++)
                            {
                                var Temp = bfeBuff[damage.defender];
                                var buffcom = Temp[j];

                                buffcom.OnBeKilled(ref refData, in inData);
                                Temp[j] = buffcom;
                            }

                            //技能系统死亡时回调点
                            if (bfeTrigger.HasBuffer(refData.damageInfo.defender))
                            {
                                var triggers = bfeTrigger[refData.damageInfo.defender];
                                for (int j = 0; j < triggers.Length; j++)
                                {
                                    if (triggers[j].TriggerConditionType_7 == TriggerConditionType.Deading &&
                                        triggers[j].float4_6.z > 0)
                                    {
                                        var temp = triggers[j];
                                        Debug.Log("死亡时");
                                        temp.float4_6.z--;
                                        temp.Boolean_4 = true;
                                        triggers[j] = temp;
                                    }
                                }
                            }
                        }
                    }


                    if (gameSetUpData.enableDamageNumber && !refData.damageInfo.disableDamageNumber)
                    {
                        var damIns = UnityHelper.HandleDamageType(ref ecb, sortKey, cdfeObstacleData,
                            cdfeEnemyData,
                            cdfePlayerData,
                            cdfeChaStats, prefabMapData, player, refData.damageInfo, refData.damageInfo.damage.normal,
                            isCritical, eT,
                            globalConfigData, out var damEntity);
                        if (!refData.damageInfo.tags.seckillDamage)
                        {
                            var formatDamage = UnityHelper.FormatDamage(math.abs(refData.damageInfo.damage.normal));
                            //var formatDamage = new float4x2(9, 9, 9, 9, 9, 9, 9, 9);
                            var damageNum = new float4x4
                            {
                                c0 = formatDamage.c0,
                                c1 = formatDamage.c1,
                                c2 = default,
                                c3 = default
                            };
                            ecb.SetComponent(sortKey, damIns, new JiYuNumber
                            {
                                value = damageNum
                            });
                        }

                        //ecb.SetComponent(index, damIns, damageUnManagedData);
                        // ecb.SetComponent(index, damIns, new DamageNumberStartTimeComponent
                        // {
                        //     time = eT
                        // });
                        if (refData.damageInfo.tags.weaponDamage)
                        {
                            var pre = inData.prefabMapData.prefabHashMap["HitEffect"];
                            var ins = refData.ecb.Instantiate(inData.sortkey, pre);
                            var attackTran = cdfeLocalTransform[refData.damageInfo.attacker];
                            var defenderTran = cdfeLocalTransform[refData.damageInfo.defender];
                            var dir = math.normalizesafe(attackTran.Position - defenderTran.Position);

                            ecb.AddComponent(sortKey, ins, new Parent
                            {
                                Value = refData.damageInfo.defender
                            });
                            ecb.AppendToBuffer(sortKey, refData.damageInfo.defender, new LinkedEntityGroup
                            {
                                Value = ins
                            });
                            ecb.SetComponent(sortKey, ins, new LocalTransform
                            {
                                Position = dir / 2f,
                                Scale = 1,
                                Rotation = quaternion.identity
                            });
                            ecb.AddComponent(sortKey, ins, new TimeToDieData
                            {
                                duration = 3f
                            });
                            ecb.AddComponent(sortKey, ins, new JiYuFrameAnimSpeed
                            {
                                value = 0.2f
                            });
                            ecb.AddComponent(sortKey, ins, new JiYuFrameAnimLoop
                            {
                                value = 0
                            });
                            UnityHelper.TryCreateAudioClip(globalConfigData, gameOthersData, 10001, out var _);
                            
                            
                        }

                        ecb.SetComponent(sortKey, damIns, new LocalTransform
                        {
                            Position = math.length(refData.damageInfo.pos) < math.EPSILON
                                ? cdfeLocalTransform[refData.damageInfo.defender].Position
                                : refData.damageInfo.pos,
                            Scale = cdfeLocalTransform[damEntity].Scale,
                            Rotation = cdfeLocalTransform[damEntity].Rotation
                        });
                        ecb.AddComponent(sortKey, damIns, new TimeToDieData
                        {
                            duration = 3f
                        });
                    }

                    // if (cdfePlayerData.HasComponent(entity))
                    // {
                    //     Debug.Log($"subDamage{refData.subDamage}");
                    //     Debug.Log($"hp{  cdfeChaStats[refData.damageInfo.defender].chaResource.hp}");
                    // }

                    refData.defenderChaStats.chaResource.totalDamage += refData.damageInfo.damage.normal;


                    refData.defenderChaStats.chaResource.hp -= (int)refData.damageInfo.damage.normal;
                    refData.defenderChaStats.chaResource.hp = math.clamp(
                        refData.defenderChaStats.chaResource.hp,
                        -refData.defenderChaStats.chaProperty.maxHp,
                        refData.defenderChaStats.chaProperty.maxHp);

                    // bullet maxHit--;
                    // if (storageInfoFromEntity.Exists(damage.bulletEntity) &&
                    //     damage.tags.directDamage)
                    // {
                    //     if (refData.defenderChaState.chaResource.hp + sumDamage >= 0)
                    //     {
                    //         var bullet = cdfechaBullet[damage.bulletEntity];
                    //         bullet.bullet.Int32_7--;
                    //         cdfechaBullet[damage.bulletEntity] = bullet;
                    //     }
                    // }

                    //cdfeChaStats[damage.defender] =   cdfeChaStats[damage.defender];
                    cdfeChaStats[damage.defender] = refData.defenderChaStats;


                    if (storageInfoFromEntity.Exists(damage.defender))
                    {
                        if (cdfeObstacleData.HasComponent(damage.defender) || cdfeAreaTag.HasComponent(damage.defender))
                        {
                            if (cdfeChaStats[damage.defender].chaResource.hp <= 0)
                            {
                                if (bfeDropsBuffer.HasBuffer(damage.defender))
                                {
                                    var dropsBuffer = bfeDropsBuffer[damage.defender];
                                    SpawnDropItems(damage.defender, sortKey, dropsBuffer, cdfeLocalTransform);
                                }

                                // var entityGroupData = inData.cdfeEntityGroupData[inData.entity];
                                // if (refData.storageInfoFromEntity.Exists(entityGroupData.renderingEntity))
                                // {
                                //    
                                // }

                                ecb.AddComponent(sortKey, damage.defender, new TimeToDieData
                                {
                                    duration = 5f
                                });
                                //ecb.DestroyEntity(index, damage.defender);
                                if (UnityHelper.TryGetObstacleJiYuAnimIndex(
                                        cdfeMapElementData[damage.defender].elementID, cdfeChaStats[damage.defender],
                                        globalConfigData, refData.damageInfo.damage.normal, out int animIndex,
                                        out bool isStrongShake))
                                {
                                    Debug.Log($"TryGetObstacleJiYuAnimIndex111");
                                    ecb.AddComponent(sortKey, damage.defender, new JiYuAnimIndex
                                    {
                                        value = animIndex
                                    });
                                    refData.ecb.AddComponent(inData.sortkey, damage.defender, new GetHitAnimObstacleData
                                    {
                                        type = isStrongShake ? 1 : 0
                                    });
                                }
                                else
                                {
                                    Debug.Log($"TryGetObstacleJiYuAnimIndex");
                                    refData.ecb.AddComponent<GetHitAnimObstacleData>(inData.sortkey, damage.defender);
                                }
                            }
                            else
                            {
                                if (UnityHelper.TryGetObstacleJiYuAnimIndex(
                                        cdfeMapElementData[damage.defender].elementID, cdfeChaStats[damage.defender],
                                        globalConfigData, refData.damageInfo.damage.normal, out int animIndex,
                                        out bool isStrongShake))
                                {
                                    Debug.Log($"TryGetObstacleJiYuAnimIndex111");
                                    ecb.AddComponent(sortKey, damage.defender, new JiYuAnimIndex
                                    {
                                        value = animIndex
                                    });
                                    refData.ecb.AddComponent(inData.sortkey, damage.defender, new GetHitAnimObstacleData
                                    {
                                        type = isStrongShake ? 1 : 0
                                    });
                                }
                                else
                                {
                                    Debug.Log($"TryGetObstacleJiYuAnimIndex");
                                    refData.ecb.AddComponent<GetHitAnimObstacleData>(inData.sortkey, damage.defender);
                                }
                            }
                        }
                    }
                }

                damages.Clear();


                //damages[i] = damage;
                // if (storageInfoFromEntity.Exists(entity) && cdfeChaStats[entity].chaResource.hp <= 0)
                // {
                //     if (cdfeObstacleData.HasComponent(entity))
                //     {
                //         ecb.DestroyEntity(index, entity);
                //     }
                //
                //     // if (cdfeEnemyData.HasComponent(entity) && !cdfeTimeToDieData.HasComponent(entity))
                //     // {
                //     //     var playerData = cdfePlayerData[player];
                //     //     playerData.playerData.killEnemy++;
                //     //     cdfePlayerData[player] = playerData;
                //     //
                //     //     ecb.AddComponent(index, entity, new TimeToDieData
                //     //     {
                //     //         duration = 20
                //     //     });
                //     // }
                // }

                // gameRandomData.rand = rand;
                // //Debug.Log($"{rand.state}");
                // ecb.SetComponent(index, wbe, gameRandomData);
            }


            void SpawnDropItems(Entity entity, int sortKey, DynamicBuffer<DropsBuffer> dropsBuffer,
                ComponentLookup<LocalTransform> cdfeLocalTransform)
            {
                ref var configTbbattle_drops =
                    ref globalConfigData.value.Value.configTbbattle_drops.configTbbattle_drops;
                ref var configTbbattle_items =
                    ref globalConfigData.value.Value.configTbbattle_items.configTbbattle_items;

                var random =
                    Unity.Mathematics.Random.CreateFromIndex((uint)(gameRandomData.seed +
                                                                    entity.Index +
                                                                    entity.Version +
                                                                    sortKey + timeTick));

                var list = new NativeList<DropsBuffer>(Allocator.Temp);
                // if (bfeGameEvent.HasBuffer(wbe))
                // {
                //     var wbeEvents = inData.bfeGameEvent[inData.wbe];
                //     foreach (var gameEvent in wbeEvents)
                //     {
                //         if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_24 &&
                //             gameEvent.int3_7.x == refData.enemyData.type)
                //         {
                //             list.Add(new DropsBuffer
                //             {
                //                 id = gameEvent.int3_7.y
                //             });
                //         }
                //     }
                // }

                list.AddRange(dropsBuffer.ToNativeArray(Allocator.Temp));
                bool isMulitDrop = list.Length >= 2;
                foreach (var drop in dropsBuffer)
                {
                    bool packIndYn = false;

                    for (int i = 0; i < configTbbattle_drops.Length; i++)
                    {
                        if (configTbbattle_drops[i].id == drop.id)
                        {
                            if (configTbbattle_drops[i].packIndYn == 1)
                            {
                                packIndYn = true;
                            }
                            else
                            {
                                packIndYn = false;
                            }

                            break;
                        }
                    }

                    if (packIndYn)
                    {
                        for (int i = 0; i < configTbbattle_drops.Length; i++)
                        {
                            if (configTbbattle_drops[i].id == drop.id)
                            {
                                var dropRate = configTbbattle_drops[i].packPower / 10000f;
                                var isDrop = random.NextFloat(0, 100) < dropRate;

                                if (!isDrop) break;

                                for (int j = 0; j < configTbbattle_drops[i].reward.Length; j++)
                                {
                                    var reward = configTbbattle_drops[i].reward[j];
                                    for (int k = 0; k < reward.y; k++)
                                    {
                                        FixedString128Bytes propBattlePrefab = default;
                                        for (int l = 0; l < configTbbattle_items.Length; l++)
                                        {
                                            if (configTbbattle_items[l].id == reward.x)
                                            {
                                                propBattlePrefab =
                                                    (FixedString128Bytes)configTbbattle_items[l].model;
                                                break;
                                            }
                                        }

                                        var prefab = prefabMapData.prefabHashMap[propBattlePrefab];
                                        //var prefab = inData.prefabMapData.prefabHashMap[propBattlePrefab];
                                        var ins = ecb.Instantiate(sortKey, prefab);
                                        //inData.bfeLinkedEntityGroup
                                        ecb.AppendToBuffer(sortKey, ins, new LinkedEntityGroup
                                        {
                                            Value = ins
                                        });

                                        float3 point = cdfeLocalTransform[entity].Position;
                                        if (isMulitDrop)
                                        {
                                            // 随机选择一个角度
                                            float angle = (float)(random.NextFloat() * 2 * math.PI); // 0到2π的随机数

                                            // 随机选择一个半径，范围在小圆到大圆之间
                                            float radius = (float)(random.NextFloat() * UnityHelper.DropRadius);

                                            // 使用极坐标转化为笛卡尔坐标
                                            point.x = point.x + radius * math.cos(angle);
                                            point.y = point.y + radius * math.sin(angle);
                                        }

                                        var newpos = new LocalTransform
                                        {
                                            Position = point,
                                            Scale = cdfeLocalTransform[prefab].Scale,
                                            Rotation = cdfeLocalTransform[prefab].Rotation
                                        };
                                        //ecb.SetComponent(sortKey, ins, newpos);

                                        var dropsData = new DropsData
                                        {
                                            point0 = newpos.Position,
                                            point1 = default,
                                            point2 = new float3(newpos.Position.x,
                                                newpos.Position.y, 0),
                                            point3 = default,
                                            isLooting = false,
                                            lootingAniDuration = gameOthersData.pickupDuration / 1000f,
                                            id = reward.x,
                                            dropPoint2 = newpos.Position
                                        };

                                        ecb.SetComponent(sortKey, ins, dropsData);
                                    }
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        //pack_id   pack_power
                        NativeHashMap<int, int> dropPairs = new NativeHashMap<int, int>(5, Allocator.Temp);
                        for (int i = 0; i < configTbbattle_drops.Length; i++)
                        {
                            if (configTbbattle_drops[i].id == drop.id)
                            {
                                dropPairs.TryAdd(configTbbattle_drops[i].packId, configTbbattle_drops[i].packPower);
                            }
                        }

                        var propid = MathHelper.SelectRandomItem(dropPairs, random);
                        if (propid == -1)
                        {
                            Debug.LogError($"选择随机有误");
                            break;
                        }


                        for (int i = 0; i < configTbbattle_drops.Length; i++)
                        {
                            if (configTbbattle_drops[i].packId == propid)
                            {
                                for (int j = 0; j < configTbbattle_drops[i].reward.Length; j++)
                                {
                                    var reward = configTbbattle_drops[i].reward[j];
                                    for (int k = 0; k < reward.y; k++)
                                    {
                                        FixedString128Bytes propBattlePrefab = default;
                                        for (int l = 0; l < configTbbattle_items.Length; l++)
                                        {
                                            if (configTbbattle_items[l].id == reward.x)
                                            {
                                                propBattlePrefab =
                                                    (FixedString128Bytes)configTbbattle_items[l].model;
                                                break;
                                            }
                                        }

                                        var prefab = prefabMapData.prefabHashMap[propBattlePrefab];
                                        //var prefab = inData.prefabMapData.prefabHashMap[propBattlePrefab];
                                        var ins = ecb.Instantiate(sortKey, prefab);
                                        ecb.AppendToBuffer(sortKey, ins, new LinkedEntityGroup
                                        {
                                            Value = ins
                                        });
                                        float3 point = cdfeLocalTransform[entity].Position;
                                        if (isMulitDrop)
                                        {
                                            // 随机选择一个角度
                                            float angle = (float)(random.NextFloat() * 2 * math.PI); // 0到2π的随机数

                                            // 随机选择一个半径，范围在小圆到大圆之间
                                            float radius = (float)(random.NextFloat() * UnityHelper.DropRadius);

                                            // 使用极坐标转化为笛卡尔坐标
                                            point.x = point.x + radius * math.cos(angle);
                                            point.y = point.y + radius * math.sin(angle);
                                        }

                                        var newpos = new LocalTransform
                                        {
                                            Position = point,
                                            Scale = cdfeLocalTransform[prefab].Scale,
                                            Rotation = cdfeLocalTransform[prefab].Rotation
                                        };
                                        //ecb.SetComponent(sortKey, ins, newpos);
                                        var dropsData = new DropsData
                                        {
                                            point0 = newpos.Position,
                                            point1 = default,
                                            point2 = new float3(newpos.Position.x,
                                                newpos.Position.y, 0),
                                            point3 = default,
                                            isLooting = false,
                                            lootingAniDuration = gameOthersData.pickupDuration / 1000f,
                                            id = reward.x,
                                            dropPoint2 = newpos.Position
                                        };

                                        ecb.SetComponent(sortKey, ins, dropsData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
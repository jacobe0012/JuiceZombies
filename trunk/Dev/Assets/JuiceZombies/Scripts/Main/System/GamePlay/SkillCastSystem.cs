//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-25 11:00:25
//---------------------------------------------------------------------


using FMOD.Studio;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Stateful;
using Unity.Transforms;
using Debug = UnityEngine.Debug;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct SkillCastSystem : ISystem
    {
        const float OneShotSkillMinDuration = 0.1f;

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
            var stateQuery = SystemAPI.QueryBuilder().WithAll<State, StateMachine>().WithNone<Prefab>().Build();
            var skillAtackQuery = SystemAPI.QueryBuilder().WithAll<SkillAttackData>().WithNone<Prefab>().Build();
            var mapMoudleQuery = SystemAPI.QueryBuilder().WithAll<MapElementData>().WithNone<BoardData>().Build();
            var player = SystemAPI.GetSingletonEntity<PlayerData>();
            new SkillCastSystemJob
            {
                fdT = SystemAPI.Time.fixedDeltaTime,
                eT = SystemAPI.Time.ElapsedTime,
                ecb = ecb,
                player = player,
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(true),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
                cdfeSpiritData = SystemAPI.GetComponentLookup<SpiritData>(true),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(),
                cdfeStateMachine = SystemAPI.GetComponentLookup<StateMachine>(),
                cdfeAgentBody = SystemAPI.GetComponentLookup<AgentBody>(),
                cdfeTrapTag = SystemAPI.GetComponentLookup<TrapTag>(true),
                cdfeHitBackData = SystemAPI.GetComponentLookup<HitBackData>(true),
                cdfeBuff = SystemAPI.GetBufferLookup<Buff>(),
                cdfeObstacleTag = SystemAPI.GetComponentLookup<ObstacleTag>(true),
                cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
                gameRandomData = gameRandomData,
                prefabMapData = prefabMapData,
                gameSetUpData = gameSetUpData,
                allCollisionEvent = SystemAPI.GetBufferLookup<StatefulTriggerEvent>(true),
                allEntities = stateQuery.ToEntityArray(Allocator.TempJob),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                globalConfigData = globalConfigData,
                gameTimeData = gameTimeData,
                skillAtackDatas = skillAtackQuery.ToComponentDataArray<SkillAttackData>(Allocator.TempJob),
                skillAtackDatasEntity = skillAtackQuery.ToEntityArray(Allocator.TempJob),
                mapModules = mapMoudleQuery.ToEntityArray(Allocator.TempJob),
                cdfeBossTag = SystemAPI.GetComponentLookup<BossTag>(true),
                cdfeSpecialEffectData = SystemAPI.GetComponentLookup<SpecialEffectData>(true),
                cdfeLinkedEntityGroup = SystemAPI.GetBufferLookup<LinkedEntityGroup>(true),
            }.ScheduleParallel();
        }


        [BurstCompile]
        public partial struct SkillCastSystemJob : IJobEntity
        {
            [ReadOnly] public float fdT;
            [ReadOnly] public double eT;
            [ReadOnly] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [NativeDisableParallelForRestriction] public ComponentLookup<PlayerData> cdfePlayerData;
            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
            [ReadOnly] public ComponentLookup<SpiritData> cdfeSpiritData;
            [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public ComponentLookup<BossTag> cdfeBossTag;
            [ReadOnly] public Entity player;
            public GameRandomData gameRandomData;
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GameSetUpData gameSetUpData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public GameTimeData gameTimeData;
            [NativeDisableParallelForRestriction] public ComponentLookup<ChaStats> cdfeChaStats;
            [NativeDisableParallelForRestriction] public ComponentLookup<StateMachine> cdfeStateMachine;
            [NativeDisableParallelForRestriction] public ComponentLookup<AgentBody> cdfeAgentBody;
            [ReadOnly] public ComponentLookup<SpecialEffectData> cdfeSpecialEffectData;
            [ReadOnly] public BufferLookup<StatefulTriggerEvent> allCollisionEvent;
            [ReadOnly] public ComponentLookup<TrapTag> cdfeTrapTag;
            [ReadOnly] public ComponentLookup<HitBackData> cdfeHitBackData;

            [NativeDisableParallelForRestriction] public BufferLookup<Buff> cdfeBuff;

            [ReadOnly] public BufferLookup<LinkedEntityGroup> cdfeLinkedEntityGroup;

            //[ReadOnly] public BufferLookup<PlayerEquipSkillBuffer> bfePlayerEquipSkillBuffer;
            [ReadOnly] public ComponentLookup<ObstacleTag> cdfeObstacleTag;

            [DeallocateOnJobCompletion] [NativeDisableParallelForRestriction]
            public NativeArray<SkillAttackData> skillAtackDatas;

            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> skillAtackDatasEntity;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> allEntities;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> mapModules;
            public EntityStorageInfoLookup storageInfoFromEntity;

            public void Execute([EntityIndexInQuery] int index, Entity entity, ref DynamicBuffer<Skill> skillsBuffer,
                ref DynamicBuffer<Trigger> triggers)
            {
                TimeLineData_ReadWrite refData = new TimeLineData_ReadWrite
                {
                    ecb = ecb,
                    skills = skillsBuffer,
                    storageInfoFromEntity = storageInfoFromEntity,
                    cdfeChaStats = cdfeChaStats,
                    rand = gameRandomData,
                    skillAtackDatas = skillAtackDatas,
                    cdfeAgentBody = cdfeAgentBody,
                    cdfeStateMachine = cdfeStateMachine,
                };
                TimeLineData_ReadOnly inData = new TimeLineData_ReadOnly
                {
                    sortkey = index,
                    player = player,
                    prefabMapData = prefabMapData,
                    globalConfig = globalConfigData,
                    cdfeChaStats = cdfeChaStats,
                    cdfeLocalTransform = cdfeLocalTransform,
                    cdfePlayerData = cdfePlayerData,
                    cdfeEnemyData = cdfeEnemyData,
                    cdfeSpiritData = cdfeSpiritData,
                    allTriggerEvent = allCollisionEvent,
                    allEntities = allEntities,
                    entity = entity,
                    cdfeTrapTag = cdfeTrapTag,
                    cdfeObstacleTag = cdfeObstacleTag,
                    skillAtackDatasEntity = skillAtackDatasEntity,
                    //bfePlayerEquipSkillBuffer = bfePlayerEquipSkillBuffer,
                    fdT = fdT,
                    eT = eT,
                    mapModules = mapModules,
                    cdfeMapElementData = cdfeMapElementData,
                    triggers = triggers,
                    cdfeBossTag = cdfeBossTag,
                };

                for (var i = 0; i < skillsBuffer.Length; i++)
                {
                    var temp = skillsBuffer[i];


                    //总tick为0
                    if (temp.Int32_9 == 0)
                    {
                        //switchSkillStatus(ref temp,index,entity);
                        //Boss类技能走另外逻辑


                        SwitchSkillStatus(ref temp, ref skillsBuffer);


                        //异常处理
                        //if (temp.Int32_0 == 0)
                        //{
                        //    temp.Int32_0 = (int)temp.CurrentTypeId;
                        //}

                        temp.Single_4 = temp.Single_4 == 0 ? 1f : temp.Single_4;
                        InitSkillData(ref temp, temp.Int32_0);
                        InitBackSkillData(ref ecb, index, temp.Int32_0, entity);
                        //添加trigger
                        AddTrigger(ref temp, index, temp.Int32_0, entity, triggers);

                        temp.OnAwake(ref refData, in inData);
                        DestroySpecialEffect(temp, entity, index);
                    }

                    // if (cdfePlayerData.HasComponent(entity))
                    // {
                    //     var chaStats = cdfeChaStats[entity];
                    //     if (chaStats.chaControlState.cantAttack)
                    //     {
                    //         skillsBuffer[i] = temp;
                    //         continue;
                    //     }
                    // }

                    if (cdfePlayerData.HasComponent(entity))
                    {
                        var chaStats = cdfeChaStats[entity];
                        if (chaStats.chaControlState.cantWeaponAttack)
                        {
                            if (cdfePlayerData[entity].playerOtherData.weaponSkillId == temp.Int32_0)
                            {
                                skillsBuffer[i] = temp;
                                continue;
                            }
                        }
                    }

                    if (!temp.Boolean_11)
                    {
                        for (int j = 0; j < triggers.Length; j++)
                        {
                            var trigger = triggers[j];
                            if (trigger.Int32_10 == temp.Int32_0 && !trigger.Boolean_26)
                            {
                                trigger.Boolean_11 = false;
                                triggers[j] = trigger;
                            }
                        }
                    }

                    if (temp.Boolean_11)
                    {
                        temp.Int32_9++;
                        skillsBuffer[i] = temp;
                        for (int j = 0; j < triggers.Length; j++)
                        {
                            var trigger = triggers[j];
                            if (trigger.Int32_10 == temp.Int32_0)
                            {
                                trigger.Boolean_11 = true;
                                triggers[j] = trigger;
                            }
                        }

                        continue;
                    }

                    //第一次加trigger时
                    if (temp.Int32_9 == 1 && temp.Int32_6 == 1)
                    {
                        ResetTriggerPar(ref triggers, temp);
                    }

                    //一个skill循环 重置trigger
                    if (temp.Int32_9 != 0 && temp.Int32_6 == 0)
                    {
                        ResetTriggerPar(ref triggers, temp);
                    }

                    if (temp.Int32_6 == 0)
                    {
                        var tempTimeScale = gameTimeData.logicTime.gameTimeScale;
                        if (gameTimeData.logicTime.gameTimeScale < math.EPSILON)
                        {
                            tempTimeScale = 1f;
                        }

                        var totalTime = ReturnSkillDuraion(temp.Int32_0) / tempTimeScale;
                        Debug.Log($"totalTime1:{totalTime}  id:{temp.Int32_0}");
                        if ((totalTime * 1000) % 20 != 0)
                        {
                            totalTime = totalTime - ((totalTime * 1000) % 20) / 1000f;
                        }

                        Debug.Log($"totalTime2:{totalTime}  id:{temp.Int32_0}");
                        temp.Single_3 = math.clamp(totalTime, OneShotSkillMinDuration, MathHelper.MaxNum);
                        ChangeCarrierBaTi(entity, index, temp);


                        //返回skill冷却时间
                        RetrunCoolDown(gameTimeData, entity, ref temp);
                        temp.OnCast(ref refData, in inData);

                        if (cdfePlayerData.HasComponent(entity))
                        {
                            var weaponSkillID = cdfePlayerData[entity].playerOtherData.weaponSkillId;
                            if (temp.Int32_0 == weaponSkillID)
                            {
                                RemoveAdditonDamage(entity, temp);
                                JudgeWeaponAttack(ref triggers, weaponSkillID);
                                //DoAddtionCount(entity, ref temp);
                            }
                        }


                        temp.Single_7 = temp.Single_2;
                        temp.Single_8 = temp.Single_3;
                        Debug.Log($"skillentity{temp.Int32_0} {temp.Single_3} {temp.Single_2}");
                        //temp.Boolean_16 = false;
                    }


                    if (temp.Int32_6 != 0)
                    {
                        temp.OnUpdate(ref refData, in inData);
                    }

                    // if (temp.Boolean_10)
                    // {
                    //     // UnityEngine.Debug.Log($"持续时间:{(int)(temp.Single_8)},充能时间：{(int)(temp.Int32_11/20)}");
                    //     if ((int)(temp.Single_8) % (int)(temp.Int32_11 / 20) == 0)
                    //     {
                    //         UnityEngine.Debug.Log(
                    //             $"充能成功！持续时间:{(int)(temp.Single_8)},充能时间：{(int)(temp.Int32_11 / 20)}");
                    //         //UnityEngine.Debug.Log($"充能成功！持续时间:{(int)(temp.Single_8 * 1000f)},充能时间：{temp.Int32_11}");
                    //         temp.OnChargeFinish(ref refData, inData);
                    //     }
                    // }

                    if (temp.Single_8 < 0)
                    {
                        //switchSkillStatus(ref temp, index, entity);
                        SwitchSkillStatus(ref temp, ref skillsBuffer);
                        temp.OnDestroy(ref refData, in inData);
                        // for (int j = 0; j < triggers.Length; j++)
                        // {
                        //     var trigger = triggers[j];
                        //     if (trigger.FixedList128BytesFMODStudioEventInstance_30.IsEmpty)
                        //     {
                        //         continue;
                        //     }
                        //
                        //     if (trigger.Int32_10 == temp.Int32_0)
                        //     {
                        //         Debug.Log($"audio销毁 skillId:{temp.Int32_0} triggerId:{trigger.Int32_0}");
                        //         foreach (var instance in trigger.FixedList128BytesFMODStudioEventInstance_30)
                        //         {
                        //             UnityHelper.DestroyAudioClip(instance);
                        //         }
                        //
                        //         trigger.FixedList128BytesFMODStudioEventInstance_30.Clear();
                        //         trigger.FixedList128Bytesint_31.Clear();
                        //         triggers[j] = trigger;
                        //     }
                        // }

                        switch (temp.Int32_10)
                        {
                            case 0:
                                if (temp.Single_7 < 0)
                                {
                                    temp.Int32_6 = 0;
                                    for (int j = 0; j < triggers.Length; j++)
                                    {
                                        var trigger = triggers[j];
                                        if (trigger.Int32_10 == temp.Int32_0)
                                        {
                                            trigger.Boolean_11 = true;
                                            triggers[j] = trigger;
                                        }
                                    }
                                }
                                else
                                {
                                    temp.Single_7 -= fdT * temp.Single_4;
                                }

                                skillsBuffer[i] = temp;
                                continue;
                                break;
                            case 1:
                                // for (int j = 0; j < triggers.Length; j++)
                                // {
                                //     var trigger = triggers[j];
                                //     if (trigger.Int32_10 == temp.Int32_0)
                                //     {
                                //         triggers.RemoveAt(j);
                                //     }
                                // }

                                skillsBuffer[i] = temp;
                                skillsBuffer.RemoveAt(i);
                                continue;
                                break;
                            case 2:

                                // if (cdfeStateMachine.HasComponent(entity))
                                // {
                                //     var stateMachine = cdfeStateMachine[entity];
                                //     //int needTick = (int)(temp.Single_3 / fdT) + 1;
                                //     // Debug.Log($"needTick:{needTick},cur:{temp.Int32_6}");
                                //     //if (stateMachine.currentState.CurrentTypeId == State.TypeId.CommonBossAttack)
                                //     //{
                                //     stateMachine.transitionToStateIndex = 0;
                                //     //stateMachine.currentState.CurrentTypeId = State.TypeId.CommonBossMove;
                                //     cdfeStateMachine[entity] = stateMachine;
                                //     //}
                                // }

                                temp.Single_7 -= fdT * temp.Single_4;
                                // if (temp.Single_7 < 0)
                                // {
                                //     Debug.Log($"cur:{temp.Int32_6}");
                                //
                                //     temp.Int32_6 = 0;
                                //     temp.Boolean_11 = true;
                                // }
                                // else
                                // {
                                //     temp.Single_7 -= fdT * temp.Single_4;
                                // }

                                skillsBuffer[i] = temp;
                                continue;
                                break;
                        }
                    }
                    else
                    {
                        temp.Single_8 -= fdT * temp.Single_4;
                    }

                    temp.Int32_9++;
                    temp.Int32_6++;

                    skillsBuffer[i] = temp;
                }
            }

            private void DestroySpecialEffect(Skill temp, Entity e, int sortKey)
            {
                if (!cdfeLinkedEntityGroup.HasBuffer(e))
                {
                    return;
                }

                ref var tbskills =
                    ref globalConfigData.value.Value.configTbskills.configTbskills;
                int skillIndex = 0;
                for (int i = 0; i < tbskills.Length; i++)
                {
                    if (tbskills[i].id == temp.Int32_0)
                    {
                        skillIndex = i;
                    }
                }

                ref var sTbskill = ref tbskills[skillIndex];
                for (int i = 0; i < sTbskill.replaceSpecialEffects.Length; i++)
                {
                    int id = sTbskill.replaceSpecialEffects[i];


                    foreach (var linkedEntity in cdfeLinkedEntityGroup[e])
                    {
                        if (cdfeSpecialEffectData.HasComponent(linkedEntity.Value))
                        {
                            var specialEffectData = cdfeSpecialEffectData[linkedEntity.Value];
                            if (specialEffectData.id == id)
                            {
                                ecb.AddComponent(sortKey, linkedEntity.Value, new TimeToDieData
                                {
                                    duration = 0.01f
                                });
                                break;
                            }
                        }
                    }
                }


                //cdfeSpecialEffectData
                //temp.Int32_0
            }

            private void ChangeCarrierBaTi(Entity entity, int sortKey, Skill temp)
            {
                //if (temp.Int32_10==2)
                if (cdfeBossTag.HasComponent(entity))
                {
                    Debug.Log($"加霸体。。。。。。。。。。。{temp.Single_3}");
                    ecb.AppendToBuffer<Buff>(sortKey, entity,
                        new Buff_BossCastSkillBaTi { carrier = entity, duration = temp.Single_3 }.ToBuff());
                }
            }


            //private void switchSkillStatus(ref Skill temp, int sortKey, Entity entity)
            //{
            //    ref var skillConfig = ref globalConfigData.value.Value.configTbskills.configTbskills;
            //    int index = 0;

            //    for (int i = 0; i < skillConfig.Length; i++)
            //    {
            //        if (skillConfig[i].id == temp.Int32_0 && skillConfig[i].skillReplace.Length>0)
            //        {
            //            index = i; break;
            //        }
            //    }
            //    ref var curSkillConfig = ref skillConfig[index];
            //    ref var replacePar = ref curSkillConfig.skillReplace;
            //    if (temp.int3_14.x > replacePar[0].x)
            //    {
            //        temp.Boolean_11 = false;
            //        ecb.AppendToBuffer<Skill>(sortKey, entity, new Skill { Int32_0 = replacePar[0].y, Entity_5 = temp.Entity_5 });
            //    }

            //}


            /// <summary>
            /// 充能技能禁用和切换
            /// </summary>
            /// <param name="temp"></param>
            /// <param name="skillsBuffer"></param>
            private void SwitchSkillStatus(ref Skill temp, ref DynamicBuffer<Skill> skillsBuffer)
            {
                if (temp.Int32_10 == 2) return;

                ref var skillConfig = ref globalConfigData.value.Value.configTbskills.configTbskills;
                NativeList<int> replaceSkills = new NativeList<int>(skillConfig.Length - 1, Allocator.Temp);
                for (int i = 0; i < skillConfig.Length; i++)
                {
                    if (skillConfig[i].chargedSkill.Length > 0)
                    {
                        // Debug.Log($"skillConfig[i].skillReplace[0].y{skillConfig[i].skillReplace[0].y}");
                        replaceSkills.Add(skillConfig[i].chargedSkill[0].y);
                    }
                }

                int index = 0;
                for (int i = 0; i < skillConfig.Length; i++)
                {
                    if (skillConfig[i].id == temp.Int32_0)
                    {
                        index = i;
                        break;
                    }
                }

                ref var curSkillConfig = ref skillConfig[index];
                //是充能类技能
                if (curSkillConfig.chargedSkill.Length > 0)
                {
                    ref var replacePar = ref curSkillConfig.chargedSkill;
                    //未充能成功
                    if (temp.int3_14.x < replacePar[0].x)
                    {
                        temp.Boolean_11 = false;
                        for (int i = 0; i < skillsBuffer.Length; i++)
                        {
                            if (skillsBuffer[i].Int32_0 == replacePar[0].y)
                            {
                                var chargeSkill = skillsBuffer[i];
                                chargeSkill.Boolean_11 = true;
                                skillsBuffer[i] = chargeSkill;
                            }
                        }
                    }
                    else
                    {
                        temp.Boolean_11 = true;
                        //temp.int3_14.x = 0;
                        for (int i = 0; i < skillsBuffer.Length; i++)
                        {
                            if (skillsBuffer[i].Int32_0 == replacePar[0].y)
                            {
                                var chargeSkill = skillsBuffer[i];
                                chargeSkill.Boolean_11 = false;
                                if (chargeSkill.int3_14.x >= replacePar[0].z)
                                {
                                    chargeSkill.Boolean_11 = true;
                                    temp.Boolean_11 = false;
                                    temp.int3_14.x = 0;
                                    chargeSkill.int3_14.x = 0;
                                }

                                skillsBuffer[i] = chargeSkill;
                            }
                        }
                    }
                }
                else
                {
                    bool isCharged = false;
                    for (int i = 0; i < replaceSkills.Length; i++)
                    {
                        // Debug.Log($"replaceSkills{replaceSkills[i]}");
                        if (replaceSkills[i] == temp.Int32_0)
                        {
                            isCharged = true;
                            break;
                        }
                    }

                    //是普通技能
                    if (!isCharged)
                    {
                        //UnityEngine. Debug.Log($"普通技能：{temp.Int32_0}");
                        temp.Boolean_11 = false;
                    }
                    //是被充能技能
                    else
                    {
                        int mainSkillID = 0, mainCount = 0, replaceCount = 0;
                        for (int i = 0; i < skillConfig.Length; i++)
                        {
                            if (skillConfig[i].chargedSkill.Length > 0 &&
                                skillConfig[i].chargedSkill[0].y == temp.Int32_0)
                            {
                                mainCount = skillConfig[i].chargedSkill[0].x;
                                replaceCount = skillConfig[i].chargedSkill[0].z;
                                mainSkillID = skillConfig[i].id;
                            }
                        }

                        for (int i = 0; i < skillsBuffer.Length; i++)
                        {
                            if (skillsBuffer[i].Int32_0 == mainSkillID)
                            {
                                if (temp.int3_14.x >= replaceCount)
                                {
                                    var mainSkill = skillsBuffer[i];
                                    mainSkill.Boolean_11 = false;
                                    temp.Boolean_11 = true;
                                    temp.int3_14.x = 0;
                                    mainSkill.int3_14.x = 0;
                                    skillsBuffer[i] = mainSkill;
                                }
                            }
                        }
                    }
                }
            }

            private void DisableReplaceSkill(ref Skill temp, NativeList<int> replaceSkills)
            {
                for (int i = 0; i < replaceSkills.Length; i++)
                {
                    // Debug.Log($"replaceSkills{replaceSkills[i]}");
                    if (replaceSkills[i] == temp.Int32_0)
                    {
                        temp.Boolean_11 = true;
                        break;
                    }
                }
            }

            #region 封装的函数

            /// <summary>
            /// 执行武器技能额外次数
            /// </summary>
            /// <param name="entity"></param>
            /// <param name="temp"></param>
            private void DoAddtionCount(Entity entity, ref Skill temp)
            {
                Debug.Log("DoAddtionCount");
                var playtemp = cdfePlayerData[entity];
                playtemp.playerData.weaponSkillExtraCount--;
                cdfePlayerData[entity] = playtemp;
                temp.Single_2 = 0;
                //Debug.Log($"{cdfePlayerData[entity].playerData.weaponSkillExtraCount}");
            }


            /// <summary>
            /// 判断是武器攻击
            /// </summary>
            /// <param name="triggers"></param>
            private static void JudgeWeaponAttack(ref DynamicBuffer<Trigger> triggers, int weaponSkillID)
            {
                for (int j = 0; j < triggers.Length; j++)
                {
                    var trigger = triggers[j];
                    if (trigger.TriggerType_5 == TriggerType.WeaponAttack)
                    {
                        //Debug.Log(" trigger.Boolean_19 = true");
                        trigger.Boolean_19 = true;
                        triggers[j] = trigger;
                    }

                    if (trigger.Int32_10 == weaponSkillID)
                    {
                        trigger.Boolean_29 = true;
                        triggers[j] = trigger;
                    }
                }
            }


            /// <summary>
            /// 移除trigger带来的额外增伤(每次技能消耗掉)
            /// </summary>
            /// <param name="entity"></param>
            private void RemoveAdditonDamage(Entity entity, Skill temp)
            {
                if (cdfeBuff.HasBuffer(entity) && cdfePlayerData.HasComponent(entity))
                {
                    var buffs = cdfeBuff[entity];
                    for (int j = 0; j < buffs.Length; j++)
                    {
                        if (buffs[j].CurrentTypeId == Buff.TypeId.Buff_DiyAdditonDamage &&
                            buffs[j].BuffArgsNew_12.args2.x < temp.int3_14.x)
                        {
                            buffs.RemoveAt(j);
                        }
                    }
                }
            }

            /// <summary>
            /// 重置skill的trigger参数
            /// </summary>
            /// <param name="triggers"></param>
            /// <param name="temp"></param>
            private static void ResetTriggerPar(ref DynamicBuffer<Trigger> triggers, Skill temp)
            {
                for (int j = 0; j < triggers.Length; j++)
                {
                    var trigger = triggers[j];
                    if (trigger.Int32_10 == temp.Int32_0)
                    {
                        trigger.Int32_2 = 0;
                        trigger.float4_6.z = trigger.float4_6.y;
                        trigger.float3_3.z = trigger.float3_3.y;
                        trigger.Single_15 =
                            trigger.float4_6.x != 0 ? math.max(trigger.float4_6.x / 1000f, 0.02f) : 0.5f;
                        triggers[j] = trigger;
                    }
                }
            }

            /// <summary>
            /// 读表返回冷却时间
            /// </summary>
            /// <param name="entity"></param>
            /// <param name="temp"></param>
            private void RetrunCoolDown(GameTimeData gameTimeData, Entity entity, ref Skill temp)
            {
                if (cdfePlayerData.HasComponent(entity))
                {
                    var weaponSkillID = cdfePlayerData[entity].playerOtherData.weaponSkillId;
                    if (temp.Int32_0 == weaponSkillID && cdfePlayerData[entity].playerData.weaponSkillExtraCount > 0 &&
                        temp.Single_2 > 0)
                    {
                        DoAddtionCount(entity, ref temp);
                    }
                    else
                    {
                        if (cdfeChaStats.HasComponent(entity))
                        {
                            var coolDown = cdfeChaStats[entity].chaProperty.defaultcoolDown;
                            ref var skills = ref globalConfigData.value.Value.configTbskills.configTbskills;
                            int skillIndex = 0;
                            for (int j = 0; j < skills.Length; j++)
                            {
                                if (skills[j].id == temp.Int32_0)
                                {
                                    skillIndex = j;
                                    break;
                                }
                            }

                            ref var skill = ref skills[skillIndex];

                            var cd = (skill.cd / 1000f) * (1 - (coolDown / 10000f) * (skill.reductionCd / 10000f));

                            if (skill.type == 3)
                            {
                                temp.Int32_10 = 2;
                            }
                            else if (math.abs(cd) - math.EPSILON <= 0)
                            {
                                temp.Int32_10 = 1;
                            }

                            temp.Single_2 = math.max(cd / gameTimeData.logicTime.gameTimeScale, 5 * fdT);
                            if (gameTimeData.logicTime.gameTimeScale < math.EPSILON)
                            {
                                temp.Single_2 = 5 * fdT;
                            }

                            temp.Single_2 = math.clamp(temp.Single_2, OneShotSkillMinDuration, MathHelper.MaxNum);
                        }
                        else
                        {
                            temp.Single_2 = 5 * fdT;
                        }
                    }
                }
                else
                {
                    if (cdfeChaStats.HasComponent(entity))
                    {
                        var coolDown = cdfeChaStats[entity].chaProperty.defaultcoolDown;
                        ref var skills = ref globalConfigData.value.Value.configTbskills.configTbskills;
                        int skillIndex = 0;
                        for (int j = 0; j < skills.Length; j++)
                        {
                            if (skills[j].id == temp.Int32_0)
                            {
                                skillIndex = j;
                                break;
                            }
                        }

                        ref var skill = ref skills[skillIndex];

                        var cd = (skill.cd / 1000f) * (1 - (coolDown / 10000f) * (skill.reductionCd / 10000f));

                        if (skill.type == 3)
                        {
                            temp.Int32_10 = 2;
                        }
                        else if (math.abs(cd) - math.EPSILON <= 0)
                        {
                            temp.Int32_10 = 1;
                        }

                        temp.Single_2 = math.max(cd / gameTimeData.logicTime.gameTimeScale, 5 * fdT);
                        if (gameTimeData.logicTime.gameTimeScale < math.EPSILON)
                        {
                            temp.Single_2 = 5 * fdT;
                        }

                        temp.Single_2 = math.clamp(temp.Single_2, OneShotSkillMinDuration, MathHelper.MaxNum);
                    }
                    else
                    {
                        temp.Single_2 = 5 * fdT;
                    }
                }
            }

            /// <summary>
            /// 加trigger并且返回trigger总时间
            /// </summary>
            /// <param name="index"></param>
            /// <param name="id"></param>
            /// <param name="entity"></param>
            /// <returns>总时间</returns>
            private void AddTrigger(ref Skill skill, int sortKey, int id, Entity entity,
                DynamicBuffer<Trigger> triggers)
            {
                ref var skillEffect =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;

                var filterTriggersIndex = GetFilterTrigger(id).GetValueArray(Allocator.Temp);


                // if (totalTime == 0)
                // {
                //     //totalTime = 0.1f;
                //     Debug.Log($"技能持续时间为{totalTime}  skillId:{id}");
                //     //return totalTime;
                // }

                for (int i = 0; i < filterTriggersIndex.Length; i++)
                {
                    var triggerID = skillEffect[filterTriggersIndex[i]].id;
                    //var triggerID = filterTriggersIndex[i];
                    Debug.Log($"Adddddd {triggerID}");
                    AddTriggerInit(ref skill, sortKey, entity, triggers, triggerID);
                }

                //var modulepar = new BlobArray<int>();
                BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
                ref var data = ref blobBuilder.ConstructRoot<int>();

                //var modulepar= blobBuilder.CreateBlobAssetReference<int>(Allocator.Temp);

                //ref BlobArray<int> par = ref modulepar;
                //par[0]=
            }

            /// <summary>
            /// 首次添加一个trigger 由技能添加 
            /// </summary>
            /// <param name="skill">添加该trigger的技能信息</param>
            /// <param name="sortKey"></param>
            /// <param name="caster">技能携带者 trigger的持有者</param>
            /// <param name="triggers"></param>
            /// <param name="skillEffect"></param>
            /// <param name="filterTriggersIndex">需要添加的trigger</param>
            private void AddTriggerInit(ref Skill skill, int sortKey, Entity caster, DynamicBuffer<Trigger> triggers,
                int triggerID)
            {
                bool isContain = false;
                for (int j = 0; j < triggers.Length; j++)
                {
                    if (triggers[j].Int32_0 == triggerID)
                    {
                        isContain = true;
                        break;
                    }
                }

                if (isContain) return;

                ref var skillEffect =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                int index = -1;
                for (int i = 0; i < skillEffect.Length; i++)
                {
                    if (triggerID == skillEffect[i].id)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                {
                    Debug.Log($"当前技能triggerID skilleffect表不存在 triggerID:{triggerID}");
                    return;
                }

                ref var skilleffect = ref skillEffect[index];


                ///先判断是否能添加trigger
                int power = skilleffect.power;
                var timeTick = (uint)(eT * 1000f);
                var rand = Random.CreateFromIndex((uint)(timeTick + caster.Index + caster.Version));
                var dropRate = math.clamp(power, 0, 10000);

                var canTrigger = rand.NextInt(0, 10001) <= dropRate;
                if (!canTrigger)
                    return;

                float delay = 0;
                TriggerType type = (TriggerType)skilleffect.triggerType;

                TriggerConditionType condition = (TriggerConditionType)(skilleffect.conditionType == 4
                    ? skilleffect.conditionType * 10 + skilleffect.conditionTypePara[0]
                    : skilleffect.conditionType);

                int delayType = skilleffect.delayType;

                float4 triggerTypeArgs = default;
                int4 triggerConditionTypeArgs = default;
                for (int j = 0; j < skilleffect.triggerTypePara.Length; j++)
                {
                    switch (j)
                    {
                        case 0:
                            triggerTypeArgs.x = skilleffect.triggerTypePara[j];
                            if (type == TriggerType.PerTime)
                            {
                                triggerTypeArgs.x = skilleffect.triggerTypePara[j] / 1000f;
                            }

                            if (type == TriggerType.PerNum)
                            {
                                if (skilleffect.triggerTypePara.Length == 1)
                                {
                                    triggerTypeArgs.y = MathHelper.MaxNum;
                                }
                            }

                            break;
                        case 1:
                            triggerTypeArgs.y = skilleffect.triggerTypePara[j];
                            if (type == TriggerType.PerNum)
                            {
                                if (skilleffect.triggerTypePara[j] == 0)
                                {
                                    triggerTypeArgs.y = MathHelper.MaxNum;
                                }
                            }

                            break;
                        case 2:
                            triggerTypeArgs.z = skilleffect.triggerTypePara[j];
                            break;
                        case 3:
                            triggerTypeArgs.w = skilleffect.triggerTypePara[j];
                            break;
                    }
                }

                for (int j = 0; j < skilleffect.conditionTypePara.Length; j++)
                {
                    switch (j)
                    {
                        case 0:
                            triggerConditionTypeArgs.x = skilleffect.conditionTypePara[j];
                            break;
                        case 1:
                            triggerConditionTypeArgs.y = skilleffect.conditionTypePara[j];
                            break;
                        case 2:
                            triggerConditionTypeArgs.z = skilleffect.conditionTypePara[j];
                            break;
                        case 3:
                            triggerConditionTypeArgs.w = skilleffect.conditionTypePara[j];
                            break;
                    }
                }

                for (int j = 0; j < skilleffect.delayTypePara.Length; j++)
                {
                    switch (j)
                    {
                        case 0:
                            delay = skilleffect.delayTypePara[j] / 1000f;
                            break;
                    }
                }

                //后延不算在延迟里面
                if (delayType == 4)
                {
                    delay = 0f;
                }

                float haloGap = 0.5f;
                if (skilleffect.triggerType == (int)TriggerType.Halo)
                {
                    if (skilleffect.triggerTypePara[0] != 0)
                    {
                        haloGap = skilleffect.triggerTypePara[0] / 1000f;
                    }
                }

                Debug.Log($"id:{skilleffect.id}");
                Debug.Log($"gsagfsgs");

                var initFloat4 = float4.zero;
                if (condition == TriggerConditionType.KillEnemy && cdfePlayerData.HasComponent(caster))
                {
                    var killType = (uint)triggerConditionTypeArgs.y;
                    int currentKillCount = 0;
                    if ((killType & 8) != 0)
                    {
                        currentKillCount +=
                            cdfePlayerData[caster].playerOtherData.killLittleMonster;
                    }

                    if ((killType & 16) != 0)
                    {
                        currentKillCount +=
                            cdfePlayerData[caster].playerOtherData.killElite;
                    }

                    if ((killType & 32) != 0)
                    {
                        currentKillCount +=
                            cdfePlayerData[caster].playerOtherData.killBoss;
                    }

                    initFloat4.x = currentKillCount;
                }


                ecb.AppendToBuffer(sortKey, caster, new Trigger
                {
                    CurrentTypeId = (Trigger.TypeId)101,
                    Int32_0 = skilleffect.id,
                    Single_1 = 0,
                    Int32_2 = 0,
                    float3_3 = new float3(delayType,
                        delay,
                        0),
                    Boolean_4 = false,
                    TriggerType_5 = type,
                    float4_6 = triggerTypeArgs,
                    TriggerConditionType_7 = condition,
                    int4_8 = triggerConditionTypeArgs,
                    float4_9 = initFloat4,
                    Int32_10 = skill.Int32_0,
                    Boolean_11 = skill.Boolean_11,
                    Entity_12 = caster,
                    Boolean_13 = false,
                    Int32_14 = skilleffect.power,
                    Single_15 = haloGap,
                    Int32_16 = 0,
                    Int32_17 = skilleffect.compareType,
                    Boolean_18 = false,
                    Boolean_19 = false,
                    Entity_20 = caster,
                    Boolean_21 = false,
                    Boolean_22 = false,
                    Int32_23 = 0,
                    float3x2_24 = default,
                    LocalTransform_25 = default,
                    Boolean_26 = false,
                    Boolean_27 = false,
                });
            }

            /// <summary>
            /// 返回skill的持续时间
            /// </summary>
            /// <param name="id"></param>
            /// <param name="skillEffect"></param>
            /// <returns></returns>
            private float ReturnSkillDuraion(int skillID)
            {
                ref var skillEffect =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                float totalTime = 0;

                for (int i = 0; i < skillEffect.Length; i++)
                {
                    if (skillEffect[i].skillId == skillID)
                    {
                        ref var trigger = ref skillEffect[i];

                        if (trigger.triggerType == (int)TriggerType.PerTime)
                        {
                            if (trigger.delayType != 2)
                            {
                                totalTime += trigger.triggerTypePara[0] / 1000f * (trigger.triggerTypePara[1] - 1);
                            }
                        }

                        if (trigger.delayType == 1 || trigger.delayType == 4 || trigger.delayType == 3)
                        {
                            totalTime += trigger.delayTypePara[0] / 1000f;
                        }

                        if (trigger.triggerType == (int)TriggerType.Halo)
                        {
                            totalTime += MathHelper.MaxNum;
                        }
                    }
                }

                return totalTime;
            }

            /// <summary>
            /// 初始化回溯数据
            /// </summary>
            /// <param name="skillID"></param>
            /// <returns></returns>
            private void InitBackSkillData(ref EntityCommandBuffer.ParallelWriter ecb, int sortkey, int skillID,
                Entity entity)
            {
                ref var skillEffect =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;

                for (int i = 0; i < skillEffect.Length; i++)
                {
                    if (skillEffect[i].skillId == skillID && skillEffect[i].calcType == 4 &&
                        skillEffect[i].calcTypePara.Length > 0)
                    {
                        //回溯
                        if (skillEffect[i].calcTypePara[0] == 3)
                        {
                            var time = skillEffect[i].calcTypePara[1];
                            ecb.AddBuffer<BackTrackTimeBuffer>(sortkey, entity);
                            ecb.AppendToBuffer(sortkey, entity, new Buff_BackTrackTime
                            {
                                id = 0,
                                startTime = 0,
                                timeElapsed = 0,
                                duration = 0,
                                permanent = true,
                                caster = default,
                                carrier = entity,
                                changeType = default,
                                canClear = 0,
                                immuneStack = 0,
                                direction = default,
                                buffState = 0,
                                argsNew = default,
                                priority = 0,
                                power = 0,
                                tickTime = 1,
                                distancePar = default,
                                isBullet = false,
                                oldValue = 0,
                                backTimeDuration = time / 1000f
                            }.ToBuff());
                        }
                    }
                }
            }

            private void InitSkillData(ref Skill temp, int skillId)
            {
                ref var tbskills =
                    ref globalConfigData.value.Value.configTbskills.configTbskills;
                int skillIndex = 0;
                for (int i = 0; i < tbskills.Length; i++)
                {
                    if (tbskills[i].id == skillId)
                    {
                        skillIndex = i;
                    }
                }

                ref var sTbskill = ref tbskills[skillIndex];
                if (sTbskill.cd <= 0)
                {
                    temp.Boolean_11 = false;
                }
            }

            /// <summary>
            /// 拿到筛选之后的triggerID
            /// </summary>
            /// <returns>在skill里面直接添加的triggerID</returns>
            private NativeHashMap<int, int> GetFilterTrigger(int skillID)
            {
                var triggerHash = new NativeHashMap<int, int>(20, Allocator.Temp);
                var effects = new NativeList<int>(20, Allocator.Temp);
                ref var skillEffect =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                ref var bulletTable =
                    ref globalConfigData.value.Value.configTbbullets.configTbbullets;
                for (int i = 0; i < skillEffect.Length; i++)
                {
                    if (skillEffect[i].skillId == skillID && skillEffect[i].effectType == 2 &&
                        skillEffect[i].conditionType != 2)
                    {
                        triggerHash.Add(skillEffect[i].id, i);
                        ref var calcTypePara = ref skillEffect[i].calcTypePara;
                        if (skillEffect[i].calcType == 0)
                        {
                            for (int j = 0; j < calcTypePara.Length; j++)
                            {
                                effects.Add(calcTypePara[j]);
                            }
                        }
                        else if (skillEffect[i].calcType == 1)
                        {
                            for (int j = 0; j < bulletTable.Length; j++)
                            {
                                if (skillEffect[i].calcTypePara[0] == bulletTable[j].id)
                                {
                                    effects.Add(bulletTable[j].skillEffect);
                                    effects.Add(bulletTable[j].deadEffect);
                                    break;
                                }
                            }
                        }
                        else if (skillEffect[i].calcType == 7)
                        {
                            //var skilleffectIDs = new NativeArray<int>(calcTypePara.Length, Allocator.Temp);
                            for (int j = 0; j < calcTypePara.Length; j++)
                            {
                                if (j % 2 == 0)
                                {
                                    //var index = j / 2;
                                    effects.Add(calcTypePara[j]);
                                }
                            }
                        }

                        if (skillEffect[i].extraType == 1 && skillEffect[i].extraTypePara.Length >= 2)
                        {
                            effects.Add(skillEffect[i].extraTypePara[0]);
                            effects.Add(skillEffect[i].extraTypePara[1]);
                        }
                        else if (skillEffect[i].extraType == 2 && skillEffect[i].extraTypePara.Length >= 1)
                        {
                            for (int j = 0; j < skillEffect[i].extraTypePara.Length; j++)
                            {
                                effects.Add(skillEffect[i].extraTypePara[j]);
                            }
                        }
                        else if (skillEffect[i].calcType == 0 && skillEffect[i].extraType==3)
                        {
                            for (int j = 0; j < bulletTable.Length; j++)
                            {
                                if (skillEffect[i].extraTypePara[0] == bulletTable[j].id)
                                {
                                    effects.Add(bulletTable[j].skillEffect);
                                    effects.Add(bulletTable[j].deadEffect);
                                    break;
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < effects.Length; i++)
                {
                    if (triggerHash.ContainsKey(effects[i]))
                    {
                        triggerHash.Remove(effects[i]);
                    }
                }

                return triggerHash;
            }

            #endregion
        }
    }
}
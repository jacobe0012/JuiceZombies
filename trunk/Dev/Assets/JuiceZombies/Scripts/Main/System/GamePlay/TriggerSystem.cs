//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-08-25 11:00:25
//---------------------------------------------------------------------

using cfg.blobstruct;
using cfg.config;
using ProjectDawn.Navigation;
using System.Globalization;
using System.Threading;
using Unity.Android.Gradle.Manifest;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SkillCastSystem))]
    public partial struct TriggerSystem : ISystem
    {
        private readonly static bool isDebug = true;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<Trigger>();
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
            var gameOtherData = SystemAPI.GetComponent<GameOthersData>(wbe);
            var stateQuery = SystemAPI.QueryBuilder().WithAll<State, StateMachine>().WithNone<Prefab>().Build();
            var skillAtackQuery = SystemAPI.QueryBuilder().WithAll<SkillAttackData>().WithNone<Prefab>().Build();
            var mapMoudleQuery = SystemAPI.QueryBuilder().WithAll<MapElementData>().WithNone<BoardData>().Build();
            var player = SystemAPI.GetSingletonEntity<PlayerData>();
            //Debug.Log($"Time:{UnityEngine.Time.timeSinceLevelLoad}");

            new TriggerSystemJob
            {
                timeTick = (uint)(SystemAPI.Time.ElapsedTime * 1000f),
                fdT = SystemAPI.Time.fixedDeltaTime,
                eT = (float)SystemAPI.Time.ElapsedTime,
                ecb = ecb,
                player = player,
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(true),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
                cdfeSpiritData = SystemAPI.GetComponentLookup<SpiritData>(true),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(),
                cdfeTargetData = SystemAPI.GetComponentLookup<TargetData>(true),
                cdfeObstacleTag = SystemAPI.GetComponentLookup<ObstacleTag>(true),
                bfeBackTrackTimeBuffer = SystemAPI.GetBufferLookup<BackTrackTimeBuffer>(true),
                cdfeBossTag = SystemAPI.GetComponentLookup<BossTag>(true),
                cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
                gameRandomData = gameRandomData,
                prefabMapData = prefabMapData,
                gameSetUpData = gameSetUpData,
                gameOtherData = gameOtherData,
                cdfeStateMachine = SystemAPI.GetComponentLookup<StateMachine>(),
                allEntities = stateQuery.ToEntityArray(Allocator.TempJob),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                globalConfigData = globalConfigData,
                gameTimeData = gameTimeData,
                skillAtackDatas = skillAtackQuery.ToComponentDataArray<SkillAttackData>(Allocator.TempJob),
                skillAtackDatasEntity = skillAtackQuery.ToEntityArray(Allocator.TempJob),
                mapModules = mapMoudleQuery.ToEntityArray(Allocator.TempJob),
                //cdfePropsData = SystemAPI.GetComponentLookup<PropsData>(true),
                cdfePhysicsCollider = SystemAPI.GetComponentLookup<PhysicsCollider>(true),
                cdfePostTransformMatrix = SystemAPI.GetComponentLookup<PostTransformMatrix>(true),
                cdfeAgentBody = SystemAPI.GetComponentLookup<AgentBody>(),
                cdfeBuff = SystemAPI.GetBufferLookup<Buff>(),
                cdfePlayerProps = SystemAPI.GetBufferLookup<PlayerProps>(),
                cdfeBulletTempTag = SystemAPI.GetComponentLookup<BulletTempTag>(true),
                wbe = wbe
            }.ScheduleParallel();
        }


        [BurstCompile]
        public partial struct TriggerSystemJob : IJobEntity
        {
            [ReadOnly] public uint timeTick;
            [ReadOnly] public float fdT;
            [ReadOnly] public float eT;
            [ReadOnly] public Entity wbe;
            [ReadOnly] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [NativeDisableParallelForRestriction] public ComponentLookup<PlayerData> cdfePlayerData;
            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
            [ReadOnly] public ComponentLookup<SpiritData> cdfeSpiritData;
            [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;
            public EntityCommandBuffer.ParallelWriter ecb;

            [ReadOnly] public Entity player;
            public GameRandomData gameRandomData;
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GameSetUpData gameSetUpData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public GameTimeData gameTimeData;
            [ReadOnly] public GameOthersData gameOtherData;

            [NativeDisableParallelForRestriction] public ComponentLookup<ChaStats> cdfeChaStats;
            [NativeDisableParallelForRestriction] public ComponentLookup<StateMachine> cdfeStateMachine;
            [NativeDisableParallelForRestriction] public BufferLookup<Buff> cdfeBuff;

            //[ReadOnly] public BufferLookup<StatefulTriggerEvent> allCollisionEvent;
            [ReadOnly] public ComponentLookup<TargetData> cdfeTargetData;
            [ReadOnly] public ComponentLookup<BulletTempTag> cdfeBulletTempTag;
            [ReadOnly] public BufferLookup<PlayerProps> cdfePlayerProps;

            [ReadOnly] public ComponentLookup<BossTag> cdfeBossTag;

            [ReadOnly] public ComponentLookup<ObstacleTag> cdfeObstacleTag;


            [ReadOnly] public BufferLookup<BackTrackTimeBuffer> bfeBackTrackTimeBuffer;

            //[ReadOnly] public ComponentLookup<PropsData> cdfePropsData;
            [ReadOnly] public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
            [ReadOnly] public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;

            [DeallocateOnJobCompletion] [NativeDisableParallelForRestriction]
            public NativeArray<SkillAttackData> skillAtackDatas;

            [NativeDisableParallelForRestriction] public ComponentLookup<AgentBody> cdfeAgentBody;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> skillAtackDatasEntity;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> allEntities;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> mapModules;
            public EntityStorageInfoLookup storageInfoFromEntity;

            public void Execute([EntityIndexInQuery] int sortkey, Entity entity,
                ref DynamicBuffer<Trigger> triggerBuffer,
                ref DynamicBuffer<Skill> skills)
            {
                if (cdfeChaStats.HasComponent(entity))
                {
                    if (cdfeChaStats[entity].chaResource.isDead)
                    {
                        return;
                    }
                }

                if (!cdfeBuff.HasBuffer(entity)) return;
                var buffs = cdfeBuff[entity];

                ref var skillEffectNew =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                TriggerData_ReadWrite refData = new TriggerData_ReadWrite
                {
                    ecb = ecb,
                    storageInfoFromEntity = storageInfoFromEntity,
                    cdfeChaStats = cdfeChaStats,
                };
                TriggerData_ReadOnly inData = new TriggerData_ReadOnly
                {
                    sortkey = sortkey,
                    player = player,
                    prefabMapData = prefabMapData,
                    globalConfigData = globalConfigData,
                    cdfeLocalTransform = cdfeLocalTransform,
                    cdfeEnemyData = cdfeEnemyData,
                    cdfeSpiritData = cdfeSpiritData,
                    entity = entity,
                    fdT = fdT,
                    mapModules = mapModules,
                    cdfeMapElementData = cdfeMapElementData,
                    entities = allEntities,
                    cdfePhysicsCollider = cdfePhysicsCollider,
                };

                for (var i = 0; i < triggerBuffer.Length; i++)
                {
                    var temp = triggerBuffer[i];
                    int tirggerIndex = 0;
                    for (int j = 0; j < skillEffectNew.Length; j++)
                    {
                        if (skillEffectNew[j].id == temp.Int32_0)
                        {
                            tirggerIndex = j;
                            break;
                        }
                    }

                    ref var triggerConfig = ref skillEffectNew[tirggerIndex];


                    if (temp.Int32_2 == 0)
                    {
                        if (temp.Int32_0 == 0)
                        {
                            temp.Int32_0 = (int)temp.CurrentTypeId;
                        }
                    }

                    temp.OnUpdate(ref refData, in inData);

                    if (temp.Boolean_11 && temp.Boolean_22 && temp.Int32_2 >= 1 && temp.float3_3.z <= 0 &&
                        !temp.Boolean_4 && temp.float4_6.z <= 0)
                    {
                        triggerBuffer[i] = temp;
                        triggerBuffer.RemoveAt(i);
                        Debug.Log($"移除trigger,id:{temp.Int32_0}");
                        continue;
                    }

                    var playerData = new PlayerData { };
                    var chaStats = new ChaStats { };
                    if (cdfePlayerData.HasComponent(entity))
                    {
                        playerData = cdfePlayerData[entity];
                    }

                    if (cdfeChaStats.HasComponent(entity))
                    {
                        chaStats = cdfeChaStats[entity];
                    }

                    switch (temp.TriggerType_5)
                    {
                        case TriggerType.PerNum:

                            int perNumCount = (int)temp.float4_6.x;
                            int perNumMaxCount = (int)temp.float4_6.z;
                            if (perNumMaxCount > 0)
                            {
                                //Debug.Log("--------------------------");
                                switch (temp.TriggerConditionType_7)
                                {
                                    case TriggerConditionType.None:

                                        temp.Boolean_4 = true;

                                        break;
                                    case TriggerConditionType.SkillEffectIdFrom:
                                        int skillID = triggerConfig.skillId;

                                        int perNumEffectId = (int)temp.int4_8.x;

                                        GetSkillEffectNum(ref temp, skills, perNumEffectId, perNumCount, skillID);

                                        break;
                                    case TriggerConditionType.SkillEffectIdTo:
                                        //这一条不用

                                        break;
                                    case TriggerConditionType.PropertyId:
                                        Debug.Log("00000000000000000000000000000");
                                        var value = UnityHelper.GetProperty(playerData,
                                            chaStats,
                                            (int)temp.int4_8.x);

                                        switch (temp.Int32_17)
                                        {
                                            //等于
                                            case 0:
                                                if (value == temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                            //小于等于
                                            case 1:
                                                if (value <= temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                            //大于等于
                                            case 2:
                                                if (value >= temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                        }

                                        break;
                                    case TriggerConditionType.KillEnemy:
                                        //默认相等
                                        //perNumCount *= (int)temp.int4_8.y;
                                        var killType = (uint)temp.int4_8.y;
                                        int currentKillCount = 0;
                                        if ((killType & 8) != 0)
                                        {
                                            currentKillCount +=
                                                cdfePlayerData[entity].playerOtherData.killLittleMonster;
                                        }

                                        if ((killType & 16) != 0)
                                        {
                                            currentKillCount +=
                                                cdfePlayerData[entity].playerOtherData.killElite;
                                        }

                                        if ((killType & 32) != 0)
                                        {
                                            currentKillCount +=
                                                cdfePlayerData[entity].playerOtherData.killBoss;
                                        }

                                        temp.float4_9.y = currentKillCount;
                                        int addition = (int)temp.float4_9.y - (int)temp.float4_9.x;

                                        if (addition >= perNumCount &&
                                            (int)temp.float4_9.x != (int)temp.float4_9.y)
                                        {
                                            Debug.Log(
                                                $"杀敌触发,当前杀敌:{currentKillCount},上次杀敌：{(int)temp.float4_9.x}，增加量:{addition}");
                                            temp.float4_6.z--;
                                            temp.Int32_23 = addition / perNumCount;
                                            temp.float4_9.x = temp.float4_9.y;
                                            temp.Boolean_4 = true;
                                        }

                                        break;
                                    case TriggerConditionType.Dodging:
                                        //默认相等
                                        break;
                                    case TriggerConditionType.Moving:
                                        //默认相等
                                        break;
                                    case TriggerConditionType.AfterRebirth:
                                        //默认相等
                                        break;

                                    case TriggerConditionType.PickUpProp:
                                        //默认相等
                                        int propID = (int)temp.int4_8.y;
                                        if (cdfePlayerProps.HasBuffer(player))
                                        {
                                            var props = cdfePlayerProps[player];
                                            foreach (var prop in props)
                                            {
                                                if (prop.propId == propID)
                                                {
                                                    int sumNum = prop.count;

                                                    if (sumNum % perNumCount == 0 && sumNum >= perNumCount &&
                                                        (int)temp.float4_9.x != sumNum)
                                                    {
                                                        Debug.Log(
                                                            $"拾取总数:{sumNum} ，道具id：{propID}，每{perNumCount}次拾取触发");
                                                        temp.float4_6.z--;
                                                        temp.float4_9.x = sumNum;
                                                        temp.Boolean_4 = true;
                                                    }

                                                    break;
                                                }
                                            }
                                        }

                                        break;
                                    case TriggerConditionType.Deading:

                                        break;
                                }
                            }

                            break;
                        case TriggerType.PerTime:
                            var time = temp.float4_6.x;
                            //循环次数
                            int perTimeCount = (int)temp.float4_6.z;
                            int checktick = math.max((int)(time / fdT / gameTimeData.logicTime.gameTimeScale), 1);
                            if (perTimeCount > 0)
                            {
                                if (temp.TriggerConditionType_7 == TriggerConditionType.None)
                                {
                                    if (temp.Int32_2 % checktick == 0)
                                    {
                                        temp.float4_6.z--;
                                        temp.Boolean_4 = true;
                                        Debug.Log(
                                            $"temp.Int32_2:{temp.Int32_2},时间:{checktick},次数：{temp.float4_6.z}");
                                    }
                                }
                                else
                                {
                                    if (temp.Int32_0 % checktick == 0)
                                    {
                                        temp.float4_6.z--;
                                        temp.Boolean_4 = false;
                                        temp.Boolean_21 = false;
                                    }

                                    if (temp.Int32_0 % checktick != 0 && temp.Boolean_21 == false)
                                    {
                                        temp.Boolean_4 = RepeatCheck(temp, entity, skills, chaStats, playerData);
                                    }
                                }
                            }
                            else
                            {
                                temp.Boolean_11 = true;
                            }

                            break;
                        case TriggerType.WeaponAttack:
                            if (temp.Boolean_18 && temp.float4_6.x == 0)
                            {
                                Debug.Log("Boolean_18Boolean_18Boolean_18Boolean_18Boolean_18");
                                temp.Boolean_18 = false;
                                switch (temp.TriggerConditionType_7)
                                {
                                    case TriggerConditionType.None:
                                        temp.Boolean_4 = true;
                                        break;
                                    case TriggerConditionType.PropertyId:

                                        var value = UnityHelper.GetProperty(playerData,
                                            chaStats,
                                            (int)temp.int4_8.x);
                                        switch (temp.Int32_17)
                                        {
                                            //等于
                                            case 0:
                                                if (value == temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                            //小于等于
                                            case 1:
                                                if (value <= temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                            //大于等于
                                            case 2:
                                                if (value >= temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                        }

                                        break;
                                }
                            }
                            else if (temp.Boolean_19 && temp.float4_6.x == 1)
                            {
                                Debug.Log(
                                    $"Boolean_19Boolean_19Boolean_19Boolean_19Boolean_19,triggerid:{temp.Int32_0}");
                                temp.Boolean_19 = false;
                                switch (temp.TriggerConditionType_7)
                                {
                                    case TriggerConditionType.None:
                                        temp.Boolean_4 = true;
                                        break;
                                    case TriggerConditionType.PropertyId:

                                        var value = UnityHelper.GetProperty(playerData,
                                            chaStats,
                                            (int)temp.int4_8.x);
                                        switch (temp.Int32_17)
                                        {
                                            //等于
                                            case 0:
                                                if (value == temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                            //小于等于
                                            case 1:
                                                if (value <= temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                            //大于等于
                                            case 2:
                                                if (value >= temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                        }

                                        break;
                                }
                            }

                            break;
                        case TriggerType.Halo:
                            var halogap = temp.float4_6.x != 0 ? temp.float4_6.x / 1000f : 0.5f;


                            checktick = math.max((int)(halogap / fdT / gameTimeData.logicTime.gameTimeScale), 1);

                            if (temp.Int32_2 % checktick == 0)
                            {
                                //Debug.Log($"ddddddddddddd {checktick}:tick:{temp.Int32_2}");
                                switch (temp.TriggerConditionType_7)
                                {
                                    case TriggerConditionType.None:
                                        temp.Boolean_4 = true;
                                        break;
                                    case TriggerConditionType.SkillEffectIdFrom:
                                        //默认等于
                                        break;
                                    case TriggerConditionType.SkillEffectIdTo:
                                        //默认等于
                                        break;
                                    case TriggerConditionType.PropertyId:
                                        //Debug.Log("ddddddddddddd PropertyId");
                                        var value = UnityHelper.GetProperty(playerData,
                                            chaStats,
                                            (int)temp.int4_8.x);
                                        switch (temp.Int32_17)
                                        {
                                            //等于
                                            case 0:
                                                if (value == temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                            //小于等于
                                            case 1:
                                                if (value <= temp.int4_8.y)
                                                {
                                                    //Debug.Log("ddddddddddddd 小于等于");
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                            //大于等于
                                            case 2:
                                                if (value >= temp.int4_8.y)
                                                {
                                                    temp.Boolean_4 = true;
                                                }

                                                break;
                                        }

                                        break;

                                    case TriggerConditionType.KillEnemy:
                                        switch (temp.Int32_17)
                                        {
                                            //等于
                                            case 0: break;
                                            //小于等于
                                            case 1: break;
                                            //大于等于
                                            case 2: break;
                                        }

                                        break;

                                    case TriggerConditionType.Dodging:
                                        //默认等于
                                        if (temp.Boolean_4)
                                        {
                                        }

                                        break;
                                    case TriggerConditionType.Moving:
                                        //默认等于
                                        if (!cdfeStateMachine.HasComponent(entity))
                                        {
                                            Debug.Log("该entity没有状态机组件！");
                                            continue;
                                        }

                                        var currentState = cdfeStateMachine[entity].currentState;
                                        if (cdfePlayerData.HasComponent(entity))
                                        {
                                            if (currentState.CurrentTypeId == State.TypeId.PlayerMove)
                                                temp.Boolean_4 = true;
                                        }
                                        else
                                        {
                                            if (currentState.CurrentTypeId == State.TypeId.LittleMonsterMove)
                                                temp.Boolean_4 = true;
                                        }

                                        break;
                                    case TriggerConditionType.AfterRebirth:
                                        //默认等于
                                        if (temp.Boolean_4)
                                        {
                                        }

                                        break;
                                    //case TriggerConditionType.CurrentCasterHP:
                                    //    int hpRations = (int)temp.int4_8.y;
                                    //    switch (temp.Int32_17)
                                    //    {
                                    //        //等于
                                    //        case 0:
                                    //            if (cdfeChaStats[entity].chaResource.hp == cdfeChaStats[entity].chaProperty.maxHp * hpRations / 10000)
                                    //            {
                                    //                temp.Boolean_4 = true;
                                    //            }
                                    //            break;
                                    //        //小于等于
                                    //        case 1:
                                    //            if (cdfeChaStats[entity].chaResource.hp<= cdfeChaStats[entity].chaProperty.maxHp * hpRations / 10000)
                                    //            {
                                    //                temp.Boolean_4=true;
                                    //            }
                                    //            break;
                                    //        //大于等于
                                    //        case 2:
                                    //            if (cdfeChaStats[entity].chaResource.hp >= cdfeChaStats[entity].chaProperty.maxHp * hpRations / 10000)
                                    //            {
                                    //                temp.Boolean_4 = true;
                                    //            }
                                    //            break;
                                    //    }
                                    //    break;
                                }
                            }


                            break;
                    }


                    if (temp.Boolean_4)
                    {
                        //武器命中时 为敌人时不需要走ontrigger
                        if (temp.TriggerType_5 == TriggerType.WeaponAttack && temp.float4_6.x == 0 &&
                            temp.float4_6.y == 0)
                        {
                            temp.Boolean_4 = false;
                            temp.Single_1 = MathHelper.MaxNum;
                            triggerBuffer[i] = temp;
                            continue;
                        }

                        if (temp.TriggerType_5 == TriggerType.Halo)
                        {
                            if (temp.Single_15 <= 0 && temp.TriggerType_5 == TriggerType.Halo)
                            {
                                temp.Boolean_4 = false;
                                Debug.Log("OnTrigger halo");
                                OnTrigger(ref triggerConfig, ref buffs, ref temp, globalConfigData, gameOtherData,
                                    triggerBuffer,
                                    skills,
                                    sortkey, entity,
                                    timeTick);
                            }
                        }
                        else
                        {
                            //3 5 为蓄力 需要走另一个ontrigger
                            if (temp.float3_3.x != 3 && temp.float3_3.x != 5)
                            {
                                if (temp.float3_3.z > 0)
                                {
                                    temp.float3_3.z -= fdT;
                                    temp.Boolean_11 = false;
                                }
                                else
                                {
                                    if (temp.TriggerType_5 == TriggerType.PerTime)
                                    {
                                        //每时间触发 触发一次后接下来的本次间隔不会再触发
                                        temp.Boolean_21 = true;
                                    }


                                    temp.Boolean_4 = false;
                                    Debug.Log($"OnTrigger nohalo,第{temp.Int32_2}帧");
                                    //temp.OnTrigger(ref refData, in inData);
                                    OnTrigger(ref triggerConfig, ref buffs, ref temp, globalConfigData, gameOtherData,
                                        triggerBuffer, skills,
                                        sortkey, entity, timeTick);
                                    //先触发再扣除
                                    if (temp.TriggerType_5 == TriggerType.PerNum)
                                    {
                                        --temp.float4_6.z;
                                        var leftCount = temp.float4_6.z;
                                        if (leftCount <= 0)
                                        {
                                            temp.Boolean_11 = true;
                                        }
                                    }
                                }
                            }
                            //蓄力
                            else
                            {
                                if (temp.float3_3.z > 0)
                                {
                                    //蓄力的索敌
                                    if (temp.float3_3.z == temp.float3_3.y)
                                    {
                                        // ChangeCarrierBaTi(entity);

                                        Debug.LogError("先索敌再延迟,正在索敌");

                                        //Debug.Log($"curTriggerTimes:{GetBeTriggeredCount(temp)}");
                                        var loc = SeekTargetForEnergy(sortkey, temp, entity, out float3 targetPos,
                                            out float3 dir, skills, out var localTransform, out Entity target,
                                            out int targetType);
                                        //if (targetType == 1)
                                        //{
                                        //    if (target == Entity.Null)
                                        //    {
                                        //        if (!cdfePlayerData.HasComponent(entity))
                                        //        {
                                        //            ref var skill= ref globalConfigData.value.Value.configTbskills.Get(temp.Int32_10);

                                        //        }
                                        //    }
                                        //}


                                        temp.LocalTransform_25 = loc;
                                        temp.float3x2_24.c0 = targetPos;
                                        temp.float3x2_24.c1 = dir;

                                        for (int m = 0; m < skills.Length; m++)
                                        {
                                            if (skills[m].Int32_0 == temp.Int32_10)
                                            {
                                                var skilltemp = skills[m];
                                                skilltemp.LocalTransform_15 = localTransform;
                                                skills[m] = skilltemp;
                                                break;
                                            }
                                        }
                                    }

                                    temp.Boolean_11 = false;
                                    temp.float3_3.z -= fdT;
                                }
                                else
                                {
                                    if (temp.TriggerType_5 == TriggerType.PerTime)
                                    {
                                        //每时间触发 触发一次后接下来的本次间隔不会再触发
                                        temp.Boolean_21 = true;
                                    }


                                    temp.Boolean_4 = false;
                                    Debug.Log($"targetPos:{temp.float3x2_24.c0},dir:{temp.float3x2_24.c1}");
                                    OnTriggerForEnergy(ref buffs, sortkey, temp, entity, skills, triggerBuffer);

                                    if (temp.TriggerType_5 == TriggerType.PerNum)
                                    {
                                        --temp.float4_6.z;
                                        var leftCount = temp.float4_6.z;
                                        if (leftCount <= 0)
                                        {
                                            temp.Boolean_11 = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //if (temp.Boolean_11 && temp.Boolean_22 && temp.Int32_2 >= 1 && temp.float3_3.z <= 0)
                    //{
                    //    triggerBuffer[i] = temp;
                    //    triggerBuffer.RemoveAt(i);
                    //    continue;
                    //}

                    //temp.Single_1 -= fdT;
                    //Debug.Log("2222222222222");
                    temp.Int32_2++;
                    temp.Single_15 -= fdT;

                    // if (temp.Single_1 < 0)
                    // {
                    //     triggerBuffer.RemoveAt(i);
                    //     return;
                    // }

                    triggerBuffer[i] = temp;
                }


                // for (var i = 0; i < skillsBuffer.Length; i++)
                // {
                //     for (int j = i + 1; j < skillsBuffer.Length; j++)
                //     {
                //         if (skillsBuffer[i].Int32_0 == skillsBuffer[j].Int32_0)
                //         {
                //             skillsBuffer.RemoveAt(j);
                //         }
                //     }
                // }
            }


            private bool RepeatCheck(Trigger temp, Entity entity, DynamicBuffer<Skill> skills, ChaStats chaStats,
                PlayerData playerData)
            {
                switch (temp.TriggerConditionType_7)
                {
                    case TriggerConditionType.SkillEffectIdFrom:
                        for (int j = 0; j < skills.Length; j++)
                        {
                            if (skills[j].Int32_0 == temp.Int32_10 && skills[j].int2x4_13.c0.x == temp.int4_8.x)
                            {
                                temp.Boolean_4 = true;
                                break;
                            }
                        }

                        break;
                    case TriggerConditionType.PropertyId:
                        var value = UnityHelper.GetProperty(playerData,
                            chaStats,
                            (int)temp.int4_8.x);
                        Debug.Log("每时间属性id");
                        switch (temp.Int32_17)
                        {
                            //等于
                            case 0:
                                if (value == temp.int4_8.y)
                                {
                                    Debug.Log("每时间属性id ===============");
                                    return true;
                                }

                                break;
                            //小于等于
                            case 1:
                                if (value <= temp.int4_8.y)
                                {
                                    Debug.Log("每时间属性id <<<<<<<<<<<<<<<<<<<<");
                                    return true;
                                }

                                break;
                            //大于等于
                            case 2:
                                if (value >= temp.int4_8.y)
                                {
                                    Debug.Log("每时间属性id >>>>>>>>>>>>>>>>>>>");
                                    return true;
                                }

                                break;
                        }

                        break;
                    case TriggerConditionType.KillEnemy:
                        ////默认相等
                        ////perNumCount *= (int)temp.int4_8.y;
                        //var killType = (uint)temp.int4_8.y;
                        //int currentKillCount = 0;
                        //if ((killType & 8) == 1)
                        //{
                        //    currentKillCount += cdfePlayerData[entity].playerOtherData.killLittleMonster;
                        //}
                        //if ((killType & 16) == 1)
                        //{
                        //    currentKillCount += cdfePlayerData[entity].playerOtherData.killLittleMonster;
                        //}
                        //if ((killType & 32) == 1)
                        //{
                        //    currentKillCount += cdfePlayerData[entity].playerOtherData.killLittleMonster;
                        //}
                        //temp.float4_9.y = currentKillCount;
                        //int addition = (int)temp.float4_9.y - (int)temp.float4_9.x;
                        //if (addition >= perNumCount)
                        //{
                        //    temp.Boolean_4 = true;

                        //}
                        break;
                    case TriggerConditionType.Dodging:
                        //默认等于

                        break;
                    case TriggerConditionType.Moving:
                        //默认等于

                        break;
                    case TriggerConditionType.AfterRebirth:
                        //默认等于
                        break;
                    case TriggerConditionType.SkillEffectIdTo:
                        //默认等于
                        break;
                }

                return false;
            }

            /// <summary>
            /// 仅用于boss技能 trigger的触发
            /// </summary>
            /// <param name="index"></param>
            /// <param name="temp"></param>
            /// <param name="entity"></param>
            private void OnTriggerForEnergy(ref DynamicBuffer<Buff> buffs, int sortKey, Trigger trigger, Entity entity,
                DynamicBuffer<Skill> skills, DynamicBuffer<Trigger> triggerBuffer)
            {
                Debug.Log($"OnTriggerForEnergy");
                ref var skillEffect =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                int triggerID = trigger.Int32_0;
                int index = -1;
                for (int i = 0; i < skillEffect.Length; i++)
                {
                    if (triggerID == skillEffect[i].id)
                    {
                        index = i;
                        break;
                    }
                }

                if (index < 0) return;


                var power = skillEffect[index].power;
                const int MaxPower = 10000;
                int dropRate = math.clamp(power, 0, MaxPower);
                bool canTrigger = gameRandomData.rand.NextInt(0, MaxPower + 1) <= dropRate;
                if (!canTrigger)
                    return;


                var calcType = skillEffect[index].calcType;
                ref var calcTypePara = ref skillEffect[index].calcTypePara;

                var skillID = skillEffect[index].skillId;

                if (calcType == 0)
                {
                    Debug.Log($"AddBossAttackAddTriggerOrAttack：{calcTypePara[0]}");
                    //发射一个弹幕
                    if (skillEffect[index].extraType == 3)
                    {
                        AddBulletWithEffectDir(calcTypePara[0], skillEffect[index].extraTypePara[0], sortKey, trigger,
                            skillID, skills, skillEffect[index].extraTypePara[1]);
                    }
                    else
                    {
                        for (int i = 0; i < calcTypePara.Length; i++)
                        {
                            AddEffect(ref buffs, calcTypePara[i], entity, sortKey, true,
                                trigger,
                                ref skills);
                        }
                    }
                }
                else if (calcType == 1)
                {
                    AddBullet(calcTypePara[0], sortKey, true, skillID, skills,
                        trigger, entity);
                }
                else if (calcType == 2)
                {
                    AddEvent(sortKey, trigger, ref calcTypePara, entity);
                }
                else if (calcType == 3)
                {
                    AddBuffForWeapon(skills, trigger, sortKey, ref calcTypePara);
                }
                else if (calcType == 4)
                {
                    int calcEffectType = calcTypePara[0];
                    if (calcEffectType == 1)
                    {
                        ChangeBossAttackType(ref calcTypePara, sortKey, entity, trigger);
                    }
                    else if (calcEffectType == 2)
                    {
                        ChangeBossMoveType(ref calcTypePara, sortKey, entity, trigger);
                    }
                    else if (calcEffectType == 3)
                    {
                        OnBackTrack(sortKey, entity, trigger);
                    }
                    else
                    {
                        OnBossMove(sortKey, entity, trigger, ref calcTypePara);
                    }
                }
                else if (calcType == 5)
                {
                    AddBuffForVulnerability(entity, sortKey, ref calcTypePara, trigger);
                }
                else if (calcType == 6)
                {
                    AddBeforeDieSkill(ref calcTypePara, sortKey, trigger, entity);
                }
                else if (calcType == 7)
                {
                    SelectSkillEffectFromGroup(ref calcTypePara, sortKey, trigger, ref triggerBuffer);
                }
            }

            private void AddBulletWithEffectDir(int effectId, int bulletId, int sortKey, Trigger trigger, int skillID,
                DynamicBuffer<Skill> skills, float delaytime)
            {
                var caster = trigger.Entity_12;
                var locer = trigger.Entity_20;

                ref var skillEffect = ref globalConfigData.value.Value.configTbskill_effectNews.Get(effectId);
                var rangeType = skillEffect.rangeType;

                var searchType = skillEffect.searchType;
                ref var searchPar = ref skillEffect.searchTypePara;
                ref var rangPar = ref skillEffect.rangeTypePara;
                var deviateType = skillEffect.deviateType;
                ref var deviateTypePar = ref skillEffect.deviateTypePara;
                ref var lockPar = ref skillEffect.targetLockOnPara;
                float lockInt = 0;
                for (int i = 0; i < lockPar.Length; i++)
                {
                    lockInt += math.pow(2, lockPar[i]);
                }

                var delayafter = skillEffect.delayType == 4 ? skillEffect.delayTypePara[0] / 1000f : 0;
                float3 dir = trigger.float3x2_24.c1;
                float3 targetPos = trigger.float3x2_24.c0;
                var time = GetBeTriggeredCount(trigger);
                Debug.Log($"time:{time}");
                Entity target = Entity.Null;
                Debug.Log("isnotseek");
                var castPos = BuffHelper.SeekTargets(ref searchPar, ref deviateTypePar, searchType, deviateType,
                    rangeType, ref rangPar,
                    allEntities, caster, locer, cdfeLocalTransform, cdfeTargetData, cdfeChaStats, (uint)lockInt,
                    out targetPos, out dir, out target, cdfeBuff, cdfeBulletTempTag, timeTick, time);
                Debug.Log($"target33444{target.Index}");
                LocalTransform targetTran = new LocalTransform
                {
                    Position = targetPos,
                    Scale = 1,
                    Rotation = quaternion.identity
                };
                if (storageInfoFromEntity.Exists(target) && cdfeLocalTransform.HasComponent(target))
                {
                    targetTran = cdfeLocalTransform[target];
                }


                if (BuffHelper.IsFloat3Equal(targetPos, float3.zero))
                {
                    ref var skillConfig =
                        ref globalConfigData.value.Value.configTbskills.configTbskills;
                    for (int i = 0; i < skillConfig.Length; i++)
                    {
                        if (trigger.Int32_10 == skillConfig[i].id && skillConfig[i].type == 1)
                        {
                            targetPos = castPos.Position;
                            if (cdfeChaStats.HasComponent(caster))
                            {
                                targetPos = castPos.Position + cdfeChaStats[caster].chaResource.direction *
                                    searchPar[0] * 0.5f / 1000f;
                            }

                            break;
                        }
                    }
                }

                Debug.Log($"dir:{dir},targetPos:{targetPos}");
                ref var targetParlist = ref skillEffect.targetPara;
                float targetPar = 0;
                for (int j = 0; j < targetParlist.Length; j++)
                {
                    targetPar += math.pow(2, targetParlist[j]);
                }
                castPos.Rotation =MathHelper.LookRotation2D(dir);
                UnityHelper.SpawnSpecialEffect(ref ecb, sortKey, eT, cdfeLocalTransform, cdfePostTransformMatrix,
                    gameTimeData,
                    globalConfigData,
                    ref skillEffect.specialEffects,
                    prefabMapData, caster, true, skillID, castPos, targetPos, skills, time, target);
                
                AddBullet(bulletId, sortKey, true, skillID, skills, trigger, caster, true, dir);
            }

            private void OnBossMove(int sortKey, Entity entity, Trigger trigger, ref BlobArray<int> calcTypePara)
            {
                ecb.AppendToBuffer(sortKey, entity,
                    new Buff_LetBossMove { carrier = entity, duration = calcTypePara[1] / 1000f, permanent = false }
                        .ToBuff());
            }

            //从技能组中选择一个trigger
            private void SelectSkillEffectFromGroup(ref BlobArray<int> calcTypePara,
                int sortKey, Trigger trigger, ref DynamicBuffer<Trigger> triggers)
            {
                int selectedTrigger = 0;

                var skilleffectIDs = new NativeArray<int>(calcTypePara.Length, Allocator.Temp);
                var skillPowers = new NativeArray<int>(calcTypePara.Length, Allocator.Temp);
                for (int i = 0; i < calcTypePara.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        var index = i / 2;
                        skilleffectIDs[index] = calcTypePara[i];
                    }
                    else
                    {
                        var index = i / 2;
                        skillPowers[index] = calcTypePara[i];
                    }
                }

                int sumPower = 0;
                for (int i = 0; i < skillPowers.Length; i++)
                {
                    sumPower += skillPowers[i];
                }

                var rand = Random.CreateFromIndex((uint)(gameRandomData.seed + sortKey));
                var randNum = rand.NextInt(0, sumPower);
                int tmpNum = 0;
                for (int i = 0; i < skillPowers.Length; i++)
                {
                    var tmp = skillPowers[i];
                    tmpNum += tmp;
                    if (tmpNum >= randNum)
                    {
                        selectedTrigger = skilleffectIDs[i];
                        break;
                    }
                }

                Debug.Log($"选择的id:{selectedTrigger}");
                AddTrigger(triggers, sortKey, selectedTrigger, trigger, trigger.Entity_12);
            }

            private void AddBuffForVulnerability(Entity entity, int sortKey, ref BlobArray<int> calcTypePara,
                Trigger trigger)
            {
                ecb.AppendToBuffer<Buff>(sortKey, entity,
                    new Buff_DiyHpAdditonDamage
                    {
                        carrier = entity, permanent = true,
                        argsNew = new BuffArgsNew
                        {
                            args1 = new int4(calcTypePara[0], calcTypePara[1], calcTypePara[2], 0),
                            args2 = new int4(trigger.Int32_10, 0, 0, 0)
                        },
                        skillId = trigger.Int32_10
                    }.ToBuff());
            }


            /// <summary>
            /// 添加弹幕
            /// </summary>
            /// <param name="bulletID">弹幕id</param>
            /// <param name="sortKey"></param>
            /// <param name="isSeeked">是否已经索敌 不需要再次索敌</param>
            /// <param name="loc">成功索敌后的目标位置</param>
            private void AddBullet(int bulletID, int sortKey, bool isSeeked,
                int skillID, DynamicBuffer<Skill> skills, Trigger trigger, Entity holder, bool isOrdered = false,
                float3 orderDir = default, float delayTime = 0f)
            {
                var caster = trigger.Entity_12;
                var locer = trigger.Entity_20;
                LocalTransform loc = cdfeLocalTransform[locer];

                bool isNeedMultip = false;
                //多次的倍率
                if (trigger.TriggerType_5 == TriggerType.PerNum &&
                    trigger.TriggerConditionType_7 == TriggerConditionType.KillEnemy)
                {
                    isNeedMultip = true;
                }

                int index = 0;
                ref var bulletConfig = ref globalConfigData.value.Value.configTbbullets.configTbbullets;
                for (int i = 0; i < bulletConfig.Length; i++)
                {
                    if (bulletConfig[i].id == bulletID)
                    {
                        index = i;
                        break;
                    }
                }

                ref var bullet = ref bulletConfig[index];
                float bulletDuration = (bullet.num / bullet.groupNum - 1) * bullet.interval / 1000f;

                Debug.Log($"bullet skill {bulletID}");


                if (bullet.trackType == 11)
                {
                    isSeeked = true;
                    loc = cdfeLocalTransform[caster];
                }

                ecb.AppendToBuffer(sortKey, caster, new BulletCastData
                {
                    id = bulletID,
                    //TODO:限定一个效果最大值 大于的话看不到两技能的间隔
                    duration = isNeedMultip
                        ? trigger.Int32_23 * bulletDuration
                        : bulletDuration,
                    tick = 0,
                    caster = caster,
                    isTargeted = isSeeked,
                    localTransform = loc,
                    curGroup = 0,
                    delay = delayTime,
                    bulletPos = default,
                    followedEntity = default,
                    skillId = skillID,
                    addTime = 0,
                    isOrderDir = isOrdered,
                    dir = orderDir,
                });
            }

            /// <summary>
            /// 用于蓄力
            /// </summary>
            /// <param name="sortKey"></param>
            /// <param name="temp"></param>
            /// <param name="entity"></param>
            /// <param name="targetPos"></param>
            /// <param name="dir"></param>
            /// <returns></returns>
            private LocalTransform SeekTargetForEnergy(int sortKey, Trigger temp, Entity entity, out float3 targetPos,
                out float3 dir, DynamicBuffer<Skill> skills, out LocalTransform targetTran, out Entity target,
                out int targetType)
            {
                dir = default;

                targetPos = float3.zero;


                ref var skillEffect =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                int triggerID = temp.Int32_0;

                int index = -1;
                for (int i = 0; i < skillEffect.Length; i++)
                {
                    if (triggerID == skillEffect[i].id)
                    {
                        index = i;
                        break;
                    }
                }

                var calcType = skillEffect[index].calcType;

                if (skillEffect[index].calcTypePara.Length > 1)
                {
                    while (true)
                    {
                        Debug.Log($"蓄力后不能跟多个值");
                    }
                }

                if (calcType != 0)
                {
                    while (true)
                    {
                        Debug.Log($"蓄力后没有跟效果id 检查配表,triggerID为{triggerID}");
                    }
                }

                var calcValue = skillEffect[index].calcTypePara[0];
                bool isCrash=false;
                if (skillEffect[index].extraType == 1)
                {
                    isCrash = true;
                }

                ref var specialEffectsIds = ref skillEffect[index].specialEffects;


                int effectIndex = -1;

                for (int i = 0; i < skillEffect.Length; i++)
                {
                    if (skillEffect[i].id == calcValue)
                    {
                        effectIndex = i;
                        break;
                    }
                }

                if (effectIndex < 0)
                {
                    while (true)
                    {
                        Debug.Log($"表中未找到effectid为{calcValue}的值");
                    }
                }
              

                

                ref var skilleffect = ref skillEffect[effectIndex];

                var searchType = skilleffect.searchType;
                ref var searchPar = ref skilleffect.searchTypePara;
                ref var rangPar = ref skilleffect.rangeTypePara;
                var deviateType = skilleffect.deviateType;
                ref var deviateTypePar = ref skilleffect.deviateTypePara;
                targetType = 0;
                if (skilleffect.searchType == 1)
                {
                    if (skilleffect.searchTypePara.Length >= 4)
                    {
                        targetType = skilleffect.searchTypePara[3];
                    }
                }

                if (skilleffect.effectType != 1)
                {
                    while (true)
                    {
                        Debug.Log($"配表有误 蓄力后没有跟效果类型为非触发效果的，id为{skilleffect.id}");
                    }
                }

                //var targetPar = skilleffect.targetPara[0];
                ref var lockPar = ref skilleffect.targetLockOnPara;
                float lockInt = 0;
                for (int i = 0; i < lockPar.Length; i++)
                {
                    lockInt += math.pow(2, lockPar[i]);
                }

                var time = GetBeTriggeredCount(temp);

                Debug.Log($"skilleffectid:{skilleffect.id} devipar:{deviateTypePar[0]}");
                var loc = BuffHelper.SeekTargets(ref searchPar, ref deviateTypePar, searchType, deviateType,
                    skilleffect.rangeType, ref rangPar,
                    allEntities, entity, entity, cdfeLocalTransform, cdfeTargetData, cdfeChaStats, (uint)lockInt,
                    out targetPos, out dir, out target, cdfeBuff, cdfeBulletTempTag, gameRandomData.seed, time);
                Debug.Log($"skilleffectid3333:{target.Index} ");
                targetTran = new LocalTransform
                {
                    Position = targetPos,
                    Scale = 1,
                    Rotation = quaternion.identity
                };
                if (storageInfoFromEntity.Exists(target) && cdfeLocalTransform.HasComponent(target))
                {
                    targetTran = cdfeLocalTransform[target];
                }

                if (BuffHelper.IsFloat3Equal(targetPos, float3.zero))
                {
                    ref var skillConfig =
                        ref globalConfigData.value.Value.configTbskills.configTbskills;
                    for (int i = 0; i < skillConfig.Length; i++)
                    {
                        if (temp.Int32_10 == skillConfig[i].id && skillConfig[i].type == 1)
                        {
                            targetPos = loc.Position + cdfeChaStats[entity].chaResource.direction * searchPar[0] *
                                0.5f / 1000f;
                            break;
                        }
                    }
                }

                //冲锋的选点规则不执行
                if (isCrash)
                {
                    ref var elementConfig = ref globalConfigData.value.Value.configTbskill_effectElements .configTbskill_effectElements;
                    ref var elementList=ref skilleffect.elementList;
                    int crashindex = 0;
                    for (int k = 0; k < elementList.Length; k++)
                    {
                        for (int m = 0; m < elementConfig.Length; m++)
                        {
                            if (elementList[k] == elementConfig[m].id && elementConfig[m].elementType == 6)
                            {
                                crashindex = m;
                                break;
                            }
                        }
                    }
                    if (ReturnForceMoveEffect(ref skilleffect, ref elementConfig, out var hitType))
                    {
                        ref var forceMove = ref elementConfig[crashindex];
                        var dirType = forceMove.directionType;
                        if (dirType == 1)
                        {
                            dir = -dir;
                        }


                        var pointType = forceMove.pointType;
                        ref var pointTypePar = ref forceMove.pointTypePara;
                        var displaceFrom = forceMove.displaceFrom;
                        if (pointType == 0)
                        {
                            float distance = pointTypePar[0] / 1000f;

                            loc.Position = GetRandomPointOutsideLine(displaceFrom,
                                distance - pointTypePar[1] / 1000f,
                                distance + pointTypePar[1] / 1000f, dir,entity);
                            //Debug.LogError($"crashPos0:{crashPos}");
                            dir = math.normalize(loc.Position - cdfeLocalTransform[entity].Position);
                        }
                        else if (pointType == 1)
                        {
                            float radius = pointTypePar[0] / 1000f;
                            loc.Position = GetRandomPointOutsideCircle(displaceFrom, pointTypePar[1] / 1000f,
                                radius, entity,targetPos);

                            //Debug.LogError($"crashPos1:{crashPos}");
                        }
                        else if (pointType == 4)
                        {
                            float crashX = 0f;
                            if (cdfeLocalTransform[entity].Position.x - targetPos.x > 0)
                            {
                                crashX = targetPos.x + pointTypePar[0] / 1000f;
                            }
                            else
                            {
                                crashX = targetPos.x - pointTypePar[0] / 1000f;
                            }
                            var crashY = targetPos.y + pointTypePar[1] / 1000f;
                            loc.Position = new float3(crashX, crashY, 0);
                        }
                        else if (pointType == 5)
                        {

                            var playerDir = cdfeChaStats[player].chaResource.direction;
                            if (playerDir.x > 0)
                            {
                                loc.Position.x = targetPos.x - pointTypePar[0] / 1000f;
                            }
                            else
                            {
                                loc.Position.x = targetPos.x + pointTypePar[0] / 1000f;
                            }
                            loc.Position.y = targetPos.y + pointTypePar[1] / 1000f;

                            if (!BuffHelper.IsInBossMap(loc.Position,cdfePlayerData[player].playerOtherData))
                            {
                                var tempDir = loc.Position - cdfeLocalTransform[player].Position;
                                loc.Position =cdfeLocalTransform[player].Position - tempDir;
                            }


                        }
                    }


                }






                Debug.LogError($"蓄力:{loc.Position},target:{targetPos}");
                //此处生成的是蓄力的特效
                UnityHelper.SpawnSpecialEffect(ref ecb, index, eT, cdfeLocalTransform, cdfePostTransformMatrix,
                    gameTimeData,
                    globalConfigData,
                    ref specialEffectsIds,
                    prefabMapData, entity, true, temp.Int32_10, loc, targetPos, skills, time, target);

                //GenerateBossWarning(loc, sortKey, dir, ref specialEffectsId);
                //GenerateWarning(index, ref skilleffect, ref rangPar, loc, duration);

                return loc;
                //else if (calcType == 1)
                //{
                //    //TODO:弹幕的索敌逻辑
                //}
                //else
                //{
                //}
            }

            private float3 GetRandomPointOutsideCircle(int displaceFrom,float offset, float radius, Entity caster, float3 targetPos)
            {
                var tempPos = displaceFrom == 1 ? cdfeLocalTransform[caster].Position : targetPos;
                var initPos = BuffHelper.GetRandomPointInCircle(tempPos, radius + offset, radius - offset, gameRandomData.seed);
                //圆 地图外
                if (!BuffHelper.IsInBossMap(initPos, cdfePlayerData[player].playerOtherData))
                {
                    var dir = initPos - cdfeLocalTransform[player].Position;
                    initPos = cdfeLocalTransform[player].Position - dir;
                    return initPos;
                }
                else
                {
                    if (!BuffHelper.IsRectCanUse(initPos, caster, globalConfigData, mapModules,
                      cdfeMapElementData, cdfeLocalTransform, cdfePhysicsCollider, cdfePlayerData[player].playerOtherData, player))
                    {
                        return initPos;
                    }
                    else
                    {
                        //根据规则扩大选点
                        int initStep = 0;
                        float3 pos = initPos;
                        do
                        {
                            initStep += 2;
                            int times = 5;

                            do
                            {
                                times--;
                                pos = BuffHelper.GetRandomPointInCircle(initPos, initStep, initStep - 2,
                                    (uint)(gameRandomData.seed + times + initStep));
                            } while (!BuffHelper.IsRectCanUse(pos, caster, globalConfigData,mapModules,
                      cdfeMapElementData,cdfeLocalTransform, cdfePhysicsCollider,cdfePlayerData[player].playerOtherData,player) &&
                                     times > 0);
                        } while (!!BuffHelper.IsRectCanUse(pos, caster, globalConfigData, mapModules,
                      cdfeMapElementData, cdfeLocalTransform, cdfePhysicsCollider, cdfePlayerData[player].playerOtherData, player) &&
                                 initStep < 100);

                        if (math.length(pos - initPos) < math.EPSILON)
                        {
                            return cdfeLocalTransform[caster].Position;
                        }

                        return pos;
                    }
                }
            }

            private float3 GetRandomPointOutsideLine(int displaceFrom, float x, float y, float3 dir, Entity caster)
            {
                dir = displaceFrom == 1 ? cdfeChaStats[caster].chaResource.direction : dir;

                var initPos = BuffHelper.GetRandomPointOutsideLine(caster, cdfeLocalTransform, cdfeChaStats,
                    x, y, dir,gameRandomData.seed);
                if (!BuffHelper.IsInBossMap(initPos, cdfePlayerData[player].playerOtherData))
                {
                    float3 pos = initPos;
                    if (BuffHelper.TryGetIntersectionWithVertical(new float2(initPos.xy), new float2(dir.xy), new float2(0, 1), out float2 intersection))
                    {
                        pos.x = intersection.x;
                        pos.y = intersection.y;
                        return pos;
                    }
                    return initPos;
                }
                else
                {
                    if (BuffHelper.IsRectCanUse(initPos, caster, globalConfigData,mapModules,
                      cdfeMapElementData,cdfeLocalTransform, cdfePhysicsCollider,cdfePlayerData[player].playerOtherData,player))
                    {
                        return initPos;
                    }
                    else
                    {
                        //根据规则扩大选点
                        int initStep = 0;
                        float3 pos = initPos;
                        do
                        {
                            initStep += 2;
                            int times = 5;

                            do
                            {
                                times--;
                                pos = BuffHelper.GetRandomPointInCircle(initPos, initStep, initStep - 2,
                                    (uint)(gameRandomData.seed + times + initStep));
                            } while (!BuffHelper.IsRectCanUse(pos, caster, globalConfigData, mapModules,
                      cdfeMapElementData, cdfeLocalTransform, cdfePhysicsCollider, cdfePlayerData[player].playerOtherData, player) &&
                                     times > 0);
                        } while (!BuffHelper.IsRectCanUse(pos, caster, globalConfigData, mapModules,
                      cdfeMapElementData, cdfeLocalTransform, cdfePhysicsCollider, cdfePlayerData[player].playerOtherData, player) &&
                                 initStep < 100);

                        if (math.length(initPos - pos) < math.EPSILON)
                        {
                            return cdfeLocalTransform[caster].Position;
                        }

                        return pos;
                    }
                }

            }

            private void GenerateWarning(int index, ref ConfigTbskill_effectNew skilleffect, ref BlobArray<int> rangPar,
                LocalTransform loc, float duration)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tbtrigger"></param>
            /// <param name="buffs"></param>
            /// <param name="triggers"></param>
            /// <param name="skills"></param>
            /// <param name="sortKey"></param>
            /// <param name="trigger"></param>
            /// <param name="caster">trigger的释放者</param>
            /// <param name="holder">trigger的挂载者 一般谁释放谁挂载</param>
            /// <param name="timeTick"></param>
            private void OnTrigger(ref ConfigTbskill_effectNew tbtrigger, ref DynamicBuffer<Buff> buffs,
                ref Trigger trigger,
                GlobalConfigData globalConfigData, GameOthersData gameOthersData,
                DynamicBuffer<Trigger> triggers,
                DynamicBuffer<Skill> skills, int sortKey,
                Entity holder, uint timeTick)
            {
                var caster = trigger.Entity_12;
                var locer = trigger.Entity_20;


                ///caster 为挂载着 holder为释放者 一般是一样的


                var rand = Random.CreateFromIndex((uint)(gameRandomData.seed + sortKey + holder.Index +
                                                         holder.Version));
                var dropRate = math.clamp(trigger.Int32_14, 0, 10000);
                var canTrigger = rand.NextInt(0, 10000) <= dropRate;
                if (isDebug)
                {
                    Debug.Log($"OnTrigger ,TriggerId:{trigger.Int32_0},dropRate:{dropRate}");
                }

                if (!canTrigger)
                    return;

                #region AnimationAndSE

                if (!tbtrigger.animation.IsEmpty)
                {
                    if (UnityHelper.TryGetAnimEnum(tbtrigger.animation, out var animationEnum))
                    {
                        if (cdfeStateMachine.HasComponent(holder))
                        {
                            var stateMachine = cdfeStateMachine[holder];
                            stateMachine.curAnim = animationEnum;
                            stateMachine.animStr = tbtrigger.animation;
                            stateMachine.animSpeedScale = tbtrigger.animationSpeed / 10000f;
                            cdfeStateMachine[holder] = stateMachine;
                        }
                    }
                    else
                    {
                        Debug.Log($"OnTrigger ,Animation:{tbtrigger.animation} {tbtrigger.id}");
                        if (cdfeStateMachine.HasComponent(holder))
                        {
                            var stateMachine = cdfeStateMachine[holder];
                            stateMachine.curAnim = (AnimationEnum)4;
                            stateMachine.animStr = tbtrigger.animation;
                            stateMachine.animSpeedScale = tbtrigger.animationSpeed / 10000f;
                            cdfeStateMachine[holder] = stateMachine;
                        }
                    }
                }

                if (tbtrigger.audio.Length > 0)
                {
                    NativeList<int> closeAudioId = new NativeList<int>(Allocator.Temp);
                    for (int i = 0;
                         i < globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews.Length;
                         i++)
                    {
                        ref var skillEffectNew = ref globalConfigData.value.Value.configTbskill_effectNews
                            .configTbskill_effectNews[i];
                        if (tbtrigger.skillId == skillEffectNew.skillId)
                        {
                            for (int j = 0; j < skillEffectNew.audioClose.Length; j++)
                            {
                                closeAudioId.Add(skillEffectNew.audioClose[j]);
                            }
                        }
                    }


                    for (int i = 0; i < tbtrigger.audio.Length; i++)
                    {
                        ref var audio = ref globalConfigData.value.Value.configTbaudios.Get(tbtrigger.audio[i]);
                        Debug.Log($"audio创建 skillId:{trigger.Int32_10} triggerId:{trigger.Int32_0} audioId:{audio.id}");
                        UnityHelper.TryCreateAudioClip(globalConfigData, gameOthersData, audio.id,
                            out var eventInstance);
                        if (closeAudioId.Contains(audio.id))
                        {
                            trigger.FixedList128BytesFMODStudioEventInstance_30.Add(eventInstance);
                            trigger.FixedList128Bytesint_31.Add(audio.id);
                        }
                    }
                }


                for (int i = 0; i < tbtrigger.audioClose.Length; i++)
                {
                    ref var audio = ref globalConfigData.value.Value.configTbaudios.Get(tbtrigger.audioClose[i]);

                    for (int j = 0; j < triggers.Length; j++)
                    {
                        var trigger0 = triggers[j];

                        if (trigger0.Int32_10 == trigger.Int32_10 &&
                            trigger0.FixedList128Bytesint_31.Contains(audio.id))
                        {
                            var index1 = trigger0.FixedList128Bytesint_31.IndexOf(audio.id);
                            var audio1 = trigger0.FixedList128BytesFMODStudioEventInstance_30.ElementAt(index1);
                            UnityHelper.DestroyAudioClip(audio1);
                            Debug.Log($"audioClose字段销毁:{audio.id}");
                        }
                    }
                }

                #endregion


                int triggerID = trigger.Int32_0;
                //ref var elementArray = ref temp;
                int index = 0;

                //TODO:判断是不是实体技能类
                ref var skillEffect =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                for (int i = 0; i < skillEffect.Length; ++i)
                {
                    if (skillEffect[i].id == triggerID)
                    {
                        index = i;
                        break;
                    }
                }

                ref var skilleffect = ref skillEffect[index];

                var calcType = skilleffect.calcType;
                ref var calcEffect = ref skilleffect.calcTypePara;
                var skillID = skilleffect.skillId;


                switch (calcType)
                {
                    //效果id
                    case 0:
                        Debug.Log($"AddTriggerOrAttack：{calcEffect[0]}");
                        //ref var skilleffect= ref globalConfigData.value.Value.configTbskill_effectNews.Get(calcEffect[0]);
                        if (skilleffect.extraType == 3)
                        {
                            Debug.Log($"extraType:{skilleffect.extraType}");
                            AddBulletWithEffectDir(calcEffect[0], skilleffect.extraTypePara[0], index, trigger,
                                trigger.Int32_10, skills, skilleffect.extraTypePara[1]);
                        }
                        else
                        {
                            AddTriggerOrAttack(ref buffs, triggers, sortKey, ref calcEffect, trigger, holder, rand,
                                ref skills);
                        }

                        //AddSkillEffectOrBuff(ref refData, inData,ref calcEffect);
                        break;
                    //弹幕
                    case 1:
                        Debug.Log($"AddBullet：{calcEffect[0]}");
                        AddBullet(calcEffect[0], sortKey, false, skillID, skills,
                            trigger, holder);
                        break;
                    //事件
                    case 2:
                        Debug.Log($"AddEvent：{calcEffect[0]}");
                        AddEvent(sortKey, trigger, ref calcEffect, holder);
                        break;
                    //伤害修正
                    case 3:
                        Debug.Log($"AddBuffForWeapon：{calcEffect[0]}");
                        AddBuffForWeapon(skills, trigger, sortKey, ref calcEffect);
                        break;
                    //boss输出方式变更
                    case 4:
                        int calcEffectType = calcEffect[0];
                        if (calcEffectType == 1)
                        {
                            ChangeBossAttackType(ref calcEffect, sortKey, holder, trigger);
                        }
                        else if (calcEffectType == 2)
                        {
                            ChangeBossMoveType(ref calcEffect, sortKey, holder, trigger);
                        }
                        else if (calcEffectType == 3)
                        {
                            OnBackTrack(sortKey, holder, trigger);
                        }
                        else
                        {
                            OnBossMove(sortKey, holder, trigger, ref calcEffect);
                        }

                        break;
                    //易伤修正
                    case 5:
                        AddBuffForVulnerability(holder, sortKey, ref calcEffect, trigger);
                        break;
                    case 6:
                        AddBeforeDieSkill(ref calcEffect, sortKey, trigger, holder);
                        break;
                    //效果组
                    case 7:
                        SelectSkillEffectFromGroup(ref calcEffect, sortKey, trigger, ref triggers);
                        break;
                }
                ////
                //if (calcType==0&& calcEffect.Length<=0)
                //{
                //    var extraType = skilleffect.extraType;
                //    ref var extraTypePar = ref skilleffect.extraTypePara;
                //    switch (extraType)
                //    {
                //        case 1:
                //            RushNotHit(ref calcEffect, sortKey, holder, trigger);
                //            break;
                //    }
                //}
                //else
                //{

                //}
            }

            /// <summary>
            /// 冲锋未命中
            /// </summary>
            /// <param name="calcEffect"></param>
            /// <param name="sortKey"></param>
            /// <param name="holder"></param>
            /// <param name="trigger"></param>
            private void RushNotHit(ref BlobArray<int> calcEffect, int sortKey, Entity holder, Trigger trigger)
            {
            }

            //触发回溯
            private void OnBackTrack(int index, Entity holder,
                Trigger trigger)
            {
                Debug.Log($"触发回溯");
                var buffer = bfeBackTrackTimeBuffer[holder];
                var tran = cdfeLocalTransform[holder];
                tran.Position = buffer[0].position;
                ecb.SetComponent(index, holder, tran);
            }

            /// <summary>
            /// 变更boss移动轨迹
            /// </summary>
            /// <param name="calcEffect"></param>
            /// <param name="index"></param>
            /// <param name="holder"></param>
            /// <param name="trigger"></param>
            private void ChangeBossMoveType(ref BlobArray<int> calcEffect, int index, Entity holder, Trigger trigger)
            {
                var buff = new Buff_ChangeBossMove
                {
                    duration = calcEffect[1] / 1000f,
                    permanent = false,
                    caster = default,
                    carrier = holder,
                    argsNew = new BuffArgsNew
                    {
                        args1 = new int4(calcEffect[2], 0, 0, 0), args2 = new int4(calcEffect[3], 0, 0, 0),
                        args3 = new int4(calcEffect[4], 0, 0, 0), args4 = new int4(calcEffect[5], 0, 0, 0)
                    }
                }.ToBuff();
                ecb.AppendToBuffer<Buff>(index, holder, buff);
            }

            private void AddBeforeDieSkill(ref BlobArray<int> calcEffect, int sortKey, Trigger trigger, Entity holder)
            {
                int skillID = calcEffect[0];
                int level = calcEffect[1];
                ecb.AppendToBuffer<Buff>(sortKey, holder,
                    new Buff_BeforeBeKilled
                    {
                        argsNew = new BuffArgsNew { args1 = new int4(skillID, level, 0, 0) }, carrier = holder,
                        permanent = true
                    }.ToBuff());
            }

            /// <summary>
            /// 变更boss输出方式
            /// </summary>
            /// <param name="calcEffect"></param>
            /// <param name="index"></param>
            /// <param name="holder"></param>
            /// <param name="trigger"></param>
            private void ChangeBossAttackType(ref BlobArray<int> calcEffect, int index, Entity holder, Trigger trigger)
            {
                var buff = new Buff_ChangeBossAttack
                {
                    duration = calcEffect[2] / 1000f,
                    permanent = false,
                    caster = default,
                    carrier = holder,
                    argsNew = new BuffArgsNew { args1 = new int4(calcEffect[1], 0, 0, 0) }
                }.ToBuff();
                ecb.AppendToBuffer<Buff>(index, holder, buff);
            }

            private void AddEvent(int sortkey, Trigger trigger, ref BlobArray<int> calcTypePara, Entity holder)
            {
                Entity caster = trigger.Entity_12;
                ref var skillConfig =
                    ref globalConfigData.value.Value.configTbskills.configTbskills;
                ref var eventConfig =
                    ref globalConfigData.value.Value.configTbevent_0s.configTbevent_0s;
                bool isWordEvent = false;
                int index = -1;
                for (int i = 0; i < skillConfig.Length; i++)
                {
                    if (skillConfig[i].id == trigger.Int32_10 && skillConfig[i].skillEventArray.Length > 0)
                    {
                        isWordEvent = true;
                        index = i;
                        break;
                    }
                }

                if (isWordEvent)
                {
                    ref var events = ref skillConfig[index].skillEventArray;
                    int eventIndex = -1;
                    for (int i = 0; i < events.Length; i++)
                    {
                        for (int j = 0; j < eventConfig.Length; j++)
                        {
                            if (eventConfig[j].id == events[i])
                            {
                                eventIndex = j;
                                break;
                            }
                        }

                        var eventType = eventConfig[eventIndex].type;
                        var gameEvent = new GameEvent
                        {
                            CurrentTypeId = (GameEvent.TypeId)eventType,
                            Int32_0 = events[i],
                            Int32_15 = trigger.Int32_10
                        };
                        Debug.Log($"eventid:{events[i]}");
                        ecb.AppendToBuffer<GameEvent>(sortkey, wbe, gameEvent);
                    }
                }
                else
                {
                    for (int k = 0; k < calcTypePara.Length; k++)
                    {
                        index = 0;

                        for (int i = 0; i < eventConfig.Length; i++)
                        {
                            if (eventConfig[i].id == calcTypePara[k])
                            {
                                index = i;
                                break;
                            }
                        }

                        var eventType = eventConfig[index].type;
                        var gameEvent = new GameEvent
                        {
                            CurrentTypeId = (GameEvent.TypeId)eventType,
                            Int32_0 = calcTypePara[k],
                            Int32_15 = trigger.Int32_10
                        };
                        ecb.AppendToBuffer<GameEvent>(sortkey, caster, gameEvent);
                    }
                }
            }

            /// <summary>
            /// 输出修正
            /// </summary>
            /// <param name="skills"></param>
            /// <param name="caster"></param>
            /// <param name="trigger"></param>
            /// <param name="index"></param>
            /// <param name="calcEffect"></param>
            private void AddBuffForWeapon(DynamicBuffer<Skill> skills, Trigger trigger, int index,
                ref BlobArray<int> calcEffect)
            {
                Entity caster = trigger.Entity_12;


                //trigger.Boolean_19 = false;
                bool isNeedMultip = false;
                //多次的倍率
                if (trigger.TriggerType_5 == TriggerType.PerNum &&
                    trigger.TriggerConditionType_7 == TriggerConditionType.KillEnemy)
                {
                    isNeedMultip = true;
                }

                int needSkillCount = 0;
                for (int i = 0; i < skills.Length; ++i)
                {
                    if (skills[i].Int32_0 == cdfePlayerData[caster].playerOtherData.weaponSkillId)
                    {
                        var temp = skills[i];
                        //temp.int3_14.y = temp.int3_14.x + 1;
                        needSkillCount = temp.int3_14.x + 1;
                        if (trigger.TriggerType_5 == TriggerType.WeaponAttack)
                        {
                            //temp.int3_14.y = temp.int3_14.x;
                            needSkillCount = temp.int3_14.x;
                        }

                        temp.int3_14.z = trigger.Int32_0;
                        skills[i] = temp;
                        break;
                    }
                }

                if (isNeedMultip)
                {
                    for (int i = 0; i < trigger.Int32_23; i++)
                    {
                        var buff = new Buff_DiyAdditonDamage
                        {
                            id = trigger.Int32_10,
                            startTime = 0,
                            timeElapsed = 0,
                            duration = 0,
                            permanent = true,
                            caster = default,
                            carrier = caster,
                            changeType = 0,
                            canClear = 0,
                            immuneStack = 0,
                            direction = default,
                            buffState = 0,
                            argsNew = new BuffArgsNew
                            {
                                args1 = new int4(calcEffect[0], calcEffect[1], calcEffect[2], 0),
                                args2 = new int4(needSkillCount, int3.zero)
                            },
                            priority = 0,
                            skillId = trigger.Int32_10
                        }.ToBuff();

                        ecb.AppendToBuffer<Buff>(index, caster, buff);
                    }
                }
                else
                {
                    var buff = new Buff_DiyAdditonDamage
                    {
                        id = trigger.Int32_10,
                        startTime = 0,
                        timeElapsed = 0,
                        duration = 0,
                        permanent = true,
                        caster = default,
                        carrier = caster,
                        changeType = 0,
                        canClear = 0,
                        immuneStack = 0,
                        direction = default,
                        buffState = 0,
                        argsNew = new BuffArgsNew
                        {
                            args1 = new int4(calcEffect[0], calcEffect[1], calcEffect[2], 0),
                            args2 = new int4(needSkillCount, int3.zero)
                        },
                        priority = 0,
                        skillId = trigger.Int32_10
                    }.ToBuff();

                    ecb.AppendToBuffer<Buff>(index, caster, buff);
                }
            }

            private void GetSkillEffectNum(ref Trigger trigger, DynamicBuffer<Skill> skills,
                int perNumEffectId, int perNumCount, int skillID)
            {
                for (int j = 0; j < skills.Length; j++)
                {
                    if (skills[j].Int32_0 == skillID)
                    {
                        var skill = skills[j];
                        if (skill.int2x4_13.c0.x == perNumEffectId)
                        {
                            if (skill.int2x4_13.c0.y % perNumCount == 0 &&
                                skill.int2x4_13.c0.y >= perNumCount &&
                                (int)trigger.float4_9.x != skill.int2x4_13.c0.y)
                            {
                                //Debug.Log($"1");
                                Debug.Log(
                                    $"id:{perNumEffectId},当前次数:{skill.int2x4_13.c0.y}，上一次次数:{trigger.float4_9.x}");
                                trigger.float4_6.z--;

                                trigger.float4_9.x = skill.int2x4_13.c0.y;

                                trigger.Boolean_4 = true;
                            }
                        }
                        else if (skill.int2x4_13.c1.x == perNumEffectId)
                        {
                            if (skill.int2x4_13.c1.y % perNumCount == 0 &&
                                skill.int2x4_13.c1.y >= perNumCount &&
                                (int)trigger.float4_9.x != skill.int2x4_13.c1.y)
                            {
                                Debug.Log(
                                    $"id:{perNumEffectId},当前次数:{skill.int2x4_13.c1.y}，上一次次数:{trigger.float4_9.x}");
                                trigger.float4_6.z--;


                                trigger.float4_9.x = skill.int2x4_13.c1.y;

                                trigger.Boolean_4 = true;
                            }
                        }
                        else if (skill.int2x4_13.c2.x == perNumEffectId)
                        {
                            if (skill.int2x4_13.c2.y % perNumCount == 0 &&
                                skill.int2x4_13.c2.y >= perNumCount &&
                                (int)trigger.float4_9.x != skill.int2x4_13.c2.y)
                            {
                                Debug.Log(
                                    $"id:{perNumEffectId},当前次数:{skill.int2x4_13.c2.y}，上一次次数:{trigger.float4_9.x}");
                                trigger.float4_6.z--;


                                trigger.float4_9.x = skill.int2x4_13.c2.y;

                                trigger.Boolean_4 = true;
                            }
                        }
                        else if (skill.int2x4_13.c3.x == perNumEffectId)
                        {
                            if (skill.int2x4_13.c3.y % perNumCount == 0 &&
                                skill.int2x4_13.c3.y >= perNumCount &&
                                (int)trigger.float4_9.x != skill.int2x4_13.c3.y)
                            {
                                Debug.Log(
                                    $"id:{perNumEffectId},当前次数:{skill.int2x4_13.c3.y}，上一次次数:{trigger.float4_9.x}");
                                trigger.float4_6.z--;


                                trigger.float4_9.x = skill.int2x4_13.c3.y;

                                trigger.Boolean_4 = true;
                            }
                        }

                        break;
                    }
                }
            }

            /// <summary>
            /// 加触发器或者effect
            /// </summary>
            /// <param name="buffs"></param>
            /// <param name="triggersbuffer"></param>
            /// <param name="index"></param>
            /// <param name="calcEffect"></param>
            /// <param name="trigger"></param>
            /// <param name="caster"></param>
            /// <param name="holder"></param>
            /// <param name="rand"></param>
            /// <param name="skills"></param>
            private void AddTriggerOrAttack(ref DynamicBuffer<Buff> buffs, DynamicBuffer<Trigger> triggersbuffer,
                int index,
                ref BlobArray<int> calcEffect,
                Trigger trigger, Entity holder, Random rand, ref DynamicBuffer<Skill> skills)
            {
                var caster = trigger.Entity_12;
                var locer = trigger.Entity_20;
                if (isDebug)
                {
                    Debug.Log($"AddTriggerOrAttack,form {trigger.Int32_0},to{calcEffect[0]}");
                }

                if (!storageInfoFromEntity.Exists(locer))
                {
                    locer = caster;
                }

                trigger.LocalTransform_25 = cdfeLocalTransform[locer];


                ref var skillEffect =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                var triggers = new NativeList<int>(Allocator.Temp);
                for (int i = 0; i < skillEffect.Length; i++)
                {
                    if (skillEffect[i].effectType == 2)
                    {
                        triggers.Add(skillEffect[i].id);
                    }
                }

                for (int i = 0; i < calcEffect.Length; i++)
                {
                    if (triggers.Contains(calcEffect[i]))
                    {
                        AddTrigger(triggersbuffer, index, calcEffect[i], trigger, caster);
                    }
                    else
                    {
                        AddEffect(ref buffs, calcEffect[i], holder, index, trigger.Boolean_13, trigger, ref skills);
                    }
                }
            }

            /// <summary>
            /// 由trigger添加trigger ，两个trigger的状态保持一致
            /// </summary>
            /// <param name="triggers"></param>
            /// <param name="sortkey"></param>
            /// <param name="triggerID"></param>
            /// <param name="triggerOld">原先的trigger</param>
            /// <param name="caster"></param>
            private void AddTrigger(DynamicBuffer<Trigger> triggers, int sortkey, int triggerID, Trigger triggerOld,
                Entity caster)
            {
                ref var skillEffect =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                int index = 0;
                for (int i = 0; i < skillEffect.Length; i++)
                {
                    if (skillEffect[i].id == triggerID)
                    {
                        index = i;
                        break;
                    }
                }

                ref var skilleffect = ref skillEffect[index];

                /////先判断是否能添加trigger
                //int power = skilleffect.power;
                //var rand = Random.CreateFromIndex((uint)(gameRandomData.seed + index + caster.Index + caster.Version));
                //var dropRate = math.clamp(power, 0, 10000);
                //Debug.Log($"OnTrigger ,TriggerId:{triggerID},dropRate:{dropRate}");
                //var canTrigger = rand.NextInt(0, 10001) <= dropRate;
                //if (!canTrigger)
                //    return;

                ////判断是否包含 包含则重置trigger
                //bool isContain = false;
                //for (int j = 0; j < triggers.Length; j++)
                //{
                //    if (triggers[j].Int32_0 == skilleffect.id)
                //    {
                //        isContain = true;
                //        break;
                //    }
                //}

                //if (isContain)
                //{
                //    Debug.Log($"isContainisContainisContain:{skilleffect.id}");
                //    for (int j = 0; j < triggers.Length; j++)
                //    {
                //        if (triggers[j].Int32_0 == skilleffect.id)
                //        {
                //            var trigger = triggers[j];
                //            trigger.Int32_2 = 0;
                //            trigger.float4_6.z = trigger.float4_6.y;
                //            trigger.float3_3.z = trigger.float3_3.y;
                //            trigger.Single_15 =
                //                trigger.float4_6.x != 0 ? math.max(trigger.float4_6.x / 1000f, 0.02f) : 0.5f;
                //            trigger.Boolean_11 = triggerOld.Boolean_11;
                //            triggers[j] = trigger;
                //        }
                //    }

                //    return;
                //}
                Debug.Log($"!!!!!!!!!!!!!!!!!!!!!!!!isContainisContainisContain:{skilleffect.id}");
                float delay = 0;
                TriggerType type = (TriggerType)skilleffect.triggerType;
                int delayType = skilleffect.delayType;
                float4 triggerTypeArgs = default;
                int4 triggerConditionTypeArgs = default;
                TriggerConditionType condition = default;
                condition = (TriggerConditionType)(skilleffect.conditionType == 4
                    ? skilleffect.conditionType * 10 + skilleffect.conditionTypePara[0]
                    : skilleffect.conditionType);
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

                float haloGap = 0.5f;
                if (skilleffect.triggerType == (int)TriggerType.Halo)
                {
                    if (skilleffect.triggerTypePara[0] != 0)
                    {
                        haloGap = skilleffect.triggerTypePara[0] / 1000f;
                    }
                }

                //if (isDebug)
                //{
                //    Debug.Log($"dropRate AddTrigger:{dropRate}");
                //}

                //后延不算在延迟里面
                if (delayType == 4)
                {
                    delay = 0f;
                }

                triggerTypeArgs.z = triggerTypeArgs.y;

                ecb.AppendToBuffer(sortkey, caster, new Trigger
                {
                    CurrentTypeId = (Trigger.TypeId)101,
                    Int32_0 = skilleffect.id,
                    Single_1 = 0,
                    Int32_2 = 0,
                    float3_3 = new float3(delayType,
                        delay,
                        delay),
                    Boolean_4 = false,
                    TriggerType_5 = type,
                    float4_6 = triggerTypeArgs,
                    TriggerConditionType_7 = condition,
                    int4_8 = triggerConditionTypeArgs,
                    float4_9 = default,
                    Int32_10 = triggerOld.Int32_10,
                    Boolean_11 = triggerOld.Boolean_11,
                    Entity_12 = triggerOld.Entity_12,
                    Boolean_13 = false,
                    Int32_14 = skilleffect.power,
                    Single_15 = haloGap,
                    Int32_16 = 0,
                    Int32_17 = skilleffect.compareType,
                    Entity_20 = triggerOld.Entity_20,
                    Boolean_22 = type != TriggerType.Halo ? true : false,
                    Boolean_26 = true,
                    Boolean_29 = triggerOld.Boolean_29
                });
            }

            /// <summary>
            /// 添加skillEffect
            /// </summary>
            /// <param name="buffs"></param>
            /// <param name="effectID"></param>
            /// <param name="caster"></param>
            /// <param name="holder"></param>
            /// <param name="index"></param>
            /// <param name="isSeeked"></param>
            /// <param name="castPos"></param>
            /// <param name="trigger"></param>
            /// <param name="skills"></param>
            private void AddEffect(ref DynamicBuffer<Buff> buffs, int effectID, Entity holder,
                int index, bool isSeeked,
                Trigger trigger, ref DynamicBuffer<Skill> skills)
            {
                var caster = trigger.Entity_12;
                var locer = trigger.Entity_20;
                if (!storageInfoFromEntity.Exists(locer))
                {
                    locer = caster;
                }

                LocalTransform castPos = cdfeLocalTransform[locer];


                if (trigger.Int32_16 != 0)
                {
                    return;
                }


                if (isDebug)
                {
                    Debug.Log($"AddEffect {effectID}");
                }

                ref var skillEffectConfig =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                int skillEffectIndex = 0;
                bool isFinded = false;
                for (int i = 0; i < skillEffectConfig.Length; i++)
                {
                    if (skillEffectConfig[i].id == effectID)
                    {
                        skillEffectIndex = i;
                        isFinded = true;
                        break;
                    }
                }

                if (!isFinded)
                {
                    return;
                }

                ref var skillEffect =
                    ref skillEffectConfig[skillEffectIndex];

                int power = skillEffect.power;
                //power = 7000;
                var rand = Random.CreateFromIndex((uint)(gameRandomData.seed + index + caster.Index + caster.Version));
                var dropRate = math.clamp(power, 0, 10000);

                var randValue = rand.NextInt(0, 10000);
                var canTrigger = randValue <= dropRate;


                if (!canTrigger)
                    return;
                if (isDebug)
                {
                    Debug.Log($"AddEffect dropRate:{randValue}");
                    Debug.Log($"caster:{caster.Index},holder:{holder.Index},player:{player.Index}");
                }

                if ((skillEffect.target == 3 && caster == player)
                    || (skillEffect.target == 5 &&
                        cdfeTargetData[caster].BelongsTo == (uint)BuffHelper.TargetEnum.BossMonster)
                    || (skillEffect.target == 4 &&
                        cdfeTargetData[caster].BelongsTo == (uint)BuffHelper.TargetEnum.LittleMonster)
                    || (skillEffect.target == 6 &&
                        cdfeTargetData[caster].BelongsTo == (uint)BuffHelper.TargetEnum.ElliteMonster)
                    || trigger.Boolean_18 && storageInfoFromEntity.Exists(trigger.Entity_20) && trigger.float4_6.y == 1)
                {
                    ref var elementList = ref skillEffect.elementList;
                    ref var stateList = ref skillEffect.battleStatus;

                    //TODO:加自身buff

                    AddBuffToPlayer(ref buffs, ref elementList, ref stateList, caster, index, ref skillEffect,
                        ref skills,
                        trigger);
                    if (trigger.Int32_10 == 511006) return;

                    #region SpecialEffects

                    UnityHelper.SpawnSpecialEffect(ref ecb, index, eT, cdfeLocalTransform, cdfePostTransformMatrix,
                        gameTimeData,
                        globalConfigData,
                        ref skillEffect.specialEffects,
                        prefabMapData, holder, false, trigger.Int32_10, default, default, skills, 1, caster);

                    #endregion

                    return;
                }

                if (trigger.Boolean_18 && storageInfoFromEntity.Exists(trigger.Entity_20) && trigger.float4_6.y == 0)
                {
                    ref var elementList = ref skillEffect.elementList;
                    ref var stateList = ref skillEffect.battleStatus;

                    //TODO:加目标buff

                    AddBuffToPlayer(ref buffs, ref elementList, ref stateList, trigger.Entity_20, index,
                        ref skillEffect,
                        ref skills,
                        trigger);

                    if (trigger.Int32_10 == 511006) return;

                    #region SpecialEffects

                    UnityHelper.SpawnSpecialEffect(ref ecb, index, eT, cdfeLocalTransform, cdfePostTransformMatrix,
                        gameTimeData,
                        globalConfigData,
                        ref skillEffect.specialEffects,
                        prefabMapData, holder, false, trigger.Int32_10, default, default, skills, 1, trigger.Entity_20);

                    #endregion

                    return;
                }

                bool isNeedMultip = false;
                //多次的倍率
                if (trigger.TriggerType_5 == TriggerType.PerNum &&
                    trigger.TriggerConditionType_7 == TriggerConditionType.KillEnemy)
                {
                    isNeedMultip = true;
                }

                if (isNeedMultip)
                {
                    for (int i = 0; i < trigger.Int32_23; i++)
                    {
                        GenerateEffectAttack(ref skillEffect, isSeeked, holder, index, effectID,
                            trigger, ref skills);
                    }
                }
                else
                {
                    GenerateEffectAttack(ref skillEffect, isSeeked, holder, index, effectID, trigger,
                        ref skills);
                }
            }

            private void GenerateEffectAttack(ref ConfigTbskill_effectNew skillEffect, bool isSeeked, Entity holder,
                int index, int effectID, Trigger trigger,
                ref DynamicBuffer<Skill> skills)
            {
                var caster = trigger.Entity_12;
                var locer = trigger.Entity_20;
                LocalTransform castPos = cdfeLocalTransform[locer];
                if (isSeeked)
                {
                    castPos = trigger.LocalTransform_25;
                    //temp.LocalTransform_25 = loc;
                    //temp.float3x2_24.c0 = targetPos;
                    //temp.float3x2_24.c1 = dir;
                }


                ref var skillEffectConfig =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                ref var elementConfig = ref globalConfigData.value.Value.configTbskill_effectElements
                    .configTbskill_effectElements;
                var rangeType = skillEffect.rangeType;
                FixedString128Bytes modelName = $"model_skilleffect_{rangeType}";
                var prefab = prefabMapData.prefabHashMap[modelName];
                var searchType = skillEffect.searchType;
                ref var searchPar = ref skillEffect.searchTypePara;
                ref var rangPar = ref skillEffect.rangeTypePara;
                var deviateType = skillEffect.deviateType;
                ref var deviateTypePar = ref skillEffect.deviateTypePara;
                ref var lockPar = ref skillEffect.targetLockOnPara;
                float lockInt = 0;
                for (int i = 0; i < lockPar.Length; i++)
                {
                    lockInt += math.pow(2, lockPar[i]);
                }

                var delayafter = skillEffect.delayType == 4 ? skillEffect.delayTypePara[0] / 1000f : 0;
                float3 dir = trigger.float3x2_24.c1;
                float3 targetPos = trigger.float3x2_24.c0;
                var time = GetBeTriggeredCount(trigger);
                Debug.Log($"time:{time}");
                Entity target = Entity.Null;
                float3 crashPos = float3.zero;
                if (!isSeeked)
                {
                    
                    Debug.Log("isnotseek");
                    castPos = BuffHelper.SeekTargets(ref searchPar, ref deviateTypePar, searchType, deviateType,
                        rangeType, ref rangPar,
                        allEntities, caster, locer, cdfeLocalTransform, cdfeTargetData, cdfeChaStats, (uint)lockInt,
                        out targetPos, out dir, out target, cdfeBuff, cdfeBulletTempTag, timeTick, time);
                    Debug.Log($"target33444{target.Index}");
                    LocalTransform targetTran = new LocalTransform
                    {
                        Position = targetPos,
                        Scale = 1,
                        Rotation = quaternion.identity
                    };
                    if (storageInfoFromEntity.Exists(target) && cdfeLocalTransform.HasComponent(target))
                    {
                        targetTran = cdfeLocalTransform[target];
                    }


                    if (BuffHelper.IsFloat3Equal(targetPos, float3.zero))
                    {
                        ref var skillConfig =
                            ref globalConfigData.value.Value.configTbskills.configTbskills;
                        for (int i = 0; i < skillConfig.Length; i++)
                        {
                            if (trigger.Int32_10 == skillConfig[i].id && skillConfig[i].type == 1)
                            {
                                targetPos = castPos.Position;
                                if (cdfeChaStats.HasComponent(caster))
                                {
                                    targetPos = castPos.Position + cdfeChaStats[caster].chaResource.direction *
                                        searchPar[0] * 0.5f / 1000f;
                                }

                                break;
                            }
                        }
                    }

                    for (int m = 0; m < skills.Length; m++)
                    {
                        if (skills[m].Int32_0 == trigger.Int32_10)
                        {
                            var skilltemp = skills[m];
                            skilltemp.LocalTransform_15 = targetTran;
                            skills[m] = skilltemp;
                            break;
                        }
                    }
                }
                else
                {
                    crashPos = trigger.LocalTransform_25.Position;
                }
                Debug.Log($"dir:{dir},targetPos:{targetPos}");
                ref var targetParlist = ref skillEffect.targetPara;
                float targetPar = 0;
                for (int j = 0; j < targetParlist.Length; j++)
                {
                    targetPar += math.pow(2, targetParlist[j]);
                }

                if (searchType == 3)
                {
                    //Debug.Log("searchType == 3");
                    int count = searchPar.Length >= 3 ? searchPar[2] : 1;
                    for (int i = 0; i < count; i++)
                    {
                        // if (gameTimeData.logicTime.gameTimeScale < math.EPSILON)
                        //     continue;
                        Debug.Log($"searchType == 3,第{i + 1}个");
                        castPos = castPos.RotateZ(-math.radians(searchPar[1]));
                        var ins = ecb.Instantiate(index, prefab);
                        var preTran = cdfeLocalTransform[prefab];
                        preTran.Position = castPos.Position;
                        preTran.Rotation = castPos.Rotation;
                        //矩形 宽高
                        //扇形 半径半径度数
                        //圆形 半径半径0
                        //环装扇形 最小半径最大半径度数
                        switch (rangeType)
                        {
                            case 0:
                                preTran.Scale = 5f;
                                preTran.Scale = 5f;
                                break;
                            case 1:
                                if (rangPar[0] == rangPar[1])
                                {
                                    Debug.Log("==");
                                    preTran.Scale *= rangPar[0] / 1000f;
                                }
                                else
                                {
                                    Debug.Log("!=");
                                    ecb.AddComponent(index, ins, new PostTransformMatrix
                                    {
                                        Value = float4x4.Scale(preTran.Scale * (rangPar[0] / 1000f),
                                            preTran.Scale * (rangPar[1] / 1000f), 1)
                                    });
                                    var collider = PhysicsHelper.CreateBoxColliderAndSetSize(
                                        cdfePhysicsCollider[prefab],
                                        new float3(rangPar[0] / 1000f, rangPar[1] / 1000f, 1));
                                    ecb.SetComponent(index, ins, collider);
                                }

                                break;
                            case 2:
                                preTran.Scale *= rangPar[1] / 1000f;

                                break;
                            case 3:
                                preTran.Scale *= rangPar[0] / 1000f;
                                break;
                            case 4:
                                preTran.Scale *= (rangPar[1] + rangPar[4] * time) / 1000f;
                                break;
                        }

                        //preTran.Scale *= 10;
                        if (isDebug)
                        {
                            Debug.Log(
                                $"SkillAttackData {caster},pretran:Position{preTran.Position},scale{preTran.Scale}");
                        }

                        ecb.SetComponent(index, ins, preTran);

                        #region SpecialEffects

                        UnityHelper.SpawnSpecialEffect(ref ecb, index, eT, cdfeLocalTransform, cdfePostTransformMatrix,
                            gameTimeData,
                            globalConfigData,
                            ref skillEffect.specialEffects,
                            prefabMapData, holder, false, trigger.Int32_10, preTran, targetPos, skills, time, target);

                        #endregion

                        //if(trigger.Int32_16==1)
                        //BuffHelper.AjustScale(rangType, ref rangPar, ins, ecb, index, cdfeLocalTransform);

                        // ecb.SetComponent<LocalTransform>(index, ins,
                        //     new LocalTransform { Position = loc.Position, Scale = cdfeLocalTransform[prefab].Scale });
                        // ecb.AddComponent(index, ins, new SkillAttackData
                        // {
                        //     data = new SkillAttack
                        //     {
                        //         CurrentTypeId = (SkillAttack.TypeId)effectID,
                        //         Int32_0 = effectID,
                        //         Entity_3 = caster,
                        //     }
                        // });
                        //GenerateAttackShape(rangeType, ins);

                        ///如果是含有buff6的相关effect 则生成一个跟随boss移动的effect
                        if (ReturnForceMoveEffect(ref skillEffect, ref elementConfig, out var hitType))
                        {
                            int crashTriggerID = -1, noCrashID = -1;
                            for (int j = 0; j < skillEffectConfig.Length; j++)
                            {
                                if (skillEffectConfig[j].id == trigger.Int32_0)
                                {
                                    if (skillEffectConfig[j].extraType == 1)
                                    {
                                        Debug.Log($"tirggerid:{trigger.Int32_0}");
                                        crashTriggerID = skillEffectConfig[j].extraTypePara[0];
                                        noCrashID = skillEffectConfig[j].extraTypePara[1];
                                        break;
                                    }
                                }
                            }

                            Debug.Log($"dirReturnForceMoveEffect:{dir}");
                            ecb.AddComponent(index, ins, new SkillAttackData
                            {
                                data = new SkillAttack_999
                                {
                                    id = effectID,
                                    duration = 0,
                                    caster = caster,
                                    skillID = skillEffect.skillId,
                                    targetPos = targetPos,
                                    crashDir = dir,
                                    args = new int4(crashTriggerID, noCrashID, 0, 0),
                                    isOnStayTrigger = false,
                                    isBullet = true,
                                    hp = 999,
                                    crashPos = crashPos
                                }.ToSkillAttack()
                            });
                            ecb.AddComponent(index, ins, new TargetData
                            {
                                tick = 0,
                                AttackWith = (uint)(targetPar)
                            });
                        }
                        //旋涡
                        else if (ReturnVortex(ref skillEffect, ref elementConfig))
                        {
                            var datas = skillAtackDatas;
                            for (int j = 0; j < skillAtackDatas.Length; j++)
                            {
                                if (skillAtackDatas[j].data.Int32_0 == effectID)
                                {
                                    var temp = skillAtackDatas[j];
                                    temp.data.Single_1 = 0;
                                    skillAtackDatas[j] = temp;
                                    break;
                                }
                            }


                            Debug.Log($"dirReturnForceMoveEffect:{dir}");
                            ecb.AddComponent(index, ins, new SkillAttackData
                            {
                                data = new SkillAttack_888
                                {
                                    id = effectID,
                                    duration = 0,
                                    caster = caster,
                                    skillID = skillEffect.skillId,
                                    targetPos = targetPos,
                                    crashDir = dir,
                                }.ToSkillAttack()
                            });
                            ecb.AddComponent(index, ins, new TargetData
                            {
                                tick = 0,
                                AttackWith = (uint)(targetPar)
                            });
                        }
                        else
                        {
                            int isTriggerID = 0;
                            for (int j = 0; j < skillEffectConfig.Length; j++)
                            {
                                if (skillEffectConfig[j].id == trigger.Int32_0)
                                {
                                    if (skillEffectConfig[j].extraType == 2)
                                    {
                                        Debug.Log($"需要命中后触发trigger:{skillEffectConfig[j].extraTypePara[0]}");
                                        isTriggerID = 1;
                                        break;
                                    }
                                }
                            }

                            Debug.Log($"bullet skill skillEffect {skillEffect.skillId}");
                            var data = new SkillAttackData
                            {
                                data = new SkillAttack_0
                                {
                                    id = effectID,
                                    duration = 0,
                                    caster = caster,
                                    skillID = skillEffect.skillId,
                                    targetPos = dir,
                                    tirggerCount = time,
                                    holder = holder,
                                    isBulletDamage = trigger.Boolean_28,
                                    triggerID = trigger.Int32_0,
                                    isWeaponAttack = trigger.Boolean_29,
                                    triggerId = isTriggerID
                                }.ToSkillAttack()
                            };
                            if (effectID == 51102502 || effectID == 51102602 || effectID == 51102702)
                            {
                                data.data.Entity_3 = default;
                            }

                            ;
                            ecb.AddComponent(index, ins, data);
                            ecb.AddComponent(index, ins, new TargetData
                            {
                                tick = 0,
                                AttackWith = (uint)(targetPar)
                            });
                            ecb.AddBuffer<Buff>(index, ins);
                            ecb.AddComponent(index, ins, new ElementData { id = skillEffect.elementId });
                        }
                    }
                }
                //生成随机的几个attack
                else if (deviateType == 1 && deviateTypePar.Length > 1 && deviateTypePar[1] > 1)
                {
                    int num = deviateTypePar[1];
                    for (int i = 0; i < num; i++)
                    {
                        // if (gameTimeData.logicTime.gameTimeScale < math.EPSILON)
                        //     continue;
                        Debug.Log($"deviateTypePar[1] > 1,第{i + 1}个");
                        var tempPos = BuffHelper.GetRandomPointInCircle(targetPos, deviateTypePar[0] / 1000f, 0,
                            (uint)(i + timeTick * 10));
                        var ins = ecb.Instantiate(index, prefab);
                        var preTran = cdfeLocalTransform[prefab];
                        preTran.Position = tempPos;
                        preTran.Rotation = castPos.Rotation;
                        //矩形 宽高
                        //扇形 半径半径度数
                        //圆形 半径半径0
                        //环装扇形 最小半径最大半径度数
                        switch (rangeType)
                        {
                            case 0:
                                preTran.Scale = 5f;
                                break;
                            case 1:
                                if (rangPar[0] == rangPar[1])
                                {
                                    Debug.Log("==");
                                    preTran.Scale *= rangPar[0] / 1000f;
                                }
                                else
                                {
                                    Debug.Log("!=");
                                    ecb.AddComponent(index, ins, new PostTransformMatrix
                                    {
                                        Value = float4x4.Scale(preTran.Scale * (rangPar[0] / 1000f),
                                            preTran.Scale * (rangPar[1] / 1000f), 1)
                                    });
                                    var collider = PhysicsHelper.CreateBoxColliderAndSetSize(
                                        cdfePhysicsCollider[prefab],
                                        new float3(rangPar[0] / 1000f, rangPar[1] / 1000f, 1));
                                    ecb.SetComponent(index, ins, collider);
                                }

                                break;
                            case 2:
                                preTran.Scale *= rangPar[1] / 1000f;

                                break;
                            case 3:
                                preTran.Scale *= rangPar[0] / 1000f;
                                break;
                            case 4:
                                preTran.Scale *= (rangPar[1] + rangPar[4] * time) / 1000f;
                                break;
                        }

                        //preTran.Scale *= 10;
                        if (isDebug)
                        {
                            Debug.Log(
                                $"SkillAttackData {caster},pretran:Position{preTran.Position},scale{preTran.Scale}");
                        }

                        Debug.Log(
                            $"pretran:Position{preTran.Position},scale{preTran.Scale}");
                        ecb.SetComponent(index, ins, preTran);

                        #region SpecialEffects

                        UnityHelper.SpawnSpecialEffect(ref ecb, index, eT, cdfeLocalTransform, cdfePostTransformMatrix,
                            gameTimeData,
                            globalConfigData,
                            ref skillEffect.specialEffects,
                            prefabMapData, holder, false, trigger.Int32_10, preTran, tempPos, skills, time, target);

                        #endregion

                        //if(trigger.Int32_16==1)
                        //BuffHelper.AjustScale(rangType, ref rangPar, ins, ecb, index, cdfeLocalTransform);

                        // ecb.SetComponent<LocalTransform>(index, ins,
                        //     new LocalTransform { Position = loc.Position, Scale = cdfeLocalTransform[prefab].Scale });
                        // ecb.AddComponent(index, ins, new SkillAttackData
                        // {
                        //     data = new SkillAttack
                        //     {
                        //         CurrentTypeId = (SkillAttack.TypeId)effectID,
                        //         Int32_0 = effectID,
                        //         Entity_3 = caster,
                        //     }
                        // });target
                        //GenerateAttackShape(rangeType, ins);

                        ///如果是含有buff6的相关effect 则生成一个跟随boss移动的effect
                        if (ReturnForceMoveEffect(ref skillEffect, ref elementConfig, out var hitType))
                        {
                            int crashTriggerID = -1, noCrashID = -1;
                            for (int j = 0; j < skillEffectConfig.Length; j++)
                            {
                                if (skillEffectConfig[j].id == trigger.Int32_0)
                                {
                                    Debug.Log($"tirggerid:{trigger.Int32_0}");
                                    if (skillEffectConfig[j].extraType == 1)
                                    {
                                        crashTriggerID = skillEffectConfig[j].extraTypePara[0];
                                        noCrashID = skillEffectConfig[j].extraTypePara[1];
                                        break;
                                    }
                                }
                            }

                            Debug.Log($"dirReturnForceMoveEffect:{dir}");
                            ecb.AddComponent(index, ins, new SkillAttackData
                            {
                                data = new SkillAttack_999
                                {
                                    id = effectID,
                                    duration = 0,
                                    caster = caster,
                                    skillID = skillEffect.skillId,
                                    targetPos = targetPos,
                                    crashDir = dir,
                                    isOnStayTrigger = false,
                                    isBullet = true,
                                    hp = 999,
                                    args = new int4(crashTriggerID, noCrashID, 0, 0),
                                    crashPos = crashPos
                                }.ToSkillAttack()
                            });
                            ecb.AddComponent(index, ins, new TargetData
                            {
                                tick = 0,
                                AttackWith = (uint)(targetPar)
                            });
                        }
                        //旋涡
                        else if (ReturnVortex(ref skillEffect, ref elementConfig))
                        {
                            var datas = skillAtackDatas;
                            for (int j = 0; j < skillAtackDatas.Length; j++)
                            {
                                if (skillAtackDatas[j].data.Int32_0 == effectID)
                                {
                                    var temp = skillAtackDatas[j];
                                    temp.data.Single_1 = 0;
                                    skillAtackDatas[j] = temp;
                                    break;
                                }
                            }

                            Debug.Log($"dirReturnForceMoveEffect:{dir}");
                            ecb.AddComponent(index, ins, new SkillAttackData
                            {
                                data = new SkillAttack_888
                                {
                                    id = effectID,
                                    duration = 0,
                                    caster = caster,
                                    skillID = skillEffect.skillId,
                                    targetPos = targetPos,
                                    crashDir = dir,
                                    holder = holder
                                }.ToSkillAttack()
                            });
                            ecb.AddComponent(index, ins, new TargetData
                            {
                                tick = 0,
                                AttackWith = (uint)(targetPar)
                            });
                        }
                        else
                        {
                            int isTriggerID = 0;
                            for (int j = 0; j < skillEffectConfig.Length; j++)
                            {
                                if (skillEffectConfig[j].id == trigger.Int32_0)
                                {
                                    if (skillEffectConfig[j].extraType == 2)
                                    {
                                        Debug.Log($"需要命中后触发trigger:{skillEffectConfig[j].extraTypePara[0]}");
                                        isTriggerID = 1;
                                        break;
                                    }
                                }
                            }


                            var data = new SkillAttackData
                            {
                                data = new SkillAttack_0
                                {
                                    id = effectID,
                                    duration = 0,
                                    caster = caster,
                                    skillID = skillEffect.skillId,
                                    targetPos = dir,
                                    tirggerCount = time,
                                    holder = holder,
                                    isBulletDamage = trigger.Boolean_28,
                                    triggerID = trigger.Int32_0,
                                    isWeaponAttack = trigger.Boolean_29,
                                    triggerId = isTriggerID
                                }.ToSkillAttack()
                            };
                            if (effectID == 51102502 || effectID == 51102602 || effectID == 51102702)
                            {
                                data.data.Entity_3 = default;
                            }

                            ecb.AddComponent(index, ins, data);
                            ecb.AddComponent(index, ins, new TargetData
                            {
                                tick = 0,
                                AttackWith = (uint)(targetPar)
                            });
                            ecb.AddBuffer<Buff>(index, ins);
                            ecb.AddComponent(index, ins, new ElementData { id = skillEffect.elementId });
                        }
                    }
                }
                else if (deviateType == 4 && deviateTypePar.Length > 1 && (searchType == 0 || searchType == 1))
                {
                    int num = deviateTypePar[1];
                    num = num > 1 ? num : 2;
                    num = num < 24 ? num : 24;
                    int Angle = 360 / num;
                    for (int i = 0; i < num; i++)
                    {
                        Debug.Log($"deviateTypePar43,第{i + 1}个 : {i * Angle}");

                        var tempPos = BuffHelper.GetPointInCircleByRadius(targetPos, deviateTypePar[0] / 1000f,
                            i * Angle);
                        Debug.Log($"deviateTypePar44 : {tempPos}");
                        var ins = ecb.Instantiate(index, prefab);
                        var preTran = cdfeLocalTransform[prefab];
                        preTran.Position = tempPos;
                        preTran.Rotation = castPos.Rotation;
                        //矩形 宽高
                        //扇形 半径半径度数
                        //圆形 半径半径0
                        //环装扇形 最小半径最大半径度数
                        switch (rangeType)
                        {
                            case 0:
                                preTran.Scale = 5f;
                                break;
                            case 1:
                                if (rangPar[0] == rangPar[1])
                                {
                                    Debug.Log("==");
                                    preTran.Scale *= rangPar[0] / 1000f;
                                }
                                else
                                {
                                    Debug.Log("!=");
                                    ecb.AddComponent(index, ins, new PostTransformMatrix
                                    {
                                        Value = float4x4.Scale(preTran.Scale * (rangPar[0] / 1000f),
                                            preTran.Scale * (rangPar[1] / 1000f), 1)
                                    });
                                    var collider = PhysicsHelper.CreateBoxColliderAndSetSize(
                                        cdfePhysicsCollider[prefab],
                                        new float3(rangPar[0] / 1000f, rangPar[1] / 1000f, 1));
                                    ecb.SetComponent(index, ins, collider);
                                }

                                break;
                            case 2:
                                preTran.Scale *= rangPar[1] / 1000f;

                                break;
                            case 3:
                                preTran.Scale *= rangPar[0] / 1000f;
                                break;
                            case 4:
                                preTran.Scale *= (rangPar[1] + rangPar[4] * time) / 1000f;
                                break;
                        }

                        //preTran.Scale *= 10;
                        if (isDebug)
                        {
                            Debug.Log(
                                $"SkillAttackData {caster},pretran:Position{preTran.Position},scale{preTran.Scale}");
                        }

                        Debug.Log(
                            $"pretran:Position{preTran.Position},scale{preTran.Scale}");
                        ecb.SetComponent(index, ins, preTran);

                        #region SpecialEffects

                        UnityHelper.SpawnSpecialEffect(ref ecb, index, eT, cdfeLocalTransform, cdfePostTransformMatrix,
                            gameTimeData,
                            globalConfigData,
                            ref skillEffect.specialEffects,
                            prefabMapData, holder, false, trigger.Int32_10, preTran, tempPos, skills, time, target);

                        #endregion

                        //if(trigger.Int32_16==1)
                        //BuffHelper.AjustScale(rangType, ref rangPar, ins, ecb, index, cdfeLocalTransform);

                        // ecb.SetComponent<LocalTransform>(index, ins,
                        //     new LocalTransform { Position = loc.Position, Scale = cdfeLocalTransform[prefab].Scale });
                        // ecb.AddComponent(index, ins, new SkillAttackData
                        // {
                        //     data = new SkillAttack
                        //     {
                        //         CurrentTypeId = (SkillAttack.TypeId)effectID,
                        //         Int32_0 = effectID,
                        //         Entity_3 = caster,
                        //     }
                        // });target
                        //GenerateAttackShape(rangeType, ins);

                        ///如果是含有buff6的相关effect 则生成一个跟随boss移动的effect
                        if (ReturnForceMoveEffect(ref skillEffect, ref elementConfig, out var hitType))
                        {
                            int crashTriggerID = -1, noCrashID = -1;
                            for (int j = 0; j < skillEffectConfig.Length; j++)
                            {
                                if (skillEffectConfig[j].id == trigger.Int32_0)
                                {
                                    Debug.Log($"tirggerid:{trigger.Int32_0}");
                                    if (skillEffectConfig[j].extraType == 1)
                                    {
                                        crashTriggerID = skillEffectConfig[j].extraTypePara[0];
                                        noCrashID = skillEffectConfig[j].extraTypePara[1];
                                        break;
                                    }
                                }
                            }

                            Debug.Log($"dirReturnForceMoveEffect:{dir}");
                            ecb.AddComponent(index, ins, new SkillAttackData
                            {
                                data = new SkillAttack_999
                                {
                                    id = effectID,
                                    duration = 0,
                                    caster = caster,
                                    skillID = skillEffect.skillId,
                                    targetPos = targetPos,
                                    crashDir = dir,
                                    isOnStayTrigger = false,
                                    isBullet = true,
                                    hp = 999,
                                    args = new int4(crashTriggerID, noCrashID, 0, 0),
                                    crashPos = crashPos
                                }.ToSkillAttack()
                            });
                            ecb.AddComponent(index, ins, new TargetData
                            {
                                tick = 0,
                                AttackWith = (uint)(targetPar)
                            });
                        }
                        //旋涡
                        else if (ReturnVortex(ref skillEffect, ref elementConfig))
                        {
                            var datas = skillAtackDatas;
                            for (int j = 0; j < skillAtackDatas.Length; j++)
                            {
                                if (skillAtackDatas[j].data.Int32_0 == effectID)
                                {
                                    var temp = skillAtackDatas[j];
                                    temp.data.Single_1 = 0;
                                    skillAtackDatas[j] = temp;
                                    break;
                                }
                            }

                            Debug.Log($"dirReturnForceMoveEffect:{dir}");
                            ecb.AddComponent(index, ins, new SkillAttackData
                            {
                                data = new SkillAttack_888
                                {
                                    id = effectID,
                                    duration = 0,
                                    caster = caster,
                                    skillID = skillEffect.skillId,
                                    targetPos = targetPos,
                                    crashDir = dir,
                                    holder = holder
                                }.ToSkillAttack()
                            });
                            ecb.AddComponent(index, ins, new TargetData
                            {
                                tick = 0,
                                AttackWith = (uint)(targetPar)
                            });
                        }
                        else
                        {
                            int isTriggerID = 0;
                            for (int j = 0; j < skillEffectConfig.Length; j++)
                            {
                                if (skillEffectConfig[j].id == trigger.Int32_0)
                                {
                                    if (skillEffectConfig[j].extraType == 2)
                                    {
                                        Debug.Log($"需要命中后触发trigger:{skillEffectConfig[j].extraTypePara[0]}");
                                        isTriggerID = 1;
                                        break;
                                    }
                                }
                            }


                            var data = new SkillAttackData
                            {
                                data = new SkillAttack_0
                                {
                                    id = effectID,
                                    duration = 0,
                                    caster = caster,
                                    skillID = skillEffect.skillId,
                                    targetPos = dir,
                                    tirggerCount = time,
                                    holder = holder,
                                    isBulletDamage = trigger.Boolean_28,
                                    triggerID = trigger.Int32_0,
                                    isWeaponAttack = trigger.Boolean_29,
                                    triggerId = isTriggerID
                                }.ToSkillAttack()
                            };
                            if (effectID == 51102502 || effectID == 51102602 || effectID == 51102702)
                            {
                                data.data.Entity_3 = default;
                            }

                            ecb.AddComponent(index, ins, data);
                            ecb.AddComponent(index, ins, new TargetData
                            {
                                tick = 0,
                                AttackWith = (uint)(targetPar)
                            });
                            ecb.AddBuffer<Buff>(index, ins);
                            ecb.AddComponent(index, ins, new ElementData { id = skillEffect.elementId });
                        }
                    }
                }
                else
                {
                    // if (gameTimeData.logicTime.gameTimeScale < math.EPSILON)
                    //     return;
                    var ins = ecb.Instantiate(index, prefab);
                    var preTran = cdfeLocalTransform[prefab];
                    preTran.Position = castPos.Position;
                    preTran.Rotation = castPos.Rotation;
                    //矩形 宽高
                    //扇形 半径半径度数
                    //圆形 半径半径0
                    //环装扇形 最小半径最大半径度数
                    switch (rangeType)
                    {
                        case 0:

                            preTran.Scale = 5f;

                            break;
                        case 1:

                            if (rangPar[0] == rangPar[1])
                            {
                                Debug.Log("==");
                                preTran.Scale *= rangPar[0] / 1000f;
                            }
                            else
                            {
                                Debug.Log("!=");
                                ecb.AddComponent(index, ins, new PostTransformMatrix
                                {
                                    Value = float4x4.Scale(preTran.Scale * (rangPar[0] / 1000f),
                                        preTran.Scale * (rangPar[1] / 1000f), 1)
                                });
                                var collider = PhysicsHelper.CreateBoxColliderAndSetSize(cdfePhysicsCollider[prefab],
                                    new float3(rangPar[0] / 1000f, rangPar[1] / 1000f, 1));
                                ecb.SetComponent(index, ins, collider);
                            }

                            break;
                        case 2:
                            preTran.Scale *= rangPar[1] / 1000f;

                            break;
                        case 3:
                            preTran.Scale *= rangPar[0] / 1000f;
                            break;
                        case 4:
                            preTran.Scale *= (rangPar[1] + rangPar[4] * time) / 1000f;
                            break;
                    }

                    //preTran.Scale *= 10;
                    if (isDebug)
                    {
                        Debug.Log($"SkillAttackData {ins},pretran:Position{preTran.Position},scale{preTran.Scale}");
                    }

                    Debug.Log(
                        $"pretran:Position{preTran.Position},scale{preTran.Scale}");
                    ecb.SetComponent(index, ins, preTran);

                    #region SpecialEffects

                    UnityHelper.SpawnSpecialEffect(ref ecb, index, eT, cdfeLocalTransform, cdfePostTransformMatrix,
                        gameTimeData,
                        globalConfigData,
                        ref skillEffect.specialEffects,
                        prefabMapData, holder, false, trigger.Int32_10, preTran, targetPos, skills, time, target);

                    #endregion

                    if (ReturnForceMoveEffect(ref skillEffect, ref elementConfig, out var hitType))
                    {
                        int crashTriggerID = -1, noCrashID = -1;
                        for (int j = 0; j < skillEffectConfig.Length; j++)
                        {
                            if (skillEffectConfig[j].id == trigger.Int32_0)
                            {
                                Debug.Log($"tirggerid:{trigger.Int32_0}");
                                if (skillEffectConfig[j].extraType == 1)
                                {
                                    crashTriggerID = skillEffectConfig[j].extraTypePara[0];
                                    noCrashID = skillEffectConfig[j].extraTypePara[1];
                                    break;
                                }
                            }
                        }

                        ecb.AddComponent(index, ins, new SkillAttackData
                        {
                            data = new SkillAttack_999
                            {
                                id = effectID,
                                duration = 0,
                                caster = caster,
                                skillID = skillEffect.skillId,
                                targetPos = targetPos,
                                crashDir = dir,
                                isOnStayTrigger = false,
                                isBullet = true,
                                hp = 999,
                                args = new int4(crashTriggerID, noCrashID, 0, hitType),
                                crashPos=crashPos
                            }.ToSkillAttack()
                        });
                        ecb.AddComponent(index, ins, new TargetData
                        {
                            tick = 0,
                            AttackWith = (uint)(targetPar)
                        });
                    }
                    //旋涡
                    else if (ReturnVortex(ref skillEffect, ref elementConfig))
                    {
                        var datas = skillAtackDatas;
                        for (int j = 0; j < skillAtackDatas.Length; j++)
                        {
                            if (skillAtackDatas[j].data.Int32_0 == effectID)
                            {
                                var temp = skillAtackDatas[j];
                                temp.data.Single_1 = 0;
                                skillAtackDatas[j] = temp;
                                break;
                            }
                        }

                        Debug.Log($"dirReturnForceMoveEffect:{dir}");
                        ecb.AddComponent(index, ins, new SkillAttackData
                        {
                            data = new SkillAttack_888
                            {
                                id = effectID,
                                duration = 0,
                                caster = caster,
                                skillID = skillEffect.skillId,
                                targetPos = targetPos,
                                crashDir = dir,
                                holder = holder
                            }.ToSkillAttack()
                        });
                        ecb.AddComponent(index, ins, new TargetData
                        {
                            tick = 0,
                            AttackWith = (uint)(targetPar)
                        });
                    }
                    else
                    {
                        int isTriggerID = 0;
                        for (int j = 0; j < skillEffectConfig.Length; j++)
                        {
                            if (skillEffectConfig[j].id == trigger.Int32_0)
                            {
                                if (skillEffectConfig[j].extraType == 2)
                                {
                                    Debug.Log($"需要命中后触发trigger:{skillEffectConfig[j].extraTypePara[0]}");
                                    isTriggerID = 1;
                                    break;
                                }
                            }
                        }

                        //Debug.Log($"bullet skill skillEffect {skillEffect.skillId}");
                        var data = new SkillAttackData
                        {
                            data = new SkillAttack_0
                            {
                                id = effectID,
                                duration = 0.1f,
                                caster = caster,
                                skillID = skillEffect.skillId,
                                targetPos = dir,
                                tirggerCount = time,
                                holder = locer,
                                isBulletDamage = trigger.Boolean_28,
                                triggerID = trigger.Int32_0,
                                isWeaponAttack = trigger.Boolean_29,
                                triggerId = isTriggerID,
                            }.ToSkillAttack()
                        };
                        if (effectID == 51102502 || effectID == 51102602 || effectID == 51102702)
                        {
                            data.data.Entity_3 = default;
                        }

                        ;
                        ecb.AddComponent(index, ins, data);
                        ecb.AddComponent(index, ins, new TargetData
                        {
                            tick = 0,
                            AttackWith = (uint)(targetPar)
                        });
                        ecb.AddBuffer<Buff>(index, ins);
                        ecb.AddComponent(index, ins, new ElementData { id = skillEffect.elementId });
                    }
                }
            }

            private int GetBeTriggeredCount(Trigger trigger)
            {
                int totalNumber = 0;
                ref var skilleffectConfig =
                    ref globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                for (int i = 0; i < skilleffectConfig.Length; i++)
                {
                    if (skilleffectConfig[i].id == trigger.Int32_0 && skilleffectConfig[i].triggerType == 4)
                    {
                        Debug.Log($"id:{trigger.Int32_0},次数:{trigger.float4_6.z}");
                        totalNumber = skilleffectConfig[i].triggerTypePara[1];
                        return totalNumber - (int)trigger.float4_6.z;
                    }
                }

                return 1;
            }

            private bool ReturnVortex(ref ConfigTbskill_effectNew skillEffect,
                ref BlobArray<ConfigTbskill_effectElement> elementConfig)
            {
                ref var elementList = ref skillEffect.elementList;
                for (int k = 0; k < elementList.Length; k++)
                {
                    for (int m = 0; m < elementConfig.Length; m++)
                    {
                        if (elementList[k] == elementConfig[m].id && elementConfig[m].elementType == 6 &&
                            elementConfig[m].displaceFrom == 2)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            private bool ReturnForceMoveEffect(ref ConfigTbskill_effectNew skillEffect,
                ref BlobArray<ConfigTbskill_effectElement> elementConfig, out int hitType)
            {
                hitType = 0;
                ref var elementList = ref skillEffect.elementList;
                for (int k = 0; k < elementList.Length; k++)
                {
                    for (int m = 0; m < elementConfig.Length; m++)
                    {
                        if (elementList[k] == elementConfig[m].id && elementConfig[m].elementType == 6 &&
                            elementConfig[m].displaceFrom != 2)
                        {
                            hitType = elementConfig[m].hitType;
                            return true;
                        }
                    }
                }

                return false;
            }

            private void AddBuffToPlayer(ref DynamicBuffer<Buff> buffs, ref BlobArray<int> elements,
                ref BlobArray<int2> states, Entity caster,
                int sortkey, ref ConfigTbskill_effectNew skilleffect, ref DynamicBuffer<Skill> skills, Trigger trigger)
            {
                if (skilleffect.conditionType != 0)
                {
                    if (skilleffect.conditionType == 2)
                    {
                        float hp = cdfeChaStats[player].chaResource.hp /
                            (cdfeChaStats[player].chaProperty.maxHp * 1f) * 10000f;
                        float compareRations = skilleffect.conditionTypePara[0];
                        int compareType = skilleffect.conditionTypePara[1];
                        if (isDebug)
                        {
                            Debug.Log($"hp:{hp},comp{compareRations},compartType:{compareType}");
                        }

                        if (hp >= compareRations && compareType != 2)
                        {
                            return;
                        }
                        else if (hp <= compareRations && compareType != 1)
                        {
                            return;
                        }
                        else if (hp == compareRations && compareType != 0)
                        {
                            return;
                        }
                    }
                    //状态是否相等
                    else if (skilleffect.conditionType == 3)
                    {
                        //状态存在
                        bool isActive = false;
                        foreach (var buff in buffs)
                        {
                            if (buff.CurrentTypeId == (Buff.TypeId)skilleffect.conditionTypePara[0])
                            {
                                isActive = true;
                                break;
                            }
                        }

                        if (!isActive) return;
                    }
                    else
                    {
                        //状态不存在
                        bool isHave = false;
                        foreach (var buff in buffs)
                        {
                            if (buff.CurrentTypeId == (Buff.TypeId)skilleffect.conditionTypePara[0])
                            {
                                isHave = true;
                            }
                        }

                        if (isHave) return;
                    }
                }


                if (isDebug)
                {
                    Debug.Log("AddBuffToCaster");
                }

                var effectID = skilleffect.id;
                bool isNeedMultip = false;
                //在技能上记录skillEffect次数
                if (trigger.TriggerType_5 == TriggerType.PerNum &&
                    trigger.TriggerConditionType_7 == TriggerConditionType.KillEnemy)
                {
                    isNeedMultip = true;
                }

                if (isNeedMultip)
                {
                    for (int i = 0; i < skills.Length; i++)
                    {
                        var temp = skills[i];
                        if (temp.Int32_0 == trigger.Int32_10)
                        {
                            //Debug.Log($"OnStart");

                            if (temp.int2x4_13.c0.x == effectID)
                            {
                                temp.int2x4_13.c0.y += trigger.Int32_23;
                            }
                            else if (temp.int2x4_13.c1.x == effectID)
                            {
                                temp.int2x4_13.c1.y += trigger.Int32_23;
                            }
                            else if (temp.int2x4_13.c2.x == effectID)
                            {
                                temp.int2x4_13.c2.y += trigger.Int32_23;
                            }
                            else if (temp.int2x4_13.c3.x == effectID)
                            {
                                temp.int2x4_13.c3.y += trigger.Int32_23;
                            }
                            else
                            {
                                if (temp.int2x4_13.c0.x == 0)
                                {
                                    temp.int2x4_13.c0.x = effectID;
                                    temp.int2x4_13.c0.y = trigger.Int32_23;
                                }
                                else if (temp.int2x4_13.c1.x == 0)
                                {
                                    temp.int2x4_13.c1.x = effectID;
                                    temp.int2x4_13.c1.y = trigger.Int32_23;
                                }
                                else if (temp.int2x4_13.c2.x == 0)
                                {
                                    temp.int2x4_13.c2.x = effectID;
                                    temp.int2x4_13.c2.y = trigger.Int32_23;
                                }
                                else if (temp.int2x4_13.c3.x == 0)
                                {
                                    temp.int2x4_13.c3.x = effectID;
                                    temp.int2x4_13.c3.y = trigger.Int32_23;
                                }
                            }

                            skills[i] = temp;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < skills.Length; i++)
                    {
                        var temp = skills[i];
                        if (temp.Int32_0 == trigger.Int32_10)
                        {
                            //Debug.Log($"OnStart");

                            if (temp.int2x4_13.c0.x == effectID)
                            {
                                temp.int2x4_13.c0.y++;
                            }
                            else if (temp.int2x4_13.c1.x == effectID)
                            {
                                temp.int2x4_13.c1.y++;
                            }
                            else if (temp.int2x4_13.c2.x == effectID)
                            {
                                temp.int2x4_13.c2.y++;
                            }
                            else if (temp.int2x4_13.c3.x == effectID)
                            {
                                temp.int2x4_13.c3.y++;
                            }
                            else
                            {
                                if (temp.int2x4_13.c0.x == 0)
                                {
                                    temp.int2x4_13.c0.x = effectID;
                                    temp.int2x4_13.c0.y = 1;
                                }
                                else if (temp.int2x4_13.c1.x == 0)
                                {
                                    temp.int2x4_13.c1.x = effectID;
                                    temp.int2x4_13.c1.y = 1;
                                }
                                else if (temp.int2x4_13.c2.x == 0)
                                {
                                    temp.int2x4_13.c2.x = effectID;
                                    temp.int2x4_13.c2.y = 1;
                                }
                                else if (temp.int2x4_13.c3.x == 0)
                                {
                                    temp.int2x4_13.c3.x = effectID;
                                    temp.int2x4_13.c3.y = 1;
                                }
                            }

                            skills[i] = temp;
                            break;
                        }
                    }
                }

                ref var elementsTable =
                    ref globalConfigData.value.Value.configTbskill_effectElements.configTbskill_effectElements;
                ref var statesConfig =
                    ref globalConfigData.value.Value.configTbbattle_statuss.configTbbattle_statuss;
                var target = caster;
                //caster = Entity.Null;
                for (int i = 0; i < states.Length; i++)
                {
                    for (int j = 0; j < statesConfig.Length; j++)
                    {
                        var stateId = states[i].x;
                        if (statesConfig[j].id == stateId)
                        {
                            bool isContains = false;
                            foreach (var buff in buffs)
                            {
                                if (buff.Int32_0 == stateId)
                                {
                                    isContains = true;
                                    break;
                                }
                            }

                            if (isContains) continue;
                            var duration = states[i].y / 1000f;
                            ecb.AppendToBuffer<Buff>(sortkey, target,
                                new Buff
                                {
                                    CurrentTypeId = (Buff.TypeId)stateId,
                                    Int32_0 = stateId,
                                    Single_3 = duration,
                                    Entity_5 = caster,
                                    Entity_6 = target,
                                    Int32_20 = trigger.Int32_10 == 511006 ? effectID : trigger.Int32_10
                                });
                        }
                    }
                }


                //怪物死亡后三秒爆炸的技能不给怪物自己挂伤害和推力
                if (trigger.Int32_10 == 511006) return;
                for (int i = 0; i < elements.Length; i++)
                {
                    //元素表！！！！
                    for (int j = 0; j < elementsTable.Length; j++)
                    {
                        if (elementsTable[j].id == elements[i])
                        {
                            switch (elementsTable[j].elementType)
                            {
                                //输出元素
                                case 1:
                                    // Debug.Log(
                                    //     $"target:{target.Index},elements{elementsTable[j].id}:{elementsTable[j].outputType},{elementsTable[j].bonusType},{elementsTable[j].calcType}");
                                    ecb.AppendToBuffer<Buff>(sortkey, target, new Buff
                                    {
                                        CurrentTypeId = (Buff.TypeId)elementsTable[j].elementType,
                                        Int32_0 = elementsTable[j].id,
                                        Single_1 = 0,
                                        Int32_2 = 0,
                                        Single_3 = 0,
                                        Boolean_4 = false,
                                        Entity_5 = caster,
                                        Entity_6 = target,
                                        int2_7 = 0,
                                        Int32_8 = 0,
                                        Int32_9 = 0,
                                        float3_10 = default,
                                        Int32_11 = elementsTable[j].stateType,
                                        BuffArgsNew_12 = new BuffArgsNew
                                        {
                                            args1 = new int4(elementsTable[j].outputType,
                                                elementsTable[j].outputTypePara[0], 0, 0),
                                            args2 = new int4(elementsTable[j].bonusType, 0, 0, 0),
                                            args3 = new int4(elementsTable[j].calcType,
                                                isNeedMultip
                                                    ? elementsTable[j].calcTypePara[0] * trigger.Int32_23
                                                    : elementsTable[j].calcTypePara[0],
                                                elementsTable[j].calcTypePara.Length > 1
                                                    ? elementsTable[j].calcTypePara[1]
                                                    : 0, 0),
                                            args4 = new int4(0, 0, 0, 0)
                                        },
                                        Int32_14 = elementsTable[j].power,
                                        Int32_20 = trigger.Int32_10
                                    });
                                    break;
                                //更改属性
                                case 2:

                                    ecb.AppendToBuffer<Buff>(sortkey, target, new Buff
                                    {
                                        CurrentTypeId = (Buff.TypeId)elementsTable[j]
                                            .elementType,
                                        Int32_0 = elementsTable[j]
                                            .id,
                                        Single_1 = 0,
                                        Int32_2 = 0,
                                        Single_3 = elementsTable[j].calcTypePara.Length > 0
                                            ? elementsTable[j].calcTypePara[0] / 1000f
                                            : 0,
                                        Boolean_4 = elementsTable[j]
                                            .calcType == 0
                                            ? true
                                            : false,
                                        Entity_5 = caster,
                                        Entity_6 = target,
                                        int2_7 = new int2(elementsTable[j].changeType,
                                            elementsTable[j].changeTypePara.Length > 0
                                                ? elementsTable[j].changeTypePara[0]
                                                : 0),
                                        Int32_8 = 0,
                                        Int32_9 = 0,
                                        float3_10 = default,
                                        Int32_11 = elementsTable[j]
                                            .stateType,
                                        BuffArgsNew_12 = new BuffArgsNew
                                        {
                                            args1 = new int4(elementsTable[j].attrId,
                                                isNeedMultip
                                                    ? elementsTable[j].attrIdPara[0] * trigger.Int32_23
                                                    : elementsTable[j].attrIdPara[0],
                                                0,
                                                0),
                                            args2 = new int4(0,
                                                0,
                                                0,
                                                0),
                                            args3 = new int4(0,
                                                0,
                                                0,
                                                0),
                                            args4 = new int4(0,
                                                0,
                                                0,
                                                0)
                                        },
                                        Int32_14 = elementsTable[j].power,
                                        Int32_20 = trigger.Int32_10
                                    });
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    //控制类
                                    var buff4 = new Buff
                                    {
                                        CurrentTypeId = (Buff.TypeId)elementsTable[j].elementType,
                                        Int32_0 = elementsTable[j].id,
                                        Single_1 = 0,
                                        Int32_2 = 0,
                                        Single_3 = isNeedMultip
                                            ? elementsTable[j].controlTypePara[0] / 1000f * trigger.Int32_23
                                            : elementsTable[j].controlTypePara[0] / 1000f,
                                        Boolean_4 = false,
                                        Entity_5 = caster,
                                        Entity_6 = target,
                                        int2_7 = new int2(elementsTable[j].changeType, 0),
                                        Int32_8 = elementsTable[j].clearType,
                                        Int32_9 = 0,
                                        float3_10 = default,
                                        Int32_11 = elementsTable[j].stateType,
                                        BuffArgsNew_12 = new BuffArgsNew
                                        {
                                            args1 = new int4(elementsTable[j].controlType, 0, 0,
                                                0),
                                            args2 = new int4(elementsTable[j].controlTypePara[0],
                                                elementsTable[j].controlTypePara.Length > 1
                                                    ? elementsTable[j].controlTypePara[1]
                                                    : 0, 0, 0),
                                            args3 = new int4(0, 0, 0, 0),
                                            args4 = new int4(0, 0, 0, 0)
                                        },
                                        Int32_13 = elementsTable[j].clearType * 2000,
                                        Int32_14 = elementsTable[j].power,
                                        Int32_20 = trigger.Int32_10
                                    };
                                    if (BuffHelper.TryAddBuffNew(ref buffs, buff4))
                                    {
                                        ecb.AppendToBuffer(sortkey, target, buff4);
                                    }


                                    break;
                                case 5:
                                    //TODO:免疫
                                    var immuneDuration = elementsTable[j].calcType == 1
                                        ? elementsTable[j].calcTypePara[0] / 1000f
                                        : 0;
                                    var buff5 = new Buff
                                    {
                                        CurrentTypeId = (Buff.TypeId)elementsTable[j].elementType,
                                        Int32_0 = elementsTable[j].id,
                                        Single_1 = 0,
                                        Int32_2 = 0,
                                        Single_3 = isNeedMultip ? immuneDuration * trigger.Int32_23 : immuneDuration,
                                        Boolean_4 = elementsTable[j].calcType == 0 ? true : false,
                                        Entity_5 = caster,
                                        Entity_6 = target,
                                        int2_7 = new int2(elementsTable[j].changeType, 0),
                                        Int32_8 = elementsTable[j].clearType,
                                        Int32_9 =
                                            elementsTable[j].calcType == 2 ? elementsTable[j].calcTypePara[0] : -1,
                                        float3_10 = default,
                                        Int32_11 = elementsTable[j].stateType,
                                        BuffArgsNew_12 = new BuffArgsNew
                                        {
                                            args1 = new int4(elementsTable[j].immuneType, 0, 0,
                                                0),
                                            args2 = new int4(0, 0, 0, 0),
                                            args3 = new int4(0, 0, 0, 0),
                                            args4 = new int4(0, 0, 0, 0)
                                        },
                                        Int32_13 = elementsTable[j].immuneType == 4
                                            ? elementsTable[j].clearType * 2000
                                            : 0,
                                        Int32_14 = elementsTable[j].power,
                                        Int32_20 = trigger.Int32_10
                                    };

                                    if (BuffHelper.TryAddBuffNew(ref buffs, buff5))
                                    {
                                        ecb.AppendToBuffer(sortkey, target, buff5);
                                    }


                                    break;
                                case 6:
                                    ecb.AppendToBuffer(sortkey, target, new Buff
                                    {
                                        CurrentTypeId = (Buff.TypeId)elementsTable[j].elementType,
                                        Int32_0 = elementsTable[j].id,
                                        Single_1 = 0,
                                        Int32_2 = 0,
                                        Single_3 = 0.02f,
                                        Boolean_4 = false,
                                        Entity_5 = caster,
                                        Entity_6 = target,
                                        int2_7 = new int2(elementsTable[j].changeType, 0),
                                        Int32_8 = elementsTable[j].clearType,
                                        Int32_9 = 0,
                                        float3_10 = default,
                                        Int32_11 = elementsTable[j].stateType,
                                        BuffArgsNew_12 = new BuffArgsNew
                                        {
                                            args1 = new int4(elementsTable[j].displaceFrom, 0, 0, 0),
                                            args2 = new int4(elementsTable[j].pointType, 0, 0, 0),
                                            args3 = new int4(elementsTable[j].pointTypePara[0],
                                                elementsTable[j].pointTypePara[1], elementsTable[j].pointTypePara[2],
                                                0),
                                            args4 = new int4(elementsTable[j].passType, 0, 0, 0)
                                        },
                                        Int32_14 = elementsTable[j].power,
                                        Int32_20 = trigger.Int32_10
                                    });
                                    break;
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}
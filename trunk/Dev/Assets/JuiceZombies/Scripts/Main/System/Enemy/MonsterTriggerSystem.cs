using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct MonsterTriggerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false;
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
            //var surfaceEntity = SystemAPI.GetSingletonEntity<CrowdSurface>();
            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameCameraSizeData = SystemAPI.GetComponent<GameCameraSizeData>(wbe);
            var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);

            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();


            new MonsterTriggerJob
            {
                prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe),
                gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe),
                globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe),
                gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe),
                enviromentData = SystemAPI.GetComponent<GameEnviromentData>(wbe),
                gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe),
                ecb = ecb,
                fdT = SystemAPI.Time.fixedDeltaTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct MonsterTriggerJob : IJobEntity
        {
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GameSetUpData gameSetUpData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public GameCameraSizeData gameCameraSizeData;
            [ReadOnly] public GameOthersData gameOthersData;
            [ReadOnly] public GameEnviromentData enviromentData;
            public GameRandomData gameRandomData;
            public float fdT;
            public EntityCommandBuffer.ParallelWriter ecb;

            public void Execute(Entity e, [EntityIndexInChunk] int entityIndexInChunk, ref ChaStats chaStats,
                in EnemyData enemyData)
            {
                if (chaStats.chaResource.env.weather == enviromentData.env.weather &&
                    chaStats.chaResource.env.time == enviromentData.env.time)
                {
                    return;
                }

                // chaStats.chaResource.env.weather = enviromentData.env.weather;
                // chaStats.chaResource.env.time = enviromentData.env.time;
                //
                // ResetChaProperty(ref chaStats);
                // chaStats.enviromentProperty = default;

                // ref var monster_trigger =
                //     ref globalConfigData.value.Value.configTbmonster_triggers.configTbmonster_triggers;
                // ref var eventConfig =
                //     ref globalConfigData.value.Value.configTbevent_0s.configTbevent_0s;
                // foreach (var feature in enemyData.enemyFeature)
                // {
                //     for (int i = 0; i < monster_trigger.Length; i++)
                //     {
                //         if (monster_trigger[i].id == feature)
                //         {
                //             bool isTrigger = false;
                //             ref var triggerType = ref monster_trigger[i].triggerType;
                //
                //             for (int j = 0; j < triggerType.Length; j++)
                //             {
                //                 var configType = triggerType[j].x;
                //                 var configValue = triggerType[j].y;
                //                 switch (configType)
                //                 {
                //                     case 1:
                //
                //                         break;
                //                     case 2:
                //
                //                         if (enviromentData.env.weather == configValue ||
                //                             enviromentData.env.time == configValue)
                //                         {
                //                             isTrigger = true;
                //                         }
                //
                //                         break;
                //                     case 3:
                //                         break;
                //                 }
                //             }
                //
                //             if (isTrigger)
                //             {
                //                 int eventId = monster_trigger[i].eventId;
                //                 int thousand = (int)(eventId / 1000f);
                //                 int enventIndex = default;
                //                 for (int j = 0; j < eventConfig.Length; j++)
                //                 {
                //                     if (eventConfig[j].id == eventId)
                //                     {
                //                         enventIndex = j;
                //                         break;
                //                     }
                //                 }
                //
                //                 ref var event0 = ref eventConfig[enventIndex];
                //
                //                 switch (event0.type)
                //                 {
                //                     //属性变更事件
                //                     case 7:
                //
                //                         int propertyId = event0.para1;
                //                         int propertyValue = event0.para2;
                //                         //ecb.AppendToBuffer(entityIndexInChunk,);
                //                         var playerData = new PlayerData { };
                //                         UnityHelper.ChangeProperty(ref chaStats, ref playerData, propertyId,
                //                             propertyValue);
                //
                //
                //                         break;
                //                 }
                //             }
                //         }
                //     }
                // }
                //
                // AddChaProperty(ref chaStats);
            }


            public void ResetChaProperty(ref ChaStats chaStats)
            {
                var newProperty = new ChaProperty
                {
                    maxHp = chaStats.chaProperty.maxHp - chaStats.enviromentProperty.maxHp,
                    defaultMaxHp = chaStats.chaProperty.defaultMaxHp - chaStats.enviromentProperty.defaultMaxHp,
                    hpRatios = chaStats.chaProperty.hpRatios - chaStats.enviromentProperty.hpRatios,
                    hpAdd = chaStats.chaProperty.hpAdd - chaStats.enviromentProperty.hpAdd,
                    curHpRatios = chaStats.chaProperty.curHpRatios - chaStats.enviromentProperty.curHpRatios,
                    hpRecovery = chaStats.chaProperty.hpRecovery - chaStats.enviromentProperty.hpRecovery,
                    defaultHpRecovery = chaStats.chaProperty.defaultHpRecovery -
                                        chaStats.enviromentProperty.defaultHpRecovery,
                    hpRecoveryRatios = chaStats.chaProperty.hpRecoveryRatios -
                                       chaStats.enviromentProperty.hpRecoveryRatios,
                    hpRecoveryAdd = chaStats.chaProperty.hpRecoveryAdd - chaStats.enviromentProperty.hpRecoveryAdd,
                    atk = chaStats.chaProperty.atk - chaStats.enviromentProperty.atk,
                    defaultAtk = chaStats.chaProperty.defaultAtk - chaStats.enviromentProperty.defaultAtk,
                    atkRatios = chaStats.chaProperty.atkRatios - chaStats.enviromentProperty.atkRatios,
                    atkAdd = chaStats.chaProperty.atkAdd - chaStats.enviromentProperty.atkAdd,
                    rebirthCount = chaStats.chaProperty.rebirthCount - chaStats.enviromentProperty.rebirthCount,
                    rebirthCount1 = chaStats.chaProperty.rebirthCount1 - chaStats.enviromentProperty.rebirthCount1,
                    critical = chaStats.chaProperty.critical - chaStats.enviromentProperty.critical,
                    tmpCritical = chaStats.chaProperty.tmpCritical - chaStats.enviromentProperty.tmpCritical,
                    criticalDamageRatios = chaStats.chaProperty.criticalDamageRatios -
                                           chaStats.enviromentProperty.criticalDamageRatios,
                    damageRatios = chaStats.chaProperty.damageRatios - chaStats.enviromentProperty.damageRatios,
                    damageAdd = chaStats.chaProperty.damageAdd - chaStats.enviromentProperty.damageAdd,
                    reduceHurtRatios = chaStats.chaProperty.reduceHurtRatios -
                                       chaStats.enviromentProperty.reduceHurtRatios,
                    reduceHurtAdd = chaStats.chaProperty.reduceHurtAdd - chaStats.enviromentProperty.reduceHurtAdd,
                    reduceBulletRatios = chaStats.chaProperty.reduceBulletRatios -
                                         chaStats.enviromentProperty.reduceBulletRatios,
                    changedFromPlayerDamage = chaStats.chaProperty.changedFromPlayerDamage -
                                              chaStats.enviromentProperty.changedFromPlayerDamage,
                    maxMoveSpeed = chaStats.chaProperty.maxMoveSpeed - chaStats.enviromentProperty.maxMoveSpeed,
                    defaultMaxMoveSpeed = chaStats.chaProperty.defaultMaxMoveSpeed -
                                          chaStats.enviromentProperty.defaultMaxMoveSpeed,
                    maxMoveSpeedRatios = chaStats.chaProperty.maxMoveSpeedRatios -
                                         chaStats.enviromentProperty.maxMoveSpeedRatios,
                    maxMoveSpeedAdd =
                        chaStats.chaProperty.maxMoveSpeedAdd - chaStats.enviromentProperty.maxMoveSpeedAdd,
                    speedRecoveryTime = chaStats.chaProperty.speedRecoveryTime -
                                        chaStats.enviromentProperty.speedRecoveryTime,
                    mass = chaStats.chaProperty.mass - chaStats.enviromentProperty.mass,
                    defaultMass = chaStats.chaProperty.defaultMass - chaStats.enviromentProperty.defaultMass,
                    massRatios = chaStats.chaProperty.massRatios - chaStats.enviromentProperty.massRatios,
                    pushForce = chaStats.chaProperty.pushForce - chaStats.enviromentProperty.pushForce,
                    defaultPushForce = chaStats.chaProperty.defaultPushForce -
                                       chaStats.enviromentProperty.defaultPushForce,
                    pushForceRatios =
                        chaStats.chaProperty.pushForceRatios - chaStats.enviromentProperty.pushForceRatios,
                    pushForceAdd = chaStats.chaProperty.pushForceAdd - chaStats.enviromentProperty.pushForceAdd,
                    reduceHitBackRatios = chaStats.chaProperty.reduceHitBackRatios -
                                          chaStats.enviromentProperty.reduceHitBackRatios,
                    collideDamageRatios = chaStats.chaProperty.collideDamageRatios -
                                          chaStats.enviromentProperty.collideDamageRatios,
                    continuousCollideDamageRatios = chaStats.chaProperty.continuousCollideDamageRatios -
                                                    chaStats.enviromentProperty.continuousCollideDamageRatios,
                    // superPushForceChance = chaStats.chaProperty.superPushForceChance -
                    //                        chaStats.enviromentProperty.superPushForceChance,
                    // maxPushForceChance = chaStats.chaProperty.maxPushForceChance -
                    //                      chaStats.enviromentProperty.maxPushForceChance,
                    scaleRatios = chaStats.chaProperty.scaleRatios - chaStats.enviromentProperty.scaleRatios,
                    // continuousCollDamagePlus = chaStats.chaProperty.continuousCollDamagePlus -
                    //                            chaStats.enviromentProperty.continuousCollDamagePlus,
                    dodge = chaStats.chaProperty.dodge - chaStats.enviromentProperty.dodge,
                    shieldCount = chaStats.chaProperty.shieldCount -
                                  chaStats.enviromentProperty.shieldCount,
                    defaultcoolDown =
                        chaStats.chaProperty.defaultcoolDown - chaStats.enviromentProperty.defaultcoolDown,
                    defaultBulletRangeRatios = chaStats.chaProperty.defaultBulletRangeRatios -
                                               chaStats.enviromentProperty.defaultBulletRangeRatios
                };
                chaStats.chaProperty = newProperty;
            }

            public void AddChaProperty(ref ChaStats chaStats)
            {
                var newProperty = new ChaProperty
                {
                    maxHp = chaStats.chaProperty.maxHp + chaStats.enviromentProperty.maxHp,
                    defaultMaxHp = chaStats.chaProperty.defaultMaxHp + chaStats.enviromentProperty.defaultMaxHp,
                    hpRatios = chaStats.chaProperty.hpRatios + chaStats.enviromentProperty.hpRatios,
                    hpAdd = chaStats.chaProperty.hpAdd + chaStats.enviromentProperty.hpAdd,
                    curHpRatios = chaStats.chaProperty.curHpRatios + chaStats.enviromentProperty.curHpRatios,
                    hpRecovery = chaStats.chaProperty.hpRecovery + chaStats.enviromentProperty.hpRecovery,
                    defaultHpRecovery = chaStats.chaProperty.defaultHpRecovery +
                                        chaStats.enviromentProperty.defaultHpRecovery,
                    hpRecoveryRatios = chaStats.chaProperty.hpRecoveryRatios +
                                       chaStats.enviromentProperty.hpRecoveryRatios,
                    hpRecoveryAdd = chaStats.chaProperty.hpRecoveryAdd + chaStats.enviromentProperty.hpRecoveryAdd,
                    atk = chaStats.chaProperty.atk + chaStats.enviromentProperty.atk,
                    defaultAtk = chaStats.chaProperty.defaultAtk + chaStats.enviromentProperty.defaultAtk,
                    atkRatios = chaStats.chaProperty.atkRatios + chaStats.enviromentProperty.atkRatios,
                    atkAdd = chaStats.chaProperty.atkAdd + chaStats.enviromentProperty.atkAdd,
                    rebirthCount = chaStats.chaProperty.rebirthCount + chaStats.enviromentProperty.rebirthCount,
                    rebirthCount1 = chaStats.chaProperty.rebirthCount1 + chaStats.enviromentProperty.rebirthCount1,
                    critical = chaStats.chaProperty.critical + chaStats.enviromentProperty.critical,
                    tmpCritical = chaStats.chaProperty.tmpCritical + chaStats.enviromentProperty.tmpCritical,
                    criticalDamageRatios = chaStats.chaProperty.criticalDamageRatios +
                                           chaStats.enviromentProperty.criticalDamageRatios,
                    damageRatios = chaStats.chaProperty.damageRatios + chaStats.enviromentProperty.damageRatios,
                    damageAdd = chaStats.chaProperty.damageAdd + chaStats.enviromentProperty.damageAdd,
                    reduceHurtRatios = chaStats.chaProperty.reduceHurtRatios +
                                       chaStats.enviromentProperty.reduceHurtRatios,
                    reduceHurtAdd = chaStats.chaProperty.reduceHurtAdd + chaStats.enviromentProperty.reduceHurtAdd,
                    reduceBulletRatios = chaStats.chaProperty.reduceBulletRatios +
                                         chaStats.enviromentProperty.reduceBulletRatios,
                    changedFromPlayerDamage = chaStats.chaProperty.changedFromPlayerDamage +
                                              chaStats.enviromentProperty.changedFromPlayerDamage,
                    maxMoveSpeed = chaStats.chaProperty.maxMoveSpeed + chaStats.enviromentProperty.maxMoveSpeed,
                    defaultMaxMoveSpeed = chaStats.chaProperty.defaultMaxMoveSpeed +
                                          chaStats.enviromentProperty.defaultMaxMoveSpeed,
                    maxMoveSpeedRatios = chaStats.chaProperty.maxMoveSpeedRatios +
                                         chaStats.enviromentProperty.maxMoveSpeedRatios,
                    maxMoveSpeedAdd =
                        chaStats.chaProperty.maxMoveSpeedAdd + chaStats.enviromentProperty.maxMoveSpeedAdd,
                    speedRecoveryTime = chaStats.chaProperty.speedRecoveryTime +
                                        chaStats.enviromentProperty.speedRecoveryTime,
                    mass = chaStats.chaProperty.mass + chaStats.enviromentProperty.mass,
                    defaultMass = chaStats.chaProperty.defaultMass + chaStats.enviromentProperty.defaultMass,
                    massRatios = chaStats.chaProperty.massRatios + chaStats.enviromentProperty.massRatios,
                    pushForce = chaStats.chaProperty.pushForce + chaStats.enviromentProperty.pushForce,
                    defaultPushForce = chaStats.chaProperty.defaultPushForce +
                                       chaStats.enviromentProperty.defaultPushForce,
                    pushForceRatios =
                        chaStats.chaProperty.pushForceRatios + chaStats.enviromentProperty.pushForceRatios,
                    pushForceAdd = chaStats.chaProperty.pushForceAdd + chaStats.enviromentProperty.pushForceAdd,
                    reduceHitBackRatios = chaStats.chaProperty.reduceHitBackRatios +
                                          chaStats.enviromentProperty.reduceHitBackRatios,
                    collideDamageRatios = chaStats.chaProperty.collideDamageRatios +
                                          chaStats.enviromentProperty.collideDamageRatios,
                    continuousCollideDamageRatios = chaStats.chaProperty.continuousCollideDamageRatios +
                                                    chaStats.enviromentProperty.continuousCollideDamageRatios,
                    // superPushForceChance = chaStats.chaProperty.superPushForceChance +
                    //                        chaStats.enviromentProperty.superPushForceChance,
                    // maxPushForceChance = chaStats.chaProperty.maxPushForceChance +
                    //                      chaStats.enviromentProperty.maxPushForceChance,
                    scaleRatios = chaStats.chaProperty.scaleRatios + chaStats.enviromentProperty.scaleRatios,
                    // continuousCollDamagePlus = chaStats.chaProperty.continuousCollDamagePlus -
                    //                            chaStats.enviromentProperty.continuousCollDamagePlus,
                    dodge = chaStats.chaProperty.dodge + chaStats.enviromentProperty.dodge,
                    shieldCount = chaStats.chaProperty.shieldCount +
                                  chaStats.enviromentProperty.shieldCount,
                    defaultcoolDown =
                        chaStats.chaProperty.defaultcoolDown + chaStats.enviromentProperty.defaultcoolDown,
                    defaultBulletRangeRatios = chaStats.chaProperty.defaultBulletRangeRatios +
                                               chaStats.enviromentProperty.defaultBulletRangeRatios
                };
                chaStats.chaProperty = newProperty;
            }
        }
    }
}
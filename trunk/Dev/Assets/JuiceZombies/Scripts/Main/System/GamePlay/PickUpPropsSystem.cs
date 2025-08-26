//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-30 12:09:01
//---------------------------------------------------------------------

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct PickUpPropsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<DropsData>();
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //var gemsQuery = SystemAPI.QueryBuilder().WithAll<DropsData>().Build();
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            new PickUpGemsJob
            {
                dT = SystemAPI.Time.DeltaTime,
                prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe),
                gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe),
                globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe),
                gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe),
                ecb = ecb,
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(),
                bfePlayerProps = SystemAPI.GetBufferLookup<PlayerProps>(),
                //cdfePropsData = SystemAPI.GetComponentLookup<PropsData>(),
                //bthBuff = SystemAPI.GetBufferLookup<BuffOld>(),
                player = SystemAPI.GetSingletonEntity<PlayerData>(),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(true)
            }.ScheduleParallel();
        }

        [BurstCompile]
        public partial struct PickUpGemsJob : IJobEntity
        {
            public float dT;
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GameSetUpData gameSetUpData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public GameOthersData gameOthersData;

            //public ComponentTypeHandle<DropsData> cthDropsData;
            //public EntityTypeHandle cthEntity;
            public EntityCommandBuffer.ParallelWriter ecb;

            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;

            [NativeDisableParallelForRestriction] public ComponentLookup<PlayerData> cdfePlayerData;

            [NativeDisableParallelForRestriction] public BufferLookup<PlayerProps> bfePlayerProps;

            //[ReadOnly] public BufferLookup<BuffOld> bthBuff;
            [ReadOnly] public Entity player;

            //[ReadOnly] public ComponentLookup<AudiosRefPlayerData> cdfeAudiosRefPlayerData;
            [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;

            // public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
            //     in v128 chunkEnabledMask)
            // {
            //     var drops = chunk.GetNativeArray(cthDropsData);
            //     var thentities = chunk.GetNativeArray(cthEntity);
            //
            //     ref var configTbbattle_items =
            //         ref globalConfigData.value.Value.configTbbattle_items.configTbbattle_items;
            //     ref var configTbbattle_exps =
            //         ref globalConfigData.value.Value.configTbbattle_exps.configTbbattle_exps;
            //     ref var configTbskills =
            //         ref globalConfigData.value.Value.configTbskills.configTbskills;
            //     ref var configTbskill_effects =
            //         ref globalConfigData.value.Value.configTbskill_effects.configTbskill_effects;
            //     ref var configTbbuff_effects =
            //         ref globalConfigData.value.Value.configTbbuff_effects.configTbbuff_effects;
            //
            //     for (var i = 0; i < chunk.Count; i++)
            //     {
            //         var entity = thentities[i];
            //         var drop = drops[i];
            //
            //         float range = 0;
            //         for (int j = 0; j < configTbbattle_items.Length; j++)
            //         {
            //             if (configTbbattle_items[j].id == drop.id)
            //             {
            //                 range = configTbbattle_items[j].pickRange;
            //                 break;
            //             }
            //         }
            //
            //         range *= (1 + cdfePlayerData[player].playerData.propsRangeRatios / 10000f);
            //         if (!drop.isLooting)
            //         {
            //             var DropPoint1Offset = gameOthersData.dropPoint1Offset / 100f;
            //             var dir = math.normalizesafesafe(drop.point0 -
            //                                          cdfeLocalTransform[player].Position);
            //             drop.point1 = new float3(drop.point0.x + dir.x * DropPoint1Offset,
            //                 drop.point0.y + dir.y * DropPoint1Offset, 0);
            //         }
            //
            //
            //         if (math.length(cdfeLocalTransform[entity].Position - cdfeLocalTransform[player].Position) <
            //             range)
            //         {
            //             drop.isLooting = true;
            //         }
            //
            //         if (drop.duration < -0.02f)
            //         {
            //             //TODO:更具id换clips
            //             UnityHelper.CreateAudioClip(gameOthersData.allAudioClips["PickGemSound"],
            //                 cdfeLocalTransform[player].Position);
            //
            //             var playerData = cdfePlayerData[player];
            //             var propsData = cdfePropsData[player];
            //             for (int j = 0; j < configTbbattle_items.Length; j++)
            //             {
            //                 if (configTbbattle_items[j].id == drop.id)
            //                 {
            //                     for (int k = 0; k < configTbbattle_items[j].effect.Length; k++)
            //                     {
            //                         //道具效果  更改属性
            //                         if (configTbbattle_items[j].effect[k].x == 1)
            //                         {
            //                             if (configTbbattle_items[j].effect[k].y == 201200)
            //                             {
            //                                 var expnum = configTbbattle_items[j].effect[k].z;
            //                                 playerData.playerData.exp +=
            //                                     expnum * (1 + playerData.playerData.expRatios / 10000f);
            //                             }
            //                             else if (configTbbattle_items[j].effect[k].y == 201300)
            //                             {
            //                                 var goldnum = configTbbattle_items[j].effect[k].z;
            //                                 playerData.playerData.gold +=
            //                                     (int)(goldnum * (1 + playerData.playerData.goldRatios / 10000f));
            //                             }
            //                         }
            //
            //                         //道具效果  释放技能
            //                         if (configTbbattle_items[j].effect[k].x == 2)
            //                         {
            //                             //炸弹  技能组7000031(40000003)  7000032(30000009,30000003)
            //                             if (configTbbattle_items[j].effect[k].y == 700003)
            //                             {
            //                                 //40000003度数(能被5整除),半径（m）,推力万分比,叠加推力上限万分比 360;100;10000;10000
            //                                 //30000009伤害万分比,作用目标 8000;11  30000003攻击力万分比 1000000   1;301;10000
            //                                 ecb.AppendToBuffer(unfilteredChunkIndex, player, new SkillOld
            //                                 {
            //                                     CurrentTypeId = (SkillOld.TypeId)700003,
            //                                     Entity_5 = player,
            //                                     Boolean_11 = true
            //                                 });
            //                             }
            //
            //                             //食物1     固定数值，万分比
            //                             if (configTbbattle_items[j].effect[k].y == 700008)
            //                             {
            //                                 int args0 = 0;
            //                                 int args1 = 0;
            //                                 for (int l = 0; l < configTbskill_effects.Length; l++)
            //                                 {
            //                                     if (configTbskill_effects[l].id == 7000081)
            //                                     {
            //                                         args0 = configTbskill_effects[l]
            //                                             .buff1Para[0];
            //                                         args1 = configTbskill_effects[l]
            //                                             .buff1Para[1];
            //                                         break;
            //                                     }
            //                                 }
            //
            //                                 ecb.AppendToBuffer(unfilteredChunkIndex, player,
            //                                     new Buff_30000020
            //                                     {
            //                                         id = 30000020,
            //                                         priority = 0,
            //                                         maxStack = 0,
            //                                         tags = 0,
            //                                         tickTime = 0,
            //                                         timeElapsed = 0,
            //                                         ticked = 0,
            //                                         duration = 0,
            //                                         permanent = false,
            //                                         caster = default,
            //                                         carrier = player,
            //                                         canBeStacked = false,
            //                                         buffStack = default,
            //                                         buffArgs = new BuffArgs
            //                                         {
            //                                             args0 = args0,
            //                                             args1 = args1,
            //                                             args2 = 0,
            //                                             args3 = 0,
            //                                             args4 = 0
            //                                         }
            //                                     }.ToBuffOld());
            //                             }
            //
            //                             #region 食物测试用
            //
            //                             //食物1     固定数值，万分比
            //                             if (configTbbattle_items[j].effect[k].y == 2)
            //                             {
            //                                 int args0 = 0;
            //                                 int args1 = 0;
            //                                 for (int l = 0; l < configTbskill_effects.Length; l++)
            //                                 {
            //                                     if (configTbskill_effects[l].id == 7000081)
            //                                     {
            //                                         args0 = configTbskill_effects[l]
            //                                             .buff1Para[0];
            //                                         args1 = configTbskill_effects[l]
            //                                             .buff1Para[1];
            //                                         break;
            //                                     }
            //                                 }
            //
            //                                 if (propsData.idToNum.ContainsKey(7000081))
            //                                 {
            //                                     propsData.idToNum[7000081] += 1;
            //                                 }
            //                                 else
            //                                 {
            //                                     propsData.idToNum.Add(7000081, 1);
            //                                 }
            //                             }
            //
            //                             #endregion
            //
            //                             //食物2
            //                             if (configTbbattle_items[j].effect[k].y == 700009)
            //                             {
            //                                 int args0 = 0;
            //                                 int args1 = 0;
            //                                 for (int l = 0; l < configTbskill_effects.Length; l++)
            //                                 {
            //                                     if (configTbskill_effects[l].id == 7000091)
            //                                     {
            //                                         args0 = configTbskill_effects[l]
            //                                             .buff1Para[0];
            //                                         args1 = configTbskill_effects[l]
            //                                             .buff1Para[1];
            //                                         break;
            //                                     }
            //                                 }
            //
            //                                 ecb.AppendToBuffer(unfilteredChunkIndex, player,
            //                                     new Buff_30000020
            //                                     {
            //                                         id = 30000020,
            //                                         priority = 0,
            //                                         maxStack = 0,
            //                                         tags = 0,
            //                                         tickTime = 0,
            //                                         timeElapsed = 0,
            //                                         ticked = 0,
            //                                         duration = 0,
            //                                         permanent = false,
            //                                         caster = default,
            //                                         carrier = player,
            //                                         canBeStacked = false,
            //                                         buffStack = default,
            //                                         buffArgs = new BuffArgs
            //                                         {
            //                                             args0 = args0,
            //                                             args1 = args1,
            //                                             args2 = 0,
            //                                             args3 = 0,
            //                                             args4 = 0
            //                                         }
            //                                     }.ToBuffOld());
            //                             }
            //
            //                             //加速道具1
            //                             //TODO:
            //                             if (configTbbattle_items[j].effect[k].y == 700004)
            //                             {
            //                             }
            //
            //                             //吸铁石   技能组 7000101
            //                             if (configTbbattle_items[j].effect[k].y == 700010)
            //                             {
            //                                 for (int l = 0; l < configTbskill_effects.Length; l++)
            //                                 {
            //                                     if (configTbskill_effects[l].id == 7000101)
            //                                     {
            //                                         //TODO:第一个参数index是 1
            //                                         ecb.AppendToBuffer(unfilteredChunkIndex, player,
            //                                             new Buff_30000019
            //                                             {
            //                                                 id = 30000019,
            //                                                 priority = 0,
            //                                                 maxStack = 0,
            //                                                 tags = 0,
            //                                                 tickTime = 0,
            //                                                 timeElapsed = 0,
            //                                                 duration = 0,
            //                                                 permanent = false,
            //                                                 caster = default,
            //                                                 carrier = player,
            //                                                 canBeStacked = false,
            //                                                 buffStack = default,
            //                                                 //TODO:
            //                                                 buffArgs = new BuffArgs
            //                                                 {
            //                                                     args0 = configTbskill_effects[l]
            //                                                         .buffEntityPara[1],
            //                                                     args1 = configTbskill_effects[l]
            //                                                         .buffEntityPara[2],
            //                                                     args2 = configTbskill_effects[l]
            //                                                         .buffEntityPara[3],
            //                                                     args3 = configTbskill_effects[l]
            //                                                         .buffEntityPara[4],
            //                                                     args4 = configTbskill_effects[l]
            //                                                         .buffEntityPara[5],
            //                                                 }
            //                                             }.ToBuffOld());
            //
            //
            //                                         break;
            //                                     }
            //                                 }
            //                             }
            //                         }
            //                     }
            //
            //                     break;
            //                 }
            //             }
            //
            //             for (int j = 0; j < configTbbattle_exps.Length; j++)
            //             {
            //                 if (j >= configTbbattle_exps.Length)
            //                 {
            //                     playerData.playerData.level = configTbbattle_exps[configTbbattle_exps.Length].id;
            //                     break;
            //                 }
            //
            //                 if (playerData.playerData.exp >= configTbbattle_exps[j].exp &&
            //                     playerData.playerData.exp < configTbbattle_exps[j + 1].exp)
            //                 {
            //                     if (playerData.playerData.level != configTbbattle_exps[j].id)
            //                     {
            //                         BuffOldData_ReadWrite refData = new BuffOldData_ReadWrite
            //                         {
            //                             ecb = ecb,
            //                             damageInfo = default,
            //                             cdfeChaStats = cdfeChaStats,
            //                             skill = default,
            //                         };
            //                         BuffOldData_ReadOnly inData = new BuffOldData_ReadOnly
            //                         {
            //                             sortkey = 0,
            //                             entity = entity,
            //                             player = player,
            //                             prefabMapData = prefabMapData,
            //                             globalConfigData = globalConfigData,
            //                             cdfeLocalTransform = cdfeLocalTransform,
            //                         };
            //
            //                         var countToLevelUp = configTbbattle_exps[j].id - playerData.playerData.level;
            //                         for (int k = 0; k < countToLevelUp; k++)
            //                         {
            //                             foreach (var buff in bthBuff[player])
            //                             {
            //                                 buff.OnLevelUp(ref refData, in inData);
            //                             }
            //                         }
            //
            //                         playerData.playerData.level = configTbbattle_exps[j].id;
            //                     }
            //
            //                     break;
            //                 }
            //             }
            //
            //             cdfePlayerData[player] = playerData;
            //             ecb.DestroyEntity(unfilteredChunkIndex, entity);
            //             continue;
            //         }
            //
            //         if (drop.isLooting)
            //         {
            //             var temp = cdfeLocalTransform[entity];
            //             temp.Position = PhysicsHelper.CubicBezier(
            //                 (gameOthersData.pickupDuration / 1000f -
            //                  drop.duration) / (gameOthersData.pickupDuration / 1000f),
            //                 drop.point0,
            //                 drop.point1,
            //                 drop.point2, new float3(cdfeLocalTransform[player].Position.x,
            //                     cdfeLocalTransform[player].Position.y, 0));
            //             cdfeLocalTransform[entity] = temp;
            //
            //             drop.duration -= dT;
            //         }
            //
            //         drops[i] = drop;
            //     }
            // }

            public void Execute([EntityIndexInQuery] int sortKey, Entity entity, ref DropsData drop)
            {
                if (!drop.isDropAnimed)
                {
                    var AnimDuration = gameOthersData.gameOtherParas.dropAnimedDuration;
                    var height = gameOthersData.gameOtherParas.dropAnimedHeight;
                    if (drop.tick == 0)
                    {
                        drop.dropPoint1 = new float3((drop.dropPoint0.x + drop.dropPoint2.x) / 2f,
                            drop.dropPoint2.y + height, 0);
                    }

                    var temp = cdfeLocalTransform[entity];
                    temp.Position = PhysicsHelper.QuardaticBezier(drop.dropAnimedElpase / AnimDuration
                        , drop.dropPoint0, drop.dropPoint1, drop.dropPoint2);
                    cdfeLocalTransform[entity] = temp;
                    if (drop.dropAnimedElpase > AnimDuration)
                    {
                        drop.isDropAnimed = true;

                        if (drop.id >= 1001 && drop.id <= 1004)
                        {
                            int type = 313;
                            var playerData = cdfePlayerData[player];
                            if (playerData.playerOtherData.guideList.Contains(type))
                            {
                                var e = ecb.CreateEntity(sortKey);

                                ecb.AddComponent(sortKey, e, new HybridEventData
                                {
                                    type = 999000 + type,
                                    args = new float4(temp.Position, 0)
                                });
                                ecb.AddComponent(sortKey, e, new TimeToDieData
                                {
                                    duration = 5
                                });
                                playerData.playerOtherData.guideList.Remove(type);
                                cdfePlayerData[player] = playerData;
                            }
                        }
                    }

                    drop.dropAnimedElpase += dT;
                }
                else
                {
                    ref var configTbbattle_items =
                        ref globalConfigData.value.Value.configTbbattle_items.configTbbattle_items;
                    // ref var configTbbattle_exps =
                    //     ref globalConfigData.value.Value.configTbbattle_exps.configTbbattle_exps;
                    ref var configTbskills =
                        ref globalConfigData.value.Value.configTbskills.configTbskills;


                    // float range = 0;
                    // bool autoPickYn = false;

                    int battleItemsIndex = -1;
                    for (int j = 0; j < configTbbattle_items.Length; j++)
                    {
                        if (configTbbattle_items[j].id == drop.id)
                        {
                            battleItemsIndex = j;
                            // range = configTbbattle_items[j].pickRange / 1000f;
                            // autoPickYn = configTbbattle_items[j].autoPickYn == 1;
                            break;
                        }
                    }

                    ref var tbbattle_items =
                        ref configTbbattle_items[battleItemsIndex];

                    float range = tbbattle_items.pickRange / 1000f;
                    range *= tbbattle_items.pickType == 1
                        ? (1 + cdfePlayerData[player].playerData.pickUpRadiusRatios / 10000f)
                        : 1f;
                    var radius=math.max(cdfePlayerData[player].playerOtherData.pickupRadius, 1);
                    range *= radius;
                    if (!drop.isLooting)
                    {
                        var DropPoint1Offset = gameOthersData.dropPoint1Offset / 100f;
                        var dir = math.normalizesafe(drop.point0 -
                                                     cdfeLocalTransform[player].Position);
                        drop.point1 = new float3(drop.point0.x + dir.x * DropPoint1Offset,
                            drop.point0.y + dir.y * DropPoint1Offset, 0);
                    }


                    if (math.length(cdfeLocalTransform[entity].Position - cdfeLocalTransform[player].Position) <
                        range || (tbbattle_items.autoPickYn == 1))
                    {
                        drop.isLooting = true;
                    }

                    if (drop.lootingAniDuration < -0.02f)
                    {
                        //TODO:更具id换clips
                        UnityHelper.TryCreateAudioClip(globalConfigData, gameOthersData, 2205,out var _);
                        // if (gameOthersData.allAudioClips.TryGetValue("PickGemSound", out var clip))
                        // {
                        //     UnityHelper.CreateAudioClip(gameOthersData.allAudioClips["PickGemSound"]);
                        // }

                        // UnityHelper.CreateAudioClip(gameOthersData.allAudioClips["PickGemSound"],
                        //     cdfeLocalTransform[player].Position);

                        var playerData = cdfePlayerData[player];
                        //var propsData = cdfePropsData[player];


                        for (int k = 0; k < tbbattle_items.effect.Length; k++)
                        {
                            var effectX = tbbattle_items.effect[k].x;
                            var effectY = tbbattle_items.effect[k].y;
                            var effectZ = tbbattle_items.effect[k].z;
                            //道具效果  更改属性
                            if (effectX == 1)
                            {
                                if (effectY == 201200)
                                {
                                    var expnum = effectZ;
                                    playerData.playerData.exp +=
                                        (int)(expnum * (1 + playerData.playerData.expRatios / 10000f));
                                }
                                else if (effectY == 201300)
                                {
                                    var goldnum = tbbattle_items.effect[k].z;
                                    playerData.playerData.gold +=
                                        (int)(goldnum * (1 + playerData.playerData.goldRatios / 10000f));
                                }
                            }

                            //道具效果  释放技能
                            if (effectX == 2)
                            {
                                ecb.AppendToBuffer(sortKey, player, new Skill()
                                {
                                    CurrentTypeId = (Skill.TypeId)1,
                                    Int32_0 = effectY,
                                    Int32_10 = 1,
                                    Entity_5 = player
                                });
                                //炸弹  技能组7000031(40000003)  7000032(30000009,30000003)
                                // if (configTbbattle_items[j].effect[k].y == 700003)
                                // {
                                //     //40000003度数(能被5整除),半径（m）,推力万分比,叠加推力上限万分比 360;100;10000;10000
                                //     //30000009伤害万分比,作用目标 8000;11  30000003攻击力万分比 1000000   1;301;10000
                                //     ecb.AppendToBuffer(unfilteredChunkIndex, player, new SkillOld
                                //     {
                                //         CurrentTypeId = (SkillOld.TypeId)700003,
                                //         Entity_5 = player,
                                //         Boolean_11 = true
                                //     });
                                // }
                                //
                                // //食物1     固定数值，万分比
                                // if (configTbbattle_items[j].effect[k].y == 700008)
                                // {
                                //     int args0 = 0;
                                //     int args1 = 0;
                                //     for (int l = 0; l < configTbskill_effects.Length; l++)
                                //     {
                                //         if (configTbskill_effects[l].id == 7000081)
                                //         {
                                //             args0 = configTbskill_effects[l]
                                //                 .buff1Para[0];
                                //             args1 = configTbskill_effects[l]
                                //                 .buff1Para[1];
                                //             break;
                                //         }
                                //     }
                                //
                                //     ecb.AppendToBuffer(unfilteredChunkIndex, player,
                                //         new Buff_30000020
                                //         {
                                //             id = 30000020,
                                //             priority = 0,
                                //             maxStack = 0,
                                //             tags = 0,
                                //             tickTime = 0,
                                //             timeElapsed = 0,
                                //             ticked = 0,
                                //             duration = 0,
                                //             permanent = false,
                                //             caster = default,
                                //             carrier = player,
                                //             canBeStacked = false,
                                //             buffStack = default,
                                //             buffArgs = new BuffArgs
                                //             {
                                //                 args0 = args0,
                                //                 args1 = args1,
                                //                 args2 = 0,
                                //                 args3 = 0,
                                //                 args4 = 0
                                //             }
                                //         }.ToBuffOld());
                                // }

                                // #region 食物测试用
                                //
                                // //食物1     固定数值，万分比
                                // if (configTbbattle_items[j].effect[k].y == 2)
                                // {
                                //     UnityEngine.Debug.LogError("捡到了");
                                //     //int args0 = 0;
                                //     //int args1 = 0;
                                //     //for (int l = 0; l < configTbskill_effects.Length; l++)
                                //     //{
                                //     //    if (configTbskill_effects[l].id == 201200)
                                //     //    {
                                //     //        args0 = configTbskill_effects[l]
                                //     //            .buff1Para[0];
                                //     //        args1 = configTbskill_effects[l]
                                //     //            .buff1Para[1];
                                //     //        break;
                                //     //    }
                                //     //}
                                //
                                //     if (propsData.idToNum.ContainsKey(1009))
                                //     {
                                //         propsData.idToNum[1009] += 1;
                                //     }
                                //     else
                                //     {
                                //         propsData.idToNum.Add(1009, 1);
                                //     }
                                // }
                                //
                                // #endregion

                                //食物2
                                // if (configTbbattle_items[j].effect[k].y == 700009)
                                // {
                                //     int args0 = 0;
                                //     int args1 = 0;
                                //     for (int l = 0; l < configTbskill_effects.Length; l++)
                                //     {
                                //         if (configTbskill_effects[l].id == 7000091)
                                //         {
                                //             args0 = configTbskill_effects[l]
                                //                 .buff1Para[0];
                                //             args1 = configTbskill_effects[l]
                                //                 .buff1Para[1];
                                //             break;
                                //         }
                                //     }
                                //
                                //     ecb.AppendToBuffer(unfilteredChunkIndex, player,
                                //         new Buff_30000020
                                //         {
                                //             id = 30000020,
                                //             priority = 0,
                                //             maxStack = 0,
                                //             tags = 0,
                                //             tickTime = 0,
                                //             timeElapsed = 0,
                                //             ticked = 0,
                                //             duration = 0,
                                //             permanent = false,
                                //             caster = default,
                                //             carrier = player,
                                //             canBeStacked = false,
                                //             buffStack = default,
                                //             buffArgs = new BuffArgs
                                //             {
                                //                 args0 = args0,
                                //                 args1 = args1,
                                //                 args2 = 0,
                                //                 args3 = 0,
                                //                 args4 = 0
                                //             }
                                //         }.ToBuffOld());
                                // }
                                //
                                // //加速道具1
                                // //TODO:
                                // if (configTbbattle_items[j].effect[k].y == 700004)
                                // {
                                // }
                                //
                                // //吸铁石   技能组 7000101
                                // if (configTbbattle_items[j].effect[k].y == 700010)
                                // {
                                //     for (int l = 0; l < configTbskill_effects.Length; l++)
                                //     {
                                //         if (configTbskill_effects[l].id == 7000101)
                                //         {
                                //             //TODO:第一个参数index是 1
                                //             ecb.AppendToBuffer(unfilteredChunkIndex, player,
                                //                 new Buff_30000019
                                //                 {
                                //                     id = 30000019,
                                //                     priority = 0,
                                //                     maxStack = 0,
                                //                     tags = 0,
                                //                     tickTime = 0,
                                //                     timeElapsed = 0,
                                //                     duration = 0,
                                //                     permanent = false,
                                //                     caster = default,
                                //                     carrier = player,
                                //                     canBeStacked = false,
                                //                     buffStack = default,
                                //                     //TODO:
                                //                     buffArgs = new BuffArgs
                                //                     {
                                //                         args0 = configTbskill_effects[l]
                                //                             .buffEntityPara[1],
                                //                         args1 = configTbskill_effects[l]
                                //                             .buffEntityPara[2],
                                //                         args2 = configTbskill_effects[l]
                                //                             .buffEntityPara[3],
                                //                         args3 = configTbskill_effects[l]
                                //                             .buffEntityPara[4],
                                //                         args4 = configTbskill_effects[l]
                                //                             .buffEntityPara[5],
                                //                     }
                                //                 }.ToBuffOld());
                                //
                                //
                                //             break;
                                //         }
                                //     }
                                // }
                            }

                            if (tbbattle_items.type == 2)
                            {
                                FixedString64Bytes str = $"{effectX};{effectY};{effectZ}";
                                playerData.playerOtherData.outerItemList.Add(str);
                            }


                            var buffers = bfePlayerProps[player];
                            bool contains = false;
                            for (int i = 0; i < buffers.Length; i++)
                            {
                                var temp = buffers[i];
                                if (temp.propId == drop.id)
                                {
                                    contains = true;
                                    temp.count++;
                                }

                                buffers[i] = temp;
                            }


                            if (!contains)
                            {
                                ecb.AppendToBuffer(sortKey, player, new PlayerProps
                                {
                                    propId = drop.id,
                                    count = 1
                                });
                            }
                        }


                        //IntroGuide
                        if (drop.id == 3001 || drop.id == 3002)
                        {
                            var e = ecb.CreateEntity(sortKey);
                            ecb.AddComponent(sortKey, e, new IntroGuideItemData
                            {
                                id = drop.id
                            });
                            ecb.AddComponent(sortKey, e, new TimeToDieData
                            {
                                duration = 3
                            });
                        }


                        // ref var tbguides = ref globalConfigData.value.Value.configTbguides.configTbguides;
                        //
                        // for (int i = 0; i < tbguides.Length; i++)
                        // {
                        //     //新手引导 触发类型3  首次拾取道具id
                        //     // if (tbguides[i].triggerType == 3 && tbguides[i].triggerPara[0] == drop.id &&
                        //     //     playerData.playerOtherData.guideList.Contains(tbguides[i].teamId))
                        //     // {
                        //     //     Debug.Log($"新手引导 触发类型3  首次拾取道具id{drop.id}");
                        //     //     var e = ecb.CreateEntity(unfilteredChunkIndex);
                        //     //
                        //     //     ecb.AddComponent(unfilteredChunkIndex, e, new HybridEventData
                        //     //     {
                        //     //         type = 99900 + tbguides[i].id
                        //     //     });
                        //     //     ecb.AddComponent(unfilteredChunkIndex, e, new TimeToDieData
                        //     //     {
                        //     //         duration = 5
                        //     //     });
                        //     //     playerData.playerOtherData.guideList.Remove(tbguides[i].teamId);
                        //     //     break;
                        //     // }
                        // }


                        // for (int j = 0; j < configTbbattle_exps.Length; j++)
                        // {
                        //     if (j >= configTbbattle_exps.Length)
                        //     {
                        //         playerData.playerData.level = configTbbattle_exps[configTbbattle_exps.Length].id;
                        //         break;
                        //     }
                        //
                        //     if (playerData.playerData.exp >= configTbbattle_exps[j].exp &&
                        //         playerData.playerData.exp < configTbbattle_exps[j + 1].exp)
                        //     {
                        //         if (playerData.playerData.level != configTbbattle_exps[j].id)
                        //         {
                        //             BuffOldData_ReadWrite refData = new BuffOldData_ReadWrite
                        //             {
                        //                 ecb = ecb,
                        //                 damageInfo = default,
                        //                 cdfeChaStats = cdfeChaStats,
                        //                 skill = default,
                        //             };
                        //             BuffOldData_ReadOnly inData = new BuffOldData_ReadOnly
                        //             {
                        //                 sortkey = 0,
                        //                 entity = entity,
                        //                 player = player,
                        //                 prefabMapData = prefabMapData,
                        //                 globalConfigData = globalConfigData,
                        //                 cdfeLocalTransform = cdfeLocalTransform,
                        //             };
                        //
                        //             var countToLevelUp = configTbbattle_exps[j].id - playerData.playerData.level;
                        //             for (int k = 0; k < countToLevelUp; k++)
                        //             {
                        //                 // foreach (var buff in bthBuff[player])
                        //                 // {
                        //                 //     buff.OnLevelUp(ref refData, in inData);
                        //                 // }
                        //             }
                        //
                        //             playerData.playerData.level = configTbbattle_exps[j].id;
                        //         }
                        //
                        //         break;
                        //     }
                        // }

                        cdfePlayerData[player] = playerData;
                        //cdfePropsData[player] = propsData;
                        ecb.DestroyEntity(sortKey, entity);
                        return;
                    }

                    if (drop.isLooting)
                    {
                        var temp = cdfeLocalTransform[entity];
                        temp.Position = PhysicsHelper.CubicBezier(
                            (gameOthersData.pickupDuration / 1000f -
                             drop.lootingAniDuration) / (gameOthersData.pickupDuration / 1000f),
                            drop.point0,
                            drop.point1,
                            drop.point2, new float3(cdfeLocalTransform[player].Position.x,
                                cdfeLocalTransform[player].Position.y, 0));
                        cdfeLocalTransform[entity] = temp;

                        drop.lootingAniDuration -= dT;
                    }
                }

                drop.tick++;
            }
        }
        // [BurstCompile]
        // public struct PickUpGemsJob : IJobChunk
        // {
        //     public float dT;
        //     [ReadOnly] public PrefabMapData prefabMapData;
        //     [ReadOnly] public GameSetUpData gameSetUpData;
        //     [ReadOnly] public GlobalConfigData globalConfigData;
        //     [ReadOnly] public GameOthersData gameOthersData;
        //
        //     public ComponentTypeHandle<DropsData> cthDropsData;
        //     public EntityTypeHandle cthEntity;
        //     public EntityCommandBuffer.ParallelWriter ecb;
        //
        //     [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;
        //
        //     [NativeDisableParallelForRestriction] public ComponentLookup<PlayerData> cdfePlayerData;
        //     [NativeDisableParallelForRestriction] public ComponentLookup<PropsData> cdfePropsData;
        //     [ReadOnly] public BufferLookup<Buff> bthBuff;
        //     [ReadOnly] public Entity player;
        //
        //     //[ReadOnly] public ComponentLookup<AudiosRefPlayerData> cdfeAudiosRefPlayerData;
        //     [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
        //
        //     public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
        //         in v128 chunkEnabledMask)
        //     {
        //         var drops = chunk.GetNativeArray(cthDropsData);
        //         var thentities = chunk.GetNativeArray(cthEntity);
        //
        //         ref var configTbbattle_items =
        //             ref globalConfigData.value.Value.configTbbattle_items.configTbbattle_items;
        //         ref var configTbbattle_exps =
        //             ref globalConfigData.value.Value.configTbbattle_exps.configTbbattle_exps;
        //         ref var configTbskills =
        //             ref globalConfigData.value.Value.configTbskills.configTbskills;
        //         ref var configTbskill_effects =
        //             ref globalConfigData.value.Value.configTbskill_effects.configTbskill_effects;
        //         ref var configTbbuff_effects =
        //             ref globalConfigData.value.Value.configTbbuff_effects.configTbbuff_effects;
        //
        //         for (var i = 0; i < chunk.Count; i++)
        //         {
        //             var entity = thentities[i];
        //             var drop = drops[i];
        //
        //             float range = 0;
        //             for (int j = 0; j < configTbbattle_items.Length; j++)
        //             {
        //                 if (configTbbattle_items[j].id == drop.id)
        //                 {
        //                     range = configTbbattle_items[j].pickRange;
        //                     break;
        //                 }
        //             }
        //
        //             range *= (1 + cdfePlayerData[player].playerData.propsRangeRatios / 10000f);
        //             if (!drop.isLooting)
        //             {
        //                 var DropPoint1Offset = gameOthersData.dropPoint1Offset / 100f;
        //                 var dir = math.normalizesafesafe(drop.point0 -
        //                                              cdfeLocalTransform[player].Position);
        //                 drop.point1 = new float3(drop.point0.x + dir.x * DropPoint1Offset,
        //                     drop.point0.y + dir.y * DropPoint1Offset, 0);
        //             }
        //
        //
        //             if (math.length(cdfeLocalTransform[entity].Position - cdfeLocalTransform[player].Position) <
        //                 range)
        //             {
        //                 drop.isLooting = true;
        //             }
        //
        //             if (drop.duration < -0.02f)
        //             {
        //                 //TODO:更具id换clips
        //                 UnityHelper.CreateAudioClip(gameOthersData.allAudioClips["PickGemSound"],
        //                     cdfeLocalTransform[player].Position);
        //
        //                 var playerData = cdfePlayerData[player];
        //                 var propsData = cdfePropsData[player];
        //                 for (int j = 0; j < configTbbattle_items.Length; j++)
        //                 {
        //                     if (configTbbattle_items[j].id == drop.id)
        //                     {
        //                         for (int k = 0; k < configTbbattle_items[j].effect.Length; k++)
        //                         {
        //                             //道具效果  更改属性
        //                             if (configTbbattle_items[j].effect[k].x == 1)
        //                             {
        //                                 if (configTbbattle_items[j].effect[k].y == 201200)
        //                                 {
        //                                     var expnum = configTbbattle_items[j].effect[k].z;
        //                                     playerData.playerData.exp +=
        //                                         expnum * (1 + playerData.playerData.expRatios / 10000f);
        //                                 }
        //                                 else if (configTbbattle_items[j].effect[k].y == 201300)
        //                                 {
        //                                     var goldnum = configTbbattle_items[j].effect[k].z;
        //                                     playerData.playerData.gold +=
        //                                         (int)(goldnum * (1 + playerData.playerData.goldRatios / 10000f));
        //                                 }
        //                             }
        //
        //                             //道具效果  释放技能
        //                             if (configTbbattle_items[j].effect[k].x == 2)
        //                             {
        //                                 //炸弹  技能组7000031(40000003)  7000032(30000009,30000003)
        //                                 if (configTbbattle_items[j].effect[k].y == 700003)
        //                                 {
        //                                     //40000003度数(能被5整除),半径（m）,推力万分比,叠加推力上限万分比 360;100;10000;10000
        //                                     //30000009伤害万分比,作用目标 8000;11  30000003攻击力万分比 1000000   1;301;10000
        //                                     ecb.AppendToBuffer(unfilteredChunkIndex, player, new SkillOld
        //                                     {
        //                                         CurrentTypeId = (SkillOld.TypeId)700003,
        //                                         Entity_5 = player,
        //                                         Boolean_11 = true
        //                                     });
        //                                 }
        //
        //                                 //食物1     固定数值，万分比
        //                                 if (configTbbattle_items[j].effect[k].y == 700008)
        //                                 {
        //                                     int args0 = 0;
        //                                     int args1 = 0;
        //                                     for (int l = 0; l < configTbskill_effects.Length; l++)
        //                                     {
        //                                         if (configTbskill_effects[l].id == 7000081)
        //                                         {
        //                                             args0 = configTbskill_effects[l]
        //                                                 .buff1Para[0];
        //                                             args1 = configTbskill_effects[l]
        //                                                 .buff1Para[1];
        //                                             break;
        //                                         }
        //                                     }
        //
        //                                     ecb.AppendToBuffer(unfilteredChunkIndex, player,
        //                                         new Buff_30000020
        //                                         {
        //                                             id = 30000020,
        //                                             priority = 0,
        //                                             maxStack = 0,
        //                                             tags = 0,
        //                                             tickTime = 0,
        //                                             timeElapsed = 0,
        //                                             ticked = 0,
        //                                             duration = 0,
        //                                             permanent = false,
        //                                             caster = default,
        //                                             carrier = player,
        //                                             canBeStacked = false,
        //                                             buffStack = default,
        //                                             buffArgs = new BuffArgs
        //                                             {
        //                                                 args0 = args0,
        //                                                 args1 = args1,
        //                                                 args2 = 0,
        //                                                 args3 = 0,
        //                                                 args4 = 0
        //                                             }
        //                                         }.ToBuffOld());
        //                                 }
        //
        //                                 #region 食物测试用
        //                                 //食物1     固定数值，万分比
        //                                 if (configTbbattle_items[j].effect[k].y == 2)
        //                                 {
        //                                     int args0 = 0;
        //                                     int args1 = 0;
        //                                     for (int l = 0; l < configTbskill_effects.Length; l++)
        //                                     {
        //                                         if (configTbskill_effects[l].id == 7000081)
        //                                         {
        //                                             args0 = configTbskill_effects[l]
        //                                                 .buff1Para[0];
        //                                             args1 = configTbskill_effects[l]
        //                                                 .buff1Para[1];
        //                                             break;
        //                                         }
        //                                     }
        //
        //                                     if (propsData.idToNum.ContainsKey(7000081))
        //                                     {
        //                                         propsData.idToNum[7000081] += 1;
        //                                     }
        //                                     else
        //                                     {
        //                                         propsData.idToNum.Add(7000081, 1);
        //                                     }
        //                                 }
        //                                 #endregion
        //                                 //食物2
        //                                 if (configTbbattle_items[j].effect[k].y == 700009)
        //                                 {
        //                                     int args0 = 0;
        //                                     int args1 = 0;
        //                                     for (int l = 0; l < configTbskill_effects.Length; l++)
        //                                     {
        //                                         if (configTbskill_effects[l].id == 7000091)
        //                                         {
        //                                             args0 = configTbskill_effects[l]
        //                                                 .buff1Para[0];
        //                                             args1 = configTbskill_effects[l]
        //                                                 .buff1Para[1];
        //                                             break;
        //                                         }
        //                                     }
        //
        //                                     ecb.AppendToBuffer(unfilteredChunkIndex, player,
        //                                         new Buff_30000020
        //                                         {
        //                                             id = 30000020,
        //                                             priority = 0,
        //                                             maxStack = 0,
        //                                             tags = 0,
        //                                             tickTime = 0,
        //                                             timeElapsed = 0,
        //                                             ticked = 0,
        //                                             duration = 0,
        //                                             permanent = false,
        //                                             caster = default,
        //                                             carrier = player,
        //                                             canBeStacked = false,
        //                                             buffStack = default,
        //                                             buffArgs = new BuffArgs
        //                                             {
        //                                                 args0 = args0,
        //                                                 args1 = args1,
        //                                                 args2 = 0,
        //                                                 args3 = 0,
        //                                                 args4 = 0
        //                                             }
        //                                         }.ToBuffOld());
        //                                 }
        //
        //                                 //加速道具1
        //                                 //TODO:
        //                                 if (configTbbattle_items[j].effect[k].y == 700004)
        //                                 {
        //                                 }
        //
        //                                 //吸铁石   技能组 7000101
        //                                 if (configTbbattle_items[j].effect[k].y == 700010)
        //                                 {
        //                                     for (int l = 0; l < configTbskill_effects.Length; l++)
        //                                     {
        //                                         if (configTbskill_effects[l].id == 7000101)
        //                                         {
        //                                             //TODO:第一个参数index是 1
        //                                             ecb.AppendToBuffer(unfilteredChunkIndex, player,
        //                                                 new Buff_30000019
        //                                                 {
        //                                                     id = 30000019,
        //                                                     priority = 0,
        //                                                     maxStack = 0,
        //                                                     tags = 0,
        //                                                     tickTime = 0,
        //                                                     timeElapsed = 0,
        //                                                     duration = 0,
        //                                                     permanent = false,
        //                                                     caster = default,
        //                                                     carrier = player,
        //                                                     canBeStacked = false,
        //                                                     buffStack = default,
        //                                                     //TODO:
        //                                                     buffArgs = new BuffArgs
        //                                                     {
        //                                                         args0 = configTbskill_effects[l]
        //                                                             .buffEntityPara[1],
        //                                                         args1 = configTbskill_effects[l]
        //                                                             .buffEntityPara[2],
        //                                                         args2 = configTbskill_effects[l]
        //                                                             .buffEntityPara[3],
        //                                                         args3 = configTbskill_effects[l]
        //                                                             .buffEntityPara[4],
        //                                                         args4 = configTbskill_effects[l]
        //                                                             .buffEntityPara[5],
        //                                                     }
        //                                                 }.ToBuffOld());
        //
        //
        //                                             break;
        //                                         }
        //                                     }
        //                                 }
        //                             }
        //                         }
        //
        //                         break;
        //                     }
        //                 }
        //
        //                 for (int j = 0; j < configTbbattle_exps.Length; j++)
        //                 {
        //                     if (j >= configTbbattle_exps.Length)
        //                     {
        //                         playerData.playerData.level = configTbbattle_exps[configTbbattle_exps.Length].id;
        //                         break;
        //                     }
        //
        //                     if (playerData.playerData.exp >= configTbbattle_exps[j].exp &&
        //                         playerData.playerData.exp < configTbbattle_exps[j + 1].exp)
        //                     {
        //                         if (playerData.playerData.level != configTbbattle_exps[j].id)
        //                         {
        //                             BuffOldData_ReadWrite refData = new BuffOldData_ReadWrite
        //                             {
        //                                 ecb = ecb,
        //                                 damageInfo = default,
        //                                 cdfeChaStats = cdfeChaStats,
        //                                 skill = default,
        //                             };
        //                             BuffOldData_ReadOnly inData = new BuffOldData_ReadOnly
        //                             {
        //                                 sortkey = 0,
        //                                 entity = entity,
        //                                 player = player,
        //                                 prefabMapData = prefabMapData,
        //                                 globalConfigData = globalConfigData,
        //                                 cdfeLocalTransform = cdfeLocalTransform,
        //                             };
        //
        //                             var countToLevelUp = configTbbattle_exps[j].id - playerData.playerData.level;
        //                             for (int k = 0; k < countToLevelUp; k++)
        //                             {
        //                                 foreach (var buff in bthBuff[player])
        //                                 {
        //                                     buff.OnLevelUp(ref refData, in inData);
        //                                 }
        //                             }
        //
        //                             playerData.playerData.level = configTbbattle_exps[j].id;
        //                         }
        //
        //                         break;
        //                     }
        //                 }
        //
        //                 cdfePlayerData[player] = playerData;
        //                 ecb.DestroyEntity(unfilteredChunkIndex, entity);
        //                 continue;
        //             }
        //
        //             if (drop.isLooting)
        //             {
        //                 var temp = cdfeLocalTransform[entity];
        //                 temp.Position = PhysicsHelper.CubicBezier(
        //                     (gameOthersData.pickupDuration / 1000f -
        //                      drop.duration) / (gameOthersData.pickupDuration / 1000f),
        //                     drop.point0,
        //                     drop.point1,
        //                     drop.point2, new float3(cdfeLocalTransform[player].Position.x,
        //                         cdfeLocalTransform[player].Position.y, 0));
        //                 cdfeLocalTransform[entity] = temp;
        //
        //                 drop.duration -= dT;
        //             }
        //
        //             drops[i] = drop;
        //         }
        //     }
        //     
        //     
        //     
        //     
        //     
        // }
    }
}
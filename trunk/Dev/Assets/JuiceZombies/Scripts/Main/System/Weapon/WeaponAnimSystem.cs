//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-09-13 12:00:25
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
    [UpdateAfter(typeof(FlipSystem))]
    public partial struct WeaponAnimSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WeaponData>();
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);
            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            new WeaponOffsetJob
            {
                fDT = SystemAPI.Time.fixedDeltaTime,
                gameOthersData = gameOthersData,
                gameTimeData = gameTimeData,
                globalConfigData = globalConfigData,
                cdfeStateMachine = SystemAPI.GetComponentLookup<StateMachine>(true),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
                bfeSkill = SystemAPI.GetBufferLookup<Skill>(true),
                cdfeJiYuFlip = SystemAPI.GetComponentLookup<JiYuFlip>(true),
                cdfeFlipData = SystemAPI.GetComponentLookup<FlipData>(true),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct WeaponOffsetJob : IJobEntity
        {
            [ReadOnly] public float fDT;
            [ReadOnly] public GameOthersData gameOthersData;
            [ReadOnly] public GameTimeData gameTimeData;

            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public ComponentLookup<StateMachine> cdfeStateMachine;
            [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
            [ReadOnly] public BufferLookup<Skill> bfeSkill;
            [ReadOnly] public ComponentLookup<JiYuFlip> cdfeJiYuFlip;
            [ReadOnly] public ComponentLookup<FlipData> cdfeFlipData;
            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;

            public void Execute([EntityIndexInQuery] int sortKey, Entity e, ref LocalTransform tran,
                ref WeaponData weaponData, in Parent parent)
            {
                if (gameTimeData.logicTime.gameTimeScale < math.EPSILON)
                {
                    return;
                }

                ref var monster_weaponConfig =
                    ref globalConfigData.value.Value.configTbweapons.configTbweapons;
                int monster_weaponIndex = default;
                for (int i = 0; i < monster_weaponConfig.Length; i++)
                {
                    if (monster_weaponConfig[i].id == weaponData.weaponId)
                    {
                        monster_weaponIndex = i;
                        break;
                    }
                }

                ref var monster_weapon = ref monster_weaponConfig[monster_weaponIndex];

                ref var animPara = ref monster_weapon.displayPara1;


                //cdfeFlip[e] = cdfeFlip[parent.Value];

                tran.Position = weaponData.offset;

                if (!weaponData.enableEnemyWeapon &&
                    (cdfeStateMachine[parent.Value].curAnim == AnimationEnum.Run)
                    && !cdfePlayerData.HasComponent(parent.Value))
                {
                    tran.Scale = 0;
                }
                else
                {
                    tran.Scale = weaponData.scale;
                }

                if ((cdfeStateMachine[parent.Value].curAnim == AnimationEnum.Die ||
                     cdfeStateMachine[parent.Value].curAnim == AnimationEnum.Die_lm) &&
                    !cdfePlayerData.HasComponent(parent.Value))
                {
                    tran.Scale = 0;
                }


                if (cdfeJiYuFlip[e].value.x == 0)
                {
                    tran.Position.x = weaponData.offset.x;
                    tran.Rotation = weaponData.rotation;
                }
                //镜像
                else
                {
                    tran.Position.x = -weaponData.offset.x;
                    tran.Rotation = new quaternion(weaponData.rotation.value.x, -weaponData.rotation.value.y,
                        -weaponData.rotation.value.z, weaponData.rotation.value.w);
                }


                if (cdfePlayerData.HasComponent(parent.Value))
                {
                    if (cdfeStateMachine[parent.Value].curAnim != AnimationEnum.Die)
                    {
                        var playerData = cdfePlayerData[parent.Value];
                        var skills = bfeSkill[parent.Value];
                        foreach (var skill in skills)
                        {
                            if (skill.Int32_0 == playerData.playerOtherData.weaponSkillId)
                            {
                                if (skill.Single_7 <= 0)
                                {
                                    weaponData.curAttackTime = weaponData.attackTime;
                                    weaponData.curRepeatTimes = weaponData.repeatTimes;
                                }

                                break;
                            }
                        }
                    }


                    if (weaponData.curAttackTime > 0)
                    {
                        ref var tbplayer_weapons =
                            ref globalConfigData.value.Value.configTbweapons.configTbweapons;
                        int player_weaponsIndex = default;
                        for (int i = 0; i < tbplayer_weapons.Length; i++)
                        {
                            if (tbplayer_weapons[i].id == weaponData.weaponId)
                            {
                                player_weaponsIndex = i;
                                break;
                            }
                        }

                        ref var player_weapons = ref tbplayer_weapons[player_weaponsIndex];
                        ref var attackAnimPara = ref player_weapons.displayPara1;
                        ref var diedDisplayPara = ref player_weapons.displayPara2;

                        if (cdfeStateMachine[parent.Value].curAnim == AnimationEnum.Die)
                        {
                            tran = WeaponAnimTran5(ref diedDisplayPara, ref weaponData, e, tran);
                        }
                        else
                        {
                            switch (player_weapons.displayType)
                            {
                                case 1:
                                    tran = WeaponAnimTran1(ref attackAnimPara, ref weaponData, e, tran);
                                    break;
                                case 2:
                                    tran = WeaponAnimTran2(ref attackAnimPara, ref weaponData, e, tran);
                                    break;
                                case 3:
                                    tran = WeaponAnimTran3(ref attackAnimPara, ref weaponData, e, tran);
                                    break;
                            }
                        }

                        weaponData.curAttackTime -= fDT;
                    }
                }
                else
                {
                    if (weaponData.curAttackTime > 0)
                    {
                        bool isBoss = cdfeEnemyData[parent.Value].type == UnityHelper.GetTargetId(5);

                        if (isBoss)
                        {
                            tran = WeaponAnimTran4(ref monster_weapon.displayPara2, ref weaponData, e, tran);
                        }
                        else
                        {
                            switch (monster_weapon.displayType)
                            {
                                case 1:
                                    tran = WeaponAnimTran1(ref animPara, ref weaponData, e, tran);
                                    break;
                                case 2:
                                    tran = WeaponAnimTran2(ref animPara, ref weaponData, e, tran);
                                    break;
                                case 3:
                                    tran = WeaponAnimTran3(ref animPara, ref weaponData, e, tran);
                                    break;
                            }
                        }

                        weaponData.curAttackTime -= fDT;
                    }
                }


                weaponData.tick++;
            }


            private LocalTransform WeaponAnimTran1(ref BlobArray<int> animPara,
                ref WeaponData weaponData, Entity e, LocalTransform tran)
            {
                float p1 = animPara[0];
                float p2 = animPara[1];
                float p3 = animPara[2];
                float p4 = animPara[3];
                float p5 = animPara[4];
                float p6 = animPara[5];
                float p9 = animPara[8];

                var curAttackTime = weaponData.attackTime - weaponData.curAttackTime;
                var rotateOffset = weaponData.rotateOffset;
                var ogiTran = new LocalTransform
                {
                    Position = weaponData.offset,
                    Scale = weaponData.scale,
                    Rotation = weaponData.rotation
                };
                var RecovTime = p9;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    rotateOffset.x = -rotateOffset.x;
                    ogiTran.Position.x = -ogiTran.Position.x;
                }


                var clamp1 = math.clamp(curAttackTime, 0,
                    p1 / 1000f);

                float t1 = clamp1 / (p1 / 1000f);

                var degree = math.lerp(0, p3, t1);

                float radians = math.radians(-degree);
                radians = -radians;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    radians = -radians;
                }

                var curDistance = -math.lerp(0, p4 / 100f, t1);

                curDistance = -curDistance;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    curDistance = -curDistance;
                }

                tran = tran.Translate(new float3(curDistance, 0f, 0f));

                tran = MathHelper.RotateAround(tran, rotateOffset, radians);


                var clamp2 = math.clamp((curAttackTime - p1 / 1000f), 0,
                    p2 / 1000f);

                float t2 = clamp2 / (p2 / 1000f);

                var curDistance2 = math.lerp(0, p5 / 100f, t2);

                curDistance2 = -curDistance2;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    curDistance2 = -curDistance2;
                }

                tran = tran.Translate(new float3(curDistance2, 0f, 0f));


                // var clamp6 = math.clamp((curAttackTime - p1 / 1000f- p2 / 1000f), 0,
                //     p6 / 1000f);
                //
                // float t6 = clamp6 / (p6 / 1000f);
                //
                // var curDistance2 = math.lerp(0, p5 / 100f, t6);
                //
                // curDistance2 = -curDistance2;
                //
                // tran = tran.Translate(new float3(curDistance2, 0f, 0f));

                //Recov
                var clampEnd = math.clamp((curAttackTime - (p1 / 1000f) - (p2 / 1000f) - (p6 / 1000f)), 0,
                    RecovTime / 1000f);

                float tR = clampEnd / (RecovTime / 1000f);

                var pos = math.lerp(tran.Position, ogiTran.Position, tR);
                var scale = math.lerp(tran.Scale, ogiTran.Scale, tR);
                var quaternion = math.nlerp(tran.Rotation, ogiTran.Rotation, tR);

                tran.Position = pos;
                tran.Scale = scale;
                tran.Rotation = quaternion;

                return tran;
            }

            private LocalTransform WeaponAnimTran2(ref BlobArray<int> animPara,
                ref WeaponData weaponData, Entity e, LocalTransform tran)
            {
                float p1 = animPara[0];
                float p2 = animPara[1];
                float p3 = animPara[2];
                float p4 = animPara[3];
                float p5 = animPara[4];
                float p6 = animPara[5];
                float p7 = animPara[6];
                float p8 = animPara[7];
                float p9 = animPara[8];


                var curAttackTime = weaponData.attackTime - weaponData.curAttackTime;
                var rotateOffset = weaponData.rotateOffset;
                var ogiTran = new LocalTransform
                {
                    Position = weaponData.offset,
                    Scale = weaponData.scale,
                    Rotation = weaponData.rotation
                };
                var RecovTime = p9;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    rotateOffset.x = -rotateOffset.x;
                    ogiTran.Position.x = -ogiTran.Position.x;
                }


                var clamp1 = math.clamp(curAttackTime, 0,
                    p1 / 1000f);

                float t1 = clamp1 / (p1 / 1000f);


                var degree = math.lerp(0, p4, t1);


                float radians = math.radians(degree);

                radians = -radians;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    radians = -radians;
                }

                tran = MathHelper.RotateAround(tran, rotateOffset, radians);


                var s1 = math.lerp(0, p6 / 1000f, t1);
                var up1 = MathHelper.Forward(tran.Rotation);


                tran.Position += (s1 * up1);

                var clamp3 = math.clamp(curAttackTime - p1 / 1000f - p2 / 1000f, 0, p3 / 1000f);
                float t3 = clamp3 / (p3 / 1000f);


                var degree3 = math.lerp(0, p5, t3);

                float radians3 = math.radians(degree3);
                radians3 = -radians3;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    radians3 = -radians3;
                }

                tran = MathHelper.RotateAround(tran, rotateOffset, radians3);


                var s2 = math.lerp(0, p7 / 1000f, t3);
                var up2 = MathHelper.Forward(tran.Rotation);


                int tempdegree = -45;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    tempdegree = -tempdegree;
                }

                var newup2 = MathHelper.RotateVector(up2, tempdegree);

                tran.Position += (s2 * -newup2);


                //Recov
                var clampEnd = math.clamp((curAttackTime - (p1 / 1000f) - (p2 / 1000f) - (p3 / 1000f) - (p8 / 1000f)),
                    0,
                    RecovTime / 1000f);

                float tR = clampEnd / (RecovTime / 1000f);

                var pos = math.lerp(tran.Position, ogiTran.Position, tR);

                var scale = math.lerp(tran.Scale, ogiTran.Scale, tR);
                var quaternion = math.nlerp(tran.Rotation, ogiTran.Rotation, tR);

                tran.Position = pos;
                tran.Scale = scale;
                tran.Rotation = quaternion;

                return tran;
            }

            private LocalTransform WeaponAnimTran3(ref BlobArray<int> animPara,
                ref WeaponData weaponData, Entity e, LocalTransform tran)
            {
                float p1 = animPara[0];
                float p2 = animPara[1];
                float p3 = animPara[2];
                float p4 = animPara[3];
                float p5 = animPara[4];
                float p6 = animPara[5];
                int p7 = animPara[6];
                float p8 = animPara[7];
                float p9 = animPara[8];


                var curAttackTime = weaponData.attackTime - weaponData.curAttackTime;
                var rotateOffset = weaponData.rotateOffset;
                var ogiTran = new LocalTransform
                {
                    Position = weaponData.offset,
                    Scale = weaponData.scale,
                    Rotation = weaponData.rotation
                };
                var RecovTime = p9;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    rotateOffset.x = -rotateOffset.x;
                    ogiTran.Position.x = -ogiTran.Position.x;
                }

                float oneTime = (p1 + p2 + p3 + p6 + p9) / 1000f;

                for (int i = 0; i < p7; i++)
                {
                    var startTime = i * ((p8 / 1000f) + oneTime);
                    curAttackTime = curAttackTime - startTime;
                    var clamp1 = math.clamp(curAttackTime, 0,
                        p1 / 1000f);

                    float t1 = clamp1 / (p1 / 1000f);


                    var degree = math.lerp(0, p4, t1);

                    float radians = math.radians(degree);


                    radians = -radians;

                    if (cdfeJiYuFlip[e].value.x == 1)
                    {
                        radians = -radians;
                    }

                    tran = MathHelper.RotateAround(tran, rotateOffset, radians);

                    var clamp3 = math.clamp(curAttackTime - p1 / 1000f - p2 / 1000f, 0, p3 / 1000f);
                    float t3 = clamp3 / (p3 / 1000f);


                    var degree3 = math.lerp(0, p5, t3);

                    float radians3 = math.radians(degree3);
                    radians3 = -radians3;
                    if (cdfeJiYuFlip[e].value.x == 1)
                    {
                        radians3 = -radians3;
                    }

                    tran = MathHelper.RotateAround(tran, rotateOffset, radians3);

                    //Recov
                    var clampEnd = math.clamp(
                        (curAttackTime - (p1 / 1000f) - (p2 / 1000f) - (p3 / 1000f) - (p6 / 1000f)),
                        0,
                        RecovTime / 1000f);

                    float tR = clampEnd / (RecovTime / 1000f);

                    var pos = math.lerp(tran.Position, ogiTran.Position, tR);

                    var scale = math.lerp(tran.Scale, ogiTran.Scale, tR);
                    var quaternion = math.nlerp(tran.Rotation, ogiTran.Rotation, tR);
                    tran.Position = pos;
                    tran.Scale = scale;
                    tran.Rotation = quaternion;
                }


                return tran;
            }

            /// <summary>
            /// 普通boss攻击施法动画
            /// </summary>
            /// <param name="animPara"></param>
            /// <param name="weaponData"></param>
            /// <param name="e"></param>
            /// <param name="tran"></param>
            /// <returns></returns>
            private LocalTransform WeaponAnimTran4(ref BlobArray<int> animPara,
                ref WeaponData weaponData, Entity e, LocalTransform tran)
            {
                float p1 = animPara[0];
                float p2 = animPara[1];
                float p3 = animPara[2];
                float p4 = animPara[3];
                float p5 = animPara[4];
                float p6 = animPara[5];
                float p7 = animPara[6];
                float p8 = animPara[7];
                float p9 = animPara[8];

                //float p9 = animPara[8];

                var curAttackTime = weaponData.attackTime - weaponData.curAttackTime;
                var rotateOffset = weaponData.rotateOffset;
                var ogiTran = new LocalTransform
                {
                    Position = weaponData.offset,
                    Scale = weaponData.scale,
                    Rotation = weaponData.rotation
                };
                rotateOffset.x = ogiTran.Position.x + p3 / 1000f;
                rotateOffset.y = ogiTran.Position.y + p4 / 1000f;
                var RecovTime = p9;

                var rotateOffset1 = new float3(ogiTran.Position.x + p1 / 1000f, ogiTran.Position.y + p2 / 1000f, 0);
                var dir = math.normalize(rotateOffset1 - rotateOffset);
                var caldegree = MathHelper.SignedAngle(dir, math.up());

                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    rotateOffset.x = -rotateOffset.x;
                    ogiTran.Position.x = -ogiTran.Position.x;
                }

                //Debug.Log($"caldegree {caldegree}");
                var clamp1 = math.clamp(curAttackTime, 0,
                    p5 / 1000f);

                float t1 = clamp1 / (p5 / 1000f);

                var degree = math.lerp(0, caldegree, t1);

                float radians = math.radians(degree);
                radians = -radians;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    radians = -radians;
                }

                tran = MathHelper.RotateAround(tran, rotateOffset, radians);

                var clamp2 = math.clamp(curAttackTime - p5 / 1000f, 0, p7 / 1000f);
                float t2 = clamp2 / (p7 / 1000f);

                var curDistance = -math.lerp(0, p6 / 100f, t2);

                curDistance = -curDistance;

                tran = tran.Translate(new float3(0f, curDistance, 0f));

                //放大缩小
                float scaleMulti = 1.5f;
                var clamp3 = math.clamp(curAttackTime - (p5 / 1000f) - (p7 / 1000f), 0, p8 / 1000f);

                float t3 = clamp3 / (p8 / 1000f);


                if (t3 < 0.5f)
                {
                    var scale3 = math.lerp(1, scaleMulti, t3);

                    tran.Scale *= scale3;
                }
                else
                {
                    var scale3 = math.lerp(scaleMulti, 1, t3);

                    tran.Scale *= scale3;
                }


                // var clamp6 = math.clamp((curAttackTime - p1 / 1000f- p2 / 1000f), 0,
                //     p6 / 1000f);
                //
                // float t6 = clamp6 / (p6 / 1000f);
                //
                // var curDistance2 = math.lerp(0, p5 / 100f, t6);
                //
                // curDistance2 = -curDistance2;
                //
                // tran = tran.Translate(new float3(curDistance2, 0f, 0f));

                //Recov
                var clampEnd = math.clamp((curAttackTime - (p5 / 1000f) - (p7 / 1000f) - (p8 / 1000f)), 0,
                    RecovTime / 1000f);

                float tR = clampEnd / (RecovTime / 1000f);

                var pos = math.lerp(tran.Position, ogiTran.Position, tR);
                var scale = math.lerp(tran.Scale, ogiTran.Scale, tR);
                var quaternion = math.nlerp(tran.Rotation, ogiTran.Rotation, tR);
                if (clampEnd > 0)
                {
                    Debug.Log($"clampEnd pos{pos} scale{scale} ");
                }

                tran.Position = pos;
                tran.Scale = scale;
                tran.Rotation = quaternion;

                return tran;
            }

            /// <summary>
            /// 玩家死亡动画
            /// </summary>
            /// <param name="animPara"></param>
            /// <param name="weaponData"></param>
            /// <param name="e"></param>
            /// <param name="tran"></param>
            /// <returns></returns>
            private LocalTransform WeaponAnimTran5(ref BlobArray<int> animPara,
                ref WeaponData weaponData, Entity e, LocalTransform tran)
            {
                float p1 = animPara[0];
                float p2 = animPara[1];
                float p3 = animPara[2];
                float p4 = animPara[3];
                float p5 = animPara[4];
                float p6 = animPara[5];
                float p7 = animPara[6];

                var curAttackTime = weaponData.secondAnimTime - weaponData.curAttackTime;
                var rotateOffset = weaponData.rotateOffset;
                // var ogiTran = new LocalTransform
                // {
                //     Position = weaponData.offset,
                //     Scale = weaponData.scale,
                //     Rotation = weaponData.rotation
                // };
                //
                // if (cdfeJiYuFlip[e].value.x == 1)
                // {
                //     rotateOffset.x = -rotateOffset.x;
                //     ogiTran.Position.x = -ogiTran.Position.x;
                // }


                var clamp1 = math.clamp(curAttackTime, 0,
                    p2 / 1000f);

                float t1 = clamp1 / (p2 / 1000f);

                var degree = math.lerp(0, p1, t1);

                float radians = math.radians(degree);
                radians = -radians;

                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    radians = -radians;
                }

                tran = tran.RotateZ(radians);


                var curDistance = -math.lerp(0, p3 / 100f, t1);

                curDistance = -curDistance;

                tran = tran.Translate(new float3(0f, curDistance, 0f));


                var clamp3 = math.clamp(curAttackTime - p2 / 1000f - p4 / 1000f, 0, p6 / 1000f);
                float t3 = clamp3 / (p6 / 1000f);


                var degree3 = math.lerp(0, p5, t3);

                float radians3 = math.radians(degree3);
                radians3 = -radians3;
                if (cdfeJiYuFlip[e].value.x == 1)
                {
                    radians3 = -radians3;
                }

                tran = tran.RotateZ(radians3);

                //tran = MathHelper.RotateAround(tran, tran.Position + rotateOffset, radians3);


                var curDistance2 = math.lerp(0, p7 / 100f, t3);

                curDistance2 = -curDistance2;

                tran = tran.Translate(new float3(0f, curDistance2, 0f));


                return tran;
            }
        }
    }
}
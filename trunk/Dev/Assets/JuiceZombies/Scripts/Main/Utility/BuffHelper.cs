//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-24 12:25:25
//---------------------------------------------------------------------


using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using cfg.config;
//using ProjectDawn.ContinuumCrowds;
using ProjectDawn.Navigation;
using Rewired;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using Random = Unity.Mathematics.Random;

namespace Main
{
    // public struct BuffArgsList
    // {
    //     public int buffId;
    //
    //     public NativeArray<int> buffargs;
    //
    //     public int target;
    // }

    public static class BuffHelper
    {
        /// <summary>
        /// 说明
        /// </summary>
        public enum TargetEnum
        {
            Nothing = 0,
            Player = 1,
            PlayerSommon = 2,
            PlayerNpc = 4,
            LittleMonster = 8,
            ElliteMonster = 16,
            BossMonster = 32,
            Bullet = 64,
            Obstacle = 128,
            Area = 256,

            All = Player | PlayerSommon | PlayerNpc | LittleMonster
                  | ElliteMonster | BossMonster | Bullet | Obstacle |
                  Area,
        }

        public const uint All = 0xffffffff;


        public const int MaxSelfRect = 10;

        /// <summary>
        /// 阵营说明
        /// </summary>
        public enum TargetGroupEnum
        {
            PlayerGroup = 7,
            EnemyGroup = 56,
            BossAndElite = 48
        }

        /// <summary>
        /// 伪状态
        /// </summary>
        public enum DiyState
        {
            NOTMOVE = 0,
            MOVE = 1,
            IDLE = 2
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwitchDiyStateForBoss(DiyState diyState, AgentBody agentBody, StateMachine stateMachine,
            float3 targetPos)
        {
            switch (diyState)
            {
                case DiyState.NOTMOVE:
                    agentBody.Stop();
                    break;
                case DiyState.MOVE:
                    agentBody.SetDestination(targetPos);
                    stateMachine.curAnim = AnimationEnum.Run;
                    break;
                case DiyState.IDLE:
                    agentBody.Stop();
                    stateMachine.curAnim = AnimationEnum.Die;
                    break;
            }
        }


        /// <summary>
        /// 移除一个技能
        /// </summary>
        /// <param name="oldSkillID">技能id</param>
        /// <param name="skillBuffer">技能dbf</param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveOldSkill(int oldSkillID, EntityManager entityManager, Entity player)
        {
            var skillBuffer = entityManager.GetBuffer<Skill>(player);
            for (int i = 0; i < skillBuffer.Length; i++)
            {
                if (skillBuffer[i].Int32_0 == oldSkillID)
                {
                    skillBuffer.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 移除一个技能
        /// </summary>
        /// <param name="oldSkillID">技能id</param>
        /// <param name="skillBuffer">技能dbf</param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveOldSkill(int oldSkillID, ref DynamicBuffer<Skill> skillBuffer,
            ref DynamicBuffer<Trigger> triggers, ref DynamicBuffer<Buff> buffs, ref DynamicBuffer<GameEvent> gameevents)
        {
            for (int i = 0; i < skillBuffer.Length; i++)
            {
                if (skillBuffer[i].Int32_0 == oldSkillID)
                {
                    skillBuffer.RemoveAt(i);
                    break;
                }
            }

            //移除旧trigger

            for (int i = 0; i < triggers.Length; i++)
            {
                if (triggers[i].Int32_10 == oldSkillID)
                {
                    triggers.RemoveAt(i);
                    //var curTrigger = triggers[i];
                    //curTrigger.Boolean_22 = true;
                    //curTrigger.Boolean_11 = true;
                    //triggers[i] = curTrigger;
                }
            }

            for (int i = 0; i < buffs.Length; i++)
            {
                if (buffs[i].Int32_20 == oldSkillID)
                {
                    var buff = buffs[i];
                    buff.Boolean_4 = false;
                    buff.Single_3 = 0f;
                    buffs[i] = buff;
                }
            }

            for (int i = 0; i < gameevents.Length; i++)
            {
                if (gameevents[i].Int32_15 == oldSkillID)
                {
                    var gameevent = gameevents[i];
                    gameevent.Single_4 = 0f;
                    gameevent.Boolean_6 = false;
                    gameevents[i] = gameevent;
                }
            }
        }

        /// <summary>
        /// 加一次性技能
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="skillID"></param>
        /// <param name="target"></param>
        /// <param name="sortKey"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOnceCastSkill(ref EntityCommandBuffer.ParallelWriter ecb,
            int skillID, Entity target, int sortKey)
        {
            AddSkillByEcb(ref ecb, sortKey, skillID, target, 1);
        }

        /// <summary>
        /// ecb加技能
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="sortKey"></param>
        /// <param name="skillID">加的技能id</param>
        /// <param name="carrier">给谁加技能</param>
        /// <param name="skillType"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddSkillByEcb(ref EntityCommandBuffer.ParallelWriter ecb, int sortKey,
            int skillID, Entity carrier, int skillType)
        {
            ecb.AppendToBuffer(sortKey, carrier, new Skill
            {
                CurrentTypeId = (Skill.TypeId)1,
                Int32_0 = skillID,
                Entity_5 = carrier,
                Int32_10 = skillType
            });
        }


        /// <summary>
        /// ecbs加技能
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="sortKey"></param>
        /// <param name="skillID">加的技能id</param>
        /// <param name="carrier">给谁加技能</param>
        /// <param name="skillType"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddSkillByEcbs(ref EntityCommandBuffer ecbs,
            int skillID, Entity carrier, int skillType)
        {
            ecbs.AppendToBuffer(carrier, new Skill
            {
                CurrentTypeId = (Skill.TypeId)1,
                Int32_0 = skillID,
                Entity_5 = carrier,
                Int32_10 = skillType
            });
        }


        /// <summary>
        /// dcb加技能
        /// </summary>
        /// <param name="skills"></param>
        /// <param name="newSkillID">加的</param>
        /// <param name="carrier"></param>
        /// <param name="skillType"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddSkillByDcb(EntityManager entityManager, int newSkillID, Entity carrier, int skillType)
        {
            Debug.Log($"AddSkillByDcbAddSkillByDcbAddSkillByDcb:{newSkillID}");
            var skillBuffer = entityManager.GetBuffer<Skill>(carrier);
            skillBuffer.Add(new Skill
            {
                CurrentTypeId = (Skill.TypeId)1,
                Int32_0 = newSkillID,
                Int32_10 = skillType,
                Entity_5 = carrier
            });
        }


        /// <summary>
        ///  ******************************索敌的机制 返回的loc为技能碰撞盒在何处生成*************************************
        /// </summary>
        /// <param name="seekTypePar">索敌参数 读表</param>
        /// <param name="deviatePar">偏移参数 读表</param>
        /// <param name="seekType">索敌类型 读表</param>
        /// <param name="deviateType">偏移类型 读表 弹幕的为-1</param>
        /// <param name="rangeType">范围类型 读表 弹幕的为-1</param>
        /// <param name="entities">用来索敌的所有entities 传</param>
        /// <param name="caster">索敌的作用方 传</param>
        /// <param name="cdfeLocalTransform"></param>
        /// <param name="cdfeTargetData"></param>
        /// <param name="cdfeChaStats"></param>
        /// <param name="targetGroup">索敌目标值 读表</param>
        /// <param name="target">返回target 有可能是null</param>
        /// <param name="dir">返回索敌方向</param>
        /// <param name="isLocatedFirst">是否先位移再索敌</param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LocalTransform SeekTargets(ref BlobArray<int> seekTypePar, ref BlobArray<int> deviatePar,
            int seekType, int deviateType, int rangeType, ref BlobArray<int> rangePar,
            NativeArray<Entity> entities, Entity caster, Entity locer,
            ComponentLookup<LocalTransform> cdfeLocalTransform,
            ComponentLookup<TargetData> cdfeTargetData,
            ComponentLookup<ChaStats> cdfeChaStats, uint targetGroup, out float3 targetPos, out float3 dir,
            out Entity target, BufferLookup<Buff> cdfeBuff, ComponentLookup<BulletTempTag> cdfeBulletTempTag, uint tick,
            int num = 1, bool isLocatedFirst = false, float3 locatedPos = default)
        {
            for (int i = 0; i < deviatePar.Length; i++)
            {
                // Debug.Log($"deviatePar{i}:{deviatePar[i]}");
            }

            Debug.Log($"debug:11:{seekType}");
            targetPos = float3.zero;
            target = Entity.Null;
            Debug.Log($"locer:{locer}");
            var loc = cdfeLocalTransform[locer];
            if (!cdfeChaStats.HasComponent(locer))
            {
                if (cdfeBulletTempTag.HasComponent(locer))
                {
                    dir = cdfeBulletTempTag[locer].dir;
                }
                else
                {
                    Debug.Log($"该holder没有chastats,不能索敌，位置:{loc.Position}");
                    if (cdfeChaStats.HasComponent(caster))
                    {
                        dir = cdfeChaStats[caster].chaResource.direction;
                        Debug.Log($"caster:{caster},面向方向:{dir}");
                    }
                    else
                    {
                        dir = float3.zero;
                        Debug.Log($"caster:{locer},面向方向:{dir}");
                    }

                    targetPos = loc.Position;
                }
            }
            else
            {
                dir = cdfeChaStats[locer].chaResource.direction;
            }

            Debug.Log($"debug:22:{seekType}");
            switch (seekType)
            {
                //无索敌 都是索自身 
                case 0:
                    if (deviateType == 1)
                    {
                        //TODO:
                        loc.Position = GetRandomPointInCircle(loc.Position, deviatePar[0] / 1000f, 0,
                            (uint)(tick * 10 + num));
                    }

                    if (rangeType == 4)
                    {
                        loc.Position += dir * rangePar[3] / 1000f * num;
                    }


                    targetPos = loc.Position;
                    Debug.Log($"dir111:{dir},loc.position:{loc.Position},targetPos:{targetPos}");
                    break;
                //自动索敌 索范围内最近的
                case 1:
                    //特殊的那个弹幕 轨迹为11 需要弹幕索敌
                    Entity tempEntity = locer;
                    if (deviateType == 11)
                    {
                        locer = caster;
                    }
					Debug.Log($"321321321{locer}");
                    if (!cdfeChaStats.HasComponent(locer))
                    {
						Debug.Log($"123123");
                        return loc;
                    }
					Debug.Log($"321321");

                    float dis = seekTypePar[0] / 1000f;


                    if (seekTypePar.Length > 2)
                    {
                        target = NerestTargetWithState(entities, cdfeLocalTransform, cdfeTargetData, cdfeChaStats,
                            cdfeBuff, locer,
                            targetGroup, ref dis, seekTypePar[2]);
                        if (deviateType == 11)
                        {
                            target = NerestTargetWithState(entities, cdfeLocalTransform, cdfeTargetData,
                                cdfeChaStats,
                                cdfeBuff, tempEntity,
                                targetGroup, ref dis, seekTypePar[2]);
                        }
                    }
                    else
                    {
                        target = NerestTarget(entities, cdfeLocalTransform, cdfeTargetData, cdfeChaStats, locer,
                            targetGroup, ref dis);
                        if (deviateType == 11)
                        {
                            target = NerestTarget(entities, cdfeLocalTransform, cdfeTargetData, cdfeChaStats,
                                tempEntity,
                                targetGroup, ref dis);
                        }

                        Debug.Log($"自动索敌,targer:{target}");
                    }


                    Debug.Log($"自动索敌 targer {target.Index}");

                    if (target == Entity.Null)
                    {
                        Debug.Log($"自动索敌未索敌成功,索敌范围：{dis}米");
                        if (rangeType == 4)
                        {
                            loc.Position += dir * rangePar[3] / 1000f * num;
                        }
                        else if (rangeType == 1)
                        {
                            loc.Position += dir * rangePar[1] / 1000f;
                            //加个偏移
                            loc.Position += dir * rangePar[3] / 1000f;
                        }
                        else if (rangeType == 3)
                        {
                            var value = math.max(seekTypePar[0] / 2f, rangePar[0]);
                            loc.Position += dir * value / 1000f;
                        }
                        else if (rangeType == 2)
                        {
                            loc.Position += dir * rangePar[1] / 1000f;
                        }

                        targetPos = loc.Position;
                        return loc;
                    }


                    targetPos = cdfeLocalTransform[target].Position;
                    loc.Position = cdfeLocalTransform[locer].Position;

                    if (rangeType == 0 || rangeType == 3)
                    {
                        loc.Position = cdfeLocalTransform[target].Position;
                    }


                    if (deviateType == 1)
                    {
                        //TODO:
                        loc.Position = GetRandomPointInCircle(loc.Position, deviatePar[0] / 1000f, 0, 1);
                    }

                    var reductionV = targetPos - cdfeLocalTransform[locer].Position;
                    if (isLocatedFirst)
                    {
                        reductionV = targetPos - locatedPos;
                    }

                    if (seekTypePar.Length > 1)
                    {
                        if (num == 1 || seekTypePar[1] == 0)
                        {
                            if (reductionV.x == 0 && reductionV.y == 0 && reductionV.z == 0)
                            {
                                if (cdfeChaStats.HasComponent(locer))
                                {
                                    dir = math.normalizesafe(cdfeChaStats[locer].chaResource.direction);
                                }
                                else
                                {
                                    dir = math.normalizesafe(cdfeChaStats[caster].chaResource.direction);
                                }
                            }
                            else
                            {
                                dir = math.normalizesafe(reductionV);
                            }
                        }

                        if (num == 1 && seekTypePar[1] != 0 && deviateType != 11)
                        {
                            var cha = cdfeChaStats[caster];
                            cha.chaResource.direction = dir;
                            cdfeChaStats[caster] = cha;
                        }
                    }
                    else
                    {
                        if (reductionV.x == 0 && reductionV.y == 0 && reductionV.z == 0)
                        {
                            dir = math.normalizesafe(cdfeChaStats[caster].chaResource.direction);
                        }
                        else
                        {
                            dir = math.normalizesafe(reductionV);
                        }
                    }


                    if (rangeType == 1)
                    {
                        loc.Position += dir * rangePar[1] / 1000f / 2;
                        //加个偏移
                        loc.Position += dir * rangePar[3] / 1000f;
                    }
                    else if (rangeType == 4)
                    {
                        loc.Position += dir * rangePar[3] / 1000f * num;
                    }


                    //身前身后
                    if (deviateType == 3)
                    {
                        //身前
                        if (deviatePar[0] == 1)
                        {
                            targetPos -= dir * (cdfeLocalTransform[target].Scale * 0.5f +
                                                cdfeLocalTransform[caster].Scale * 0.5f);
                        }
                        else if (deviatePar[0] == 2)
                        {
                            targetPos += dir * (cdfeLocalTransform[target].Scale * 0.5f +
                                                cdfeLocalTransform[caster].Scale * 0.5f);
                        }
                    }


                    var qua = MathHelper.LookRotation2D(dir);
                    loc.Rotation = qua;
                    if (deviateType == 2)
                    {
                        loc = loc.RotateZ(-math.radians(deviatePar[0]));
                        dir = MathHelper.RotateVector(dir, -deviatePar[0]);
                    }

                    break;
                //定向索敌 caster的面向方向
                case 2:
                    qua = MathHelper.LookRotation2D(dir);
                    loc.Rotation = qua;
                    if (deviateType == 2)
                    {
                        loc = loc.RotateZ(-math.radians(deviatePar[0]));
                        dir = MathHelper.RotateVector(dir, deviatePar[0]);
                    }

                    if (rangeType == 4)
                    {
                        loc.Position += dir * rangePar[3] / 1000f * num;
                    }

                    else if (rangeType == 1)
                    {
                        loc.Position += dir * rangePar[1] / 1000f / 2;
                        //加个偏移
                        loc.Position += dir * rangePar[3] / 1000f;
                    }


                    targetPos = loc.Position;
                    Debug.Log($"dir333:{dir}");
                    break;

                case 3:
                    //非定向
                    loc.Rotation = MathHelper.LookRotation2D(math.up());
                    loc = loc.RotateZ(-math.radians(seekTypePar[0]));
                    dir = MathHelper.Forward(loc.Rotation);
                    if (rangeType == 4)
                    {
                        loc.Position += dir * rangePar[3] / 1000f * num;
                    }

                    else if (rangeType == 1)
                    {
                        loc.Position += dir * rangePar[1] / 1000f / 2;
                        //加个偏移
                        loc.Position += dir * rangePar[3] / 1000f;
                    }

                    targetPos = loc.Position;

                    //身前身后
                    if (deviateType == 3)
                    {
                        //身前
                        if (deviatePar[0] == 1)
                        {
                            targetPos -= dir * (cdfeLocalTransform[caster].Scale * 0.5f +
                                                cdfeLocalTransform[caster].Scale * 0.5f);
                        }
                        else if (deviatePar[0] == 2)
                        {
                            targetPos += dir * (cdfeLocalTransform[caster].Scale * 0.5f +
                                                cdfeLocalTransform[caster].Scale * 0.5f);
                        }
                    }

                    Debug.Log($"dir4444:{dir}");
                    break;

                //绝对坐标点
                case 4:
                    targetPos = new float3(seekTypePar[0], seekTypePar[1], 0);
                    dir = targetPos - cdfeLocalTransform[locer].Position;
                    if (dir.x != 0 && dir.y != 0 && dir.z != 0)
                    {
                        dir = math.normalizesafe(dir);
                    }

                    qua = MathHelper.LookRotation2D(dir);
                    loc.Rotation = qua;
                    loc.Position = targetPos;
                    if (rangeType != 0 || rangeType != 1)
                    {
                        loc.Position = cdfeLocalTransform[locer].Position;
                        if (rangeType == 4)
                        {
                            loc.Position += dir * rangePar[3] / 1000f * num;
                        }
                    }

                    if (deviateType == 1)
                    {
                        //TODO:
                        loc.Position = GetRandomPointInCircle(loc.Position, deviatePar[0] / 1000f, 0,
                            (uint)(tick * 10 + 1));
                    }

                    if (rangeType == 1)
                    {
                        loc.Position += dir * rangePar[1] / 1000f / 2;
                        //加个偏移
                        loc.Position += dir * rangePar[3] / 1000f;
                    }

                    Debug.Log($"dir555:{dir}");
                    break;
                //boss面向
                case 5:
                    if (cdfeTargetData.HasComponent(locer) &&
                        cdfeTargetData[locer].BelongsTo == (uint)BuffHelper.TargetEnum.BossMonster)
                    {
                        var tempdir = cdfeChaStats[locer].chaResource.direction;
                        if (tempdir.x > 0)
                        {
                            loc.Rotation = MathHelper.LookRotation2D(math.up());
                            loc = loc.RotateZ(-math.radians(90));
                            dir = MathHelper.Forward(loc.Rotation);
                        }
                        else
                        {
                            loc.Rotation = MathHelper.LookRotation2D(math.up());
                            loc = loc.RotateZ(-math.radians(270));
                            dir = MathHelper.Forward(loc.Rotation);
                        }

                        if (seekTypePar.Length > 0)
                        {
                            var deviatX = seekTypePar[0] / 1000f;
                            var deviatY = seekTypePar[1] / 1000f;
                            loc.Position += dir * deviatX;
                            loc.Position += new float3(0, 1, 0) * deviatY;
                        }


                        Debug.Log($"dir:{dir}");
                        if (deviateType == 2)
                        {
                            loc.Rotation = MathHelper.LookRotation2D(dir);
                            loc = loc.RotateZ(-math.radians(deviatePar[0]));
                            dir = MathHelper.Forward(loc.Rotation);
                        }

                        if (rangeType == 4)
                        {
                            loc.Position += dir * rangePar[3] / 1000f * num;
                        }

                        else if (rangeType == 1)
                        {
                            loc.Position += dir * rangePar[1] / 1000f / 2;
                            //加个偏移
                            loc.Position += dir * rangePar[3] / 1000f;
                        }
                    }
                    else if (cdfeTargetData.HasComponent(caster) &&
                             cdfeTargetData[caster].BelongsTo == (uint)BuffHelper.TargetEnum.BossMonster)
                    {
                        var tempdir = cdfeChaStats[caster].chaResource.direction;
                        if (tempdir.x > 0)
                        {
                            loc.Rotation = MathHelper.LookRotation2D(math.up());
                            loc = loc.RotateZ(-math.radians(90));
                            dir = MathHelper.Forward(loc.Rotation);
                        }
                        else
                        {
                            loc.Rotation = MathHelper.LookRotation2D(math.up());
                            loc = loc.RotateZ(-math.radians(270));
                            dir = MathHelper.Forward(loc.Rotation);
                        }

                        Debug.Log($"dir:{dir}");
                        if (deviateType == 2)
                        {
                            loc.Rotation = MathHelper.LookRotation2D(dir);
                            loc = loc.RotateZ(-math.radians(deviatePar[0]));
                            dir = MathHelper.Forward(loc.Rotation);
                        }

                        if (rangeType == 4)
                        {
                            loc.Position += dir * rangePar[3] / 1000f * num;
                        }

                        else if (rangeType == 1)
                        {
                            loc.Position += dir * rangePar[1] / 1000f / 2;
                            //加个偏移
                            loc.Position += dir * rangePar[3] / 1000f;
                        }
                    }


                    targetPos = loc.Position;
                    Debug.Log($"dir666:{dir}");
                    break;
            }

            //Debug.Log($"loc:{loc.Position}");
            return loc;
        }

        //public NativeList<LocalTransform> noDirctedSeekTargets()


        /// <summary>
        /// 以 targetPos为圆心 从半径min到max的圆环内随机选一点 rmin可为0
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="rMax"></param>
        /// <param name="rMin"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetRandomPointInCircle(float3 targetPos, float rMax, float rMin, uint seed)
        {
            var rand = Random.CreateFromIndex(seed);
            float randomR = rand.NextFloat(rMin, rMax);
            float randomAngle = math.radians(rand.NextFloat(0f, 360f)); // 生成随机角度
			
            float randomX = targetPos.x + randomR * math.sin(randomAngle); // 计算随机点的 x 坐标
            float randomY = targetPos.y + randomR * math.cos(randomAngle); // 计算随机点的 y 坐标
            //var minXL = -mapWidth / 2f + prefabSize / 2f + 10;
            //var minXR = mapWidth / 2f - prefabSize / 2f - 10;
            //var minYB = -mapWidth / 2f + prefabSize / 2f + 10;
            //var minYU = mapWidth / 2f - prefabSize / 2f - 10;
            //randomX = randomX < minXL ? minXL : randomX;
            //randomX = randomX > minXR ? minXR : randomX;
            //randomY = randomY < minYB ? minYB : randomY;
            //randomY = randomY > minYU ? minYU : randomY;
            return new float3(randomX, randomY, 0);
        }


        /// <summary>
        /// 方向上射线的随机一点
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="cdfeLocalTransform"></param>
        /// <param name="cdfeChaStats"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="dir">方向</param>
        /// <param name="seed"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetRandomPointOutsideLine(Entity caster,
            ComponentLookup<LocalTransform> cdfeLocalTransform,
            ComponentLookup<ChaStats> cdfeChaStats, float min, float max, float3 dir, uint seed)
        {
            var rand = Unity.Mathematics.Random.CreateFromIndex(seed);
            float randomDistance = rand.NextFloat(min, max + 0.1f);
            //IsPosCanUse
            return cdfeLocalTransform[caster].Position + dir * randomDistance;
        }

        /// <summary>
        /// 根据圆心坐标获取圆上对应角度的点
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="cdfeLocalTransform"></param>
        /// <param name="cdfeChaStats"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="dir">方向</param>
        /// <param name="seed"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetPointInCircleByRadius(float3 targetPos, float radius, int Angle)
        {
            float angleRad = math.radians(Angle);
			
            float X = targetPos.x + radius * math.sin(angleRad); // 计算随机点的 x 坐标
            float Y = targetPos.y + radius * math.cos(angleRad); // 计算随机点的 y 坐标
            return new float3(X, Y, 0);
        }


        //[BurstCompile]

        //public static IsContains(float3 )

        #region boss地图生成

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GenerateBossMap(NativeArray<Entity> obstacles,
            ComponentLookup<MapElementData> cdfeMapElementData, ComponentLookup<LocalTransform> cdfeLocalTransform,
            uint seed, ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix,
            ref EntityCommandBuffer.ParallelWriter ecb, int sortKey,
            GlobalConfigData configData,
            PrefabMapData prefabMapData, int bossID, int mapID, out Entity groupEntity, out Entity surfaceEntity,
            out float3 bossScenePos)
        {
            ref var bossTable = ref configData.value.Value.configTbmonsters.configTbmonsters;
            int bossMapID = -1;
            for (int i = 0; i < bossTable.Length; i++)
            {
                if (bossID == bossTable[i].id)
                {
                    bossMapID = bossTable[i].sceneBossId;
                    break;
                }
            }

            Debug.Log($"bossID{bossID}");
            if (bossMapID < 0)
            {
            }

            int index = 0;
            ref var sceneBossTable = ref configData.value.Value.configTbscene_bosss.configTbscene_bosss;
            for (int i = 0; i < sceneBossTable.Length; ++i)
            {
                if (bossMapID == sceneBossTable[i].id)
                {
                    index = i;
                    break;
                }
            }

            Debug.Log($"boss1111：{index}，scenebossid：{sceneBossTable[index]}");

            var edgeWidth = sceneBossTable[index].edgeWidth / 1000f;
            //boss地图的长宽
            var mapSize = (int)(sceneBossTable[index].areaSize[0] / 1000f);
            ref var eventGroup = ref sceneBossTable[index].eventGroup;
            //var finalSize = edgeWidth * 2 + mapSize;
            var moduleTemplateID = sceneBossTable[index].moduleTemplateId;
            var picBorder = prefabMapData.prefabHashMap[sceneBossTable[index].edgeModule[0]];

            ref var moduleTemplatePar = ref sceneBossTable[index].moduleTemplatePara;

            Debug.Log($"boss2222222：{index}，scenebossid：{sceneBossTable[index]}");
            ref var sceneMoudleTable = ref configData.value.Value.configTbscene_modules.configTbscene_modules;
            for (int i = 0; i < sceneMoudleTable.Length; ++i)
            {
                if (sceneMoudleTable[i].id == mapID)
                {
                    index = i;
                    break;
                }
            }

            Debug.Log($"boss33333333：{index}，mapID：{mapID}");
            //普通的地图的长宽
            var mapWidth = sceneMoudleTable[index].size[0] / 1000;
            var mapHeight = sceneMoudleTable[index].size[1] / 1000;
            var mapType = sceneMoudleTable[index].mapType;
            var size = sceneMoudleTable[index].size[0] > sceneMoudleTable[index].size[1]
                ? sceneMoudleTable[index].size[1] / 1000
                : sceneMoudleTable[index].size[0] / 1000;


            float3 pos = default;
            switch (mapType)
            {
                case 1:
                    pos = new float3(1999, 0, 0);
                    break;
                case 2:
                    pos = new float3(0, 1999, 0);
                    break;
                case 3:
                    pos = new float3(1999, 0, 0);

                    break;
                case 4:
                    //TODO:全开放的boss场景选点
                    pos = new float3(1999, 1999, 0);
                    break;
            }

            bossScenePos = new float3(mapSize, pos.xy);
            //上下无限
            if (mapType == 1)
            {
                GetNeedPrefab(mapID, out Entity mapPrefab, out NativeList<Entity> listLeft,
                    out NativeList<Entity> listRight, out NativeList<Entity> listBottom,
                    out NativeList<Entity> listTop, configData, prefabMapData);
                GetNeedPrefab(mapID, out mapPrefab, out NativeHashMap<int, bool> borderPrefabLeft,
                    out NativeHashMap<int, bool> borderPrefabRight,
                    out NativeHashMap<int, bool> borderPrefabTop,
                    out NativeHashMap<int, bool> borderPrefabBottom, configData, prefabMapData);

                for (int j = -1; j < 2; j++)
                {
                    //int mapIndex = j + 1;
                    //float3 startPos = new float3(-0.5f * mapWidth, mapheight * (j - 0.5f), 0);
                    for (int k = 0; k < mapHeight / mapWidth; k++)
                    {
                        var mapCenter = ecb.Instantiate(sortKey, mapPrefab);

                        var mapLeft = InstanceBorder(borderPrefabLeft, k, listLeft, ecb, sortKey);
                        var mapRight = InstanceBorder(borderPrefabRight, k, listRight, ecb, sortKey);
                        int ajustPar = k == 0 ? -1 : 1;
                        var scale = size / 10.24f * (mapSize / 120f);
                        var mapYpos = pos.y + ajustPar * mapSize / 2f + j * mapSize * (mapHeight / mapWidth);
                        ecb.SetComponent(sortKey, mapCenter,
                            new LocalTransform { Scale = scale, Position = new float3(pos.x, mapYpos, 0) });
                        var borderPos = AjustBorderPosAndScale(mapLeft, mapSize, mapYpos, pos.x, -1, scale);
                        ecb.AddComponent(sortKey, mapLeft, new BoardData { type = BoardTypeEnum.Left });
                        ecb.SetComponent(sortKey, mapLeft, new LocalTransform { Scale = scale, Position = borderPos });
                        borderPos = AjustBorderPosAndScale(mapRight, mapSize, mapYpos, pos.x, 1, scale);
                        ecb.AddComponent(sortKey, mapRight, new BoardData { type = BoardTypeEnum.Right });
                        ecb.SetComponent(sortKey, mapRight, new LocalTransform { Scale = scale, Position = borderPos });

                        if (k == 1 && j == 0)
                        {
                            //上下障碍物生成
                            var prefab = prefabMapData.prefabHashMap["pic_map_bg_boss_boardUB"];
                            var insUp = ecb.Instantiate(sortKey, prefab);
                            var upPos = new float3(pos.x, mapYpos + edgeWidth / 2f, 0);
                            ecb.SetComponent(sortKey, insUp, new LocalTransform { Position = upPos, Scale = mapSize });
                            AddDataToBorder(insUp, ecb, sortKey);
                            ecb.AddComponent(sortKey, insUp, new BoardData { type = BoardTypeEnum.Up });
                            var insBt = ecb.Instantiate(sortKey, prefab);
                            var bmPos = new float3(pos.x, mapYpos - mapSize - edgeWidth / 2f, 0);
                            ecb.SetComponent(sortKey, insBt, new LocalTransform { Position = bmPos, Scale = mapSize });
                            AddDataToBorder(insBt, ecb, sortKey);
                            ecb.AddComponent(sortKey, insBt, new BoardData { type = BoardTypeEnum.Down });

                            //生成障碍物
                            var startPosx = pos.x - mapSize / 2f + 15f;
                            var posY = mapYpos + 15f / 2;

                            for (int i = 0; i < 8; i++)
                            {
                                var posX = startPosx + i * 30f;
                                var ins = ecb.Instantiate(sortKey, picBorder);
                                ecb.SetComponent(sortKey, ins,
                                    new LocalTransform { Position = new float3(posX, posY, 0), Scale = 28f });
                            }

                            posY = mapYpos - mapSize - 15f / 2;
                            for (int i = 0; i < 8; i++)
                            {
                                var posX = startPosx + i * 30f;
                                var ins = ecb.Instantiate(sortKey, picBorder);
                                ecb.SetComponent(sortKey, ins,
                                    new LocalTransform { Position = new float3(posX, posY, 0), Scale = 28f });
                            }
                        }
                    }
                }
            }
            //全开放
            else if (mapType == 4)
            {
                var prefab = prefabMapData.prefabHashMap[sceneMoudleTable[index].model];
                SpawnABigMap(prefab, mapSize, pos.xy, sortKey, ecb, prefabMapData, edgeWidth, picBorder);
            }
            //全封闭
            else
            {
                GetNeedPrefab(mapID, out Entity mapPrefab, out NativeList<Entity> borderPrefabLeft,
                    out NativeList<Entity> borderPrefabRight, out NativeList<Entity> borderPrefabTop,
                    out NativeList<Entity> borderPrefabBottom, configData, prefabMapData);
                var borderLeft = GetRandomEntity(borderPrefabLeft);
                var borderRight = GetRandomEntity(borderPrefabRight);
                var borderTop = GetRandomEntity(borderPrefabTop);
                var borderBottom = GetRandomEntity(borderPrefabBottom);
                var newMap = ecb.Instantiate(sortKey, mapPrefab);
                borderLeft = ecb.Instantiate(sortKey, borderLeft);
                borderRight = ecb.Instantiate(sortKey, borderRight);
                borderBottom = ecb.Instantiate(sortKey, borderBottom);
                borderTop = ecb.Instantiate(sortKey, borderTop);


                AjustMapScaleAndPos(newMap, 0, mapSize, ecb, sortKey, pos);
                AjustMapScaleAndPos(borderLeft, -1, mapSize, ecb, sortKey, pos);
                AjustMapScaleAndPos(borderRight, 1, mapSize, ecb, sortKey, pos);
                AjustMapScaleAndPos(borderTop, 2, mapSize, ecb, sortKey, pos);
                AjustMapScaleAndPos(borderBottom, -2, mapSize, ecb, sortKey, pos);
                AddDataToBorder(borderLeft, ecb, sortKey);
                AddDataToBorder(borderRight, ecb, sortKey);
                AddDataToBorder(borderTop, ecb, sortKey);
                AddDataToBorder(borderBottom, ecb, sortKey);
                ecb.SetComponent(sortKey, borderLeft, new BoardData
                {
                    type = BoardTypeEnum.Left
                });
                ecb.SetComponent(sortKey, borderRight, new BoardData
                {
                    type = BoardTypeEnum.Right
                });
                ecb.SetComponent(sortKey, borderTop, new BoardData
                {
                    type = BoardTypeEnum.Up
                });
                ecb.SetComponent(sortKey, borderBottom, new BoardData { type = BoardTypeEnum.Down });
                Debug.Log($"boss99999：{index}，scenebossid：{sceneMoudleTable[index]}");
            }


            switch (mapType)
            {
                case 1:

                    break;
                case 2:

                case 3:
                    //scale = size / 20.48f * (finalSize / 120f);
                    break;
            }

            //var mapPrefab = prefabMapData.prefabHashMap[mapModuleName];
            //var mapEntity = ecb.Instantiate(sortKey, mapPrefab);

            // ecb.AddBuffer<GameEvent>(sortKey, mapEntity);


            //生成suface

            prefabMapData.prefabHashMap.TryGetValue((FixedString128Bytes)$"CrowdSurface", out var surfacePrefab);

            var surface = ecb.Instantiate(sortKey, surfacePrefab);
            int crowdSize = mapSize + 80;
            ecb.SetComponent(sortKey, surface, new LocalTransform
            {
                Position = new float3(pos.x - crowdSize / 2f, pos.y - crowdSize / 2f, 0),
                Scale = 1,
                Rotation = quaternion.identity
            });
            Debug.Log($"scalescalescale {size}");

            // ecb.SetComponent(sortKey, surface, new CrowdSurface
            // {
            //     Size = crowdSize,
            //     Width = (int)(crowdSize / 2f),
            //     Height = (int)(crowdSize / 2f),
            //     Density = new Density
            //     {
            //         Min = 0.7f,
            //         Max = 2f,
            //         Exponent = 0.3f
            //     },
            //     Slope = new Slope
            //     {
            //         Min = 0,
            //         Max = 1
            //     },
            //     Layers = NavigationLayers.Everything
            // });
            //var group = Entity.Null;
            var group = ecb.CreateEntity(sortKey);

            ecb.AppendToBuffer(sortKey, surface, new LinkedEntityGroup
            {
                Value = group
            });
            ecb.AddComponent(sortKey, group, LocalTransform.Identity);
            ecb.AddComponent(sortKey, group, new LocalToWorld());
            // ecb.AddComponent(sortKey, group, new CrowdGroup
            // {
            //     Surface = surface,
            //     Speed = new Speed
            //     {
            //         Min = 0.1f,
            //         Max = 20f
            //     },
            //     CostWeights = new CostWeights
            //     {
            //         Distance = 0.1f,
            //         Time = 0.1f,
            //         Discomfort = 0.8f,
            //         Normalize = true
            //     },
            //     GoalSource = CrowdGoalSource.AgentDestination,
            //     Grounded = true,
            //     MappingRadius = 5f
            // });
            groupEntity = group;
            surfaceEntity = surface;
            for (int i = 0; i < eventGroup.Length; i++)
            {
                //AddBossEventToMap(ref ecb, sortKey, configData, mapEntity, eventGroup[i]);
            }

            //GenerateBoard(finalSize, edgeWidth, pos, prefabMapData, ecb, sortKey, surface);
            //GenerateEdge(mapsize, edgeWidth, ecb, sortKey, prefabMapData, pos, areaID, configData, surface);


            Debug.Log($"bossrrrrrrrr：{index}，scenebossid：{sceneMoudleTable[index]}");
            GenerateMapElement(obstacles, cdfeMapElementData, cdfeLocalTransform, seed, cdfePostTransformMatrix,
                configData, moduleTemplateID, ref moduleTemplatePar, pos,
                prefabMapData,
                ecb, sortKey, surface, mapType, mapSize, mapSize, false, true, seed);

            return pos;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddDataToBorder(Entity entity, EntityCommandBuffer.ParallelWriter ecb, int sortKey)
        {
            ecb.AddComponent<ObstacleTag>(sortKey, entity);
            var elementData = new MapElementData { };
            ////初始化地形信息
            elementData.elementID = 2001;
            //给area添加基础信息
            ecb.SetComponent(sortKey, entity, elementData);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Entity GetRandomEntity(NativeList<Entity> entities)
        {
            var random = Random.CreateFromIndex((uint)(entities.Length + 10086));
            int randomIndex = random.NextInt(0, entities.Length);
            return entities[randomIndex];
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SpawnABigMap(Entity prefab, float mapSize, float2 pos, int sortKey,
            EntityCommandBuffer.ParallelWriter ecb, PrefabMapData prefabMapData, float edgeWidth, Entity picBorder)
        {
            int mapCount = 3;

            for (int i = 0; i < mapCount; i++)
            {
                for (int j = 0; j < mapCount; j++)
                {
                    float posmapX = i * mapSize - (mapCount / 2) * mapSize;
                    float posmapY = j * mapSize - (mapCount / 2) * mapSize;
                    var offset = new float2(posmapX, posmapY);
                    var newPos = new float2(offset + pos);

                    var insmap = ecb.Instantiate(sortKey, prefab);
                    var tranPrefab = new LocalTransform { Position = new float3(newPos, 0), Scale = mapSize / 10.24f };

                    ecb.SetComponent(sortKey, insmap, tranPrefab);
                    float mapYpos = newPos.y;
                    float mapXpos = newPos.x;

                    if (i == 1 && j == 2)
                    {
                        /////////////////////////////////////////////////
                        //////上下障碍物生成
                        /////////////////////////////////////////////////
                        var borderprefab = prefabMapData.prefabHashMap["pic_map_bg_boss_boardUB"];
                        var insUp = ecb.Instantiate(sortKey, borderprefab);
                        var upPos = new float3(pos.x, mapYpos - mapSize / 2f + edgeWidth / 2f, 0);
                        ecb.SetComponent(sortKey, insUp, new LocalTransform { Position = upPos, Scale = mapSize });
                        //AddDataToBorder(insUp, ecb, sortKey);
                        ecb.AddComponent(sortKey, insUp, new BoardData { type = BoardTypeEnum.Up });
                        var insBt = ecb.Instantiate(sortKey, borderprefab);
                        var bmPos = new float3(pos.x, mapYpos - mapSize - mapSize / 2f - edgeWidth / 2f, 0);
                        ecb.SetComponent(sortKey, insBt, new LocalTransform { Position = bmPos, Scale = mapSize });
                        //AddDataToBorder(insBt, ecb, sortKey);
                        ecb.AddComponent(sortKey, insBt, new BoardData { type = BoardTypeEnum.Down });

                        //生成障碍物
                        var startPosx = pos.x - mapSize / 2f + 15f;
                        var posY = mapYpos - mapSize / 2f + 15f / 2;

                        for (int k = 0; k < 8; k++)
                        {
                            var posX = startPosx + k * 30f;
                            var ins = ecb.Instantiate(sortKey, picBorder);
                            ecb.SetComponent(sortKey, ins,
                                new LocalTransform { Position = new float3(posX, posY, 0), Scale = 28f });
                        }

                        posY = mapYpos - mapSize - mapSize / 2f - 15f / 2;
                        for (int k = 0; k < 8; k++)
                        {
                            var posX = startPosx + k * 30f;
                            var ins = ecb.Instantiate(sortKey, picBorder);
                            ecb.SetComponent(sortKey, ins,
                                new LocalTransform { Position = new float3(posX, posY, 0), Scale = 28f });
                        }


                        //////////////////////////////////////////////////////////////////////////
                        ////左右障碍物生成
                        //////////////////////////////////////////////////////////////////////////
                        var border = prefabMapData.prefabHashMap["pic_map_bg_boss_boardLR"];
                        var insL = ecb.Instantiate(sortKey, border);
                        var leftPos = new float3(pos.x - mapSize / 2f - edgeWidth / 4f + 7, pos.y, 0);
                        ecb.SetComponent(sortKey, insL, new LocalTransform { Position = leftPos, Scale = mapSize });
                        //AddDataToBorder(insUp, ecb, sortKey);
                        ecb.AddComponent(sortKey, insL, new BoardData { type = BoardTypeEnum.Left });

                        var insR = ecb.Instantiate(sortKey, border);
                        var rightPos = new float3(pos.x + mapSize / 2f + edgeWidth / 4f - 7, pos.y, 0);
                        ecb.SetComponent(sortKey, insR, new LocalTransform { Position = rightPos, Scale = mapSize });
                        //AddDataToBorder(insBt, ecb, sortKey);
                        ecb.AddComponent(sortKey, insR, new BoardData { type = BoardTypeEnum.Right });

                        //生成障碍物
                        var startPosY = pos.y - mapSize / 2f + 15f;
                        var posx = mapXpos - mapSize / 2f - 15f / 2;

                        for (int k = 0; k < 8; k++)
                        {
                            posY = startPosY + k * 30f;
                            var ins = ecb.Instantiate(sortKey, picBorder);
                            var loc = new LocalTransform
                            {
                                Position = new float3(posx, posY, 0), Scale = 28f,
                                Rotation = quaternion.RotateZ(math.radians(90))
                            };
                            ecb.SetComponent(sortKey, ins, loc);
                        }

                        posx = mapXpos + mapSize / 2f + 15f / 2;
                        for (int k = 0; k < 8; k++)
                        {
                            posY = startPosY + k * 30f;
                            var ins = ecb.Instantiate(sortKey, picBorder);
                            var loc = new LocalTransform
                            {
                                Position = new float3(posx, posY, 0), Scale = 28f,
                                Rotation = quaternion.RotateZ(-math.radians(90))
                            };
                            ecb.SetComponent(sortKey, ins, loc);
                        }
                    }
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 AjustBorderPosAndScale(Entity border, int mapSize, float mapYpos, float x,
            int type, float scale)
        {
            if (type == -1)
            {
                return new float3(x - mapSize / 2f - scale * 5.12f / 2,
                    mapYpos, 0);
            }
            else
            {
                return new float3(x + mapSize / 2f + scale * 5.12f / 2,
                    mapYpos, 0);
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Entity InstanceBorder(NativeHashMap<int, bool> borderPrefab, int order,
            NativeList<Entity> listPrefab, EntityCommandBuffer.ParallelWriter ecb, int sortKey)
        {
            //Debug.LogError($"order:{order}");
            //foreach (var i in borderPrefab)
            //{
            //    //Debug.LogError($"entity:{i.Key},{i.Value}");
            //}

            var num = borderPrefab.Count;
            if (num <= 1)
            {
                order = 0;
                Debug.LogError("！！！！！！！！！！！！！！！！！！！配表错误！！！！！！！");
            }

            var prefab = listPrefab[order];
            var border = ecb.Instantiate(sortKey, prefab);
            if (borderPrefab[order])
            {
                ecb.AddComponent(sortKey, border, new JiYuFlip { value = { x = 1, y = 0 } });
            }

            ecb.AddComponent<ObstacleTag>(sortKey, border);
            var elementData = new MapElementData { };
            ////初始化地形信息
            elementData.elementID = 2001;
            //给area添加基础信息
            ecb.SetComponent(sortKey, border, elementData);
            return border;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AjustMapScaleAndPos(Entity entity, int borderPos, int mapSize,
            EntityCommandBuffer.ParallelWriter ecb, int sortKey, float3 pos)
        {
            float scale = mapSize / 20.48f;
            var temp = new LocalTransform { Scale = scale };

            float xPos = -mapSize / 2f - scale * 5.12f / 2;
            float yPos = mapSize / 2f + scale * 5.12f / 2;

            if (borderPos == 0)
            {
                temp.Position = pos;
            }
            //右
            else if (borderPos == 1)
            {
                temp.Position = new float3(pos.x - xPos, pos.y, 0);
            }
            //左
            else if (borderPos == -1)
            {
                temp.Position = new float3(pos.x + xPos, pos.y, 0);
            }
            else if (borderPos == 2)
            {
                temp.Position = new float3(pos.x, pos.y - yPos, 0);
            }
            else
            {
                temp.Position = new float3(pos.x, pos.y + yPos, 0);
            }

            ecb.SetComponent<LocalTransform>(sortKey, entity, temp);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetNeedPrefab(int mapID, out Entity mapPrefab, out NativeList<Entity> borderPrefabLeft,
            out NativeList<Entity> borderPrefabRight, out NativeList<Entity> borderPrefabBottom,
            out NativeList<Entity> borderPrefabTop, GlobalConfigData globalConfigData, PrefabMapData prefabMapData)
        {
            mapPrefab = default;
            borderPrefabLeft = new NativeList<Entity>(5, Allocator.Temp);
            borderPrefabRight = new NativeList<Entity>(5, Allocator.Temp);
            borderPrefabBottom = new NativeList<Entity>(5, Allocator.Temp);
            borderPrefabTop = new NativeList<Entity>(5, Allocator.Temp);
            FixedString32Bytes flipStr = $"_flip";
            ref var mapArray = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
            for (int i = 0; i < mapArray.Length; i++)
            {
                if (mapArray[i].id == mapID)
                {
                    mapPrefab = prefabMapData.prefabHashMap[mapArray[i].model];
                    for (int j = 0; j < mapArray[i].bgLeft.Length; j++)
                    {
                        if (mapArray[i].bgLeft[j].Contains(flipStr))
                        {
                            int index = mapArray[i].bgLeft[j].IndexOf(flipStr);
                            FixedString128Bytes newString = mapArray[i].bgLeft[j].Substring(0, index);
                            borderPrefabLeft.Add(prefabMapData.prefabHashMap[newString]);
                        }
                        else
                        {
                            borderPrefabLeft.Add(prefabMapData.prefabHashMap[mapArray[i].bgLeft[j]]);
                        }
                    }

                    for (int j = 0; j < mapArray[i].bgRight.Length; j++)
                    {
                        if (mapArray[i].bgRight[j].Contains(flipStr))
                        {
                            int index = mapArray[i].bgRight[j].IndexOf(flipStr);
                            FixedString128Bytes newString = mapArray[i].bgRight[j].Substring(0, index);
                            borderPrefabRight.Add(prefabMapData.prefabHashMap[newString]);
                        }
                        else
                        {
                            borderPrefabRight.Add(prefabMapData.prefabHashMap[mapArray[i].bgRight[j]]);
                        }
                    }

                    for (int j = 0; j < mapArray[i].bgDown.Length; j++)
                    {
                        borderPrefabBottom.Add(prefabMapData.prefabHashMap[mapArray[i].bgDown[j]]);
                    }

                    for (int j = 0; j < mapArray[i].bgUp.Length; j++)
                    {
                        borderPrefabTop.Add(prefabMapData.prefabHashMap[mapArray[i].bgUp[j]]);
                    }

                    break;
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetNeedPrefab(int mapID, out Entity mapPrefab,
            out NativeHashMap<int, bool> borderPrefabLeft,
            out NativeHashMap<int, bool> borderPrefabRight, out NativeHashMap<int, bool> borderPrefabBottom,
            out NativeHashMap<int, bool> borderPrefabTop, GlobalConfigData globalConfigData,
            PrefabMapData prefabMapData)
        {
            mapPrefab = default;
            ref var mapArray = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
            borderPrefabLeft = default;
            borderPrefabRight = default;
            borderPrefabBottom = default;
            borderPrefabTop = default;
            for (int i = 0; i < mapArray.Length; i++)
            {
                if (mapArray[i].id == mapID)
                {
                    borderPrefabLeft = new NativeHashMap<int, bool>(mapArray[i].bgLeft.Length, Allocator.Temp);
                    borderPrefabRight = new NativeHashMap<int, bool>(mapArray[i].bgRight.Length, Allocator.Temp);
                    borderPrefabBottom = new NativeHashMap<int, bool>(mapArray[i].bgDown.Length, Allocator.Temp);
                    borderPrefabTop = new NativeHashMap<int, bool>(mapArray[i].bgUp.Length, Allocator.Temp);

                    FixedString32Bytes flipStr = $"_flip";
                    mapPrefab = prefabMapData.prefabHashMap[mapArray[i].model];
                    for (int j = 0; j < mapArray[i].bgLeft.Length; j++)
                    {
                        if (mapArray[i].bgLeft[j].Contains(flipStr))
                        {
                            borderPrefabLeft.Add(j, true);
                            //Debug.LogError($"1:{newString},PREFAB{prefabMapData.prefabHashMap[newString].Index}");
                        }
                        else
                        {
                            borderPrefabLeft.Add(j, false);
                            //Debug.LogError($"2:{mapArray[i].bgLeft[j]},PREFAB{prefabMapData.prefabHashMap[mapArray[i].bgLeft[j]].Index}");
                        }
                    }

                    for (int j = 0; j < mapArray[i].bgRight.Length; j++)
                    {
                        if (mapArray[i].bgRight[j].Contains(flipStr))
                        {
                            borderPrefabRight.Add(j, true);
                            //Debug.LogError($"1:{newString}");
                        }
                        else
                        {
                            borderPrefabRight.Add(j, false);
                            //Debug.LogError($"2:{mapArray[i].bgRight[j]}");
                        }
                    }

                    for (int j = 0; j < mapArray[i].bgDown.Length; j++)
                    {
                        if (mapArray[i].bgDown[j].Contains(flipStr))
                        {
                            borderPrefabBottom.Add(j, true);
                        }
                        else
                        {
                            borderPrefabBottom.Add(j, false);
                        }
                    }

                    for (int j = 0; j < mapArray[i].bgUp.Length; j++)
                    {
                        if (mapArray[i].bgUp[j].Contains(flipStr))
                        {
                            borderPrefabTop.Add(j, true);
                        }
                        else
                        {
                            borderPrefabTop.Add(j, false);
                        }
                    }

                    break;
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GenerateBoard(float finalSize, float edgeWidth, float3 pos, PrefabMapData prefabMapData,
            EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity linkedEntity)
        {
            //FixedString128Bytes str = $"pic_map_bg_boss_boardLR";
            var mapPrefab = prefabMapData.prefabHashMap[$"pic_map_bg_boss_boardLR"];
            var mapEntityLeft = ecb.Instantiate(sortKey, mapPrefab);
            LocalTransform loc = new LocalTransform();

            var mapWidth = finalSize - edgeWidth;
            loc.Position = new float3(pos.x - mapWidth / 2f - edgeWidth / 2f, pos.y, 0);
            loc.Scale = finalSize;
            ecb.SetComponent(sortKey, mapEntityLeft, loc);

            var mapEntityRight = ecb.Instantiate(sortKey, mapPrefab);

            loc.Position = new float3(pos.x + mapWidth / 2f + edgeWidth / 2f, pos.y, 0);
            ecb.SetComponent(sortKey, mapEntityRight, loc);


            mapPrefab = prefabMapData.prefabHashMap[$"pic_map_bg_boss_boardUB"];
            var mapEntityUp = ecb.Instantiate(sortKey, mapPrefab);

            loc.Position = new float3(pos.x, pos.y + mapWidth / 2f + edgeWidth / 2f, 0);
            ecb.SetComponent(sortKey, mapEntityUp, loc);


            var mapEntityBottom = ecb.Instantiate(sortKey, mapPrefab);

            loc.Position = new float3(pos.x, pos.y - mapWidth / 2f - edgeWidth / 2f, 0);
            ecb.SetComponent(sortKey, mapEntityBottom, loc);

            ecb.AppendToBuffer(sortKey, linkedEntity, new LinkedEntityGroup
            {
                Value = mapEntityLeft
            });
            ecb.AppendToBuffer(sortKey, linkedEntity, new LinkedEntityGroup
            {
                Value = mapEntityRight
            });
            ecb.AppendToBuffer(sortKey, linkedEntity, new LinkedEntityGroup
            {
                Value = mapEntityUp
            });
            ecb.AppendToBuffer(sortKey, linkedEntity, new LinkedEntityGroup
            {
                Value = mapEntityBottom
            });
            ecb.SetComponent(sortKey, mapEntityLeft, new BoardData
            {
                type = BoardTypeEnum.Left
            });

            ecb.SetComponent(sortKey, mapEntityRight, new BoardData
            {
                type = BoardTypeEnum.Right
            });
            ecb.SetComponent(sortKey, mapEntityUp, new BoardData
            {
                type = BoardTypeEnum.Up
            });
            ecb.SetComponent(sortKey, mapEntityBottom, new BoardData
            {
                type = BoardTypeEnum.Down
            });
        }

        #region 生成障碍物地形

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapsize"></param>
        /// <param name="configData"></param>
        /// <param name="moduleTemplateID">sceneboss表对应的组件模板id</param>
        /// <param name="moduleTemplatePar">sceneboss表对应的组件模板参数</param>
        /// <param name="pos"></param>
        /// <param name="prefabMapData"></param>
        /// <param name="ecb"></param>
        /// <param name="sortKey"></param>
        /// <param name="linkedEntity"></param>
        /// <param name="mapType"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GenerateMapElement(NativeArray<Entity> mapModels,
            ComponentLookup<MapElementData> cdfeMapElementData, ComponentLookup<LocalTransform> cdfeLocalTransform,
            uint seed, ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix,
            GlobalConfigData configData, int moduleTemplateID,
            ref BlobArray<int> moduleTemplatePar, float3 pos, PrefabMapData prefabMapData,
            EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity linkedEntity, int mapType, float mapWidth,
            float mapHeight, bool isEvent = false, bool isBossMap = true, uint seed1 = 1)
        {
            var startPos = new float3(pos.x - mapWidth / 2f, pos.y + mapHeight / 2f, 0);
            ref var moduleTemplate =
                ref configData.value.Value.configTbmodule_templates.configTbmodule_templates;
            ref var moduleRefreshConfig =
                ref configData.value.Value.configTbmodule_refreshs.configTbmodule_refreshs;

            GetSceneModels(out NativeList<int> moduleIds, out NativeList<int> refreshIds,
                out NativeList<Entity> modules, ref moduleTemplatePar, moduleTemplateID, configData, prefabMapData,
                isEvent, seed1);
            int numCount = 0;
            int maxRectCount = 0;

            for (int i = 0; i < refreshIds.Length; i++)
            {
                for (int j = 0; j < moduleRefreshConfig.Length; j++)
                {
                    if (moduleRefreshConfig[j].id == refreshIds[i])
                    {
                        if (moduleRefreshConfig[j].selfYn == 0)
                        {
                            maxRectCount += moduleRefreshConfig[j].num;
                        }
                    }
                }
            }

            maxRectCount = maxRectCount > 0 ? maxRectCount : MaxSelfRect;
            //Debug.LogError($"maxRectCount:{maxRectCount}");
            NativeList<Rect> outRects = new NativeList<Rect>(maxRectCount, Allocator.Temp);
            for (int i = 0; i < moduleTemplate.Length; i++)
            {
                if (moduleTemplate[i].id == moduleTemplateID)
                {
                    numCount += moduleTemplate[i].num;
                    int startIndex = numCount - moduleTemplate[i].num;
                    if (moduleTemplate[i].time == 0)
                    {
                        for (int j = startIndex; j < numCount; j++)
                        {
                            NativeList<float2> points = SelectPoints(startPos, ref outRects, false, refreshIds[j], seed,
                                configData, mapWidth, mapHeight, mapModels, cdfeMapElementData, cdfeLocalTransform);
                            InitModuleEntities(cdfePostTransformMatrix, points, modules[j], moduleIds[j], configData,
                                ecb, sortKey,
                                linkedEntity, prefabMapData, mapType, pos, mapWidth, mapHeight, isBossMap);
                            Debug.Log($"j:{moduleIds[j]}");
                        }
                    }
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitModuleEntities(ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix,
            NativeList<float2> points, Entity prefab, int moduleId,
            GlobalConfigData globalConfigData, EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity linkedEntity,
            PrefabMapData prefabMapData, int mapType, float3 mappos, float mapWidth, float mapHeight, bool isBossMap)
        {
            ref var sceneModule = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
            //ref var paraPos = ref configTbmodule_template.paraPos;

            int num = points.Length;
            for (int i = 0; i < num; i++)
            {
                var pos = points[i];
                Debug.Log($"pos:{pos}");
                Entity ins = default;
                GenerateBossMapModule(cdfePostTransformMatrix, ref ecb, globalConfigData, prefabMapData, sortKey,
                    moduleId,
                    new float3(pos.xy, 0),
                    ref ins, mapType, mappos, mapWidth, mapHeight);
                //if(isBossMap)
                //{
                //    ecb.AppendToBuffer(sortKey, linkedEntity, new LinkedEntityGroup
                //    {
                //        Value = ins
                //    });
                //}
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GenerateBossMapModule(ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix,
            ref EntityCommandBuffer.ParallelWriter ecb,
            GlobalConfigData configData,
            PrefabMapData prefabMapData, int sortKey, int itemID, float3 pos, ref Entity ins, int mapType,
            float3 mappos, float mapWidth, float mapHeight)
        {
            float scaleMulti = 1;

            int index = -1;
            ref var mapModelTable = ref configData.value.Value.configTbscene_modules.configTbscene_modules;
            ref var elementConfig = ref configData.value.Value.configTbelement_effects.configTbelement_effects;
            for (int i = 0; i < mapModelTable.Length; i++)
            {
                if (mapModelTable[i].id == itemID)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0) return;

            ref var mapModel = ref mapModelTable[index];

            var pathPriority = mapModel.pathPriority;
            var propName = mapModel.model;
            FixedString32Bytes flipStr = $"_flip";
            if (propName.Contains(flipStr))
            {
                int indexName = propName.IndexOf(flipStr);
                propName = propName.Substring(0, indexName);

                Debug.LogError($"propName:{propName}");
            }

            var propPrefab = prefabMapData.prefabHashMap[propName];

            ins = ecb.Instantiate(sortKey, propPrefab);
            var size = mapModel.size[0] > mapModel.size[1]
                ? mapModel.size[0] / 1000f
                : mapModel.size[1] / 1000f;
            var elementId = mapModel.elementId;
            var displayOrder = mapModel.displayOrder;
            //    Debug.Log($"Onhit 障碍物触发pos:{pos},id:{itemID},name:{propName}");
            //TODO: 在地图内部 不可在地图外部生成
            int gapSize = 5;
            if (mapType == 1 || mapType == 3)
            {
                //上下无限
                if (pos.x > (mappos.x + mapWidth / 2f - size / 2f))
                {
                    pos.x = mappos.x + mapWidth / 2f - size / 2f - gapSize;
                }
                else if (pos.x < (mappos.x + (-mapWidth) / 2f + size / 2f))
                {
                    Debug.Log($"未生效:{pos.x}");
                    pos.x = mappos.x + (-mapWidth) / 2f + size / 2f + gapSize;
                    Debug.Log($"生效:{pos.x}");
                }
            }

            if (mapType == 3)
            {
                //全封闭
                if (pos.y > (mappos.y + mapHeight / 2f - size / 2f))
                {
                    pos.y = mappos.y + mapHeight / 2f - size / 2f - gapSize;
                }
                else if (pos.y < (mappos.y - mapHeight / 2f + size / 2f))
                {
                    pos.y = mappos.y - mapHeight / 2f + size / 2f + gapSize;
                }
            }

            var newloc = new LocalTransform { Scale = size, Position = pos, Rotation = quaternion.identity };
            if (cdfePostTransformMatrix.HasComponent(propPrefab))
            {
                var postTransformMatrix = cdfePostTransformMatrix[propPrefab];
                var scaleprefab = postTransformMatrix.Value.DecomposeScale();
                scaleMulti *= scaleprefab.x;
                ecb.SetComponent(sortKey, ins, new PostTransformMatrix
                {
                    Value = float4x4.Scale(size * scaleMulti, size * scaleMulti, 1)
                });
            }
            else
            {
                newloc.Scale *= scaleMulti;
            }

            if (mapModel.model.Contains(flipStr))
            {
                Debug.Log("Contains");
                newloc.Rotation = quaternion.AxisAngle(math.up(), math.radians(180));
            }

            ecb.SetComponent(sortKey, ins,
                new MapElementData { elementID = itemID });
            if (elementId != 0)
            {
                ecb.AddComponent(sortKey, ins, new ElementData
                {
                    type = 0,
                    id = elementId
                });
            }

            if (mapModel.type == 1)
            {
                ecb.SetComponent(sortKey, ins,
                    new JiYuSort { value = new int2((int)JiYuLayer.Area, math.clamp(displayOrder, 0, 10)) });

                ecb.SetComponent(sortKey, ins,
                    new TargetData
                    {
                        BelongsTo = (uint)BuffHelper.TargetEnum.Area,
                        AttackWith = 1 + 2 + 4 + 8 + 16 + 32 + 128
                    });
                if (pathPriority >= 25)
                {
                    ecb.AddComponent(sortKey, ins, new AgentShape
                    {
                        Radius = size / 2f,
                        Height = 0,
                        Type = ShapeType.Circle
                    });
                    ecb.AddComponent(sortKey, ins, new Agent
                    {
                        Layers = NavigationLayers.Default
                    });
                    ecb.AddComponent(sortKey, ins, new AgentBody
                    {
                        Force = default,
                        Velocity = default,
                        Destination = default,
                        RemainingDistance = 0,
                        IsStopped = true
                    });
                }

                //给火地形加临时生命值
                if (elementId == 201)
                {
                    int maxHp = 0;
                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].from == 201 && elementConfig[i].target == 102)
                        {
                            maxHp = elementConfig[i].para[0];
                            break;
                        }
                    }

                    ecb.AddComponent(sortKey, ins, new AreaTempHp { maxHp = maxHp });
                    ecb.AddComponent(sortKey, ins, new TimeToDieData { duration = mapModel.duration / 1000f });
                }

                ecb.AddComponent(sortKey, ins, new SkillAttackData
                {
                    data = new SkillAttack_9999
                    {
                        duration = 9999f,
                        isBullet = true,
                        hp = MathHelper.MaxNum,
                        isOnStayTrigger = true,
                        onStayTriggerCd = 0.5f,
                    }.ToSkillAttack()
                });
            }
            else
            {
                newloc.Scale = 0.001f;
                ecb.SetComponent(sortKey, ins, new PushColliderData
                {
                    targetScale = size
                });
                ecb.AddComponent<ObstacleTag>(sortKey, ins);

                int hp = mapModel.hp;
                var chaStats = new ChaStats() { };
                if (hp > 0)
                {
                    chaStats.chaProperty.maxHp = hp;
                    chaStats.chaResource.hp = chaStats.chaProperty.maxHp;
                    //ecbs.AddBuffer<DamageInfo>(entity);
                    ecb.AddBuffer<Buff>(sortKey, ins);
                }

                chaStats.chaProperty.mass = mapModel.mass;
                ecb.AddComponent(sortKey, ins, chaStats);
                ecb.AddComponent(sortKey, ins, new AgentShape
                {
                    Radius = size / 2f,
                    Height = 0,
                    Type = ShapeType.Circle
                });
                ecb.AddComponent(sortKey, ins, new Agent
                {
                    Layers = NavigationLayers.Default
                });
                ecb.AddComponent(sortKey, ins, new AgentBody
                {
                    Force = default,
                    Velocity = default,
                    Destination = default,
                    RemainingDistance = 0,
                    IsStopped = true
                });
            }

            ecb.SetComponent(sortKey, ins, newloc);
        }


        //public static void AddDataToElement(Entity entity, int mapModuleID, GlobalConfigData globalConfigData,
        //    EntityCommandBuffer.ParallelWriter ecb, int sortKey)
        //{
        //    //Debug.LogError("AddDataToElement");

        //    ref var elementArray = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
        //    int hp = 0, type = 0, pathPriority = 0, width = 0, display_order = 0, elementId = 0;
        //    ref var eventsTable = ref globalConfigData.value.Value.configTbevent_0s.configTbevent_0s;
        //    for (int i = 0; i < elementArray.Length; i++)
        //    {
        //        if (mapModuleID == elementArray[i].id)
        //        {
        //            hp = elementArray[i].hp;
        //            type = elementArray[i].type;
        //            pathPriority = elementArray[i].pathPriority;
        //            width = elementArray[i].size[0] / 1000;
        //            display_order = elementArray[i].displayOrder;


        //            //restitutionRatios = elementArray[i].impactSpeedRatio;
        //            elementId = elementArray[i].elementId;
        //            //TODO：Add SKill

        //            // ref var events = ref elementArray[i].event0;
        //            // if (events.Length <= 0) break;
        //            // ecb.AddBuffer<GameEvent>(sortKey, entity);
        //            // for (int j = 0; j < events.Length; j++)
        //            // {
        //            //     int eventID = events[j];
        //            //     //AddEventTo(eventID, entity, globalConfigData);
        //            // }

        //            break;
        //        }
        //    }


        //    if (type == 2)
        //    {
        //        //TODO:
        //        // unsafe
        //        // {
        //        //     var colliderPtr = cdfePhysicsCollider[entity].ColliderPtr;
        //        //
        //        //     colliderPtr->SetRestitution(restitutionRatios / 10000f);
        //        // }

        //        //给碰撞体增加ChaStats组件 和可承受伤害组件
        //        ecb.AddComponent<ObstacleTag>(sortKey, entity);
        //        if (hp > 0)
        //        {
        //            ecb.AddComponent(sortKey, entity, new ChaStats
        //            {
        //                chaProperty = new ChaProperty
        //                {
        //                    maxHp = hp,
        //                    hpRatios = 0,
        //                    hpAdd = 0,
        //                    hpRecovery = 0,
        //                    hpRecoveryRatios = 0,
        //                    hpRecoveryAdd = 0,
        //                    atk = 0,
        //                    atkRatios = 0,
        //                    atkAdd = 0,
        //                    rebirthCount = 0,
        //                    critical = 0,
        //                    criticalDamageRatios = 0,
        //                    damageRatios = 0,
        //                    damageAdd = 0,
        //                    reduceHurtRatios = 0,
        //                    reduceHurtAdd = 0,
        //                    maxMoveSpeed = 0,
        //                    maxMoveSpeedRatios = 0,
        //                    speedRecoveryTime = 0,
        //                    mass = 0,
        //                    massRatios = 0,
        //                    pushForce = 0,
        //                    pushForceRatios = 0,
        //                    reduceHitBackRatios = 0,
        //                    dodge = 0,
        //                    shieldCount = 0,
        //                    defaultcoolDown = 0,
        //                },
        //                enviromentProperty = default,
        //                chaResource = new ChaResource
        //                {
        //                    curPushForce = 0,
        //                    curMoveSpeed = 0,
        //                    direction = default,
        //                    continuousCollCount = 0,
        //                    actionSpeed = 0,
        //                    hp = hp,
        //                    env = default
        //                },
        //                chaControlState = default
        //            });
        //            //ecbs.AddBuffer<DamageInfo>(entity);
        //            ecb.AddBuffer<Buff>(sortKey, entity);
        //        }

        //        ecb.AddComponent(sortKey, entity, new Agent { Layers = NavigationLayers.Default });

        //        ecb.AddComponent(sortKey, entity,
        //            new AgentShape { Radius = width / 2f, Height = 0, Type = ShapeType.Circle });
        //        ecb.AddComponent(sortKey, entity,
        //            new AgentBody
        //            {
        //                Force = default, Velocity = default, Destination = default, RemainingDistance = 0,
        //                IsStopped = true
        //            });
        //        if (elementId != 0)
        //        {
        //            ecb.AddComponent(sortKey, entity, new ElementData { type = type, id = elementId });
        //        }
        //    }
        //    else if (type == 1)
        //    {
        //        ecb.SetComponent(sortKey, entity,
        //            new TargetData
        //            {
        //                BelongsTo = (uint)BuffHelper.TargetEnum.Area,
        //                AttackWith = 1 + 2 + 4 + 8 + 16 + 32 + 128
        //            });
        //        ecb.AddComponent<AreaTag>(sortKey, entity);
        //        ecb.SetComponent(sortKey, entity,
        //            new JiYuSort { value = new int2((int)JiYuLayer.Area, math.clamp(display_order, 0, 10)) });
        //        if (pathPriority >= 25)
        //        {
        //            ecb.AddComponent(sortKey, entity, new AgentShape
        //            {
        //                Radius = width / 2f,
        //                Height = 0,
        //                Type = ShapeType.Circle
        //            });
        //            ecb.AddComponent(sortKey, entity, new Agent { Layers = NavigationLayers.Default });
        //            ecb.AddComponent(sortKey, entity,
        //                new AgentBody
        //                {
        //                    Force = default, Velocity = default, Destination = default, RemainingDistance = 0,
        //                    IsStopped = true
        //                });
        //            if (elementId != 0)
        //            {
        //                ecb.AddComponent(sortKey, entity, new ElementData { type = 0, id = elementId });
        //            }
        //        }
        //    }

        //    //ecbs.AddComponent<ObstacleTag>(entity);
        //}
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NativeList<float2> SelectPoints(float3 mapstartPos, ref NativeList<Rect> outRects,
            bool isNeedCheck,
            int refreshID, uint seedtemp, GlobalConfigData globalConfigData, float mapwidth, float mapHeight,
            NativeArray<Entity> mapModels, ComponentLookup<MapElementData> cdfeMapElementData,
            ComponentLookup<LocalTransform> cdfeLocalTransform)
        {
            ref var sceneModuleConfig =
                ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
            ref var moduleRefreshConfig =
                ref globalConfigData.value.Value.configTbmodule_refreshs.configTbmodule_refreshs;
            int index = 0;
            for (int i = 0; i < moduleRefreshConfig.Length; i++)
            {
                if (moduleRefreshConfig[i].id == refreshID)
                {
                    index = i;
                    break;
                }
            }

            ref var moduleRefresh = ref moduleRefreshConfig[index];
            var rationsStx = moduleRefresh.coordinate[0].x / 10000f;
            var rationsSty = moduleRefresh.coordinate[0].y / 10000f;
            var rationsEndx = moduleRefresh.coordinate[1].x / 10000f;
            var rationsEndy = moduleRefresh.coordinate[1].y / 10000f;
            int moduleID = moduleRefresh.sceneModule;
            float2 rectSize = default;
            for (int j = 0; j < sceneModuleConfig.Length; j++)
            {
                if (sceneModuleConfig[j].id == moduleID)
                {
                    rectSize = sceneModuleConfig[j].size.Length > 1
                        ? new float2(sceneModuleConfig[j].size[0] / 1000f, sceneModuleConfig[j].size[1] / 1000f)
                        : default;
                    break;
                }
            }

            float2 startPos = new(rationsStx * mapwidth, rationsSty * mapHeight);
            float2 endPos = new(rationsEndx * mapwidth, rationsEndy * mapHeight);
            //坐标转换为实际坐标
            float2 startPosT = new float2(mapstartPos.x + startPos.x, mapstartPos.y - startPos.y);
            float2 endPosT = new float2(mapstartPos.x + endPos.x, mapstartPos.y - endPos.y);

            Debug.Log($"startT:{startPosT},endPost:{endPosT}");
            bool isRandGenerate = moduleRefresh.randType == 1 ? true : false;
            //生成数量
            int count = moduleRefresh.num;
            NativeList<float2> Positionlist = new NativeList<float2>(count, Allocator.Temp);

            int gap = moduleRefresh.pointRange / 1000;
            if (isRandGenerate)
            {
                for (int k = 0; k < count; ++k)
                {
                    int times = 0;

                    Rect itemRect;
                    float3 randomPos = float3.zero;
                    do
                    {
                        uint seed = (uint)(k + seedtemp * 3 + times * 11);
                        Debug.Log($"seed:{seed}");
                        var random = Unity.Mathematics.Random.CreateFromIndex(seed);
                        var randPos = random.NextFloat2(startPosT, endPosT);

                        if (float.IsNaN(randPos.x) || float.IsNaN(randPos.y))
                        {
                            randPos = float2.zero;
                        }

                        times++;
                        randomPos = new float3(randPos.xy, 0);

                        itemRect = new Rect(randPos.x - gap, randPos.y - gap,
                            rectSize.x / 1000f + gap * 2,
                            rectSize.y / 1000f + gap * 2);

                        if (IsMaxLimitLoop(times))
                        {
                            Debug.LogError(
                                $"生成规则有误 选点失败,已选点个数{k + 1}个,未选点个数{count - k - 1}个,错误刷新组id为{moduleRefresh.id},刷新组规则id为{moduleRefresh.group}");

                            break;
                        }
                    } while (ItemOverlap(itemRect, outRects) || !IsPosCanUse(randomPos, globalConfigData, mapModels,
                                 cdfeMapElementData, cdfeLocalTransform));

                    outRects.Add(itemRect);
                    Positionlist.Add(new float2(itemRect.x + gap, itemRect.y - gap));
                    Debug.Log($"x:{itemRect.x + gap},y:{itemRect.y - gap}");
                }
            }
            else
            {
                Positionlist = SelectPointsRandomSequential(ref outRects, rectSize, startPosT, endPosT, count, gap);
            }


            if (moduleRefresh.selfYn == 1 ? true : false)
            {
                outRects.Clear();
            }


            return Positionlist;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsMaxLimitLoop(int times)
        {
            return times > 1000 ? true : false;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFloat3Equal(float3 a, float3 b)
        {
            var bool3 = math.abs(a - b) < math.EPSILON;
            if (bool3.x && bool3.y && bool3.z)
            {
                return true;
            }

            return false;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ItemOverlap(Rect judgeRect, NativeList<Rect> itemRects1)
        {
            foreach (Rect rect in itemRects1)
            {
                if (judgeRect.Overlaps(rect))
                {
                    return true;
                }
            }

            return false;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NativeList<float2> SelectPointsRandomSequential(ref NativeList<Rect> outRects,
            float2 size, float2 startPosT, float2 endPosT, int count, int gap)
        {
            NativeList<float2> result = new NativeList<float2>(count, Allocator.Temp);
            //float2 targetPos = new float2(startPosT.x, startPosT.y);

            int i = 0;
            uint seed = (uint)(startPosT.y);
            //Debug.LogError($"seec:{seed}");
            var random = Unity.Mathematics.Random.CreateFromIndex(seed);
            var targetPos =
                random.NextFloat2(new float2(startPosT.x, startPosT.y), new float2(endPosT.x, endPosT.y));
            while (i < count)
            {
                if (result.Length != 0)
                {
                    targetPos = result[^1];
                }

                var judgePos = targetPos.x + 2 * gap + size.x * 0.5f + size.x * 0.5f;
                //Debug.LogError($"judgePos:{judgePos}");
                if (judgePos > endPosT.x)
                {
                    //Debug.LogError($"换行:{endPosT.x}");
                    targetPos = new float2(startPosT.x, targetPos.y - 2 * gap - size.y);
                }
                else
                {
                    targetPos = new float2(judgePos, targetPos.y);
                }

                var rect = new Rect(targetPos.x - gap, targetPos.y - gap,
                    size.x + gap * 2,
                    size.y + gap * 2);
                outRects.Add(rect);
                result.Add(targetPos);
                ++i;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleIds"></param>
        /// <param name="refreshIds"></param>
        /// <param name="modules"></param>
        /// <param name="moduleTemplatePar">sceneboss表对应的组件模板参数</param>
        /// <param name="templateId">sceneboss表对应的组件模板id</param>
        /// <param name="globalConfigData"></param>
        /// <param name="prefabMapData"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetSceneModels(out NativeList<int> moduleIds, out NativeList<int> refreshIds,
            out NativeList<Entity> modules, ref BlobArray<int> moduleTemplatePar, int templateId,
            GlobalConfigData globalConfigData, PrefabMapData prefabMapData, bool isEvent, uint seed1)
        {
            modules = new NativeList<Entity>(Allocator.Temp);
            refreshIds = new NativeList<int>(Allocator.Temp);
            moduleIds = new NativeList<int>(Allocator.Temp);

            ref var sceneConfig = ref globalConfigData.value.Value.configTbscenes.configTbscenes;
            ref var moduleTemplateConfig =
                ref globalConfigData.value.Value.configTbmodule_templates.configTbmodule_templates;
            ref var sceneModuleConfig =
                ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
            ref var moduleRefreshConfig =
                ref globalConfigData.value.Value.configTbmodule_refreshs.configTbmodule_refreshs;
            for (int i = 0; i < moduleTemplateConfig.Length; i++)
            {
                ///sceneboss表对应的组件模板id等于module_template表的id
                if (moduleTemplateConfig[i].id == templateId)
                {
                    //根据索引，取sceneboss表对应的组件模板参数里的id 该id对应的是module_refresh表的组id
                    var moduleGroupID = moduleTemplatePar[moduleTemplateConfig[i].ruleId - 1];
                    if (isEvent)
                    {
                        moduleGroupID = moduleTemplatePar[moduleTemplateConfig[i].ruleId];
                    }

                    var tmpIDs = new NativeList<int>(Allocator.Temp);
                    var tmpRefreshIDs = new NativeList<int>(Allocator.Temp);
                    for (int j = 0; j < moduleRefreshConfig.Length; j++)
                    {
                        //根据module_refresh表的组id 取出刷新组id和地图组件id 根据权重加n次
                        if (moduleRefreshConfig[j].group == moduleGroupID)
                        {
                            for (int k = 0; k < moduleRefreshConfig[j].power; k++)
                            {
                                tmpIDs.Add(moduleRefreshConfig[j].sceneModule);
                                tmpRefreshIDs.Add(moduleRefreshConfig[j].id);
                            }
                        }
                    }

                    int m = 0, times = 0;
                    do
                    {
                        times++;
                        var rand = Unity.Mathematics.Random.CreateFromIndex(
                            (uint)(i * 100 + m + times * 10 + seed1));
                        Debug.Log($"seed9999:{(uint)(i * 100 + m + seed1)}");
                        //取出的地图组件id为一个随机组
                        int randomNum = rand.NextInt(0, tmpIDs.Length);

                        //不能取重复
                        if (!refreshIds.Contains(tmpRefreshIDs[randomNum]))
                        {
                            //最终的刷新组id
                            refreshIds.Add(tmpRefreshIDs[randomNum]);
                            Debug.Log($"refreshIds:{tmpRefreshIDs[randomNum]}");
                            moduleIds.Add(tmpIDs[randomNum]);
                            //处理flip
                            for (int k = 0; k < sceneModuleConfig.Length; k++)
                            {
                                if (sceneModuleConfig[k].id == tmpIDs[randomNum])
                                {
                                    FixedString32Bytes flipStr = $"_flip";
                                    FixedString128Bytes pic = sceneModuleConfig[k].model;
                                    if (sceneModuleConfig[k].model.Contains(flipStr))
                                    {
                                        int index = sceneModuleConfig[k].model.IndexOf(flipStr);
                                        pic = sceneModuleConfig[k].model.Substring(0, index);
                                    }

                                    var prefab = prefabMapData.prefabHashMap[pic];
                                    modules.Add(prefab);
                                    //Debug.LogError($"sceneModule[k].id:{sceneModule[k].id}");
                                    break;
                                }
                            }

                            m++;
                            times = 0;
                        }
                    } while (m < moduleTemplateConfig[i].num && times < 1000);
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddBossEventToMap(ref EntityCommandBuffer.ParallelWriter ecb, int sortKey,
            GlobalConfigData configData,
            Entity mapEntity, int eventID)
        {
            ref var eventsTable = ref configData.value.Value.configTbevent_0s.configTbevent_0s;

            for (int i = 0; i < eventsTable.Length; i++)
            {
                if (eventsTable[i].id != eventID) continue;
                switch (eventID)
                {
                    //case 3012:
                    //    ecb.AppendToBuffer(sortKey, mapEntity, new GameEvent_3012
                    //    {
                    //        id = eventID,
                    //        triggerType = eventsTable[i].triggerType,
                    //        eventType = eventsTable[i].type,
                    //        triggerGap = eventsTable[i].triggerPara[0] / 1000f,
                    //        remainTime = 1,
                    //        duration = 0,
                    //        isPermanent = true,
                    //        target = default,
                    //        onceTime = 0,
                    //        colliderScale = 0,
                    //        delayTime = 0,
                    //    }.ToGameEvent());
                    //    break;
                    //case 3013:
                    //    ecb.AppendToBuffer(sortKey, mapEntity, new GameEvent_3013
                    //    {
                    //        id = eventID,
                    //        triggerType = eventsTable[i].triggerType,
                    //        eventType = eventsTable[i].type,
                    //        triggerGap = eventsTable[i].triggerPara[0] / 1000f,
                    //        remainTime = 1,
                    //        duration = 0,
                    //        isPermanent = true,
                    //        target = default,
                    //        onceTime = 0,
                    //        colliderScale = 0,
                    //        delayTime = 0
                    //    }.ToGameEvent());
                    //    break;
                }
            }
        }

        #endregion

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GenerateEdge(float mapsize, float edgeWidth,
            EntityCommandBuffer.ParallelWriter ecb, int sortKey, PrefabMapData prefabMapData, float3 pos, int areaID,
            GlobalConfigData configData, Entity linkedEntity)
        {
            ref var sceneMoudleTable = ref configData.value.Value.configTbscene_modules.configTbscene_modules;

            int index = 0;
            for (int i = 0; i < sceneMoudleTable.Length; ++i)
            {
                if (sceneMoudleTable[i].id == areaID)
                {
                    index = i;
                    break;
                }
            }

            var areaModuleName = sceneMoudleTable[index].model;
            var areaSize = sceneMoudleTable[index].size[0] / 1000f;

            float edgeRegionHeigth = mapsize + edgeWidth * 2;
            //i是列 j是行
            for (int i = 0; i < edgeRegionHeigth / areaSize; i++)
            {
                for (int j = 0; j < edgeWidth / areaSize; j++)
                {
                    var entityL = ecb.Instantiate(sortKey, prefabMapData.prefabHashMap[areaModuleName]);
                    var entityR = ecb.Instantiate(sortKey, prefabMapData.prefabHashMap[areaModuleName]);
                    ecb.SetComponent(sortKey, entityL, new LocalTransform
                    {
                        Position = pos + new float3
                        {
                            x = -mapsize / 2f - edgeWidth + areaSize * (j + 0.5f),
                            y = mapsize / 2f + edgeWidth - areaSize * (i + 0.5f),
                            z = 0
                        },
                        Scale = areaSize
                    });
                    ecb.SetComponent(sortKey, entityR, new LocalTransform
                    {
                        Position = pos + new float3
                        {
                            x = mapsize / 2f + areaSize * (j + 0.5f),
                            y = mapsize / 2f + edgeWidth - areaSize * (i + 0.5f),
                            z = 0
                        },
                        Scale = areaSize
                    });
                    ecb.AppendToBuffer(sortKey, linkedEntity, new LinkedEntityGroup
                    {
                        Value = entityL
                    });
                    ecb.AppendToBuffer(sortKey, linkedEntity, new LinkedEntityGroup
                    {
                        Value = entityR
                    });
                }
            }

            //i是行,j是列
            for (int i = 0; i < mapsize / areaSize; i++)
            {
                for (int j = 0; j < edgeWidth / areaSize; j++)
                {
                    var entityT = ecb.Instantiate(sortKey, prefabMapData.prefabHashMap[areaModuleName]);
                    var entityB = ecb.Instantiate(sortKey, prefabMapData.prefabHashMap[areaModuleName]);
                    ecb.SetComponent(sortKey, entityT, new LocalTransform
                    {
                        Position = pos + new float3
                        {
                            x = -mapsize / 2f + areaSize * (0.5f + i),
                            y = mapsize / 2f + edgeWidth - areaSize * (j + 0.5f),
                            z = 0
                        },
                        Scale = areaSize
                    });
                    ecb.SetComponent(sortKey, entityB, new LocalTransform
                    {
                        Position = pos + new float3
                        {
                            x = -mapsize / 2f + areaSize * (0.5f + i),
                            y = -mapsize / 2f - areaSize * (j + 0.5f),
                            z = 0
                        },
                        Scale = areaSize
                    });

                    ecb.AppendToBuffer(sortKey, linkedEntity, new LinkedEntityGroup
                    {
                        Value = entityT
                    });
                    ecb.AppendToBuffer(sortKey, linkedEntity, new LinkedEntityGroup
                    {
                        Value = entityB
                    });
                }
            }
        }

        #endregion

        /// <summary>
        /// 生成item,并且挂相应数据
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="eventID"></param>
        /// <param name="configData"></param>
        /// <param name="prefabMapData"></param>
        /// <param name="sortKey"></param>
        /// <param name="mapIndex"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity GenerateItemAndAddData(ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix,
            Entity player, ref EntityCommandBuffer.ParallelWriter ecb,
            ComponentLookup<LocalTransform> cdfeLocalTransform, ComponentLookup<WeaponData> cdfeWeaponData,
            ComponentLookup<ChaStats> cdfeChaStats, ComponentLookup<PhysicsMass> cdfePhysicsMass,
            ComponentLookup<AgentLocomotion> cdfeAgentLocomotion, ComponentLookup<EnemyData> cdfeEnemyData,
            GlobalConfigData configData, PrefabMapData prefabMapData, GameTimeData timeData,
            GameOthersData gameOthersData, int sortKey,
            int configType, int itemID, int dropItemId,
            float3 pos, float duration = MathHelper.MaxNum, bool isinBoss = false,float bossScenSize=0f)
        {
            //Debug.Log($"GenerateItemAndAddData");
            pos.z = 0;
            Entity ins = default;
            switch (configType)
            {
                case 1:
                    GenerateMapModule(cdfePostTransformMatrix, cdfeLocalTransform[player].Position, gameOthersData,
                        ref ecb, configData, prefabMapData, sortKey, itemID, pos, ref ins, isinBoss,bossScenSize);
                    ecb.AddComponent(sortKey, ins, new TimeToDieData
                    {
                        duration = duration
                    });
                    break;


                case 2:

                    ref var monsterTable = ref configData.value.Value.configTbmonsters.configTbmonsters;
                    int monsterIndex = 0;
                    for (int i = 0; i < monsterTable.Length; i++)
                    {
                        if (monsterTable[i].id == itemID)
                        {
                            monsterIndex = i;
                            break;
                        }
                    }

                    ref var monster = ref monsterTable[monsterIndex];

                    ref var monsterAttrTable = ref configData.value.Value.configTbmonster_attrs.configTbmonster_attrs;
                    int monsterAttrIndex = 0;
                    for (int i = 0; i < monsterAttrTable.Length; i++)
                    {
                        if (monsterAttrTable[i].id == monster.monsterAttrId)
                        {
                            monsterAttrIndex = i;
                            break;
                        }
                    }

                    ref var monsterAttr = ref monsterAttrTable[monsterAttrIndex];


                    ref var monster_models =
                        ref configData.value.Value.configTbmonster_models.configTbmonster_models;

                    int monster_modelsIndex = 0;
                    for (int m = 0; m < monster_models.Length; m++)
                    {
                        if (monsterAttr.bookId == monster_models[m].id)
                        {
                            monster_modelsIndex = m;
                            break;
                        }
                    }

                    ref var monsterModel = ref monster_models[monster_modelsIndex];


                    if (!prefabMapData.prefabHashMap.TryGetValue(monsterModel.model, out var monsterPrefab))
                    {
                        Debug.LogError($"{monsterModel.model} 找不到预制件");
                        return Entity.Null;
                    }

                    ref var tbmonster_templates =
                        ref configData.value.Value.configTbmonster_templates.configTbmonster_templates;
                    int monster_templatesIndex = 0;
                    for (int i = 0; i < tbmonster_templates.Length; i++)
                    {
                        //TODO:正反遍历
                        if (tbmonster_templates[i].monsterId == monster.id &&
                            tbmonster_templates[i].id == gameOthersData.monsterRefreshId)
                        {
                            monster_templatesIndex = i;
                            break;
                        }
                    }

                    ref var monster_template = ref tbmonster_templates[monster_templatesIndex];

                    ref var tbmonster_weapons =
                        ref configData.value.Value.configTbweapons.configTbweapons;

                    ref var levelConfig = ref configData.value.Value.configTblevels.configTblevels;
                    int levelIndex = 0;
                    for (int l = 0; l < levelConfig.Length; l++)
                    {
                        if (levelConfig[l].id == gameOthersData.levelId)
                        {
                            levelIndex = l;
                            break;
                        }
                    }

                    ref var level = ref levelConfig[levelIndex];

                    ins = ecb.Instantiate(sortKey, monsterPrefab);

                    for (int i = 0; i < monster_template.reward.Length; i++)
                    {
                        var dropnum = monster_template.reward[i].y;
                        ecb.AppendToBuffer(sortKey, ins, new DropsBuffer
                        {
                            id = dropItemId == 0 ? monster_template.reward[i].x : dropItemId
                        });
                    }

                    ref var monsterWeapon = ref tbmonster_weapons[0];
                    Entity weaponIns = Entity.Null;

                    #region Weapon

                    if (monsterWeapon.type == 1)
                    {
                        int monsterWeaponIndex = 0;
                        for (int l = 0; l < tbmonster_weapons.Length; l++)
                        {
                            if (monster.monsterWeaponId == tbmonster_weapons[l].id)
                            {
                                monsterWeaponIndex = l;
                                break;
                            }
                        }

                        monsterWeapon = ref tbmonster_weapons[monsterWeaponIndex];

                        //FixedString128Bytes weaponName = $"weapon_{monster.monsterWeaponId}";

                        if (prefabMapData.prefabHashMap.TryGetValue(monsterWeapon.model,
                                out var weaponPrefab))
                        {
                            var weaponData = cdfeWeaponData[weaponPrefab];


                            weaponIns = ecb.Instantiate(sortKey, weaponPrefab);

                            ecb.AppendToBuffer(sortKey, ins, new LinkedEntityGroup()
                            {
                                Value = weaponIns
                            });
                            ecb.AddComponent(sortKey, weaponIns, new Parent()
                            {
                                Value = ins
                            });
                            weaponData.enableEnemyWeapon =
                                monster.monsterWeaponRunDisplay == 1 ? true : false;
                            weaponData.weaponId = monster.monsterWeaponId;
                            ref var animPara = ref monsterWeapon.displayPara1;

                            if (monsterAttr.type == 5)
                            {
                                ref var animPara2 = ref monsterWeapon.displayPara2;
                                BuffHelper.SetWeaponData(ref weaponData, ref animPara2, monsterWeapon.displayType,
                                    true);
                            }
                            else
                            {
                                BuffHelper.SetWeaponData(ref weaponData, ref animPara, monsterWeapon.displayType);
                            }

                            weaponData.offset = new float3(monster.monsterWeaponIndex[0] / 1000f,
                                monster.monsterWeaponIndex[1] / 1000f, 0);
                            weaponData.scale = monster.monsterWeaponIndex[2] / 10000f;
                            weaponData.rotation = quaternion.RotateZ(math.radians(monster.monsterWeaponIndex[3]));


                            ecb.SetComponent(sortKey, weaponIns, weaponData);
                        }
                    }

                    #endregion

                    var chaStat = cdfeChaStats[monsterPrefab];

                    var AgentLocomotion = cdfeAgentLocomotion[monsterPrefab];
                    var enemyData = cdfeEnemyData[monsterPrefab];

                    chaStat.chaProperty.maxHp =
                        (int)(monster_template.hp * (level.hp / 10000f) *
                              (monsterAttr.hp / 10000f));
                    chaStat.chaResource.hp =
                        (int)(monster_template.hp * (level.hp / 10000f) *
                              (monsterAttr.hp / 10000f));

                    chaStat.chaProperty.atk =
                        (int)(monster_template.atk * (level.atk / 10000f) *
                              (monsterAttr.atk / 10000f) *
                              (monsterWeapon.paraAtk / 10000f));


                    if (monster.monsterWeaponId > 0 && monsterAttr.type != 5)
                    {
                        chaStat.chaProperty.pushForce =
                            (int)((monsterAttr.force) *
                                  (monsterWeapon.paraForce / 10000f));
                    }
                    else
                    {
                        chaStat.chaProperty.pushForce =
                            (int)((monsterAttr.force));
                    }


                    chaStat.chaProperty.maxMoveSpeed = (int)(monsterAttr.speed * monsterWeapon.paraSpeed / 10000f);
                    chaStat.chaProperty.mass = monsterAttr.mass;
                    if (cdfePhysicsMass.HasComponent(monsterPrefab))
                    {
                        var physicsMass = cdfePhysicsMass[monsterPrefab];
                        physicsMass.InverseMass = 1f / (float)monsterAttr.mass;
                        ecb.SetComponent(sortKey, ins, physicsMass);
                    }

                    //chaStat.chaProperty.speedRecoveryTime = monsterAttr.speedRefresh;
                    chaStat.chaProperty.reduceHitBackRatios = monsterAttr.repelResistAddition;
                    chaStat.chaProperty.scaleRatios = 10000;

                    chaStat.chaResource.actionSpeed = 1;

                    AgentLocomotion.Speed = chaStat.chaProperty.maxMoveSpeed / 1000f;
                    enemyData.enemyID = monster.id;
                    enemyData.type = UnityHelper.GetTargetId(monsterAttr.type);
                    enemyData.commonSkillCd =
                        monster.commonCd / 1000f +
                        math.max(monsterWeapon.paraInterval / 1000f - monsterWeapon.displayTime / 1000f, 0);
                    enemyData.curCommonSkillCd = enemyData.commonSkillCd;


                    enemyData.changeDirYn = monsterAttr.changeDirYn;
                    enemyData.attackType = EnemyAttackType.None;
                    enemyData.isHitBackAttack = false;
                    if (monsterWeapon.type == 1)
                    {
                        //enemyData.entityGroup.weaponEntity = weaponIns;
                        enemyData.weaponId = monsterWeapon.id;
                        switch (monsterWeapon.atkType)
                        {
                            case 1:
                                enemyData.attackType = EnemyAttackType.NormalShortAttack;
                                enemyData.isHitBackAttack = false;
                                //enemyData.attackRadius = UnityHelper.ShortAttackRange;
                                break;
                            case 2:
                                enemyData.attackType = EnemyAttackType.NormalShortAttack;
                                enemyData.isHitBackAttack = true;
                                //enemyData.attackRadius = UnityHelper.ShortAttackRange;
                                break;
                            case 3:
                                enemyData.attackType = EnemyAttackType.NormalShortAttack;
                                enemyData.isHitBackAttack = true;
                                //enemyData.attackRadius = UnityHelper.ShortAttackRange;
                                break;
                            case 4:
                                enemyData.attackType = EnemyAttackType.NormalLongAttack;
                                enemyData.isHitBackAttack = false;
                                //enemyData.attackRadius = UnityHelper.LongAttackRange;
                                break;
                            case 5:
                                enemyData.attackType = EnemyAttackType.NormalLongAttack;
                                enemyData.isHitBackAttack = true;
                                //enemyData.attackRadius = UnityHelper.LongAttackRange;
                                break;
                        }
                    }

                    enemyData.moveType = (EnemyMoveType)1;
                    enemyData.moveTypePara = new int4(1, 0, 0, 0);
                    if (monster.monsterWeaponId > 0)
                    {
                        enemyData.moveType = (EnemyMoveType)monsterWeapon.moveType[0];
                        enemyData.moveTypePara = new int4(monsterWeapon.moveType[0], monsterWeapon.moveType[1],
                            monsterWeapon.moveType[2], monsterWeapon.moveType[3]);
                    }

                    enemyData.targetGroup = 1;
                    for (int l = 0; l < monsterModel.featureId.Length; l++)
                    {
                        var id = monsterModel.featureId[l];
                        enemyData.enemyFeature.Add(id);

                        ecb.AppendToBuffer(sortKey, ins, new Buff()
                        {
                            CurrentTypeId = (Buff.TypeId)id + 999000,
                            Int32_0 = id,
                            Boolean_4 = true,
                            Entity_6 = ins
                        });
                    }

                    //混合spine流程
                    if (enemyData.isHybridSpine)
                    {
                        ecb.AddComponent(sortKey, ins, new SpineHybridData()
                        {
                        });
                        ecb.AddComponent(sortKey, ins, new JiYuFlip()
                        {
                        });
                    }

                    if (enemyData.isHybridSpine)
                    {
                        var newentity = ecb.CreateEntity(sortKey);
                        ecb.AddComponent(sortKey, newentity, new HybridEventData
                        {
                            type = 12,
                            args = new float4(monster.id, float3.zero),
                            bossEntity = ins
                        });
                    }

                    var stateType = monster.skillGroup.Length > 0 || monster.passiveSkill.Length > 0 ? 3 : 2;
                    enemyData.canCastSkill = stateType == 3;
                    if (enemyData.type == (int)BuffHelper.TargetEnum.BossMonster)
                    {
                        ecb.AddComponent<BossTag>(sortKey, ins);
                    }

                    UnityHelper.AddStateToStatesBuffer(ref ecb, sortKey,
                        stateType, ins);

                    chaStat.chaProperty.atk = math.max(1, chaStat.chaProperty.atk);
                    chaStat.chaProperty.maxHp = math.max(1, chaStat.chaProperty.maxHp);

                    chaStat.chaResource.hp = chaStat.chaProperty.maxHp;
                    chaStat.chaProperty.defaultMass = chaStat.chaProperty.mass;
                    chaStat.chaProperty.defaultAtk = chaStat.chaProperty.atk;
                    chaStat.chaProperty.defaultMaxHp = chaStat.chaProperty.maxHp;
                    chaStat.chaProperty.defaultHpRecovery = chaStat.chaProperty.hpRecovery;
                    chaStat.chaProperty.defaultMaxMoveSpeed = chaStat.chaProperty.maxMoveSpeed;
                    chaStat.chaProperty.defaultPushForce = chaStat.chaProperty.pushForce;
                    if (monster.timeToDie > 0)
                    {
                        ecb.AddComponent(sortKey, ins, new TimeToDieData
                        {
                            duration = monster.timeToDie / 1000f
                        });
                    }

                    ecb.SetComponent(sortKey, ins, chaStat);
                    ecb.SetComponent(sortKey, ins, AgentLocomotion);
                    ecb.SetComponent(sortKey, ins, enemyData);
                    ecb.SetComponent(sortKey, ins, new TargetData
                    {
                        tick = 0,
                        BelongsTo = (uint)enemyData.type,
                        AttackWith = (uint)BuffHelper.TargetGroupEnum.PlayerGroup
                    });
                    for (int l = 0; l < monster.skillGroup.Length; l++)
                    {
                        var skill = new Skill()
                        {
                            CurrentTypeId = (Skill.TypeId)1,
                            Int32_0 = monster.skillGroup[l],
                            Entity_5 = ins,
                            Int32_10 = 2,
                            //Boolean_11 = true,
                            Int32_6 = 2
                        };
                        ecb.AppendToBuffer(sortKey, ins, skill);
                    }

                    for (int l = 0; l < monster.passiveSkill.Length; l++)
                    {
                        var skill = new Skill()
                        {
                            CurrentTypeId = (Skill.TypeId)1,
                            Int32_0 = monster.passiveSkill[l],
                            Entity_5 = ins,
                            Int32_10 = 2,
                            Boolean_17 = true
                        };
                        ecb.AppendToBuffer(sortKey, ins, skill);
                    }

                    var newpos2 = new LocalTransform
                    {
                        Position = pos,
                        Scale = 0.001f,
                        Rotation = cdfeLocalTransform[monsterPrefab].Rotation
                    };

                    var scale = monsterAttr.modelSize / 10000f;
                    bool isBossScene = monster.sceneBossId > 0;
                    // if (isBossScene)
                    // {
                    //     scale = UnityHelper.BossScale;
                    // }

                    ecb.SetComponent(sortKey, ins, new PushColliderData
                    {
                        targetScale = scale
                    });
                    ecb.SetComponent(sortKey, ins, newpos2);

                    ecb.SetComponent(sortKey, ins, new AgentShape
                    {
                        Radius = scale / 2f,
                        Height = 0,
                        Type = ShapeType.Circle
                    });
                    break;
                case 3:
                    ref var battleItemTable = ref configData.value.Value.configTbbattle_items.configTbbattle_items;
                    int battleItemIndex = 0;
                    for (int i = 0; i < battleItemTable.Length; i++)
                    {
                        if (battleItemTable[i].id == itemID)
                        {
                            battleItemIndex = i;
                            break;
                        }
                    }

                    ref var battleItem = ref battleItemTable[battleItemIndex];

                    if (!prefabMapData.prefabHashMap.TryGetValue(battleItem.model, out var prefabBattleItem))
                    {
                        Debug.LogError($"{battleItem.model} 找不到预制件");
                        return Entity.Null;
                    }

                    ins = ecb.Instantiate(sortKey, prefabBattleItem);

                    var newpos = new LocalTransform
                    {
                        Position = pos,
                        Scale = cdfeLocalTransform[prefabBattleItem].Scale,
                        Rotation = cdfeLocalTransform[prefabBattleItem].Rotation
                    };

                    //ecb.SetComponent(sortKey, ins, newpos);
                    newpos.Position.x += 5;
                    var dropsData = new DropsData
                    {
                        point0 = newpos.Position,
                        point1 = default,
                        point2 = new float3(newpos.Position.x,
                            newpos.Position.y, 0),
                        point3 = default,
                        isLooting = false,
                        lootingAniDuration = gameOthersData.pickupDuration / 1000f,
                        id = battleItem.id,
                        dropPoint2 = newpos.Position,
                        dropPoint0 = pos
                    };

                    ecb.SetComponent(sortKey, ins, dropsData);
                    ecb.AddComponent(sortKey, ins, new TimeToDieData
                    {
                        duration = duration
                    });

                    break;
            }


            return ins;
        }

        /// <summary>
        /// 生成item,并且挂相应数据
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="eventID"></param>
        /// <param name="configData"></param>
        /// <param name="prefabMapData"></param>
        /// <param name="sortKey"></param>
        /// <param name="mapIndex"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity GenerateMonsterAndAddData(ref EntityCommandBuffer.ParallelWriter ecb,
            ComponentLookup<LocalTransform> cdfeLocalTransform, ComponentLookup<WeaponData> cdfeWeaponData,
            ComponentLookup<ChaStats> cdfeChaStats, ComponentLookup<PhysicsMass> cdfePhysicsMass,
            ComponentLookup<AgentLocomotion> cdfeAgentLocomotion, ComponentLookup<EnemyData> cdfeEnemyData,
            GlobalConfigData configData, PrefabMapData prefabMapData, GameTimeData timeData,
            GameOthersData gameOthersData, int sortKey, int monsterId, int defaultAtk, int defaultHp, int dropItemId,
            float3 pos)
        {
            Debug.Log($"GenerateMonsterAndAddData");
            pos.z = 0;
            Entity ins = default;

            ref var monsterTable = ref configData.value.Value.configTbmonsters.configTbmonsters;
            int monsterIndex = 0;
            for (int i = 0; i < monsterTable.Length; i++)
            {
                if (monsterTable[i].id == monsterId)
                {
                    monsterIndex = i;
                    break;
                }
            }

            ref var monster = ref monsterTable[monsterIndex];

            ref var monsterAttrTable = ref configData.value.Value.configTbmonster_attrs.configTbmonster_attrs;
            int monsterAttrIndex = 0;
            for (int i = 0; i < monsterAttrTable.Length; i++)
            {
                if (monsterAttrTable[i].id == monster.monsterAttrId)
                {
                    monsterAttrIndex = i;
                    break;
                }
            }

            ref var monsterAttr = ref monsterAttrTable[monsterAttrIndex];

            ref var monster_models =
                ref configData.value.Value.configTbmonster_models.configTbmonster_models;

            int monster_modelsIndex = 0;
            for (int m = 0; m < monster_models.Length; m++)
            {
                if (monsterAttr.bookId == monster_models[m].id)
                {
                    monster_modelsIndex = m;
                    break;
                }
            }

            ref var monsterModel = ref monster_models[monster_modelsIndex];

            if (!prefabMapData.prefabHashMap.TryGetValue(monsterModel.model, out var monsterPrefab))
            {
                Debug.LogError($"{monsterModel.model} 找不到预制件");
                return Entity.Null;
            }

            ref var tbmonster_templates =
                ref configData.value.Value.configTbmonster_templates.configTbmonster_templates;
            int monster_templatesIndex = 0;
            for (int i = 0; i < tbmonster_templates.Length; i++)
            {
                //TODO:正反遍历
                if (tbmonster_templates[i].monsterId == monster.id &&
                    tbmonster_templates[i].id == gameOthersData.monsterRefreshId)
                {
                    monster_templatesIndex = i;
                    break;
                }
            }

            ref var monster_template = ref tbmonster_templates[monster_templatesIndex];

            ref var tbmonster_weapons =
                ref configData.value.Value.configTbweapons.configTbweapons;

            ref var levelConfig = ref configData.value.Value.configTblevels.configTblevels;
            int levelIndex = 0;
            for (int l = 0; l < levelConfig.Length; l++)
            {
                if (levelConfig[l].id == gameOthersData.levelId)
                {
                    levelIndex = l;
                    break;
                }
            }

            ref var level = ref levelConfig[levelIndex];

            ins = ecb.Instantiate(sortKey, monsterPrefab);
            if (dropItemId != 0)
            {
                ecb.AppendToBuffer(sortKey, ins, new DropsBuffer
                {
                    id = dropItemId
                });
            }
            // for (int i = 0; i < monster_template.reward.Length; i++)
            // {
            //     var dropnum = monster_template.reward[i].y;
            //    
            // }

            ref var monsterWeapon = ref tbmonster_weapons[0];
            Entity weaponIns = Entity.Null;

            #region Weapon

            if (monsterWeapon.type == 1)
            {
                int monsterWeaponIndex = 0;
                for (int l = 0; l < tbmonster_weapons.Length; l++)
                {
                    if (monster.monsterWeaponId == tbmonster_weapons[l].id)
                    {
                        monsterWeaponIndex = l;
                        break;
                    }
                }

                monsterWeapon = ref tbmonster_weapons[monsterWeaponIndex];

                //FixedString128Bytes weaponName = $"weapon_{monster.monsterWeaponId}";

                if (prefabMapData.prefabHashMap.TryGetValue(monsterWeapon.model,
                        out var weaponPrefab))
                {
                    var weaponData = cdfeWeaponData[weaponPrefab];


                    weaponIns = ecb.Instantiate(sortKey, weaponPrefab);

                    ecb.AppendToBuffer(sortKey, ins, new LinkedEntityGroup()
                    {
                        Value = weaponIns
                    });
                    ecb.AddComponent(sortKey, weaponIns, new Parent()
                    {
                        Value = ins
                    });
                    weaponData.enableEnemyWeapon =
                        monster.monsterWeaponRunDisplay == 1 ? true : false;
                    weaponData.weaponId = monster.monsterWeaponId;
                    ref var animPara = ref monsterWeapon.displayPara1;

                    if (monsterAttr.type == 5)
                    {
                        ref var animPara2 = ref monsterWeapon.displayPara2;
                        BuffHelper.SetWeaponData(ref weaponData, ref animPara2, monsterWeapon.displayType,
                            true);
                    }
                    else
                    {
                        BuffHelper.SetWeaponData(ref weaponData, ref animPara, monsterWeapon.displayType);
                    }

                    weaponData.offset = new float3(monster.monsterWeaponIndex[0] / 1000f,
                        monster.monsterWeaponIndex[1] / 1000f, 0);
                    weaponData.scale = monster.monsterWeaponIndex[2] / 10000f;
                    weaponData.rotation = quaternion.RotateZ(math.radians(monster.monsterWeaponIndex[3]));


                    ecb.SetComponent(sortKey, weaponIns, weaponData);
                }
            }

            #endregion

            var chaStat = cdfeChaStats[monsterPrefab];

            var AgentLocomotion = cdfeAgentLocomotion[monsterPrefab];
            var enemyData = cdfeEnemyData[monsterPrefab];

            chaStat.chaProperty.maxHp =
                (int)(defaultHp *
                      (monsterAttr.hp / 10000f));
            chaStat.chaResource.hp =
                chaStat.chaProperty.maxHp;

            chaStat.chaProperty.atk =
                (int)(defaultAtk *
                      (monsterAttr.atk / 10000f) *
                      (monsterWeapon.paraAtk / 10000f));


            if (monster.monsterWeaponId > 0 && monsterAttr.type != 5)
            {
                chaStat.chaProperty.pushForce =
                    (int)((monsterAttr.force) *
                          (monsterWeapon.paraForce / 10000f));
            }
            else
            {
                chaStat.chaProperty.pushForce =
                    (int)((monsterAttr.force));
            }


            chaStat.chaProperty.maxMoveSpeed = (int)(monsterAttr.speed * monsterWeapon.paraSpeed / 10000f);
            chaStat.chaProperty.mass = monsterAttr.mass;
            if (cdfePhysicsMass.HasComponent(monsterPrefab))
            {
                var physicsMass = cdfePhysicsMass[monsterPrefab];
                physicsMass.InverseMass = 1f / (float)monsterAttr.mass;
                ecb.SetComponent(sortKey, ins, physicsMass);
            }

            //chaStat.chaProperty.speedRecoveryTime = monsterAttr.speedRefresh;
            chaStat.chaProperty.reduceHitBackRatios = monsterAttr.repelResistAddition;
            chaStat.chaProperty.scaleRatios = 10000;

            chaStat.chaResource.actionSpeed = 1;

            AgentLocomotion.Speed = chaStat.chaProperty.maxMoveSpeed / 1000f;
            enemyData.enemyID = monster.id;
            enemyData.type = UnityHelper.GetTargetId(monsterAttr.type);
            enemyData.commonSkillCd =
                monster.commonCd / 1000f +
                math.max(monsterWeapon.paraInterval / 1000f - monsterWeapon.displayTime / 1000f, 0);
            enemyData.curCommonSkillCd = enemyData.commonSkillCd;


            enemyData.changeDirYn = monsterAttr.changeDirYn;
            enemyData.attackType = EnemyAttackType.None;
            enemyData.isHitBackAttack = false;
            if (monsterWeapon.type == 1)
            {
                //enemyData.entityGroup.weaponEntity = weaponIns;
                enemyData.weaponId = monsterWeapon.id;
                switch (monsterWeapon.atkType)
                {
                    case 1:
                        enemyData.attackType = EnemyAttackType.NormalShortAttack;
                        enemyData.isHitBackAttack = false;
                        //enemyData.attackRadius = UnityHelper.ShortAttackRange;
                        break;
                    case 2:
                        enemyData.attackType = EnemyAttackType.NormalShortAttack;
                        enemyData.isHitBackAttack = true;
                        //enemyData.attackRadius = UnityHelper.ShortAttackRange;
                        break;
                    case 3:
                        enemyData.attackType = EnemyAttackType.NormalShortAttack;
                        enemyData.isHitBackAttack = true;
                        //enemyData.attackRadius = UnityHelper.ShortAttackRange;
                        break;
                    case 4:
                        enemyData.attackType = EnemyAttackType.NormalLongAttack;
                        enemyData.isHitBackAttack = false;
                        //enemyData.attackRadius = UnityHelper.LongAttackRange;
                        break;
                    case 5:
                        enemyData.attackType = EnemyAttackType.NormalLongAttack;
                        enemyData.isHitBackAttack = true;
                        //enemyData.attackRadius = UnityHelper.LongAttackRange;
                        break;
                }
            }

            enemyData.moveType = (EnemyMoveType)1;
            enemyData.moveTypePara = new int4(1, 0, 0, 0);
            if (monster.monsterWeaponId > 0)
            {
                enemyData.moveType = (EnemyMoveType)monsterWeapon.moveType[0];
                enemyData.moveTypePara = new int4(monsterWeapon.moveType[0], monsterWeapon.moveType[1],
                    monsterWeapon.moveType[2], monsterWeapon.moveType[3]);
            }

            enemyData.targetGroup = 1;
            for (int l = 0; l < monsterModel.featureId.Length; l++)
            {
                var id = monsterModel.featureId[l];
                enemyData.enemyFeature.Add(id);

                ecb.AppendToBuffer(sortKey, ins, new Buff()
                {
                    CurrentTypeId = (Buff.TypeId)id + 999000,
                    Int32_0 = id,
                    Boolean_4 = true,
                    Entity_6 = ins
                });
            }

            //混合spine流程
            if (enemyData.isHybridSpine)
            {
                ecb.AddComponent(sortKey, ins, new SpineHybridData()
                {
                });
                ecb.AddComponent(sortKey, ins, new JiYuFlip()
                {
                });
            }

            if (enemyData.isHybridSpine)
            {
                var newentity = ecb.CreateEntity(sortKey);
                ecb.AddComponent(sortKey, newentity, new HybridEventData
                {
                    type = 12,
                    args = new float4(monster.id, float3.zero),
                    bossEntity = ins
                });
            }

            var stateType = monster.skillGroup.Length > 0 || monster.passiveSkill.Length > 0 ? 3 : 2;
            enemyData.canCastSkill = stateType == 3;
            if (enemyData.type == (int)BuffHelper.TargetEnum.BossMonster)
            {
                ecb.AddComponent<BossTag>(sortKey, ins);
            }

            UnityHelper.AddStateToStatesBuffer(ref ecb, sortKey,
                stateType, ins);

            chaStat.chaProperty.atk = math.max(1, chaStat.chaProperty.atk);
            chaStat.chaProperty.maxHp = math.max(1, chaStat.chaProperty.maxHp);

            chaStat.chaResource.hp = chaStat.chaProperty.maxHp;
            chaStat.chaProperty.defaultMass = chaStat.chaProperty.mass;
            chaStat.chaProperty.defaultAtk = chaStat.chaProperty.atk;
            chaStat.chaProperty.defaultMaxHp = chaStat.chaProperty.maxHp;
            chaStat.chaProperty.defaultHpRecovery = chaStat.chaProperty.hpRecovery;
            chaStat.chaProperty.defaultMaxMoveSpeed = chaStat.chaProperty.maxMoveSpeed;
            chaStat.chaProperty.defaultPushForce = chaStat.chaProperty.pushForce;
            if (monster.timeToDie > 0)
            {
                ecb.AddComponent(sortKey, ins, new TimeToDieData
                {
                    duration = monster.timeToDie / 1000f
                });
            }

            ecb.SetComponent(sortKey, ins, chaStat);
            ecb.SetComponent(sortKey, ins, AgentLocomotion);

            ecb.SetComponent(sortKey, ins, enemyData);
            ecb.SetComponent(sortKey, ins, new TargetData
            {
                tick = 0,
                BelongsTo = (uint)enemyData.type,
                AttackWith = (uint)BuffHelper.TargetGroupEnum.PlayerGroup
            });
            for (int l = 0; l < monster.skillGroup.Length; l++)
            {
                var skill = new Skill()
                {
                    CurrentTypeId = (Skill.TypeId)1,
                    Int32_0 = monster.skillGroup[l],
                    Entity_5 = ins,
                    Int32_10 = 2,
                    //Boolean_11 = true,
                    Int32_6 = 2
                };
                ecb.AppendToBuffer(sortKey, ins, skill);
            }

            for (int l = 0; l < monster.passiveSkill.Length; l++)
            {
                var skill = new Skill()
                {
                    CurrentTypeId = (Skill.TypeId)1,
                    Int32_0 = monster.passiveSkill[l],
                    Entity_5 = ins,
                    Int32_10 = 2,
                    Boolean_17 = true
                };
                ecb.AppendToBuffer(sortKey, ins, skill);
            }

            var newpos2 = new LocalTransform
            {
                Position = pos,
                Scale = 0.001f,
                Rotation = cdfeLocalTransform[monsterPrefab].Rotation
            };

            var scale = monsterAttr.modelSize / 10000f;
            bool isBossScene = monster.sceneBossId > 0;
            // if (isBossScene)
            // {
            //     scale = UnityHelper.BossScale;
            // }

            ecb.SetComponent(sortKey, ins, new PushColliderData
            {
                targetScale = scale
            });
            ecb.SetComponent(sortKey, ins, newpos2);

            ecb.SetComponent(sortKey, ins, new AgentShape
            {
                Radius = scale / 2f,
                Height = 0,
                Type = ShapeType.Circle
            });


            return ins;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GenerateMapModule(ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix,
            float3 playerPos, GameOthersData gameOthersData, ref EntityCommandBuffer.ParallelWriter ecb,
            GlobalConfigData configData,
            PrefabMapData prefabMapData, int sortKey, int itemID, float3 pos, ref Entity ins, bool isinBoss = false, float bossScenSize = 0)
        {
            int mapType = gameOthersData.mapData.mapType;

            float mapHeight, mapWidth;
            if (isinBoss)
            {
                mapWidth = mapHeight = bossScenSize;
            }
            else
            {
                mapHeight = gameOthersData.mapData.mapSize.y;
                mapWidth = gameOthersData.mapData.mapSize.x;
            }

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
                //startPos = new float3(mapCenterPos.x - mapWidth / 2f, mapCenterPos.y + mapheight / 2f, 0);
            }
            else
            {
                switch (mapType)
                {
                    case 1:
                        var curMapIndex = (int)((playerPos.y + mapWidth * 3) / mapHeight) + 1;
                        startPos = new float3(-mapWidth / 2f, -(mapWidth * 3) + mapHeight * curMapIndex, 0);
                        mapCenterPos = new float3(startPos.x + mapWidth / 2f, startPos.y - mapHeight / 2f, 0);
                        break;
                    case 3:
                        //startPos = new float3(-mapWidth / 2f, mapHeight / 2f, 0);
                        break;
                    case 4:
                        //还没想好
                        break;
                }
            }


            int index = -1;
            ref var mapModelTable = ref configData.value.Value.configTbscene_modules.configTbscene_modules;
            ref var elementConfig = ref configData.value.Value.configTbelement_effects.configTbelement_effects;
            for (int i = 0; i < mapModelTable.Length; i++)
            {
                if (mapModelTable[i].id == itemID)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0) return;

            ref var mapModel = ref mapModelTable[index];

            var pathPriority = mapModel.pathPriority;
            var propName = mapModel.model;
            FixedString32Bytes flipStr = $"_flip";
            if (propName.Contains(flipStr))
            {
                int indexName = propName.IndexOf(flipStr);
                propName = propName.Substring(0, indexName);

                //Debug.LogError($"1:{newString},PREFAB{prefabMapData.prefabHashMap[newString].Index}");
            }

            var propPrefab = prefabMapData.prefabHashMap[propName];
            ins = ecb.Instantiate(sortKey, propPrefab);

            float moduleWidth = mapModel.size[0] / 1000f;
            float moduleHeight = mapModel.size[1] / 1000f;
            var size = mapModel.size[0] > mapModel.size[1]
                ? mapModel.size[0] / 1000f
                : mapModel.size[1] / 1000f;
            var elementId = mapModel.elementId;
            var displayOrder = mapModel.displayOrder;
            //Debug.Log($"Onhit 障碍物触发pos:{pos},id:{itemID},name:{propName}");
            //TODO:在地图内部 不可在地图外部生成
            int gapSize = 5;
            //if (mapType == 1 || mapType == 3)
            //{
            //    //上下无限
            //    if (pos.x > (mapCenterPos.x+mapWidth / 2f - moduleWidth / 2f))
            //    {
            //        pos.x = mapCenterPos.x+ mapWidth / 2f - moduleWidth / 2f - gapSize;
            //    }
            //    else if (pos.x < (mapCenterPos.x - mapWidth / 2f + moduleWidth / 2f))
            //    {
            //        Debug.Log($"未生效:{pos.x}");
            //        pos.x = mapCenterPos.x-mapWidth / 2f + moduleWidth / 2f + gapSize;
            //        Debug.Log($"生效:{pos.x}");
            //    }
            //}

            //if (mapType == 3)
            //{
            //    //全封闭
            //    if (pos.y > (mapCenterPos.y+mapHeight / 2f - moduleHeight / 2f))
            //    {
            //        pos.y = mapCenterPos.y+ mapHeight / 2f - moduleHeight / 2f - gapSize;
            //    }
            //    else if (pos.y < (mapCenterPos.y - mapHeight / 2f + moduleHeight / 2f))
            //    {
            //        pos.y = mapCenterPos.y-mapHeight / 2f + moduleHeight / 2f + gapSize;
            //    }
            //}
            float scaleMulti = 1;
            var sizeScale = moduleHeight > moduleWidth ? moduleHeight : moduleWidth;
            var newloc = new LocalTransform { Scale = sizeScale, Position = pos, Rotation = quaternion.identity };

            if (cdfePostTransformMatrix.HasComponent(propPrefab))
            {
                var postTransformMatrix = cdfePostTransformMatrix[propPrefab];
                var scale = postTransformMatrix.Value.DecomposeScale();
                scaleMulti *= scale.x;
                ecb.SetComponent(sortKey, ins, new PostTransformMatrix
                {
                    Value = float4x4.Scale(sizeScale * scaleMulti, sizeScale * scaleMulti, 1)
                });
            }
            else
            {
                newloc.Scale *= scaleMulti;
            }


            ecb.SetComponent(sortKey, ins,
                new JiYuSort { value = new int2((int)JiYuLayer.Area, math.clamp(displayOrder, 0, 10)) });
            ecb.SetComponent(sortKey, ins,
                new MapElementData { elementID = itemID });
            if (elementId != 0)
            {
                ecb.AddComponent(sortKey, ins, new ElementData
                {
                    type = 0,
                    id = elementId
                });
            }

            if (mapModel.type == 1)
            {
                ecb.SetComponent(sortKey, ins,
                    new TargetData
                    {
                        BelongsTo = (uint)BuffHelper.TargetEnum.Area,
                        AttackWith = 1 + 2 + 4 + 8 + 16 + 32 + 128
                    });
                if (pathPriority >= 25)
                {
                    ecb.AddComponent(sortKey, ins, new AgentShape
                    {
                        Radius = size / 2f,
                        Height = 0,
                        Type = ShapeType.Circle
                    });
                    ecb.AddComponent(sortKey, ins, new Agent
                    {
                        Layers = NavigationLayers.Default
                    });
                    ecb.AddComponent(sortKey, ins, new AgentBody
                    {
                        Force = default,
                        Velocity = default,
                        Destination = default,
                        RemainingDistance = 0,
                        IsStopped = true
                    });
                }

                //给火地形加临时生命值
                if (elementId == 201)
                {
                    int maxHp = 0;
                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].from == 201 && elementConfig[i].target == 102)
                        {
                            maxHp = elementConfig[i].para[0];
                            break;
                        }
                    }

                    ecb.AddComponent(sortKey, ins, new AreaTempHp { maxHp = maxHp });
                    ecb.AddComponent(sortKey, ins, new TimeToDieData { duration = mapModel.duration / 1000f });
                }

                ecb.AddComponent(sortKey, ins, new SkillAttackData
                {
                    data = new SkillAttack_9999
                    {
                        duration = 9999f,
                        isBullet = true,
                        hp = MathHelper.MaxNum,
                        isOnStayTrigger = true,
                        onStayTriggerCd = 0.5f,
                    }.ToSkillAttack()
                });
            }
            else
            {
                newloc.Scale = 0.001f;
                ecb.SetComponent(sortKey, ins, new PushColliderData
                {
                    targetScale = size
                });

                int hp = mapModel.hp;
                if (hp > 0)
                {
                    ecb.AddComponent(sortKey, ins, new ChaStats
                    {
                        chaProperty = new ChaProperty
                        {
                            maxHp = hp,
                            hpRatios = 0,
                            hpAdd = 0,
                            hpRecovery = 0,
                            hpRecoveryRatios = 0,
                            hpRecoveryAdd = 0,
                            atk = 0,
                            atkRatios = 0,
                            atkAdd = 0,
                            rebirthCount = 0,
                            critical = 0,
                            criticalDamageRatios = 0,
                            damageRatios = 0,
                            damageAdd = 0,
                            reduceHurtRatios = 0,
                            reduceHurtAdd = 0,
                            maxMoveSpeed = 0,
                            maxMoveSpeedRatios = 0,
                            speedRecoveryTime = 0,
                            mass = 0,
                            massRatios = 0,
                            pushForce = 0,
                            pushForceRatios = 0,
                            reduceHitBackRatios = 0,
                            dodge = 0,
                            shieldCount = 0,
                            defaultcoolDown = 0,
                        },
                        enviromentProperty = default,
                        chaResource = new ChaResource
                        {
                            curPushForce = 0,
                            curMoveSpeed = 0,
                            direction = default,
                            continuousCollCount = 0,
                            actionSpeed = 0,
                            hp = hp,
                            env = default
                        },
                        chaControlState = default
                    });
                    //ecbs.AddBuffer<DamageInfo>(entity);
                    ecb.AddBuffer<Buff>(sortKey, ins);
                }

                ecb.AddComponent<ObstacleTag>(sortKey, ins);

                ecb.AddComponent(sortKey, ins, new AgentShape
                {
                    Radius = size / 2f,
                    Height = 0,
                    Type = ShapeType.Circle
                });
                ecb.AddComponent(sortKey, ins, new Agent
                {
                    Layers = NavigationLayers.Default
                });
                ecb.AddComponent(sortKey, ins, new AgentBody
                {
                    Force = default,
                    Velocity = default,
                    Destination = default,
                    RemainingDistance = 0,
                    IsStopped = true
                });
            }

            ecb.SetComponent(sortKey, ins, newloc);


            //TODO：Add SKill
            // ref var events = ref mapModel.event0;
            // if (events.Length <= 0) break;
            //for (int j = 0; j < events.Length; j++)
            //{
            //    int temp = events[j];
            //    AddEventTo(temp, ins, ecb, configData, sortKey);
            //}
        }


        /// <summary>
        /// 生成的prop,并且挂相应数据
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="eventID"></param>
        /// <param name="configData"></param>
        /// <param name="prefabMapData"></param>
        /// <param name="sortKey"></param>
        /// <param name="mapIndex"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity AddDataToProps(ref EntityCommandBuffer.ParallelWriter ecb, int eventID,
            GlobalConfigData configData, PrefabMapData prefabMapData, int sortKey, int mapIndex)
        {
            Entity propEntity = default;
            ref var eventsMap = ref configData.value.Value.configTbevent_0s.configTbevent_0s;
            int tableType = 0, propID = 0;
            for (int i = 0; i < eventsMap.Length; i++)
            {
                if (eventsMap[i].id == eventID)
                {
                    tableType = eventsMap[i].para2;
                    propID = eventsMap[i].para3;
                    break;
                }
            }

            //TODO:
            switch (tableType)
            {
                case 1:
                    ref var mapModelTable = ref configData.value.Value.configTbscene_modules.configTbscene_modules;
                    for (int i = 0; i < mapModelTable.Length; i++)
                    {
                        if (mapModelTable[i].id == propID)
                        {
                            var propName = mapModelTable[i].model;
                            var propPrefab = prefabMapData.prefabHashMap[propName];
                            propEntity = ecb.Instantiate(sortKey, propPrefab);
                            var size = mapModelTable[i].size[0] > mapModelTable[i].size[1]
                                ? mapModelTable[i].size[0]
                                : mapModelTable[i].size[1];
                            ecb.SetComponent(sortKey, propEntity, new LocalTransform { Scale = size });
                            ecb.AddComponent(sortKey, propEntity,
                                new MapElementData { elementID = propID });
                            ecb.RemoveComponent<MapElementData>(sortKey, propEntity);
                            if (propID / 1000 == 1)
                            {
                                ecb.AddComponent(sortKey, propEntity, new AreaTag());
                            }
                            else
                            {
                                int hp = mapModelTable[i].hp;
                                if (hp != 0)
                                {
                                    ecb.AddComponent(sortKey, propEntity, new ChaStats
                                    {
                                        chaProperty = new ChaProperty
                                        {
                                            maxHp = hp,
                                        },
                                        enviromentProperty = default,
                                        chaResource = new ChaResource()
                                        {
                                            hp = hp,
                                        },
                                        chaControlState = default
                                    });
                                    ecb.AddBuffer<DamageInfo>(sortKey, propEntity);
                                    ecb.AddBuffer<BuffOld>(sortKey, propEntity);
                                }

                                ecb.AddComponent(sortKey, propEntity, new AreaTag());
                            }
                            //TODO：Add SKill
                            // ref var events = ref mapModelTable[i].event0;
                            // if (events.Length <= 0) break;
                            // for (int j = 0; j < events.Length; j++)
                            // {
                            //     int temp = events[j];
                            //     AddEventTo(temp, propEntity, ecb, configData, sortKey);
                            // }

                            break;
                        }
                    }

                    break;
                case 2:
                    ref var monsterTable = ref configData.value.Value.configTbmonsters.configTbmonsters;
                    for (int i = 0; i < monsterTable.Length; i++)
                    {
                        if (monsterTable[i].id == propID)
                        {
                            // var propName = monsterTable[i].model;
                            // var propPrefab = prefabMapData.prefabHashMap[propName];
                            // var propEntity = ecb.Instantiate(sortKey, propPrefab);
                            break;
                        }
                    }

                    break;
                case 3:
                    ref var battleItemTable = ref configData.value.Value.configTbbattle_items.configTbbattle_items;
                    for (int i = 0; i < battleItemTable.Length; i++)
                    {
                        if (battleItemTable[i].id == propID)
                        {
                            var propName = battleItemTable[i].model;
                            var propPrefab = prefabMapData.prefabHashMap[propName];
                            propEntity = ecb.Instantiate(sortKey, propPrefab);
                            break;
                        }
                    }

                    break;
            }

            return propEntity;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddEventTo(int eventID, Entity propEntity, EntityCommandBuffer.ParallelWriter ecb,
            GlobalConfigData configData, int sortKey)
        {
            ref var eventsTable = ref configData.value.Value.configTbevent_0s.configTbevent_0s;
            int triggerType = 0, type = 0;
            for (int i = 0; i < eventsTable.Length; i++)
            {
                if (eventsTable[i].id == eventID)
                {
                    triggerType = eventsTable[i].triggerType;
                    type = eventsTable[i].type;
                }
            }

            DynamicBuffer<GameEvent> GameEvent = ecb.AddBuffer<GameEvent>(sortKey, propEntity);
            switch (eventID)
            {
                //case 1001:
                //    ecb.AppendToBuffer(sortKey, propEntity, new GameEvent_1001
                //    {
                //        id = eventID,
                //        triggerType = triggerType,
                //        eventType = type,
                //        triggerGap = 0,
                //        remainTime = 1f,
                //        duration = 0,
                //        isPermanent = true,
                //        target = default,
                //        onceTime = 0,
                //        colliderScale = 0,
                //        delayTime = 1f
                //    }.ToGameEvent());
                //    break;
                //case 1002:
                //    ecb.AppendToBuffer(sortKey, propEntity, new GameEvent_1002
                //    {
                //        id = eventID,
                //        triggerType = triggerType,
                //        eventType = type,
                //        triggerGap = 0,
                //        remainTime = 1f,
                //        duration = 0,
                //        isPermanent = true,
                //        target = default,
                //        onceTime = 0,
                //        colliderScale = 0,
                //        delayTime = 1f
                //    }.ToGameEvent());

                //    break;
                //case 1004:
                //    ecb.AppendToBuffer(sortKey, propEntity, new GameEvent_1004
                //    {
                //        id = eventID,
                //        triggerType = triggerType,
                //        eventType = type,
                //        triggerGap = 0,
                //        remainTime = 1f,
                //        duration = 0,
                //        isPermanent = true,
                //        target = default,
                //        onceTime = 0,
                //        colliderScale = 0,
                //        delayTime = 1f
                //    }.ToGameEvent());
                //    break;
                //case 1005:
                //    ecb.AppendToBuffer(sortKey, propEntity, new GameEvent_1005
                //    {
                //        id = eventID,
                //        triggerType = triggerType,
                //        eventType = type,
                //        triggerGap = 0,
                //        remainTime = 1f,
                //        duration = 0,
                //        isPermanent = true,
                //        target = default,
                //        onceTime = 0,
                //        colliderScale = 0,
                //        delayTime = 1f
                //    }.ToGameEvent());
                //    break;
                ////case 1006:
                ////    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                ////    GameEvent.Add(new GameEvent_1001
                ////    {

                ////        id = 1006,

                ////        isPermanent = true,
                ////        remainTime = 1,
                ////        target = default,

                ////    }.ToGameEvent());
                ////    break;
                //case 1007:
                //    ecb.AppendToBuffer(sortKey, propEntity, new GameEvent_1007
                //    {
                //        id = eventID,
                //        eventType = type,
                //        triggerType = triggerType,
                //        isPermanent = true,
                //        duration = 0f,
                //        remainTime = 1f,
                //        target = default,
                //        triggerGap = 0f,
                //    }.ToGameEvent());
                //    break;
                //case 1008:
                //    ecb.AppendToBuffer(sortKey, propEntity, new GameEvent_1008
                //    {
                //        id = eventID,
                //        eventType = type,
                //        triggerType = triggerType,
                //        isPermanent = true,
                //        duration = 0f,
                //        remainTime = 1f,
                //        target = default,
                //        triggerGap = 2f,
                //        delayTime = 0,
                //    }.ToGameEvent());
                //    break;
                ////case 1009:
                ////    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                ////    GameEvent.Add(new GameEvent_1009
                ////    {
                ////        id = 1009,
                ////        isPermanent = false,
                ////        remainTime = 1,
                ////        target = default,
                ////    }.ToGameEvent());
                ////    break;
                ////case 1010:
                ////    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                ////    GameEvent.Add(new GameEvent_1010
                ////    {
                ////        id = 1010,
                ////        isPermanent = true,
                ////        remainTime = 1,
                ////        target = default,
                ////    }.ToGameEvent());
                ////    break;
                ////case 2002:
                ////    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                ////    GameEvent.Add(new GameEvent_2002
                ////    {

                ////        id = 2002,
                ////        isPermanent = true,
                ////        remainTime = 1,
                ////        target = default,
                ////    }.ToGameEvent());
                ////    break;
                ////case 2003:
                ////    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                ////    GameEvent.Add(new GameEvent_2003
                ////    {

                ////        id = 2003,

                ////        isPermanent = true,
                ////        remainTime = 1,
                ////        target = default,

                ////    }.ToGameEvent());
                ////    break;
                ////case 2004:
                ////    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                ////    GameEvent.Add(new GameEvent_2004
                ////    {

                ////        id = 2004,
                ////        isPermanent = true,
                ////        remainTime = 1,
                ////        target = default,
                ////    }.ToGameEvent());
                ////    break;
                ////case 2005:
                ////    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                ////    GameEvent.Add(new GameEvent_2005
                ////    {
                ////        id = 2005,
                ////        isPermanent = true,
                ////        remainTime = 1,
                ////        target = default,
                ////    }.ToGameEvent());
                ////    break;
            }
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSortNew(DynamicBuffer<Buff> buffer, int left, int right)
        {
            if (left < right)
            {
                int i = left;
                int j = right;
                int middle = buffer[(left + right) / 2].Int32_13;
                while (true)
                {
                    while (i < right && buffer[i].Int32_13 < middle)
                    {
                        i++;
                    }


                    while (j > 0 && buffer[j].Int32_13 > middle)
                    {
                        j--;
                    }

                    if (i == j) break;
                    var temp = buffer[i];
                    buffer[i] = buffer[j];
                    buffer[j] = temp;
                    if (buffer[i].Int32_13 == buffer[j].Int32_13) j--;
                }

                QuickSortNew(buffer, left, i);
                QuickSortNew(buffer, i + 1, right);
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSort(DynamicBuffer<BuffOld> buffer, int left, int right)
        {
            if (left < right)
            {
                int i = left;
                int j = right;
                int middle = buffer[(left + right) / 2].Int32_1;
                while (true)
                {
                    while (i < right && buffer[i].Int32_1 < middle)
                    {
                        i++;
                    }


                    while (j > 0 && buffer[j].Int32_1 > middle)
                    {
                        j--;
                    }

                    if (i == j) break;
                    var temp = buffer[i];
                    buffer[i] = buffer[j];
                    buffer[j] = temp;
                    if (buffer[i].Int32_1 == buffer[j].Int32_1) j--;
                }

                QuickSort(buffer, left, i);
                QuickSort(buffer, i + 1, right);
            }
        }

        /// <summary>
        /// 是否替换buff
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="dirtyBuff">要加的buff</param>
        /// <returns>是否加此buff</returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryAddBuffNew(ref DynamicBuffer<Buff> buffer, Buff dirtyBuff)
        {
            bool canAdd = false;
            if (buffer.Length <= 1)
            {
                return !canAdd;
            }

            if (dirtyBuff.CurrentTypeId != Buff.TypeId.Buff_4 && dirtyBuff.CurrentTypeId != Buff.TypeId.Buff_5)
            {
                return !canAdd;
            }

            var changeType = dirtyBuff.int2_7.x;

            for (var i = 0; i < buffer.Length; i++)
            {
                var buff = buffer[i];
                if (dirtyBuff.Int32_0 == buff.Int32_0)
                {
                    if (changeType == 0)
                    {
                        canAdd = true;
                    }
                    else if (changeType == 1)
                    {
                        if (dirtyBuff.Single_3 > buff.Single_3)
                        {
                            canAdd = true;
                            buffer.RemoveAt(i);
                        }
                    }
                    else if (changeType == 2)
                    {
                        if (dirtyBuff.Single_1 > buff.Single_1)
                        {
                            canAdd = true;
                            buffer.RemoveAt(i);
                        }
                    }

                    break;
                }
            }

            return canAdd;
        }


        /// <summary>
        /// 判断该位置有没有障碍物或者地形危险度大于25的点
        /// </summary>
        /// <param name="randomPoint"></param>
        /// <param name="globalConfig"></param>
        /// <param name="mapModules"></param>
        /// <param name="cdfeMapElementData"></param>
        /// <param name="cdfeLocalTransform"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPosCanUse(float3 randomPoint, GlobalConfigData globalConfig,
            NativeArray<Entity> mapModules, ComponentLookup<MapElementData> cdfeMapElementData,
            ComponentLookup<LocalTransform> cdfeLocalTransform)
        {

        
            ref var scene_module = ref globalConfig.value.Value.configTbscene_modules.configTbscene_modules;
            for (int i = 0; i < mapModules.Length; i++)
            {
                for (int j = 0; j < scene_module.Length; j++)
                {
                    if (scene_module[j].id == cdfeMapElementData[mapModules[i]].elementID)
                    {
                        float width, height;
                        if (scene_module[j].id == 2001)
                        {
                           continue;
                        }
                        else
                        {
                            if (scene_module[j].size.Length < 2)
                            {
                                Debug.LogError(
                                    $"size错误,id为{scene_module[j].id}");
                                continue;
                            }

                            width = scene_module[j].size[0] / 1000f;
                            height = scene_module[j].size[1] / 1000f;
                        }


                        var pos = cdfeLocalTransform[mapModules[i]].Position;
                        int danger = scene_module[j].pathPriority;
                        Rect rect = new Rect(pos.x - width * 0.5f, pos.y - height * 0.5f, width, height);

                        if (!rect.Contains(randomPoint))
                        {
                            break;
                        }
                        else
                        {
                            Debug.LogError(
                                $"Contains,pos{pos},randomPoint{randomPoint},mapModules[i]].elementID{scene_module[j].id}");
                            if ((scene_module[j].id / 1000 == 2) || danger >= 25)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// 判断该位置有没有障碍物或者地形危险度大于25的点
        /// </summary>
        /// <param name="globalConfig"></param>
        /// <param name="mapModules"></param>
        /// <param name="cdfeMapElementData"></param>
        /// <param name="cdfeLocalTransform"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]



        public static bool IsFullyOverlapping(this Rect bigRect, Rect smallRect)
        {
            return bigRect.xMin <= smallRect.xMin &&
                   bigRect.yMin <= smallRect.yMin &&
                   (bigRect.width + bigRect.xMin) >= (smallRect.width + smallRect.xMin) &&
                   (bigRect.height + bigRect.yMin) >= (smallRect.height + smallRect.yMin);
        }

        public static bool IsInBossMap(float3 targetPos,PlayerOtherData playerOtherData)
        {
            //Rect rectNew = new Rect(targetPos.x - cdfeLocalTransform[rectEntity].Scale * 0.5f,
            //    targetPos.y - cdfeLocalTransform[rectEntity].Scale * 0.5f, cdfeLocalTransform[rectEntity].Scale,
            //    cdfeLocalTransform[rectEntity].Scale);
            if (playerOtherData.isBossFight)
            {
                float3 bossScenePos = playerOtherData.bossScenePos;
                var size = bossScenePos.x;
                var bossPos = bossScenePos.yz;
                var walkScene = new Rect(bossPos.x - size * 0.5f, bossPos.y - size * 0.5f, size, size);
                if (!walkScene.Contains(targetPos))
                {
                    return false;
                }
                //if (!IsFullyOverlapping(walkScene, rectNew))
                //{
                   
                //}
                return true;
            }
            else
            {
                return true;
            }
        }

        public static bool Approximately(float a, float b)
        {
            return math.abs(b - a) < math.max(1E-06f * math.max(math.abs(a), math.abs(b)), math.EPSILON * 8f);
        }

        public static bool TryGetIntersectionWithVertical(float2 start, float2 direction, float2 verticalStart, out float2 intersection)
        {
            intersection = float2.zero;

            // 向量 1 的起点 (x1, y1) 和方向 (dx1, dy1)
            float x1 = start.x;
            float y1 = start.y;
            float dx1 = direction.x;
            float dy1 = direction.y;

            // 竖直向量的起点 (x2, y2) 和方向 (0, 1)
            float x2 = verticalStart.x;
            float y2 = verticalStart.y;

            // 如果 dx1 = 0，向量 1 也是竖直的
            if (Approximately(dx1, 0))
            {
                // 检查是否平行或重合
                if (Approximately(x1, x2))
                {
                    // 重合（无数交点），返回 false 或根据需求处理
                    return false;
                }
                // 平行，无交点
                return false;
            }

            // 计算 t = (x2 - x1) / dx1
            float t = (x2 - x1) / dx1;

            // 计算交点的 Y 坐标
            float y = y1 + t * dy1;

            // 交点坐标
            intersection = new float2(x2, y);

            // 验证交点是否在竖直向量上（可选，取决于是否限制 s 的范围）
            // float s = y - y2; // 若需限制 s >= 0，可检查 s 的值

            return true;
        }



        public static bool IsRectCanUse(float3 targetPos, Entity rectEntity, GlobalConfigData globalConfig,
            NativeArray<Entity> mapModules, ComponentLookup<MapElementData> cdfeMapElementData,
            ComponentLookup<LocalTransform> cdfeLocalTransform, ComponentLookup<PhysicsCollider> cdfePhysicsCollider,
            PlayerOtherData playerOtherData, Entity player)
        {
            //float3 sizeRations = float3.zero;

            Rect rectNew = new Rect(targetPos.x - cdfeLocalTransform[rectEntity].Scale*0.5f,
                targetPos.y - cdfeLocalTransform[rectEntity].Scale* 0.5f, cdfeLocalTransform[rectEntity].Scale,
                cdfeLocalTransform[rectEntity].Scale);
           
            var playerRect = new Rect(cdfeLocalTransform[player].Position.x - cdfeLocalTransform[player].Scale*0.5f, cdfeLocalTransform[player].Position.y - cdfeLocalTransform[player].Scale*0.5f, cdfeLocalTransform[player].Scale, cdfeLocalTransform[player].Scale);
            if (playerRect.Overlaps(rectNew))
            {
                return false;
            }
            ref var scene_module = ref globalConfig.value.Value.configTbscene_modules.configTbscene_modules;
            for (int i = 0; i < mapModules.Length; i++)
            {
                if (!cdfeMapElementData.HasComponent(mapModules[i]))
                {
                    continue;
                }

                for (int j = 0; j < scene_module.Length; j++)
                {
                    if (scene_module[j].id == cdfeMapElementData[mapModules[i]].elementID)
                    {
                        if (scene_module[j].size.Length < 2)
                        {
                            continue;
                        }

                        var width = scene_module[j].size[0] / 1000f;
                        var height = scene_module[j].size[1] / 1000f;
                        var pos = cdfeLocalTransform[mapModules[i]].Position;
                        int danger = scene_module[j].pathPriority;
                        unsafe
                        {
                            Unity.Physics.BoxCollider* bxPtr =
                                (Unity.Physics.BoxCollider*)cdfePhysicsCollider[mapModules[i]].ColliderPtr;
                            var sizeRations = bxPtr->Size;
                            width *= sizeRations.x;
                            height *= sizeRations.y;
                        }

                        Rect rect = new Rect(pos.x - width * 0.5f, pos.y - height * 0.5f, width, height);

                        if (!rect.Overlaps(rectNew))
                        {
                            break;
                        }
                        else
                        {
                            if ((scene_module[j].id / 1000 == 2) || danger >= 25)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// 根据PhysicsCollider目标类型找到最近的目标entity
        /// </summary>
        /// <param name="allEntities">所有含有PhysicsCollider</param>
        /// <param name="cdfeLocalTransform"></param>
        /// <param name="cdfeChaStats"></param>
        /// <param name="cdfePhysicsCollider"></param>
        /// <param name="thisEntity">此entity</param>
        /// <param name="distance">探测的最远距离</param>
        /// <returns>目标entity</returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity NerestTarget(NativeArray<Entity> allEntities,
            ComponentLookup<LocalTransform> cdfeLocalTransform, ComponentLookup<TargetData> cdfeTargetData,
            ComponentLookup<ChaStats> cdfeChaStats,
            Entity thisEntity, uint targetGroup, ref float distance)
        {
            Entity target = thisEntity;

            for (var i = 0; i < allEntities.Length; i++)
            {
                if (allEntities[i] == thisEntity || !cdfeTargetData.HasComponent(allEntities[i]))
                {
                    continue;
                }

                var otherBelong = cdfeTargetData[allEntities[i]];
                var thisBelong = cdfeTargetData[thisEntity];
                if (!PhysicsHelper.IsTargetEnabled(targetGroup, otherBelong.BelongsTo))
                {
                    continue;
                }

                float nextDis = math.distance(cdfeLocalTransform[allEntities[i]].Position,
                    cdfeLocalTransform[thisEntity].Position);

                if (cdfeChaStats.HasComponent(allEntities[i]))
                {
                    if (distance > nextDis && cdfeChaStats[allEntities[i]].chaResource.hp > 0)
                    {
                        target = allEntities[i];
                        distance = nextDis;
                    }
                }

                // if (distance > nextDis)
                // {
                //     
                //     target = allEntities[i];
                //     distance = nextDis;
                // }
            }

            if (target == thisEntity)
            {
                return default;
            }

            return target;
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetStateIndex(DynamicBuffer<State> states, Main.State.TypeId typeId)
        {
            foreach (var state in states)
            {
                if (state.CurrentTypeId == typeId)
                {
                    return state.Int32_0;
                }
            }

            //Debug.LogError($"{typeId} not found!");
            return default;
        }

        /// <summary>
        /// 拿到元素反应的参数
        /// </summary>
        /// <param name="configData"></param>
        /// <param name="casterId"></param>
        /// <param name="carrierId"></param>
        /// <param name="elementArgs"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetElementArgs(GlobalConfigData configData, int casterId, int carrierId,
            out NativeList<int> elementArgs)
        {
            elementArgs = new NativeList<int>(5, Allocator.Temp);
            ref var elementEffectConfig =
                ref configData.value.Value.configTbelement_effects.configTbelement_effects;

            for (int j = 0; j < elementEffectConfig.Length; j++)
            {
                if (elementEffectConfig[j].from == casterId && elementEffectConfig[j].id == carrierId)
                {
                    for (int i = 0; i < elementEffectConfig[j].para.Length; i++)
                    {
                        elementArgs.Add(elementEffectConfig[j].para[i]);
                    }

                    return true;
                }
            }

            return false;
        }

        #region 待定

        /// <summary>
        /// 根据读表调整碰撞盒的范围 
        /// TODO:
        /// </summary>
        /// <param name="rangeType"></param>
        /// <param name="rangeTypePar"></param>
        /// <param name="skillPrefab"></param>
        /// <param name="ecb"></param>
        /// <param name="sortKey"></param>
        /// <param name="cdfeLoctransform"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AjustScale(int rangeType, ref BlobArray<int> rangeTypePar, Entity skillPrefab,
            EntityCommandBuffer.ParallelWriter ecb, int sortKey, ComponentLookup<LocalTransform> cdfeLoctransform)
        {
            var loc = cdfeLoctransform[skillPrefab];
            switch (rangeType)
            {
                case 0: break;
                case 1:

                    ecb.AddComponent(sortKey, skillPrefab, new PostTransformMatrix
                    {
                        Value = float4x4.Scale(rangeTypePar[0] / 1000f, rangeTypePar[1] / 1000f, 1)
                    });
                    break;
                case 2:
                    ecb.SetComponent<LocalTransform>(sortKey, skillPrefab,
                        new LocalTransform
                            { Position = loc.Position, Scale = rangeTypePar[1] / 1000f, Rotation = loc.Rotation });
                    break;
                case 3:
                    ecb.SetComponent<LocalTransform>(sortKey, skillPrefab,
                        new LocalTransform
                            { Position = loc.Position, Scale = rangeTypePar[0] / 1000f, Rotation = loc.Rotation });
                    break;
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MergeBuffs(ref DynamicBuffer<BuffOld> buffer)
        {
            if (buffer.Length <= 1)
            {
                return;
            }

            for (var i = 0; i < buffer.Length; i++)
            {
                if (!buffer[i].Boolean_11) continue;

                for (var j = i + 1; j < buffer.Length; j++)
                {
                    if (buffer[i].Int32_0 == buffer[j].Int32_0)
                    {
                        var temp = buffer[i];
                        if (!temp.BuffStack_12.keepOriginalTime)
                        {
                            temp.Single_7 = buffer[j].Single_7;
                        }

                        if (temp.BuffStack_12.stack < temp.Int32_2)
                        {
                            temp.BuffStack_12.stack++;
                        }

                        // if (buffer[j].Int32_16 <= buffer[j].Int32_17)
                        // {
                        //     temp.Int32_16 = buffer[j].Int32_16;
                        // }

                        buffer[i] = temp;
                        buffer.RemoveAt(j);
                    }
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MergeBuffsNew(ref DynamicBuffer<Buff> buffer)
        {
            // if (buffer.IsEmpty)
            // {
            //     return;
            // }

            NativeList<int> deleteList = new NativeList<int>(Allocator.Temp);
            //NativeHashMap<int, int> idValueMap = new NativeHashMap<int, int>(20, Allocator.Temp);
            for (var i = 0; i < buffer.Length; i++)
            {
                var buff0 = buffer[i];
                if (buff0.CurrentTypeId != Buff.TypeId.Buff_2) continue;
                //if (buff0.Int32_2 != 0) continue;
                if (deleteList.Contains(i)) continue;

                int count = 0;
                for (var j = i + 1; j < buffer.Length; j++)
                {
                    var buff1 = buffer[j];
                    if (buff1.CurrentTypeId != Buff.TypeId.Buff_2) continue;
                    if (buff0.Int32_0 != buffer[j].Int32_0) continue;
                    count++;
                    int delIndex = 0;
                    int buff1Value = 0;
                    int buff0Value = 0;
                    switch (buff0.int2_7.x)
                    {
                        //默认为值叠加，时间按照后触发计算
                        case 0:

                            delIndex = buff0.Int32_2 >= buff1.Int32_2 ? j : i;

                            if (deleteList.Contains(delIndex))
                            {
                                continue;
                            }
                            else
                            {
                                deleteList.Add(delIndex);
                            }

                            buff1Value = buff1.Int32_18 == 0 ? buff1.BuffArgsNew_12.args1.y : buff1.Int32_18;
                            buff0Value = buff0.Int32_18 == 0 ? buff0.BuffArgsNew_12.args1.y : buff0.Int32_18;

                            if (delIndex == j)
                            {
                                if (buff0.int2_7.y > 0)
                                {
                                    buff0.Int32_18 = math.min(buff1Value + buff0Value,
                                        buff0.int2_7.y);
                                    buff0.Single_3 = buff1.Single_3;
                                }
                                else
                                {
                                    buff0.Int32_18 = math.max(buff1Value + buff0Value,
                                        buff0.int2_7.y);
                                    buff0.Single_3 = buff1.Single_3;
                                }
                            }
                            else
                            {
                                if (buff1.int2_7.y > 0)
                                {
                                    buff1.Int32_18 = math.min(buff1Value + buff0Value,
                                        buff1.int2_7.y);
                                    buff1.Single_3 = buff0.Single_3;
                                }
                                else
                                {
                                    buff1.Int32_18 = math.max(buff1Value + buff0Value,
                                        buff1.int2_7.y);
                                    buff1.Single_3 = buff0.Single_3;
                                }
                            }


                            break;
                        //相同元素id后触发的完全替换新出发的
                        case 1:

                            delIndex = buff0.Int32_2 >= buff1.Int32_2 ? j : i;

                            if (deleteList.Contains(delIndex))
                            {
                                continue;
                            }
                            else
                            {
                                deleteList.Add(delIndex);
                            }

                            if (delIndex == j)
                            {
                                buff0.Single_3 = buff1.Single_3;
                            }
                            else
                            {
                                buff1.Single_3 = buff0.Single_3;
                            }

                            break;
                        //相同元素id重复触发只叠加值，时间不变
                        case 2:
                            delIndex = buff0.Int32_2 >= buff1.Int32_2 ? j : i;

                            if (deleteList.Contains(delIndex))
                            {
                                continue;
                            }
                            else
                            {
                                deleteList.Add(delIndex);
                            }

                            buff1Value = buff1.Int32_18 == 0 ? buff1.BuffArgsNew_12.args1.y : buff1.Int32_18;
                            buff0Value = buff0.Int32_18 == 0 ? buff0.BuffArgsNew_12.args1.y : buff0.Int32_18;

                            if (delIndex == j)
                            {
                                if (buff0.int2_7.y > 0)
                                {
                                    buff0.Int32_18 = math.min(buff1Value + buff0Value,
                                        buff0.int2_7.y);
                                }
                                else
                                {
                                    buff0.Int32_18 = math.max(buff1Value + buff0Value,
                                        buff0.int2_7.y);
                                }
                            }
                            else
                            {
                                if (buff1.int2_7.y > 0)
                                {
                                    buff1.Int32_18 = math.min(buff1Value + buff0Value,
                                        buff1.int2_7.y);
                                }
                                else
                                {
                                    buff1.Int32_18 = math.max(buff1Value + buff0Value,
                                        buff1.int2_7.y);
                                }
                            }

                            break;
                    }


                    buffer[j] = buff1;
                }

                if (count == 0)
                {
                    buff0.Int32_18 = buff0.BuffArgsNew_12.args1.y;
                    //Debug.Log($"buff0.Int32_0:{buff0.Int32_0} {buff0.Int32_18}");
                }

                buffer[i] = buff0;
            }

            foreach (var dele in deleteList)
            {
                buffer.RemoveAt(dele);
            }
        }


        public static Entity NerestTargetWithState(NativeArray<Entity> allEntities,
            ComponentLookup<LocalTransform> cdfeLocalTransform, ComponentLookup<TargetData> cdfeTargetData,
            ComponentLookup<ChaStats> cdfeChaStats, BufferLookup<Buff> cdfeBuff,
            Entity thisEntity, uint targetGroup, ref float distance, int stateID)
        {
            var target = NerestTargetWithStateFirst(allEntities, cdfeLocalTransform, cdfeTargetData, cdfeChaStats,
                cdfeBuff, thisEntity, targetGroup, ref distance, stateID);

            if (target == thisEntity)
            {
                return NerestTarget(allEntities, cdfeLocalTransform, cdfeTargetData, cdfeChaStats, thisEntity,
                    targetGroup, ref distance);
            }

            return target;
        }


        /// <summary>
        /// 根据PhysicsCollider目标类型找到最近的目标entity
        /// </summary>
        /// <param name="allEntities">所有含有PhysicsCollider</param>
        /// <param name="cdfeLocalTransform"></param>
        /// <param name="cdfeChaStats"></param>
        /// <param name="cdfePhysicsCollider"></param>
        /// <param name="thisEntity">此entity</param>
        /// <param name="distance">探测的最远距离</param>
        /// <returns>目标entity</returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity NerestTargetWithStateFirst(NativeArray<Entity> allEntities,
            ComponentLookup<LocalTransform> cdfeLocalTransform, ComponentLookup<TargetData> cdfeTargetData,
            ComponentLookup<ChaStats> cdfeChaStats, BufferLookup<Buff> cdfeBuff,
            Entity thisEntity, uint targetGroup, ref float distance, int stateID)
        {
            Entity target = thisEntity;

            for (var i = 0; i < allEntities.Length; i++)
            {
                if (allEntities[i] == thisEntity || !cdfeTargetData.HasComponent(allEntities[i]))
                {
                    continue;
                }

                var otherBelong = cdfeTargetData[allEntities[i]];
                var thisBelong = cdfeTargetData[thisEntity];
                if (!PhysicsHelper.IsTargetEnabled(targetGroup, otherBelong.BelongsTo))
                {
                    continue;
                }

                if (!cdfeBuff.HasBuffer(allEntities[i])) continue;
                var buffs = cdfeBuff[allEntities[i]];
                bool isFind = false;
                foreach (var buff in buffs)
                {
                    if (buff.Int32_0 == stateID)
                    {
                        isFind = true;
                        break;
                    }
                }

                if (!isFind)
                {
                    continue;
                }

                float nextDis = math.distance(cdfeLocalTransform[allEntities[i]].Position,
                    cdfeLocalTransform[thisEntity].Position);
                //TODO:
                if (cdfeChaStats.HasComponent(allEntities[i]))
                {
                    if (distance > nextDis && cdfeChaStats[allEntities[i]].chaResource.hp > 0)
                    {
                        target = allEntities[i];
                        distance = nextDis;
                    }
                }

                // if (distance > nextDis)
                // {
                //     
                //     target = allEntities[i];
                //     distance = nextDis;
                // }
            }

            return target;
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetElementArgs(GlobalConfigData configData, ComponentLookup<ElementData> cdfeElementData,
            Entity caster, Entity carrier, out NativeList<int> elementArgs)
        {
            elementArgs = new NativeList<int>(5, Allocator.Temp);
            if (!cdfeElementData.HasComponent(caster) || !cdfeElementData.HasComponent(carrier))
            {
                return false;
            }

            int casterId = cdfeElementData[caster].id;
            int carrierId = cdfeElementData[carrier].id;
            if (TryGetElementArgs(configData, casterId, carrierId, out elementArgs))
            {
                return true;
            }


            return false;
        }

        #endregion


        #region Trigger

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTriggerForBullet(ref SkillAttackData_ReadWrite refData,
            in SkillAttackData_ReadOnly inData,
            int sortkey, int triggerID, Entity caster, Entity target, float3 dir)
        {
			
            if (!refData.storageInfoFromEntity.Exists(caster) || !refData.storageInfoFromEntity.Exists(target))
            {
                return;
            }

            if (triggerID == 0)
            {
                return;
            }

            Debug.Log($"AddTriggerForBullet caster{caster} target{target} ");
            var locer = refData.ecb.CreateEntity(inData.sortkey);
            //Debug.Log($"AddTriggerForBullet triggerID {triggerID}");


            refData.ecb.AddBuffer<Trigger>(inData.sortkey, locer);
            refData.ecb.AddBuffer<Skill>(inData.sortkey, locer);
            refData.ecb.AddBuffer<Buff>(inData.sortkey, locer);
            refData.ecb.AddComponent<TargetData>(inData.sortkey, locer, new TargetData
            {
                tick = 0,
                BelongsTo = (uint)BuffHelper.TargetEnum.Bullet,
                AttackWith = (uint)BuffHelper.TargetEnum.All
            });
            refData.ecb.AddComponent<ChaStats>(inData.sortkey, locer,new ChaStats
            {
                chaProperty = default,
                enviromentProperty = default,
                chaResource = new ChaResource
                {
                    isDead = false,
                    hp = 0,
                    curPushForce = 0,
                    curMoveSpeed = 0,
                    direction = dir,
                    continuousCollCount = 0,
                    actionSpeed = 0,
                    env = default,
                    totalDamage = 0
                },
                chaControlState = default,
                chaImmuneState = default
            });
            refData.ecb.AddComponent(inData.sortkey, locer, new BulletTempTag
            {
                onHitTarget = target,
                dir = dir
            });
            //float3 dir = default;
            float3 pos = default;

            if (!refData.storageInfoFromEntity.Exists(target))
            {
                pos = refData.cdfeLocalTransform[inData.entity].Position;
            }
            else
            {
                //应该走不到
                dir = math.normalizesafe(refData.cdfeLocalTransform[inData.entity].Position -
                                         refData.cdfeLocalTransform[target].Position);

                pos = refData.cdfeLocalTransform[target].Position +
                      dir * (refData.cdfeLocalTransform[target].Scale / 2f);
            }


            pos.z = 0;


            refData.ecb.AddComponent(inData.sortkey, locer, new LocalTransform
            {
                Position = pos,
                Scale = 1,
                Rotation = default
            });
            refData.ecb.AddComponent(inData.sortkey, locer, new TimeToDieData
            {
                duration = 15f
            });

            ref var skillEffect =
                ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            int index = -1;
            for (int i = 0; i < skillEffect.Length; i++)
            {
                if (skillEffect[i].id == triggerID)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                Debug.LogError($"当前技能triggerID skilleffect表不存在 triggerID:{triggerID}");
                return;
            }

            ref var skilleffect = ref skillEffect[index];

            ///先判断是否能添加trigger
            int power = skilleffect.power;
            var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.timeTick + caster.Index +
                                                                       caster.Version));
            var dropRate = math.clamp(power, 0, 10000);

            var canTrigger = rand.NextInt(0, 10001) <= dropRate;
            if (!canTrigger)
                return;

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

            float delay = 0;
            TriggerType type = (TriggerType)skilleffect.triggerType;
            int delayType = skilleffect.delayType;
            float4 triggerTypeArgs = default;
            int4 triggerConditionTypeArgs = default;
            TriggerConditionType condition = default;
            condition = (TriggerConditionType)(skilleffect.conditionType == 4
                ? skilleffect.conditionType * 10 + skilleffect.conditionTypePara[0]
                : skilleffect.conditionType);
            int skillid = skilleffect.skillId;
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
            if (delayType == 4)
            {
                delay = 0;
            }

            triggerTypeArgs.z = triggerTypeArgs.y;
            refData.ecb.AppendToBuffer(sortkey, locer, new Trigger
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
                Int32_10 = skillid,
                Boolean_11 = false,
                Entity_12 = caster,
                Boolean_13 = false,
                Int32_14 = power,
                Single_15 = haloGap,
                Int32_16 = 0,
                Int32_17 = skilleffect.compareType,
                Boolean_18 = false,
                Boolean_19 = false,
                Entity_20 = locer,
                Boolean_21 = false,
                Boolean_22 = false,
                Int32_23 = 0,
                LocalTransform_25 = refData.cdfeLocalTransform[inData.entity],
                Boolean_26 = false,
                Boolean_27 = false,
                Boolean_28 = true,
                float3x2_24 = new float3x2
                {
                    c0 = refData.cdfeLocalTransform[inData.entity].Position,
                    c1 = inData.cdfeChaStats[caster].chaResource.direction
                }
            });

            Debug.Log($"AddTrigger {skilleffect.id}");
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTriggerForExtra(ref SkillAttackData_ReadWrite refData,
            in SkillAttackData_ReadOnly inData,
            int sortkey, int triggerID, Entity caster, Entity target, float3 dir)
        {
            if (!refData.storageInfoFromEntity.Exists(caster) || !refData.storageInfoFromEntity.Exists(target))
            {
                return;
            }

            if (triggerID == 0)
            {
                return;
            }

            Debug.Log($"AddTriggerForExtra caster{caster} target{target} ");

            //float3 dir = default;
            float3 pos = default;

            if (!refData.storageInfoFromEntity.Exists(target))
            {
                pos = refData.cdfeLocalTransform[inData.entity].Position;
            }
            else
            {
                //应该走不到
                dir = math.normalizesafe(refData.cdfeLocalTransform[inData.entity].Position -
                                         refData.cdfeLocalTransform[target].Position);

                pos = refData.cdfeLocalTransform[target].Position +
                      dir * (refData.cdfeLocalTransform[target].Scale / 2f);
            }


            pos.z = 0;


            ref var skillEffect =
                ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            int index = -1;
            for (int i = 0; i < skillEffect.Length; i++)
            {
                if (skillEffect[i].id == triggerID)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                Debug.LogError($"当前技能triggerID skilleffect表不存在 triggerID:{triggerID}");
                return;
            }

            ref var skilleffect = ref skillEffect[index];

            ///先判断是否能添加trigger
            int power = skilleffect.power;
            var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.timeTick + caster.Index +
                                                                       caster.Version));
            var dropRate = math.clamp(power, 0, 10000);

            var canTrigger = rand.NextInt(0, 10001) <= dropRate;
            if (!canTrigger)
                return;


            float delay = 0;
            TriggerType type = (TriggerType)skilleffect.triggerType;
            int delayType = skilleffect.delayType;
            float4 triggerTypeArgs = default;
            int4 triggerConditionTypeArgs = default;
            TriggerConditionType condition = default;
            condition = (TriggerConditionType)(skilleffect.conditionType == 4
                ? skilleffect.conditionType * 10 + skilleffect.conditionTypePara[0]
                : skilleffect.conditionType);
            int skillid = skilleffect.skillId;
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
            if (delayType == 4)
            {
                delay = 0;
            }

            triggerTypeArgs.z = triggerTypeArgs.y;
            refData.ecb.AppendToBuffer(sortkey, caster, new Trigger
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
                Int32_10 = skillid,
                Boolean_11 = false,
                Entity_12 = caster,
                Boolean_13 = false,
                Int32_14 = power,
                Single_15 = haloGap,
                Int32_16 = 0,
                Int32_17 = skilleffect.compareType,
                Boolean_18 = false,
                Boolean_19 = false,
                Entity_20 = caster,
                Boolean_21 = false,
                Boolean_22 = false,
                Int32_23 = 0,
                LocalTransform_25 = refData.cdfeLocalTransform[inData.entity],
                Boolean_26 = false,
                Boolean_27 = false,
                Boolean_28 = true,
                float3x2_24 = new float3x2
                {
                    c0 = refData.cdfeLocalTransform[inData.entity].Position,
                    c1 = inData.cdfeChaStats[caster].chaResource.direction
                }
            });

            Debug.Log($"AddTrigger {skilleffect.id}");
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTriggerForDeathBullet(ref SkillAttackData_ReadWrite refData,
            in SkillAttackData_ReadOnly inData,
            int sortkey, int triggerID, Entity caster, float3 dir)
        {
            if (!refData.storageInfoFromEntity.Exists(caster))
            {
                return;
            }

            if (triggerID == 0)
            {
                return;
            }

            Debug.Log($"AddTriggerForBullet caster{caster} ");
            var locer = refData.ecb.CreateEntity(inData.sortkey);
            //Debug.Log($"AddTriggerForBullet triggerID {triggerID}");


            refData.ecb.AddBuffer<Trigger>(inData.sortkey, locer);
            refData.ecb.AddBuffer<Skill>(inData.sortkey, locer);
            refData.ecb.AddBuffer<Buff>(inData.sortkey, locer);
			
            refData.ecb.AddComponent(inData.sortkey, locer, new BulletTempTag
            {
                dir = dir
            });
            // refData.ecb.AddComponent(inData.sortkey, locer, new BulletTempTag
            // {
            //     onHitTarget = target
            // });
            //float3 dir0 = default;
            float3 pos = default;
            pos = refData.cdfeLocalTransform[inData.entity].Position;
            // if (!refData.storageInfoFromEntity.Exists(target))
            // {
            //     pos = refData.cdfeLocalTransform[inData.entity].Position;
            // }
            // else
            // {
            //     //应该走不到
            //     dir = math.normalizesafe(refData.cdfeLocalTransform[inData.entity].Position -
            //                              refData.cdfeLocalTransform[target].Position);
            //
            //     pos = refData.cdfeLocalTransform[target].Position +
            //           dir * (refData.cdfeLocalTransform[target].Scale / 2f);
            // }


            pos.z = 0;


            refData.ecb.AddComponent(inData.sortkey, locer, new LocalTransform
            {
                Position = pos,
                Scale = 1,
                Rotation = default
            });
            refData.ecb.AddComponent(inData.sortkey, locer, new TimeToDieData
            {
                duration = 15f
            });

            ref var skillEffect =
                ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            int index = -1;
            for (int i = 0; i < skillEffect.Length; i++)
            {
                if (skillEffect[i].id == triggerID)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                Debug.LogError($"当前技能triggerID skilleffect表不存在 triggerID:{triggerID}");
                return;
            }

            ref var skilleffect = ref skillEffect[index];

            ///先判断是否能添加trigger
            int power = skilleffect.power;
            var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.timeTick + caster.Index +
                                                                       caster.Version));
            var dropRate = math.clamp(power, 0, 10000);

            var canTrigger = rand.NextInt(0, 10001) <= dropRate;
            if (!canTrigger)
                return;

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

            float delay = 0;
            TriggerType type = (TriggerType)skilleffect.triggerType;
            int delayType = skilleffect.delayType;
            float4 triggerTypeArgs = default;
            int4 triggerConditionTypeArgs = default;
            TriggerConditionType condition = default;
            condition = (TriggerConditionType)(skilleffect.conditionType == 4
                ? skilleffect.conditionType * 10 + skilleffect.conditionTypePara[0]
                : skilleffect.conditionType);
            int skillid = skilleffect.skillId;
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
            if (delayType == 4)
            {
                delay = 0;
            }

            triggerTypeArgs.z = triggerTypeArgs.y;
            refData.ecb.AppendToBuffer(sortkey, locer, new Trigger
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
                Int32_10 = skillid,
                Boolean_11 = false,
                Entity_12 = caster,
                Boolean_13 = false,
                Int32_14 = power,
                Single_15 = haloGap,
                Int32_16 = 0,
                Int32_17 = skilleffect.compareType,
                Boolean_18 = false,
                Boolean_19 = false,
                Entity_20 = locer,
                Boolean_21 = false,
                Boolean_22 = false,
                Int32_23 = 0,
                LocalTransform_25 = refData.cdfeLocalTransform[inData.entity],
                Boolean_26 = false,
                Boolean_27 = false,
                Boolean_28 = true,
                float3x2_24 = new float3x2
                {
                    c0 = refData.cdfeLocalTransform[inData.entity].Position,
                    c1 = inData.cdfeChaStats[caster].chaResource.direction
                }
            });

            Debug.Log($"AddTrigger {skilleffect.id}");
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTriggerForBulletCanFire(ref SkillAttackData_ReadWrite refData,
            in SkillAttackData_ReadOnly inData,
            int sortkey, int triggerID, Entity caster, Entity target, int sonBulletId, int skillId, int addTime,
            int sonBulletNum, float sonBulletRange)
        {
            var locer = refData.ecb.CreateEntity(inData.sortkey);
            refData.ecb.AddBuffer<Trigger>(inData.sortkey, locer);
            refData.ecb.AddBuffer<Skill>(inData.sortkey, locer);
            refData.ecb.AddBuffer<Buff>(inData.sortkey, locer);


            if (sonBulletId != 0)
            {
                refData.ecb.AddBuffer<BulletCastData>(inData.sortkey, locer);
                refData.ecb.AppendToBuffer(inData.sortkey, locer, new BulletCastData
                {
                    id = sonBulletId,
                    duration = 0,
                    tick = 0,
                    caster = caster,
                    skillId = skillId,
                    addTime = addTime,
                    sonBulletNum = sonBulletNum,
                    sonBulletRange = sonBulletRange
                });
            }

            var dir = math.normalizesafe(refData.cdfeLocalTransform[inData.entity].Position -
                                         refData.cdfeLocalTransform[target].Position);

            var pos = refData.cdfeLocalTransform[target].Position +
                      dir * (refData.cdfeLocalTransform[target].Scale / 2f);
            pos.z = 0;


            refData.ecb.AddComponent(inData.sortkey, locer, new LocalTransform
            {
                Position = pos,
                Scale = 1,
                Rotation = default
            });
            refData.ecb.AddComponent(inData.sortkey, locer, new TimeToDieData
            {
                duration = 15f
            });
            refData.ecb.AddComponent(inData.sortkey, locer, new BulletTempTag
            {
                onHitTarget = target,
                dir = dir
            });

            ref var skillEffect =
                ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
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

            ///先判断是否能添加trigger
            int power = skilleffect.power;
            var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.timeTick + caster.Index +
                                                                       caster.Version));
            var dropRate = math.clamp(power, 0, 10000);

            var canTrigger = rand.NextInt(0, 10001) <= dropRate;
            if (!canTrigger)
                return;

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

            float delay = 0;
            TriggerType type = (TriggerType)skilleffect.triggerType;
            int delayType = skilleffect.delayType;
            float4 triggerTypeArgs = default;
            int4 triggerConditionTypeArgs = default;
            TriggerConditionType condition = default;
            condition = (TriggerConditionType)(skilleffect.conditionType == 4
                ? skilleffect.conditionType * 10 + skilleffect.conditionTypePara[0]
                : skilleffect.conditionType);
            int skillid = skilleffect.skillId;
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

            triggerTypeArgs.z = triggerTypeArgs.y;
            refData.ecb.AppendToBuffer(sortkey, locer, new Trigger
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
                Int32_10 = skillid,
                Boolean_11 = false,
                Entity_12 = caster,
                Boolean_13 = false,
                Int32_14 = power,
                Single_15 = haloGap,
                Int32_16 = 0,
                Int32_17 = skilleffect.compareType,
                Entity_20 = locer
            });
            //Debug.Log($"AddTrigger {skilleffect.id}");
        }

        #endregion

        #region Weapon

        /// <summary>
        /// 设置武器数据
        /// </summary>
        /// <param name="weaponData"></param>
        /// <param name="animPara"></param>
        /// <param name="displayType"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetWeaponData(ref WeaponData weaponData, ref BlobArray<int> animPara, int displayType,
            bool isBossAnim = false)
        {
            if (isBossAnim)
            {
                weaponData.attackTime =
                    (animPara[4] + animPara[6] + animPara[7] + animPara[8]) / 1000f;
                weaponData.repeatTimes = 1;
                weaponData.interval = 0.02f;
                //Debug.Log($"SetWeaponData{ weaponData.attackTime}");
            }
            else
            {
                switch (displayType)
                {
                    case 1:
                        weaponData.attackTime =
                            (animPara[0] + animPara[1] + animPara[5] + animPara[8]) / 1000f;
                        weaponData.repeatTimes = 1;
                        weaponData.interval = 0.02f;
                        break;
                    case 2:
                        weaponData.attackTime =
                            (animPara[0] + animPara[1] + animPara[2] + animPara[7] + animPara[8]) / 1000f;
                        weaponData.repeatTimes = 1;
                        weaponData.interval = 0.02f;
                        break;
                    case 3:
                        weaponData.attackTime =
                            ((animPara[0] + animPara[1] + animPara[2] + animPara[5] + animPara[8]) * animPara[7] +
                             (animPara[6] - 1) * animPara[7]) / 1000f;
                        weaponData.repeatTimes = math.max(animPara[6], 1);
                        weaponData.interval = math.max(animPara[7] / 1000f, 0.02f);
                        break;
                }
            }
        }

        /// <summary>
        /// 设置武器数据
        /// </summary>
        /// <param name="weaponData"></param>
        /// <param name="animPara"></param>
        /// <param name="displayType"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetWeaponData(ref WeaponData weaponData, ref List<int> animPara, int displayType)
        {
            switch (displayType)
            {
                case 1:
                    weaponData.attackTime =
                        (animPara[0] + animPara[1] + animPara[5] + animPara[8]) / 1000f;
                    weaponData.repeatTimes = 1;
                    weaponData.interval = 0.02f;
                    break;
                case 2:
                    weaponData.attackTime =
                        (animPara[0] + animPara[1] + animPara[2] + animPara[7] + animPara[8]) / 1000f;
                    weaponData.repeatTimes = 1;
                    weaponData.interval = 0.02f;
                    break;
                case 3:
                    weaponData.attackTime =
                        (animPara[0] + animPara[1] + animPara[2] + animPara[5] + animPara[8]) / 1000f;
                    weaponData.repeatTimes = math.max(animPara[6], 1);
                    weaponData.interval = math.max(animPara[7] / 1000f, 0.02f);
                    break;
                case 4:
                    weaponData.secondAnimTime =
                        (animPara[4] + animPara[6] + animPara[7] + animPara[8]) / 1000f;
                    weaponData.repeatTimes = 1;
                    weaponData.interval = 0.02f;
                    break;
                case 5:
                    weaponData.secondAnimTime =
                        (animPara[1] + animPara[3] + animPara[5]) / 1000f + 10;
                    weaponData.repeatTimes = 1;
                    weaponData.interval = 0.02f;
                    break;
            }
        }

        #endregion

        /// <summary>
        /// 加状态 状态不可叠加
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        /// <param name="states">状态id集合</param>
        /// <param name="carrier">携带者</param>
        /// <param name="caster">释放者</param>
        /// <param name="skillid">该状态对应的skillid</param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddStates(ref SkillAttackData_ReadWrite refData,
            SkillAttackData_ReadOnly inData, ref BlobArray<int2> states, Entity carrier, Entity caster, int skillid)
        {
            bool isHasBuff = false;
            if (refData.bfeBuff.HasBuffer(carrier))
            {
                isHasBuff = true;
            }

            ref var statesConfig =
                ref inData.config.value.Value.configTbbattle_statuss.configTbbattle_statuss;
            for (int i = 0; i < states.Length; i++)
            {
                for (int j = 0; j < statesConfig.Length; j++)
                {
                    //Debug.Log($"Addstat:{states[i].x}");
                    var stateId = states[i].x;
                    if (statesConfig[j].id == stateId)
                    {
                        var duration = states[i].y / 1000f;
                        bool isContains = false;
                        if (isHasBuff)
                        {
                            var buffs = refData.bfeBuff[carrier];
                            foreach (var buff in buffs)
                            {
                                if (buff.Int32_0 == stateId)
                                {
                                    Debug.Log("states isContains");
                                    isContains = true;
                                    break;
                                }
                            }
                        }

                        if (isContains) continue;
                        refData.ecb.AppendToBuffer<Buff>(inData.sortkey, carrier,
                            new Buff
                            {
                                CurrentTypeId = (Buff.TypeId)stateId,
                                Int32_0 = stateId,
                                Single_3 = duration,
                                Entity_6 = carrier,
                                Entity_5 = caster,
                                Int32_20 = skillid,
                            });
                    }
                }
            }
        }
    }
}
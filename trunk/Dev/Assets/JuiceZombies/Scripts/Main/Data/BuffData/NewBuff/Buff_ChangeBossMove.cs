//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    /// <summary>
    /// 更改boss的移动轨迹
    /// new
    /// buffargs 0:id 1:增加值 2:正负面类型
    /// </summary>
    public partial struct Buff_ChangeBossMove : IBuff
    {
        ///<summary>0
        ///buff的id
        ///</summary>
        public int id;

        ///<summary>1
        ///buff添加时间点/刻 替换优先级
        ///</summary>
        public float startTime;

        ///<summary>2
        ///buff已经存在了多少时间了，单位：帧
        ///</summary>
        public int timeElapsed;

        ///<summary>3
        ///剩余多久，单位：秒
        ///</summary>
        public float duration;

        ///<summary>4
        ///是否是一个永久的buff，永久的duration不会减少，但是timeElapsed还会增加
        ///</summary>
        public bool permanent;

        ///<summary>5
        ///buff的施法者是谁，当然可以是空的
        ///</summary>
        public Entity caster;

        ///<summary>6
        ///buff的携带者，实际上是作为参数传递给脚本用，具体是谁，可定是所在控件的this.gameObject了
        ///</summary>
        public Entity carrier;

        /// <summary>7
        /// 替换类型 用于同类buff的替换 仅用于免疫和控制和属性变更
        /// </summary>
        public int2 changeType;

        /// <summary>8
        /// 是否能够清除 用于不同类buff的替换 仅用于控制类
        /// new字段
        /// </summary>
        public int canClear;

        /// <summary>9
        /// 护盾层数 仅作用于免疫
        /// new
        /// </summary>
        public int immuneStack;

        /// <summary>10
        /// 从 effect传入的的方向
        /// new
        /// </summary>
        public float3 direction;

        /// <summary>11
        /// buff的状态类型 分无状态 正面状态和负面状态
        /// </summary>
        public int buffState;

        /// <summary>12
        /// float4-1:output 2:bonus_type 3:calc_type and pra
        /// </summary>
        public BuffArgsNew argsNew;

        ///<summary>13
        ///buff的优先级，优先级越低的buff越后面执行，这是一个非常重要的属性
        ///比如经典的“吸收50点伤害”和“受到的伤害100%反弹给攻击者”应该反弹多少，取决于这两个buff的priority谁更高
        ///</summary>
        public int priority;

        /// <summary>14
        /// 元素的概率 
        /// </summary>
        public int power;

        /// <summary>15
        /// 用来处理ontick的逻辑 =0时不执行ontick 
        /// </summary>
        public int tickTime;

        /// <summary>16
        /// 用来处理移动伤害的逻辑  第一个float3的x为每移动多少米 y为当前移动总距离 z为1时该移动伤害才生效！！！！
        /// 第二个记录上帧的位置 
        /// </summary>
        public float3x2 distancePar;

        /// <summary>17
        /// 是否是弹幕
        /// </summary>
        public bool isBullet;

        /// <summary>
        ///  18 buff2里上一份属性变更的值
        /// </summary>
        public int oldValue;

        /// <summary>
        /// 19 是否禁用特效
        /// </summary>
        public bool isDisableSpecial;

        /// <summary>
        /// 20 技能id
        /// </summary>
        public int skillId;

        private bool isHitBackAttackOld;


        //记录移动目标的位置
        private float3 targetPostionHelp;

        public void OnOccur(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            if (!refData.cdfeChaStats.HasComponent(carrier) || !refData.cdfeEnemyData.HasComponent(carrier))
            {
                return;
            }

            //var data = refData.cdfeEnemyData[carrier];
            //isHitBackAttackOld = data.isHitBackAttack;
            //enemyAttackTypeOld=data.attackType;

            //if (argsNew.args1.x == 1)
            //{
            //    data.attackType = EnemyAttackType.CollideAttack;
            //    data.isHitBackAttack = false;
            //}
            //else if (argsNew.args1.x == 2)
            //{
            //    data.attackType = EnemyAttackType.CollideAttack;
            //    data.isHitBackAttack = true;
            //}
            //refData.cdfeEnemyData[carrier] = data;
        }


        public void OnRemoved(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            if (!refData.cdfeChaStats.HasComponent(carrier) || !refData.cdfeEnemyData.HasComponent(carrier))
            {
                return;
            }

            //var data = refData.cdfeEnemyData[carrier];
            //data.attackType =enemyAttackTypeOld;
            //data.isHitBackAttack = isHitBackAttackOld;
            //refData.cdfeEnemyData[carrier]=data;
        }

        public void OnUpdate(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            if (!refData.cdfeEnemyData.HasComponent(carrier)) return;
            var seekTarget = refData.cdfeEnemyData[carrier].target;
            int moveType = argsNew.args1.x,
                moveType1 = argsNew.args2.x,
                moveType2 = argsNew.args3.x,
                moveType3 = argsNew.args4.x;
            bool targetExist = refData.storageInfoFromEntity.Exists(seekTarget);
            var targetData = inData.cdfeTargetData[carrier];
            if (targetExist)
            {
                var targetTran = inData.cdfeLocalTransform[seekTarget];
                var myTran = inData.cdfeLocalTransform[carrier];

                var agent = refData.cdfeAgentBody[carrier];
                var locomontion = refData.cdfeAgentLocomotion[carrier];

                agent.SetDestination(targetTran.Position);
                locomontion.StoppingDistance = (targetTran.Scale + myTran.Scale / 2f) / 2f;

                switch ((EnemyMoveType)moveType)
                {
                    case EnemyMoveType.Default:
                        agent.SetDestination(inData.cdfeLocalTransform[seekTarget].Position);
                        break;
                    case EnemyMoveType.FollowPlayerShadow:
                        if (moveType2 == 0)
                        {
                            targetPostionHelp = inData.cdfeLocalTransform[seekTarget].Position;
                        }
                        else
                        {
                            if (targetData.tick >= 0 &&
                                targetData.tick % (uint)(moveType2 / inData.fdT) == 0)
                            {
                                targetPostionHelp = inData.cdfeLocalTransform[seekTarget].Position;
                            }
                        }

                        agent.SetDestination(targetPostionHelp);

                        break;
                    case EnemyMoveType.FollowPlayerShadowWithOffset:
                        if (moveType2 == 0)
                        {
                            targetPostionHelp = inData.cdfeLocalTransform[seekTarget].Position;
                        }
                        else
                        {
                            if (targetData.tick >= 0 &&
                                targetData.tick % (uint)(moveType2 / inData.fdT) == 0)
                            {
                                targetPostionHelp = inData.cdfeLocalTransform[seekTarget].Position;
                            }
                            else
                            {
                                var deltaPosition = inData.cdfeLocalTransform[seekTarget].Position -
                                                    targetPostionHelp;
                                if (math.pow(deltaPosition.x, 2) + math.pow(deltaPosition.y, 2) +
                                    math.pow(deltaPosition.z, 2) != 0)
                                {
                                    targetPostionHelp += moveType3 * 0.02f *
                                                         math.normalizesafe(
                                                             inData.cdfeLocalTransform[seekTarget]
                                                                 .Position - targetPostionHelp);
                                }
                            }
                        }

                        agent.SetDestination(targetPostionHelp);
                        break;
                    case EnemyMoveType.Escape:
                        if (moveType2 == 0)
                        {
                            targetPostionHelp = inData.cdfeLocalTransform[seekTarget].Position;
                            var toTargetVector = refData.cdfeLocalTransform[carrier].Position - targetPostionHelp;
                            if (math.length(toTargetVector) < moveType3)
                            {
                                targetPostionHelp = inData.cdfeLocalTransform[seekTarget].Position +
                                                    math.normalizesafe(toTargetVector) * (moveType3);
                                bool overMap = false;
                                for (int i = 0; i < 48; i++)
                                {
                                    targetPostionHelp += toTargetVector * 500;
                                    if (BuffHelper.IsPosCanUse(targetPostionHelp, inData.globalConfigData,
                                            inData.mapModules,
                                            inData.cdfeMapElementData, inData.cdfeLocalTransform))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (i == 47)
                                        {
                                            overMap = true;
                                        }
                                    }

                                    if (inData.gameOthersData.mapData.mapType == 1)
                                    {
                                        if (targetPostionHelp.x > inData.gameOthersData.mapData.mapSize.x / 2 ||
                                            targetPostionHelp.x < inData.gameOthersData.mapData.mapSize.x / 2)
                                        {
                                            overMap = true;
                                            break;
                                        }
                                    }
                                    else if (inData.gameOthersData.mapData.mapType == 3)
                                    {
                                        if (targetPostionHelp.x > inData.gameOthersData.mapData.mapSize.x / 2 ||
                                            targetPostionHelp.x < inData.gameOthersData.mapData.mapSize.x / 2 ||
                                            targetPostionHelp.y > inData.gameOthersData.mapData.mapSize.y / 2 ||
                                            targetPostionHelp.y < inData.gameOthersData.mapData.mapSize.y / 2)
                                        {
                                            overMap = true;
                                            break;
                                        }
                                    }
                                }

                                bool noPostion = false;
                                if (overMap)
                                {
                                    var random =
                                        Unity.Mathematics.Random.CreateFromIndex(inData.timeTick +
                                            (uint)inData.entity.Index);
                                    for (int i = 0; i < 10; i++)
                                    {
                                        var posHelp = random.NextFloat2(0, 1);
                                        targetPostionHelp.x =
                                            inData.cdfeLocalTransform[seekTarget].Position.x +
                                            posHelp.x * moveType3;
                                        targetPostionHelp.y =
                                            inData.cdfeLocalTransform[seekTarget].Position.y +
                                            posHelp.y * moveType3;
                                        if (BuffHelper.IsPosCanUse(targetPostionHelp, inData.globalConfigData,
                                                inData.mapModules, inData.cdfeMapElementData,
                                                inData.cdfeLocalTransform))
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if (i == 9)
                                            {
                                                noPostion = true;
                                            }
                                        }
                                    }
                                }

                                if (noPostion)
                                {
                                    targetPostionHelp = inData.cdfeLocalTransform[seekTarget].Position;
                                }
                            }
                        }
                        else
                        {
                            if (targetData.tick >= 0 &&
                                targetData.tick % (uint)(moveType2 / inData.fdT) == 0)
                            {
                                targetPostionHelp = inData.cdfeLocalTransform[seekTarget].Position;
                                var toTargetVector = refData.cdfeLocalTransform[carrier].Position - targetPostionHelp;
                                if (math.length(toTargetVector) < moveType3)
                                {
                                    targetPostionHelp = inData.cdfeLocalTransform[seekTarget].Position +
                                                        math.normalizesafe(toTargetVector) * (moveType3);
                                    bool overMap = false;
                                    for (int i = 0; i < 48; i++)
                                    {
                                        targetPostionHelp += toTargetVector * 500;
                                        if (BuffHelper.IsPosCanUse(targetPostionHelp, inData.globalConfigData,
                                                inData.mapModules, inData.cdfeMapElementData,
                                                inData.cdfeLocalTransform))
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if (i == 47)
                                            {
                                                overMap = true;
                                            }
                                        }

                                        if (inData.gameOthersData.mapData.mapType == 1)
                                        {
                                            if (targetPostionHelp.x > inData.gameOthersData.mapData.mapSize.x / 2 ||
                                                targetPostionHelp.x < inData.gameOthersData.mapData.mapSize.x / 2)
                                            {
                                                overMap = true;
                                                break;
                                            }
                                        }
                                        else if (inData.gameOthersData.mapData.mapType == 3)
                                        {
                                            if (targetPostionHelp.x > inData.gameOthersData.mapData.mapSize.x / 2 ||
                                                targetPostionHelp.x < inData.gameOthersData.mapData.mapSize.x / 2 ||
                                                targetPostionHelp.y > inData.gameOthersData.mapData.mapSize.y / 2 ||
                                                targetPostionHelp.y < inData.gameOthersData.mapData.mapSize.y / 2)
                                            {
                                                overMap = true;
                                                break;
                                            }
                                        }
                                    }

                                    bool noPostion = false;
                                    if (overMap)
                                    {
                                        var random =
                                            Unity.Mathematics.Random.CreateFromIndex(inData.timeTick +
                                                (uint)inData.entity.Index);
                                        for (int i = 0; i < 10; i++)
                                        {
                                            var posHelp = random.NextFloat2(0, 1);
                                            targetPostionHelp.x =
                                                inData.cdfeLocalTransform[seekTarget].Position.x +
                                                posHelp.x * moveType3;
                                            targetPostionHelp.y =
                                                inData.cdfeLocalTransform[seekTarget].Position.y +
                                                posHelp.y * moveType3;
                                            if (BuffHelper.IsPosCanUse(targetPostionHelp, inData.globalConfigData,
                                                    inData.mapModules, inData.cdfeMapElementData,
                                                    inData.cdfeLocalTransform))
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                if (i == 9)
                                                {
                                                    noPostion = true;
                                                }
                                            }
                                        }
                                    }

                                    if (noPostion)
                                    {
                                        targetPostionHelp =
                                            inData.cdfeLocalTransform[seekTarget].Position;
                                    }
                                }
                            }
                        }

                        agent.SetDestination(targetPostionHelp);
                        break;
                    case EnemyMoveType.KeepDis:
                        int timeGapTick = (int)((moveType1 / 1000f) / inData.fdT);
                        float radius = moveType2 / 1000f;
                        float angle = moveType3;
                        if (targetData.tick > 0 &&
                            targetData.tick % timeGapTick == 0)
                        {
                            int maxTimes = 0;
                            float3 pos = default;
                            do
                            {
                                var dir = math.normalizesafe(
                                    inData.cdfeLocalTransform[seekTarget].Position -
                                    refData.cdfeLocalTransform[carrier].Position);
                                var randomAngle = Unity.Mathematics.Random.CreateFromIndex((uint)(tickTime + 1))
                                    .NextFloat(-angle, angle + 0.1f);
                                var dirNew = new float3(MathHelper.RotateVector(dir.xy, randomAngle), 0);
                                pos = refData.cdfeLocalTransform[carrier].Position + dirNew * radius;

                                maxTimes++;
                                if (maxTimes > 120)
                                {
                                    var randomR = Unity.Mathematics.Random.CreateFromIndex((uint)(tickTime + 1))
                                        .NextFloat(0, radius);
                                    var randomA = Unity.Mathematics.Random.CreateFromIndex((uint)(tickTime + 1))
                                        .NextFloat(0, angle);
                                    dirNew = new float3(MathHelper.RotateVector(dir.xy, randomA), 0);
                                    pos = refData.cdfeLocalTransform[carrier].Position + dirNew * randomR;
                                }
                            } while (!BuffHelper.IsPosCanUse(pos, inData.globalConfigData, inData.mapModules,
                                         inData.cdfeMapElementData, inData.cdfeLocalTransform));

                            agent.SetDestination(pos);
                        }

                        break;
                    //仅用于boss
                    case EnemyMoveType.KeepPlayerOnLine:
                        var playerPos = inData.cdfeLocalTransform[inData.player].Position;
                        var dirL = new float3(-1, 0, 0);
                        var dirR = new float3(1, 0, 0);
                        var leftPos = playerPos + dirL * moveType1 / 1000f + new float3(0, 1, 0) * moveType2 / 1000f;
                        var rightPos = playerPos + dirR * moveType1 / 1000f + new float3(0, 1, 0) * moveType2 / 1000f;
                        var bossPos = inData.cdfeLocalTransform[inData.entity].Position;
                        if (math.distance(leftPos, bossPos) > math.distance(rightPos, bossPos))
                        {
                            //朝右边走
                            agent.SetDestination(rightPos);
                        }
                        else
                        {
                            //朝左边走
                            agent.SetDestination(leftPos);
                        }

                        break;
                }

                refData.cdfeAgentBody[carrier] = agent;
                refData.cdfeAgentLocomotion[carrier] = locomontion;
            }
        }

        public void OnTick(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            //Entity attaker = default;
            //var damage = new Damage
            //{
            //    normal = 0,
            //    bullet = 0,
            //    collide = 0,
            //    environment = 0
            //};
            //if (inData.cdfeMapElementData.HasComponent(caster))
            //{
            //    damage.environment = buffArgs.args0;
            //}
            //else
            //{
            //    damage.normal = buffArgs.args0;
            //    attaker = caster;
            //}

            //refData.ecb.AppendToBuffer(inData.sortkey, inData.wbe, new DamageInfo
            //{
            //    attacker = attaker,
            //    defender = carrier,
            //    tags = new DamageInfoTag
            //    {
            //        directDamage = true,
            //        periodDamage = false,
            //        reflectDamage = false,
            //        copyDamage = false,
            //        directHeal = false,
            //        periodHeal = false
            //    },
            //    damage = damage,
            //    criticalRate = 0,
            //    criticalDamage = 0,
            //    hitRate = 1,
            //    degree = 0,
            //    pos = default,
            //    bulletEntity = default,
            //});
        }

        public void OnHit(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeHurt(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnKill(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeforeBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnPerUnitMove(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            //ref battleConstConfig=ref inData.globalConfigData.value.Value.configtbb
            //float moveDis = 5000 / 1000f;
            //float moveDamage = 200 / 10000f * refData.cdfeChaStats[carrier].chaProperty.
        }
    }
}
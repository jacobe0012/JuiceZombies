using Unity.Mathematics;
using UnityEngine;


namespace Main
{
    public partial struct LittleMonsterMove : IState
    {
        ///<summary>0
        ///动画ID
        ///</summary>
        public int stateId;

        ///<summary>1
        ///存在了多少时间了，单位：秒
        ///</summary>
        public float timeElapsed;

        ///<summary>2
        ///倍速，1=100%，0.1=10%是最小值
        ///</summary>
        public float timeScale;

        ///<summary>3
        ///剩余多少时间，单位：秒
        ///</summary>
        public float duration;

        ///<summary>4
        ///持续多少帧了，单位：帧
        ///</summary>
        public int tick;

        ///<summary>5
        ///是否是一次性播放动画
        ///</summary>
        public bool isOneShotAnim;


        /// <summary>
        /// 记录目标上一次移动的目标点
        /// </summary>
        public float3 lastPosition;

        /// <summary>
        /// 索敌随机半径
        /// </summary>
        //public float randomRadius;
        public int tbmonster_weaponsIndex;


        public void OnStateEnter(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            refData.stateMachine.curAnim = AnimationEnum.Run;

            ref var tbmonster_weapons = ref
                inData.configData.value.Value.configTbweapons.configTbweapons;
            tbmonster_weaponsIndex = -1;
            for (int i = 0; i < tbmonster_weapons.Length; i++)
            {
                if (tbmonster_weapons[i].id == refData.enemyData.weaponId)
                {
                    tbmonster_weaponsIndex = i;
                    break;
                }
            }

            //ref var monster_weapon = ref tbmonster_weapons[tbmonster_weaponsIndex];
        }

        public void OnStateExit(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
        }

        /// <summary>
        /// 状态转换函数
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnStateUpdate(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            if (refData.chaStats.chaResource.hp <= 0)
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.LittleMonsterDie);
                return;
            }

            if (inData.cdfeHitBackData.HasComponent(inData.entity))
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.LittleMonsterGetHit);
                return;
            }

            if (refData.chaStats.IsStrongControl())
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.LittleMonsterBeControlled);
                return;
            }

            bool canAttack = true;
            if (!refData.chaStats.chaImmuneState.immuneControl)
            {
                if (refData.chaStats.chaControlState.cantWeaponAttack)
                {
                    canAttack = false;
                }
            }

            refData.physicsVelocity.Angular = 0;
            var targetData = inData.cdfeTargetData[inData.entity];
            float distance = 999f;

            // if (!refData.storageInfoFromEntity.Exists(refData.enemyData.target) ||
            //     inData.cdfeChaStats[refData.enemyData.target].chaResource.hp < 0)
            // {
            refData.enemyData.target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
                inData.cdfeTargetData,
                inData.cdfeChaStats, inData.entity, refData.enemyData.targetGroup,
                ref distance);

            bool targetExist = refData.storageInfoFromEntity.Exists(refData.enemyData.target);


            if (math.length(refData.chaStats.chaResource.direction) < math.EPSILON)
            {
                if (refData.storageInfoFromEntity.Exists(refData.enemyData.target))
                {
                    refData.chaStats.chaResource.direction = math.normalizesafe(
                        inData.cdfeLocalTransform[refData.enemyData.target].Position - refData.localTransform.Position);
                }
                else
                {
                    refData.chaStats.chaResource.direction = MathHelper.picForward;
                }
            }

            refData.AgentLocomotion.Speed = (refData.chaStats.chaProperty.maxMoveSpeed / 1000f) *
                                            inData.gameTimeData.logicTime.gameTimeScale;
            ref var tbmonster_weapons = ref
                inData.configData.value.Value.configTbweapons.configTbweapons;

            ref var monster_weapon = ref tbmonster_weapons[tbmonster_weaponsIndex];


            refData.enemyData.attackRadius = refData.localTransform.Scale +
                                             (refData.chaStats.chaProperty.scaleRatios / 10000f) *
                                             (monster_weapon.para2 / 1000f);

            if (refData.enemyData.attackType != EnemyAttackType.None &&
                refData.enemyData.attackType != EnemyAttackType.CollideAttack)
            {
                if (targetExist && math.length(refData.localTransform.Position -
                                               inData.cdfeLocalTransform[refData.enemyData.target].Position) <
                    refData.enemyData.attackRadius && refData.enemyData.curCommonSkillCd < 0 && canAttack)
                {
                    refData.enemyData.curCommonSkillCd = refData.enemyData.commonSkillCd;
                    refData.stateMachine.transitionToStateIndex =
                        BuffHelper.GetStateIndex(inData.states, State.TypeId.LittleMonsterAttack);
                    return;
                }
            }

            refData.enemyData.curCommonSkillCd -= (inData.fdT * timeScale);

            int moveType1 = refData.enemyData.moveTypePara.x,
                moveType2 = refData.enemyData.moveTypePara.y,
                moveType3 = refData.enemyData.moveTypePara.z,
                moveType4 = refData.enemyData.moveTypePara.w;


            if (targetExist)
            {
                //var lastTempPosition = lastPosition;

                var myTran = inData.cdfeLocalTransform[inData.entity];
                switch (refData.enemyData.moveType)
                {
                    case EnemyMoveType.Default:
                        var targetTran = inData.cdfeLocalTransform[refData.enemyData.target];

                        lastPosition = targetTran.Position;
                        refData.AgentLocomotion.StoppingDistance = (targetTran.Scale + myTran.Scale / 2f) / 2f;

                        if (TryHandleStack(ref refData, in inData, out var tempPos))
                        {
                            lastPosition = tempPos;
                        }

                        break;
                    case EnemyMoveType.FollowPlayerShadow:

                        if (targetData.tick % (uint)((moveType2 / 1000f) / inData.fdT) == 0)
                        {
                            lastPosition = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                        }


                        break;
                    case EnemyMoveType.FollowPlayerShadowWithOffset:
                        if (moveType2 == 0)
                        {
                            lastPosition = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                        }
                        else
                        {
                            if (targetData.tick >= 0 &&
                                targetData.tick % (uint)((moveType2 / 1000f) / inData.fdT) == 0)
                            {
                                lastPosition = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                            }
                            else
                            {
                                var deltaPosition = inData.cdfeLocalTransform[refData.enemyData.target].Position -
                                                    lastPosition;
                                if (math.pow(deltaPosition.x, 2) + math.pow(deltaPosition.y, 2) +
                                    math.pow(deltaPosition.z, 2) != 0)
                                {
                                    lastPosition += (moveType3 / 1000f) * 0.02f *
                                                    math.normalizesafe(
                                                        inData.cdfeLocalTransform[refData.enemyData.target]
                                                            .Position - lastPosition);
                                }
                            }
                        }

                        break;
                    case EnemyMoveType.Escape:
                        var randomRadius = moveType4 / 1000f;
                        if (targetData.tick % (uint)((moveType2 / 1000f) / inData.fdT) == 0)
                        {
                            var targetPos = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                            const float Interval = 6f;
                            var disRadius = moveType3 / 1000f;
                            var dis = math.length(refData.localTransform.Position - targetPos);
                            var dir = math.normalize(refData.localTransform.Position - targetPos);

                            if (dis > disRadius + Interval || dis < disRadius)
                            {
                                lastPosition = targetPos + (disRadius) * dir;
                            }
                        }

                        if (math.length(lastPosition - myTran.Position) < randomRadius)
                        {
                            lastPosition = myTran.Position;
                        }

                        break;
                    case EnemyMoveType.KeepDis:
                        int timeGapTick = (int)((moveType1 / 1000f) / inData.fdT);
                        float radius = moveType2 / 1000f;
                        float angle = moveType3;
                        if (targetData.tick > 0 && targetData.tick % timeGapTick == 0)
                        {
                            int maxTimes = 0;
                            float3 pos = default;
                            do
                            {
                                var dir = math.normalizesafe(
                                    inData.cdfeLocalTransform[refData.enemyData.target].Position -
                                    refData.localTransform.Position);
                                var randomAngle = Unity.Mathematics.Random.CreateFromIndex((uint)(tick + 1))
                                    .NextFloat(-angle, angle + 0.1f);
                                var dirNew = new float3(MathHelper.RotateVector(dir.xy, randomAngle), 0);
                                pos = refData.localTransform.Position + dirNew * radius;

                                maxTimes++;
                                if (maxTimes > 120)
                                {
                                    var randomR = Unity.Mathematics.Random.CreateFromIndex((uint)(tick + 1))
                                        .NextFloat(0, radius);
                                    var randomA = Unity.Mathematics.Random.CreateFromIndex((uint)(tick + 1))
                                        .NextFloat(0, angle);
                                    dirNew = new float3(MathHelper.RotateVector(dir.xy, randomA), 0);
                                    pos = refData.localTransform.Position + dirNew * randomR;
                                }
                            } while (!BuffHelper.IsPosCanUse(pos, inData.configData, inData.mapModules,
                                         inData.cdfeMapElementData, inData.cdfeLocalTransform));

                            lastPosition = pos;
                        }

                        break;
                    case EnemyMoveType.RandomSelect:
                    //refData.agentBody.SetDestination(lastPosition);
                    case EnemyMoveType.WalkStraight:
                        refData.chaStats.chaControlState.bossCanMove = true;
                        if (targetData.tick % (uint)((moveType2 / 1000f) / inData.fdT) == 0)
                        {
                            refData.chaStats.chaControlState.bossCanMove = false;
                            var temp = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                            var dis = refData.chaStats.chaProperty.maxMoveSpeed * moveType2 / 1000f;
                            float3 pos = inData.cdfeLocalTransform[inData.entity].Position;
                            //玩家在上方
                            if (temp.y >= myTran.Position.y)
                            {
                                pos = inData.cdfeLocalTransform[inData.entity].Position + new float3(0, 1, 0) * dis;
                            }
                            else
                            {
                                pos = inData.cdfeLocalTransform[inData.entity].Position + new float3(0, -1, 0) * dis;
                            }

                            lastPosition = pos;
                        }

                        break;
                }

                refData.agentBody.SetDestination(lastPosition);
                var curPos = myTran.Position;
                if (math.length(curPos - lastPosition) < 2f)
                {
                    refData.stateMachine.curAnim = AnimationEnum.Idle;
                }
                else
                {
                    refData.stateMachine.curAnim = AnimationEnum.Run;
                }
            }
        }


        bool TryHandleStack(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData, out float3 pos)
        {
            const float StuckThreshold = 5f;
            const float ReverseTime = 5f;
            const float StuckRange = 4f;
            pos = default;
            if (refData.enemyData.enemyMove.isReversing)
            {
                // 处理反向移动
                refData.enemyData.enemyMove.reverseTimer += inData.fdT;
                if (refData.enemyData.enemyMove.reverseTimer >= ReverseTime)
                {
                    refData.enemyData.enemyMove.isReversing = false;
                    refData.enemyData.enemyMove.reverseTimer = 0f;
                    // 恢复正常移动行为
                }
                else
                {
                    // 向反方向移动
                    var targetTran = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                    var myTran = inData.cdfeLocalTransform[inData.entity].Position;
                    var dir = math.normalizesafe(targetTran - myTran);
                    pos = -dir * 50f;
                    return true;
                    //refData.agentBody.SetDestination(-dir * 50f);
                }
            }
            else
            {
                // 检测是否卡住
                var curPos = inData.cdfeLocalTransform[inData.entity].Position;
                if (math.distance(curPos,
                        refData.enemyData.enemyMove.lastPosition) < StuckRange)
                {
                    refData.enemyData.enemyMove.stuckTimer += inData.fdT;
                    if (refData.enemyData.enemyMove.stuckTimer >= StuckThreshold)
                    {
                        refData.enemyData.enemyMove.isStuck = true;
                        refData.enemyData.enemyMove.stuckTimer = 0f;
                        // 启动反向移动
                        refData.enemyData.enemyMove.isReversing = true;
                        Debug.Log($"检测怪物卡住,启动怪物反向移动");
                    }
                }
                else
                {
                    // 移动正常，重置计时器
                    refData.enemyData.enemyMove.stuckTimer = 0f;
                    refData.enemyData.enemyMove.lastPosition = curPos;
                }
            }

            return false;
        }
    }
}
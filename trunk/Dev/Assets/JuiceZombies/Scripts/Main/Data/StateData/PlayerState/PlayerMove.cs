//---------------------------------------------------------------------
// UnicornStudio
// Author: 迅捷蟹
// Time: 2023-07-28 15:58:22
//---------------------------------------------------------------------


using Unity.Collections;
using Unity.Mathematics;

namespace Main
{
    public partial struct PlayerMove : IState
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


        public void OnStateEnter(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            refData.stateMachine.curAnim = AnimationEnum.Run;
            refData.physicsVelocity.Angular = 0;
        }

        public void OnStateExit(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
        }

        public void OnStateUpdate(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            //refData.playerData.recoverAccTime -= inData.fdT;

            if (refData.chaStats.chaResource.hp <= 0)
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.PlayerDie);
                return;
            }

            if (inData.cdfeHitBackData.HasComponent(inData.entity))
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.PlayerGetHit);
                return;
            }
            if (refData.chaStats.IsStrongControl(true))
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.PlayerBeController);
                return;
            }

            if (math.length(inData.inputs.Move) < math.EPSILON)
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.PlayerIdle);
                return;
            }

            float speedRatios = 1f;
            if (math.length(inData.inputs.Move) > math.EPSILON)
            {
                refData.chaStats.chaResource.direction = new float3(inData.inputs.Move.x, inData.inputs.Move.y, 0);
                if (!refData.chaStats.chaImmuneState.immuneControl)
                {
                    if (refData.chaStats.chaControlState.counterMove)
                    {
                        refData.chaStats.chaResource.direction = -refData.chaStats.chaResource.direction;
                    }

                    if (refData.chaStats.chaControlState.cantMove)
                    {
                        speedRatios = 0;
                    }
                }
            }


            ref var config =
                ref inData.configData.value.Value.configTbbattle_constants.configTbbattle_constants;


            int battle_speed_converted_force_factor = 0;
            int battle_speed_converted_atk = 0;
            int battle_force_factor = 0;
            for (int i = 0; i < config.Length; i++)
            {
                if (config[i].constantName == (FixedString128Bytes)"battle_speed_converted_force_factor")
                {
                    battle_speed_converted_force_factor = config[i].constantValue;
                }

                if (config[i].constantName == (FixedString128Bytes)"battle_speed_converted_atk")
                {
                    battle_speed_converted_atk = config[i].constantValue;
                }

                if (config[i].constantName == (FixedString128Bytes)"battle_force_factor")
                {
                    battle_force_factor = config[i].constantValue;
                }
            }

            // refData.chaStats.chaProperty.acceleration = (int)(refData.chaStats.defaultProperty.acceleration *
            //                                                   (1 + refData.chaStats.chaProperty.maxMoveSpeedPlus /
            //                                                       10000f));
            //TODO:更改maxMoveSpeedRatios时 需要改变maxMoveSpeed
            // refData.chaStats.chaProperty.maxMoveSpeed = (int)(refData.playerData.chaProperty.maxMoveSpeed *
            //                                                   (1 + refData.chaStats.chaProperty.maxMoveSpeedRatios /
            //                                                       10000f));

            if (math.length(inData.inputs.Move) > math.EPSILON)
            {
                refData.chaStats.chaResource.curMoveSpeed = refData.chaStats.chaProperty.maxMoveSpeed * speedRatios *
                                                            inData.gameTimeData.logicTime.gameTimeScale;

                if (refData.chaStats.chaControlState.softForce && math.length(refData.physicsVelocity.Linear) <=
                    refData.chaStats.chaProperty.maxMoveSpeed / 1000f)
                {
                    refData.physicsVelocity.Linear +=
                        (50 * inData.fdT) * refData.chaStats.chaResource.direction;
                }
                else
                {
                    refData.physicsVelocity.Linear =
                        (refData.chaStats.chaResource.curMoveSpeed / 1000f) * refData.chaStats.chaResource.direction;
                }
            }
            // else
            // {
            //     // if (refData.playerData.recoverAccTime > 0)
            //     // {
            //     // refData.chaStats.chaResource.curMoveSpeed -= (refData.chaStats.chaProperty.acceleration) *
            //     //                                              inData.fdT;
            //     // refData.chaStats.chaResource.curMoveSpeed = math.clamp(refData.chaStats.chaResource.curMoveSpeed, 0,
            //     //     refData.chaStats.chaProperty.maxMoveSpeed);
            //     //}
            //     // else
            //     // {
            //     // refData.chaStats.chaResource.curMoveSpeed = 0;
            //     // if (!refData.chaStats.chaImmuneState.immuneControl && !refData.chaStats.chaControlState.softForce)
            //     // {
            //     //     refData.physicsVelocity.Linear = 0;
            //     // }
            //
            //
            //     // if (refData.chaStats.chaResource.curMoveSpeed / 100f > 0)
            //     // {
            //     //     refData.physicsVelocity.Linear -=
            //     //         (refData.chaStats.chaResource.curMoveSpeed / 100f) * refData.chaStats.chaResource.direction;
            //     // }
            //     // refData.physicsVelocity.Linear =
            //     //     (refData.chaStats.chaResource.curMoveSpeed / 1000f) * refData.chaStats.chaResource.direction;
            //     // var buffs = inData.bfeBuff[inData.entity];
            //     // bool notMove = false;
            //     // foreach (var buff in buffs)
            //     // {
            //     //     if (buff.CurrentTypeId == Buff.TypeId.Buff_NotLetVIsZero)
            //     //     {
            //     //         notMove = true;
            //     //     }
            //     // }
            //
            //
            //     //refData.physicsVelocity.Linear = 0;
            //     // refData.physicsVelocity.Linear =
            //     //     refData.chaStats.chaResource.curMoveSpeed * refData.chaStats.chaResource.direction;
            //     // refData.localTransform.Position += (refData.chaStats.TotalProperty.curMoveSpeed / 100f) *
            //     //                                    refData.chaStats.chaResource.direction *
            //     //                                    inData.fdT;
            //
            //
            //     //Debug.Log($"{refData.chaStats.chaProperty.pushForcePlus}");
            //     //定义 玩家推力F=玩家速度属性(实时变化)*武器推力系数(装备表配置值)*推力修正系数(constans表中常量值)
            //     //TODO: *武器推力系数(装备表配置值)
            //
            //
            //     // refData.chaStats.chaProperty.curPushForce = math.max(
            //     //     (refData.chaStats.TotalProperty.curMoveSpeed / 100f) *
            //     //     (battle_force_factor / 10000f),
            //     //     refData.chaStats.TotalProperty.pushForce);
            // }


            // if (refData.chaStats.chaProperty.curCoolDown < 0)
            // {
            //     refData.chaStats.chaProperty.curCoolDown = refData.chaStats.TotalProperty.coolDown;
            // }
            //
            // refData.chaStats.chaProperty.curCoolDown -= (int)(inData.fdT * timeScale * 1000);
        }
    }
}
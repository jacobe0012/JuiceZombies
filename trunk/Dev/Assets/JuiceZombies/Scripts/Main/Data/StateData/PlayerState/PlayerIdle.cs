//---------------------------------------------------------------------
// UnicornStudio
// Author: 迅捷蟹
// Time: 2023-07-28 15:58:20
//---------------------------------------------------------------------


using Unity.Mathematics;

namespace Main
{
    public partial struct PlayerIdle : IState
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
            refData.stateMachine.curAnim = AnimationEnum.Idle;
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
            
            if (math.length(inData.inputs.Move) > math.EPSILON)
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.PlayerMove);
                return;
            }




            if (!refData.chaStats.chaImmuneState.immuneControl && refData.chaStats.chaControlState.softForce)
            {
                // if (math.length(refData.physicsVelocity.Linear) <=
                //     refData.chaStats.chaProperty.maxMoveSpeed / 1000f)
                // {
                //     refData.physicsVelocity.Linear +=
                //         (50 * inData.fdT) * refData.chaStats.chaResource.direction;
                // }
            }
            else
            {
                refData.chaStats.chaResource.curMoveSpeed = 0;
                refData.physicsVelocity.Linear =
                    (refData.chaStats.chaResource.curMoveSpeed / 1000f) * refData.chaStats.chaResource.direction;
            }


            // if (refData.chaStats.chaProperty.curCoolDown < 0)
            // {
            //     refData.chaStats.chaProperty.curCoolDown = refData.chaStats.TotalProperty.coolDown;
            // }
            //
            // refData.chaStats.chaProperty.curCoolDown -= (int)(inData.fdT * timeScale * 1000);
        }
    }
}
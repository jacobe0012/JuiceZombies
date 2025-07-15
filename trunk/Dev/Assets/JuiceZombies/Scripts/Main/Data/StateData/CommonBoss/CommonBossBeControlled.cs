using Unity.Mathematics;
using Unity.Collections;
namespace Main
{
    public partial struct CommonBossBeControlled : IState
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

        private int lowSpeed;
        public void OnStateEnter(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            refData.stateMachine.curAnim = AnimationEnum.Run;

            ref var config = ref inData.configData.value.Value.configTbbattle_constants.configTbbattle_constants;
            int status_lock_speed = 0;

            for (int i = 0; i < config.Length; i++)
            {
                if (config[i].constantName == (FixedString128Bytes)"status_lock_speed")
                {
                    status_lock_speed = config[i].constantValue;
                    break;
                }
            }
            lowSpeed=status_lock_speed;
            refData.AgentLocomotion.Speed =lowSpeed/1000f;

            if (refData.chaStats.chaControlState.counterMove)
            {
                if (refData.storageInfoFromEntity.Exists(refData.enemyData.target))
                {
                    var targetPos = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                    var myPos = inData.cdfeLocalTransform[inData.entity].Position;
                    var dir = math.normalizesafe(myPos - targetPos);
                    refData.chaStats.chaResource.direction = dir;
                    refData.agentBody.SetDestination(dir * 50f + myPos);
                }
            }

            if (refData.chaStats.chaControlState.forcedmove.y)
            {
                if (refData.storageInfoFromEntity.Exists(refData.enemyData.target))
                {
                    var targetPos = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                    var myPos = inData.cdfeLocalTransform[inData.entity].Position;
                    var dir = math.normalizesafe(targetPos - myPos);
                    refData.chaStats.chaResource.direction = dir;
                    refData.agentBody.SetDestination(targetPos);
                }
            }

            if (refData.chaStats.chaControlState.forcedmove.x)
            {
                if (refData.storageInfoFromEntity.Exists(refData.enemyData.target))
                {
                    var targetPos = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                    var myPos = inData.cdfeLocalTransform[inData.entity].Position;
                    var dir = math.normalizesafe(myPos - targetPos);
                    refData.chaStats.chaResource.direction = dir;
                    refData.agentBody.SetDestination(dir * 50f + myPos);
                }
            }

            if (refData.chaStats.chaControlState.cantMove)
            {
                refData.stateMachine.curAnim = AnimationEnum.Idle;
                refData.agentBody.Stop();
                refData.AgentLocomotion.Speed = 0;
                refData.physicsVelocity.Linear = 0;
                refData.physicsVelocity.Angular = 0;
            }
        }

        public void OnStateExit(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            refData.AgentLocomotion.Speed =refData.chaStats.chaProperty.maxMoveSpeed/1000f;
        }

        public void OnStateUpdate(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            if (refData.chaStats.chaResource.hp <= 0)
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.CommonBossDie);
                return;
            }

            if (!refData.chaStats.IsStrongControl())
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.CommonBossMove);
                return;
            }

            if (inData.cdfeHitBackData.HasComponent(inData.entity))
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.LittleMonsterGetHit);
                return;
            }

            /*
            if (refData.chaStats.chaControlState.counterMove)
            {
                if (refData.storageInfoFromEntity.Exists(refData.enemyData.target))
                {
                    var targetPos = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                    var myPos = inData.cdfeLocalTransform[inData.entity].Position;
                    var dir = math.normalizesafe(myPos - targetPos);
                    refData.chaStats.chaResource.direction = dir;
                    refData.agentBody.SetDestination(dir * 50f + myPos);
                }
            }

            if (refData.chaStats.chaControlState.forcedmove.y)
            {
                if (refData.storageInfoFromEntity.Exists(refData.enemyData.target))
                {
                    var targetPos = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                    var myPos = inData.cdfeLocalTransform[inData.entity].Position;
                    var dir = math.normalizesafe(targetPos - myPos);
                    refData.chaStats.chaResource.direction = dir;
                    refData.agentBody.SetDestination(targetPos);
                }
            }

            if (refData.chaStats.chaControlState.forcedmove.x)
            {
                if (refData.storageInfoFromEntity.Exists(refData.enemyData.target))
                {
                    var targetPos = inData.cdfeLocalTransform[refData.enemyData.target].Position;
                    var myPos = inData.cdfeLocalTransform[inData.entity].Position;
                    var dir = math.normalizesafe(myPos - targetPos);
                    refData.chaStats.chaResource.direction = dir;
                    refData.agentBody.SetDestination(dir * 50f + myPos);
                }
            }*/
            /*
            if (refData.chaStats.chaControlState.cantMove)
            {
                //refData.stateMachine.curAnim = AnimationEnum.Idle;
                refData.agentBody.Stop();
                refData.physicsVelocity.Linear = 0;
            }*/
        }
    }
}
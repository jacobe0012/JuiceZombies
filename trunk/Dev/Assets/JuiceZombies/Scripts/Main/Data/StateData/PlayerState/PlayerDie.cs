//---------------------------------------------------------------------
// UnicornStudio
// Author: 迅捷蟹
// Time: 2023-07-28 15:58:20
//---------------------------------------------------------------------


using Unity.Mathematics;
using Unity.Physics;

namespace Main
{
    public partial struct PlayerDie : IState
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
            refData.stateMachine.curAnim = AnimationEnum.Die;


            refData.chaStats.chaResource.isDead = true;
            refData.chaStats.chaResource.curMoveSpeed = 0;
            refData.agentBody.Stop();
            refData.physicsVelocity = new PhysicsVelocity
            {
                Linear = float3.zero,
                Angular = float3.zero
            };

            var newentity = refData.ecb.CreateEntity(inData.sortkey);
            refData.ecb.AddComponent(inData.sortkey, newentity, new HybridEventData
            {
                type = 4,
                args = new float4(1, 0, 0, 0)
            });
            //死亡武器动画
            var entityGroup = inData.cdfeEntityGroupData[inData.entity];
            var temp = refData.cdfeWeaponData[entityGroup.weaponEntity];
            temp.curAttackTime = temp.secondAnimTime;
            temp.curRepeatTimes = temp.repeatTimes;
            refData.cdfeWeaponData[entityGroup.weaponEntity] = temp;
            //refData.physicsCollider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.None);

            //refData.ecb.RemoveComponent<PhysicsCollider>(inData.sortkey, inData.entity);

            //refData.agentShape.Radius = -1;

            //refData.ecb.SetBuffer<Skill>(inData.sortkey, inData.entity);
        }

        public void OnStateExit(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
        }

        public void OnStateUpdate(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            if (!refData.chaStats.chaResource.isDead)
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.PlayerIdle);
                return;
            }
        }
    }
}
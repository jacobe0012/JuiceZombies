using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    public partial struct CommonBossGetHit : IState
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

        public float hitTime;
        public float a;

        public void OnStateEnter(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            //refData.stateMachine.curAnim = AnimationEnum.GetHit;

            refData.agentBody.Stop();

            ref var config = ref inData.configData.value.Value.configTbbattle_constants.configTbbattle_constants;
            ref var environment = ref inData.configData.value.Value.configTbenvironments.Get(103);

            int battle_force_factor = 0;
            int battle_monster_restore_time_min = 0;
            int battle_monster_restore_unit = 0;
             int battle_player_restore_unit11 =environment.bossPara[0];
            for (int i = 0; i < config.Length; i++)
            {
                if (config[i].constantName == (FixedString128Bytes)"battle_force_factor")
                {
                    battle_force_factor = config[i].constantValue;
                }

                if (config[i].constantName == (FixedString128Bytes)"battle_monster_restore_time_min")
                {
                    battle_monster_restore_time_min = config[i].constantValue;
                }

                if (config[i].constantName == (FixedString128Bytes)"battle_monster_restore_unit")
                {
                    battle_monster_restore_unit = config[i].constantValue;
                }
                
                
            }

            var chaStats = inData.cdfeChaStats[inData.entity];
            chaStats.chaProperty.mass = math.max(chaStats.chaProperty.mass, 1);


            refData.chaStats.chaResource.curMoveSpeed = math.length(refData.physicsVelocity.Linear) * 1000f;
            refData.chaStats.chaResource.direction = math.normalizesafe(refData.physicsVelocity.Linear);

            //var attackerChaStats = inData.cdfeChaStats[attacker];
            //预计僵直时间 = 速度/推力修正系数/m(角色)*角色的 每单位速度硬直时间 ）+ 基础硬直时间
            hitTime = (refData.chaStats.chaResource.curMoveSpeed) / (battle_force_factor / 10000f) /
                chaStats.chaProperty.mass *
                (battle_monster_restore_unit / 1000f) + battle_monster_restore_time_min / 1000f;
            if (refData.playerData.playerOtherData.isBossFight && inData.gameEnviromentData.env.weather == 103)
            {
                 hitTime *=(1+battle_player_restore_unit11/10000f) ;
            }

            
            a = (refData.chaStats.chaResource.curMoveSpeed / 1000f) / hitTime;

            float physicsDamping = (2 * (a / 100f)) / hitTime;
            refData.physicsDamping.Linear = physicsDamping;
            //Debug.Log($"refData.chaStats.chaResource.curMoveSpeed{refData.chaStats.chaResource.curMoveSpeed}111speedRecoveryTime{refData.chaStats.chaProperty.speedRecoveryTime}a{a} time{hitTime} physicsDamping{physicsDamping}");

            //Debug.Log($"{refData.chaStats.chaResource.curMoveSpeed}");
        }

        public void OnStateExit(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            refData.ecb.RemoveComponent<HitBackData>(inData.sortkey, inData.entity);
        }

        public void OnStateUpdate(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            //Debug.Log($"refData.chaStats.chaResource.curMoveSpeed{refData.chaStats.chaResource.curMoveSpeed} ");

            hitTime -= inData.fdT;
            //Debug.Log($"{hitTime}");
            if (refData.chaStats.chaResource.hp <= 0)
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.CommonBossDie);
                return;
            }

            // if (refData.chaStats.IsStrongControl())
            // {
            //     refData.stateMachine.transitionToStateIndex =
            //         BuffHelper.GetStateIndex(inData.states, State.TypeId.CommonBossBeControlled);
            //     return;
            // }

            if (refData.chaStats.chaResource.curMoveSpeed < math.EPSILON ||
                math.length(refData.physicsVelocity.Linear) < math.EPSILON)
            {
                refData.chaStats.chaResource.continuousCollCount = 0;
                refData.ecb.RemoveComponent<HitBackData>(inData.sortkey, inData.entity);
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.CommonBossMove);
                refData.physicsVelocity.Angular = 0;
                refData.physicsVelocity.Linear = 0;
                refData.physicsDamping.Linear = 0.35f;

                return;
            }

            refData.physicsVelocity.Angular = 0;
            //Debug.Log($"{refData.chaStats.chaResource.curMoveSpeed}");
            refData.chaStats.chaResource.curMoveSpeed -= a * 1000f *
                                                         inData.fdT;


            // refData.physicsVelocity.Linear =
            //     (refData.chaStats.chaResource.curMoveSpeed / 100f) * refData.chaStats.chaResource.direction;
        }
    }
}
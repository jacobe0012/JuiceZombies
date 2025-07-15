using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    public partial struct CommonBossAttack : IState
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


        public float3 attackDir;

        public int curSkillId;

        public void OnStateEnter(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            Debug.Log($"bossattack OnStateEnter ");
            var rand = Unity.Mathematics.Random.CreateFromIndex(inData.timeTick + (uint)inData.entity.Index);

            if (!UnityHelper.TryChooseACommonBossSkill(ref refData.Skills, inData.configData,
                    rand, refData.enemyData.enemyID, true,
                    out int skillId))
            {
                Debug.Log($"bossattack Attack2Move ");
                refData.enemyData.curCommonSkillCd = 0.5f;
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.CommonBossMove);
                return;
            }

            refData.stateMachine.curAnim = AnimationEnum.Idle;
         
            var entityGroupData = inData.cdfeEntityGroupData[inData.entity];
            if (refData.storageInfoFromEntity.Exists(entityGroupData.weaponEntity))
            {
                var temp = refData.cdfeWeaponData[entityGroupData.weaponEntity];
                temp.curAttackTime = temp.attackTime;
                //temp.curRepeatTimes = temp.repeatTimes;
                refData.cdfeWeaponData[entityGroupData.weaponEntity] = temp;
            }

            refData.agentBody.Stop();

            Debug.Log($"bossattack skillId {skillId}");
            curSkillId = skillId;
        }

        public void OnStateExit(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
        }

        public void OnStateUpdate(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            refData.AgentLocomotion.Speed = (refData.chaStats.chaProperty.maxMoveSpeed / 1000f) *
                                            inData.gameTimeData.logicTime.gameTimeScale;
            if (refData.chaStats.chaResource.hp <= 0)
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.CommonBossDie);
                return;
            }

            foreach (var skill in refData.Skills)
            {
                if (skill.Int32_0 == curSkillId && skill.Single_8 < 0)
                {
                    refData.stateMachine.animSpeedScale = 1;
                    refData.stateMachine.transitionToStateIndex =
                        BuffHelper.GetStateIndex(inData.states, State.TypeId.CommonBossMove);
                    return;
                }
            }
        }
    }
}
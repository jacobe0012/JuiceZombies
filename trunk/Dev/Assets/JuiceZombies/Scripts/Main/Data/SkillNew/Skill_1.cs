using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main
{
    /// <summary>
    /// 棒球棍的技能逻辑
    /// </summary>
    public partial struct Skill_1 : ISkill
    {
        ///<summary>0
        ///技能的id
        ///</summary>
        public int id;

        ///<summary>1
        ///技能等级 暂时未使用
        ///</summary>
        public int level;

        ///<summary>2
        ///冷却时间 受冷却减免影响
        ///</summary>
        public float cooldown;


        ///<summary>3
        ///持续时间，单位：秒
        ///</summary>
        public float duration;


        ///<summary>4
        ///倍速，1=100%，0.1=10%是最小值
        ///</summary>
        public float timeScale;

        ///<summary>5
        ///技能的施法者
        ///</summary>
        public Entity caster;

        ///<summary>6
        ///技能已经运行了多少帧了 技能到时间会重置 无需赋值
        ///</summary>
        public int tick;

        /// <summary>7
        /// 技能当前冷却时间 无需赋值
        /// </summary>
        public float curCooldown;

        ///<summary>8
        ///剩余时间，单位：秒 无需赋值
        ///</summary>
        public float curDuration;

        ///<summary>9
        ///技能从创建到现在总的tick
        ///</summary>
        public int totalTick;

        ///<summary>10
        ///技能类型
        /// 0:循环(玩家类)  1:单次(事件类)   2:选择(通用boss类)
        ///</summary>
        public int skillType;

        ///<summary>11
        ///是否禁用 技能如果被禁用则不会执行
        ///</summary>
        public bool isDisable;

        ///<summary>12
        ///一次性释放技能的坐标 废弃
        ///</summary>
        public float3 pos;

        /// <summary>
        /// 13...args.c0 x存id y存次数
        /// args.c1
        /// args.c2
        /// args.c3
        /// </summary>
        public int2x4 args;

        /// <summary>
        /// 14 x=当前技能的次数 
        /// </summary>
        public int3 skillTimes;

        /// <summary>
        /// 15 存储skill相关的trigger的索敌位置 仅用于呲水枪
        /// </summary>
        public LocalTransform targetPos;


        ///<summary>
        ///16 生成此技能时销毁某个id特效
        ///</summary>
        public int destorySpId;

        ///<summary>
        ///17 是否是被动技能
        ///</summary>
        public bool isPassiveSkill;
        
        
        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
            skillTimes.x++;

            if (inData.cdfeEnemyData.HasComponent(caster) && inData.cdfeEnemyData[caster].canCastSkill)
            {
                ref var skillEffectConfig =
                    ref inData.globalConfig.value.Value.configTbskill_effectNews.configTbskill_effectNews;
                ReturnMinTrigger(inData, out int minID);
                for (int i = 0; i < skillEffectConfig.Length; i++)
                {
                    if (skillEffectConfig[i].skillId == minID)
                    {
                        if (skillEffectConfig[i].triggerType == 4)
                        {
                            BuffHelper.SwitchDiyStateForBoss(BuffHelper.DiyState.MOVE, refData.cdfeAgentBody[caster],
                                refData.cdfeStateMachine[caster], inData.cdfeLocalTransform[inData.player].Position);
                            break;
                        }
                    }
                }
            }
        }

        private void ReturnMinTrigger(TimeLineData_ReadOnly inData, out int minID)
        {
            minID = 99999;
            for (int i = 0; i < inData.triggers.Length; i++)
            {
                minID = minID < inData.triggers[0].Int32_0 ? minID : inData.triggers[0].Int32_0;
            }
        }

        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
        }


        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
        }
    }
}
using cfg.config;
using System.Globalization;
using System;
using Unity.Mathematics;

namespace Main
{
    /// <summary>
    ///升级武器品质，如果已达到，角色触发事件
    /// </summary>
    public partial struct GameEvent_52 : IGameEvent
    {
        /// <summary>
        ///事件id 0
        /// </summary>
        public int id;

        /// <summary>
        /// 触发事件类型id 1
        /// </summary>
        public GameEventTriggerType triggerType;

        /// <summary>
        /// 触发间隔时间 2
        /// 当 触发事件类型为 间隔触发(3)时用该参数
        /// </summary>
        public float triggerGap;

        /// <summary>
        /// 指定时间(2)触发时间点 3
        /// </summary>
        public float onceTime;

        /// <summary>
        /// 剩余执行时间 4
        /// </summary>
        public float remainTime;

        /// <summary>
        /// 已经执行时间的tick  5
        /// </summary>
        public int durationTick;

        /// <summary>
        /// 是否永久事件 6
        /// 永久存在
        /// </summary>
        public bool isPermanent;

        /// <summary>
        /// 参数123   7
        /// </summary>
        public int3 args123;

        /// <summary>
        /// 参数456   8
        /// </summary>
        public int3 args456;

        /// <summary>
        /// 数量      9
        /// </summary>
        public int num;


        /// <summary>
        /// 触发次数上限 10
        /// </summary>
        public int maxLimit;

        /// <summary>
        /// 是否启用 11
        /// </summary>
        public bool isActive;

        /// <summary>
        /// 是否是环境事件 12
        /// </summary>
        public bool isEnvEvent;

        /// <summary>
        /// 是否是随机事件 13
        /// </summary>
        public bool isRandomEvent;

        /// <summary>
        /// 参数789   14
        /// </summary>
        public int3 args789;

        /// <summary>
        /// 技能id   15
        /// </summary>
        public int skillId;

        public void OnBeDie(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnBeHurt(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCharacterEnter(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            UpdateWeaponQuality(ref refData, inData);
        }

        private void UpdateWeaponQuality(ref GameEventData_ReadWrite refData, GameEventData_ReadOnly inData)
        {
            //int weaponQuality = refData.cdfePlayerData[inData.player].playerOtherData.weaponQuality;

            int weaponQuality = refData.cdfePlayerData[inData.player].playerOtherData.weaponId % 100;

            if (weaponQuality >= args123.x)
            {
                int index = -1;
                ref var eventConfig = ref inData.config.value.Value.configTbevent_0s.configTbevent_0s;
                for (int i = 0; i < eventConfig.Length; i++)
                {
                    if (eventConfig[i].id == args123.y)
                    {
                        index = i;
                        break;
                    }
                }

                refData.ecb.AppendToBuffer<GameEvent>(inData.sortKey, inData.player,
                    new GameEvent { CurrentTypeId = (GameEvent.TypeId)eventConfig[index].type, Int32_0 = args123.y });
            }
            else
            {
                //TODO:改当前品质
                var skills = refData.cdfeSkills[inData.player];
                var triggers = refData.cdfeTriggers[inData.player];
                var buffs = refData.cdfeBuffs[inData.player];
                var gameevents = refData.cdfeGameEvent;
                int weaponSkillid = refData.cdfePlayerData[inData.player].playerOtherData.weaponSkillId;
                var playdata = refData.cdfePlayerData[inData.player];

                BuffHelper.RemoveOldSkill(weaponSkillid, ref skills, ref triggers, ref buffs, ref gameevents);

                ref var qualityconfig = ref inData.config.value.Value.configTbequip_qualitys.configTbequip_qualitys;
                int newSkillID = ReturnNewWeaponSkill(inData, weaponSkillid, weaponQuality);
                
                playdata.playerOtherData.weaponId /= 100;
                playdata.playerOtherData.weaponId *= 100;
                playdata.playerOtherData.weaponId += 11;
                //playdata.playerOtherData.weaponQuality = 11;
                
                playdata.playerOtherData.weaponSkillId = newSkillID;
                refData.cdfePlayerData[inData.player] = playdata;

                BuffHelper.AddSkillByEcb(ref refData.ecb, inData.sortKey, newSkillID, inData.player, 0);
                ref var skillConfig = ref inData.config.value.Value.configTbskills.configTbskills;
                for (int i = 0; i < skillConfig.Length; i++)
                {
                    if (skillConfig[i].id == newSkillID)
                    {
                        if (skillConfig[i].chargedSkill.Length > 0)
                        {
                            int chargeid = skillConfig[i].chargedSkill[0].y;
                            BuffHelper.AddSkillByEcb(ref refData.ecb, inData.sortKey, chargeid, inData.player, 0);
                        }
                    }
                }
            }
        }

        private int ReturnNewWeaponSkill(GameEventData_ReadOnly inData, int oldweaponSkillid, int oldweaponQuality)
        {
            ref var skillConfig = ref inData.config.value.Value.configTbskills.configTbskills;
            int tempSkillid = oldweaponSkillid + (6 - oldweaponQuality) * 10;
            for (int i = 0; i < skillConfig.Length; i++)
            {
                if (skillConfig[i].id == tempSkillid)
                {
                    return tempSkillid;
                }
            }

            tempSkillid -= 10;
            for (int i = 0; i < skillConfig.Length; i++)
            {
                if (skillConfig[i].id == tempSkillid)
                {
                    return tempSkillid;
                }
            }

            tempSkillid -= 10;
            for (int i = 0; i < skillConfig.Length; i++)
            {
                if (skillConfig[i].id == tempSkillid)
                {
                    return tempSkillid;
                }
            }

            return -1;
        }

        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            UpdateWeaponQuality(ref refData, inData);
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            UpdateWeaponQuality(ref refData, inData);
        }


        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }
    }
}
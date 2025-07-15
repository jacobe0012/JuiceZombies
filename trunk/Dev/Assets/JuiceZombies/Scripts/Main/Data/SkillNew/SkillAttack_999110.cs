using System;
using System.Globalization;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    //血之结界
    public partial struct SkillAttack_999110 : ISkillAttack
    {
        /// <summary>
        /// 0
        /// </summary>
        public int id;

        /// <summary>
        /// 1 一次性释放的技能实体 值=0
        /// </summary>
        public float duration;

        /// <summary>
        /// 2
        /// </summary>
        public int tick;

        /// <summary>
        /// 3 技能实体释放者
        /// </summary>
        public Entity caster;

        /// <summary>
        /// 4 是否是弹幕
        /// </summary>
        public bool isBullet;

        /// <summary>
        /// 5 弹幕hp 初始hp 当前hp
        /// </summary>
        public int hp;

        /// <summary>
        /// 实体的击中目标 6
        /// </summary>
        public Entity target;

        /// <summary>
        /// 7
        /// </summary>
        public int4 args;

        /// <summary>
        /// 弹幕速度 8
        /// </summary>
        public float speed;

        /// <summary>
        /// 弹幕用 触发器id 9
        /// </summary>
        public int triggerID;

        /// <summary>
        /// 是否被障碍物吸收 10
        /// </summary>
        public bool isAbsorb;

        /// <summary>
        /// 目标int值 11
        /// </summary>
        public int targetInt;

        /// <summary>
        /// 是否可触发OnStay回调 12
        /// </summary>
        public bool isOnStayTrigger;


        /// <summary>
        /// 弹幕旋转速度 13  度/s 
        /// </summary>
        public float rotateSpeed;

        /// <summary>
        /// 弹幕类型参数 14
        /// </summary>
        public float3 trackPar;

        /// <summary>
        /// 终点位置 15
        /// </summary>
        public float3 targetPos;

        /// <summary>
        /// 跟随的目标 16
        /// </summary>
        public Entity followedEntity;

        /// <summary>
        /// 当前可触发OnStay回调 冷却 17
        /// </summary>
        public float curOnStayTriggerCd;

        /// <summary>
        /// 总持续时间 18
        /// </summary>
        public float totalDuration;

        /// <summary>
        /// skill的id 19
        /// </summary>
        public int skillID;

        /// <summary>
        /// 是否走exit回调 20
        /// </summary>
        public bool enableExit;

        /// <summary>
        /// 回调类型 21  0:只走onEnter 1:走onEnter或者onStay
        /// </summary>
        public int funcType;

        /// <summary>
        /// 死亡触发器id 22 
        /// </summary>
        public int deadEffectID;

        /// <summary>
        /// OnStay回调cd 23
        /// </summary>
        public float onStayTriggerCd;

        /// <summary>
        /// 每时间触发的当前次数  24 用来处理多段矩形
        /// </summary>
        public int tirggerCount;

        /// <summary>
        /// 弹幕生成的tick时间 25
        /// </summary>
        public int curTick;

        /// <summary>
        /// 脏东西的entity 26
        /// </summary>
        public Entity holder;

        /// <summary>
        /// 是否是弹幕伤害 27
        /// </summary>
        public bool isBulletDamage;

        /// <summary>
        /// 是否是武器攻击 28
        /// </summary>
        public bool isWeaponAttack;

        /// <summary>
        /// 加速度 mm/s^2  29
        /// </summary>
        public int acceleration;

        /// <summary>
        /// 默认飞行方向 目标朝向向量 30 
        /// </summary>
        public float3 defaultDir;

        /// <summary>
        /// 弹幕最大速度 31
        /// </summary>
        public float defaultSpeed;

        /// <summary>
        /// triggerId 32
        /// </summary>
        public int triggerId;

        /// <summary>
        /// 用于冲锋的选点 33
        /// </summary>
        public float3 crashPos;



        /// <summary>
        /// 每帧做位置变动
        /// Tween参数:
        /// t:当前时间
        /// b:初始位置
        /// c:变化量
        /// d:持续时间
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        /// <returns>变动后的LT</returns>
        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            var acc = acceleration / 1000f;
            speed += acc * inData.fDT;
            return refData.cdfeLocalTransform[inData.entity];
        }

        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            //if (!refData.storageInfoFromEntity.Exists(caster)) return;
        }


        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            if (refData.bfeBuff.HasBuffer(target))
            {
                var buffs = refData.bfeBuff[target];
                int index = 0, count = 0;
                ref var monsterFeatures =
                    ref inData.config.value.Value.configTbmonster_features.configTbmonster_features;

                for (int i = 0; i < monsterFeatures.Length; i++)
                {
                    if (monsterFeatures[i].id == 110)
                    {
                        index = i;
                        break;
                    }
                }

                ref var monsterfeature = ref monsterFeatures[index];
                var attRations = monsterfeature.para[2];
                var maxCount = monsterfeature.para[4];
                var buffDuration = monsterfeature.para[3] / 1000f;
                for (int i = 0; i < buffs.Length; i++)
                {
                    var buff = buffs[i];
                    if (buff.CurrentTypeId == Buff.TypeId.Buff_Martyrs)
                    {
                        count++;
                    }
                }

                if (count < maxCount)
                {
                    refData.ecb.AppendToBuffer(inData.sortkey, target,
                        new Buff_Martyrs
                        {
                            duration = buffDuration, permanent = false, carrier = target,
                            argsNew = new BuffArgsNew { args1 = new int4(203020, attRations, 0, 0) }
                        }.ToBuff());
                }
            }

            //UnityHelper.ChangeProperty(ref refData.ecb, inData.sortkey, ref refData.cdfeChaStats,
            //                   ref refData.cdfePlayerData, refData.cdfeLocalTransform, 203020, attRations, target);
        }


        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }


        public void OnExit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }
    }
}
//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------

using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Main
{
    //diy buff 角色每次受到伤害，有【参数1】的将【参数2】*受到的伤害均分给自身范围内所有敌方目标。
    //new 
    public partial struct Buff_DiyGameEvent53 : IBuff
    {
        ///<summary>0
        ///buff的id
        ///</summary>
        public int id;

        ///<summary>1
        ///buff添加时间点/刻 替换优先级
        ///</summary>
        public float startTime;

        ///<summary>2
        ///buff已经存在了多少时间了，单位：帧
        ///</summary>
        public int timeElapsed;

        ///<summary>3
        ///剩余多久，单位：秒
        ///</summary>
        public float duration;

        ///<summary>4
        ///是否是一个永久的buff，永久的duration不会减少，但是timeElapsed还会增加
        ///</summary>
        public bool permanent;

        ///<summary>5
        ///buff的施法者是谁，当然可以是空的
        ///</summary>
        public Entity caster;

        ///<summary>6
        ///buff的携带者，实际上是作为参数传递给脚本用，具体是谁，可定是所在控件的this.gameObject了
        ///</summary>
        public Entity carrier;

        /// <summary>7
        /// 替换类型 用于同类buff的替换 仅用于免疫和控制和属性变更 为属性变更时 x是类型 y是上限
        /// </summary>
        public int2 changeType;

        /// <summary>8
        /// 是否能够清除 用于不同类buff的替换 仅用于控制类
        /// new字段
        /// </summary>
        public int canClear;

        /// <summary>9
        /// 护盾层数 仅作用于免疫
        /// new
        /// </summary>
        public int immuneStack;

        /// <summary>10
        /// 从 effect传入的的方向
        /// new
        /// </summary>
        public float3 direction;

        /// <summary>11
        /// buff的状态类型 分无状态 正面状态和负面状态
        /// </summary>
        public int buffState;

        /// <summary>12
        /// float4-1:output 2:bonus_type 3:calc_type and pra
        /// </summary>
        public BuffArgsNew argsNew;

        ///<summary>13
        ///buff的优先级，优先级越低的buff越后面执行，这是一个非常重要的属性
        ///比如经典的“吸收50点伤害”和“受到的伤害100%反弹给攻击者”应该反弹多少，取决于这两个buff的priority谁更高
        ///</summary>
        public int priority;

        /// <summary>14
        /// 元素的概率 
        /// </summary>
        public int power;

        /// <summary>15
        /// 用来处理ontick的逻辑 =0时不执行ontick 
        /// </summary>
        public int tickTime;

        /// <summary>16
        /// 用来处理移动伤害的逻辑  第一个float3的x为每移动多少米 y为当前移动总距离 z为1时该移动伤害才生效！！！！
        /// 第二个记录上帧的位置 
        /// </summary>
        public float3x2 distancePar;

        /// <summary>17
        /// 是否是弹幕
        /// </summary>
        public bool isBullet;

        /// <summary>
        ///  18 buff2里上一份属性变更的值
        /// </summary>
        public int oldValue;

        /// <summary>
        /// 19 是否禁用特效
        /// </summary>
        public bool isDisableSpecial;

        /// <summary>
        /// 20 技能id
        /// </summary>
        public int skillId;

        public void OnOccur(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnRemoved(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnTick(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            //Entity attaker = default;
            //var damage = new Damage
            //{
            //    normal = 0,
            //    bullet = 0,
            //    collide = 0,
            //    environment = 0
            //};
            //if (inData.cdfeMapElementData.HasComponent(caster))
            //{
            //    damage.environment = buffArgs.args0;
            //}
            //else
            //{
            //    damage.normal = buffArgs.args0;
            //    attaker = caster;
            //}

            //refData.ecb.AppendToBuffer(inData.sortkey, inData.wbe, new DamageInfo
            //{
            //    attacker = attaker,
            //    defender = carrier,
            //    tags = new DamageInfoTag
            //    {
            //        directDamage = true,
            //        periodDamage = false,
            //        reflectDamage = false,
            //        copyDamage = false,
            //        directHeal = false,
            //        periodHeal = false
            //    },
            //    damage = damage,
            //    criticalRate = 0,
            //    criticalDamage = 0,
            //    hitRate = 1,
            //    degree = 0,
            //    pos = default,
            //    bulletEntity = default,
            //});
        }

        public void OnHit(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeHurt(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            NativeList<Entity> takeDamagers = new NativeList<Entity>(Allocator.Temp);
            var rand = Random.CreateFromIndex((uint)(inData.timeTick * 10 + refData.damageInfo.attacker.Index));
            var randValue = rand.NextInt(0, 10001);
            UnityEngine.Debug.Log($"概率为：{randValue}");
            if (randValue <= argsNew.args1.x)
            {
                UnityEngine.Debug.Log($"概率ok，免伤");
                var originalDamage = refData.damageInfo.damage.normal;
                refData.damageInfo.tags.directDamage = false;
                refData.damageInfo.tags.directHeal = true;
                refData.damageInfo.damage.normal = 0;
                refData.damageInfo.disableDamageNumber = true;
                var damageValue = originalDamage * argsNew.args2.x / 10000f;
                for (int i = 0; i < inData.entities.Length; i++)
                {
                    var entity = inData.entities[i];
                    var dis = math.distance(inData.cdfeLocalTransform[entity].Position,
                        inData.cdfeLocalTransform[carrier].Position);
                    if (dis <= 50f && entity != carrier)
                    {
                        takeDamagers.Add(entity);
                    }
                }

                if (takeDamagers.Length > 0)
                {
                    UnityEngine.Debug.Log($"有怪，流离");
                    var apartDamages = damageValue / takeDamagers.Length;
                    UnityEngine.Debug.Log($"流离伤害:{apartDamages},人数：{takeDamagers.Length}");
                    for (int i = 0; i < takeDamagers.Length; i++)
                    {
                        var entity = takeDamagers[i];
                        var damageInfoTag = new DamageInfoTag
                        {
                            directDamage = true,
                        };
                        var damage = new Damage
                        {
                            normal = (int)math.floor(apartDamages),
                        };
                        var casterChaStats = refData.cdfeChaStats[carrier];
                        refData.ecb.AppendToBuffer(inData.sortkey, inData.wbe, new DamageInfo
                        {
                            attacker = carrier,
                            defender = entity,
                            tags = damageInfoTag,
                            damage = damage,
                            criticalRate = casterChaStats
                                               .chaProperty.critical /
                                           10000f,
                            criticalDamage = casterChaStats
                                                 .chaProperty.criticalDamageRatios /
                                             10000f,
                            hitRate = 1,
                            degree = 0,
                            pos = default,
                            bulletEntity = default
                        });
                    }
                }
            }
        }

        public void OnKill(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeforeBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnPerUnitMove(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            //ref battleConstConfig=ref inData.globalConfigData.value.Value.configtbb
            float moveDis = 5000 / 1000f;
            float moveDamage = 200 / 10000f * refData.cdfeChaStats[carrier].chaProperty.maxHp;
        }
    }
}
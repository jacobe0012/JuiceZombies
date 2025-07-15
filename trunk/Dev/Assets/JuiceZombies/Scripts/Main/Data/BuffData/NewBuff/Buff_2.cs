//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    /// <summary>
    /// 属性变更buff
    /// new
    /// buffargs 0:id 1:增加值 2:正负面类型
    /// </summary>
    public partial struct Buff_2 : IBuff
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
        /// 替换类型 x:替换类型 y:上限
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

        //private bool isFind;

        public void OnOccur(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            //isFind = false;
            if (!refData.cdfeChaStats.HasComponent(carrier))
            {
                return;
            }

            var chaStats = refData.cdfeChaStats[carrier];
            var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.timeTick + caster.Index +
                                                                       caster.Version));
            var dropRate = math.clamp(power, 0, 10000);
            Debug.Log($"dropRate:{dropRate}");
            var canTrigger = rand.NextInt(0, 10001) <= dropRate;
            if (!canTrigger)
                return;

            // var curBuffs = refData.buff;
            // int oldBuffIndex = -1;
            // for (int i = 0; i < curBuffs.Length; i++)
            // {
            //     if (curBuffs[i].Int32_0 == id)
            //     {
            //         oldBuffIndex = i;
            //         break;
            //     }
            // }

            // for (int i = 0; i < curBuffs.Length; i++)
            // {
            //     if (curBuffs[i].Int32_0 == id && oldBuffIndex != i)
            //     {
            //         isFind = true;
            //         break;
            //     }
            // }

            //oldValue = argsNew.args1.y;
            Debug.Log($"直接添加:id为{id},参数为：{oldValue}");
            UnityHelper.ChangeProperty(ref refData.ecb, inData.sortkey, ref refData.cdfeChaStats,
                ref refData.cdfePlayerData, ref refData.cdfePhysicsMass, refData.cdfeLocalTransform,
                argsNew.args1.x, oldValue, carrier);
            // //不需要替换关系时 直接加
            // if (!isFind)
            // {
            //    
            //    
            // }
            // else
            // {
            //     //isChange = true;
            //     Debug.Log($"替换:id为{curBuffs[oldBuffIndex].Int32_0}");
            //     ActiveChangeType(changeType.x, curBuffs, oldBuffIndex, changeType.y, ref refData, inData);
            // }
        }


     

        public void OnRemoved(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            if (!refData.cdfeChaStats.HasComponent(carrier))
            {
                return;
            }
            //var chaStats = refData.cdfeChaStats[carrier];


            // if (!isFind)
            // {
            Debug.Log($"移除属性变更 ，id:{argsNew.args1.x},值为{oldValue}");
            // UnityHelper.ChangeProperty(ref refData.ecb, inData.sortkey, ref refData.cdfeChaStats,
            //     ref refData.cdfePlayerData, ref refData.cdfePhysicsMass, refData.cdfeLocalTransform,
            //     argsNew.args1.x, -oldValue, carrier);
            UnityHelper.RemoveChangeProperty(ref refData.ecb, inData.sortkey, ref refData.cdfeChaStats,
                ref refData.cdfePlayerData, ref refData.cdfePhysicsMass, inData.cdfeLocalTransform, argsNew.args1.x,
                oldValue, carrier);
            //}

            // var tran = refData.cdfeLocalTransform[carrier];
            // var chaStatsNew = refData.cdfeChaStats[carrier];
            // if (chaStats.chaProperty.scaleRatios != chaStatsNew.chaProperty.scaleRatios)
            // {
            //     refData.ecb.AddComponent(inData.sortkey, carrier, new PushColliderData
            //     {
            //         toBeSmall = chaStatsNew.chaProperty.scaleRatios < chaStats.chaProperty.scaleRatios,
            //         initScale = tran.Scale,
            //         targetScale = tran.Scale * (chaStatsNew.chaProperty.scaleRatios / 10000f)
            //     });
            // }
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
            //float moveDis = 5000 / 1000f;
            //float moveDamage = 200 / 10000f * refData.cdfeChaStats[carrier].chaProperty.
        }
        
           /// <summary>
        /// 存在替换buff时 
        /// </summary>
        /// <param name="changeType"></param>
        /// <param name="curBuffs"></param>
        /// <param name="buffIndex"></param>
        /// <param name="limitValue"></param>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        private void ActiveChangeType(int changeType, DynamicBuffer<Buff> curBuffs, int buffIndex, int limitValue,
            ref BuffData_ReadWrite refData, BuffData_ReadOnly inData)
        {
            var temp = curBuffs[buffIndex];
            var oldProp = temp.Int32_18;

            UnityHelper.RemoveChangeProperty(ref refData.ecb, inData.sortkey, ref refData.cdfeChaStats,
                ref refData.cdfePlayerData, ref refData.cdfePhysicsMass, refData.cdfeLocalTransform,
                argsNew.args1.x, oldProp, carrier);


            switch (changeType)
            {
                //不做比较
                case 0:
                    temp = curBuffs[buffIndex];
                    oldValue = temp.Int32_18 + argsNew.args1.y;
                    if (limitValue != 0)
                    {
                        oldValue = oldValue <= limitValue ? oldValue : limitValue;
                    }

                    temp.Single_3 = duration;

                    //temp.Boolean_4 = false;
                    //curBuffs[buffIndex] = temp;
                    ////curBuffs[buffIndex].OnRemoved(ref refData, inData);
                    ////curBuffs.RemoveAt(buffIndex);
                    //UnityHelper.ChangeProperty(ref refData.ecb, inData.sortkey, ref refData.cdfeChaStats,
                    //    ref refData.cdfePlayerData, ref refData.cdfePhysicsMass, refData.cdfeLocalTransform,
                    //    argsNew.args1.x, oldValue, carrier);
                    break;
                case 1:
                    oldValue = argsNew.args1.y;
                    temp = curBuffs[buffIndex];
                    temp.Single_3 = duration;
                    Debug.Log($"value:{oldValue}");
                    break;
                case 2:
                    temp = curBuffs[buffIndex];
                    oldValue = curBuffs[buffIndex].Int32_18 + argsNew.args1.y;
                    if (limitValue != 0)
                    {
                        oldValue = oldValue <= limitValue ? oldValue : limitValue;
                    }

                    break;
            }

            temp.Int32_18 = oldValue;
            curBuffs[buffIndex] = temp;

            duration = 0;
            permanent = false;

            UnityHelper.ChangeProperty(ref refData.ecb, inData.sortkey, ref refData.cdfeChaStats,
                ref refData.cdfePlayerData, ref refData.cdfePhysicsMass, refData.cdfeLocalTransform,
                argsNew.args1.x, oldValue, carrier);

            Debug.Log($"{id} oldProp:{oldProp} newprop {oldValue}  {argsNew.args1.x}");
        }
    }
}
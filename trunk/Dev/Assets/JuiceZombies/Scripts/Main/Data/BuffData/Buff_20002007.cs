//---------------------------------------------------------------------
// UnicornStudio
// Author: 如初
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    //子弹时间
    public partial struct Buff_20002007 : IBuffOld
    {
        ///<summary>0
        ///buff的id
        ///</summary>
        public int id;

        ///<summary>1
        ///buff的优先级，优先级越低的buff越后面执行，这是一个非常重要的属性
        ///比如经典的“吸收50点伤害”和“受到的伤害100%反弹给攻击者”应该反弹多少，取决于这两个buff的priority谁更高
        ///</summary>
        public int priority;

        ///<summary>2
        ///buff堆叠的规则中需要的层数，在这个游戏里只要id和caster相同的buffObj就可以堆叠
        ///激战2里就不同，尽管图标显示堆叠，其实只是统计了有多少个相同id的buffObj作为层数显示了
        ///</summary>
        public int maxStack;

        ///<summary>3
        ///buff的tag
        ///</summary>
        public int tags;

        ///<summary>4
        ///buff的工作周期，单位：秒。
        ///每多少秒执行工作一次，如果<=0则代表不会周期性工作，只要>0，则最小值为Time.FixedDeltaTime。
        ///</summary>
        public float tickTime;

        ///<summary>5
        ///buff已经存在了多少时间了，单位：帧
        ///</summary>
        public int timeElapsed;

        ///<summary>6
        ///buff执行了多少次onTick了，如果不会执行onTick，那将永远是0
        ///</summary>
        public int ticked;

        ///<summary>7
        ///剩余多久，单位：秒
        ///</summary>
        public float duration;

        ///<summary>8
        ///是否是一个永久的buff，永久的duration不会减少，但是timeElapsed还会增加
        ///</summary>
        public bool permanent;


        ///<summary>9
        ///buff的施法者是谁，当然可以是空的
        ///</summary>
        public Entity caster;

        ///<summary>10
        ///buff的携带者，实际上是作为参数传递给脚本用，具体是谁，可定是所在控件的this.gameObject了
        ///</summary>
        public Entity carrier;

        ///<summary>11
        ///合并时是否保持原有buff持续时间
        ///</summary>
        public bool canBeStacked;

        ///<summary>12  
        ///合并时是否保持原有buff持续时间
        ///</summary>
        public BuffStack buffStack;

        ///<summary>13 
        ///buff参数
        ///</summary>
        public BuffArgs buffArgs;

        /// <summary>14
        /// 当前移动总距离
        /// </summary>
        public float totalMoveDistance;

        /// <summary>15
        /// 每移动多少米调用一次
        /// </summary>
        public int xMetresPerInvoke;

        /// <summary>16
        /// 上一tick的位置
        /// </summary>
        public float3 lastPosition;

        public uint oldFilter;

        public void OnOccur(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
            Debug.Log("子弹时间");
            for (int i = 0; i < inData.entities.Length; i++)
            {
                if (inData.entities[i].Index != carrier.Index)
                {
                    var stats = refData.cdfeChaStats[inData.entities[i]];
                    stats.chaResource.actionSpeed *= 0.5f;
                    refData.cdfeChaStats[inData.entities[i]] = stats;
                    var skill = refData.cdfeSkill[inData.entities[i]];
                    for (int j = 0; j < skill.Length; j++)
                    {
                        var temp = skill[j];
                        temp.Single_2 *= 1.5f;
                        skill[j] = temp;
                    }
                    //   Debug.Log($"entity:{inData.entities[i]},actionSpeed:{stats.chaResource.actionSpeed}");
                }
            }
        }

        public void OnRemoved(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
            for (int i = 0; i < inData.entities.Length; i++)
            {
                if (inData.entities[i].Index != carrier.Index)
                {
                    var stats = refData.cdfeChaStats[inData.entities[i]];
                    stats.chaResource.actionSpeed /= 0.5f;
                    refData.cdfeChaStats[inData.entities[i]] = stats;
                    //   Debug.Log($"entity:{inData.entities[i]},actionSpeed:{stats.chaResource.actionSpeed}");
                }
            }

            refData.ecb.AppendToBuffer(inData.sortkey, carrier,
                new Buff_20002008
                {
                    id = 20002008,
                    priority = 0,
                    maxStack = 0,
                    tags = 0,
                    tickTime = 0,
                    timeElapsed = 0,
                    ticked = 0,
                    duration = buffArgs.args0,
                    permanent = false,
                    caster = default,
                    carrier = carrier,
                    canBeStacked = false,
                    buffStack = default,
                    buffArgs = default
                }.ToBuffOld());
        }

        public void OnTick(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
        }

        public void OnCast(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
        }

        public void OnHit(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
        }

        public void OnBeHurt(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
        }

        public void OnKill(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
        }

        public void OnBeforeBeKilled(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
        }

        public void OnBeKilled(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
        }

        public void OnPerUnitMove(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
        }

        public void OnLevelUp(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
        {
        }
    }
}
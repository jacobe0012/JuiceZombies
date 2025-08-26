//---------------------------------------------------------------------
// UnicornStudio
// Author: 如初
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------

using System.Globalization;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    //强制位移 用于冲锋和跳跃
    public partial struct Buff_6 : IBuff
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

        private float velocity;

        public void OnOccur(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.timeTick + caster.Index +
                                                                       caster.Version));
            var dropRate = math.clamp(power, 0, 10000);
            Debug.LogError($"dropRate:{dropRate}");
            var canTrigger = rand.NextInt(0, 10001) <= dropRate;
            if (!canTrigger)
                return;

            buffState = 0;
            float4 pointTypePar = argsNew.args3;
            int displayForm = argsNew.args1.x, pointType = argsNew.args2.x, passType = argsNew.args4.x;


            float offset = pointTypePar.y;
            float3 tarPos = default;
            if (pointType == 0)
            {
                float distance = pointTypePar.x;

                tarPos = GetRandomPointOutsideLine(inData, ref refData, displayForm, distance - offset,
                    distance + offset);
            }
            else if (pointType == 1)
            {
                float radius = pointTypePar.x;
                tarPos = GetRandomPointOutsideCircle(inData, displayForm, offset, radius);
            }

            if (duration == 0)
            {
                //传送
                var loc = new LocalTransform
                {
                    Position = tarPos,
                    Rotation = inData.cdfeLocalTransform[carrier].Rotation,
                    Scale = inData.cdfeLocalTransform[carrier].Scale
                };
                refData.ecb.SetComponent<LocalTransform>(inData.sortkey, carrier, loc);
            }
            else
            {
                var trueDis = math.distance(inData.cdfeLocalTransform[caster].Position, tarPos);
                //如果没有碰撞盒
                if (passType == 1)
                {
                    Debug.LogError("碰撞盒改变");
                    var physicsCollider = refData.cdfePhysicsCollider[carrier];
                    var newCollider =
                        PhysicsHelper.CreateColliderWithBelongsTo(physicsCollider, 0);
                    refData.ecb.SetComponent(inData.sortkey, carrier, newCollider);


                    //refData.ecb.AddComponent(inData.sortkey, carrier, new IgnoreHitBackData
                    //{
                    //    //id = 0,
                    //    IgnoreType = 0
                    //});
                }

                velocity = trueDis / duration * inData.fdT;
            }
        }

        public float3 GetRandomPointOutsideCircle(BuffData_ReadOnly inData, float displayForm, float rMin, float rMax)
        {
            //var rand = Random.CreateFromIndex((uint)(inData.eT * 1000));
            //float randomR = rand.NextFloat(rMin, rMax);
            //float randomAngle = rand.NextFloat(0f, 2f * math.PI); // 生成随机角度
            ////var targetPos = displayForm == 1 ? refData.cdfeChaStats[caster].chaResource.direction * randomR + inData.cdfeLocalTransform[caster].Position : inData.cdfeLocalTransform[carrier].Position;
            //float randomX = targetPos.x + randomR * math.cos(randomAngle); // 计算随机点的 x 坐标
            //float randomY = targetPos.y + randomR * math.sin(randomAngle); // 计算随机点的 y 坐标
            //return new float3(randomX, randomY, 0);
            var targetPos = displayForm == 1
                ? BuffHelper.GetRandomPointInCircle(inData.cdfeLocalTransform[caster].Position, rMax, 0,1)
                : inData.cdfeLocalTransform[carrier].Position;
            return BuffHelper.GetRandomPointInCircle(targetPos, rMin, 0, 1);
        }


        public float3 GetRandomPointOutsideLine(BuffData_ReadOnly inData, ref BuffData_ReadWrite refData,
            float displayForm, float x, float y)
        {
            var dir = displayForm == 1 ? refData.cdfeChaStats[caster].chaResource.direction : direction;
            var targetPos = inData.cdfeLocalTransform[carrier].Position;

            // 生成随机距离
            var rand = Unity.Mathematics.Random.CreateFromIndex(inData.timeTick);
            float randomDistance = rand.NextFloat(x, y);

            // 计算随机点的坐标
            return targetPos + dir * randomDistance;
            //// 大正方形的边界
            //float minX = -x / 2f;
            //float maxX = x / 2f;
            //float minY = -x / 2f;
            //float maxY = x / 2f;

            //// 小正方形的边界
            //float minYInner = -y / 2f;
            //float maxYInner = y / 2f;

            //Vector2 randomPoint = new Vector2(UnityEngine.Random.Range(targetPos.x + minX, targetPos.x + maxX),
            //    UnityEngine.Random.Range(targetPos.y + minY, targetPos.y + maxY));

            //// 如果随机点在小正方形内，则重复选择直到找到一个在小正方形外的点
            //while (randomPoint.y >= minYInner && randomPoint.y <= maxYInner && randomPoint.x >= minYInner &&
            //       randomPoint.x <= maxYInner)
            //{
            //    randomPoint = new Vector2(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY));
            //}

            //var result = new float3(randomPoint.x, randomPoint.y, 0);
        }

        public void OnRemoved(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            //冲锋完毕后 加一个临时的boss可移动 来改变boss的朝向
            refData.ecb.AppendToBuffer(inData.sortkey, carrier,
                   new Buff_LetBossMove { carrier = carrier, duration = 0.04f, permanent = false }
                       .ToBuff());

        }

        public void OnUpdate(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            var loc = new LocalTransform
            {
                Position = inData.cdfeLocalTransform[carrier].Position + direction * velocity,
                Rotation = inData.cdfeLocalTransform[carrier].Rotation,
                Scale = inData.cdfeLocalTransform[carrier].Scale
            };
            refData.cdfeLocalTransform[carrier]=loc;
            //refData.ecb.SetComponent<LocalTransform>(inData.sortkey, carrier, loc);
        }

        public void OnHit(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeHurt(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnTick(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
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
            float moveDis = 5000 / 1000f;
            float moveDamage = 200 / 10000f * refData.cdfeChaStats[carrier].chaProperty.maxHp;
        }
    }
}
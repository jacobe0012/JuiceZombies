////---------------------------------------------------------------------
//// JiYuStudio
//// Author: 格伦
//// Time: 2023-07-17 12:15:10
////---------------------------------------------------------------------

//using System;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    //灼烧 固定tick为1s   参数：伤害万分比,作用目标
//    public partial struct Buff_DelayBoom : IBuffOld
//    {
//        ///<summary>0
//        ///buff的id
//        ///</summary>
//        public int id;

//        ///<summary>1
//        ///buff的优先级，优先级越低的buff越后面执行，这是一个非常重要的属性
//        ///比如经典的“吸收50点伤害”和“受到的伤害100%反弹给攻击者”应该反弹多少，取决于这两个buff的priority谁更高
//        ///</summary>
//        public int priority;

//        ///<summary>2
//        ///buff堆叠的规则中需要的层数，在这个游戏里只要id和caster相同的buffObj就可以堆叠
//        ///激战2里就不同，尽管图标显示堆叠，其实只是统计了有多少个相同id的buffObj作为层数显示了
//        ///</summary>
//        public int maxStack;

//        ///<summary>3
//        ///buff的tag
//        ///</summary>
//        public int tags;

//        ///<summary>4
//        ///buff的工作周期，单位：秒。
//        ///每多少秒执行工作一次，如果<=0则代表不会周期性工作，只要>0，则最小值为Time.FixedDeltaTime。
//        ///</summary>
//        public float tickTime;

//        ///<summary>5
//        ///buff已经存在了多少时间了，单位：帧
//        ///</summary>
//        public int timeElapsed;

//        ///<summary>6
//        ///buff执行了多少次onTick了，如果不会执行onTick，那将永远是0
//        ///</summary>
//        public int ticked;

//        ///<summary>7
//        ///剩余多久，单位：秒
//        ///</summary>
//        public float duration;

//        ///<summary>8
//        ///是否是一个永久的buff，永久的duration不会减少，但是timeElapsed还会增加
//        ///</summary>
//        public bool permanent;


//        ///<summary>9
//        ///buff的施法者是谁，当然可以是空的
//        ///</summary>
//        public Entity caster;

//        ///<summary>10
//        ///buff的携带者，实际上是作为参数传递给脚本用，具体是谁，可定是所在控件的this.gameObject了
//        ///</summary>
//        public Entity carrier;

//        ///<summary>11
//        ///合并时是否保持原有buff持续时间
//        ///</summary>
//        public bool canBeStacked;

//        ///<summary>12  
//        ///合并时是否保持原有buff持续时间
//        ///</summary>
//        public BuffStack buffStack;

//        ///<summary>13 
//        ///buff参数
//        ///</summary>
//        public BuffArgs buffArgs;

//        /// <summary>14
//        /// 当前移动总距离
//        /// </summary>
//        public float totalMoveDistance;

//        /// <summary>15
//        /// 每移动多少米调用一次
//        /// </summary>
//        public int xMetresPerInvoke;

//        /// <summary>16
//        /// 上一tick的位置
//        /// </summary>
//        public float3 lastPosition;

//        ///<summary>11
//        ///buff对于角色的ChaControlState的影响
//        ///</summary>
//        //public ChaControlState stateMod;
//        public void OnOccur(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnRemoved(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//            if (!BuffHelper.TryGetParmeter(inData.globalConfigData, 7000031,
//                    out var buff1))
//                return;
//            var degree = buff1[1].x;
//            var radius = buff1[2].x;
//            var pushForce = buff1[3].x;
//            var pushForceMax = buff1[4].x;

//            int pushRatios = 0;

//            int buffTargetType = 0;
//            int buff1index = 0;
//            int buff2index = 0;
//            int buff2Attr = 0;

//            //TODO：技能实体炸弹碰撞盒名字更改
//            var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);
//            var newpos = new LocalTransform
//            {
//                Position = inData.cdfeLocalTransform[inData.entity].Position,
//                Scale = inData.cdfeLocalTransform[prefab].Scale * radius,
//                Rotation = inData.cdfeLocalTransform[prefab].Rotation
//            };
//            refData.ecb.SetComponent(inData.sortkey, ins, newpos);

//            refData.ecb.AddComponent(inData.sortkey, ins,
//                new SkillAttackData
//                {
//                    data = new SkillAttack_0
//                    {
//                        duration = 0,
//                        tick = 0,
//                        caster = inData.player
//                    }.ToSkillAttack()
//                });

//            refData.ecb.AppendToBuffer(inData.sortkey, ins,
//                new SkillHitEffectBuffer
//                {
//                    buffID = 40000003,
//                    buffArgs = new float3x4
//                    {
//                        c0 = new float3(pushRatios, 0, 0),
//                    }
//                });
//            //TODO:buff30000009处理
//            refData.ecb.AppendToBuffer(inData.sortkey, ins,
//                new SkillHitEffectBuffer
//                {
//                    buffID = 30000009,
//                    buffArgs = new float3x4
//                    {
//                        c0 = default,
//                        c1 = default,
//                        c2 = default,
//                        c3 = default
//                    }
//                });
//            refData.ecb.AppendToBuffer(inData.sortkey, ins,
//                new SkillHitEffectBuffer
//                {
//                    buffID = 30000003,
//                    buffArgs = new float3x4
//                    {
//                        c0 = new float3(buff2index, 203000, 10000),
//                        c1 = default,
//                        c2 = default,
//                        c3 = default
//                    }
//                });
//        }

//        public void OnTick(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnCast(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnHit(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnBeHurt(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnKill(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnBeforeBeKilled(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnBeKilled(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnPerUnitMove(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnLevelUp(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }
//    }
//}


////---------------------------------------------------------------------
//// UnicornStudio
//// Author: 如初
//// Time: 2023-07-17 12:15:10
////---------------------------------------------------------------------

//using System;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    //受伤后撤 并发射弹幕 沙子
//    //args0 后撤距离 args1 effectID
//    public partial struct Buff_20002017 : IBuffOld
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


//        public void OnOccur(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {

//        }

//        public void OnRemoved(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnTick(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//          //  Debug.Log($"OnTick，ffffffffffffff,timeElapsed{timeElapsed}");

//        }

//        public void OnCast(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnHit(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        private float3 GetRandOffsetPosFilter(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData, int offset, Entity target)
//        {

//            float3 randomPoint = float3.zero;
//            do
//            {
//                var targetPos = inData.cdfeLocalTransform[target].Position;

//                var random = new Unity.Mathematics.Random();

//                // 在圆内生成随机半径和角度
//                float randomRadius = (float)random.NextFloat() * offset;
//                float randomAngle = (float)random.NextFloat() * math.PI * 2;

//                // 将极坐标转换为笛卡尔坐标
//                float xOffset = randomRadius * math.cos(randomAngle);
//                float yOffset = randomRadius * math.sin(randomAngle);

//                // 在中心点上偏移生成的随机坐标
//                randomPoint = new float3(targetPos.x + xOffset, targetPos.y + yOffset, 0);
//            }
//            while (!IsPosCanUse(randomPoint, ref refData, inData));

//            //Debug.Log($"randomPoint{randomPoint}");
//            return randomPoint;
//        }
//        private bool IsPosCanUse(float3 randomPoint, ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//            ref var scene_module = ref inData.globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
//            for (int i = 0; i < inData.mapModules.Length; i++)
//            {

//                for (int j = 0; j < scene_module.Length; j++)
//                {
//                    if (scene_module[i].id == inData.cdfeMapElementData[inData.mapModules[i]].elementID)
//                    {
//                        var pos = inData.cdfeLocalTransform[inData.mapModules[i]].Position;

//                        Rect rect = new Rect(pos.x, pos.y, scene_module[i].size[0], scene_module[i].size[1]);
//                        if (rect.Contains(randomPoint))
//                        {
//                            return false;
//                        }
//                        break;
//                    }
//                }

//            }
//            return true;
//        }
//        public void OnBeHurt(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {

//            //var dis = math.distance(inData.cdfeLocalTransform[caster].Position, inData.cdfeLocalTransform[inData.player].Position);
//            //if (!BuffHelper.TryGetParmeter(inData.globalConfigData, buffArgs.args1, out var buff, 50000016)) return;
//            //var moveDis = buff[1].x;
//            //if (math.abs(dis - buffArgs.args0) < 0.5f)
//            //{
//            //   var pos= GetRandOffsetPosFilter(ref refData, in inData, moveDis, inData.player);
//            //    //TODO:添加boss闪现动画：
//            //    refData.ecb.SetComponent<LocalTransform>(inData.sortkey,caster,new LocalTransform { Position = pos,Scale= inData.cdfeLocalTransform[caster].Scale, Rotation = inData.cdfeLocalTransform[caster].Rotation });
//            //    GenerateLimeBullet(ref refData, inData);

//            //}

//            //Debug.Log($"OnBeHurt，buffArgs.args0{buffArgs.args0}");
//            ////无敌
//            //refData.ecb.AppendToBuffer(inData.sortkey, carrier,
//            //    new Buff_20002001
//            //    {
//            //        id = 20002001,
//            //        priority = 0,
//            //        maxStack = 0,
//            //        tags = 0,
//            //        tickTime = 0,
//            //        timeElapsed = 0,
//            //        ticked = 0,
//            //        duration = 0.3f,
//            //        permanent = false,
//            //        caster = default,
//            //        carrier = carrier,
//            //        canBeStacked = false,
//            //        buffStack = default,
//            //        buffArgs = default
//            //    }.ToBuffOld());


//        }

//        private void GenerateLimeBullet(ref BuffOldData_ReadWrite refData, BuffOldData_ReadOnly inData)
//        {

//            ref var bulletConfig = ref inData.globalConfigData.value.Value.configTbbullets.configTbbullets;

//            ref var skilleffectConfig = ref inData.globalConfigData.value.Value.configTbskill_effects.configTbskill_effects;


//            if (!BuffHelper.TryGetParmeter(inData.globalConfigData, buffArgs.args1, out var buff1))
//                return;

//            int bulletIndex = 0;

//            for (int i = 0; i < bulletConfig.Length; i++)
//            {
//                if (bulletConfig[i].id == buff1[1].x)
//                {
//                    bulletIndex = i;
//                    break;
//                }
//            }

//            ref var bullet = ref bulletConfig[bulletIndex];

//            //var skillPrefab = inData.prefabMapData.prefabHashMap["skill_bullet_lime"];
//            //var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);

//            //refData.ecb.AppendToBuffer(inData.sortkey, caster, new BulletCastData
//            //{
//            //    id = buff1[1].x,
//            //    duration = 1f,
//            //    tick = 0,
//            //    caster = caster,
//            //    num = bullet.num,
//            //    degree = 0,
//            //    fireInternal = 1f/ buff1[4].x
//            //});

//            //var newLoc = new LocalTransform
//            //{
//            //    Position = inData.cdfeLocalTransform[inData.player].Position,
//            //    Scale = inData.cdfeLocalTransform[skillPrefab].Scale * buffArgs.args0,
//            //    Rotation = inData.cdfeLocalTransform[inData.player].Rotation,
//            //};
//            //refData.ecb.SetComponent(inData.sortkey, skillEntity, newLoc);
//            //refData.ecb.AddComponent(inData.sortkey, skillEntity,
//            //  new SkillAttackData
//            //  {
//            //      data = new SkillAttack_4020131
//            //      {
//            //          id = id,
//            //          duration = buffArgs.args1,
//            //          tick = 0,
//            //          caster = inData.player,
//            //          isBullet = false,
//            //          hp = 0,
//            //          stayAttack = false,
//            //          stayAttackInterval = 0,
//            //          curStayAttackInterval = 0,
//            //          skillDelay = 0
//            //      }.ToSkillAttack()
//            //  });
//        }

//        public void OnKill(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {
//        }

//        public void OnBeforeBeKilled(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//        {


//        }

//       public void OnBeKilled(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
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


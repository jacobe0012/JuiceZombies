//using System;
//using Unity.Collections;
//using Unity.Entities;

//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace Main
//{
//    //扔鞭炮
//    public partial struct SkillAttack_Throw210601 : ISkillAttack
//    {
//        /// <summary>
//        /// 0
//        /// </summary>
//        public int id;

//        /// <summary>
//        /// 1 一次性释放的技能实体 值=0
//        /// </summary>
//        public float duration;

//        /// <summary>
//        /// 2
//        /// </summary>
//        public int tick;

//        /// <summary>
//        /// 3 技能实体释放者
//        /// </summary>
//        public Entity caster;

//        /// <summary>
//        /// 4 是否是弹幕
//        /// </summary>
//        public bool isBullet;

//        /// <summary>
//        /// 5 弹幕hp
//        /// </summary>
//        public int hp;

//        /// <summary>
//        /// 6 是否是持续性攻击
//        /// </summary>
//        public bool stayAttack;

//        /// <summary>
//        /// 7 持续性攻击间隔
//        /// </summary>
//        public float stayAttackInterval;

//        /// <summary>
//        /// 8 当前持续性攻击间隔
//        /// </summary>
//        public float curStayAttackInterval;


//        /// <summary>
//        /// 技能实体执行延时 单位s  9
//        /// </summary>
//        public float skillDelay;

//        /// <summary>
//        /// 实体的击中目标 10
//        /// </summary>
//        public Entity target;

//        public int speed;

//        private float flyDuraion;
//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            //var dir = new float3(0, 1, 0);
//            var actionspeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON
//            ? 1
//            : inData.cdfeChaStats[caster].chaResource.actionSpeed;

//            //math.clamp(inData.cdfeChaStats[caster].chaResource.actionSpeed,1,)
//            var dir = math.mul(refData.cdfeLocalTransform[inData.entity].Rotation, new float3(0, 1, 0));
//            var temp = refData.cdfeLocalTransform[inData.entity];
//            temp.Position += dir * speed * inData.fDT * actionspeed;
//            return temp;


//        }

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//            if (BuffHelper.TryGetParmeter(inData.config, id, 40000034))
//            {
//                var buff1 = BuffHelper.UpdateParmeter(inData.config, id, 40000034);
//                var duration = buff1[0].x;
//                var range = buff1[1].x;
//                var boomScale = buff1[2].x / 2f;
//                var scalePropRate = buff1[2].z;
//                boomScale *= (1 + scalePropRate / 10000f);
//                var attackCount = buff1[3].x;

//                var pushRate = buff1[4].x;
//                var pushPropRate = buff1[4].z;
//                var buff2 = BuffHelper.UpdateParmeter(inData.config, id, 30000003);
//                var damageRate = buff2[1].x;
//                var damagePropRate = buff2[1].z;
//                NativeArray<int> timeArray = new NativeArray<int>(attackCount, Allocator.Temp);
//                timeArray = SelectBoomTime(duration, attackCount);

//                for (int i = 0; i < attackCount; i++)
//                {
//                    GenerateFire(timeArray[i],i, ref refData, inData, pushRate, pushPropRate, damageRate, damagePropRate,boomScale,range);
//                }

//            }

//        }

//        private void GenerateFire(int tick, int randNum, ref SkillAttackData_ReadWrite refData, SkillAttackData_ReadOnly inData, int pushRate, int pushPropRate, int damageRate, int damagePropRate, float boomScale, int range)
//        {
//            var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);

//            //UnityEngine.Debug.Log($"pushRate:{pushRate},pushPropRate:{pushPropRate},damageRate:{damageRate},damagePropRate:{damagePropRate}");
//            var targetPos = GetRandOffsetPos(ref refData, inData, range, refData.cdfeLocalTransform[inData.entity], randNum);
//            var newpos = new LocalTransform
//            {
//                Position = targetPos,
//                //TODO:大小*10
//                Scale = refData.cdfeLocalTransform[prefab].Scale * boomScale,
//                Rotation = refData.cdfeLocalTransform[prefab].Rotation,
//            };

//            refData.ecb.SetComponent(inData.sortkey, ins, newpos);
//            refData.ecb.AddComponent(inData.sortkey, ins,
//                new SkillAttackData
//                {
//                    data = new SkillAttack_0
//                    {
//                        id = id,
//                        duration = 0.5f,
//                        tick = 0,
//                        caster = inData.player,
//                        isBullet = false,
//                        hp = 0,
//                        stayAttack = false,
//                        stayAttackInterval = 0,
//                        curStayAttackInterval = 0,
//                        skillDelay = 0,
//                    }.ToSkillAttack()
//                });

//            refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, ins,
//        new SkillHitEffectBuffer
//        {
//            buffID = 40000028,
//            buffArgs = new float3x4 { c0 = new float3(pushRate, 215000, pushPropRate) }
//        });
//            refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, ins,
//        new SkillHitEffectBuffer{ 
//            buffID = 30000003, 
//            buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) } 
//        });

//        }


//        private float3 GetRandOffsetPos(ref SkillAttackData_ReadWrite refData, SkillAttackData_ReadOnly inData, int offset,LocalTransform loc, int i)
//        {
//            var targetPos = loc.Position;

//            var random = Unity.Mathematics.Random.CreateFromIndex((uint)i);

//            // 在圆内生成随机半径和角度
//            float randomRadius = (float)random.NextFloat() * offset;
//            float randomAngle = (float)random.NextFloat() * math.PI * 2;

//            // 将极坐标转换为笛卡尔坐标
//            float xOffset = randomRadius * math.cos(randomAngle);
//            float yOffset = randomRadius * math.sin(randomAngle);

//            // 在中心点上偏移生成的随机坐标
//            float3 randomPoint = new float3(targetPos.x + xOffset, targetPos.y + yOffset, 0);
//            //Debug.Log($"randomPoint{randomPoint}");
//            return randomPoint;
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            flyDuraion = duration;
//        }


//        private NativeArray<int> SelectBoomTime(int duration, int attackCount)
//        {

//            NativeArray<int> posArray = new NativeArray<int>(attackCount, Allocator.Temp);
//            int tickCount = (int)(duration / 1000f * 50);
//            for (int i = 1; i <= attackCount; ++i)
//            {
//                var rand = new Unity.Mathematics.Random((uint)i);
//                posArray[i - 1] = rand.NextInt(1, tickCount + 1);
//            }
//            return posArray;
//        }

//    }
//}


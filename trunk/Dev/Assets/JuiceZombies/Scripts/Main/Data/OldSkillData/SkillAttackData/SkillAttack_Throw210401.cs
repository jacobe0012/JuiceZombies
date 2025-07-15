//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    //没有位置大小方向变动的技能实体 用于实现荆棘的逻辑
//    public partial struct SkillAttack_Throw210401 : ISkillAttack
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
//            if (BuffHelper.TryGetParmeter(inData.config, id, 40000028))
//            {
//                var buff1 = BuffHelper.UpdateParmeter(inData.config, id, 40000028);
//                var skillAttackDuration = buff1[0].x / 1000f;

//                var pushRate = buff1[2].x;
//                var pushPropRate = buff1[2].z;
//                var buff2 = BuffHelper.UpdateParmeter(inData.config, id, 30000003);
//                var damageRate = buff2[1].x;
//                var damagePropRate = buff2[1].z;

//                var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//                var ins = refData.ecb.Instantiate(inData.sortkey, prefab);

//                // UnityEngine.Debug.Log(
//                //     $"pushRate:{pushRate},pushPropRate:{pushPropRate},damageRate:{damageRate},damagePropRate:{damagePropRate}");
//                var newpos = new LocalTransform
//                {
//                    Position = refData.cdfeLocalTransform[inData.entity].Position,
//                    //TODO:大小*10
//                    Scale = refData.cdfeLocalTransform[inData.entity].Scale,
//                    Rotation = refData.cdfeLocalTransform[prefab].Rotation,
//                };

//                refData.ecb.SetComponent(inData.sortkey, ins, newpos);
//                refData.ecb.AddComponent(inData.sortkey, ins,
//                    new SkillAttackData
//                    {
//                        data = new SkillAttack_0
//                        {
//                            id = id,
//                            duration = 0.5f,
//                            tick = 0,
//                            caster = inData.player,
//                            isBullet = false,
//                            hp = 0,
//                            stayAttack = false,
//                            stayAttackInterval = 0,
//                            curStayAttackInterval = 0,
//                            //skillDelay = skillAttackDuration- flyDuraion>0?skillAttackDuration-flyDuraion:0,
//                            skillDelay =  0,

//                        }.ToSkillAttack()
//                    });

//                refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, ins,
//                    new SkillHitEffectBuffer
//                    {
//                        buffID = 40000028,
//                        buffArgs = new float3x4 { c0 = new float3(pushRate, 215000, pushPropRate) }
//                    });
//                refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, ins,
//                    new SkillHitEffectBuffer
//                    {
//                        buffID = 30000003,
//                        buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//                    });
//            }
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
//    }
//}


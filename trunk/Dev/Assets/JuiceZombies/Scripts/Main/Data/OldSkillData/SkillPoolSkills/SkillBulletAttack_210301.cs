//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    //投掷长矛
//    public partial struct SkillBulletAttack_210301 : ISkillAttack
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
//        /// 实体的击中目标 6
//        /// </summary>
//        public Entity target;

//        /// <summary>
//        /// 7
//        /// </summary>
//        public int4 args;

//        public int speed;
//        public float width;
//        public bool isS;
//        public float3 startPoint;

//        public int deSpeedRatios;

//        public int damageRatios;
//        // public int skilleffectid;
//        // private int radius;
//        // private int pushRate;
//        // private int attackRate;
//        // private int attackAddRate;
//        // private int damageValue;


//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
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
//                ? 1
//                : inData.cdfeChaStats[caster].chaResource.actionSpeed;

//            //math.clamp(inData.cdfeChaStats[caster].chaResource.actionSpeed,1,)
//            var dir = math.mul(refData.cdfeLocalTransform[inData.entity].Rotation, new float3(0, 1, 0));
//            var temp = refData.cdfeLocalTransform[inData.entity];
//            temp.Position += dir * (speed / 100f) * inData.fDT * actionspeed;

//            return temp;
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            // throw new System.NotImplementedException();
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            ref var bulletConfig = ref inData.config.value.Value.configTbbullets.configTbbullets;
//            ref var skillConfig = ref inData.config.value.Value.configTbskills.configTbskills;
//            ref var skilleffectConfig = ref inData.config.value.Value.configTbskill_effects.configTbskill_effects;

//            if (inData.cdfeObstacleTag.HasComponent(target) && isS)
//            {
//                var endPoint = refData.cdfeLocalTransform[inData.entity].Position;

//                var prefab = inData.prefabMapData.prefabHashMap["SkillSpearS"];
//                var entity = refData.ecb.Instantiate(inData.sortkey, prefab);
//                var dir = endPoint - startPoint;
//                float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir), new float3(0, 1, 0));

//                var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));

//                var pos = math.lerp(endPoint, startPoint, 0.5f);
//                var tran = new LocalTransform
//                {
//                    Position = pos,
//                    //TODO:
//                    Scale = width,
//                    Rotation = qua
//                };
//                var length = math.length(endPoint - startPoint);

//                refData.ecb.AddComponent(inData.sortkey, entity, new PostTransformMatrix
//                {
//                    Value = float4x4.Scale(width, length, width)
//                });
//                refData.ecb.SetComponent(inData.sortkey, entity, tran);
//                //refData.ecb.AddComponent(inData.sortkey, entity, new SkillAttackData
//                //{
//                //    //TODO:
//                //    data = new SkillAttack_210301S()
//                //    {
//                //        id = 210306,
//                //        duration = 3,
//                //        tick = 0,
//                //        caster = inData.player,
//                //        isBullet = false,
//                //        hp = 0,
//                //        stayAttack = true,
//                //        stayAttackInterval = 0.5f,
//                //        curStayAttackInterval = 0,
//                //        skillDelay = 0,
//                //        target = default,
//                //        width = width,
//                //        deSpeedRatios = deSpeedRatios,
//                //        damageRatios = damageRatios
//                //    }.ToSkillAttack()
//                //});
//                refData.ecb.AppendToBuffer(inData.sortkey, entity, new SkillHitEffectBuffer
//                {
//                    buffID = 30000003,
//                    buffArgs = new float3x4 { c0 = new float3(damageRatios, 2030000, 10000) }
//                });

//                refData.ecb.DestroyEntity(inData.sortkey, inData.entity);
//            }
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            startPoint = refData.cdfeLocalTransform[inData.entity].Position;
//        }
//    }
//}


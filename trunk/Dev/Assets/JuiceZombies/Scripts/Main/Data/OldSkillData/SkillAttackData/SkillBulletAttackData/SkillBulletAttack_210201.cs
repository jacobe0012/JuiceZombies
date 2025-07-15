//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    //投掷石头
//    public partial struct SkillBulletAttack_210201 : ISkillAttack
//    {
//        /// <summary>
//        /// 0
//        /// </summary>
//        public int id;

//        /// <summary>
//        /// 1 一次性释放的技能实体 值=0
//        /// </summary>
//        public float  duration;

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
//        //public int skilleffectid;
//        //private int radius;
//        //private int pushRate;
//        //private int attackRate;
//        //private int attackAddRate;
//        //private int damageValue;


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


//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
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

//            int skillEffectId = 0;
//            for (int i = 0; i < skillConfig.Length; i++)
//            {
//                if (id == skillConfig[i].id)
//                {
//                    skillEffectId = skillConfig[i].skillEffectArray[0];
//                    return;
//                }
//            }

//            var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);

//            //Debug.LogError($"radius{radius}");
//            var newpos = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.entity].Position,
//                //TODO:大小*10
//                Scale = 10,
//                Rotation = refData.cdfeLocalTransform[prefab].Rotation,
//            };

//        //    refData.ecb.SetComponent(inData.sortkey, ins, newpos);

//        //    refData.ecb.AddComponent(inData.sortkey, ins,
//        //        new SkillAttackData
//        //        {
//        //            data = new SkillAttack_0
//        //            {
//        //                id = 210201,
//        //                duration = 0f,
//        //                tick = 0,
//        //                caster = inData.player,
//        //                isBullet = false,
//        //                hp = 0,
//        //            }.ToSkillAttack()
//        //        });

//        //    // UnityEngine.Debug.Log($"pushRate:{pushRate}attackRate:{attackRate}");
//        //    refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//        //    {
//        //        buffID = 40000010,
//        //        buffArgs = new float3x4
//        //        {
//        //            c0 = new float3(pushRate, 0, 0),
//        //            c1 = default,
//        //            c2 = default,
//        //            c3 = default
//        //        }
//        //    });
//        //    refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//        //    {
//        //        buffID = 30000003,
//        //        buffArgs = new float3x4
//        //        {
//        //            c0 = new float3(attackRate, 203000, attackAddRate),
//        //            c1 = default,
//        //            c2 = default,
//        //            c3 = default
//        //        }
//        //    });
//        //    refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//        //    {
//        //        buffID = 30000004,
//        //        buffArgs = new float3x4
//        //        {
//        //            c0 = new float3(damageValue, 0, 0),
//        //            c1 = default,
//        //            c2 = default,
//        //            c3 = default
//        //        }
//        //    });
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }
//    }
//}


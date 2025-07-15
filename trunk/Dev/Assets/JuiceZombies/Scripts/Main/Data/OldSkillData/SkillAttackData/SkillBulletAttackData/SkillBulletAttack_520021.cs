//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    //没有位置大小方向变动的技能实体
//    public partial struct SkillBulletAttack_520021 : ISkillAttack
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
//        public int skilleffectid;

//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            var actionspeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON ? 1 : inData.cdfeChaStats[caster].chaResource.actionSpeed;
//            var dir = math.mul(refData.cdfeLocalTransform[inData.entity].Rotation, new float3(0, 1, 0));
//            var temp = refData.cdfeLocalTransform[inData.entity];
//            temp.Position += dir * (speed / 100f) * inData.fDT*actionspeed;

//            return temp;
//        }
//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//           // throw new System.NotImplementedException();
//        }
//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            //ref var bulletConfig = ref inData.config.value.Value.configTbbarrages.configTbbarrages;
//            ref var skillConfig = ref inData.config.value.Value.configTbskills.configTbskills;

//            ref var skilleffectConfig = ref inData.config.value.Value.configTbskill_effects.configTbskill_effects;


//            int _buff1Index1 = 0;
//            int _buff1Index2 = 0;


//            int _buff2arg0 = 0;
//            int _buff2arg1 = 0;
//            int _buff2arg2 = 0;


//           // BuffHelper.UpdateParmeterNew(inData.config,skilleffectid,)
//            for (int i = 0; i < skilleffectConfig.Length; i++)
//            {
//                if (skilleffectConfig[i].id == skilleffectid)
//                {
//                    _buff1Index1 = skilleffectConfig[i].buff1Para[1];
//                    _buff1Index2 = skilleffectConfig[i].buff1Para[2];
//                    //buff2参数
//                    _buff2arg0 = skilleffectConfig[i].buff2Para[0];

//                    _buff2arg1 = skilleffectConfig[i].buff2Attr[0].y;
//                    _buff2arg2 = skilleffectConfig[i].buff2Attr[0].z;

//                    break;
//                }
//            }


//            var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);


//            var newpos = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.entity].Position,
//                Scale = _buff1Index1,
//                Rotation = refData.cdfeLocalTransform[prefab].Rotation,
//            };

//            refData.ecb.SetComponent(inData.sortkey, ins, newpos);

//            refData.ecb.AddComponent(inData.sortkey, ins,
//                new SkillAttackData
//                {
//                    data = new SkillAttack_0
//                    {
//                        id = 5200211,
//                        duration = 2,
//                        tick = 0,
//                        caster = caster,
//                        isBullet = false,
//                        hp = 0
//                    }.ToSkillAttack()
//                });

//            refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            {
//                buffID = 40000010,
//                buffArgs = new float3x4
//                {
//                    c0 = new float3(_buff1Index2 / 10000, 0, 0),
//                    c1 = default,
//                    c2 = default,
//                    c3 = default
//                }
//            });
//            refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            {
//                buffID = 30000003,
//                buffArgs = new float3x4
//                {
//                    c0 = new float3(_buff2arg0, _buff2arg1, _buff2arg2),
//                    c1 = default,
//                    c2 = default,
//                    c3 = default
//                }
//            });
//        }

//    }
//}


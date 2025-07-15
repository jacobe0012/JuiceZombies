////using NSprites;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;


//namespace Main
//{
//    public partial struct SkillBossLaserAttcak : ISkillAttack
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

//        public Entity entity;

//        //改变旋转位置
//        public float angle;

//        //改变初始位置
//        public float3 InitPosition;
//        //扩散速度
//        public float diffuseSpeed;


//        public quaternion quaternion;

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//            var actionspeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON ? 1 : inData.cdfeChaStats[caster].chaResource.actionSpeed;
//            diffuseSpeed*=actionspeed;
//            //Debug.Log(InitPosition + "  " + refData.cdfeLocalTransform[inData.entity].Scale + "  " + Quaternion.Euler(0, 0, angle) + "  ");


//            refData.ecb.AddComponent(inData.sortkey, inData.entity, new PostTransformMatrix
//            {
//                Value = float4x4.Scale(8, (1*tick* diffuseSpeed/100), 1)
//            });

//            //设置实体的LocalTransform,圆形主要是大小
//            var newLoc = new LocalTransform
//            {

//                //caster是怪物本身
//                Position = InitPosition,
//                Scale = 1,
//                Rotation = quaternion,


//            };


//            return newLoc;
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//          //  throw new System.NotImplementedException();
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//    }
//}


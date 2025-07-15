//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine.UIElements;


//namespace Main
//{
//    /// <summary>
//    /// 生成烟雾弹
//    /// </summary>
//    public partial struct SkillAttack_4020131 : ISkillAttack
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
//        /// 4 是否是弹幕 失效
//        /// </summary>
//        public bool isBullet;

//        /// <summary>
//        /// 5 弹幕hp  失效
//        /// </summary>
//        public int hp;

//        /// <summary>
//        /// 6 是否是持续性攻击  持续性攻击技能系统 失效
//        /// </summary>
//        public bool stayAttack;

//        /// <summary>
//        /// 7 持续性攻击间隔  持续性攻击技能系统
//        /// </summary>
//        public float stayAttackInterval;

//        /// <summary>
//        /// 8 当前持续性攻击间隔  持续性攻击技能系统
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

//        public float radius;


//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//            return refData.cdfeLocalTransform[inData.entity];
//            //// UnityEngine.Debug.Log("dfsafsadfasdfsdfaaaaaaaaa");
//            //return newLoc;

//            // UnityEngine.Debug.Log($"postion:{newq.Position},scale:{newq.Scale},rotation:{newq.Rotation}");
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            for (int i = 0; i < inData.allEntities.Length; ++i)
//            {
//                float dis = math.length(refData.cdfeLocalTransform[inData.entity].Position -
//                                        refData.cdfeLocalTransform[inData.allEntities[i]].Position);
//                if (math.abs(dis - radius) < 0.1f)
//                {
//                    //refData.ecb.AppendToBuffer(inData.sortkey, inData.allEntities[i],
//                    //    new Buff_20003003 { carrier = inData.allEntities[i], duration = duration }.ToBuffOld());

//                }
//            }
//        }
//    }


//}


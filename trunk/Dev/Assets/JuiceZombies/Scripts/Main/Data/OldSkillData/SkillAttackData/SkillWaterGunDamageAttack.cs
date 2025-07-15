//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;


//namespace Main
//{
//    public partial struct SkillWaterGunDamageAttack : ISkillAttack
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

//        public Entity skillEntity;

//        public LocalTransform curLoc;

//        public float3 dir;
//        public int pushRate;
//        public int damageRate;

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            // UnityEngine.Debug.Log("dfsafsadfasdfsdfaaaaaaaaa");
//            return curLoc;

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

//        }
//    }
//}


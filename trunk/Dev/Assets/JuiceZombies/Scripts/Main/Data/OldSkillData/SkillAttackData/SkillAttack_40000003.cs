//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;

//namespace Main
//{
//    //扇形击退攻击
//    public partial struct SkillAttack_40000003 : ISkillAttack
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

//        /// 扇形角度
//        /// </summary>
//        public float angle;

//        public float radius;

//        public float diffuseSpeed;

//        public float scale;

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            var actionspeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON? 1: inData.cdfeChaStats[caster].chaResource.actionSpeed;
//            float aByFrame = diffuseSpeed / 50*actionspeed;
//            float radiansByFrame = math.radians(aByFrame);
//            // 更新物体的位置和旋转角度
//            var newLoc = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.player].Position,
//                Scale = scale,
//                Rotation = refData.cdfeLocalTransform[inData.entity].Rotation,
//            };

//            var newq = newLoc.RotateZ(radiansByFrame);
//            //UnityEngine.Debug.Log($"postion:{newq.Position},scale:{newq.Scale},rotation:{newq.Rotation}");

//            return newq;
//        }

//        /// <summary>
//        /// 如果为有充能的技能实体 onDestroy为重置操作
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
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


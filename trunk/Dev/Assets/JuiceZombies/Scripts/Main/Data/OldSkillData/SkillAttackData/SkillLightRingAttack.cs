//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using Unity.Entities;

//namespace Main
//{
//    public partial struct SkillLifhtRingAttack : ISkillAttack
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

//        public Entity entity;

//        public float radius;

//        public float scale;


//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            // 更新物体的Scale和旋转角度
//            var newLoc = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.player].Position,
//                Scale = refData.cdfeLocalTransform[inData.entity].Scale,
//                Rotation = refData.cdfeLocalTransform[inData.entity].Rotation,
//            };

//            //var newq = newLoc.RotateZ(radiansByFrame);
//          //  UnityEngine.Debug.Log($"postion:{newLoc.Position},scale:{newLoc.Scale},rotation:{newLoc.Rotation}");

//            return newLoc;
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            //throw new System.NotImplementedException();
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//    }
//}


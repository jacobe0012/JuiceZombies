//using Unity.Transforms;
//using Unity.Entities;
//using Unity.Mathematics;

//namespace Main
//{
//    public partial struct SkillMotorAttack : ISkillAttack
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

//        public Entity skillPrefab;

//        public LocalTransform currentLoc;

//        public float diffuseSpeed;
//        public float width;

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//            var actionspeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON ? 1 : inData.cdfeChaStats[caster].chaResource.actionSpeed;
//            diffuseSpeed*=actionspeed;
//            //UnityEngine.Debug.Log($"dfsdfsfsfsafffffffffffffffffffffffffff");
//            // 更新物体的位置
//            var newLoc = new LocalTransform
//            {
//                Position = currentLoc.Position,
//                Scale = currentLoc.Scale,
//                Rotation = currentLoc.Rotation,
//            };

//            var newq = newLoc.Translate(newLoc.Up()*diffuseSpeed * 50*tick / 1000);
//            // UnityEngine.Debug.Log($"postion:{newq.Position},scale:{newq.Scale},rotation:{newq.Rotation}");
//           // UnityEngine.Debug.Log($"newLoc{newq}");
//            return newq;
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//           // throw new System.NotImplementedException();
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            // refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, inData.entity,
//            //     new SkillHitEffectBuffer
//            //     {
//            //         buffID = 40000008,
//            //         buffArgs = new float3x4 { c0 = new float3(10000000, 0, 0), c1 = dir, c2 = new float3(2, 0, 0) }
//            //     });
//            // refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, inData.entity,
//            //     new SkillHitEffectBuffer
//            //         { buffID = 30000003, buffArgs = new float3x4 { c0 = new float3(attackRate, 0, 0) } });
//           // GenerateProp(ref refData, inData, newLoc);
//        }
//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//    }
//}


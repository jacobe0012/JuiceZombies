//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;


//namespace Main
//{
//    public partial struct SkillTrapAttack : ISkillAttack
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
//        public float width;
//        public float height;
//        public Entity currentEnemy;
//        public LocalTransform skillLoc;


//        public bool isBreak;

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (!refData.cdfeLocalTransform.HasComponent(currentEnemy)|| !refData.cdfeLocalTransform.HasComponent(inData.entity))
//            {
//               // UnityEngine.Debug.Log($"currentEnemy:{currentEnemy.Index}");
//                var temp = new PostTransformMatrix
//                {
//                    Value = float4x4.Scale(0, 0, 1)
//                };
//                refData.ecb.AddComponent(inData.sortkey, inData.entity, temp);
//                return skillLoc;
//            }


//            float distance = math.distance(refData.cdfeLocalTransform[currentEnemy].Position, skillLoc.Position);


//            float3 dir = refData.cdfeLocalTransform[currentEnemy].Position - skillLoc.Position;
//            if (distance >= height)
//            {
//                distance = height; 
//            }
//            var enemyLoc = new LocalTransform
//            {
//                Position = skillLoc.Position + math.normalizesafe(dir) * distance,
//                Scale = refData.cdfeLocalTransform[currentEnemy].Scale,
//                Rotation = refData.cdfeLocalTransform[currentEnemy].Rotation
//            };
//            refData.ecb.SetComponent(inData.sortkey, currentEnemy, enemyLoc);

//            float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                new float3(0, 1, 0));


//            var qua = quaternion.AxisAngle(new float3(0, 0, 1),
//                math.radians(needAngel));

//            var newLoc = new LocalTransform
//            {
//                Position = skillLoc.Position + 0.5f * dir,
//                Scale = 1,
//                Rotation = qua
//            };

//            var pos = new PostTransformMatrix
//            {
//                Value = float4x4.Scale(1, distance, 1)
//            };
//            refData.ecb.AddComponent(inData.sortkey, inData.entity, pos);

//            return newLoc;
//            //// UnityEngine.Debug.Log("dfsafsadfasdfsdfaaaaaaaaa");
//            //return newLoc;

//            // UnityEngine.Debug.Log($"postion:{newq.Position},scale:{newq.Scale},rotation:{newq.Rotation}");
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (inData.cdfeTrapTag.HasComponent(currentEnemy))
//                refData.ecb.RemoveComponent<TrapTag>(inData.sortkey, currentEnemy);
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//    }
//}


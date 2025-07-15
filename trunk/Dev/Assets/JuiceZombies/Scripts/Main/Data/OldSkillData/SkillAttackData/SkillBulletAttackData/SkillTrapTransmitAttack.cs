//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;

//namespace Main
//{
//    //陷阱发射
//    public partial struct SkillTrapTransmitAttack : ISkillAttack
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

//        public LocalTransform localTrans;


//        public float3 dir;

//        public float flySpeed;

//        public Entity transmitTarget;

//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        /// 
//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            var actionspeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON ? 1 : inData.cdfeChaStats[caster].chaResource.actionSpeed;
//            flySpeed *= actionspeed;
//            var newLoc = new LocalTransform
//            {
//                Position = localTrans.Position + math.normalizesafe(dir) * 100 * inData.fDT,
//                Scale = refData.cdfeLocalTransform[inData.entity].Scale,
//                Rotation = refData.cdfeLocalTransform[inData.entity].Rotation
//            };
//            if (inData.cdfeObstacleTag.HasComponent(target)||inData.cdfePlayerData.HasComponent(target)||inData.cdfeTrapTag.HasComponent(target))
//            {
//                return localTrans;
//            }
//            //var newq= newLoc.Translate(newLoc.Up() * flySpeed * 50 * tick / 1000);
//           // UnityEngine.Debug.Log($"newLoc{newLoc},flyspeed{flySpeed},dir{dir}");   
//            localTrans= newLoc;


//            return newLoc;
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            // throw new System.NotImplementedException();
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            var skillLoc = refData.cdfeLocalTransform[inData.entity];
//            if (refData.storageInfoFromEntity.Exists(transmitTarget) && refData.cdfeLocalTransform.HasComponent(transmitTarget))
//            {
//                if (!inData.bfeBossTag.HasComponent(transmitTarget))
//                {
//                    var loc = refData.cdfeLocalTransform[transmitTarget];
//                    loc.Position = skillLoc.Position;
//                    loc.Rotation = skillLoc.Rotation;
//                    refData.cdfeLocalTransform[transmitTarget] = loc;
//                }
//            }


//        }
//    }
//}


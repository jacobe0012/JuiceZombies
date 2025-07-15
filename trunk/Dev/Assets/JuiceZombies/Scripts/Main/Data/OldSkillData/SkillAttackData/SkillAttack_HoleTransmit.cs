//using cfg.blobstruct;

//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;

//namespace Main
//{
//    //黑洞发射
//    public partial struct SkillAttack_HoleTransmit : ISkillAttack
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

//        public  uint oldColliderWith;

//        public LocalTransform loc;

//        public  int flySpeed;

//        public int additon;

//        public int parentSkillID;


//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//            var flySpeedOfTick = flySpeed / 1000f / (1 / inData.fDT);


//            //float tempDis = 999f;
//            for (int i = 0; i < inData.otherEntities.Length; i++)
//            {
//                if (inData.cdfeObstacleTag.HasComponent(inData.otherEntities[i]))
//                {
//                    SetMonsterPos(ref refData, inData, loc);
//                   // UnityEngine.Debug.Log($"trans111111111111111");
//                    return loc;
//                    //var dis = math.distance(refData.cdfeLocalTransform[inData.otherEntities[i]].Position,loc.Position);
//                    //tempDis=tempDis<dis?tempDis:dis;
//                    //UnityEngine.Debug.Log($"tempDis{tempDis}");
//                }
//            }
//            loc.Position += inData.cdfeChaStats[inData.player].chaResource.direction * flySpeedOfTick;
//            SetMonsterPos(ref refData, inData, loc);
//            //var dis1 = math.distance(newPos, loc.Position);
//            //if (dis1 < tempDis)
//            //{
//            //    loc.Position = newPos;
//            //}

//           // UnityEngine.Debug.Log($"trans2222222222222");
//            return loc;
//        }


//        private void SetMonsterPos(ref SkillAttackData_ReadWrite refData, SkillAttackData_ReadOnly inData, LocalTransform loc)
//        {
//            for (int i = 0; i < inData.allEntities.Length; i++)
//            {
//                var entity = inData.allEntities[i];

//                //if (!refData.storageInfoFromEntity.Exists(entity)) { return loc; }
//                if (inData.cdfeBlackHoleSuction.HasComponent(entity))
//                {
//                    var targetLoc = refData.cdfeLocalTransform[entity];
//                    targetLoc.Position = loc.Position;
//                    refData.cdfeLocalTransform[entity] = targetLoc;
//                }

//            }
//        }

//        /// <summary>
//        /// 如果为有充能的技能实体 onDestroy为重置操作
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            // UnityEngine.Debug.Log($"OnDestroy");

//            var buff_ = BuffHelper.UpdateParmeterNew(inData.config, id, 50000005);
//            var gap = buff_[0].x / 1000f;
//            var nextEffectID = buff_[1].x;

//            var buff1 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 40000003);
//            var pushRate = buff1[2].x*(1+additon);
//            var pushPropRate = buff1[2].z;

//            var buff2 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 30000003);
//            var damageRate = buff2[0].x;
//            var damagePropRate = buff2[0].z;

//            var buff3 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 30000004);
//            var obstDamage = buff3[0].x;

//            //UnityEngine.Debug.Log($"length:{inData.allEntities.Length}");


//            for (int i = 0; i < inData.allEntities.Length; i++)
//            {
//                var entity = inData.allEntities[i];
//                if (refData.cdfePhysicsCollider.HasComponent(entity) && inData.cdfeBlackHoleSuction.HasComponent(entity))
//                {
//                    //  UnityEngine.Debug.Log($"set set");
//                    var temp = refData.cdfePhysicsCollider[entity];
//                    var newCollider = PhysicsHelper.CreateColliderWithCollidesWith(temp, oldColliderWith);
//                    refData.ecb.SetComponent<PhysicsCollider>(inData.sortkey, entity, newCollider);
//                    refData.ecb.RemoveComponent<BlackHoleSuction>(inData.sortkey, entity);
//                }

//            }

//            // UnityEngine.Debug.Log($"pushRate:{pushRate},damageRate:{damageRate},obstDamage:{obstDamage}");
//            var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);


//            var newpos = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.entity].Position,
//                //TODO:大小*10
//                Scale = 20,
//                Rotation = refData.cdfeLocalTransform[prefab].Rotation,
//            };

//            refData.ecb.SetComponent(inData.sortkey, ins, newpos);
//            refData.ecb.AddComponent(inData.sortkey, ins,
//                new SkillAttackData
//                {
//                    data = new SkillAttack_0
//                    {
//                        id = nextEffectID,
//                        duration = 0.02f,
//                        tick = 0,
//                        caster = inData.player,
//                        isBullet = false,
//                        hp = 0,
//                        stayAttack = false,
//                        stayAttackInterval = 0,
//                        curStayAttackInterval = 0,
//                        skillDelay = gap
//                    }.ToSkillAttack()
//                });

//            refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            {
//                buffID = 40000014,
//                buffArgs = new float3x4 { c0 = new float3(pushRate, 209000, pushPropRate) }
//            });
//            refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            {
//                buffID = 30000003,
//                buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//            });
//            refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            {
//                buffID = 30000004,
//                buffArgs = new float3x4 { c0 = new float3(obstDamage, 0, 0) }
//            });

//            if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 401036, inData.config, out int needEffectID))
//            {
//                //if (!BuffHelper.TryGetParmeter(inData.config, needEffectID, 40000022)) return;
//                //var buff = BuffHelper.UpdateParmeter(inData.config, needEffectID, 40000022);
//                if (!BuffHelper.TryGetParmeter(inData.config, parentSkillID, 40000013)) return;
//                var buff1_ = BuffHelper.UpdateParmeter(inData.config, parentSkillID, 40000013);
//                var radius = buff1_[1].x * (1 + inData.cdfeChaStats[inData.player].chaProperty.skillRangeRatios * (buff1[1].z / 10000f));
//                float duration = buff1_[0].x / 1000f;

//                if (!BuffHelper.TryGetParmeter(inData.config, parentSkillID, 50000001)) return;
//                var buff2_ = BuffHelper.UpdateParmeter(inData.config, parentSkillID, 50000001);
//                var offset = buff2_[1].x;

//                if (!BuffHelper.TryGetParmeter(inData.config, parentSkillID, 30000004)) return;
//                var buff3_ = BuffHelper.UpdateParmeter(inData.config, parentSkillID, 30000004);
//                var obstDamage1 = buff3_[1].x;


//                // Debug.Log($"skillEffectID444444为{currentSkillEffectID}");
//                var skillPrefab = inData.prefabMapData.prefabHashMap["SkillMagnet"];
//                var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//                refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//                {

//                    data = new SkillAttack_BlackHole
//                    {
//                        id = 40000021,
//                        caster = inData.player,
//                        duration = duration,
//                        skillDelay = 0,
//                        curStayAttackInterval = 0,
//                        hp = 999,
//                        offset = offset,
//                        radius = radius,
//                        isBullet = true,
//                        skillPrefab = skillPrefab,
//                        stayAttack = false,
//                        stayAttackInterval = 0,
//                        tick = 0,
//                        trueSkillID = parentSkillID,
//                    }.ToSkillAttack()
//                });

//            }
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            //if (!refData.storageInfoFromEntity.Exists(target)) { return; }
//            //if(! refData.cdfePhysicsCollider.HasComponent(target) ) { return; }
//            //if(inData.cdfeObstacleTag.HasComponent(target)) { return; }
//            //var temp = refData.cdfePhysicsCollider[target];
//            //oldColliderWith= temp.Value.Value.GetCollisionFilter().CollidesWith;
//            //var newCollider= PhysicsHelper.CreateColliderWithCollidesWith(temp, 0);

//            //refData.ecb.SetComponent<PhysicsCollider>(inData.sortkey,target, newCollider);
//            //refData.ecb.AddComponent<BlackHoleSuction>(inData.sortkey, target);

//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {


//        }


//    }
//}


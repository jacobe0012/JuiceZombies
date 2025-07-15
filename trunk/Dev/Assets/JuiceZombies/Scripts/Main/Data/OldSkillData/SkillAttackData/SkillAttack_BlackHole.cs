//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;

//namespace Main
//{
//    //黑洞
//    public partial struct SkillAttack_BlackHole : ISkillAttack
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


//        /// 偏移量
//        /// </summary>
//        public float offset;

//        public float radius;

//        public Entity skillPrefab;

//        private uint oldColliderWith;

//        private  LocalTransform loc;

//        public int trueSkillID;

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//           // if(refData.storageInfoFromEntity.Exists(target)&& inData.cdfeObstacleTag.HasComponent(target)) { return loc; }
//           for(int i = 0; i < inData.otherEntities.Length; i++)
//            {
//                if (inData.cdfeObstacleTag.HasComponent(inData.otherEntities[i])||inData.otherEntities[i]==inData.player) {
//                    var newLoc1 = new LocalTransform
//                    {

//                        Position = refData.cdfeLocalTransform[inData.player].Position ,
//                        Scale = refData.cdfeLocalTransform[skillPrefab].Scale * radius,
//                        Rotation = refData.cdfeLocalTransform[inData.player].Rotation,
//                    };
//                    loc = newLoc1;
//                    SetMonsterPos(ref refData, inData, loc);
//                   // UnityEngine.Debug.Log($"trans3333333333333333");
//                    return loc;
//                    //var dis =math.distance( refData.cdfeLocalTransform[ inData.otherEntities[i]].Position,refData.cdfeLocalTransform[inData.player].Position);
//                    //offset=offset>dis?dis:offset;
//                }
//            }

//            var dir = inData.cdfeChaStats[inData.player].chaResource.direction;
//            float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                    new UnityEngine.Vector3(0, 1, 0));

//            var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));


//            // 更新物体的位置
//            var newLoc = new LocalTransform
//            {

//                Position = refData.cdfeLocalTransform[inData.player].Position + dir * offset,
//                Scale = refData.cdfeLocalTransform[skillPrefab].Scale * radius,
//                Rotation = qua,
//            };

//            loc = newLoc;
//            SetMonsterPos(ref refData,inData, loc);
//            //UnityEngine.Debug.Log($"trans44444444444444444");
//            return loc;
//        }

//        private void SetMonsterPos(ref SkillAttackData_ReadWrite refData, SkillAttackData_ReadOnly inData, LocalTransform loc)
//        {
//            // UnityEngine.Debug.Log($"target{target}");
//            if (!refData.storageInfoFromEntity.Exists(target)) { return; }
//            if (!refData.cdfeLocalTransform.HasComponent(target)) { return; }
//            if (inData.cdfeObstacleTag.HasComponent(target)) { return; }
//            var targetLoc = refData.cdfeLocalTransform[target];
//            targetLoc.Position = loc.Position;
//            refData.cdfeLocalTransform[target] = targetLoc;
//        }


//        /// <summary>
//        /// 如果为有充能的技能实体 onDestroy为重置操作
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            int addition = 0;
//            if(BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 401034, inData.config,out int needEffectID))
//            {
//                if(!BuffHelper.TryGetParmeter(inData.config, needEffectID, 40000021))return;
//                var buff = BuffHelper.UpdateParmeter(inData.config, needEffectID, 40000021);
//                addition = buff[0].x;
//            }


//            UnityEngine.Debug.Log($"OnDestroyBlackHole");
//            if (!BuffHelper.TryGetParmeterNew(inData.config, id, 50000005)) return;
//            var buff_ = BuffHelper.UpdateParmeterNew(inData.config, id, 50000005);
//            var gap = buff_[0].x / 1000f;
//            var nextEffectID = buff_[1].x;

//            if (nextEffectID == 1004062)
//            {
//                if (!BuffHelper.TryGetParmeterNew(inData.config, nextEffectID, 30000003)) return;
//                var buff2 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 30000003);
//                var damageRate = buff2[0].x;
//                var damagePropRate = buff2[0].z;


//                var prefab = inData.prefabMapData.prefabHashMap["SkillMagnet"];
//                var skillEntity = refData.ecb.Instantiate(inData.sortkey, prefab);


//                if (!BuffHelper.TryGetParmeter(inData.config, nextEffectID, 40000015)) return;
//                var buff = BuffHelper.UpdateParmeter(inData.config, nextEffectID, 40000015);
//                var scale = buff[1].x;
//                var rollDuration = buff[0].x / 1000f;
//                var rollSpeed = buff[2].x*(1+addition);

//                loc.Scale = refData.cdfeLocalTransform[prefab].Scale * scale;

//                refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//                {
//                    data = new SkillAttack_HoleRoll
//                    {
//                        id = nextEffectID,
//                        caster = inData.player,
//                        duration = rollDuration,
//                        skillDelay = gap,
//                        curStayAttackInterval = 0,
//                        hp = 999,
//                        oldColliderWith = oldColliderWith,
//                        isBullet = true,
//                        stayAttack = false,
//                        stayAttackInterval = 0,
//                        tick = 0,
//                        loc = loc,
//                        target = default,
//                        rollSpeed = rollSpeed,

//                    }.ToSkillAttack()
//                });
//                refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//                {
//                    buffID = 30000003,
//                    buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//                });


//            }
//            else
//            {
//                if (!BuffHelper.TryGetParmeterNew(inData.config, nextEffectID, 30000012)) return;
//                var buff2 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 30000012);
//                var damageRate = buff2[0].x;
//                var damagePropRate = buff2[0].z;
//                var damageGap = buff2[1].x / 1000f;

//                if (!BuffHelper.TryGetParmeter(inData.config, nextEffectID, 40000014)) return;
//                var buff = BuffHelper.UpdateParmeter(inData.config, nextEffectID, 40000014);
//                var transmitDuration = buff[0].x / 1000f;
//                var flySpeed = buff[2].x * (1 + addition);
//                var skillPrefab = inData.prefabMapData.prefabHashMap["SkillMagnet"];
//                var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//                refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//                {
//                    data = new SkillAttack_HoleTransmit
//                    {
//                        id = nextEffectID,
//                        caster = inData.player,
//                        duration = transmitDuration,
//                        skillDelay = gap,
//                        curStayAttackInterval = 0,
//                        hp = 999,
//                        oldColliderWith = oldColliderWith,
//                        isBullet = true,
//                        stayAttack = true,
//                        stayAttackInterval = damageGap,
//                        tick = 0,
//                        loc = loc,
//                        target = default,
//                        flySpeed = flySpeed,
//                        additon=addition,
//                        parentSkillID=id
//                    }.ToSkillAttack()
//                });
//                refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//                {
//                    buffID = 30000012,
//                    buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//                });


//            }


//        }


//        /// <summary>
//        /// 此刻生成新的技能实体  用来做吸怪的效果
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//            //装备技能时不执行ondestroy的逻辑 所以id必定要传一个不可读buff的id 但是需要follow
//            if(id== 40000021)
//            {
//                id = trueSkillID;
//            }

//            if (!BuffHelper.TryGetParmeterNew(inData.config, id, 30000012)) return;
//            var buff = BuffHelper.UpdateParmeterNew(inData.config, id, 30000012);
//            var damageRate = buff[0].x;
//            var damagePropRate = buff[0].z;
//            var damageGap = buff[1].x / 1000f;

//            var skillPrefab = inData.prefabMapData.prefabHashMap["SkillMagnet"];
//            var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//            refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//            {

//                data = new SkillAttack_FollowOther
//                {
//                    id = id,
//                    caster = inData.player,
//                    duration = duration,
//                    skillDelay = skillDelay,
//                    curStayAttackInterval = 0,
//                    hp = 999,
//                    isBullet = true,
//                    stayAttack = true,
//                    stayAttackInterval = damageGap,
//                    tick = 0,
//                    flowed=target,
//                    target = target,
//                }.ToSkillAttack()
//            });

//            UnityEngine.Debug.Log($"damageRate:{damageRate}");
//            refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//            {
//                buffID = 30000012,
//                buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//            });


//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (!refData.storageInfoFromEntity.Exists(target)) { return; }
//            if(! refData.cdfePhysicsCollider.HasComponent(target) ) { return; }
//            if(inData.cdfeObstacleTag.HasComponent(target)) { return; }
//            var temp = refData.cdfePhysicsCollider[target];
//            oldColliderWith= temp.Value.Value.GetCollisionFilter().CollidesWith;
//           // UnityEngine.Debug.Log($"{oldColliderWith}oldColliderWith");
//            var newCollider= PhysicsHelper.CreateColliderWithCollidesWith(temp, 0);

//            refData.ecb.SetComponent<PhysicsCollider>(inData.sortkey,target, newCollider);
//            refData.ecb.AddComponent<BlackHoleSuction>(inData.sortkey, target);

//        }
//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }


//    }
//}


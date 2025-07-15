//using cfg.blobstruct;

//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;

//namespace Main
//{
//    //链锤的逻辑
//    public partial struct SkillAttack_Hammer : ISkillAttack
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

//        public LocalTransform loc;

//        public int rollSpeed;
//        public float scale;

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            var newLoc = loc;
//            newLoc.Scale = scale;
//            //var RollySpeedOfTick = rollSpeed / 1000f / (1 / inData.fDT);
//            var RollySpeedOfTick = 20 / (1 / inData.fDT);
//            for (int i = 0; i < inData.otherEntities.Length; i++)
//            {
//                if (inData.cdfeObstacleTag.HasComponent(inData.otherEntities[i]))
//                {
//                   // SetMonsterPos(ref refData, inData, loc);
//                    //loc = loc.RotateZ(RollySpeedOfTick);
//                    newLoc=newLoc.RotateZ(RollySpeedOfTick*tick);
//                    UnityEngine.Debug.Log($"dddddddddddddddddddddloc.Position:{loc.Position}");
//                    return newLoc;
//                }
//            }

//            //var newq = newLoc.Translate(newLoc.Up() * diffuseSpeed * 50 * tick / 1000);
//            //loc = loc.RotateZ(RollySpeedOfTick);
//            newLoc = newLoc.RotateZ(RollySpeedOfTick * tick);
//            newLoc =newLoc.Translate(loc.Up() * RollySpeedOfTick * tick);

//            //loc.Position += inData.cdfeChaStats[inData.player].chaResource.direction * RollySpeedOfTick;
//            UnityEngine.Debug.Log($"loc.Position:{loc.Position},RollySpeedOfTick:{RollySpeedOfTick}");
//            // SetMonsterPos(ref refData, inData, loc);

//            return newLoc;
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
//            // UnityEngine.Debug.Log($"OnDestroy");

//            var buff_ = BuffHelper.UpdateParmeterNew(inData.config, id, 50000005);
//            var gap = buff_[0].x / 1000f;
//            var nextEffectID = buff_[1].x;

//            var buff1 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 40000003);
//            var scale = buff1[1].x;
//            var pushRate = buff1[2].x;
//            var pushPropRate = buff1[2].z;

//            var buff2 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 30000003);
//            var damageRate = buff2[0].x;
//            var damagePropRate = buff2[0].z;

//            var buff3 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 30000004);
//            var obstDamage = buff3[0].x;

//            //UnityEngine.Debug.Log($"length:{inData.allEntities.Length}");


//            // UnityEngine.Debug.Log($"pushRate:{pushRate},damageRate:{damageRate},obstDamage:{obstDamage}");
//            var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);


//            var newpos = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.entity].Position,
//                //TODO:大小*10
//                Scale = scale,
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
//                        skillDelay=gap
//                    }.ToSkillAttack()
//                });

//            refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            {
//                buffID = 40000003,
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

//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (!refData.storageInfoFromEntity.Exists(target)) { return; }
//            //if(! refData.cdfePhysicsCollider.HasComponent(target) ) { return; }
//            if(inData.cdfeObstacleTag.HasComponent(target)) { return; }

//            var buff1 = BuffHelper.UpdateParmeter(inData.config, id, 40000015);
//            var pushGap = buff1[3].x/1000f;
//            var pushRate = buff1[4].x;
//            var pushPropRate = buff1[4].z;
//            UnityEngine.Debug.Log($"pushRate{pushRate} ,pushGap{pushGap}");
//            var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);


//            var newpos = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.entity].Position,
//                //TODO:大小*10
//                Scale = 30,
//                Rotation = refData.cdfeLocalTransform[prefab].Rotation,
//            };

//            refData.ecb.SetComponent(inData.sortkey, ins, newpos);
//            refData.ecb.AddComponent(inData.sortkey, ins,
//                new SkillAttackData
//                {
//                    data = new SkillAttack_0
//                    {
//                        id = id,
//                        duration = 0.02f,
//                        tick = 0,
//                        caster = inData.player,
//                        isBullet = false,
//                        hp = 0,
//                        stayAttack = false,
//                        stayAttackInterval = 0,
//                        curStayAttackInterval = 0
//                    }.ToSkillAttack()
//                });

//            refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            {
//                buffID = 40000015,
//                buffArgs = new float3x4 { c0 = new float3(pushRate, 209000, pushPropRate) }
//            });


//            // var temp = refData.cdfePhysicsCollider[target];
//            // //oldColliderWith= temp.Value.Value.GetCollisionFilter().CollidesWith;
//            //// UnityEngine.Debug.Log($"{oldColliderWith}oldColliderWith");
//            // var newCollider= PhysicsHelper.CreateColliderWithCollidesWith(temp, 0);

//            // refData.ecb.SetComponent<PhysicsCollider>(inData.sortkey,target, newCollider);
//             refData.ecb.AppendToBuffer(inData.sortkey, target,new Buff_IgnoreDamgeOrPush {caster=default, duration=pushGap,carrier=target,buffArgs=new BuffArgs {args0=id,args1=0} }.ToBuffOld());

//        }
//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {


//        }

//    }
//}


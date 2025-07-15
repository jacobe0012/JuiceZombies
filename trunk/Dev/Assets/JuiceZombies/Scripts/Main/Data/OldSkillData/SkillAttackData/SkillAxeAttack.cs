//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    //斧头的技能实体逻辑
//    public partial struct SkillAxeAttack : ISkillAttack
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
//            var actionspeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON
//                ? 1
//                : inData.cdfeChaStats[caster].chaResource.actionSpeed;
//            float aByFrame = diffuseSpeed * inData.fDT * actionspeed;
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
//            //UnityEngine.Debug.Log("dffdsffffffffffffffff1111111111111111111111");
//            var buff = BuffHelper.UpdateParmeterNew(inData.config, id, 50000005);
//            var gap = buff[0].x / 1000f;
//            var nextEffectID = buff[1].x;
//            ref var skillEffectsArray = ref inData.config.value.Value.configTbskill_effects.configTbskill_effects;
//            int limitDistance = 0;
//            for (int i = 0; i < skillEffectsArray.Length; i++)
//            {
//                if (skillEffectsArray[i].id == nextEffectID)
//                {
//                    limitDistance = skillEffectsArray[i].limit[1].y;
//                    break;
//                }
//            }


//            var buff1 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 40000008);

//            var width = buff1[0].x;
//            var height = buff1[1].x;
//            var pushForce = buff1[3].x;
//            var pushPropRate = buff1[3].z;
//            var diffuseSpeed = buff1[4].x / 100f;

//            var buff2 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 30000003);
//            var damageRate = buff2[0].x;
//            var damagePropRate = buff2[0].z;


//            float3 dir = inData.cdfeChaStats[inData.player].chaResource.direction;
//            if (IsAttackToEnemy(limitDistance, inData, ref refData))
//            {
//                dir = refData.cdfeLocalTransform[target].Position - refData.cdfeLocalTransform[inData.entity].Position;
//            }

//            dir = math.normalizesafe(dir);
//            //Debug.Log($"dir:{dir}");
//            //refData.ecb.SetComponent(inData.sortkey, skillEntity, enemyLoc);

//            int count = 1;
//            float angleOffset = 0;
//            float selfOffset = 0;
//            if (BuffHelper.TryGetParmeter(inData.config, nextEffectID, 50000007))
//            {
//                var buff3 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 50000007);
//                count = buff3[0].x;
//                angleOffset = buff3[1].x;
//                selfOffset = buff3[2].x;
//                //Debug.Log($"count:{count},angleOffset:{angleOffset}");
//            }

//            for (int i = 1; i <= count; i++)
//            {
//                var skillPrefab = inData.prefabMapData.prefabHashMap["SkillAxe"];
//                var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//                var newLoc = refData.cdfeLocalTransform[inData.player];
//                newLoc.Scale = scale;
//                if (i != 1)
//                {
//                    if (i % 2 == 0)
//                    {
//                        var q = quaternion.EulerXYZ(0, 0, math.radians(-angleOffset * (i / 2)));
//                        dir = math.mul(q, dir);
//                        //var q = quaternion.AxisAngle(new float3(0, 0, -1), math.radians(-angleOffset * (i / 2)));
//                        //q=new quaternion(q.value.x,q.value.y,q.value.z, -q.value.w);
//                        //dir = math.rotate(q, dir);
//                        Debug.Log($"i % 2 == 0:{i / 2}--------q:{q.value}");
//                        newLoc.Rotation = q;

//                        //var old = new Vector3(dir.x, dir.y, 0);
//                        //var f= Quaternion.AngleAxis(math.radians(-angleOffset * (i / 2)),new float3(0, 0, -1)) * old;

//                        //newLoc.r(math.radians(-angleOffset*(i/2)));
//                    }
//                    else
//                    {
//                        var q = quaternion.EulerXYZ(0, 0, math.radians(angleOffset * (i / 2)));
//                        dir = math.mul(q, dir);
//                        //var q = quaternion.AxisAngle(new float3(0, 0,-1), math.radians(angleOffset * (i / 2)));
//                        //dir = math.rotate(q, dir);
//                        Debug.Log($"i % 2 != 0:{i / 2}--------q:{q.value}");
//                        newLoc.Rotation = q;
//                    }
//                }

//                refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//                {
//                    data = new SkillAxeFlyAttack
//                    {
//                        dir = dir,
//                        id = nextEffectID,
//                        duration = height / diffuseSpeed,
//                        tick = 0,
//                        caster = inData.player,
//                        diffuseSpeed = diffuseSpeed,
//                        hp = 0,
//                        isBullet = false,
//                        skillDelay = gap,
//                        curStayAttackInterval = 0,
//                        stayAttack = false,
//                        stayAttackInterval = 0,
//                        scale = scale,
//                        loc = newLoc,
//                    }.ToSkillAttack()
//                });

//                refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//                {
//                    buffID = 40000008,
//                    buffArgs = new float3x4 { c0 = new float3(pushForce, 209000, pushPropRate) }
//                });
//                refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//                {
//                    buffID = 30000003,
//                    buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//                });
//            }
//        }

//        public bool IsAttackToEnemy(int limitDistance, SkillAttackData_ReadOnly inData,
//            ref SkillAttackData_ReadWrite refData)
//        {
//            //判断是否对怪
//            float distance = 999f;
//            target = BuffHelper.NerestTarget(inData.allEntities, refData.cdfeLocalTransform,
//                inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                inData.cdfeObstacleTag,
//                inData.entity, TargetAttackType.Enemy, ref distance);
//            if (distance > limitDistance)
//            {
//                target = default;
//            }

//            return target != default ? true : false;
//        }


//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }
//    }
//}


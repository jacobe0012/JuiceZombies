//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;

//namespace Main
//{
//    public partial struct SkillWaterGunAttack : ISkillAttack
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

//        public float width;
//        public float height;
//        public float scale;
//        public Entity currentEnemy;

//        public float pushRate;
//        public float damageRate;
//        public float buffDuration;
//        public int debuffRate;
//        public int buffStack;

//        private int attackTimes;

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {


//            //float aByFrame = diffuseSpeed / 50;
//            //float radiansByFrame = math.radians(aByFrame);
//            // 更新物体的位置和旋转角度


//            float distance = math.distance(refData.cdfeLocalTransform[currentEnemy].Position, refData.cdfeLocalTransform[inData.player].Position);
//            if (distance >= height)
//            {
//                distance= height;
//            }

//            var pos = new PostTransformMatrix
//            {
//                Value = float4x4.Scale(width, distance, 1)
//            };

//            // UnityEngine.Debug.Log($"distance:{distance}");

//            refData.ecb.AddComponent(inData.sortkey, skillEntity, pos);

//            float3 dir = refData.cdfeLocalTransform [currentEnemy].Position - refData.cdfeLocalTransform[inData.player].Position;
//            float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                new float3(0, 1, 0));
//            var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));

//            var newLoc = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.player].Position+ 0.5f* dir,
//                Scale = 1,
//                Rotation = qua
//            };


//            var damageAttackLoc = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.player].Position + dir,
//                Scale = 1,
//                Rotation = qua
//            };

//            Entity skillEntityDamage = default;
//            if (tick == 0)
//            {

//                var skillPrefab = inData.prefabMapData.prefabHashMap["SkillWaterGunDamage"];
//                skillEntityDamage = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//                var data = new SkillAttackData
//                {
//                    data = new SkillWaterGunDamageAttack
//                    {
//                        caster = inData.player,
//                        curLoc = damageAttackLoc,
//                        duration = duration,
//                        tick = 0,
//                        damageRate = (int)damageRate,
//                        pushRate =(int) pushRate,
//                    }.ToSkillAttack()
//                };
//                // 创建实际的技能实体 用于碰撞
//                refData.ecb.AddComponent(inData.sortkey, skillEntityDamage, data);
//            }
//            if (skillEntityDamage != default)
//            {
//                //添加推力和伤害
//                refData.ecb.AppendToBuffer(inData.sortkey, skillEntityDamage, new SkillHitEffectBuffer
//                {
//                    buffID = 40000011,
//                    buffArgs = new float3x4 { c0 = new float3(pushRate, 209000, 10000), c1 = dir }
//                });
//                refData.ecb.AppendToBuffer(inData.sortkey, skillEntityDamage, new SkillHitEffectBuffer
//                {
//                    buffID = 30000003,
//                    buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, 10000), c1 = dir }
//                });
//            }
//            if (id == 1002061)
//            {
//                refData.ecb.AppendToBuffer(inData.sortkey, currentEnemy, new Buff_20003007
//                {
//                    id=20003007,
//                    duration = buffDuration,
//                    carrier = currentEnemy,
//                    buffArgs = new BuffArgs { args0 = debuffRate },
//                }.ToBuffOld());


//            }

//            // UnityEngine.Debug.Log("dfsafsadfasdfsdfaaaaaaaaa");
//            return newLoc;

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

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {


//            if (attackTimes % 5 == 0)
//            {
//                if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 401024, inData.config, out int skillEffectID))
//                {
//                    duration += 0.1f;
//                }
//                if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 401026, inData.config, out skillEffectID))
//                {
//                    if (BuffHelper.TryGetParmeter(inData.config, skillEffectID, 40000026))
//                    {
//                        var buff = BuffHelper.UpdateParmeter(inData.config, skillEffectID, 40000026);
//                        var attackAddtion = buff[1].x;
//                        var pushAddtion = buff[2].x;
//                        var heightAddtion = buff[3].x;
//                        var maxTimes= buff[4].x;
//                        if (attackTimes > maxTimes * 5)
//                        {
//                            return;
//                        }
//                        pushRate *= (1 + pushAddtion / 10000f);
//                        damageRate *= (1 + attackAddtion / 10000f);
//                        height *= (1 + heightAddtion / 10000f);
//                    }
//                }
//            }


//            attackTimes++;
//           // throw new System.NotImplementedException();
//        }
//    }
//    public partial struct SkillWaterGun_S2_Attack : ISkillAttack
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

//        public float width;
//        public float height;
//        public Entity currentEnemy;
//        public Entity skillEntity;
//        public int pushRate;
//        public int damageRate;


//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {


//            //float aByFrame = diffuseSpeed / 50;
//            //float radiansByFrame = math.radians(aByFrame);
//            // 更新物体的位置和旋转角度


//            //float distance = math.distance(refData.cdfeLocalTransform[currentEnemy].Position, refData.cdfeLocalTransform[inData.player].Position);
//            //if (distance >= height)
//            //{
//            //    distance = height;
//            //}

//            var pos = new PostTransformMatrix
//            {
//                Value = float4x4.Scale(width, height, 1)
//            };

//            // UnityEngine.Debug.Log($"distance:{distance}");

//            refData.ecb.AddComponent(inData.sortkey, skillEntity, pos);

//            float3 dir = refData.cdfeLocalTransform[currentEnemy].Position - refData.cdfeLocalTransform[inData.player].Position;
//            float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                new float3(0, 1, 0));
//            var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));

//            var newLoc = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.player].Position + 0.5f * math.normalizesafe(dir) * height,
//                Scale = 1,
//                Rotation = qua
//            };

//            // UnityEngine.Debug.Log("dfsafsadfasdfsdfaaaaaaaaa");
//            return newLoc;

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

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            //throw new System.NotImplementedException();
//        }
//    }
//}


using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace Main
{
    //直线运动的弹幕
    public partial struct SkillAttack_14 : ISkillAttack
    {
        /// <summary>
        /// 0
        /// </summary>
        public int id;

        /// <summary>
        /// 1 一次性释放的技能实体 值=0
        /// </summary>
        public float duration;

        /// <summary>
        /// 2
        /// </summary>
        public int tick;

        /// <summary>
        /// 3 技能实体释放者
        /// </summary>
        public Entity caster;

        /// <summary>
        /// 4 是否是弹幕
        /// </summary>
        public bool isBullet;

        /// <summary>
        /// 5 弹幕hp 初始hp 当前hp
        /// </summary>
        public int hp;

        /// <summary>
        /// 实体的击中目标 6
        /// </summary>
        public Entity target;

        /// <summary>
        /// 7
        /// </summary>
        public int4 args;

        /// <summary>
        /// 弹幕速度 8
        /// </summary>
        public float speed;

        /// <summary>
        /// 弹幕用 触发器id 9
        /// </summary>
        public int triggerID;

        /// <summary>
        /// 是否被障碍物吸收 10
        /// </summary>
        public bool isAbsorb;

        /// <summary>
        /// 目标int值 11
        /// </summary>
        public int targetInt;

        /// <summary>
        /// 是否可触发OnStay回调 12
        /// </summary>
        public bool isOnStayTrigger;

        /// <summary>
        /// 弹幕旋转速度 13  度/s   
        /// </summary>
        public float rotateSpeed;

        /// <summary>
        /// 弹幕类型参数 14
        /// </summary>
        public float3 trackPar;

        /// <summary>
        /// 终点位置 15
        /// </summary>
        public float3 targetPos;

        /// <summary>
        /// 跟随的目标 16
        /// </summary>
        public Entity followedEntity;

        /// <summary>
        /// 当前可触发OnStay回调 冷却 17
        /// </summary>
        public float curOnStayTriggerCd;

        /// <summary>
        /// 总持续时间 18
        /// </summary>
        public float totalDuration;

        /// <summary>
        /// skill的id 19
        /// </summary>
        public int skillID;

        /// <summary>
        /// 是否走exit回调 20
        /// </summary>
        public bool enableExit;

        /// <summary>
        /// 回调类型 21  0:只走onEnter 1:走onEnter或者onStay
        /// </summary>
        public int funcType;

        /// <summary>
        /// 死亡触发器id 22 
        /// </summary>
        public int deadEffectID;

        /// <summary>
        /// OnStay回调cd 23
        /// </summary>
        public float onStayTriggerCd;

        /// <summary>
        /// 每时间触发的当前次数  24 用来处理多段矩形
        /// </summary>
        public int tirggerCount;

        /// <summary>
        /// 弹幕生成的tick时间 25
        /// </summary>
        public int curTick;

        /// <summary>
        /// 脏东西的entity 26
        /// </summary>
        public Entity holder;

        /// <summary>
        /// 是否是弹幕伤害 27
        /// </summary>
        public bool isBulletDamage;

        /// <summary>
        /// 是否是武器攻击 28
        /// </summary>
        public bool isWeaponAttack;

        /// <summary>
        /// 加速度 mm/s^2  29
        /// </summary>
        public int acceleration;

        /// <summary>
        /// 默认飞行方向 目标朝向向量 30 
        /// </summary>
        public float3 defaultDir;

        /// <summary>
        /// 弹幕最大速度 31
        /// </summary>
        public float defaultSpeed;

        /// <summary>
        /// triggerId 32
        /// </summary>
        public int triggerId;

        /// <summary>
        /// 用于冲锋的选点 33
        /// </summary>
        public float3 crashPos;


        
        //记录的飞行方向
        public float3 dir;

        //记录的释放着行动速度
        public float actionSpeed;
        private float3 oldCasterPos;

        /// <summary>
        /// 每帧做位置变动
        /// Tween参数:
        /// t:当前时间
        /// b:初始位置
        /// c:变化量
        /// d:持续时间
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        /// <returns>变动后的LT</returns>
        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {

            var acc = acceleration / 1000f;
            speed += acc * inData.fDT;
            var temp = refData.cdfeLocalTransform[inData.entity];
            temp.Position =refData.cdfeLocalTransform[caster].Position + math.normalize(defaultDir) * speed * inData.fDT * actionSpeed * tick;
         
        
            
            //temp.Position += math.normalize(defaultDir) * speed * inData.fDT * actionSpeed;
            //var dir= refData.cdfeLocalTransform[caster].Position - oldCasterPos;
            //temp.Position += dir;
        
            //Debug.Log($"att0 speed{speed} actionSpeed{actionSpeed}");
            //var radians = math.radians(rotateSpeed);
            //radians /= (1f / inData.fDT);
            //temp = temp.RotateZ(-radians * actionSpeed);

            return temp;
        }

        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            BuffHelper.AddTriggerForDeathBullet(ref refData, in inData, inData.sortkey, deadEffectID, caster, dir);
        }

        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            oldCasterPos = refData.cdfeLocalTransform[caster].Position;
            ref var bulletsConfig =
                ref inData.config.value.Value.configTbbullets.configTbbullets;

            int bulletIndex = -1;
            for (int i = 0; i < bulletsConfig.Length; i++)
            {
                if (bulletsConfig[i].id == id)
                {
                    bulletIndex = i;
                    break;
                }
            }

            ref var bullets =
                ref bulletsConfig[bulletIndex];
            //Debug.LogError($"{id}caster{caster}");
            actionSpeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON
                ? 1
                : inData.cdfeChaStats[caster].chaResource.actionSpeed;
            // if (bullets.r)
            // {
            //     
            // }
            dir = math.mul(refData.cdfeLocalTransform[inData.entity].Rotation, MathHelper.picForward);
            dir = defaultDir;
            Debug.Log($"bullets.id {id}  bullets.rotationType {bullets.rotationType}");
            if (bullets.rotationType == 2)
            {
                var tran = refData.cdfeLocalTransform[inData.entity];
                tran.Rotation = quaternion.identity;
                refData.cdfeLocalTransform[inData.entity] = tran;
            }
            else if (bullets.rotationType == 3)
            {
                var tran = refData.cdfeLocalTransform[inData.entity];
                tran.Rotation = quaternion.identity;
                refData.cdfeLocalTransform[inData.entity] = tran;
                if (defaultDir.x > 0)
                {
                    refData.ecb.SetComponent(inData.sortkey, inData.entity, new JiYuFlip
                    {
                        value = new int2(1, 0)
                    });
                }
            }

            //设置弹幕collider
            // if (!refData.storageInfoFromEntity.Exists(inData.entity) ||
            //     !refData.cdfeLocalTransform.HasComponent(inData.entity))
            // {
            //     return;
            // }

            //var oldColliderWith = refData.physicsCollider.Value.Value.GetCollisionFilter().CollidesWith;

            //Debug.LogError($"1CollidesWith {refData.physicsCollider.Value.Value.GetCollisionFilter().CollidesWith}");
            // refData.physicsCollider =
            //     PhysicsHelper.CreateColliderWithCollidesWith(refData.physicsCollider, (uint)targetInt | 128);
            //Debug.LogError($"2CollidesWith {refData.physicsCollider.Value.Value.GetCollisionFilter().CollidesWith}");
        }

        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            UnityEngine.Debug.Log($"OnHit1 {id}");
            if (!refData.storageInfoFromEntity.Exists(target) || !refData.cdfeLocalTransform.HasComponent(target))
            {
                return;
            }

            if (inData.cdfeObstacleTag.HasComponent(target) && isAbsorb)
            {
                UnityEngine.Debug.Log($"11isAbsorb {isAbsorb}");
                hp = 0;
                return;
            }
            //if(target.)

            BuffHelper.AddTriggerForBullet(ref refData, in inData, inData.sortkey, triggerID, caster, target, dir);
        }

        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        public void OnExit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        private void AddTriggerForBullet(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData,
            int sortkey, int triggerID, Entity caster)
        {
            var locer = refData.ecb.CreateEntity(inData.sortkey);
            refData.ecb.AddBuffer<Trigger>(inData.sortkey, locer);
            refData.ecb.AddBuffer<Skill>(inData.sortkey, locer);

            var dir = math.normalizesafe(refData.cdfeLocalTransform[inData.entity].Position -
                                         refData.cdfeLocalTransform[target].Position);

            var pos = refData.cdfeLocalTransform[target].Position +
                      dir * (refData.cdfeLocalTransform[target].Scale / 2f);

            pos.z = 0;

            refData.ecb.AddComponent(inData.sortkey, locer, new LocalTransform
            {
                Position = pos,
                Scale = 1,
                Rotation = default
            });
            refData.ecb.AddComponent(inData.sortkey, locer, new TimeToDieData
            {
                duration = 15f
            });

            ref var skillEffect =
                ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            int index = 0;
            for (int i = 0; i < skillEffect.Length; i++)
            {
                if (skillEffect[i].id == triggerID)
                {
                    index = i;
                    break;
                }
            }

            ref var skilleffect = ref skillEffect[index];

            ///先判断是否能添加trigger
            int power = skilleffect.power;
            var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.timeTick + caster.Index +
                                                                       caster.Version));
            var dropRate = math.clamp(power, 0, 10000);

            var canTrigger = rand.NextInt(0, 10001) <= dropRate;
            if (!canTrigger)
                return;

            ////判断是否包含 包含则重置trigger
            //bool isContain = false;
            //for (int j = 0; j < triggers.Length; j++)
            //{
            //    if (triggers[j].Int32_0 == skilleffect.id)
            //    {
            //        isContain = true;
            //        break;
            //    }
            //}
            //if (isContain)
            //{
            //    for (int j = 0; j < triggers.Length; j++)
            //    {
            //        if (triggers[j].Int32_0 == skilleffect.id)
            //        {
            //            var trigger = triggers[j];
            //            trigger.Int32_2 = 0;
            //            trigger.float4_6.z = trigger.float4_6.y;
            //            trigger.float3_3.z = trigger.float3_3.y;
            //            trigger.Single_15 =
            //                trigger.float4_6.x != 0 ? math.max(trigger.float4_6.x / 1000f, 0.02f) : 0.5f;
            //            trigger.Boolean_11 = triggerOld.Boolean_11;
            //            triggers[j] = trigger;
            //        }

            //    }
            //    return;
            //}

            float delay = 0;
            TriggerType type = (TriggerType)skilleffect.triggerType;
            int delayType = skilleffect.delayType;
            float4 triggerTypeArgs = default;
            int4 triggerConditionTypeArgs = default;
            TriggerConditionType condition = default;
            condition = (TriggerConditionType)(skilleffect.conditionType == 4
                ? skilleffect.conditionType * 10 + skilleffect.conditionTypePara[0]
                : skilleffect.conditionType);
            int skillid = skilleffect.skillId;
            for (int j = 0; j < skilleffect.triggerTypePara.Length; j++)
            {
                switch (j)
                {
                    case 0:
                        triggerTypeArgs.x = skilleffect.triggerTypePara[j];
                        if (type == TriggerType.PerTime)
                        {
                            triggerTypeArgs.x = skilleffect.triggerTypePara[j] / 1000f;
                        }

                        if (type == TriggerType.PerNum)
                        {
                            if (skilleffect.triggerTypePara.Length == 1)
                            {
                                triggerTypeArgs.y = MathHelper.MaxNum;
                            }
                        }

                        break;
                    case 1:
                        triggerTypeArgs.y = skilleffect.triggerTypePara[j];
                        if (type == TriggerType.PerNum)
                        {
                            if (skilleffect.triggerTypePara[j] == 0)
                            {
                                triggerTypeArgs.y = MathHelper.MaxNum;
                            }
                        }

                        break;
                    case 2:
                        triggerTypeArgs.z = skilleffect.triggerTypePara[j];
                        break;
                    case 3:
                        triggerTypeArgs.w = skilleffect.triggerTypePara[j];
                        break;
                }
            }

            for (int j = 0; j < skilleffect.conditionTypePara.Length; j++)
            {
                switch (j)
                {
                    case 0:
                        triggerConditionTypeArgs.x = skilleffect.conditionTypePara[j];
                        break;
                    case 1:
                        triggerConditionTypeArgs.y = skilleffect.conditionTypePara[j];
                        break;
                    case 2:
                        triggerConditionTypeArgs.z = skilleffect.conditionTypePara[j];
                        break;
                    case 3:
                        triggerConditionTypeArgs.w = skilleffect.conditionTypePara[j];
                        break;
                }
            }

            for (int j = 0; j < skilleffect.delayTypePara.Length; j++)
            {
                switch (j)
                {
                    case 0:
                        delay = skilleffect.delayTypePara[j] / 1000f;
                        break;
                }
            }

            float haloGap = 0.5f;
            if (skilleffect.triggerType == (int)TriggerType.Halo)
            {
                if (skilleffect.triggerTypePara[0] != 0)
                {
                    haloGap = skilleffect.triggerTypePara[0] / 1000f;
                }
            }

            //if (isDebug)
            //{
            //    Debug.Log($"dropRate AddTrigger:{dropRate}");
            //}


            refData.ecb.AppendToBuffer(sortkey, locer, new Trigger
            {
                CurrentTypeId = (Trigger.TypeId)101,
                Int32_0 = skilleffect.id,
                Single_1 = 0,
                Int32_2 = 0,
                float3_3 = new float3(delayType,
                    delay,
                    0),
                Boolean_4 = false,
                TriggerType_5 = type,
                float4_6 = triggerTypeArgs,
                TriggerConditionType_7 = condition,
                int4_8 = triggerConditionTypeArgs,
                float4_9 = default,
                Int32_10 = skillid,
                Boolean_11 = false,
                Entity_12 = caster,
                Boolean_13 = true,
                Int32_14 = power,
                Single_15 = haloGap,
                Int32_16 = 0,
                Int32_17 = skilleffect.compareType,
                Entity_20 = locer,
                LocalTransform_25 = refData.cdfeLocalTransform[inData.entity],
                float3x2_24 = new float3x2
                {
                    c0 = refData.cdfeLocalTransform[inData.entity].Position,
                    c1 = inData.cdfeChaStats[inData.entity].chaResource.direction
                }
            });
            //Debug.Log($"AddTrigger {skilleffect.id}");
        }

        /// <summary>
        /// 添加trigger
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        /// <param name="sortKey"></param>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="triggers"></param>
        /// <param name="triggerID"></param>
    }
}
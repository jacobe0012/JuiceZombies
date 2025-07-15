using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace Main
{
    //追踪 必须重合
    public partial struct SkillAttack_13 : ISkillAttack
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



        public float actionSpeed;

        //记录的飞行方向
        public float3 dir;

        public int rotationType;

        public quaternion tempQuaternion;

        private bool isOverlap;

        private int tickDie;
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

            if (refData.storageInfoFromEntity.Exists(followedEntity))
            {
                var targetPos = refData.cdfeLocalTransform[followedEntity].Position;
                var dir = math.normalizesafe(targetPos - refData.cdfeLocalTransform[inData.entity].Position);

                var newrot = MathHelper.LookRotation2D(dir);
                var degrees = MathHelper.Angle(tempQuaternion, newrot);
                if (math.abs(degrees) > math.EPSILON)
                {
                    tempQuaternion =
                        math.nlerp(tempQuaternion, newrot,
                            (trackPar.x / (1f / inData.fDT)) / degrees
                        );
                }
              
                var dis = math.distance(targetPos, temp.Position);
                //Debug.Log($"dddddddddddddddd:{dis}");
                if (dis <=1.5f&&tickDie<0)
                {
                    //Debug.Log($"isOverlap = true");
                    isOverlap = true;
                }
            }

            var forward = math.mul(tempQuaternion, MathHelper.picForward);
            var right = math.mul(tempQuaternion, math.right());

            float amplitude = trackPar.y / 1000f;
            float frequency = trackPar.z;
            float moveY = amplitude * math.sin(inData.eT * frequency); // 计算上下位移
            right *= moveY;
            forward += right;
            //Debug.LogError($"forward{forward} followedEntity{followedEntity.Index}");
            temp.Position += forward * speed * inData.fDT * actionSpeed;
            if (rotationType == 1)
            {
                temp.Rotation = tempQuaternion;
            }
            else if (rotationType == 2)
            {
                //var tran = refData.cdfeLocalTransform[inData.entity];
                temp.Rotation = quaternion.identity;
                //refData.cdfeLocalTransform[inData.entity] = tran;
            }


            return temp;
        }

        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            BuffHelper.AddTriggerForDeathBullet(ref refData, in inData, inData.sortkey, deadEffectID, caster, dir);
        }

        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            isOverlap = false;
            tickDie = -1;
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
            rotationType = bullets.rotationType;
            actionSpeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON
                ? 1
                : inData.cdfeChaStats[caster].chaResource.actionSpeed;
            //dir = math.mul(refData.cdfeLocalTransform[inData.entity].Rotation, MathHelper.picForward);
            if (rotationType == 2)
            {
                var tran = refData.cdfeLocalTransform[inData.entity];
                tran.Rotation = quaternion.identity;
                refData.cdfeLocalTransform[inData.entity] = tran;
            }
            else if (rotationType == 3)
            {
                var tran = refData.cdfeLocalTransform[inData.entity];
                tran.Rotation = quaternion.identity;
                refData.cdfeLocalTransform[inData.entity] = tran;
                if (dir.x > 0)
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
            //Debug.LogError($"OnHit1");
            if (!refData.storageInfoFromEntity.Exists(target) || !refData.cdfeLocalTransform.HasComponent(target))
            {
                return;
            }

            if (inData.cdfeObstacleTag.HasComponent(target) && isAbsorb)
            {
                hp = 0;
                return;
            }

         


        }


        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {


            if (isOverlap)
            {
                tickDie = tick + 1;
                isOverlap = false;
                BuffHelper.AddTriggerForBullet(ref refData, in inData, inData.sortkey, triggerID, caster, target, dir);
                Debug.Log($"isoverlap,tickDie:{tickDie}");
            }
            else
            {
                hp = 9999;
                duration = 9999f;
            }


            if (tick==tickDie&&tickDie > 0)
            {
                Debug.Log($"isoverlap,tick:{tick}");
                hp = 0;
                duration = 0;
            }

            // if (id == 1003063)
            // {
            //     var buff1 = BuffHelper.UpdateParmeter(inData.config, id, 20003001);
            //     var rate = buff1[1].x;
            //     var duration = buff1[0].x / 1000f;
            //     var probability = buff1[2].x;
            //     for (int i = 0; i < inData.otherEntities.Length; i++)
            //     {
            //         var rand = inData.rand.NextInt(probability, 10000);
            //         if (rand <= probability)
            //         {
            //             refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
            //                 new Buff_20003001
            //                 {
            //                     id = 20003001,
            //                     priority = 0,
            //                     maxStack = 0,
            //                     tags = 0,
            //                     tickTime = 0,
            //                     timeElapsed = 0,
            //                     ticked = 0,
            //                     duration = duration,
            //                     permanent = false,
            //                     caster = default,
            //                     buffArgs = new BuffArgs
            //                     {
            //                         args0 = rate
            //                     },
            //                     carrier = inData.otherEntities[i],
            //                     canBeStacked = false,
            //                     buffStack = default,
            //                 }.ToBuffOld());
            //         }
            //
            //         var buff2 = BuffHelper.UpdateParmeterNew(inData.config, id, 20003009);
            //         rate = buff2[0].x;
            //         duration = buff2[1].x / 1000f;
            //         refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
            //             new Buff_20003009
            //             {
            //                 id = 20003009,
            //                 priority = 0,
            //                 maxStack = 0,
            //                 tags = 0,
            //                 tickTime = 1f,
            //                 timeElapsed = 0,
            //                 ticked = 0,
            //                 duration = duration,
            //                 permanent = false,
            //                 caster = default,
            //                 buffArgs = new BuffArgs
            //                 {
            //                     args0 = rate
            //                 },
            //                 carrier = inData.otherEntities[i],
            //                 canBeStacked = false,
            //                 buffStack = default,
            //             }.ToBuffOld());
            //     }
            // }
            // else if (id == 1003073)
            // {
            //     for (int i = 0; i < inData.otherEntities.Length; i++)
            //     {
            //         refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
            //             new Buff_20003008
            //             {
            //                 id = 20003008,
            //                 priority = 0,
            //                 maxStack = 0,
            //                 tags = 0,
            //                 tickTime = 1f,
            //                 timeElapsed = 0,
            //                 ticked = 0,
            //                 duration = duration,
            //                 permanent = false,
            //                 caster = default,
            //                 buffArgs = new BuffArgs
            //                 {
            //                     args0 = id
            //                 },
            //                 carrier = inData.otherEntities[i],
            //                 canBeStacked = false,
            //                 buffStack = default,
            //             }.ToBuffOld());
            //     }
            // }
        }

        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        public void OnExit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }
    }
}
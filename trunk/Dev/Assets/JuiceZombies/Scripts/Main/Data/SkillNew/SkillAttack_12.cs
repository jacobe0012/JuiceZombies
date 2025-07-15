using cfg.config;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace Main
{
    //后撤弹幕
    public partial struct SkillAttack_12 : ISkillAttack
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

        private int backEndTick;

        private float3 startPos;

        private int revertTick;

        private float3 fadeDir;

        private int fadeTick;
        private float startDisappearTime;
        private float disappearDuration;

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
            if (!refData.storageInfoFromEntity.Exists(caster))
            {
                return temp;
            }

            var casterPos = refData.cdfeLocalTransform[caster].Position;

            if (tick <= backEndTick)
            {
                if (refData.storageInfoFromEntity.Exists(followedEntity))
                {
                    Debug.Log($"tick:{tick},backendtick:{backEndTick}");
                    var targetPos = refData.cdfeLocalTransform[followedEntity].Position;
                    dir = math.normalize(targetPos - casterPos);
                }
                else
                {
                    dir = inData.cdfeChaStats[caster].chaResource.direction;
                }

                temp.Rotation = MathHelper.LookRotation2D(dir);
                //float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir), new float3(0, 1, 0));

                //temp = MathHelper.RotateAround(temp, casterPos, math.radians(-needAngel));

                float3 offset = -MathHelper.Forward(temp.Rotation) * trackPar.x / 1000f * inData.fDT *
                                math.clamp((tick * 1f / backEndTick) / 2f + 0.5f, 0f, 1f) * actionSpeed;
                temp.Position = casterPos + offset;
            }
            else
            {
                Debug.Log($"tick1111:{target.Index}");
                var targetdata = refData.cdfeTargetData[inData.entity];
                if (revertTick < 0)
                {
                    targetdata.AttackWith = (uint)targetInt;
                    refData.cdfeTargetData[inData.entity] = targetdata;
                    Debug.Log($"tick00000000:{tick},backendtick:{backEndTick}");
                    temp.Position += dir * speed * inData.fDT * actionSpeed;
                }
                else
                {
                    return temp;
      //              targetdata.AttackWith = (uint)0;
      //              refData.cdfeTargetData[inData.entity] = targetdata;
      //              Debug.Log($"tick00000000:{tick},revertTick:{revertTick}");

 					//float3 forward = math.normalizesafe(new float3(MathHelper.RotateVector(dir.xy, 180), 0));
      //              if (refData.storageInfoFromEntity.Exists(caster))
      //              {
						//if(math.length(fadeDir) > math.EPSILON){
						//	forward = fadeDir;
						//}else{
 					//		forward = math.normalizesafe(refData.cdfeLocalTransform[caster].Position - temp.Position);
						//}
                      
      //                  const float fadeLength = 3;
						
      //                   if (math.length(refData.cdfeLocalTransform[caster].Position - temp.Position) <= 3f && math.length(fadeDir) < math.EPSILON )
      //                  {
						//	fadeDir = forward;
						//	fadeTick = tick + 5;
                
      //                  }
						
      //                  if(fadeTick > 0 && tick >= fadeTick){
						// 	refData.ecb.AddComponent(inData.sortkey, inData.entity, new TimeToDieData
      //                      {
      //                          duration = 0
      //                      });
						//}
      //                  //Debug.LogError($"forward {forward} ");
      //              }

      //              temp.Position += forward * trackPar.z / 1000f * inData.fDT * actionSpeed;
      //              temp.Rotation = MathHelper.LookRotation2D(-forward);
                }
            }

          	var radians = math.radians(rotateSpeed);
            radians /= (1f / inData.fDT);
            temp = temp.RotateZ(-radians * actionSpeed);
            return  temp;
        }

        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            BuffHelper.AddTriggerForDeathBullet(ref refData, in inData, inData.sortkey, deadEffectID, caster, dir);
        }

        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            //开始消失时间 单位ms
            startDisappearTime = 50f;

            //消失的持续时间 单位ms
            disappearDuration = 1000f;

            refData.ecb.AddComponent<JiYuColor>(inData.sortkey, inData.entity);
            var temp = refData.cdfeTargetData[inData.entity];
            temp.AttackWith = 0;
            refData.cdfeTargetData[inData.entity] = temp;
			fadeTick = 0;
            Debug.Log($"tick00000000:{tick},duration:{duration}");
            revertTick = -1;
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

            backEndTick = (int)((bullets.trackTypePara2 / 1000f) / inData.fDT);

            if (refData.storageInfoFromEntity.Exists(followedEntity))
            {
                dir = math.normalize(refData.cdfeLocalTransform[followedEntity].Position -
                                     refData.cdfeLocalTransform[inData.entity].Position);
            }

            startPos = refData.cdfeLocalTransform[inData.entity].Position;


            //Debug.Log($"bullets.id {id}  bullets.rotationType {bullets.rotationType}");
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
            //弹幕发出去后才生效
            if (tick > backEndTick + 4)
            {
                BuffHelper.AddTriggerForBullet(ref refData, in inData, inData.sortkey, triggerID, caster, target, dir);
                revertTick = tick+(int)(50/1000*inData.fDT);
            }
        }

        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            if (duration <= 1 && revertTick < 0)
            {
                revertTick = tick + (int)(startDisappearTime / 1000 * inData.fDT); 
            }

            if (tick>=revertTick&&revertTick>0)
            {
                refData.ecb.SetComponent(inData.sortkey, inData.entity, new JiYuColor { value =GetEaseOutAlpha(tick, revertTick,(disappearDuration/1000f)/inData.fDT)});
            }


        }
        /// <summary>
        /// 计算先快后慢的透明度变化
        /// </summary>
        /// <param name="startTime">开始时间（Time.time的起始值）</param>
        /// <param name="duration">持续时间（秒）</param>
        /// <returns>0到1之间的透明度值</returns>
        public float GetEaseOutAlpha(int currentTick, int startTime, float duration)
        {
            // 计算经过的时间比例
            float t = Mathf.Clamp01((currentTick - startTime) / duration);
            // 应用Ease Out Quad公式：1 - (1 - t)^2
            float alpha = (1f - t) * (1f - t);
            return alpha;
        }
        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        public void OnExit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }
    }
}
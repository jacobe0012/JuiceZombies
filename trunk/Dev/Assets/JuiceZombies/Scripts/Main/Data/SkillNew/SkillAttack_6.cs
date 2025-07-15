using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    //分裂
    public partial struct SkillAttack_6 : ISkillAttack
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



        /// <summary>
        /// 分裂弹幕的生成时间 
        /// </summary>
        public int addTime;

        /// <summary>
        /// 分裂弹幕的组号
        /// </summary>
        public int groupIndex;

        //记录的飞行方向
        public float3 dir;

        //记录的释放着行动速度
        public float actionSpeed;


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
            temp.Position += dir * speed * inData.fDT * actionSpeed;
            var radians = math.radians(rotateSpeed);
            radians /= (1f / inData.fDT);
            temp = temp.RotateZ(-radians * actionSpeed);

            return temp;
        }

        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            BuffHelper.AddTriggerForDeathBullet(ref refData, in inData, inData.sortkey, deadEffectID, caster, dir);
        }

        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
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
            actionSpeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON
                ? 1
                : inData.cdfeChaStats[caster].chaResource.actionSpeed;

            dir = math.mul(refData.cdfeLocalTransform[inData.entity].Rotation, MathHelper.picForward);
            dir = defaultDir;
            Debug.Log($"bullets.id {id}  bullets.rotationType {bullets.rotationType}");
            if (bullets.rotationType != 1)
            {
                var tran = refData.cdfeLocalTransform[inData.entity];
                tran.Rotation = quaternion.identity;
                refData.cdfeLocalTransform[inData.entity] = tran;
            }
            //设置弹幕collider
            // if (!refData.storageInfoFromEntity.Exists(inData.entity) ||
            //     !refData.cdfeLocalTransform.HasComponent(inData.entity))
            // {
            //     return;
            // }
        }

        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            UnityEngine.Debug.Log($"OnHit6 {id}");
            if (!refData.storageInfoFromEntity.Exists(target) || !refData.cdfeLocalTransform.HasComponent(target))
            {
                return;
            }

            if (inData.cdfeObstacleTag.HasComponent(target) && isAbsorb)
            {
                UnityEngine.Debug.Log($"66isAbsorb {isAbsorb}");
                hp = 0;
                return;
            }

            if (refData.cdfeBulletSonData.HasComponent(target))
            {
                var bulletSonData = refData.cdfeBulletSonData[target];

                if (!bulletSonData.Equals(caster, skillID, addTime))
                {
                    bulletSonData.addTime = addTime;
                    bulletSonData.skillId = skillID;
                    bulletSonData.caster = caster;
                    refData.cdfeBulletSonData[target] = bulletSonData;
                }
                else
                {
                    return;
                }
            }
            else
            {
                refData.ecb.AddComponent(inData.sortkey, target, new BulletSonData
                {
                    caster = caster,
                    skillId = skillID,
                    addTime = addTime
                });
            }

            hp--;
            ref var tbbullets = ref inData.config.value.Value.configTbbullets.configTbbullets;
            int bulletIndex = 0;
            for (int i = 0; i < tbbullets.Length; i++)
            {
                if (tbbullets[i].id == id)
                {
                    bulletIndex = i;
                    break;
                }
            }

            ref var bullet = ref tbbullets[bulletIndex];
            int sonBulletId = (int)trackPar.x;
            int sonBulletNum = (int)trackPar.y;
            float sonBulletRange = trackPar.z / 1000f;

            Debug.Log($"OnHit_6 skillId{skillID} addTime{addTime} sonBulletId{sonBulletId}");
            BuffHelper.AddTriggerForBulletCanFire(ref refData, in inData, inData.sortkey, triggerID, caster, target,
                sonBulletId, skillID, addTime, sonBulletNum, sonBulletRange);
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
    }
}
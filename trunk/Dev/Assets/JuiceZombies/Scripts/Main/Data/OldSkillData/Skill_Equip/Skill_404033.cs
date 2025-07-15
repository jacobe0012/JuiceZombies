//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;
//using UnityEngine.UIElements;


//namespace Main
//{
//    /// <summary>
//    /// 每杀敌100扔出一枚炸弹 
//    /// </summary>
//    public partial struct Skill_404033 : ISkillOld
//    {
//        ///<summary>0
//        ///技能的id
//        ///</summary>
//        public int id;

//        ///<summary>1
//        ///技能等级
//        ///</summary>
//        public int level;

//        ///<summary>2
//        ///冷却时间，单位秒。尽管游戏设计里面是没有冷却时间的，但是我们依然需要这个数据
//        ///因为作为一个ARPG子分类，和ARPG游戏有一样的问题：一次按键（时间够久）会发生连续多次使用技能，所以得有一个GCD来避免问题
//        ///当然和wow的gcd不同，这个“GCD”就只会让当前使用的技能进入0.1秒的冷却
//        ///</summary>
//        public float cooldown;


//        ///<summary>3
//        ///持续时间，单位：秒
//        ///</summary>
//        public float duration;


//        ///<summary>4
//        ///倍速，1=100%，0.1=10%是最小值
//        ///</summary>
//        public float timeScale;

//        ///<summary>5
//        ///Timeline的焦点对象也就是创建timeline的负责人，比如技能产生的timeline，就是技能的施法者
//        ///</summary>
//        public Entity caster;

//        ///<summary>6
//        ///技能已经运行了多少帧了 无需赋值
//        ///</summary>
//        public int tick;

//        /// <summary>7
//        /// 技能当前冷却时间 无需赋值
//        /// </summary>
//        public float curCooldown;

//        ///<summary>8
//        ///剩余时间，单位：秒
//        ///</summary>
//        public float curDuration;

//        ///<summary>9
//        ///距离这个技能最近的目标
//        ///</summary>
//        public Entity target;

//        /// <summary>
//        /// 是否是充能技能 10
//        /// </summary>


//        /// <summary>
//        /// 充能时间 毫秒 11 （充能次数充能也转换成时间进行取模）
//        /// </summary>


//        /// <summary>
//        /// 技能实体的执行时间 用于充能技能 12
//        /// </summary>


//        ///<summary>13
//        ///技能从创建到现在总的tick
//        ///</summary>
//        public int totalTick;
//                ///<summary>14
//        ///是否时一次性释放技能
//        ///</summary>
//        public bool isOneShotSkill;
//        ///<summary>15
//        ///是否是确定位置坐标
//        ///</summary>
//        public bool isUseCertainPos;

//        ///<summary>16
//        ///一次性释放技能的坐标
//        ///</summary>
//        public float3 pos;
//        public LocalTransform curLoc;

//        private int scale;
//        private int pushRate;
//        private int speed;
//        private int offset;
//        private int currentSkillEffectID;

//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {

//            //if (!BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 404033, inData.globalConfig, out int needEffectID)) return;
//            //currentSkillEffectID = needEffectID;
//            //if (!BuffHelper.TryGetParmeter(inData.globalConfig, needEffectID, 40000003)) return;
//            //var buff = BuffHelper.UpdateParmeter(inData.globalConfig, needEffectID, 40000003);
//            //scale = buff[2].x;
//            //pushRate= buff[3].x;
//            //if (!BuffHelper.TryGetParmeter(inData.globalConfig, needEffectID, 50000010)) return;
//            //var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, needEffectID, 50000010);
//            //speed = buff1[1].x;
//            //offset = buff1[2].x;

//            //if(!BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 404034, inData.globalConfig, out needEffectID))
//            //{
//            //    if (!BuffHelper.TryGetParmeter(inData.globalConfig, needEffectID, 20003017)) return;
//            //    var buff2 = BuffHelper.UpdateParmeter(inData.globalConfig, needEffectID, 20003017);

//            //}

//            //if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 402034, inData.globalConfig, out needEffectID))
//            //{
//            //    duration = 3;
//            //}
//            //if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 402036, inData.globalConfig, out needEffectID))
//            //{
//            //    cooldown = 30f;
//            //}
//            ////refData.ecb.AppendToBuffer(inData.sortkey, inData.entity, new Buff_20002013 { carrier = inData.entity, duration = duration, id = 20002013, permanent = false,tickTime=0,buffArgs=new BuffArgs { args0=needEffectID} }.ToBuffOld());


//        }


//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //throw new NotImplementedException();
//        }


//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            ////每杀敌100生成炸弹
//            ////    Debug.Log($"continuousCollCount{inData.cdfeChaStats[inData.entity].chaResource.continuousCollCount}");
//            //if (inData.cdfePlayerData[inData.entity].playerData.killEnemy % 100==0)
//            //{
//            //    GenerateStunGrenade(ref refData, inData,currentSkillEffectID );
//            //}


//        }
//        private void GenerateStunGrenade(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData, int currentSkillEffectID)
//        {
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 40000028)) return;
//            var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 40000028);
//            float scale = buff1[1].x;
//            var scalePropRate = buff1[1].z;
//            scale *= (1 + scalePropRate / 10000f);


//            var buff3 = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 50000010);
//            var throwSpeed = buff3[1].x;
//            var offset = buff3[2].x;

//            //BuffHelper.SetSkillAttackTarget(inData.globalConfig, currentSkillEffectID,);

//            if (!IsAttackToEnemy((int)scale, inData)) return;

//            refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff_20003017 { id = 20003017, carrier = target,duration=1,canBeStacked=true,permanent=true,caster=default,tickTime=0 }.ToBuffOld());

//            var targetPos = GetRandOffsetPos(ref refData, inData, offset, target);
//            var flyDis = math.distance(targetPos, inData.cdfeLocalTransform[inData.entity].Position);
//            var flyDuration = flyDis / throwSpeed * 100f;
//            //实例化skill1
//            var skillPrefab = inData.prefabMapData.prefabHashMap["skill_stungrenade"];
//            var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);

//            var newpos = new LocalTransform
//            {
//                Position = GetRandOffsetPos(ref refData, inData, offset, target),
//                //TODO:大小*10
//                Scale = inData.cdfeLocalTransform[skillPrefab].Scale * scale,
//                Rotation = inData.cdfeLocalTransform[skillPrefab].Rotation,
//            };

//            // Debug.Log($"position{inData.cdfeLocalTransform[inData.entity].Position}");
//            refData.ecb.SetComponent(inData.sortkey, skillEntity, newpos);

//            refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//            {
//                data = new SkillThrowBoomAttack
//                {
//                    duration = flyDuration,
//                    tick = 0,
//                    caster = inData.entity,
//                    hp = 0,
//                    isBullet = true,
//                    stayAttack = false,
//                    curStayAttackInterval = 0,
//                    stayAttackInterval = 0,
//                    skillDelay = 0,
//                    id = currentSkillEffectID,
//                    speed = throwSpeed
//                }.ToSkillAttack()
//            });
//        }

//        public bool IsAttackToEnemy(int limitDistance, TimeLineData_ReadOnly inData)
//        {
//            //判断是否对怪
//            float distance = 999f;
//            target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//                inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                inData.cdfeObstacleTag,
//                inData.entity, TargetAttackType.Enemy, ref distance);
//            if (distance > limitDistance)
//            {
//                target = default;
//            }

//            return target != default ? true : false;
//        }

//        private float3 GetRandOffsetPos(ref TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData, int offset, Entity target)
//        {
//            var targetPos = inData.cdfeLocalTransform[target].Position;

//            var random = refData.rand.rand;

//            // 在圆内生成随机半径和角度
//            float randomRadius = (float)random.NextFloat() * offset;
//            float randomAngle = (float)random.NextFloat() * math.PI * 2;

//            // 将极坐标转换为笛卡尔坐标
//            float xOffset = randomRadius * math.cos(randomAngle);
//            float yOffset = randomRadius * math.sin(randomAngle);

//            // 在中心点上偏移生成的随机坐标
//            float3 randomPoint = new float3(targetPos.x + xOffset, targetPos.y + yOffset, 0);

//            return randomPoint;
//        }
//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }
//    }
//}

